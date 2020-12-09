namespace Northwoods.Go
{
	/// <summary>
	/// Specifies when a scroll bar should be visible.
	/// </summary>
	public enum GoViewScrollBarVisibility
	{
		/// <summary>
		/// Do not show the scroll bar at any time.
		/// See <see cref="P:Northwoods.Go.GoView.ShowVerticalScrollBar" /> and <see cref="P:Northwoods.Go.GoView.ShowHorizontalScrollBar" />.
		/// </summary>
		Hide,
		/// <summary>
		/// Always show the scroll bar, even if disabled.
		/// See <see cref="P:Northwoods.Go.GoView.ShowVerticalScrollBar" /> and <see cref="P:Northwoods.Go.GoView.ShowHorizontalScrollBar" />.
		/// </summary>
		Show,
		/// <summary>
		/// Show the scroll bar if needed, and hide it if not needed.
		/// See <see cref="P:Northwoods.Go.GoView.ShowVerticalScrollBar" /> and <see cref="P:Northwoods.Go.GoView.ShowHorizontalScrollBar" />.
		/// </summary>
		IfNeeded
	}
}
