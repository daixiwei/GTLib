namespace pumpkin.displayInternal
{
    using pumpkin.display;
    using System;
    using UnityEngine;

    /// <summary>
    /// The graphics generator interface.
    /// </summary>
    public interface IGraphicsGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <returns></returns>
        Mesh applyToMeshRenderer(MeshRenderer meshRenderer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="renderQueue"></param>
        /// <returns></returns>
        bool renderStage(DisplayObjectContainer stage, int renderQueue);
    }
}

