namespace com.gt.mpnet.bitswarm
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public enum PacketReadState
    {
        /// <summary>
        /// 
        /// </summary>
        WAIT_NEW_PACKET,
        /// <summary>
        /// 
        /// </summary>
        WAIT_DATA_SIZE,
        /// <summary>
        /// 
        /// </summary>
        WAIT_DATA_SIZE_FRAGMENT,
        /// <summary>
        /// 
        /// </summary>
        WAIT_DATA,
        /// <summary>
        /// 
        /// </summary>
        INVALID_DATA
    }
}

