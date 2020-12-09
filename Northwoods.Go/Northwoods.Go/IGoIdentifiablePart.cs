namespace Northwoods.Go
{
	/// <summary>
	/// This interface provides a unique identifier for a part of a document.
	/// </summary>
	/// <remarks>
	/// <see cref="T:Northwoods.Go.GoLink" />, <see cref="T:Northwoods.Go.GoLabeledLink" />, <see cref="T:Northwoods.Go.GoNode" />,
	/// <see cref="T:Northwoods.Go.GoPort" />, and <see cref="T:Northwoods.Go.GoComment" /> all implement this interface.
	/// </remarks>
	public interface IGoIdentifiablePart
	{
		/// <summary>
		/// Gets or sets the unique ID of this part in its document, a non-negative integer.
		/// </summary>
		/// <value>
		/// This value is invalid unless this <see cref="T:Northwoods.Go.GoObject" /> is part of a
		/// <see cref="T:Northwoods.Go.GoDocument" /> whose <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> property is true.
		/// This property should not be set when this object is part of a document.
		/// The initial value should be -1, which instructs the document,
		/// when the object is added to the document, to find a unique ID and assign
		/// it to this object.
		/// </value>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is false, you may set and get this
		/// <c>PartID</c>property.  However, you will need to manage any uniqueness of values,
		/// and (if desired) you will need to manage a hashtable to make finding parts fast.
		/// <see cref="T:Northwoods.Go.GoDocument" /> will not set any <c>PartID</c> unless
		/// <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is true or unless you call
		/// <see cref="M:Northwoods.Go.GoDocument.EnsureUniquePartID" />.
		/// </remarks>
		int PartID
		{
			get;
			set;
		}
	}
}
