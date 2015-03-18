using com.gt.assets;
using com.gt.entities;
using System;
using System.Collections.Generic;

namespace com.gt
{

    /// <summary>
    /// 
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        /// <param name="loadType"></param>
        void LoadAsset(AssetParameter assetParameter, string loadType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        void AddAsset(IAssetBundle bundle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        void RemoveAsset(IAssetBundle bundle);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAssetManager : IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        void LoadAsset(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadType"></param>
        void LoadAsset(string path, string loadType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        void LoadAsset(AssetParameter assetParameter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetParameter"></param>
        /// <param name="loadType"></param>
        void LoadAsset(AssetParameter assetParameter, string loadType);

        /// <summary>
        /// Return asset by path 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAssetBundle GetAsset(string path);

        /// <summary>
        /// The put asset loader
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="assetLoader"></param>
        void PutAssetLoader(string loadType, IAssetLoader assetLoader);

        /// <summary>
        /// The begin load entity
        /// </summary>
        void BeginLoadEntity();

        /// <summary>
        /// The stop load entity
        /// </summary>
        void EndLoadEntity();

        /// <summary>
        /// The unload asset
        /// </summary>
        /// <param name="path"></param>
        void UnLoadAsset(string path);

        /// <summary>
        /// The clear temp asset
        /// </summary>
        void ClearTempAsset();

        /// <summary>
        /// Return the progress in percent of completion
        /// </summary>
        float Progress
        {
            get;
        }

        /// <summary>
        /// Return the is complete
        /// </summary>
        bool IsComplete
        {
            get;
        }
    }
}
