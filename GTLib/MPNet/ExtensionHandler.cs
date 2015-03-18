using com.gt.events;
using com.gt.units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.gt.mpnet
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MessageHandler
    {
        //protected MPNetClient mpnet;
        /// <summary>
        /// 
        /// </summary>
        protected Logger Log;
        /// <summary>
        /// 
        /// </summary>
        protected int m_PrefabConnecterId;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="netmp"></param>
        protected MessageHandler(int prefabConnecterId)
        {
            Log = new Logger(GetType());
            this.m_PrefabConnecterId = prefabConnecterId;
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Init();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="mpnet"></param>
        //internal void SetMPNetClient(MPNetClient mpnet)
        //{
        //    this.mpnet = mpnet;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        protected void DispatchEvent(BaseEvent evt)
        {
            //if (mpnet == null)
            //{
            //    throw new ArgumentNullException("MPNetClient is null!");
            //}
            GTLib.NetManager.DispatchEvent(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="messageHandler"></param>
        protected void RegisterMessageHandler(string cmd, ExtensionMessageDelegate messageHandler)
        {
            ((MPNetManager)GTLib.NetManager).AddMessageHandler(m_PrefabConnecterId, cmd, messageHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="messageHandler"></param>
        protected void UnRegisterMessageHandler(string cmd, ExtensionMessageDelegate messageHandler)
        {
            ((MPNetManager)GTLib.NetManager).RemoveMessageHandler(m_PrefabConnecterId, cmd, messageHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        internal int PrefabConnecterId
        {
            get
            {
                return m_PrefabConnecterId;
            }
        }
    }
}
