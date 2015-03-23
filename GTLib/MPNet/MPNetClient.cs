namespace com.gt.mpnet
{
    using com.gt.mpnet.bitswarm;
    using com.gt.mpnet.core;
    using com.gt.entities;
    using com.gt.units;
    using com.gt.mpnet.requests;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Timers;
    using com.gt.events;
    using com.gt.mpnet.controllers;

    /// <summary>
    /// 
    /// </summary>
    public class MPNetClient : IDispatchable
    {
        private BitSwarmClient bitSwarm;
        private EventDispatcher dispatcher;
        private object eventsLocker;
        private Queue<BaseEvent> eventsQueue;
        private bool inited;
        private bool isConnecting;
        private LagMonitor lagMonitor;
        private MPUser mySelf;
        private string sessionToken;
        private Logger log;
        private MPNetManager m_NetManger;
        private int m_Id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netManger"></param>
        public MPNetClient()
            : this(0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="netManger"></param>
        /// <param name="debug"></param>
        public MPNetClient(int id)
        {
            m_Id = id;
            m_NetManger = (MPNetManager)GTLib.NetManager;
            inited = false;
            isConnecting = false;
            eventsLocker = new object();
            eventsQueue = new Queue<BaseEvent>();
            log = new Logger(typeof(MPNetClient));
            Initialize();
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
        public void RemoveAllEventListeners()
        {
            dispatcher.RemoveAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void RemoveEventListener(string eventType, EventListenerDelegate listener)
        {
            dispatcher.RemoveEventListener(eventType, listener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Connect(string host, int port)
        {
            Log.Info("host:" + host + "port:" + port);
            if (IsConnected)
            {
                Log.Warn("Already connected");
            }
            else if (isConnecting)
            {
                Log.Warn("A connection attempt is already in progress");
            }
            else
            {
                if (host == null || host.Length == 0)
                {
                    throw new ArgumentException("Invalid connection host/address");
                }
                if ((port < 0) || (port > 0xffff))
                {
                    throw new ArgumentException("Invalid connection port");
                }
                try
                {
                    IPAddress.Parse(host);
                }
                catch (FormatException)
                {
                    try
                    {
                        host = Dns.GetHostEntry(host).AddressList[0].ToString();
                    }
                    catch (Exception exception)
                    {
                        string str = "Failed to lookup hostname " + host + ". Connection failed. Reason " + exception.Message;
                        log.Error(str);
                        Hashtable data = new Hashtable();
                        data["success"] = false;
                        data["errorMessage"] = str;
                        DispatchEvent(new MPEvent(MPEvent.CONNECTION, data));
                        return;
                    }
                }
                isConnecting = true;
                bitSwarm.Connect(host, port);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                if (bitSwarm.ReconnectionSeconds > 0)
                {
                    Send(new ManualDisconnectionRequest());
                }
                bitSwarm.Disconnect(ClientDisconnectionReason.MANUAL);
            }
            else
            {
                Log.Info("You are not connected");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        internal void DispatchEvent(BaseEvent evt)
        {
            lock (eventsLocker)
            {
                eventsQueue.Enqueue(evt);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableLagMonitor(bool enabled)
        {
            EnableLagMonitor(enabled, 4, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="interval"></param>
        public void EnableLagMonitor(bool enabled, int interval)
        {
            EnableLagMonitor(enabled, interval, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="interval"></param>
        /// <param name="queueSize"></param>
        public void EnableLagMonitor(bool enabled, int interval, int queueSize)
        {
            if (enabled)
            {
                lagMonitor = new LagMonitor(this, interval, queueSize);
                lagMonitor.Start();
            }
            else
            {
                lagMonitor.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetReconnectionSeconds()
        {
            return bitSwarm.ReconnectionSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal BitSwarmClient GetSocketEngine()
        {
            return bitSwarm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        internal void HandleClientDisconnection(string reason)
        {
            Hashtable data = new Hashtable();
            data.Add("reason", reason);
            DispatchEvent(new MPEvent(MPEvent.CONNECTION_LOST, data));
            bitSwarm.ReconnectionSeconds = 0;
            bitSwarm.Disconnect(reason);
            Reset();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void HandleConnectionProblem(BaseEvent e, bool success)
        {
            BitSwarmEvent event2 = e as BitSwarmEvent;
            Hashtable data = new Hashtable();
            data["success"] = success;
            data["errorMessage"] = event2.Params["message"];
            DispatchEvent(new MPEvent(MPEvent.CONNECTION, data));
            isConnecting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public void HandleHandShake(IMPObject parameters)
        {
            IMPObject obj2 = parameters;
            if (obj2.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                sessionToken = obj2.GetUtfString(HandshakeRequest.KEY_SESSION_TOKEN);
                bitSwarm.CompressionThreshold = obj2.GetInt(HandshakeRequest.KEY_COMPRESSION_THRESHOLD);
                bitSwarm.MaxMessageSize = obj2.GetInt(HandshakeRequest.KEY_MAX_MESSAGE_SIZE);
                if (Debug)
                {
                    log.Debug(string.Format("Handshake response: tk => {0}, ct => {1}", sessionToken, bitSwarm.CompressionThreshold));
                }
                if (bitSwarm.IsReconnecting)
                {
                    bitSwarm.IsReconnecting = false;
                    DispatchEvent(new MPEvent(MPEvent.CONNECTION_RESUME));
                }
                else
                {
                    isConnecting = false;
                    Hashtable data = new Hashtable();
                    data["success"] = true;
                    DispatchEvent(new MPEvent(MPEvent.CONNECTION, data));
                }
            }
            else
            {
                short errorCode = obj2.GetShort(BaseRequest.KEY_ERROR_CODE);

                Hashtable hashtable2 = new Hashtable();
                hashtable2["success"] = false;
                hashtable2["errorCode"] = errorCode;
                DispatchEvent(new MPEvent(MPEvent.CONNECTION, hashtable2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        internal void HandleExtension(string cmd, IMPObject parameters)
        {
            m_NetManger.HandleExtension(this, cmd, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleLogout()
        {
            if ((lagMonitor != null) && lagMonitor.IsRunning)
            {
                lagMonitor.Stop();
            }
            mySelf = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            if (!inited)
            {
                if (dispatcher == null)
                {
                    dispatcher = new EventDispatcher(this);
                }
                bitSwarm = new BitSwarmClient(this);
                bitSwarm.IoHandler = new MPIOHandler(bitSwarm);
                bitSwarm.Init();
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.CONNECT, new EventListenerDelegate(OnSocketConnect));
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.DISCONNECT, new EventListenerDelegate(OnSocketClose));
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.RECONNECTION_TRY, new EventListenerDelegate(OnSocketReconnectionTry));
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.IO_ERROR, new EventListenerDelegate(OnSocketIOError));
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.SECURITY_ERROR, new EventListenerDelegate(OnSocketSecurityError));
                bitSwarm.Dispatcher.AddEventListener(BitSwarmEvent.DATA_ERROR, new EventListenerDelegate(OnSocketDataError));
                inited = true;
                Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitUDP()
        {
            InitUDP(null, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udpHost"></param>
        public void InitUDP(string udpHost)
        {
            InitUDP(udpHost, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udpHost"></param>
        /// <param name="udpPort"></param>
        public void InitUDP(string udpHost, int udpPort)
        {
            if (!IsConnected)
            {
                Log.Warn("Cannot initialize UDP protocol until the client is connected to SFS2X.");
            }
            else
            {
                if ((udpHost == null) || (udpHost.Length == 0))
                {
                    throw new ArgumentException("Invalid UDP host/address");
                }
                if (udpPort < 0 || udpPort > 65535)
                {
                    throw new ArgumentException("Invalid UDP port range");
                }
                try
                {
                    IPAddress.Parse(udpHost);
                }
                catch (FormatException)
                {
                    try
                    {
                        udpHost = Dns.GetHostEntry(udpHost).AddressList[0].ToString();
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Failed to lookup hostname ", udpHost, ". UDP init failed. Reason ", exception.Message);
                        Hashtable data = new Hashtable();
                        data["success"] = false;
                        DispatchEvent(new MPEvent(MPEvent.UDP_INIT, data));
                        return;
                    }
                }
                if ((bitSwarm.UdpManager == null) || !bitSwarm.UdpManager.Inited)
                {
                    IUDPManager manager = new UDPManager(this);
                    bitSwarm.UdpManager = manager;
                }
                try
                {
                    bitSwarm.UdpManager.Initialize(udpHost, udpPort);
                }
                catch (Exception exception2)
                {
                    Log.Error("Exception initializing UDP: ", exception2.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void KillConnection()
        {
            bitSwarm.KillConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketClose(BaseEvent e)
        {
            BitSwarmEvent event2 = e as BitSwarmEvent;
            Reset();
            Hashtable data = new Hashtable();
            data["reason"] = event2.Params["reason"];
            DispatchEvent(new MPEvent(MPEvent.CONNECTION_LOST, data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketConnect(BaseEvent e)
        {
            BitSwarmEvent event2 = e as BitSwarmEvent;
            if ((bool)event2.Params["success"])
            {
                SendHandshakeRequest((bool)event2.Params["isReconnection"]);
            }
            else
            {
                Log.Warn("Connection attempt failed");
                HandleConnectionProblem(event2, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketDataError(BaseEvent e)
        {
            Hashtable data = new Hashtable();
            data["errorMessage"] = e.Params["message"];
            DispatchEvent(new MPEvent(MPEvent.SOCKET_ERROR, data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketIOError(BaseEvent e)
        {
            BitSwarmEvent event2 = e as BitSwarmEvent;
            if (isConnecting)
            {
                HandleConnectionProblem(event2, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketReconnectionTry(BaseEvent e)
        {
            DispatchEvent(new MPEvent(MPEvent.CONNECTION_RETRY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnSocketSecurityError(BaseEvent e)
        {
            BitSwarmEvent event2 = e as BitSwarmEvent;
            if (isConnecting)
            {
                HandleConnectionProblem(event2, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ProcessEvents()
        {
            BaseEvent[] eventArray;
            lock (eventsLocker)
            {
                eventArray = eventsQueue.ToArray();
                eventsQueue.Clear();
            }
            foreach (BaseEvent event2 in eventArray)
            {
                if (!m_NetManger.CacheQueueProcess)
                {
                    m_NetManger.PostRunnable(delegate
                    {
                        Dispatcher.DispatchEvent(event2);
                    });
                }
                else
                {
                    Dispatcher.DispatchEvent(event2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Reset()
        {
            if (lagMonitor != null)
            {
                lagMonitor.Destroy();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal void Send(IRequest request)
        {
            if (!IsConnected)
            {
                log.Warn("You are not connected. Request cannot be sent: ", request.ToString());
            }
            else
            {
                try
                {
                    request.Validate(this);
                    request.Execute(this);
                    bitSwarm.Send(request.Message);
                }
                catch (Exception error)
                {
                    string message = error.Message;
                    log.Warn(message);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public void SetReconnectionSeconds(int seconds)
        {
            bitSwarm.ReconnectionSeconds = seconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isReconnection"></param>
        private void SendHandshakeRequest(bool isReconnection)
        {
            IRequest request = new HandshakeRequest(Version, !isReconnection ? null : sessionToken);
            Send(request);
        }

        /// <summary>
        /// The id
        /// </summary>
        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// The my self user
        /// </summary>
        public MPUser MySelf
        {
            get
            {
                return mySelf;
            }
            set
            {
                mySelf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SessionToken
        {
            get
            {
                return sessionToken;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CompressionThreshold
        {
            get
            {
                return bitSwarm.CompressionThreshold;
            }
        }

        /// <summary>
        /// The curren ip
        /// </summary>
        public string CurrentIp
        {
            get
            {
                return bitSwarm.ConnectionIp;
            }
        }

        /// <summary>
        /// The curren port
        /// </summary>
        public int CurrentPort
        {
            get
            {
                return bitSwarm.ConnectionPort;
            }
        }

        /// <summary>
        /// The debug
        /// </summary>
        public bool Debug
        {
            get
            {
                return (Logger.LoggingLevel & LogLevel.DEBUG) != 0;
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
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool connected = false;
                if (bitSwarm != null)
                {
                    connected = bitSwarm.Connected;
                }
                return connected;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnecting
        {
            get
            {
                return isConnecting;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LagMonitor LagMonitor
        {
            get
            {
                return lagMonitor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Logger Log
        {
            get
            {
                return log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxMessageSize
        {
            get
            {
                return bitSwarm.MaxMessageSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UdpAvailable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UdpInited
        {
            get
            {
                return ((bitSwarm.UdpManager != null) && bitSwarm.UdpManager.Inited);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Version
        {
            get
            {
                return string.Concat("1.0.0");
            }
        }
    }
}

