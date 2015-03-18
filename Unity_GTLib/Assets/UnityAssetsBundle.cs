using com.gt.assets;
using System.Collections.Generic;
using UnityEngine;

namespace com.platform.unity.assets
{
    /// <summary>
    /// 
    /// </summary>
    public class UnityAssetsBundle : IAssetBundle
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
        internal AssetBundle m_bundle;
        /// <summary>
        /// 
        /// </summary>
        public List<Object> objects;

        /// <summary>
        /// 
        /// </summary>
        public UnityAssetsBundle(UnityAssetManager assetManager, AssetParameter parameter)
        {
            m_AssetManager = assetManager;
            this.m_Parameter = parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        internal void SetAssetBundle(AssetBundle bundle)
        {
            m_bundle = bundle;
            if (bundle == null)
            {
                throw new System.NullReferenceException("Asset " + m_Parameter.Path + " Is Null!");
            }
            if (m_Parameter.UnFile)
            {
                m_bundle.Unload(false);
            }
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
            if (objects == null) throw new System.Exception("AssetBundle is null!is con't be load!");
            foreach (object ob in objects)
            {
                if (((Object)ob).name == name)
                {
                    if (ob is TextAsset)
                    {
                        if (typeof(T) == typeof(System.Byte[]))
                        {
                            return (T)((object)((TextAsset)ob).bytes);
                        }
                    }
                    return (T)ob;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MainAsset<T>()
        {
            if (objects == null) throw new System.Exception("AssetBundle is null!is con't be load!");
            object ob = objects[0];
            return (T)ob;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Unload(string name)
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                Object tem = objects[i];
                if (tem.name == name)
                {
                    Object.DestroyImmediate(tem, true);
                    objects.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unloadAssetManager"></param>
        public void Unload(bool unloadAssetManager)
        {

            if (objects == null) throw new System.Exception("AssetBundle is null!is con't be unload!");
            foreach (Object tem in objects)
            {
                Object.DestroyImmediate(tem, true);
            }

            if (unloadAssetManager)
            {
                m_AssetManager.RemoveCacheAsset(this);
            }
            objects = null;
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

    }
}
