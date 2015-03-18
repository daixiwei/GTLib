namespace com.gt.entities
{
    using com.gt.units;
    using System;
    using System.Collections;

    /// <summary>
    /// The MPArray class.
    /// </summary>
    public interface IMPArray : ICollection, IEnumerable
    {
        /// <summary>
        /// The add data wrapper object.
        /// </summary>
        /// <param name="val"></param>
        void Add(MPDataWrapper val);
        /// <summary>
        /// The add bool
        /// </summary>
        /// <param name="val"></param>
        void AddBool(bool val);
        /// <summary>
        /// The add byte
        /// </summary>
        /// <param name="val"></param>
        void AddByte(byte val);
        /// <summary>
        /// The add byte array.
        /// </summary>
        /// <param name="val"></param>
        void AddByteArray(ByteArray val);
        /// <summary>
        /// The add double.
        /// </summary>
        /// <param name="val"></param>
        void AddDouble(double val);
        /// <summary>
        /// The add float.
        /// </summary>
        /// <param name="val"></param>
        void AddFloat(float val);
        /// <summary>
        /// The add int.
        /// </summary>
        /// <param name="val"></param>
        void AddInt(int val);
        /// <summary>
        /// The add long.
        /// </summary>
        /// <param name="val"></param>
        void AddLong(long val);
        /// <summary>
        /// The add null.
        /// </summary>
        void AddNull();
        /// <summary>
        /// The add MPArray.
        /// </summary>
        /// <param name="val"></param>
        void AddMPArray(IMPArray val);
        /// <summary>
        /// The add MPObject.
        /// </summary>
        /// <param name="val"></param>
        void AddMPObject(IMPObject val);
        /// <summary>
        /// The add short.
        /// </summary>
        /// <param name="val"></param>
        void AddShort(short val);
        /// <summary>
        /// The add string.
        /// </summary>
        /// <param name="val"></param>
        void AddUtfString(string val);
        /// <summary>
        /// The add class.
        /// </summary>
        /// <param name="val"></param>
        void AddClass(object val);
        /// <summary>
        /// The contains object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Contains(object obj);
        /// <summary>
        /// Get bool of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool GetBool(int index);
        /// <summary>
        /// Get byte of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        byte GetByte(int index);
        /// <summary>
        /// Get byte array of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ByteArray GetByteArray(int index);
        /// <summary>
        /// Get double of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double GetDouble(int index);
        /// <summary>
        /// Get object of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object GetElementAt(int index);
        /// <summary>
        /// Get float of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        float GetFloat(int index);
        /// <summary>
        /// Get int of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetInt(int index);
        /// <summary>
        /// Get long of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        long GetLong(int index);
        /// <summary>
        /// Get MPArray of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMPArray GetMPArray(int index);
        /// <summary>
        /// Get MPObject of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMPObject GetMPObject(int index);
        /// <summary>
        /// Get short of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        short GetShort(int index);
        /// <summary>
        /// Get string of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetUtfString(int index);
        /// <summary>
        /// Get class object of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object GetClass(int index);
        /// <summary>
        /// Get MPDataWrapper of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        MPDataWrapper GetWrappedElementAt(int index);
        /// <summary>
        /// Check null object of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsNull(int index);
        /// <summary>
        /// Remove element object of index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object RemoveElementAt(int index);
        /// <summary>
        /// The Array size.
        /// </summary>
        /// <returns></returns>
        int Size();
        /// <summary>
        /// To byte array
        /// </summary>
        /// <returns></returns>
        ByteArray ToBinary();
    }
}

