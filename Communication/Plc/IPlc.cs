using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Plc
{
    public interface IPlc
    {
        Result<bool[]> ReadBoolean(int address, int length);
        Result<byte[]> ReadByte(int address, int length);
        Result<ushort[]> ReadUInt16(int address, int length);
        Result<short[]> ReadInt16(int address, int length);
        Result<uint[]> ReadUInt32(int address, int length);
        Result<int[]> ReadInt32(int address, int length);
        Result<float[]> ReadSingle(int address, int length);
        Result<double[]> ReadDouble(int address, int length);
        Result<string> ReadString(int address, int length);
        Result Write(int address, byte[] value);
        Result Write(int address, ushort[] value);
        Result Write(int address, short[] value);
        Result Write(int address, uint[] value);
        Result Write(int address, string value);
        Result Write(int address, double[] value);
        Result Write(int address, float[] value);
        Result Write(int address, bool[] value);
        Result Write(int address, int[] values);
        Result Write(int address, int value);
    }
}
