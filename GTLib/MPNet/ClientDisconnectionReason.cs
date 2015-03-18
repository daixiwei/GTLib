using System;
using System.Collections.Generic;

namespace com.gt.mpnet
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientDisconnectionReason
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string BAN = "ban";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string IDLE = "idle";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string KICK = "kick";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string MANUAL = "manual";
        /// <summary>
        /// 
        /// </summary>
        private static string[] reasons = new string[] { "idle", "kick", "ban" };
        /// <summary>
        /// 
        /// </summary>
        public static readonly string UNKNOWN = "unknown";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reasonId"></param>
        /// <returns></returns>
        public static string GetReason(int reasonId)
        {
            return reasons[reasonId];
        }
    }
}
