namespace com.gt.mpnet.requests
{
    using com.gt.mpnet;
    using com.gt.mpnet.bitswarm;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        void Execute(MPNetClient mpnet);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        void Validate(MPNetClient mpnet);

        /// <summary>
        /// 
        /// </summary>
        bool IsEncrypted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IMessage Message { get; }

        /// <summary>
        /// 
        /// </summary>
        int TargetController { get; set; }
    }
}

