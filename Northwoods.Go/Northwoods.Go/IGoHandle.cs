namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies the properties used by <see cref="T:Northwoods.Go.GoSelection" />
	/// for displaying selection handles, normally implemented by <see cref="T:Northwoods.Go.GoHandle" />.
	/// </summary>
	public interface IGoHandle
	{
		/// <summary>
		/// Gets the object being used to implement the handle's visual representation.
		/// </summary>
		GoObject GoObject
		{
			get;
		}

		/// <summary>
		/// Gets or sets an identifier for this handle.
		/// </summary>
		/// <remarks>
		/// Since an object may get many handles when selected, this property
		/// provides a means of distinguishing them.
		/// </remarks>
		/// <seealso cref="T:Northwoods.Go.GoSelection" />
		int HandleID
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the selected object for this handle.
		/// </summary>
		/// <remarks>
		/// This value should be contained by a <see cref="T:Northwoods.Go.GoSelection" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.HandledObject" />
		GoObject SelectedObject
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the object that actually gets the handles.
		/// </summary>
		/// <remarks>
		/// This should be the same as <c>SelectedObject.SelectionObject</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.SelectedObject" />
		GoObject HandledObject
		{
			get;
		}
	}
}
