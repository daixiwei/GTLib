namespace pumpkin.display
{
    using pumpkin.swf;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The display object container class.
    /// </summary>
    public class DisplayObjectContainer : DisplayObject
    {
        /// <summary>
        /// 
        /// </summary>
        public BitmapAssetInfo bmpInfo;
        /// <summary>
        /// 
        /// </summary>
        protected readonly List<DisplayObject> m_Children = new List<DisplayObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayObject"></param>
        public void addChild(DisplayObject displayObject)
        {
            if ((displayObject != null) && (m_Children.IndexOf(displayObject) == -1))
            {
                if (displayObject.parent != null)
                {
                    displayObject.parent.removeChild(displayObject);
                }
                displayObject.parent= this;
                m_Children.Add(displayObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayObject"></param>
        /// <param name="index"></param>
        public void addChildAt(DisplayObject displayObject, int index)
        {
            addChild(displayObject);
            setChildIndex(displayObject, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DisplayObject getChildAt(int id)
        {
            if (id >= m_Children.Count)
            {
                return null;
            }
            return m_Children[id];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public int getChildIndex(DisplayObject child)
        {
            return m_Children.IndexOf(child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocal"></param>
        public override void invalidateMatrix(bool isLocal)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].invalidateMatrix(false);
            }
            base.invalidateMatrix(isLocal);
        }

        /// <summary>
        /// 
        /// </summary>
        public void removeAllChildren()
        {
            while (numChildren > 0)
            {
                removeChildAt(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayObject"></param>
        public void removeChild(DisplayObject displayObject)
        {
            if ((displayObject != null) && (m_Children.IndexOf(displayObject) != -1))
            {
                displayObject.parent = null;
                m_Children.Remove(displayObject);
                invalidateMatrix(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void removeChildAt(int id)
        {
            removeChild(getChildAt(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        public void setChildIndex(DisplayObject child, int index)
        {
            if (index <= m_Children.Count)
            {
                int num = getChildIndex(child);
                if (num >= 0)
                {
                    if (index < num)
                    {
                        for (int i = num; i > index; i--)
                        {
                            m_Children[i] = m_Children[i - 1];
                        }
                        m_Children[index] = child;
                    }
                    else if (num < index)
                    {
                        for (int j = num; j < index; j++)
                        {
                            m_Children[j] = m_Children[j + 1];
                        }
                        m_Children[index] = child;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void updateFrame()
        {
            base.updateFrame();
            for (int i = 0; i < m_Children.Count; i++)
            {
                if (m_Children[i] is MovieClip && (m_Children[i] as MovieClip).Destroy)
                {
                    m_Children.RemoveAt(i);
                    --i;
                    continue;
                }
                m_Children[i].updateFrame();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int numChildren
        {
            get
            {
                return m_Children.Count;
            }
        }
    }
}

