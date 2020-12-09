using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public abstract class ProcessBase
    {
        public string Id = "";
        public virtual Result Init()
        {

            return new Result();
        }
        public abstract Result Run();
    }
}
