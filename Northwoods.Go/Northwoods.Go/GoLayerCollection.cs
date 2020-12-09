using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// An ordered collection of layers.
	/// </summary>
	[Serializable]
	public sealed class GoLayerCollection : ICollection<GoLayer>, IEnumerable<GoLayer>, IEnumerable
	{
		[Serializable]
		internal sealed class ZOrderComparer : IComparer<GoObject>
		{
			private GoLayerCollection myLayers;

			internal ZOrderComparer(GoLayerCollection layers)
			{
				myLayers = layers;
			}

			public int Compare(GoObject a, GoObject b)
			{
				if (a == null || b == null || a == b)
				{
					return 0;
				}
				if (a.Layer != b.Layer)
				{
					int num = myLayers.IndexOf(a.Layer);
					int num2 = myLayers.IndexOf(b.Layer);
					if (num < num2)
					{
						return -1;
					}
					if (num > num2)
					{
						return 1;
					}
				}
				if (a.Layer == null)
				{
					return 0;
				}
				GoLayer layer = a.Layer;
				int num3 = layer.IndexOf(a.TopLevelObject);
				int num4 = layer.IndexOf(b.TopLevelObject);
				if (num3 < num4)
				{
					return -1;
				}
				if (num3 > num4)
				{
					return 1;
				}
				return AFirst(a.TopLevelObject, a, b);
			}

			private int AFirst(GoObject obj, GoObject a, GoObject b)
			{
				if (obj == a)
				{
					return -1;
				}
				if (obj == b)
				{
					return 1;
				}
				GoGroup goGroup = obj as GoGroup;
				if (goGroup != null)
				{
					foreach (GoObject item in goGroup.GetEnumerator())
					{
						int num = AFirst(item, a, b);
						if (num != 0)
						{
							return num;
						}
					}
				}
				return 0;
			}
		}

		private static readonly RectangleF NullRect;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int InsertedLayer = 801;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int RemovedLayer = 802;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int MovedLayer = 803;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int ChangedDefault = 804;

		private IGoLayerCollectionContainer myLayerCollectionContainer;

		private bool myIsInDocument;

		private List<GoLayer> myLayers = new List<GoLayer>();

		private GoLayer myDefaultLayer;

		[NonSerialized]
		private ZOrderComparer myComparer;

		/// <summary>
		/// Get the number of layers.
		/// </summary>
		public int Count => myLayers.Count;

		/// <summary>
		/// This collection is never read-only programmatically.
		/// </summary>
		public bool IsReadOnly => false;

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the layers in reverse order.
		/// </summary>
		public GoLayerCollectionEnumerator Backwards => new GoLayerCollectionEnumerator(myLayers, forward: false);

		/// <summary>
		/// Gets the document or view to which this layer collection belongs.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.Document" />
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.View" />
		public IGoLayerCollectionContainer LayerCollectionContainer => myLayerCollectionContainer;

		internal bool IsInDocument => myIsInDocument;

		/// <summary>
		/// Gets the document that this layer collection belongs to, or null if this is in a view.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.View" />
		public GoDocument Document => myLayerCollectionContainer as GoDocument;

		/// <summary>
		/// Gets the view that this layer collection belongs to, or null if this is in a document.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayerCollection.Document" />
		public GoView View => myLayerCollectionContainer as GoView;

		/// <summary>
		/// Gets the first layer, normally drawn underneath all other layers.
		/// </summary>
		public GoLayer Bottom => myLayers[0];

		/// <summary>
		/// Gets the last layer, normally drawn on top of all other layers.
		/// </summary>
		public GoLayer Top => myLayers[checked(Count - 1)];

		/// <summary>
		/// Gets or sets the default layer for adding objects.
		/// </summary>
		/// <value>
		/// The <see cref="T:Northwoods.Go.GoLayer" /> must belong to the same <see cref="P:Northwoods.Go.GoLayerCollection.LayerCollectionContainer" />
		/// and must not be null.
		/// Initially this value is the first and only layer, created when this collection
		/// was initialized.
		/// </value>
		public GoLayer Default
		{
			get
			{
				return myDefaultLayer;
			}
			set
			{
				GoLayer goLayer = myDefaultLayer;
				if (goLayer != value)
				{
					if (value == null || value.LayerCollectionContainer != LayerCollectionContainer)
					{
						throw new ArgumentException("The new GoLayerCollection.Default layer must belong to the same document or view.");
					}
					myDefaultLayer = value;
					LayerCollectionContainer.RaiseChanged(804, 0, null, 0, goLayer, NullRect, 0, value, NullRect);
				}
			}
		}

		internal GoLayerCollection()
		{
		}

		internal void init(IGoLayerCollectionContainer lcc)
		{
			myLayerCollectionContainer = lcc;
			myIsInDocument = (myLayerCollectionContainer is GoDocument);
			myDefaultLayer = new GoLayer();
			myDefaultLayer.init(myLayerCollectionContainer);
			myLayers.Add(myDefaultLayer);
			UpdateLayerIndices();
			myDefaultLayer.Identifier = 0;
		}

		/// <summary>
		/// Copy references to all of the layers in this collection into an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(GoLayer[] array, int index)
		{
			myLayers.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new GoLayerCollectionEnumerator(myLayers, forward: true);
		}

		IEnumerator<GoLayer> IEnumerable<GoLayer>.GetEnumerator()
		{
			return new GoLayerCollectionEnumerator(myLayers, forward: true);
		}

		/// <summary>
		/// Get an Enumerator that iterates over all of the layers of this collection.
		/// </summary>
		/// <returns></returns>
		public GoLayerCollectionEnumerator GetEnumerator()
		{
			return new GoLayerCollectionEnumerator(myLayers, forward: true);
		}

		/// <summary>
		/// Returns a newly allocated array holding references to all of the layers.
		/// </summary>
		public GoLayer[] CopyArray()
		{
			return myLayers.ToArray();
		}

		/// <summary>
		/// Create a new <see cref="T:Northwoods.Go.GoLayer" /> positioned after (i.e. in front of) an existing layer.
		/// </summary>
		/// <param name="dest">an existing layer in this collection, or null to signify the <see cref="P:Northwoods.Go.GoLayerCollection.Top" /> layer</param>
		/// <returns>The new <see cref="T:Northwoods.Go.GoLayer" />.</returns>
		/// <remarks>
		/// The new layer will be owned by this collection's <see cref="P:Northwoods.Go.GoLayerCollection.LayerCollectionContainer" />,
		/// which may be either a <see cref="T:Northwoods.Go.GoDocument" /> or a <see cref="T:Northwoods.Go.GoView" />.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" /> hint.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerBefore(Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.Remove(Northwoods.Go.GoLayer)" />
		public GoLayer CreateNewLayerAfter(GoLayer dest)
		{
			if (dest != null && IndexOf(dest) < 0)
			{
				throw new ArgumentException("Cannot create a new layer after a layer that is not in this layer collection.");
			}
			GoLayer goLayer = new GoLayer();
			goLayer.init(LayerCollectionContainer);
			InsertAfter(dest, goLayer);
			goLayer.Identifier = FindUniqueIdentifier();
			return goLayer;
		}

		/// <summary>
		/// Create a new <see cref="T:Northwoods.Go.GoLayer" /> positioned before (i.e. behind) an existing layer.
		/// </summary>
		/// <param name="dest">an existing layer in this collection, or null to signify the <see cref="P:Northwoods.Go.GoLayerCollection.Bottom" /> layer</param>
		/// <returns>The new <see cref="T:Northwoods.Go.GoLayer" />.</returns>
		/// <remarks>
		/// The new layer will be owned by this collection's <see cref="P:Northwoods.Go.GoLayerCollection.LayerCollectionContainer" />,
		/// which may be either a <see cref="T:Northwoods.Go.GoDocument" /> or a <see cref="T:Northwoods.Go.GoView" />.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" /> hint.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerAfter(Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.Remove(Northwoods.Go.GoLayer)" />
		public GoLayer CreateNewLayerBefore(GoLayer dest)
		{
			if (dest != null && IndexOf(dest) < 0)
			{
				throw new ArgumentException("Cannot create a new layer before a layer that is not in this layer collection.");
			}
			GoLayer goLayer = new GoLayer();
			goLayer.init(LayerCollectionContainer);
			InsertBefore(dest, goLayer);
			goLayer.Identifier = FindUniqueIdentifier();
			return goLayer;
		}

		internal void InsertAfter(GoLayer dest, GoLayer newlayer)
		{
			if (dest == null)
			{
				dest = Top;
			}
			int num = IndexOf(dest);
			checked
			{
				if (num >= 0)
				{
					myLayers.Insert(num + 1, newlayer);
					UpdateLayerIndices();
					LayerCollectionContainer.RaiseChanged(801, 1, newlayer, num, dest, NullRect, num + 1, newlayer, NullRect);
				}
			}
		}

		internal void InsertBefore(GoLayer dest, GoLayer newlayer)
		{
			if (dest == null)
			{
				dest = Bottom;
			}
			int num = IndexOf(dest);
			if (num >= 0)
			{
				myLayers.Insert(num, newlayer);
				UpdateLayerIndices();
				LayerCollectionContainer.RaiseChanged(801, 0, newlayer, num, dest, NullRect, num, newlayer, NullRect);
			}
		}

		/// <summary>
		/// Add a reference to a document layer into this view's collection of layers.
		/// </summary>
		/// <param name="dest">A layer already in this view's collection of layers.</param>
		/// <param name="doclayer">A layer owned by this view's document.</param>
		/// <remarks>
		/// By allowing a view's collection of layers to include both view layers owned by
		/// the view and document layers owned by the view's document, we permit each view on
		/// a document to display a different set of document layers, perhaps in different
		/// orders.
		/// This functionality is called by <see cref="M:Northwoods.Go.GoView.InitializeLayersFromDocument" />.
		/// It is an error if this collection does not belong to a <see cref="T:Northwoods.Go.GoView" />,
		/// or if <paramref name="doclayer" /> does not belong to this view's <see cref="P:Northwoods.Go.GoView.Document" />.
		/// The document layer continues to be owned by the document; the view just acquires
		/// a shared reference to that layer.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" /> hint.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.InsertDocumentLayerBefore(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerAfter(Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.Remove(Northwoods.Go.GoLayer)" />
		public void InsertDocumentLayerAfter(GoLayer dest, GoLayer doclayer)
		{
			if (IndexOf(doclayer) >= 0)
			{
				MoveAfter(dest, doclayer);
				return;
			}
			if (dest != null && IndexOf(dest) < 0)
			{
				throw new ArgumentException("Cannot insert a document layer after a layer that is not in this layer collection.");
			}
			GoView view = View;
			if (view == null)
			{
				throw new ArgumentException("Cannot insert a layer into a document layer collection.");
			}
			if (doclayer == null || !doclayer.IsInDocument || view.Document != doclayer.Document)
			{
				throw new ArgumentException("Layer to be inserted into a view layer collection must be a document layer in the view's document.");
			}
			InsertAfter(dest, doclayer);
		}

		/// <summary>
		/// Add a reference to a document layer into this view's collection of layers.
		/// </summary>
		/// <param name="dest">A layer already in this view's collection of layers.</param>
		/// <param name="doclayer">A layer owned by this view's document.</param>
		/// <remarks>
		/// It is an error if this collection does not belong to a <see cref="T:Northwoods.Go.GoView" />,
		/// or if <paramref name="doclayer" /> does not belong to this view's <see cref="P:Northwoods.Go.GoView.Document" />.
		/// The document layer continues to be owned by the document; the view just acquires
		/// a shared reference to that layer.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" /> hint.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.InsertDocumentLayerAfter(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerBefore(Northwoods.Go.GoLayer)" />
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.Remove(Northwoods.Go.GoLayer)" />
		public void InsertDocumentLayerBefore(GoLayer dest, GoLayer doclayer)
		{
			if (IndexOf(doclayer) >= 0)
			{
				MoveBefore(dest, doclayer);
				return;
			}
			if (dest != null && IndexOf(dest) < 0)
			{
				throw new ArgumentException("Cannot insert a document layer before a layer that is not in this layer collection.");
			}
			GoView view = View;
			if (view == null)
			{
				throw new ArgumentException("Cannot insert a layer into a document layer collection.");
			}
			if (doclayer == null || !doclayer.IsInDocument || view.Document != doclayer.Document)
			{
				throw new ArgumentException("Layer to be inserted into a view layer collection must be a document layer in the view's document.");
			}
			InsertBefore(dest, doclayer);
		}

		/// <summary>
		/// Adding arbitrary layers to this collection is not supported.
		/// Use <see cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerBefore(Northwoods.Go.GoLayer)" />, <see cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerAfter(Northwoods.Go.GoLayer)" />,
		/// <see cref="M:Northwoods.Go.GoLayerCollection.InsertDocumentLayerBefore(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer)" />, or <see cref="M:Northwoods.Go.GoLayerCollection.InsertDocumentLayerAfter(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer)" /> instead.
		/// </summary>
		/// <param name="layer"></param>
		public void Add(GoLayer layer)
		{
			throw new InvalidOperationException("GoLayerCollection.Add");
		}

		/// <summary>
		/// Removing all layers from this collection is not supported.
		/// At least one layer must remain.
		/// </summary>
		public void Clear()
		{
			throw new InvalidOperationException("GoLayerCollection.Clear");
		}

		/// <summary>
		/// A predicate to see if a particular <see cref="T:Northwoods.Go.GoLayer" /> is a member of this collection.
		/// </summary>
		/// <param name="layer">a <see cref="T:Northwoods.Go.GoLayer" /></param>
		/// <returns></returns>
		public bool Contains(GoLayer layer)
		{
			return IndexOf(layer) >= 0;
		}

		/// <summary>
		/// Remove a layer from this collection.
		/// </summary>
		/// <param name="layer"></param>
		/// <remarks>
		/// If the layer is owned by this collection's container, this method
		/// calls <see cref="M:Northwoods.Go.GoLayer.Clear" /> on <paramref name="layer" />.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.RemovedLayer" /> hint.
		/// </remarks>
		public bool Remove(GoLayer layer)
		{
			if (layer == null)
			{
				return false;
			}
			int num = IndexOf(layer);
			if (num < 0)
			{
				return false;
			}
			bool flag = layer.LayerCollectionContainer == LayerCollectionContainer;
			if (flag)
			{
				layer.Clear();
			}
			GoLayer goLayer = null;
			foreach (GoLayer myLayer in myLayers)
			{
				if (myLayer != layer && myLayer.LayerCollectionContainer == LayerCollectionContainer)
				{
					goLayer = myLayer;
					break;
				}
			}
			if (goLayer == null)
			{
				return false;
			}
			GoLayer oldVal = null;
			checked
			{
				if (num + 1 < myLayers.Count)
				{
					oldVal = myLayers[num + 1];
				}
				if (flag)
				{
					layer.LayerCollectionIndex = -1;
				}
				myLayers.RemoveAt(num);
				UpdateLayerIndices();
				try
				{
					LayerCollectionContainer.RaiseChanged(802, 0, layer, 0, oldVal, NullRect, 0, null, NullRect);
				}
				finally
				{
					if (layer == Default)
					{
						Default = goLayer;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Reorder the layers in this collection by moving a layer to be immediately
		/// before (i.e. behind) another layer.
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="moving"></param>
		/// <remarks>
		/// Both layer arguments must be present in this collection.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.MovedLayer" /> hint.
		/// </remarks>
		public void MoveBefore(GoLayer dest, GoLayer moving)
		{
			if (moving == null || Count <= 1)
			{
				return;
			}
			int num = -1;
			if (dest == null)
			{
				dest = Bottom;
				num = 0;
				if (dest == moving)
				{
					return;
				}
			}
			else
			{
				num = IndexOf(dest);
				if (num < 0)
				{
					throw new ArgumentException("MoveBefore destination layer must be in the GoLayerCollection");
				}
			}
			int num2 = IndexOf(moving);
			if (num2 < 0)
			{
				throw new ArgumentException("MoveBefore layer to be moved must be in the GoLayerCollection");
			}
			checked
			{
				if (num - 1 != num2 && num != num2)
				{
					if (num > num2)
					{
						num--;
					}
					moveInCollection(num, moving, num2, undoing: false);
				}
			}
		}

		/// <summary>
		/// Reorder the layers in this collection by moving a layer to be immediately
		/// after (i.e. in front of) another layer.
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="moving"></param>
		/// <remarks>
		/// Both layer arguments must be present in this collection.
		/// The <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// call gets a <see cref="F:Northwoods.Go.GoLayerCollection.MovedLayer" /> hint.
		/// </remarks>
		public void MoveAfter(GoLayer dest, GoLayer moving)
		{
			if (moving == null || Count <= 1)
			{
				return;
			}
			int num = -1;
			checked
			{
				if (dest == null)
				{
					dest = Top;
					num = Count - 1;
					if (dest == moving)
					{
						return;
					}
				}
				else
				{
					num = IndexOf(dest);
					if (num < 0)
					{
						throw new ArgumentException("MoveAfter destination layer must be in the GoLayerCollection");
					}
				}
				int num2 = IndexOf(moving);
				if (num2 < 0)
				{
					throw new ArgumentException("MoveAfter layer to be moved must be in the GoLayerCollection");
				}
				if (num + 1 != num2 && num != num2)
				{
					if (num > num2)
					{
						num--;
					}
					moveInCollection(num + 1, moving, num2, undoing: false);
				}
			}
		}

		internal void moveInCollection(int newidx, GoLayer moving, int oldidx, bool undoing)
		{
			if (oldidx >= 0 && oldidx < myLayers.Count && newidx >= 0 && newidx < myLayers.Count)
			{
				myLayers.RemoveAt(oldidx);
				myLayers.Insert(newidx, moving);
				UpdateLayerIndices();
			}
			LayerCollectionContainer.RaiseChanged(803, 0, this, oldidx, moving, NullRect, newidx, moving, NullRect);
		}

		/// <summary>
		/// Return the next layer in the Z-order, given a layer.
		/// </summary>
		/// <param name="layer">a <see cref="T:Northwoods.Go.GoLayer" /> that is in this layer collection</param>
		/// <param name="relativeZorder">
		/// a value of <c>1</c> returns the next layer (just in front of <paramref name="layer" />);
		/// a value of <c>-1</c> returns the previous layer (just behind <paramref name="layer" />)
		/// </param>
		/// <returns>
		/// null if <paramref name="layer" /> is null or does not belong to this layer collection
		/// or if there is no layer at the Z-order position indicated by <paramref name="relativeZorder" />.
		/// </returns>
		public GoLayer NextLayer(GoLayer layer, int relativeZorder)
		{
			if (layer == null)
			{
				return null;
			}
			int num = IndexOf(layer);
			if (num < 0)
			{
				return null;
			}
			num = checked(num + relativeZorder);
			if (num < 0 || num >= Count)
			{
				return null;
			}
			return myLayers[num];
		}

		/// <summary>
		/// Search for a layer with a particular identifier.
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		/// <seealso cref="P:Northwoods.Go.GoLayer.Identifier" />
		public GoLayer Find(object identifier)
		{
			if (identifier == null)
			{
				return null;
			}
			foreach (GoLayer backward in Backwards)
			{
				object identifier2 = backward.Identifier;
				if (identifier2 != null && identifier2.Equals(identifier))
				{
					return backward;
				}
			}
			return null;
		}

		private object FindUniqueIdentifier()
		{
			int i;
			for (i = Count; Find(i) != null; i = checked(i + 1))
			{
			}
			return i;
		}

		/// <summary>
		/// Sort an <c>Array</c> of <see cref="T:Northwoods.Go.GoObject" />s by their Z-order position in these layers.
		/// </summary>
		/// <param name="a">an array of <see cref="T:Northwoods.Go.GoObject" />s in layers of this layer collection; this array is modified</param>
		/// <remarks>
		/// <para>
		/// This method does not modify the order of any objects in any layer--it simply
		/// modifies the argument array by sorting the objects by their current Z-order in these layers.
		/// For example, if you have a few selected objects, you can use this method to find the
		/// selected object which is in front of the other selected objects.  That would be the last
		/// object in the array after calling this method.
		/// </para>
		/// <para>
		/// If one or more elements of the argument array <paramref name="a" /> are not
		/// <see cref="T:Northwoods.Go.GoObject" />s that belong to one of these layers, the resulting sort order is indeterminate.
		/// In many circumstances you can easily create an array of <c>GoObject</c>s by calling
		/// <see cref="M:Northwoods.Go.GoLayerCollection.CopyArray" />.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayerCollection.SortByZOrder(Northwoods.Go.GoCollection)" />
		public void SortByZOrder(GoObject[] a)
		{
			if (a.Length > 1)
			{
				ZOrderComparer comparer = GetComparer();
				Array.Sort(a, 0, a.Length, comparer);
			}
		}

		/// <summary>
		/// Modify the order of the objects in the given <see cref="T:Northwoods.Go.GoCollection" /> <paramref name="c" />
		/// to be in their Z-order position in the layers of this collection.
		/// </summary>
		/// <param name="c">a <see cref="T:Northwoods.Go.GoCollection" /> that will be modified</param>
		/// <remarks>See the remarks for <see cref="M:Northwoods.Go.GoLayerCollection.SortByZOrder(Northwoods.Go.GoObject[])" />.</remarks>
		public void SortByZOrder(GoCollection c)
		{
			if (!c.IsEmpty)
			{
				ZOrderComparer comparer = GetComparer();
				c.Sort(comparer);
			}
		}

		private ZOrderComparer GetComparer()
		{
			if (myComparer == null)
			{
				myComparer = new ZOrderComparer(this);
			}
			using (GoLayerCollectionEnumerator goLayerCollectionEnumerator = GetEnumerator())
			{
				while (goLayerCollectionEnumerator.MoveNext())
				{
					goLayerCollectionEnumerator.Current.InitializeIndices();
				}
			}
			return myComparer;
		}

		internal GoLayer LayerAt(int i)
		{
			return myLayers[i];
		}

		internal int IndexOf(GoLayer layer)
		{
			if (IsInDocument)
			{
				return layer.LayerCollectionIndex;
			}
			if (layer.IsInView)
			{
				return layer.LayerCollectionIndex;
			}
			return myLayers.IndexOf(layer);
		}

		private void UpdateLayerIndices()
		{
			for (int i = 0; i < myLayers.Count; i = checked(i + 1))
			{
				GoLayer goLayer = myLayers[i];
				if (IsInDocument)
				{
					goLayer.LayerCollectionIndex = i;
				}
				else if (goLayer.IsInView)
				{
					goLayer.LayerCollectionIndex = i;
				}
			}
		}

		/// <summary>
		/// Get an Enumerator that iterates over all of the objects in all of the layers
		/// in this collection, in the desired order.
		/// </summary>
		/// <param name="forward"></param>
		/// <returns></returns>
		public GoLayerCollectionObjectEnumerator GetObjectEnumerator(bool forward)
		{
			return new GoLayerCollectionObjectEnumerator(myLayers, forward);
		}
	}
}
