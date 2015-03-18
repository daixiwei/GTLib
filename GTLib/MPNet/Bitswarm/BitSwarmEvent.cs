namespace com.gt.mpnet.bitswarm
{
    using com.gt.events;
    using com.gt.mpnet.core;
    using System;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class BitSwarmEvent : BaseEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CONNECT = "connect";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string DATA_ERROR = "dataError";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string DISCONNECT = "disconnect";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string IO_ERROR = "ioError";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string RECONNECTION_TRY = "reconnectionTry";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string SECURITY_ERROR = "securityError";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public BitSwarmEvent(string type) : base(type, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        public BitSwarmEvent(string type, Hashtable arguments) : base(type, arguments)
        {
        }
    }
}

