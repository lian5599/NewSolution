using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	[Serializable]
	internal class GoHandleRotated : GoHandle
	{
		private float myAngle;

		public float Angle
		{
			get
			{
				return myAngle;
			}
			set
			{
				myAngle = value;
			}
		}

		public override void Paint(Graphics g, GoView view)
		{
			GoShape.DrawPath(g, view, Pen, Brush, GetPath());
		}

		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			RectangleF a = RotateHelper.GetRotatedBounds(rect, Angle);
			float penWidth = PenWidth;
			GoObject.InflateRect(ref a, penWidth, penWidth);
			return RectangleF.Union(rect, a);
		}

		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			graphicsPath.AddRectangle(Bounds);
			if (Angle != 0f)
			{
				Matrix matrix = new Matrix();
				matrix.RotateAt(Angle, base.Center);
				graphicsPath.Transform(matrix);
				matrix.Dispose();
			}
			return graphicsPath;
		}

		public override bool ContainsPoint(PointF p)
		{
			return base.ContainsPoint(RotateHelper.RotatePoint(p, base.Center, 0f - Angle));
		}
	}
}
