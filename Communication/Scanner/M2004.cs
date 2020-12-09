using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Scanner
{
    public class M2004<T> : ScannerBase where T : ICommunication, new()
    {
        bool scanResult = false;
        public M2004(string Id, string paramStr, int paramInt, int timeout = 2000)
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
                    scanResult = false;

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    while (!scanResult)
                    {
                        SendCommand("LON", true);
                        if (stopwatch.ElapsedMilliseconds > Timeout)
                        {
                            throw new Exception(Id + "扫码超时:" + Timeout.ToString());
                        }
                        Thread.Sleep(50);
                    }
                    string msg = scanResult ? Id + "扫码成功" : Id + "扫码失败";
                    return new Result<string>(Sn, scanResult, msg, 2004);
                }
                catch (Exception e)
                {
                    return new Result<string>(e.Message, 2004);
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
                Sn = ResponseTemp.Substring(startPos,endPos - startPos);
                if (Sn == "NG")
                    scanResult = false;
                else
                {
                    scanResult = true;
                    OnSnRecieved(Sn);
                }
                ResponseTemp = string.Empty;
                ResponseEvent.Set();
            }
        }
    }
}
