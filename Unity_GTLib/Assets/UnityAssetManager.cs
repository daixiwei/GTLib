using com.gt.assets;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using com.gt;
using com.gt.entities;

namespace com.platform.unity.assets
{

    /// <summary>
    /// 
    /// </summary>
    public class UnityAssetManager : BaseAssetManager
    {

        /// <summary>
        /// 
        /// </summary>
        public UnityAssetManager() : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadType"></param>
        public override void LoadAsset(string path, string loadType)
        {
            LoadAsset(new AssetParameter(path, AssetParameterType.PersistentDataPath, AssetStorageType.Temporary), loadType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        /// <param name="loadType"></param>
        public override void LoadAsset(AssetParameter assetParameter, string loadType)
        {
            base.LoadAsset(assetParameter, loadType);
        }



        /// <summary>
        /// 
        /// </summary>
        public override void BeginLoadEntity()
        {
            if (this.beginLoad)
            {
                throw new ArgumentException("Already loaded entity!");
            }
            this.beginLoad = true;
            ICollection values = this.needs.Values;
            AssetParameter[] array = new AssetParameter[needs.Count];
            values.CopyTo(array, 0);
            ((UnityGameManager)GTLib.GameManager).BeginLoadAssetsEntity(array, this);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void EndLoadEntity()
        {
            if (beginLoad)
            {
                needs.Clear();
                beginLoad = false;
                ((UnityGameManager)GTLib.GameManager).StopCurrenLoadEntity();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        internal void  RemoveCacheAsset(IAssetBundle bundle)
        {
            assets.Remove(bundle.Name);
            if (bundle.Parameter.PathType == AssetParameterType.Resources) return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        internal void CompleteLoad(IAssetBundle asset)
        {
            if (!beginLoad) throw new ArgumentException("Not loading entity, can't complete be load!");
            if (asset != null && asset.Parameter != null && !assets.ContainsKey(asset.Parameter.Path))
            {
                IAssetLoader assetLoader = GetAssetLoader(asset.Parameter.LoadType);
                if (assetLoader != null)
                {
                    assetLoader.AddAsset(asset);
                }
                assets.Add(asset.Parameter.Path, asset);
                needs.Remove(asset.Parameter.Path);
                loaded++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, IAssetLoader> AssetLoaders
        {
            get
            {
                return assetLoaders;
            }
        }
    }
}
