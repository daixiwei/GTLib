using com.gt.entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace com.gt.mpnet.requests
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginRequest : BaseRequest
    {
        public static readonly string KEY_ID = "id";
        public static readonly string KEY_PARAMS = "p";
        public static readonly string KEY_PASSWORD = "pw";
        public static readonly string KEY_PRIVILEGE_ID = "pi";
        public static readonly string KEY_RECONNECTION_SECONDS = "rs";
        public static readonly string KEY_USER_NAME = "un";
        private IMPObject parameters;
        private string password;
        private string userName;

        public LoginRequest(string userName)
            : base(RequestType.Login)
        {
            Init(userName, null, null);
        }

        public LoginRequest(string userName, string password)
            : base(RequestType.Login)
        {
            Init(userName, password, null);
        }


        public LoginRequest(string userName, string password, IMPObject parameters)
            : base(RequestType.Login)
        {
            Init(userName, password, parameters);
        }

        public override void Execute(MPNetClient mpnet)
        {
            mpo.PutUtfString(KEY_USER_NAME, this.userName);
            if (password.Length > 0)
            {
                password = MD5Hash(password);
            }
            mpo.PutUtfString(KEY_PASSWORD, this.password);
            if (this.parameters != null)
            {
                mpo.PutMPObject(KEY_PARAMS, this.parameters);
            }
        }

        private void Init(string userName, string password, IMPObject parameters)
        {
            this.userName = userName;
            this.password = (password != null) ? password : "";
            this.parameters = parameters;
        }

        private string MD5Hash(string instr)
        {
            StringBuilder builder = new StringBuilder(string.Empty);
            foreach (byte num2 in new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(instr)))
            {
                builder.Append(num2.ToString("x2"));
            }
            return builder.ToString();
        }

        public override void Validate(MPNetClient mpnet)
        {
            if (mpnet.MySelf != null)
            {
                throw new Exception("LoginRequest Error You are already logged in. Logout first");
            }
        }
    }
}
