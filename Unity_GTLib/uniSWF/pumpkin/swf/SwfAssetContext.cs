namespace pumpkin.swf
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class SwfAssetContext
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly List<MovieClipAssetInfo> exports = new List<MovieClipAssetInfo>();
        /// <summary>
        /// 
        /// </summary>
        public string swfPrefix;
        /// <summary>
        /// 
        /// </summary>
        public Texture texture;
        /// <summary>
        /// 
        /// </summary>
        public Material material;
        /// <summary>
        /// 
        /// </summary>
        internal readonly List<BitmapAssetInfo> bitmaps = new List<BitmapAssetInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MovieClipAssetInfo getAssetInfoByName(string name)
        {
            foreach (MovieClipAssetInfo info in this.exports)
            {
                if (((info.className != null) && (info.className.CompareTo(name) == 0)))
                {
                    return info;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public MovieClipAssetInfo getMovieInfoByCid(int cid)
        {
            return this.exports[cid];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MovieClipAssetInfo getMovieInfoByName(string name)
        {
            foreach (MovieClipAssetInfo info in this.exports)
            {
                if (info.className == name)
                {
                    return info;
                }
            }
            return null;
        }
    }
}