using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Helper;
using TKS.Manager;
using Northwoods.Go;

namespace FlowCharter
{
    /// <summary>
    /// This class is used to hold information that the user can edit
    /// in the Node Properties grid.
    /// Only define value type property.
    /// </summary>
    [Serializable]
    public class GraphNodeInfoBase : ICustomTypeDescriptor
    {
        public GraphNodeInfoBase(GraphNode n) 
        {
            this.GraphNode = n;
        }

        protected GraphNode myNode = null;

        [Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public GraphNode GraphNode
        {
            get { return myNode; }
            set { myNode = value; }
        }
        public override String ToString()
        {
            return this.Name;
        }
        // ICustomTypeDescriptor
        /// <summary>
        /// When the document is read-only, this implementation of GetAttributes
        /// dynamically adds a ReadOnlyAttribute to the collection of attributes for
        /// this class, to disable the PropertyGrid in which this object is being
        /// displayed.
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            AttributeCollection coll = TypeDescriptor.GetAttributes(this, true);
            GraphDoc doc = this.GraphNode.Document as GraphDoc;
            if (doc != null && doc.IsReadOnly)
            {
                ReadOnlyAttribute ro = new ReadOnlyAttribute(true);
                Attribute[] atts = new Attribute[coll.Count + 1];
                coll.CopyTo(atts, 0);
                atts[coll.Count] = ro;
                return new AttributeCollection(atts);
            }
            return coll;
        }

        public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
        public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
        public Object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
        public PropertyDescriptorCollection GetProperties() { return TypeDescriptor.GetProperties(this, true); }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return TypeDescriptor.GetProperties(this, attributes, true); }
        public Object GetPropertyOwner(PropertyDescriptor pd) { return this; }


        [Category("Identity")][ReadOnly(true)]
        [Description("An integer identifier, unique within the document")]
        public int ID { get; set; }

        [Category("Identity")]
        [Description("The node's text label; must be unique within the document")]
        public string Kind
        {
            get 
            {
                if (GraphNode!=null)
                {
                    return this.GraphNode.Kind;
                }
                return "";
            }
        }

        [Category("Identity")]
        [Description("The node's text label; must be unique within the document")]
        public String Name
        {
            get { return this.GraphNode.Text; }
            set 
            {
                if (GraphNode != null && value != this.GraphNode.Text)
                {
                    this.GraphNode.Text = value;
                    Output.OutputDefineChange();
                    this.GraphNode.Text = value;
                }
            }//this.GraphNode.MakeUniqueName(value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public Result Result { get; set; }
        [Category("Output")]
        [Description("Output Data")]
        public Dictionary<string, object> OutPutResults { get; set; } = new Dictionary<string, object>();
        public Result Run() 
        {           
            Console.WriteLine(this.ToString() + " run");
            GraphNode.Shadowed = true;
            Result = Action();
            if (Result.IsSuccess)
            {
                Thread.Sleep(500);
                GraphNode.Shadowed = false;
            }
            return Result;
        }
        protected virtual Result Action()
        {
            return new Result();
        }

        public virtual Result RunAll()
        {           
            this.Run();
            if (Result.IsSuccess)
            {                               
                foreach (var item in GraphNode.Destinations)
                {
                    GraphNode subNode = item as GraphNode;

                    Result = (subNode.UserObject as GraphNodeInfoBase).RunAll();
                }
            }
            return Result;
        }
        protected virtual void Dispose()
        {
            this.Result = null;
            foreach (var item in GraphNode.Sources)
            {
                GraphNode subNode = item as GraphNode;
                (subNode.UserObject as GraphNodeInfoBase).Dispose();
            }
        }
        public virtual void OutputDeclaration() { }

        public void UpdateFlow(string flowName)
        {
            if (Kind.ToLower().Contains("start"))
            {
                flowName = Name;
            }
            if (!Flow.Instance.FlowNode.ContainsKey(flowName)) 
            {
                Flow.Instance.FlowNode.Add(Name, new Dictionary<string, GraphNodeInfoBase>());
            }
            foreach (var item in GraphNode.Destinations)
            {
                GraphNodeInfoBase NodeInfo = (item as GraphNode).UserObject as GraphNodeInfoBase;
                if (!Flow.Instance.FlowNode[flowName].ContainsKey(NodeInfo.Name))
                {
                    Flow.Instance.FlowNode[flowName].Add(NodeInfo.Name, NodeInfo);
                    NodeInfo.UpdateFlow(flowName);
                }
            }
            foreach (var item in GraphNode.Sources)
            {
                GraphNodeInfoBase NodeInfo = (item as GraphNode).UserObject as GraphNodeInfoBase;
                if (!Flow.Instance.FlowNode[flowName].ContainsKey(NodeInfo.Name))
                {
                    Flow.Instance.FlowNode[flowName].Add(NodeInfo.Name, NodeInfo);
                    NodeInfo.UpdateFlow(flowName);
                }
            }
        }

        protected void AddOutput(string key,object value)
        {
            if (!OutPutResults.ContainsKey(key))
            {
                OutPutResults.Add(key, new object());
            }
            OutPutResults[key] = value;
        }
    }
}
