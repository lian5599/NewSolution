using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Scanner
{
    public interface IScanner
    {
        event SnRecievedEventHandler SnRecieved;
        Result<string> Trigger();
        Task<Result<string>> TriggerAsync();
    }
}
