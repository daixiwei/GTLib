namespace pumpkin.display
{
    using pumpkin.swf;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The movie clip player class.
    /// </summary>
    public class MovieClipPlayer : DisplayObjectContainer
    {
        /// <summary>
        /// 
        /// </summary>
        public delegate void FrameCallback();

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<short, DisplayObjectContainer> m_InstanceCache;
        /// <summary>
        /// 
        /// </summary>
        public const byte STATE_PLAYING = 1;
        /// <summary>
        /// 
        /// </summary>
        public const byte STATE_PLAYING_REVERSE = 2;
        /// <summary>
        /// 
        /// </summary>
        public const byte STATE_STOPPED = 0;
        /// <summary>
        /// 
        /// </summary>
        protected SwfAssetContext assetContext;
        /// <summary>
        /// 
        /// </summary>
        protected int m_FrameNum = -1;
        /// <summary>
        /// 
        /// </summary>
        private bool m_Loop = true;
        /// <summary>
        /// 
        /// </summary>
        internal MovieClipAssetInfo m_MovieClipInfo;
        /// <summary>
        /// 
        /// </summary>
        protected byte m_PlayState = STATE_PLAYING;
        private int m_SetFrameNextUpdate = -1;
        /// <summary>
        /// 
        /// </summary>
        protected string m_SymbolName;
        /// <summary>
        /// 
        /// </summary>
        protected string m_NextSymbolName;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, FrameCallback> frameCallbacks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetContext"></param>
        /// <param name="symbolName"></param>
        internal MovieClipPlayer(SwfAssetContext assetContext, string symbolName)
        {
            this.assetContext = assetContext;
            if (symbolName != null)
            {
                setSymbolName(symbolName);
            }
            if (totalFrames == 1)
            {
                m_PlayState = STATE_STOPPED;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameNum"></param>
        /// <param name="isFrameUpdate"></param>
        protected void _setFrame(int frameNum, bool isFrameUpdate)
        {
            if (m_FrameNum == frameNum)
            {
                return;
            }
            if (frameNum < 0)
            {
                frameNum = 0;
            }
            else if (frameNum >= totalFrames)
            {
                frameNum = totalFrames - 1;
            }
            m_FrameNum = frameNum;
            base.m_Children.Clear();

            if ((m_FrameNum < 0) || (m_FrameNum >= m_MovieClipInfo.frames.Count))
            {
                return;
            }

            DisplayObjectInfo[] displayList = m_MovieClipInfo.frames[m_FrameNum];
            int len = displayList.Length;
            for (int i = 0; i < len; i++)
            {
                DisplayObjectInfo dispInfo = displayList[i];

                if (m_InstanceCache == null)
                {
                    m_InstanceCache = new Dictionary<short, DisplayObjectContainer>();
                }
                if (dispInfo.isBitmap)
                {
                    DisplayObjectContainer displayObject = !m_InstanceCache.ContainsKey(dispInfo.instanceId) ? null : m_InstanceCache[dispInfo.instanceId];
                    if ((displayObject != null) && (displayObject.cachedCid != dispInfo.cid))
                    {
                        displayObject = null;
                    }
                    if (displayObject == null)
                    {
                        BitmapAssetInfo bmpInfo = assetContext.bitmaps[dispInfo.cid];

                        displayObject = new DisplayObjectContainer();
                        bmpInfo.material = assetContext.material;
                        displayObject.bmpInfo = bmpInfo;
                        displayObject.cachedCid = dispInfo.cid;

                        m_InstanceCache[dispInfo.instanceId] = displayObject;
                    }
                    base.addChild(displayObject);
                    dispInfo.tranform.applyToSprite(displayObject);
                }
                else
                {
                    MovieClipAssetInfo info3 = assetContext.exports[dispInfo.cid];
                    MovieClipPlayer player = !m_InstanceCache.ContainsKey(dispInfo.instanceId) ? null : (m_InstanceCache[dispInfo.instanceId] is MovieClipPlayer ? (MovieClipPlayer)m_InstanceCache[dispInfo.instanceId] : null);
                    if ((player != null) && (player.cachedCid != dispInfo.cid))
                    {
                        player = null;
                    }
                    if (player == null)
                    {
                        player = new MovieClipPlayer(null, null);
                        player.setSymbolByCid(assetContext, dispInfo.cid);
                        player.cachedCid = dispInfo.cid;

                        if (isFrameUpdate)
                        {
                            player.m_FrameNum = -1;
                        }
                        else
                        {
                            player.setFrame(1, isFrameUpdate);
                        }
                        m_InstanceCache[dispInfo.instanceId] = player;
                    }
                    base.addChild(player);
                    dispInfo.tranform.applyToSprite(player);
                }
            }

            if (frameCallbacks != null && frameCallbacks.ContainsKey(m_FrameNum))
            {
                frameCallbacks[m_FrameNum]();
            }
            if (!string.IsNullOrEmpty(m_NextSymbolName) && frameNum >= totalFrames - 1)
            {
                setSymbolName(m_NextSymbolName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameOrLabel"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool addFrameScript(object frameOrLabel, FrameCallback callback)
        {
            if (frameCallbacks == null) return false;
            if (m_MovieClipInfo == null)
            {
                return false;
            }
            if (m_SymbolName == null)
            {
                return false;
            }
            int num = 0;
            if (frameOrLabel is string)
            {
                num = getFrameLabel((string)frameOrLabel) - 1;
            }
            else if (frameOrLabel is int)
            {
                num = ((int)frameOrLabel) - 1;
            }
            if ((num < 0) || (num >= m_MovieClipInfo.frames.Count))
            {
                return false;
            }
            if (frameCallbacks.ContainsKey(num))
            {
                return false;
            }
            frameCallbacks[num] = callback;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameOrLabel"></param>
        /// <returns></returns>
        public bool removeFrameScript(object frameOrLabel)
        {
            if (frameCallbacks == null) return false;
            if (m_MovieClipInfo == null)
            {
                return false;
            }
            if (m_SymbolName == null)
            {
                return false;
            }
            int num = 0;
            if (frameOrLabel is string)
            {
                num = getFrameLabel((string)frameOrLabel) - 1;
            }
            else if (frameOrLabel is int)
            {
                num = ((int)frameOrLabel) - 1;
            }
            if ((num < 0) || (num >= m_MovieClipInfo.frames.Count))
            {
                return false;
            }

            if (!frameCallbacks.ContainsKey(num))
            {
                return false;
            }
            frameCallbacks.Remove(num);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void clearFrameScript()
        {
            frameCallbacks.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getCurrentFrame()
        {
            return (m_FrameNum + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public int getFrameLabel(string label)
        {
            if (m_MovieClipInfo == null)
            {
                return 0;
            }
            if (!m_MovieClipInfo.labels.ContainsKey(label))
            {
                return 0;
            }
            return m_MovieClipInfo.labels[label];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getSymbolName()
        {
            return m_SymbolName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getTotalFrames()
        {
            return ((m_MovieClipInfo == null) ? 0 : m_MovieClipInfo.frames.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        public void invalidateFrameCache()
        {
            m_FrameNum = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameNum"></param>
        public void setFrame(int frameNum)
        {
            setFrame(frameNum, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameNum"></param>
        /// <param name="isFrameUpdate"></param>
        protected void setFrame(int frameNum, bool isFrameUpdate)
        {
            _setFrame(frameNum - 1, isFrameUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public void setFrameLabel(string label)
        {
            if ((m_MovieClipInfo != null) && m_MovieClipInfo.labels.ContainsKey(label))
            {
                setFrame(m_MovieClipInfo.labels[label]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public void setPlayState(byte state)
        {
            m_PlayState = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetContext"></param>
        /// <param name="cid"></param>
        internal void setSymbolByCid(SwfAssetContext assetContext, int cid)
        {
            m_MovieClipInfo = assetContext.getMovieInfoByCid(cid);
            this.assetContext = assetContext;
            m_FrameNum = -1;
            m_SymbolName = "cid(" + cid + ")";
            if (m_MovieClipInfo != null)
            {
                m_SymbolName = m_MovieClipInfo.className;
            }

            setFrame(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolName"></param>
        public void setSymbolName(string symbolName)
        {
            if (assetContext != null)
            {
                m_FrameNum = -1;
                m_NextSymbolName = null;
                clearFrameScript();
                m_SymbolName = symbolName;
                if (m_MovieClipInfo == null || m_MovieClipInfo.className != symbolName)
                {

                    m_MovieClipInfo = assetContext.getMovieInfoByName(symbolName);
                }

                setFrame(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="nextName"></param>
        public void setSymbolNameEndToNext(string symbolName, string nextName)
        {
            setSymbolName(symbolName);
            m_NextSymbolName = nextName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public bool checkSymmbolName(string symbolName)
        {
            if (assetContext != null)
            {
                MovieClipAssetInfo tem = assetContext.getMovieInfoByName(symbolName);
                return tem != null;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void updateFrame()
        {
            if (m_SetFrameNextUpdate != -1)
            {
                setFrame(m_SetFrameNextUpdate, true);
                m_SetFrameNextUpdate = -1;
            }
            if (m_PlayState == STATE_PLAYING)
            {
                if ((m_FrameNum + 1) >= totalFrames)
                {
                    if (m_Loop) _setFrame(0, true);
                    else setPlayState(STATE_STOPPED);
                }
                else
                {
                    _setFrame(m_FrameNum + 1, true);
                }
            }
            else if (m_PlayState == STATE_PLAYING_REVERSE)
            {
                if ((m_FrameNum - 1) < 0)
                {
                    if (m_Loop) _setFrame(totalFrames - 1, true);
                    else setPlayState(STATE_STOPPED);
                }
                else
                {
                    _setFrame(m_FrameNum - 1, true);
                }
            }
            else
            {
                _setFrame(m_FrameNum, true);
            }
            base.updateFrame();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfUri"></param>
        /// <returns></returns>
        internal static SwfAssetContext _preloadSWF(string swfUri)
        {
            return BuiltinResourceLoader.instance.loadSWF(swfUri, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swfUri"></param>
        /// <returns></returns>
        public static bool unloadSwf(string swfUri)
        {
            return BuiltinResourceLoader.instance.unloadSWF(swfUri);
        }

        /// <summary>
        /// 
        /// </summary>
        public int currentFrame
        {
            get
            {
                return getCurrentFrame();
            }
            set
            {
                m_SetFrameNextUpdate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, int> currentLabels
        {
            get
            {
                return m_MovieClipInfo.labels;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isPlaying
        {
            get
            {
                return ((m_PlayState == STATE_PLAYING) || (m_PlayState == STATE_PLAYING_REVERSE));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool looping
        {
            get
            {
                return m_Loop;
            }
            set
            {
                m_Loop = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte playState
        {
            get
            {
                return m_PlayState;
            }
            set
            {
                m_PlayState = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int totalFrames
        {
            get
            {
                return getTotalFrames();
            }
        }
    }
}