using com.gt.assets;
using System;
using System.Collections.Generic;
using System.Collections;
using com.gt;
using com.gt.entities;
using System.IO;

namespace com.platform.windows.assets
{

    /// <summary>
    /// 
    /// </summary>
    public class WinAssetManager : BaseAssetManager
    {
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator syLoadUnityAssetsBundle;

        /// <summary>
        /// 
        /// </summary>
        public WinAssetManager()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadType"></param>
        public override void LoadAsset(string path, string loadType)
        {
            LoadAsset(new AssetParameter(path,  AssetStorageType.Temporary), loadType);
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
            GTLib.GameManager.StartGTCoroutine(syLoadUnityAssetsBundle = LoadWinAssetsBundle(array, this));

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
                if (syLoadUnityAssetsBundle != null)
                {
                    GTLib.GameManager.StopGTCoroutine(syLoadUnityAssetsBundle);
                    syLoadUnityAssetsBundle = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="needs"></param>
        /// <returns></returns>
        private IEnumerator LoadWinAssetsBundle(AssetParameter[] needs, WinAssetManager winAssetManager)
        {
            foreach (AssetParameter par in needs)
            {
                string path = GetPath(par);
                byte[] assetObject = File.ReadAllBytes(path);
                ResourcesBundle tem = new ResourcesBundle(winAssetManager, par, assetObject);
                yield return tem;
            }
            yield return null;
            syLoadUnityAssetsBundle = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static string GetPath(AssetParameter parameter)
        {
            string path = "";
            string type = "Resources";
            path = Directory.GetCurrentDirectory() + type + "/";
            return path + parameter.Path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        internal void RemoveCacheAsset(IAssetBundle bundle)
        {
            assets.Remove(bundle.Name);
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
