using System;
using System.Collections.Generic;
using com.gt;
using com.gt.events;
using com.gt.units;
using com.gt.mpnet.controllers;
using com.gt.mpnet.requests;
using com.gt.entities;

namespace com.gt.mpnet
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MessageTransmitter 
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly static IMPObject nullparameters = new MPObject();
        /// <summary>
        /// 
        /// </summary>
        protected MPNetClient mpnet;
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
        /// <param name="prefabConnecterId"></param>
        public MessageTransmitter(int prefabConnecterId) 
        {
            this.Log = new Logger(typeof(MessageTransmitter));
            this.m_PrefabConnecterId = prefabConnecterId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpnet"></param>
        internal void SetMPNetClient(MPNetClient mpnet)
        {
            this.mpnet = mpnet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        protected void DispatchEvent(BaseEvent evt)
        {
            GTLib.NetManager.DispatchEvent(evt);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        protected void Send(IRequest request)
        {
            if (mpnet == null)
                throw new ArgumentException("Net connecter is null,it can't be send message.");
            mpnet.Send(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extCmd"></param>
        protected void SendExtensionRequest(string extCmd)
        {
            SendExtensionRequest(extCmd, nullparameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extCmd"></param>
        /// <param name="parameters"></param>
        protected void SendExtensionRequest(string extCmd, IMPObject parameters)
        {
            SendExtensionRequest(extCmd, parameters, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extCmd"></param>
        /// <param name="parameters"></param>
        /// <param name="useUDP"></param>
        protected void SendExtensionRequest(string extCmd, IMPObject parameters, bool useUDP)
        {
            ExtensionRequest request = new ExtensionRequest(extCmd, parameters, useUDP);
            Send(request);
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
