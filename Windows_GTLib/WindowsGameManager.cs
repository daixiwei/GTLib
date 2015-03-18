using com.gt;
using com.gt.assets;
using com.gt.coroutine;
using com.gt.events;
using com.gt.mpnet;
using com.gt.units;
using com.platform.windows.assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace com.platform.windows
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WindowsGameManager : IGameManager
    {
        
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<Type, IService> m_Services;
        /// <summary>
        /// 
        /// </summary>
        private Queue<Runnable> m_CacheQueue;
        /// <summary>
        /// 
        /// </summary>
        private object cachesLocker;
        /// <summary>
        /// 
        /// </summary>
        private Logger m_Log;
        /// <summary>
        /// 
        /// </summary>
        protected bool isInit;
        /// <summary>
        /// 
        /// </summary>
        private CoroutineManager m_CoroutineManager;
        /// <summary>
        /// 
        /// </summary>
        private int lastTime;
        /// <summary>
        /// 
        /// </summary>
        private static float fixedDeltaTime;
        /// <summary>
        /// 
        /// </summary>
        private bool isRun;

        public WindowsGameManager()
        {
            cachesLocker = new object();
            m_Log = new Logger(typeof(WindowsGameManager));
            m_Services = new Dictionary<Type, IService>();
            m_CacheQueue = new Queue<Runnable>();
            m_CoroutineManager = new CoroutineManager();
            InitServices();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitServices()
        {
            GTLib.GameManager = this;
            GTLib.AssetManager = new WinAssetManager();
            m_Services.Add(typeof(IAssetManager), GTLib.AssetManager);
            GTLib.NetManager = new MPNetManager();
            m_Services.Add(typeof(INetManager), GTLib.NetManager);

            //init services
            ICollection values = m_Services.Values;
            foreach (IService sv in values)
            {
                sv.Init(this);
            }

            Init();
            isInit = true;
            isRun = true;
            lastTime = DateTime.Now.Millisecond;
            Thread run = new Thread(FixedUpdate);
            run.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            while (isRun)
            {
                int now = DateTime.Now.Millisecond;
                fixedDeltaTime = (now - lastTime) / 1000f;
                if (isInit)
                {
                    m_CoroutineManager.OnUpdate();
                    ProcessEvents();
                }
                lastTime = DateTime.Now.Millisecond;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : IService
        {
            return (T)m_Services[typeof(T)];
        }

        /// <summary>
        /// The Process Events
        /// </summary>
        protected virtual void ProcessEvents()
        {
            if (m_CacheQueue.Count > 0)
            {
                Runnable[] runnableArray;
                lock (cachesLocker)
                {
                    runnableArray = m_CacheQueue.ToArray();
                    m_CacheQueue.Clear();
                }
                foreach (Runnable runnable in runnableArray)
                {
                    runnable();
                }
            }

            ICollection<IService> values = m_Services.Values;
            foreach (IService service in values)
            {
                service.ProcessEvents();
            }
        }

        /// <summary>
        /// Adds the cache task.
        /// </summary>
        public virtual void PostRunnable(Runnable runnable)
        {
            lock (cachesLocker)
            {
                m_CacheQueue.Enqueue(runnable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="typeName"></param>
        /// <param name="message"></param>
        public void Print(LogLevel level, string typeName, string message)
        {
            string tem = string.Concat(typeName, " - ", message);
            switch (level)
            {
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                    Console.WriteLine("[info] " + tem);
                    break;
                case LogLevel.WARN:
                    Console.WriteLine("[warn] " + tem);
                    break;
                case LogLevel.ERROR:
                    Console.WriteLine("[error] " + tem);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        public void StartGTCoroutine(IEnumerator routine)
        {
            m_CoroutineManager.StartCoroutine(routine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="endCallBack"></param>
        public void StartGTCoroutine(IEnumerator routine, Runnable endCallBack)
        {
            m_CoroutineManager.StartCoroutine(routine, endCallBack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        public void StopGTCoroutine(IEnumerator routine)
        {
            m_CoroutineManager.StartCoroutine(routine);
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopAllGTCoroutine()
        {
            m_CoroutineManager.Clear();
        }

        /// <summary>
        /// The logger
        /// </summary>
        public Logger Log
        {
            get
            {
                return m_Log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float DeltaTime 
        { 
            get 
            { 
                return fixedDeltaTime; 
            } 
        }
    }
}
