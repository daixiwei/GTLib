using com.gt;
using com.gt.coroutine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.gt.coroutine
{

    /// <summary>
    /// 
    /// </summary>
    public class CoroutineObject 
    {
        /// <summary>
        /// 
        /// </summary>
        internal IEnumerator enumer;
        /// <summary>
        /// 
        /// </summary>
        internal Runnable endCallBack;
        /// <summary>
        /// 
        /// </summary>
        private float waitTime;
        /// <summary>
        /// 
        /// </summary>
        internal bool dstroy;
        
        /// <summary>
        /// 
        /// </summary>
        private IWaitCoroutine waitCoroutine;

        /// <summary>
        /// 
        /// </summary>
        public CoroutineObject()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Destroy()
        {
            if (waitCoroutine != null)
                waitCoroutine.Dispose();
            waitCoroutine = null;
            dstroy = true;
            enumer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool MoveNext()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextObject"></param>
        protected virtual void MoveNextObject(object nextObject)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void _MoveNext()
        {
            if (waitTime > 0)
            {
                waitTime -= GTLib.GameManager.DeltaTime;
                return;
            }
            if (!MoveNext()) return;

            
            if (waitCoroutine != null)
            {
                if (waitCoroutine.WaitDone)
                {
                    waitCoroutine = null;
                }
                else
                {
                    return;
                }
            }
            if (enumer.MoveNext())
            {
                object tem = enumer.Current;
                if (tem != null)
                {
                    if (tem is GTWaitForSeconds)
                    {
                        waitTime = ((GTWaitForSeconds)tem).m_Seconds;
                    }
                    if (tem is IWaitCoroutine)
                    {
                        waitCoroutine = (IWaitCoroutine)tem;
                    }
                    MoveNextObject(tem);
                }
            }
            else
            {
                dstroy = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator Enumerator
        {
            get { return enumer; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDestroy
        {
            get { return dstroy; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Runnable EndCallBack
        {
            get { return endCallBack; }
        }

    }
}
