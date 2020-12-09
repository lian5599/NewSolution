using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Holds information for the <see cref="T:Northwoods.Go.GoView" /> events involving both
	/// a <see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" /> and some input event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class knows about the <see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" />
	/// that got clicked as well as about how and where the click happened.
	/// Such <see cref="T:Northwoods.Go.GoView" /> events include:
	/// <see cref="E:Northwoods.Go.GoView.ObjectSingleClicked" />,
	/// <see cref="E:Northwoods.Go.GoView.ObjectDoubleClicked" />, 
	/// <see cref="E:Northwoods.Go.GoView.ObjectContextClicked" />,
	/// and <c>GoView.ObjectHover</c> (in full Windows Forms).
	/// </para>
	/// <para>
	/// The <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropReject" /> event also uses this event argument class.
	/// Those event handlers may set the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> property
	/// to be <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" />.
	/// <see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" /> may also be null in this case.
	/// </para>
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoInputEventArgs" />
	/// <seealso cref="T:Northwoods.Go.GoSelectionEventArgs" />
	[Serializable]
	public class GoObjectEventArgs : GoInputEventArgs
	{
		private GoObject myObject;

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" /> associated with this event.
		/// </summary>
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
		/// Create a <see cref="T:Northwoods.Go.GoObjectEventArgs" /> that knows about an input event
		/// involving a <see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public GoObjectEventArgs(GoObject obj, GoInputEventArgs evt)
			: base(evt)
		{
			myObject = obj;
		}
	}
}
