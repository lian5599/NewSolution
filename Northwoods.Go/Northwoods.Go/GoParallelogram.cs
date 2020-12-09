using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of a parallelogram.
	/// </summary>
	[Serializable]
	public class GoParallelogram : GoShape
	{
		private const int flagReshapableSkew = 1048576;

		private const int flagDirection = 2097152;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoParallelogram.Skew" /> property.
		/// </summary>
		public const int ChangedSkew = 1466;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoParallelogram.ReshapableSkew" /> property.
		/// </summary>
		public const int ChangedReshapableSkew = 1467;

		/// <summary>
		/// A special handle ID for a handle which controls the skew length of a parallelogram.
		/// </summary>
		public const int SkewHandleID = 1038;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoParallelogram.Direction" /> property.
		/// </summary>
		public const int ChangedDirection = 1468;

		private SizeF mySkew = new SizeF(10f, 0f);

		private PointF[] myPoints = new PointF[4];

		/// <summary>
		/// Gets or sets the direction of the fixed diagonal.
		/// True is Top Right to Bottom Left; False is Bottom Right to Top Left.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Determines the direction of the fixed diagonal.")]
		public virtual bool Direction
		{
			get
			{
				return (base.InternalFlags & 0x200000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x200000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 2097152;
					}
					else
					{
						base.InternalFlags &= -2097153;
					}
					Changed(1468, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the tiltedness of the parallelogram, in both directions.
		/// </summary>
		/// <value>
		/// By default the value is 10x0, so that the top-left corner is shifted to
		/// the right by 10 document units but kept at the same Y coordinate.
		/// Similar shifts occur along the Y axis according to the skew height.
		/// If given a negative value, it is set to 0.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The tiltedness of the parallelogram")]
		public SizeF Skew
		{
			get
			{
				return mySkew;
			}
			set
			{
				SizeF sizeF = mySkew;
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
					mySkew = value;
					ResetPath();
					Changed(1466, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the skew reshaping handle.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can reshape the skew of this resizable object.")]
		public virtual bool ReshapableSkew
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
					Changed(1467, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a parallelogramwith a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline and no <see cref="T:System.Drawing.Brush" /> fill, whose "tiltedness" is specified by <see cref="P:Northwoods.Go.GoParallelogram.Skew" />.
		/// </summary>
		public GoParallelogram()
		{
			base.InternalFlags |= 3146240;
		}

		/// <summary>
		/// Make sure any internal state is copied correctly.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoParallelogram goParallelogram = (GoParallelogram)base.CopyObject(env);
			if (goParallelogram != null)
			{
				goParallelogram.myPoints = (PointF[])myPoints.Clone();
			}
			return goParallelogram;
		}

		/// <summary>
		/// Paint a parallelogram shape, possibly with a shadow.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, getPoints());
		}

		/// <summary>
		/// Consider the sides of the parallelogram when determining if a point is inside.
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
			float x = bounds.X;
			float y = bounds.Y;
			float num = bounds.X + bounds.Width;
			float num2 = bounds.Y + bounds.Height;
			SizeF skew = Skew;
			bool direction = Direction;
			float num3 = Math.Min(skew.Width, bounds.Width);
			float num4 = Math.Min(skew.Height, bounds.Height);
			myPoints[0].X = x + (direction ? num3 : 0f);
			myPoints[0].Y = y + (direction ? num4 : 0f);
			myPoints[1].X = num - (direction ? 0f : num3);
			myPoints[1].Y = y + (direction ? 0f : num4);
			myPoints[2].X = num - (direction ? num3 : 0f);
			myPoints[2].Y = num2 - (direction ? num4 : 0f);
			myPoints[3].X = x + (direction ? 0f : num3);
			myPoints[3].Y = num2 - (direction ? 0f : num4);
			return myPoints;
		}

		/// <summary>
		/// Support either allowing the user to move the skew control handle,
		/// or treating the parallelogram as a whole object.
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
			if (whichHandle == 1038 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				RectangleF bounds = Bounds;
				SizeF skew = Skew;
				if (Direction)
				{
					if (newPoint.X < bounds.X)
					{
						skew.Width = 0f;
					}
					else if (newPoint.X >= bounds.X + bounds.Width)
					{
						skew.Width = bounds.Width;
					}
					else
					{
						skew.Width = newPoint.X - bounds.X;
					}
				}
				else if (newPoint.X >= bounds.X + bounds.Width)
				{
					skew.Width = 0f;
				}
				else if (newPoint.X < bounds.X)
				{
					skew.Width = bounds.Width;
				}
				else
				{
					skew.Width = bounds.X + bounds.Width - newPoint.X;
				}
				if (newPoint.Y < bounds.Y)
				{
					skew.Height = 0f;
				}
				else if (newPoint.Y >= bounds.Y + bounds.Height)
				{
					skew.Height = bounds.Height;
				}
				else
				{
					skew.Height = newPoint.Y - bounds.Y;
				}
				Skew = skew;
				ResetPath();
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
				ResetPath();
			}
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> and <see cref="P:Northwoods.Go.GoParallelogram.ReshapableSkew" />
		/// are true, this supports a skew control handle.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape() && ReshapableSkew)
			{
				RectangleF bounds = Bounds;
				SizeF skew = Skew;
				PointF pointF = default(PointF);
				pointF = (Direction ? new PointF(bounds.X + skew.Width, bounds.Y + skew.Height) : new PointF(bounds.X + bounds.Width - skew.Width, bounds.Y + skew.Height));
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, pointF, 1038, filled: true);
				MakeDiamondResizeHandle(handle, 1);
			}
		}

		/// <summary>
		/// The closest point of a parallelogram that intersects with a given line
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
			PointF[] points = getPoints();
			PointF pointF = GoShape.ExpandPointOnEdge(points[0], bounds, shift);
			PointF pointF2 = GoShape.ExpandPointOnEdge(points[1], bounds, shift);
			PointF pointF3 = GoShape.ExpandPointOnEdge(points[2], bounds, shift);
			PointF pointF4 = GoShape.ExpandPointOnEdge(points[3], bounds, shift);
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
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1466:
				Skew = e.GetSize(undo);
				break;
			case 1467:
				ReshapableSkew = (bool)e.GetValue(undo);
				break;
			case 1468:
				Direction = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				ResetPath();
				break;
			}
		}
	}
}
