using System;

namespace Northwoods.Go
{
	/// <summary>
	/// This small rectangle is normally the <see cref="T:Northwoods.Go.GoSubGraph" />'s
	/// <see cref="P:Northwoods.Go.GoSubGraph.Handle" /> and handles single clicks to collapse
	/// and expand the subgraph.
	/// </summary>
	[Serializable]
	public class GoSubGraphHandle : GoCollapsibleHandle
	{
		/// <summary>
		/// Unlike a regular <see cref="T:Northwoods.Go.GoCollapsibleHandle" />, subgraph handles
		/// treat a Ctrl-click when expanding as a command to call <see cref="M:Northwoods.Go.GoSubGraph.ExpandAll" />.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override bool OnSingleClick(GoInputEventArgs evt, GoView view)
		{
			GoSubGraph goSubGraph = base.Parent as GoSubGraph;
			if (goSubGraph == null || !goSubGraph.Collapsible)
			{
				return false;
			}
			view?.StartTransaction();
			string text = null;
			if (goSubGraph.IsExpanded)
			{
				goSubGraph.Collapse();
				text = "Collapsed SubGraph";
			}
			else if (evt.Control)
			{
				goSubGraph.ExpandAll();
				text = "Expanded All SubGraphs";
			}
			else
			{
				goSubGraph.Expand();
				text = "Expanded SubGraph";
			}
			view?.FinishTransaction(text);
			return true;
		}
	}
}
