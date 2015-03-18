namespace com.gt.mpnet.bitswarm
{
    using com.gt.mpnet.core;
    using com.gt.units;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class PendingPacket
    {
        /// <summary>
        /// 
        /// </summary>
        private ByteArray buffer;
        /// <summary>
        /// 
        /// </summary>
        private PacketHeader header;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        public PendingPacket(PacketHeader header)
        {
            this.header = header;
            buffer = new ByteArray();
            buffer.Compressed = header.Compressed;
        }

        /// <summary>
        /// 
        /// </summary>
        public ByteArray Buffer
        {
            get
            {
                return buffer;
            }
            set
            {
                buffer = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PacketHeader Header
        {
            get
            {
                return header;
            }
        }
    }
}

