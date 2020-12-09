namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the appearance of a <see cref="T:Northwoods.Go.GoCollapsibleHandle" />,
	/// both when it is expanded and when it is collapsed.
	/// </summary>
	public enum GoCollapsibleHandleStyle
	{
		/// <summary>
		/// A "+" when the handle is collapsed; a "-" when the handle is expanded.
		/// </summary>
		PlusMinus,
		/// <summary>
		/// A triangle pointing to the right when the handle is collapsed;
		/// a triangle pointing up when the handle is expanded.
		/// </summary>
		TriangleRight,
		/// <summary>
		/// A triangle pointing down when the handle is collapsed;
		/// a triangle pointing up when the handle is expanded.
		/// </summary>
		TriangleUp,
		/// <summary>
		/// Two open triangles pointing down when the handle is collapsed;
		/// two open triangles pointing up when the handle is expanded.
		/// </summary>
		ChevronUp
	}
}
