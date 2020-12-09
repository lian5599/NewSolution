namespace Northwoods.Go
{
	/// <summary>
	/// Specifies the resize behavior of a GoHexagon.
	/// </summary>
	public enum GoHexagonReshapeBehavior
	{
		/// <summary>
		/// A resize behavior for <see cref="T:Northwoods.Go.GoHexagon" />
		/// </summary>
		FreeForm,
		/// <summary>
		/// A resize behavior for <see cref="T:Northwoods.Go.GoHexagon" />
		/// </summary>
		CrosswiseSymmetry,
		/// <summary>
		/// A resize behavior for <see cref="T:Northwoods.Go.GoHexagon" />
		/// </summary>
		LengthwiseSymmetry,
		/// <summary>
		/// A resize behavior for <see cref="T:Northwoods.Go.GoHexagon" />
		/// </summary>
		CompleteSymmetry
	}
}
