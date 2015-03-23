using com.gt.mpnet.bitswarm;
using com.gt.mpnet.requests;
using System;
using System.Collections;
using System.Collections.Generic;
using com.gt.mpnet.core;
using com.gt.entities;

namespace com.gt.mpnet.controllers
{

    /// <summary>
    /// 
    /// </summary>
    public class SystemController : BaseController
    {
        private Dictionary<int, RequestDelegate> requestHandlers;

        public SystemController(BitSwarmClient bitSwarm)
            : base(bitSwarm)
        {
            this.requestHandlers = new Dictionary<int, RequestDelegate>();
            this.InitRequestHandlers();
        }

        private void FnClientDisconnection(IMessage msg)
        {
            int dr = msg.Content.GetByte("dr");
            mpnet.HandleClientDisconnection(ClientDisconnectionReason.GetReason(dr));
        }

        private void FnHandshake(IMessage msg)
        {
            mpnet.HandleHandShake(msg.Content);
        }

        private void FnLogin(IMessage msg)
        {
            IMPObject content = msg.Content;
            Hashtable data = new Hashtable();
            if (content.IsNull(BaseRequest.KEY_ERROR_CODE))
            {
                mpnet.MySelf = new MPUser(content.GetInt(LoginRequest.KEY_ID), content.GetUtfString(LoginRequest.KEY_USER_NAME));
                mpnet.MySelf.PrivilegeId = content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
                mpnet.SetReconnectionSeconds(content.GetShort(LoginRequest.KEY_RECONNECTION_SECONDS));
                mpnet.MySelf.PrivilegeId = content.GetShort(LoginRequest.KEY_PRIVILEGE_ID);
                data["user"] = mpnet.MySelf;
                data["data"] = content.GetMPObject(LoginRequest.KEY_PARAMS);
                MPEvent evt = new MPEvent(MPEvent.LOGIN, data);
                mpnet.DispatchEvent(evt);
            }
            else
            {
                short code = content.GetShort(BaseRequest.KEY_ERROR_CODE);
                data["errorCode"] = code;
                mpnet.DispatchEvent(new MPEvent(MPEvent.LOGIN_ERROR, data));
            }
        }

        private void FnLogout(IMessage msg)
        {
            mpnet.HandleLogout();
            IMPObject content = msg.Content;
            Hashtable data = new Hashtable();
            mpnet.DispatchEvent(new MPEvent(MPEvent.LOGOUT, data));
        }

        private void FnPingPong(IMessage msg)
        {
            int num = mpnet.LagMonitor.OnPingPong();
            Hashtable data = new Hashtable();
            data["lagValue"] = num;
            mpnet.DispatchEvent(new MPEvent(MPEvent.PING_PONG, data));
        }

        public override void HandleMessage(IMessage message)
        {
            if (mpnet.Debug)
            {
                log.Info("Message: ", ((RequestType)message.Id).ToString(), " ", message.ToString());
            }
            if (!requestHandlers.ContainsKey(message.Id))
            {
                log.Warn("Unknown message id: " + message.Id);
            }
            else
            {
                RequestDelegate delegate2 = requestHandlers[message.Id];
                delegate2(message);
            }
        }

        private void InitRequestHandlers()
        {
            requestHandlers[0] = new RequestDelegate(FnHandshake);
            requestHandlers[1] = new RequestDelegate(FnLogin);
            requestHandlers[2] = new RequestDelegate(FnLogout);
  
            requestHandlers[0x1d] = new RequestDelegate(FnPingPong);
            requestHandlers[0x3ed] = new RequestDelegate(FnClientDisconnection);
        }
    }
}
