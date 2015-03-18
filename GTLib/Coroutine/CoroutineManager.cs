using System;
using System.Collections;
using System.Collections.Generic;

namespace com.gt.coroutine
{

    /// <summary>
    /// 
    /// </summary>
    public class CoroutineManager
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<CoroutineObject> coroutineList;

        
        /// <summary>
        /// 
        /// </summary>
        public CoroutineManager()
        {
            coroutineList = new List<CoroutineObject>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int CoroutineListSize
        {
            get
            {
                return coroutineList.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        public virtual void StartCoroutine(IEnumerator routine)
        {
            StartCoroutine(routine, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        public virtual void StartCoroutine(IEnumerator routine, Runnable endCallBack)
        {
            foreach (CoroutineObject co in coroutineList)
            {
                if (co.Enumerator.Equals(routine)) return;
            }

            CoroutineObject ro = CreateCoroutineObject();
            ro.enumer = routine;
            ro.endCallBack = endCallBack;
            coroutineList.Add(ro);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual CoroutineObject CreateCoroutineObject()
        {
            return new CoroutineObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine(IEnumerator routine)
        {
            for (int i=0;i<coroutineList.Count;++i)
            {
                CoroutineObject co = coroutineList[i];
                if (co.Enumerator.Equals(routine))
                {
                    co.Destroy();
                    coroutineList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (CoroutineObject co in coroutineList)
            {
                co.Destroy();
            }
            coroutineList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdate()
        {
            if (coroutineList.Count <= 0) return;
            for (int i = 0; i < coroutineList.Count; ++i)
            {
                CoroutineObject enumer = coroutineList[i];
                enumer._MoveNext();
                if (enumer.IsDestroy && enumer.Enumerator!=null)
                {
                    if (enumer.EndCallBack!=null) enumer.EndCallBack();
                    coroutineList.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }
    }
}
