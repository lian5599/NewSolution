using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// Write customizable XML, using class-specific <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s,
	/// creating an <c>XmlDocument</c> or just writing to an <c>XmlWriter</c>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To produce XML, this class opens a <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />,
	/// generates XML instructions, starts a root element, and then
	/// iterates twice over the set of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
	/// The first pass (implemented by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" />)
	/// gives each object a chance to detect and remember shared objects and
	/// set up other definitions in preliminary XML elements that are part of the root.
	/// The second pass (implemented by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" />)
	/// actually produces elements for the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
	/// </para>
	/// <para>
	/// You must provide type-specific customizations by supplying instances of
	/// <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />.  Each transformer is associated with a <c>Type</c>.
	/// As this writer processes each object that it is rendering, it searches
	/// for the appropriate transformer to invoke, starting with that type and
	/// trying its base types.
	/// </para>
	/// <para>
	/// By default there are no transformers registered for this writer, so this
	/// writer is unable to actually do anything with the specified <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
	/// You will need to call <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> for each class of object
	/// for which you want to produce XML.
	/// These calls to <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> are sometimes done in an override
	/// of <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.RegisterTransformers" />, but you probably do not need to define
	/// a class inheriting from <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.
	/// </para>
	/// <para>
	/// For the <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" /> pass, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" />
	/// is called for each object in <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />, which in turn
	/// calls <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateDefinitions(System.Object)" /> on the registered transformer
	/// for the object's <c>Type</c>.
	/// </para>
	/// <para>
	/// For the <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" /> pass, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />
	/// is called for each object in <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />, which in turn
	/// calls <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElement(System.Object)" />,
	/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateAttributes(System.Object)" />,
	/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateBody(System.Object)" />, and
	/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElementFinish(System.Object)" />
	/// on the registered transformer for the object's <c>Type</c>.
	/// </para>
	/// <para>
	/// The various <c>Invoke...</c> methods do the actual lookup for a transformer
	/// and invoke the respective method on the first transformer found.
	/// </para>
	/// </remarks>
	/// <example>
	/// This writer could be used as follows:
	/// <code>
	/// public void SaveSimpleXml(String path) {
	///   GoXmlWriter xw = new GoXmlWriter();
	///   xw.RootElementName = "graph";
	///   xw.NodesGeneratedFirst = true;
	///   // tell the writer how to handle two kinds of classes
	///   xw.AddTransformer(new SimpleXmlTransformBasicNode());
	///   xw.AddTransformer(new SimpleXmlTransformLink());
	///   // specify the objects to be generated
	///   xw.Objects = myView.Document;
	///
	///   using (StreamWriter sw = new StreamWriter(path)) {
	///     xw.Generate(sw);
	///   }
	/// }
	/// </code>
	/// See the description of <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> for example transformer definitions.
	/// </example>
	public class GoXmlWriter : GoXmlReaderWriterBase
	{
		private IEnumerable myObjects;

		private string myRootElementName = "graph";

		private string myDefaultNamespace;

		private bool myNodesGeneratedFirst = true;

		private XmlElement myWriterElement;

		private XmlWriterSettings myWriterSettings = new XmlWriterSettings();

		private XmlWriter myWriter;

		private XmlDocument myDocument;

		private Dictionary<object, string> mySharedObjects = new Dictionary<object, string>();

		private Dictionary<string, string> myPrefixes = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the <c>XmlWriterSettings</c> used in the call to <c>XmlWriter.Create</c>
		/// to customize the <c>XmlWriter</c>.
		/// </summary>
		/// <value>
		/// The default value has its <c>XmlWriterSettings.Indent</c> property set to true.
		/// </value>
		public XmlWriterSettings XmlWriterSettings
		{
			get
			{
				return myWriterSettings;
			}
			set
			{
				myWriterSettings = value;
			}
		}

		/// <summary>
		/// Gets the <c>XmlWriter</c> used to actually write XML to the output stream.
		/// </summary>
		/// <value>
		/// The value is constructed and initialized during XML generation, and is
		/// automatically closed afterwards.
		/// This value is null when constructing an <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlDocument" />.
		/// </value>
		public XmlWriter XmlWriter => myWriter;

		/// <summary>
		/// Gets the <c>XmlDocument</c> that is constructed by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate" /> when not writing
		/// directly to a stream with <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />.
		/// </summary>
		/// <value>
		/// An <c>XmlDocument</c>; this value is null when writing directly to an <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />.
		/// </value>
		public XmlDocument XmlDocument => myDocument;

		/// <summary>
		/// Gets or sets the <c>XmlElement</c> that is being constructed when creating
		/// a DOM rather than writing directly to a <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />.
		/// </summary>
		public XmlElement WriterElement
		{
			get
			{
				return myWriterElement;
			}
			set
			{
				myWriterElement = value;
			}
		}

		/// <summary>
		/// Gets or sets the collection of objects to be generated.
		/// </summary>
		/// <value>
		/// This must be set before calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate(System.Xml.XmlWriter)" />
		/// or <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Caution: <see cref="T:Northwoods.Go.GoNode" /> implements <b>ICollection</b>,
		/// so if you set this property to refer to a node in your document,
		/// this writer will try to generate XML for the parts of your node,
		/// which is probably not what you intend.
		/// </para>
		/// <para>
		/// Caution: If you are generating tree-structured XML corresponding to a
		/// logically tree-structured graph of nodes and links, by making use of the
		/// <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />.<see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.TreeStructured" />
		/// property, you will need to make sure that the collection that you
		/// assign to this property contains only the logical root nodes of
		/// the logical trees in your document.
		/// That transformer will automatically generate nested XML for the nodes
		/// corresponding to the tree-children of each node given in this collection property,
		/// and recursively through the whole tree.  That will cause the generation of
		/// duplicate XML elements for every single logical node in your trees.
		/// </para>
		/// </remarks>
		public IEnumerable Objects
		{
			get
			{
				return myObjects;
			}
			set
			{
				myObjects = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the root element.
		/// </summary>
		/// <value>
		/// This should be set, along with <see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" />
		/// if desired, before calling <c>Generate</c>.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />
		public string RootElementName
		{
			get
			{
				return myRootElementName;
			}
			set
			{
				myRootElementName = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" /> makes two
		/// passes over the collection of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />, generating
		/// the objects that implement <c>IGoNode</c> first before generating
		/// the other objects.
		/// </summary>
		/// <value>This defaults to true.</value>
		public bool NodesGeneratedFirst
		{
			get
			{
				return myNodesGeneratedFirst;
			}
			set
			{
				myNodesGeneratedFirst = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.WriteAttrVal(System.String,System.Drawing.Color)" />
		/// tries to write out the name of a <c>Color</c> when known, instead of just
		/// writing out the integer ARGB value.
		/// </summary>
		/// <value>This defaults to false</value>
		internal bool ColorsNamed => false;

		/// <summary>
		/// Gets or sets the "xmlns" attribute namespace to be
		/// defined for the root element.
		/// </summary>
		/// <value>
		/// This defaults to null--no "xmlns" attribute is created for the root element.
		/// </value>
		/// <remarks>
		/// If you want to specify a default namespace, you will need to set this property,
		/// along with the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.RootElementName" />, before calling <c>Generate</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />
		public string DefaultNamespace
		{
			get
			{
				return myDefaultNamespace;
			}
			set
			{
				myDefaultNamespace = value;
			}
		}

		/// <summary>
		/// Create a writer with a set of <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s defined.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.RegisterTransformers" />.
		/// </remarks>
		public GoXmlWriter()
		{
			XmlWriterSettings.ConformanceLevel = ConformanceLevel.Auto;
			XmlWriterSettings.Indent = true;
			RegisterTransformers();
		}

		/// <summary>
		/// Find an identifier associated with an object that can be referenced.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>a string if the object is present; null otherwise or if <paramref name="obj" /> is null</returns>
		/// <remarks>
		/// Call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.MakeShared(System.Object)" /> to add an object to this writer's table
		/// of shared objects.
		/// </remarks>
		public string FindShared(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			mySharedObjects.TryGetValue(obj, out string value);
			return value;
		}

		/// <summary>
		/// Add an object to the table of shared objects.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>a unique identifier for this shared object, or null if <paramref name="obj" /> is null</returns>
		/// <remarks>
		/// This does nothing if the <paramref name="obj" /> is already
		/// in the table, but just returns its identifier.
		/// This calls <see cref="M:Northwoods.Go.Xml.GoXmlWriter.FindShared(System.Object)" /> to see if the object
		/// is already known as a shared object.
		/// </remarks>
		public string MakeShared(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			string text = FindShared(obj);
			if (text == null)
			{
				text = XmlConvert.ToString(mySharedObjects.Count);
				mySharedObjects[obj] = text;
			}
			return text;
		}

		/// <summary>
		/// Remove all objects from the table of shared objects.
		/// </summary>
		/// <remarks>
		/// This also resets the IDs returned by calls to <see cref="M:Northwoods.Go.Xml.GoXmlWriter.MakeShared(System.Object)" />.
		/// </remarks>
		public void ClearAllShareds()
		{
			mySharedObjects.Clear();
		}

		/// <summary>
		/// Only a subclass of a <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> can provide a
		/// value for the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> property.
		/// </summary>
		/// <param name="w">
		/// an <c>XmlWriter</c> initialized at the
		/// start of <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate(System.Xml.XmlWriter)" />, or null at the end of
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate(System.Xml.XmlWriter)" /> after closing the <c>XmlWriter</c>
		/// </param>
		protected virtual void SetXmlWriter(XmlWriter w)
		{
			myWriter = w;
		}

		/// <summary>
		/// Only a subclass of a <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> can provide a
		/// value for the DOM.
		/// </summary>
		/// <param name="d">the <c>XmlDocument</c> being constructed by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate" />
		/// when there is no <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /></param>
		protected virtual void SetXmlDocument(XmlDocument d)
		{
			myDocument = d;
		}

		/// <summary>
		/// Start an <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> and generate XML.
		/// </summary>
		/// <param name="destination">the <c>Stream</c> to be written</param>
		/// <remarks>
		/// <para>
		/// This calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateInstructions" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootAttributes" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" />, and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" />.
		/// The value of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlDocument" /> will be null, since no
		/// DOM is constructed.
		/// </para>
		/// <para>
		/// This calls <c>XmlWriter.Create</c> with the value of
		/// <c>this.XmlWriterSettings</c>.
		/// </para>
		/// </remarks>
		public void Generate(Stream destination)
		{
			XmlWriter xmlWriter = null;
			try
			{
				xmlWriter = XmlWriter.Create(destination, XmlWriterSettings);
				Generate(xmlWriter);
			}
			finally
			{
				xmlWriter?.Close();
			}
		}

		/// <summary>
		/// Start an <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> and generate XML.
		/// </summary>
		/// <param name="destination">the <c>TextWriter</c> to be written</param>
		/// <remarks>
		/// <para>
		/// This calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateInstructions" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootAttributes" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" />, and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" />.
		/// The value of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlDocument" /> will be null, since no
		/// DOM is constructed.
		/// </para>
		/// <para>
		/// This calls <c>XmlWriter.Create</c> with the value of
		/// <c>this.XmlWriterSettings</c>.
		/// </para>
		/// </remarks>
		public void Generate(TextWriter destination)
		{
			XmlWriter xmlWriter = null;
			try
			{
				xmlWriter = XmlWriter.Create(destination, XmlWriterSettings);
				Generate(xmlWriter);
			}
			finally
			{
				xmlWriter?.Close();
			}
		}

		/// <summary>
		/// Generate XML to an <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />.
		/// </summary>
		/// <param name="destination">the <c>XmlWriter</c> to be written</param>
		/// <remarks>
		/// This calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateInstructions" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootAttributes" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" />, and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" />.
		/// The value of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlDocument" /> will be null, since no
		/// DOM is constructed.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual void Generate(XmlWriter destination)
		{
			Initialize();
			if (base.TransformerCount == 0)
			{
				throw new InvalidOperationException("No IGoXmlTransformers have been registered for any Types");
			}
			try
			{
				SetXmlWriter(destination);
				GenerateCommon(inst: true);
			}
			finally
			{
				GenerateCleanupCommon();
			}
		}

		private void GenerateCommon(bool inst)
		{
			if (inst)
			{
				GenerateInstructions();
			}
			GenerateRootElement();
			foreach (KeyValuePair<string, string> myPrefix in myPrefixes)
			{
				string key = myPrefix.Key;
				string value = myPrefix.Value;
				WriteNamespaceAttribute(key, value);
			}
			GenerateRootAttributes();
			base.ObjectStack.Add(Objects);
			GenerateDefinitions();
			GenerateObjects();
		}

		private void GenerateCleanupCommon()
		{
			base.ObjectStack.Clear();
			SetXmlWriter(null);
		}

		/// <summary>
		/// Construct an <c>XmlDocument</c> representing the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// </summary>
		/// <returns>a newly created <c>XmlDocument</c></returns>
		/// <remarks>
		/// This method creates an <c>XmlDocument</c>.
		/// This then calls, in order,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootAttributes" />, 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateDefinitions" />, and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObjects" />.
		/// Note that this does not call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateInstructions" />.
		/// The value of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> is set to null, since
		/// no stream is used.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual XmlDocument Generate()
		{
			Initialize();
			if (base.TransformerCount == 0)
			{
				throw new InvalidOperationException("No IGoXmlTransformers have been registered for any Types");
			}
			SetXmlWriter(null);
			SetXmlDocument(new XmlDocument());
			GenerateCommon(inst: false);
			GenerateCleanupCommon();
			return XmlDocument;
		}

		/// <summary>
		/// Do the initialization needed by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.Generate(System.Xml.XmlWriter)" />.
		/// </summary>
		protected virtual void Initialize()
		{
			base.ObjectStack.Clear();
			ClearAllShareds();
		}

		/// <summary>
		/// Generate XML instructions before the root element.
		/// </summary>
		/// <remarks>
		/// By default this does nothing.
		/// </remarks>
		protected virtual void GenerateInstructions()
		{
		}

		/// <summary>
		/// Start the root element, using <see cref="P:Northwoods.Go.Xml.GoXmlWriter.RootElementName" /> and <see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" /> property value is a <see cref="T:Northwoods.Go.GoDocument" />,
		/// and if there is a <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> registered for that <c>GoDocument</c> type,
		/// the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.RootElementName" /> is set to that transformer's
		/// <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" />.<see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" />.
		/// </para>
		/// <para>
		/// If not writing to a <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" />, this is
		/// responsible for setting <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />.
		/// </para>
		/// </remarks>
		protected virtual void GenerateRootElement()
		{
			GoDocument goDocument = Objects as GoDocument;
			if (goDocument != null)
			{
				GoXmlBindingTransformer goXmlBindingTransformer = FindTransformer(goDocument.GetType()) as GoXmlBindingTransformer;
				if (goXmlBindingTransformer != null)
				{
					RootElementName = goXmlBindingTransformer.ElementName;
				}
			}
			XmlWriter xmlWriter = XmlWriter;
			if (xmlWriter != null)
			{
				xmlWriter.WriteStartElement(null, RootElementName, DefaultNamespace);
				return;
			}
			XmlDocument xmlDocument = XmlDocument;
			if (xmlDocument != null)
			{
				XmlElement xmlElement = xmlDocument.CreateElement(null, RootElementName, DefaultNamespace);
				xmlDocument.AppendChild(xmlElement);
				WriterElement = xmlElement;
			}
		}

		/// <summary>
		/// Generate attributes for the root element.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" /> property value is a <see cref="T:Northwoods.Go.GoDocument" />,
		/// and if there is a <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> registered for that <c>GoDocument</c> type,
		/// this calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.GenerateAttributes(System.Object)" /> on that transformer,
		/// passing it the <see cref="T:Northwoods.Go.GoDocument" />.
		/// This provides an easy-to-define mechanism for binding document properties with root element attributes.
		/// </para>
		/// <para>
		/// Otherwise, this does nothing.
		/// You may want to define namespace attributes, as is shown in the
		/// description for <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />, by calling 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" /> before this method is called.
		/// The <c>Generate</c> methods will call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteNamespaceAttribute(System.String,System.String)" />
		/// for each prefix/namespaceURI pair, just after calling
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateRootElement" />, and just before calling this method.
		/// </para>
		/// </remarks>
		protected virtual void GenerateRootAttributes()
		{
			GoDocument goDocument = Objects as GoDocument;
			if (goDocument != null)
			{
				(FindTransformer(goDocument.GetType()) as GoXmlBindingTransformer)?.GenerateAttributes(goDocument);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.DefineObject(System.Object)" /> for each object in <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// </summary>
		/// <remarks>
		/// This gives each object a chance to identify common objects
		/// and perhaps produce some shared elements that precede the
		/// regular rendering.
		/// You probably do not need to override this method.
		/// </remarks>
		protected virtual void GenerateDefinitions()
		{
			foreach (object @object in Objects)
			{
				DefineObject(@object);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" /> for each object in <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// </summary>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlWriter.NodesGeneratedFirst" /> is false, this method just
		/// calls <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" /> on each object in the order that
		/// they are enumerated by the collection of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlWriter.NodesGeneratedFirst" /> is true, this method enumerates
		/// the collection twice, first to call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" />
		/// on all objects that implement <c>IGoNode</c>, then second to generate
		/// all the rest of the objects.
		/// </remarks>
		protected virtual void GenerateObjects()
		{
			if (NodesGeneratedFirst)
			{
				foreach (object @object in Objects)
				{
					if (@object is IGoNode)
					{
						GenerateObject(@object);
					}
				}
				foreach (object object2 in Objects)
				{
					if (!(object2 is IGoNode))
					{
						GenerateObject(object2);
					}
				}
			}
			else
			{
				foreach (object object3 in Objects)
				{
					GenerateObject(object3);
				}
			}
		}

		/// <summary>
		/// This may produce shared elements that precede the regular rendering of the objects.
		/// </summary>
		/// <param name="obj">an <c>Object</c>; if null this method does nothing</param>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeSkipGeneration(System.Type,System.Object)" /> returns true, no definition elements are generated.
		/// Otherwise this calls <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateDefinitions(System.Type,System.Object)" />.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual void DefineObject(object obj)
		{
			if (obj != null)
			{
				Type type = obj.GetType();
				if (!InvokeSkipGeneration(type, obj))
				{
					InvokeGenerateDefinitions(type, obj);
				}
			}
		}

		/// <summary>
		/// This produces an element for an object, including any nested elements that help represent the rendering.
		/// </summary>
		/// <param name="obj">an <c>Object</c>; if null this method does nothing</param>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeSkipGeneration(System.Type,System.Object)" /> returns true, no element is generated.
		/// Otherwise this starts writing an element by calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateElement(System.Type,System.Object)" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateAttributes(System.Type,System.Object)" />, <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateBody(System.Type,System.Object)" />,
		/// and the <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateElementFinish(System.Type,System.Object)" />.
		/// If <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateElement(System.Type,System.Object)" /> returned true to indicate that it started
		/// a new element, this method will call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteEndElement" /> in order to finish it.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual void GenerateObject(object obj)
		{
			if (obj == null)
			{
				return;
			}
			Type type = obj.GetType();
			if (!InvokeSkipGeneration(type, obj))
			{
				bool num = InvokeGenerateElement(type, obj);
				InvokeGenerateAttributes(type, obj);
				base.ObjectStack.Add(obj);
				InvokeGenerateBody(type, obj);
				if (base.ObjectStack.Count > 0)
				{
					base.ObjectStack.RemoveAt(base.ObjectStack.Count - 1);
				}
				InvokeGenerateElementFinish(type, obj);
				if (num)
				{
					WriteEndElement();
				}
			}
		}

		/// <summary>
		/// If the given object is not already known to be a shared object,
		/// define it and generate it, so that you can use simple ID references
		/// to the shared object both during generation and during consumption.
		/// </summary>
		/// <param name="obj">an <c>Object</c>; if null this method does nothing</param>
		/// <remarks>
		/// This convenience method can be called from an override of
		/// <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.GenerateDefinitions(System.Object)" />
		/// when there are auxiliary objects
		/// that can be shared by the primary <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" /> but are
		/// not in that collection of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// If <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeSkipGeneration(System.Type,System.Object)" /> returns true,
		/// or if the <paramref name="obj" /> is already shared,
		/// no element is generated.
		/// This method is implemented as:
		/// <code>
		///  if (obj == null)
		///    return;
		///  Type t = obj.GetType();
		///  if (InvokeSkipGeneration(t, obj))
		///    return;
		///  String id = FindShared(obj);
		///  if (id == null) {
		///    MakeShared(obj);
		///    DefineObject(obj);
		///    GenerateObject(obj);
		///  }
		/// </code>
		/// </remarks>
		public virtual void DefineAndGenerateSharedObject(object obj)
		{
			if (obj != null)
			{
				Type type = obj.GetType();
				if (!InvokeSkipGeneration(type, obj) && FindShared(obj) == null)
				{
					MakeShared(obj);
					DefineObject(obj);
					GenerateObject(obj);
				}
			}
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.SkipGeneration(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.SkipGeneration(System.Object)" /></param>
		/// <returns>
		/// This returns the result of calling <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.SkipGeneration(System.Object)" />.
		/// If no transformer is found, this returns false.
		/// </returns>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual bool InvokeSkipGeneration(Type type, object obj)
		{
			Type type2 = type;
			while (type2 != null)
			{
				IGoXmlTransformer transformer = GetTransformer(type2);
				if (transformer != null)
				{
					return transformer.SkipGeneration(obj);
				}
				type2 = type2.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateDefinitions(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateDefinitions(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeGenerateDefinitions(Type type, object obj)
		{
			Type type2 = type;
			IGoXmlTransformer transformer;
			while (true)
			{
				if (type2 != null)
				{
					transformer = GetTransformer(type2);
					if (transformer != null)
					{
						break;
					}
					type2 = type2.BaseType;
					continue;
				}
				return;
			}
			transformer.GenerateDefinitions(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElement(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElement(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// </remarks>
		/// <returns>
		/// True if an element was started.
		/// This returns the result of calling <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElement(System.Object)" />.
		/// If no transformer is found, this returns false.
		/// This method is practically never overridden.
		/// </returns>
		public virtual bool InvokeGenerateElement(Type type, object obj)
		{
			Type type2 = type;
			while (type2 != null)
			{
				IGoXmlTransformer transformer = GetTransformer(type2);
				if (transformer != null)
				{
					return transformer.GenerateElement(obj);
				}
				type2 = type2.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateAttributes(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateAttributes(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeGenerateAttributes(Type type, object obj)
		{
			Type type2 = type;
			IGoXmlTransformer transformer;
			while (true)
			{
				if (type2 != null)
				{
					transformer = GetTransformer(type2);
					if (transformer != null)
					{
						break;
					}
					type2 = type2.BaseType;
					continue;
				}
				return;
			}
			transformer.GenerateAttributes(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateBody(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateBody(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeGenerateBody(Type type, object obj)
		{
			Type type2 = type;
			IGoXmlTransformer transformer;
			while (true)
			{
				if (type2 != null)
				{
					transformer = GetTransformer(type2);
					if (transformer != null)
					{
						break;
					}
					type2 = type2.BaseType;
					continue;
				}
				return;
			}
			transformer.GenerateBody(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElementFinish(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElementFinish(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeGenerateElementFinish(Type type, object obj)
		{
			Type type2 = type;
			IGoXmlTransformer transformer;
			while (true)
			{
				if (type2 != null)
				{
					transformer = GetTransformer(type2);
					if (transformer != null)
					{
						break;
					}
					type2 = type2.BaseType;
					continue;
				}
				return;
			}
			transformer.GenerateElementFinish(obj);
		}

		/// <summary>
		/// Start a new XML element, with the name given by <paramref name="name" />.
		/// </summary>
		/// <param name="name">the local name for the element; must not be null</param>
		/// <remarks>
		/// This starts a new element in the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" />.
		/// You need to make a corresponding call to <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteEndElement" />
		/// when you are finished writing this element.
		/// This method will write to the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> if it
		/// is available or else it will create an XML element as a child of the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />.
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" /> to be the newly started <c>XmlElement</c>.
		/// </remarks>
		public virtual void WriteStartElement(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			XmlWriter xmlWriter = XmlWriter;
			if (xmlWriter != null)
			{
				xmlWriter.WriteStartElement(null, name, DefaultNamespace);
				return;
			}
			XmlElement writerElement = WriterElement;
			XmlDocument xmlDocument = XmlDocument;
			if (writerElement != null && xmlDocument != null)
			{
				XmlElement xmlElement = xmlDocument.CreateElement(null, name, DefaultNamespace);
				writerElement.AppendChild(xmlElement);
				WriterElement = xmlElement;
			}
		}

		/// <summary>
		/// Start a new XML element, with the name given by <paramref name="name" />,
		/// and an optional <paramref name="prefix" /> and an optional associated namespace <paramref name="uri" />.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="name">the local name for the element; must not be null</param>
		/// <param name="uri"></param>
		/// <remarks>
		/// <para>
		/// This starts a new element with the given <paramref name="prefix" />, if not null.
		/// If a namespace <paramref name="uri" /> is given, that is associated with the new element,
		/// and with the prefix, if it was given.
		/// You need to make a corresponding call to <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteEndElement" />
		/// when you are finished writing this element.
		/// This method will write to the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> if it
		/// is available or else it will create an XML element as a child of the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />.
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" /> to be the newly started <c>XmlElement</c>.
		/// </para>
		/// <para>
		/// Note that GoXml currently does not support multiple <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" />s
		/// with the same <see cref="P:Northwoods.Go.Xml.GoXmlTransformer.ElementName" /> but different namespaces.
		/// You can call this method if you need to generate an element in a namespace other
		/// than the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" />.
		/// Calling this method does not modify this writer's table of prefix to namespace
		/// mappings -- it does not call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />.
		/// </para>
		/// </remarks>
		internal virtual void WriteStartElement(string prefix, string name, string uri)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			XmlWriter xmlWriter = XmlWriter;
			if (xmlWriter != null)
			{
				xmlWriter.WriteStartElement(prefix, name, uri);
				return;
			}
			XmlElement writerElement = WriterElement;
			XmlDocument xmlDocument = XmlDocument;
			if (writerElement != null && xmlDocument != null)
			{
				XmlElement xmlElement = xmlDocument.CreateElement(prefix, name, uri);
				if (xmlElement != null)
				{
					writerElement.AppendChild(xmlElement);
					WriterElement = xmlElement;
				}
			}
		}

		/// <summary>
		/// Finish the current XML element started by a call to <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteStartElement(System.String)" />.
		/// </summary>
		/// <remarks>
		/// If we are constructing a DOM instead of writing to a stream,
		/// this sets the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" /> to be the <c>ParentNode</c> assuming
		/// it is an <c>XmlElement</c>.
		/// </remarks>
		public virtual void WriteEndElement()
		{
			XmlWriter xmlWriter = XmlWriter;
			if (xmlWriter != null)
			{
				xmlWriter.WriteEndElement();
				return;
			}
			XmlElement writerElement = WriterElement;
			if (writerElement != null)
			{
				WriterElement = (writerElement.ParentNode as XmlElement);
			}
		}

		/// <summary>
		/// Write out a string as the body of an element.
		/// </summary>
		/// <param name="s">the text to be written</param>
		/// <remarks>
		/// This method will write to the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> if it
		/// is available or else it will create an XML text node in the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />.
		/// If <paramref name="s" /> is null, nothing is written and no <c>XmlText</c>
		/// node is created.
		/// </remarks>
		public virtual void WriteTextBody(string s)
		{
			if (s != null)
			{
				XmlWriter xmlWriter = XmlWriter;
				if (xmlWriter != null)
				{
					xmlWriter.WriteString(s);
				}
				else if (WriterElement != null && XmlDocument != null)
				{
					XmlText newChild = XmlDocument.CreateTextNode(s);
					WriterElement.AppendChild(newChild);
				}
			}
		}

		/// <summary>
		/// Write an attribute of the name <paramref name="name" /> with a
		/// string representation for the value <paramref name="val" />.
		/// </summary>
		/// <param name="name">
		/// the attribute name; if null, no attribute is written.
		/// If you call <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />, typically before calling a <c>Generate</c> method,
		/// you can also include a prefix in this name, in the format "prefix:localname".
		/// </param>
		/// <param name="val">the value to write; if null, no attribute is written</param>
		/// <returns><paramref name="val" /></returns>
		/// <remarks>
		/// This method will write to the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.XmlWriter" /> if it is available,
		/// or else it will set the attribute of the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.WriterElement" />.
		/// </remarks>
		public virtual string WriteAttrVal(string name, string val)
		{
			if (name != null && val != null)
			{
				XmlWriter xmlWriter = XmlWriter;
				if (xmlWriter != null)
				{
					int num = name.IndexOf(':');
					if (num > 0)
					{
						string prefix = name.Substring(0, num);
						string localName = name.Substring(num + 1);
						string namespaceUri = GetNamespaceUri(prefix);
						xmlWriter.WriteAttributeString(localName, namespaceUri, val);
					}
					else
					{
						xmlWriter.WriteAttributeString(name, val);
					}
				}
				else
				{
					XmlElement writerElement = WriterElement;
					if (writerElement != null)
					{
						int num2 = name.IndexOf(':');
						if (num2 > 0)
						{
							string prefix2 = name.Substring(0, num2);
							string localName2 = name.Substring(num2 + 1);
							string namespaceUri2 = GetNamespaceUri(prefix2);
							writerElement.SetAttribute(localName2, namespaceUri2, val);
						}
						else
						{
							writerElement.SetAttribute(name, val);
						}
					}
				}
			}
			return val;
		}

		/// <summary>
		/// Write an "xmlns:prefix" attribute for a namespace given by <paramref name="uri" />.
		/// </summary>
		/// <param name="prefix">must not be a null string</param>
		/// <param name="uri">must not be a null string</param>
		/// <remarks>
		/// <para>
		/// Namespaces are normally defined on the root element by calling
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" /> before calling one of the <c>Generate</c> methods.
		/// That will cause this method to be called to add the appropriate
		/// <code>xmlns:prefix="uri"</code> attributes to the root element.
		/// </para>
		/// </remarks>
		public virtual void WriteNamespaceAttribute(string prefix, string uri)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			XmlWriter xmlWriter = XmlWriter;
			if (xmlWriter != null)
			{
				xmlWriter.WriteAttributeString("xmlns", prefix, null, uri);
			}
			else if (WriterElement != null)
			{
				WriterElement.SetAttribute("xmlns:" + prefix, uri);
			}
		}

		/// <summary>
		/// Look up the namespace URI for a prefix string.
		/// </summary>
		/// <param name="prefix">must not be null</param>
		/// <returns>the URI <c>String</c>, if known; null if <paramref name="prefix" /> is null</returns>
		/// <remarks>
		/// This is used by <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteStartElement(System.String)" /> and <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />
		public virtual string GetNamespaceUri(string prefix)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			myPrefixes.TryGetValue(prefix, out string value);
			return value;
		}

		/// <summary>
		/// Define the namespace URI for a prefix string, used when writing prefixed attribute names.
		/// </summary>
		/// <param name="prefix">must not be null</param>
		/// <param name="uri">if null this method removes any URI associated with the <paramref name="prefix" /></param>
		/// <remarks>
		/// This should be called to establish a namespace URI for a prefix
		/// before calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteStartElement(System.String)" /> or <see cref="M:Northwoods.Go.Xml.GoXmlWriter.WriteAttrVal(System.String,System.String)" />
		/// with a name that has that prefix.
		/// To specify the default namespace for the root element,
		/// set the <see cref="P:Northwoods.Go.Xml.GoXmlWriter.DefaultNamespace" /> property.
		/// Calls to this method before a call to a <c>Generate</c> method
		/// will result in the addition of "xmlns:prefix" attributes in the root element.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.Xml.GoXmlWriter.GetNamespaceUri(System.String)" />
		/// <example>
		/// <para>
		/// As an example, consider the following code adapted from the GoSvg implementation:
		/// <code>
		///   GoXmlWriter w = ...
		///   w.DefaultNamespace = "http://www.w3.org/2000/svg";
		///   w.SetNamespaceUri("xlink", "http://www.w3.org/1999/xlink");
		/// </code>
		/// </para>
		/// <para>
		/// The above code causes the root element to have an "xmlns" attribute.
		/// It also defines a prefix, "xlink", that refers to a particular namespace URI.
		/// The result may look like:
		/// <code>
		///   &lt;svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"&gt;
		///   ...
		///   &lt;/svg&gt;
		/// </code>
		/// </para>
		/// <para>
		/// By calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.SetNamespaceUri(System.String,System.String)" />, you can then define a
		/// <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> with code such as:
		/// <code>
		/// public override void GenerateBody(Object obj) {
		///   base.GenerateBody(obj);
		///   GoImage img = (GoImage)obj;
		///   Image image = img.Image;
		///   String id = this.Writer.FindShared(image);
		///   if (id != null) {
		///     WriteStartElement("use");
		///     WriteAttrVal("xlink:href", "#S" + id);
		///     WriteEndElement();
		///   }
		/// }
		/// </code>
		/// </para>
		/// <para>
		/// The above transformer method example is simplified from the code used
		/// by the SVG transformer for <c>GoImage</c>s, where there needs to be
		/// a link to the element representing the actual Image.
		/// </para>
		/// </example>
		public virtual void SetNamespaceUri(string prefix, string uri)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (uri == null)
			{
				myPrefixes.Remove(prefix);
			}
			else
			{
				myPrefixes[prefix] = uri;
			}
		}
	}
}
