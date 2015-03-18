namespace com.gt.mpnet.core
{
    using com.gt.events;
    using System;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class MPEvent : BaseEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public const string MARK_RESULT = "code";
        /// <summary>
        /// 
        /// </summary>
        public const string HANDSHAKE = "handshake";
        /// <summary>
        /// 
        /// </summary>
        public const string LOGIN = "login";
        /// <summary>
        /// 
        /// </summary>
        public const string LOGIN_ERROR = "loginError";
        /// <summary>
        /// 
        /// </summary>
        public const string LOGOUT = "logout";
        /// <summary>
        /// 
        /// </summary>
        public const string PING_PONG = "pingpong";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CONNECTION = "connection";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string EXTENSION_RESPONSE = "extensionResponse";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CONNECTION_LOST = "connectionLost";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CONNECTION_RESUME = "connectionResume";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CONNECTION_RETRY = "connectionRetry";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string SOCKET_ERROR = "socketError";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string UDP_INIT = "udpInit";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public MPEvent(string type) : base(type, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public MPEvent(string type, Hashtable data) : base(type, data)
        {
        }
    }
}

