namespace com.gt.entities
{
    using com.gt.units;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class MPDataSerializer
    {
        private static readonly string CLASS_FIELDS_KEY = "$F";
        private static readonly string CLASS_MARKER_KEY = "$C";
        private static readonly string FIELD_NAME_KEY = "N";
        private static readonly string FIELD_VALUE_KEY = "V";
        private static MPDataSerializer instance;
        internal static MPDataSerializer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MPDataSerializer();
                }
                return instance;
            }
        }

        private MPDataSerializer()
        {
        }
        
        internal ByteArray Array2Binary(IMPArray array)
        {
            ByteArray buffer = new ByteArray();
            buffer.WriteByte(Convert.ToByte(MPDataType.MP_ARRAY));
            buffer.WriteShort(Convert.ToInt16(array.Size()));
            return Arr2bin(array, buffer);
        }

        internal ByteArray Object2Binary(IMPObject obj)
        {
            ByteArray buffer = new ByteArray();
            buffer.WriteByte(Convert.ToByte(MPDataType.MP_OBJECT));
            buffer.WriteShort(Convert.ToInt16(obj.Size()));
            return Obj2bin(obj, buffer);
        }

        internal IMPArray Binary2Array(ByteArray data)
        {
            if (data.Length < 3)
            {
                throw new Exception("Can't decode an MPArray. Byte data is insufficient. Size: " + data.Length + " byte(s)");
            }
            data.Position = 0;
            return DecodeMPArray(data);
        }

        internal IMPObject Binary2Object(ByteArray data)
        {
            if (data.Length < 3)
            {
                throw new Exception("Can't decode an MPObject. Byte data is insufficient. Size: " + data.Length + " byte(s)");
            }
            data.Position = 0;
            return DecodeMPObject(data);
        }

        private ByteArray AddData(ByteArray buffer, ByteArray newData)
        {
            buffer.WriteBytes(newData.Bytes);
            return buffer;
        }

        private ByteArray Arr2bin(IMPArray array, ByteArray buffer)
        {
            for (int i = 0; i < array.Size(); i++)
            {
                MPDataWrapper wrappedElementAt = array.GetWrappedElementAt(i);
                buffer = EncodeObject(buffer, wrappedElementAt.Type, wrappedElementAt.Data);
            }
            return buffer;
        }

        private MPDataWrapper BinDecode_BOOL(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.BOOL, buffer.ReadBool());
        }

        private MPDataWrapper BinDecode_BYTE(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.BYTE, buffer.ReadByte());
        }

        private MPDataWrapper BinDecode_BYTE_ARRAY(ByteArray buffer)
        {
            int count = buffer.ReadInt();
            if (count < 0)
            {
                throw new Exception("Array negative size: " + count);
            }
            return new MPDataWrapper(MPDataType.BYTE_ARRAY, new ByteArray(buffer.ReadBytes(count)));
        }

        private MPDataWrapper BinDecode_DOUBLE(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.DOUBLE, buffer.ReadDouble());
        }

        private MPDataWrapper BinDecode_FLOAT(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.FLOAT, buffer.ReadFloat());
        }

        private MPDataWrapper BinDecode_INT(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.INT, buffer.ReadInt());
        }

        private MPDataWrapper BinDecode_LONG(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.LONG, buffer.ReadLong());
        }

        private MPDataWrapper BinDecode_NULL(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.NULL, null);
        }

        private MPDataWrapper BinDecode_SHORT(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.SHORT, buffer.ReadShort());
        }

        private MPDataWrapper BinDecode_UTF_STRING(ByteArray buffer)
        {
            return new MPDataWrapper(MPDataType.UTF_STRING, buffer.ReadUTF());
        }

        private ByteArray BinEncode_BOOL(ByteArray buffer, bool val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.BOOL);
            newData.WriteBool(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_BYTE(ByteArray buffer, byte val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.BYTE);
            newData.WriteByte(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_BYTE_ARRAY(ByteArray buffer, ByteArray val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.BYTE_ARRAY);
            newData.WriteInt(val.Length);
            newData.WriteBytes(val.Bytes);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_DOUBLE(ByteArray buffer, double val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.DOUBLE);
            newData.WriteDouble(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_FLOAT(ByteArray buffer, float val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.FLOAT);
            newData.WriteFloat(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_INT(ByteArray buffer, double val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.DOUBLE);
            newData.WriteDouble(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_INT(ByteArray buffer, int val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.INT);
            newData.WriteInt(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_LONG(ByteArray buffer, long val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.LONG);
            newData.WriteLong(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_NULL(ByteArray buffer)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.NULL);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_SHORT(ByteArray buffer, short val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.SHORT);
            newData.WriteShort(val);
            return AddData(buffer, newData);
        }

        private ByteArray BinEncode_UTF_STRING(ByteArray buffer, string val)
        {
            ByteArray newData = new ByteArray();
            newData.WriteByte(MPDataType.UTF_STRING);
            newData.WriteUTF(val);
            return AddData(buffer, newData);
        }

        private MPDataWrapper DecodeObject(ByteArray buffer)
        {
            MPDataType type = (MPDataType)Convert.ToInt32(buffer.ReadByte());
            switch (type)
            {
                case MPDataType.NULL:
                    return BinDecode_NULL(buffer);
                case MPDataType.BOOL:
                    return BinDecode_BOOL(buffer);
                case MPDataType.BYTE:
                    return BinDecode_BYTE(buffer);
                case MPDataType.BYTE_ARRAY:
                    return BinDecode_BYTE_ARRAY(buffer);
                case MPDataType.SHORT:
                    return BinDecode_SHORT(buffer);
                case MPDataType.INT:
                    return BinDecode_INT(buffer);
                case MPDataType.LONG:
                    return BinDecode_LONG(buffer);
                case MPDataType.FLOAT:
                    return BinDecode_FLOAT(buffer);
                case MPDataType.DOUBLE:
                    return BinDecode_DOUBLE(buffer);
                case MPDataType.UTF_STRING:
                    return BinDecode_UTF_STRING(buffer);
                case MPDataType.MP_ARRAY:
                    buffer.Position--;
                    return new MPDataWrapper(MPDataType.MP_ARRAY, DecodeMPArray(buffer));
            }
            if (type != MPDataType.MP_OBJECT)
            {
                throw new Exception("Unknow MPDataType ID: " + type);
            }
            buffer.Position--;
            IMPObject mpObj = DecodeMPObject(buffer);
            byte num = Convert.ToByte(MPDataType.MP_OBJECT);
            object data = mpObj;
            if (mpObj.ContainsKey(CLASS_MARKER_KEY) && mpObj.ContainsKey(CLASS_FIELDS_KEY))
            {
                num = Convert.ToByte(MPDataType.CLASS);
                data = Mp2Cs(mpObj);
            }
            return new MPDataWrapper(num, data);
        }

        private IMPArray DecodeMPArray(ByteArray buffer)
        {
            IMPArray array = MPArray.NewInstance();
            MPDataType type = (MPDataType)Convert.ToInt32(buffer.ReadByte());
            if (type != MPDataType.MP_ARRAY)
            {
                throw new Exception(string.Concat("Invalid MPDataType. Expected: ", MPDataType.MP_ARRAY, ", found: ", type));
            }
            int num = buffer.ReadShort();
            if (num < 0)
            {
                throw new Exception("Can't decode MPArray. Size is negative: " + num);
            }
            for (int i = 0; i < num; i++)
            {
                MPDataWrapper val = DecodeObject(buffer);
                if (val == null)
                {
                    throw new Exception("Could not decode SFSArray item at index: " + i);
                }
                array.Add(val);
            }
            return array;
        }

        private IMPObject DecodeMPObject(ByteArray buffer)
        {
            MPObject obj2 = MPObject.NewInstance();
            byte num = buffer.ReadByte();
            if (num != Convert.ToByte(MPDataType.MP_OBJECT))
            {
                throw new Exception(string.Concat("Invalid MPDataType. Expected: ", MPDataType.MP_OBJECT, ", found: ", num));
            }
            int num2 = buffer.ReadShort();
            if (num2 < 0)
            {
                throw new Exception("Can't decode MPObject. Size is negative: " + num2);
            }
            for (int i = 0; i < num2; i++)
            {
                string key = buffer.ReadUTF();
                MPDataWrapper val = DecodeObject(buffer);
                if (val == null)
                {
                    throw new Exception("Could not decode value for MPObject with key: " + key);
                }
                obj2.Put(key, val);
            }
            return obj2;
        }

        private ByteArray EncodeObject(ByteArray buffer, int typeId, object data)
        {
            switch (((MPDataType)typeId))
            {
                case MPDataType.NULL:
                    buffer = BinEncode_NULL(buffer);
                    return buffer;
                case MPDataType.BOOL:
                    buffer = BinEncode_BOOL(buffer, (bool)data);
                    return buffer;
                case MPDataType.BYTE:
                    buffer = BinEncode_BYTE(buffer, (byte)data);
                    return buffer;
                case MPDataType.SHORT:
                    buffer = BinEncode_SHORT(buffer, (short)data);
                    return buffer;
                case MPDataType.INT:
                    buffer = BinEncode_INT(buffer, (int)data);
                    return buffer;
                case MPDataType.LONG:
                    buffer = BinEncode_LONG(buffer, (long)data);
                    return buffer;
                case MPDataType.FLOAT:
                    buffer = BinEncode_FLOAT(buffer, (float)data);
                    return buffer;
                case MPDataType.DOUBLE:
                    buffer = BinEncode_DOUBLE(buffer, (double)data);
                    return buffer;
                case MPDataType.UTF_STRING:
                    buffer = BinEncode_UTF_STRING(buffer, (string)data);
                    return buffer;
                case MPDataType.MP_ARRAY:
                    buffer = AddData(buffer, Array2Binary((IMPArray)data));
                    return buffer;
                case MPDataType.BYTE_ARRAY:
                    buffer = BinEncode_BYTE_ARRAY(buffer, (ByteArray)data);
                    return buffer;
                case MPDataType.MP_OBJECT:
                    buffer = AddData(buffer, Object2Binary((MPObject)data));
                    return buffer;
                case MPDataType.CLASS:
                    buffer = AddData(buffer, Object2Binary(Cs2Mp(data)));
                    return buffer;
            }
            throw new Exception("Unrecognized type in MPObject serialization: " + typeId);
        }

        private ByteArray EncodeMPObjectKey(ByteArray buffer, string val)
        {
            buffer.WriteUTF(val);
            return buffer;
        }

        private int GetTypedArraySize(ByteArray buffer)
        {
            short num = buffer.ReadShort();
            if (num < 0)
            {
                throw new Exception("Array negative size: " + num);
            }
            return num;
        }

        private ByteArray Obj2bin(IMPObject obj, ByteArray buffer)
        {
            foreach (string str in obj.GetKeys())
            {
                MPDataWrapper data = obj.GetData(str);
                buffer = EncodeMPObjectKey(buffer, str);
                buffer = EncodeObject(buffer, data.Type, data.Data);
            }
            return buffer;
        }

        

        //----------------------------------  Class Mapping  -------------------------------------
        //*********************************** Class to MPObject  *********************************
        private IMPObject Cs2Mp(object csObj)
        {
            IMPObject sfsObj = MPObject.NewInstance();
            ConvertCsObj(csObj, sfsObj);
            return sfsObj;
        }

        private void ConvertCsObj(object csObj, IMPObject mpObj)
        {
            Type type = csObj.GetType();
            string fullName = type.FullName;
            if (!(mpObj is Serializable))
            {
                throw new Exception(string.Concat("Cannot serialize object: ", csObj, ", type: ", fullName, " -- It doesn't implement the SerializableSFSType interface"));
            }
            IMPArray val = MPArray.NewInstance();
            mpObj.PutUtfString(CLASS_MARKER_KEY, fullName);
            mpObj.PutMPArray(CLASS_FIELDS_KEY, val);
            foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                string name = info.Name;
                object obj2 = info.GetValue(csObj);
                IMPObject obj3 = MPObject.NewInstance();
                MPDataWrapper wrapper = WrapField(obj2);
                if (wrapper != null)
                {
                    obj3.PutUtfString(FIELD_NAME_KEY, name);
                    obj3.Put(FIELD_VALUE_KEY, wrapper);
                    val.AddMPObject(obj3);
                }
                else
                {
                    throw new Exception(string.Concat("Cannot serialize field of object: ", csObj, ", field: ", name, ", type: ", info.GetType().Name, " -- unsupported type!"));
                }
            }
        }

        private MPDataWrapper WrapField(object val)
        {
            if (val == null)
            {
                return new MPDataWrapper(MPDataType.NULL, null);
            }
            MPDataWrapper wrapper = null;
            if (val is bool)
            {
                return new MPDataWrapper(MPDataType.BOOL, val);
            }
            if (val is int)
            {
                return new MPDataWrapper(MPDataType.INT, val);
            }
            if (val is double)
            {
                return new MPDataWrapper(MPDataType.DOUBLE, val);
            }
            if (val is string)
            {
                return new MPDataWrapper(MPDataType.UTF_STRING, val);
            }
            if (val is Serializable)
            {
                return new MPDataWrapper(MPDataType.MP_OBJECT, Cs2Mp(val));
            }
            if (val is IDictionary)
            {
                wrapper = new MPDataWrapper(MPDataType.MP_OBJECT, UnrollDictionary(val as IDictionary));
            }
            if (val.GetType().IsArray)
            {
                wrapper = new MPDataWrapper(MPDataType.MP_ARRAY, UnrollArray((object[])val));
            }
            if (val is ICollection)
            {
                wrapper = new MPDataWrapper(MPDataType.MP_ARRAY, UnrollCollection(val as ICollection));
            }
            if (val is IMPObject)
            {
                wrapper = new MPDataWrapper(MPDataType.MP_OBJECT, val);
            }
            if (val is IMPArray)
            {
                wrapper = new MPDataWrapper(MPDataType.MP_ARRAY, val);
            }
            return wrapper;
        }

        private IMPArray UnrollArray(object[] arr)
        {
            IMPArray array = MPArray.NewInstance();
            foreach (object item in arr)
            {
                array.Add(WrapField(item));
            }
            return array;
        }

        private IMPArray UnrollCollection(ICollection collection)
        {
            IMPArray array = MPArray.NewInstance();

            foreach (object item in collection)
            {
                array.Add(WrapField(item));
            }
            return array;
        }

        private IMPObject UnrollDictionary(IDictionary dict)
        {
            IMPObject obj2 = MPObject.NewInstance();
            IEnumerator enumerator = dict.Keys.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = (string)enumerator.Current;
                    MPDataWrapper val = WrapField(dict[current]);
                    if (val == null)
                    {
                        throw new Exception(string.Concat("Cannot serialize field of dictionary with key: ", current, ", ", dict[current], " -- unsupported type!"));
                    }
                    obj2.Put(current, val);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return obj2;
        }

        //*********************************** MPObject to Class *********************************
        private object Mp2Cs(IMPObject mpObj)
        {
            if (!mpObj.ContainsKey(CLASS_MARKER_KEY) || !mpObj.ContainsKey(CLASS_FIELDS_KEY))
            {
                throw new Exception("The MPObject passed does not represent any serialized class.");
            }
            string utfString = mpObj.GetUtfString(CLASS_MARKER_KEY);
            Type type = Type.GetType(utfString);

            if (type == null)
            {
                throw new Exception("Cannot find type: " + utfString);
            }
            object csObj = Activator.CreateInstance(type);
            if (!(csObj is Serializable))
            {
                throw new Exception(string.Concat("Cannot deserialize object: ", csObj, ", type: ", utfString, " -- It doesn't implement the Serializable interface"));
            }
            ConvertMPObject(mpObj.GetMPArray(CLASS_FIELDS_KEY), csObj, type);
            return csObj;
        }

        private void ConvertMPObject(IMPArray fieldList, object csObj, Type objType)
        {
            for (int i = 0; i < fieldList.Size(); i++)
            {
                IMPObject sFSObject = fieldList.GetMPObject(i);
                string utfString = sFSObject.GetUtfString(FIELD_NAME_KEY);
                MPDataWrapper data = sFSObject.GetData(FIELD_VALUE_KEY);
                object obj3 = UnwrapField(data);
                FieldInfo field = objType.GetField(utfString, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field == null)
                {
                    throw new Exception(string.Format("The deserialized class ({0}) doesn't contain the field: {1}", objType.FullName, utfString));
                }
                field.SetValue(csObj, obj3);
            }
        }

        private object UnwrapField(MPDataWrapper wrapper)
        {
            object obj2 = null;
            int type = wrapper.Type;
            if (type <= (int)MPDataType.UTF_STRING)
            {
                return wrapper.Data;
            }
            switch (type)
            {
                case (int)MPDataType.MP_ARRAY:
                    return RebuildArray(wrapper.Data as IMPArray);

                case (int)MPDataType.MP_OBJECT:
                    {
                        IMPObject data = wrapper.Data as IMPObject;
                        if (data.ContainsKey(CLASS_MARKER_KEY) && data.ContainsKey(CLASS_FIELDS_KEY))
                        {
                            return Cs2Mp(data);
                        }
                        return RebuildDict(wrapper.Data as IMPObject);
                    }
                case (int)MPDataType.CLASS:
                    obj2 = wrapper.Data;
                    break;
            }
            return obj2;
        }

        private ArrayList RebuildArray(IMPArray mpArr)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < mpArr.Size(); i++)
            {
                list.Add(UnwrapField(mpArr.GetWrappedElementAt(i)));
            }
            return list;
        }

        private Hashtable RebuildDict(IMPObject mpObj)
        {
            Hashtable hashtable = new Hashtable();
            foreach (string str in mpObj.GetKeys())
            {
                hashtable[str] = UnwrapField(mpObj.GetData(str));
            }
            return hashtable;
        }

    }
}

