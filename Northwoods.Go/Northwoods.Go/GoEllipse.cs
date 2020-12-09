using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of an ellipse or circle.
	/// </summary>
	[Serializable]
	public class GoEllipse : GoShape
	{
		/// <summary>
		/// Draw a possibly shadowed ellipse.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			Brush brush = Brush;
			RectangleF bounds = Bounds;
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (brush != null)
				{
					Brush shadowBrush = GetShadowBrush(view);
					GoShape.DrawEllipse(g, view, null, shadowBrush, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
				else if (Pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					GoShape.DrawEllipse(g, view, shadowPen, null, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
			}
			GoShape.DrawEllipse(g, view, Pen, brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> by adding an elliptical shape.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			RectangleF bounds = Bounds;
			graphicsPath.AddEllipse(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			return graphicsPath;
		}

		/// <summary>
		/// A point is in this object only if it really is inside the ellipse.
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
			num2 += num;
			num3 += num;
			if (num2 == 0f || num3 == 0f)
			{
				return false;
			}
			float num6 = p.X - num4;
			float num7 = p.Y - num5;
			return num6 * num6 / (num2 * num2) + num7 * num7 / (num3 * num3) <= 1f;
		}

		/// <summary>
		/// A point on the ellipse that is coincident with a line drawn from
		/// the center of the ellipse to the given point.
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
			return NearestIntersectionOnEllipse(a, p1, p2, out result);
		}

		/// <summary>
		/// Find the intersection point of the elliptical path defined by rectangle rect and an infinite
		/// line p1-p2 that is closest to point p1.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool NearestIntersectionOnEllipse(RectangleF rect, PointF p1, PointF p2, out PointF result)
		{
			if (rect.Width == 0f)
			{
				return GoStroke.NearestIntersectionOnLine(new PointF(rect.X, rect.Y), new PointF(rect.X, rect.Y + rect.Height), p1, p2, out result);
			}
			if (rect.Height == 0f)
			{
				return GoStroke.NearestIntersectionOnLine(new PointF(rect.X, rect.Y), new PointF(rect.X + rect.Width, rect.Y), p1, p2, out result);
			}
			float num = rect.Width / 2f;
			float num2 = rect.Height / 2f;
			float num3 = rect.X + num;
			float num4 = rect.Y + num2;
			float num5 = 9999f;
			if (p1.X > p2.X)
			{
				num5 = (p1.Y - p2.Y) / (p1.X - p2.X);
			}
			else if (p1.X < p2.X)
			{
				num5 = (p2.Y - p1.Y) / (p2.X - p1.X);
			}
			if (Math.Abs(num5) < 9999f)
			{
				float num6 = p1.Y - num4 - num5 * (p1.X - num3);
				if (num * num * (num5 * num5) + num2 * num2 - num6 * num6 < 0f)
				{
					result = default(PointF);
					return false;
				}
				float num7 = (float)Math.Sqrt(num * num * (num5 * num5) + num2 * num2 - num6 * num6);
				float num8 = (0f - num * num * num5 * num6 + num * num2 * num7) / (num2 * num2 + num * num * (num5 * num5)) + num3;
				float num9 = (0f - num * num * num5 * num6 - num * num2 * num7) / (num2 * num2 + num * num * (num5 * num5)) + num3;
				float num10 = num5 * (num8 - num3) + num6 + num4;
				float num11 = num5 * (num9 - num3) + num6 + num4;
				float num12 = Math.Abs((p1.X - num8) * (p1.X - num8)) + Math.Abs((p1.Y - num10) * (p1.Y - num10));
				float num13 = Math.Abs((p1.X - num9) * (p1.X - num9)) + Math.Abs((p1.Y - num11) * (p1.Y - num11));
				if (num12 < num13)
				{
					result = new PointF(num8, num10);
				}
				else
				{
					result = new PointF(num9, num11);
				}
			}
			else
			{
				float num14 = num2 * num2;
				float num15 = num * num;
				float num16 = p1.X - num3;
				float num17 = num14 - num14 / num15 * (num16 * num16);
				if (num17 < 0f)
				{
					result = default(PointF);
					return false;
				}
				float num18 = (float)Math.Sqrt(num17);
				float num19 = num4 + num18;
				float num20 = num4 - num18;
				float num21 = Math.Abs(num19 - p1.Y);
				float num22 = Math.Abs(num20 - p1.Y);
				if (num21 < num22)
				{
					result = new PointF(p1.X, num19);
				}
				else
				{
					result = new PointF(p1.X, num20);
				}
			}
			return true;
		}

		/// <summary>
		/// Find the intersection point of the elliptical path defined by rectangle rect and an infinite
		/// line p1-p2 that is closest to point p1 within the area from startAngle through the sweepAngle.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <param name="startAngle"></param>
		/// <param name="sweepAngle"></param>
		/// <returns></returns>
		public static bool NearestIntersectionOnArc(RectangleF rect, PointF p1, PointF p2, out PointF result, float startAngle, float sweepAngle)
		{
			float num = rect.Width / 2f;
			float num2 = rect.Height / 2f;
			float num3 = rect.X + num;
			float num4 = rect.Y + num2;
			float num5;
			float num6;
			if (sweepAngle < 0f)
			{
				num5 = startAngle + sweepAngle;
				num6 = 0f - sweepAngle;
			}
			else
			{
				num5 = startAngle;
				num6 = sweepAngle;
			}
			if (p1.X != p2.X)
			{
				float num7 = (!(p1.X > p2.X)) ? ((p2.Y - p1.Y) / (p2.X - p1.X)) : ((p1.Y - p2.Y) / (p1.X - p2.X));
				float num8 = p1.Y - num4 - num7 * (p1.X - num3);
				float num9 = (float)Math.Sqrt(num * num * (num7 * num7) + num2 * num2 - num8 * num8);
				float num10 = (0f - num * num * num7 * num8 + num * num2 * num9) / (num2 * num2 + num * num * (num7 * num7)) + num3;
				float num11 = (0f - num * num * num7 * num8 - num * num2 * num9) / (num2 * num2 + num * num * (num7 * num7)) + num3;
				float num12 = num7 * (num10 - num3) + num8 + num4;
				float num13 = num7 * (num11 - num3) + num8 + num4;
				float num14 = GoStroke.GetAngle(num10 - num3, num12 - num4);
				float num15 = GoStroke.GetAngle(num11 - num3, num13 - num4);
				if (num14 < num5)
				{
					num14 += 360f;
				}
				if (num15 < num5)
				{
					num15 += 360f;
				}
				if (num14 > num5 + num6)
				{
					num14 -= 360f;
				}
				if (num15 > num5 + num6)
				{
					num15 -= 360f;
				}
				bool flag = num14 >= num5 && num14 <= num5 + num6;
				bool flag2 = num15 >= num5 && num15 <= num5 + num6;
				if (flag && flag2)
				{
					float num16 = Math.Abs((p1.X - num10) * (p1.X - num10)) + Math.Abs((p1.Y - num12) * (p1.Y - num12));
					float num17 = Math.Abs((p1.X - num11) * (p1.X - num11)) + Math.Abs((p1.Y - num13) * (p1.Y - num13));
					if (num16 < num17)
					{
						result = new PointF(num10, num12);
					}
					else
					{
						result = new PointF(num11, num13);
					}
					return true;
				}
				if (flag && !flag2)
				{
					result = new PointF(num10, num12);
					return true;
				}
				if (!flag && flag2)
				{
					result = new PointF(num11, num13);
					return true;
				}
				result = default(PointF);
				return false;
			}
			float num18 = (float)Math.Sqrt(num2 * num2 - num2 * num2 / (num * num) * ((p1.X - num3) * (p1.X - num3)));
			float num19 = num4 + num18;
			float num20 = num4 - num18;
			float num21 = GoStroke.GetAngle(p1.X - num3, num19 - num4);
			float num22 = GoStroke.GetAngle(p1.X - num3, num20 - num4);
			if (num21 < num5)
			{
				num21 += 360f;
			}
			if (num22 < num5)
			{
				num22 += 360f;
			}
			if (num21 > num5 + num6)
			{
				num21 -= 360f;
			}
			if (num22 > num5 + num6)
			{
				num22 -= 360f;
			}
			bool flag3 = num21 >= num5 && num21 <= num5 + num6;
			bool flag4 = num22 >= num5 && num22 <= num5 + num6;
			if (flag3 && flag4)
			{
				float num23 = Math.Abs(num19 - p1.Y);
				float num24 = Math.Abs(num20 - p1.Y);
				if (num23 < num24)
				{
					result = new PointF(p1.X, num19);
				}
				else
				{
					result = new PointF(p1.X, num20);
				}
				return true;
			}
			if (flag3 && !flag4)
			{
				result = new PointF(p1.X, num19);
				return true;
			}
			if (!flag3 && flag4)
			{
				result = new PointF(p1.X, num20);
				return true;
			}
			result = default(PointF);
			return false;
		}
	}
}
