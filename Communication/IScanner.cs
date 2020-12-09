using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Helper;
using Helper;

namespace Scanner
{
    public interface IScanner
    {       
        string Id { get; set; }

        void Connect();
        Result<string> Trigger();
        Task<Result<string>> TriggerAsync();

    }
}
