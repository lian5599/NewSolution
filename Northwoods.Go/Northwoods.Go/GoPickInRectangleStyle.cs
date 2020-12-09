namespace Northwoods.Go
{
	/// <summary>
	/// Specifies how <see cref="M:Northwoods.Go.GoDocument.PickObjectsInRectangle(System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />, 
	/// <see cref="M:Northwoods.Go.GoLayer.PickObjectsInRectangle(System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />, and <see cref="M:Northwoods.Go.GoView.PickObjectsInRectangle(System.Boolean,System.Boolean,System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />
	/// decide which objects are considered "in" a given rectangle.
	/// </summary>
	public enum GoPickInRectangleStyle
	{
		/// <summary>
		/// Include any object whose <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> is
		/// fully contained by the pick rectangle.
		/// </summary>
		AnyContained = 1,
		/// <summary>
		/// Include any object whose <see cref="P:Northwoods.Go.GoObject.SelectionObject" />'s
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" /> intersects the pick rectangle.
		/// </summary>
		AnyIntersectsBounds = 2,
		/// <summary>
		/// Include only selectable objects whose <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> is
		/// fully contained by the pick rectangle.
		/// </summary>
		SelectableOnlyContained = 257,
		/// <summary>
		/// Include only selectable objects whose <see cref="P:Northwoods.Go.GoObject.SelectionObject" />'s
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" /> intersects the pick rectangle.
		/// </summary>
		SelectableOnlyIntersectsBounds = 258
	}
}
