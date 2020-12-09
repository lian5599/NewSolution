namespace Northwoods.Go
{
	/// <summary>
	/// Specify when to calculate the routing of <see cref="T:Northwoods.Go.IGoRoutable" /> objects.
	/// </summary>
	/// <remarks>
	/// This provides values for the <see cref="T:Northwoods.Go.GoDocument" /> property <see cref="P:Northwoods.Go.GoDocument.RoutingTime" />.
	/// </remarks>
	public enum GoRoutingTime
	{
		/// <summary>
		/// Never delay routing.
		/// </summary>
		Immediate = 0,
		/// <summary>
		/// Delay calls to <see cref="T:Northwoods.Go.IGoRoutable" />.<see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" />
		/// if <see cref="T:Northwoods.Go.GoDocument" />.<see cref="P:Northwoods.Go.GoDocument.SuspendsRouting" /> is true.
		/// </summary>
		/// <value>
		/// This is the default value for <see cref="T:Northwoods.Go.GoDocument" />.<see cref="P:Northwoods.Go.GoDocument.RoutingTime" />.
		/// </value>
		Delayed = 1,
		/// <summary>
		/// Delay calls, as in the <see cref="F:Northwoods.Go.GoRoutingTime.Delayed" /> case, but also route <see cref="T:Northwoods.Go.IGoRoutable" />
		/// objects that intersect with avoidable objects that were moved,
		/// after a move, copy, drop, or paste.
		/// </summary>
		/// <remarks>
		/// This case supports automatically rerouting links around avoidable objects when those
		/// objects are dragged onto those links.
		/// </remarks>
		AfterNodesDragged = 3,
		/// <summary>
		/// Delay calls, as in the <see cref="F:Northwoods.Go.GoRoutingTime.Delayed" /> case, but also route <see cref="T:Northwoods.Go.IGoRoutable" />
		/// objects that were moved that also intersect with avoidable objects,
		/// after a move, copy, drop, or paste.
		/// </summary>
		/// <remarks>
		/// This case supports automatically rerouting links around avoidable objects when the links
		/// have been dragged onto those avoidable objects.
		/// </remarks>
		AfterLinksDragged = 5,
		/// <summary>
		/// Delay calls, as in the <see cref="F:Northwoods.Go.GoRoutingTime.Delayed" /> case, but also route <see cref="T:Northwoods.Go.IGoRoutable" />
		/// objects after a move, copy, drop, or paste to avoid intersections.
		/// </summary>
		/// <remarks>
		/// This case is a combination of <see cref="F:Northwoods.Go.GoRoutingTime.AfterNodesDragged" /> and <see cref="F:Northwoods.Go.GoRoutingTime.AfterLinksDragged" />.
		/// </remarks>
		AfterNodesAndLinksDragged = 7
	}
}
