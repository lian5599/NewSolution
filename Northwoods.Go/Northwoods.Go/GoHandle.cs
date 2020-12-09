using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// This class is used to show that a document object is selected by
	/// decorating the object with handles that are added to a view layer.
	/// </summary>
	/// <remarks>
	/// Instances of this class are assumed only to be used in <see cref="T:Northwoods.Go.GoView" />
	/// layers, not as part of a <see cref="T:Northwoods.Go.GoDocument" />.  Thus they do not
	/// participate in undo/redo nor can they be selected.
	/// </remarks>
	[Serializable]
	public class GoHandle : GoShape, IGoHandle
	{
		private int myHandleID;

		private GoObject mySelectedObject;

		private GoHandleStyle myStyle = GoHandleStyle.Rectangle;

		private string myCursorName;

		/// <summary>
		/// Gets the object being used to implement the handle's visual representation.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.GoObject" />
		[Description("Just returns the GoHandle itself.")]
		public GoObject GoObject => this;

		/// <summary>
		/// Gets or sets an identifier for this handle.
		/// </summary>
		/// <value>
		/// The default identifier is <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c>.
		/// </value>
		/// <remarks>
		/// Because handles are only used in views, setting this
		/// property is not tracked by the undo/redo mechanism.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.HandleID" />
		[Description("An identifier for this handle, often a GoObject spot value.")]
		public int HandleID
		{
			get
			{
				return myHandleID;
			}
			set
			{
				myHandleID = value;
			}
		}

		/// <summary>
		/// Gets or sets the selected object for this handle.
		/// </summary>
		/// <remarks>
		/// Because handles are only used in views, setting this
		/// property is not tracked by the undo/redo mechanism.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.SelectedObject" />
		/// <seealso cref="T:Northwoods.Go.GoSelection" />
		[Description("The selected object that this handle is marking.")]
		public GoObject SelectedObject
		{
			get
			{
				return mySelectedObject;
			}
			set
			{
				mySelectedObject = value;
			}
		}

		/// <summary>
		/// Gets the object that actually gets the handles.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoHandle.HandledObject" />
		/// <seealso cref="M:Northwoods.Go.GoHandle.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		/// <remarks>
		/// This is the <see cref="P:Northwoods.Go.GoHandle.SelectedObject" />'s <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>SelectionObject</c>.
		/// </remarks>
		[Description("The object that actually gets the handles.")]
		public GoObject HandledObject
		{
			get
			{
				if (SelectedObject != null)
				{
					return SelectedObject.SelectionObject;
				}
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the appearance style for this handle.
		/// </summary>
		/// <value>
		/// The default style is <see cref="F:Northwoods.Go.GoHandleStyle.Rectangle" />
		/// </value>
		/// <remarks>
		/// Normally, bounding handles are open rectangles that go
		/// around the handled object, and resize handles are small
		/// rectangles that are filled with a selection color.
		/// Because handles are only used in views, setting this
		/// property is not tracked by the undo/redo mechanism.
		/// </remarks>
		[Description("The appearance style.")]
		public GoHandleStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				myStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the cursor to be used when the mouse
		/// is over this handle object.
		/// </summary>
		/// <value>
		/// The default value is null.
		/// </value>
		/// <remarks>
		/// <para>
		/// <see cref="M:Northwoods.Go.GoHandle.GetCursorName(Northwoods.Go.GoView)" /> uses this property, taking
		/// precedence over the standard cursor name for the <see cref="P:Northwoods.Go.GoHandle.HandleID" />,
		/// as returned by <see cref="M:Northwoods.Go.GoHandle.GetCursorNameForHandleID(System.Int32)" />.
		/// </para>
		/// <para>
		/// Because handles are only used in views, setting this
		/// property is not tracked by the undo/redo mechanism.
		/// </para>
		/// </remarks>
		public string CursorName
		{
			get
			{
				return myCursorName;
			}
			set
			{
				myCursorName = value;
			}
		}

		/// <summary>
		/// Handles should never be selected, so they have no SelectionObject.
		/// </summary>
		public override GoObject SelectionObject => null;

		/// <summary>
		/// The constructor produces a rectangular handle whose <see cref="P:Northwoods.Go.GoHandle.HandleID" /> is
		/// <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c> and that has zero width and height.
		/// </summary>
		public GoHandle()
		{
			base.Size = new SizeF(0f, 0f);
		}

		/// <summary>
		/// We never expect to copy handles.
		/// </summary>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			return null;
		}

		internal static string GetCursorNameForHandleID(int id)
		{
			switch (id)
			{
			case 0:
				return null;
			case 2:
				return "nw-resize";
			case 32:
				return "n-resize";
			case 4:
				return "ne-resize";
			case 256:
				return "w-resize";
			case 64:
				return "e-resize";
			case 16:
				return "sw-resize";
			case 128:
				return "s-resize";
			case 8:
				return "se-resize";
			case 1:
				return "move";
			case 1024:
				return "hand";
			case 1025:
				return "hand";
			default:
				return "move";
			}
		}

		/// <summary>
		/// Return the appropriate resize cursor name for the handle ID's corresponding to
		/// the standard spots, but return null if the object cannot be resized.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// null if <see cref="M:Northwoods.Go.GoView.CanResizeObjects" /> is false or if
		/// <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>CanResize</c> and <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>CanReshape</c> are both false;
		/// otherwise the value of <see cref="P:Northwoods.Go.GoHandle.CursorName" /> if it is non-null;
		/// finally a cursor name depending on the <see cref="P:Northwoods.Go.GoHandle.HandleID" />,
		/// such as "se-resize" for <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>BottomRight</c> or
		/// "hand" for <see cref="F:Northwoods.Go.GoLink.RelinkableToHandle" />, or "move"
		/// for most other handles, or finally null if the <see cref="P:Northwoods.Go.GoHandle.HandleID" />
		/// is <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c>.
		/// </returns>
		public override string GetCursorName(GoView view)
		{
			GoObject handledObject = HandledObject;
			if (handledObject == null || (view != null && !view.CanResizeObjects()) || (!handledObject.CanResize() && !handledObject.CanReshape()))
			{
				return null;
			}
			string text = CursorName;
			if (text == null)
			{
				text = GetCursorNameForHandleID(HandleID);
			}
			return text;
		}

		/// <summary>
		/// Draw this handle according to the handle's style.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// Handles never have shadows, but their outlines and fills
		/// can be specified by the <see cref="P:Northwoods.Go.GoShape.Pen" /> and
		/// <see cref="P:Northwoods.Go.GoShape.Brush" /> properties.
		/// Hollow handles (those with a <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c>
		/// <see cref="P:Northwoods.Go.GoHandle.HandleID" />), should have no <see cref="P:Northwoods.Go.GoShape.Brush" />.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			RectangleF bounds = Bounds;
			switch (Style)
			{
			case GoHandleStyle.None:
				break;
			default:
				GoShape.DrawRectangle(g, view, Pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			case GoHandleStyle.Ellipse:
				GoShape.DrawEllipse(g, view, Pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			case GoHandleStyle.Diamond:
			{
				PointF[] array2 = view.AllocTempPointArray(4);
				array2[0].X = bounds.X + bounds.Width / 2f;
				array2[0].Y = bounds.Y;
				array2[1].X = bounds.X + bounds.Width;
				array2[1].Y = bounds.Y + bounds.Height / 2f;
				array2[2].X = array2[0].X;
				array2[2].Y = bounds.Y + bounds.Height;
				array2[3].X = bounds.X;
				array2[3].Y = array2[1].Y;
				GoShape.DrawPolygon(g, view, Pen, Brush, array2);
				view.FreeTempPointArray(array2);
				break;
			}
			case GoHandleStyle.TriangleTopLeft:
			case GoHandleStyle.TriangleTopRight:
			case GoHandleStyle.TriangleBottomRight:
			case GoHandleStyle.TriangleBottomLeft:
			case GoHandleStyle.TriangleMiddleTop:
			case GoHandleStyle.TriangleMiddleRight:
			case GoHandleStyle.TriangleMiddleBottom:
			case GoHandleStyle.TriangleMiddleLeft:
			{
				PointF[] array = view.AllocTempPointArray(3);
				ComputeTrianglePoints(array);
				GoShape.DrawPolygon(g, view, Pen, Brush, array);
				view.FreeTempPointArray(array);
				break;
			}
			}
		}

		private void ComputeTrianglePoints(PointF[] v)
		{
			RectangleF bounds = Bounds;
			switch (Style)
			{
			case GoHandleStyle.TriangleBottomRight:
				v[0].X = bounds.X + bounds.Width / 2f;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y + bounds.Height / 2f;
				break;
			case GoHandleStyle.TriangleMiddleBottom:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width / 2f;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y;
				break;
			case GoHandleStyle.TriangleBottomLeft:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y + bounds.Height / 2f;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y + bounds.Height;
				v[2].X = bounds.X + bounds.Width / 2f;
				v[2].Y = bounds.Y;
				break;
			case GoHandleStyle.TriangleMiddleLeft:
				v[0].X = bounds.X + bounds.Width;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y + bounds.Height / 2f;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y;
				break;
			case GoHandleStyle.TriangleTopLeft:
				v[0].X = bounds.X + bounds.Width / 2f;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y + bounds.Height / 2f;
				break;
			case GoHandleStyle.TriangleMiddleTop:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y + bounds.Height;
				v[1].X = bounds.X + bounds.Width / 2f;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			case GoHandleStyle.TriangleTopRight:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y + bounds.Height / 2f;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y;
				v[2].X = bounds.X + bounds.Width / 2f;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			case GoHandleStyle.TriangleMiddleRight:
				v[0].X = bounds.X;
				v[0].Y = bounds.Y;
				v[1].X = bounds.X + bounds.Width;
				v[1].Y = bounds.Y + bounds.Height / 2f;
				v[2].X = bounds.X;
				v[2].Y = bounds.Y + bounds.Height;
				break;
			}
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> representing this shape.
		/// </summary>
		/// <returns>a newly allocated <c>GraphicsPath</c></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			RectangleF bounds = Bounds;
			switch (Style)
			{
			default:
				graphicsPath.AddRectangle(bounds);
				break;
			case GoHandleStyle.Ellipse:
				graphicsPath.AddEllipse(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				break;
			case GoHandleStyle.Diamond:
			{
				PointF[] array2 = new PointF[4];
				array2[0].X = bounds.X + bounds.Width / 2f;
				array2[0].Y = bounds.Y;
				array2[1].X = bounds.X + bounds.Width;
				array2[1].Y = bounds.Y + bounds.Height / 2f;
				array2[2].X = array2[0].X;
				array2[2].Y = bounds.Y + bounds.Height;
				array2[3].X = bounds.X;
				array2[3].Y = array2[1].Y;
				graphicsPath.AddPolygon(array2);
				break;
			}
			case GoHandleStyle.TriangleTopLeft:
			case GoHandleStyle.TriangleTopRight:
			case GoHandleStyle.TriangleBottomRight:
			case GoHandleStyle.TriangleBottomLeft:
			case GoHandleStyle.TriangleMiddleTop:
			case GoHandleStyle.TriangleMiddleRight:
			case GoHandleStyle.TriangleMiddleBottom:
			case GoHandleStyle.TriangleMiddleLeft:
			{
				PointF[] array = new PointF[3];
				ComputeTrianglePoints(array);
				graphicsPath.AddPolygon(array);
				break;
			}
			case GoHandleStyle.None:
				graphicsPath.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);
				break;
			}
			return graphicsPath;
		}

		/// <summary>
		/// Determines if the given point is inside this handle's bounds.
		/// </summary>
		/// <param name="p">
		/// A <c>PointF</c> value in document coordinates.
		/// </param>
		/// <returns>
		/// True if the point is considered "on" the handle.
		/// </returns>
		/// <remarks>
		/// This assumes that handles are actually hollow when the
		/// <see cref="P:Northwoods.Go.GoHandle.HandleID" /> is <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c>--
		/// that is, a point well inside the handle's bounds is
		/// not considered "on" the handle if the handle's ID is
		/// <see cref="P:Northwoods.Go.GoHandle.GoObject" />.<c>NoHandle</c>.  This is useful for
		/// letting bounding handles be ignored by mouse over behavior.
		/// Note that this method does not take the <see cref="P:Northwoods.Go.GoHandle.Style" />
		/// into account--it assumes the handle is rectangular.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			RectangleF a = Bounds;
			float penWidth = PenWidth;
			GoObject.InflateRect(ref a, penWidth / 2f, penWidth / 2f);
			if (!GoObject.ContainsRect(a, p))
			{
				return false;
			}
			if (HandleID == 0)
			{
				GoObject.InflateRect(ref a, 0f - penWidth, 0f - penWidth);
				return !GoObject.ContainsRect(a, p);
			}
			return true;
		}

		/// <summary>
		/// Handles should never be selected, so they should not get their own selection handles.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
		}
	}
}
