namespace Northwoods.Go
{
	/// <summary>
	/// This interface is implemented by those <see cref="T:Northwoods.Go.GoObject" />s that have a shape
	/// determined by the positions of more than one object.
	/// </summary>
	/// <seealso cref="T:Northwoods.Go.GoLink" />
	/// <seealso cref="T:Northwoods.Go.GoLabeledLink" />
	public interface IGoRoutable
	{
		/// <summary>
		/// This method should modify the shape of this <see cref="T:Northwoods.Go.GoObject" /> according
		/// to the bounds of the objects in the document.
		/// </summary>
		void CalculateRoute();

		/// <summary>
		/// Request the recalculation of the shape of this object.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this <see cref="T:Northwoods.Go.GoObject" /> is part of a <see cref="T:Northwoods.Go.GoDocument" />
		/// this should possibly delay the call to <see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" />
		/// by calling <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.UpdateRoute(Northwoods.Go.IGoRoutable)" />.
		/// Otherwise this should just call <see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" />
		/// to determine a new route immediately.
		/// </para>
		/// <para>
		/// This is typically implemented as:
		/// <pre><code>
		/// public virtual void UpdateRoute() {
		///   GoDocument doc = this.Document;
		///   if (doc != null)
		///     doc.UpdateRoute(this);
		///   else
		///     CalculateRoute();
		/// }
		/// </code></pre>
		/// </para>
		/// </remarks>
		void UpdateRoute();
	}
}
