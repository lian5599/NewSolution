using System;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// The "mode-less" tool used to handle the user's mouse click to invoke a context menu.
	/// </summary>
	/// <remarks>
	/// An instance of this tool is one of the first tools in the default
	/// <see cref="T:Northwoods.Go.GoView" />'s <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
	/// By default this will make the object at the mouse point be the only
	/// selected object.
	/// Set <see cref="P:Northwoods.Go.GoToolContext.SingleSelection" /> if you want to let users operate a context
	/// menu on more than one selected object.
	/// </remarks>
	[Serializable]
	public class GoToolContext : GoTool
	{
		[NonSerialized]
		private ContextMenuStrip myBackgroundContextMenuStrip;

		private bool mySingleSelection = true;

		/// <summary>
		/// Gets or sets whether the <see cref="P:Northwoods.Go.GoTool.CurrentObject" /> becomes
		/// the one and only selected object even if there are other already-selected objects.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		public virtual bool SingleSelection
		{
			get
			{
				return mySingleSelection;
			}
			set
			{
				mySingleSelection = value;
			}
		}

		/// <summary>
		/// Gets the view's original ContextMenuStrip, before being temporarily reset by this tool.
		/// </summary>
		/// <remarks>
		/// This is set by <see cref="M:Northwoods.Go.GoToolContext.Start" /> and is restored by <see cref="M:Northwoods.Go.GoToolContext.Stop" />.
		/// The view's <c>ContextMenuStrip</c> property has to be set to null temporarily to avoid
		/// being invoked as well as an object specific context menu.
		/// We recommend not using the view's <c>Control.ContextMenuStrip</c> property, but bringing
		/// up a context menu explicitly when handling the <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" />
		/// event.
		/// </remarks>
		public ContextMenuStrip BackgroundContextMenuStrip => myBackgroundContextMenuStrip;

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolContext(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// The context menu tool can be started when the last input event is a context menu button.
		/// </summary>
		/// <returns>true if <see cref="P:Northwoods.Go.GoInputEventArgs.IsContextButton" /> is true for the
		/// <see cref="P:Northwoods.Go.GoTool.LastInput" /></returns>
		public override bool CanStart()
		{
			return base.LastInput.IsContextButton;
		}

		/// <summary>
		/// When there is a selectable document object under the last input event point,
		/// select it if it isn't already in the selection and call <see cref="M:Northwoods.Go.GoView.DoContextClick(Northwoods.Go.GoInputEventArgs)" />.
		/// </summary>
		public override void DoMouseUp()
		{
			DoSelect(base.LastInput);
			base.View.DoContextClick(base.LastInput);
			StopTool();
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.GoToolContext.SingleSelection" /> is false, the user's context-click on a selected
		/// object does not modify the selection, thereby allowing the context-click action to
		/// apply to a multiple selection.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// If the user holds down the Control and/or Shift keys, or when <see cref="P:Northwoods.Go.GoToolContext.SingleSelection" />
		/// is true, the standard <see cref="M:Northwoods.Go.GoTool.DoSelect(Northwoods.Go.GoInputEventArgs)" /> behavior occurs.
		/// When the user context-clicks over an object that is not selected, it becomes
		/// the single selection.
		/// When the user context-clicks over no object, the selection is cleared.
		/// </remarks>
		public override void DoSelect(GoInputEventArgs evt)
		{
			if (SingleSelection || evt.Control || evt.Shift)
			{
				base.DoSelect(evt);
				return;
			}
			base.CurrentObject = base.View.PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: true);
			if (base.CurrentObject == null)
			{
				base.Selection.Clear();
			}
			else if (!base.Selection.Contains(base.CurrentObject))
			{
				base.Selection.Select(base.CurrentObject);
			}
		}

		/// <summary>
		/// If the view has a context menu, remember it and set it to null if there
		/// is an object at the last input event point.
		/// </summary>
		/// <remarks>
		/// This disables the default context menu behavior for <c>Control</c>s when there
		/// is a selectable document object under the mouse.  The object is free to bring
		/// up its own context menu as part of the <see cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// method.
		/// When there is no object under the mouse, the view's context menu pops
		/// up normally.
		/// We recommend not using the view's <c>Control.ContextMenu</c> property, but bringing
		/// up a context menu explicitly when handling the <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" />
		/// event.
		/// </remarks>
		public override void Start()
		{
			ContextMenuStrip contextMenuStrip = base.View.ContextMenuStrip;
			if (contextMenuStrip != null)
			{
				base.CurrentObject = base.View.PickObject(doc: true, view: false, base.LastInput.DocPoint, selectableOnly: false);
				if (base.CurrentObject != null)
				{
					myBackgroundContextMenuStrip = contextMenuStrip;
					base.View.ContextMenuStrip = null;
				}
			}
		}

		/// <summary>
		/// Restore the original context menu for the view, if any.
		/// </summary>
		public override void Stop()
		{
			if (myBackgroundContextMenuStrip != null)
			{
				base.View.ContextMenuStrip = myBackgroundContextMenuStrip;
				myBackgroundContextMenuStrip = null;
			}
			base.CurrentObject = null;
		}
	}
}
