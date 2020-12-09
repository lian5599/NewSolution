using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object with eight sides which is kept convex and symmetrical in relation
	/// to its X and Y axis
	/// </summary>
	/// <remarks>
	/// If you need an eight-sided shape without the constraints that this class
	/// imposes, use <see cref="T:Northwoods.Go.GoPolygon" /> instead.
	/// </remarks>
	[Serializable]
	public class GoOctagon : GoShape
	{
		private const int flagReshapableCorner = 1048576;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoOctagon.Corner" /> property.
		/// </summary>
		public const int ChangedCorner = 1469;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoOctagon.ReshapableCorner" /> property.
		/// </summary>
		public const int ChangedReshapableCorner = 1470;

		/// <summary>
		/// A special handle ID for a handle which controls the corner's width.
		/// </summary>
		public const int CornerWidthHandleID = 1030;

		/// <summary>
		/// A special handle ID for a handle which controls the corner's height.
		/// </summary>
		public const int CornerHeightHandleID = 1031;

		private SizeF myCorner = new SizeF(10f, 10f);

		private PointF[] myPoints = new PointF[8];

		/// <summary>
		/// Gets or sets the width and height of each corner.
		/// </summary>
		/// <value>
		/// This defaults to 10x10.
		/// If given a negative value, it is set to 0.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum width and height of each corner")]
		public virtual SizeF Corner
		{
			get
			{
				return myCorner;
			}
			set
			{
				SizeF sizeF = myCorner;
				if (value.Width < 0f)
				{
					value.Width = 0f;
				}
				if (value.Height < 0f)
				{
					value.Height = 0f;
				}
				if (sizeF != value)
				{
					myCorner = value;
					ResetPath();
					Changed(1469, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the corner reshaping handles.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can reshape the corner of this resizable object.")]
		public virtual bool ReshapableCorner
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
					Changed(1470, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces an octagon with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline, no <see cref="T:System.Drawing.Brush" /> fill, and a default corner dimensions.
		/// </summary>
		public GoOctagon()
		{
			base.InternalFlags |= 1049088;
		}

		/// <summary>
		/// Paint a possibly shadowed octagon.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <seealso cref="P:Northwoods.Go.GoOctagon.Corner" />
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, getPoints());
		}

		/// <summary>
		/// Consider the actual shape of the octagon to determine
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
			return GetPath().IsVisible(p);
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> by adding lines between the points.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			graphicsPath.AddLines(getPoints());
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		private PointF[] getPoints()
		{
			RectangleF bounds = Bounds;
			SizeF corner = Corner;
			if (corner.Width > bounds.Width / 2f)
			{
				corner.Width = bounds.Width / 2f;
			}
			if (corner.Height > bounds.Height / 2f)
			{
				corner.Height = bounds.Height / 2f;
			}
			myPoints[0] = new PointF(bounds.X + corner.Width, bounds.Y);
			myPoints[1] = new PointF(bounds.X, bounds.Y + corner.Height);
			myPoints[2] = new PointF(bounds.X, bounds.Y + bounds.Height - corner.Height);
			myPoints[3] = new PointF(bounds.X + corner.Width, bounds.Y + bounds.Height);
			myPoints[4] = new PointF(bounds.X + bounds.Width - corner.Width, bounds.Y + bounds.Height);
			myPoints[5] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - corner.Height);
			myPoints[6] = new PointF(bounds.X + bounds.Width, bounds.Y + corner.Height);
			myPoints[7] = new PointF(bounds.X + bounds.Width - corner.Width, bounds.Y);
			return myPoints;
		}

		/// <summary>
		/// Support either allowing the user to move the corner control handles,
		/// or treating the octagon as a whole object.
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
			if (whichHandle >= 1030 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				RectangleF bounds = Bounds;
				SizeF corner = Corner;
				switch (whichHandle)
				{
				case 1030:
					if (newPoint.X < bounds.X)
					{
						corner.Width = 0f;
					}
					else if (newPoint.X >= bounds.X + bounds.Width / 2f)
					{
						corner.Width = bounds.Width / 2f;
					}
					else
					{
						corner.Width = newPoint.X - bounds.X;
					}
					break;
				case 1031:
					if (newPoint.Y < bounds.Y)
					{
						corner.Height = 0f;
					}
					else if (newPoint.Y >= bounds.Y + bounds.Height / 2f)
					{
						corner.Height = bounds.Height / 2f;
					}
					else
					{
						corner.Height = newPoint.Y - bounds.Y;
					}
					break;
				}
				Corner = corner;
				ResetPath();
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> and <see cref="P:Northwoods.Go.GoOctagon.ReshapableCorner" />
		/// are true, this supports corner control handles.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape() && ReshapableCorner)
			{
				RectangleF bounds = Bounds;
				PointF loc = new PointF(bounds.X + Corner.Width, bounds.Y);
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, loc, 1030, filled: true);
				MakeDiamondResizeHandle(handle, 64);
				loc = new PointF(bounds.X, bounds.Y + Corner.Height);
				handle = sel.CreateResizeHandle(this, selectedObj, loc, 1031, filled: true);
				MakeDiamondResizeHandle(handle, 128);
			}
		}

		/// <summary>
		/// The closest point of a octagon that intersects with a given line
		/// is the closest such point of each of its eight line segments.
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
			PointF pointF = GoShape.ExpandPointOnEdge(points[0], bounds, shift);
			PointF pointF2 = GoShape.ExpandPointOnEdge(points[1], bounds, shift);
			PointF pointF3 = GoShape.ExpandPointOnEdge(points[2], bounds, shift);
			PointF pointF4 = GoShape.ExpandPointOnEdge(points[3], bounds, shift);
			PointF pointF5 = GoShape.ExpandPointOnEdge(points[4], bounds, shift);
			PointF pointF6 = GoShape.ExpandPointOnEdge(points[5], bounds, shift);
			PointF pointF7 = GoShape.ExpandPointOnEdge(points[6], bounds, shift);
			PointF pointF8 = GoShape.ExpandPointOnEdge(points[7], bounds, shift);
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF9 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF3, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF4, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF4, pointF5, p1, p2, out result2))
			{
				float num5 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num5 < num)
				{
					num = num5;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF5, pointF6, p1, p2, out result2))
			{
				float num6 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num6 < num)
				{
					num = num6;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF6, pointF7, p1, p2, out result2))
			{
				float num7 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num7 < num)
				{
					num = num7;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF7, pointF8, p1, p2, out result2))
			{
				float num8 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num8 < num)
				{
					num = num8;
					pointF9 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF8, pointF, p1, p2, out result2))
			{
				float num9 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num9 < num)
				{
					num = num9;
					pointF9 = result2;
				}
			}
			result = pointF9;
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
			case 1469:
				Corner = e.GetSize(undo);
				break;
			case 1470:
				ReshapableCorner = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
