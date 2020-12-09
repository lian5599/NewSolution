namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies ways for the <see cref="T:Northwoods.Go.GoToolAction" /> tool to
	/// invoke actions on objects.  
	/// </summary>
	/// <remarks>
	/// Tools (<see cref="T:Northwoods.Go.IGoTool" />) are ways to easily specify behavior in response
	/// to mouse events for a whole <see cref="T:Northwoods.Go.GoView" />.
	/// To have a <see cref="T:Northwoods.Go.GoObject" /> specify mouse-event behavior, have your
	/// object subclass implement this interface, and make sure the <see cref="T:Northwoods.Go.GoView" />'s
	/// <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list starts with an instance of
	/// <see cref="T:Northwoods.Go.GoToolAction" />, as it does by default.
	/// The <see cref="T:Northwoods.Go.GoToolAction" /> will handle mouse events and set the
	/// properties and invoke the methods of the object implementing this interface.
	/// Typically this will be some object where a click (and perhaps a drag)
	/// should execute some code.  One such class is <see cref="T:Northwoods.Go.GoButton" />.
	/// </remarks>
	public interface IGoActionObject
	{
		/// <summary>
		/// Gets or sets whether the <see cref="T:Northwoods.Go.GoToolAction" /> tool should consider
		/// activating this object and invoking the object's action.
		/// </summary>
		bool ActionEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the object's Action is about to be invoked,
		/// normally by a mouse button being pressed.
		/// </summary>
		bool ActionActivated
		{
			get;
			set;
		}

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoToolAction" /> as the user is
		/// starting to manipulate the object, normally by a mouse press.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that the input event occurred in</param>
		/// <param name="e">the <see cref="T:Northwoods.Go.GoInputEventArgs" /> describing the input event</param>
		void OnActionActivated(GoView view, GoInputEventArgs e);

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoToolAction" /> as the user is
		/// adjusting the object, normally by a mouse drag or move.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that the input event occurred in</param>
		/// <param name="e">the <see cref="T:Northwoods.Go.GoInputEventArgs" /> describing the input event</param>
		void OnActionAdjusted(GoView view, GoInputEventArgs e);

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoToolAction" /> to perform the object's
		/// action, normally by a mouse button being released.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that the input event occurred in</param>
		/// <param name="e">the <see cref="T:Northwoods.Go.GoInputEventArgs" /> describing the input event</param>
		void OnAction(GoView view, GoInputEventArgs e);

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoToolAction" /> to notify the
		/// object that the tool is being cancelled.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that the input event occurred in</param>
		void OnActionCancelled(GoView view);
	}
}
