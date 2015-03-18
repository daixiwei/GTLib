namespace com.gt.entities
{


    using com.gt.units;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class MPArray : IMPArray, ICollection, IEnumerable
    {
        private List<MPDataWrapper> dataHolder = new List<MPDataWrapper>();

        public void Add(MPDataWrapper wrappedObject)
        {
            dataHolder.Add(wrappedObject);
        }

        public void AddBool(bool val)
        {
            AddObject(val, MPDataType.BOOL);
        }

        public void AddByte(byte val)
        {
            AddObject(val, MPDataType.BYTE);
        }

        public void AddByteArray(ByteArray val)
        {
            AddObject(val, MPDataType.BYTE_ARRAY);
        }

        public void AddDouble(double val)
        {
            AddObject(val, MPDataType.DOUBLE);
        }

        public void AddFloat(float val)
        {
            AddObject(val, MPDataType.FLOAT);
        }

        public void AddInt(int val)
        {
            AddObject(val, MPDataType.INT);
        }

        public void AddLong(long val)
        {
            AddObject(val, MPDataType.LONG);
        }

        public void AddNull()
        {
            AddObject(null, MPDataType.NULL);
        }

        private void AddObject(object val, MPDataType tp)
        {
            Add(new MPDataWrapper((int) tp, val));
        }

        public void AddMPArray(IMPArray val)
        {
            AddObject(val, MPDataType.MP_ARRAY);
        }

        public void AddMPObject(IMPObject val)
        {
            AddObject(val, MPDataType.MP_OBJECT);
        }

        public void AddShort(short val)
        {
            AddObject(val, MPDataType.SHORT);
        }

        public void AddUtfString(string val)
        {
            AddObject(val, MPDataType.UTF_STRING);
        }

        public void AddClass(object val)
        {
            AddObject(val, MPDataType.CLASS);
        }

        public bool Contains(object obj)
        {
            if ((obj is IMPArray) || (obj is IMPObject))
            {
                throw new Exception("ISFSArray and ISFSObject are not supported by this method.");
            }
            for (int i = 0; i < Size(); i++)
            {
                if (object.Equals(GetElementAt(i), obj))
                {
                    return true;
                }
            }
            return false;
        }

        private ICollection GetArray(int index)
        {
            return GetValue<ICollection>(index);
        }

        public bool GetBool(int index)
        {
            return GetValue<bool>(index);
        }

        public byte GetByte(int index)
        {
            return GetValue<byte>(index);
        }

        public ByteArray GetByteArray(int index)
        {
            return GetValue<ByteArray>(index);
        }


        public double GetDouble(int index)
        {
            return GetValue<double>(index);
        }

        public object GetElementAt(int index)
        {
            object data = null;
            if (dataHolder[index] != null)
            {
                data = dataHolder[index].Data;
            }
            return data;
        }

        public float GetFloat(int index)
        {
            return GetValue<float>(index);
        }

        public int GetInt(int index)
        {
            return GetValue<int>(index);
        }


        public long GetLong(int index)
        {
            return GetValue<long>(index);
        }

        public IMPArray GetMPArray(int index)
        {
            return GetValue<IMPArray>(index);
        }

        public IMPObject GetMPObject(int index)
        {
            return GetValue<IMPObject>(index);
        }

        public short GetShort(int index)
        {
            return GetValue<short>(index);
        }

        public string GetUtfString(int index)
        {
            return GetValue<string>(index);
        }

        public object GetClass(int index)
        {
            return GetValue<object>(index);
        }

        public T GetValue<T>(int index)
        {
            if (index >= dataHolder.Count)
            {
                return default(T);
            }
            MPDataWrapper wrapper = dataHolder[index];
            return (T) wrapper.Data;
        }

        public MPDataWrapper GetWrappedElementAt(int index)
        {
            return dataHolder[index];
        }

        public bool IsNull(int index)
        {
            if (index >= dataHolder.Count)
            {
                return true;
            }
            MPDataWrapper wrapper = dataHolder[index];
            return (wrapper.Type == 0);
        }

        public static MPArray NewFromArray(List<MPDataWrapper> o)
        {
            return null;
        }

        public static MPArray NewFromBinaryData(ByteArray ba)
        {
            return MPDataSerializer.Instance.Binary2Array(ba) as MPArray;
        }

        public static MPArray NewInstance()
        {
            return new MPArray();
        }

        public object RemoveElementAt(int index)
        {
            if (index >= dataHolder.Count)
            {
                return null;
            }
            MPDataWrapper wrapper = dataHolder[index];
            dataHolder.RemoveAt(index);
            return wrapper.Data;
        }

        public int Size()
        {
            return dataHolder.Count;
        }

        void ICollection.CopyTo(Array toArray, int index)
        {
            foreach (MPDataWrapper wrapper in dataHolder)
            {
                toArray.SetValue(wrapper, index);
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MPArrayEnumerator(dataHolder);
        }

        public ByteArray ToBinary()
        {
            return MPDataSerializer.Instance.Array2Binary(this);
        }

        int ICollection.Count
        {
            get
            {
                return dataHolder.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }
    }
}

