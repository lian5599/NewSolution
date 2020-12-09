using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A 2-D representation of a cylinder, with the radius 
	/// or perspective "pitch" controlled by an additional GoHandle.
	/// </summary>
	[Serializable]
	public class GoCylinder : GoShape
	{
		private const int flagResizableRadius = 1048576;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCylinder.MinorRadius" /> property.
		/// </summary>
		public const int ChangedMinorRadius = 1481;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCylinder.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 1482;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCylinder.Perspective" /> property.
		/// </summary>
		public const int ChangedPerspective = 1483;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoCylinder.ResizableRadius" /> property.
		/// </summary>
		public const int ChangedResizableRadius = 1484;

		/// <summary>
		/// A special handle ID for a handle which controls the radius length.
		/// </summary>
		public const int RadiusHandleID = 1032;

		private PointF[] myPoints = new PointF[4];

		private float myMinorRadius = 10f;

		private Orientation myOrientation = Orientation.Vertical;

		private GoPerspective myPerspective;

		/// <summary>
		/// Gets or sets the length of minor radius of the cylinder's ellipse. 
		/// </summary>
		/// <value>
		/// This defaults to 10.
		/// If given a negative value, it uses zero instead.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The length of cylinder's ellipse's minor radius.")]
		public virtual float MinorRadius
		{
			get
			{
				return myMinorRadius;
			}
			set
			{
				float num = myMinorRadius;
				if (value < 0f)
				{
					value = 0f;
				}
				if (num != value)
				{
					myMinorRadius = value;
					ResetPath();
					Changed(1481, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets how to draw the cylinder, whether the parallel lines
		/// of the cylinder are drawn horizontally or vertically.
		/// </summary>
		/// <value>
		/// This defaults to <c>Orientation.Vertical</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Vertical)]
		[Description("Whether the parallel lines of the cylinder are drawn horizontally or vertically.")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				Orientation orientation = myOrientation;
				if (orientation != value)
				{
					myOrientation = value;
					ResetPath();
					Changed(1482, 0, orientation, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets how to draw the cylinder, whether the full ellipse 
		/// is drawn on the top or bottom for <c>Orientation.Vertical</c> or left or right for 
		/// <c>Orientation.Horizontal</c>,
		/// </summary>
		/// <value>
		/// This defaults to <c>GoPerspective.TopLeft</c>
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoPerspective.TopLeft)]
		[Description("Whether the cylinder's full ellipse is drawn on the top or bottom of the cylinder.")]
		public virtual GoPerspective Perspective
		{
			get
			{
				return myPerspective;
			}
			set
			{
				GoPerspective goPerspective = myPerspective;
				if (goPerspective != value)
				{
					myPerspective = value;
					ResetPath();
					Changed(1483, 0, goPerspective, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the radius control handle.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether to add the radius control handle.")]
		public virtual bool ResizableRadius
		{
			get
			{
				return (base.InternalFlags & 0x100000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x100000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 1048576;
					}
					else
					{
						base.InternalFlags &= -1048577;
					}
					Changed(1484, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a cylinder with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline, no <see cref="T:System.Drawing.Brush" /> fill, and a default radius dimensions.
		/// </summary>
		public GoCylinder()
		{
			base.InternalFlags |= 1049088;
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			RectangleF bounds = Bounds;
			PointF[] points = getPoints();
			float num = MinorRadius;
			if (IsApprox(num, 0f))
			{
				RectangleF bounds2 = Bounds;
				graphicsPath.AddRectangle(bounds2);
				graphicsPath.CloseAllFigures();
			}
			else if (Orientation == Orientation.Vertical)
			{
				if (num > bounds.Height / 2f)
				{
					num = bounds.Height / 2f;
				}
				graphicsPath.AddArc(points[0].X, points[0].Y - num, bounds.Width, num * 2f, 180f, 180f);
				graphicsPath.AddLine(points[3], points[2]);
				graphicsPath.AddArc(points[1].X, points[1].Y - num, bounds.Width, num * 2f, 0f, 180f);
				graphicsPath.AddLine(points[1], points[0]);
				graphicsPath.CloseFigure();
				if (Perspective == GoPerspective.TopLeft || Perspective == GoPerspective.TopRight)
				{
					graphicsPath.AddArc(points[0].X, points[0].Y - num, bounds.Width, num * 2f, 0f, 180f);
				}
				else
				{
					graphicsPath.AddArc(points[1].X, points[1].Y - num, bounds.Width, num * 2f, 180f, 180f);
				}
			}
			else
			{
				if (num > bounds.Width / 2f)
				{
					num = bounds.Width / 2f;
				}
				graphicsPath.AddArc(points[0].X - num, points[0].Y, num * 2f, bounds.Height, 90f, 180f);
				graphicsPath.AddLine(points[0], points[1]);
				graphicsPath.AddArc(points[1].X - num, points[1].Y, num * 2f, bounds.Height, 270f, 180f);
				graphicsPath.AddLine(points[2], points[3]);
				graphicsPath.CloseFigure();
				if (Perspective == GoPerspective.TopLeft || Perspective == GoPerspective.BottomLeft)
				{
					graphicsPath.AddArc(points[0].X - num, points[0].Y, num * 2f, bounds.Height, 270f, 180f);
				}
				else
				{
					graphicsPath.AddArc(points[1].X - num, points[1].Y, num * 2f, bounds.Height, 90f, 180f);
				}
			}
			return graphicsPath;
		}

		private PointF[] getPoints()
		{
			RectangleF bounds = Bounds;
			float num = MinorRadius;
			if (Orientation == Orientation.Vertical)
			{
				if (num > bounds.Height / 2f)
				{
					num = bounds.Height / 2f;
				}
				myPoints[0] = new PointF(bounds.X, bounds.Y + num);
				myPoints[1] = new PointF(bounds.X, bounds.Y + bounds.Height - num);
				myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num);
				myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y + num);
			}
			else
			{
				if (num > bounds.Width / 2f)
				{
					num = bounds.Width / 2f;
				}
				myPoints[0] = new PointF(bounds.X + num, bounds.Y);
				myPoints[1] = new PointF(bounds.X + bounds.Width - num, bounds.Y);
				myPoints[2] = new PointF(bounds.X + bounds.Width - num, bounds.Y + bounds.Height);
				myPoints[3] = new PointF(bounds.X + num, bounds.Y + bounds.Height);
			}
			return myPoints;
		}

		/// <summary>
		/// Consider the actual shape of the cylinder to determine
		/// if a given point is inside.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// Currently this does not take the pen width into account.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			if (!base.ContainsPoint(p))
			{
				return false;
			}
			GraphicsPath path = GetPath();
			bool result = path.IsVisible(p);
			DisposePath(path);
			return result;
		}

		/// <summary>
		/// Support either allowing the user to move the radius control handle.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (whichHandle == 1032 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				RectangleF bounds = Bounds;
				float minorRadius = MinorRadius;
				minorRadius = (MinorRadius = ((myOrientation == Orientation.Vertical) ? ((Perspective != 0 && Perspective != GoPerspective.TopRight) ? ((newPoint.Y > bounds.Y + bounds.Height) ? 0f : ((!(newPoint.Y < bounds.Y)) ? ((bounds.Y + bounds.Height - newPoint.Y) / 2f) : (bounds.Height / 2f))) : ((newPoint.Y > bounds.Y + bounds.Height) ? (bounds.Height / 2f) : ((!(newPoint.Y < bounds.Y)) ? ((newPoint.Y - bounds.Y) / 2f) : 0f))) : ((Perspective == GoPerspective.TopLeft || Perspective == GoPerspective.BottomLeft) ? ((newPoint.X > bounds.X + bounds.Width) ? (bounds.Width / 2f) : ((!(newPoint.X < bounds.X)) ? ((newPoint.X - bounds.X) / 2f) : 0f)) : ((newPoint.X > bounds.X + bounds.Width) ? 0f : ((!(newPoint.X < bounds.X)) ? ((bounds.X + bounds.Width - newPoint.X) / 2f) : (bounds.Width / 2f))))));
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
				ResetPath();
			}
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> and <see cref="P:Northwoods.Go.GoCylinder.ResizableRadius" />
		/// are true, this supports a radius control handle.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape() && ResizableRadius)
			{
				RectangleF bounds = Bounds;
				PointF pointF = default(PointF);
				float minorRadius = MinorRadius;
				pointF = ((Orientation == Orientation.Vertical) ? ((Perspective != 0 && Perspective != GoPerspective.TopRight) ? new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height - 2f * minorRadius) : new PointF(bounds.X + bounds.Width / 2f, bounds.Y + 2f * minorRadius)) : ((Perspective == GoPerspective.TopLeft || Perspective == GoPerspective.BottomLeft) ? new PointF(bounds.X + 2f * minorRadius, bounds.Y + bounds.Height / 2f) : new PointF(bounds.X + bounds.Width - 2f * minorRadius, bounds.Y + bounds.Height / 2f)));
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1032, filled: true);
				MakeDiamondResizeHandle(handle, (Orientation == Orientation.Horizontal) ? 64 : 128);
			}
		}

		/// <summary>
		/// The closest point of a cylinder that intersects with a given line
		/// is the closest such point of each two line segments and two ellipses.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float shift = PenWidth / 2f;
			PointF[] points = getPoints();
			PointF a = GoShape.ExpandPointOnEdge(points[0], bounds, shift);
			PointF b = GoShape.ExpandPointOnEdge(points[1], bounds, shift);
			PointF a2 = GoShape.ExpandPointOnEdge(points[2], bounds, shift);
			PointF b2 = GoShape.ExpandPointOnEdge(points[3], bounds, shift);
			float num = 1E+21f;
			PointF pointF = default(PointF);
			RectangleF rect;
			RectangleF rect2;
			float startAngle;
			float startAngle2;
			if (Orientation == Orientation.Vertical)
			{
				rect = new RectangleF(bounds.X, bounds.Y, bounds.Width, MinorRadius * 2f);
				rect2 = new RectangleF(bounds.X, bounds.Y + bounds.Height - MinorRadius * 2f, bounds.Width, MinorRadius * 2f);
				startAngle = 180f;
				startAngle2 = 0f;
			}
			else
			{
				rect = new RectangleF(bounds.X, bounds.Y, MinorRadius * 2f, bounds.Height);
				rect2 = new RectangleF(bounds.X + bounds.Width - MinorRadius * 2f, bounds.Y, MinorRadius * 2f, bounds.Height);
				startAngle = 90f;
				startAngle2 = 270f;
			}
			if (GoEllipse.NearestIntersectionOnArc(rect, p1, p2, out PointF result2, startAngle, 180f))
			{
				float num2 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
				if (num2 < num)
				{
					num = num2;
					pointF = result2;
				}
			}
			if (Orientation == Orientation.Horizontal)
			{
				if (GoEllipse.NearestIntersectionOnArc(rect2, p1, p2, out result2, 270f, 90f))
				{
					float num3 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
					if (num3 < num)
					{
						num = num3;
						pointF = result2;
					}
				}
				if (GoEllipse.NearestIntersectionOnArc(rect2, p1, p2, out result2, 0f, 90f))
				{
					float num4 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
					if (num4 < num)
					{
						num = num4;
						pointF = result2;
					}
				}
			}
			else if (GoEllipse.NearestIntersectionOnArc(rect2, p1, p2, out result2, startAngle2, 180f))
			{
				float num5 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
				if (num5 < num)
				{
					num = num5;
					pointF = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(a, b, p1, p2, out result2))
			{
				float num6 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
				if (num6 < num)
				{
					num = num6;
					pointF = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(a2, b2, p1, p2, out result2))
			{
				float num7 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
				if (num7 < num)
				{
					num = num7;
					pointF = result2;
				}
			}
			result = pointF;
			return num < 1E+21f;
		}

		/// <summary>
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1481:
				MinorRadius = e.GetFloat(undo);
				break;
			case 1482:
				Orientation = (Orientation)e.GetValue(undo);
				break;
			case 1483:
				Perspective = (GoPerspective)e.GetValue(undo);
				break;
			case 1484:
				ResizableRadius = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				ResetPath();
				break;
			}
		}
	}
}
