namespace com.gt.mpnet.core
{
    using com.gt.mpnet.bitswarm;
    using com.gt.units;
    using gt.entities;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class MPIOHandler : IoHandler
    {
        private BitSwarmClient bitSwarm;
        private readonly ByteArray EMPTY_BUFFER = new ByteArray();
        private FiniteStateMachine fsm;
        public static readonly int INT_BYTE_SIZE = 4;
        private Logger log;
        private PendingPacket pendingPacket;
        private IProtocolCodec protocolCodec;
        public static readonly int SHORT_BYTE_SIZE = 2;
        private int skipBytes = 0;

        /// <summary>
        /// /
        /// </summary>
        /// <param name="bitSwarm"></param>
        public MPIOHandler(BitSwarmClient bitSwarm)
        {
            this.bitSwarm = bitSwarm;
            this.log = bitSwarm.Log;
            this.protocolCodec = new ProtocolCodec(this, bitSwarm);
            this.InitStates();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteArray HandleDataSize(ByteArray data)
        {
            string messages = string.Concat("Handling Header Size. Length: ", data.Length, " (", !this.pendingPacket.Header.BigSized ? "small" : "big", ")");
            log.Debug(messages);
            int num = -1;
            int pos = SHORT_BYTE_SIZE;
            if (pendingPacket.Header.BigSized)
            {
                if (data.Length >= INT_BYTE_SIZE)
                {
                    num = data.ReadInt();
                }
                pos = 4;
            }
            else if (data.Length >= SHORT_BYTE_SIZE)
            {
                num = data.ReadUShort();
            }
            log.Debug("Data size is " + num);
            if (num != -1)
            {
                pendingPacket.Header.ExpectedLength = num;
                data = ResizeByteArray(data, pos, data.Length - pos);
                fsm.ApplyTransition(PacketReadTransition.SizeReceived);
                return data;
            }
            fsm.ApplyTransition(PacketReadTransition.IncompleteSize);
            pendingPacket.Buffer.WriteBytes(data.Bytes);
            data = EMPTY_BUFFER;
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteArray HandleDataSizeFragment(ByteArray data)
        {
            log.Debug("Handling Size fragment. Data: " + data.Length);
            int count = !pendingPacket.Header.BigSized ? (SHORT_BYTE_SIZE - pendingPacket.Buffer.Length) : (INT_BYTE_SIZE - pendingPacket.Buffer.Length);
            if (data.Length >= count)
            {
                pendingPacket.Buffer.WriteBytes(data.Bytes, 0, count);
                int num2 = !pendingPacket.Header.BigSized ? 2 : 4;
                ByteArray array = new ByteArray();
                array.WriteBytes(pendingPacket.Buffer.Bytes, 0, num2);
                array.Position = 0;
                int num3 = !pendingPacket.Header.BigSized ? array.ReadShort() : array.ReadInt();
                log.Debug("DataSize is ready: " + num3 + " bytes");
                pendingPacket.Header.ExpectedLength = num3;
                pendingPacket.Buffer = new ByteArray();
                fsm.ApplyTransition(PacketReadTransition.WholeSizeReceived);
                if (data.Length > count)
                {
                    data = ResizeByteArray(data, count, data.Length - count);
                    return data;
                }
                data = EMPTY_BUFFER;
                return data;
            }
            pendingPacket.Buffer.WriteBytes(data.Bytes);
            data = EMPTY_BUFFER;
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteArray HandleInvalidData(ByteArray data)
        {
            if (this.skipBytes == 0)
            {
                this.fsm.ApplyTransition(PacketReadTransition.InvalidDataFinished);
                return data;
            }
            int pos = Math.Min(data.Length, this.skipBytes);
            data = this.ResizeByteArray(data, pos, data.Length - pos);
            this.skipBytes -= pos;
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteArray HandleNewPacket(ByteArray data)
        {
            log.Debug("Handling New Packet of size " + data.Length);
            byte headerByte = data.ReadByte();
            if (~(headerByte & 0x80) > 0)
            {
                throw new Exception(string.Concat("Unexpected header byte: ", headerByte));
            }
            PacketHeader header = PacketHeader.FromBinary(headerByte);
            pendingPacket = new PendingPacket(header);
            fsm.ApplyTransition(PacketReadTransition.HeaderReceived);
            return ResizeByteArray(data, 1, data.Length - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteArray HandlePacketData(ByteArray data)
        {
            int count = pendingPacket.Header.ExpectedLength - pendingPacket.Buffer.Length;
            bool flag = data.Length > count;
            ByteArray array = new ByteArray(data.Bytes);
            try
            {
                string message = string.Concat("Handling Data: ", data.Length, ", previous state: ", pendingPacket.Buffer.Length, "/", pendingPacket.Header.ExpectedLength);
                log.Debug(message);
                if (data.Length >= count)
                {
                    pendingPacket.Buffer.WriteBytes(data.Bytes, 0, count);
                    log.Debug("<<< Packet Complete >>>");
                    if (pendingPacket.Header.Compressed)
                    {
                        pendingPacket.Buffer.Uncompress();
                    }
                    protocolCodec.OnPacketRead(pendingPacket.Buffer);
                    fsm.ApplyTransition(PacketReadTransition.PacketFinished);
                }
                else
                {
                    pendingPacket.Buffer.WriteBytes(data.Bytes);
                }
                if (flag)
                {
                    data = ResizeByteArray(data, count, data.Length - count);
                    return data;
                }
                data = EMPTY_BUFFER;
            }
            catch (Exception exception)
            {
                log.Error("Error handling data: " + exception.Message + " " + exception.StackTrace);
                skipBytes = count;
                fsm.ApplyTransition(PacketReadTransition.InvalidData);
                return array;
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitStates()
        {
            this.fsm = new FiniteStateMachine();
            this.fsm.AddAllStates(typeof(PacketReadState));
            this.fsm.AddStateTransition(PacketReadState.WAIT_NEW_PACKET, PacketReadState.WAIT_DATA_SIZE, PacketReadTransition.HeaderReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA, PacketReadTransition.SizeReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadTransition.IncompleteSize);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadState.WAIT_DATA, PacketReadTransition.WholeSizeReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.PacketFinished);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.INVALID_DATA, PacketReadTransition.InvalidData);
            this.fsm.AddStateTransition(PacketReadState.INVALID_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.InvalidDataFinished);
            this.fsm.SetCurrentState(PacketReadState.WAIT_NEW_PACKET);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void OnDataRead(ByteArray data)
        {
            if (data.Length == 0)
            {
                throw new Exception("Unexpected empty packet data: no readable bytes available!");
            }
            if (bitSwarm != null && bitSwarm.MPNet.Debug)
            {
                if (data.Length > 0x400)
                {
                    log.Info("Data Read: Size > 1024, dump omitted");
                }
            }
            data.Position = 0;
            while (data.Length > 0)
            {
                if (ReadState == PacketReadState.WAIT_NEW_PACKET)
                {
                    data = HandleNewPacket(data);
                }
                else
                {
                    if (ReadState == PacketReadState.WAIT_DATA_SIZE)
                    {
                        data = HandleDataSize(data);
                        continue;
                    }
                    if (ReadState == PacketReadState.WAIT_DATA_SIZE_FRAGMENT)
                    {
                        data = HandleDataSizeFragment(data);
                        continue;
                    }
                    if (ReadState == PacketReadState.WAIT_DATA)
                    {
                        data = HandlePacketData(data);
                        continue;
                    }
                    if (ReadState == PacketReadState.INVALID_DATA)
                    {
                        data = HandleInvalidData(data);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void OnDataWrite(IMessage message)
        {
            ByteArray binData = new ByteArray();
            ByteArray array2 = message.Content.ToBinary();
            bool compressed = false;
            if (array2.Length > bitSwarm.CompressionThreshold)
            {
                array2.Compress();
                compressed = true;
            }
            if (array2.Length > bitSwarm.MaxMessageSize)
            {
                throw new Exception(string.Concat("Message size is too big: ", array2.Length, ", the server limit is: ", bitSwarm.MaxMessageSize));
            }
            int num = SHORT_BYTE_SIZE;
            if (array2.Length > 0xffff)
            {
                num = INT_BYTE_SIZE;
            }
            bool useBlueBox = false;
            binData.WriteByte(new PacketHeader(message.IsEncrypted, compressed, useBlueBox, num == INT_BYTE_SIZE).Encode());
            if (num > SHORT_BYTE_SIZE)
            {
                binData.WriteInt(array2.Length);
            }
            else
            {
                binData.WriteUShort(Convert.ToUInt16(array2.Length));
            }
            binData.WriteBytes(array2.Bytes);

            if (bitSwarm.Socket.IsConnected)
            {
                if (message.IsUDP)
                {
                    WriteUDP(message, binData);
                }
                else
                {
                    WriteTCP(message, binData);
                }
            }
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
        /// <param name="message"></param>
        /// <param name="writeBuffer"></param>
        private void WriteTCP(IMessage message, ByteArray writeBuffer)
        {
            bitSwarm.Socket.Write(writeBuffer.Bytes);
            if (bitSwarm.Debug)
            {
                log.Info("Data written");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="writeBuffer"></param>
        private void WriteUDP(IMessage message, ByteArray writeBuffer)
        {
            bitSwarm.UdpManager.Send(writeBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public IProtocolCodec Codec
        {
            get
            {
                return protocolCodec;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private PacketReadState ReadState
        {
            get
            {
                return (PacketReadState) fsm.GetCurrentState();
            }
        }
    }
}

