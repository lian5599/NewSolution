namespace Northwoods.Go
{
	/// <summary>
	/// This enumeration represents the different states of a <see cref="T:Northwoods.Go.GoSubGraph" />.
	/// </summary>
	/// <remarks>
	/// The state of a subgraph is available through its <see cref="P:Northwoods.Go.GoSubGraph.State" />
	/// property, or via the <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" /> property.
	/// </remarks>
	public enum GoSubGraphState
	{
		/// <summary>
		/// The normal state where the subgraph children are visible.
		/// </summary>
		Expanded,
		/// <summary>
		/// The state where the subgraph children are not visible and have been repositioned
		/// on top of each other near the subgraph handle.
		/// </summary>
		Collapsed,
		/// <summary>
		/// A temporary state during a call to <see cref="M:Northwoods.Go.GoSubGraph.Expand" />.
		/// </summary>
		Expanding,
		/// <summary>
		/// A temporary state during a call to <see cref="M:Northwoods.Go.GoSubGraph.Collapse" />.
		/// </summary>
		Collapsing
	}
}
