namespace pumpkin.display
{
    using pumpkin.swf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The display object class.
    /// </summary>
    public class DisplayObject 
    {
        /// <summary>
        /// 
        /// </summary>
        internal short cachedCid = -1;
        /// <summary>
        /// 
        /// </summary>
        internal int updateCounter = -1;
        /// <summary>
        /// 
        /// </summary>
        protected Matrix m_FullMatrix = new Matrix();
        /// <summary>
        /// 
        /// </summary>
        protected bool m_GroupMatrixDirty = true;
        /// <summary>
        /// 
        /// </summary>
        protected Matrix m_Matrix = new Matrix();
        /// <summary>
        /// 
        /// </summary>
        protected bool m_MatrixDirty = true;
        /// <summary>
        /// 
        /// </summary>
        protected bool m_OverrideMatrix = false;
        /// <summary>
        /// 
        /// </summary>
        protected DisplayObjectContainer m_Parent;
        /// <summary>
        /// 
        /// </summary>
        protected float m_Alpha = 1f;

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix _fastGetFullMatrixRef()
        {
            
            if (m_MatrixDirty || m_GroupMatrixDirty)
            {
                updateMatrix();
            }
            return m_FullMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DisplayObject getChildAt(int id)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childMatrixIn"></param>
        /// <returns></returns>
        public Matrix getFullMatrix(Matrix childMatrixIn)
        {
            if (m_MatrixDirty || m_GroupMatrixDirty)
            {
                updateMatrix();
            }
            return childMatrixIn.mult(m_FullMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float getInheritedAlpha()
        {
            if (parent == null)
            {
                return m_Alpha;
            }
            return (m_Alpha * parent.getInheritedAlpha());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocal"></param>
        public virtual void invalidateMatrix(bool isLocal)
        {
            m_GroupMatrixDirty = m_GroupMatrixDirty || !isLocal;
            m_MatrixDirty = m_GroupMatrixDirty || isLocal;
            updateCounter = -1;
            if (m_OverrideMatrix && isLocal)
            {
                m_OverrideMatrix = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inMatrix"></param>
        public void setMatrixOverride(Matrix inMatrix)
        {
            m_Matrix.setMatrix(inMatrix);
            m_OverrideMatrix = true;
            invalidateMatrix(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void updateFrame(){}

        /// <summary>
        /// 
        /// </summary>
        protected virtual void updateMovieClip(){}

        /// <summary>
        /// 
        /// </summary>
        protected void updateMatrix()
        {
            if (m_MatrixDirty || (m_GroupMatrixDirty && (parent != null)))
            {
                if (m_GroupMatrixDirty && (parent != null))
                {
                    parent.updateMatrix();
                }
                if (!m_OverrideMatrix && m_MatrixDirty)
                {
                    updateMovieClip();
                }
                if (parent != null)
                {
                    Matrix tem = parent.getFullMatrix(m_Matrix);
                    m_FullMatrix.setMatrix(tem);
                }
                else
                {
                    Matrix tem = m_Matrix;
                    m_FullMatrix.setMatrix(tem);
                }
                m_MatrixDirty = false;
                m_GroupMatrixDirty = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal float internalAlpha
        {
            get
            {
                return m_Alpha;
            }
            set
            {
                if (m_Alpha != value)
                {
                    m_Alpha = value;
                    if (m_Alpha > 1f)
                    {
                        m_Alpha = 1f;
                    }
                    if (m_Alpha < 0f)
                    {
                        m_Alpha = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DisplayObjectContainer parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }
    }
}