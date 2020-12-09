namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies the properties used by <see cref="T:Northwoods.Go.GoLayer" />,
	/// <see cref="T:Northwoods.Go.GoDocument" />, and <see cref="T:Northwoods.Go.GoView" /> for describing what
	/// kinds of actions the user is allowed to do.
	/// </summary>
	public interface IGoLayerAbilities
	{
		/// <summary>
		/// Gets or sets whether the user can select objects in this layer, document or view.
		/// </summary>
		bool AllowSelect
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can move selected objects in this layer, document or view.
		/// </summary>
		bool AllowMove
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can copy selected objects in this layer, document or view.
		/// </summary>
		bool AllowCopy
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can resize selected objects in this layer, document or view.
		/// </summary>
		bool AllowResize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can reshape resizable selected objects in this layer, document or view.
		/// </summary>
		bool AllowReshape
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can delete selected objects in this layer, document or view.
		/// </summary>
		bool AllowDelete
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can insert objects in this layer, document or view.
		/// </summary>
		bool AllowInsert
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can link objects in this layer, document or view.
		/// </summary>
		bool AllowLink
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the user can edit objects in this layer, document or view.
		/// </summary>
		bool AllowEdit
		{
			get;
			set;
		}

		/// <summary>
		/// Called to see if the user can select objects in this layer, document or view.
		/// </summary>
		bool CanSelectObjects();

		/// <summary>
		/// Called to see if the user can move selected objects in this layer, document or view.
		/// </summary>
		bool CanMoveObjects();

		/// <summary>
		/// Called to see if the user can copy selected objects in this layer, document or view.
		/// </summary>
		bool CanCopyObjects();

		/// <summary>
		/// Called to see if the user can resize selected objects in this layer, document or view.
		/// </summary>
		bool CanResizeObjects();

		/// <summary>
		/// Called to see if the user can reshape resizable selected objects in this layer, document or view.
		/// </summary>
		bool CanReshapeObjects();

		/// <summary>
		/// Called to see if the user can delete selected objects in this layer, document or view.
		/// </summary>
		bool CanDeleteObjects();

		/// <summary>
		/// Called to see if the user can insert objects in this layer, document or view.
		/// </summary>
		bool CanInsertObjects();

		/// <summary>
		/// Called to see if the user can link objects in this layer, document or view.
		/// </summary>
		bool CanLinkObjects();

		/// <summary>
		/// Called to see if the user can edit objects in this layer, document or view.
		/// </summary>
		bool CanEditObjects();

		/// <summary>
		/// Set the <c>AllowMove</c>, <c>AllowResize</c>, <c>AllowReshape</c>,
		/// <c>AllowDelete</c>, <c>AllowInsert</c>, <c>AllowLink</c>, and
		/// <c>AllowEdit</c> properties.
		/// </summary>
		/// <param name="b"></param>
		void SetModifiable(bool b);
	}
}
