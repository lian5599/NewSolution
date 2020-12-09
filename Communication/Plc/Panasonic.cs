using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communication.Plc
{
    public class Panasonic<T> : PlcBase where T : ICommunication, new()
    {
        string ResponseTemp { get; set; } = string.Empty;
        public Panasonic(string Id,string paramStr, int paramInt, int timeout = 2000)
        {
            this.Id = Id;
            HardwareId = "Plc";
            Kind = this.GetType().Name.Replace("`1","");
            Mode = typeof(T).Name;
            this.ParamStr = paramStr;
            this.ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
            DataConverter = new ConverterBase();
        }

        protected override byte[] GetReadCmd(int address, int length = 1)
        {
            string begin = "<01#RDD";
            string beginAdd = address.ToString("00000");
            string endAdd = (address + length - 1).ToString("00000");
            string end = "**\r";
            string cmd = begin + beginAdd + endAdd + end;
            return Encoding.ASCII.GetBytes(cmd);
        }

        protected override byte[] GetWriteCmd(int address, byte[] values)
        {
            string begin = "<01#WDD";
            int length = values.Length % 2 == 0 ? values.Length / 2 : values.Length / 2 + 1;
            string beginAdd = address.ToString("00000");
            string endAdd = (address + length - 1).ToString("00000");
            string valuesStr = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                valuesStr += values[i].ToString("X2");
            }
            if (values.Length % 2 == 1)
            {
                valuesStr += "00";
            }
            string end = "**\r";
            string cmd = begin + beginAdd + endAdd + valuesStr + end;
            return Encoding.ASCII.GetBytes(cmd);
        }
        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            string startStr = "<";
            string endStr = "\r";
            ResponseTemp = ResponseTemp + Encoding.ASCII.GetString(recievedMessage);
            int startPos = ResponseTemp.IndexOf(startStr);
            int endPos = ResponseTemp.IndexOf(endStr) + 1;

            if (endPos > 0)
            {
                string oneResult = ResponseTemp.Substring(startPos, endPos - startPos);
                ResponseTemp = ResponseTemp.Substring(endPos);
                int WriteResponse = oneResult.IndexOf("WD");
                int ReadResponse = oneResult.IndexOf("RD");
                int ErrorResponse = oneResult.IndexOf("!");

                if (ErrorResponse > 0)
                {
                    throw new Exception("plc错误：" + oneResult);
                }
                if (WriteResponse > 0)
                {

                }
                else if (ReadResponse > 0)
                {
                    ReadResponse += 2;
                    int dataLength = (endPos - ReadResponse - 3) / 4;

                    DataBytes = new byte[2 * dataLength];
                    for (int i = 0; i < 2 * dataLength; i++)
                    {
                        DataBytes[i] = Convert.ToByte(oneResult.Substring(ReadResponse + 2 * i, 2), 16);
                    }
                }
                ResponseEvent.Set();
            }
        }
        //protected override void MessageRecieved(object sender, byte[] recievedMessage)
        //{
        //    _responsedStr = _responsedStr + Encoding.ASCII.GetString(recievedMessage);

        //    int ResponseEnd = _responsedStr.IndexOf("\r");
        //    if (ResponseEnd > 0)
        //    {
        //        int WriteResponse = _responsedStr.IndexOf("WD");
        //        int ReadResponse = _responsedStr.IndexOf("RD");
        //        int ErrorResponse = _responsedStr.IndexOf("!");

        //        if (ErrorResponse > 0)
        //        {
        //            throw new Exception("plc错误：" + _responsedStr);
        //        }
        //        if (WriteResponse > 0)
        //        {

        //        }
        //        else if (ReadResponse > 0)
        //        {                  
        //            ReadResponse += 2;
        //            int dataLength = (ResponseEnd - ReadResponse - 2) / 4;

        //            ReceiveBytes = new byte[2 * dataLength];
        //            for (int i = 0; i < 2 * dataLength; i++)
        //            {
        //                ReceiveBytes[i] = Convert.ToByte(_responsedStr.Substring(ReadResponse + 2 * i, 2), 16);
        //            }

        //            //valuesTemp.Clear();
        //            //for (int i =0; i < dataLength ; i++)
        //            //{
        //            //    string oneDataStr = _responsedMessage.Substring(ReadResponse + 2, 2) + _responsedMessage.Substring(ReadResponse, 2);
        //            //    int data = Convert.ToInt32(oneDataStr, 16);
        //            //    valuesTemp.Add(data);
        //            //    ReadResponse += 4;
        //            //}
        //        }
        //        _responsedStr = string.Empty;
        //        _responsed = true;
        //    }
        //}
    }
}