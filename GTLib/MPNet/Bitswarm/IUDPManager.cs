namespace com.gt.mpnet.bitswarm
{
    using com.gt.units;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IUDPManager
    {
        /// <summary>
        /// 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udpAddr"></param>
        /// <param name="udpPort"></param>
        void Initialize(string udpAddr, int udpPort);

        /// <summary>
        /// 
        /// </summary>
        void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryData"></param>
        void Send(ByteArray binaryData);

        /// <summary>
        /// 
        /// </summary>
        bool Inited { get; }

        /// <summary>
        /// 
        /// </summary>
        long NextUdpPacketId { get; }
    }
}

