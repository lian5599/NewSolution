using System.ComponentModel;
using System.Security;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// GoContextMenu is just a <c>ContextMenu</c> that provides an easy way to 
	/// get the GoView in which a context menu <c>ToolStripMenuItem.Click</c> event handler
	/// was invoked.
	/// </summary>
	/// <example>
	/// Adding a context menu to a particular node class:
	/// <code>
	///  public override GoContextMenu GetContextMenu(GoView view) {
	///    GoContextMenu cm = new GoContextMenu(view);
	///    cm.ToolStripMenuItems.Add(new ToolStripMenuItem("Copy", new EventHandler(this.Copy_Command)));
	///    return cm;
	///  }
	///  private void Copy_Command(Object sender, EventArgs e) {
	///    GoView v = GoContextMenu.FindView(sender as ToolStripMenuItem);
	///    if (v != null)
	///      v.EditCopy();
	///  }
	/// </code>
	/// </example>
	[ToolboxItem(false)]
	[DesignTimeVisible(false)]
	public class GoContextMenu : ContextMenuStrip
	{
		private GoView myView;

		/// <summary>
		/// Gets the GoView that this GoContextMenu was created for.
		/// </summary>
		public GoView View => myView;

		/// <summary>
		/// This <c>ContextMenu</c> subclass remembers the <see cref="T:Northwoods.Go.GoView" /> in which the
		/// <c>ToolStripMenuItem</c>s commands should operate.
		/// </summary>
		/// <param name="view">The <see cref="T:Northwoods.Go.GoView" /> in which this <c>ContextMenu</c> is being invoked.</param>
		public GoContextMenu(GoView view)
		{
			myView = view;
		}

		/// <summary>
		/// For a ToolStripMenuItem in a GoContextMenu, this static method gets the GoView in which
		/// the menu item command should be operating.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		/// <remarks>
		/// Sometimes an object specific context menu item command will need access to the
		/// view in which the context menu was invoked.
		/// </remarks>
		public static GoView FindView(ToolStripMenuItem m)
		{
			if (m == null)
			{
				return null;
			}
			ToolStrip toolStrip = null;
			try
			{
				toolStrip = m.Owner;
			}
			catch (VerificationException)
			{
			}
			catch (SecurityException)
			{
			}
			if (toolStrip is GoContextMenu)
			{
				return ((GoContextMenu)toolStrip).View;
			}
			return null;
		}
	}
}
