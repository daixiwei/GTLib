namespace com.gt.units
{
    using System;
    using System.Collections;

    public class XMLNode : Hashtable
    {
        public XMLNode GetNode(string path)
        {
            return (this.GetObject(path) as XMLNode);
        }

        public XMLNodeList GetNodeList(string path)
        {
            return (this.GetObject(path) as XMLNodeList);
        }

        private object GetObject(string path)
        {
            char[] separator = new char[] { '>' };
            string[] strArray = path.Split(separator);
            XMLNode node = this;
            XMLNodeList list = null;
            bool flag = false;
            for (int i = 0; i < strArray.Length; i++)
            {
                object obj2;
                if (flag)
                {
                    node = (XMLNode) list[int.Parse(strArray[i])];
                    obj2 = node;
                    flag = false;
                }
                else
                {
                    obj2 = node[strArray[i]];
                    if (obj2 is ArrayList)
                    {
                        list = (XMLNodeList) (obj2 as ArrayList);
                        flag = true;
                    }
                    else
                    {
                        if (i != (strArray.Length - 1))
                        {
                            string str = "";
                            for (int j = 0; j <= i; j++)
                            {
                                str = str + ">" + strArray[j];
                            }
                        }
                        return obj2;
                    }
                }
            }
            if (flag)
            {
                return list;
            }
            return node;
        }

        public string GetValue(string path)
        {
            return (this.GetObject(path) as string);
        }
    }
}

