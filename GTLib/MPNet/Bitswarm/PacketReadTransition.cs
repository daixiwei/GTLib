namespace com.gt.mpnet.bitswarm
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public enum  PacketReadTransition
    {
        /// <summary>
        /// 
        /// </summary>
        HeaderReceived,
        /// <summary>
        /// 
        /// </summary>
        SizeReceived,
        /// <summary>
        /// 
        /// </summary>
        IncompleteSize,
        /// <summary>
        /// 
        /// </summary>
        WholeSizeReceived,
        /// <summary>
        /// 
        /// </summary>
        PacketFinished,
        /// <summary>
        /// 
        /// </summary>
        InvalidData,
        /// <summary>
        /// 
        /// </summary>
        InvalidDataFinished
    }
}

