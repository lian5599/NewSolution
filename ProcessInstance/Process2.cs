using Communication.Plc;
using Communication.Scanner;
using Helper;
using TKS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace ProcessInstance
{
    public class Process2 : ProcessBase
    {
        public Process2()
        {
            Id  = "左屏扫码标志";
        }

        public override Result Run()
        {
            try
            {
                object param = Flow.Instance.GetOutput("左屏扫码标志");
                if (param.ToString() == "1")
                {
                    return new Result();
                }
                else
                {
                    return new Result(param.ToString() + " != 1");
                }
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message);
                return new Result(e.Message);
            }
        }   
    }
}
            