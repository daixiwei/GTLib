namespace com.gt.mpnet.core.sockets
{
    using com.gt.mpnet;
    using gt.units;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class UDPSocketLayer : ISocketLayer
    {
        private byte[] byteBuffer;
        private bool connected = false;
        private UdpClient connection;
        private IPAddress ipAddress;
        private bool isDisconnecting = false;
        private Logger log;
        private OnDataDelegate onData = null;
        private OnErrorDelegate onError = null;
        private IPEndPoint sender;
        //private NetConnecter sfs;
        private int socketNumber;
        private int socketPollSleep;
        private Thread thrSocketReader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sfs"></param>
        public UDPSocketLayer(MPNetClient sfs)
        {
            //this.sfs = sfs;
            if (sfs != null)
            {
                this.log = sfs.Log;
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
            this.socketNumber = port;
            this.ipAddress = adr;
            try
            {
                connection = new UdpClient(ipAddress.ToString(), socketNumber);
                sender = new IPEndPoint(IPAddress.Any, 0);
                thrSocketReader = new Thread(new ThreadStart(Read));
                thrSocketReader.Start();
            }
            catch (SocketException exception)
            {
                string err = "Connection error: " + exception.Message + " " + exception.StackTrace;
                HandleError(err, exception.SocketErrorCode);
            }
            catch (Exception exception2)
            {
                string str2 = "General exception on connection: " + exception2.Message + " " + exception2.StackTrace;
                HandleError(str2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            isDisconnecting = true;
            connected = false;
            try
            {
                connection.Client.Shutdown(SocketShutdown.Both);
                connection.Close();
            }
            catch (Exception)
            {
                LogWarn("Trying to disconnect a non-connected udp socket");
            }
            isDisconnecting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf"></param>
        private void HandleBinaryData(byte[] buf)
        {
            CallOnData(buf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        private void HandleError(string err)
        {
            HandleError(err, SocketError.NotSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="err"></param>
        /// <param name="se"></param>
        private void HandleError(string err, SocketError se)
        {
            if (!isDisconnecting)
            {
                LogError(err);
                CallOnError(err, se);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Kill()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void LogError(string msg)
        {
            if (log != null)
            {
                log.Error("UDPSocketLayer: ", msg);
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
                log.Warn("UDPSocketLayer: ", msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Read()
        {
            connected = true;
            while (connected)
            {
                try
                {
                    if (socketPollSleep > 0)
                    {
                        Thread.Sleep(this.socketPollSleep);
                    }
                    byteBuffer = connection.Receive(ref sender);
                    if (byteBuffer.Length == 0)
                    {
                        break;
                    }
                    HandleBinaryData(byteBuffer);
                    continue;
                }
                catch (SocketException exception)
                {
                    HandleError("Error reading data from socket: " + exception.Message, exception.SocketErrorCode);
                    continue;
                }
                catch (Exception exception2)
                {
                    HandleError("General error reading data from socket: " + exception2.Message + " " + exception2.StackTrace);
                    continue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            WriteSocket(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf"></param>
        private void WriteSocket(byte[] buf)
        {
            try
            {
                connection.Send(buf, buf.Length);
            }
            catch (SocketException exception)
            {
                string err = "Error writing to socket: " + exception.Message;
                HandleError(err, exception.SocketErrorCode);
            }
            catch (Exception exception2)
            {
                string str2 = "General error writing to socket: " + exception2.Message + " " + exception2.StackTrace;
                HandleError(str2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return connected;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionDelegate OnConnect
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
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
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
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
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SocketPollSleep
        {
            get
            {
                return socketPollSleep;
            }
            set
            {
                socketPollSleep = value;
            }
        }
    }
}

