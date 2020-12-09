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

namespace ProcessInstance
{
    public class Process1 : ProcessBase
    {
        string name = "subprocess";
        public Process1()
        {
            Id  = name;
        }
        public override Result Init()
        {
            try
            {
                //flag = Manager1.GetInstance().flag(name);
                //scanner = Manager1.GetInstance().scanner(name);
                return base.Init();
            }
            catch (Exception e)
            {
                return new Result(e.Message,410);
            }
        }

        public override Result Run()
        {
            bool isSucess = UiHelper.AskUser("isSucess?");
            return new Result(isSucess);
        }   
    }
}
