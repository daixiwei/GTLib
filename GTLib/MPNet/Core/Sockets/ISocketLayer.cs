namespace com.gt.mpnet.core.sockets
{
    using System;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    public interface ISocketLayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adr"></param>
        /// <param name="port"></param>
        void Connect(IPAddress adr, int port);
        
        /// <summary>
        /// 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        void Kill();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void Write(byte[] data);

        /// <summary>
        /// 
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 
        /// </summary>
        ConnectionDelegate OnConnect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        OnDataDelegate OnData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ConnectionDelegate OnDisconnect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        OnErrorDelegate OnError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool RequiresConnection { get; }
    }
}

