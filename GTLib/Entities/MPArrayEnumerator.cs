namespace com.gt.entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class MPArrayEnumerator : IEnumerator
    {
        private int cursorIndex;
        private List<MPDataWrapper> data;

        public MPArrayEnumerator(List<MPDataWrapper> data)
        {
            this.data = data;
            cursorIndex = -1;
        }

        bool IEnumerator.MoveNext()
        {
            if (cursorIndex < data.Count)
            {
                cursorIndex++;
            }
            return (cursorIndex != data.Count);
        }

        void IEnumerator.Reset()
        {
            cursorIndex = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                if ((cursorIndex < 0) || (cursorIndex == data.Count))
                {
                    throw new InvalidOperationException();
                }
                return data[cursorIndex].Data;
            }
        }
    }
}

