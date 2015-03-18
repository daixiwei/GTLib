
namespace com.gt.units
{
    using com.gt.entities;
    using ComponentAce.Compression.Libs.zlib;
    using System;
    using System.IO;
    using System.Text;

    public class ByteArray
    {
        private byte[] buffer;
        private bool compressed;
        private int position;

        public ByteArray()
        {
            position = 0;
            compressed = false;
            buffer = new byte[0];
        }

        public ByteArray(byte[] buf)
        {
            position = 0;
            compressed = false;
            buffer = buf;
        }

        private void CheckCompressedRead()
        {
            if (compressed)
            {
                throw new Exception("Only raw bytes can be read from a compressed array.");
            }
        }

        private void CheckCompressedWrite()
        {
            if (compressed)
            {
                throw new Exception("Only raw bytes can be written a compressed array. Call Uncompress first.");
            }
        }

        public void Compress()
        {
            if (compressed)
            {
                throw new Exception("Buffer is already compressed");
            }
            MemoryStream stream = new MemoryStream();
            using (ZOutputStream stream2 = new ZOutputStream(stream, 9))
            {
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Flush();
            }
            buffer = stream.ToArray();
            position = 0;
            compressed = true;
        }

        public bool ReadBool()
        {
            CheckCompressedRead();
            return (buffer[position++] == 1);
        }

        public byte ReadByte()
        {
            CheckCompressedRead();
            return buffer[position++];
        }

        public byte[] ReadBytes(int count)
        {
            byte[] dst = new byte[count];
            Buffer.BlockCopy(buffer, position, dst, 0, count);
            position += count;
            return dst;
        }

        public double ReadDouble()
        {
            CheckCompressedRead();
            return BitConverter.ToDouble(ReverseOrder(ReadBytes(8)), 0);
        }

        public float ReadFloat()
        {
            CheckCompressedRead();
            return BitConverter.ToSingle(ReverseOrder(ReadBytes(4)), 0);
        }

        public int ReadInt()
        {
            CheckCompressedRead();
            return BitConverter.ToInt32(ReverseOrder(ReadBytes(4)), 0);
        }

        public long ReadLong()
        {
            CheckCompressedRead();
            return BitConverter.ToInt64(ReverseOrder(ReadBytes(8)), 0);
        }

        public short ReadShort()
        {
            CheckCompressedRead();
            return BitConverter.ToInt16(ReverseOrder(ReadBytes(2)), 0);
        }

        public ushort ReadUShort()
        {
            CheckCompressedRead();
            return BitConverter.ToUInt16(ReverseOrder(ReadBytes(2)), 0);
        }

        public string ReadUTF()
        {
            CheckCompressedRead();
            ushort count = ReadUShort();
            if (count == 0) return null;
            string str = Encoding.UTF8.GetString(buffer, position, count);
            position += count;
            return str;
        }

        public byte[] ReverseOrder(byte[] dt)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return dt;
            }
            byte[] buffer = new byte[dt.Length];
            int num = 0;
            for (int i = dt.Length - 1; i >= 0; i--)
            {
                buffer[num++] = dt[i];
            }
            return buffer;
        }

        public void Uncompress()
        {
            MemoryStream stream = new MemoryStream();
            using (ZOutputStream stream2 = new ZOutputStream(stream))
            {
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Flush();
            }
            buffer = stream.ToArray();
            position = 0;
            compressed = false;
        }

        public void WriteBool(bool b)
        {
            CheckCompressedWrite();
            byte[] data = new byte[] { !b ? ((byte)0) : ((byte)1) };
            WriteBytes(data);
        }

        public void WriteByte(MPDataType tp)
        {
            WriteByte(Convert.ToByte((int)tp));
        }

        public void WriteByte(byte b)
        {
            byte[] data = new byte[] { b };
            WriteBytes(data);
        }

        public void WriteBytes(byte[] data)
        {
            WriteBytes(data, 0, data.Length);
        }

        public void WriteBytes(byte[] data, int ofs, int count)
        {
            byte[] dst = new byte[count + buffer.Length];
            Buffer.BlockCopy(buffer, 0, dst, 0, buffer.Length);
            Buffer.BlockCopy(data, ofs, dst, buffer.Length, count);
            buffer = dst;
        }

        public void WriteDouble(double d)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(d);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteFloat(float f)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(f);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteInt(int i)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(i);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteLong(long l)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(l);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteShort(short s)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(s);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteUShort(ushort us)
        {
            CheckCompressedWrite();
            byte[] bytes = BitConverter.GetBytes(us);
            WriteBytes(ReverseOrder(bytes));
        }

        public void WriteUTF(string str)
        {
            CheckCompressedWrite();
            if (string.IsNullOrEmpty(str))
            {
                WriteUShort(0);
                return;
            }
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int num3 = str[i];
                if ((num3 >= 1) && (num3 <= 0x7f))
                {
                    num++;
                }
                else if (num3 > 0x7ff)
                {
                    num += 3;
                }
                else
                {
                    num += 2;
                }
            }
            if (num > 0x8000)
            {
                throw new FormatException("String length cannot be greater then 32768 !");
            }
            WriteUShort(Convert.ToUInt16(num));
            WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        public byte[] Bytes
        {
            get
            {
                return buffer;
            }
            set
            {
                buffer = value;
                compressed = false;
            }
        }

        public int BytesAvailable
        {
            get
            {
                int num = buffer.Length - position;
                if ((num <= buffer.Length) && (num >= 0))
                {
                    return num;
                }
                return 0;
            }
        }

        public bool Compressed
        {
            get
            {
                return compressed;
            }
            set
            {
                compressed = value;
            }
        }

        public int Length
        {
            get
            {
                return buffer.Length;
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}

