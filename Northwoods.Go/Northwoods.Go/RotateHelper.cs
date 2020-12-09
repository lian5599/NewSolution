using System;
using System.Drawing;

namespace Northwoods.Go
{
	[Serializable]
	internal class RotateHelper
	{
		private RotateHelper()
		{
		}

		public static PointF RotatePoint(PointF p, PointF c, float angle)
		{
			if (angle == 0f || p == c)
			{
				return p;
			}
			double num = (double)angle * Math.PI / 180.0;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			double num4 = (double)p.X - (double)c.X;
			double num5 = (double)p.Y - (double)c.Y;
			return new PointF((float)((double)c.X + num2 * num4 - num3 * num5), (float)((double)c.Y + num3 * num4 + num2 * num5));
		}

		public static PointF RotatePoint(PointF p, PointF c, double cosine, double sine)
		{
			if (p == c)
			{
				return p;
			}
			double num = (double)p.X - (double)c.X;
			double num2 = (double)p.Y - (double)c.Y;
			return new PointF((float)((double)c.X + cosine * num - sine * num2), (float)((double)c.Y + sine * num + cosine * num2));
		}

		public static void RotatePoints(PointF[] u, PointF[] r, PointF rotatePoint, float angle)
		{
			if (angle == 0f)
			{
				int length = Math.Min(u.Length, r.Length);
				Array.Copy(u, r, length);
				return;
			}
			double num = (double)angle * Math.PI / 180.0;
			double cosine = Math.Cos(num);
			double sine = Math.Sin(num);
			RotatePoints(u, r, rotatePoint, cosine, sine);
		}

		public static void RotatePoints(PointF[] u, PointF[] r, PointF rotatePoint, double cosine, double sine)
		{
			int num = Math.Min(u.Length, r.Length);
			if (cosine == 1.0 && sine == 0.0)
			{
				Array.Copy(u, r, num);
				return;
			}
			for (int i = 0; i < num; i = checked(i + 1))
			{
				r[i] = RotatePoint(u[i], rotatePoint, cosine, sine);
			}
		}

		public static PointF RescalePoint(RectangleF oldr, RectangleF newr, PointF rp, float angle)
		{
			PointF pointF = rp;
			double cosine = 1.0;
			double num = 0.0;
			if (angle != 0f)
			{
				double num2 = (double)angle * Math.PI / 180.0;
				cosine = Math.Cos(num2);
				num = Math.Sin(num2);
				pointF = RotatePoint(rp, new PointF(oldr.X + oldr.Width / 2f, oldr.Y + oldr.Height / 2f), cosine, 0.0 - num);
			}
			float num3 = (oldr.Width <= 0f) ? 1f : (newr.Width / oldr.Width);
			float num4 = (oldr.Height <= 0f) ? 1f : (newr.Height / oldr.Height);
			float x = newr.X + (pointF.X - oldr.X) * num3;
			float y = newr.Y + (pointF.Y - oldr.Y) * num4;
			PointF pointF2 = new PointF(x, y);
			if (angle != 0f)
			{
				return RotatePoint(pointF2, new PointF(newr.X + newr.Width / 2f, newr.Y + newr.Height / 2f), cosine, num);
			}
			return pointF2;
		}

		public static RectangleF GetRotatedBounds(RectangleF r, float angle)
		{
			if (angle == 0f)
			{
				return r;
			}
			double num = (double)angle * Math.PI / 180.0;
			double cosine = Math.Cos(num);
			double sine = Math.Sin(num);
			PointF c = new PointF(r.X + r.Width / 2f, r.Y + r.Height / 2f);
			PointF pointF = RotatePoint(new PointF(r.X, r.Y), c, cosine, sine);
			PointF pointF2 = RotatePoint(new PointF(r.X + r.Width, r.Y), c, cosine, sine);
			PointF pointF3 = RotatePoint(new PointF(r.X + r.Width, r.Y + r.Height), c, cosine, sine);
			PointF pointF4 = RotatePoint(new PointF(r.X, r.Y + r.Height), c, cosine, sine);
			float num2 = Math.Min(pointF.X, Math.Min(pointF2.X, Math.Min(pointF3.X, pointF4.X)));
			float num3 = Math.Max(pointF.X, Math.Max(pointF2.X, Math.Max(pointF3.X, pointF4.X)));
			float num4 = Math.Min(pointF.Y, Math.Min(pointF2.Y, Math.Min(pointF3.Y, pointF4.Y)));
			float num5 = Math.Max(pointF.Y, Math.Max(pointF2.Y, Math.Max(pointF3.Y, pointF4.Y)));
			return new RectangleF(num2, num4, num3 - num2, num5 - num4);
		}

		public static void AddRectangleHandles(GoObject myObject, RectangleF rect, PointF center, float angle, GoSelection sel, GoObject selectedObj)
		{
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			if (myObject.CanResize() && flag)
			{
				float x = rect.X;
				float x2 = rect.X + rect.Width / 2f;
				float x3 = rect.X + rect.Width;
				float y = rect.Y;
				float y2 = rect.Y + rect.Height / 2f;
				float y3 = rect.Y + rect.Height;
				double num = (double)angle * Math.PI / 180.0;
				double cosine = Math.Cos(num);
				double sine = Math.Sin(num);
				SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x, y), center, cosine, sine), 2, filled: true), angle);
				SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x3, y), center, cosine, sine), 4, filled: true), angle);
				SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x3, y3), center, cosine, sine), 8, filled: true), angle);
				SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x, y3), center, cosine, sine), 16, filled: true), angle);
				if (myObject.CanReshape() && flag2)
				{
					SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x2, y), center, cosine, sine), 32, filled: true), angle);
					SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x3, y2), center, cosine, sine), 64, filled: true), angle);
					SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x2, y3), center, cosine, sine), 128, filled: true), angle);
					SetResizeCursor(sel.CreateResizeHandle(myObject, selectedObj, RotatePoint(new PointF(x, y2), center, cosine, sine), 256, filled: true), angle);
				}
			}
		}

		private static void SetResizeCursor(IGoHandle handle, float angle)
		{
			GoHandle goHandle = handle as GoHandle;
			if (goHandle != null)
			{
				float num = angle;
				switch (goHandle.HandleID)
				{
				default:
					return;
				case 64:
					num += 0f;
					break;
				case 8:
					num += 45f;
					break;
				case 128:
					num += 90f;
					break;
				case 16:
					num += 135f;
					break;
				case 256:
					num += 180f;
					break;
				case 2:
					num += 225f;
					break;
				case 32:
					num += 270f;
					break;
				case 4:
					num += 315f;
					break;
				}
				if (num < 0f)
				{
					num += 360f;
				}
				else if (num >= 360f)
				{
					num -= 360f;
				}
				if (num < 22.5f)
				{
					goHandle.CursorName = "e-resize";
				}
				else if (num < 67.5f)
				{
					goHandle.CursorName = "se-resize";
				}
				else if (num < 112.5f)
				{
					goHandle.CursorName = "s-resize";
				}
				else if (num < 157.5f)
				{
					goHandle.CursorName = "sw-resize";
				}
				else if (num < 202.5f)
				{
					goHandle.CursorName = "w-resize";
				}
				else if (num < 247.5f)
				{
					goHandle.CursorName = "nw-resize";
				}
				else if (num < 292.5f)
				{
					goHandle.CursorName = "n-resize";
				}
				else if (num < 337.5f)
				{
					goHandle.CursorName = "ne-resize";
				}
				else
				{
					goHandle.CursorName = "e-resize";
				}
			}
		}
	}
}
