using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A four-sided figure in which two of the sides run parrallel
	/// and the others do not.
	/// </summary>
	[Serializable]
	public class GoTrapezoid : GoShape
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTrapezoid.A" /> property.
		/// </summary>
		public const int ChangedPointA = 1460;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTrapezoid.B" /> property.
		/// </summary>
		public const int ChangedPointB = 1461;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTrapezoid.C" /> property.
		/// </summary>
		public const int ChangedPointC = 1462;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTrapezoid.D" /> property.
		/// </summary>
		public const int ChangedPointD = 1463;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedAllPoints = 1464;

		/// <summary>
		/// This <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint is a synonym for <see cref="F:Northwoods.Go.GoTrapezoid.ChangedAllPoints" />.
		/// </summary>
		public const int ChangedMultiplePoints = 1464;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTrapezoid.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 1465;

		/// <summary>
		/// A special handle ID for a handle at Point A.
		/// </summary>
		public const int PointAHandleID = 1034;

		/// <summary>
		/// A special handle ID for a handle at Point B.
		/// </summary>
		public const int PointBHandleID = 1035;

		/// <summary>
		/// A special handle ID for a handle at Point C.
		/// </summary>
		public const int PointCHandleID = 1036;

		/// <summary>
		/// A special handle ID for a handle at Point D.
		/// </summary>
		public const int PointDHandleID = 1037;

		private PointF[] myPoints = new PointF[4];

		private Orientation myOrientation;

		/// <summary>
		/// Gets or sets how to draw the trapezoid, based on whether its
		/// parallel pair of sides run vertically or horizontally.
		/// </summary>
		/// <value>
		/// This defaults to <c>Orientation.Horizontal</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Horizontal)]
		[Description("Whether the prominent pair of verticies point vertically or horizontally")]
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
					Changed(1465, 0, orientation, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						RectangleF bounds = Bounds;
						A = new PointF(bounds.X, bounds.Y);
						B = new PointF(bounds.X + bounds.Width, bounds.Y);
						C = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
						D = new PointF(bounds.X, bounds.Y + bounds.Height);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the first of the four trapezoid's points.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (0, 0).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.B" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.C" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.D" />
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The first point in this trapezoid.")]
		public PointF A
		{
			get
			{
				return myPoints[0];
			}
			set
			{
				PointF pointF = myPoints[0];
				if (!(pointF != value))
				{
					return;
				}
				myPoints[0] = value;
				ResetPath();
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1460, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				if (base.Initializing)
				{
					return;
				}
				PointF pointF2 = default(PointF);
				if (Orientation == Orientation.Horizontal)
				{
					pointF2 = B;
					if (myPoints[0].X > B.X)
					{
						pointF2.X = myPoints[0].X;
					}
					pointF2.Y = myPoints[0].Y;
					B = pointF2;
				}
				else
				{
					pointF2 = D;
					if (myPoints[0].Y > D.Y)
					{
						pointF2.Y = myPoints[0].Y;
					}
					pointF2.X = myPoints[0].X;
					D = pointF2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the second of the four trapezoid's points.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (8, 0).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.A" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.C" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.D" />
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The second point in this trapezoid.")]
		public PointF B
		{
			get
			{
				return myPoints[1];
			}
			set
			{
				PointF pointF = myPoints[1];
				if (!(pointF != value))
				{
					return;
				}
				myPoints[1] = value;
				ResetPath();
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1461, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				if (base.Initializing)
				{
					return;
				}
				PointF pointF2 = default(PointF);
				if (Orientation == Orientation.Horizontal)
				{
					pointF2 = A;
					if (myPoints[1].X < A.X)
					{
						pointF2.X = myPoints[1].X;
					}
					pointF2.Y = myPoints[1].Y;
					A = pointF2;
				}
				else
				{
					pointF2 = C;
					if (myPoints[1].Y > C.Y)
					{
						pointF2.Y = myPoints[1].Y;
					}
					pointF2.X = myPoints[1].X;
					C = pointF2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the third of the four trapezoid's points.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (10, 10).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.A" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.B" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.D" />
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The third point in this trapezoid.")]
		public PointF C
		{
			get
			{
				return myPoints[2];
			}
			set
			{
				PointF pointF = myPoints[2];
				if (!(pointF != value))
				{
					return;
				}
				myPoints[2] = value;
				ResetPath();
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1462, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				if (base.Initializing)
				{
					return;
				}
				PointF pointF2 = default(PointF);
				if (Orientation == Orientation.Horizontal)
				{
					pointF2 = D;
					if (myPoints[2].X < D.X)
					{
						pointF2.X = myPoints[2].X;
					}
					pointF2.Y = myPoints[2].Y;
					D = pointF2;
				}
				else
				{
					pointF2 = B;
					if (myPoints[2].Y < B.Y)
					{
						pointF2.Y = myPoints[2].Y;
					}
					pointF2.X = myPoints[2].X;
					B = pointF2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the fourth of the four trapezoid's points;
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (0, 10).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.A" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.B" />
		/// <seealso cref="P:Northwoods.Go.GoTrapezoid.C" />
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The fourth point in this trapezoid.")]
		public PointF D
		{
			get
			{
				return myPoints[3];
			}
			set
			{
				PointF pointF = myPoints[3];
				if (!(pointF != value))
				{
					return;
				}
				myPoints[3] = value;
				ResetPath();
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1463, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				if (base.Initializing)
				{
					return;
				}
				PointF pointF2 = default(PointF);
				if (Orientation == Orientation.Horizontal)
				{
					pointF2 = C;
					if (myPoints[3].X > C.X)
					{
						pointF2.X = myPoints[3].X;
					}
					pointF2.Y = myPoints[3].Y;
					C = pointF2;
				}
				else
				{
					pointF2 = A;
					if (myPoints[3].Y < A.Y)
					{
						pointF2.Y = myPoints[3].Y;
					}
					pointF2.X = myPoints[3].X;
					A = pointF2;
				}
			}
		}

		/// <summary>
		/// The constructor produces a trapazoid with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline, no <see cref="T:System.Drawing.Brush" /> fill.
		/// </summary>
		public GoTrapezoid()
		{
			base.InternalFlags |= 512;
			myPoints[1] = new PointF(8f, 0f);
			myPoints[2] = new PointF(10f, 10f);
			myPoints[3] = new PointF(0f, 10f);
		}

		/// <summary>
		/// Make sure the cloned trapezoid does not share any internal data references with the
		/// original one.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoTrapezoid goTrapezoid = (GoTrapezoid)base.CopyObject(env);
			if (goTrapezoid != null)
			{
				goTrapezoid.myPoints = (PointF[])myPoints.Clone();
			}
			return goTrapezoid;
		}

		/// <summary>
		/// Calculate the minimum rectangle that includes all four points.
		/// </summary>
		/// <returns></returns>
		protected override RectangleF ComputeBounds()
		{
			float x = myPoints[0].X;
			float num = x;
			if (myPoints[1].X < x)
			{
				x = myPoints[1].X;
			}
			else if (myPoints[1].X > num)
			{
				num = myPoints[1].X;
			}
			if (myPoints[2].X < x)
			{
				x = myPoints[2].X;
			}
			else if (myPoints[2].X > num)
			{
				num = myPoints[2].X;
			}
			if (myPoints[3].X < x)
			{
				x = myPoints[3].X;
			}
			else if (myPoints[3].X > num)
			{
				num = myPoints[3].X;
			}
			float y = myPoints[0].Y;
			float num2 = y;
			if (myPoints[1].Y < y)
			{
				y = myPoints[1].Y;
			}
			else if (myPoints[1].Y > num2)
			{
				num2 = myPoints[1].Y;
			}
			if (myPoints[2].Y < y)
			{
				y = myPoints[2].Y;
			}
			else if (myPoints[2].Y > num2)
			{
				num2 = myPoints[2].Y;
			}
			if (myPoints[3].Y < y)
			{
				y = myPoints[3].Y;
			}
			else if (myPoints[3].Y > num2)
			{
				num2 = myPoints[3].Y;
			}
			return new RectangleF(x, y, num - x, num2 - y);
		}

		/// <summary>
		/// When the bounds change, update the points appropriately.
		/// </summary>
		/// <param name="old"></param>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			RectangleF bounds = Bounds;
			if (old.Width == bounds.Width && old.Height == bounds.Height)
			{
				float num = bounds.X - old.X;
				float num2 = bounds.Y - old.Y;
				if (num != 0f || num2 != 0f)
				{
					GoStroke.TranslatePoints(myPoints, num, num2);
					base.InvalidBounds = false;
				}
			}
			else
			{
				Changing(1464);
				GoStroke.RescalePoints(myPoints, old, bounds);
				base.InvalidBounds = false;
				Changed(1464, 0, null, old, 0, null, bounds);
			}
		}

		/// <summary>
		/// Reset all of the trapezoid's points.
		/// </summary>
		/// <param name="points">an array of <c>PointF</c> of length 4</param>
		/// <remarks>
		/// The first element of the array corresponds to point <see cref="P:Northwoods.Go.GoTrapezoid.A" />,
		/// the second to point <see cref="P:Northwoods.Go.GoTrapezoid.B" />, the third to <see cref="P:Northwoods.Go.GoTrapezoid.C" />,
		/// and the fourth to <see cref="P:Northwoods.Go.GoTrapezoid.D" />.
		/// </remarks>
		public virtual void SetPoints(PointF[] points)
		{
			if (points == null || points.Length != 4)
			{
				throw new ArgumentException("Trapezoids always have four points");
			}
			if (points[0] != myPoints[0] || points[1] != myPoints[1] || points[2] != myPoints[2] || points[3] != myPoints[3])
			{
				Changing(1464);
				ResetPath();
				Array.Copy(points, 0, myPoints, 0, 4);
				base.InvalidBounds = true;
				Changed(1464, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Consider the actual shape of the trapezoid to determine
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
		/// The closest point of a trapezoid that intersects with a given line
		/// is the closest such point of each of its four line segments.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float shift = PenWidth / 2f;
			PointF pointF = GoShape.ExpandPointOnEdge(myPoints[0], bounds, shift);
			PointF pointF2 = GoShape.ExpandPointOnEdge(myPoints[1], bounds, shift);
			PointF pointF3 = GoShape.ExpandPointOnEdge(myPoints[2], bounds, shift);
			PointF pointF4 = GoShape.ExpandPointOnEdge(myPoints[3], bounds, shift);
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF5 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF3, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF4, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF4, pointF, p1, p2, out result2))
			{
				float num5 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num5 < num)
				{
					num = num5;
					pointF5 = result2;
				}
			}
			result = pointF5;
			return num < 1E+21f;
		}

		/// <summary>
		/// Paint a possibly shadowed trapezoid.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, myPoints);
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> by adding lines between the points.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			graphicsPath.AddLines(myPoints);
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		/// <summary>
		/// If reshapeable, ensures that the set pair of sides maintains
		/// their perpecidularity when the figure is altered
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> is true, this method always sets
		/// the corresponding point property to <paramref name="newPoint" />.
		/// Otherwise it only sets it when the <paramref name="evttype" />
		/// is <see cref="F:Northwoods.Go.GoInputState.Finish" /> or <see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			bool flag = Orientation == Orientation.Horizontal;
			if (whichHandle >= 1034 && whichHandle <= 1037 && CanReshape() && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				PointF a = A;
				PointF b = B;
				PointF c = C;
				PointF d = D;
				switch (whichHandle)
				{
				case 1034:
					a = newPoint;
					if (flag)
					{
						if (a.X > b.X)
						{
							a.X = b.X;
						}
					}
					else if (a.Y > d.Y)
					{
						a.Y = d.Y;
					}
					A = a;
					break;
				case 1035:
					b = newPoint;
					if (flag)
					{
						if (b.X < a.X)
						{
							b.X = a.X;
						}
					}
					else if (b.Y > c.Y)
					{
						b.Y = c.Y;
					}
					B = b;
					break;
				case 1036:
					c = newPoint;
					if (flag)
					{
						if (c.X < d.X)
						{
							c.X = d.X;
						}
					}
					else if (c.Y < b.Y)
					{
						c.Y = b.Y;
					}
					C = c;
					break;
				case 1037:
					d = newPoint;
					if (flag)
					{
						if (d.X > c.X)
						{
							d.X = c.X;
						}
					}
					else if (d.Y < a.Y)
					{
						d.Y = a.Y;
					}
					D = d;
					break;
				}
			}
			else if (flag && (whichHandle == 256 || whichHandle == 64) && CanReshape() && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				PointF pointF = default(PointF);
				PointF pointF2 = default(PointF);
				PointF pointF3 = default(PointF);
				PointF pointF4 = default(PointF);
				bool flag2 = A.X <= B.X;
				bool flag3 = true;
				switch (whichHandle)
				{
				case 256:
					if (flag2)
					{
						pointF = A;
						pointF2 = D;
						pointF3 = B;
						pointF4 = C;
					}
					else
					{
						pointF = B;
						pointF2 = C;
						pointF3 = A;
						pointF4 = D;
					}
					flag3 = true;
					break;
				case 64:
					if (flag2)
					{
						pointF = B;
						pointF2 = C;
						pointF3 = A;
						pointF4 = D;
					}
					else
					{
						pointF = A;
						pointF2 = D;
						pointF3 = B;
						pointF4 = C;
					}
					flag3 = false;
					break;
				}
				float num = pointF.X - pointF2.X;
				pointF.X = newPoint.X + num / 2f;
				pointF2.X = newPoint.X - num / 2f;
				if (flag3)
				{
					if (pointF.X > pointF3.X)
					{
						pointF.X = pointF3.X;
						pointF2.X = pointF.X - num;
					}
					if (pointF2.X > pointF4.X)
					{
						pointF2.X = pointF4.X;
						pointF.X = pointF2.X + num;
					}
				}
				else
				{
					if (pointF.X <= pointF3.X)
					{
						pointF.X = pointF3.X;
						pointF2.X = pointF.X - num;
					}
					if (pointF2.X < pointF4.X)
					{
						pointF2.X = pointF4.X;
						pointF.X = pointF2.X + num;
					}
				}
				if (flag3)
				{
					if (flag2)
					{
						A = pointF;
						D = pointF2;
					}
					else
					{
						B = pointF;
						C = pointF2;
					}
				}
				else if (flag2)
				{
					B = pointF;
					C = pointF2;
				}
				else
				{
					A = pointF;
					D = pointF2;
				}
			}
			else if (!flag && (whichHandle == 32 || whichHandle == 128) && CanReshape() && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				PointF pointF5 = default(PointF);
				PointF pointF6 = default(PointF);
				PointF pointF7 = default(PointF);
				PointF pointF8 = default(PointF);
				bool flag4 = A.Y <= D.Y;
				bool flag5 = true;
				switch (whichHandle)
				{
				case 32:
					if (flag4)
					{
						pointF5 = A;
						pointF6 = B;
						pointF7 = D;
						pointF8 = C;
					}
					else
					{
						pointF5 = D;
						pointF6 = C;
						pointF7 = A;
						pointF8 = B;
					}
					flag5 = true;
					break;
				case 128:
					if (flag4)
					{
						pointF5 = D;
						pointF6 = C;
						pointF7 = A;
						pointF8 = B;
					}
					else
					{
						pointF5 = A;
						pointF6 = B;
						pointF7 = D;
						pointF8 = C;
					}
					flag5 = false;
					break;
				}
				float num2 = pointF5.Y - pointF6.Y;
				pointF5.Y = newPoint.Y + num2 / 2f;
				pointF6.Y = newPoint.Y - num2 / 2f;
				if (flag5)
				{
					if (pointF5.Y > pointF7.Y)
					{
						pointF5.Y = pointF7.Y;
						pointF6.Y = pointF5.Y - num2;
					}
					if (pointF6.Y > pointF8.Y)
					{
						pointF6.Y = pointF8.Y;
						pointF5.Y = pointF6.Y + num2;
					}
				}
				else
				{
					if (pointF5.Y < pointF7.Y)
					{
						pointF5.Y = pointF7.Y;
						pointF6.Y = pointF5.Y - num2;
					}
					if (pointF6.Y < pointF8.Y)
					{
						pointF6.Y = pointF8.Y;
						pointF5.Y = pointF6.Y + num2;
					}
				}
				if (flag5)
				{
					if (flag4)
					{
						A = pointF5;
						B = pointF6;
					}
					else
					{
						D = pointF5;
						C = pointF6;
					}
				}
				else if (flag4)
				{
					D = pointF5;
					C = pointF6;
				}
				else
				{
					A = pointF5;
					B = pointF6;
				}
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// Calculate a new bounding rectangle for this object.
		/// </summary>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="handle"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="reshape"></param>
		/// <returns>
		/// A new <c>RectangleF</c> bounding rectangle, in document coordinates.
		/// </returns>
		/// <remarks>
		/// This is normally called from <see cref="M:Northwoods.Go.GoTrapezoid.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />.
		/// </remarks>
		public override RectangleF ComputeResize(RectangleF origRect, PointF newPoint, int handle, SizeF min, SizeF max, bool reshape)
		{
			if (handle <= 16)
			{
				return base.ComputeResize(origRect, newPoint, handle, min, max, reshape);
			}
			float x = origRect.X;
			float y = origRect.Y;
			float num = origRect.X + origRect.Width;
			float num2 = origRect.Y + origRect.Height;
			RectangleF result = origRect;
			switch (handle)
			{
			case 32:
			{
				float num3 = (Orientation != 0) ? (Math.Abs(A.Y - B.Y) / 2f) : 0f;
				result.Y = Math.Max(newPoint.Y - num3, num2 - max.Height);
				result.Y = Math.Min(result.Y, num2 - min.Height);
				result.Height = num2 - result.Y;
				if (result.Height <= 0f)
				{
					result.Height = 1f;
				}
				break;
			}
			case 256:
			{
				float num3 = (Orientation != 0) ? 0f : (Math.Abs(A.X - D.X) / 2f);
				result.X = Math.Max(newPoint.X - num3, num - max.Width);
				result.X = Math.Min(result.X, num - min.Width);
				result.Width = num - result.X;
				if (result.Width <= 0f)
				{
					result.Width = 1f;
				}
				break;
			}
			case 64:
			{
				float num3 = (Orientation != 0) ? 0f : (Math.Abs(B.X - C.X) / 2f);
				result.Width = Math.Min(newPoint.X + num3 - x, max.Width);
				result.Width = Math.Max(result.Width, min.Width);
				break;
			}
			case 128:
				if (Orientation == Orientation.Horizontal)
				{
					float num3 = 0f;
				}
				else
				{
					float num3 = Math.Abs(C.Y - D.Y) / 2f;
				}
				result.Height = Math.Min(newPoint.Y - y, max.Height);
				result.Height = Math.Max(result.Height, min.Height);
				break;
			}
			return result;
		}

		/// <summary>
		/// If Resizable and Reshapable, support individual triangle vertex
		/// resize handles.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			if (!CanResize() || !CanReshape())
			{
				base.AddSelectionHandles(sel, selectedObj);
				return;
			}
			PointF pointF = default(PointF);
			PointF pointF2 = default(PointF);
			PointF pointF3 = default(PointF);
			PointF pointF4 = default(PointF);
			bool flag = A.X <= B.X;
			bool flag2 = A.Y <= D.Y;
			sel.RemoveHandles(this);
			if (flag && flag2)
			{
				pointF = A;
				pointF2 = B;
				pointF3 = D;
				pointF4 = C;
			}
			else if (!flag && flag2)
			{
				pointF = B;
				pointF2 = A;
				pointF3 = C;
				pointF4 = D;
			}
			else if (flag && !flag2)
			{
				pointF = D;
				pointF2 = C;
				pointF3 = A;
				pointF4 = B;
			}
			else
			{
				pointF = C;
				pointF2 = D;
				pointF3 = B;
				pointF4 = A;
			}
			PointF loc = new PointF((pointF.X + pointF3.X) / 2f, (pointF.Y + pointF3.Y) / 2f);
			sel.CreateResizeHandle(this, selectedObj, loc, 256, filled: true);
			loc = new PointF((pointF2.X + pointF4.X) / 2f, (pointF2.Y + pointF4.Y) / 2f);
			sel.CreateResizeHandle(this, selectedObj, loc, 64, filled: true);
			loc = new PointF((pointF.X + pointF2.X) / 2f, (pointF.Y + pointF2.Y) / 2f);
			sel.CreateResizeHandle(this, selectedObj, loc, 32, filled: true);
			loc = new PointF((pointF3.X + pointF4.X) / 2f, (pointF3.Y + pointF4.Y) / 2f);
			sel.CreateResizeHandle(this, selectedObj, loc, 128, filled: true);
			sel.CreateResizeHandle(this, selectedObj, A, 1034, filled: true);
			sel.CreateResizeHandle(this, selectedObj, B, 1035, filled: true);
			sel.CreateResizeHandle(this, selectedObj, C, 1036, filled: true);
			sel.CreateResizeHandle(this, selectedObj, D, 1037, filled: true);
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1464)
			{
				if (e.IsBeforeChanging)
				{
					e.OldValue = myPoints.Clone();
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
			if (subHint == 1464)
			{
				if (!e.IsBeforeChanging)
				{
					e.NewValue = myPoints.Clone();
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
					float dx = rect2.X - rect.X;
					float dy = rect2.Y - rect.Y;
					GoStroke.TranslatePoints(myPoints, dx, dy);
				}
				break;
			}
			case 1460:
				A = e.GetPoint(undo);
				break;
			case 1461:
				B = e.GetPoint(undo);
				break;
			case 1462:
				C = e.GetPoint(undo);
				break;
			case 1463:
				D = e.GetPoint(undo);
				break;
			case 1464:
			{
				PointF[] array = (PointF[])e.GetValue(undo);
				if (array != null)
				{
					SetPoints(array);
				}
				break;
			}
			case 1465:
				Orientation = (Orientation)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
