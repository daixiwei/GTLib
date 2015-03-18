using com.gt.entities;

namespace com.gt.mpnet.bitswarm
{

    using com.gt.units;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 
        /// </summary>
        IMPObject Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool IsEncrypted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool IsUDP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        long PacketId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int TargetController { get; set; }
    }
}

