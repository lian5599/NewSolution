using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An object in the shape of a diamond or rhombus whose corners are at
	/// the midpoints of the bounding rectangle's edges.
	/// </summary>
	[Serializable]
	public class GoDiamond : GoShape
	{
		private PointF[] myPoints = new PointF[4];

		/// <summary>
		/// Don't share any internal data between copies.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoDiamond goDiamond = (GoDiamond)base.CopyObject(env);
			if (goDiamond != null)
			{
				goDiamond.myPoints = (PointF[])myPoints.Clone();
			}
			return goDiamond;
		}

		/// <summary>
		/// Paint a diamond shape, possibly with a shadow.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPolygon(g, view, getPoints());
		}

		private PointF[] getPoints()
		{
			RectangleF bounds = Bounds;
			myPoints[0].X = bounds.X + bounds.Width / 2f;
			myPoints[0].Y = bounds.Y;
			myPoints[1].X = bounds.X + bounds.Width;
			myPoints[1].Y = bounds.Y + bounds.Height / 2f;
			myPoints[2].X = myPoints[0].X;
			myPoints[2].Y = bounds.Y + bounds.Height;
			myPoints[3].X = bounds.X;
			myPoints[3].Y = myPoints[1].Y;
			return myPoints;
		}

		/// <summary>
		/// Consider the sides of the diamond when determining if a point is inside.
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

		/// <summary>
		/// The closest point of a diamond that intersects with a given line
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
	}
}
