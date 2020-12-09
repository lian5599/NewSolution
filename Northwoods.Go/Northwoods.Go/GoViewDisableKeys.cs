using System;

namespace Northwoods.Go
{
	/// <summary>
	/// Controls the behavior of <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />
	/// via the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DisableKeys" /> property.
	/// </summary>
	/// <remarks>
	/// These flags can be combined bit-wise.
	/// </remarks>
	[Flags]
	public enum GoViewDisableKeys
	{
		/// <summary>
		/// All keyboard behavior of <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" /> is enabled.
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Disable the arrow keys
		/// from calling <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		/// <remarks>
		/// Moving selected objects using the arrow keys is disabled by default.
		/// </remarks>
		ArrowMove = 0x1,
		/// <summary>
		/// Disable the <c>Escape</c> key
		/// from clearing the <see cref="P:Northwoods.Go.GoView.Selection" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		/// <remarks>
		/// However the <c>Escape</c> key will normally continue to stop the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </remarks>
		CancelDeselects = 0x2,
		/// <summary>
		/// Disable the <c>Ctrl-A</c> key
		/// from calling <see cref="M:Northwoods.Go.GoView.SelectAll" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		SelectAll = 0x4,
		/// <summary>
		/// Disable having letters and digits select the next <see cref="T:Northwoods.Go.IGoLabeledPart" />
		/// by not calling <see cref="M:Northwoods.Go.GoView.SelectNextNode(System.Char)" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		SelectsByFirstChar = 0x8,
		/// <summary>
		/// Disable the <c>Delete</c> key
		/// from calling <see cref="M:Northwoods.Go.GoView.EditDelete" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Delete = 0x10,
		/// <summary>
		/// Disable the <c>Ctrl-C</c> and <c>Ctrl-X</c> and <c>Ctrl-V</c> keys
		/// from calling <see cref="M:Northwoods.Go.GoView.EditCopy" /> and <see cref="M:Northwoods.Go.GoView.EditCut" /> and <see cref="M:Northwoods.Go.GoView.EditPaste" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Clipboard = 0x20,
		/// <summary>
		/// Disable the <c>F2</c> key
		/// from calling <see cref="M:Northwoods.Go.GoView.EditEdit" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Edit = 0x40,
		/// <summary>
		/// Disable the <c>Page-Down</c> and <c>Page-Up</c> keys
		/// from calling <see cref="M:Northwoods.Go.GoView.ScrollPage(System.Single,System.Single)" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Page = 0x80,
		/// <summary>
		/// Disable the <c>Home</c> key
		/// from setting the <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Home = 0x100,
		/// <summary>
		/// Disable the <c>End</c> key
		/// from setting the <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		End = 0x200,
		/// <summary>
		/// Disable the <c>Ctrl-Z</c> and <c>Ctrl-Y</c> keys
		/// from calling <see cref="M:Northwoods.Go.GoView.Undo" /> and <see cref="M:Northwoods.Go.GoView.Redo" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		Undo = 0x400,
		/// <summary>
		/// Disable the arrow keys
		/// from calling <see cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		ArrowScroll = 0x800,
		/// <summary>
		/// Disable all standard operations for keys
		/// in <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" />.
		/// </summary>
		All = 0xFFF
	}
}
