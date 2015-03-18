namespace com.gt.mpnet.core
{
    using com.gt.mpnet.bitswarm;
    using com.gt.entities;
    using com.gt.units;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IProtocolCodec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        void OnPacketRead(IMPObject packet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        void OnPacketRead(ByteArray packet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void OnPacketWrite(IMessage message);

        /// <summary>
        /// 
        /// </summary>
        IoHandler IOHandler { get; set; }
    }
}

