using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Scanner
{
    public class M120<T> : ScannerBase where T : ICommunication, new()
    {
        public M120(string Id, string paramStr, int paramInt, int timeout = 2000)
        {
            this.Id = Id;
            HardwareId = "Scanner";
            Kind = this.GetType().Name.Replace("`1", "");
            Mode = typeof(T).Name;
            this.ParamStr = paramStr;
            this.ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
        }
        public override Result<string> Trigger()
        {
            lock (TriggerLocker)
            {
                try
                {
                    SendCommand("LOFF\r\n", false);
                    SendCommand("LON\r\n");
                    SendCommand("LOFF\r\n", false);
                    return new Result<string>() { Content = Sn, Message = Id + "扫码成功" };
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("超时"))
                    {
                        SendCommand("LOFF\r\n", false);
                    }
                    return new Result<string>(e.Message, 120);
                } 
            }
        }

        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            string startStr = "A79Z";
            string endStr = "Z97A";
            ResponseTemp = ResponseTemp + Encoding.ASCII.GetString(recievedMessage);
            int startPos = ResponseTemp.IndexOf(startStr) + startStr.Length;
            int endPos = ResponseTemp.IndexOf(endStr);

            if (startPos >= startStr.Length && endPos > 0)
            {
                Sn = ResponseTemp.Substring(startPos, endPos - startPos);
                OnSnRecieved(Sn);
                ResponseTemp = string.Empty;
                ResponseEvent.Set();
            }
            //_responsedStr = _responsedStr + Encoding.ASCII.GetString(recievedMessage); ;

            //if (_responsedStr.Contains("Z97A"))
            //{
            //    Sn = string.Empty;
            //    _responsed = true;
            //    _responsedStr = _responsedStr.Replace("Z97A", "");
            //    Sn = _responsedStr.Replace("A79Z", "");
            //    OnSnRecieved(Sn);
            //    _responsedStr = string.Empty;
            //}
        }
    }
}
