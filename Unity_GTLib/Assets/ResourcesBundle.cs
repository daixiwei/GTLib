using com.gt.assets;
using System.Collections.Generic;
using UnityEngine;

namespace com.platform.unity.assets
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourcesBundle : IAssetBundle
    {

        /// <summary>
        /// 
        /// </summary>
        private UnityAssetManager m_AssetManager;
        /// <summary>
        /// 
        /// </summary>
        private AssetParameter m_Parameter;

        /// <summary>
        /// 
        /// </summary>
        private Object assetObject;

        /// <summary>
        /// 
        /// </summary>
        public ResourcesBundle(UnityAssetManager assetManager, AssetParameter parameter,Object assetObject)
        {
            m_AssetManager = assetManager;
            this.m_Parameter = parameter;
            this.assetObject = assetObject;
            m_AssetManager.CompleteLoad(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Load<T>(string name)
        {
            return (T)(object)assetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MainAsset<T>()
        {
            return (T)(object)assetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unloadAssetManager"></param>
        public void Unload(bool unloadAssetManager)
        {
            assetObject = null;
            if (unloadAssetManager)
            {
                m_AssetManager.RemoveCacheAsset(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AssetParameter Parameter
        {
            get
            {
                return m_Parameter;
            }
            set
            {
                this.m_Parameter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return m_Parameter == null ? "" : m_Parameter.Path; }
        }


        public void Unload(string name)
        {
            
        }
    }
}
