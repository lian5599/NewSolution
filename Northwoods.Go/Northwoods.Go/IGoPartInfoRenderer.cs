namespace Northwoods.Go
{
	/// <summary>
	/// This interface should be implemented by classes that generate JavaScript
	/// along with the rendering of a view
	/// and that want to generate property/value collections
	/// as JavaScript objects to be associated with particular
	/// <see cref="T:Northwoods.Go.GoObject" />s in that rendering.
	/// </summary>
	public interface IGoPartInfoRenderer
	{
		/// <summary>
		/// Allocate and return a <see cref="T:Northwoods.Go.GoPartInfo" /> for this renderer.
		/// </summary>
		/// <returns>a <see cref="T:Northwoods.Go.GoPartInfo" /></returns>
		/// <remarks>
		/// An override of <see cref="M:Northwoods.Go.GoObject.GetPartInfo(Northwoods.Go.GoView,Northwoods.Go.IGoPartInfoRenderer)" /> can call this to
		/// allocate an empty <see cref="T:Northwoods.Go.GoPartInfo" /> in order to specify all
		/// of the properties without getting the standard property values
		/// that the renderer would produce in a call to <see cref="M:Northwoods.Go.IGoPartInfoRenderer.GetStandardPartInfo(Northwoods.Go.GoObject)" />.
		/// </remarks>
		GoPartInfo CreatePartInfo();

		/// <summary>
		/// Produce the standard <see cref="T:Northwoods.Go.GoPartInfo" /> for a given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>
		/// This may be null if the given object is not interesting enough to warrant
		/// generating property/value information.
		/// </returns>
		/// <remarks>
		/// This is called by <see cref="M:Northwoods.Go.GoObject.GetPartInfo(Northwoods.Go.GoView,Northwoods.Go.IGoPartInfoRenderer)" /> to get the
		/// renderer's standard property values for the given object.
		/// An override of <see cref="M:Northwoods.Go.GoObject.GetPartInfo(Northwoods.Go.GoView,Northwoods.Go.IGoPartInfoRenderer)" /> will normally
		/// call the base method and modify the result, but if that result is
		/// null it may want to call <see cref="M:Northwoods.Go.IGoPartInfoRenderer.CreatePartInfo" /> in order to
		/// pass along its specific information for that object.
		/// </remarks>
		GoPartInfo GetStandardPartInfo(GoObject obj);

		/// <summary>
		/// Tell the renderer to associate a given <see cref="T:Northwoods.Go.GoPartInfo" />
		/// with a particular <see cref="T:Northwoods.Go.GoObject" />, so that the user agent
		/// will be able to find the JavaScript object as information describing
		/// the <see cref="T:Northwoods.Go.GoObject" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="info"></param>
		void AssociatePartInfo(GoObject obj, GoPartInfo info);

		/// <summary>
		/// This higher-level method is called to produce a <see cref="T:Northwoods.Go.GoPartInfo" /> for a given
		/// object and then call <see cref="M:Northwoods.Go.IGoPartInfoRenderer.AssociatePartInfo(Northwoods.Go.GoObject,Northwoods.Go.GoPartInfo)" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// Overrides of <see cref="M:Northwoods.Go.GoObject.GetPartInfo(Northwoods.Go.GoView,Northwoods.Go.IGoPartInfoRenderer)" /> may want to call this
		/// method in order to provide information about child objects as well as
		/// returning information for the given (parent) object.
		/// </remarks>
		void AddPartInfo(GoObject obj);
	}
}
