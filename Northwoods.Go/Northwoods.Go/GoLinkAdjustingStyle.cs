namespace Northwoods.Go
{
	/// <summary>
	/// Specifies how <see cref="T:Northwoods.Go.GoLink" />.<see cref="M:Northwoods.Go.GoLink.CalculateStroke" /> should determine the
	/// points of the link's stroke.
	/// </summary>
	public enum GoLinkAdjustingStyle
	{
		/// <summary>
		/// Clear all of the existing points and add points for the standard kinds of strokes.
		/// </summary>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoLink" /> has a number of standard appearances:
		/// <list type="bullet">
		/// <item>
		/// When both ports have link spots that are <c>GoObject.NoSpot</c>,
		/// draw a straight line if the <see cref="P:Northwoods.Go.GoStroke.Style" /> is <see cref="F:Northwoods.Go.GoStrokeStyle.Line" />
		/// or draw a Bezier curve if the stroke style is <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" />.
		/// </item>
		/// <item>
		/// When only one port has a link spot, draw a two segment straight line.
		/// </item>
		/// <item>
		/// When both ports have link spots, draw either a three-segment line
		/// or a Bezier curve, depending on the value of <see cref="P:Northwoods.Go.GoStroke.Style" />.
		/// </item>
		/// <item>
		/// If <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true, draw a five-segment line (either
		/// straight segments or with rounded corners) or a Bezier curve.
		/// </item>
		/// <item>
		/// If <see cref="P:Northwoods.Go.GoLink.IsSelfLoop" /> is true, draw either a Bezier curve or
		/// a five-segment line if <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> is true.
		/// </item>
		/// </list>
		/// </remarks>
		Calculate,
		/// <summary>
		/// When there are more than the standard number of points in the stroke,
		/// scale and rotate the intermediate points so that the link's shape stays
		/// approximately the same.
		/// </summary>
		/// <remarks>
		/// This style, implemented by <see cref="M:Northwoods.Go.GoLink.RescalePoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />, does not
		/// maintain the horizontal and vertical line segments of an orthogonal link.
		/// Orthogonal links with this adjusting style will instead recalculate
		/// the standard stroke path, as if the adjusting style were <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Calculate" />.
		/// </remarks>
		Scale,
		/// <summary>
		/// When there are more than the standard number of points in the stroke,
		/// linearly interpolate the intermediate points along the X and Y dimensions
		/// between the ports.
		/// </summary>
		/// <remarks>
		/// This style, implemented by <see cref="M:Northwoods.Go.GoLink.StretchPoints(System.Int32,System.Drawing.PointF,System.Int32,System.Drawing.PointF)" />, does not
		/// maintain the horizontal and vertical line segments of an orthogonal link.
		/// Orthogonal links with this adjusting style will instead only modify the
		/// end points of the existing path, as if the adjusting style were <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.End" />.
		/// </remarks>
		Stretch,
		/// <summary>
		/// When there are more than the standard number of points in the stroke,
		/// or if the stroke is orthogonal, just modify the end points,
		/// while leaving the intermediate points unchanged.
		/// </summary>
		/// <remarks>
		/// This style maintains orthogonality for orthogonal links.
		/// </remarks>
		End
	}
}
