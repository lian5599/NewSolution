namespace Northwoods.Go
{
	/// <summary>
	/// Provide access to a text string for a <see cref="T:Northwoods.Go.GoObject" />.
	/// </summary>
	/// <remarks>
	/// The text string is used by <see cref="M:Northwoods.Go.GoDocument.FindNode(System.String)" /> and
	/// <see cref="M:Northwoods.Go.GoView.SelectNextNode(System.Char)" /> to search for objects meeting certain criteria.
	/// </remarks>
	public interface IGoLabeledPart
	{
		/// <summary>
		/// Gets a text string for this <see cref="T:Northwoods.Go.GoObject" />.
		/// </summary>
		/// <value>
		/// This may return null.
		/// </value>
		/// <remarks>
		/// For <see cref="T:Northwoods.Go.IGoLabeledNode" />s, this is normally implemented to return
		/// the <see cref="P:Northwoods.Go.IGoLabeledNode.Label" />'s <see cref="P:Northwoods.Go.GoText.Text" /> value.
		/// You may wish to implement this differently if there is no label but
		/// there is a string naturally associated with the object, or if the
		/// desired string might be different than the label's text string.
		/// </remarks>
		string Text
		{
			get;
		}
	}
}
