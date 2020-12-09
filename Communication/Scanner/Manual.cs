using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Scanner
{
    public class Manual<T> : ScannerBase where T : ICommunication, new()
    {
        public Manual(string Id, string paramStr, int paramInt, int timeout = 2000)
        {
            this.Id = Id;
            HardwareId = "Manual";
            Kind = this.GetType().Name.Replace("`1", "");
            Mode = typeof(T).Name;
            this.ParamStr = paramStr;
            this.ParamInt = paramInt;
            Timeout = timeout;
            communication = new T();
        }

        public override Result<string> Trigger()
        {
            throw new NotImplementedException();
        }

        protected override void MessageRecieved(object sender, byte[] recievedMessage)
        {
            ResponseTemp = ResponseTemp + Encoding.ASCII.GetString(recievedMessage); ;
            if (ResponseTemp.Contains("\r"))
            {
                Sn = string.Empty;
                Sn = ResponseTemp.Replace("\r", "");
                Sn = Sn.Replace("\n", "");
                OnSnRecieved(Sn);
                ResponseTemp = string.Empty;
                ResponseEvent.Set();
            }
        }
    }
}
