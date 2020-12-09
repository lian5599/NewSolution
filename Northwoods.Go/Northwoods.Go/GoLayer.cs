using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This represents a collection of objects that are to be drawn behind or in front of
	/// objects in other layers.
	/// </summary>
	[Serializable]
	public sealed class GoLayer : IGoCollection, ICollection<GoObject>, IEnumerable<GoObject>, IEnumerable, IGoLayerAbilities
	{
		internal sealed class GoLayerCache
		{
			private GoView myView;

			private List<GoObject> myObjects;

			private RectangleF myRect;

			private List<GoStroke> myStrokes;

			private List<IGoDragSnapper> mySnappers;

			internal GoView View => myView;

			internal List<GoObject> Objects => myObjects;

			internal List<GoStroke> Strokes => myStrokes;

			internal List<IGoDragSnapper> Snappers => mySnappers;

			internal RectangleF Rect
			{
				get
				{
					return myRect;
				}
				set
				{
					myRect = value;
				}
			}

			internal GoLayerCache(GoView view)
			{
				myView = view;
				myObjects = new List<GoObject>();
				myStrokes = new List<GoStroke>();
				mySnappers = new List<IGoDragSnapper>();
			}

			internal void Reset()
			{
				myObjects.Clear();
				myStrokes.Clear();
				mySnappers.Clear();
				myRect = new RectangleF(0f, 0f, 0f, 0f);
			}
		}

		[Serializable]
		internal sealed class ZComparer : IComparer<IGoDragSnapper>
		{
			internal ZComparer()
			{
			}

			public int Compare(IGoDragSnapper x, IGoDragSnapper y)
			{
				if (x == null || y == null || x == y)
				{
					return 0;
				}
				GoObject goObject = (GoObject)x;
				GoObject goObject2 = (GoObject)y;
				GoLayer layer = goObject.Layer;
				GoObject topLevelObject = goObject.TopLevelObject;
				int num = layer.IndexOf(topLevelObject);
				int num2 = layer.IndexOf(goObject2.TopLevelObject);
				if (num < num2)
				{
					return -1;
				}
				if (num > num2)
				{
					return 1;
				}
				return AFirst(topLevelObject, goObject, goObject2);
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

		private static readonly RectangleF NullRect = default(RectangleF);

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint used for all property changes to <see cref="T:Northwoods.Go.GoObject" />s.
		/// </summary>
		public const int ChangedObject = 901;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int InsertedObject = 902;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int RemovedObject = 903;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int ChangedObjectLayer = 904;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int ChangedObjectZOrder = 905;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowView" /> property.
		/// </summary>
		public const int ChangedAllowView = 910;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowSelect" /> property.
		/// </summary>
		public const int ChangedAllowSelect = 911;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowMove" /> property.
		/// </summary>
		public const int ChangedAllowMove = 912;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowCopy" /> property.
		/// </summary>
		public const int ChangedAllowCopy = 913;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowResize" /> property.
		/// </summary>
		public const int ChangedAllowResize = 914;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowReshape" /> property.
		/// </summary>
		public const int ChangedAllowReshape = 915;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowDelete" /> property.
		/// </summary>
		public const int ChangedAllowDelete = 916;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowInsert" /> property.
		/// </summary>
		public const int ChangedAllowInsert = 917;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowLink" /> property.
		/// </summary>
		public const int ChangedAllowLink = 918;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowEdit" /> property.
		/// </summary>
		public const int ChangedAllowEdit = 919;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.AllowPrint" /> property.
		/// </summary>
		public const int ChangedAllowPrint = 920;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoLayer.Identifier" /> property.
		/// </summary>
		public const int ChangedIdentifier = 930;

		private static IComparer<IGoDragSnapper> myZComparer = new ZComparer();

		private IGoLayerCollectionContainer myLayerCollectionContainer;

		private bool myIsInDocument;

		private GoDocument myDocument;

		[NonSerialized]
		private GoView myView;

		private List<GoObject> myObjects = new List<GoObject>();

		private bool myAllowView = true;

		private bool myAllowPrint = true;

		private bool myAllowSelect = true;

		private bool myAllowMove = true;

		private bool myAllowCopy = true;

		private bool myAllowResize = true;

		private bool myAllowReshape = true;

		private bool myAllowDelete = true;

		private bool myAllowInsert = true;

		private bool myAllowLink = true;

		private bool myAllowEdit = true;

		private object myIdentifier;

		[NonSerialized]
		private List<GoLayerCache> myCaches;

		[NonSerialized]
		private bool myCachedPick;

		[NonSerialized]
		private bool myCachedPickSelectable;

		[NonSerialized]
		private PointF myCachedPickPoint;

		[NonSerialized]
		private GoObject myCachedPickObject;

		private bool myValidIndices;

		private int myLayerCollectionIndex = -1;

		/// <summary>
		/// This predicate is true when there are no objects in this collection.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.IsEmpty" />
		[Browsable(false)]
		public bool IsEmpty => myObjects.Count == 0;

		/// <summary>
		/// Gets the number of objects in this layer.
		/// </summary>
		[Description("The number of objects in this layer.")]
		public int Count => myObjects.Count;

		/// <summary>
		/// This collection is never read-only programmatically,
		/// but might not be modifiable by the user: <see cref="M:Northwoods.Go.GoLayer.SetModifiable(System.Boolean)" />.
		/// </summary>
		public bool IsReadOnly => false;

		IEnumerable<GoObject> IGoCollection.Backwards => new GoLayerEnumerator(myObjects, forward: false);

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the GoObjects in reverse order.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.Backwards" />
		[Browsable(false)]
		public GoLayerEnumerator Backwards => new GoLayerEnumerator(myObjects, forward: false);

		/// <summary>
		/// Gets the document or view to which this layer belongs.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayer.IsInDocument" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.Document" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.IsInView" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.View" />
		[Browsable(false)]
		public IGoLayerCollectionContainer LayerCollectionContainer => myLayerCollectionContainer;

		internal int LayerCollectionIndex
		{
			get
			{
				return myLayerCollectionIndex;
			}
			set
			{
				myLayerCollectionIndex = value;
			}
		}

		/// <summary>
		/// Gets whether this layer belongs to a document.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayer.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.Document" />
		[Browsable(false)]
		public bool IsInDocument => myIsInDocument;

		/// <summary>
		/// Gets the document that this layer belongs to, or null if this is a view layer.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayer.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.IsInDocument" />
		[Category("Ownership")]
		[Description("The document in which this layer belongs.")]
		public GoDocument Document => myDocument;

		/// <summary>
		/// Gets whether this layer belongs to a view.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayer.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.View" />
		[Browsable(false)]
		public bool IsInView => !myIsInDocument;

		/// <summary>
		/// Gets the view that this layer belongs to, or null if this is a document layer.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoLayer.LayerCollectionContainer" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.IsInView" />
		[Category("Ownership")]
		[Description("The view in which this layer belongs.")]
		public GoView View => myView;

		/// <summary>
		/// Gets or sets whether the user can see objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from seeing objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// seeable by the user because the object is not visible.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanViewObjects" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Visible" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can see the objects in this layer.")]
		public bool AllowView
		{
			get
			{
				return myAllowView;
			}
			set
			{
				bool flag = myAllowView;
				if (flag != value)
				{
					myAllowView = value;
					LayerCollectionContainer.RaiseChanged(910, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the objects in this layer will be printed.
		/// </summary>
		/// <remarks>
		/// A false value prevents the view from printing objects in this layer.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanPrintObjects" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanViewObjects" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the view can print the objects in this layer.")]
		public bool AllowPrint
		{
			get
			{
				return myAllowPrint;
			}
			set
			{
				bool flag = myAllowPrint;
				if (flag != value)
				{
					myAllowPrint = value;
					LayerCollectionContainer.RaiseChanged(920, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can select objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from selecting objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// selectable by the user because the object or its document disallows it,
		/// or because the view disallows it, or because the object is not visible.
		/// Your code can always select objects programmatically by calling
		/// <c>aView.Selection.Select(obj)</c> or <c>aView.Selection.Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanSelectObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowSelect" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Selectable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can select objects in this layer.")]
		public bool AllowSelect
		{
			get
			{
				return myAllowSelect;
			}
			set
			{
				bool flag = myAllowSelect;
				if (flag != value)
				{
					myAllowSelect = value;
					LayerCollectionContainer.RaiseChanged(911, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can move selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from moving objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// movable by the user because the object or its document disallows it,
		/// or because the view disallows it.
		/// Your code can always move objects programmatically by calling
		/// <c>obj.Position = newPos</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanMoveObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowMove" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Movable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can move the selected objects in this layer.")]
		public bool AllowMove
		{
			get
			{
				return myAllowMove;
			}
			set
			{
				bool flag = myAllowMove;
				if (flag != value)
				{
					myAllowMove = value;
					LayerCollectionContainer.RaiseChanged(912, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can copy selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from copying objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// copyable by the user because the object or its document disallows it,
		/// or because the view disallows it.
		/// Your code can always copy objects programmatically by calling
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanCopyObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowCopy" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Copyable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can copy the selected objects in this layer.")]
		public bool AllowCopy
		{
			get
			{
				return myAllowCopy;
			}
			set
			{
				bool flag = myAllowCopy;
				if (flag != value)
				{
					myAllowCopy = value;
					LayerCollectionContainer.RaiseChanged(913, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can resize selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from resizing objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// resizable by the user because the object or its document disallows it,
		/// or because the view disallows it.
		/// Your code can always resize objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanResizeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowResize" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Resizable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can resize the selected objects in this layer.")]
		public bool AllowResize
		{
			get
			{
				return myAllowResize;
			}
			set
			{
				bool flag = myAllowResize;
				if (flag != value)
				{
					myAllowResize = value;
					LayerCollectionContainer.RaiseChanged(914, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can reshape resizable objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from reshaping objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// reshapable by the user because the object or its document disallows it,
		/// or because the view disallows it.
		/// Your code can always reshape objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanReshapeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowReshape" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Reshapable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can reshape the resizable objects in this layer.")]
		public bool AllowReshape
		{
			get
			{
				return myAllowReshape;
			}
			set
			{
				bool flag = myAllowReshape;
				if (flag != value)
				{
					myAllowReshape = value;
					LayerCollectionContainer.RaiseChanged(915, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can delete selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from deleting objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// deletable by the user because the object or its document disallows it,
		/// or because the view disallows it.
		/// Your code can always delete objects programmatically by calling
		/// <c>obj.Remove()</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanDeleteObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowDelete" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Deletable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can delete the selected objects in this layer.")]
		public bool AllowDelete
		{
			get
			{
				return myAllowDelete;
			}
			set
			{
				bool flag = myAllowDelete;
				if (flag != value)
				{
					myAllowDelete = value;
					LayerCollectionContainer.RaiseChanged(916, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can insert objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from inserting objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// insertable by the user because the document or view disallows it.
		/// Your code can always insert objects programmatically by calling
		/// <c>Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanInsertObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowInsert" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can insert objects in this layer.")]
		public bool AllowInsert
		{
			get
			{
				return myAllowInsert;
			}
			set
			{
				bool flag = myAllowInsert;
				if (flag != value)
				{
					myAllowInsert = value;
					LayerCollectionContainer.RaiseChanged(917, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can link objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from linking objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// linkable by the user because the document or the ports disallow it,
		/// or because the view disallows it.
		/// Your code can always link objects programmatically by calling
		/// <c>Document.LinksLayers.Add(newLink)</c>, where <c>newLink</c> is
		/// a newly created instance of a class like <see cref="T:Northwoods.Go.GoLink" /> or
		/// <see cref="T:Northwoods.Go.GoLabeledLink" /> whose <see cref="P:Northwoods.Go.IGoLink.FromPort" /> and
		/// <see cref="P:Northwoods.Go.IGoLink.ToPort" /> properties have been set to ports in
		/// the same document.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanLinkObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowLink" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can link ports in this layer.")]
		public bool AllowLink
		{
			get
			{
				return myAllowLink;
			}
			set
			{
				bool flag = myAllowLink;
				if (flag != value)
				{
					myAllowLink = value;
					LayerCollectionContainer.RaiseChanged(918, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can edit objects in this layer.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from editing objects in this layer
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// editable by the user because the document or the object disallows it,
		/// or because the view disallows it.
		/// Your code can always edit objects programmatically by calling
		/// <c>obj.DoBeginEdit(aView)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanEditObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowEdit" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can edit objects in this layer.")]
		public bool AllowEdit
		{
			get
			{
				return myAllowEdit;
			}
			set
			{
				bool flag = myAllowEdit;
				if (flag != value)
				{
					myAllowEdit = value;
					LayerCollectionContainer.RaiseChanged(919, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets an identifier for this layer.
		/// </summary>
		/// <value>
		/// The default value is null.  However, the initial, default layer for
		/// each <see cref="T:Northwoods.Go.GoLayerCollection" /> has as a default identifier an
		/// instance of <c>Integer</c> <c>0</c>.
		/// </value>
		/// <remarks>
		/// Typically identifiers will be <c>String</c>s, but could be other more
		/// complex serializable objects.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.MergeLayersFrom(Northwoods.Go.GoDocument)" />
		[DefaultValue(null)]
		[Description("An identifier for this layer.")]
		public object Identifier
		{
			get
			{
				return myIdentifier;
			}
			set
			{
				object obj = myIdentifier;
				if (obj != value)
				{
					myIdentifier = value;
					LayerCollectionContainer.RaiseChanged(930, 0, this, 0, obj, NullRect, 0, value, NullRect);
				}
			}
		}

		internal List<GoLayerCache> Caches
		{
			get
			{
				if (myCaches == null)
				{
					myCaches = new List<GoLayerCache>();
				}
				return myCaches;
			}
		}

		internal GoLayer()
		{
		}

		internal void init(IGoLayerCollectionContainer lcc)
		{
			myLayerCollectionContainer = lcc;
			myIsInDocument = (lcc is GoDocument);
			myDocument = (lcc as GoDocument);
			myView = (lcc as GoView);
		}

		/// <summary>
		/// Add an object to this layer.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// The <paramref name="obj" /> must not already belong to a different document or view, nor to a group.
		/// If the object already belongs to this layer, nothing happens.
		/// The object's <see cref="P:Northwoods.Go.GoObject.Layer" /> property will be changed to be this layer.
		/// If the object already belonged to a different layer in this same document or view,
		/// the Changed hint will be <see cref="F:Northwoods.Go.GoLayer.ChangedObjectLayer" />, otherwise it will be
		/// <see cref="F:Northwoods.Go.GoLayer.InsertedObject" />.
		/// </remarks>
		public void Add(GoObject obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj.Layer != null)
			{
				if (obj.Layer.LayerCollectionContainer != LayerCollectionContainer)
				{
					throw new ArgumentException("Cannot add an object to a layer when it is already part of a different document's or view's layer.");
				}
				if (obj.Parent != null)
				{
					throw new ArgumentException("Cannot add an object to a layer when it is part of a group.");
				}
				GoLayer layer = obj.Layer;
				if (layer != this)
				{
					changeLayer(obj, layer, undoing: false);
				}
			}
			else
			{
				if (obj.Parent != null)
				{
					obj.Parent.Remove(obj);
				}
				addToLayer(obj, undoing: false);
			}
		}

		internal void addToLayer(GoObject obj, bool undoing)
		{
			obj.LayerIndex = myObjects.Count;
			myObjects.Add(obj);
			obj.SetLayer(this, obj, undoing);
			InsertIntoCaches(obj);
			RectangleF bounds = obj.Bounds;
			LayerCollectionContainer.RaiseChanged(902, 0, obj, 0, null, NullRect, 0, this, bounds);
		}

		internal void changeLayer(GoObject obj, GoLayer oldLayer, bool undoing)
		{
			oldLayer.RemoveFromCaches(obj);
			int num = GoCollection.fastRemove(oldLayer.myObjects, obj);
			if (num < oldLayer.myObjects.Count)
			{
				oldLayer.ResetIndices();
			}
			obj.LayerIndex = myObjects.Count;
			myObjects.Add(obj);
			obj.SetLayer(this, obj, undoing);
			InsertIntoCaches(obj);
			RectangleF bounds = obj.Bounds;
			LayerCollectionContainer.RaiseChanged(904, 0, obj, num, oldLayer, bounds, -1, this, bounds);
		}

		internal void removeFromLayer(GoObject obj, bool undoing)
		{
			try
			{
				obj.SetBeingRemoved(value: true);
				RemoveFromCaches(obj);
				int num = GoCollection.fastRemove(myObjects, obj);
				if (num < myObjects.Count)
				{
					ResetIndices();
				}
				RectangleF bounds = obj.Bounds;
				LayerCollectionContainer.RaiseChanged(903, 0, obj, num, this, bounds, 0, null, NullRect);
			}
			finally
			{
				obj.SetLayer(null, obj, undoing);
				obj.SetBeingRemoved(value: false);
			}
		}

		internal void moveInLayer(int newidx, GoObject obj, int oldidx, bool undoing)
		{
			moveInLayerInternal(newidx, obj, oldidx);
			RectangleF bounds = obj.Bounds;
			LayerCollectionContainer.RaiseChanged(905, 0, obj, oldidx, obj, bounds, newidx, obj, bounds);
		}

		internal void moveInLayerInternal(int newidx, GoObject obj, int oldidx)
		{
			if (oldidx >= 0 && oldidx < myObjects.Count && newidx >= 0 && newidx < myObjects.Count)
			{
				myObjects.RemoveAt(oldidx);
				myObjects.Insert(newidx, obj);
				ResetIndices();
				ResetCaches();
			}
		}

		/// <summary>
		/// Make sure this layer no longer holds an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// It is an error if the object <paramref name="obj" /> currently belongs to a different layer.
		/// If the object already is not in this layer, this method does nothing.
		/// Just before changing the object's <see cref="P:Northwoods.Go.GoObject.Layer" /> property
		/// to null, the object's document or view will be notified with a Changed hint
		/// of <see cref="F:Northwoods.Go.GoLayer.RemovedObject" />.
		/// During that notification the object's <see cref="P:Northwoods.Go.GoObject.BeingRemoved" />
		/// property will be true.
		/// For convenience, if the object belongs to a <see cref="T:Northwoods.Go.GoGroup" />,
		/// this method will remove the object from the group.
		/// </remarks>
		public bool Remove(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			GoLayer layer = obj.Layer;
			if (layer == null)
			{
				return false;
			}
			if (layer == this)
			{
				GoGroup parent = obj.Parent;
				if (parent != null)
				{
					parent.Remove(obj);
				}
				else
				{
					removeFromLayer(obj, undoing: false);
				}
				return true;
			}
			throw new ArgumentException("Cannot remove an object from a layer if it does not belong to that layer.");
		}

		/// <summary>
		/// Change the Z-order of an object in this layer by moving it to be before (i.e. behind) another object.
		/// </summary>
		/// <param name="dest">
		/// the object in this layer that <paramref name="moving" /> will be placed before;
		/// if null, <paramref name="moving" /> will be painted as the first (i.e. rear-most) object in this layer.
		/// </param>
		/// <param name="moving">
		/// the top-level object in this layer whose Z-order will be changed
		/// </param>
		/// <remarks>
		/// This method signals an exception if <paramref name="moving" /> is not a top-level object in this layer,
		/// if <paramref name="dest" /> is not null and is not in this layer, or
		/// if you are trying to move an object before (i.e. behind) itself.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertBefore(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		public void MoveBefore(GoObject dest, GoObject moving)
		{
			if (moving == null || Count == 0)
			{
				return;
			}
			int num = -1;
			if (dest == null)
			{
				dest = myObjects[0];
				num = 0;
				if (dest == moving)
				{
					return;
				}
			}
			else
			{
				dest = dest.TopLevelObject;
				num = IndexOf(dest);
				if (num < 0)
				{
					throw new ArgumentException("MoveBefore destination object must be in the GoLayer");
				}
			}
			if (moving.Parent != null)
			{
				throw new ArgumentException("Cannot change Z-order of a child object; call GoGroup.InsertBefore instead");
			}
			int num2 = IndexOf(moving);
			if (num2 < 0)
			{
				throw new ArgumentException("MoveBefore object to be moved must be in the GoLayer");
			}
			checked
			{
				if (num - 1 != num2 && num != num2)
				{
					if (num > num2)
					{
						num--;
					}
					moveInLayer(num, moving, num2, undoing: false);
				}
			}
		}

		/// <summary>
		/// Change the Z-order of an object in this layer by moving it to be after (i.e. in front of) another object.
		/// </summary>
		/// <param name="dest">
		/// the object in this layer that <paramref name="moving" /> will be placed after;
		/// if null, <paramref name="moving" /> will be painted as the last (i.e. front-most) object in this layer.
		/// </param>
		/// <param name="moving">
		/// the top-level object in this layer whose Z-order will be changed
		/// </param>
		/// <remarks>
		/// This method signals an exception if <paramref name="moving" /> is not a top-level object in this layer,
		/// if <paramref name="dest" /> is not null and is not in this layer, or
		/// if you are trying to move an object after (i.e. in front of) itself.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.InsertAfter(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" />
		public void MoveAfter(GoObject dest, GoObject moving)
		{
			if (moving == null || Count == 0)
			{
				return;
			}
			int num = -1;
			checked
			{
				if (dest == null)
				{
					dest = myObjects[Count - 1];
					num = Count - 1;
					if (dest == moving)
					{
						return;
					}
				}
				else
				{
					dest = dest.TopLevelObject;
					num = IndexOf(dest);
					if (num < 0)
					{
						throw new ArgumentException("MoveAfter destination object must be in the GoLayer");
					}
				}
				if (moving.Parent != null)
				{
					throw new ArgumentException("Cannot change Z-order of a child object; call GoGroup.InsertAfter instead");
				}
				int num2 = IndexOf(moving);
				if (num2 < 0)
				{
					throw new ArgumentException("MoveAfter object to be moved must be in the GoLayer");
				}
				if (num + 1 != num2 && num != num2)
				{
					if (num > num2)
					{
						num--;
					}
					moveInLayer(num + 1, moving, num2, undoing: false);
				}
			}
		}

		/// <summary>
		/// Return the next object in the Z-order, given an object in this layer.
		/// </summary>
		/// <param name="obj">a <see cref="T:Northwoods.Go.GoObject" /> that is in this layer</param>
		/// <param name="relativeZorder">
		/// a value of <c>1</c> returns the next object (just in front of <paramref name="obj" />);
		/// a value of <c>-1</c> returns the previous object (just behind <paramref name="obj" />)
		/// </param>
		/// <returns>
		/// null if <paramref name="obj" /> is null or does not belong to this layer
		/// or if there is no object at the Z-order position indicated by <paramref name="relativeZorder" />.
		/// </returns>
		public GoObject NextObject(GoObject obj, int relativeZorder)
		{
			if (obj == null)
			{
				return null;
			}
			int num = IndexOf(obj.TopLevelObject);
			if (num < 0)
			{
				return null;
			}
			num = checked(num + relativeZorder);
			if (num < 0 || num >= Count)
			{
				return null;
			}
			return myObjects[num];
		}

		internal int IndexOf(GoObject obj)
		{
			if (obj.Layer != this)
			{
				throw new ArgumentException("GoObject doesn't belong to GoLayer");
			}
			if (myValidIndices)
			{
				return obj.LayerIndex;
			}
			return myObjects.IndexOf(obj);
		}

		internal void InitializeIndices()
		{
			if (!myValidIndices)
			{
				myValidIndices = true;
				for (int i = 0; i < myObjects.Count; i = checked(i + 1))
				{
					myObjects[i].LayerIndex = i;
				}
			}
		}

		private void ResetIndices()
		{
			myValidIndices = false;
		}

		/// <summary>
		/// Re-parent some objects to be top-level objects, even if they
		/// are part of groups in this same layer.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="reparentLinks">whether to make sure all connected links belong to the proper subgraph or layer</param>
		/// <returns>a collection of the objects that were added to this layer</returns>
		/// <remarks>
		/// This method tries to preserve the links connecting any ports of the objects
		/// being added to this layer.
		/// Just as with the <see cref="M:Northwoods.Go.GoLayer.Add(Northwoods.Go.GoObject)" /> method, you may find that you will want to 
		/// set the <see cref="P:Northwoods.Go.GoObject.Selectable" /> property to true for each of the objects
		/// being added to this layer.  The need for this depends on the behavior you want
		/// in your application.
		/// </remarks>
		public IGoCollection AddCollection(IGoCollection coll, bool reparentLinks)
		{
			GoCollection goCollection = new GoCollection();
			goCollection.InternalChecksForDuplicates = false;
			goCollection.AddRange(coll);
			foreach (GoObject item in goCollection)
			{
				if ((item.Layer != this || !item.IsTopLevel) && item.Layer != null)
				{
					GoGroup.setAllNoClear(item, b: true);
				}
			}
			foreach (GoObject item2 in goCollection)
			{
				if (item2.Layer != this || !item2.IsTopLevel)
				{
					item2.Remove();
					Add(item2);
				}
			}
			foreach (GoObject item3 in goCollection)
			{
				GoGroup.setAllNoClear(item3, b: false);
			}
			if (reparentLinks && IsInDocument)
			{
				GoSubGraphBase.ReparentAllLinksToSubGraphs(goCollection, behind: true, Document.LinksLayer);
			}
			return goCollection;
		}

		/// <summary>
		/// Determine if an object belongs to this layer.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This implementation currently depends on the <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// property, a "back-pointer".
		/// </remarks>
		public bool Contains(GoObject obj)
		{
			if (obj != null)
			{
				return obj.Layer == this;
			}
			return false;
		}

		/// <summary>
		/// Remove all objects from this layer.
		/// </summary>
		/// <remarks>
		/// This repeatedly calls <see cref="M:Northwoods.Go.GoLayer.Remove(Northwoods.Go.GoObject)" />.
		/// The default implementation tries to avoid duplicate removals,
		/// in case removing one object automatically removes another one.
		/// </remarks>
		public void Clear()
		{
			int num;
			for (num = myObjects.Count; num > 0; num = Math.Min(num, myObjects.Count))
			{
				GoObject obj = myObjects[num = checked(num - 1)];
				Remove(obj);
			}
		}

		/// <summary>
		/// Returns a newly allocated array of all of the GoObjects in this collection.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.IGoCollection.CopyArray" />
		public GoObject[] CopyArray()
		{
			return myObjects.ToArray();
		}

		/// <summary>
		/// Copy references to all of the objects in this collection into an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(GoObject[] array, int index)
		{
			myObjects.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new GoLayerEnumerator(myObjects, forward: true);
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			return new GoLayerEnumerator(myObjects, forward: true);
		}

		/// <summary>
		/// Get an GoLayerEnumerator that iterates over all objects in this layer.
		/// </summary>
		/// <returns></returns>
		public GoLayerEnumerator GetEnumerator()
		{
			return new GoLayerEnumerator(myObjects, forward: true);
		}

		/// <summary>
		/// Called to see if the user can see objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowView</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoLayer.Paint(System.Drawing.Graphics,Northwoods.Go.GoView,System.Drawing.RectangleF)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowView" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanView" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanPrintObjects" />
		public bool CanViewObjects()
		{
			return AllowView;
		}

		/// <summary>
		/// Called to see if the view should print objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowPrint</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoLayer.Paint(System.Drawing.Graphics,Northwoods.Go.GoView,System.Drawing.RectangleF)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowPrint" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanViewObjects" />
		public bool CanPrintObjects()
		{
			return AllowPrint;
		}

		/// <summary>
		/// Called to see if the user can select objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowSelect</c>, and, if this is a document layer,
		/// <c>Document.CanSelectObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />
		/// and <see cref="M:Northwoods.Go.GoLayer.PickObject(System.Drawing.PointF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowSelect" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanSelectObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanSelect" />
		public bool CanSelectObjects()
		{
			if (AllowSelect)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanSelectObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can move selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowMove</c>, and, if this is a document layer,
		/// <c>Document.CanMoveObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowMove" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanMoveObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanMove" />
		public bool CanMoveObjects()
		{
			if (AllowMove)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanMoveObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can copy selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowCopy</c>, and, if this is a document layer,
		/// <c>Document.CanCopyObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// and <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowCopy" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanCopyObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanCopy" />
		public bool CanCopyObjects()
		{
			if (AllowCopy)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanCopyObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can resize selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowResize</c>, and, if this is a document layer,
		/// <c>Document.CanResizeObjects</c>.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowResize" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanResizeObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanResize" />
		public bool CanResizeObjects()
		{
			if (AllowResize)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanResizeObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can reshape resizable objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowReshape</c>, and, if this is a document layer,
		/// <c>Document.CanReshapeObjects</c>.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowReshape" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanReshapeObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanReshape" />
		public bool CanReshapeObjects()
		{
			if (AllowReshape)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanReshapeObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can delete selected objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowDelete</c>, and, if this is a document layer,
		/// <c>Document.CanDeleteObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />
		/// and <see cref="M:Northwoods.Go.GoView.EditCut" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowDelete" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanDeleteObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanDelete" />
		public bool CanDeleteObjects()
		{
			if (AllowDelete)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanDeleteObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can insert objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowInsert</c>, and, if this is a document layer,
		/// <c>Document.CanInsertObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.EditPaste" /> and by
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowInsert" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanInsertObjects" />
		public bool CanInsertObjects()
		{
			if (AllowInsert)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanInsertObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can link objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowLink &amp;&amp; Document.CanLinkObjects</c>.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolLinking" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowLink" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanLinkObjects" />
		public bool CanLinkObjects()
		{
			if (AllowLink)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanLinkObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can edit objects in this layer.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowEdit &amp;&amp; Document.CanEditObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowEdit" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanEditObjects" />
		public bool CanEditObjects()
		{
			if (AllowEdit)
			{
				if (IsInDocument)
				{
					return LayerCollectionContainer.CanEditObjects();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// This method sets some properties that determine whether the user can
		/// modify any objects in this layer.
		/// </summary>
		/// <param name="b"></param>
		/// <remarks>
		/// By default this just sets the <see cref="P:Northwoods.Go.GoLayer.AllowMove" />, <see cref="P:Northwoods.Go.GoLayer.AllowResize" />, 
		/// <see cref="P:Northwoods.Go.GoLayer.AllowReshape" />, <see cref="P:Northwoods.Go.GoLayer.AllowDelete" />, <see cref="P:Northwoods.Go.GoLayer.AllowInsert" />, 
		/// <see cref="P:Northwoods.Go.GoLayer.AllowLink" />, and <see cref="P:Northwoods.Go.GoLayer.AllowEdit" /> properties.
		/// </remarks>
		public void SetModifiable(bool b)
		{
			AllowMove = b;
			AllowResize = b;
			AllowReshape = b;
			AllowDelete = b;
			AllowInsert = b;
			AllowLink = b;
			AllowEdit = b;
		}

		/// <summary>
		/// Render all of the visible objects in this layer within a rectangle to a <c>Graphics</c>.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="clipRect"></param>
		/// <remarks>
		/// This method calls <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> on each object in this
		/// layer whose <see cref="M:Northwoods.Go.GoObject.CanView" /> property is true and whose
		/// paint bounds, calculated by calling <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />,
		/// intersects with <paramref name="clipRect" />.
		/// If <see cref="P:Northwoods.Go.GoView.IsPrinting" /> is true, it calls <see cref="M:Northwoods.Go.GoObject.CanPrint" />
		/// on each object instead of calling <see cref="M:Northwoods.Go.GoObject.CanView" />.
		/// If <see cref="M:Northwoods.Go.GoLayer.CanViewObjects" /> is false,
		/// or if <see cref="P:Northwoods.Go.GoView.IsPrinting" /> is true
		/// and <see cref="M:Northwoods.Go.GoLayer.CanPrintObjects" /> is false, this method does nothing.
		/// </remarks>
		public void Paint(Graphics g, GoView view, RectangleF clipRect)
		{
			bool isPrinting = view.IsPrinting;
			if (isPrinting ? (!CanPrintObjects()) : (!CanViewObjects()))
			{
				return;
			}
			RectangleF docExtent = view.DocExtent;
			GoLayerCache goLayerCache = FindCache(view);
			if (goLayerCache != null && goLayerCache.Rect == docExtent)
			{
				foreach (GoObject @object in goLayerCache.Objects)
				{
					if (isPrinting ? @object.CanPrint() : @object.CanView())
					{
						RectangleF bounds = @object.Bounds;
						bounds = @object.ExpandPaintBounds(bounds, view);
						if (GoObject.IntersectsRect(bounds, clipRect))
						{
							@object.Paint(g, view);
						}
					}
				}
			}
			else if (CacheWanted(view))
			{
				if (goLayerCache == null)
				{
					goLayerCache = new GoLayerCache(view);
					Caches.Add(goLayerCache);
				}
				else
				{
					goLayerCache.Reset();
				}
				goLayerCache.Rect = docExtent;
				using (GoLayerEnumerator goLayerEnumerator = GetEnumerator())
				{
					while (goLayerEnumerator.MoveNext())
					{
						GoObject current2 = goLayerEnumerator.Current;
						RectangleF bounds2 = current2.Bounds;
						bounds2 = current2.ExpandPaintBounds(bounds2, view);
						if ((isPrinting ? current2.CanPrint() : current2.CanView()) && GoObject.IntersectsRect(bounds2, clipRect))
						{
							current2.Paint(g, view);
						}
						if (GoObject.IntersectsRect(bounds2, docExtent))
						{
							goLayerCache.Objects.Add(current2);
							InsertIntoCache1(goLayerCache, current2);
						}
					}
				}
				InitializeIndices();
				goLayerCache.Snappers.Sort(myZComparer);
			}
			else
			{
				using (GoLayerEnumerator goLayerEnumerator = GetEnumerator())
				{
					while (goLayerEnumerator.MoveNext())
					{
						GoObject current3 = goLayerEnumerator.Current;
						if (isPrinting ? current3.CanPrint() : current3.CanView())
						{
							RectangleF bounds3 = current3.Bounds;
							bounds3 = current3.ExpandPaintBounds(bounds3, view);
							if (GoObject.IntersectsRect(bounds3, clipRect))
							{
								current3.Paint(g, view);
							}
						}
					}
				}
			}
		}

		private bool CacheWanted(GoView view)
		{
			if (IsInDocument && GoDocument.myCaching)
			{
				return !view.IsPrinting;
			}
			return false;
		}

		internal GoLayerCache FindCache(GoView view)
		{
			foreach (GoLayerCache cach in Caches)
			{
				if (cach.View == view)
				{
					return cach;
				}
			}
			return null;
		}

		internal GoLayerCache FindCache(RectangleF r)
		{
			GoLayerCache goLayerCache = null;
			foreach (GoLayerCache cach in Caches)
			{
				if (GoObject.ContainsRect(cach.Rect, r) && (goLayerCache == null || cach.Objects.Count < goLayerCache.Objects.Count))
				{
					goLayerCache = cach;
				}
			}
			return goLayerCache;
		}

		internal GoLayerCache FindCache(PointF p)
		{
			GoLayerCache goLayerCache = null;
			foreach (GoLayerCache cach in Caches)
			{
				if (GoObject.ContainsRect(cach.Rect, p) && (goLayerCache == null || cach.Objects.Count < goLayerCache.Objects.Count))
				{
					goLayerCache = cach;
				}
			}
			return goLayerCache;
		}

		internal void ResetCaches()
		{
			myCaches = new List<GoLayerCache>();
			ResetPickCache();
		}

		internal void ResetPickCache()
		{
			myCachedPick = false;
			myCachedPickObject = null;
		}

		internal void UpdateCaches(GoObject obj, GoChangedEventArgs evt)
		{
			ResetPickCache();
			foreach (GoLayerCache cach in Caches)
			{
				RectangleF oldRect = evt.OldRect;
				oldRect = obj.ExpandPaintBounds(oldRect, cach.View);
				RectangleF newRect = evt.NewRect;
				newRect = obj.ExpandPaintBounds(newRect, cach.View);
				bool num = GoObject.IntersectsRect(cach.Rect, oldRect);
				bool flag = GoObject.IntersectsRect(cach.Rect, newRect);
				if (!num && flag)
				{
					if (!cach.Objects.Contains(obj))
					{
						cach.Objects.Add(obj);
					}
					InsertIntoCache1(cach, obj);
					InitializeIndices();
					cach.Snappers.Sort(myZComparer);
				}
			}
		}

		internal void InsertIntoCaches(GoObject obj)
		{
			ResetPickCache();
			RectangleF bounds = obj.Bounds;
			foreach (GoLayerCache cach in Caches)
			{
				RectangleF b = obj.ExpandPaintBounds(bounds, cach.View);
				if (GoObject.IntersectsRect(cach.Rect, b))
				{
					cach.Objects.Add(obj);
					InsertIntoCache1(cach, obj);
					InitializeIndices();
					cach.Snappers.Sort(myZComparer);
				}
			}
		}

		private void InsertIntoCache1(GoLayerCache cache, GoObject obj)
		{
			IGoDragSnapper goDragSnapper = obj as IGoDragSnapper;
			if (goDragSnapper != null && !cache.Snappers.Contains(goDragSnapper))
			{
				cache.Snappers.Add(goDragSnapper);
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					InsertIntoCache1(cache, item);
				}
			}
		}

		internal void RemoveFromCaches(GoObject obj)
		{
			ResetPickCache();
			RectangleF bounds = obj.Bounds;
			foreach (GoLayerCache cach in Caches)
			{
				RectangleF b = obj.ExpandPaintBounds(bounds, cach.View);
				if (GoObject.IntersectsRect(cach.Rect, b))
				{
					GoCollection.fastRemove(cach.Objects, obj);
					RemoveFromCache1(cach, obj);
				}
			}
		}

		private void RemoveFromCache1(GoLayerCache cache, GoObject obj)
		{
			GoStroke goStroke = obj as GoStroke;
			if (goStroke != null)
			{
				GoCollection.fastRemove(cache.Strokes, goStroke);
			}
			IGoDragSnapper goDragSnapper = obj as IGoDragSnapper;
			if (goDragSnapper != null)
			{
				GoCollection.fastRemove(cache.Snappers, goDragSnapper);
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					RemoveFromCache1(cache, item);
				}
			}
		}

		/// <summary>
		/// Find a visible object in this layer at a given point.
		/// </summary>
		/// <param name="p">the point in document coordinates</param>
		/// <param name="selectableOnly">this is passed on to calls to <see cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" /></param>
		/// <returns>the result of <see cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" />; null if no object contains the point <paramref name="p" /></returns>
		/// <remarks>
		/// This method calls <see cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" /> on each object in this
		/// layer until a call returns an object, which is returned by this method.
		/// The objects are tested in reverse order in which they are painted, so
		/// as to get the "top-most" object at the given point <paramref name="p" />.
		/// If <see cref="M:Northwoods.Go.GoLayer.CanViewObjects" /> is false, this method does nothing.
		/// If <paramref name="selectableOnly" /> is true but <see cref="M:Northwoods.Go.GoLayer.CanSelectObjects" />
		/// is false, this method returns null.
		/// Please note that if an object is found, it might not be a top-level object.
		/// In fact, when <paramref name="selectableOnly" /> is false, it is very likely
		/// that if any object is found at the given point, it will be a child of some
		/// group.
		/// </remarks>
		public GoObject PickObject(PointF p, bool selectableOnly)
		{
			if (!CanViewObjects())
			{
				return null;
			}
			if (selectableOnly && !CanSelectObjects())
			{
				return null;
			}
			if (myCachedPickPoint == p && myCachedPick && myCachedPickSelectable == selectableOnly)
			{
				return myCachedPickObject;
			}
			GoLayerCache goLayerCache = FindCache(p);
			checked
			{
				if (goLayerCache != null)
				{
					List<GoObject> objects = goLayerCache.Objects;
					for (int num = objects.Count - 1; num >= 0; num--)
					{
						GoObject goObject = objects[num].Pick(p, selectableOnly);
						if (goObject != null)
						{
							myCachedPick = true;
							myCachedPickPoint = p;
							myCachedPickSelectable = selectableOnly;
							myCachedPickObject = goObject;
							return goObject;
						}
					}
				}
				else
				{
					foreach (GoObject backward in Backwards)
					{
						GoObject goObject2 = backward.Pick(p, selectableOnly);
						if (goObject2 != null)
						{
							myCachedPick = true;
							myCachedPickPoint = p;
							myCachedPickSelectable = selectableOnly;
							myCachedPickObject = goObject2;
							return goObject2;
						}
					}
				}
				myCachedPick = true;
				myCachedPickPoint = p;
				myCachedPickSelectable = selectableOnly;
				myCachedPickObject = null;
				return null;
			}
		}

		/// <summary>
		/// Return a collection of objects that can be picked at a particular point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> location in document coordinates.</param>
		/// <param name="selectableOnly">If true, only consider objects for which <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.</param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// See the remarks about <see cref="M:Northwoods.Go.GoLayer.PickObject(System.Drawing.PointF,System.Boolean)" />.
		/// </remarks>
		public IGoCollection PickObjects(PointF p, bool selectableOnly, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection
				{
					InternalChecksForDuplicates = false
				};
			}
			if (coll.Count >= max)
			{
				return coll;
			}
			if (!CanViewObjects())
			{
				return coll;
			}
			if (selectableOnly && !CanSelectObjects())
			{
				return coll;
			}
			GoLayerCache goLayerCache = FindCache(p);
			checked
			{
				if (goLayerCache != null)
				{
					List<GoObject> objects = goLayerCache.Objects;
					for (int num = objects.Count - 1; num >= 0; num--)
					{
						GoObject goObject = objects[num];
						GoGroup goGroup = goObject as GoGroup;
						if (goGroup != null)
						{
							goGroup.PickObjects(p, selectableOnly, coll, max);
						}
						else
						{
							GoObject goObject2 = goObject.Pick(p, selectableOnly);
							if (goObject2 != null)
							{
								coll.Add(goObject2);
								if (coll.Count >= max)
								{
									return coll;
								}
							}
						}
					}
					return coll;
				}
				foreach (GoObject backward in Backwards)
				{
					GoGroup goGroup2 = backward as GoGroup;
					if (goGroup2 != null)
					{
						goGroup2.PickObjects(p, selectableOnly, coll, max);
					}
					else
					{
						GoObject goObject3 = backward.Pick(p, selectableOnly);
						if (goObject3 != null)
						{
							coll.Add(goObject3);
							if (coll.Count >= max)
							{
								return coll;
							}
						}
					}
				}
				return coll;
			}
		}

		/// <summary>
		/// Return a collection of objects that are surrounded by a given rectangle.
		/// </summary>
		/// <param name="rect">A <c>RectangleF</c> in document coordinates.</param>
		/// <param name="pickstyle">
		/// If <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyContained" />
		/// or <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyIntersectsBounds" />,
		/// only consider objects for which <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.
		/// </param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// This heeds this <see cref="T:Northwoods.Go.GoLayer" />'s <see cref="M:Northwoods.Go.GoLayer.CanViewObjects" />
		/// and the <see cref="T:Northwoods.Go.GoObject" />'s <see cref="M:Northwoods.Go.GoObject.CanView" /> predicates.
		/// If <paramref name="pickstyle" /> is <c>GoPickInRectangleStyle.SelectableOnlyContained</c>, this method also heeds the
		/// <see cref="M:Northwoods.Go.GoLayer.CanSelectObjects" /> and <see cref="M:Northwoods.Go.GoObject.CanSelect" /> predicates.
		/// This actually checks to see if the whole <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// is within the <paramref name="rect" /> bounds.  Such a policy allows a
		/// <see cref="T:Northwoods.Go.GoGroup" /> to be selected even though only one part of the group
		/// is in the rectangle, the object's <see cref="P:Northwoods.Go.GoObject.SelectionObject" /> that appears
		/// to the user to be selected.
		/// This will consider the children of <see cref="T:Northwoods.Go.GoGroup" />s.
		/// Once it finds a selectable object within the rectangle,
		/// it does not recurse further into that object.
		/// This method is called by <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />.
		/// </remarks>
		public IGoCollection PickObjectsInRectangle(RectangleF rect, GoPickInRectangleStyle pickstyle, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection
				{
					InternalChecksForDuplicates = false
				};
			}
			if (coll.Count >= max)
			{
				return coll;
			}
			if (!CanViewObjects())
			{
				return coll;
			}
			if (GoDocument.PickStyleSelectableOnly(pickstyle) && !CanSelectObjects())
			{
				return coll;
			}
			GoLayerCache goLayerCache = FindCache(rect);
			if (goLayerCache != null)
			{
				List<GoObject> objects = goLayerCache.Objects;
				int count = objects.Count;
				for (int i = 0; i < count; i = checked(i + 1))
				{
					GoObject obj = objects[i];
					PickObjectsInRectangleInternal(obj, rect, pickstyle, coll, max);
					if (coll.Count >= max)
					{
						return coll;
					}
				}
				return coll;
			}
			using (GoLayerEnumerator goLayerEnumerator = GetEnumerator())
			{
				while (goLayerEnumerator.MoveNext())
				{
					GoObject current = goLayerEnumerator.Current;
					PickObjectsInRectangleInternal(current, rect, pickstyle, coll, max);
					if (coll.Count >= max)
					{
						return coll;
					}
				}
				return coll;
			}
		}

		private void PickObjectsInRectangleInternal(GoObject obj, RectangleF rect, GoPickInRectangleStyle pickstyle, IGoCollection coll, int max)
		{
			if (coll.Count >= max || !obj.CanView())
			{
				return;
			}
			if (GoDocument.PickStyleAny(pickstyle) || obj.CanSelect())
			{
				GoObject goObject = obj.SelectionObject;
				if (goObject == null)
				{
					goObject = obj;
				}
				if (GoDocument.PickStyleContained(pickstyle))
				{
					if (goObject.ContainedByRectangle(rect))
					{
						coll.Add(obj);
						return;
					}
				}
				else if (GoDocument.PickStyleIntersectsBounds(pickstyle) && GoObject.IntersectsRect(goObject.Bounds, rect))
				{
					coll.Add(obj);
					return;
				}
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					PickObjectsInRectangleInternal(item, rect, pickstyle, coll, max);
				}
			}
		}
	}
}
