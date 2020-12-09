using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// An object with six sides which has one pair of parallel sides.
	/// </summary>
	/// <remarks>
	/// If you need a six-sided shape without the constraints that this class
	/// imposes, use <see cref="T:Northwoods.Go.GoPolygon" /> instead.
	/// You may find that this class is useful for implementing pentagonal
	/// shapes where three adjacent sides form two right angles.
	/// </remarks>
	[Serializable]
	public class GoHexagon : GoShape
	{
		private const int flagReshapableCorner = 1048576;

		private const int flagCrosswiseSymmetry = 2097152;

		private const int flagLengthwiseSymmetry = 4194304;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.DistanceLeft" /> property.
		/// </summary>
		public const int ChangedDistanceLeft = 1442;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.DistanceRight" /> property.
		/// </summary>
		public const int ChangedDistanceRight = 1443;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.DistanceTop" /> property.
		/// </summary>
		public const int ChangedDistanceTop = 1444;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.DistanceBottom" /> property.
		/// </summary>
		public const int ChangedDistanceBottom = 1445;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 1446;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.ReshapeBehavior" /> property.
		/// </summary>
		public const int ChangedReshapeBehavior = 1447;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.ReshapableCorner" /> property.
		/// </summary>
		public const int ChangedReshapableCorner = 1448;

		/// <summary>
		/// A special handle ID for a handle which controls the variable corner length.
		/// </summary>
		public const int LeftTopSideHandleID = 1026;

		/// <summary>
		/// A special handle ID for a handle which controls the location of the tips.
		/// </summary>
		public const int RightBottomSideHandleID = 1027;

		/// <summary>
		/// A special handle ID for a handle which controls the variable corner length.
		/// </summary>
		public const int LeftTopPointHandleID = 1028;

		/// <summary>
		/// A special handle ID for a handle which controls the location of the tips.
		/// </summary>
		public const int RightBottomPointHandleID = 1029;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.KeepsLengthwiseSymmetry" /> property.
		/// </summary>
		public const int ChangedKeepsLengthwiseSymmetry = 1449;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoHexagon.KeepsCrosswiseSymmetry" /> property.
		/// </summary>
		public const int ChangedKeepsCrosswiseSymmetry = 1450;

		private PointF[] myPoints = new PointF[6];

		private float myDistanceLeft = 10f;

		private float myDistanceRight = 10f;

		private float myDistanceTop = 10f;

		private float myDistanceBottom = 10f;

		private Orientation myOrientation;

		private GoHexagonReshapeBehavior myReshapeBehavior = GoHexagonReshapeBehavior.CompleteSymmetry;

		/// <summary>
		/// The distance between the left/top point and the Hexagon's left border.
		/// </summary>
		/// <value>
		/// This defaults to 10.
		/// A negative number yields a concave figure. 
		///             </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The distance between the left/top point and the Hexagon's left border.")]
		public virtual float DistanceLeft
		{
			get
			{
				return myDistanceLeft;
			}
			set
			{
				float num = myDistanceLeft;
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Vertical)
				{
					value = Bounds.Width / 2f;
				}
				if (num == value)
				{
					return;
				}
				myDistanceLeft = value;
				ResetPath();
				Changed(1442, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(myDistanceLeft));
				if (base.Initializing)
				{
					return;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (Orientation == Orientation.Horizontal)
					{
						DistanceRight = value;
					}
					else
					{
						DistanceRight = Bounds.Width - value;
					}
				}
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Vertical)
				{
					DistanceRight = Bounds.Width / 2f;
				}
			}
		}

		/// <summary>
		/// The distance between the right/bottom point and the Hexagon's right border.
		/// </summary>
		/// <value>
		/// This defaults to 10.
		/// A negative number yields a concave figure.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The distance between the right/bottom point and the Hexagon's right border.")]
		public virtual float DistanceRight
		{
			get
			{
				return myDistanceRight;
			}
			set
			{
				float num = myDistanceRight;
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Vertical)
				{
					value = Bounds.Width / 2f;
				}
				if (num == value)
				{
					return;
				}
				myDistanceRight = value;
				ResetPath();
				Changed(1443, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(myDistanceRight));
				if (base.Initializing)
				{
					return;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (Orientation == Orientation.Horizontal)
					{
						DistanceLeft = value;
					}
					else
					{
						DistanceLeft = Bounds.Width - value;
					}
				}
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Vertical)
				{
					myDistanceRight = Bounds.Width / 2f;
					DistanceLeft = Bounds.Width / 2f;
				}
			}
		}

		/// <summary>
		/// The distance between the left/top point and the Hexagon's top border.
		/// </summary>
		/// <value>
		/// This defaults to 10.
		/// A negative number yields a concave figure.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The distance between the left/top point and the Hexagon's top border.")]
		public virtual float DistanceTop
		{
			get
			{
				return myDistanceTop;
			}
			set
			{
				float num = myDistanceTop;
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Horizontal)
				{
					value = Bounds.Height / 2f;
				}
				if (num == value)
				{
					return;
				}
				myDistanceTop = value;
				ResetPath();
				Changed(1444, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(myDistanceTop));
				if (base.Initializing)
				{
					return;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (Orientation == Orientation.Vertical)
					{
						DistanceBottom = value;
					}
					else
					{
						DistanceBottom = Bounds.Height - value;
					}
				}
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Horizontal)
				{
					DistanceBottom = Bounds.Height / 2f;
				}
			}
		}

		/// <summary>
		/// The distance between the right/bottom point and the Hexagon's bottom border.
		/// </summary>
		/// <value>
		/// This defaults to 10.
		/// A negative number yields a concave figure.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The distance between the right/bottom point and the Hexagon's bottom border.")]
		public virtual float DistanceBottom
		{
			get
			{
				return myDistanceBottom;
			}
			set
			{
				float num = myDistanceBottom;
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Horizontal)
				{
					value = Bounds.Height / 2f;
				}
				if (num == value)
				{
					return;
				}
				myDistanceBottom = value;
				ResetPath();
				Changed(1445, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(myDistanceBottom));
				if (base.Initializing)
				{
					return;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (Orientation == Orientation.Vertical)
					{
						DistanceTop = value;
					}
					else
					{
						DistanceTop = Bounds.Height - value;
					}
				}
				if (KeepsLengthwiseSymmetry && Orientation == Orientation.Horizontal)
				{
					DistanceTop = Bounds.Height / 2f;
				}
			}
		}

		/// <summary>
		/// Gets or sets how to draw the hexagon, based on whether its
		/// prominent pair of verticies point vertically or horizontally.
		/// </summary>
		/// <value>
		/// This defaults to <c>"Orientation.Horizontal"</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Horizontal)]
		[Description("Whether the pair of parallel lines run vertically or horizontally")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				Orientation orientation = myOrientation;
				if (orientation == value)
				{
					return;
				}
				myOrientation = value;
				ResetPath();
				Changed(1446, 0, orientation, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (base.Initializing)
				{
					return;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (value == Orientation.Vertical)
					{
						DistanceRight = Bounds.Width - ((DistanceLeft < 0f) ? 0f : DistanceLeft);
						DistanceBottom = DistanceTop;
					}
					else
					{
						DistanceBottom = Bounds.Height - ((DistanceTop < 0f) ? 0f : DistanceTop);
						DistanceRight = DistanceLeft;
					}
				}
				if (KeepsLengthwiseSymmetry)
				{
					if (value == Orientation.Vertical)
					{
						DistanceLeft = Bounds.Width / 2f;
						DistanceRight = Bounds.Width / 2f;
					}
					else
					{
						DistanceTop = Bounds.Height / 2f;
						DistanceBottom = Bounds.Height / 2f;
					}
				}
			}
		}

		/// <summary>
		/// Determines the resize behavior when the user manipulates
		/// the resize handles.
		/// </summary>
		/// <value>
		/// This defaults to <c>GoHexagonResizeBehavior.FreeForm</c>.
		/// </value>
		/// <remarks>
		/// Note, however, that the actual resize behavior is constrained by the
		/// <see cref="P:Northwoods.Go.GoHexagon.KeepsLengthwiseSymmetry" /> and <see cref="P:Northwoods.Go.GoHexagon.KeepsCrosswiseSymmetry" />
		/// properties.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(GoHexagonReshapeBehavior.CompleteSymmetry)]
		[Description("What kind of symmetry to maintain when reshaping")]
		public virtual GoHexagonReshapeBehavior ReshapeBehavior
		{
			get
			{
				return myReshapeBehavior;
			}
			set
			{
				GoHexagonReshapeBehavior goHexagonReshapeBehavior = myReshapeBehavior;
				if (goHexagonReshapeBehavior != value)
				{
					myReshapeBehavior = value;
					Changed(1447, 0, goHexagonReshapeBehavior, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// Gets or sets whether to maintain the lengthwise distances at one half the crosswise bounds size member.
		/// (eg. Orientation == Orientation.Horizontal;
		///      DistanceTop = Bounds.Height/2; DistanceBottom = Bounds.Height/2)
		/// <summary>
		/// Gets or sets whether to maintain symmetry in respect to the lengthwise axis.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether to maintain symmetry in respect to the lengthwise axis.")]
		public virtual bool KeepsLengthwiseSymmetry
		{
			get
			{
				return (base.InternalFlags & 0x400000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x400000) != 0;
				if (flag == value)
				{
					return;
				}
				if (value)
				{
					base.InternalFlags |= 4194304;
				}
				else
				{
					base.InternalFlags &= -4194305;
				}
				Changed(1449, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					if (Orientation == Orientation.Vertical)
					{
						DistanceLeft = Bounds.Width / 2f;
						DistanceRight = Bounds.Width / 2f;
					}
					else
					{
						DistanceTop = Bounds.Height / 2f;
						DistanceBottom = Bounds.Height / 2f;
					}
				}
			}
		}

		/// Gets or sets whether to maintain the crosswise distances equal to each other and
		/// the crosswise distances equal to each other.
		/// (eg. Orientation == Orientation.Horizontal;
		///      DistanceTop = Bounds.Height/2; DistanceBottom = Bounds.Height/2)
		/// <summary>
		/// Gets or sets whether to maintain symmetry in respect to the crosswise axis.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether to maintain symmetry in respect to the crosswise axis.")]
		public virtual bool KeepsCrosswiseSymmetry
		{
			get
			{
				return (base.InternalFlags & 0x200000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x200000) != 0;
				if (flag == value)
				{
					return;
				}
				if (value)
				{
					base.InternalFlags |= 2097152;
				}
				else
				{
					base.InternalFlags &= -2097153;
				}
				Changed(1450, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					if (Orientation == Orientation.Vertical)
					{
						DistanceBottom = DistanceTop;
						DistanceRight = base.Width - ((DistanceLeft < 0f) ? 0f : DistanceLeft);
					}
					else
					{
						DistanceRight = DistanceLeft;
						DistanceBottom = base.Height - ((DistanceTop < 0f) ? 0f : DistanceTop);
					}
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
					Changed(1448, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a hexagon with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline, no <see cref="T:System.Drawing.Brush" /> fill, <see cref="P:Northwoods.Go.GoHexagon.Orientation" /> of <c>Orientation.Vertical</c>,
		/// and default distance dimensions.
		/// </summary>
		public GoHexagon()
		{
			base.InternalFlags |= 7340544;
		}

		/// <summary>
		/// Paint a possibly shadowed Hexagon.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, getPoints());
		}

		/// <summary>
		/// Consider the actual shape of the hexagon to determine
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
		/// The closest point of a hexagon that intersects with a given line
		/// is the closest such point of each of its six line segments.
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
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF7 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF3, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF4, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF4, pointF5, p1, p2, out result2))
			{
				float num5 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num5 < num)
				{
					num = num5;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF5, pointF6, p1, p2, out result2))
			{
				float num6 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num6 < num)
				{
					num = num6;
					pointF7 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF6, pointF, p1, p2, out result2))
			{
				float num7 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num7 < num)
				{
					num = num7;
					pointF7 = result2;
				}
			}
			result = pointF7;
			return num < 1E+21f;
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
			float num = DistanceLeft;
			float num2 = DistanceRight;
			float num3 = DistanceTop;
			float num4 = DistanceBottom;
			if (Orientation == Orientation.Horizontal)
			{
				if (num3 > bounds.Height)
				{
					num3 = bounds.Height;
				}
				else if (num3 < 0f)
				{
					num3 = 0f;
				}
				if (num4 > bounds.Height)
				{
					num4 = bounds.Height;
				}
				else if (num4 < 0f)
				{
					num4 = 0f;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (num < 0f - bounds.Width / 2f)
					{
						num = 0f - bounds.Width / 2f;
					}
					if (num > bounds.Width / 2f)
					{
						num = bounds.Width / 2f;
					}
					if (num2 < 0f - bounds.Width / 2f)
					{
						num2 = 0f - bounds.Width / 2f;
					}
					if (num2 > bounds.Width / 2f)
					{
						num2 = bounds.Width / 2f;
					}
					if (num >= 0f)
					{
						myPoints[0] = new PointF(bounds.X, bounds.Y + num3);
						myPoints[1] = new PointF(bounds.X + num, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width - num, bounds.Y);
						myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y + num3);
						myPoints[4] = new PointF(bounds.X + bounds.Width - num, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X + num, bounds.Y + bounds.Height);
					}
					else
					{
						myPoints[0] = new PointF(bounds.X - num, bounds.Y + num3);
						myPoints[1] = new PointF(bounds.X, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y);
						myPoints[3] = new PointF(bounds.X + bounds.Width + num, bounds.Y + num3);
						myPoints[4] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X, bounds.Y + bounds.Height);
					}
				}
				else
				{
					if (Math.Abs(num) > bounds.Width)
					{
						num = ((!(num <= 0f)) ? bounds.Width : (0f - bounds.Width));
						num2 = 0f;
					}
					if (num2 < 0f - (bounds.Width - Math.Abs(num)))
					{
						num2 = 0f - (bounds.Width - Math.Abs(num));
					}
					else if (num2 > bounds.Width - Math.Abs(num))
					{
						num2 = bounds.Width - Math.Abs(num);
					}
					if (num >= 0f)
					{
						if (num2 >= 0f)
						{
							myPoints[0] = new PointF(bounds.X, bounds.Y + num3);
							myPoints[1] = new PointF(bounds.X + num, bounds.Y);
							myPoints[2] = new PointF(bounds.X + bounds.Width - num2, bounds.Y);
							myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num4);
							myPoints[4] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height);
							myPoints[5] = new PointF(bounds.X + num, bounds.Y + bounds.Height);
						}
						else
						{
							myPoints[0] = new PointF(bounds.X, bounds.Y + num3);
							myPoints[1] = new PointF(bounds.X + num, bounds.Y);
							myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y);
							myPoints[3] = new PointF(bounds.X + bounds.Width + num2, bounds.Y + bounds.Height - num4);
							myPoints[4] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
							myPoints[5] = new PointF(bounds.X + num, bounds.Y + bounds.Height);
						}
					}
					else if (num2 >= 0f)
					{
						myPoints[0] = new PointF(bounds.X - num, bounds.Y + num3);
						myPoints[1] = new PointF(bounds.X, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width - num2, bounds.Y);
						myPoints[3] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num4);
						myPoints[4] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X, bounds.Y + bounds.Height);
					}
					else
					{
						myPoints[0] = new PointF(bounds.X - num, bounds.Y + num3);
						myPoints[1] = new PointF(bounds.X, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y);
						myPoints[3] = new PointF(bounds.X + bounds.Width + num2, bounds.Y + bounds.Height - num4);
						myPoints[4] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X, bounds.Y + bounds.Height);
					}
				}
			}
			else
			{
				if (num > bounds.Width)
				{
					num = bounds.Width;
				}
				if (num < 0f)
				{
					num = 0f;
				}
				if (num2 > bounds.Width)
				{
					num2 = bounds.Width;
				}
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (KeepsCrosswiseSymmetry)
				{
					if (num3 < 0f - bounds.Height / 2f)
					{
						num3 = 0f - bounds.Height / 2f;
					}
					if (num3 > bounds.Height / 2f)
					{
						num3 = bounds.Height / 2f;
					}
					if (num4 < 0f - bounds.Height / 2f)
					{
						num4 = 0f - bounds.Height / 2f;
					}
					if (num4 > bounds.Height / 2f)
					{
						num4 = bounds.Height / 2f;
					}
					if (num3 >= 0f)
					{
						myPoints[0] = new PointF(bounds.X + num, bounds.Y);
						myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y + num3);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num3);
						myPoints[3] = new PointF(bounds.X + num, bounds.Y + bounds.Height);
						myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height - num3);
						myPoints[5] = new PointF(bounds.X, bounds.Y + num3);
					}
					else
					{
						myPoints[0] = new PointF(bounds.X + num, bounds.Y - num3);
						myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
						myPoints[3] = new PointF(bounds.X + num, bounds.Y + bounds.Height + num3);
						myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X, bounds.Y);
					}
				}
				else
				{
					if (Math.Abs(num3) > bounds.Height)
					{
						num3 = ((!(num3 <= 0f)) ? bounds.Height : (0f - bounds.Height));
						num4 = 0f;
					}
					if (num4 < 0f - (bounds.Height - Math.Abs(num3)))
					{
						num4 = 0f - (bounds.Height - Math.Abs(num3));
					}
					if (num4 > bounds.Height - Math.Abs(num3))
					{
						num4 = bounds.Height - Math.Abs(num3);
					}
					if (num3 >= 0f)
					{
						if (num4 >= 0f)
						{
							myPoints[0] = new PointF(bounds.X + num, bounds.Y);
							myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y + num3);
							myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num4);
							myPoints[3] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height);
							myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height - num4);
							myPoints[5] = new PointF(bounds.X, bounds.Y + num3);
						}
						else
						{
							myPoints[0] = new PointF(bounds.X + num, bounds.Y);
							myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y + num3);
							myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
							myPoints[3] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height + num4);
							myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height);
							myPoints[5] = new PointF(bounds.X, bounds.Y + num3);
						}
					}
					else if (num4 >= 0f)
					{
						myPoints[0] = new PointF(bounds.X + num, bounds.Y - num3);
						myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height - num4);
						myPoints[3] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height);
						myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height - num4);
						myPoints[5] = new PointF(bounds.X, bounds.Y);
					}
					else
					{
						myPoints[0] = new PointF(bounds.X + num, bounds.Y - num3);
						myPoints[1] = new PointF(bounds.X + bounds.Width, bounds.Y);
						myPoints[2] = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
						myPoints[3] = new PointF(bounds.X + bounds.Width - num2, bounds.Y + bounds.Height + num4);
						myPoints[4] = new PointF(bounds.X, bounds.Y + bounds.Height);
						myPoints[5] = new PointF(bounds.X, bounds.Y);
					}
				}
			}
			return myPoints;
		}

		/// <summary>
		/// Support either allowing the user to move a corner control handle,
		/// or treating the hexagon as a whole object, maintaining either or
		/// both kinds of symmetry.
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
			bool cross = false;
			bool length = false;
			DetermineReshapeBehavior(ref cross, ref length);
			if (whichHandle >= 1026 && whichHandle <= 1029 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				RectangleF bounds = Bounds;
				RectangleF bounds2 = Bounds;
				bool flag = Orientation == Orientation.Horizontal;
				PointF pointF = default(PointF);
				float distanceLeft = DistanceLeft;
				float distanceRight = DistanceRight;
				float distanceTop = DistanceTop;
				float distanceBottom = DistanceBottom;
				float num = DistanceLeft;
				float num2 = DistanceRight;
				float num3 = DistanceTop;
				float num4 = DistanceBottom;
				switch (whichHandle)
				{
				case 1026:
					pointF = myPoints[0];
					if (flag)
					{
						num = newPoint.X - pointF.X;
						if (num > bounds.Width - Math.Abs(num2))
						{
							num = bounds.Width - Math.Abs(num2);
						}
						if (num < 0f)
						{
							bounds2.X = newPoint.X;
							bounds2.Y = bounds.Y;
							if (distanceLeft < 0f)
							{
								if (!cross)
								{
									bounds2.Width = bounds.Width + distanceLeft - num;
									bounds2.Height = bounds.Height;
								}
								else
								{
									bounds2.Width = bounds.Width + distanceLeft * 2f - num * 2f;
									bounds2.Height = bounds.Height;
								}
							}
							else if (!cross)
							{
								bounds2.Width = bounds.Width - num;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width - num * 2f;
								bounds2.Height = bounds.Height;
							}
							break;
						}
						bounds2.X = pointF.X;
						bounds2.Y = bounds.Y;
						if (distanceLeft < 0f)
						{
							if (!cross)
							{
								bounds2.Width = bounds.Width + distanceLeft;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width + distanceLeft * 2f;
								bounds2.Height = bounds.Height;
							}
						}
						else
						{
							bounds2.Width = bounds.Width;
							bounds2.Height = bounds.Height;
						}
						break;
					}
					num3 = newPoint.Y - pointF.Y;
					if (num3 > bounds.Height - Math.Abs(num4))
					{
						num3 = bounds.Height - Math.Abs(num4);
					}
					if (num3 < 0f)
					{
						bounds2.Y = newPoint.Y;
						bounds2.X = bounds.X;
						if (distanceTop < 0f)
						{
							if (!cross)
							{
								bounds2.Height = bounds.Height + distanceTop - num3;
								bounds2.Width = bounds.Width;
							}
							else
							{
								bounds2.Height = bounds.Height + distanceTop * 2f - num3 * 2f;
								bounds2.Width = bounds.Width;
							}
						}
						else if (!cross)
						{
							bounds2.Height = bounds.Height - num3;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - num3 * 2f;
							bounds2.Width = bounds.Width;
						}
						break;
					}
					bounds2.Y = pointF.Y;
					bounds2.X = bounds.X;
					if (distanceTop < 0f)
					{
						if (!cross)
						{
							bounds2.Height = bounds.Height + distanceTop;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height + distanceTop * 2f;
							bounds2.Width = bounds.Width;
						}
					}
					else
					{
						bounds2.Height = bounds.Height;
						bounds2.Width = bounds.Width;
					}
					break;
				case 1027:
					pointF = myPoints[3];
					if (flag)
					{
						num2 = pointF.X - newPoint.X;
						if (num2 > bounds.Width - Math.Abs(num))
						{
							num2 = bounds.Width - Math.Abs(num);
						}
						if (num2 < 0f)
						{
							bounds2.X = bounds.X;
							bounds2.Y = bounds.Y;
							if (distanceRight < 0f)
							{
								bounds2.Width = bounds.Width + distanceRight - num2;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width - num2;
								bounds2.Height = bounds.Height;
							}
						}
						else
						{
							bounds2.X = bounds.X;
							bounds2.Y = bounds.Y;
							if (distanceRight < 0f)
							{
								bounds2.Width = bounds.Width + distanceRight;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width;
								bounds2.Height = bounds.Height;
							}
						}
						break;
					}
					num4 = pointF.Y - newPoint.Y;
					if (num4 > bounds.Height - Math.Abs(num3))
					{
						num4 = bounds.Height - Math.Abs(num3);
					}
					if (num4 < 0f)
					{
						bounds2.Y = bounds.Y;
						bounds2.X = bounds.X;
						if (distanceBottom < 0f)
						{
							bounds2.Height = bounds.Height + distanceBottom - num4;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - num4;
							bounds2.Width = bounds.Width;
						}
					}
					else
					{
						bounds2.Y = bounds.Y;
						bounds2.X = bounds.X;
						if (distanceBottom < 0f)
						{
							bounds2.Height = bounds.Height + distanceBottom;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height;
							bounds2.Width = bounds.Width;
						}
					}
					break;
				case 1028:
					pointF = myPoints[1];
					if (flag)
					{
						num = pointF.X - newPoint.X;
						if (num < 0f - (bounds.Width - Math.Abs(num2)))
						{
							num = 0f - (bounds.Width - Math.Abs(num2));
						}
						if (num <= 0f)
						{
							bounds2.X = pointF.X;
							bounds2.Y = bounds.Y;
							if (distanceLeft < 0f)
							{
								bounds2.Width = bounds.Width;
								bounds2.Height = bounds.Height;
							}
							else if (!cross)
							{
								bounds2.Width = bounds.Width - distanceLeft;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width - distanceLeft * 2f;
								bounds2.Height = bounds.Height;
							}
						}
						else
						{
							bounds2.X = newPoint.X;
							bounds2.Y = bounds.Y;
							if (distanceLeft < 0f)
							{
								if (!cross)
								{
									bounds2.Width = bounds.Width + num;
								}
								else
								{
									bounds2.Width = bounds.Width + num * 2f;
								}
								bounds2.Height = bounds.Height;
							}
							else if (!cross)
							{
								bounds2.Width = bounds.Width - distanceLeft + num;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width - distanceLeft * 2f + num * 2f;
								bounds2.Height = bounds.Height;
							}
						}
						if (!length)
						{
							num3 = ((!(newPoint.Y < bounds.Y)) ? ((!(newPoint.Y > bounds.Y + bounds.Height)) ? (newPoint.Y - bounds.Y) : bounds.Height) : 0f);
						}
						break;
					}
					num3 = pointF.Y - newPoint.Y;
					if (num3 < 0f - (bounds.Height - Math.Abs(num4)))
					{
						num3 = 0f - (bounds.Height - Math.Abs(num4));
					}
					if (num3 <= 0f)
					{
						bounds2.Y = pointF.Y;
						bounds2.X = bounds.X;
						if (distanceTop < 0f)
						{
							bounds2.Height = bounds.Height;
							bounds2.Width = bounds.Width;
						}
						else if (!cross)
						{
							bounds2.Height = bounds.Height - distanceTop;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - distanceTop * 2f;
							bounds2.Width = bounds.Width;
						}
					}
					else
					{
						bounds2.Y = newPoint.Y;
						bounds2.X = bounds.X;
						if (distanceTop < 0f)
						{
							if (!cross)
							{
								bounds2.Height = bounds.Height + num3;
								bounds2.Width = bounds.Width;
							}
							else
							{
								bounds2.Height = bounds.Height + num3 * 2f;
								bounds2.Width = bounds.Width;
							}
						}
						else if (!cross)
						{
							bounds2.Height = bounds.Height - distanceTop + num3;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - distanceTop * 2f + num3 * 2f;
							bounds2.Width = bounds.Width;
						}
					}
					if (!length)
					{
						num = ((!(newPoint.X < bounds.X)) ? ((!(newPoint.X > bounds.X + bounds.Width)) ? (newPoint.X - bounds.X) : bounds.Width) : 0f);
					}
					break;
				case 1029:
					pointF = myPoints[2];
					if (flag)
					{
						num2 = newPoint.X - pointF.X;
						if (num2 < 0f - (bounds.Width - Math.Abs(num)))
						{
							num2 = 0f - (bounds.Width - Math.Abs(num));
						}
						if (num2 < 0f)
						{
							if (distanceRight < 0f)
							{
								bounds2 = bounds;
							}
							else
							{
								bounds2.X = bounds.X;
								bounds2.Y = bounds.Y;
								bounds2.Width = bounds.Width - distanceRight;
								bounds2.Height = bounds.Height;
							}
						}
						else
						{
							bounds2.X = bounds.X;
							bounds2.Y = bounds.Y;
							if (distanceRight < 0f)
							{
								bounds2.Width = bounds.Width + num2;
								bounds2.Height = bounds.Height;
							}
							else
							{
								bounds2.Width = bounds.Width - distanceRight + num2;
								bounds2.Height = bounds.Height;
							}
						}
						if (!length)
						{
							num4 = ((!(newPoint.Y < bounds.Y)) ? ((!(newPoint.Y > bounds.Y + bounds.Height)) ? (bounds.Y + bounds.Height - newPoint.Y) : 0f) : bounds.Height);
						}
						break;
					}
					num4 = newPoint.Y - pointF.Y;
					if (num4 < 0f - (bounds.Height - Math.Abs(num3)))
					{
						num4 = 0f - (bounds.Height - Math.Abs(num3));
					}
					if (num4 < 0f)
					{
						bounds2.Y = bounds.Y;
						bounds2.X = bounds.X;
						if (distanceBottom < 0f)
						{
							bounds2.Height = bounds.Height;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - distanceBottom;
							bounds2.Width = bounds.Width;
						}
					}
					else
					{
						bounds2.Y = bounds.Y;
						bounds2.X = bounds.X;
						if (distanceBottom < 0f)
						{
							bounds2.Height = bounds.Height + num4;
							bounds2.Width = bounds.Width;
						}
						else
						{
							bounds2.Height = bounds.Height - distanceBottom + num4;
							bounds2.Width = bounds.Width;
						}
					}
					if (!length)
					{
						num2 = ((!(newPoint.X < bounds.X)) ? ((!(newPoint.X > bounds.X + bounds.Width)) ? (bounds.X + bounds.Width - newPoint.X) : 0f) : bounds.Width);
					}
					break;
				}
				if (cross)
				{
					if (Orientation == Orientation.Horizontal)
					{
						if (Math.Abs(num) > bounds2.Width / 2f)
						{
							num = bounds2.Width / 2f * (float)Math.Sign(num);
						}
						num2 = num;
					}
					else
					{
						if (Math.Abs(num3) > bounds2.Height / 2f)
						{
							num3 = bounds2.Height / 2f * (float)Math.Sign(num3);
						}
						num4 = num3;
					}
				}
				DistanceLeft = num;
				DistanceTop = num3;
				if (!cross)
				{
					DistanceRight = num2;
					DistanceBottom = num4;
				}
				Bounds = bounds2;
				ResetPath();
				return;
			}
			RectangleF bounds3 = Bounds;
			RectangleF rectangleF = default(RectangleF);
			base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			rectangleF = Bounds;
			if (!CanReshape())
			{
				float num5 = rectangleF.Width / bounds3.Width;
				float num6 = rectangleF.Height / bounds3.Height;
				DistanceLeft *= num5;
				DistanceTop *= num6;
				if (!cross)
				{
					DistanceRight *= num5;
					DistanceBottom *= num6;
				}
			}
			if (cross)
			{
				if (Orientation == Orientation.Vertical)
				{
					DistanceRight = Bounds.Width - ((DistanceLeft < 0f) ? 0f : DistanceLeft);
					DistanceBottom = DistanceTop;
				}
				else
				{
					DistanceBottom = Bounds.Height - ((DistanceTop < 0f) ? 0f : DistanceTop);
					DistanceRight = DistanceLeft;
				}
			}
			if (length)
			{
				if (Orientation == Orientation.Vertical)
				{
					DistanceLeft = Bounds.Width / 2f;
					DistanceRight = Bounds.Width / 2f;
				}
				else
				{
					DistanceTop = Bounds.Height / 2f;
					DistanceBottom = Bounds.Height / 2f;
				}
			}
			ResetPath();
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> and <see cref="P:Northwoods.Go.GoHexagon.ReshapableCorner" />
		/// are true, this supports corner control handles.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape() && ReshapableCorner)
			{
				bool flag = Orientation == Orientation.Horizontal;
				bool cross = false;
				bool length = false;
				DetermineReshapeBehavior(ref cross, ref length);
				PointF[] points = getPoints();
				PointF pointF = default(PointF);
				pointF = ((!flag) ? points[5] : points[1]);
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1026, filled: true);
				MakeDiamondResizeHandle(handle, flag ? 64 : 128);
				pointF = points[0];
				handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1028, filled: true);
				MakeDiamondResizeHandle(handle, (!length) ? 1 : (flag ? 64 : 128));
				if (!cross)
				{
					pointF = ((!flag) ? points[2] : points[4]);
					handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1027, filled: true);
					MakeDiamondResizeHandle(handle, flag ? 64 : 128);
					pointF = points[3];
					handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1029, filled: true);
					MakeDiamondResizeHandle(handle, (!length) ? 1 : (flag ? 64 : 128));
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cross"></param>
		/// <param name="length"></param>
		private void DetermineReshapeBehavior(ref bool cross, ref bool length)
		{
			cross = KeepsCrosswiseSymmetry;
			length = KeepsLengthwiseSymmetry;
			switch (ReshapeBehavior)
			{
			case GoHexagonReshapeBehavior.FreeForm:
				break;
			case GoHexagonReshapeBehavior.LengthwiseSymmetry:
				length = true;
				break;
			case GoHexagonReshapeBehavior.CrosswiseSymmetry:
				cross = true;
				break;
			case GoHexagonReshapeBehavior.CompleteSymmetry:
				length = true;
				cross = true;
				break;
			}
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
			case 1442:
				DistanceLeft = e.GetFloat(undo);
				break;
			case 1443:
				DistanceRight = e.GetFloat(undo);
				break;
			case 1444:
				DistanceTop = e.GetFloat(undo);
				break;
			case 1445:
				DistanceBottom = e.GetFloat(undo);
				break;
			case 1447:
				ReshapeBehavior = (GoHexagonReshapeBehavior)e.GetValue(undo);
				break;
			case 1446:
				Orientation = (Orientation)e.GetValue(undo);
				break;
			case 1448:
				ReshapableCorner = (bool)e.GetValue(undo);
				break;
			case 1449:
				KeepsLengthwiseSymmetry = (bool)e.GetValue(undo);
				break;
			case 1450:
				KeepsCrosswiseSymmetry = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
