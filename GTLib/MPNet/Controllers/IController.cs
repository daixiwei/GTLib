namespace com.gt.mpnet.controllers
{
    using com.gt.mpnet.bitswarm;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void HandleMessage(IMessage message);

        /// <summary>
        /// 
        /// </summary>
        int Id { get; set; }
    }
}

