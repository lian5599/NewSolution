using System;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// This interface represents the methods that <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> or <see cref="T:Northwoods.Go.Xml.GoXmlReader" />
	/// will call to generate or consume XML for an arbitrary type of object.
	/// </summary>
	public interface IGoXmlTransformer
	{
		/// <summary>
		/// Gets or sets which <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> is using this transformer.
		/// </summary>
		GoXmlWriter Writer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets which <see cref="T:Northwoods.Go.Xml.GoXmlReader" /> is using this transformer.
		/// </summary>
		GoXmlReader Reader
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <c>Type</c> for which these transformer methods apply.
		/// </summary>
		/// <remarks>
		/// This will be used for both writing/generation and reading/consumption.
		/// </remarks>
		Type TransformerType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the element name for this transformer.
		/// </summary>
		/// <remarks>
		/// This will be used for both writing/generation and reading/consumption.
		/// The name should be a local name -- it should not have any prefix and colon embedded in it.
		/// </remarks>
		string ElementName
		{
			get;
			set;
		}

		/// <summary>
		/// Return true if the <see cref="T:Northwoods.Go.Xml.GoXmlWriter" /> should not generate XML for an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		bool SkipGeneration(object obj);

		/// <summary>
		/// This method is called in a first pass over the objects,
		/// remembering any shared objects and perhaps rendering definition elements for those shared objects.
		/// </summary>
		/// <param name="obj"></param>
		void GenerateDefinitions(object obj);

		/// <summary>
		/// Start an element for an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if an element was started</returns>
		bool GenerateElement(object obj);

		/// <summary>
		/// Generate attributes for the current element, helping to render an object.
		/// </summary>
		/// <param name="obj"></param>
		void GenerateAttributes(object obj);

		/// <summary>
		/// Generate text and/or nested elements that render an object, after all
		/// calls to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateAttributes(System.Object)" />.
		/// </summary>
		/// <param name="obj"></param>
		void GenerateBody(object obj);

		/// <summary>
		/// Finish the generation of elements started by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateElement(System.Object)" />,
		/// after all calls to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.GenerateBody(System.Object)" />.
		/// </summary>
		/// <param name="obj"></param>
		void GenerateElementFinish(object obj);

		/// <summary>
		/// Construct an object, given the current state of the <see cref="T:Northwoods.Go.Xml.GoXmlReader" />.
		/// </summary>
		/// <returns>an object, or null if the current element does not require creating an object</returns>
		object Allocate();

		/// <summary>
		/// Use attribute values to further initialize the object constructed by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" />.
		/// </summary>
		/// <param name="obj">the object constructed by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /></param>
		void ConsumeAttributes(object obj);

		/// <summary>
		/// Read any text or elements contained in the current element to continue building the object,
		/// after all calls to <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeAttributes(System.Object)" />.
		/// </summary>
		/// <param name="obj">the object constructed by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /></param>
		void ConsumeBody(object obj);

		/// <summary>
		/// Process a child object constructed by having consumed a child element of this body,
		/// as part of the implementation of a <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.ConsumeBody(System.Object)" />.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		void ConsumeChild(object parent, object child);

		/// <summary>
		/// Finish any initialization of the object allocated by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" />,
		/// whose attributes have been read and whose body has been traversed and any child objects consumed.
		/// </summary>
		/// <param name="obj">the object constructed by <see cref="M:Northwoods.Go.Xml.IGoXmlTransformer.Allocate" /></param>
		void ConsumeObjectFinish(object obj);

		/// <summary>
		/// Update the property named by <paramref name="prop" /> in the given object
		/// to the <paramref name="referred" /> object.
		/// </summary>
		/// <param name="obj">an object being constructed, with a reference property that was delayed and now needs updating</param>
		/// <param name="prop">the name of the reference property belonging to <paramref name="obj" /> that needs to be updated</param>
		/// <param name="referred">the object that the property should be referring to; if the object was not found,
		/// the string that was used as the search key</param>
		void UpdateReference(object obj, string prop, object referred);
	}
}
