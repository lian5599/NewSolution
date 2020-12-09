namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the shape of the handle.
	/// </summary>
	public enum GoHandleStyle
	{
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />
		/// </summary>
		None = 0,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />
		/// </summary>
		Rectangle = 1,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />
		/// </summary>
		Ellipse = 2,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />
		/// </summary>
		Diamond = 3,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the TopLeft.
		/// </summary>
		TriangleTopLeft = 10,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the TopRight.
		/// </summary>
		TriangleTopRight = 11,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the BottomRight.
		/// </summary>
		TriangleBottomRight = 12,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the BottomLeft.
		/// </summary>
		TriangleBottomLeft = 13,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the MiddleTop.
		/// </summary>
		TriangleMiddleTop = 14,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the MiddleRight.
		/// </summary>
		TriangleMiddleRight = 0xF,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the MiddleBottom.
		/// </summary>
		TriangleMiddleBottom = 0x10,
		/// <summary>
		/// An appearance style for <see cref="T:Northwoods.Go.GoHandle" />: a triangle with a point at the MiddleLeft.
		/// </summary>
		TriangleMiddleLeft = 17
	}
}
