
namespace com.gt.assets
{

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
        Permanent = 8,
    }

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
        public string LoadType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public AssetParameter(string path)
        {
            Init(path, AssetStorageType.Permanent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="storageType"></param>
        public AssetParameter(string path, AssetStorageType storageType)
        {

            Init(path, storageType);
        }

        private void Init(string path, AssetStorageType storageType)
        {
            Name = path.Substring(path.LastIndexOf("/") + 1);
            this.Path = path;
            this.StorageType = storageType;
        }
    }
}
