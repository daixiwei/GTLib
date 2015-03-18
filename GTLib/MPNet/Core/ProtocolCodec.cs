namespace com.gt.mpnet.core
{
    using com.gt.mpnet.bitswarm;
    using com.gt.entities;
    using com.gt.units;
    using System;
    using com.gt.mpnet.controllers;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class ProtocolCodec : IProtocolCodec
    {
        private static readonly string ACTION_ID = "a";
        private BitSwarmClient bitSwarm;
        private static readonly string CONTROLLER_ID = "c";
        private IoHandler ioHandler = null;
        private Logger log;
        private Dictionary<ushort, IMessage> acceptList = new Dictionary<ushort, IMessage>();
        private static readonly string PARAM_ID = "p";
        private static readonly string UDP_PACKET_ID = "i";
        private static readonly string USER_ID = "u";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioHandler"></param>
        /// <param name="bitSwarm"></param>
        public ProtocolCodec(IoHandler ioHandler, BitSwarmClient bitSwarm)
        {
            this.ioHandler = ioHandler;
            this.log = bitSwarm.Log;
            this.bitSwarm = bitSwarm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestObject"></param>
        private void DispatchRequest(IMPObject requestObject)
        {
            IMessage message = new Message();
            if (requestObject.IsNull(CONTROLLER_ID))
            {
                throw new Exception("Request rejected: No Controller ID in request!");
            }
            if (requestObject.IsNull(ACTION_ID))
            {
                throw new Exception("Request rejected: No Action ID in request!");
            }
            message.Id = Convert.ToInt32(requestObject.GetShort(ACTION_ID));
            message.Content = requestObject.GetMPObject(PARAM_ID);
            message.IsUDP = requestObject.ContainsKey(UDP_PACKET_ID);
            if (message.IsUDP)
            {
                message.PacketId = requestObject.GetLong(UDP_PACKET_ID);
            }
            int @byte = requestObject.GetByte(CONTROLLER_ID);
            IController controller = this.bitSwarm.GetController(@byte);
            if (controller == null)
            {
                throw new Exception("Cannot handle server response. Unknown controller, id: " + @byte);
            }
            controller.HandleMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="pos"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private ByteArray ResizeByteArray(ByteArray array, int pos, int len)
        {
            byte[] dst = new byte[len];
            Buffer.BlockCopy(array.Bytes, pos, dst, 0, len);
            return new ByteArray(dst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public void OnPacketRead(IMPObject packet)
        {
            DispatchRequest(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public void OnPacketRead(ByteArray packet)
        {
            IMPObject requestObject = MPObject.NewFromBinaryData(packet);
            DispatchRequest(requestObject);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void OnPacketWrite(IMessage message)
        {
            IMPObject obj2 = null;
            if (message.IsUDP)
            {
                obj2 = PrepareUDPPacket(message);
            }
            else
            {
                obj2 = PrepareTCPPacket(message);
            }
            message.Content = obj2;
            ioHandler.OnDataWrite(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IMPObject PrepareTCPPacket(IMessage message)
        {
            IMPObject obj2 = new MPObject();
            obj2.PutByte(CONTROLLER_ID, Convert.ToByte(message.TargetController));
            obj2.PutShort(ACTION_ID, Convert.ToInt16(message.Id));
            obj2.PutMPObject(PARAM_ID, message.Content);
            return obj2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IMPObject PrepareUDPPacket(IMessage message)
        {
            IMPObject obj2 = new MPObject();
            obj2.PutByte(CONTROLLER_ID, Convert.ToByte(message.TargetController));
            obj2.PutInt(USER_ID, (bitSwarm.MPNet.MySelf == null) ? -1 : bitSwarm.MPNet.MySelf.Id);
            obj2.PutLong(UDP_PACKET_ID, bitSwarm.NextUdpPacketId());
            obj2.PutMPObject(PARAM_ID, message.Content);
            return obj2;
        }

        /// <summary>
        /// 
        /// </summary>
        public IoHandler IOHandler
        {
            get
            {
                return this.ioHandler;
            }
            set
            {
                if (this.ioHandler != null)
                {
                    throw new Exception("IOHandler is already defined for thir ProtocolHandler instance: " + this);
                }
                this.ioHandler = value;
            }
        }
    }
}

