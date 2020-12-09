using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Plc
{
    public class MelsecMcNet<T> : PlcBase where T : ICommunication, new()
    {
        //byte[] ReadResponseModel = {
        //                   0xD0, 0x00,
        //                   0x00,
        //                   0xFF,
        //                   0xFF, 0x03,
        //                   0x00,                    //站号
        //                   0x02,0x00,               //响应数据长度
        //                   0x00, 0x00 };
        public MelsecMcNet(string Id, string paramStr, int paramInt, int timeout = 2000)
        {
            this.Id = Id;
            HardwareId = "Plc";
            Kind = this.GetType().Name.Replace("`1", "");
            Mode = typeof(T).Name;
            this.ParamStr = paramStr;
            this.ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
            DataConverter = new ConverterBase();
        }

        protected override byte[] GetReadCmd(int address, int length = 1)
        {
            byte[] readModel = {
                           0x50, 0x00,          //副头部
                           0x00,                //网络编号
                           0xFF,                //PLC编号
                           0xFF, 0x03,          //IO编号
                           0x00,                //模块站号
                           0x0C, 0x00,          //请求数据长度,计算为之后的所有数据
                           0x10, 0x00,          //时钟,表示等待PLC响应的timeout的时间，高低位互换，实际为0001，即最大等待时间250ms*1                         
                           0x01, 0x04,          //指令,实际为0401即为批量读取
                           0x00, 0x00,          //子指令,值0表示按字(16位)读取,如果值1则表示按位读取
                           0xFA, 0x0A, 0x00,    //首地址,实际为000AFA
                           0xA8,                //软元件
                           0x01, 0x00           //长度                                          
                       };
            readModel[15] = (byte)(address % 255);
            readModel[16] = (byte)(address / 255);
            readModel[19] = (byte)length;
            return readModel;
        }

        protected override byte[] GetWriteCmd(int address, byte[] values)
        {
            byte[] writeModel = {
                           0x50, 0x00,            //副头部
                           0x00,                  //网络编号
                           0xFF,                  //PLC编号
                           0xFF, 0x03,            //IO编号
                           0x00,                  //模块站号

                           0x01, 0x00,            //请求数据长度,计算为之后的所有数据

                           0x10, 0x00,            //时钟,表示等待PLC响应的timeout的时间，高低位互换，实际为0001，即最大等待时间250ms*1 
                           0x01, 0x14,            //指令,实际为0401即为批量读取
                           0x00, 0x00,            //子指令,值0表示按字(16位)读取,如果值1则表示按位读取
                           0xFA, 0x0A, 0x00,      //首地址,实际为000AFA
                           0xA8,                  //软元件

                           0x01, 0x00,            //长度
                           /*0x00, 0x00 */};
            byte[] writeReal = new byte[writeModel.Length + values.Length];
            Array.Copy(writeModel, 0, writeReal, 0, writeModel.Length);
            writeReal[15] = (byte)(address % 255);
            writeReal[16] = (byte)(address / 255);
            writeReal[19] = (byte)(values.Length / 2);
            Array.Copy(values, 0, writeReal, writeModel.Length, values.Length);

            byte[] g = BitConverter.GetBytes(12 + values.Length);
            writeReal[7] = g[0];
            writeReal[8] = g[1];
            return writeReal;
        }

        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            byte[] _responseBytes = new byte[responseBytesTemp.Length + recievedMessage.Length];
            responseBytesTemp.CopyTo(_responseBytes, 0);
            recievedMessage.CopyTo(_responseBytes, responseBytesTemp.Length);

            int dataLength = _responseBytes[7] - 2;
            if (_responseBytes.Length == 11 + dataLength)
            {
                if (dataLength > 0)
                {
                    DataBytes = new byte[dataLength];
                    Array.Copy(_responseBytes, 11, DataBytes, 0, dataLength);
                }
                responseBytesTemp = new byte[0];
                ResponseEvent.Set();
            }
            else responseBytesTemp = _responseBytes;
        }
    }
}
