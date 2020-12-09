using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// A shape with three straight sides.
	/// </summary>
	[Serializable]
	public class GoTriangle : GoShape
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTriangle.A" /> property.
		/// </summary>
		public const int ChangedPointA = 1431;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTriangle.B" /> property.
		/// </summary>
		public const int ChangedPointB = 1432;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoTriangle.C" /> property.
		/// </summary>
		public const int ChangedPointC = 1433;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedAllPoints = 1434;

		private PointF[] myPoints = new PointF[3];

		/// <summary>
		/// Gets or sets this triangle's first point.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (0, 0).
		/// </value>
		/// <remarks>
		/// <see cref="P:Northwoods.Go.GoTriangle.B" />.
		/// <see cref="P:Northwoods.Go.GoTriangle.C" />.
		/// </remarks>
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The first of three points in this triangle.")]
		public PointF A
		{
			get
			{
				return myPoints[0];
			}
			set
			{
				PointF pointF = myPoints[0];
				if (pointF != value)
				{
					ResetPath();
					myPoints[0] = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1431, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets this triangle's second point.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (10, 0).
		/// </value>
		/// <remarks>
		/// <see cref="P:Northwoods.Go.GoTriangle.A" />.
		/// <see cref="P:Northwoods.Go.GoTriangle.C" />.
		/// </remarks>
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The second of three points in this triangle.")]
		public PointF B
		{
			get
			{
				return myPoints[1];
			}
			set
			{
				PointF pointF = myPoints[1];
				if (pointF != value)
				{
					ResetPath();
					myPoints[1] = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1432, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets this triangle's third point.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// The initial value is (5, 10).
		/// </value>
		/// <remarks>
		/// <see cref="P:Northwoods.Go.GoTriangle.A" />.
		/// <see cref="P:Northwoods.Go.GoTriangle.B" />.
		/// </remarks>
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The third of three points in this triangle.")]
		public PointF C
		{
			get
			{
				return myPoints[2];
			}
			set
			{
				PointF pointF = myPoints[2];
				if (pointF != value)
				{
					ResetPath();
					myPoints[2] = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1433, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// The constructor produces a triangle that the user cannot resize in realtime.
		/// </summary>
		public GoTriangle()
		{
			base.InternalFlags |= 512;
			myPoints[1] = new PointF(10f, 0f);
			myPoints[2] = new PointF(5f, 10f);
		}

		/// <summary>
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoTriangle goTriangle = (GoTriangle)base.CopyObject(env);
			if (goTriangle != null)
			{
				goTriangle.myPoints = (PointF[])myPoints.Clone();
			}
			return goTriangle;
		}

		/// <summary>
		/// Calculate the minimum rectangle that includes all three points.
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
				Changing(1434);
				GoStroke.RescalePoints(myPoints, old, bounds);
				base.InvalidBounds = false;
				Changed(1434, 0, null, old, 0, null, bounds);
			}
		}

		/// <summary>
		/// Paint a triangle, possibly shadowed.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, myPoints);
		}

		/// <summary>
		/// Consider the actual shape of the triangle to determine
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
		/// The closest point of a triangle that intersects with a given line
		/// is the closest such point of each of its three line segments.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF bounds = Bounds;
			float shift = PenWidth / 2f;
			PointF pointF = GoShape.ExpandPointOnEdge(A, bounds, shift);
			PointF pointF2 = GoShape.ExpandPointOnEdge(B, bounds, shift);
			PointF pointF3 = GoShape.ExpandPointOnEdge(C, bounds, shift);
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF4 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF4 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF3, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF4 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF4 = result2;
				}
			}
			result = pointF4;
			return num < 1E+21f;
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
		/// Support either allowing the user to move the triangle vertices around
		/// individually, or treating the triangle as a whole object.
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
			if (whichHandle >= 8192 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				switch (whichHandle)
				{
				case 8192:
					A = newPoint;
					break;
				case 8193:
					B = newPoint;
					break;
				case 8194:
					C = newPoint;
					break;
				}
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
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
			sel.RemoveHandles(this);
			sel.CreateResizeHandle(this, selectedObj, A, 8192, filled: true);
			sel.CreateResizeHandle(this, selectedObj, B, 8193, filled: true);
			sel.CreateResizeHandle(this, selectedObj, C, 8194, filled: true);
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1434)
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
			if (subHint == 1434)
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
			case 1431:
				A = e.GetPoint(undo);
				break;
			case 1432:
				B = e.GetPoint(undo);
				break;
			case 1433:
				C = e.GetPoint(undo);
				break;
			case 1434:
			{
				PointF[] array = (PointF[])e.GetValue(undo);
				if (array != null)
				{
					ResetPath();
					myPoints = array;
				}
				break;
			}
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
