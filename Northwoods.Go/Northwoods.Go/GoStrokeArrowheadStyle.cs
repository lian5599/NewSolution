namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the general shape of an arrowhead.
	/// </summary>
	public enum GoStrokeArrowheadStyle
	{
		/// <summary>
		/// The arrow head is drawn as a polygon.
		/// </summary>
		/// <remarks>
		/// The number of sides for the polygon is determined by <see cref="M:Northwoods.Go.GoStroke.GetArrowheadPointsCount(System.Boolean)" />.
		/// </remarks>
		Polygon,
		/// <summary>
		/// The arrowhead is drawn as a circle.
		/// </summary>
		/// <remarks>
		/// The circle's diameter is specified by the stroke's arrow shaft length,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />.
		/// </remarks>
		Circle,
		/// <summary>
		/// The arrowhead is a line crossing perpendicularly to the shaft.
		/// </summary>
		/// <remarks>
		/// The arrow length determines the distance of the line from the end point,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" />.
		/// The arrow width determines the length of the line,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />.
		/// The arrow shaft length should normally be zero,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />.
		/// If the maximum of the arrow length and the arrow shaft length exceed
		/// the end line segment length, the whole arrowhead is scaled down to fit.
		/// </remarks>
		Cross,
		/// <summary>
		/// The arrowhead is a line crossing at an angle across the shaft.
		/// </summary>
		/// <remarks>
		/// The arrow length determines the distance of the line from the end point,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" />.
		/// The arrow width determines the length of the line,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />.
		/// The arrow shaft length should normally be zero,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />.
		/// If the maximum of the arrow length and the arrow shaft length exceed
		/// the end line segment length, the whole arrowhead is scaled down to fit.
		/// </remarks>
		Slash,
		/// <summary>
		/// The arrowhead is a line crossing at an angle across the shaft.
		/// </summary>
		/// <remarks>
		/// The arrow length determines the distance of the line from the end point,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" />.
		/// The arrow width determines the length of the line,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />.
		/// The arrow shaft length should normally be zero,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />.
		/// If the maximum of the arrow length and the arrow shaft length exceed
		/// the end line segment length, the whole arrowhead is scaled down to fit.
		/// </remarks>
		BackSlash,
		/// <summary>
		/// The arrowhead is an X crossing across the shaft (commonly used in UML class diagrams for no navigation)
		/// </summary>
		/// <remarks>
		/// The arrow length determines the height of the X,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" />.
		/// The arrow width determines the width of the X,
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />.
		/// The arrow shaft length is the offset from the port
		/// <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> or <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />.
		/// If the maximum of the arrow length and the arrow shaft length exceed
		/// the end line segment length, the whole arrowhead is scaled down to fit.
		/// </remarks>
		X
	}
}
