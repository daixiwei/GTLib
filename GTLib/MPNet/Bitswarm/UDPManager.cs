namespace com.gt.mpnet.bitswarm
{
    using com.gt.mpnet;
    using com.gt.mpnet.core;
    using com.gt.mpnet.core.sockets;
    using com.gt.entities;
    using com.gt.units;
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class UDPManager : IUDPManager
    {
        private int currentAttempt;
        private bool initSuccess = false;
        private Timer initThread;
        private object initThreadLocker = new object();
        private bool locked = false;
        private Logger log;
        private readonly int MAX_RETRY = 3;
        private long packetId;
        private readonly int RESPONSE_TIMEOUT = 0xbb8;
        private MPNetClient mpnet;
        private ISocketLayer udpSocket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        public UDPManager(MPNetClient mpnet)
        {
            this.mpnet = mpnet;
            packetId = 0L;
            if (mpnet != null)
            {
                log = mpnet.Log;
            }
            else
            {
                log = new Logger(typeof(UDPManager));
            }
            currentAttempt = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            udpSocket.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udpAddr"></param>
        /// <param name="udpPort"></param>
        public void Initialize(string udpAddr, int udpPort)
        {
            if (initSuccess)
            {
                log.Warn("UDP Channel already initialized!");
            }
            else if (!locked)
            {
                locked = true;
                udpSocket = new UDPSocketLayer(mpnet);
                udpSocket.OnData = new OnDataDelegate(OnUDPData);
                udpSocket.OnError = new OnErrorDelegate(OnUDPError);
                IPAddress adr = IPAddress.Parse(udpAddr);
                udpSocket.Connect(adr, udpPort);
                SendInitializationRequest();
            }
            else
            {
                log.Warn("UPD initialization is already in progress!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void OnTimeout(object state)
        {
            if (!initSuccess)
            {
                object initThreadLocker = this.initThreadLocker;
                lock (initThreadLocker)
                {
                    if (initThread == null)
                    {
                        return;
                    }
                }
                if (currentAttempt < MAX_RETRY)
                {
                    currentAttempt++;
                    log.Debug("UDP Init Attempt: " + this.currentAttempt);
                    SendInitializationRequest();
                    StartTimer();
                }
                else
                {
                    currentAttempt = 0;
                    locked = false;
                    Hashtable data = new Hashtable();
                    data["success"] = false;
                    mpnet.DispatchEvent(new MPEvent(MPEvent.UDP_INIT, data));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bt"></param>
        private void OnUDPData(byte[] bt)
        {
            ByteArray ba = new ByteArray(bt);
            if (ba.BytesAvailable < 4)
            {
                log.Warn("Too small UDP packet. Len: " + ba.Length);
            }
            else
            {
                bool flag = (ba.ReadByte() & 0x20) > 0;
                short num2 = ba.ReadShort();
                if (num2 != ba.BytesAvailable)
                {
                    string str = string.Concat("Insufficient UDP data. Expected: ", num2, ", got: ", ba.BytesAvailable);
                    log.Warn(str);
                }
                else
                {
                    ByteArray array2 = new ByteArray(ba.ReadBytes(ba.BytesAvailable));
                    if (flag)
                    {
                        array2.Uncompress();
                    }
                    IMPObject packet = MPObject.NewFromBinaryData(array2);
                    if (packet.ContainsKey("h"))
                    {
                        if (!initSuccess)
                        {
                            StopTimer();
                            locked = false;
                            initSuccess = true;
                            Hashtable data = new Hashtable();
                            data["success"] = true;
                            mpnet.DispatchEvent(new MPEvent(MPEvent.UDP_INIT, data));
                        }
                    }
                    else
                    {
                        mpnet.GetSocketEngine().IoHandler.Codec.OnPacketRead(packet);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="se"></param>
        private void OnUDPError(string error, SocketError se)
        {
            log.Warn("Unexpected UDP I/O Error. ", error, " [", se.ToString(), "]");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            StopTimer();
            currentAttempt = 1;
            initSuccess = false;
            locked = false;
            packetId = 0L;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryData"></param>
        public void Send(ByteArray binaryData)
        {
            if (initSuccess)
            {
                try
                {
                    udpSocket.Write(binaryData.Bytes);
                }
                catch (Exception exception)
                {
                    log.Warn("WriteUDP operation failed due to Error: " + exception.Message + " " + exception.StackTrace);
                }
            }
            else
            {
                log.Warn("UDP protocol is not initialized yet. Pleas use the initUDP() method.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendInitializationRequest()
        {
            IMPObject obj2 = new MPObject();
            obj2.PutByte("c", 1);
            obj2.PutByte("h", 1);
            obj2.PutLong("i", NextUdpPacketId);
            obj2.PutInt("u", mpnet.MySelf.Id);
            ByteArray array = obj2.ToBinary();
            ByteArray array2 = new ByteArray();
            array2.WriteByte((byte) 0x80);
            array2.WriteShort(Convert.ToInt16(array.Length));
            array2.WriteBytes(array.Bytes);
            udpSocket.Write(array2.Bytes);
            StartTimer();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartTimer()
        {
            if (initThread != null)
            {
                initThread.Dispose();
            }
            initThread = new Timer(new TimerCallback(OnTimeout), null, RESPONSE_TIMEOUT, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopTimer()
        {
            object initThreadLocker = this.initThreadLocker;
            lock (initThreadLocker)
            {
                if (initThread != null)
                {
                    initThread.Dispose();
                    initThread = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Inited
        {
            get
            {
                return initSuccess;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long NextUdpPacketId
        {
            get
            {
                long num;
                packetId = (num = this.packetId) + 1L;
                return num;
            }
        }
    }
}

