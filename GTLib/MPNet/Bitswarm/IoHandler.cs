namespace com.gt.mpnet.bitswarm
{
    using com.gt.mpnet.core;
    using com.gt.units;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IoHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        void OnDataRead(ByteArray buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void OnDataWrite(IMessage message);

        /// <summary>
        /// 
        /// </summary>
        IProtocolCodec Codec { get; }
    }
}

