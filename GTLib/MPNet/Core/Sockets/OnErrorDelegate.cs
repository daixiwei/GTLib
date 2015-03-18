namespace com.gt.mpnet.core.sockets
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    /// <param name="se"></param>
    public delegate void OnErrorDelegate(string error, SocketError se);
}

