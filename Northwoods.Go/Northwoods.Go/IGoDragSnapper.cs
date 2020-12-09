using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface should be implemented by those <see cref="T:Northwoods.Go.GoObject" />s that
	/// want to act like grids for affecting how objects are dragged or resized in a <see cref="T:Northwoods.Go.GoView" />.
	/// </summary>
	/// <remarks>
	/// <see cref="T:Northwoods.Go.GoGrid" /> implements this interface.
	/// </remarks>
	public interface IGoDragSnapper
	{
		/// <summary>
		/// Gets whether <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />
		/// should look at any <see cref="T:Northwoods.Go.IGoDragSnapper" /> that might be behind this one.
		/// </summary>
		bool SnapOpaque
		{
			get;
		}

		/// <summary>
		/// This predicate is called by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /> to
		/// decide whether this grid-like object should take part.
		/// </summary>
		/// <param name="p">a <c>PointF</c> in document coordinates</param>
		/// <param name="obj">the <see cref="T:Northwoods.Go.GoObject" /> being moved or resized</param>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that is moving or resizing an object</param>
		/// <returns>true if the point <paramref name="p" /> is in (or near) this object and
		/// if <see cref="M:Northwoods.Go.IGoDragSnapper.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /> should be called</returns>
		/// <remarks>
		/// An implementation of this predicate should probably return false if
		/// <see cref="M:Northwoods.Go.GoObject.CanView" /> returns false.
		/// It also should probably return false if this object is part of the
		/// <paramref name="view" />'s <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		bool CanSnapPoint(PointF p, GoObject obj, GoView view);

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" /> to
		/// find the point to which an object should be moved or resized.
		/// </summary>
		/// <param name="p">a <c>PointF</c> in document coordinates</param>
		/// <param name="obj">the <see cref="T:Northwoods.Go.GoObject" /> being moved or resized</param>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> that is moving or resizing an object</param>
		/// <returns></returns>
		PointF SnapPoint(PointF p, GoObject obj, GoView view);
	}
}
