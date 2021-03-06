using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of an elliptical curve.
	/// </summary>
	[Serializable]
	public class GoArc : GoShape
	{
		private const int flagResizableStartAngle = 1048576;

		private const int flagResizableEndAngle = 2097152;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoArc.StartAngle" /> property.
		/// </summary>
		public const int ChangedStartAngle = 1471;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoArc.SweepAngle" /> property.
		/// </summary>
		public const int ChangedSweepAngle = 1472;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoArc.ResizableStartAngle" /> property.
		/// </summary>
		public const int ChangedResizableStartAngle = 1473;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoArc.ResizableEndAngle" /> property.
		/// </summary>
		public const int ChangedResizableEndAngle = 1474;

		/// <summary>
		/// A special handle ID for a handle which controls the start angle of the arc.
		/// </summary>
		public const int StartAngleHandleID = 1044;

		/// <summary>
		/// A special handle ID for a handle which controls the end angle of the arc.
		/// </summary>
		public const int EndAngleHandleID = 1045;

		private float myStartAngle;

		private float mySweepAngle = 300f;

		/// <summary>
		/// Gets or sets the initial angle of the section of the ellipse to be drawn.
		/// </summary>
		/// <value>
		/// This value is in degrees, measured clockwise from zero along the positive X axis.
		/// The value should range from zero to just below 360.  Values outside this range
		/// are adjusted to equivalent angles that fall in this range.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The start angle for the side of the arc")]
		public float StartAngle
		{
			get
			{
				return myStartAngle;
			}
			set
			{
				float num = myStartAngle;
				if (value < 0f)
				{
					value = 360f - (0f - value) % 360f;
				}
				else if (value >= 360f)
				{
					value %= 360f;
				}
				if (num != value)
				{
					myStartAngle = value;
					ResetPath();
					Changed(1471, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle of the width of the section of the ellipse to be drawn.
		/// </summary>
		/// <value>
		/// This value is in degrees, measured clockwise from the <see cref="P:Northwoods.Go.GoArc.StartAngle" />.
		/// Absolute values equal to or greater than 360 degrees are adjusted to the equivalent
		/// angles less than 360 degrees.
		/// The default value is 300.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(300)]
		[Description("The sweep angle for the body of the arc")]
		public float SweepAngle
		{
			get
			{
				return mySweepAngle;
			}
			set
			{
				float num = mySweepAngle;
				if (value > 360f || value < -360f)
				{
					value %= 360f;
				}
				if (num != value)
				{
					mySweepAngle = value;
					ResetPath();
					Changed(1472, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the resizing handle controlling the start angle.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can resize the start angle of this resizable object.")]
		public virtual bool ResizableStartAngle
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
					Changed(1473, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to add the resizing handle controlling the end angle.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can resize the end angle of this resizable object.")]
		public virtual bool ResizableEndAngle
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
					Changed(1474, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces an arc with a standard black <see cref="P:Northwoods.Go.GoShape.Pen" />
		/// outline and no <see cref="T:System.Drawing.Brush" /> fill.
		/// </summary>
		public GoArc()
		{
			base.InternalFlags |= 3146240;
		}

		/// <summary>
		/// Draw a possibly shadowed arc.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			float startAngle = StartAngle;
			float sweepAngle = SweepAngle;
			RectangleF bounds = Bounds;
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (Pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					GoShape.DrawArc(g, view, shadowPen, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height, startAngle, sweepAngle);
				}
			}
			GoShape.DrawArc(g, view, Pen, bounds.X, bounds.Y, bounds.Width, bounds.Height, startAngle, sweepAngle);
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> by adding an arc.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			RectangleF bounds = Bounds;
			if (bounds.Width > 0f && bounds.Height > 0f)
			{
				graphicsPath.AddArc(bounds.X, bounds.Y, bounds.Width, bounds.Height, StartAngle, SweepAngle);
			}
			return graphicsPath;
		}

		/// <summary>
		/// Support allowing the user to move the angle control handles.
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
			if ((whichHandle == 1044 || whichHandle == 1045) && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				switch (whichHandle)
				{
				case 1044:
				{
					RectangleF bounds2 = Bounds;
					float num6 = bounds2.Width / 2f;
					float num7 = bounds2.Height / 2f;
					float num8 = bounds2.X + num6;
					float num9 = bounds2.Y + num7;
					float angle = GoStroke.GetAngle(newPoint.X - num8, newPoint.Y - num9);
					float num10 = SweepAngle - (angle - StartAngle);
					if (SweepAngle >= 0f)
					{
						if (num10 < 0f)
						{
							num10 += 360f;
						}
					}
					else if (num10 >= 0f)
					{
						num10 -= 360f;
					}
					SweepAngle = num10;
					StartAngle = angle;
					break;
				}
				case 1045:
				{
					RectangleF bounds = Bounds;
					float num = bounds.Width / 2f;
					float num2 = bounds.Height / 2f;
					float num3 = bounds.X + num;
					float num4 = bounds.Y + num2;
					float num5 = GoStroke.GetAngle(newPoint.X - num3, newPoint.Y - num4) - StartAngle;
					if (SweepAngle >= 0f)
					{
						if (num5 < 0f)
						{
							num5 += 360f;
						}
					}
					else if (num5 >= 0f)
					{
						num5 -= 360f;
					}
					SweepAngle = num5;
					break;
				}
				}
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> is true, supports angle control handles if
		/// <see cref="P:Northwoods.Go.GoArc.ResizableStartAngle" /> and/or <see cref="P:Northwoods.Go.GoArc.ResizableEndAngle" /> are true.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (CanReshape())
			{
				if (ResizableStartAngle)
				{
					PointF pointAtAngle = GetPointAtAngle(StartAngle);
					IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, pointAtAngle, 1044, filled: true);
					MakeDiamondResizeHandle(handle, 1);
				}
				if (ResizableEndAngle)
				{
					PointF pointAtAngle2 = GetPointAtAngle(StartAngle + SweepAngle);
					IGoHandle handle2 = sel.CreateResizeHandle(this, selectedObj, pointAtAngle2, 1045, filled: true);
					MakeDiamondResizeHandle(handle2, 1);
				}
			}
		}

		internal PointF GetPointAtAngle(float ang)
		{
			RectangleF bounds = Bounds;
			float num = bounds.Width / 2f;
			float num2 = bounds.Height / 2f;
			float num3 = bounds.X + num;
			float num4 = bounds.Y + num2;
			if (num == 0f)
			{
				return new PointF(num3, num4);
			}
			float num5 = (float)Math.Cos((double)(ang / 180f) * Math.PI);
			float num6 = 1f - num2 * num2 / (num * num);
			float num7 = (float)((double)num * Math.Sqrt((1f - num6) / (1f - num6 * num5 * num5))) * num5;
			return new PointF(num3 + num7, (float)((double)num4 + Math.Tan((double)(ang / 180f) * Math.PI) * (double)num7));
		}

		/// <summary>
		/// A point is in this object only if it really is inside the section of the ellipse.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool ContainsPoint(PointF p)
		{
			if (!base.ContainsPoint(p))
			{
				return false;
			}
			RectangleF bounds = Bounds;
			float num = PenWidth / 2f;
			float num2 = bounds.Width / 2f;
			float num3 = bounds.Height / 2f;
			float num4 = bounds.X + num2;
			float num5 = bounds.Y + num3;
			num2 += num + 2f;
			num3 += num + 2f;
			if (num2 == 0f || num3 == 0f)
			{
				return false;
			}
			float num6 = p.X - num4;
			float num7 = p.Y - num5;
			if (num6 * num6 / (num2 * num2) + num7 * num7 / (num3 * num3) > 1f)
			{
				return false;
			}
			num2 = bounds.Width / 2f - num - 2f;
			num3 = bounds.Height / 2f - num - 2f;
			if (num2 > 0f && num3 > 0f && num6 * num6 / (num2 * num2) + num7 * num7 / (num3 * num3) < 1f)
			{
				return false;
			}
			float angle = GoStroke.GetAngle(p.X - num4, p.Y - num5);
			float num8;
			float num9;
			if (SweepAngle < 0f)
			{
				num8 = StartAngle + SweepAngle;
				num9 = 0f - SweepAngle;
			}
			else
			{
				num8 = StartAngle;
				num9 = SweepAngle;
			}
			if (num9 > 360f)
			{
				return true;
			}
			if (num8 + num9 > 360f)
			{
				if (!(angle >= num8))
				{
					return angle <= num8 + num9 - 360f;
				}
				return true;
			}
			if (angle >= num8)
			{
				return angle <= num8 + num9;
			}
			return false;
		}

		/// <summary>
		/// Find the intersection points of an arc and the infinite line p1-p2
		/// that is closest to point p1.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF a = Bounds;
			float num = PenWidth / 2f;
			GoObject.InflateRect(ref a, num, num);
			return GoEllipse.NearestIntersectionOnArc(a, p1, p2, out result, StartAngle, SweepAngle);
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
			case 1471:
				StartAngle = e.GetFloat(undo);
				break;
			case 1472:
				SweepAngle = e.GetFloat(undo);
				break;
			case 1473:
				ResizableStartAngle = (bool)e.GetValue(undo);
				break;
			case 1474:
				ResizableEndAngle = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
