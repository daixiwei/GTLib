
namespace com.gt.assets
{
    /// <summary>
    /// 
    /// </summary>
    public enum AssetParameterType
    {
        /// <summary>
        /// 
        /// </summary>
        StreamingAssetsPath=0,
        /// <summary>
        /// 
        /// </summary>
        Resources=1,
        /// <summary>
        /// 
        /// </summary>
        PersistentDataPath=2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AssetStorageType
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Temporary = 2,
        /// <summary>
        /// 
        /// </summary>
        AssetCollection = 4,
        /// <summary>
        /// 
        /// </summary>
        Permanent = 8,
    }

    /// <summary>
    /// 
    /// </summary>
    //public enum AssetRuntimePlatform
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Windows,
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Android,
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Ios
    //}

    /// <summary>
    /// 
    /// </summary>
    public class AssetParameter
    {
        /// <summary>
        /// 
        /// </summary>
        private bool m_UnFile = true;
        /// <summary>
        /// 
        /// </summary>
        public AssetStorageType StorageType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool UnFile { get { return m_UnFile; } set { this.m_UnFile = value; } }
        /// <summary>
        /// 
        /// </summary>
        private AssetParameterType m_Type = AssetParameterType.PersistentDataPath;
        /// <summary>
        /// 
        /// </summary>
        public AssetParameterType PathType { get { return m_Type; } set { m_Type = value; } }
        /// <summary>
        /// 
        /// </summary>
        public string LoadType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public AssetParameter(string path)
        {
            Init(path, AssetParameterType.PersistentDataPath, AssetStorageType.AssetCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public AssetParameter(string path, AssetParameterType type)
        {

            Init(path, type, AssetStorageType.AssetCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="storageType"></param>
        public AssetParameter(string path, AssetStorageType storageType)
        {

            Init(path, AssetParameterType.PersistentDataPath, storageType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public AssetParameter(string path, AssetParameterType type, AssetStorageType storageType)
        {
            Init(path, type, storageType);
        }

        private void Init(string path, AssetParameterType type, AssetStorageType storageType)
        {
            Name = path.Substring(path.LastIndexOf("/") + 1);
            this.Path = path;
            this.PathType = type;
            this.StorageType = storageType;
        }
    }
}
