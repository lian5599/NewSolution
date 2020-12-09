using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This tool allows the user to add a copy of an object to the document by
	/// dragging in the background where and how large it should be.
	/// </summary>
	/// <example>
	/// For modeless use, you would typically create this tool when
	/// initializing a GoView:
	/// <code>
	///   GoToolCreating ctool = new GoToolCreating(goView1);
	///   GoPolygon tri = new GoPolygon();
	///   tri.BrushColor = Color.LightBlue;
	///   tri.AddPoint(5, 0);
	///   tri.AddPoint(0, 10);
	///   tri.AddPoint(10, 10);
	///   ctool.Prototype = tri;
	///   goView1.ReplaceMouseTool(typeof(GoToolRubberBanding), ctool);
	/// </code>
	/// </example>
	/// <example>
	/// <code>
	///   // For modal use, you might create this tool in a command invoked from a menu or toolbar button or keyboard event handler:
	///   GoToolCreating ctool = new GoToolCreating(goView1);
	///   GoPolygon tri = new GoPolygon();
	///   tri.BrushColor = Color.LightBlue;
	///   tri.AddPoint(5, 0);
	///   tri.AddPoint(0, 10);
	///   tri.AddPoint(10, 10);
	///   ctool.Prototype = tri;
	///   ctool.Modal = true;
	///   ctool.OneShot = false;
	///   goView1.Tool = ctool;
	/// </code>
	/// </example>
	[Serializable]
	public class GoToolCreating : GoTool
	{
		private bool myModal;

		private bool myOneShot = true;

		private bool myAutoScrolling = true;

		private GoObject myPrototype;

		private SizeF myMinimumSize = new SizeF(10f, 10f);

		private SizeF myMaximumSize = new SizeF(999999f, 999999f);

		private bool myResizesSelectionObject = true;

		private bool mySnapsToGrid;

		[NonSerialized]
		private GoObject myNewObject;

		[NonSerialized]
		private bool myActive;

		/// <summary>
		/// Gets or sets the GoObject that should be copied, sized, and positioned
		/// by this tool.
		/// </summary>
		/// <value>
		/// By default this is just a GoRectangle, but it is normally set to
		/// the kind of object, initialized the way you want, that you want the
		/// user to draw.  The object should be Resizable and ResizesRealtime,
		/// and probably should be Reshapable.
		/// </value>
		/// <remarks>
		/// You might override this property to get the prototype object from
		/// a collection of objects, chosen by the some context that your
		/// application determines.  For example, this might always return
		/// </remarks>
		public virtual GoObject Prototype
		{
			get
			{
				return myPrototype;
			}
			set
			{
				myPrototype = value;
			}
		}

		/// <summary>
		/// Whether this tool can be used to implement an object-creating mode
		/// for a GoView, or whether this tool can be get started when appropriate
		/// as one of the GoView.MouseMoveTools.
		/// </summary>
		/// <value>
		/// This defaults to false, to be used as a MouseMoveTool.
		/// </value>
		/// <remarks>
		/// Depending on whether you want the user to remain in creation mode
		/// after a mouse up, you might want to set <see cref="P:Northwoods.Go.GoToolCreating.OneShot" />
		/// </remarks>
		public virtual bool Modal
		{
			get
			{
				return myModal;
			}
			set
			{
				myModal = value;
			}
		}

		/// <summary>
		/// Whether this tool automatically stops itself on a mouse up event.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <remarks>
		/// You might want to set this to false if <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is true.
		/// When <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is true and <see cref="P:Northwoods.Go.GoToolCreating.OneShot" /> is false,
		/// each call to <see cref="M:Northwoods.Go.GoToolCreating.DoCreate" /> occurs in its own transaction
		/// and this tool handles Ctrl-Z and Ctrl-Y to Undo and Redo each creation.
		/// </remarks>
		public virtual bool OneShot
		{
			get
			{
				return myOneShot;
			}
			set
			{
				myOneShot = value;
			}
		}

		/// <summary>
		/// Gets or sets whether dragging the mouse near the edges of the view
		/// should cause the view to scroll automatically.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		public virtual bool AutoScrolling
		{
			get
			{
				return myAutoScrolling;
			}
			set
			{
				myAutoScrolling = value;
			}
		}

		/// <summary>
		/// Gets or sets the minimum size for the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />,
		/// as computed by <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" />.
		/// </summary>
		/// <value>
		/// The default minimum size, in document coordinates, is 10x10.
		/// Any new value must have positive Width and Height.
		/// </value>
		public virtual SizeF MinimumSize
		{
			get
			{
				return myMinimumSize;
			}
			set
			{
				if (value.Width > 0f && value.Height > 0f)
				{
					myMinimumSize = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum size for the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />,
		/// as computed by <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" />.
		/// </summary>
		/// <value>
		/// The default maximum size, in document coordinates, is really large.
		/// Any new value must have positive Width and Height.
		/// </value>
		public virtual SizeF MaximumSize
		{
			get
			{
				return myMaximumSize;
			}
			set
			{
				if (value.Width > 0f && value.Height > 0f)
				{
					myMaximumSize = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> or the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />'s
		/// <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> has its <see cref="P:Northwoods.Go.GoObject.Bounds" /> set
		/// to the value returned by <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" />.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		public virtual bool ResizesSelectionObject
		{
			get
			{
				return myResizesSelectionObject;
			}
			set
			{
				myResizesSelectionObject = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the result of <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" /> heeds the view's
		/// grid by calling <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// When this is true, in order for <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" /> to snap
		/// the <c>GoView.FirstInput.DocPoint</c> and <c>GoView.LastInput.DocPoint</c>,
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.GridSnapDrag" /> should
		/// be a value other than <c>GoViewSnapStyle.None</c>, or the view's
		/// <see cref="P:Northwoods.Go.GoView.Grid" />'s <see cref="M:Northwoods.Go.GoGrid.CanSnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /> predicate
		/// should return true.
		/// </remarks>
		public virtual bool SnapsToGrid
		{
			get
			{
				return mySnapsToGrid;
			}
			set
			{
				mySnapsToGrid = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoObject" /> that is a copy of the <see cref="P:Northwoods.Go.GoToolCreating.Prototype" />
		/// that will be added to this view's document in <see cref="M:Northwoods.Go.GoToolCreating.DoCreate" />.
		/// </summary>
		/// <remarks>
		/// This property is set by <see cref="M:Northwoods.Go.GoToolCreating.DoMouseMove" /> when this tool is <see cref="P:Northwoods.Go.GoToolCreating.Active" />.
		/// The value is the result of a call to <see cref="M:Northwoods.Go.GoToolCreating.CopyPrototype" />.
		/// While the user is dragging the mouse around, this new object will
		/// be part of the view.  The <see cref="M:Northwoods.Go.GoToolCreating.DoCreate" /> method will remove this new object
		/// from the view and add it to the document.
		/// </remarks>
		public GoObject NewObject
		{
			get
			{
				return myNewObject;
			}
			set
			{
				myNewObject = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this tool is in the process of drawing a new object.
		/// </summary>
		/// <value>
		/// The value is set to true in <see cref="M:Northwoods.Go.GoToolCreating.DoMouseDown" /> if <see cref="M:Northwoods.Go.GoToolCreating.CanStart" /> is true,
		/// and in <see cref="M:Northwoods.Go.GoToolCreating.DoMouseMove" /> if <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is false.
		/// </value>
		public bool Active
		{
			get
			{
				return myActive;
			}
			set
			{
				myActive = value;
			}
		}

		/// <summary>
		/// This constructs a modeless creation tool that has no <see cref="P:Northwoods.Go.GoToolCreating.Prototype" /> object.
		/// </summary>
		/// <param name="view"></param>
		public GoToolCreating(GoView view)
			: base(view)
		{
		}

		/// <summary>
		/// This tool, when used modelessly, does not start when the user is using
		/// the context button or when the mouse is over an object in the document.
		/// </summary>
		/// <returns></returns>
		public override bool CanStart()
		{
			if (!base.View.CanSelectObjects())
			{
				return false;
			}
			if (base.LastInput.IsContextButton)
			{
				return false;
			}
			if (!IsBeyondDragSize())
			{
				return false;
			}
			return base.View.PickObject(doc: true, view: false, base.FirstInput.DocPoint, selectableOnly: true) == null;
		}

		/// <summary>
		/// Starting this tool starts a transaction covering all of the
		/// changes made until this tool is stopped.
		/// </summary>
		/// <remarks>
		/// On WinForms it also changes the cursor to a Cross.
		/// This calls <see cref="M:Northwoods.Go.GoTool.StartTransaction" /> if
		/// <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is false or if <see cref="P:Northwoods.Go.GoToolCreating.OneShot" /> is true.
		/// </remarks>
		public override void Start()
		{
			if (!Modal || OneShot)
			{
				StartTransaction();
			}
			base.View.CursorName = "crosshair";
		}

		/// <summary>
		/// Stopping this tool will remove the temporary <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> from
		/// the view, if it had not yet been added to the document.
		/// </summary>
		/// <remarks>
		/// This also calls <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> if
		/// <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is false or if <see cref="P:Northwoods.Go.GoToolCreating.OneShot" /> is true.
		/// </remarks>
		public override void Stop()
		{
			if (NewObject != null && NewObject.IsInView)
			{
				NewObject.Remove();
			}
			NewObject = null;
			base.View.StopAutoScroll();
			base.View.CursorName = "default";
			Active = false;
			if (!Modal || OneShot)
			{
				StopTransaction();
			}
		}

		private void Activate()
		{
			Active = true;
			if (!base.FirstInput.Shift && !base.Selection.IsEmpty)
			{
				base.Selection.Clear();
			}
		}

		/// <summary>
		/// A mouse-down activates this tool so that a mouse-move causes
		/// the <see cref="P:Northwoods.Go.GoToolCreating.Prototype" /> object to be copied as the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />.
		/// </summary>
		/// <remarks>
		/// This method is not called when this tool is used modelessly.
		/// But when used modally, we want to wait for a mouse-down before
		/// creating and resizing the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />.
		/// </remarks>
		public override void DoMouseDown()
		{
			Activate();
		}

		/// <summary>
		/// Create and resize the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> that is added to this view.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When this tool is already activated, the first mouse-move will call
		/// <see cref="M:Northwoods.Go.GoToolCreating.CopyPrototype" /> and add the resulting object, as the
		/// <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />, to the view.
		/// Further mouse-moves will continuously resize the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />
		/// (or the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />'s <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// if <see cref="P:Northwoods.Go.GoToolCreating.ResizesSelectionObject" /> is true)
		/// according to the bounds returned by <see cref="M:Northwoods.Go.GoToolCreating.ComputeBox" />.
		/// </para>
		/// <para>
		/// If this tool is started but not yet activated, mouse-moves are ignored
		/// when <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is true until a mouse-down activates this tool.
		/// When <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is false, a mouse-move automatically activates
		/// this tool so that the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> is created and mouse moves
		/// resize it.
		/// </para>
		/// </remarks>
		public override void DoMouseMove()
		{
			if (!Active)
			{
				if (Modal)
				{
					return;
				}
				Activate();
			}
			if (NewObject == null)
			{
				NewObject = CopyPrototype();
				if (NewObject == null)
				{
					return;
				}
				base.View.Layers.Default.Add(NewObject);
			}
			if (ResizesSelectionObject)
			{
				NewObject.SelectionObject.Bounds = ComputeBox();
			}
			else
			{
				NewObject.Bounds = ComputeBox();
			}
			if (AutoScrolling)
			{
				base.View.DoAutoScroll(base.LastInput.ViewPoint);
			}
		}

		/// <summary>
		/// A mouse-up calls <see cref="M:Northwoods.Go.GoToolCreating.DoCreate" /> to add the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />
		/// to the view's document.
		/// </summary>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoToolCreating.OneShot" /> is true, this tool stops itself.
		/// Otherwise the user can continue to create new objects.
		/// </remarks>
		public override void DoMouseUp()
		{
			if (Active)
			{
				DoCreate();
				NewObject = null;
				Active = false;
			}
			if (OneShot)
			{
				StopTool();
			}
		}

		/// <summary>
		/// If <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> and not <see cref="P:Northwoods.Go.GoToolCreating.OneShot" />, the current
		/// <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> is removed but this tool remains active to allow
		/// further object creations; otherwise this tool is stopped.
		/// </summary>
		public override void DoCancelMouse()
		{
			if (Active && Modal && !OneShot)
			{
				if (NewObject != null && NewObject.IsInView)
				{
					NewObject.Remove();
				}
				NewObject = null;
			}
			else
			{
				StopTool();
			}
		}

		/// <summary>
		/// Compute the Bounds for the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />, given the
		/// initial and current positions of the mouse.
		/// </summary>
		/// <returns>
		/// a RectangleF in document coordinates, within the limits
		/// imposed by <see cref="P:Northwoods.Go.GoToolCreating.MinimumSize" /> and <see cref="P:Northwoods.Go.GoToolCreating.MaximumSize" />
		/// </returns>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoToolCreating.SnapsToGrid" /> is true, the resulting box
		/// will have its top-left and bottom-right points snapped to the
		/// view's grid.  However, the <see cref="P:Northwoods.Go.GoToolCreating.MinimumSize" /> and
		/// <see cref="P:Northwoods.Go.GoToolCreating.MaximumSize" /> limits still apply, so you probably
		/// want to set the <see cref="P:Northwoods.Go.GoToolCreating.MinimumSize" /> to be the same as
		/// the <c>GoView.GridCellSize</c>.
		/// </remarks>
		public virtual RectangleF ComputeBox()
		{
			PointF p = base.FirstInput.DocPoint;
			PointF p2 = base.LastInput.DocPoint;
			if (SnapsToGrid)
			{
				p = base.View.SnapPoint(p, NewObject);
				p2 = base.View.SnapPoint(p2, NewObject);
			}
			RectangleF result = new RectangleF(Math.Min(p2.X, p.X), Math.Min(p2.Y, p.Y), Math.Abs(p2.X - p.X), Math.Abs(p2.Y - p.Y));
			SizeF minimumSize = MinimumSize;
			SizeF maximumSize = MaximumSize;
			if (result.Width < minimumSize.Width)
			{
				result.Width = minimumSize.Width;
			}
			else if (result.Width > maximumSize.Width)
			{
				result.Width = maximumSize.Width;
			}
			if (result.Height < minimumSize.Height)
			{
				result.Height = minimumSize.Height;
			}
			else if (result.Height > maximumSize.Height)
			{
				result.Height = maximumSize.Height;
			}
			return result;
		}

		/// <summary>
		/// Add the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> to this view's document.
		/// </summary>
		/// <remarks>
		/// This removes the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> from the view and adds it to the view's document.
		/// It also selects the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" />.
		/// </remarks>
		public virtual void DoCreate()
		{
			GoObject newObject = NewObject;
			if (newObject != null)
			{
				newObject.Remove();
				if (Modal && !OneShot)
				{
					base.View.StartTransaction();
				}
				if (ResizesSelectionObject)
				{
					newObject.SelectionObject.Bounds = ComputeBox();
				}
				else
				{
					newObject.Bounds = ComputeBox();
				}
				base.View.Document.Add(newObject);
				base.View.Selection.Select(newObject);
				if (Modal && !OneShot)
				{
					base.View.FinishTransaction("Drag Created");
				}
				else
				{
					base.TransactionResult = "Drag Created";
				}
			}
		}

		/// <summary>
		/// This method is called to create the <see cref="P:Northwoods.Go.GoToolCreating.NewObject" /> by
		/// making a copy of the <see cref="P:Northwoods.Go.GoToolCreating.Prototype" />.
		/// </summary>
		/// <returns>a copy of <see cref="P:Northwoods.Go.GoToolCreating.Prototype" />, or null if unsuccessful</returns>
		public virtual GoObject CopyPrototype()
		{
			if (Prototype == null)
			{
				return null;
			}
			return base.View.Document.CreateCopyDictionary().CopyComplete(Prototype);
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.GoToolCreating.Modal" /> is true and <see cref="P:Northwoods.Go.GoToolCreating.OneShot" /> is false,
		/// handle Ctrl-Z and Ctrl-Y for undo and redo.
		/// </summary>
		public override void DoKeyDown()
		{
			if (Modal && !OneShot)
			{
				bool control = base.LastInput.Control;
				Keys key = base.LastInput.Key;
				if (control && key == Keys.Z)
				{
					base.View.Undo();
				}
				else if (control && key == Keys.Y)
				{
					base.View.Redo();
				}
			}
			base.DoKeyDown();
		}
	}
}
