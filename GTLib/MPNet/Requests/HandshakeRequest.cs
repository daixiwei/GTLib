using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public class HandshakeRequest : BaseRequest, IRequest
    {
        public static readonly string KEY_API = "api";
        public static readonly string KEY_CLIENT_TYPE = "cl";
        public static readonly string KEY_COMPRESSION_THRESHOLD = "ct";
        public static readonly string KEY_MAX_MESSAGE_SIZE = "ms";
        public static readonly string KEY_RECONNECTION_TOKEN = "rt";
        public static readonly string KEY_SESSION_TOKEN = "tk";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiVersion"></param>
        /// <param name="reconnectionToken"></param>
        public HandshakeRequest(string apiVersion, string reconnectionToken)
            : base(RequestType.Handshake)
        {
            mpo.PutUtfString(KEY_API, apiVersion);

            string val = "C#MonoOrWin";
            mpo.PutUtfString(KEY_CLIENT_TYPE, val);
            mpo.PutBool("bin", true);
            if (reconnectionToken != null)
            {
                mpo.PutUtfString(KEY_RECONNECTION_TOKEN, reconnectionToken);
            }
        }
    }
}
