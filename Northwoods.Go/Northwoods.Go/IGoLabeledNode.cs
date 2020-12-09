namespace Northwoods.Go
{
	/// <summary>
	/// This interface inherits from <see cref="T:Northwoods.Go.IGoLabeledPart" /> to add
	/// access to a <see cref="T:Northwoods.Go.GoText" /> label.
	/// </summary>
	/// <remarks>
	/// <see cref="T:Northwoods.Go.GoNode" /> implements this interface, as does <see cref="T:Northwoods.Go.GoComment" />.
	/// Typically you will want to implement or override this method to provide
	/// efficient access to a particular <see cref="T:Northwoods.Go.GoText" /> object in your group.
	/// Note that this interface is independent of <see cref="T:Northwoods.Go.IGoNode" />--in other words,
	/// not all <see cref="T:Northwoods.Go.GoObject" /> classes that implement <see cref="T:Northwoods.Go.IGoLabeledPart" />
	/// or <see cref="T:Northwoods.Go.IGoLabeledNode" /> will implement <see cref="T:Northwoods.Go.IGoNode" />.
	/// </remarks>
	public interface IGoLabeledNode : IGoLabeledPart
	{
		/// <summary>
		/// Gets a <see cref="T:Northwoods.Go.GoText" /> representing an object's label.
		/// </summary>
		/// <remarks>
		/// This property is typically used by an implementation of the
		/// <see cref="T:Northwoods.Go.IGoLabeledPart" />'s <see cref="P:Northwoods.Go.IGoLabeledPart.Text" /> property.
		/// Normally the <c>F2</c> key will invoke <see cref="M:Northwoods.Go.GoView.EditEdit" />
		/// to start the user's in-place editing of this text label.
		/// </remarks>
		GoText Label
		{
			get;
		}
	}
}
