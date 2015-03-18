using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public enum RequestType
    {
        Login = 1,
        Logout = 2,
        PingPong = 0x1d,
        Handshake = 0,
        CallExtension=13,
        ManualDisconnection=0x1a
    }
}
