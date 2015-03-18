using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public class ManualDisconnectionRequest : BaseRequest
    {
        public ManualDisconnectionRequest()
            : base(RequestType.ManualDisconnection)
        {
        }
    }
}
