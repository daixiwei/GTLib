namespace com.gt.entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MPDataWrapper
    {
        private object data;
        private int type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="data"></param>
        public MPDataWrapper(MPDataType tp, object data)
        {
            type = (int) tp;
            this.data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public MPDataWrapper(int type, object data)
        {
            this.type = type;
            this.data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Type
        {
            get
            {
                return type;
            }
        }
    }
}

