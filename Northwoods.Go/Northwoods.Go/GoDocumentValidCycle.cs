namespace Northwoods.Go
{
	/// <summary>
	/// Specifies what kinds of cycles may be made by a valid link from a port.
	/// </summary>
	/// <remarks>
	/// This provides values for the <see cref="T:Northwoods.Go.GoDocument" /> property <see cref="P:Northwoods.Go.GoDocument.ValidCycle" />.
	/// </remarks>
	public enum GoDocumentValidCycle
	{
		/// <summary>
		/// No restrictions on cycles, except when imposed by <see cref="P:Northwoods.Go.GoPort.IsValidSelfNode" />
		/// or <see cref="P:Northwoods.Go.GoPort.IsValidDuplicateLinks" />.
		/// </summary>
		/// <remarks>
		/// This is the default for <see cref="P:Northwoods.Go.GoDocument.ValidCycle" />.
		/// </remarks>
		All,
		/// <summary>
		/// A valid link from a port will not produce a directed cycle in the graph.
		/// </summary>
		/// <remarks>
		/// This option uses <see cref="M:Northwoods.Go.GoDocument.MakesDirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" /> in the
		/// implementation of <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />.
		/// </remarks>
		NotDirected,
		/// <summary>
		/// A valid link from a port will not produce a directed cycle in the graph,
		/// assuming there are no directed cycles anywhere accessible from either port.
		/// </summary>
		/// <remarks>
		/// This option uses <see cref="M:Northwoods.Go.GoDocument.MakesDirectedCycleFast(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" /> in the
		/// implementation of <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />.
		/// </remarks>
		NotDirectedFast,
		/// <summary>
		/// A valid link from a port will not produce an undirected cycle in the graph.
		/// </summary>
		/// <remarks>
		/// Traversal of all links during the check for any undirected cycles ignores
		/// the implicit direction of each link.
		/// This option uses <see cref="M:Northwoods.Go.GoDocument.MakesUndirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" /> in the
		/// implementation of <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />.
		/// </remarks>
		NotUndirected,
		/// <summary>
		/// Any number of destination links may go out of a port, but at most one
		/// source link may come into a port, and there are no directed cycles.
		/// </summary>
		DestinationTree,
		/// <summary>
		/// Any number of source links may come into a port, but at most one
		/// destination link may go out of a port, and there are no directed cycles.
		/// </summary>
		SourceTree
	}
}
