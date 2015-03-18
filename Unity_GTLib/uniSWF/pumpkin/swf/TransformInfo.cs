namespace pumpkin.swf
{
    using pumpkin.display;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class TransformInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public float alpha;
        /// <summary>
        /// 
        /// </summary>
        public Matrix matrix ;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        public void applyToSprite(DisplayObject sprite)
        {
            if ((alpha != 1f) || (alpha != sprite.internalAlpha))
            {
                sprite.internalAlpha = alpha;
            }
            sprite.setMatrixOverride(matrix);
        }
    }
}

