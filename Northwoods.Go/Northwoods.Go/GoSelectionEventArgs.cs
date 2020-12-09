using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Holds information for the <see cref="T:Northwoods.Go.GoView" /> events that involve
	/// a <see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> but are not associated with any input event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Although both this kind of event and <see cref="T:Northwoods.Go.GoObjectEventArgs" />
	/// are associated with a particular <see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" />, this class is
	/// used when no particular mouse input event information is relevant.
	/// Such events include:
	/// <see cref="E:Northwoods.Go.GoView.LinkCreated" />, 
	/// <see cref="E:Northwoods.Go.GoView.LinkRelinked" />, 
	/// <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" />, 
	/// <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" />, 
	/// <see cref="E:Northwoods.Go.GoView.ObjectResized" />,
	/// <c>GoView.ObjectEdited</c> (in WinForms).
	/// With these events there is only a single object involved.
	/// The name of this class, <c>GoSelectionEventArgs</c> may be somewhat misleading,
	/// since not all events involve the view's <see cref="T:Northwoods.Go.GoSelection" />.  In the list
	/// above, most do, but <c>LinkCreated</c> and <c>ObjectEdited</c> might not involve
	/// selection.
	/// </para>
	/// <para>
	/// Additional <see cref="T:Northwoods.Go.GoView" /> events are not even associated with one
	/// particular object--they use the <see cref="T:System.EventArgs" /> class instead of this class.
	/// Such events include:
	/// <see cref="E:Northwoods.Go.GoView.ClipboardCopied" />,
	/// <see cref="E:Northwoods.Go.GoView.ClipboardPasted" />,
	/// <see cref="E:Northwoods.Go.GoView.SelectionCopied" />, 
	/// <see cref="E:Northwoods.Go.GoView.SelectionDeleting" />, 
	/// <see cref="E:Northwoods.Go.GoView.SelectionDeleted" />, and
	/// <see cref="E:Northwoods.Go.GoView.SelectionMoved" />.
	/// These events implicitly use the <see cref="P:Northwoods.Go.GoView.Selection" /> to identify
	/// the set of affected objects.
	/// </para>
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoInputEventArgs" />
	/// <seealso cref="T:Northwoods.Go.GoObjectEventArgs" />
	[Serializable]
	public class GoSelectionEventArgs : EventArgs
	{
		private GoObject myObject;

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> associated with this event.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public GoObject GoObject
		{
			get
			{
				return myObject;
			}
			set
			{
				myObject = value;
			}
		}

		/// <summary>
		/// This constructor creates an <c>EventArgs</c> that knows that a particular
		/// <see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> has just been added or removed from the selection,
		/// or is the principal object for another event.
		/// </summary>
		/// <param name="obj"></param>
		public GoSelectionEventArgs(GoObject obj)
		{
			myObject = obj;
		}
	}
}
