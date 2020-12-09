using Helper;
using TKS.Manager;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TKS;

namespace FlowCharter
{
    [Serializable]
    public class EndNodeInfo : GraphNodeInfoBase
    {
        public EndNodeInfo(GraphNode n) : base(n) { }

        protected override Result Action()
        {
            try
            {
                Dispose();
                return new Result();
                //(item.UserObject as GraphNodeInfoBase).RunAll();
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message);
                return new Result(e.Message);
            }
        }
    }

}
