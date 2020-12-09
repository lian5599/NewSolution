using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Plc
{
    public interface IConverter
    {
        string ToString(byte[] bytes);
        bool[] ToBool(byte[] bytes,int length);
        ushort[] ToUInt16(byte[] bytes, int length);
        short[] ToInt16(byte[] bytes, int length);
        int[] ToInt32(byte[] bytes, int length);
        uint[] ToUInt32(byte[] bytes, int length);
        double[] ToDouble(byte[] bytes, int length);
        float[] ToFloat(byte[] bytes, int length);

        byte[] ToByteArray(string value);
        byte[] ToByteArray(ushort[] value);
        byte[] ToByteArray(short[] value);
        byte[] ToByteArray(uint[] value);
        byte[] ToByteArray(int[] value);
        byte[] ToByteArray(double[] value);
        byte[] ToByteArray(float[] value);
    }
}
