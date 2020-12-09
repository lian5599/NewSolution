using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// GoDocument and GoView implement this interface for holding a GoLayerCollection collection.
	/// </summary>
	public interface IGoLayerCollectionContainer : IGoLayerAbilities
	{
		/// <summary>
		/// Gets the collection of layers.
		/// </summary>
		GoLayerCollection Layers
		{
			get;
		}

		/// <summary>
		/// This method is called when one of a document's or a view's
		/// layer collection, layer, or object is modified.
		/// </summary>
		/// <param name="hint">identifies the kind of change</param>
		/// <param name="subhint">identifies a particular kind of change for the <paramref name="hint" /></param>
		/// <param name="obj">helps identify the change</param>
		/// <param name="oldI">specifies an old/previous integer value</param>
		/// <param name="oldVal">specifies an old/previous object value</param>
		/// <param name="oldRect">specifies an old/previous float, <c>PointF</c>, <c>SizeF</c>, or <c>RectangleF</c> value</param>
		/// <param name="newI">specifies a new integer value</param>
		/// <param name="newVal">specifies a new object value</param>
		/// <param name="newRect">specifies a new float, <c>PointF</c>, <c>SizeF</c>, or <c>RectangleF</c> value</param>
		void RaiseChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);
	}
}
