namespace pumpkin.display
{
    using pumpkin.swf;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The movie clip class.
    /// </summary>
    public class MovieClip : MovieClipPlayer
    {
        /// <summary>
        /// 
        /// </summary>
        public SwfURI swfUri;
        
        /// <summary>
        /// 
        /// </summary>
        public bool Destroy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected float m_x;
        /// <summary>
        /// 
        /// </summary>
        protected float m_y;
        /// <summary>
        /// 
        /// </summary>
        protected float m_Rotation;
        /// <summary>
        /// 
        /// </summary>
        protected float m_scaleX = 1f;
        /// <summary>
        /// 
        /// </summary>
        protected float m_scaleY = 1f;
        /// <summary>
        /// 
        /// </summary>
        protected bool m_Visible = true;
        /// <summary>
        /// 
        /// </summary>
        protected Color m_ColorTransform = Color.white;

        /// <summary>
        /// 
        /// </summary>
        public MovieClip() : base(null, null)
        {
            SwfURI url = new SwfURI(null);
            initUri(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public MovieClip(SwfURI uri) : base(null, null)
        {
            initUri(uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkage"></param>
        public MovieClip(string linkage) : base(null, null)
        {
            initUri(new SwfURI(linkage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swf"></param>
        /// <param name="symbolName"></param>
        public MovieClip(string swf, string symbolName) : base(null, null)
        {
            SwfURI uri = new SwfURI(swf + ":" + symbolName);
            initUri(uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void gotoAndPlay(object to)
        {
            base.setPlayState(MovieClipPlayer.STATE_PLAYING);
            if (to is string)
            {
                base.setFrameLabel((string) to);
            }
            else if (to is int)
            {
                base.setFrame((int) to);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        public void gotoAndStop(object to)
        {
            base.setPlayState(MovieClipPlayer.STATE_STOPPED);
            if (to is string)
            {
                base.setFrameLabel((string) to);
            }
            else if (to is int)
            {
                base.setFrame((int) to);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        private void initUri(SwfURI uri)
        {
            frameCallbacks = new Dictionary<int, FrameCallback>();
            if (((uri != null) && (uri.swf != null)) && (uri.swf.Length != 0))
            {
                swfUri = uri;
                base.assetContext = MovieClipPlayer._preloadSWF(swfUri.swf);
                if (swfUri.linkage != null)
                {
                    base.setSymbolName(swfUri.linkage);
                }
                if (swfUri.label != null)
                {
                    gotoAndStop(swfUri.label);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void play()
        {
            base.setPlayState(MovieClipPlayer.STATE_PLAYING);
        }

        /// <summary>
        /// 
        /// </summary>
        public void playBackwards()
        {
            base.setPlayState(MovieClipPlayer.STATE_PLAYING_REVERSE);
        }

        /// <summary>
        /// 
        /// </summary>
        public void stop()
        {
            base.setPlayState(MovieClipPlayer.STATE_STOPPED);
        }

        /// <summary>
        /// 
        /// </summary>
        public void stopAll()
        {
            stop();
            for (int i = 0; i < base.numChildren; i++)
            {
                DisplayObject obj2 = getChildAt(i);
                if (obj2 is MovieClip)
                {
                    (obj2 as MovieClip).stopAll();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void updateMovieClip()
        {
            m_Matrix.b = m_Matrix.c = m_Matrix.tx = m_Matrix.ty = 0f;
            m_Matrix.a = m_scaleX;
            m_Matrix.d = m_scaleY;
            float rad = (float)((m_Rotation * Mathf.PI) / 180.0);
            if (rad != 0.0)
            {
                m_Matrix.rotate(rad);
            }
            m_Matrix.tx = m_x;
            m_Matrix.ty = m_y;
        }

        /// <summary>
        /// 
        /// </summary>
        public float alpha
        {
            get
            {
                return m_ColorTransform.a;
            }
            set
            {
                m_ColorTransform.a = m_Alpha;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color colorTransform
        {
            get
            {
                return m_ColorTransform;
            }
            set
            {
                m_ColorTransform = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                if (m_Rotation != value)
                {
                    invalidateMatrix(true);
                    m_Rotation = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float scaleX
        {
            get
            {
                return m_scaleX;
            }
            set
            {
                if (m_scaleX != value)
                {
                    invalidateMatrix(true);
                    m_scaleX = value;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float scaleY
        {
            get
            {
                return m_scaleY;
            }
            set
            {
                if (m_scaleY != value)
                {
                    invalidateMatrix(true);
                    m_scaleY = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float x
        {
            get
            {
                if (m_OverrideMatrix)
                {
                    return m_Matrix.tx;
                }
                return m_x;
            }
            set
            {
                if (((m_x != value) && !m_OverrideMatrix) || ((value != m_Matrix.tx) && m_OverrideMatrix))
                {
                    invalidateMatrix(true);
                    m_x = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float y
        {
            get
            {
                if (m_OverrideMatrix)
                {
                    return m_Matrix.ty;
                }
                return m_y;
            }
            set
            {
                if (((m_y != value) && !m_OverrideMatrix) || ((value != m_Matrix.ty) && m_OverrideMatrix))
                {
                    invalidateMatrix(true);
                    m_y = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                m_Visible = value;
                invalidateMatrix(true);
            }
        }
    }
}

