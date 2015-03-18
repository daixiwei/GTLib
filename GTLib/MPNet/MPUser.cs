using com.gt.entities;
using System;
using System.Collections.Generic;

namespace com.gt.mpnet
{
    /// <summary>
    /// 
    /// </summary>
    public class MPUser
    {
        protected int id;
        protected string name;
        protected int privilegeId;

        public MPUser(int id, string name)
        {
            this.id = -1;
            this.privilegeId = 0;
            this.Init(id, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mpa"></param>
        /// <returns></returns>
        public static MPUser FromSFSArray(IMPArray mpa)
        {
            MPUser user = new MPUser(mpa.GetInt(0), mpa.GetUtfString(1))
            {
                PrivilegeId = mpa.GetShort(2)
            };
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPlayerId()
        {
            int num = 0;
            return num;
        }

        private void Init(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString()
        {
            return string.Concat("[User: ", this.name, ", Id: ", this.id, "]");
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                return (this.PlayerId > 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PlayerId
        {
            get
            {
                return this.GetPlayerId();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PrivilegeId
        {
            get
            {
                return this.privilegeId;
            }
            set
            {
                this.privilegeId = value;
            }
        }
    }
}
