namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the appearance of the grid.
	/// </summary>
	public enum GoViewGridStyle
	{
		/// <summary>
		/// Do not display any grid.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		None,
		/// <summary>
		/// Display a grid consisting of dots.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		Dot,
		/// <summary>
		/// Display a grid consisting of small crosses.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		Cross,
		/// <summary>
		/// Display a grid consisting of lines.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		Line,
		/// <summary>
		/// Display a grid consisting of only horizontal lines.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		HorizontalLine,
		/// <summary>
		/// Display a grid consisting of only vertical lines.  See <see cref="P:Northwoods.Go.GoView.GridStyle" />.
		/// </summary>
		VerticalLine
	}
}
