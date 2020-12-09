using System;

namespace Northwoods.Go
{
	/// <summary>
	/// This tool, normally the last tool considered by the tool manager,
	/// handles a possible change of selection by the user.
	/// </summary>
	/// <remarks>
	/// This tool assumes it is being invoked due to a mouse up, so
	/// it need not expect any additional mouse events.
	/// In other words, it is expected to be used modelessly, as one of the
	/// <see cref="P:Northwoods.Go.GoView.MouseUpTools" />.
	/// No transaction is performed by this tool, although it is possible
	/// (but unconventional) that selecting an object or clicking on it
	/// might perform one.
	/// </remarks>
	[Serializable]
	public class GoToolSelecting : GoTool
	{
		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolSelecting(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// This very simple tool just tries to select something at the last mouse point
		/// and tries to click on it.
		/// </summary>
		/// <remarks>
		/// This assumes the last mouse input state is such that calling <see cref="M:Northwoods.Go.GoTool.DoSelect(Northwoods.Go.GoInputEventArgs)" />
		/// is appropriate--this would normally be on mouse up, when no other tool was applicable.
		/// </remarks>
		public override void Start()
		{
			DoSelect(base.LastInput);
			DoClick(base.LastInput);
			StopTool();
		}
	}
}
