namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies the properties that a <c>Control</c> may have
	/// to be best used by a <see cref="P:Northwoods.Go.IGoControlObject.GoControl" /> as a graphical object.
	/// </summary>
	public interface IGoControlObject
	{
		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.IGoControlObject.GoControl" /> that is managing this <c>Control</c>.
		/// </summary>
		/// <remarks>
		/// This backpointer is set right after <see cref="P:Northwoods.Go.IGoControlObject.GoControl" />.<c>CreateControl</c>
		/// creates this <c>Control</c>.
		/// Access to the <see cref="P:Northwoods.Go.IGoControlObject.GoControl" /> is important at several times:
		/// for initializing the control using state available in the GoControl,
		/// for saving any modified state to the GoControl, and for ending the edit.
		/// </remarks>
		GoControl GoControl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.IGoControlObject.GoView" /> that contains this <c>Control</c>.
		/// </summary>
		/// <remarks>
		/// This backpointer is set right after <see cref="P:Northwoods.Go.IGoControlObject.GoControl" />.<c>CreateControl</c>
		/// creates this <c>Control</c>.
		/// You can probably also find the <see cref="P:Northwoods.Go.IGoControlObject.GoView" /> by looking at this
		/// <c>Control</c>'s <c>Parent</c> property.
		/// </remarks>
		GoView GoView
		{
			get;
			set;
		}
	}
}
