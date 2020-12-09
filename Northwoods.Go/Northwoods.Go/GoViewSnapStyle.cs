namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the move or resize behavior based on the grid.
	/// </summary>
	public enum GoViewSnapStyle
	{
		/// <summary>
		/// Ignore the grid while moving or resizing an object.
		/// See <see cref="P:Northwoods.Go.GoGrid.SnapDrag" /> and <see cref="P:Northwoods.Go.GoGrid.SnapResize" />
		/// </summary>
		None,
		/// <summary>
		/// Move or resize an object to a grid point continuously as the mouse moves.
		/// See <see cref="P:Northwoods.Go.GoGrid.SnapDrag" /> and <see cref="P:Northwoods.Go.GoGrid.SnapResize" />
		/// </summary>
		Jump,
		/// <summary>
		/// Move or resize an object to a grid point only on a mouse up.
		/// See <see cref="P:Northwoods.Go.GoGrid.SnapDrag" /> and <see cref="P:Northwoods.Go.GoGrid.SnapResize" />
		/// </summary>
		After
	}
}
