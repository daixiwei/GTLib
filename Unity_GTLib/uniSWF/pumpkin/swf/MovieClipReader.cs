namespace pumpkin.swf
{
    using com.gt.units;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    internal class MovieClipReader
    {

        /// <summary>
        /// The read movie clip asset info method.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private MovieClipAssetInfo readMovieClipAssetInfo(ByteArray b)
        {
            MovieClipAssetInfo info = new MovieClipAssetInfo();

            int labelsLen = b.ReadByte();
            for (int i = 0; i < labelsLen; i++)
            {
                if (info.labels == null) info.labels = new Dictionary<string, int>();
                string str = b.ReadUTF();
                int num4 = b.ReadShort();
                info.labels[str] = num4;
            }
            int framesLen = b.ReadShort();
            for (int i = 0; i < framesLen; i++)
            {
                int displaysLen = b.ReadShort();
                DisplayObjectInfo[] frameInfo = new DisplayObjectInfo[displaysLen];
                for (int k = 0; k < displaysLen; k++)
                {
                    DisplayObjectInfo info3 = new DisplayObjectInfo();

                    info3.isBitmap = b.ReadBool();
                    info3.cid = b.ReadShort();
                    info3.instanceId = b.ReadShort();
                    info3.tranform = this.readTransformInfo(b);

                    frameInfo[k] = info3;
                }
                info.addFrame(frameInfo);

            }

            return info;
        }

        /// <summary>
        /// The read swf asset context method
        /// </summary>
        /// <param name="b"></param>
        /// <param name="texSize"></param>
        /// <returns></returns>
        public SwfAssetContext readSwfAssetContext(ByteArray b, Vector2 texSize)
        {
            SwfAssetContext context = new SwfAssetContext();
            //read version number
            int num = b.ReadByte();

            //read bitmap clip asset info list
            int bitmapSize = b.ReadShort();
            for (int i = 0; i < bitmapSize; ++i)
            {
                BitmapAssetInfo info = new BitmapAssetInfo();
                //int cip = b.ReadShort();
                info.srcRect = new Rect()
                { 
                    x = b.ReadShort(),
                    y = b.ReadShort(),
                    width = b.ReadShort(),
                    height = b.ReadShort()
                };
                info.uvRect = new Rect()
                {
                    x = info.srcRect.x / texSize.x,
                    y = info.srcRect.y / texSize.y,
                    width = info.srcRect.width / texSize.x,
                    height = info.srcRect.height / texSize.y,
                };
                context.bitmaps.Add(info);
            }

            //read movie clip asset info list
            int movieClipsLen = b.ReadShort();
            for (int i = 0; i < movieClipsLen; i++)
            {
                string str = b.ReadUTF();
                MovieClipAssetInfo info = this.readMovieClipAssetInfo(b);
                if (info != null)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        info.className = str;
                    }
                    info.assetContext = context;
                    context.exports.Add(info);
                }
            }
            return context;
        }

        /// <summary>
        /// The read matrix method.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private Matrix readMatrix(ByteArray b)
        {
            Matrix matrix = new Matrix
            {
                a = b.ReadFloat(),
                b = b.ReadFloat(),
                c = b.ReadFloat(),
                d = b.ReadFloat(),
                tx = b.ReadFloat(),
                ty = b.ReadFloat()
            };
            return matrix;
        }

        /// <summary>
        /// The read transform info method.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private TransformInfo readTransformInfo(ByteArray b)
        {
            TransformInfo info = new TransformInfo
            {
                matrix = this.readMatrix(b),
                alpha = b.ReadByte()/255f, 
            };
            return info;
        }
    }
}

