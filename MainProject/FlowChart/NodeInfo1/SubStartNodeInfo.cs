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
    public class SubStartNodeInfo : GraphNodeInfoBase
    {
        public SubStartNodeInfo(GraphNode n) : base(n) { }
    }

}
