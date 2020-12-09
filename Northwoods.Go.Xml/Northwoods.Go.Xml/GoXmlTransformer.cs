using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// This abstract class holds methods called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />
	/// and <see cref="T:Northwoods.Go.Xml.GoXmlReader" />,
	/// used to generate or parse XML for objects of a particular class.
	/// </summary>
	/// <remarks>
	/// <para>
	/// When you define transformers for your application's object classes,
	/// you will want to inherit from this class.
	/// This provides standard implementations for all of the
	/// <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> methods, which basically just call the
	/// same method on the base type's transformer.
	/// </para>
	/// <para>
	/// To make it easier to access the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Writer" />'s and
	/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Reader" />'s properties and methods,
	/// a number of their properties and methods are provided here also,
	/// whose implementations just delegate to the writer or reader.
	/// An instance of this class can only be used for one <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />
	/// or <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> at a time, not for both at once.
	/// </para>
	/// </remarks>
	/// <example>
	/// This continues the examples given with the description of <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />
	/// and <see cref="T:Northwoods.Go.Xml.GoXmlReader" />,
	/// defining simple transformers for <c>GoBasicNode</c> and <c>GoLabeledLink</c>.
	/// <code>
	///   public class SimpleXmlTransformBasicNode : GoXmlTransformer {
	///     public SimpleXmlTransformBasicNode() {
	///       this.TransformerType = typeof(GoBasicNode); 
	///       this.ElementName = "node";
	///       this.IdAttributeUsedForSharedObjects = true;
	///     }
	///
	///     public override void GenerateAttributes(Object obj) {
	///       base.GenerateAttributes(obj);
	///       GoBasicNode n = (GoBasicNode)obj;
	///       WriteAttrVal("label", n.Text);
	///       WriteAttrVal("shapebounds", n.SelectionObject.Bounds);
	///     }
	///
	///     public override Object Allocate() {
	///       GoBasicNode n = new GoBasicNode();
	///       // you might want to do other common initialization here
	///       return n;
	///     }
	///     public override void ConsumeAttributes(Object obj) {
	///       base.ConsumeAttributes(obj);
	///       GoBasicNode n = (GoBasicNode)obj;
	///       n.Text = StringAttr("label", "");
	///       n.SelectionObject.Bounds = RectangleFAttr("shapebounds", new RectangleF(100, 100, 40, 50));
	///     }
	///   }
	///
	///   public class SimpleXmlTransformLink : GoXmlTransformer {
	///     public SimpleXmlTransformLink() {
	///       this.TransformerType = typeof(GoLabeledLink);
	///       this.ElementName = "link";
	///     }
	///
	///     public override void GenerateAttributes(Object obj) {
	///       base.GenerateAttributes(obj);
	///       GoLabeledLink link = (GoLabeledLink)obj;
	///       GoNode n = link.FromNode as GoNode;
	///       WriteAttrVal("from", this.Writer.FindShared(n));
	///       n = link.ToNode as GoNode;
	///       WriteAttrVal("to", this.Writer.FindShared(n));
	///       GoText lab = link.MidLabel as GoText;
	///       if (lab != null)
	///         WriteAttrVal("label", lab.Text);
	///     }
	///
	///     public override Object Allocate() {
	///       GoLabeledLink ll = new GoLabeledLink();
	///       GoText lab = new GoText();
	///       lab.Selectable = false;
	///       ll.MidLabel = lab;
	///       // you might want to do other common initialization here
	///       return ll;
	///     }
	///     public override void ConsumeAttributes(Object obj) {
	///       base.ConsumeAttributes(obj);
	///       GoLabeledLink link = (GoLabeledLink)obj;
	///       String fromid = StringAttr("from", null);
	///       GoBasicNode from = this.Reader.FindShared(fromid) as GoBasicNode;
	///       if (from != null) {
	///         link.FromPort = from.Port;
	///       }
	///       String toid = StringAttr("to", null);
	///       GoBasicNode to = this.Reader.FindShared(toid) as GoBasicNode;
	///       if (to != null) {
	///         link.ToPort = to.Port;
	///       }
	///       GoText lab = link.MidLabel as GoText;
	///       if (lab != null &amp;&amp; IsAttrPresent("label")) {
	///         lab.Text = StringAttr("label", lab.Text);
	///       }
	///     }
	///   }
	/// </code>
	/// </example>
	public abstract class GoXmlTransformer : IGoXmlTransformer
	{
		private GoXmlWriter myWriter;

		private GoXmlReader myReader;

		private Type myTransformerType;

		private string myElementName;

		private bool myIdAttributeUsedForSharedObjects;

		private bool myGeneratesPortsAsChildElements;

		private bool myBodyConsumesChildElements;

		private IGoXmlTransformer myInheritsFromTransformer;

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> that is using this transformer.
		/// </summary>
		/// <value>This must be non-null when this transformer is being used for writing.</value>
		public GoXmlWriter Writer
		{
			get
			{
				return myWriter;
			}
			set
			{
				myWriter = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> that is using this transformer.
		/// </summary>
		/// <value>This must be non-null when this transformer is being used for reading.</value>
		public GoXmlReader Reader
		{
			get
			{
				return myReader;
			}
			set
			{
				myReader = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the principal element used to render this object.
		/// </summary>
		/// <value>
		/// The name should be a local name -- it should not have any prefix and colon embedded in it.
		/// </value>
		public string ElementName
		{
			get
			{
				return myElementName;
			}
			set
			{
				myElementName = value;
			}
		}

		/// <summary>
		/// Returns the <c>Type</c> for which these transformer methods apply.
		/// </summary>
		public Type TransformerType
		{
			get
			{
				return myTransformerType;
			}
			set
			{
				myTransformerType = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this transformer reads/writes the "id" attribute
		/// using the identifier associated with shared objects.
		/// </summary>
		/// <value>The initial value is false.</value>
		/// <remarks>
		/// Setting this property to true will cause <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateDefinitions(System.Object)" />
		/// to call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.MakeShared(System.Object)" /> in order to associate an
		/// identifier with the object, and it will cause
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateAttributes(System.Object)" /> to write out an "id" attribute whose
		/// value is that identifier, as returned by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.FindShared(System.Object)" />.
		/// When reading, <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeAttributes(System.Object)" /> will look at the value of
		/// the "id" attribute of the current element and (if present) will call
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.MakeShared(System.String,System.Object)" /> to associate that identifier with
		/// the object.  Your reading code can then call <see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" />
		/// to try to resolve an identifier into a reference.
		/// </remarks>
		public bool IdAttributeUsedForSharedObjects
		{
			get
			{
				return myIdAttributeUsedForSharedObjects;
			}
			set
			{
				myIdAttributeUsedForSharedObjects = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this transformer writes child elements
		/// representing ports (<c>IGoPort</c>), if this transformer is operating on an <c>IGoNode</c>.
		/// </summary>
		/// <value>The initial value is false.</value>
		/// <remarks>
		/// <para>
		/// Setting this property to true will cause <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateDefinitions(System.Object)" />
		/// to enumerate all of the <c>IGoPort</c>s for the given <c>IGoNode</c> (assuming
		/// the argument object indeed implements <c>IGoNode</c>) and call
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" /> on each port.
		/// A true value will also cause <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateBody(System.Object)" /> to enumerate all the ports
		/// and call <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" /> on each one.
		/// </para>
		/// <para>
		/// Note that this property only affects generation.  Typically you
		/// will also want to set <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> to true
		/// and override <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" /> to add the port to be a child of
		/// this parent node.
		/// </para>
		/// </remarks>
		public bool GeneratesPortsAsChildElements
		{
			get
			{
				return myGeneratesPortsAsChildElements;
			}
			set
			{
				myGeneratesPortsAsChildElements = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> should iterate over all of
		/// the child elements of this XML node, calling <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />
		/// for each one, and passing the result to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" />.
		/// </summary>
		/// <value>The initial value is false.</value>
		/// <remarks>
		/// <para>
		/// See the documentation for <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> and <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" />.
		/// </para>
		/// <para>
		/// Note that this property only affects consumption.  Typically if you
		/// are working with a simple (non-subgraph) node that contains some ports,
		/// you will also want to set the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.GeneratesPortsAsChildElements" />
		/// property to true and make sure a transformer is registered for the port
		/// type(s), so that child elements are generated for the ports.
		/// </para>
		/// </remarks>
		public bool BodyConsumesChildElements
		{
			get
			{
				return myBodyConsumesChildElements;
			}
			set
			{
				myBodyConsumesChildElements = value;
			}
		}

		/// <summary>
		/// Gets or sets a transformer from which this transformer will inherit implementation
		/// of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> methods.
		/// </summary>
		/// <value>this defaults to null</value>
		/// <remarks>
		/// <para>
		/// Normally when you want to extend the behavior of a <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> or
		/// a <see cref="T:Northwoods.Go.Xml.GoXmlReader" />, you define your own transformer inheriting from
		/// <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> and add it to the writer and/or the reader.
		/// You can do this for additional <c>Type</c>s, or you can replace the transformer
		/// for a <c>Type</c>.
		/// </para>
		/// <para>
		/// Sometimes you would like to modify the behavior of a transformer but for some reason
		/// you are unable to inherit from that <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />-implementing class.
		/// This property makes it easier to insert additional functionality into an existing
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> by replacing a particular <c>Type</c>'s transformer with
		/// your own transformer that does some stuff and then delegates the rest of the
		/// implementation to the old transformer that had been registered for that <c>Type</c>.
		/// </para>
		/// <para>
		/// For example, imagine that you are using a <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />
		/// such as <c>GoSvgWriter</c> that already
		/// knows how to handle many <c>Type</c>s, but you want to add some attributes and
		/// some elements to what is generated for that <c>Type</c>.  If you cannot easily
		/// define your own transformer class inheriting from the <c>Type</c>'s existing 
		/// transformer class, you could do something such as the following to add information
		/// to each generated SVG element for <c>GoSubGraph</c>s.
		/// <code>
		/// public class ExtraSubGraphTransformer : GoSvgGenerator {
		///   public ExtraSubGraphTransformer() { this.TransformerType = typeof(GoSubGraph); }
		///   public override void GenerateAttributes(Object obj) {
		///     WriteAttrVal("extra", "attribute");
		///     base.GenerateAttributes(obj);
		///   }
		///   public override void GenerateBody(Object obj) {
		///     WriteStartElement("extra");
		///     WriteTextBody("element");
		///     WriteEndElement();
		///     base.GenerateBody(obj);
		///   }
		/// }
		/// </code>
		/// Then where you initialize the <c>GoSvgWriter</c>, you can replace the transformer
		/// for <c>GoSubGraph</c> with your own transformer that also makes use of the old one.
		/// <code>
		///   GoSvgWriter w = new GoSvgWriter();
		///   ExtraSubGraphTransformer egt = new ExtraSubGraphTransformer();
		///   egt.InheritsFromTransformer = w.GetTransformer(typeof(GoSubGraph));
		///   w.SetTransformer(typeof(GoSubGraph), egt);
		///   . . . more initialization and use of the writer
		/// </code>
		/// </para>
		/// </remarks>
		public IGoXmlTransformer InheritsFromTransformer
		{
			get
			{
				return myInheritsFromTransformer;
			}
			set
			{
				myInheritsFromTransformer = value;
			}
		}

		/// <summary>
		/// Get the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Writer" />'s <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />.
		/// </summary>
		public XmlWriter XmlWriter => Writer.XmlWriter;

		/// <summary>
		/// Get the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Reader" />'s <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />.
		/// </summary>
		public XmlReader XmlReader => Reader.XmlReader;

		/// <summary>
		/// Get the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Reader" />'s or the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Writer" />'s <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlDocument" />.
		/// </summary>
		public XmlDocument XmlDocument
		{
			get
			{
				if (Reader != null)
				{
					return Reader.XmlDocument;
				}
				if (Writer != null)
				{
					return Writer.XmlDocument;
				}
				return null;
			}
		}

		/// <summary>
		/// Get the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Reader" />'s <see cref="P:Northwoods.Go.Xml.GoXmlReader.ReaderNode" />, an <c>XmlNode</c>.
		/// </summary>
		/// <remarks>
		/// This value is undefined (and should be null) when writing, or when reading
		/// and either <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is false or the consuming process
		/// is not at a point where there would be an <c>XmlNode</c> that is being consumed.
		/// </remarks>
		public XmlNode ReaderNode
		{
			get
			{
				return Reader.ReaderNode;
			}
			set
			{
				Reader.ReaderNode = value;
			}
		}

		/// <summary>
		/// Get the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Writer" />'s <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />, an <c>XmlElement</c>.
		/// </summary>
		/// <remarks>
		/// This value is undefined (and should be null) when reading, or when writing
		/// directly to a stream or when the generation process is not at a point where
		/// there would be an <c>XmlElement</c> that is being constructed.
		/// </remarks>
		public XmlElement WriterElement
		{
			get
			{
				return Writer.WriterElement;
			}
			set
			{
				Writer.WriterElement = value;
			}
		}

		/// <summary>
		/// Return true if the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.Writer" /> should not generate XML for an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" />
		/// and by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SkipGeneration(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeSkipGeneration(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual bool SkipGeneration(object obj)
		{
			if (InheritsFromTransformer != null)
			{
				return InheritsFromTransformer.SkipGeneration(obj);
			}
			return Writer.InvokeSkipGeneration(TransformerType.BaseType, obj);
		}

		/// <summary>
		/// Generate elements for the definitions section of the document, rendering any shared objects
		/// that this object needs to refer to.
		/// </summary>
		/// <param name="obj">the object being generated</param>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" />.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.IdAttributeUsedForSharedObjects" /> is true,
		/// this first associates an identifier with the given <paramref name="obj" />
		/// by calling <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.MakeShared(System.Object)" />
		/// This method then calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateDefinitions(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateDefinitions(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual void GenerateDefinitions(object obj)
		{
			if (IdAttributeUsedForSharedObjects)
			{
				Writer.MakeShared(obj);
			}
			if (GeneratesPortsAsChildElements)
			{
				IGoNode goNode = obj as IGoNode;
				if (goNode != null)
				{
					foreach (IGoPort port in goNode.Ports)
					{
						Writer.DefineObject(port.GoObject);
					}
				}
			}
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.GenerateDefinitions(obj);
			}
			else
			{
				Writer.InvokeGenerateDefinitions(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Start producing an XML element for a given object.
		/// </summary>
		/// <param name="obj">the object being generated</param>
		/// <returns>true if it started an element</returns>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />.
		/// By default this starts an element named by <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" />, if it is non-null.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" /> is null, this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateElement(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateElement(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual bool GenerateElement(object obj)
		{
			if (ElementName != null)
			{
				WriteStartElement(ElementName);
				return true;
			}
			if (InheritsFromTransformer != null)
			{
				return InheritsFromTransformer.GenerateElement(obj);
			}
			return Writer.InvokeGenerateElement(TransformerType.BaseType, obj);
		}

		/// <summary>
		/// Generate attributes for the current element, helping to render an object.
		/// </summary>
		/// <param name="obj">the object being generated</param>
		/// <remarks>
		/// <para>
		/// You will typically override this method to call methods such as <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.String)" />
		/// or <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Single)" /> to read property values and set them on the object <paramref name="obj" />
		/// being initialized.  The set of attributes usually matches those consumed in an override of
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeAttributes(System.Object)" />.
		/// </para>
		/// <para>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.IdAttributeUsedForSharedObjects" /> is true,
		/// this method also sees if <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.FindShared(System.Object)" />
		/// has an identifier associated with the given <paramref name="obj" />.
		/// If so, this writes out an "id" attribute with that identifier string.
		/// This method then calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateAttributes(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateAttributes(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </para>
		/// <para>
		/// You should not try to define any "xmlns" namespaces.
		/// Instead, you can define the default namespace for the root element
		/// by setting <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" />,
		/// and you can define namespaces and their prefixes by calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />
		/// before XML generation.
		/// </para>
		/// </remarks>
		public virtual void GenerateAttributes(object obj)
		{
			if (IdAttributeUsedForSharedObjects)
			{
				string text = Writer.FindShared(obj);
				if (text != null)
				{
					XmlWriter xmlWriter = XmlWriter;
					if (xmlWriter != null)
					{
						xmlWriter.WriteAttributeString("id", text);
					}
					else if (WriterElement != null)
					{
						WriterElement.SetAttribute("id", text);
					}
				}
			}
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.GenerateAttributes(obj);
			}
			else
			{
				Writer.InvokeGenerateAttributes(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Generate any nested elements that are part of the rendering of an object.
		/// </summary>
		/// <param name="obj">the object being generated</param>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateBody(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateBody(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual void GenerateBody(object obj)
		{
			if (GeneratesPortsAsChildElements)
			{
				IGoNode goNode = obj as IGoNode;
				if (goNode != null)
				{
					foreach (IGoPort port in goNode.Ports)
					{
						Writer.GenerateObject(port.GoObject);
					}
				}
			}
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.GenerateBody(obj);
			}
			else
			{
				Writer.InvokeGenerateBody(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Finish generating any elements that were started by <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateElement(System.Object)" />.
		/// </summary>
		/// <param name="obj">the object being generated</param>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateElementFinish(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateElementFinish(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual void GenerateElementFinish(object obj)
		{
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.GenerateElementFinish(obj);
			}
			else
			{
				Writer.InvokeGenerateElementFinish(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Override this method to construct a new object for the current XML element.
		/// </summary>
		/// <returns>
		/// An object of type <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />, or null if no object
		/// needs to be created for the current element.
		/// </returns>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />.
		/// By default this returns a new instance of <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />,
		/// if that property is not null and represents a class.
		/// This should be overridden to construct an object.
		/// The construction may need to use the values of some attributes on the current XML element
		/// in order to decide what to construct or which constructor to call.
		/// If this is called by a nested call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />
		/// within an implementation of <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" />, then the
		/// <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" /> property will refer to the object for
		/// which the body is being consumed.
		/// The <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" /> can be used to find and return an
		/// existing child of the parent object, rather than constructing a new object as the child.
		/// Note that unlike the other implementations of <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> methods,
		/// this method does not default to calling <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeAllocate" /> on
		/// the base <c>Type</c>, since that is unlikely to create an instance of the desired type.
		/// </remarks>
		public virtual object Allocate()
		{
			Type transformerType = TransformerType;
			if (transformerType != null && transformerType.IsClass)
			{
				return Activator.CreateInstance(transformerType);
			}
			return null;
		}

		/// <summary>
		/// Consume attributes for the current element, helping to initialize most of the object's properties.
		/// </summary>
		/// <param name="obj">the object being constructed</param>
		/// <remarks>
		/// <para>
		/// </para>
		/// You will typically override this method to call methods such as <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.StringAttr(System.String,System.String)" />
		/// or <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SingleAttr(System.String,System.Single)" /> to read property values and set them on the object <paramref name="obj" />
		/// being initialized.  The set of attributes usually matches those generated in an override of
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateAttributes(System.Object)" />, although sometimes some of the attributes are needed in
		/// the override of <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Allocate" />.
		/// <para>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeAttributes(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeAttributes(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.IdAttributeUsedForSharedObjects" /> is true,
		/// this method also looks at the "id" attribute and, if present, will associate that
		/// identifier with the given <paramref name="obj" /> by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.MakeShared(System.String,System.Object)" />.
		/// </para>
		/// </remarks>
		public virtual void ConsumeAttributes(object obj)
		{
			if (IdAttributeUsedForSharedObjects)
			{
				string text = StringAttr("id", null);
				if (text != null)
				{
					Reader.MakeShared(text, obj);
				}
			}
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.ConsumeAttributes(obj);
			}
			else
			{
				Reader.InvokeConsumeAttributes(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Consume elements that help define more details or parts of an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// <para>
		/// If you want to read child (nested) XML elements and have transformer(s)
		/// defined for those kinds of elements, it is sufficient to set
		/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> to true and to override
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" /> to add those child objects to the parent object.
		/// For more sophisticated parsing/traversing of the child XML elements, you may
		/// want to override this method to do what you need.
		/// </para>
		/// <para>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> is true, this method
		/// iterates through the child elements of this XML node, calls
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" /> on each one, and then passes the resulting
		/// object to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" />.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.XmlReader" /> is available, because <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" />
		/// is false, this method keeps reading until it encounters an <c>EndElement</c> XML node.
		/// Otherwise, if we are traversing a DOM, this method enumerates each of the
		/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ReaderNode" />'s XML <c>ChildNodes</c>.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> is false,
		/// this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeBody(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </para>
		/// </remarks>
		public virtual void ConsumeBody(object obj)
		{
			if (BodyConsumesChildElements)
			{
				XmlReader xmlReader = XmlReader;
				if (xmlReader != null)
				{
					if (xmlReader.IsEmptyElement)
					{
						return;
					}
					string name = xmlReader.Name;
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							object child = Reader.ConsumeObject();
							Reader.InvokeConsumeChild(TransformerType, obj, child);
						}
						else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == name)
						{
							break;
						}
					}
				}
				else if (ReaderNode != null)
				{
					XmlNode readerNode = ReaderNode;
					{
						IEnumerator enumerator = readerNode.ChildNodes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								XmlNode xmlNode2 = ReaderNode = (XmlNode)enumerator.Current;
								object child2 = Reader.ConsumeObject();
								Reader.InvokeConsumeChild(TransformerType, obj, child2);
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					ReaderNode = readerNode;
				}
			}
			else if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.ConsumeBody(obj);
			}
			else
			{
				Reader.InvokeConsumeBody(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> is true,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> will call <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />
		/// on each child XML element and pass the resulting object to this method.
		/// </summary>
		/// <param name="parent">the result of an earlier call to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Allocate" />
		/// that was passed to a call to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /></param>
		/// <param name="child">the result of the call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />; may be null</param>
		/// <remarks>
		/// <para>
		/// Typically you would override this method to add the child to the parent in
		/// the appropriate manner, or perhaps to set a property of the parent to refer
		/// to the child object.
		/// Note that this method will not get called by <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> unless
		/// <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.BodyConsumesChildElements" /> is true or unless some
		/// transformer's implementation of <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" /> does so.
		/// </para>
		/// <para>
		/// This is called by the standard implementation of <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeBody(System.Object)" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeChild(System.Object,System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeChild(System.Type,System.Object,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </para>
		/// </remarks>
		public virtual void ConsumeChild(object parent, object child)
		{
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.ConsumeChild(parent, child);
			}
			else
			{
				Reader.InvokeConsumeChild(TransformerType.BaseType, parent, child);
			}
		}

		/// <summary>
		/// Finish building the object for the current element.
		/// </summary>
		/// <param name="obj">the object being constructed</param>
		/// <remarks>
		/// This is called by <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ConsumeObjectFinish(System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeObjectFinish(System.Type,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual void ConsumeObjectFinish(object obj)
		{
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.ConsumeObjectFinish(obj);
			}
			else
			{
				Reader.InvokeConsumeObjectFinish(TransformerType.BaseType, obj);
			}
		}

		/// <summary>
		/// Fix up a reference property whose value was delayed until the referred-to-object
		/// became available.
		/// </summary>
		/// <param name="obj">
		/// The object that contains an unresolved reference
		/// </param>
		/// <param name="prop">
		/// a <c>String</c> that the names the property or somehow identifies which
		/// reference in the object given by <paramref name="obj" /> needs to be
		/// updated with the correct reference given by <paramref name="referred" />
		/// </param>
		/// <param name="referred">
		/// an <c>Object</c> that is the result of a call to
		/// <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" />, or if that
		/// call returned null, the <c>String</c> which was the reference string passed
		/// to the earlier call to <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.AddDelayedRef(System.Object,System.String,System.String)" />.
		/// </param>
		/// <remarks>
		/// This is called from <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" />.
		/// By default this calls <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" /> on the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.InheritsFromTransformer" />,
		/// to allow another specific transformer to implement this behavior,
		/// or else calls <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeUpdateReference(System.Type,System.Object,System.String,System.Object)" />,
		/// to allow this <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.TransformerType" />'s base <c>Type</c> to provide a default behavior.
		/// </remarks>
		public virtual void UpdateReference(object obj, string prop, object referred)
		{
			if (InheritsFromTransformer != null)
			{
				InheritsFromTransformer.UpdateReference(obj, prop, referred);
			}
			else
			{
				Reader.InvokeUpdateReference(TransformerType.BaseType, obj, prop, referred);
			}
		}

		/// <summary>
		/// Start a new XML element, with the name given by <paramref name="name" />.
		/// </summary>
		/// <param name="name">the local name for the element; must not be null</param>
		/// <remarks>
		/// You need to make a corresponding call to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteEndElement" />
		/// when you are finished writing this element.
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.WriterElement" /> to be the newly started <c>XmlElement</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteStartElement(System.String)" />
		public void WriteStartElement(string name)
		{
			Writer.WriteStartElement(name);
		}

		/// <summary>
		/// Start a new XML element, with the name given by <paramref name="name" />,
		/// and with an optional <paramref name="prefix" /> and an optional namespace <paramref name="uri" />.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="name">the local name for the element; must not be null</param>
		/// <param name="uri"></param>
		/// <remarks>
		/// You need to make a corresponding call to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteEndElement" />
		/// when you are finished writing this element.
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.WriterElement" /> to be the newly started <c>XmlElement</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteStartElement(System.String,System.String,System.String)" />
		internal void WriteStartElement(string prefix, string name, string uri)
		{
			Writer.WriteStartElement(prefix, name, uri);
		}

		/// <summary>
		/// Finish the current XML element started by a call to <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteStartElement(System.String)" />.
		/// </summary>
		/// <remarks>
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.WriterElement" /> to be the <c>ParentNode</c> assuming
		/// it is an <c>XmlElement</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteEndElement" />
		public void WriteEndElement()
		{
			Writer.WriteEndElement();
		}

		/// <summary>
		/// Write out a string as the body of an element.
		/// </summary>
		/// <param name="s">the text to be written</param>
		/// <remarks>
		/// This method will write to the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.XmlWriter" /> if it
		/// is available or else it will create an XML text node in the <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.WriterElement" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteTextBody(System.String)" />
		public void WriteTextBody(string s)
		{
			Writer.WriteTextBody(s);
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the reference to the object <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name</param>
		/// <param name="val">the object for which we write a string reference</param>
		/// <returns>a string representing a reference to that object</returns>
		/// <remarks>
		/// This converts the object reference to a string by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.MakeShared(System.Object)" />
		/// and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.String)" />
		public string WriteAttrRef(string name, object val)
		{
			string text = "null";
			if (val != null)
			{
				text = Writer.MakeShared(val);
			}
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// This writes the attribute by calling <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />
		public string WriteAttrVal(string name, string val)
		{
			Writer.WriteAttrVal(name, val);
			return val;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Int32FromString(System.String,System.Int32)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, int val)
		{
			string text = XmlConvert.ToString(val);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SingleFromString(System.String,System.Single)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, float val)
		{
			string text = XmlConvert.ToString(val);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.DoubleFromString(System.String,System.Double)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, double val)
		{
			string text = XmlConvert.ToString(val);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.BooleanFromString(System.String,System.Boolean)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, bool val)
		{
			string text = XmlConvert.ToString(val);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.PointFFromString(System.String,System.Drawing.PointF)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, PointF val)
		{
			string text = XmlConvert.ToString(val.X) + " " + XmlConvert.ToString(val.Y);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SizeFFromString(System.String,System.Drawing.SizeF)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, SizeF val)
		{
			string text = XmlConvert.ToString(val.Width) + " " + XmlConvert.ToString(val.Height);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.RectangleFFromString(System.String,System.Drawing.RectangleF)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, RectangleF val)
		{
			string text = XmlConvert.ToString(val.X) + " " + XmlConvert.ToString(val.Y) + " " + XmlConvert.ToString(val.Width) + " " + XmlConvert.ToString(val.Height);
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ColorFromString(System.String,System.Drawing.Color)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, Color val)
		{
			string text = (val == Color.Empty) ? "none" : ((!Writer.ColorsNamed || !val.IsNamedColor) ? XmlConvert.ToString(val.ToArgb()) : val.ToKnownColor().ToString());
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.TypeFromString(System.String,System.Type)" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, Type val)
		{
			string text = val.FullName + ", " + val.Assembly.GetName().Name;
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Int32ArrayFromString(System.String,System.Int32[])" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, int[] val)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < val.Length; i++)
			{
				int value = val[i];
				if (i > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(XmlConvert.ToString(value));
			}
			string text = stringBuilder.ToString();
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SingleArrayFromString(System.String,System.Single[])" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, float[] val)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < val.Length; i++)
			{
				float value = val[i];
				if (i > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(XmlConvert.ToString(value));
			}
			string text = stringBuilder.ToString();
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.PointFArrayFromString(System.String,System.Drawing.PointF[])" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, PointF[] val)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < val.Length; i++)
			{
				PointF pointF = val[i];
				if (i > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(XmlConvert.ToString(pointF.X) + " " + XmlConvert.ToString(pointF.Y));
			}
			string text = stringBuilder.ToString();
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">the attribute name, or null to just convert the value to a string and return it</param>
		/// <param name="val">the value to write</param>
		/// <returns>the value converted to a string</returns>
		/// <remarks>
		/// <para>
		/// This converts the object reference to a string and then writes the attribute by calling
		/// <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </para>
		/// <para>
		/// You can use this method to just convert a value to a string by passing null as the attribute name.
		/// You can convert a string back to a value by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ColorArrayFromString(System.String,System.Drawing.Color[])" />.
		/// </para>
		/// </remarks>
		public string WriteAttrVal(string name, Color[] val)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < val.Length; i++)
			{
				Color val2 = val[i];
				if (i > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(WriteAttrVal(null, val2));
			}
			string text = stringBuilder.ToString();
			Writer.WriteAttrVal(name, text);
			return text;
		}

		/// <summary>
		/// Read in a string that is the body of an element.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This just calls <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<see cref="M:Northwoods.Go.Xml.GoXmlReader.ReadTextBody" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlReader.ReadTextBody" />
		public string ReadTextBody()
		{
			return Reader.ReadTextBody();
		}

		/// <summary>
		/// This predicate returns true if the given attribute name is present
		/// in the current element when reading.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlReader.ReadAttrVal(System.String)" />
		public bool IsAttrPresent(string name)
		{
			return Reader.ReadAttrVal(name) != null;
		}

		/// <summary>
		/// Find a shared object referenced by a given attribute.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present</param>
		/// <returns>
		/// An object found in the table of shared object, indexed by the value of the attribute.
		/// If the attribute's value is the string "null" or if its value is not found in the
		/// shared object table, this method returns null/Nothing.
		/// If the attribute is not present, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlTransformer.StringAttr(System.String,System.String)" />
		public object RefAttr(string name, object def)
		{
			string text = Reader.ReadAttrVal(name);
			if (text != null)
			{
				try
				{
					if (text == "null")
					{
						return null;
					}
					return Reader.FindShared(text);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Return an attribute's string value.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present</param>
		/// <returns>
		/// The value of the attribute named by <paramref name="name" />;
		/// if the attribute is not present, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlReader.ReadAttrVal(System.String)" />
		public string StringAttr(string name, string def)
		{
			string text = Reader.ReadAttrVal(name);
			if (text != null)
			{
				return text;
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as an integer.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Int32FromString(System.String,System.Int32)" /></returns>
		public int Int32Attr(string name, int def)
		{
			return Int32FromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as an integer.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An integer parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Int32)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public int Int32FromString(string str, int def)
		{
			if (str != null)
			{
				try
				{
					return XmlConvert.ToInt32(str);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a single float.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SingleFromString(System.String,System.Single)" /></returns>
		public float SingleAttr(string name, float def)
		{
			return SingleFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a single float.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A single float parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Single)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public float SingleFromString(string str, float def)
		{
			if (str != null)
			{
				try
				{
					return XmlConvert.ToSingle(str);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a double float.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.DoubleFromString(System.String,System.Double)" /></returns>
		public double DoubleAttr(string name, double def)
		{
			return DoubleFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a double.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A double float parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Double)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public double DoubleFromString(string str, double def)
		{
			if (str != null)
			{
				try
				{
					return XmlConvert.ToDouble(str);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a boolean.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.BooleanFromString(System.String,System.Boolean)" /></returns>
		public bool BooleanAttr(string name, bool def)
		{
			return BooleanFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a boolean.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A boolean parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Boolean)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public bool BooleanFromString(string str, bool def)
		{
			if (str != null)
			{
				try
				{
					return XmlConvert.ToBoolean(str);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a <c>PointF</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.PointFFromString(System.String,System.Drawing.PointF)" /></returns>
		public PointF PointFAttr(string name, PointF def)
		{
			return PointFFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a <c>PointF</c>.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An <c>PointF</c> parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.PointF)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public PointF PointFFromString(string str, PointF def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					float x = def.X;
					float y = def.Y;
					if (array.Length != 0)
					{
						x = XmlConvert.ToSingle(array[0]);
					}
					if (array.Length > 1)
					{
						y = XmlConvert.ToSingle(array[1]);
					}
					return new PointF(x, y);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a <c>SizeF</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SizeFFromString(System.String,System.Drawing.SizeF)" /></returns>
		public SizeF SizeFAttr(string name, SizeF def)
		{
			return SizeFFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a <c>SizeF</c>.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A <c>SizeF</c> parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.SizeF)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public SizeF SizeFFromString(string str, SizeF def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					float width = def.Width;
					float height = def.Height;
					if (array.Length != 0)
					{
						width = XmlConvert.ToSingle(array[0]);
					}
					if (array.Length > 1)
					{
						height = XmlConvert.ToSingle(array[1]);
					}
					return new SizeF(width, height);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a <c>RectangleF</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.RectangleFFromString(System.String,System.Drawing.RectangleF)" /></returns>
		public RectangleF RectangleFAttr(string name, RectangleF def)
		{
			return RectangleFFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a <c>RectangleF</c>.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A <c>RectangleF</c> parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.RectangleF)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public RectangleF RectangleFFromString(string str, RectangleF def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					float x = def.X;
					float y = def.Y;
					float width = def.Width;
					float height = def.Height;
					if (array.Length != 0)
					{
						x = XmlConvert.ToSingle(array[0]);
					}
					if (array.Length > 1)
					{
						y = XmlConvert.ToSingle(array[1]);
					}
					if (array.Length > 2)
					{
						width = XmlConvert.ToSingle(array[2]);
					}
					if (array.Length > 3)
					{
						height = XmlConvert.ToSingle(array[3]);
					}
					return new RectangleF(x, y, width, height);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as a <c>Color</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ColorFromString(System.String,System.Drawing.Color)" /></returns>
		public Color ColorAttr(string name, Color def)
		{
			return ColorFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a <c>Color</c> value.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A <c>Color</c> parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.Color)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public Color ColorFromString(string str, Color def)
		{
			switch (str)
			{
			case "none":
			case "Empty":
				return Color.Empty;
			default:
				try
				{
					return Color.FromArgb(XmlConvert.ToInt32(str));
				}
				catch (Exception)
				{
					try
					{
						return Color.FromName(str);
					}
					catch (Exception)
					{
						return def;
					}
				}
			case null:
				return def;
			}
		}

		/// <summary>
		/// Read an attribute's string value as a <c>Type</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.TypeFromString(System.String,System.Type)" /></returns>
		public Type TypeAttr(string name, Type def)
		{
			return TypeFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as a <c>Type</c>.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// A <c>Type</c> parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Type)" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public Type TypeFromString(string str, Type def)
		{
			if (str != null)
			{
				try
				{
					return Type.GetType(str, throwOnError: true, ignoreCase: true);
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as an array of integers.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Int32ArrayFromString(System.String,System.Int32[])" /></returns>
		public int[] Int32ArrayAttr(string name, int[] def)
		{
			return Int32ArrayFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as an array of integers.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An array of integers parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Int32[])" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public int[] Int32ArrayFromString(string str, int[] def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						int num = array2[i] = XmlConvert.ToInt32(array[i]);
					}
					return array2;
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as an array of single floats.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.SingleArrayFromString(System.String,System.Single[])" /></returns>
		public float[] SingleArrayAttr(string name, float[] def)
		{
			return SingleArrayFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as an array of single floats.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An array of single floats parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Single[])" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public float[] SingleArrayFromString(string str, float[] def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					float[] array2 = new float[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						float num = array2[i] = XmlConvert.ToSingle(array[i]);
					}
					return array2;
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as an array of <c>PointF</c>.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.PointFArrayFromString(System.String,System.Drawing.PointF[])" /></returns>
		public PointF[] PointFArrayAttr(string name, PointF[] def)
		{
			return PointFArrayFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as an array of <c>PointF</c>.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An array of <c>PointF</c>s parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.PointF[])" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public PointF[] PointFArrayFromString(string str, PointF[] def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					PointF[] array2 = new PointF[(array.Length + 1) / 2];
					int num;
					for (num = 0; num < array.Length; num++)
					{
						float x = XmlConvert.ToSingle(array[num]);
						num++;
						float y = 0f;
						if (num < array.Length)
						{
							y = XmlConvert.ToSingle(array[num]);
						}
						array2[num / 2] = new PointF(x, y);
					}
					return array2;
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}

		/// <summary>
		/// Read an attribute's string value as an array of color values.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <param name="def">the default value to return if the attribute is not present or there is a parsing error</param>
		/// <returns>the result of calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.ColorArrayFromString(System.String,System.Drawing.Color[])" /></returns>
		public Color[] ColorArrayAttr(string name, Color[] def)
		{
			return ColorArrayFromString(Reader.ReadAttrVal(name), def);
		}

		/// <summary>
		/// Parse a string value as an array of color values.
		/// </summary>
		/// <param name="str">the string to parse</param>
		/// <param name="def">the default value to return if the string is null or there is a syntax or range error</param>
		/// <returns>
		/// An array of colors parsed from the string <paramref name="str" />;
		/// if there is a parsing exception, this method returns the value of <paramref name="def" />.
		/// </returns>
		/// <remarks>
		/// You can convert a value to a string by calling <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.Color[])" />
		/// and passing null as the first argument (the attribute name).
		/// </remarks>
		public Color[] ColorArrayFromString(string str, Color[] def)
		{
			if (str != null)
			{
				try
				{
					string[] array = str.Split(' ');
					Color[] array2 = new Color[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						Color color = array2[i] = ColorFromString(array[i], Color.Empty);
					}
					return array2;
				}
				catch (Exception)
				{
					return def;
				}
			}
			return def;
		}
	}
}
