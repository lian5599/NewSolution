using System;
using System.Collections.Generic;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// Use this abstract class to implement temporary data structures representing
	/// collections of properties that do not exist in your actual <see cref="T:Northwoods.Go.GoObject" /> classes,
	/// in conjunction with <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This abstract class does not define any properties, so there will not
	/// be any property name conflicts with any properties that you want to define.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>
	/// The <see cref="T:Northwoods.Go.GoMultiTextNode" /> class holds a variable number of items, along with
	/// corresponding ports for each item.  It would be natural to represent such a node in XML
	/// by having a fixed set of attributes corresponding to node properties, and then a collection
	/// of nested elements describing each of the items and their ports.
	/// </para>
	/// <para>
	/// However, the <see cref="T:Northwoods.Go.GoMultiTextNode" /> class is not organized such that there is a
	/// collection of objects (<see cref="T:Northwoods.Go.GoGroup" />s) that are holding an item and its ports.
	/// Instead an item has the same <see cref="P:Northwoods.Go.GoObject.Parent" /> as the item's port.
	/// The items and ports all belong directly to the node in one big collection.
	/// So there is no <see cref="T:Northwoods.Go.GoObject" /> class representing an "item" for which you could
	/// define a <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" />.
	/// </para>
	/// <para>
	/// Instead you can define an auxiliary data class that holds the information about each item.
	/// Here's the start of such a definition:
	/// </para>
	/// <code>
	/// internal class MultiTextNodeItem {  // this way requires implementing a transformer subclass
	///   public GoObject Item;
	///   public GoObject LeftPort;
	///   public GoObject RightPort;
	/// }
	/// </code>
	/// <para>
	/// Then one can define a transformer for <c>MultiTextNodeItem</c> and a transformer
	/// for <see cref="T:Northwoods.Go.GoMultiTextNode" /> that generates nested child elements for each item
	/// and then consumes them by creating the appropriate <see cref="T:Northwoods.Go.GoObject" />s and
	/// adding them to the <see cref="T:Northwoods.Go.GoMultiTextNode" />.
	/// </para>
	/// <para>
	/// However, you can avoid having to implement a transformer class
	/// for <c>MultiTextNodeItem</c> if you inherit from <see cref="T:Northwoods.Go.Xml.GoXmlSimpleData" />
	/// and define real .NET properties for each property you want to store so that
	/// you can use <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />.
	/// </para>
	/// <code>
	/// internal class MultiTextNodeItem : GoXmlSimpleData {  // no transformer subclass needed
	///   public MultiTextNodeItem() { }
	///   public GoObject Item {
	///     get { return (GoObject)Get("Item"); }
	///     set { Set("Item", value); }
	///   }
	///   public GoObject LeftPort {
	///     get { return (GoObject)Get("LeftPort"); }
	///     set { Set("LeftPort", value); }
	///   }
	///   public GoObject RightPort {
	///     get { return (GoObject)Get("RightPort"); }
	///     set { Set("RightPort", value); }
	///   }
	/// }
	/// </code>
	/// <para>
	/// You can then define a <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> for <c>MultiTextNodeItem</c>.
	/// The number of attributes/properties that you decide to read/write is dependent on your
	/// particular application's needs, of course.  But one possibility is something like:
	/// </para>
	/// <code>
	/// MultiTextNodeItem item = new MultiTextNodeItem();  // represents an item as if it were a single GoObject
	/// GoMultiTextNode mtn = new GoMultiTextNode();
	/// item.Item = mtn.CreateText("", 0);
	/// item.LeftPort = mtn.CreatePort(true, 0);
	/// item.RightPort = mtn.CreatePort(false, 0);
	/// GoXmlBindingTransformer st = new GoXmlBindingTransformer("item", item);
	/// st.AddBinding("text", "Item.Text");
	/// st.AddBinding("sel", "Item.Selectable");
	/// st.AddBinding("bold", "Item.Bold");
	/// st.AddBinding("width", "Item.Width");
	/// st.AddBinding("wrap", "Item.WrappingWidth");
	/// st.AddBinding("LeftPort", "LeftPort", GoXmlBindingFlags.DefinesShared);
	/// st.AddBinding("RightPort", "RightPort", GoXmlBindingFlags.DefinesShared);
	/// readerorwriter.AddTransformer(st);
	/// </code>
	/// <para>
	/// The <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> for <see cref="T:Northwoods.Go.GoMultiTextNode" /> might be implemented as:
	/// </para>
	/// <code>
	/// internal class SimpleGoMultiTextNodeTransformer : GoXmlBindingTransformer {
	///   public SimpleGoMultiTextNodeTransformer(String eltname, GoMultiTextNode obj) : base(eltname, obj) {
	///     this.IdAttributeUsedForSharedObjects = true;  // all nodes get an "id" attribute
	///     this.BodyConsumesChildElements = true;  // make sure ConsumeChild gets called
	///     AddBinding("ItemWidth");  // attribute name is same as property name
	///     AddBinding("back", "Brush.Color");
	///     AddBinding("loc", "ListGroup.Location");
	///     AddBinding("TopPort", "TopPort", GoXmlBindingFlags.DefinesShared);  // write out reference for and to the TopPort
	///     AddBinding("BottomPort", "BottomPort", GoXmlBindingFlags.DefinesShared);  // ibid for BottomPort
	///   }
	///
	///   public override void GenerateBody(Object obj) {
	///     base.GenerateBody(obj);
	///     GoMultiTextNode mtn = (GoMultiTextNode)obj;
	///     for (int i = 0; i &lt; mtn.ItemCount; i++) {
	///       MultiTextNodeItem dummy = new MultiTextNodeItem();
	///       dummy.Item = mtn.GetItem(i);
	///       dummy.LeftPort = mtn.GetLeftPort(i);
	///       dummy.RightPort = mtn.GetRightPort(i);
	///       this.Writer.GenerateObject(dummy);
	///     }
	///   }
	///
	///   public override void ConsumeChild(Object parent, Object child) {
	///     base.ConsumeChild(parent, child);
	///     GoMultiTextNode mtn = (GoMultiTextNode)parent;
	///     MultiTextNodeItem item = child as MultiTextNodeItem;
	///     if (item != null) {
	///       // because MultiTextNodeItem inherits from GoXmlSimpleData,
	///       // the GoObject properties of item will have been copied for you already,
	///       // so you can just add them to your group
	///       mtn.AddItem(item.Item, item.LeftPort, item.RightPort);
	///     }
	///   }
	/// }
	/// </code>
	/// <para>
	/// An example of the resulting XML could be:
	/// </para>
	/// <code>
	/// &lt;GoMultiTextNode id="30" ItemWidth="100" back="-18751" loc="36.5 379" TopPort="73" BottomPort="74"&gt;
	///   &lt;item text="first" sel="false" bold="true" width="100" wrap="100" LeftPort="75" RightPort="76" /&gt;
	///   &lt;item text="second" sel="true" bold="false" width="100" wrap="100" LeftPort="77" RightPort="78" /&gt;
	///   &lt;item text="third" sel="true" bold="true" width="100" wrap="100" LeftPort="79" RightPort="80" /&gt;
	/// &lt;/GoMultiTextNode&gt;
	/// </code>
	/// </example>
	public abstract class GoXmlSimpleData : ICloneable
	{
		private Dictionary<string, object> myTable = new Dictionary<string, object>();

		/// <summary>
		/// Gets the value of a property, given the property name.
		/// </summary>
		/// <param name="s">the property name, must not be null or empty</param>
		/// <returns><c>Object</c></returns>
		public object Get(string s)
		{
			myTable.TryGetValue(s, out object value);
			return value;
		}

		/// <summary>
		/// Sets the value of a property, given the property name.
		/// </summary>
		/// <param name="s">the property name, must not be null or empty</param>
		/// <param name="x"><c>Object</c></param>
		public void Set(string s, object x)
		{
			myTable[s] = x;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="T:Northwoods.Go.Xml.GoXmlSimpleData" />-inheriting class
		/// and copies all of the property values -- copying <see cref="T:Northwoods.Go.GoObject" />s
		/// and cloning <c>ICloneable</c>s.
		/// </summary>
		/// <returns>an instance of this same <see cref="T:Northwoods.Go.Xml.GoXmlSimpleData" />-inheriting type</returns>
		public object Clone()
		{
			GoXmlSimpleData goXmlSimpleData = (GoXmlSimpleData)Activator.CreateInstance(GetType());
			Dictionary<string, object> dictionary = goXmlSimpleData.myTable;
			GoCopyDictionary goCopyDictionary = new GoCopyDictionary();
			foreach (KeyValuePair<string, object> item in myTable)
			{
				GoObject goObject = item.Value as GoObject;
				if (goObject != null)
				{
					dictionary[item.Key] = goCopyDictionary.CopyComplete(goObject);
				}
				else
				{
					ICloneable cloneable = goObject as ICloneable;
					if (cloneable != null)
					{
						dictionary[item.Key] = cloneable.Clone();
					}
				}
			}
			return goXmlSimpleData;
		}
	}
}
