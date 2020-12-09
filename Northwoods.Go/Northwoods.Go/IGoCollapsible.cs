namespace Northwoods.Go
{
	/// <summary>
	/// This interface should be implemented by all groups that want to use
	/// a <see cref="T:Northwoods.Go.GoCollapsibleHandle" /> as the object that users click on
	/// to call <see cref="M:Northwoods.Go.IGoCollapsible.Collapse" /> or <see cref="M:Northwoods.Go.IGoCollapsible.Expand" />.
	/// </summary>
	/// <remarks>
	/// One such class is <see cref="T:Northwoods.Go.GoSubGraph" />.
	/// </remarks>
	public interface IGoCollapsible
	{
		/// <summary>
		/// Gets or sets whether users may click on a handle to call
		/// <see cref="M:Northwoods.Go.IGoCollapsible.Collapse" /> or <see cref="M:Northwoods.Go.IGoCollapsible.Expand" />.
		/// </summary>
		/// <value>
		/// When this value is false, a click does not collapse or expand this object.
		/// But you can always call <see cref="M:Northwoods.Go.IGoCollapsible.Collapse" /> or <see cref="M:Northwoods.Go.IGoCollapsible.Expand" />
		/// programmatically.
		/// </value>
		bool Collapsible
		{
			get;
			set;
		}

		/// <summary>
		/// Gets whether this object has been expanded or not.
		/// </summary>
		/// <value>
		/// The initial value depends on the particular class.
		/// </value>
		bool IsExpanded
		{
			get;
		}

		/// <summary>
		/// Calling this method will change this object so that it is
		/// smaller and/or simpler in appearance and behavior.
		/// </summary>
		/// <remarks>Afterwards, <see cref="P:Northwoods.Go.IGoCollapsible.IsExpanded" /> should be false.</remarks>
		void Collapse();

		/// <summary>
		/// Calling this method will change this object so that it displays
		/// more details and/or objects.
		/// </summary>
		/// <remarks>Afterwards, <see cref="P:Northwoods.Go.IGoCollapsible.IsExpanded" /> should be true.</remarks>
		void Expand();
	}
}
