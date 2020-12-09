using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// This shape holds both closed and open figures consisting of
	/// segments that are either straight lines or Bezier curves.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This shape class, the most capable of the <see cref="T:Northwoods.Go.GoShape" />s, is a
	/// generalization of the <see cref="T:Northwoods.Go.GoPolygon" /> and <see cref="T:Northwoods.Go.GoStroke" /> classes.
	/// You can also rotate or flip a drawing, extract figures as drawings, or
	/// insert other shapes as figures.
	/// The <see cref="T:Northwoods.Go.GoFigure" /> enumeration specifies a number of predefined drawings.
	/// </para>
	/// <para>
	/// The term "figure" is used in two different but related ways here.
	/// Inside a GoDrawing there can be more than one figure, where each figure is a
	/// closed or open series of straight or curved segments.
	/// Although most <see cref="T:Northwoods.Go.GoFigure" /> enumerations specify <see cref="T:Northwoods.Go.GoDrawing" />s consisting
	/// of a single figure, some of the more complicated <see cref="T:Northwoods.Go.GoFigure" />s specify
	/// <see cref="T:Northwoods.Go.GoDrawing" />s that contain multiple figures.
	/// </para>
	/// <para>
	/// The most common usage is to just use the <see cref="M:Northwoods.Go.GoDrawing.#ctor(Northwoods.Go.GoFigure)" />
	/// constructor, followed by setting the <see cref="P:Northwoods.Go.GoShape.BrushColor" />,
	/// <see cref="P:Northwoods.Go.GoShape.PenColor" />, or other <see cref="T:Northwoods.Go.GoShape" /> properties.
	/// Remember that an instance of a GoDrawing is a single <see cref="T:Northwoods.Go.GoShape" />,
	/// with a single <see cref="P:Northwoods.Go.GoShape.Brush" /> and a single <see cref="P:Northwoods.Go.GoShape.Pen" />
	/// covering the whole shape.
	/// </para>
	/// <para>
	/// You can define your own drawings by specifying the points and the nature of each
	/// segment, along with whether each figure is open or closed.
	/// In a manner similar to drawing with a pencil, for each figure you want to create,
	/// call the <c>StartAt</c> method, call <c>LineTo</c> and/or <c>CurveTo</c> as many
	/// times as you need, followed by a call to <c>CloseFigure</c> if you want that figure
	/// to be a closed path and potentially filled by the <see cref="P:Northwoods.Go.GoShape.Brush" />.
	/// For example, this produces an open shape consisting of two curves:
	/// <pre>
	/// <code>
	/// GoDrawing s = new GoDrawing();
	/// s.StartAt(50, 50);
	/// s.CurveTo(75, 75, 100, 100, 50, 150);
	/// s.CurveTo(100, 150, 150, 100, 200, 200);
	/// </code>
	/// </pre>
	/// This produces a square with two curved lines connecting the top-left corner
	/// with the bottom-right corner:
	/// <pre>
	/// <code>
	/// GoDrawing s = new GoDrawing();
	/// s.StartAt(50, 50);
	/// s.LineTo(150, 50);
	/// s.LineTo(150, 150);
	/// s.LineTo(50, 150);
	/// s.LineTo(50, 50);
	/// s.CurveTo(125, 75, 125, 75, 150, 150);
	/// s.CurveTo(75, 125, 75, 125, 50, 50);
	/// s.CloseFigure();
	/// s.BrushColor = Color.Green;
	/// </code>
	/// </pre>
	///
	/// The above drawing is completely filled by the green color.
	/// If you had not called <see cref="M:Northwoods.Go.GoDrawing.CloseFigure" />, no brush would be painted
	/// in that figure.  Users could only pick this drawing by clicking along the lines.
	/// </para>
	/// <para>
	/// If you call <see cref="M:Northwoods.Go.GoDrawing.CloseFigure" /> and also set <see cref="P:Northwoods.Go.GoDrawing.FillMode" />
	/// to <c>FillMode.Alternate</c>, the inner area between the two curves will be open.
	/// No brush will be painted there, and <see cref="M:Northwoods.Go.GoDrawing.ContainsPoint(System.Drawing.PointF)" /> at any point
	/// in there will return false, although <see cref="M:Northwoods.Go.GoDrawing.ContainsPoint(System.Drawing.PointF)" /> will return
	/// true where the brush is painted.
	/// </para>
	/// <para>
	/// Here's an example of creating a drawing with three figures: two squares
	/// connected by a line.
	/// <pre><code>
	/// GoDrawing s = new GoDrawing();
	/// s.StartAt(50, 50);
	/// s.LineTo(100, 50);
	/// s.LineTo(100, 100);
	/// s.LineTo(50, 100);
	/// s.CloseFigure();
	/// s.StartAt(150, 50);
	/// s.LineTo(200, 50);
	/// s.LineTo(200, 100);
	/// s.LineTo(150, 100);
	/// s.CloseFigure();
	/// s.StartAt(100, 75);
	/// s.LineTo(150, 75);
	/// s.FillSimpleGradient(Color.Green, GoObject.MiddleLeft);
	/// </code></pre>
	/// Note that the gradient extends all the way from the left side to the right side.
	/// The left square is relatively light colored; the right square is relatively dark.
	/// </para>
	/// <para>
	/// You can rotate a drawing by setting the <see cref="P:Northwoods.Go.GoDrawing.Angle" /> property.
	/// Or you can rotate about any point by calling <see cref="M:Northwoods.Go.GoDrawing.Rotate(System.Drawing.PointF,System.Single)" />,
	/// which will also set the <see cref="P:Northwoods.Go.GoDrawing.Angle" />.  Please note that changing
	/// the angle may very well change the <see cref="P:Northwoods.Go.GoObject.Bounds" />, including
	/// the <see cref="P:Northwoods.Go.GoObject.Position" />.  You can get the bounds of this drawing
	/// as if the angle were zero by using the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedBounds" /> property.
	/// Just setting the <see cref="P:Northwoods.Go.GoDrawing.Angle" /> property will rotate the drawing about
	/// the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedCenter" /> -- the center of the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedBounds" />.
	/// </para>
	/// <para>
	/// You can also flip a drawing about either a horizontal axis or a vertical one.
	/// </para>
	/// <para>
	/// When a drawing is <see cref="P:Northwoods.Go.GoObject.Resizable" /> and <see cref="P:Northwoods.Go.GoObject.Reshapable" />,
	/// and when <see cref="P:Northwoods.Go.GoDrawing.ReshapablePoints" /> is true, the user will be able to drag any of
	/// the points of the drawing.
	/// </para>
	/// <para>
	/// For figures that are have their first point and their last point at the same <c>PointF</c>
	/// location, moving either shape might produce undesirable results unless the other point
	/// is moved too.  Set the <see cref="P:Northwoods.Go.GoDrawing.SameEndPoints" /> property to true to have
	/// <see cref="M:Northwoods.Go.GoDrawing.DoResizePointHandles(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" /> (called by <see cref="M:Northwoods.Go.GoDrawing.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />) keep both end points
	/// of figures together.
	/// </para>
	/// <para>
	/// Adjacent curved segments may meet at an angle.  Sometimes you would like to make sure
	/// that curves remain smooth between two curved segments, even when the user reshapes
	/// the points.  You can set <see cref="P:Northwoods.Go.GoDrawing.SmoothCurves" />, or call <see cref="M:Northwoods.Go.GoDrawing.MakeSmoothCurves" />.
	/// But note that curved segments will still form angles with an adjacent straight line segments.
	/// </para>
	/// <para>
	/// There are also methods for modifying individual points, for inserting segments into an
	/// existing figure, for opening or closing existing figures, for inserting whole shapes as
	/// figures into a drawing, and for extracting figures from a drawing into a new GoDrawing.
	/// </para>
	/// <para>
	/// The basic points and segment style and figure information is stored in a <see cref="T:Northwoods.Go.GoDrawingData" />,
	/// accessible via the <see cref="P:Northwoods.Go.GoDrawing.Data" /> property.  This information is used to construct
	/// a <c>GraphicsPath</c> in the <see cref="M:Northwoods.Go.GoDrawing.MakePath" /> method.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoDrawing.PointsCount" /> property returns the number of points in the whole drawing.
	/// The <see cref="P:Northwoods.Go.GoDrawing.FiguresCount" /> property returns the number of figures in the whole drawing.
	/// </para>
	/// <para>
	/// Since it is hard for a user to pick a figure when they have to precisely target pixels
	/// where the line is drawn, the <see cref="M:Northwoods.Go.GoDrawing.ContainsPoint(System.Drawing.PointF)" /> predicate allows for a certain
	/// amount of leeway.  This is governed by the <see cref="P:Northwoods.Go.GoDrawing.PickMargin" /> property.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoDrawing : GoShape
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int AddedPoint = 1151;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedPoint = 1152;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ModifiedPoint = 1153;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.Data" /> property.
		/// </summary>
		public const int ChangedData = 1154;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.FillMode" /> property.
		/// </summary>
		public const int ChangedFillMode = 1155;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.Angle" /> property.
		/// </summary>
		public const int ChangedAngle = 1156;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.Rotatable" /> property.
		/// </summary>
		internal const int ChangedRotatable = 1157;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.RotatesRealtime" /> property.
		/// </summary>
		internal const int ChangedRotatesRealtime = 1158;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.RotationPoint" /> property.
		/// </summary>
		internal const int ChangedRotationPoint = 1159;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedBounds" /> property.
		/// </summary>
		public const int ChangedUnrotatedBounds = 1160;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.SmoothCurves" /> property.
		/// </summary>
		public const int ChangedSmoothCurves = 1161;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.SameEndPoints" /> property.
		/// </summary>
		public const int ChangedSameEndPoints = 1162;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.ShowsBoundingHandle" /> property.
		/// </summary>
		public const int ChangedShowsBoundingHandle = 1163;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.ReshapablePoints" /> property.
		/// </summary>
		public const int ChangedReshapablePoints = 1164;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.ReshapableRectangle" /> property.
		/// </summary>
		public const int ChangedReshapableRectangle = 1165;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.PickMargin" /> property.
		/// </summary>
		public const int ChangedPickMargin = 1166;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDrawing.Figure" /> property.
		/// </summary>
		public const int ChangedFigure = 1167;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		internal const int ChangedClosed = 1169;

		internal const byte BeginCurveFlag = 64;

		internal const int flagSomeClosed = 1;

		internal const int flagFillModeWinding = 2;

		internal const int flagRotatable = 4;

		internal const int flagRotatesRealtime = 8;

		internal const int flagSmoothCurves = 16;

		internal const int flagSameEndPoints = 32;

		internal const int flagShowsBoundingHandle = 64;

		internal const int flagReshapablePoints = 128;

		internal const int flagReshapableRectangle = 256;

		internal const int flagAllClosed = 512;

		internal const int flagSkipUnrotatedBounds = 33554432;

		private int myInternalShapeFlags = 454;

		private GoFigure myFigure;

		private int myPointsCount;

		private PointF[] myPoints = new PointF[8];

		private byte[] myActions = new byte[8];

		private float myAngle;

		private RectangleF myUnrotatedBounds = new RectangleF(0f, 0f, 10f, 10f);

		private PointF[] myUnrotatedPoints = new PointF[8];

		private float myPickMargin = 3f;

		private PointF myRotationPoint = new PointF(-8.999999E+09f, -8.999999E+09f);

		[NonSerialized]
		private GraphicsPath myClosedPath;

		/// <summary>
		/// Gets or sets the last known kind of <see cref="T:Northwoods.Go.GoFigure" /> for this shape.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoFigure.None" />.
		/// </value>
		/// <remarks>
		/// Although this may have a value that is a particular <see cref="T:Northwoods.Go.GoFigure" />,
		/// the actual path may have been modified arbitrarily since this property was last set.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(GoFigure.None)]
		[Description("The kind of figure that this shape last took.")]
		public virtual GoFigure Figure
		{
			get
			{
				return myFigure;
			}
			set
			{
				GoFigure goFigure = myFigure;
				if (goFigure != value)
				{
					myFigure = value;
					Changed(1167, 0, goFigure, GoObject.NullRect, 0, myFigure, GoObject.NullRect);
					if (!base.Initializing)
					{
						float angle = Angle;
						_ = RotationPoint;
						RectangleF unrotatedBounds = UnrotatedBounds;
						Angle = 0f;
						GoFigureDrawing.Init(this, value);
						Bounds = unrotatedBounds;
						Angle = angle;
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of points in this shape.
		/// </summary>
		[Category("Appearance")]
		[Description("The number of points in this shape.")]
		public int PointsCount => myPointsCount;

		/// <summary>
		/// Gets or sets both the array of bytes of <see cref="T:Northwoods.Go.GoDrawingAction" />s and the array of <c>PointF</c>s,
		/// bundled together in a <see cref="T:Northwoods.Go.GoDrawingData" /> object.
		/// </summary>
		/// <remarks>
		/// Getting this property will result in a new <see cref="T:Northwoods.Go.GoDrawingData" /> with copies of the two arrays.
		/// </remarks>
		[Category("Appearance")]
		public GoDrawingData Data
		{
			get
			{
				return new GoDrawingData(CopyActionsArray(), CopyPointsArray());
			}
			set
			{
				if (value != null)
				{
					SetData(value.Actions, value.Points);
				}
			}
		}

		/// <summary>
		/// Returns the number of figures currently in this shape.
		/// </summary>
		/// <remarks>
		/// The number of figures is the same as the number of <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> points in the shape.
		/// </remarks>
		[Category("Appearance")]
		public int FiguresCount
		{
			get
			{
				int num = 0;
				int pointsCount = PointsCount;
				checked
				{
					for (int i = 0; i < pointsCount; i++)
					{
						if (GetAction(i) == GoDrawingAction.StartAt)
						{
							num++;
						}
					}
					return num;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the first and last point of each figure are at the same point.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// For polycurves setting this property to true will cause the loss of
		/// the straight line connecting the last and first points.
		/// (But note that if the whole figure consists of just the four points
		/// of a Bezier curve, having the first and last points the same may
		/// result in a degenerate Bezier curve: a straight line.)
		/// Even when this property is true, the first and last point of each figure
		/// might not be at the same position if they are modified by calling <see cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the first and the last point of each figure are at the same point.")]
		public virtual bool SameEndPoints
		{
			get
			{
				return (InternalShapeFlags & 0x20) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 0x20) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 32;
					}
					else
					{
						InternalShapeFlags &= -33;
					}
					ResetPath();
					Changed(1162, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether all points joining any two adjacent Bezier segments form a smooth curve.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// A value of true means that there are no corners between Bezier curves,
		/// although there still may be corners between straight line segments and
		/// between a Bezier segment and a straight segment.
		/// </value>
		/// <remarks>
		/// Even when this property is true, the rendered path might not be smooth
		/// if the points are modified by calling <see cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether all points joining any two adjacent Bezier segments form a smooth curve.")]
		public virtual bool SmoothCurves
		{
			get
			{
				return (InternalShapeFlags & 0x10) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 0x10) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 16;
					}
					else
					{
						InternalShapeFlags &= -17;
					}
					ResetPath();
					Changed(1161, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (value && !base.Initializing)
					{
						MakeSmoothCurves();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets how <see cref="M:Northwoods.Go.GoDrawing.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> and <see cref="M:Northwoods.Go.GoDrawing.ContainsPoint(System.Drawing.PointF)" />
		/// operate on closed figures.
		/// </summary>
		/// <value>
		/// The default value is <c>FillMode.Winding</c>.
		/// </value>
		/// <remarks>
		/// Consider also calling <see cref="M:Northwoods.Go.GoDrawing.ReversePaths" /> before combining two shapes.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(FillMode.Winding)]
		[Description("How to determine whether a point is inside or outside this shape.")]
		public virtual FillMode FillMode
		{
			get
			{
				if ((InternalShapeFlags & 2) == 0)
				{
					return FillMode.Alternate;
				}
				return FillMode.Winding;
			}
			set
			{
				bool flag = (InternalShapeFlags & 2) != 0;
				bool flag2 = value == FillMode.Winding;
				if (flag != flag2)
				{
					if (flag2)
					{
						InternalShapeFlags |= 2;
					}
					else
					{
						InternalShapeFlags &= -3;
					}
					ResetPath();
					Changed(1155, flag ? 1 : 0, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the approximate width of the region on either side of an open shape's line that
		/// is still considered "inside" the shape when picking.
		/// </summary>
		/// <value>
		/// The default value is 3.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(3f)]
		[Description("About how close users need to be to the stroke to pick it")]
		public virtual float PickMargin
		{
			get
			{
				return myPickMargin;
			}
			set
			{
				float num = myPickMargin;
				if (num != value)
				{
					myPickMargin = value;
					Changed(1166, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object, when selected, displays a bounding selection
		/// handle around this shape.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// This value is considered by <see cref="M:Northwoods.Go.GoDrawing.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> whether or
		/// not this shape is Resizable.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether a bounding handle appears around this shape when selected.")]
		public virtual bool ShowsBoundingHandle
		{
			get
			{
				return (InternalShapeFlags & 0x40) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 0x40) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 64;
					}
					else
					{
						InternalShapeFlags &= -65;
					}
					Changed(1163, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object, when selected, displays resize handles
		/// at each point of this shape, instead of the standard four or eight resize
		/// handles at the corners and middles of the rectangular bounds.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// This value is considered by <see cref="M:Northwoods.Go.GoDrawing.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> when this
		/// shape is Resizable and Reshapable.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the resize handles appear at each point.")]
		public virtual bool ReshapablePoints
		{
			get
			{
				return (InternalShapeFlags & 0x80) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 0x80) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 128;
					}
					else
					{
						InternalShapeFlags &= -129;
					}
					Changed(1164, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object, when selected, displays the standard four or eight resize
		/// handles at the corners and middles of the rectangular bounds, when this object is
		/// reshapable and <see cref="P:Northwoods.Go.GoDrawing.ReshapablePoints" /> is false.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// This value is considered by <see cref="M:Northwoods.Go.GoDrawing.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> when this
		/// shape is Resizable and Reshapable and when <see cref="P:Northwoods.Go.GoDrawing.ReshapablePoints" /> is false.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the resize handles appear at each spot of the rectangular box.")]
		public virtual bool ReshapableRectangle
		{
			get
			{
				return (InternalShapeFlags & 0x100) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 0x100) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 256;
					}
					else
					{
						InternalShapeFlags &= -257;
					}
					Changed(1165, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Bounds</c> of this shape if the <see cref="P:Northwoods.Go.GoDrawing.Angle" /> were zero.
		/// </summary>
		[Category("Bounds")]
		[Description("The Bounds this shape would have if the angle were zero.")]
		public RectangleF UnrotatedBounds
		{
			get
			{
				return myUnrotatedBounds;
			}
			set
			{
				if (myUnrotatedBounds != value)
				{
					float angle = Angle;
					if (angle == 0f)
					{
						Bounds = value;
						return;
					}
					double num = (double)angle * Math.PI / 180.0;
					double cosine = Math.Cos(num);
					double sine = Math.Sin(num);
					SetUnrotatedBounds(value, cosine, sine);
				}
			}
		}

		/// <summary>
		/// Gets the center point of the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedBounds" />.
		/// </summary>
		/// <remarks>
		/// This is the center that the shape rotated about. Unlike <see cref="P:Northwoods.Go.GoObject.Center" />,
		/// this point does not move when the shape is rotated.
		/// </remarks>
		[Browsable(false)]
		public PointF UnrotatedCenter => new PointF(myUnrotatedBounds.X + myUnrotatedBounds.Width / 2f, myUnrotatedBounds.Y + myUnrotatedBounds.Height / 2f);

		/// <summary>
		/// Gets or sets the angle at which this shape is drawn.
		/// </summary>
		/// <value>
		/// The default value is zero.
		/// </value>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoDrawing.Rotate(System.Drawing.PointF,System.Single)" /> about the unrotated center of this shape,
		/// with the angle being the difference between the old angle and the new one.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(0f)]
		[Description("The angle at which this shape is drawn")]
		public virtual float Angle
		{
			get
			{
				return myAngle;
			}
			set
			{
				float num = myAngle;
				if (num != value)
				{
					if (!base.Initializing)
					{
						Rotate(UnrotatedCenter, value - num);
					}
					else
					{
						setAngle(value);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the axis-of-rotation point.
		/// </summary>
		/// <value>
		/// This defaults to the <see cref="P:Northwoods.Go.GoDrawing.UnrotatedCenter" /> of this shape.
		/// </value>
		[Category("Behavior")]
		[Description("The point around which the user rotates this shape.")]
		internal PointF RotationPoint
		{
			get
			{
				if (IsRotationPointSet)
				{
					return myRotationPoint;
				}
				return UnrotatedCenter;
			}
			set
			{
				PointF pointF = myRotationPoint;
				if (pointF != value)
				{
					myRotationPoint = value;
					Changed(1159, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		internal bool IsRotationPointSet
		{
			get
			{
				if (myRotationPoint.X == -8.999999E+09f)
				{
					return myRotationPoint.Y != -8.999999E+09f;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets or sets whether the user can rotate this object.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the user can interactively rotate this object")]
		internal bool Rotatable
		{
			get
			{
				return (InternalShapeFlags & 4) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 4) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 4;
					}
					else
					{
						InternalShapeFlags &= -5;
					}
					Changed(1157, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object is actually rotated while the user is turning it.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether this objects is actually rotated while the user is rotating it")]
		internal bool RotatesRealtime
		{
			get
			{
				return (InternalShapeFlags & 8) != 0;
			}
			set
			{
				bool flag = (InternalShapeFlags & 8) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalShapeFlags |= 8;
					}
					else
					{
						InternalShapeFlags &= -9;
					}
					Changed(1158, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		internal int InternalShapeFlags
		{
			get
			{
				return myInternalShapeFlags;
			}
			set
			{
				myInternalShapeFlags = value;
			}
		}

		/// <summary>
		/// Construct a shape with no points in it.
		/// </summary>
		public GoDrawing()
		{
		}

		/// <summary>
		/// Construct a shape that is initialized to look like a
		/// particular <see cref="T:Northwoods.Go.GoFigure" />.
		/// </summary>
		/// <param name="f"></param>
		/// <remarks>
		/// The initial <see cref="P:Northwoods.Go.GoDrawing.Angle" /> will be zero.
		/// You can always change the figure by setting the <see cref="P:Northwoods.Go.GoDrawing.Figure" /> property.
		/// </remarks>
		public GoDrawing(GoFigure f)
		{
			myFigure = f;
			GoFigureDrawing.Init(this, f);
		}

		/// <summary>
		/// Overridden to avoid sharing internal data structures.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoDrawing goDrawing = (GoDrawing)base.CopyObject(env);
			if (goDrawing != null)
			{
				PointF[] destinationArray = new PointF[myPointsCount];
				Array.Copy(myPoints, 0, destinationArray, 0, myPointsCount);
				goDrawing.myPoints = destinationArray;
				byte[] destinationArray2 = new byte[myPointsCount];
				Array.Copy(myActions, 0, destinationArray2, 0, myPointsCount);
				goDrawing.myActions = destinationArray2;
				PointF[] destinationArray3 = new PointF[myPointsCount];
				Array.Copy(myUnrotatedPoints, 0, destinationArray3, 0, myPointsCount);
				goDrawing.myUnrotatedPoints = destinationArray3;
				goDrawing.myClosedPath = null;
			}
			return goDrawing;
		}

		private void InsertPoint(int i, byte act, PointF p)
		{
			if (PointsCount == 0 && act != 0)
			{
				return;
			}
			if (i < 0)
			{
				i = 0;
			}
			if (i > myPointsCount)
			{
				i = myPointsCount;
			}
			int j = i;
			byte b = 0;
			checked
			{
				if (act != 0 && IsFlaggedClosed(i - 1))
				{
					unchecked
					{
						myActions[i - 1] &= 127;
						act = (byte)(act | 0x80);
					}
				}
				else if (myPointsCount > 1 && myPointsCount > i && i > 0 && act == 0 && !IsStartAt(i - 1))
				{
					for (; !IsEndOfFigure(j); j++)
					{
					}
					if (IsFlaggedClosed(j))
					{
						myActions[i - 1] |= 128;
						b = unchecked((byte)(b | 1));
					}
				}
				int num = myPoints.Length;
				if (myPointsCount >= num)
				{
					int num2 = Math.Max(num * 2, myPointsCount + 1);
					PointF[] destinationArray = new PointF[num2];
					Array.Copy(myPoints, 0, destinationArray, 0, num);
					myPoints = destinationArray;
					byte[] destinationArray2 = new byte[num2];
					Array.Copy(myActions, 0, destinationArray2, 0, num);
					myActions = destinationArray2;
					PointF[] destinationArray3 = new PointF[num2];
					Array.Copy(myUnrotatedPoints, 0, destinationArray3, 0, num);
					myUnrotatedPoints = destinationArray3;
				}
				if (myPointsCount > i)
				{
					Array.Copy(myPoints, i, myPoints, i + 1, myPointsCount - i);
					Array.Copy(myActions, i, myActions, i + 1, myPointsCount - i);
					Array.Copy(myUnrotatedPoints, i, myUnrotatedPoints, i + 1, myPointsCount - i);
				}
				myPointsCount++;
				myPoints[i] = p;
				myActions[i] = act;
				if (Angle != 0f)
				{
					myUnrotatedPoints[i] = RotateHelper.RotatePoint(p, UnrotatedCenter, 0f - Angle);
				}
				else
				{
					myUnrotatedPoints[i] = p;
				}
				UpdateClosedFlags();
				ResetPath();
				base.InvalidBounds = true;
				Changed(1151, i, null, ToRect(act, p), j, null, ToRect(b, p));
			}
		}

		private void RemovePoint(int i)
		{
			if (i < 0 || i >= myPointsCount)
			{
				return;
			}
			int j = i;
			byte b = 0;
			checked
			{
				if (IsEndOfFigure(i) && IsFlaggedClosed(i) && !IsStartAt(i - 1))
				{
					myActions[i - 1] |= 128;
				}
				else if (IsStartAt(i) && i > 1 && i < PointsCount - 1 && !IsStartAt(i + 1))
				{
					if (IsFlaggedClosed(i - 1))
					{
						myActions[i - 1] &= 15;
						b = unchecked((byte)(b | 2));
					}
					for (j = i; !IsEndOfFigure(j); j++)
					{
					}
					if (IsFlaggedClosed(j))
					{
						myActions[j] &= 15;
						b = unchecked((byte)(b | 1));
					}
				}
				PointF p = myPoints[i];
				byte a = myActions[i];
				if (myPointsCount > i + 1)
				{
					Array.Copy(myPoints, i + 1, myPoints, i, myPointsCount - i - 1);
					Array.Copy(myActions, i + 1, myActions, i, myPointsCount - i - 1);
					Array.Copy(myUnrotatedPoints, i + 1, myUnrotatedPoints, i, myPointsCount - i - 1);
				}
				myPointsCount--;
				UpdateClosedFlags();
				ResetPath();
				base.InvalidBounds = true;
				Changed(1152, i, null, ToRect(a, p), j, null, ToRect(b, p));
			}
		}

		private RectangleF ToRect(byte a, PointF p)
		{
			return new RectangleF(p.X, p.Y, (int)a, 0f);
		}

		private byte ToByte(RectangleF r)
		{
			return checked((byte)r.Width);
		}

		/// <summary>
		/// Get the action at a particular index.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <returns>A <see cref="T:Northwoods.Go.GoDrawingAction" /> value</returns>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// You cannot modify the action for a particular point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDrawing.PointsCount" />
		public GoDrawingAction GetAction(int i)
		{
			if (i >= 0 && i < myPointsCount)
			{
				return (GoDrawingAction)checked((byte)(myActions[i] & 0xF));
			}
			throw new ArgumentException("GoDrawing.GetAction given an invalid index");
		}

		/// <summary>
		/// Get the point at a particular index.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <returns>A <c>PointF</c> in document coordinates</returns>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Single,System.Single)" />
		/// <seealso cref="P:Northwoods.Go.GoDrawing.PointsCount" />
		public PointF GetPoint(int i)
		{
			if (i >= 0 && i < myPointsCount)
			{
				return myPoints[i];
			}
			throw new ArgumentException("GoDrawing.GetPoint given an invalid index");
		}

		/// <summary>
		/// Replace the point at a particular index;
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.GetPoint(System.Int32)" />
		/// <seealso cref="P:Northwoods.Go.GoDrawing.PointsCount" />
		public void SetPoint(int i, PointF p)
		{
			if (i >= 0 && i < myPointsCount)
			{
				PointF pointF = myPoints[i];
				if (pointF != p)
				{
					myPoints[i] = p;
					myUnrotatedPoints[i] = RotateHelper.RotatePoint(p, UnrotatedCenter, 0f - Angle);
					ResetPath();
					base.InvalidBounds = true;
					Changed(1153, i, null, ToRect(myActions[i], pointF), i, null, ToRect(myActions[i], p));
				}
				return;
			}
			throw new ArgumentException("GoDrawing.SetPoint given an invalid index");
		}

		/// <summary>
		/// Replace the point at a particular index;
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.GetPoint(System.Int32)" />
		/// <seealso cref="P:Northwoods.Go.GoDrawing.PointsCount" />
		public void SetPoint(int i, float x, float y)
		{
			SetPoint(i, new PointF(x, y));
		}

		/// <summary>
		/// Remove all figures from this shape.
		/// </summary>
		/// <remarks>
		/// Afterwards the <see cref="P:Northwoods.Go.GoDrawing.PointsCount" /> will be zero.
		/// The paints and other properties are not affected.
		/// </remarks>
		public void ClearPoints()
		{
			SetData(null, null);
		}

		/// <summary>
		/// This returns a copy of the array of bytes representing the GoDrawingAction for each point.
		/// </summary>
		internal byte[] CopyActionsArray()
		{
			byte[] array = new byte[PointsCount];
			Array.Copy(myActions, 0, array, 0, PointsCount);
			for (int i = 0; i < array.Length; i = checked(i + 1))
			{
				array[i] &= 191;
			}
			return array;
		}

		/// <summary>
		/// This returns a copy of the array of points defining this shape.
		/// </summary>
		internal PointF[] CopyPointsArray()
		{
			PointF[] array = new PointF[PointsCount];
			Array.Copy(myPoints, 0, array, 0, PointsCount);
			return array;
		}

		/// <summary>
		/// Replace all of the points and actions for this shape.
		/// </summary>
		/// <param name="actions">A <c>byte</c> array whose values are one of the permitted actions</param>
		/// <param name="points">A <c>PointF</c> array whose points are in document coordinates</param>
		/// <remarks>
		/// The length of <paramref name="actions" /> and the length of <paramref name="points" />
		/// should be the same.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.CopyPointsArray" />
		internal void SetData(byte[] actions, PointF[] points)
		{
			int num = 0;
			if (actions != null && points != null)
			{
				num = Math.Min(actions.Length, points.Length);
			}
			RectangleF unrotatedBounds = UnrotatedBounds;
			Changing(1154);
			if (num > myPoints.Length)
			{
				myPoints = new PointF[num];
				myActions = new byte[num];
				myUnrotatedPoints = new PointF[num];
			}
			myPointsCount = num;
			if (num > 0)
			{
				Array.Copy(actions, 0, myActions, 0, num);
				Array.Copy(points, 0, myPoints, 0, num);
				RotateHelper.RotatePoints(points, myUnrotatedPoints, UnrotatedCenter, 0f - Angle);
			}
			int pointsCount = PointsCount;
			int num2 = 0;
			checked
			{
				for (int i = 1; i < pointsCount; i++)
				{
					byte b = myActions[i];
					if ((b & 0xF) == 3)
					{
						if (num2 == 0)
						{
							myActions[i] = 67;
						}
						else if (b == 131)
						{
							myActions[i] = 131;
						}
						else
						{
							myActions[i] = 3;
						}
						num2++;
						if (num2 == 3)
						{
							num2 = 0;
						}
					}
				}
				UpdateClosedFlags();
				ResetPath();
				base.InvalidBounds = true;
				Changed(1154, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
			}
		}

		/// <summary>
		/// Extend this shape by starting a new figure at a particular point.
		/// </summary>
		/// <param name="p"></param>
		public void StartAt(PointF p)
		{
			InsertPoint(PointsCount, 0, p);
		}

		/// <summary>
		/// Extend this shape by starting a new figure at a particular point.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.StartAt(System.Drawing.PointF)" />.
		/// </remarks>
		public void StartAt(float x, float y)
		{
			StartAt(new PointF(x, y));
		}

		/// <summary>
		/// Extend the current figure by drawing a straight line to a point.
		/// </summary>
		/// <param name="p">the destination point</param>
		public void LineTo(PointF p)
		{
			if (PointsCount <= 0)
			{
				throw new InvalidOperationException("Start each figure of a GoDrawing by first calling StartAt.");
			}
			InsertPoint(PointsCount, 1, p);
		}

		/// <summary>
		/// Extend the current figure by drawing a straight line to a point.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.LineTo(System.Drawing.PointF)" />.
		/// </remarks>
		public void LineTo(float x, float y)
		{
			LineTo(new PointF(x, y));
		}

		/// <summary>
		/// Extend the current figure by drawing a Bezier curve from the last
		/// point to another point, with two control points to guide the curve.
		/// </summary>
		/// <param name="c1">the first control point</param>
		/// <param name="c2">the second control point</param>
		/// <param name="p">the destination point</param>
		public void CurveTo(PointF c1, PointF c2, PointF p)
		{
			if (PointsCount <= 0)
			{
				throw new InvalidOperationException("Start each figure of a GoDrawing by first calling StartAt.");
			}
			InsertPoint(PointsCount, 67, c1);
			InsertPoint(PointsCount, 3, c2);
			InsertPoint(PointsCount, 3, p);
		}

		/// <summary>
		/// Extend the current figure by drawing a Bezier curve from the last
		/// point to another point, with two control points to guide the curve.
		/// </summary>
		/// <param name="c1x"></param>
		/// <param name="c1y"></param>
		/// <param name="c2x"></param>
		/// <param name="c2y"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.CurveTo(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF)" />.
		/// </remarks>
		public void CurveTo(float c1x, float c1y, float c2x, float c2y, float x, float y)
		{
			CurveTo(new PointF(c1x, c1y), new PointF(c2x, c2y), new PointF(x, y));
		}

		/// <summary>
		/// Start a new figure at the given point, perhaps ending any ongoing figure at that index.
		/// </summary>
		/// <param name="i">the index for starting a new figure</param>
		/// <param name="p">the start point</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoDrawing.IsSegmentIndex(System.Int32)" /> should be true for the given index <paramref name="i" />.
		/// </remarks>
		public void InsertStartAt(int i, PointF p)
		{
			if (IsSegmentIndex(i))
			{
				InsertPoint(i, 0, p);
				return;
			}
			throw new ArgumentException("Invalid index for starting a new figure: " + i);
		}

		/// <summary>
		/// Start a new figure at the given point, perhaps ending any ongoing figure at that index.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.InsertStartAt(System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		public void InsertStartAt(int i, float x, float y)
		{
			InsertStartAt(i, new PointF(x, y));
		}

		/// <summary>
		/// Insert a straight line segment into this shape at the given index.
		/// </summary>
		/// <param name="i">must be greater than zero</param>
		/// <param name="p">the destination point</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoDrawing.IsSegmentIndex(System.Int32)" /> should be true for the given index <paramref name="i" />.
		/// </remarks>
		public void InsertLineTo(int i, PointF p)
		{
			if (IsSegmentIndex(i) && i > 0)
			{
				InsertPoint(i, 1, p);
				return;
			}
			throw new ArgumentException("Invalid index for inserting a straight line: " + i);
		}

		/// <summary>
		/// Insert a straight line segment into this shape at the given index.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.InsertLineTo(System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		public void InsertLineTo(int i, float x, float y)
		{
			InsertLineTo(i, new PointF(x, y));
		}

		/// <summary>
		/// Insert a Bezier curve segment into this shape at the given index.
		/// </summary>
		/// <param name="i">the index for the first point, <paramref name="c1" />; must be greater than zero</param>
		/// <param name="c1">the first control point</param>
		/// <param name="c2">the second control point</param>
		/// <param name="p">the destination point</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoDrawing.IsSegmentIndex(System.Int32)" /> should be true for the given index <paramref name="i" />.
		/// </remarks>
		public void InsertCurveTo(int i, PointF c1, PointF c2, PointF p)
		{
			checked
			{
				if (IsSegmentIndex(i) && i > 0)
				{
					InsertPoint(i, 67, c1);
					InsertPoint(i + 1, 3, c2);
					InsertPoint(i + 2, 3, p);
					return;
				}
				throw new ArgumentException("Invalid index for inserting a Bezier curve: " + i);
			}
		}

		/// <summary>
		/// Extend the current figure by drawing a Bezier curve from the last
		/// point to another point, with two control points to guide the curve.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="c1x"></param>
		/// <param name="c1y"></param>
		/// <param name="c2x"></param>
		/// <param name="c2y"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.InsertCurveTo(System.Int32,System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF)" />.
		/// </remarks>
		public void InsertCurveTo(int i, float c1x, float c1y, float c2x, float c2y, float x, float y)
		{
			InsertCurveTo(i, new PointF(c1x, c1y), new PointF(c2x, c2y), new PointF(x, y));
		}

		/// <summary>
		/// Insert a straight line segment into this shape that will end at the given index.
		/// </summary>
		/// <param name="i">the destination index of the line</param>
		/// <param name="p">the starting point of the new line</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoDrawing.IsSegmentIndex(System.Int32)" /> should be true for the given index <paramref name="i" />.
		/// Useful for attaching to the beginning of figures.
		/// </remarks>
		public void InsertLineFrom(int i, PointF p)
		{
			if (IsSegmentIndex(i) && i >= 0 && i < myPointsCount)
			{
				InsertPoint(checked(i + 1), 1, GetPoint(i));
				SetPoint(i, p);
				return;
			}
			throw new ArgumentException("Invalid index for a straight line destination: " + i);
		}

		/// <summary>
		/// Insert a straight line segment into this shape that will end at the given index.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.InsertLineFrom(System.Int32,System.Drawing.PointF)" />.
		/// </remarks>
		public void InsertLineFrom(int i, float x, float y)
		{
			InsertLineFrom(i, new PointF(x, y));
		}

		/// <summary>
		/// Insert a Bezier curve segment into this shape that will end at the given index.
		/// </summary>
		/// <param name="i">the destination index of the curve</param>
		/// <param name="c1">the first control point</param>
		/// <param name="c2">the second control point</param>
		/// <param name="p">the starting point of the new curve</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoDrawing.IsSegmentIndex(System.Int32)" /> should be true for the given index <paramref name="i" />.
		/// Useful for attaching to the beginning of figures.
		/// </remarks>
		public void InsertCurveFrom(int i, PointF c1, PointF c2, PointF p)
		{
			checked
			{
				if (IsSegmentIndex(i) && i >= 0 && i < myPointsCount)
				{
					InsertPoint(i + 1, 67, c1);
					InsertPoint(i + 2, 3, c2);
					InsertPoint(i + 3, 3, GetPoint(i));
					SetPoint(i, p);
					return;
				}
				throw new ArgumentException("Invalid index for a Bezier curve destination: " + i);
			}
		}

		/// <summary>
		/// Insert a Bezier curve segment into this shape that will end at the given index.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="c1x"></param>
		/// <param name="c1y"></param>
		/// <param name="c2x"></param>
		/// <param name="c2y"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.InsertCurveFrom(System.Int32,System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF)" />.
		/// </remarks>
		public void InsertCurveFrom(int i, float c1x, float c1y, float c2x, float c2y, float x, float y)
		{
			InsertCurveFrom(i, new PointF(c1x, c1y), new PointF(c2x, c2y), new PointF(x, y));
		}

		/// <summary>
		/// This predicate is true when the given index is the start of a segment of this shape.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns>
		/// true unless the <paramref name="idx" /> refers to the second or third
		/// points of a Bezier <see cref="F:Northwoods.Go.GoDrawingAction.CurveTo" /> (open or closed)
		/// -- we cannot break up a curve.
		/// </returns>
		public bool IsSegmentIndex(int idx)
		{
			if (idx < 0)
			{
				return false;
			}
			if (idx == 0)
			{
				return true;
			}
			int pointsCount = PointsCount;
			if (pointsCount == 0)
			{
				return true;
			}
			if (idx >= pointsCount)
			{
				return true;
			}
			GoDrawingAction goDrawingAction = (GoDrawingAction)myActions[idx];
			if (goDrawingAction != 0 && goDrawingAction != GoDrawingAction.LineTo && goDrawingAction != GoDrawingAction.ClosedLine)
			{
				return goDrawingAction == GoDrawingAction.BeginCurveTo;
			}
			return true;
		}

		private int NextSegmentIndex(int idx)
		{
			int pointsCount = PointsCount;
			checked
			{
				for (int i = idx + 1; i < pointsCount; i++)
				{
					if (IsSegmentIndex(i))
					{
						return i;
					}
				}
				return pointsCount;
			}
		}

		private int PreviousSegmentIndex(int idx)
		{
			int pointsCount = PointsCount;
			if (idx > pointsCount)
			{
				idx = pointsCount;
			}
			checked
			{
				for (int num = idx - 1; num >= 0; num--)
				{
					if (IsSegmentIndex(num))
					{
						return num;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// This predicate is true when the given index is the start of a figure in this shape.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		public bool IsFigureIndex(int idx)
		{
			if (idx >= 0 && idx < PointsCount)
			{
				return IsStartAt(idx);
			}
			return false;
		}

		private bool IsStartAt(int i)
		{
			return myActions[i] == 0;
		}

		/// <summary>
		/// Iterates through the figure until it finds the next <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> from the index.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns>The next <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> index, or the <see cref="P:Northwoods.Go.GoDrawing.PointsCount" />
		/// if there is no next <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> index.</returns>
		private int NextFigureIndex(int idx)
		{
			int pointsCount = PointsCount;
			checked
			{
				for (int i = idx + 1; i < pointsCount; i++)
				{
					if (IsFigureIndex(i))
					{
						return i;
					}
				}
				return pointsCount;
			}
		}

		/// <summary>
		/// Iterates backwards through the figure until it finds the previous <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> from the index.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns>The previous <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> index.</returns>
		private int PreviousFigureIndex(int idx)
		{
			int pointsCount = PointsCount;
			if (idx > pointsCount)
			{
				idx = pointsCount;
			}
			checked
			{
				for (int num = idx - 1; num >= 0; num--)
				{
					if (IsFigureIndex(num))
					{
						return num;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// This predicate determines if the figure containing the given index is closed. 
		/// </summary>
		/// <param name="i">
		/// An index contained in the figure to be determined closed or not.
		/// If less than zero, it will be set to zero.
		/// If beyond the end of the array, it will be set to the last index.
		/// </param>
		/// <returns>
		/// true if the figure containing the index is closed.
		/// false if the figure containing the index is open, or if there are no points.
		/// </returns>
		/// <remarks>
		/// A closed figure will be filled with a brush if one is set, and will have
		/// a line drawn from its end point to its start point, if they are not already at the same point.
		/// To close or open figures, use <see cref="M:Northwoods.Go.GoDrawing.OpenFigure(System.Int32)" /> and <see cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" />.
		/// </remarks>
		public bool IsClosedFigure(int i)
		{
			if (PointsCount == 0)
			{
				return false;
			}
			checked
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i >= PointsCount)
				{
					i = PointsCount - 1;
				}
				while (!IsEndOfFigure(i))
				{
					i++;
				}
				return IsFlaggedClosed(i);
			}
		}

		private bool IsFlaggedClosed(int i)
		{
			return (myActions[i] & 0x80) != 0;
		}

		private bool IsEndOfFigure(int i)
		{
			checked
			{
				if (i == PointsCount - 1)
				{
					return true;
				}
				if (i < myPointsCount - 1 && GetAction(i + 1) == GoDrawingAction.StartAt)
				{
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Opens the figure containing the given index.
		/// </summary>
		/// <param name="i">Any index in the figure</param>
		/// <remarks>
		/// A closed figure will be filled with a brush if one is set, and will have
		/// a line drawn from its end point to its start point, if they are not already at the same point.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.CloseFigure" />
		/// <seealso cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoDrawing.IsClosedFigure(System.Int32)" />
		public void OpenFigure(int i)
		{
			checked
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i >= PointsCount)
				{
					i = PointsCount - 1;
				}
				while (!IsEndOfFigure(i))
				{
					i++;
				}
				if (myActions[i] == 129 || myActions[i] == 131)
				{
					myActions[i] &= 15;
					ResetPath();
					Changed(1169, i, true, GoObject.NullRect, i, false, GoObject.NullRect);
				}
				UpdateClosedFlags();
			}
		}

		/// <summary>
		/// Closes the figure containing the given index.
		/// </summary>
		/// <param name="i">Any index in the figure</param>
		/// <remarks>
		/// <para>
		/// A closed figure will be filled with a brush if one is set, and will have
		/// a line drawn from its end point to its start point, if they are not already at the same point.
		/// </para>
		/// <para>
		/// This method is different from <see cref="M:Northwoods.Go.GoDrawing.CompleteFigure(System.Int32)" /> because
		/// this results in the figure being filled with a brush, whereas <see cref="M:Northwoods.Go.GoDrawing.CompleteFigure(System.Int32)" />
		/// causes the figure to be left unfilled when painted.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.OpenFigure(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoDrawing.IsClosedFigure(System.Int32)" />
		public void CloseFigure(int i)
		{
			checked
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i >= PointsCount)
				{
					i = PointsCount - 1;
				}
				while (!IsEndOfFigure(i))
				{
					i++;
				}
				if (!IsStartAt(i) && (myActions[i] & 0x80) == 0)
				{
					myActions[i] |= 128;
					ResetPath();
					Changed(1169, i, false, GoObject.NullRect, i, true, GoObject.NullRect);
				}
				UpdateClosedFlags();
			}
		}

		/// <summary>
		/// Close the current (last) figure.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" /> passing it the last index.
		/// You may find it convenient to call this after constructing the last segment of
		/// a figure that you want to be filled with the brush.
		/// </remarks>
		public void CloseFigure()
		{
			CloseFigure(checked(PointsCount - 1));
		}

		/// <summary>
		/// Opens all figures in the shape.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.OpenFigure(System.Int32)" /> with the end index of each figure.
		/// </remarks>
		public void OpenAllFigures()
		{
			for (int i = 0; i < PointsCount; i = checked(i + 1))
			{
				if (IsEndOfFigure(i))
				{
					OpenFigure(i);
				}
			}
		}

		/// <summary>
		/// Closes all figures in the shape.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" /> with the end index of each figure.
		/// You may find it convenient to call this after constructing some figures a segment at a time,
		/// when you intend all of the figures to be filled with the brush.
		/// </remarks>
		public void CloseAllFigures()
		{
			for (int i = 0; i < PointsCount; i = checked(i + 1))
			{
				if (IsEndOfFigure(i))
				{
					CloseFigure(i);
				}
			}
		}

		/// <summary>
		/// Determines if the GoDrawing has closed figures
		/// </summary>
		/// <returns>true if at least one figure is closed, false otherwise</returns>
		/// <remarks>
		/// When this is true, <see cref="M:Northwoods.Go.GoDrawing.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> will fill the closed
		/// figures of this shape with the <see cref="P:Northwoods.Go.GoShape.Brush" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.IsClosedFigure(System.Int32)" />
		public bool HasClosedFigures()
		{
			return (InternalShapeFlags & 1) != 0;
		}

		private bool HasAllClosedFigures()
		{
			return (InternalShapeFlags & 0x200) != 0;
		}

		private void UpdateClosedFlags()
		{
			int num = 0;
			int num2 = 0;
			checked
			{
				for (int i = 0; i < PointsCount; i++)
				{
					if (IsStartAt(i))
					{
						num++;
					}
					if ((i + 1 == PointsCount || IsStartAt(i + 1)) && IsFlaggedClosed(i))
					{
						num2++;
					}
				}
				if (num2 == 0)
				{
					InternalShapeFlags &= -2;
				}
				else
				{
					InternalShapeFlags |= 1;
				}
				if (num2 > 0 && num2 == num)
				{
					InternalShapeFlags |= 512;
				}
				else
				{
					InternalShapeFlags &= -513;
				}
			}
		}

		/// <summary>
		/// Completes a figure by adding a line segment from the last point to the start point of the figure, if needed.
		/// </summary>
		/// <param name="i">an index in the figure</param>
		/// <remarks>
		/// This method is different from <see cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" /> because, if needed,
		/// it inserts a point that is the same as the first point of the segment without
		/// causing the figure to be filled with a brush.  <see cref="M:Northwoods.Go.GoDrawing.CloseFigure(System.Int32)" />
		/// results in the figure being filled when painted.
		/// </remarks>
		public void CompleteFigure(int i)
		{
			checked
			{
				if (i < 0)
				{
					i = 0;
				}
				else if (i >= PointsCount)
				{
					i = PointsCount - 1;
				}
				int num = i;
				while (!IsFigureIndex(num))
				{
					num--;
				}
				int num2 = NextFigureIndex(num) - 1;
				PointF point = GetPoint(num);
				PointF point2 = GetPoint(num2);
				if (num2 - num >= 2 && point != point2)
				{
					InsertLineTo(num2 + 1, point);
				}
			}
		}

		/// <summary>
		/// Complete the current (last) figure.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.CompleteFigure(System.Int32)" /> passing it the last index.
		/// You may find it convenient to call this after constructing the last segment of
		/// a figure that you want to complete by having it return straight to the first
		/// point of the segment.
		/// </remarks>
		public void CompleteFigure()
		{
			CompleteFigure(checked(PointsCount - 1));
		}

		/// <summary>
		/// Completes all figures by adding a line segment from the last point to the start point of each figure, if needed.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.CompleteFigure(System.Int32)" /> with each figure index.
		/// </remarks>
		public void CompleteAllFigures()
		{
			for (int num = 0; num < PointsCount; num = NextFigureIndex(num))
			{
				CompleteFigure(num);
			}
		}

		/// <summary>
		/// Remove the action at the given index.
		/// </summary>
		/// <param name="idx">a valid insertion index greater than zero</param>
		/// <remarks>
		/// This will remove the action and point for the index <paramref name="idx" />.
		/// If the action is a <see cref="F:Northwoods.Go.GoDrawingAction.CurveTo" /> (open or closed),
		/// this will remove the three points of this Bezier curve segment.
		/// This method does not simply remove a <c>StartAt</c> action, causing
		/// two figures to be combined--call <see cref="M:Northwoods.Go.GoDrawing.JoinFigure(System.Int32)" /> to do that.
		/// </remarks>
		public void RemoveSegment(int idx)
		{
			int pointsCount = PointsCount;
			if (pointsCount <= 0)
			{
				return;
			}
			checked
			{
				if (idx >= pointsCount)
				{
					idx = pointsCount - 1;
				}
				int num = idx;
				while (!IsSegmentIndex(num))
				{
					num--;
				}
				GoDrawingAction goDrawingAction = unchecked((GoDrawingAction)myActions[num]);
				if (goDrawingAction <= GoDrawingAction.LineTo)
				{
					switch (goDrawingAction)
					{
					default:
						return;
					case GoDrawingAction.StartAt:
					{
						if (num >= pointsCount - 1)
						{
							RemovePoint(num);
							return;
						}
						GoDrawingAction goDrawingAction2;
						unchecked
						{
							goDrawingAction2 = (GoDrawingAction)myActions[checked(num + 1)];
						}
						if (goDrawingAction2 <= GoDrawingAction.LineTo)
						{
							switch (goDrawingAction2)
							{
							default:
								return;
							case GoDrawingAction.StartAt:
								RemovePoint(num);
								return;
							case GoDrawingAction.LineTo:
								break;
							}
						}
						else
						{
							switch (goDrawingAction2)
							{
							default:
								return;
							case GoDrawingAction.ClosedLine:
								break;
							case GoDrawingAction.BeginCurveTo:
								myPoints[num] = myPoints[num + 3];
								RemovePoint(num + 1);
								RemovePoint(num + 1);
								RemovePoint(num + 1);
								return;
							}
						}
						myPoints[num] = myPoints[num + 1];
						RemovePoint(num + 1);
						return;
					}
					case GoDrawingAction.LineTo:
						break;
					}
				}
				else
				{
					switch (goDrawingAction)
					{
					default:
						return;
					case GoDrawingAction.ClosedLine:
						break;
					case GoDrawingAction.BeginCurveTo:
						RemovePoint(num);
						RemovePoint(num);
						RemovePoint(num);
						return;
					}
				}
				RemovePoint(num);
			}
		}

		/// <summary>
		/// Remove the <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> action that begins the figure at point <paramref name="idx" />,
		/// causing the figure to become part of the previous figure.
		/// </summary>
		/// <param name="idx">the zero-based index of a point</param>
		/// <remarks>
		/// This method does nothing if the figure is the first one in this shape.
		/// This method will iterate idx backward until it reaches a StartAt.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.RemoveSegment(System.Int32)" />
		public void JoinFigure(int idx)
		{
			int pointsCount = PointsCount;
			checked
			{
				if (pointsCount > 0)
				{
					if (idx >= pointsCount)
					{
						idx = pointsCount - 1;
					}
					int num = idx;
					while (!IsSegmentIndex(num))
					{
						num--;
					}
					if (num > 0 && GetAction(num) == GoDrawingAction.StartAt)
					{
						RemovePoint(num);
					}
				}
			}
		}

		internal void BreakUpBezier(PointF start, PointF c1, PointF c2, PointF end, float fraction, out PointF curve1cp1, out PointF curve1cp2, out PointF midpoint, out PointF curve2cp1, out PointF curve2cp2)
		{
			float num = 1f - fraction;
			PointF pointF = new PointF(start.X * num + c1.X * fraction, start.Y * num + c1.Y * fraction);
			PointF pointF2 = new PointF(c1.X * num + c2.X * fraction, c1.Y * num + c2.Y * fraction);
			PointF pointF3 = new PointF(c2.X * num + end.X * fraction, c2.Y * num + end.Y * fraction);
			PointF pointF4 = new PointF(pointF.X * num + pointF2.X * fraction, pointF.Y * num + pointF2.Y * fraction);
			PointF pointF5 = new PointF(pointF2.X * num + pointF3.X * fraction, pointF2.Y * num + pointF3.Y * fraction);
			PointF pointF6 = new PointF(pointF4.X * num + pointF5.X * fraction, pointF4.Y * num + pointF5.Y * fraction);
			new PointF(pointF6.X, pointF6.Y);
			curve1cp1 = pointF;
			curve1cp2 = pointF4;
			midpoint = pointF6;
			curve2cp1 = pointF5;
			curve2cp2 = pointF3;
		}

		/// <summary>
		/// Removes a figure or figures from a GoDrawing, placing them in a new GoDrawing.
		/// Returns null if the parameters are incorrect.
		/// </summary>
		/// <param name="start">An index that is a <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> vertex</param>
		/// <param name="end">The <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> vertex to stop extraction at, or the last+1 index</param>
		/// <returns>A GoDrawing created from the removed figure(s).</returns>
		public GoDrawing ExtractShape(int start, int end)
		{
			if (start > end)
			{
				throw new ArgumentException("GoDrawing.ExtractShape indexes cross");
			}
			if (start >= myPointsCount || GetAction(start) != 0)
			{
				throw new ArgumentException("GoDrawing.ExtractShape given an invalid start index");
			}
			if (end > myPointsCount)
			{
				end = myPointsCount;
			}
			if (end < myPointsCount && GetAction(end) != 0)
			{
				throw new ArgumentException("GoDrawing.SetPoint given an invalid end index");
			}
			checked
			{
				int num = end - start;
				PointF[] array = new PointF[num];
				byte[] array2 = new byte[num];
				GoDrawing obj = (GoDrawing)Copy();
				Array.Copy(myPoints, start, array, 0, num);
				Array.Copy(myActions, start, array2, 0, num);
				GoDrawingData goDrawingData2 = obj.Data = new GoDrawingData(array2, array);
				int num2 = start + (PointsCount - end);
				PointF[] array3 = new PointF[num2];
				byte[] array4 = new byte[num2];
				Array.Copy(myPoints, 0, array3, 0, start);
				Array.Copy(myPoints, end, array3, start, myPointsCount - end);
				Array.Copy(myActions, 0, array4, 0, start);
				Array.Copy(myActions, end, array4, start, myPointsCount - end);
				Data = new GoDrawingData(array4, array3);
				return obj;
			}
		}

		/// <summary>
		/// Inserts a GoShape into the GoDrawing at the given index.
		/// </summary>
		/// <param name="i">The <see cref="F:Northwoods.Go.GoDrawingAction.StartAt" /> index to insert before, or the last+1 index to insert at the end</param>
		/// <param name="s">The shape to insert</param>
		public void InsertShape(int i, GoShape s)
		{
			GraphicsPath graphicsPath = s.MakePath();
			PointF[] pathPoints = graphicsPath.PathPoints;
			byte[] pathTypes = graphicsPath.PathTypes;
			checked
			{
				PointF[] array = new PointF[graphicsPath.PointCount + PointsCount];
				byte[] array2 = new byte[graphicsPath.PointCount + PointsCount];
				if (i > PointsCount)
				{
					i = PointsCount;
				}
				while (i < PointsCount && !IsFigureIndex(i))
				{
					i++;
				}
				Array.Copy(myPoints, 0, array, 0, i);
				Array.Copy(myActions, 0, array2, 0, i);
				Array.Copy(pathPoints, 0, array, i, graphicsPath.PointCount);
				Array.Copy(pathTypes, 0, array2, i, graphicsPath.PointCount);
				Array.Copy(myPoints, i, array, i + graphicsPath.PointCount, PointsCount - i);
				Array.Copy(myActions, i, array2, i + graphicsPath.PointCount, PointsCount - i);
				Data = new GoDrawingData(array2, array);
			}
		}

		/// <summary>
		/// Reverse the order of actions and points in this shape.
		/// </summary>
		/// <remarks>
		/// The resulting shape may look the same, but may combine differently with other shapes
		/// due to a change in deciding whether an area is inside or outside of the shape.
		/// </remarks>
		public void ReversePaths()
		{
			if (PointsCount == 0)
			{
				return;
			}
			PointF[] array = CopyPointsArray();
			byte[] array2 = CopyActionsArray();
			Array.Reverse(array, 0, array.Length);
			Array.Reverse(array2, 0, array2.Length);
			checked
			{
				if (array2[array2.Length - 1] == 0)
				{
					Array.Copy(array2, 0, array2, 1, array2.Length - 1);
					array2[0] = 0;
				}
				for (int i = 0; i < myPointsCount; i++)
				{
					if ((array2[i] & 0x80) != 0)
					{
						array2[i] &= 15;
						for (; i < PointsCount - 1 && array2[i + 1] != 0; i++)
						{
						}
						if (array2[i] != 0)
						{
							array2[i] |= 128;
						}
					}
				}
				SetData(array2, array);
			}
		}

		/// <summary>
		/// Horizontally flips this GoDrawing about its center.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.FlipHorizontal(System.Single)" /> with the horizontal center.
		/// </remarks>
		public void FlipHorizontal()
		{
			FlipHorizontal(Location.X + base.Width / 2f);
		}

		/// <summary>
		/// Horizontally flips this GoDrawing about a given line.
		/// </summary>
		/// <param name="line">The x value to flip about.</param>
		public void FlipHorizontal(float line)
		{
			PointF[] array = new PointF[myPointsCount];
			for (int i = 0; i < myPointsCount; i = checked(i + 1))
			{
				array[i].X = line * 2f - myPoints[i].X;
				array[i].Y = myPoints[i].Y;
			}
			SetData(myActions, array);
		}

		/// <summary>
		/// Vertically flips this GoDrawing about its center.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoDrawing.FlipVertical(System.Single)" /> with the vertical center.
		/// </remarks>
		public void FlipVertical()
		{
			FlipVertical(Location.Y + base.Height / 2f);
		}

		/// <summary>
		/// Vertically flips this GoDrawing about a given line.
		/// </summary>
		/// <param name="line">The y value to flip about.</param>
		public void FlipVertical(float line)
		{
			PointF[] array = new PointF[myPointsCount];
			for (int i = 0; i < myPointsCount; i = checked(i + 1))
			{
				array[i].Y = line * 2f - myPoints[i].Y;
				array[i].X = myPoints[i].X;
			}
			SetData(myActions, array);
		}

		/// <summary>
		/// Iterates through all of the points, attempting to smooth any adjacent bezier curves.
		/// </summary>
		public void MakeSmoothCurves()
		{
			int pointsCount = PointsCount;
			checked
			{
				if (pointsCount >= 1)
				{
					for (int i = 1; i <= pointsCount; i++)
					{
						SmoothCurveAt(i - 1, 0);
					}
				}
			}
		}

		private void SmoothCurveAt(int idx, int side)
		{
			if (idx < 0 || idx >= PointsCount)
			{
				return;
			}
			checked
			{
				if ((GetAction(idx) & GoDrawingAction.Mask) != GoDrawingAction.CurveTo)
				{
					if (!SameEndPoints || !IsFigureIndex(idx))
					{
						return;
					}
					idx = NextFigureIndex(idx) - 1;
				}
				int i = idx - 1;
				int num = NextSegmentIndex(idx);
				if (num != idx + 1)
				{
					return;
				}
				if (num == NextFigureIndex(idx))
				{
					if (!SameEndPoints)
					{
						return;
					}
					num = PreviousFigureIndex(idx) + 1;
				}
				if (num < PointsCount && (GetAction(num) & GoDrawingAction.Mask) == GoDrawingAction.CurveTo)
				{
					PointF point = GetPoint(i);
					PointF point2 = GetPoint(idx);
					PointF point3 = GetPoint(num);
					switch (side)
					{
					case -1:
					{
						double num5 = Math.Sqrt((point3.X - point2.X) * (point3.X - point2.X) + (point3.Y - point2.Y) * (point3.Y - point2.Y));
						double radians2 = GetRadians(point2.X - point.X, point2.Y - point.Y);
						SetPoint(num, new PointF((float)((double)point2.X + num5 * Math.Cos(radians2)), (float)((double)point2.Y + num5 * Math.Sin(radians2))));
						break;
					}
					case 1:
					{
						double num4 = Math.Sqrt((point.X - point2.X) * (point.X - point2.X) + (point.Y - point2.Y) * (point.Y - point2.Y));
						double radians = GetRadians(point2.X - point3.X, point2.Y - point3.Y);
						SetPoint(i, new PointF((float)((double)point2.X + num4 * Math.Cos(radians)), (float)((double)point2.Y + num4 * Math.Sin(radians))));
						break;
					}
					default:
					{
						PointF result = default(PointF);
						GoStroke.NearestPointOnLine(point3, point, point2, out result);
						float num2 = point2.X - result.X;
						float num3 = point2.Y - result.Y;
						SetPoint(i, new PointF(point.X + num2, point.Y + num3));
						SetPoint(num, new PointF(point3.X + num2, point3.Y + num3));
						break;
					}
					}
				}
			}
		}

		private static double GetRadians(float x, float y)
		{
			double num;
			if (x == 0f)
			{
				num = ((!(y > 0f)) ? (-Math.PI / 2.0) : (Math.PI / 2.0));
			}
			else if (y == 0f)
			{
				num = ((!(x > 0f)) ? Math.PI : 0.0);
			}
			else
			{
				num = Math.Atan(Math.Abs(y / x));
				if (x < 0f)
				{
					num = ((!(y < 0f)) ? (Math.PI - num) : (num + Math.PI));
				}
				else if (y < 0f)
				{
					num = Math.PI * 2.0 - num;
				}
			}
			return num;
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			checked
			{
				if (PointsCount > 1)
				{
					int num = 0;
					for (int i = 0; i < PointsCount - 1; i++)
					{
						if (!IsStartAt(i) || !IsStartAt(i + 1))
						{
							num++;
						}
					}
					if (!IsStartAt(PointsCount - 1))
					{
						num++;
					}
					if (num > 1)
					{
						PointF[] array = new PointF[num];
						byte[] array2 = new byte[num];
						num = 0;
						for (int j = 0; j < PointsCount - 1; j++)
						{
							if (!IsStartAt(j) || !IsStartAt(j + 1))
							{
								array[num] = myPoints[j];
								array2[num] = myActions[j];
								array2[num] &= 191;
								num++;
							}
						}
						if (!IsStartAt(PointsCount - 1))
						{
							array[num] = myPoints[PointsCount - 1];
							array2[num] = myActions[PointsCount - 1];
							num++;
						}
						return new GraphicsPath(array, array2, FillMode);
					}
				}
				GraphicsPath graphicsPath = new GraphicsPath(FillMode);
				graphicsPath.AddRectangle(new RectangleF(base.Left, base.Top, 0.1f, 0.1f));
				return graphicsPath;
			}
		}

		private GraphicsPath MakePartialClosedPath(GraphicsPath g)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			int[] array = new int[g.PointCount];
			checked
			{
				for (int j = 0; j < g.PointCount; j++)
				{
					if (g.PathTypes[j] == 0)
					{
						i = j;
					}
					if (g.PathTypes[j] == 129 || g.PathTypes[j] == 131)
					{
						for (num = j; i <= num; i++)
						{
							array[num2++] = i;
						}
					}
				}
				if (num2 <= 0)
				{
					return null;
				}
				PointF[] array2 = new PointF[num2];
				byte[] array3 = new byte[num2];
				for (int k = 0; k < num2; k++)
				{
					array2[k] = g.PathPoints[array[k]];
					array3[k] = g.PathTypes[array[k]];
				}
				return new GraphicsPath(array2, array3, FillMode);
			}
		}

		/// <summary>
		/// Overridden from base class to include myClosedPath.
		/// </summary>
		protected override void ResetPath()
		{
			if (myClosedPath != null)
			{
				myClosedPath.Dispose();
				myClosedPath = null;
			}
			base.ResetPath();
		}

		private GraphicsPath GetClosedPath()
		{
			if (!HasClosedFigures())
			{
				return null;
			}
			GraphicsPath path = GetPath();
			if (HasAllClosedFigures())
			{
				return path;
			}
			if (myClosedPath == null)
			{
				myClosedPath = MakePartialClosedPath(path);
			}
			return myClosedPath;
		}

		/// <summary>
		/// Paint this shape, with an optional shadow.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			if (PointsCount < 1)
			{
				return;
			}
			GraphicsPath path = GetPath();
			GraphicsPath closedPath = GetClosedPath();
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (Brush != null && closedPath != null)
				{
					Brush shadowBrush = GetShadowBrush(view);
					if (shadowBrush != null)
					{
						g.TranslateTransform(shadowOffset.Width, shadowOffset.Height);
						GoShape.DrawPath(g, view, null, shadowBrush, closedPath);
						g.TranslateTransform(0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
				if (Pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					if (shadowPen != null)
					{
						g.TranslateTransform(shadowOffset.Width, shadowOffset.Height);
						GoShape.DrawPath(g, view, shadowPen, null, path);
						g.TranslateTransform(0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
			}
			if (Brush != null && closedPath != null)
			{
				GoShape.DrawPath(g, view, null, Brush, closedPath);
			}
			GoShape.DrawPath(g, view, Pen, null, path);
			DisposePath(path);
		}

		internal void SetUnrotatedBounds(RectangleF newr, double cosine, double sine)
		{
			RectangleF unrotatedBounds = UnrotatedBounds;
			Changing(1154);
			RectangleF unrotatedBounds2 = UnrotatedBounds;
			PointF unrotatedCenter = UnrotatedCenter;
			PointF rp = RotateHelper.RotatePoint(RotationPoint, unrotatedCenter, cosine, 0.0 - sine);
			GoStroke.RescalePoints(myUnrotatedPoints, unrotatedBounds2, newr);
			PointF pointF = new PointF(newr.X + newr.Width / 2f, newr.Y + newr.Height / 2f);
			RotateHelper.RotatePoints(myUnrotatedPoints, myPoints, pointF, cosine, sine);
			InternalShapeFlags |= 33554432;
			base.InvalidBounds = true;
			myUnrotatedBounds = newr;
			ResetPath();
			_ = Bounds;
			Changed(1160, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
			Changed(1154, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
			if (IsRotationPointSet)
			{
				PointF p = RotateHelper.RescalePoint(unrotatedBounds2, newr, rp, 0f);
				RotationPoint = RotateHelper.RotatePoint(p, pointF, cosine, sine);
			}
		}

		/// <summary>
		/// Changing the bounds of a shape may change all of its points.
		/// </summary>
		/// <param name="oldr">the earlier bounds, a <c>RectangleF</c> in document coordinates</param>
		/// <remarks>
		/// All of the points are modified to reflect the translation and
		/// scaling of the new bounding rectangle from the old one.
		/// </remarks>
		protected override void OnBoundsChanged(RectangleF oldr)
		{
			base.OnBoundsChanged(oldr);
			RectangleF unrotatedBounds = UnrotatedBounds;
			PointF rotationPoint = RotationPoint;
			RectangleF bounds = Bounds;
			if (oldr.Width == bounds.Width && oldr.Height == bounds.Height)
			{
				float num = bounds.X - oldr.X;
				float num2 = bounds.Y - oldr.Y;
				if (num != 0f || num2 != 0f)
				{
					GoStroke.TranslatePoints(myPoints, num, num2);
					GoStroke.TranslatePoints(myUnrotatedPoints, num, num2);
					myUnrotatedBounds.X += num;
					myUnrotatedBounds.Y += num2;
					ResetPath();
					base.InvalidBounds = false;
					if (IsRotationPointSet)
					{
						RotationPoint = new PointF(rotationPoint.X + num, rotationPoint.Y + num2);
					}
				}
			}
			else
			{
				Changing(1154);
				PointF unrotatedCenter = UnrotatedCenter;
				PointF rotatePoint = RotateHelper.RescalePoint(oldr, bounds, unrotatedCenter, Angle);
				GoStroke.RescalePoints(myPoints, oldr, bounds);
				RotateHelper.RotatePoints(myPoints, myUnrotatedPoints, rotatePoint, 0f - Angle);
				ResetPath();
				base.InvalidBounds = true;
				Changed(1154, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
				if (IsRotationPointSet)
				{
					RotationPoint = RotateHelper.RescalePoint(oldr, bounds, rotationPoint, Angle);
				}
			}
		}

		/// <summary>
		/// The bounding rectangle of a shape is computed as the smallest
		/// rectangle that includes all of its points.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// If there are only zero or one points, the size will be zero.
		/// </remarks>
		protected override RectangleF ComputeBounds()
		{
			int pointsCount = PointsCount;
			if (pointsCount <= 0)
			{
				PointF position = base.Position;
				return new RectangleF(position.X, position.Y, 0f, 0f);
			}
			if (pointsCount == 1)
			{
				return new RectangleF(myPoints[0].X, myPoints[1].Y, 0f, 0f);
			}
			RectangleF result = ComputeBoundsInternal(myPoints, myActions, pointsCount);
			if ((InternalShapeFlags & 0x2000000) == 0)
			{
				RectangleF unrotatedBounds = UnrotatedBounds;
				if (Angle != 0f)
				{
					myUnrotatedBounds = ComputeBoundsInternal(myUnrotatedPoints, myActions, pointsCount);
				}
				else
				{
					myUnrotatedBounds = result;
				}
				Changed(1160, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
			}
			else
			{
				InternalShapeFlags &= -33554433;
			}
			return result;
		}

		private RectangleF ComputeBoundsInternal(PointF[] v, byte[] a, int vlen)
		{
			RectangleF result = SimpleBounds(v, vlen);
			bool flag = false;
			checked
			{
				for (int i = 1; i < vlen; i++)
				{
					if ((a[i] & 0xF) == 3)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return result;
				}
				float num = Math.Max(result.Width, result.Height);
				float epsilon = Math.Min(Math.Max(0.01f, num / 100f), 1f);
				PointF pointF = v[0];
				float num2 = pointF.X;
				float num3 = pointF.Y;
				float num4 = pointF.X;
				float num5 = pointF.Y;
				int num6 = 0;
				for (int j = 1; j < vlen; j++)
				{
					if ((a[j] & 0xF) == 3)
					{
						num6++;
					}
					switch (num6)
					{
					case 3:
					{
						num6 = 0;
						PointF s = v[j - 3];
						PointF c = v[j - 2];
						PointF c2 = v[j - 1];
						PointF e = v[j];
						RectangleF rectangleF = GoStroke.BezierBounds(s, c, c2, e, epsilon);
						num2 = Math.Min(num2, rectangleF.X);
						num3 = Math.Min(num3, rectangleF.Y);
						num4 = Math.Max(num4, rectangleF.X + rectangleF.Width);
						num5 = Math.Max(num5, rectangleF.Y + rectangleF.Height);
						break;
					}
					case 0:
						pointF = v[j];
						num2 = Math.Min(num2, pointF.X);
						num3 = Math.Min(num3, pointF.Y);
						num4 = Math.Max(num4, pointF.X);
						num5 = Math.Max(num5, pointF.Y);
						break;
					}
				}
				return new RectangleF(num2, num3, num4 - num2, num5 - num3);
			}
		}

		internal RectangleF SimpleBounds(PointF[] v, int vlen)
		{
			PointF pointF = v[0];
			float num = pointF.X;
			float num2 = pointF.Y;
			float num3 = pointF.X;
			float num4 = pointF.Y;
			for (int i = 1; i < vlen; i = checked(i + 1))
			{
				pointF = v[i];
				num = Math.Min(num, pointF.X);
				num2 = Math.Min(num2, pointF.Y);
				num3 = Math.Max(num3, pointF.X);
				num4 = Math.Max(num4, pointF.Y);
			}
			return new RectangleF(num, num2, num3 - num, num4 - num2);
		}

		/// <summary>
		/// A point is in a figure only if it is inside its fill area if
		/// the figure is closed (even if it has no Brush), and
		/// only if the point is near any of the lines of the figures
		/// if those figures are open.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// This currently does not take any pen width into account.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			if (!base.ContainsPoint(p))
			{
				return false;
			}
			GraphicsPath closedPath = GetClosedPath();
			if (closedPath != null)
			{
				bool flag = closedPath.IsVisible(p);
				if (!flag)
				{
					flag = (GetSegmentNearPoint(p, PickMargin + PenWidth / 2f) >= 0);
				}
				return flag;
			}
			return GetSegmentNearPoint(p, PickMargin) >= 0;
		}

		/// <summary>
		/// Return the index of the first point of a segment of this shape
		/// that is close to a given point.
		/// </summary>
		/// <param name="pnt">A <c>PointF</c> in document coordinates</param>
		/// <param name="pickMargin"></param>
		/// <returns>
		/// The zero-based index of the first point of a segment,
		/// or <c>-1</c> if no segment is near <paramref name="pnt" />.
		/// </returns>
		/// <remarks>
		/// This is sensitive to the <see cref="P:Northwoods.Go.GoDrawing.PickMargin" /> value.
		/// For Bezier segments, the index of the first of each set of
		/// points is returned.
		/// </remarks>
		public int GetSegmentNearPoint(PointF pnt, float pickMargin)
		{
			RectangleF bounds = Bounds;
			float num = 1f;
			float num2 = Math.Max(pickMargin, 0f);
			num += num2;
			if (pnt.X < bounds.X - num || pnt.X > bounds.X + bounds.Width + num || pnt.Y < bounds.Y - num || pnt.Y > bounds.Y + bounds.Height + num)
			{
				return -1;
			}
			int pointsCount = PointsCount;
			if (pointsCount <= 1)
			{
				return -1;
			}
			num -= num2 / 2f;
			float epsilon = num * Math.Max(1f, Math.Max(bounds.Width, bounds.Height) / 1000f);
			PointF pointF = myPoints[0];
			int num3 = 0;
			checked
			{
				for (int i = 1; i < pointsCount; i++)
				{
					GoDrawingAction action = GetAction(i);
					if (action == GoDrawingAction.StartAt)
					{
						if (IsFlaggedClosed(i - 1))
						{
							PointF pointF2 = myPoints[i - 1];
							if (pointF2 != pointF && GoStroke.LineContainsPoint(pointF2, pointF, num, pnt))
							{
								return i - 1;
							}
						}
						pointF = myPoints[i];
						num3 = 0;
						continue;
					}
					if ((action & GoDrawingAction.Mask) == GoDrawingAction.CurveTo)
					{
						num3++;
					}
					switch (num3)
					{
					case 3:
					{
						num3 = 0;
						PointF s = myPoints[i - 3];
						PointF c = myPoints[i - 2];
						PointF c2 = myPoints[i - 1];
						PointF e = myPoints[i];
						if (GoStroke.BezierContainsPoint(s, c, c2, e, epsilon, pnt))
						{
							return i - 3;
						}
						break;
					}
					case 0:
					{
						PointF a = myPoints[i - 1];
						PointF b = myPoints[i];
						if (GoStroke.LineContainsPoint(a, b, num, pnt))
						{
							return i - 1;
						}
						break;
					}
					}
				}
				if (IsFlaggedClosed(pointsCount - 1))
				{
					PointF pointF3 = myPoints[pointsCount - 1];
					if (pointF3 != pointF && GoStroke.LineContainsPoint(pointF3, pointF, num, pnt))
					{
						return pointsCount - 1;
					}
				}
				return -1;
			}
		}

		/// <summary>
		/// The closest intersection point of a shape with a line is the
		/// closest such point for each of its segments.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		/// <remarks>
		/// This currently does not always take into account any pen width.
		/// </remarks>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float num = 1f;
			float num2 = 1E+21f;
			PointF pointF = default(PointF);
			int num3 = 0;
			int pointsCount = PointsCount;
			checked
			{
				for (int i = 1; i < pointsCount; i++)
				{
					if ((GetAction(i) & GoDrawingAction.Mask) == GoDrawingAction.CurveTo)
					{
						num3++;
					}
					PointF result2;
					switch (num3)
					{
					case 3:
					{
						num3 = 0;
						PointF pointF2 = myPoints[i - 3];
						PointF c = myPoints[i - 2];
						PointF c2 = myPoints[i - 1];
						PointF pointF3 = myPoints[i];
						if (GoStroke.LineContainsPoint(p1, p2, num, pointF3))
						{
							float num5 = (pointF3.X - p1.X) * (pointF3.X - p1.X) + (pointF3.Y - p1.Y) * (pointF3.Y - p1.Y);
							if (num5 < num2)
							{
								num2 = num5;
								pointF = pointF3;
							}
						}
						if (GoStroke.LineContainsPoint(p1, p2, num, pointF2))
						{
							float num6 = (pointF2.X - p1.X) * (pointF2.X - p1.X) + (pointF2.Y - p1.Y) * (pointF2.Y - p1.Y);
							if (num6 < num2)
							{
								num2 = num6;
								pointF = pointF2;
							}
						}
						if (GoStroke.BezierNearestIntersectionOnLine(pointF2, c, c2, pointF3, p1, p2, 1f, out result2))
						{
							float num7 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num7 < num2)
							{
								num2 = num7;
								pointF = result2;
							}
						}
						break;
					}
					case 0:
					{
						PointF a = GoShape.ExpandPointOnEdge(myPoints[i - 1], bounds, num);
						PointF b = GoShape.ExpandPointOnEdge(myPoints[i], bounds, num);
						if (GoStroke.NearestIntersectionOnLine(a, b, p1, p2, out result2))
						{
							float num4 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num4 < num2)
							{
								num2 = num4;
								pointF = result2;
							}
						}
						break;
					}
					}
				}
				result = pointF;
				return num2 < 1E+21f;
			}
		}

		/// <summary>
		/// Display the appropriate selected appearance, normally, resize selection handles
		/// at each point of the shape.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// If this shape is resizable and reshapable, we add resize selection
		/// handles at each shape point, with handle IDs equal to
		/// <see cref="F:Northwoods.Go.GoObject.LastHandle" /> plus the index of the point.
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			RemoveSelectionHandles(sel);
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			if (ShowsBoundingHandle || !(CanResize() && flag))
			{
				sel.CreateBoundingHandle(this, selectedObj);
			}
			if (!(CanResize() && flag))
			{
				return;
			}
			if (CanReshape() && flag2)
			{
				if (ReshapablePoints)
				{
					AddPointHandles(sel, selectedObj);
				}
				if (ReshapableRectangle || !ReshapablePoints)
				{
					AddRectangleHandles(sel, selectedObj);
				}
			}
			else
			{
				AddRectangleHandles(sel, selectedObj);
			}
		}

		/// <summary>
		/// Add selection handles for each point of this shape, if this shape is
		/// resizable and reshapable.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public virtual void AddPointHandles(GoSelection sel, GoObject selectedObj)
		{
			checked
			{
				int num = PointsCount - 1;
				int num2 = 0;
				for (int i = 0; i <= num; i++)
				{
					PointF loc = myPoints[i];
					IGoHandle goHandle = sel.CreateResizeHandle(this, selectedObj, loc, 8192 + i, filled: true);
					if ((GetAction(i) & GoDrawingAction.Mask) != GoDrawingAction.CurveTo)
					{
						continue;
					}
					num2++;
					if (num2 > 2)
					{
						num2 = 0;
						continue;
					}
					GoHandle goHandle2 = goHandle.GoObject as GoHandle;
					if (goHandle2 != null)
					{
						goHandle2.Style = GoHandleStyle.Diamond;
						RectangleF a = goHandle2.Bounds;
						GoObject.InflateRect(ref a, a.Width / 12f, a.Height / 12f);
						goHandle2.Bounds = a;
					}
				}
			}
		}

		/// <summary>
		/// Produce a rotated rectangle to surround the shape.
		/// </summary>
		/// <returns></returns>
		public override IGoHandle CreateBoundingHandle()
		{
			if (Angle == 0f)
			{
				return base.CreateBoundingHandle();
			}
			GoHandleRotated goHandleRotated = new GoHandleRotated();
			RectangleF a = UnrotatedBounds;
			GoObject.InflateRect(ref a, 1f, 1f);
			goHandleRotated.Bounds = a;
			goHandleRotated.Angle = Angle;
			return goHandleRotated;
		}

		/// <summary>
		/// Add the standard selection handles, except that the standard four or
		/// eight resize handles are positioned at the rotated corners and sides
		/// of this shape.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.DoResizeRectangleHandles(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />
		public virtual void AddRectangleHandles(GoSelection sel, GoObject selectedObj)
		{
			RotateHelper.AddRectangleHandles(this, UnrotatedBounds, UnrotatedCenter, Angle, sel, selectedObj);
		}

		/// <summary>
		/// When the resize handles are at each point of the shape, the user's
		/// dragging of a resize handle should just change that point in the curve.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> is true, this method always calls
		/// <see cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// Otherwise it only calls <see cref="M:Northwoods.Go.GoDrawing.SetPoint(System.Int32,System.Drawing.PointF)" /> when the <paramref name="evttype" />
		/// is <see cref="F:Northwoods.Go.GoInputState.Finish" /> or <see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (PointsCount != 0)
			{
				if (whichHandle >= 8192)
				{
					DoResizePointHandles(view, origRect, newPoint, whichHandle, evttype, min, max);
				}
				else
				{
					DoResizeRectangleHandles(view, origRect, newPoint, whichHandle, evttype, min, max);
				}
			}
		}

		/// <summary>
		/// This method is called to handle dragging of resize handles for the points of this shape.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public virtual void DoResizePointHandles(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			checked
			{
				int num = whichHandle - 8192;
				if (view.LastInput.Control && evttype == GoInputState.Start)
				{
					int num2 = NextSegmentIndex(num);
					if (num2 == PointsCount || GetAction(num2) == GoDrawingAction.StartAt)
					{
						num2 = PreviousSegmentIndex(num2);
					}
					switch (GetAction(num2) & GoDrawingAction.Mask)
					{
					case GoDrawingAction.LineTo:
						InsertLineTo(num2, GetPoint(num));
						break;
					case GoDrawingAction.CurveTo:
						InsertCurveTo(num2, GetPoint(num), GetPoint(num), GetPoint(num));
						break;
					}
					SetPoint(num, newPoint);
				}
				else if (view.LastInput.Shift)
				{
					if (evttype == GoInputState.Start)
					{
						RemoveSegment(num);
						if (num >= PointsCount || num >= NextFigureIndex(num))
						{
							num = PreviousSegmentIndex(num);
						}
					}
				}
				else
				{
					SetPoint(num, newPoint);
				}
				if (SameEndPoints)
				{
					int num3 = NextFigureIndex(num);
					int num4 = PreviousFigureIndex(num + 1);
					if (num == num3 - 1)
					{
						if (num3 - 1 > num4 + 2)
						{
							SetPoint(num4, newPoint);
						}
					}
					else if (num == num4 && num3 - 1 > num4 + 2)
					{
						SetPoint(num3 - 1, newPoint);
					}
				}
				if (SmoothCurves)
				{
					if (IsSegmentIndex(num))
					{
						SmoothCurveAt(num - 1, 1);
					}
					else if (IsSegmentIndex(num - 1))
					{
						SmoothCurveAt(num + 1, -1);
					}
					else if (IsSegmentIndex(num + 1))
					{
						SmoothCurveAt(num, 0);
					}
				}
			}
		}

		/// <summary>
		/// This method is called to handle resizing the standard eight corner/middle resize handles.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <seealso cref="M:Northwoods.Go.GoDrawing.AddRectangleHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		public virtual void DoResizeRectangleHandles(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			double num = (double)Angle * Math.PI / 180.0;
			double cosine = Math.Cos(num);
			double num2 = Math.Sin(num);
			int spot = SpotOpposite(whichHandle);
			GetRectangleSpotLocation(UnrotatedBounds, spot);
			PointF newPoint2 = RotateHelper.RotatePoint(newPoint, UnrotatedCenter, cosine, 0.0 - num2);
			RectangleF newr = ComputeResize(UnrotatedBounds, newPoint2, whichHandle, min, max, CanReshape() && !view.LastInput.Shift);
			PointF p = new PointF(newr.X + newr.Width / 2f, newr.Y + newr.Height / 2f);
			PointF pointF = RotateHelper.RotatePoint(p, UnrotatedCenter, cosine, num2);
			newr.X += pointF.X - p.X;
			newr.Y += pointF.Y - p.Y;
			GoToolResizing goToolResizing = view.Tool as GoToolResizing;
			if (ResizesRealtime)
			{
				SetUnrotatedBounds(newr, cosine, num2);
				if (goToolResizing != null && base.IsInView)
				{
					if (evttype == GoInputState.Finish || evttype == GoInputState.Cancel)
					{
						RemoveSelectionHandles(view.Selection);
						Remove();
						goToolResizing.CurrentObject = goToolResizing.RealObject;
						goToolResizing.RealObject = null;
					}
					if (evttype == GoInputState.Finish)
					{
						(goToolResizing.CurrentObject as GoDrawing)?.SetUnrotatedBounds(newr, cosine, num2);
					}
				}
			}
			else if (goToolResizing != null && evttype == GoInputState.Start)
			{
				goToolResizing.RealObject = this;
				GoDrawing goDrawing = (GoDrawing)Copy();
				goDrawing.ReshapablePoints = false;
				goDrawing.ReshapableRectangle = true;
				goDrawing.Rotatable = false;
				goDrawing.ResizesRealtime = true;
				goToolResizing.CurrentObject = goDrawing;
				view.Layers.Default.Add(goDrawing);
			}
		}

		/// <summary>
		/// Change the angle of this shape by rotating it around a given axis point.
		/// </summary>
		/// <param name="rotatePoint">the rotation axis point</param>
		/// <param name="angle">the degrees by which to turn; positive is clockwise</param>
		public virtual void Rotate(PointF rotatePoint, float angle)
		{
			if (angle != 0f)
			{
				RectangleF unrotatedBounds = UnrotatedBounds;
				PointF unrotatedCenter = UnrotatedCenter;
				PointF pointF = RotateHelper.RotatePoint(unrotatedCenter, rotatePoint, angle);
				Changing(1154);
				if (rotatePoint != UnrotatedCenter)
				{
					float num = pointF.X - unrotatedCenter.X;
					float num2 = pointF.Y - unrotatedCenter.Y;
					GoStroke.TranslatePoints(myUnrotatedPoints, num, num2);
					myUnrotatedBounds.X += num;
					myUnrotatedBounds.Y += num2;
					rotatePoint = UnrotatedCenter;
				}
				setAngle(Angle + angle);
				RotateHelper.RotatePoints(myUnrotatedPoints, myPoints, rotatePoint, Angle);
				base.InvalidBounds = true;
				pointF = RotateHelper.RotatePoint(unrotatedCenter, rotatePoint, angle);
				Changed(1160, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
				Changed(1154, 0, null, unrotatedBounds, 0, null, UnrotatedBounds);
			}
		}

		internal void setAngle(float angle)
		{
			float angle2 = Angle;
			while (angle >= 360f)
			{
				angle -= 360f;
			}
			while (angle < 0f)
			{
				angle += 360f;
			}
			if (angle2 != angle)
			{
				myAngle = angle;
				Changed(1156, 0, null, GoObject.MakeRect(angle2), 0, null, GoObject.MakeRect(angle));
			}
		}

		/// <summary>
		/// This predicate is true if this object is <see cref="P:Northwoods.Go.GoDrawing.Rotatable" />.
		/// </summary>
		/// <returns></returns>
		internal bool CanRotate()
		{
			if (!Rotatable)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1154)
			{
				if (e.IsBeforeChanging)
				{
					PointF[] array = new PointF[PointsCount];
					Array.Copy(myPoints, 0, array, 0, PointsCount);
					byte[] array2 = new byte[PointsCount];
					Array.Copy(myActions, 0, array2, 0, PointsCount);
					e.OldValue = new GoDrawingData(array2, array);
				}
			}
			else
			{
				base.CopyOldValueForUndo(e);
			}
		}

		/// <summary>
		/// Copies state to permit a redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyNewValueForRedo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1154)
			{
				if (!e.IsBeforeChanging)
				{
					PointF[] array = new PointF[PointsCount];
					Array.Copy(myPoints, 0, array, 0, PointsCount);
					byte[] array2 = new byte[PointsCount];
					Array.Copy(myActions, 0, array2, 0, PointsCount);
					e.NewValue = new GoDrawingData(array2, array);
				}
			}
			else
			{
				base.CopyNewValueForRedo(e);
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1001:
			{
				base.ChangeValue(e, undo);
				RectangleF rect = e.GetRect(!undo);
				RectangleF rect2 = e.GetRect(undo);
				if (rect.Width == rect2.Width && rect.Height == rect2.Height)
				{
					float num = rect2.X - rect.X;
					float num2 = rect2.Y - rect.Y;
					GoStroke.TranslatePoints(myPoints, num, num2);
					GoStroke.TranslatePoints(myUnrotatedPoints, num, num2);
					myUnrotatedBounds.X += num;
					myUnrotatedBounds.Y += num2;
				}
				break;
			}
			case 1151:
				if (undo)
				{
					RemovePoint(e.OldInt);
					if ((ToByte(e.NewRect) & 1) == 1)
					{
						CloseFigure(e.NewInt);
					}
				}
				else
				{
					InsertPoint(e.OldInt, ToByte(e.OldRect), e.OldRect.Location);
				}
				break;
			case 1152:
				if (undo)
				{
					if ((ToByte(e.NewRect) & 2) == 2)
					{
						CloseFigure(e.OldInt);
					}
					if ((ToByte(e.NewRect) & 1) == 1)
					{
						CloseFigure(e.NewInt);
					}
					InsertPoint(e.OldInt, ToByte(e.OldRect), e.OldRect.Location);
				}
				else
				{
					RemovePoint(e.OldInt);
				}
				break;
			case 1153:
				if (undo)
				{
					SetPoint(e.OldInt, e.OldRect.Location);
				}
				else
				{
					SetPoint(e.OldInt, e.NewRect.Location);
				}
				break;
			case 1154:
				Data = (GoDrawingData)e.GetValue(undo);
				break;
			case 1155:
				FillMode = (FillMode)e.GetInt(undo);
				break;
			case 1156:
				Angle = e.GetFloat(undo);
				break;
			case 1157:
				Rotatable = (bool)e.GetValue(undo);
				break;
			case 1158:
				RotatesRealtime = (bool)e.GetValue(undo);
				break;
			case 1159:
				RotationPoint = e.GetPoint(undo);
				break;
			case 1160:
				myUnrotatedBounds = e.GetRect(undo);
				break;
			case 1161:
				SmoothCurves = (bool)e.GetValue(undo);
				break;
			case 1162:
				SameEndPoints = (bool)e.GetValue(undo);
				break;
			case 1163:
				ShowsBoundingHandle = (bool)e.GetValue(undo);
				break;
			case 1164:
				ReshapablePoints = (bool)e.GetValue(undo);
				break;
			case 1165:
				ReshapableRectangle = (bool)e.GetValue(undo);
				break;
			case 1166:
				PickMargin = e.GetFloat(undo);
				break;
			case 1167:
				Figure = (GoFigure)e.GetValue(undo);
				break;
			case 1169:
				if ((bool)e.GetValue(undo))
				{
					CloseFigure(e.OldInt);
				}
				else
				{
					OpenFigure(e.OldInt);
				}
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
