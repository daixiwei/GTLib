using System;
using System.Collections.Generic;
using com.gt.mpnet.requests;
using com.gt.mpnet;
using com.gt.mpnet.bitswarm;
using com.gt.units;
using com.gt.entities;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtensionRequest : BaseRequest
    {
        private string extCmd;
        public static readonly string KEY_CMD = "c";
        public static readonly string KEY_PARAMS = "p";
        private IMPObject parameters;
        private bool useUDP;

        public ExtensionRequest(string extCmd, IMPObject parameters)
            : base(RequestType.CallExtension)
        {
            this.Init(extCmd, parameters, false);
        }

        public ExtensionRequest(string extCmd, IMPObject parameters, bool useUDP)
            : base(RequestType.CallExtension)
        {
            this.Init(extCmd, parameters, useUDP);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sfs"></param>
        public override void Execute(MPNetClient sfs)
        {
            mpo.PutUtfString(KEY_CMD, extCmd);
            mpo.PutMPObject(KEY_PARAMS, parameters);
        }

        private void Init(string extCmd, IMPObject parameters, bool useUDP)
        {
            base.targetController = 1;
            this.extCmd = extCmd;
            this.parameters = parameters;
            this.useUDP = useUDP;
            if (parameters == null)
            {
                parameters = new MPObject();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sfs"></param>
        public override void Validate(MPNetClient sfs)
        {
            List<string> errors = new List<string>();
            if ((extCmd == null) || (extCmd.Length == 0))
            {
                errors.Add("Missing extension command");
            }
            if (parameters == null)
            {
                errors.Add("Missing extension parameters");
            }
            if (errors.Count > 0)
            {
                string msg = "";
                foreach (string err in errors)
                {
                    msg = msg + err;
                }
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseUDP
        {
            get
            {
                return useUDP;
            }
        }
    }
}
