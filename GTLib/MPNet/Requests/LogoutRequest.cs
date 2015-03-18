using System;
using System.Collections.Generic;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public class LogoutRequest : BaseRequest
    {
        public LogoutRequest()
            : base(RequestType.Logout)
        {
        }

        public override void Validate(MPNetClient mpnet)
        {
            if (mpnet.MySelf == null)
            {
                throw new Exception("LogoutRequest Error You are not logged in at the moment!");
            }
        }
    }
}
