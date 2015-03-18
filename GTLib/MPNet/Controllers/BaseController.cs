namespace com.gt.mpnet.controllers
{
    using com.gt.mpnet;
    using com.gt.units;
    using System;
    using com.gt.mpnet.bitswarm;
    using System.Collections.Generic;
    using com.gt.events;

    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseController : IController
    {
        protected BitSwarmClient bitSwarm;
        protected int id = -1;
        protected Logger log;
        protected MPNetClient mpnet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitSwarm"></param>
        public BaseController(BitSwarmClient bitSwarm)
        {
            this.bitSwarm = bitSwarm;
            if (bitSwarm != null)
            {
                log = bitSwarm.Log;
                mpnet = bitSwarm.MPNet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public abstract void HandleMessage(IMessage message);

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
                if (id != -1)
                {
                    throw new Exception("Controller ID is already set: " + id + ". Can't be changed at runtime!");
                }
                id = value;
            }
        }
    }
}

