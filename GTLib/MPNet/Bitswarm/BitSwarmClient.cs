using com.gt.events;

namespace com.gt.mpnet.bitswarm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;
    using com.gt.mpnet;
    using com.gt.mpnet.controllers;
    using com.gt.mpnet.core;
    using com.gt.mpnet.core.sockets;
    using com.gt.units;

    /// <summary>
    /// 
    /// </summary>
    public class BitSwarmClient : IDispatchable
    {
        private bool attemptingReconnection;
        private int compressionThreshold;
        private Dictionary<int, IController> controllers;
        private bool controllersInited;
        private EventDispatcher dispatcher;
        private IoHandler ioHandler;
        private string lastIpAddress;
        private int lastTcpPort;
        private Logger log;
        private bool manualDisconnection;
        private int maxMessageSize;
        private int reconnectionSeconds;
        private Timer retryTimer;
        private MPNetClient mpnet;
        private ISocketLayer socket;
        private IUDPManager udpManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sfs"></param>
        public BitSwarmClient(MPNetClient mpnet)
        {
            controllers = new Dictionary<int, IController>();
            compressionThreshold = 0x1e8480;
            maxMessageSize = 0x2710;
            reconnectionSeconds = 0;
            attemptingReconnection = false;
            manualDisconnection = false;
            retryTimer = null;
            this.mpnet = mpnet;
            log = mpnet.Log;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="controller"></param>
        private void AddController(int id, IController controller)
        {
            if (controller == null)
            {
                throw new ArgumentException("Controller is null, it can't be added.");
            }
            if (controllers.ContainsKey(id))
            {
                throw new ArgumentException(string.Concat("A controller with id: ", id, " already exists! Controller can't be added: ", controller));
            }

            controllers[id] = controller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void AddEventListener(string eventType, EventListenerDelegate listener)
        {
            dispatcher.AddEventListener(eventType, listener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            lastIpAddress = ip;
            lastTcpPort = port;
            socket.Connect(IPAddress.Parse(this.lastIpAddress), this.lastTcpPort);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            socket.OnConnect = (ConnectionDelegate)Delegate.Remove(this.socket.OnConnect, new ConnectionDelegate(this.OnSocketConnect));
            socket.OnDisconnect = (ConnectionDelegate)Delegate.Remove(this.socket.OnDisconnect, new ConnectionDelegate(this.OnSocketClose));
            socket.OnData = (OnDataDelegate)Delegate.Remove(this.socket.OnData, new OnDataDelegate(this.OnSocketData));
            socket.OnError = (OnErrorDelegate)Delegate.Remove(this.socket.OnError, new OnErrorDelegate(this.OnSocketError));
            if (socket.IsConnected)
            {
                socket.Disconnect();
            }
            socket = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            this.Disconnect(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        public void Disconnect(string reason)
        {
            if (reason == ClientDisconnectionReason.MANUAL)
            {
                manualDisconnection = true;
            }

            socket.Disconnect();
            if (udpManager != null)
            {
                udpManager.Disconnect();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        private void DispatchEvent(BitSwarmEvent evt)
        {
            dispatcher.DispatchEvent(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IController GetController(int id)
        {
            return controllers[id];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if (dispatcher == null)
            {
                dispatcher = new EventDispatcher(this);
            }
            if (!controllersInited)
            {
                InitControllers();
                controllersInited = true;
            }
            if (socket == null)
            {
                socket = new TCPSocketLayer(this);
                socket.OnConnect = (ConnectionDelegate)Delegate.Combine(socket.OnConnect, new ConnectionDelegate(OnSocketConnect));
                socket.OnDisconnect = (ConnectionDelegate)Delegate.Combine(socket.OnDisconnect, new ConnectionDelegate(OnSocketClose));
                socket.OnData = (OnDataDelegate)Delegate.Combine(socket.OnData, new OnDataDelegate(OnSocketData));
                socket.OnError = (OnErrorDelegate)Delegate.Combine(socket.OnError, new OnErrorDelegate(OnSocketError));
            }
        }

        private void InitControllers()
        {
            SystemController sysController = new SystemController(this);
            ExtensionController extController = new ExtensionController(this);
            AddController(0, sysController);
            AddController(1, extController);
        }

        /// <summary>
        /// 
        /// </summary>
        public void KillConnection()
        {
            socket.Kill();
            OnSocketClose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long NextUdpPacketId()
        {
            return udpManager.NextUdpPacketId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRetryConnectionEvent(object source, ElapsedEventArgs e)
        {
            retryTimer.Enabled = false;
            socket.Connect(IPAddress.Parse(lastIpAddress), lastTcpPort);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSocketClose()
        {
            bool flag = mpnet == null || !attemptingReconnection ;
            bool md = manualDisconnection;
            manualDisconnection = false;
            if ((attemptingReconnection || flag) || md)
            {
                if (udpManager != null)
                {
                    udpManager.Reset();
                }
                if (md)
                {
                    Hashtable arguments = new Hashtable();
                    arguments["reason"] = ClientDisconnectionReason.MANUAL;
                    DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, arguments));
                }
            }
            else
            {
                log.Debug("Attempting reconnection in " + this.ReconnectionSeconds + " sec");
                attemptingReconnection = true;
                DispatchEvent(new BitSwarmEvent(BitSwarmEvent.RECONNECTION_TRY));
                RetryConnection(ReconnectionSeconds * 0x3e8);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSocketConnect()
        {
            BitSwarmEvent evt = new BitSwarmEvent(BitSwarmEvent.CONNECT);
            Hashtable hashtable = new Hashtable();
            hashtable["success"] = true;
            hashtable["isReconnection"] = attemptingReconnection;
            evt.Params = hashtable;
            this.DispatchEvent(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSocketData(byte[] data)
        {
            try
            {
                ByteArray buffer = new ByteArray(data);
                this.ioHandler.OnDataRead(buffer);
            }
            catch (Exception exception)
            {
                log.Error("## SocketDataError: " + exception.Message);
                BitSwarmEvent evt = new BitSwarmEvent(BitSwarmEvent.DATA_ERROR);
                Hashtable hashtable = new Hashtable();
                hashtable["message"] = exception.ToString();
                evt.Params = hashtable;
                this.DispatchEvent(evt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="se"></param>
        private void OnSocketError(string message, SocketError se)
        {
            BitSwarmEvent evt = null;
            Hashtable hashtable = new Hashtable();
            hashtable["message"] = message + ((se == SocketError.NotSocket) ? "" : (" [" + se.ToString() + "]"));
            if (se != SocketError.AccessDenied)
            {
                if (!attemptingReconnection && !mpnet.IsConnecting)
                {
                    Hashtable arguments = new Hashtable();
                    arguments["reason"] = ClientDisconnectionReason.UNKNOWN;
                    DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, arguments));
                }
                evt = new BitSwarmEvent(BitSwarmEvent.IO_ERROR)
                {
                    Params = hashtable
                };
            }
            else
            {
                evt = new BitSwarmEvent(BitSwarmEvent.SECURITY_ERROR)
                {
                    Params = hashtable
                };
            }
            DispatchEvent(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        private void RetryConnection(int timeout)
        {
            if (retryTimer == null)
            {
                retryTimer = new Timer((double)timeout);
            }
            retryTimer.AutoReset = false;
            retryTimer.Elapsed += new ElapsedEventHandler(OnRetryConnectionEvent);
            retryTimer.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Send(IMessage message)
        {
            ioHandler.Codec.OnPacketWrite(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompressionThreshold
        {
            get
            {
                return compressionThreshold;
            }
            set
            {
                if (value <= 100)
                {
                    throw new ArgumentException("Compression threshold cannot be < 100 bytes.");
                }
                compressionThreshold = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Connected
        {
            get
            {
                if (socket == null)
                {
                    return false;
                }
                return socket.IsConnected;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionIp
        {
            get
            {
                if (!Connected)
                {
                    return "Not Connected";
                }
                return lastIpAddress;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionPort
        {
            get
            {
                if (!Connected)
                {
                    return -1;
                }
                return lastTcpPort;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Debug
        {
            get
            {
                return (mpnet == null || mpnet.Debug);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventDispatcher Dispatcher
        {
            get
            {
                return dispatcher;
            }
            set
            {
                dispatcher = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IoHandler IoHandler
        {
            get
            {
                return ioHandler;
            }
            set
            {
                if (ioHandler != null)
                {
                    throw new Exception("IOHandler is already set!");
                }
                ioHandler = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReconnecting
        {
            get
            {
                return attemptingReconnection;
            }
            set
            {
                attemptingReconnection = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Logger Log
        {
            get
            {
                if (mpnet == null)
                {
                    return new Logger(typeof(BitSwarmClient));
                }
                return mpnet.Log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxMessageSize
        {
            get
            {
                return maxMessageSize;
            }
            set
            {
                maxMessageSize = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ReconnectionSeconds
        {
            get
            {
                if (reconnectionSeconds < 0)
                {
                    return 0;
                }
                return reconnectionSeconds;
            }
            set
            {
                reconnectionSeconds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MPNetClient MPNet
        {
            get
            {
                return mpnet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ISocketLayer Socket
        {
            get
            {
                return socket;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IUDPManager UdpManager
        {
            get
            {
                return udpManager;
            }
            set
            {
                udpManager = value;
            }
        }
    }
}

