namespace Northwoods.Go
{
	/// <summary>
	/// Specifies values for <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.SheetStyle" />,
	/// which affects the behavior of <see cref="M:Northwoods.Go.GoView.UpdateExtent" /> to automatically
	/// scroll and scale the view as the view's size is changed, or (in WinForms) when
	/// the PrintScale is changed.
	/// </summary>
	public enum GoViewSheetStyle
	{
		/// <summary>
		/// The GoView.Sheet is not visible, and the view does not rescale/scroll automatically as the view's size is changed.
		/// </summary>
		/// <remarks>
		/// Scrolling, i.e. setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocPosition" />,
		/// is limited to stay within the bounds of the <see cref="P:Northwoods.Go.GoView.Document" />.
		/// </remarks>
		None,
		/// <summary>
		/// The GoView.Sheet is visible, but do not rescale/scroll automatically as the view's size is changed.
		/// </summary>
		/// <remarks>
		/// Scrolling, i.e. setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocPosition" />,
		/// is not limited to stay within the bounds of the <see cref="P:Northwoods.Go.GoView.Document" />,
		/// but may also include anywhere within the <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// Printing is limited to the region defined by
		/// <see cref="P:Northwoods.Go.GoView.Sheet" />.<see cref="P:Northwoods.Go.GoSheet.MarginBounds" />. (WinForms only).
		/// </remarks>
		Sheet,
		/// <summary>
		/// Keep the whole page visible as the view's size is changed.
		/// </summary>
		/// <remarks>
		/// Scrolling, i.e. setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocPosition" />,
		/// is not limited to stay within the bounds of the <see cref="P:Northwoods.Go.GoView.Document" />,
		/// but may also include anywhere within the <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// Printing is limited to the region defined by
		/// <see cref="P:Northwoods.Go.GoView.Sheet" />.<see cref="P:Northwoods.Go.GoSheet.MarginBounds" />. (WinForms only).
		/// </remarks>
		WholeSheet,
		/// <summary>
		/// Keep the whole width of the page visible as the view's size is changed.
		/// </summary>
		/// <remarks>
		/// Scrolling, i.e. setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocPosition" />,
		/// is not limited to stay within the bounds of the <see cref="P:Northwoods.Go.GoView.Document" />,
		/// but may also include anywhere within the <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// Printing is limited to the region defined by
		/// <see cref="P:Northwoods.Go.GoView.Sheet" />.<see cref="P:Northwoods.Go.GoSheet.MarginBounds" />. (WinForms only).
		/// </remarks>
		SheetWidth,
		/// <summary>
		/// Keep the whole height of the page visible as the view's size is changed.
		/// </summary>
		/// <remarks>
		/// Scrolling, i.e. setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocPosition" />,
		/// is not limited to stay within the bounds of the <see cref="P:Northwoods.Go.GoView.Document" />,
		/// but may also include anywhere within the <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// Printing is limited to the region defined by
		/// <see cref="P:Northwoods.Go.GoView.Sheet" />.<see cref="P:Northwoods.Go.GoSheet.MarginBounds" />. (WinForms only).
		/// </remarks>
		SheetHeight
	}
}
