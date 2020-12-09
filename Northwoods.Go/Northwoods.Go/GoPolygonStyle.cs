namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the kind of curve drawn between the points of the polygon.
	/// </summary>
	public enum GoPolygonStyle
	{
		/// <summary>
		/// A straight line is drawn between each two consecutive points in the <see cref="T:Northwoods.Go.GoPolygon" />,
		/// and between the last and first points to close the curve.
		/// </summary>
		Line,
		/// <summary>
		/// A Bezier curve is drawn using four points: a start point, two control points, and an end point.
		/// Additional sets of three points specify additional Bezier curves, where the new start point
		/// is the old end point.
		/// </summary>
		/// <remarks>
		/// If the first point and the last point are not at exactly the same position, there is
		/// a straight line drawn between the two to complete the closed shape.
		/// </remarks>
		Bezier
	}
}
