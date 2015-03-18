namespace pumpkin.swf
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class SwfURI
    {
        /// <summary>
        /// 
        /// </summary>
        public object label;
        /// <summary>
        /// 
        /// </summary>
        public string linkage;
        /// <summary>
        /// 
        /// </summary>
        public string swf;

        /// <summary>
        /// 
        /// </summary>
        public SwfURI()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public SwfURI(string uri)
        {
            if (uri != null)
            {
                string[] strArray = uri.Split(":".ToCharArray());
                if (strArray.Length >= 1)
                {
                    this.swf = strArray[0];
                }
                if (strArray.Length >= 2)
                {
                    this.linkage = strArray[1];
                }
                if (strArray.Length >= 3)
                {
                    this.label = strArray[2];
                }
            }
        }
    }
}