using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This abstract class provides the base for all of the predefined tools.
	/// </summary>
	[Serializable]
	public abstract class GoTool : IGoTool
	{
		private static Size myDragSize = SystemInformation.DragSize;

		[NonSerialized]
		private GoView myView;

		[NonSerialized]
		private string myTransactionResult;

		[NonSerialized]
		private GoObject myCurrentObject;

		[NonSerialized]
		private bool myCurrentObjectWasSelected;

		/// <summary>
		/// Gets the view for which this tool is handling canonicalized input events.
		/// </summary>
		public GoView View
		{
			get
			{
				return myView;
			}
			set
			{
				if (value != null)
				{
					myView = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dimensions, in pixels, of the rectangle that a drag operation must extend
		/// to be considered a drag operation.
		/// </summary>
		/// <value>
		/// This <c>Size</c> is in view coordinates, not in document coordinates.
		/// The default value is 4x4.
		/// </value>
		/// <remarks>The rectangle is centered on the mouse-down point.</remarks>
		public static Size DragSize
		{
			get
			{
				return myDragSize;
			}
			set
			{
				myDragSize = value;
			}
		}

		/// <summary>
		/// Gets this view's FirstInput property.
		/// </summary>
		public GoInputEventArgs FirstInput => View.FirstInput;

		/// <summary>
		/// Gets this view's LastInput property.
		/// </summary>
		public GoInputEventArgs LastInput => View.LastInput;

		/// <summary>
		/// Gets this view's Selection property.
		/// </summary>
		public GoSelection Selection => View.Selection;

		/// <summary>
		/// Gets or sets this tool's CurrentObject property.
		/// </summary>
		/// <remarks>
		/// Often different methods of a tool will need to deal with the "current"
		/// <see cref="T:Northwoods.Go.GoObject" /> that the user is working with.  This property
		/// is provided so each tool doesn't need to define it.  Not every tool
		/// uses this property.
		/// </remarks>
		public GoObject CurrentObject
		{
			get
			{
				return myCurrentObject;
			}
			set
			{
				myCurrentObject = value;
			}
		}

		internal bool CurrentObjectWasSelected
		{
			get
			{
				return myCurrentObjectWasSelected;
			}
			set
			{
				myCurrentObjectWasSelected = value;
			}
		}

		/// <summary>
		/// Gets or sets whether to abort the current transaction if this tool is stopped;
		/// if set to a string, the string specifies the name of the transaction that will
		/// be finished when the tool stops.
		/// </summary>
		/// <remarks>
		/// This determines whether <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> calls
		/// <see cref="M:Northwoods.Go.GoView.AbortTransaction" /> or <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />,
		/// depending on whether the value is null or a <c>String</c>.
		/// </remarks>
		public string TransactionResult
		{
			get
			{
				return myTransactionResult;
			}
			set
			{
				myTransactionResult = value;
			}
		}

		/// <summary>
		/// The constructor associates a view with the tool.
		/// </summary>
		/// <param name="view">
		/// This <see cref="T:Northwoods.Go.GoView" /> must not be null.
		/// </param>
		protected GoTool(GoView view)
		{
			if (view == null)
			{
				throw new ArgumentNullException("view");
			}
			myView = view;
		}

		/// <summary>
		/// This predicate should be true if this tool can be activated to be the view's current tool.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// By default, this returns true.
		/// This is normally only called by the <see cref="T:Northwoods.Go.GoToolManager" /> to decide whether this tool should be started as a
		/// mode-less mouse tool.
		/// </remarks>
		public virtual bool CanStart()
		{
			return true;
		}

		/// <summary>
		/// This method is called when this tool becomes the view's current tool.
		/// </summary>
		/// <remarks>
		/// Typically you will want to put initialization code here for each time the tool is started.
		/// By default, this does nothing.
		/// You should not normally be calling this method directly--only the view should.
		/// </remarks>
		public virtual void Start()
		{
		}

		/// <summary>
		/// This method is called when this tool is about to be replaced as the view's current tool.
		/// </summary>
		/// <remarks>
		/// Typically you will want to put termination code here for each time the tool is stopped.
		/// By default, this does nothing.
		/// You should not normally be calling this method directly--only the view should.
		/// If you want to cause this tool to stop, call <see cref="M:Northwoods.Go.GoTool.StopTool" /> instead,
		/// which will eventually call this method.
		/// </remarks>
		public virtual void Stop()
		{
		}

		/// <summary>
		/// The view calls this method upon a mouse down event; all of the event
		/// information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this does nothing.
		/// </remarks>
		public virtual void DoMouseDown()
		{
		}

		/// <summary>
		/// The view calls this method upon a mouse move event; all of the event
		/// information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this does nothing.
		/// </remarks>
		public virtual void DoMouseMove()
		{
		}

		/// <summary>
		/// The view calls this method upon a mouse up event; all of the event
		/// information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this just calls <see cref="M:Northwoods.Go.GoTool.StopTool" />.
		/// </remarks>
		public virtual void DoMouseUp()
		{
			StopTool();
		}

		/// <summary>
		/// The view calls this method as the mouse wheel rotates; all of the event
		/// information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default this does nothing.
		/// </remarks>
		public virtual void DoMouseWheel()
		{
		}

		/// <summary>
		/// The view calls this method after the mouse rests for a while at a point;
		/// all of the event information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this does nothing.
		/// </remarks>
		public virtual void DoMouseHover()
		{
		}

		/// <summary>
		/// The view calls this method when the user cancels the gesture with the mouse;
		/// all of the event information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this just calls <see cref="M:Northwoods.Go.GoTool.StopTool" />.
		/// </remarks>
		public virtual void DoCancelMouse()
		{
			StopTool();
		}

		/// <summary>
		/// The view calls this method when the user presses a key on the keyboard;
		/// all of the event information is provided by the <see cref="P:Northwoods.Go.GoTool.LastInput" /> property.
		/// </summary>
		/// <remarks>
		/// By default, this just calls <see cref="M:Northwoods.Go.GoTool.DoCancelMouse" /> if the user pressed
		/// the <c>Escape</c> key.
		/// </remarks>
		public virtual void DoKeyDown()
		{
			if (LastInput.Key == Keys.Escape)
			{
				DoCancelMouse();
			}
		}

		/// <summary>
		/// Any tool can call this method in order to implement the standard selection behavior
		/// for a user click.
		/// </summary>
		/// <param name="evt">a <see cref="T:Northwoods.Go.GoInputEventArgs" /> describing the input event</param>
		/// <remarks>
		/// This sets the <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> to be the result of a call
		/// to the view's <see cref="M:Northwoods.Go.GoView.PickObject(System.Boolean,System.Boolean,System.Drawing.PointF,System.Boolean)" /> to pick the selectable
		/// document object at the current point.
		/// If an object is found, what happens to the selection depends on any
		/// modifiers to the event:
		/// if <see cref="P:Northwoods.Go.GoInputEventArgs.Control" /> is true,
		/// we toggle the selectedness of the current object;
		/// if <see cref="P:Northwoods.Go.GoInputEventArgs.Shift" /> is true,
		/// we add the current object to the selection;
		/// otherwise we just make the current object the only selection.
		/// If no object is found and neither <see cref="P:Northwoods.Go.GoInputEventArgs.Control" />
		/// nor <see cref="P:Northwoods.Go.GoInputEventArgs.Shift" /> are true, we empty the selection.
		/// </remarks>
		public virtual void DoSelect(GoInputEventArgs evt)
		{
			CurrentObject = View.PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: true);
			CurrentObjectWasSelected = View.Selection.Contains(CurrentObject);
			if (CurrentObject != null)
			{
				if (evt.Control)
				{
					Selection.Toggle(CurrentObject);
				}
				else if (evt.Shift)
				{
					Selection.Add(CurrentObject);
				}
				else
				{
					Selection.Select(CurrentObject);
				}
			}
			else if (!evt.Control && !evt.Shift)
			{
				Selection.Clear();
			}
		}

		/// <summary>
		/// Any tool can call this method in order to implement the standard click behavior.
		/// </summary>
		/// <param name="evt">a <see cref="T:Northwoods.Go.GoInputEventArgs" /> describing the input event</param>
		/// <returns></returns>
		/// <remarks>
		/// By default, this just calls either <see cref="M:Northwoods.Go.GoView.DoDoubleClick(Northwoods.Go.GoInputEventArgs)" />
		/// or <see cref="M:Northwoods.Go.GoView.DoSingleClick(Northwoods.Go.GoInputEventArgs)" />, depending on whether
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DoubleClick" /> is true.
		/// </remarks>
		public virtual bool DoClick(GoInputEventArgs evt)
		{
			if (evt.DoubleClick)
			{
				return View.DoDoubleClick(evt);
			}
			return View.DoSingleClick(evt);
		}

		/// <summary>
		/// This method just causes the view's current tool to be stopped
		/// and to start the view's default tool instead as the current tool.
		/// </summary>
		/// <remarks>
		/// Call this method when this tool is finished its task.
		/// When the view replaces this tool with the default one, it will
		/// call the <see cref="M:Northwoods.Go.GoTool.Stop" /> method on this tool.
		/// </remarks>
		public void StopTool()
		{
			if (View.Tool == this)
			{
				View.Tool = null;
			}
		}

		/// <summary>
		/// Start a transaction on the view.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This is typically called in overrides of <see cref="M:Northwoods.Go.GoTool.Start" />.
		/// This method also sets the <see cref="P:Northwoods.Go.GoTool.TransactionResult" /> to null,
		/// so that a call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will abort the
		/// transaction rather than finishing it normally, unless
		/// <see cref="P:Northwoods.Go.GoTool.TransactionResult" /> has been set to something.
		/// Not all tools involve changes to the view's document, and thus not
		/// all tools need to start and stop transactions.
		/// </remarks>
		public bool StartTransaction()
		{
			TransactionResult = null;
			return View.StartTransaction();
		}

		/// <summary>
		/// Stop the current transaction, aborting it if <see cref="P:Northwoods.Go.GoTool.TransactionResult" /> is null.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This is typically called in overrides of <see cref="M:Northwoods.Go.GoTool.Stop" />.
		/// If the <see cref="P:Northwoods.Go.GoTool.TransactionResult" /> is null, this calls <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// otherwise this calls <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />.
		/// </remarks>
		public bool StopTransaction()
		{
			if (TransactionResult == null)
			{
				return View.AbortTransaction();
			}
			return View.FinishTransaction(TransactionResult);
		}

		/// <summary>
		/// Determine if the mouse has gone beyond the <see cref="P:Northwoods.Go.GoTool.DragSize" /> distance to
		/// be considered a drag instead of a sloppy click.
		/// </summary>
		/// <returns>
		/// Returns true if the <see cref="P:Northwoods.Go.GoInputEventArgs.ViewPoint" /> of <see cref="P:Northwoods.Go.GoTool.LastInput" />
		/// is further away from <see cref="P:Northwoods.Go.GoTool.FirstInput" /> than half the distance specified by
		/// <see cref="P:Northwoods.Go.GoTool.DragSize" />.
		/// </returns>
		public virtual bool IsBeyondDragSize()
		{
			Point viewPoint = View.FirstInput.ViewPoint;
			Point viewPoint2 = View.LastInput.ViewPoint;
			if (Math.Abs(checked(viewPoint2.X - viewPoint.X)) <= DragSize.Width / 2)
			{
				return Math.Abs(checked(viewPoint2.Y - viewPoint.Y)) > DragSize.Height / 2;
			}
			return true;
		}

		/// <summary>
		/// This shared method helps do subtraction of <c>PointF</c> values.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static SizeF SubtractPoints(PointF a, PointF b)
		{
			return new SizeF(a.X - b.X, a.Y - b.Y);
		}

		/// <summary>
		/// This shared method helps do subtraction of <c>PointF</c> and <c>SizeF</c> values.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static SizeF SubtractPoints(PointF a, SizeF b)
		{
			return new SizeF(a.X - b.Width, a.Y - b.Height);
		}

		/// <summary>
		/// This shared method helps do subtraction of <c>PointF</c> and <c>SizeF</c> values.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static SizeF SubtractPoints(SizeF a, PointF b)
		{
			return new SizeF(a.Width - b.X, a.Height - b.Y);
		}
	}
}
