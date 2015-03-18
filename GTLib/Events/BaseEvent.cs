namespace com.gt.events
{
    using System;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class BaseEvent
    {
        /// <summary>
        /// 
        /// </summary>
        protected Hashtable arguments;
        /// <summary>
        /// 
        /// </summary>
        protected object target;
        /// <summary>
        /// 
        /// </summary>
        protected string type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public BaseEvent(string type)
        {
            this.type = type;
            if (arguments == null)
            {
                arguments = new Hashtable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public BaseEvent(string type, Hashtable args)
        {
            this.type = type;
            arguments = args;
            if (arguments == null)
            {
                arguments = new Hashtable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BaseEvent Clone()
        {
            return new BaseEvent(type, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return type + " [ " + (target == null ? "null" : target.ToString()) + "]";
        }

        /// <summary>
        /// 
        /// </summary>
        public Hashtable Params
        {
            get
            {
                return arguments;
            }
            set
            {
                arguments = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        /// <summary>
        /// The type get/set
        /// </summary>
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
    }
}

