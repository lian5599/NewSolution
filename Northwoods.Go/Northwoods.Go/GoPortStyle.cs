namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the appearance of the port.
	/// </summary>
	public enum GoPortStyle
	{
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> does not appear, although it may have visible
		/// links connected and may participate in linking operations.
		/// </summary>
		None = 0,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as any <see cref="T:Northwoods.Go.GoObject" />, as
		/// provided by the <see cref="P:Northwoods.Go.GoPort.PortObject" /> property.
		/// </summary>
		Object = 1,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as an ellipse or circle.
		/// </summary>
		Ellipse = 2,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle, forming an arrow
		/// in the direction of a link coming in at this port's <see cref="P:Northwoods.Go.GoPort.ToSpot" />.
		/// </summary>
		Triangle = 3,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a rectangle or square.
		/// </summary>
		Rectangle = 4,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a diamond or rhombus.
		/// </summary>
		Diamond = 5,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a plus sign or "+".
		/// </summary>
		Plus = 6,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a times sign or "x".
		/// </summary>
		Times = 7,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a combination of a plus sign and a times sign, like an asterisk or star.
		/// </summary>
		PlusTimes = 8,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the TopLeft.
		/// </summary>
		TriangleTopLeft = 20,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the TopRight.
		/// </summary>
		TriangleTopRight = 21,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the BottomRight.
		/// </summary>
		TriangleBottomRight = 22,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the BottomLeft.
		/// </summary>
		TriangleBottomLeft = 23,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the MiddleTop.
		/// </summary>
		TriangleMiddleTop = 24,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the MiddleRight.
		/// </summary>
		TriangleMiddleRight = 25,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the MiddleBottom.
		/// </summary>
		TriangleMiddleBottom = 26,
		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoPort" /> appears as a triangle with a point at the MiddleLeft.
		/// </summary>
		TriangleMiddleLeft = 27
	}
}
