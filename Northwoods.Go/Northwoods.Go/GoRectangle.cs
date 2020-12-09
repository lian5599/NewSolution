using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of a rectangle or square, whose corners are at
	/// the corners of the bounding rectangle.
	/// </summary>
	[Serializable]
	public class GoRectangle : GoShape
	{
		/// <summary>
		/// Paint a possibly shadowed rectangle.
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
					GoShape.DrawRectangle(g, view, null, shadowBrush, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
				else if (Pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					GoShape.DrawRectangle(g, view, shadowPen, null, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
			}
			GoShape.DrawRectangle(g, view, Pen, brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}
	}
}
