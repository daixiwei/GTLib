namespace pumpkin.swf
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class MovieClipAssetInfo 
    {
        /// <summary>
        /// 
        /// </summary>
        public SwfAssetContext assetContext;
        /// <summary>
        /// 
        /// </summary>
        public string className;
        /// <summary>
        /// 
        /// </summary>
        public List<DisplayObjectInfo[]> frames;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, int> labels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameInfo"></param>
        public void addFrame(DisplayObjectInfo[] frameInfo)
        {
            if (frames == null) frames = new List<DisplayObjectInfo[]>();
            this.frames.Add(frameInfo);
        }
    }
}

