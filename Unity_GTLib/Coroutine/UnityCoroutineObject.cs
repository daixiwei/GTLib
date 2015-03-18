using com.gt.coroutine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.platform.unity.coroutine
{

    /// <summary>
    /// 
    /// </summary>
    public class UnityCoroutineObject : CoroutineObject
    {
        /// <summary>
        /// 
        /// </summary>
        private WWW www;

        /// <summary>
        /// 
        /// </summary>
        public override void Destroy()
        {
            if (www != null)
                www.Dispose();
            www = null;
            base.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool MoveNext()
        {
            if (www != null)
            {
                if (www.isDone)
                {
                    www = null;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextObject"></param>
        protected override void MoveNextObject(object nextObject)
        {
            if (nextObject is WWW)
            {
                www = (WWW)nextObject;
            }
        }
    }
}
