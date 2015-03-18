using com.gt.units;

namespace com.gt.entities
{
    

    /// <summary>
    /// The MPObject class.
    /// </summary>
    public interface IMPObject
    {
        /// <summary>
        /// The contains key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);
        /// <summary>
        /// Get bool of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetBool(string key);
        /// <summary>
        /// Get byte of key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte GetByte(string key);
        /// <summary>
        /// Get byte array of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ByteArray GetByteArray(string key);
        /// <summary>
        /// Get MPDataWrapper of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        MPDataWrapper GetData(string key);
        /// <summary>
        /// Get double of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        double GetDouble(string key);
        /// <summary>
        /// Get float of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        float GetFloat(string key);
        /// <summary>
        /// Get int of key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetInt(string key);
        /// <summary>
        /// Get all keys.
        /// </summary>
        /// <returns></returns>
        string[] GetKeys();
        /// <summary>
        /// Get long of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long GetLong(string key);
        /// <summary>
        /// Get MPArray of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IMPArray GetMPArray(string key);
        /// <summary>
        /// Get MPObject of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IMPObject GetMPObject(string key);
        /// <summary>
        /// Get short of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        short GetShort(string key);
        /// <summary>
        /// Get string of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetUtfString(string key);
        /// <summary>
        /// Get class object of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetClass(string key);
        /// <summary>
        /// Check null object of key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsNull(string key);
        /// <summary>
        /// Put MPDataWrapper to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void Put(string key, MPDataWrapper val);
        /// <summary>
        /// Put bool to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutBool(string key, bool val);
        /// <summary>
        /// Put byte to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutByte(string key, byte val);
        /// <summary>
        /// Put byte array to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutByteArray(string key, ByteArray val);
        /// <summary>
        /// Put double to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutDouble(string key, double val);
        /// <summary>
        /// Put float to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutFloat(string key, float val);
        /// <summary>
        /// Put int to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutInt(string key, int val);
        /// <summary>
        /// Put long to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutLong(string key, long val);
        /// <summary>
        /// Put null object to key.
        /// </summary>
        /// <param name="key"></param>
        void PutNull(string key);
        /// <summary>
        /// Put MPArray to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutMPArray(string key, IMPArray val);
        /// <summary>
        /// Put MPObject to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutMPObject(string key, IMPObject val);
        /// <summary>
        /// Put short to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutShort(string key, short val);
        /// <summary>
        /// Put string to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutUtfString(string key, string val);
        /// <summary>
        /// Put class object to key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void PutClass(string key, object val);
        /// <summary>
        /// Remove objecy of key.
        /// </summary>
        /// <param name="key"></param>
        void RemoveElement(string key);
        /// <summary>
        /// Get size.
        /// </summary>
        /// <returns></returns>
        int Size();
        /// <summary>
        /// The to byte array.
        /// </summary>
        /// <returns></returns>
        ByteArray ToBinary();
    }
}

