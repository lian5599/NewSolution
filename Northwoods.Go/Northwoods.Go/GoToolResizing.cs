using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to handle the user's resize operation.
	/// </summary>
	/// <remarks>
	/// This modeless tool is started when the user starts to drag
	/// a resize handle.
	/// An instance of this tool is the default
	/// <see cref="T:Northwoods.Go.GoView" />'s <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
	/// </remarks>
	[Serializable]
	public class GoToolResizing : GoTool
	{
		private SizeF myMinimumSize = new SizeF(1f, 1f);

		private SizeF myMaximumSize = new SizeF(1E+21f, 1E+21f);

		private bool myHidesSelectionHandles = true;

		[NonSerialized]
		private IGoHandle myResizeHandle;

		[NonSerialized]
		private RectangleF myOriginalBounds;

		[NonSerialized]
		private PointF myOriginalPoint;

		[NonSerialized]
		private bool mySelectionHidden;

		[NonSerialized]
		private GoObject mySelectedObject;

		[NonSerialized]
		private GoObject myRealObject;

		/// <summary>
		/// Gets or sets the handle with which the user is performing a resize.
		/// </summary>
		/// <seealso cref="T:Northwoods.Go.GoHandle" />
		public IGoHandle ResizeHandle
		{
			get
			{
				return myResizeHandle;
			}
			set
			{
				myResizeHandle = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial bounding rectangle of the resize handle's handled object.
		/// </summary>
		/// <value>
		/// This <c>RectangleF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoToolResizing.DoCancelMouse" /> uses this information to invoke
		/// <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" /> with the original bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolResizing.ResizeHandle" />
		/// <seealso cref="P:Northwoods.Go.GoToolResizing.OriginalPoint" />
		/// <seealso cref="P:Northwoods.Go.GoHandle.HandledObject" />
		public RectangleF OriginalBounds
		{
			get
			{
				return myOriginalBounds;
			}
			set
			{
				myOriginalBounds = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial center of the resize handle.
		/// </summary>
		/// <value>
		/// This <c>PointF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoToolResizing.DoCancelMouse" /> uses this information to invoke
		/// <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" /> with the original point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolResizing.ResizeHandle" />
		/// <seealso cref="P:Northwoods.Go.GoToolResizing.OriginalBounds" />
		/// <seealso cref="T:Northwoods.Go.GoHandle" />
		public PointF OriginalPoint
		{
			get
			{
				return myOriginalPoint;
			}
			set
			{
				myOriginalPoint = value;
			}
		}

		/// <summary>
		/// Gets or sets the minimum size that the user may resize to.
		/// </summary>
		/// <value>
		/// This is initially a 1x1 <c>SizeF</c> value, thus preventing the user from
		/// making zero either the width or the height.
		/// </value>
		/// <remarks>
		/// This minimum size is passed to <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />.
		/// </remarks>
		public virtual SizeF MinimumSize
		{
			get
			{
				return myMinimumSize;
			}
			set
			{
				myMinimumSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum size that the user may resize to.
		/// </summary>
		/// <value>
		/// This is initially a huge value, which should place no constraint on the user.
		/// </value>
		/// <remarks>
		/// This maximum size is passed to <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />.
		/// </remarks>
		public virtual SizeF MaximumSize
		{
			get
			{
				return myMaximumSize;
			}
			set
			{
				myMaximumSize = value;
			}
		}

		/// <summary>
		/// Gets or sets whether selection handles should be removed as the resize starts.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		public virtual bool HidesSelectionHandles
		{
			get
			{
				return myHidesSelectionHandles;
			}
			set
			{
				myHidesSelectionHandles = value;
			}
		}

		/// <summary>
		/// Gets or sets the objects that ought to be resized, but isn't the
		/// <c>CurrentObject</c>, because <c>ResizesRealtime</c> is false.
		/// </summary>
		/// <remarks>
		/// When this has a value, the <c>CurrentObject</c> should be a copy
		/// of the real object, and this copy should be part of the view
		/// rather than part of the document.
		/// </remarks>
		internal GoObject RealObject
		{
			get
			{
				return myRealObject;
			}
			set
			{
				myRealObject = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolResizing(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// We can start the resize tool when there is a resize handle
		/// under the mouse and the user may resize it.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// The view must permit user resizing.
		/// <see cref="M:Northwoods.Go.GoToolResizing.PickResizeHandle(System.Drawing.PointF)" /> is called to determine if there
		/// is a handle at the first input event point.  If there is such
		/// a handle, and if its <see cref="P:Northwoods.Go.IGoHandle.HandledObject" />'s
		/// <see cref="M:Northwoods.Go.GoObject.CanResize" /> predicate returns true, then
		/// we can start resizing.
		/// The user cannot resize using the context menu mouse button.
		/// </remarks>
		public override bool CanStart()
		{
			if (base.FirstInput.IsContextButton)
			{
				return false;
			}
			if (!base.View.CanResizeObjects())
			{
				return false;
			}
			IGoHandle goHandle = PickResizeHandle(base.FirstInput.DocPoint);
			if (goHandle != null && goHandle.HandledObject != null)
			{
				if (!goHandle.HandledObject.CanResize())
				{
					return goHandle.HandledObject.CanReshape();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Start up the resize tool, assuming <see cref="M:Northwoods.Go.GoToolResizing.CanStart" /> returned true.
		/// </summary>
		/// <remarks>
		/// This sets the <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> to be the
		/// <see cref="P:Northwoods.Go.IGoHandle.HandledObject" /> of the handle returned by
		/// <see cref="M:Northwoods.Go.GoToolResizing.PickResizeHandle(System.Drawing.PointF)" />.
		/// It starts a transaction, hides any selection handles for the current
		/// object, and remembers the object's <see cref="P:Northwoods.Go.GoToolResizing.OriginalBounds" /> and
		/// the handle's <see cref="P:Northwoods.Go.GoToolResizing.OriginalPoint" />.
		/// Finally it calls <see cref="M:Northwoods.Go.GoToolResizing.DoResizing(Northwoods.Go.GoInputState)" /> with an event type of
		/// <c>GoInputState.Start</c>.
		/// </remarks>
		public override void Start()
		{
			IGoHandle goHandle = PickResizeHandle(base.FirstInput.DocPoint);
			if (goHandle == null)
			{
				return;
			}
			GoObject handledObject = goHandle.HandledObject;
			if (handledObject == null)
			{
				return;
			}
			base.CurrentObject = handledObject;
			StartTransaction();
			if (base.Selection.GetHandleCount(handledObject) > 0)
			{
				mySelectedObject = goHandle.SelectedObject;
				if (HidesSelectionHandles)
				{
					mySelectionHidden = true;
					handledObject.RemoveSelectionHandles(base.Selection);
				}
			}
			ResizeHandle = goHandle;
			OriginalBounds = handledObject.Bounds;
			OriginalPoint = goHandle.GoObject.GetSpotLocation(1);
			base.LastInput.InputState = GoInputState.Start;
			DoResizing(GoInputState.Start);
		}

		/// <summary>
		/// Clean up this resizing tool's state.
		/// </summary>
		/// <remarks>
		/// This removes any visible resize box, restores the selection handles if needed,
		/// and stops the transaction (either aborting or finishing it).
		/// </remarks>
		public override void Stop()
		{
			base.View.DrawXorBox(default(Rectangle), drawnew: false);
			if (mySelectionHidden)
			{
				mySelectionHidden = false;
				GoObject currentObject = base.CurrentObject;
				if (currentObject != null && currentObject.Document == base.View.Document)
				{
					if (!base.Selection.Contains(mySelectedObject))
					{
						base.Selection.Add(mySelectedObject);
					}
					else
					{
						currentObject.AddSelectionHandles(base.Selection, mySelectedObject);
					}
				}
			}
			mySelectedObject = null;
			base.CurrentObject = null;
			ResizeHandle = null;
			StopTransaction();
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoToolResizing.DoResizing(Northwoods.Go.GoInputState)" />.
		/// </summary>
		public override void DoMouseMove()
		{
			base.LastInput.InputState = GoInputState.Continue;
			DoResizing(GoInputState.Continue);
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoToolResizing.DoResizing(Northwoods.Go.GoInputState)" /> for the last time and finish the resize transaction.
		/// </summary>
		public override void DoMouseUp()
		{
			base.LastInput.InputState = GoInputState.Finish;
			DoResizing(GoInputState.Finish);
			base.TransactionResult = "Resize";
			base.View.RaiseObjectResized(base.CurrentObject);
			StopTool();
		}

		/// <summary>
		/// Cancelling a resize operation causes the current object's <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />
		/// method to be called with the <see cref="P:Northwoods.Go.GoToolResizing.OriginalBounds" /> and <see cref="P:Northwoods.Go.GoToolResizing.OriginalPoint" />.
		/// </summary>
		public override void DoCancelMouse()
		{
			if (base.CurrentObject != null)
			{
				base.LastInput.InputState = GoInputState.Cancel;
				base.CurrentObject.DoResize(base.View, OriginalBounds, OriginalPoint, ResizeHandle.HandleID, GoInputState.Cancel, MinimumSize, MaximumSize);
			}
			base.TransactionResult = null;
			StopTool();
		}

		/// <summary>
		/// Find a resize handle in the view at the given point.
		/// </summary>
		/// <param name="dc">a <c>PointF</c> in document coordinates</param>
		/// <returns>an <see cref="T:Northwoods.Go.IGoHandle" />, usually a <see cref="T:Northwoods.Go.GoHandle" /></returns>
		public virtual IGoHandle PickResizeHandle(PointF dc)
		{
			return base.View.PickObject(doc: false, view: true, dc, selectableOnly: true) as IGoHandle;
		}

		/// <summary>
		/// This is called while the user is dragging the mouse and when the user releases the mouse.
		/// </summary>
		/// <param name="evttype">A value of <c>GoInputState.Start</c> when called from <see cref="M:Northwoods.Go.GoToolResizing.Start" />,
		/// a value of <c>GoInputState.Continue</c> when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseMove" />, and
		/// a value of <c>GoInputState.Finish</c> when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseUp" />.</param>
		/// <remarks>
		/// Basically this just calls <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" /> on the
		/// <see cref="P:Northwoods.Go.GoTool.CurrentObject" />.  Objects are responsible for their
		/// own resize behavior.
		/// However, this calls <see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /> to adjust the
		/// input event point to make sure the current resize point is a valid one
		/// according to whatever grids there are.
		/// </remarks>
		public virtual void DoResizing(GoInputState evttype)
		{
			if (base.CurrentObject != null)
			{
				GoInputEventArgs lastInput = base.LastInput;
				lastInput.DocPoint = base.View.SnapPoint(lastInput.DocPoint, base.CurrentObject);
				lastInput.ViewPoint = base.View.ConvertDocToView(lastInput.DocPoint);
				GoObject currentObject = base.CurrentObject;
				RectangleF bounds = currentObject.Bounds;
				currentObject.DoResize(base.View, OriginalBounds, lastInput.DocPoint, ResizeHandle.HandleID, evttype, MinimumSize, MaximumSize);
				if (!mySelectionHidden && ((bounds == currentObject.Bounds && currentObject.Document == base.View.Document) || currentObject.View == base.View))
				{
					currentObject.AddSelectionHandles(base.Selection, mySelectedObject);
				}
			}
		}
	}
}
