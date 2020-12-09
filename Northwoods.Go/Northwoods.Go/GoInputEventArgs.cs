using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// Holds information for unified input events for views, for both
	/// keyboard input and mouse input, including mouse button and mouse wheel
	/// and drag-and-drop mouse actions, where no <see cref="T:Northwoods.Go.GoObject" />
	/// is involved.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For input events that occur in the "background", there is of course
	/// no particular <see cref="T:Northwoods.Go.GoObject" />.
	/// Such events include:
	/// <see cref="E:Northwoods.Go.GoView.BackgroundDoubleClicked" />, 
	/// <see cref="E:Northwoods.Go.GoView.BackgroundSingleClicked" />, 
	/// <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" />, 
	/// <c>GoView.ExternalObjectsDropped</c> (in WinForms), and
	/// <c>GoView.BackgroundHover</c> (in WinForms).
	/// </para>
	/// <para>
	/// For input events that do
	/// involve an object, event handlers use the <see cref="T:Northwoods.Go.GoObjectEventArgs" />
	/// class.  When no particular input information is associated with an
	/// event, <see cref="T:Northwoods.Go.GoSelectionEventArgs" /> is used when there is a
	/// particular object, and <see cref="T:System.EventArgs" /> is used otherwise.
	/// </para>
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoObjectEventArgs" />
	/// <seealso cref="T:Northwoods.Go.GoSelectionEventArgs" />
	[Serializable]
	public class GoInputEventArgs : EventArgs
	{
		private Point myViewPoint;

		private PointF myDocPoint;

		private MouseButtons myButtons;

		private Keys myModifiers;

		private Keys myKey;

		[NonSerialized]
		private MouseEventArgs myMouseEventArgs;

		[NonSerialized]
		private DragEventArgs myDragEventArgs;

		[NonSerialized]
		private KeyEventArgs myKeyEventArgs;

		private bool myDoubleClick;

		private int myDelta;

		private GoInputState myInputState = GoInputState.Start;

		/// <summary>
		/// Gets or sets the point at which this input event occurred.
		/// </summary>
		/// <value>
		/// The <c>Point</c> is in view coordinates.
		/// </value>
		/// <remarks>
		/// This should be valid for mouse and drag-and-drop events.
		/// For keyboard input, this is the last available mouse point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />
		public Point ViewPoint
		{
			get
			{
				return myViewPoint;
			}
			set
			{
				myViewPoint = value;
			}
		}

		/// <summary>
		/// Gets or sets the point at which this input event occurred.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> is in document coordinates.
		/// </value>
		/// <remarks>
		/// This should be valid for mouse and drag-and-drop events.
		/// For keyboard input, this is the last available mouse point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.ViewPoint" />
		public PointF DocPoint
		{
			get
			{
				return myDocPoint;
			}
			set
			{
				myDocPoint = value;
			}
		}

		/// <summary>
		/// Gets or sets the MouseButtons used with this input event.
		/// </summary>
		/// <value>
		/// The <c>MouseButtons</c> value will be some combination of
		/// <c>MouseButtons.Left</c>, <c>MouseButtons.Middle</c>, and <c>MouseButtons.Right</c>.
		/// </value>
		/// <remarks>
		/// This value may not be meaningful for keyboard input, but should be valid
		/// for mouse and drag-and-drop events.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Modifiers" />
		public MouseButtons Buttons
		{
			get
			{
				return myButtons;
			}
			set
			{
				myButtons = value;
			}
		}

		/// <summary>
		/// Gets or sets the modifier keys used with this input event.
		/// </summary>
		/// <value>
		/// The <c>Keys</c> value will be some combination of
		/// <c>Keys.Control</c>, <c>Keys.Shift</c>, and <c>Keys.Alt</c>.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Control" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Shift" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Alt" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Buttons" />
		public Keys Modifiers
		{
			get
			{
				return myModifiers;
			}
			set
			{
				myModifiers = value;
			}
		}

		/// <summary>
		/// Gets or sets the key pressed as this input event.
		/// </summary>
		/// <remarks>
		/// The <c>Keys</c> value will be something like <c>Keys.C</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Control" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Shift" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Alt" />
		/// <seealso cref="P:Northwoods.Go.GoInputEventArgs.Buttons" />
		public Keys Key
		{
			get
			{
				return myKey;
			}
			set
			{
				myKey = value;
			}
		}

		/// <summary>
		/// Gets or sets the MouseEventArgs associated with this input event.
		/// </summary>
		public MouseEventArgs MouseEventArgs
		{
			get
			{
				return myMouseEventArgs;
			}
			set
			{
				myMouseEventArgs = value;
			}
		}

		/// <summary>
		/// Gets or sets the DragEventArgs associated with this input event.
		/// </summary>
		public DragEventArgs DragEventArgs
		{
			get
			{
				return myDragEventArgs;
			}
			set
			{
				myDragEventArgs = value;
			}
		}

		/// <summary>
		/// Gets or sets the KeyEventArgs associated with this input event.
		/// </summary>
		public KeyEventArgs KeyEventArgs
		{
			get
			{
				return myKeyEventArgs;
			}
			set
			{
				myKeyEventArgs = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this is a double-click event.
		/// </summary>
		public bool DoubleClick
		{
			get
			{
				return myDoubleClick;
			}
			set
			{
				myDoubleClick = value;
			}
		}

		/// <summary>
		/// Gets or sets the amount of change associated with a mouse-wheel rotation.
		/// </summary>
		public int Delta
		{
			get
			{
				return myDelta;
			}
			set
			{
				myDelta = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoInputState" /> as defined by various tools.
		/// </summary>
		/// <value>
		/// This property is not meaningful or valid unless the input event is
		/// associated with certain tools such as <see cref="T:Northwoods.Go.GoToolDragging" />
		/// and <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </value>
		public GoInputState InputState
		{
			get
			{
				return myInputState;
			}
			set
			{
				myInputState = value;
			}
		}

		/// <summary>
		/// Gets whether <see cref="P:Northwoods.Go.GoInputEventArgs.Modifiers" /> has <c>Keys.Control</c> set.
		/// </summary>
		public virtual bool Control => (Modifiers & Keys.Control) == Keys.Control;

		/// <summary>
		/// Gets whether <see cref="P:Northwoods.Go.GoInputEventArgs.Modifiers" /> has <c>Keys.Shift</c> set.
		/// </summary>
		public virtual bool Shift => (Modifiers & Keys.Shift) == Keys.Shift;

		/// <summary>
		/// Gets whether <see cref="P:Northwoods.Go.GoInputEventArgs.Modifiers" /> has <c>Keys.Alt</c> set.
		/// </summary>
		public virtual bool Alt => (Modifiers & Keys.Alt) == Keys.Alt;

		/// <summary>
		/// Gets whether <see cref="P:Northwoods.Go.GoInputEventArgs.Buttons" /> equals <c>MouseButtons.Right</c>.
		/// </summary>
		public virtual bool IsContextButton => Buttons == MouseButtons.Right;

		/// <summary>
		/// The constructor produces an empty object, describing no event.
		/// </summary>
		public GoInputEventArgs()
		{
		}

		/// <summary>
		/// This copy constructor makes a copy of the argument object.
		/// </summary>
		/// <param name="evt"></param>
		public GoInputEventArgs(GoInputEventArgs evt)
		{
			ViewPoint = evt.ViewPoint;
			DocPoint = evt.DocPoint;
			Buttons = evt.Buttons;
			Modifiers = evt.Modifiers;
			Key = evt.Key;
			MouseEventArgs = evt.MouseEventArgs;
			DragEventArgs = evt.DragEventArgs;
			KeyEventArgs = evt.KeyEventArgs;
			DoubleClick = evt.DoubleClick;
			Delta = evt.Delta;
			InputState = evt.InputState;
		}
	}
}
