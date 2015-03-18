using com.gt.entities;
using com.gt.mpnet.bitswarm;
using System;
using System.Collections.Generic;

namespace com.gt.mpnet.requests
{

    /// <summary>
    /// 
    /// </summary>
    public class BaseRequest : IRequest
    {
        private int id;
        private bool isEncrypted;
        public static readonly string KEY_ERROR_CODE = "ec";
        public static readonly string KEY_ERROR_PARAMS = "ep";
        protected IMPObject mpo;
        protected int targetController;

        public BaseRequest(RequestType tp)
        {
            mpo = MPObject.NewInstance();
            targetController = 0;
            isEncrypted = false;
            id = (int)tp;
        }

        public BaseRequest(int id)
        {
            mpo = MPObject.NewInstance();
            targetController = 0;
            isEncrypted = false;
            this.id = id;
        }

        public virtual void Execute(MPNetClient mpnet)
        {
        }

        public virtual void Validate(MPNetClient mpnet)
        {
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public bool IsEncrypted
        {
            get
            {
                return isEncrypted;
            }
            set
            {
                isEncrypted = value;
            }
        }

        public IMessage Message
        {
            get
            {
                IMessage message = new Message
                {
                    Id = this.id,
                    IsEncrypted = this.isEncrypted,
                    TargetController = this.targetController,
                    Content = this.mpo
                };
                if (this is ExtensionRequest)
                {
                    message.IsUDP = (this as ExtensionRequest).UseUDP;
                }
                return message;
            }
        }

        public int TargetController
        {
            get
            {
                return targetController;
            }
            set
            {
                targetController = value;
            }
        }

        public RequestType Type
        {
            get
            {
                return (RequestType)id;
            }
            set
            {
                id = (int)value;
            }
        }
    }
}
