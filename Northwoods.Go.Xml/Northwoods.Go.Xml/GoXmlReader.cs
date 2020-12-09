using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// Read customizable XML, using class-specific <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s,
	/// using an <c>XmlDocument</c> or just an <c>XmlReader</c>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Depending on which overload of <c>Consume</c> is called, you can either
	/// traverse an <c>XmlDocument</c> that you supply, you can load an <c>XmlDocument</c>
	/// from a Stream (if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true), or you can just handle the
	/// elements as they are read in by an <c>XmlReader</c> from a Stream
	/// (if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is false).
	/// The processing of XML elements is basically done in a single pass, but there
	/// is built-in functionality for a second pass to fix up references in the objects
	/// that were constructed.
	/// </para>
	/// <para>
	/// You must provide type-specific customizations by supplying instances of
	/// <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />.  Each transformer is associated with a <c>Type</c>
	/// (<see cref="P:Northwoods.Go.Xml.IGoXmlTransformer.TransformerType" />) and an <c>ElementName</c> string
	/// (<see cref="P:Northwoods.Go.Xml.IGoXmlTransformer.ElementName" />).
	/// </para>
	/// <para>
	/// By default there are no transformers registered for this reader, so this
	/// reader is unable to actually do anything with the XML elements it sees.
	/// You will need to call <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> for each class of object
	/// for which you want to consume XML.
	/// These calls to <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> are sometimes done in an override
	/// of <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.RegisterTransformers" />, but you probably do not need to define
	/// a class inheriting from <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.
	/// </para>
	/// <para>
	/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" /> does the bulk of the work to traverse the
	/// elements in the <c>XmlDocument</c> root, or (if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is false)
	/// just to read elements from the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />.
	/// As this reader processes each element, it calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" />.
	/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" /> calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeAllocate" /> to actually
	/// allocate an object, and if an object is allocated, calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeAttributes(System.Type,System.Object)" />
	/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeBody(System.Type,System.Object)" />, and <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeObjectFinish(System.Type,System.Object)" />
	/// to actually reconstruct the rest of the object.
	/// </para>
	/// <para>
	/// Although most of the information that needs to be reconstructed will be held in
	/// attributes of the element representing an object, there will sometimes be references
	/// to other objects.  If you can be sure that the references will be to objects that
	/// have already been read, you can call <see cref="M:Northwoods.Go.Xml.GoXmlReader.MakeShared(System.String,System.Object)" /> to remember an object
	/// by a key string, and you can call <see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" /> to recover that object
	/// by using the same key.
	/// </para>
	/// <para>
	/// However, if the reference is to something that might not yet have been read and
	/// constructed, you can make use of the delayed-references mechanism that this reader offers.
	/// You can just call <see cref="M:Northwoods.Go.Xml.GoXmlReader.AddDelayedRef(System.Object,System.String,System.String)" /> to remember the referring object,
	/// the property or other identifying string of the reference in this referring object,
	/// and the reference string to the referred-to object.  This reference string is the
	/// same as the key used to identify shared objects.
	/// </para>
	/// <para>
	/// Then the <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" /> method, called as a "second pass"
	/// after all of the elements in the XML root have been read, iterates over all of those
	/// delayed references, calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" /> to resolve the reference,
	/// and then calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeUpdateReference(System.Type,System.Object,System.String,System.Object)" /> so that the transformer
	/// gets a chance to fix up the reference.
	/// </para>
	/// <para>
	/// The various <c>Invoke...</c> methods do the actual lookup for a transformer
	/// and invoke the respective method on the first transformer found.
	/// </para>
	/// </remarks>
	/// <example>
	/// This reader could be used as follows:
	/// <code>
	/// public void LoadSimpleXml(String path) {
	///   myView.Document.Clear();  // assume we're replacing current document contents
	///
	///   GoXmlReader xr = new GoXmlReader();
	///   // tell the reader how to handle two kinds of elements
	///   xr.AddTransformer(new SimpleXmlTransformBasicNode());
	///   xr.AddTransformer(new SimpleXmlTransformLink());
	///   xr.RootObject = myView.Document;
	///
	///   using (StreamReader sr = new StreamReader(path)) {
	///     xr.Consume(sr);
	///   }
	/// }
	/// </code>
	/// See the description of <see cref="T:Northwoods.Go.Xml.GoXmlTransformer" /> for example transformer definitions.
	/// </example>
	public class GoXmlReader : GoXmlReaderWriterBase
	{
		internal struct DelayedInfo
		{
			public object Delayed;

			public string Property;

			public string Reference;
		}

		private Dictionary<string, List<IGoXmlTransformer>> myConsumers = new Dictionary<string, List<IGoXmlTransformer>>();

		private bool myUseDOM;

		private XmlNode myReaderNode;

		private object myRootObject;

		private XmlReaderSettings myReaderSettings = new XmlReaderSettings();

		private XmlReader myReader;

		private XmlDocument myDocument;

		private Dictionary<string, object> mySharedObjects = new Dictionary<string, object>();

		private List<DelayedInfo> myDelayeds = new List<DelayedInfo>();

		/// <summary>
		/// Gets or sets the <c>XmlReaderSettings</c> used in the call to <c>XmlReader.Create</c>
		/// to customize the <c>XmlReader</c>.
		/// </summary>
		/// <value>
		/// The default value is just a newly created <c>XmlReaderSettings</c>.
		/// </value>
		public XmlReaderSettings XmlReaderSettings
		{
			get
			{
				return myReaderSettings;
			}
			set
			{
				myReaderSettings = value;
			}
		}

		/// <summary>
		/// Gets the <c>XmlReader</c> used to actually read XML from the input stream.
		/// </summary>
		/// <remarks>
		/// The value is constructed and initialized during XML generation, and is
		/// automatically closed afterwards.  If <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> will just load the whole DOM (available as the
		/// <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlDocument" /> property), close the stream, and set this property to null.
		/// </remarks>
		public XmlReader XmlReader => myReader;

		/// <summary>
		/// Gets the <c>XmlDocument</c> that was loaded from the stream/file.
		/// </summary>
		/// <remarks>
		/// This value is supplied by a call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlDocument)" />,
		/// or it is created by a call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" /> if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true.
		/// </remarks>
		public XmlDocument XmlDocument => myDocument;

		/// <summary>
		/// Gets or sets the current <c>XmlNode</c> of the DOM that this reader
		/// is looking at.
		/// </summary>
		/// <value>
		/// An <c>XmlNode</c>, or null if reading iteratively from the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />
		/// because <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is false or because no <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlDocument" /> was supplied.
		/// </value>
		/// <remarks>
		/// This property needs to be maintained by implementations of
		/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeBody(System.Object)" />.
		/// </remarks>
		public XmlNode ReaderNode
		{
			get
			{
				return myReaderNode;
			}
			set
			{
				myReaderNode = value;
			}
		}

		/// <summary>
		/// Gets or sets the object representing the root element of the XML document
		/// being read.
		/// </summary>
		/// <value>
		/// Initially this is null and gets set to the value returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// and then to the value returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />.
		/// You may want to provide an <c>IList</c> or a <c>IGoCollection</c> that
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" /> will <c>Add</c> to.
		/// </value>
		/// <remarks>
		/// If all of the top-level objects that will be read are <c>GoObject</c>s,
		/// it is fairly common to provide a <c>GoDocument</c>, so that all those new
		/// nodes and links get added to that document.
		/// After <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" /> or <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlDocument)" />
		/// are finished, this property is reset to null.
		/// </remarks>
		public object RootObject
		{
			get
			{
				return myRootObject;
			}
			set
			{
				myRootObject = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s should
		/// use the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlDocument" /> or the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />.
		/// </summary>
		/// <value>
		/// This defaults to false; set this to true before calling <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" />
		/// if you need to be able to "look-ahead" in the XML by traversing the DOM.
		/// This value is automatically set to true if you call <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlDocument)" />.
		/// </value>
		/// <remarks>
		/// The DOM is typically used to make it easier to handle references
		/// between objects.
		/// </remarks>
		public bool UseDOM
		{
			get
			{
				return myUseDOM;
			}
			set
			{
				myUseDOM = value;
			}
		}

		/// <summary>
		/// Create a reader with a set of <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s defined.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.RegisterTransformers" />.
		/// </remarks>
		public GoXmlReader()
		{
			XmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
			RegisterTransformers();
		}

		/// <summary>
		/// Look up a shared object in a hashtable by identifier.
		/// </summary>
		/// <param name="key">a String</param>
		/// <returns>an object, or null if not found or if <paramref name="key" /> is null</returns>
		/// <remarks>
		/// If <paramref name="key" /> is null, this method returns null.
		/// </remarks>
		public object FindShared(string key)
		{
			if (key == null)
			{
				return null;
			}
			mySharedObjects.TryGetValue(key, out object value);
			return value;
		}

		/// <summary>
		/// Remember an object in a hashtable, using a particular identifier name.
		/// </summary>
		/// <param name="key">a String name</param>
		/// <param name="val">some object</param>
		/// <remarks>
		/// If <paramref name="key" /> is null, this method does nothing.
		/// </remarks>
		public void MakeShared(string key, object val)
		{
			if (key != null)
			{
				mySharedObjects[key] = val;
			}
		}

		/// <summary>
		/// Remove all objects from the table of shared objects.
		/// </summary>
		/// <remarks>
		/// This also resets the IDs returned by calls to <see cref="M:Northwoods.Go.Xml.GoXmlReader.MakeShared(System.String,System.Object)" />.
		/// </remarks>
		public void ClearAllShareds()
		{
			mySharedObjects.Clear();
		}

		/// <summary>
		/// Remember an object, one of its properties, and the string representation
		/// of a reference to another object.
		/// </summary>
		/// <param name="val">
		/// The object that contains an unresolved reference
		/// </param>
		/// <param name="prop">
		/// a <c>String</c> that the names the property or somehow identifies which
		/// reference in the object given by <paramref name="val" /> needs to be
		/// updated with the correct reference in an implementation of
		/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" />
		/// </param>
		/// <param name="refstr">
		/// a <c>String</c> that holds information needed to resolve the reference
		/// in an implementation of <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" />;
		/// normally this is an ID returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" />
		/// </param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" /> uses this stored information
		/// to fix up references.
		/// </remarks>
		public void AddDelayedRef(object val, string prop, string refstr)
		{
			DelayedInfo item = default(DelayedInfo);
			item.Delayed = val;
			item.Property = prop;
			item.Reference = refstr;
			myDelayeds.Add(item);
		}

		/// <summary>
		/// Remove all entries from the table of delayed references.
		/// </summary>
		public void ClearAllDelayeds()
		{
			myDelayeds.Clear();
		}

		/// <summary>
		/// Only a subclass of a <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> can provide a
		/// value for the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> property.
		/// </summary>
		/// <param name="w">an <c>XmlReader</c> initialized at the
		/// start of <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" />, or set to null at the end of 
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" /> or <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> after closing the <c>XmlReader</c></param>
		protected virtual void SetXmlReader(XmlReader w)
		{
			myReader = w;
		}

		/// <summary>
		/// Only a subclass of a <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> can provide a value
		/// for the DOM.
		/// </summary>
		/// <param name="d">an <c>XmlDocument</c> loaded from the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />
		/// by <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true</param>
		protected virtual void SetXmlDocument(XmlDocument d)
		{
			myDocument = d;
		}

		/// <summary>
		/// Start an <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> and consume XML.
		/// </summary>
		/// <param name="source">a <c>Stream</c></param>
		/// <returns>
		/// an object that represents the whole document, as returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// </returns>
		/// <remarks>
		/// <para>
		/// This does some initialization, creates a <c>XmlReader</c>,
		/// calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> to perhaps create a <c>XmlDocument</c>.
		/// Then it calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeInstructions" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" />.
		/// Note that if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true, the whole DOM is loaded
		/// by <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> and the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> will be null.
		/// </para>
		/// <para>
		/// This calls <c>XmlReader.Create</c> with the value of
		/// <c>this.XmlReaderSettings</c>.
		/// </para>
		/// </remarks>
		public object Consume(Stream source)
		{
			XmlReader xmlReader = null;
			object obj = null;
			try
			{
				xmlReader = XmlReader.Create(source, XmlReaderSettings);
				return Consume(xmlReader);
			}
			finally
			{
				xmlReader?.Close();
			}
		}

		/// <summary>
		/// Start an <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> and consume XML.
		/// </summary>
		/// <param name="source">a <c>TextReader</c></param>
		/// <returns>
		/// an object that represents the whole document, as returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// </returns>
		/// <remarks>
		/// <para>
		/// This does some initialization, creates a <c>XmlReader</c>,
		/// calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> to perhaps create a <c>XmlDocument</c>.
		/// Then it calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeInstructions" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" />.
		/// Note that if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true, the whole DOM is loaded
		/// by <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> and the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> will be null.
		/// </para>
		/// <para>
		/// This calls <c>XmlReader.Create</c> with the value of
		/// <c>this.XmlReaderSettings</c>.
		/// </para>
		/// </remarks>
		public object Consume(TextReader source)
		{
			XmlReader xmlReader = null;
			object obj = null;
			try
			{
				xmlReader = XmlReader.Create(source, XmlReaderSettings);
				return Consume(xmlReader);
			}
			finally
			{
				xmlReader?.Close();
			}
		}

		/// <summary>
		/// Consume XML from an <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />.
		/// </summary>
		/// <param name="source">an <c>XmlReader</c></param>
		/// <returns>
		/// an object that represents the whole document, as returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// </returns>
		/// <remarks>
		/// This does some initialization,
		/// calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> to perhaps create a <c>XmlDocument</c>.
		/// Then it calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeInstructions" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" />.
		/// Note that if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is true, the whole DOM is loaded
		/// by <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" /> and the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> will be null.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual object Consume(XmlReader source)
		{
			Initialize();
			if (base.TransformerCount == 0)
			{
				throw new InvalidOperationException("No IGoXmlTransformers have been registered for any Types");
			}
			object result = null;
			try
			{
				SetXmlReader(source);
				LoadDOM();
				ConsumeCommon();
				result = ConsumeCleanupCommon();//bylzy123
				return result;
			}
			finally
			{
				result = ConsumeCleanupCommon();
			}
		}

		private void ConsumeCommon()
		{
			ConsumeInstructions();
			if (XmlReader != null)
			{
				XmlReader.MoveToContent();
			}
			else if (XmlDocument != null)
			{
				ReaderNode = XmlDocument.DocumentElement;
			}
			RootObject = ConsumeRootElement();
			RootObject = ConsumeRootAttributes(RootObject);
			base.ObjectStack.Add(RootObject);
			ConsumeRootBody(RootObject);
			ProcessDelayedObjects();
		}

		private object ConsumeCleanupCommon()
		{
			object rootObject = RootObject;
			RootObject = null;
			base.ObjectStack.Clear();
			SetXmlReader(null);
			return rootObject;
		}

		/// <summary>
		/// Consume an <c>XmlDocument</c>
		/// </summary>
		/// <param name="doc">an <c>XmlDocument</c></param>
		/// <returns>
		/// an object that represents the whole document, as returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// </returns>
		/// <remarks>
		/// After some initialization, this calls, in order, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeInstructions" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />, <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />,
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" />, and <see cref="M:Northwoods.Go.Xml.GoXmlReader.ProcessDelayedObjects" />.
		/// This does not use a <c>XmlReader</c> or call <see cref="M:Northwoods.Go.Xml.GoXmlReader.LoadDOM" />,
		/// since an <c>XmlDocument</c> is supplied.
		/// <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" /> is set to true.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual object Consume(XmlDocument doc)
		{
			Initialize();
			if (base.TransformerCount == 0)
			{
				throw new InvalidOperationException("No IGoXmlTransformers have been registered for any Types");
			}
			object result = null;
			try
			{
				UseDOM = true;
				SetXmlReader(null);
				SetXmlDocument(doc);
				ConsumeCommon();
				return result;
			}
			finally
			{
				result = ConsumeCleanupCommon();
			}
		}

		/// <summary>
		/// Do the initialization needed by <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlReader)" />
		/// and <see cref="M:Northwoods.Go.Xml.GoXmlReader.Consume(System.Xml.XmlDocument)" />.
		/// </summary>
		protected virtual void Initialize()
		{
			base.ObjectStack.Clear();
			ClearAllShareds();
			ClearAllDelayeds();
			SetupConsumers();
		}

		/// <summary>
		/// This method constructs and loads an <c>XmlDocument</c>
		/// from the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> if <see cref="P:Northwoods.Go.Xml.GoXmlReader.UseDOM" />
		/// is true.
		/// </summary>
		/// <remarks>
		/// If this does load a <c>XmlDocument</c> from the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" />,
		/// it sets the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> property to null afterwards,
		/// and sets the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlDocument" /> property to that new <c>XmlDocument</c>.
		/// You probably do not need to override this method.
		/// </remarks>
		protected virtual void LoadDOM()
		{
			if (UseDOM && XmlReader != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				SetXmlDocument(xmlDocument);
				xmlDocument.Load(XmlReader);
				SetXmlReader(null);
			}
		}

		private void SetupConsumers()
		{
			myConsumers.Clear();
			foreach (KeyValuePair<Type, IGoXmlTransformer> transformer in base.Transformers)
			{
				Type key = transformer.Key;
				IGoXmlTransformer value = transformer.Value;
				string elementName = value.ElementName;
				if (elementName != null && elementName.Length > 0)
				{
					myConsumers.TryGetValue(elementName, out List<IGoXmlTransformer> value2);
					if (value2 == null)
					{
						value2 = new List<IGoXmlTransformer>();
						value2.Add(value);
						myConsumers[elementName] = value2;
					}
					else
					{
						int index = value2.Count;
						for (int i = 0; i < value2.Count; i++)
						{
							Type transformerType = value2[i].TransformerType;
							if (key.IsSubclassOf(transformerType))
							{
								index = i;
								break;
							}
						}
						value2.Insert(index, value);
					}
				}
			}
		}

		/// <summary>
		/// Consume XML instructions before the root element.
		/// </summary>
		/// <remarks>
		/// By default this does nothing.
		/// </remarks>
		protected virtual void ConsumeInstructions()
		{
		}

		/// <summary>
		/// Start the root element.
		/// </summary>
		/// <returns>
		/// <para>
		/// The data structure representing the whole document body.
		/// If <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" /> is non-null, it returns that.
		/// If the current element name is associated with a <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />
		/// and that transformer's <see cref="P:Northwoods.Go.Xml.GoXmlBindingTransformer.Prototype" /> is a
		/// <see cref="T:Northwoods.Go.GoDocument" />, this returns a copy of that <see cref="T:Northwoods.Go.GoDocument" />.
		/// Otherwise this returns an empty <c>List</c>.
		/// </para>
		/// </returns>
		/// <remarks>
		/// <para>
		/// You could override this to return a particular collection or
		/// collection-like object, such as a <c>GoDocument</c> or a <c>GoCollection</c>.
		/// If you return a different kind of root object, you may need to override
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" /> and <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" />
		/// to handle initializing and adding new objects to your root object.
		/// </para>
		/// <para>
		/// The <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" /> property is set to the value of this method.
		/// All <c>Consume</c> methods will return this value, after having reset
		/// the <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" /> property to null.
		/// </para>
		/// </remarks>
		protected virtual object ConsumeRootElement()
		{
			if (RootObject != null)
			{
				return RootObject;
			}
			XmlReader xmlReader = XmlReader;
			if (xmlReader != null)
			{
				string localName = xmlReader.LocalName;
				myConsumers.TryGetValue(localName, out List<IGoXmlTransformer> value);
				if (value != null)
				{
					for (int i = 0; i < value.Count; i++)
					{
						GoXmlBindingTransformer goXmlBindingTransformer = value[i] as GoXmlBindingTransformer;
						if (goXmlBindingTransformer != null)
						{
							GoDocument goDocument = goXmlBindingTransformer.Prototype as GoDocument;
							if (goDocument != null)
							{
								return goDocument.Copy();
							}
						}
					}
				}
			}
			else if (ReaderNode != null)
			{
				string localName2 = ReaderNode.LocalName;
				myConsumers.TryGetValue(localName2, out List<IGoXmlTransformer> value2);
				if (value2 != null)
				{
					for (int j = 0; j < value2.Count; j++)
					{
						GoXmlBindingTransformer goXmlBindingTransformer2 = value2[j] as GoXmlBindingTransformer;
						if (goXmlBindingTransformer2 != null)
						{
							GoDocument goDocument2 = goXmlBindingTransformer2.Prototype as GoDocument;
							if (goDocument2 != null)
							{
								return goDocument2.Copy();
							}
						}
					}
				}
			}
			return new List<object>();
		}

		/// <summary>
		/// Consume attributes for the root element.
		/// </summary>
		/// <param name="obj">the data structure representing the whole document body,
		/// the result of a call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" /></param>
		/// <returns>
		/// The data structure representing the whole document body.
		/// </returns>
		/// <remarks>
		/// By default this does nothing but return its argument.
		/// However, if <see cref="P:Northwoods.Go.Xml.GoXmlReader.RootObject" /> is a <see cref="T:Northwoods.Go.GoDocument" />,
		/// and if there is a <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" /> registered for that <c>GoDocument</c> type,
		/// this calls <see cref="M:Northwoods.Go.Xml.GoXmlBindingTransformer.ConsumeAttributes(System.Object)" /> on that transformer,
		/// passing it the <see cref="T:Northwoods.Go.GoDocument" />.
		/// This provides an easy-to-define mechanism for binding document properties with root element attributes.
		/// </remarks>
		protected virtual object ConsumeRootAttributes(object obj)
		{
			GoDocument goDocument = RootObject as GoDocument;
			if (goDocument != null)
			{
				(FindTransformer(goDocument.GetType()) as GoXmlBindingTransformer)?.ConsumeAttributes(goDocument);
			}
			return obj;
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" /> for each element found in the root element.
		/// </summary>
		/// <param name="obj">the data structure representing the whole document body,
		/// the result of a call to <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" /></param>
		/// <remarks>
		/// The default implementation assumes <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />
		/// returns either an <c>IList</c> or an <c>IGoCollection</c>
		/// (such as a <c>GoDocument</c> or a <c>GoCollection</c>)
		/// representing the collection of objects to be read from the XML.
		/// If the object returned by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" /> is a
		/// <c>GoObject</c> and root object is a <c>IGoCollection</c>, the
		/// <c>GoObject</c> is <c>Add</c>'ed to the <c>IGoCollection</c>.
		/// Otherwise, if the root object is an <c>IList</c>, the new
		/// object is <c>Add</c>ed to the <c>IList</c>.
		/// If you have overridden <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" /> or <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootAttributes(System.Object)" />
		/// to return a different kind of object, you may need to override this method too.
		/// </remarks>
		protected virtual void ConsumeRootBody(object obj)
		{
			XmlReader xmlReader = XmlReader;
			IList list = obj as IList;
			ICollection<GoObject> collection = obj as ICollection<GoObject>;
			IGoCollection goCollection = obj as IGoCollection;
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
						object obj2 = ConsumeObject();
						if (obj2 != null)
						{
							if (goCollection != null && obj2 is GoObject)
							{
								goCollection.Add((GoObject)obj2);
							}
							else if (list != null)
							{
								list.Add(obj2);
							}
							else if (collection != null && obj2 is GoObject)
							{
								collection.Add((GoObject)obj2);
							}
						}
					}
					else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == name)
					{
						break;
					}
				}
			}
			else if (XmlDocument != null)
			{
				{
					IEnumerator enumerator = XmlDocument.DocumentElement.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlNode xmlNode2 = ReaderNode = (XmlNode)enumerator.Current;
							object obj3 = ConsumeObject();
							if (obj3 != null)
							{
								if (goCollection != null && obj3 is GoObject)
								{
									goCollection.Add((GoObject)obj3);
								}
								else if (list != null)
								{
									list.Add(obj3);
								}
								else if (collection != null && obj3 is GoObject)
								{
									collection.Add((GoObject)obj3);
								}
							}
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
			}
		}

		/// <summary>
		/// This produces an object from the current element.
		/// </summary>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeAllocate" /> produces an object,
		/// this starts reading an element by calling <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeAttributes(System.Type,System.Object)" />
		/// and then <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeBody(System.Type,System.Object)" />, passing it that object.
		/// Finally this calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeObjectFinish(System.Type,System.Object)" />.
		/// This method will return null if <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeAllocate" /> returns null.
		/// This method is called by <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootBody(System.Object)" /> and by any other code
		/// that needs to read/construct an object.
		/// You probably do not need to override this method.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.Xml.GoXmlReader.ReaderNode" />
		public virtual object ConsumeObject()
		{
			object obj = InvokeAllocate();
			if (obj != null)
			{
				Type type = obj.GetType();
				InvokeConsumeAttributes(type, obj);
				base.ObjectStack.Add(obj);
				InvokeConsumeBody(type, obj);
				if (base.ObjectStack.Count > 0)
				{
					base.ObjectStack.RemoveAt(base.ObjectStack.Count - 1);
				}
				InvokeConsumeObjectFinish(type, obj);
			}
			return obj;
		}

		/// <summary>
		/// This method fixes up any objects that had references that were unable to
		/// be resolved during the regular Consume steps.
		/// </summary>
		/// <remarks>
		/// This uses the information remembered from calls to <see cref="M:Northwoods.Go.Xml.GoXmlReader.AddDelayedRef(System.Object,System.String,System.String)" />.
		/// For each object/property pair that was delayed, this method calls
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.FindShared(System.String)" /> to get the real object corresponding to the
		/// saved string reference, and calls <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeUpdateReference(System.Type,System.Object,System.String,System.Object)" />
		/// to actually assign the object's property with that reference.
		/// You probably do not need to override this method.
		/// </remarks>
		public virtual void ProcessDelayedObjects()
		{
			for (int i = 0; i < myDelayeds.Count; i++)
			{
				DelayedInfo delayedInfo = myDelayeds[i];
				object delayed = delayedInfo.Delayed;
				object obj = FindShared(delayedInfo.Reference);
				if (obj == null)
				{
					obj = delayedInfo.Reference;
				}
				InvokeUpdateReference(delayed.GetType(), delayed, delayedInfo.Property, obj);
			}
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with its <see cref="P:Northwoods.Go.Xml.IGoXmlTransformer.ElementName" />.
		/// </summary>
		/// <remarks>
		/// This searches for the most type-specific <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> associated
		/// with a <see cref="P:Northwoods.Go.Xml.IGoXmlTransformer.ElementName" />.
		/// </remarks>
		/// <returns>
		/// This returns the result of calling <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" />.
		/// If no transformer is found, this returns false.
		/// This method is practically never overridden.
		/// </returns>
		public virtual object InvokeAllocate()
		{
			XmlReader xmlReader = XmlReader;
			if (xmlReader != null)
			{
				string localName = xmlReader.LocalName;
				myConsumers.TryGetValue(localName, out List<IGoXmlTransformer> value);
				if (value != null)
				{
					for (int i = 0; i < value.Count; i++)
					{
						object obj = value[i].Allocate();
						if (obj != null)
						{
							return obj;
						}
					}
				}
			}
			else if (ReaderNode != null)
			{
				string localName2 = ReaderNode.LocalName;
				myConsumers.TryGetValue(localName2, out List<IGoXmlTransformer> value2);
				if (value2 != null)
				{
					for (int j = 0; j < value2.Count; j++)
					{
						object obj2 = value2[j].Allocate();
						if (obj2 != null)
						{
							return obj2;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Call the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />.<see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeAttributes(System.Object)" />
		/// method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeAttributes(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to the method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeConsumeAttributes(Type type, object obj)
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
			transformer.ConsumeAttributes(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeBody(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeBody(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to the method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeConsumeBody(Type type, object obj)
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
			transformer.ConsumeBody(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeChild(System.Object,System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="parent">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeChild(System.Object,System.Object)" /></param>
		/// <param name="child">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeChild(System.Object,System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="parent" /> and <paramref name="child" /> parameters are passed to the method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeConsumeChild(Type type, object parent, object child)
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
			transformer.ConsumeChild(parent, child);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeObjectFinish(System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeObjectFinish(System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" /> parameter is passed to the method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeConsumeObjectFinish(Type type, object obj)
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
			transformer.ConsumeObjectFinish(obj);
		}

		/// <summary>
		/// Call the <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" /> method of the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />
		/// that is associated with the given <paramref name="type" />.
		/// </summary>
		/// <param name="type">the <c>Type</c> for which we seek an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		/// <param name="obj">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" /></param>
		/// <param name="prop">a <c>String</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" /></param>
		/// <param name="referred">an <c>Object</c> to be passed to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.UpdateReference(System.Object,System.String,System.Object)" /></param>
		/// <remarks>
		/// This searches for an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> starting at the
		/// <paramref name="type" /> and proceeding up the type hierarchy.
		/// The <paramref name="obj" />, <paramref name="prop" />, and <paramref name="referred" />
		/// parameter values are passed to the method call.
		/// This method is practically never overridden.
		/// </remarks>
		public virtual void InvokeUpdateReference(Type type, object obj, string prop, object referred)
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
			transformer.UpdateReference(obj, prop, referred);
		}

		/// <summary>
		/// Return the body of the current text element as a string,
		/// ignoring any nested elements.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// If an <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> is available, and is not an empty element,
		/// this method concatenates the string values, ignoring nested elements.
		/// Otherwise, if there is a <see cref="P:Northwoods.Go.Xml.GoXmlReader.ReaderNode" />, this method
		/// just returns the node's <c>InnerText</c>.
		/// </remarks>
		public virtual string ReadTextBody()
		{
			XmlReader xmlReader = XmlReader;
			if (xmlReader != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!xmlReader.IsEmptyElement)
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Text)
						{
							stringBuilder.Append(xmlReader.Value);
						}
						else if (xmlReader.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
					}
				}
				return stringBuilder.ToString();
			}
			if (ReaderNode != null)
			{
				return ReaderNode.InnerText;
			}
			return "";
		}

		/// <summary>
		/// Return an attribute's string value.
		/// </summary>
		/// <param name="name">the name of the attribute</param>
		/// <returns>
		/// The value of the attribute named by <paramref name="name" />;
		/// if <paramref name="name" /> is null or if the attribute is not present
		/// in the <see cref="P:Northwoods.Go.Xml.GoXmlReader.XmlReader" /> or <see cref="P:Northwoods.Go.Xml.GoXmlReader.ReaderNode" />,
		/// this method returns null.
		/// </returns>
		public virtual string ReadAttrVal(string name)
		{
			if (name == null)
			{
				return null;
			}
			string result = null;
			XmlReader xmlReader = XmlReader;
			if (xmlReader != null)
			{
				result = xmlReader[name];
			}
			else
			{
				XmlNode readerNode = ReaderNode;
				if (readerNode != null)
				{
					XmlAttribute xmlAttribute = readerNode.Attributes[name];
					if (xmlAttribute != null)
					{
						result = xmlAttribute.Value;
					}
				}
			}
			return result;
		}
	}
}
