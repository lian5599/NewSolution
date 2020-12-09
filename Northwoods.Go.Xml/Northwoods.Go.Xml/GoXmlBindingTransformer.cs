#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// This class implements a <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> with which you can easily
	/// declare a binding between XML attributes and <see cref="T:Northwoods.Go.GoObject" /> properties,
	/// and which automatically makes a copy of a <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" /> object
	/// when consuming an XML element for this transformer.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Use this kind of transformer when you have complete control over the XML schema
	/// that you want to read and write and can follow the conventions established by this class.
	/// </para>
	/// <para>
	/// The following two transformers are similar to the SimpleXmlTransformBasicNode
	/// and SimpleXmlTransformLink transformer subclasses defined in the remarks for the
	/// <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> documentation.  For each <c>GoXmlBindingTransformer</c>
	/// you need to specify an element name, create a prototype object, and add bindings
	/// between attribute names and property names.
	/// </para>
	/// <code>
	/// // create a prototype node that is copied when consuming a "node" element
	/// GoBasicNode bn = new GoBasicNode();
	/// bn.LabelSpot = GoObject.Middle;
	/// bn.Text = "";
	///
	/// // ElementName is "node"; Prototype is this GoBasicNode
	/// GoXmlBindingTransformer bt1 = new GoXmlBindingTransformer("node", bn);
	///
	/// // generates attributes for all named ports, for GoBasicNode just "Port",
	/// // to define their id's without generating separate elements for the ports
	/// bt1.HandlesNamedPorts = true;
	///
	/// // read/write three attributes for each node, each attribute's value given by a property
	/// // note that the property can be a "path" of property names separated by periods,
	/// // when the desired value is not an immediate property on the object (the GoBasicNode in this case)
	/// bt1.AddBinding("label", "Text");
	/// bt1.AddBinding("color", "Shape.BrushColor");
	/// // typically the Location property is last, in case any of the previous
	/// // properties cause a change in node size and thus in the node's Location
	/// bt1.AddBinding("loc", "Location");
	///
	/// // register this transformer for GoBasicNodes and the element name "node"
	/// readerorwriter.AddTransformer(bt1);
	///
	/// // create a prototype link
	/// GoLabeledLink ll = new GoLabeledLink();
	/// GoText lab = new GoText();
	/// lab.Selectable = false;
	/// ll.MidLabel = lab;
	///
	/// // ElementName is "link"; Prototype is this GoLabeledLink
	/// GoXmlBindingTransformer bt2 = new GoXmlBindingTransformer("link", ll);
	///
	/// // read/write three attributes for each link, including the two ports and the MidLabel's string
	/// bt2.AddBinding("from", "FromPort");
	/// bt2.AddBinding("to", "ToPort");
	/// bt2.AddBinding("label", "MidLabel.Text");
	///
	/// // register this transformer for GoLabeledLinks and the element name "link"
	/// readerorwriter.AddTransformer(bt2);
	/// </code>
	/// <para>
	/// These two transformers produce an XML document such as:
	/// <code>
	/// &lt;graph&gt;
	///   &lt;node Port="0" label="Linen" color="-331546" loc="112 195" /&gt;
	///   &lt;node Port="1" label="DarkKhaki" color="-4343957" loc="221 155" /&gt;
	///   &lt;node Port="2" label="LightSteelBlue" color="-5192482" loc="237 221" /&gt;
	///   &lt;node Port="3" label="DeepSkyBlue" color="-16728065" loc="146 288" /&gt;
	///   &lt;link from="0" to="1" label="zeroone" /&gt;
	///   &lt;link from="1" to="2" label="onetwo" /&gt;
	///   &lt;link from="2" to="3" label="twothree" /&gt;
	///   &lt;link from="0" to="3" label="zerothree" /&gt;
	///   &lt;/graph&gt;
	/// </code>
	/// Note that the ports get unique identifiers, not the nodes,
	/// because <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> is true.
	/// If you also want the nodes to get unique identifiers,
	/// you can set <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.IdAttributeUsedForSharedObjects" /> to true.
	/// Neither identifier is at all related to an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
	/// </para>
	/// <para>
	/// As special cases, there are two property names that provide special behavior.
	/// For the <see cref="T:Northwoods.Go.GoStroke" />, <see cref="T:Northwoods.Go.GoLabeledLink" />, <see cref="T:Northwoods.Go.GoPolygon" />,
	/// and <see cref="T:Northwoods.Go.GoDrawing" /> classes, you can bind to the "Points" property
	/// in order to get or set the array of points used to define those shapes.
	/// The "Points" property is not treated specially for objects of any other type.
	/// </para>
	/// <para>
	/// Also as a special case for node classes, the "TreeParentNode" property binding is handled as a reference
	/// to another node, where the other node is considered to be the "parent" node in a tree-structured diagram.
	/// This supports the definition of XML that only has elements for nodes, with an implicit link from a
	/// "parent" node to the node whose element includes the attribute corresponding to the "TreeParentNode" property.
	/// The <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" /> property of this transformer provides a link that is copied when
	/// consuming such an element.  The "TreeParentNode" property is not treated specially for
	/// objects that are not instances of <c>IGoNode</c>.
	/// </para>
	/// <para>If you define a property binding using the special "TreeParentNode" property name,
	/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> should not be true,
	/// because <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> denotes nested XML elements,
	/// not the use of an attribute that refers to a node.
	/// It also does not make sense to have more than one attribute/property binding
	/// using the special "TreeParentNode" property name.
	/// </para>
	/// <code>
	/// // create a prototype node that is copied when consuming a "node" element
	/// GoBasicNode bn = new GoBasicNode();
	/// bn.LabelSpot = GoObject.Middle;
	/// bn.Text = "";
	///
	/// // ElementName is "node"; Prototype is this GoBasicNode
	/// GoXmlBindingTransformer bt1 = new GoXmlBindingTransformer("node", bn);
	///
	/// // all nodes get an "id" attribute
	/// bt1.IdAttributeUsedForSharedObjects = true;
	///
	/// // read/write three attributes for each node, each attribute's value given by a property
	/// // note that the property can be a "path" of property names separated by periods,
	/// // when the desired value is not an immediate property on the object (the GoBasicNode in this case)
	/// bt1.AddBinding("label", "Text");
	/// bt1.AddBinding("color", "Shape.BrushColor");
	/// // typically the Location property is last, in case any of the previous
	/// // properties cause a change in node size and thus in the node's Location
	/// bt1.AddBinding("loc", "Location");
	///
	/// // use the special "TreeParentNode" pseudo-property to indicate that the "parent"
	/// // attribute should be a reference to the node's parent node, if any
	/// bt1.AddBinding("parent", "TreeParentNode");
	/// // create a prototype link to connect a parent node to this transformer's node
	/// GoLink ll = new GoLink();
	/// ll.ToArrow = true;
	/// ll.Pen = new Pen(Color.Blue, 2);
	/// bt1.TreeLinkPrototype = ll;
	///
	/// // register this transformer for GoBasicNodes and the element name "node"
	/// readerorwriter.AddTransformer(bt1);
	/// </code>
	/// <para>
	/// This single transformer for <c>GoBasicNode</c> produces an XML document such as:
	/// <code>
	/// &lt;graph&gt;
	///   &lt;node id="0" label="Crimson" color="-2354116" loc="102 196" /&gt;
	///   &lt;node id="1" label="DarkCyan" color="-16741493" loc="210 157" parent="0" /&gt;
	///   &lt;node id="2" label="PaleGoldenrod" color="-1120086" loc="226 219" parent="0" /&gt;
	///   &lt;node id="3" label="Silver" color="-4144960" loc="343 254" parent="2" /&gt;
	///   &lt;node id="4" label="Purple" color="-8388480" loc="344 189" parent="2" /&gt;
	/// &lt;/graph&gt;
	/// </code>
	/// Note that the nodes have identifiers, not the ports.  These identifiers are not at all
	/// related to an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
	/// You can control which ports are used at each node in a tree diagram by setting
	/// the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeParentNodePortPath" /> and <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeChildNodePortPath" />
	/// properties of the transformer.
	/// You can also control whether links go from the parent node to the child node,
	/// or vice-versa, by setting the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinksToChildren" /> property.
	/// </para>
	/// <para>
	/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> and <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> also make special provision
	/// for a root that is a <see cref="T:Northwoods.Go.GoDocument" /> whose Type is associated with a
	/// <c>GoXmlBindingTransformer</c>.  When writing, the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" />
	/// is used as the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.RootElementName" />.  When reading,
	/// if there is no <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" />, the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" />
	/// is copied to be the <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" />.  This makes it easy to bind
	/// <see cref="T:Northwoods.Go.GoDocument" /> properties with XML root attributes.  However most of the other
	/// properties of transformers, such as <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" />
	/// or the other tree-related properties, are ignored when applied to the root object.
	/// </para>
	/// <para>
	/// Bindings are inherited from transformers in the same manner that their corresponding Types
	/// inherit properties.  So if you define a transformer for a Type <c>C</c> with a binding for
	/// the <c>CProp</c> property, and if you define a transformer for the Type <c>D</c> that inherits
	/// from <c>C</c>, the elements generated or consumed for instances of Type <c>D</c> will
	/// automatically include an attribute for the <c>CProp</c> property.
	/// </para>
	/// <para>
	/// The binding mechanism uses reflection, so your application will need permission to use reflection.
	/// Using reflection is also slower than the equivalent functionality implemented as regular code
	/// that you implement in overrides of <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateAttributes(System.Object)" /> and
	/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeAttributes(System.Object)" />.
	/// </para>
	/// </remarks>
	/// <seealso cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" />
	public class GoXmlBindingTransformer : GoXmlTransformer
	{
		internal class Binding
		{
			public string AttributeName;

			public string PropertyPath;

			public bool NoGet;

			public bool NoSet;

			public bool DefinesShared;

			public bool RethrowsException;

			internal string[] Properties;
		}

		private static bool myDefaultTracingEnabled;

		private object myPrototype;

		private bool myHandlesNamedPorts;

		private bool myHandlesChildren;

		private string myChildrenCollectionPath;

		private bool myHandlesChildAttributes;

		private bool myHandlesSubGraphCollapsedChildren;

		private bool myTreeStructured;

		private string myTreeParentNodePortPath;

		private string myTreeChildNodePortPath;

		private IGoLink myTreeLinkPrototype;

		private bool myTreeLinksToChildren = true;

		private bool myTracingEnabled;

		private List<Binding> myBindings = new List<Binding>();

		private Dictionary<string, object> myPathArrays = new Dictionary<string, object>();

		/// <summary>
		/// Gets or sets the object which will be copied and initialized
		/// when consuming an XML element with this transformer.
		/// </summary>
		/// <value>
		/// The value should either be a <c>GoObject</c> or <c>GoDocument</c> or implement <c>ICloneable</c>.
		/// This will be set in the constructor, and should not be
		/// replaced during generation or consumption.
		/// When the prototype is a <see cref="T:Northwoods.Go.GoDocument" /> and is the root,
		/// this transformer is used in a special manner to support binding document
		/// properties to root attributes.
		/// The documentation for <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.#ctor(System.String,System.Object)" /> provides more details.
		/// </value>
		public object Prototype
		{
			get
			{
				return myPrototype;
			}
			set
			{
				if (value == null || (!(value is GoObject) && !(value is ICloneable) && !(value is GoDocument)))
				{
					throw new ArgumentOutOfRangeException("Prototype must be a GoObject, a GoDocument, or implement ICloneable");
				}
				myPrototype = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the identifiers for named ports should
		/// be attributes on this element.
		/// </summary>
		/// <value>
		/// The default is false.  This may be set to true for those transformers whose objects contain ports.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is commonly set to true for node types that predefine a
		/// fixed set of ports, when it is not desired to represent
		/// information about each port besides the identifier.
		/// Thus it is natural to set this property to true for types such
		/// as <c>GoBasicNode</c>, <c>GoIconicNode</c>, <c>GoTextNode</c>,
		/// and <c>GoSimpleNode</c>.
		/// </para>
		/// <para>
		/// See also the description for <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String,Northwoods.Go.Xml.GoXmlBindingFlags)" />.
		/// </para>
		/// <para>
		/// Note that this does not produce nested XML elements, unlike <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" />.
		/// </para>
		/// <para>
		/// These port identifiers are not at all related to an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
		/// </para>
		/// </remarks>
		public bool HandlesNamedPorts
		{
			get
			{
				return myHandlesNamedPorts;
			}
			set
			{
				myHandlesNamedPorts = value;
			}
		}

		/// <summary>
		/// Gets or sets whether there are child objects of this <see cref="T:Northwoods.Go.GoGroup" /> that should be
		/// represented as nested XML elements in the body of this transformer's element.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// <para>
		/// If you are defining a transformer for a <see cref="T:Northwoods.Go.GoSubGraphBase" />-inheriting class,
		/// you'll want to set this to true.
		/// </para>
		/// <para>
		/// Setting this to true also automatically sets <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> to true.
		/// </para>
		/// <para>
		/// You may find that nested elements are being generated for child objects for which you do
		/// not want separate XML elements.  This may happen, for example, when you define a transformer
		/// for the <see cref="T:Northwoods.Go.GoText" /> class and for a node class that has a predefined label as
		/// well as a variable number of additional <see cref="T:Northwoods.Go.GoText" /> children.
		/// You have at least two choices.
		/// </para>
		/// <para>
		/// One choice is to replace all of those non-label <see cref="T:Northwoods.Go.GoText" /> child objects with
		/// instances of some class that inherits from <see cref="T:Northwoods.Go.GoText" />.  Because transformers are
		/// defined per type, this is an easy way to discriminate between the usages of text objects.
		/// Your transformer should then be defined for your custom class inheriting from <see cref="T:Northwoods.Go.GoText" />,
		/// instead of for <see cref="T:Northwoods.Go.GoText" /> itself.
		/// </para>
		/// <para>
		/// The other choice is to make the transformer for <see cref="T:Northwoods.Go.GoText" /> smarter.
		/// Use a transformer inheriting from <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> and override the
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SkipGeneration(System.Object)" /> method to return true when the text object
		/// is actually a label handled as a node attribute, and to return false when the text object
		/// is supposed to be represented as a separate nested XML element.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.ChildrenCollectionPath" />
		/// <seealso cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" />
		public bool HandlesChildren
		{
			get
			{
				return myHandlesChildren;
			}
			set
			{
				myHandlesChildren = value;
				base.BodyConsumesChildElements |= value;
			}
		}

		/// <summary>
		/// Gets or sets the property path to find the <see cref="T:Northwoods.Go.GoGroup" /> holding the
		/// child objects corresponding to nested elements when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> is true.
		/// </summary>
		/// <value>
		/// This defaults to the empty string, "", meaning that this transformer's object
		/// is the <see cref="T:Northwoods.Go.GoGroup" /> containing the child objects to be generated or consumed as nested XML elements.
		/// </value>
		/// <remarks>
		/// For some complicated node classes, the group that is actually supposed to hold a
		/// varying collection of objects is not the node itself but a child or grandchild of the
		/// node.  It is common for such a group to actually be a <c>GoListGroup</c> or something similar.
		/// This property just specifies a path from the transformer's object to that group.
		/// </remarks>
		public string ChildrenCollectionPath
		{
			get
			{
				return myChildrenCollectionPath;
			}
			set
			{
				if (myChildrenCollectionPath != value && value != null)
				{
					myChildrenCollectionPath = SavePropertyPath(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this transformer gets to read or write attributes on elements in the body of this element.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is usually set to true only when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> is also set to true.
		/// When this is true, <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" /> and <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" />
		/// will be called for each child object of this transformer's (parent) object.
		/// </para>
		/// <para>
		/// This property is infrequently used; but it is very handy when you want a "parent" object to
		/// associate attribute values with each "child" object, but those attributes are not properties
		/// on the "child" object.  The only pre-defined use of this property is to help save bounds or path
		/// information for each node or link inside a collapsed subgraph.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesSubGraphCollapsedChildren" />
		public bool HandlesChildAttributes
		{
			get
			{
				return myHandlesChildAttributes;
			}
			set
			{
				myHandlesChildAttributes = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" /> and <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" />
		/// handle collapsed <see cref="T:Northwoods.Go.GoSubGraph" />s information about the saved relative bounds or
		/// saved paths of subgraph child <see cref="T:Northwoods.Go.GoObject" />s.
		/// </summary>
		/// <value>
		/// This defaults to false, and is only effective when
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> and <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> are both true.
		/// </value>
		/// <remarks>
		/// You will probably want to set this to true for the transformer defined for your
		/// <see cref="T:Northwoods.Go.GoSubGraph" /> class.
		/// </remarks>
		public bool HandlesSubGraphCollapsedChildren
		{
			get
			{
				return myHandlesSubGraphCollapsedChildren;
			}
			set
			{
				myHandlesSubGraphCollapsedChildren = value;
			}
		}

		/// <summary>
		/// Gets or sets whether nested XML elements are treated as logical child nodes
		/// connected by links, rather than being structural parts of nodes like subgraphs.
		/// </summary>
		/// <value>
		/// The default is false.  This may be set to true for those transformers that represent nodes.
		/// </value>
		/// <remarks>
		/// <para>
		/// When this property is true, the XML will typically look like:
		/// <code>
		/// &lt;node id="0" label="Root" loc="71 190"&gt;
		///   &lt;node id="1" label="A1" loc="182 74" /&gt;
		///   &lt;node id="2" label="A2" loc="186 276"&gt;
		///     &lt;node id="3" label="B1" loc="272 236"&gt;
		///       &lt;node id="4" label="C1" loc="346 292"&gt;
		///         &lt;node id="5" label="D1" loc="445 259" /&gt;
		///         &lt;node id="6" label="D2" loc="448 348" /&gt;
		///       &lt;/node&gt;
		///     &lt;/node&gt;
		///     &lt;node id="7" label="B2" loc="287 358" /&gt;
		///   &lt;/node&gt;
		/// &lt;/node&gt;
		/// </code>
		/// </para>
		/// <para>
		/// When this property is true, it does not make sense to have an attribute/property
		/// binding using the special property named "TreeParentNode".  The "parent"
		/// of any node is implicit in the XML structure, so no node reference is necessary.
		/// </para>
		/// <para>
		/// Setting this to true also automatically sets <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> to true.
		/// </para>
		/// <para>
		/// Caution: when generating XML, be sure that the collection that you assign to the
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" /> property only contains
		/// the root nodes of the trees.  If the collection contains a child node as well as
		/// the root node, the XML element with nested elements will be generated in duplicate
		/// for that child node as well as for the whole tree starting at the root node.
		/// </para>
		/// </remarks>
		public bool TreeStructured
		{
			get
			{
				return myTreeStructured;
			}
			set
			{
				myTreeStructured = value;
				base.BodyConsumesChildElements |= value;
			}
		}

		/// <summary>
		/// Gets or sets the property path to find the port on a logical parent node
		/// to which a copy of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" /> will be connected,
		/// when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true or when using the "TreeParentNode" property binding.
		/// </summary>
		/// <value>
		/// This defaults to "Port", which should work for most node classes
		/// that are defined to have a single port.
		/// </value>
		public string TreeParentNodePortPath
		{
			get
			{
				return myTreeParentNodePortPath;
			}
			set
			{
				if (myTreeParentNodePortPath != value && value != null)
				{
					myTreeParentNodePortPath = SavePropertyPath(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the property path to find the port on a logical child node
		/// to which a copy of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" /> will be connected,
		/// when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true or when using the "TreeParentNode" property binding.
		/// </summary>
		/// <value>
		/// This defaults to "Port", which should work for most node classes
		/// that are defined to have a single port.
		/// </value>
		public string TreeChildNodePortPath
		{
			get
			{
				return myTreeChildNodePortPath;
			}
			set
			{
				if (myTreeChildNodePortPath != value && value != null)
				{
					myTreeChildNodePortPath = SavePropertyPath(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets an instance of a link that is copied to connect all
		/// new nodes in the constructed diagram,
		/// when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true or when using the "TreeParentNode" property binding.
		/// </summary>
		/// <value>
		/// The default value is null.
		/// This must be set when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true or when using the "TreeParentNode" property binding.
		/// </value>
		public IGoLink TreeLinkPrototype
		{
			get
			{
				return myTreeLinkPrototype;
			}
			set
			{
				if (value == null || value.GoObject == null)
				{
					throw new ArgumentOutOfRangeException("TreeLinkPrototype must be a GoObject that implements IGoLink ");
				}
				myTreeLinkPrototype = value;
			}
		}

		/// <summary>
		/// Gets or sets whether newly created links should connect from
		/// logical parent nodes to logical child nodes,
		/// when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true or when using the "TreeParentNode" property binding.
		/// </summary>
		/// <value>
		/// The default value is true.  A false value will connect links in the opposite direction.
		/// </value>
		public bool TreeLinksToChildren
		{
			get
			{
				return myTreeLinksToChildren;
			}
			set
			{
				myTreeLinksToChildren = value;
			}
		}

		/// <summary>
		/// Gets or sets whether to write information to trace listeners
		/// when tring to read or write properties that do not exist on a given object.
		/// </summary>
		/// <value>
		/// The default is false: when generating XML, nonexisting properties cause the
		/// attribute not to be written; when consuming XML, an attribute will be ignored
		/// if the property cannot be set.
		/// </value>
		/// <remarks>
		/// </remarks>
		public bool TracingEnabled
		{
			get
			{
				return myTracingEnabled;
			}
			set
			{
				myTracingEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets a shared/static variable that provides the default value
		/// for <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TracingEnabled" /> for newly created <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />s.
		/// </summary>
		/// <value>
		/// The default value is false.  Setting this property does not modify the
		/// value of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TracingEnabled" /> for any existing <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />.
		/// </value>
		public static bool DefaultTracingEnabled
		{
			get
			{
				return myDefaultTracingEnabled;
			}
			set
			{
				myDefaultTracingEnabled = value;
			}
		}

		/// <summary>
		/// Create a <c>GoXmlBindingTransformer</c> with a
		/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" /> that is the Type of the
		/// given <see cref="T:Northwoods.Go.GoObject" /> or <c>ICloneable</c> <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" />,
		/// and with an <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" /> that is that Type's Name.
		/// </summary>
		/// <param name="proto"></param>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.#ctor(System.String,System.Object)" />
		public GoXmlBindingTransformer(object proto)
		{
			InitCommon(proto);
			base.ElementName = proto.GetType().Name;
		}

		/// <summary>
		/// Create a <c>GoXmlBindingTransformer</c> with a given <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" />
		/// and a given <see cref="T:Northwoods.Go.GoObject" /> or <c>ICloneable</c> <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" />,
		/// whose Type is the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />.
		/// </summary>
		/// <param name="eltname"></param>
		/// <param name="proto"></param>
		/// <remarks>
		/// <para>
		/// There is a special use of <c>GoXmlBindingTransformer</c> when it applies
		/// to a <see cref="T:Northwoods.Go.GoDocument" /> that is the root.
		/// If the prototype object is a <see cref="T:Northwoods.Go.GoDocument" /> and if it is the value of
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />,
		/// this transformer's <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" /> is used
		/// as the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.RootElementName" />,
		/// and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootAttributes" /> will call this transformer's
		/// <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateAttributes(System.Object)" /> method.
		/// If the prototype object is a <see cref="T:Northwoods.Go.GoDocument" /> and if an instance of it is the value of
		/// <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" /> will call this transformer's
		/// <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeAttributes(System.Object)" /> method.
		/// This permits the use of this binding mechanism for binding root attributes
		/// with <see cref="T:Northwoods.Go.GoDocument" /> properties.
		/// </para>
		/// <para>
		/// However, reading such a document will not make a copy of the <see cref="T:Northwoods.Go.GoDocument" /> prototype.
		/// </para>
		/// </remarks>
		public GoXmlBindingTransformer(string eltname, object proto)
		{
			InitCommon(proto);
			base.ElementName = eltname;
		}

		private void InitCommon(object proto)
		{
			if (proto == null)
			{
				throw new ArgumentNullException("proto");
			}
			TracingEnabled = DefaultTracingEnabled;
			Prototype = proto;
			base.TransformerType = proto.GetType();
			GoLink goLink = new GoLink();
			goLink.ToArrow = true;
			TreeLinkPrototype = goLink;
			ChildrenCollectionPath = "";
			TreeParentNodePortPath = "Port";
			TreeChildNodePortPath = "Port";
		}

		/// <summary>
		/// Associate an attribute on this transformer's element with a property value on
		/// this transformer's type of object.
		/// </summary>
		/// <param name="attrname">an XML attribute name</param>
		/// <param name="proppath">a .NET property name, or a sequence of properties separated by periods</param>
		/// <param name="flags"><see cref="T:Northwoods.Go.Xml.GoXmlBindingFlags" /> flags describing the nature of the binding</param>
		/// <remarks>
		/// <para>
		/// See <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" /> for examples.
		/// </para>
		/// <para>
		/// This override also allow you to specify that this attribute, during consumption,
		/// defines the identifier by which a reference object may be referred to.
		/// The referred-to object is typically a child <see cref="T:Northwoods.Go.GoObject" /> of a <see cref="T:Northwoods.Go.GoGroup" />.
		/// For example, if you add a <see cref="T:Northwoods.Go.GoPort" /> to your custom <see cref="T:Northwoods.Go.GoNode" /> class,
		/// but do not want to have a separate XML element defining and describing that port, you will
		/// still need to generate the identifier for that port so that links will be able to
		/// find that port.  If your node class defines a property that refers to your extra port,
		/// you can call this method to make sure the identifier is written for the port and that
		/// reading this attribute will make sure that node's port is given that identifier.
		/// <code>transformer.AddBinding("xport", "SpecialPort", GoXmlBindingFlags.DefinesShared)</code>
		/// </para>
		/// <para>
		/// The order in which bindings are added determines the order in which properties are set
		/// when the attributes are consumed.  Since setting some properties, such as the text string
		/// for a label, may modify the position or the location of a node, you should add the
		/// binding for a node's position or location last.
		/// </para>
		/// <para>
		/// Setting the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> property is similar to defining a shared
		/// attribute property path for each of the ports on a node that has a name, as returned by
		/// <see cref="M:Northwoods.Go.GoGroup.FindName(Northwoods.Go.GoObject)" />.
		/// </para>
		/// <para>
		/// As special cases, there are two property names that provide special behavior.
		/// For the <see cref="T:Northwoods.Go.GoStroke" />, <see cref="T:Northwoods.Go.GoLabeledLink" />, <see cref="T:Northwoods.Go.GoPolygon" />,
		/// and <see cref="T:Northwoods.Go.GoDrawing" /> classes, you can bind to the "Points" property
		/// in order to get or set the array of points used to define those shapes.
		/// The "Points" property is not treated specially for objects of any other type.
		/// </para>
		/// <para>
		/// Also as a special case for node classes, the "TreeParentNode" property binding is handled as a reference
		/// to another node, where the other node is considered to be the "parent" node in a tree-structured diagram.
		/// This supports the definition of XML that only has elements for nodes, with an implicit link from a
		/// "parent" node to the node whose element includes the attribute corresponding to the "TreeParentNode" property.
		/// The <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" /> property of this transformer provides a link that is copied when
		/// consuming such an element.  The "TreeParentNode" property is not treated specially for
		/// objects that are not instances of <c>IGoNode</c>.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" />
		public void AddBinding(string attrname, string proppath, GoXmlBindingFlags flags)
		{
			if (attrname == null || attrname == "")
			{
				throw new ArgumentNullException("attrname");
			}
			if (proppath == null || proppath == "")
			{
				throw new ArgumentNullException("proppath");
			}
			Binding binding = new Binding();
			binding.AttributeName = attrname;
			binding.Properties = MakePropertiesArray(proppath);
			binding.PropertyPath = MakePropertyPath(binding.Properties);
			binding.NoGet = ((flags & GoXmlBindingFlags.NoGet) != 0);
			binding.NoSet = ((flags & GoXmlBindingFlags.NoSet) != 0);
			binding.DefinesShared = ((flags & GoXmlBindingFlags.DefinesShared) != 0);
			binding.RethrowsException = ((flags & GoXmlBindingFlags.RethrowsExceptions) != 0);
			myBindings.Add(binding);
		}

		/// <summary>
		/// Associate an attribute on this transformer's element with a property value on
		/// this transformer's type of object.
		/// </summary>
		/// <param name="attrname">an XML attribute name</param>
		/// <param name="proppath">a .NET property name, or a sequence of properties separated by periods</param>
		/// <remarks>
		/// <para>
		/// <code>transformer.AddBinding("label", "Text")</code>
		/// will generate a "label" attribute whose value is the value of the "Text" property
		/// on the object being generated.  When the element is consumed, the "Text" property
		/// of the newly copied object will be set to the value of the "label" attribute.
		/// </para>
		/// <para>
		/// This mechanism can handle all of the data types for which there are predefined methods
		/// in <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> to read and write attribute values.  For example, if
		/// the data type of the property is <c>Color</c>, it will read the attribute using the
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ColorAttr(System.String,System.Drawing.Color)" /> method and will write the attribute using the
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.Color)" /> method.
		/// </para>
		/// <para>
		/// To support access to properties that are not defined on the transformer's
		/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" /> Type, the <paramref name="proppath" /> can be a "path"
		/// of properties, separated by periods.  For example:
		/// <code>transformer.AddBinding("img", "Image.Name")</code>
		/// where the <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> is defined for a <see cref="T:Northwoods.Go.GoIconicNode" />,
		/// will read and write the value of the <c>GoIconicNode.Image.Name</c> property.
		/// </para>
		/// <para>
		/// As special cases, there are two property names that provide special behavior.
		/// For the <see cref="T:Northwoods.Go.GoStroke" />, <see cref="T:Northwoods.Go.GoLabeledLink" />, <see cref="T:Northwoods.Go.GoPolygon" />,
		/// and <see cref="T:Northwoods.Go.GoDrawing" /> classes, you can bind to the "Points" property
		/// in order to get or set the array of points used to define those shapes.
		/// The "Points" property is not treated specially for objects of any other type.
		/// </para>
		/// <para>
		/// Also as a special case for node classes, the "TreeParentNode" property binding is handled as a reference
		/// to another node, where the other node is considered to be the "parent" node in a tree-structured diagram.
		/// This supports the definition of XML that only has elements for nodes, with an implicit link from a
		/// "parent" node to the node whose element includes the attribute corresponding to the "TreeParentNode" property.
		/// The <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" /> property of this transformer provides a link that is copied when
		/// consuming such an element.  The "TreeParentNode" property is not treated specially for
		/// objects that are not instances of <c>IGoNode</c>.
		/// </para>
		/// <para>
		/// This mechanism has special code to handle <c>Brush</c>es and <c>Pen</c>s,
		/// because those types must not be modified.  Instead, they are cloned, the property
		/// is set, and then the brush or pen property is set on the shape object.
		/// However, in most cases, you should bind to the <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.BrushColor" />
		/// and perhaps other <c>GoShape.Brush...</c> properties, and to the <see cref="P:Northwoods.Go.GoShape.PenColor" />
		/// and <see cref="P:Northwoods.Go.GoShape.PenWidth" /> properties, since those properties can handle the cases
		/// when there is no <see cref="P:Northwoods.Go.GoShape.Brush" /> or <see cref="P:Northwoods.Go.GoShape.Pen" /> value.
		/// </para>
		/// </remarks>
		public void AddBinding(string attrname, string proppath)
		{
			AddBinding(attrname, proppath, GoXmlBindingFlags.Default);
		}

		/// <summary>
		/// Associate an attribute on this transformer's element with the same named property value on
		/// this transformer's type of object.
		/// </summary>
		/// <param name="prop">a property name, without periods, that will also be the attribute name</param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" />.
		/// </remarks>
		public void AddBinding(string prop)
		{
			if (prop == null || prop.IndexOf('.') >= 0)
			{
				throw new ArgumentOutOfRangeException("Attribute or Property name cannot contain '.'");
			}
			AddBinding(prop, prop, GoXmlBindingFlags.Default);
		}

		private string SavePropertyPath(string path)
		{
			if (path == null)
			{
				return null;
			}
			string[] props = MakePropertiesArray(path);
			string text = MakePropertyPath(props);
			myPathArrays[path] = text;
			return text;
		}

		private string[] MakePropertiesArray(string path)
		{
			if (path == "")
			{
				return new string[0];
			}
			string[] array = path.Split('.');
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i] != "")
				{
					num++;
				}
			}
			string[] array2 = new string[num];
			int num2 = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null && array[j] != "")
				{
					array2[num2++] = array[j];
				}
			}
			return array2;
		}

		private string MakePropertyPath(string[] props)
		{
			string text = "";
			foreach (string str in props)
			{
				if (text.Length > 0)
				{
					text += ".";
				}
				text += str;
			}
			return text;
		}

		internal static void Trace(string msg)
		{
			System.Diagnostics.Trace.WriteLine(msg);
		}

		private object GetPropByPath(object obj, string path)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (path == null || path == "")
			{
				return obj;
			}
			myPathArrays.TryGetValue(path, out object value);
			string[] array = value as string[];
			if (array != null)
			{
				return GetPropByPath(obj, array, 0, path);
			}
			return GetPropByPath(obj, MakePropertiesArray(path), 0, path);
		}

		private object GetPropByPath(object obj, string[] props, int pidx, string path)
		{
			if (obj == null)
			{
				return null;
			}
			string text = props[pidx];
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				if (TracingEnabled)
				{
					Trace("No property '" + text + "' on type: " + type.FullName + ", for property path '" + MakePropertyPath(props) + "'");
				}
				return null;
			}
			if (pidx >= props.Length - 1)
			{
				return GetPropValue(property, obj, path, rethrows: false);
			}
			object propValue = GetPropValue(property, obj, path, rethrows: false);
			return GetPropByPath(propValue, props, pidx + 1, path);
		}

		private object GetPropValue(PropertyInfo pinfo, object obj, string proppath, bool rethrows)
		{
			PropertyInfo propertyInfo = pinfo;
			while (propertyInfo != null && !propertyInfo.CanRead)
			{
				propertyInfo = propertyInfo.DeclaringType.BaseType.GetProperty(pinfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (propertyInfo == null || !propertyInfo.CanRead)
			{
				if (TracingEnabled)
				{
					Trace("No property getter for property '" + pinfo.Name + "' on type: " + obj.GetType().FullName + ", for property path '" + proppath + "'");
				}
			}
			else
			{
				try
				{
					return propertyInfo.GetValue(obj, null);
				}
				catch (Exception ex)
				{
					if (TracingEnabled)
					{
						Trace("Exception getting property '" + pinfo.Name + "' on type: " + obj.GetType().FullName + ", for property path '" + proppath + "'\n" + ex.Message);
					}
					TargetInvocationException ex2 = ex as TargetInvocationException;
					if (ex2 != null && rethrows)
					{
						throw ex2.InnerException;
					}
				}
			}
			return null;
		}

		private void SetPropValue(PropertyInfo pinfo, object obj, object val, string proppath, bool rethrows)
		{
			PropertyInfo propertyInfo = pinfo;
			while (propertyInfo != null && !propertyInfo.CanWrite)
			{
				propertyInfo = propertyInfo.DeclaringType.BaseType.GetProperty(pinfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (propertyInfo == null || !propertyInfo.CanWrite)
			{
				if (TracingEnabled)
				{
					Trace("No property setter for property '" + pinfo.Name + "' on type: " + obj.GetType().FullName + ", for property path '" + proppath + "'");
				}
			}
			else
			{
				try
				{
					propertyInfo.SetValue(obj, val, null);
				}
				catch (Exception ex)
				{
					if (TracingEnabled)
					{
						Trace("Exception setting property '" + pinfo.Name + "' on type: " + obj.GetType().FullName + ", for property path '" + proppath + "'\n" + ex.Message);
					}
					TargetInvocationException ex2 = ex as TargetInvocationException;
					if (ex2 != null && rethrows)
					{
						throw ex2.InnerException;
					}
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" />'s <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />'s
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> property is true, call <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" /> on
		/// that parent class's transformer to allow it to add attributes to the new element.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>the result of the call to the base method</returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" /> only when the base call to
		/// <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateElement(System.Object)" /> returned true.
		/// </remarks>
		public override bool GenerateElement(object obj)
		{
			bool num = base.GenerateElement(obj);
			if (num)
			{
				object parentObject = base.Writer.ParentObject;
				if (parentObject != null)
				{
					GoXmlBindingTransformer goXmlBindingTransformer = base.Writer.FindTransformer(parentObject.GetType()) as GoXmlBindingTransformer;
					if (goXmlBindingTransformer != null && goXmlBindingTransformer.HandlesChildAttributes)
					{
						goXmlBindingTransformer.GenerateChildAttributes(parentObject, obj, this);
					}
				}
			}
			return num;
		}

		/// <summary>
		/// This calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GeneratePortReferences(System.Object)" /> (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> is true)
		/// and <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateAttributeProperties(System.Object)" />.
		/// </summary>
		/// <param name="obj"></param>
		public override void GenerateAttributes(object obj)
		{
			base.GenerateAttributes(obj);
			if (HandlesNamedPorts)
			{
				GeneratePortReferences(obj);
			}
			GenerateAttributeProperties(obj);
		}

		/// <summary>
		/// Generate attributes defined by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" />.
		/// </summary>
		/// <param name="obj"></param>
		protected void GenerateAttributeProperties(object obj)
		{
			foreach (Binding myBinding in myBindings)
			{
				GenAttr(obj, myBinding, 0);
			}
		}

		/// <summary>
		/// Write attribute values that are references to named ports on the node <paramref name="obj" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This is called by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateAttributes(System.Object)" /> when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> is true.
		/// This method does nothing for <paramref name="obj" /> objects that are not instances of <see cref="T:Northwoods.Go.GoGroup" />.
		/// For each <see cref="T:Northwoods.Go.GoPort" /> that is an immediate child object of the node,
		/// it calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrRef(System.String,System.Object)" /> to write an attribute
		/// whose name is the group child name for the port and whose value is a reference identifier.
		/// These port identifiers are not at all related to an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
		/// All attributes defined by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" /> or
		/// otherwise generated by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateAttributes(System.Object)" /> must not duplicate any port names.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumePortReferences(System.Object)" />
		protected virtual void GeneratePortReferences(object obj)
		{
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup)
				{
					GoPort goPort = item as GoPort;
					if (goPort != null)
					{
						string text = goGroup.FindName(goPort);
						if (text != null && text != "")
						{
							WriteAttrRef(text, goPort);
						}
					}
				}
			}
		}

		private void GenAttr(object obj, Binding ap, int pidx)
		{
			if (ap.NoGet)
			{
				return;
			}
			string attributeName = ap.AttributeName;
			string[] properties = ap.Properties;
			string text = properties[pidx];
			if (pidx >= properties.Length - 1)
			{
				GenAttrVal(obj, attributeName, text, ap.RethrowsException);
				return;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				if (TracingEnabled)
				{
					Trace("No property '" + text + "' on type: " + type.FullName + ", for attribute '" + attributeName + "'");
				}
				return;
			}
			object propValue = GetPropValue(property, obj, ap.PropertyPath, ap.RethrowsException);
			if (propValue == null)
			{
				if (TracingEnabled)
				{
					Trace("No value for property '" + text + "' on type: " + type.FullName + " for attribute '" + attributeName + "'");
				}
			}
			else
			{
				GenAttr(propValue, ap, pidx + 1);
			}
		}

		private void GenAttrVal(object obj, string attrname, string prop, bool rethrows)
		{
			if (prop == null || prop == "")
			{
				return;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				if (!GenAttrValSpecial(obj, attrname, prop) && TracingEnabled)
				{
					Trace("Property '" + prop + "' not found on type: " + type.FullName + " for attribute '" + attrname + "'");
				}
				return;
			}
			object propValue = GetPropValue(property, obj, prop, rethrows);
			if (propValue is string)
			{
				WriteAttrVal(attrname, (string)propValue);
			}
			else if (propValue is int)
			{
				WriteAttrVal(attrname, (int)propValue);
			}
			else if (propValue is Enum)
			{
				WriteAttrVal(attrname, propValue.ToString());
			}
			else if (propValue is float)
			{
				WriteAttrVal(attrname, (float)propValue);
			}
			else if (propValue is double)
			{
				WriteAttrVal(attrname, (double)propValue);
			}
			else if (propValue is bool)
			{
				WriteAttrVal(attrname, (bool)propValue);
			}
			else if (propValue is PointF)
			{
				WriteAttrVal(attrname, (PointF)propValue);
			}
			else if (propValue is SizeF)
			{
				WriteAttrVal(attrname, (SizeF)propValue);
			}
			else if (propValue is RectangleF)
			{
				WriteAttrVal(attrname, (RectangleF)propValue);
			}
			else if (propValue is Color)
			{
				WriteAttrVal(attrname, (Color)propValue);
			}
			else if (propValue is Type)
			{
				WriteAttrVal(attrname, (Type)propValue);
			}
			else if (propValue is int[])
			{
				WriteAttrVal(attrname, (int[])propValue);
			}
			else if (propValue is float[])
			{
				WriteAttrVal(attrname, (float[])propValue);
			}
			else if (propValue is PointF[])
			{
				WriteAttrVal(attrname, (PointF[])propValue);
			}
			else if (propValue is Color[])
			{
				WriteAttrVal(attrname, (Color[])propValue);
			}
			else if (!(propValue is ValueType) && property.PropertyType != typeof(string))
			{
				WriteAttrRef(attrname, propValue);
			}
		}

		private bool GenAttrValSpecial(object obj, string attrname, string prop)
		{
			if (prop == "Points")
			{
				if (obj is GoStroke)
				{
					WriteAttrVal(attrname, ((GoStroke)obj).CopyPointsArray());
					return true;
				}
				if (obj is GoLabeledLink)
				{
					WriteAttrVal(attrname, ((GoLabeledLink)obj).RealLink.CopyPointsArray());
					return true;
				}
				if (obj is GoPolygon)
				{
					WriteAttrVal(attrname, ((GoPolygon)obj).CopyPointsArray());
					return true;
				}
				if (obj is GoDrawing)
				{
					GoDrawingData data = ((GoDrawing)obj).Data;
					WriteAttrVal(attrname, data.Points);
					int[] array = new int[data.Actions.Length];
					Array.Copy(data.Actions, array, array.Length);
					WriteAttrVal(attrname + "Actions", array);
					return true;
				}
			}
			else if (prop == "TreeParentNode")
			{
				IGoNode goNode = obj as IGoNode;
				if (goNode != null)
				{
					GoSubGraphBase goSubGraphBase = obj as GoSubGraphBase;
					if (TreeLinksToChildren)
					{
						IEnumerable<IGoNode> enumerable;
						if (goSubGraphBase == null)
						{
							enumerable = goNode.Sources;
						}
						else
						{
							IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalSources;
							enumerable = enumerable2;
						}
						using (IEnumerator<IGoNode> enumerator = enumerable.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								IGoNode current = enumerator.Current;
								WriteAttrRef(attrname, current.GoObject);
								return true;
							}
						}
					}
					else
					{
						IEnumerable<IGoNode> enumerable3;
						if (goSubGraphBase == null)
						{
							enumerable3 = goNode.Destinations;
						}
						else
						{
							IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalDestinations;
							enumerable3 = enumerable2;
						}
						using (IEnumerator<IGoNode> enumerator = enumerable3.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								IGoNode current2 = enumerator.Current;
								WriteAttrRef(attrname, current2.GoObject);
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// This calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" />
		/// for child objects (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> is true) or for
		/// logical-child nodes (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true).
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Whether this considers the <see cref="P:Northwoods.Go.IGoNode.Destinations" /> nodes or the
		/// <see cref="P:Northwoods.Go.IGoNode.Sources" /> nodes depends on the value of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinksToChildren" />.
		/// </remarks>
		public override void GenerateDefinitions(object obj)
		{
			base.GenerateDefinitions(obj);
			if (HandlesChildren)
			{
				GoGroup goGroup = GetPropByPath(obj, ChildrenCollectionPath) as GoGroup;
				if (goGroup != null)
				{
					foreach (GoObject item in goGroup)
					{
						base.Writer.DefineObject(item);
					}
				}
			}
			if (!TreeStructured)
			{
				return;
			}
			IGoNode goNode = obj as IGoNode;
			if (goNode == null)
			{
				return;
			}
			GoSubGraphBase goSubGraphBase = obj as GoSubGraphBase;
			if (TreeLinksToChildren)
			{
				IEnumerable<IGoNode> enumerable;
				if (goSubGraphBase == null)
				{
					enumerable = goNode.Destinations;
				}
				else
				{
					IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalDestinations;
					enumerable = enumerable2;
				}
				foreach (IGoNode item2 in enumerable)
				{
					base.Writer.DefineObject(item2);
				}
			}
			else
			{
				IEnumerable<IGoNode> enumerable3;
				if (goSubGraphBase == null)
				{
					enumerable3 = goNode.Sources;
				}
				else
				{
					IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalSources;
					enumerable3 = enumerable2;
				}
				foreach (IGoNode item3 in enumerable3)
				{
					base.Writer.DefineObject(item3);
				}
			}
		}

		/// <summary>
		/// This calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />
		/// for child objects (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> is true) or for
		/// logical-child nodes (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true).
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Whether this considers the <see cref="P:Northwoods.Go.IGoNode.Destinations" /> nodes or the
		/// <see cref="P:Northwoods.Go.IGoNode.Sources" /> nodes depends on the value of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinksToChildren" />.
		/// </remarks>
		public override void GenerateBody(object obj)
		{
			base.GenerateBody(obj);
			if (HandlesChildren)
			{
				GoGroup goGroup = GetPropByPath(obj, ChildrenCollectionPath) as GoGroup;
				if (goGroup != null)
				{
					foreach (GoObject item in goGroup)
					{
						base.Writer.GenerateObject(item);
					}
				}
			}
			if (!TreeStructured)
			{
				return;
			}
			IGoNode goNode = obj as IGoNode;
			if (goNode == null)
			{
				return;
			}
			GoSubGraphBase goSubGraphBase = obj as GoSubGraphBase;
			if (TreeLinksToChildren)
			{
				IEnumerable<IGoNode> enumerable;
				if (goSubGraphBase == null)
				{
					enumerable = goNode.Destinations;
				}
				else
				{
					IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalDestinations;
					enumerable = enumerable2;
				}
				foreach (IGoNode item2 in enumerable)
				{
					base.Writer.GenerateObject(item2);
				}
			}
			else
			{
				IEnumerable<IGoNode> enumerable3;
				if (goSubGraphBase == null)
				{
					enumerable3 = goNode.Sources;
				}
				else
				{
					IEnumerable<IGoNode> enumerable2 = goSubGraphBase.ExternalSources;
					enumerable3 = enumerable2;
				}
				foreach (IGoNode item3 in enumerable3)
				{
					base.Writer.GenerateObject(item3);
				}
			}
		}

		/// <summary>
		/// This handles nested XML elements when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> or
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> are true.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <remarks>
		/// <para>
		/// When <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> or <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.GeneratesPortsAsChildElements" /> is true,
		/// and when the <paramref name="parent" /> is a <see cref="T:Northwoods.Go.GoGroup" /> and
		/// the <paramref name="child" /> is a <see cref="T:Northwoods.Go.GoObject" />,
		/// this will add the child object to the group.
		/// </para>
		/// <para>
		/// To support some flexibility to allow the <see cref="T:Northwoods.Go.GoGroup" /> be a property
		/// of the <paramref name="parent" /> object instead of requiring the <paramref name="parent" />
		/// to be the <see cref="T:Northwoods.Go.GoGroup" /> itself, the child <see cref="T:Northwoods.Go.GoObject" /> is added to the
		/// <paramref name="parent" />'s <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.ChildrenCollectionPath" /> property,
		/// if that value is a <see cref="T:Northwoods.Go.GoGroup" />.
		/// </para>
		/// <para>
		/// When <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" /> is true the XML element in the body
		/// produces a separate node that is connected by a link to this node.
		/// Both the <paramref name="parent" /> and the <paramref name="child" />
		/// are assumed to implement <see cref="T:Northwoods.Go.IGoNode" />.
		/// This method makes a copy of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinkPrototype" />.
		/// It finds the port that is the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeParentNodePortPath" />
		/// property on the <paramref name="parent" /> node, and it finds the port
		/// that is the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeChildNodePortPath" /> property on the
		/// <paramref name="child" /> node.
		/// Then it sets the <see cref="T:Northwoods.Go.IGoLink" />.<see cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// and <see cref="P:Northwoods.Go.IGoLink.ToPort" /> properties of the new link to those
		/// ports.  If <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinksToChildren" /> is true, the
		/// <see cref="P:Northwoods.Go.IGoLink.FromPort" /> is assigned the <paramref name="parent" />'s port,
		/// and the <see cref="P:Northwoods.Go.IGoLink.ToPort" /> is assigned the <paramref name="child" />'s port.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeLinksToChildren" /> is false, the ports are exchanged so
		/// that the link goes in the opposite direction.
		/// The child node and the link are then added to the
		/// <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" />.
		/// </para>
		/// </remarks>
		public override void ConsumeChild(object parent, object child)
		{
			base.ConsumeChild(parent, child);
			if (HandlesChildren || base.GeneratesPortsAsChildElements)
			{
				GoGroup goGroup = GetPropByPath(parent, ChildrenCollectionPath) as GoGroup;
				GoObject goObject = child as GoObject;
				if (goGroup != null && goObject != null)
				{
					goGroup.Add(goObject);
				}
			}
			if (TreeStructured)
			{
				IGoNode goNode = parent as IGoNode;
				IGoNode goNode2 = child as IGoNode;
				if (goNode != null && goNode2 != null)
				{
					AddParentLink(goNode, goNode2);
				}
			}
		}

		private void AddParentLink(IGoNode parentnode, IGoNode childnode)
		{
			IGoPort goPort = GetPropByPath(parentnode, TreeParentNodePortPath) as IGoPort;
			IGoPort goPort2 = GetPropByPath(childnode, TreeChildNodePortPath) as IGoPort;
			if (goPort == null || goPort2 == null)
			{
				return;
			}
			IGoLink goLink = TreeLinkPrototype.GoObject.Copy() as IGoLink;
			if (TreeLinksToChildren)
			{
				goLink.FromPort = goPort;
				goLink.ToPort = goPort2;
			}
			else
			{
				goLink.FromPort = goPort2;
				goLink.ToPort = goPort;
			}
			IGoCollection goCollection = base.Reader.RootObject as IGoCollection;
			if (goCollection != null)
			{
				goCollection.Add(childnode.GoObject);
				goCollection.Add(goLink.GoObject);
				return;
			}
			IList list = base.Reader.RootObject as IList;
			if (list != null)
			{
				list.Add(childnode.GoObject);
				list.Add(goLink.GoObject);
			}
			else if (base.Reader.RootObject is ICollection<GoObject>)
			{
				list.Add(childnode.GoObject);
				list.Add(goLink.GoObject);
			}
		}

		/// <summary>
		/// Construct an object for this element by making a copy of the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" />.
		/// </summary>
		/// <returns>a copy of the value of <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" /></returns>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" /> is a <see cref="T:Northwoods.Go.GoObject" /> or a <see cref="T:Northwoods.Go.GoDocument" />,
		/// this returns the value of <see cref="M:Northwoods.Go.GoObject.Copy" />.
		/// If the <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" /> implements the <c>ICloneable</c> interface,
		/// this returns the value of <c>Clone()</c>.
		/// Otherwise this raises an exception.
		/// When a copy is made, and when the <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" />'s <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />'s
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> property is true, this calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" /> on
		/// that parent class's transformer to allow it to consume attributes of this current element.
		/// </remarks>
		public override object Allocate()
		{
			object obj = null;
			GoObject goObject = Prototype as GoObject;
			if (goObject != null)
			{
				obj = goObject.Copy();
			}
			else
			{
				ICloneable cloneable = Prototype as ICloneable;
				if (cloneable != null)
				{
					obj = cloneable.Clone();
				}
				else
				{
					GoDocument goDocument = Prototype as GoDocument;
					if (goDocument == null)
					{
						throw new Exception("Don't know how to copy: " + Prototype.ToString());
					}
					obj = goDocument.Copy();
				}
			}
			if (obj != null)
			{
				object parentObject = base.Reader.ParentObject;
				if (parentObject != null)
				{
					GoXmlBindingTransformer goXmlBindingTransformer = base.Reader.FindTransformer(parentObject.GetType()) as GoXmlBindingTransformer;
					if (goXmlBindingTransformer != null && goXmlBindingTransformer.HandlesChildAttributes)
					{
						goXmlBindingTransformer.ConsumeChildAttributes(parentObject, obj, this);
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// This calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumePortReferences(System.Object)" /> (if <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> is true),
		/// and <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeAttributeProperties(System.Object)" />.
		/// </summary>
		/// <param name="obj"></param>
		public override void ConsumeAttributes(object obj)
		{
			base.ConsumeAttributes(obj);
			if (HandlesNamedPorts)
			{
				ConsumePortReferences(obj);
			}
			ConsumeAttributeProperties(obj);
		}

		/// <summary>
		/// Consume attributes defined by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" />
		/// </summary>
		/// <param name="obj"></param>
		protected void ConsumeAttributeProperties(object obj)
		{
			foreach (Binding myBinding in myBindings)
			{
				ConsAttr(obj, myBinding, 0);
			}
		}

		/// <summary>
		/// Read attribute values that are references to named ports on the node <paramref name="obj" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This is called by <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeAttributes(System.Object)" /> when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesNamedPorts" /> is true.
		/// This method does nothing for <paramref name="obj" /> objects that are not instances of <see cref="T:Northwoods.Go.GoGroup" />.
		/// For each <see cref="T:Northwoods.Go.GoPort" /> that is an immediate child object of the node,
		/// and that has a group child name,
		/// it calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.StringAttr(System.String,System.String)" /> to read an identifier
		/// for that port.
		/// If the identifier is a non-empty string, it calls
		/// <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.MakeShared(System.String,System.Object)" />
		/// to define the ID for that port.
		/// These port identifiers are not at all related to an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
		/// If the identifier is an empty string, the port will not get a shared
		/// identifier, so it will not be able to be found by other attributes/properties
		/// that refer to it, but the port remains on the node.
		/// If the attribute is not present in the element, the port is removed from the node.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GeneratePortReferences(System.Object)" />
		protected virtual void ConsumePortReferences(object obj)
		{
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				GoCollection goCollection = null;
				foreach (GoObject item in goGroup)
				{
					GoPort goPort = item as GoPort;
					if (goPort != null)
					{
						string text = goGroup.FindName(goPort);
						if (text != null && text != "")
						{
							string text2 = StringAttr(text, null);
							if (text2 == null)
							{
								if (goCollection == null)
								{
									goCollection = new GoCollection();
								}
								goCollection.Add(goPort);
							}
							else if (!(text2 == ""))
							{
								base.Reader.MakeShared(text2, goPort);
							}
						}
					}
				}
				if (goCollection != null)
				{
					foreach (GoObject item2 in goCollection)
					{
						item2.Remove();
					}
				}
			}
		}

		private void ConsAttr(object obj, Binding ap, int pidx)
		{
			if (ap.NoSet)
			{
				return;
			}
			string attributeName = ap.AttributeName;
			if (!IsAttrPresent(attributeName))
			{
				return;
			}
			string[] properties = ap.Properties;
			string text = properties[pidx];
			if (pidx >= properties.Length - 1)
			{
				ConsAttrVal(obj, attributeName, text, ap);
				return;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				if (TracingEnabled)
				{
					Trace("No property '" + text + "' on type: " + type.FullName + ", for attribute '" + attributeName + "' with value: " + StringAttr(attributeName, ""));
				}
				return;
			}
			object propValue = GetPropValue(property, obj, ap.PropertyPath, ap.RethrowsException);
			if (propValue == null)
			{
				if (TracingEnabled)
				{
					Trace("No value for property '" + text + "' on type: " + type.FullName + " for attribute '" + attributeName + "' with value: " + StringAttr(attributeName, ""));
				}
				return;
			}
			if (property.PropertyType.IsValueType)
			{
				ConsAttr(propValue, ap, pidx + 1);
				SetPropValue(property, obj, propValue, ap.PropertyPath, ap.RethrowsException);
				return;
			}
			object obj2 = ConsNewObject(propValue);
			ConsAttr(obj2, ap, pidx + 1);
			if (obj2 != propValue)
			{
				SetPropValue(property, obj, obj2, ap.PropertyPath, ap.RethrowsException);
			}
		}

		private object ConsNewObject(object obj)
		{
			if (obj is Brush)
			{
				return ((Brush)obj).Clone();
			}
			if (obj is Pen)
			{
				return ((Pen)obj).Clone();
			}
			return obj;
		}

		private void ConsAttrVal(object obj, string attrname, string prop, Binding ap)
		{
			if (prop == null || prop == "")
			{
				return;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				if (!ConsAttrValSpecial(obj, attrname, prop) && TracingEnabled)
				{
					Trace("Property '" + prop + "' not found on type: " + type.FullName + " for attribute '" + attrname + "' with value: " + StringAttr(attrname, ""));
				}
				return;
			}
			object propValue = GetPropValue(property, obj, ap.PropertyPath, ap.RethrowsException);
			Type propertyType = property.PropertyType;
			object val;
			if (propertyType == typeof(string))
			{
				val = StringAttr(attrname, (string)propValue);
			}
			else if (propertyType == typeof(int))
			{
				val = Int32Attr(attrname, (int)propValue);
			}
			else if (propertyType.IsEnum)
			{
				val = Enum.Parse(propertyType, StringAttr(attrname, propValue.ToString()), ignoreCase: true);
			}
			else if (propertyType == typeof(float))
			{
				val = SingleAttr(attrname, (float)propValue);
			}
			else if (propertyType == typeof(double))
			{
				val = DoubleAttr(attrname, (double)propValue);
			}
			else if (propertyType == typeof(bool))
			{
				val = BooleanAttr(attrname, (bool)propValue);
			}
			else if (propertyType == typeof(PointF))
			{
				val = PointFAttr(attrname, (PointF)propValue);
			}
			else if (propertyType == typeof(SizeF))
			{
				val = SizeFAttr(attrname, (SizeF)propValue);
			}
			else if (propertyType == typeof(RectangleF))
			{
				val = RectangleFAttr(attrname, (RectangleF)propValue);
			}
			else if (propertyType == typeof(Color))
			{
				val = ColorAttr(attrname, (Color)propValue);
			}
			else if (propertyType == typeof(Type))
			{
				val = TypeAttr(attrname, (Type)propValue);
			}
			else if (propertyType == typeof(int[]))
			{
				val = Int32ArrayAttr(attrname, (int[])propValue);
			}
			else if (propertyType == typeof(float[]))
			{
				val = SingleArrayAttr(attrname, (float[])propValue);
			}
			else if (propertyType == typeof(PointF[]))
			{
				val = PointFArrayAttr(attrname, (PointF[])propValue);
			}
			else
			{
				if (!(propertyType == typeof(Color[])))
				{
					if (propertyType.IsValueType)
					{
						return;
					}
					if (ap.DefinesShared)
					{
						if (propValue != null)
						{
							string key = StringAttr(attrname, null);
							base.Reader.MakeShared(key, propValue);
						}
						return;
					}
					val = RefAttr(attrname, null);
					if (val != null)
					{
						SetPropValue(property, obj, val, ap.PropertyPath, ap.RethrowsException);
						return;
					}
					string text = StringAttr(attrname, null);
					if (text != null && text != "null")
					{
						base.Reader.AddDelayedRef(obj, ap.PropertyPath, text);
					}
					return;
				}
				val = ColorArrayAttr(attrname, (Color[])propValue);
			}
			SetPropValue(property, obj, val, ap.PropertyPath, ap.RethrowsException);
		}

		private bool ConsAttrValSpecial(object obj, string attrname, string prop)
		{
			if (prop == "Points")
			{
				if (obj is GoStroke)
				{
					((GoStroke)obj).SetPoints(PointFArrayAttr(attrname, null));
					return true;
				}
				if (obj is GoLabeledLink)
				{
					((GoLabeledLink)obj).RealLink.SetPoints(PointFArrayAttr(attrname, null));
					return true;
				}
				if (obj is GoPolygon)
				{
					((GoPolygon)obj).SetPoints(PointFArrayAttr(attrname, null));
					return true;
				}
				if (obj is GoDrawing)
				{
					PointF[] array = PointFArrayAttr(attrname, null);
					int[] array2 = Int32ArrayAttr(attrname + "Actions", null);
					if (array != null && array2 != null)
					{
						byte[] array3 = new byte[array2.Length];
						for (int i = 0; i < array3.Length; i++)
						{
							array3[i] = (byte)array2[i];
						}
						((GoDrawing)obj).Data = new GoDrawingData(array3, array);
					}
					return true;
				}
			}
			else if (prop == "TreeParentNode")
			{
				IGoNode goNode = obj as IGoNode;
				if (goNode != null)
				{
					IGoNode goNode2 = RefAttr(attrname, null) as IGoNode;
					if (goNode2 != null)
					{
						AddParentLink(goNode2, goNode);
					}
					else
					{
						string text = StringAttr(attrname, null);
						if (text != null && text != "null")
						{
							base.Reader.AddDelayedRef(obj, prop, text);
						}
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Fix up references to objects that did not yet exist when the attribute was read.
		/// </summary>
		/// <param name="obj">the object with a property that could not be set before, because the referred-to object was not available</param>
		/// <param name="proppath">a property path registered with a call to <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.AddBinding(System.String,System.String)" /></param>
		/// <param name="referred">the resolved object reference</param>
		public override void UpdateReference(object obj, string proppath, object referred)
		{
			if (referred != null)
			{
				foreach (Binding myBinding in myBindings)
				{
					if (myBinding.PropertyPath == proppath)
					{
						SetAttr(obj, myBinding, 0, referred);
						return;
					}
				}
				base.UpdateReference(obj, proppath, referred);
			}
		}

		private void SetAttr(object obj, Binding ap, int pidx, object referred)
		{
			string[] properties = ap.Properties;
			string text = properties[pidx];
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pidx >= properties.Length - 1)
			{
				if (property == null)
				{
					if (!SetAttrSpecial(obj, text, referred) && TracingEnabled)
					{
						Trace("Property not found: " + text + " on type: " + type.FullName);
					}
				}
				else if (property.PropertyType.IsInstanceOfType(referred))
				{
					property.SetValue(obj, referred, null);
				}
				else if (TracingEnabled)
				{
					Trace("Referent not found for: " + referred.ToString() + " for attribute '" + ap.AttributeName + "'");
				}
			}
			else
			{
				object propValue = GetPropValue(property, obj, ap.PropertyPath, ap.RethrowsException);
				SetAttr(propValue, ap, pidx + 1, referred);
			}
		}

		private bool SetAttrSpecial(object obj, string prop, object referred)
		{
			if (prop == "TreeParentNode")
			{
				IGoNode goNode = referred as IGoNode;
				IGoNode goNode2 = obj as IGoNode;
				if (goNode != null && goNode2 != null)
				{
					AddParentLink(goNode, goNode2);
				}
				else if (TracingEnabled)
				{
					Trace("Not a 'tree parent' IGoNode: " + referred.ToString() + " for " + obj.ToString());
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// This is called on the <paramref name="parent" />'s <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />
		/// for each <paramref name="child" /> object, when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> and
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> are true,
		/// to allow a parent object to add attributes to each child element.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <param name="childtransformer">the <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> being used to write the <paramref name="child" /> object</param>
		/// <remarks>
		/// <para>
		/// For example, this can be used to help generate a dictionary that maps key values
		/// to child objects by writing a "key" attribute value on each child object.
		/// This could also be used to store a "value" attribute value on each child object,
		/// assuming the child object itself is the key in the dictionary.
		/// </para>
		/// <para>
		/// By default this does nothing, unless <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesSubGraphCollapsedChildren" />
		/// and <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> are both true and this <paramref name="parent" /> is
		/// a <see cref="T:Northwoods.Go.GoSubGraph" /> and the <paramref name="child" /> is a <see cref="T:Northwoods.Go.GoObject" />.
		/// If those conditions are true, this will write out saved bounds or saved path
		/// information for each child object if the subgraph is not expanded.
		/// </para>
		/// <para>
		/// Here is an example transformer for <see cref="T:Northwoods.Go.GoSubGraph" />:
		/// </para>
		/// <code>
		/// GoSubGraph sg = new GoSubGraph();
		/// ... do subgraph initialization ...
		/// GoXmlBindingTransformer st = new GoXmlBindingTransformer("subgraph", sg);
		/// st.HandlesNamedPorts = true;
		/// // generates children and consumes them by adding to the subgraph
		/// st.HandlesChildren = true;
		/// // make sure reading/writing each child calls the Generate/ConsumeChildAttributes methods
		/// st.HandlesChildAttributes = true;
		/// // actually read/write SavedBounds and SavedPath attributes on children, when collapsed
		/// st.HandlesSubGraphCollapsedChildren = true;
		/// st.AddBinding("back", "BackgroundColor");
		/// st.AddBinding("opacity", "Opacity");
		/// st.AddBinding("border", "BorderPen.Color");
		/// st.AddBinding("borderwidth", "BorderPen.Width");
		/// st.AddBinding("loc", "Location");
		/// // SavedBounds, and SavedPaths if collapsed on each child, handled by Generate/ConsumeChildAttributes
		/// // define these AFTER defining Location binding
		/// st.AddBinding("wasexpanded", "WasExpanded");
		/// st.AddBinding("expanded", "IsExpanded");
		/// readerorwriter.AddTransformer(st);
		/// </code>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" />
		protected virtual void GenerateChildAttributes(object parent, object child, GoXmlBindingTransformer childtransformer)
		{
			if (!HandlesSubGraphCollapsedChildren)
			{
				return;
			}
			GoSubGraph goSubGraph = parent as GoSubGraph;
			GoObject goObject = child as GoObject;
			if (goSubGraph != null && goObject != null && !goSubGraph.IsExpanded)
			{
				if (goSubGraph.SavedBounds.ContainsKey(goObject))
				{
					RectangleF val = goSubGraph.SavedBounds[goObject];
					childtransformer.WriteAttrVal("GoSubGraph.SavedBounds", val);
				}
				if (goSubGraph.SavedPaths.ContainsKey(goObject))
				{
					PointF[] val2 = goSubGraph.SavedPaths[goObject];
					childtransformer.WriteAttrVal("GoSubGraph.SavedPath", val2);
				}
			}
		}

		/// <summary>
		/// This is called on the <paramref name="parent" />'s <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />
		/// for each <paramref name="child" /> object, when <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildren" /> and
		/// <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> are true,
		/// to allow a parent object to read attributes of each child element.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <param name="childtransformer">the <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> being used to read the <paramref name="child" /> object</param>
		/// <remarks>
		/// <para>
		/// For example, this can be used to reconstruct a dictionary mapping key values
		/// to child objects when the key values are attributes on each child element.
		/// This could also be used to store a "value" attribute value on each child object,
		/// assuming the child object itself is the key in the dictionary.
		/// </para>
		/// <para>
		/// By default this does nothing, unless <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesSubGraphCollapsedChildren" />
		/// and <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.HandlesChildAttributes" /> are both true and this <paramref name="parent" /> is
		/// a <see cref="T:Northwoods.Go.GoSubGraph" /> and the <paramref name="child" /> is a <see cref="T:Northwoods.Go.GoObject" />.
		/// If those conditions are true, this will read in saved bounds or saved path
		/// information for each child object if the subgraph is not expanded.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateChildAttributes(System.Object,System.Object,Northwoods.Go.Xml.GoXmlBindingTransformer)" />
		protected virtual void ConsumeChildAttributes(object parent, object child, GoXmlBindingTransformer childtransformer)
		{
			if (!HandlesSubGraphCollapsedChildren)
			{
				return;
			}
			GoSubGraph goSubGraph = parent as GoSubGraph;
			GoObject goObject = child as GoObject;
			if (goSubGraph != null && goObject != null && !goSubGraph.IsExpanded)
			{
				if (childtransformer.IsAttrPresent("GoSubGraph.SavedBounds"))
				{
					RectangleF value = childtransformer.RectangleFAttr("GoSubGraph.SavedBounds", default(RectangleF));
					goSubGraph.SavedBounds[goObject] = value;
					goObject.Visible = false;
					goObject.Printable = false;
				}
				if (childtransformer.IsAttrPresent("GoSubGraph.SavedPath"))
				{
					PointF[] value2 = childtransformer.PointFArrayAttr("GoSubGraph.SavedPath", new PointF[0]);
					goSubGraph.SavedPaths[goObject] = value2;
					goObject.Visible = false;
					goObject.Printable = false;
				}
			}
		}
	}
}
