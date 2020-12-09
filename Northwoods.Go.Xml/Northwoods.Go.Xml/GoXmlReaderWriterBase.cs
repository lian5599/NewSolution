using System;
using System.Collections.Generic;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// This base class for both <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> and <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />
	/// defines methods to manage the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s that they use.
	/// </summary>
	public abstract class GoXmlReaderWriterBase
	{
		private Dictionary<Type, IGoXmlTransformer> myTransformers = new Dictionary<Type, IGoXmlTransformer>();

		private List<object> myObjectStack = new List<object>();

		private object myUserObject;

		internal int TransformerCount => myTransformers.Count;

		internal Dictionary<Type, IGoXmlTransformer> Transformers => myTransformers;

		/// <summary>
		/// Gets an <c>List</c> of <c>Object</c> acting as a stack of Objects that have been created
		/// during the walking of the XML tree during <c>Consume</c>, or that have been seen
		/// during <c>Generate</c>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// During a <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.<c>Consume</c>,
		/// the bottom object, at index zero, will be the result of <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeRootElement" />.
		/// <see cref="M:Northwoods.Go.Xml.GoXmlReader.ConsumeObject" /> will push the result of <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeAllocate" />
		/// onto this stack just before calling <see cref="M:Northwoods.Go.Xml.GoXmlReader.InvokeConsumeBody(System.Type,System.Object)" />,
		/// and then pop it off immediately afterwards.
		/// Hence during the processing of an element's body,
		/// i.e. during calls to <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />.<see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeBody(System.Object)" />,
		/// the value of <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" /> will be that parent element.
		/// Access to the whole stack of objects may be needed for establishing the
		/// context in which objects should be searched for, created, or modified.
		/// For example, when reading an element representing a node that has nested
		/// elements representing parts of the node, such as ports, the transformer for
		/// ports may need to implement <see cref="M:Northwoods.Go.Xml.GoXmlTransformer.Allocate" /> to return
		/// an existing port of the node, rather than blindly allocating a new one that
		/// would conflict with the existing port.
		/// </para>
		/// <para>
		/// During a <see cref="T:Northwoods.Go.Xml.GoXmlWriter" />.<c>Generate</c>,
		/// the bottom object, at index zero, will be the value of <see cref="P:Northwoods.Go.Xml.GoXmlWriter.Objects" />.
		/// <see cref="M:Northwoods.Go.Xml.GoXmlWriter.GenerateObject(System.Object)" /> will push the argument <c>Object</c>
		/// onto this stack just before calling <see cref="M:Northwoods.Go.Xml.GoXmlWriter.InvokeGenerateBody(System.Type,System.Object)" />,
		/// and the pop it off immediately afterwards.
		/// Hence during the processing of an element's body,
		/// i.e. during calls to <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />.<see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateBody(System.Object)" />,
		/// the value of <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ParentObject" /> will be that "parent" object.
		/// Access to the whole stack of objects may be needed for establishing the
		/// context in which objects should generated, or to permit communication between the
		/// transformers for those objects.
		/// </para>
		/// </remarks>
		public List<object> ObjectStack => myObjectStack;

		/// <summary>
		/// Gets the current parent object when consuming or generating the body of an element.
		/// </summary>
		/// <value>This is just the top Object on the <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ObjectStack" />, or null if the stack is empty.</value>
		/// <remarks>
		/// This property is useful when nested calls to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /> (and
		/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeAttributes(System.Object)" />) need to look at the parent object
		/// to decide whether an existing child object should be returned instead of allocating a new one.
		/// </remarks>
		public object ParentObject
		{
			get
			{
				int count = ObjectStack.Count;
				if (count >= 1)
				{
					return ObjectStack[count - 1];
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the parent object of the current parent object when consuming or generating the body of an element.
		/// </summary>
		/// <value>This is the Object just under the top one on the <see cref="P:Northwoods.Go.Xml.GoXmlReaderWriterBase.ObjectStack" />,
		/// or null if the stack is empty or has only one object in it.</value>
		/// <remarks>
		/// This property is useful when nested calls to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /> (and
		/// <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeAttributes(System.Object)" />) need to look at the parent object of the parent object
		/// to decide whether an existing (grand-)child object should be returned instead of allocating a new one.
		/// </remarks>
		public object GrandParentObject
		{
			get
			{
				int count = ObjectStack.Count;
				if (count >= 2)
				{
					return ObjectStack[count - 2];
				}
				return null;
			}
		}

		/// <summary>
		/// Gets or sets an object holding additional information for use by your application;
		/// this property is not used by GoXml.
		/// </summary>
		/// <value>initially null</value>
		public object UserObject
		{
			get
			{
				return myUserObject;
			}
			set
			{
				myUserObject = value;
			}
		}

		/// <summary>
		/// This method creates instances of all of the standard <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />s
		/// to be used when generating XML.
		/// </summary>
		/// <remarks>
		/// If you want your own reader or writer, override this method to make
		/// sure all of your customized transformer instances are added automatically.
		/// Just call <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> with new instances
		/// of all of the transformers for the kinds of <c>GoObject</c>s
		/// that you want to handle.
		/// By default this method does not register any transformers.
		/// </remarks>
		public virtual void RegisterTransformers()
		{
		}

		/// <summary>
		/// Cause this XML writer to use the given <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> for
		/// objects of the type given by the transformer's <see cref="P:Northwoods.Go.Xml.IGoXmlTransformer.TransformerType" />.
		/// </summary>
		/// <param name="g">an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /></param>
		public void AddTransformer(IGoXmlTransformer g)
		{
			if (g != null)
			{
				SetTransformer(g.TransformerType, g);
			}
		}

		/// <summary>
		/// Cause this XML writer to use the given <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> for
		/// objects of the type given by <paramref name="t" />.
		/// </summary>
		/// <param name="t">a <c>Type</c> representing a class; must not be null</param>
		/// <param name="g">
		/// an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />;
		/// if null, removes any existing transformer for the type <paramref name="t" />
		/// </param>
		/// <remarks>
		/// Although it's most common to use <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.AddTransformer(Northwoods.Go.Xml.IGoXmlTransformer)" /> to register
		/// an <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> for a class Type, if you want to share
		/// the same transformer for multiple Types, you can do so using this method.
		/// </remarks>
		public IGoXmlTransformer SetTransformer(Type t, IGoXmlTransformer g)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			myTransformers.TryGetValue(t, out IGoXmlTransformer value);
			if (g == null)
			{
				myTransformers.Remove(t);
			}
			else
			{
				if (this is GoXmlReader)
				{
					g.Reader = (GoXmlReader)this;
				}
				if (this is GoXmlWriter)
				{
					g.Writer = (GoXmlWriter)this;
				}
				g.TransformerType = t;
				myTransformers[t] = g;
			}
			return value;
		}

		/// <summary>
		/// Returns the <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> associated with a given type.
		/// </summary>
		/// <param name="t">a <c>Type</c>, or null if <paramref name="t" /> is null</param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />, or null if none is specified
		/// for the exact <c>Type</c> <paramref name="t" />
		/// </returns>
		public IGoXmlTransformer GetTransformer(Type t)
		{
			if (t == null)
			{
				return null;
			}
			myTransformers.TryGetValue(t, out IGoXmlTransformer value);
			return value;
		}

		/// <summary>
		/// Returns the first <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" /> associated with a given type or its base types.
		/// </summary>
		/// <param name="t">a <c>Type</c>, or null if <paramref name="t" /> is null</param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.Xml.IGoXmlTransformer" />, or null if the type and none of its
		/// base <c>Type</c>s have a transformer associated with it, as returned by
		/// calls to <see cref="M:Northwoods.Go.Xml.GoXmlReaderWriterBase.GetTransformer(System.Type)" />
		/// </returns>
		public IGoXmlTransformer FindTransformer(Type t)
		{
			Type type = t;
			while (type != null)
			{
				IGoXmlTransformer transformer = GetTransformer(type);
				if (transformer != null)
				{
					return transformer;
				}
				type = type.BaseType;
			}
			return null;
		}
	}
}
