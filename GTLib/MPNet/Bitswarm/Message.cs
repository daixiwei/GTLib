namespace com.gt.mpnet.bitswarm
{
    using com.gt.entities;
    using com.gt.units;
    using System;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class Message : IMessage
    {
        private IMPObject content;
        private int id;
        private bool isEncrypted = false;
        private bool isUDP;
        private long packetId;
        private int targetController;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{ Message id: " + id + " }");
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public IMPObject Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEncrypted
        {
            get
            {
                return isEncrypted;
            }
            set
            {
                isEncrypted = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUDP
        {
            get
            {
                return isUDP;
            }
            set
            {
                isUDP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long PacketId
        {
            get
            {
                return packetId;
            }
            set
            {
                packetId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TargetController
        {
            get
            {
                return targetController;
            }
            set
            {
                targetController = value;
            }
        }
    }
}

