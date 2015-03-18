using System;
using System.Collections.Generic;

namespace com.gt.coroutine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWaitCoroutine
    {
        /// <summary>
        /// 
        /// </summary>
        bool WaitDone { get; }
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
    }
}
