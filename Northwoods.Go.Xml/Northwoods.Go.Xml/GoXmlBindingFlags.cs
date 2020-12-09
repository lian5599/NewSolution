using System;

namespace Northwoods.Go.Xml
{
	/// <summary>
	/// These flags describe characteristics of an attribute-property binding
	/// use by <see cref="T:Northwoods.Go.Xml.GoXmlBindingTransformer" />.
	/// </summary>
	[Flags]
	public enum GoXmlBindingFlags
	{
		/// <summary>
		/// The normal binding of an attribute with a property.
		/// </summary>
		Default = 0x0,
		/// <summary>
		/// Do not get this object property; the XML attribute is not generated.
		/// </summary>
		/// <remarks>
		/// Presumably the property is write-only.
		/// </remarks>
		NoGet = 0x1,
		/// <summary>
		/// Do not set the object property when consuming XML.
		/// </summary>
		/// <remarks>
		/// Presumably the property is read-only.
		/// </remarks>
		NoSet = 0x2,
		/// <summary>
		/// The object property value is a reference;
		/// when reading XML, the attribute value should be a unique identifier
		/// that will be associated with the property value.
		/// </summary>
		DefinesShared = 0x4,
		/// <summary>
		/// If a property getter or setter throws an exception, instead of ignoring it, rethrow it.
		/// </summary>
		RethrowsExceptions = 0x8
	}
}
