using System.Collections.Generic;
using com.gt.units;
using System;
using System.Collections;
using com.gt.coroutine;

namespace com.gt.assets
{
    

    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseAssetManager : IAssetManager,IWaitCoroutine
    {
        /// <summary>
        /// 
        /// </summary>
        protected Logger m_Log;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, IAssetBundle> assets = new Dictionary<string, IAssetBundle>();
        /// <summary>
        /// 
        /// </summary>
        protected int loaded = 0;
        /// <summary>
        /// 
        /// </summary>
        protected int toLoad = 0;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string,AssetParameter> needs;
        /// <summary>
        /// 
        /// </summary>
        protected bool beginLoad;

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, IAssetLoader> assetLoaders;

        /// <summary>
        /// 
        /// </summary>
        protected BaseAssetManager()
        {
            needs = new Dictionary<string,AssetParameter>();
            assetLoaders = new Dictionary<string, IAssetLoader>();
            m_Log = new Logger(typeof(BaseAssetManager));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        public virtual void Init(IGameManager gameManager){}

        /// <summary>
        /// 
        /// </summary>
        public virtual void ProcessEvents() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Destroy()
        {
            loaded = toLoad = 0;
            needs.Clear();
            ICollection<IAssetBundle> tem = assets.Values;
            foreach (IAssetBundle ab in tem)
            {
                ab.Unload(false);
            }
            assets.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        public virtual void LoadAsset(AssetParameter assetParameter)
        {
            LoadAsset(assetParameter, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        public virtual void LoadAsset(AssetParameter assetParameter, string loadType)
        {
            if (beginLoad) throw new ArgumentException("Loading entity, can't add new resources!");
            if (needs.Count == 0)
            {
                loaded = 0;
                toLoad = 0;
            }
            if (!assets.ContainsKey(assetParameter.Path) && !needs.ContainsKey(assetParameter.Path))
            {
                if (assetLoaders.ContainsKey(loadType))
                {
                    assetLoaders[loadType].LoadAsset(assetParameter, loadType);
                }
                needs.Add(assetParameter.Path, assetParameter);
            }
            else
            {
                if (assets.ContainsKey(assetParameter.Path))
                {
                    IAssetBundle assetBundle = assets[assetParameter.Path];
                    if (assetBundle.Parameter.StorageType <= assetParameter.StorageType)
                    assetBundle.Parameter = assetParameter;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public virtual void LoadAsset(string path)
        {
            LoadAsset(path, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadType"></param>
        public virtual void LoadAsset(string path, string loadType)
        {
            LoadAsset(new AssetParameter(path,  AssetStorageType.Temporary));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="assetLoader"></param>
        public void PutAssetLoader(string loadType, IAssetLoader assetLoader)
        {
            if (!assetLoaders.ContainsKey(loadType))
            {
                assetLoaders.Add(loadType, assetLoader);
            }
            else
            {
                assetLoaders[loadType] = assetLoader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadType"></param>
        /// <returns></returns>
        public virtual IAssetLoader GetAssetLoader(string loadType)
        {
            if (assetLoaders.ContainsKey(loadType))
            {
                return assetLoaders[loadType];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public virtual void UnLoadAsset(string path)
        {
            if (beginLoad) throw new ArgumentException("Loading entity, can't add new resources!");
            IAssetBundle asset = assets[path];
            if (asset != null)
            {
                if (assetLoaders.ContainsKey(asset.Parameter.LoadType))
                {
                    assetLoaders[asset.Parameter.LoadType].RemoveAsset(asset);
                }
                asset.Unload(true);
            }
        }

        /// <summary>
        /// The get asset method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">fileName the asset file name</param>
        /// <returns></returns>
        public virtual IAssetBundle GetAsset(string path)
        {
            IAssetBundle assetBundle = assets[path];
            if (assetBundle == null) throw new Exception("Asset not loaded: " + path);
            return assetBundle;
        }

        /// <summary>
        /// The begin load entity
        /// </summary>
        public abstract void BeginLoadEntity();

        /// <summary>
        /// The stop load entity
        /// </summary>
        public abstract void EndLoadEntity();

        /// <summary>
        /// The clear temp asset
        /// </summary>
        public virtual void ClearTempAsset()
        {
            ICollection<IAssetBundle> list = assets.Values;
            IAssetBundle[] array = new IAssetBundle[list.Count];
            list.CopyTo(array,0);
            for (int i=0;i<array.Length;++i)
            {
                IAssetBundle bundle =array[i];
                if (bundle.Parameter.StorageType == AssetStorageType.Temporary)
                {
                    UnLoadAsset(bundle.Name);
                }
            }
            GC.Collect();
        }

        /// <summary>
        /// Return the progress in percent of completion
        /// </summary>
        public float Progress
        {
            get
            {
                if (toLoad == 0) return 1;
                return Math.Min(1, loaded / (float)toLoad);
            }
        }

        /// <summary>
        /// Return the is complete
        /// </summary>
        public bool IsComplete
        {
            get
            {
                if (needs.Count == 0)
                {
                    beginLoad = false;
                }
                return needs.Count == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WaitDone
        {
            get { return IsComplete; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            EndLoadEntity();
        }
    }

}