using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// This exists for compatibility with GoDiagram Web.
	/// </summary>
	public class GoToolStripMenuItem : ToolStripMenuItem
	{
		/// <summary>
		/// This constructor can be used by GoDiagram Web for passing JavaScript
		/// as the Click action to the client browser.
		/// </summary>
		public GoToolStripMenuItem(string s, string script)
		{
			base.Visible = false;
			Enabled = false;
		}
	}
}
