using System.Collections.Generic;
using com.gt;
using com.gt.units;
using UnityEngine;
using com.gt.assets;
using System;
using com.gt.events;
using System.Collections;
using System.IO;
using com.gt.entities;
using System.Reflection;
using com.platform.unity.assets;
using com.gt.coroutine;
using com.platform.unity.coroutine;
using pumpkin.swf;
using com.gt.mpnet;

namespace com.platform.unity
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UnityGameManager : MonoBehaviour,IGameManager
    {
        
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<Type, IService> m_Services;
        /// <summary>
        /// 
        /// </summary>
        private object cachesLocker = new object();
        /// <summary>
        /// 
        /// </summary>
        private Queue<Runnable> m_CacheQueue;
        /// <summary>
        /// 
        /// </summary>
        private Logger m_Log;
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator syLoadUnityAssetsBundle;
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
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            cachesLocker = new object();
            m_Log = new Logger(typeof(UnityGameManager));
            m_Services = new Dictionary<Type, IService>();
            m_CacheQueue = new Queue<Runnable>();
            m_CoroutineManager = new UnityCoroutineManager();
            InitServices();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitServices()
        {
            GTLib.GameManager = this;
            GTLib.AssetManager = new UnityAssetManager();
            m_Services.Add(typeof(IAssetManager), GTLib.AssetManager);
            GTLib.AssetManager.PutAssetLoader("swf", BuiltinResourceLoader.instance);
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
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();

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
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (isInit)
            {
                m_CoroutineManager.OnUpdate();
                ProcessEvents();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            ICollection values = m_Services.Values;
            foreach (IService sv in values)
            {
                sv.Destroy();
            }
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
        public virtual void Print(LogLevel level, string typeName, string message)
        {
            string tem = string.Concat(typeName, " - ", message);
            switch (level)
            {
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                    Debug.Log(tem);
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(tem);
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(tem);
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
        /// 
        /// </summary>
        /// <param name="needs"></param>
        /// <param name="unityAssetManager"></param>
        internal virtual void BeginLoadAssetsEntity(AssetParameter[] needs, UnityAssetManager unityAssetManager)
        {
            StartCoroutine(syLoadUnityAssetsBundle = LoadUnityAssetsBundle(needs, unityAssetManager));
        }

        /// <summary>
        /// 
        /// </summary>
        internal void StopCurrenLoadEntity()
        {
            if (syLoadUnityAssetsBundle != null)
            {
                StopCoroutine(syLoadUnityAssetsBundle);
                syLoadUnityAssetsBundle = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="needs"></param>
        /// <returns></returns>
        private IEnumerator LoadUnityAssetsBundle(AssetParameter[] needs, UnityAssetManager unityAssetManager)
        {
            foreach(AssetParameter par in needs)
            {
                if (par.PathType == AssetParameterType.Resources)
                {
                    UnityEngine.Object assetObject = Resources.Load(par.Path);
                    ResourcesBundle tem = new ResourcesBundle((UnityAssetManager)GTLib.AssetManager, par, assetObject);
                    yield return tem;
                }
                else
                {
                    string path = GetPath(par);
                    WWW www = new WWW(path + ".assetbundle");
                    www.threadPriority = ThreadPriority.Low;
                    yield return www;
                    UnityAssetsBundle tem = new UnityAssetsBundle((UnityAssetManager)GTLib.AssetManager, par);
                    tem.SetAssetBundle(www.assetBundle);
                }
            }
            yield return null;
            syLoadUnityAssetsBundle = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="gobject"></param>
        /// <returns></returns>
        public static GameObject CopyGameObject(Transform parent, GameObject gobject)
        {
            GameObject tem1 = (GameObject)GameObject.Instantiate(gobject);
            tem1.transform.parent = parent;
            tem1.transform.localPosition = Vector3.zero;
            tem1.transform.localScale = Vector3.one;
            return tem1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static string GetPath(AssetParameter parameter)
        {
            string path = "";
            string type = parameter.PathType == AssetParameterType.PersistentDataPath ? Application.persistentDataPath : Application.streamingAssetsPath;

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (parameter.PathType == AssetParameterType.PersistentDataPath)
                {
                    path = ("file:///" + type + "/Pack_Android/");
                }
                else if (parameter.PathType == AssetParameterType.StreamingAssetsPath)
                {
                   path = ("file://" + type + "/");
                }
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                if (parameter.PathType == AssetParameterType.PersistentDataPath)
                {
                    path = ("file:///" + type + "/Pack_Android/");
                }
                else if (parameter.PathType == AssetParameterType.StreamingAssetsPath)
                {
                    path = (type + "/");
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (parameter.PathType == AssetParameterType.PersistentDataPath)
                {
                    path = ("file:///" + type + "/Pack_IOS/");
                }
                else if (parameter.PathType == AssetParameterType.StreamingAssetsPath)
                {
                    path = (type + "/");
                }
            }
            return path + parameter.Path;
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
                return Time.fixedDeltaTime;
            } 
        }
    }
}
