using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Plc
{
    public class ConverterBase : IConverter
    {
        public ConverterBase() { }
        public ConverterBase(DataFormat dataFormat) { DataFormat = dataFormat; }//todo response data format handle

        public DataFormat DataFormat { get ; set; }//response data format

        public byte[] ToByteArray(double[] value)
        {
            byte[] by = new byte[value.Length * 8];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 8 * i);
            }
            return by;
        }

        public byte[] ToByteArray(float[] value)
        {
            byte[] by = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 4 * i);
            }
            return by;
        }

        public byte[] ToByteArray(short[] value)
        {
            byte[] by = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 2 * i);
            }
            return by;
        }

        public byte[] ToByteArray(int[] value)
        {
            byte[] by = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 4 * i);
            }
            return by;
        }

        public byte[] ToByteArray(string value)
        {
            byte[] by = Encoding.ASCII.GetBytes(value).Reverse().ToArray();
            return by;
        }

        public byte[] ToByteArray(ushort[] value)
        {
            byte[] by = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 2 * i);
            }
            return by;
        }

        public byte[] ToByteArray(uint[] value)
        {
            byte[] by = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(value[i]);
                temp.CopyTo(by, 4 * i);
            }
            return by;
        }

        public bool[] ToBool(byte[] bytes, int length)
        {
            ushort[]arrayUshort =  ToUInt16(bytes, length);
            bool[] array = new bool[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = arrayUshort[i] == 1;
                //BitArray bitSet = new BitArray(a);
                //array[i] = BitConverter.ToBoolean(bytes, 2 * i);
            }
            return array;
        }

        public double[] ToDouble(byte[] bytes, int length)
        {
            double[] array = new double[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BitConverter.ToDouble(bytes, 8 * i);
            }
            return array;
        }

        public float[] ToFloat(byte[] bytes, int length)
        {
            try
            {
                float[] array = new float[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = BitConverter.ToSingle(bytes, 4 * i);
                }
                return array;
            }
            catch (Exception E)
            {

                throw new Exception(E.Message + ";" + BitConverter.ToString(bytes) + ";" + length);
            }
        }

        public short[] ToInt16(byte[] bytes, int length)
        {
            short[] array = new short[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BitConverter.ToInt16(bytes, 2 * i);
            }
            return array;
        }

        public int[] ToInt32(byte[] bytes, int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BitConverter.ToInt32(bytes, 4 * i);
            }
            return array;
        }


        public string ToString(byte[] bytes)
        {
            bytes = bytes.Reverse().ToArray();
            return Encoding.ASCII.GetString(bytes);
        }

        public ushort[] ToUInt16(byte[] bytes, int length)
        {
            ushort[] array = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BitConverter.ToUInt16(bytes, 2 * i);
            }
            return array;
        }

        public uint[] ToUInt32(byte[] bytes, int length)
        {
            uint[] array = new uint[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = BitConverter.ToUInt32(bytes, 4 * i);
            }
            return array;
        }


        protected byte[] ByteTransDataFormat4(byte[] value, int index = 0)
        {
            byte[] array = new byte[4];
            switch (DataFormat)
            {
                case DataFormat.ABCD:
                    array[0] = value[index + 3];
                    array[1] = value[index + 2];
                    array[2] = value[index + 1];
                    array[3] = value[index];
                    break;
                case DataFormat.BADC:
                    array[0] = value[index + 2];
                    array[1] = value[index + 3];
                    array[2] = value[index];
                    array[3] = value[index + 1];
                    break;
                case DataFormat.CDAB:
                    array[0] = value[index + 1];
                    array[1] = value[index];
                    array[2] = value[index + 3];
                    array[3] = value[index + 2];
                    break;
                case DataFormat.DCBA:
                    array[0] = value[index];
                    array[1] = value[index + 1];
                    array[2] = value[index + 2];
                    array[3] = value[index + 3];
                    break;
            }
            return array;
        }

        protected byte[] ByteTransDataFormat8(byte[] value, int index = 0)
        {
            byte[] array = new byte[8];
            switch (DataFormat)
            {
                case DataFormat.ABCD:
                    array[0] = value[index + 7];
                    array[1] = value[index + 6];
                    array[2] = value[index + 5];
                    array[3] = value[index + 4];
                    array[4] = value[index + 3];
                    array[5] = value[index + 2];
                    array[6] = value[index + 1];
                    array[7] = value[index];
                    break;
                case DataFormat.BADC:
                    array[0] = value[index + 6];
                    array[1] = value[index + 7];
                    array[2] = value[index + 4];
                    array[3] = value[index + 5];
                    array[4] = value[index + 2];
                    array[5] = value[index + 3];
                    array[6] = value[index];
                    array[7] = value[index + 1];
                    break;
                case DataFormat.CDAB:
                    array[0] = value[index + 1];
                    array[1] = value[index];
                    array[2] = value[index + 3];
                    array[3] = value[index + 2];
                    array[4] = value[index + 5];
                    array[5] = value[index + 4];
                    array[6] = value[index + 7];
                    array[7] = value[index + 6];
                    break;
                case DataFormat.DCBA:
                    array[0] = value[index];
                    array[1] = value[index + 1];
                    array[2] = value[index + 2];
                    array[3] = value[index + 3];
                    array[4] = value[index + 4];
                    array[5] = value[index + 5];
                    array[6] = value[index + 6];
                    array[7] = value[index + 7];
                    break;
            }
            return array;
        }
    }

    public enum DataFormat
    {
        ABCD,
        BADC,
        CDAB,
        DCBA
    }
}
