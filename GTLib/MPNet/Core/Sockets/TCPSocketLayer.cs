namespace com.gt.mpnet.core.sockets
{
    using com.gt.mpnet.bitswarm;
    using com.gt.units;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class TCPSocketLayer : ISocketLayer
    {
        private byte[] byteBuffer = new byte[READ_BUFFER_SIZE];
        private TcpClient connection;
        private FiniteStateMachine fsm;
        private IPAddress ipAddress;
        private bool isDisconnecting = false;
        private Logger log;
        private NetworkStream networkStream;
        private ConnectionDelegate onConnect;
        private OnDataDelegate onData = null;
        private ConnectionDelegate onDisconnect;
        private OnErrorDelegate onError = null;
        private static readonly int READ_BUFFER_SIZE = 0x1000;
        private int socketNumber;
        private int socketPollSleep;
        private Thread thrConnect;
        private Thread thrSocketReader;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        public TCPSocketLayer(BitSwarmClient bs)
        {
            //this.bitSwarm = bs;
            if (bs != null)
            {
                this.log = bs.Log;
            }
            this.InitStates();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CallOnConnect()
        {
            if (this.onConnect != null)
            {
                this.onConnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void CallOnData(byte[] data)
        {
            if (this.onData != null)
            {
                this.onData(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CallOnDisconnect()
        {
            if (this.onDisconnect != null)
            {
                this.onDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="se"></param>
        private void CallOnError(string msg, SocketError se)
        {
            if (this.onError != null)
            {
                this.onError(msg, se);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adr"></param>
        /// <param name="port"></param>
        public void Connect(IPAddress adr, int port)
        {
            if (this.State != States.Disconnected)
            {
                this.LogWarn("Calling connect when the socket is not disconnected");
            }
            else
            {
                this.socketNumber = port;
                this.ipAddress = adr;
                this.fsm.ApplyTransition(Transitions.StartConnect);
                this.thrConnect = new Thread(new ThreadStart(this.ConnectThread));
                this.thrConnect.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectThread()
        {
            try
            {
                this.connection = new TcpClient();
                this.connection.Client.Connect(this.ipAddress, this.socketNumber);
                this.networkStream = this.connection.GetStream();
                this.fsm.ApplyTransition(Transitions.ConnectionSuccess);
                this.CallOnConnect();
                this.thrSocketReader = new Thread(new ThreadStart(this.Read));
                this.thrSocketReader.Start();
            }
            catch (SocketException exception)
            {
                string err = "Connection error: " + exception.Message + " " + exception.StackTrace;
                this.HandleError(err, exception.SocketErrorCode);
            }
            catch (Exception exception2)
            {
                string str2 = "General exception on connection: " + exception2.Message + " " + exception2.StackTrace;
                this.HandleError(str2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            if (this.State != States.Connected)
            {
                this.LogWarn("Calling disconnect when the socket is not connected");
            }
            else
            {
                this.isDisconnecting = true;
                try
                {
                    this.connection.Client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                    this.LogWarn("Trying to disconnect a non-connected tcp socket");
                }
                this.HandleDisconnection();
                this.isDisconnecting = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        private void HandleBinaryData(byte[] buf, int size)
        {
            byte[] dst = new byte[size];
            Buffer.BlockCopy(buf, 0, dst, 0, size);
            this.CallOnData(dst);
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleDisconnection()
        {
            if (this.State != States.Disconnected)
            {
                this.fsm.ApplyTransition(Transitions.Disconnect);
                this.CallOnDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        private void HandleError(string err)
        {
            this.HandleError(err, SocketError.NotSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        /// <param name="se"></param>
        private void HandleError(string err, SocketError se)
        {
            this.fsm.ApplyTransition(Transitions.ConnectionFailure);
            if (!this.isDisconnecting)
            {
                LogError(err);
                this.CallOnError(err, se);
            }
            this.HandleDisconnection();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitStates()
        {
            this.fsm = new FiniteStateMachine();
            this.fsm.AddAllStates(typeof(States));
            this.fsm.AddStateTransition(States.Disconnected, States.Connecting, Transitions.StartConnect);
            this.fsm.AddStateTransition(States.Connecting, States.Connected, Transitions.ConnectionSuccess);
            this.fsm.AddStateTransition(States.Connecting, States.Disconnected, Transitions.ConnectionFailure);
            this.fsm.AddStateTransition(States.Connected, States.Disconnected, Transitions.Disconnect);
            this.fsm.SetCurrentState(States.Disconnected);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Kill()
        {
            this.connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void LogError(string msg)
        {
            if (log != null)
            {
                log.Error("TCPSocketLayer: ",msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void LogWarn(string msg)
        {
            if (log != null)
            {
                log.Warn("TCPSocketLayer: ", msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Read()
        {
            int size = 0;
            while (true)
            {
                try
                {
                    if (this.State != States.Connected)
                    {
                        break;
                    }
                    if (this.socketPollSleep > 0)
                    {
                        Thread.Sleep(this.socketPollSleep);
                    }
                    size = this.networkStream.Read(this.byteBuffer, 0, READ_BUFFER_SIZE);
                    if (size < 1)
                    {
                        this.HandleError("Connection closed by the remote side");
                        break;
                    }
                    this.HandleBinaryData(this.byteBuffer, size);
                }
                catch (SocketException exception)
                {
                    this.HandleError("Error reading data from socket: " + exception.Message, exception.SocketErrorCode);
                }
                catch (Exception exception2)
                {
                    this.HandleError("General error reading data from socket: " + exception2.Message + "\n" + exception2.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            this.WriteSocket(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf"></param>
        private void WriteSocket(byte[] buf)
        {
            if (this.State != States.Connected)
            {
                this.LogError("Trying to write to disconnected socket");
            }
            else
            {
                try
                {
                    this.networkStream.Write(buf, 0, buf.Length);
                }
                catch (SocketException exception)
                {
                    string err = "Error writing to socket: " + exception.Message;
                    this.HandleError(err, exception.SocketErrorCode);
                }
                catch (Exception exception2)
                {
                    string str2 = "General error writing to socket: " + exception2.Message + " " + exception2.StackTrace;
                    this.HandleError(str2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return (State == States.Connected);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionDelegate OnConnect
        {
            get
            {
                return onConnect;
            }
            set
            {
                onConnect = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OnDataDelegate OnData
        {
            get
            {
                return onData;
            }
            set
            {
                onData = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionDelegate OnDisconnect
        {
            get
            {
                return onDisconnect;
            }
            set
            {
                onDisconnect = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OnErrorDelegate OnError
        {
            get
            {
                return onError;
            }
            set
            {
                onError = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequiresConnection
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SocketPollSleep
        {
            get
            {
                return this.socketPollSleep;
            }
            set
            {
                this.socketPollSleep = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public States State
        {
            get
            {
                return (States) this.fsm.GetCurrentState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum States
        {
            Disconnected,
            Connecting,
            Connected
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Transitions
        {
            StartConnect,
            ConnectionSuccess,
            ConnectionFailure,
            Disconnect
        }
    }
}

