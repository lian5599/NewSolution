using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communication.Plc
{
    public abstract class PlcBase : CommunicationBase, IPlc
    {
        protected readonly object ReadLocker = new object();
        protected IConverter DataConverter { get; set; }
        protected byte[] responseBytesTemp { get; set; } = new byte[0];
        protected byte[] DataBytes { get; set; }
        protected abstract byte[] GetReadCmd(int address, int length = 1);
        protected abstract byte[] GetWriteCmd(int address, byte[] values);

        public  Result Write(int address, int value)
        {
            return Write(address, new int[1] { value });
        }
        public Task<Result> WriteAsync(int address, int value)
        {
            return Task.Run<Result>(() =>
            {
                return Write(address, value);
            });
        }       
        public Task<Result> WriteAsync(int address, int[] values)
        {
            return Task.Run<Result>(() =>
            {
                return Write(address, values);
            });
        }
        public Result<byte[]> ReadByte(int address, int length)
        {
            lock (ReadLocker)
            {
                try
                {
                    SendCommand(GetReadCmd(address, length));
                    return new Result<byte[]>(DataBytes);
                }
                catch (Exception e)
                {
                    return new Result<byte[]>(null, false, e.Message, (int)ErrorCode.fatalError);
                } 
            }
        }
        public Result<ushort[]> ReadUInt16(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length);
            if (re.IsSuccess)
            {
                ushort[] array = DataConverter.ToUInt16(re.Content, length);
                return new Result<ushort[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<ushort[]>(null, false, re.Message,re.ErrorCode);
        }
        public Result<short[]> ReadInt16(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length);
            if (re.IsSuccess)
            {
                short[] array = DataConverter.ToInt16(re.Content,length);
                return new Result<short[]>(array,true,"成功"+array[0].ToString());
            }
            else return new Result<short[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result<uint[]> ReadUInt32(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length * 2);
            if (re.IsSuccess)
            {
                uint[] array = DataConverter.ToUInt32(re.Content, length);
                return new Result<uint[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<uint[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result<int[]> ReadInt32(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length * 2);
            if (re.IsSuccess)
            {
                int[] array = DataConverter.ToInt32(re.Content, length);
                return new Result<int[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<int[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result<double[]> ReadDouble(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length * 4);
            if (re.IsSuccess)
            {
                double[] array = DataConverter.ToDouble(re.Content, length);
                return new Result<double[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<double[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result<float[]> ReadSingle(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length * 2);
            if (re.IsSuccess)
            {
                float[] array = DataConverter.ToFloat(re.Content, length);
                return new Result<float[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<float[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result<string> ReadString(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length);
            if (re.IsSuccess)
            {
                string str = DataConverter.ToString(re.Content);

                return new Result<string>(str, true, "成功" + str);
            }
            else return new Result<string>(null, false, re.Message, re.ErrorCode);
        }

        public Result<bool[]> ReadBoolean(int address, int length)
        {
            Result<byte[]> re = ReadByte(address, length);
            if (re.IsSuccess)
            {
                bool[] array = DataConverter.ToBool(re.Content, length);
                return new Result<bool[]>(array, true, "成功" + array[0].ToString());
            }
            else return new Result<bool[]>(null, false, re.Message, re.ErrorCode);
        }
        public Result Write(int address, byte[] values)
        {
            int length = values.Length % 2 == 0 ? values.Length / 2 : values.Length / 2 + 1;
            try
            {
                SendCommand(GetWriteCmd(address, values));
                return new Result() { Message = Id + "写入" + address.ToString() + "到" + (address + length - 1).ToString() + "成功:" + string.Join(",", values) };
            }
            catch (Exception e)
            {
                return new Result(Id + "写入" + address.ToString() + "到" + (address + length - 1).ToString() + "失败:" + e.Message, (int)ErrorCode.fatalError);
            }
        }
        public Result Write(int address, ushort[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, short[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, uint[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, int[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, string value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, double[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, float[] value)
        {
            byte[] by = DataConverter.ToByteArray(value);
            return Write(address, by);
        }
        public Result Write(int address, bool[] value)
        {
            throw new NotImplementedException();
        }
    }
}
