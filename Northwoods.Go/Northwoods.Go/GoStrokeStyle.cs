namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the kind of curve drawn between the points of the stroke.
	/// </summary>
	public enum GoStrokeStyle
	{
		/// <summary>
		/// A straight line is drawn between each two consecutive points in the <see cref="T:Northwoods.Go.GoStroke" />.
		/// </summary>
		Line,
		/// <summary>
		/// Rounded line is like <see cref="F:Northwoods.Go.GoStrokeStyle.Line" /> but with curves at each point where the lines intersect.
		/// Currently this is only implemented for adjacent segments that intersect at right angles.
		/// </summary>
		RoundedLine,
		/// <summary>
		/// A Bezier curve is drawn using four points: a start point, two control points, and an end point.
		/// Additional sets of three points specify additional Bezier curves, where the new start point
		/// is the old end point.
		/// </summary>
		Bezier,
		/// <summary>
		/// Like <see cref="F:Northwoods.Go.GoStrokeStyle.RoundedLine" /> but the path also "jumps" or "hops" over intersections with
		/// other strokes that have this style.
		/// </summary>
		RoundedLineWithJumpOvers,
		/// <summary>
		/// Like <see cref="F:Northwoods.Go.GoStrokeStyle.RoundedLineWithJumpOvers" /> but the path has a gap across intersections with
		/// other strokes that have this style.
		/// </summary>
		RoundedLineWithJumpGaps
	}
}
