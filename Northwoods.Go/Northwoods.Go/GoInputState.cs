namespace Northwoods.Go
{
	/// <summary>
	/// Specifies a kind of abstract input event state.
	/// </summary>
	public enum GoInputState
	{
		/// <summary>
		/// Used by tools and object event handling methods to indicate that the
		/// user is no longer interested in completing the mouse gesture command.
		/// </summary>
		/// <remarks>
		/// Typically this is caused by the user pressing the Escape key.
		/// </remarks>
		Cancel,
		/// <summary>
		/// Used by tools and object event handling methods to indicate that the
		/// user has started a mouse gesture command.
		/// </summary>
		/// <remarks>
		/// Typically this is caused by a mouse button down event.
		/// </remarks>
		Start,
		/// <summary>
		/// Used by tools and object event handling methods to indicate that the
		/// user is continuing a mouse gesture command.
		/// </summary>
		/// <remarks>
		/// Typically this is caused by a mouse move event.
		/// </remarks>
		Continue,
		/// <summary>
		/// Used by tools and object event handling methods to indicate that the
		/// user has finished a mouse gesture command.
		/// </summary>
		/// <remarks>
		/// Typically this is caused by a mouse button up event.
		/// </remarks>
		Finish
	}
}
