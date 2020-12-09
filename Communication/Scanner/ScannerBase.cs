using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Scanner
{
    public abstract class ScannerBase : CommunicationBase,IScanner
    {
        protected  readonly object TriggerLocker = new object();
        protected string ResponseTemp { get; set; } = string.Empty;
        public string Sn { get; set; }

        public event SnRecievedEventHandler SnRecieved;
        protected void OnSnRecieved(string sn)
        {
            SnRecieved?.Invoke(this, sn);
        }
        public abstract Result<string> Trigger();
        public  Task<Result<string>> TriggerAsync()
        {
            return Task.Run<Result<string>>(() =>
            {
                return Trigger();
            });
        }
    }
    public delegate void SnRecievedEventHandler(object sender, string sn);
}
