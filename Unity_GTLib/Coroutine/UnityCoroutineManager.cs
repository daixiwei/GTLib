using com.gt;
using com.gt.coroutine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.platform.unity.coroutine
{

    /// <summary>
    /// 
    /// </summary>
    public class UnityCoroutineManager : CoroutineManager
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="endCallBack"></param>
        /// <returns></returns>
        protected override CoroutineObject CreateCoroutineObject()
        {
            UnityCoroutineObject ro = new UnityCoroutineObject();
            return ro;
        }
    }
}
