
namespace com.gt.mpnet
{
    using com.gt.mpnet;
    using com.gt.mpnet.controllers;
    using com.gt.mpnet.requests;
    using com.gt.units;
    using System;
    using System.Collections.Generic;
    using System.Timers;

    /// <summary>
    /// 
    /// </summary>
    public class LagMonitor
    {
        //private int interval;
        private int lastReqTime;
        private Timer pollTimer;
        private int queueSize;
        private MPNetClient mpnet;
        private List<int> valueQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sfs"></param>
        public LagMonitor(MPNetClient mpnet) : this(mpnet, 4, 10)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        /// <param name="interval"></param>
        public LagMonitor(MPNetClient mpnet, int interval) : this(mpnet, interval, 10)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        /// <param name="interval"></param>
        /// <param name="queueSize"></param>
        public LagMonitor(MPNetClient mpnet, int interval, int queueSize)
        {
            if (interval < 1)
            {
                interval = 1;
            }
            this.mpnet = mpnet;
            this.queueSize = queueSize;
            valueQueue = new List<int>();
            pollTimer = new Timer();
            pollTimer.Enabled = false;
            pollTimer.AutoReset = true;
            pollTimer.Elapsed += new ElapsedEventHandler(OnPollEvent);
            pollTimer.Interval = interval * 1000;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            Stop();
            pollTimer.Dispose();
            mpnet = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int OnPingPong()
        {
            int item = DateTime.Now.Millisecond - lastReqTime;
            if (valueQueue.Count >= queueSize)
            {
                valueQueue.RemoveAt(0);
            }
            valueQueue.Add(item);
            return AveragePingTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnPollEvent(object source, ElapsedEventArgs e)
        {
            mpnet.Log.Debug("********** Polling!!");
            lastReqTime = DateTime.Now.Millisecond;
            mpnet.Send(new PingPongRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                pollTimer.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                pollTimer.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AveragePingTime
        {
            get
            {
                if (valueQueue.Count == 0)
                {
                    return 0;
                }
                int num = 0;
                foreach (int num2 in valueQueue)
                {
                    num += num2;
                }
                return (num / valueQueue.Count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return pollTimer.Enabled;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LastPingTime
        {
            get
            {
                if (valueQueue.Count > 0)
                {
                    return valueQueue[valueQueue.Count - 1];
                }
                return 0;
            }
        }
    }
}

