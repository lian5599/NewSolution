using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Holds information for the <see cref="T:Northwoods.Go.GoView" /> event <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" />
	/// that occurs when a mouse moves onto or out of a <see cref="T:Northwoods.Go.GoObject" />.
	/// </summary>
	/// <remarks>
	/// The <see cref="P:Northwoods.Go.GoObjectEnterLeaveEventArgs.From" /> property refers to the object that the mouse had been over.
	/// The <see cref="P:Northwoods.Go.GoObjectEnterLeaveEventArgs.To" /> property refers to the object that the mouse is now over.
	/// Either property may be null, if the mouse had been, or now is, over the view's background.
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoInputEventArgs" />
	[Serializable]
	public class GoObjectEnterLeaveEventArgs : GoInputEventArgs
	{
		private GoObject myFrom;

		private GoObject myTo;

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoObject" /> that the mouse had been over.
		/// </summary>
		/// <value>
		/// If this value is null, the mouse had not been over any object, but had been in the view's background.
		/// </value>
		public GoObject From => myFrom;

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoObject" /> that the mouse is now over.
		/// </summary>
		/// <value>
		/// If this value is null, the mouse is no longer over an object, but is in the view's background.
		/// </value>
		public GoObject To => myTo;

		/// <summary>
		/// Create a <see cref="T:Northwoods.Go.GoObjectEnterLeaveEventArgs" /> that knows about an input event
		/// involving a <see cref="T:Northwoods.Go.GoObject" />.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="evt"></param>
		public GoObjectEnterLeaveEventArgs(GoObject from, GoObject to, GoInputEventArgs evt)
			: base(evt)
		{
			myFrom = from;
			myTo = to;
		}
	}
}
