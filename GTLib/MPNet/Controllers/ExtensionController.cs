
using com.gt.entities;
using com.gt.mpnet.bitswarm;
using com.gt.mpnet.core;
using System.Collections;

namespace com.gt.mpnet.controllers
{

    /// <summary>
    /// 
    /// </summary>
    public class ExtensionController : BaseController
    {
        public static readonly string KEY_CMD = "c";
        public static readonly string KEY_PARAMS = "p";

        public ExtensionController(BitSwarmClient bitSwarm)
            : base(bitSwarm)
        {
        }

        public override void HandleMessage(IMessage message)
        {
            if (mpnet.Debug)
            {
                log.Info(message.ToString());
            }
            IMPObject content = message.Content;
            //Hashtable data = new Hashtable();
            //data["cmd"] = content.GetUtfString(KEY_CMD);
            //data["params"] = content.GetMPObject(KEY_PARAMS);
            //if (message.IsUDP)
            //{
                //data["packetId"] = message.PacketId;
            //}
            //MPEvent evt = new MPEvent(MPEvent.EXTENSION_RESPONSE, data);
            string cmd = content.GetUtfString(KEY_CMD);
            IMPObject parameters = content.GetMPObject(KEY_PARAMS);
            if (message.IsUDP)
            {
                parameters.PutLong("packetId",message.PacketId);
            }
            mpnet.HandleExtension(cmd, parameters);
            //mpnet.DispatchEvent(new MPEvent(MPEvent.EXTENSION_RESPONSE, data));
        }
    }
}
