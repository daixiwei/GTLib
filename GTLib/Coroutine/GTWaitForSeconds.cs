using System;
using System.Collections.Generic;

namespace com.gt.coroutine
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class GTWaitForSeconds
    {
        /// <summary>
        /// 
        /// </summary>
        public float m_Seconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public GTWaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds;
        }
    }
}
