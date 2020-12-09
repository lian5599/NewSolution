#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// The GoObject abstract class represents graphical objects that can be
	/// added to <see cref="T:Northwoods.Go.GoDocument" />s to be displayed by <see cref="T:Northwoods.Go.GoView" />s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Read the User Guide and FAQ for more details.
	/// </para>
	/// </remarks>
	[Serializable]
	public abstract class GoObject
	{
		/// <summary>
		/// This is an empty <c>RectangleF</c>, which is convenient when calling <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		public static readonly RectangleF NullRect;

		/// <summary>
		/// Use this spot when no particular spot seems appropriate.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int NoSpot = 0;

		/// <summary>
		/// Use this handle when no particular handle seems appropriate.
		/// </summary>
		public const int NoHandle = 0;

		/// <summary>
		/// This represents the point at the center of the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int Middle = 1;

		/// <summary>
		/// This represents the point at the center of the object's bounds;
		/// the same as <seealso cref="F:Northwoods.Go.GoObject.Middle" />.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int MiddleCenter = 1;

		/// <summary>
		/// This represents a corner point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int TopLeft = 2;

		/// <summary>
		/// This represents a corner point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int TopRight = 4;

		/// <summary>
		/// This represents a corner point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int BottomRight = 8;

		/// <summary>
		/// This represents a corner point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int BottomLeft = 16;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int MiddleTop = 32;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="F:Northwoods.Go.GoObject.MiddleTop" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int TopCenter = 32;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int MiddleRight = 64;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int MiddleBottom = 128;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="F:Northwoods.Go.GoObject.MiddleBottom" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int BottomCenter = 128;

		/// <summary>
		/// This represents a point in the object's bounds.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public const int MiddleLeft = 256;

		/// <summary>
		/// User-defined spots should have identifiers greater than this value.
		/// </summary>
		public const int LastSpot = 8192;

		/// <summary>
		/// User-defined handles should have identifiers greater than this value.
		/// </summary>
		public const int LastHandle = 8192;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RepaintAll = 1000;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Bounds" /> property.
		/// </summary>
		public const int ChangedBounds = 1001;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Visible" /> property.
		/// </summary>
		public const int ChangedVisible = 1003;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Selectable" /> property.
		/// </summary>
		public const int ChangedSelectable = 1004;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Movable" /> property.
		/// </summary>
		public const int ChangedMovable = 1005;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Copyable" /> property.
		/// </summary>
		public const int ChangedCopyable = 1006;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Resizable" /> property.
		/// </summary>
		public const int ChangedResizable = 1007;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Reshapable" /> property.
		/// </summary>
		public const int ChangedReshapable = 1008;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Deletable" /> property.
		/// </summary>
		public const int ChangedDeletable = 1009;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Editable" /> property.
		/// </summary>
		public const int ChangedEditable = 1010;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> property.
		/// </summary>
		public const int ChangedAutoRescales = 1011;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> property.
		/// </summary>
		public const int ChangedResizesRealtime = 1012;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Shadowed" /> property.
		/// </summary>
		public const int ChangedShadowed = 1013;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedAddedObserver = 1014;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedRemovedObserver = 1015;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.DragsNode" /> property.
		/// </summary>
		public const int ChangedDragsNode = 1016;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Printable" /> property.
		/// </summary>
		public const int ChangedPrintable = 1017;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoObject.Initializing" /> property.
		/// </summary>
		public const int ChangedInitializing = 1041;

		/// <summary>
		/// Users can define their own <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> event subhint values greater than this value.
		/// </summary>
		public const int LastChangedHint = 10000;

		internal const int flagVisible = 1;

		internal const int flagSelectable = 2;

		internal const int flagMovable = 4;

		internal const int flagCopyable = 8;

		internal const int flagResizable = 16;

		internal const int flagReshapable = 32;

		internal const int flagDeletable = 64;

		internal const int flagEditable = 128;

		internal const int flagAutoRescales = 256;

		internal const int flagResizesRealtime = 512;

		internal const int flagShadowed = 1024;

		internal const int flagDragsNode = 2048;

		internal const int flagSuspendsUpdates = 4096;

		internal const int flagSkipsUndoManager = 8192;

		internal const int flagSkipsBoundsChanged = 16384;

		internal const int flagInvalidBounds = 32768;

		internal const int flagBeingRemoved = 65536;

		internal const int flagInitializing = 131072;

		internal const int flagReserved1 = 262144;

		internal const int flagPrintable = 524288;

		internal const int flagObject1 = 1048576;

		internal const int flagObject2 = 2097152;

		internal const int flagObject3 = 4194304;

		internal const int flagObject4 = 8388608;

		internal const int flagObject5 = 16777216;

		internal const int flagObject6 = 33554432;

		internal const int flagObject7 = 67108864;

		internal const int flagObject8 = 134217728;

		internal const int flagObject9 = 268435456;

		internal const int flagObject10 = 536870912;

		internal const int flagObject11 = 1073741824;

		private GoLayer myLayer;

		private GoGroup myParent;

		private RectangleF myBounds = new RectangleF(0f, 0f, 10f, 10f);

		private int myInternalFlags = 524671;

		private GoCollection myObservers;

		private int myLayerIndex;

		/// <summary>
		/// Gets the layer to which this object belongs.
		/// </summary>
		/// <remarks>
		/// If this object is not part of any layer, either directly
		/// as a top-level object, or as part of a group,
		/// then this property value will be null.
		/// You cannot set this property--call <see cref="T:Northwoods.Go.GoLayer" />.<see cref="M:Northwoods.Go.GoLayer.Add(Northwoods.Go.GoObject)" /> instead.
		/// <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> will not set this property directly, nor will
		/// it automatically add the copied object to some layer to set this property
		/// indirectly.
		/// The caller of <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> is responsible for deciding which
		/// <see cref="T:Northwoods.Go.GoLayer" /> to add the newly copied object, if any.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.IsInDocument" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Document" />
		/// <seealso cref="P:Northwoods.Go.GoObject.IsInView" />
		/// <seealso cref="P:Northwoods.Go.GoObject.View" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnLayerChanged(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer,Northwoods.Go.GoObject)" />
		[Category("Ownership")]
		[Description("The GoLayer to which this object belongs.")]
		public GoLayer Layer => myLayer;

		internal int LayerIndex
		{
			get
			{
				return myLayerIndex;
			}
			set
			{
				myLayerIndex = value;
			}
		}

		/// <summary>
		/// Gets whether this object belongs to a document.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoObject.Document" />
		[Browsable(false)]
		public bool IsInDocument => Layer?.IsInDocument ?? false;

		/// <summary>
		/// Gets the document that this object belongs to,
		/// or null if this is not in a layer or if this is in a view layer.
		/// </summary>
		/// <remarks>
		/// You cannot set this property--call <see cref="T:Northwoods.Go.GoLayer" />.<see cref="M:Northwoods.Go.GoLayer.Add(Northwoods.Go.GoObject)" /> instead.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.IsInDocument" />
		[Category("Ownership")]
		[Description("The GoDocument to which this object belongs, if in a document layer.")]
		public GoDocument Document => Layer?.Document;

		/// <summary>
		/// Gets whether this object belongs to a view.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoObject.View" />
		[Browsable(false)]
		public bool IsInView => Layer?.IsInView ?? false;

		/// <summary>
		/// Gets the view that this object belongs to,
		/// or null if this is not in a layer or if this is in a document layer.
		/// </summary>
		/// <remarks>
		/// You cannot set this property--call <see cref="T:Northwoods.Go.GoLayer" />.<see cref="M:Northwoods.Go.GoLayer.Add(Northwoods.Go.GoObject)" /> instead.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.IsInView" />
		[Category("Ownership")]
		[Description("The GoView to which this object belongs, if in a view layer.")]
		public GoView View => Layer?.View;

		/// <summary>
		/// Gets the parent group for this object.
		/// </summary>
		/// <remarks>
		/// If this object belongs to a <see cref="T:Northwoods.Go.GoGroup" />, we return that group.
		/// Otherwise we return null.
		/// You cannot set this property--call <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.Add(Northwoods.Go.GoObject)" /> instead.
		/// This property does not depend on the object belonging to a layer.
		/// <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> will not set this property directly, nor will
		/// it automatically add the copied object to some group to set this property
		/// indirectly.
		/// The caller of <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> is responsible for deciding if and
		/// where to add the newly copied object to a <see cref="T:Northwoods.Go.GoGroup" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.IsTopLevel" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnParentChanged(Northwoods.Go.GoGroup,Northwoods.Go.GoGroup)" />
		[Category("Ownership")]
		[Description("The parent GoGroup for this object, or null if top-level.")]
		public GoGroup Parent => myParent;

		/// <summary>
		/// Gets whether this object is a top-level object.
		/// </summary>
		/// <remarks>
		/// This property is true if there is no <see cref="P:Northwoods.Go.GoObject.Parent" /> group.
		/// This property does not depend on the object belonging to a layer.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.TopLevelObject" />
		[Browsable(false)]
		public bool IsTopLevel => myParent == null;

		/// <summary>
		/// Gets the top-level object for this object.
		/// </summary>
		/// <remarks>
		/// If this object's <see cref="P:Northwoods.Go.GoObject.IsTopLevel" /> property is true,
		/// we just return this object.
		/// Otherwise we look up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until
		/// we find the top-level object.
		/// This property does not depend on the object belonging to a layer.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Parent" />
		[Browsable(false)]
		public GoObject TopLevelObject
		{
			get
			{
				GoObject goObject = this;
				while (goObject.Parent != null)
				{
					goObject = goObject.Parent;
				}
				return goObject;
			}
		}

		/// <summary>
		/// This convenience property gets the highest-level object in the <see cref="P:Northwoods.Go.GoObject.Parent" />
		/// chain that is not a <see cref="T:Northwoods.Go.GoSubGraphBase" />.
		/// </summary>
		/// <remarks>
		/// If this is a top-level object, or if this is an immediate child of a <see cref="T:Northwoods.Go.GoSubGraphBase" />,
		/// the value is just itself.
		/// Note that the value need not be a real <see cref="T:Northwoods.Go.IGoNode" />--it could very well be a
		/// group or a link.
		/// However, if it is an immediate child of a <see cref="T:Northwoods.Go.GoSubGraphBase" /> and is
		/// also a <see cref="T:Northwoods.Go.IGoPort" />, the parent node value is the subgraph, not the port itself.
		/// </remarks>
		[Browsable(false)]
		public GoObject ParentNode
		{
			get
			{
				GoObject goObject = this;
				while (goObject.Parent != null)
				{
					if (goObject.Parent is GoSubGraphBase)
					{
						if (goObject is IGoPort)
						{
							goObject = goObject.Parent;
						}
						break;
					}
					goObject = goObject.Parent;
				}
				return goObject;
			}
		}

		/// <summary>
		/// Gets or sets the bounding rectangle for this object.
		/// </summary>
		/// <value>
		/// This <c>RectangleF</c> value describes the size and position of the object
		/// in document coordinates.
		/// The <c>Width</c> and <c>Height</c> must be non-negative.
		/// </value>
		/// <remarks>
		/// When getting the bounds, if <see cref="P:Northwoods.Go.GoObject.InvalidBounds" /> is true,
		/// we call <see cref="M:Northwoods.Go.GoObject.ComputeBounds" /> to get the correct updated bounds.
		/// When setting the bounds, we call <see cref="M:Northwoods.Go.GoObject.OnBoundsChanged(System.Drawing.RectangleF)" />,
		/// <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.OnChildBoundsChanged(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />
		/// on the <see cref="P:Northwoods.Go.GoObject.Parent" /> (if any),
		/// and <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> with a subhint of <c>ChangedBounds</c>.
		/// You should override setting this property if you want to make sure this
		/// object never gets certain bounds, such as a size that's too small or large,
		/// or a position that is "out-of-bounds" for your application.
		/// However, if you only want to constrain how the user is allowed to
		/// move this object around with the mouse, you should override
		/// <see cref="M:Northwoods.Go.GoObject.ComputeMove(System.Drawing.PointF,System.Drawing.PointF)" /> instead, or override <see cref="M:Northwoods.Go.GoObject.DoMove(Northwoods.Go.GoView,System.Drawing.PointF,System.Drawing.PointF)" />
		/// if the constraint should be specific to a particular view or if
		/// something other than the <see cref="P:Northwoods.Go.GoObject.Location" /> should be set.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Position" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Size" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Center" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Left" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Top" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Width" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Height" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Right" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Bottom" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Location" />
		[Browsable(false)]
		public virtual RectangleF Bounds
		{
			get
			{
				if (InvalidBounds && !SkipsBoundsChanged)
				{
					InvalidBounds = false;
					SkipsBoundsChanged = true;
					Bounds = ComputeBounds();
					SkipsBoundsChanged = false;
				}
				return myBounds;
			}
			set
			{
				RectangleF rectangleF = myBounds;
				if (!(value.Width >= 0f) || !(value.Height >= 0f) || !(rectangleF != value))
				{
					return;
				}
				myBounds = value;
				Changed(1001, 0, null, rectangleF, 0, null, value);
				if (!SkipsBoundsChanged && !Initializing)
				{
					SkipsBoundsChanged = true;
					OnBoundsChanged(rectangleF);
					if (InvalidBounds)
					{
						InvalidBounds = false;
						Bounds = ComputeBounds();
					}
				}
				SkipsBoundsChanged = false;
				GoGroup parent = Parent;
				if (parent == null)
				{
					return;
				}
				parent.InvalidatePaintBounds();
				if (!parent.SkipsBoundsChanged && !parent.Initializing && !Initializing)
				{
					parent.SkipsBoundsChanged = true;
					parent.OnChildBoundsChanged(this, rectangleF);
					if (parent.InvalidBounds)
					{
						parent.InvalidBounds = false;
						parent.Bounds = parent.ComputeBounds();
					}
					parent.SkipsBoundsChanged = false;
				}
			}
		}

		/// <summary>
		/// Gets or sets this object's top-left corner's position.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Location" />
		[Browsable(false)]
		public PointF Position
		{
			get
			{
				RectangleF bounds = Bounds;
				return new PointF(bounds.X, bounds.Y);
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.X = value.X;
				bounds.Y = value.Y;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's dimensions.
		/// </summary>
		/// <value>
		/// The <c>SizeF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Browsable(false)]
		public SizeF Size
		{
			get
			{
				RectangleF bounds = Bounds;
				return new SizeF(bounds.Width, bounds.Height);
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.Width = value.Width;
				bounds.Height = value.Height;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's center position.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Browsable(false)]
		public PointF Center
		{
			get
			{
				RectangleF bounds = Bounds;
				return new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.X = value.X - bounds.Width / 2f;
				bounds.Y = value.Y - bounds.Height / 2f;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's left coordinate, the X position.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The coordinate of the left side of the Bounds.")]
		public float Left
		{
			get
			{
				return Bounds.X;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.X = value;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's top coordinate, the Y position.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The coordinate of the top side of the Bounds.")]
		public float Top
		{
			get
			{
				return Bounds.Y;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.Y = value;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's width.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The width of the Bounds.")]
		public float Width
		{
			get
			{
				return Bounds.Width;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.Width = value;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's height.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The height of the Bounds.")]
		public float Height
		{
			get
			{
				return Bounds.Height;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.Height = value;
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's right coordinate.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The coordinate of the right side of the Bounds.")]
		public float Right
		{
			get
			{
				RectangleF bounds = Bounds;
				return bounds.X + bounds.Width;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.X += value - (bounds.X + bounds.Width);
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's bottom coordinate.
		/// </summary>
		/// <value>
		/// The <c>float</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This is just a convenience property that operates on this object's
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// </remarks>
		[Category("Bounds")]
		[Description("The coordinate of the bottom side of the Bounds.")]
		public float Bottom
		{
			get
			{
				RectangleF bounds = Bounds;
				return bounds.Y + bounds.Height;
			}
			set
			{
				RectangleF bounds = Bounds;
				bounds.Y += value - (bounds.Y + bounds.Height);
				Bounds = bounds;
			}
		}

		/// <summary>
		/// Gets or sets this object's natural position.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This property is normally the object's <see cref="F:Northwoods.Go.GoObject.TopLeft" /> position.
		/// However, it is common for certain kinds of objects to assume that the
		/// assigned location actually refers to a different spot of the bounding
		/// rectangle.  For example, for <see cref="T:Northwoods.Go.GoText" /> objects, the text's
		/// alignment property determines the <c>Location</c>.  For groups, one of
		/// the child objects might be the natural thing to be positioned as the
		/// user would see it.  For example, the icon of a node might provide the
		/// Location for the node as a whole.
		/// If you override this property, you should also override
		/// <see cref="M:Northwoods.Go.GoObject.SetSizeKeepingLocation(System.Drawing.SizeF)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSizeKeepingLocation(System.Drawing.SizeF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Bounds" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		[Category("Bounds")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The natural location for this object, perhaps different from Position.")]
		public virtual PointF Location
		{
			get
			{
				return Position;
			}
			set
			{
				Position = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the user can see this object.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanView" /> method
		/// instead of getting this property.
		/// If you set this property to false, you probably also want to set
		/// the <see cref="P:Northwoods.Go.GoObject.Printable" /> property to false.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from seeing this object
		/// by the normal painting mechanisms.
		/// Even when this property value is true, this object might not be
		/// seeable by the user because the object's layer is not visible.
		/// A user will not normally be able to select an invisible object
		/// or do other interactive operations with an invisible object.
		/// However, an invisible object can still be part of a layer and
		/// document or view, and can still take part in all programmatic
		/// operations such as manipulating its bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanView" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowView" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Printable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can see this object.")]
		public virtual bool Visible
		{
			get
			{
				return (InternalFlags & 1) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 1) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 1;
					}
					else
					{
						InternalFlags &= -2;
					}
					Changed(1003, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the view can print this object.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanPrint" /> method
		/// instead of getting this property.
		/// You may also want to set the <see cref="P:Northwoods.Go.GoObject.Visible" /> property
		/// when setting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the view from printing this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// printable by the view because the object's layer is not printable.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanPrint" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowPrint" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Visible" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether a view can print this object.")]
		public virtual bool Printable
		{
			get
			{
				return (InternalFlags & 0x80000) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x80000) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 524288;
					}
					else
					{
						InternalFlags &= -524289;
					}
					Changed(1017, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can select this object.
		/// </summary>
		/// <value>
		/// This defaults to true.  However, for some objects, such
		/// as <see cref="T:Northwoods.Go.GoPort" />, this defaults to false.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanSelect" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from selecting this object
		/// by the normal mechanisms.
		/// Normally all the child objects of a group should have Selectable set to false.
		/// Even when this property value is true, this object might not be
		/// selectable by the user because its layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always select objects programmatically by calling
		/// <c>aView.Selection.Select(obj)</c> or <c>aView.Selection.Add(obj)</c>.
		/// When this object's <c>CanSelect</c> is false, then if this object is
		/// part of a group, the normal selection mechanism will see if the
		/// group's <c>CanSelect</c> is true.  If so, the group will be selected.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanSelect" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanSelectObjects" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can select this object.")]
		public virtual bool Selectable
		{
			get
			{
				return (InternalFlags & 2) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 2) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 2;
					}
					else
					{
						InternalFlags &= -3;
					}
					Changed(1004, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can move this object.
		/// </summary>
		/// <value>
		/// This defaults to true.  However, for some objects, such
		/// as <see cref="T:Northwoods.Go.GoLink" />, this defaults to false.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanMove" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from moving this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// movable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always move objects programmatically by calling
		/// <c>obj.Position = newPos</c>.
		/// When this object is part of a group, the default drag behavior
		/// is to move just the object.  However, the <see cref="P:Northwoods.Go.GoObject.DraggingObject" />
		/// really determines what is actually moved.  Note that some objects
		/// will not be able to move, or will not be able to move freely,
		/// because of constraints enforced in setting the <see cref="P:Northwoods.Go.GoObject.Bounds" />
		/// or in the parent's <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" /> method.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanMove" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanMoveObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoMove(Northwoods.Go.GoView,System.Drawing.PointF,System.Drawing.PointF)" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can move this object.")]
		public virtual bool Movable
		{
			get
			{
				return (InternalFlags & 4) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 4) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 4;
					}
					else
					{
						InternalFlags &= -5;
					}
					Changed(1005, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can copy this object.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanCopy" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from copying this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// copyable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always copy objects programmatically by calling
		/// <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
		/// When this object is part of a group, the default drag behavior
		/// is to copy just the object.  However, the <see cref="P:Northwoods.Go.GoObject.DraggingObject" />
		/// really determines what is actually copied.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanCopy" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanCopyObjects" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can copy this object.")]
		public virtual bool Copyable
		{
			get
			{
				return (InternalFlags & 8) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 8) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 8;
					}
					else
					{
						InternalFlags &= -9;
					}
					Changed(1006, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can resize this object.
		/// </summary>
		/// <value>
		/// This defaults to true.  However, for some objects, such
		/// as <see cref="T:Northwoods.Go.GoText" /> and <see cref="T:Northwoods.Go.GoPort" />,
		/// this defaults to false.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanResize" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from resizing this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// resizable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always resize objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// For an object to be resizable, its <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// is really what should be resizable.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanResize" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanResizeObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Reshapable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can resize this object.")]
		public virtual bool Resizable
		{
			get
			{
				return (InternalFlags & 0x10) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x10) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 16;
					}
					else
					{
						InternalFlags &= -17;
					}
					Changed(1007, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can reshape this object.
		/// </summary>
		/// <value>
		/// This defaults to true.  However, for some objects, such as
		/// <see cref="T:Northwoods.Go.GoImage" /> this defaults to false, so that users
		/// cannot change the aspect ratio.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanReshape" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from reshaping this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// reshapable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always reshape objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>
		/// or by making other changes to alter the shape of the object.
		/// For an object to be reshapable, its <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// is really what should be reshapable--for a <see cref="T:Northwoods.Go.GoGroup" />,
		/// this is often one of its child objects.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanReshape" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanReshapeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Resizable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can reshape this resizable object.")]
		public virtual bool Reshapable
		{
			get
			{
				return (InternalFlags & 0x20) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x20) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 32;
					}
					else
					{
						InternalFlags &= -33;
					}
					Changed(1008, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can delete this object.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanDelete" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from deleting this object
		/// by the normal mechanisms (i.e., removing it from its layer).
		/// Even when this property value is true, this object might not be
		/// deletable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always delete objects programmatically by calling
		/// <c>obj.Remove()</c>.
		/// When this object is part of a group, the default edit delete behavior
		/// is to remove the object from the group.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanDelete" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanDeleteObjects" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether users can delete this object.")]
		public virtual bool Deletable
		{
			get
			{
				return (InternalFlags & 0x40) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x40) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 64;
					}
					else
					{
						InternalFlags &= -65;
					}
					Changed(1009, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can edit this object.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// You should normally call the <see cref="M:Northwoods.Go.GoObject.CanEdit" /> method
		/// instead of getting this property.
		/// </value>
		/// <remarks>
		/// A false value prevents the user from editing this object
		/// by the normal mechanisms.
		/// Even when this property value is true, this object might not be
		/// editable by the user because the layer or document disallows it,
		/// or because the view disallows it.
		/// Your code can always edit objects programmatically by calling
		/// <c>obj.DoBeginEdit(aView)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CanEdit" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanEditObjects" />
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether users can edit this object.")]
		public virtual bool Editable
		{
			get
			{
				return (InternalFlags & 0x80) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x80) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 128;
					}
					else
					{
						InternalFlags &= -129;
					}
					Changed(1010, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object automatically rescales its appearance when
		/// its size changes.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <remarks>
		/// For most objects, particularly shapes, it is natural that when the size
		/// changes, the object adjusts its appearance appropriately smaller or larger,
		/// and accomodating any change in aspect ratio.
		/// Most classes ignore this property.
		/// However, there are certain circumstances where users do not expect this behavior,
		/// for example for <see cref="T:Northwoods.Go.GoText" /> instances that are part of a resizable group.
		/// Typically you will set this property false for text objects added to groups.
		/// <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.RescaleChildren(System.Drawing.RectangleF)" /> heeds this property.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether this object automatically rescales its appearance when its size changes.")]
		public virtual bool AutoRescales
		{
			get
			{
				return (InternalFlags & 0x100) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x100) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 256;
					}
					else
					{
						InternalFlags &= -257;
					}
					Changed(1011, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object continuously changes its size during a user resizing operation.
		/// [In Web Forms there is no continuous visual feedback of a resize.]
		/// </summary>
		/// <value>
		/// This defaults to false.  However, for some objects, such
		/// as <see cref="T:Northwoods.Go.GoPolygon" /> and <see cref="T:Northwoods.Go.GoStroke" />,
		/// this defaults to true.
		/// </value>
		/// <remarks>
		/// One advantage of a false value for this property is that only one undo record
		/// is generated for a user's resizing operation--namely the final one on mouse up.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether this object's size continuously changes during a user resizing operation.")]
		public virtual bool ResizesRealtime
		{
			get
			{
				return (InternalFlags & 0x200) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x200) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 512;
					}
					else
					{
						InternalFlags &= -513;
					}
					Changed(1012, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object is painted with a drop shadow.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// Normally the drop shadow is achieved by painting the shadow first,
		/// using <see cref="M:Northwoods.Go.GoObject.GetShadowBrush(Northwoods.Go.GoView)" /> or <see cref="M:Northwoods.Go.GoObject.GetShadowPen(Northwoods.Go.GoView,System.Single)" />
		/// at the <see cref="M:Northwoods.Go.GoObject.GetShadowOffset(Northwoods.Go.GoView)" />, and then doing the regular
		/// painting.
		/// The shadow affects the paint bounds, but not the object's bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether this object is painted with a drop shadow.")]
		public virtual bool Shadowed
		{
			get
			{
				return (InternalFlags & 0x400) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x400) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 1024;
					}
					else
					{
						InternalFlags &= -1025;
					}
					Changed(1013, 0, flag, NullRect, 0, value, NullRect);
					if (Parent != null)
					{
						Parent.InvalidatePaintBounds();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this object, when selected and a child of a group,
		/// and when dragged, drags the parent node instead.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// This is used by <see cref="P:Northwoods.Go.GoObject.DraggingObject" /> so that
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> or <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// will move or copy the node instead of the child object.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether this selected child object, when dragged, drags the node instead.")]
		public virtual bool DragsNode
		{
			get
			{
				return (InternalFlags & 0x800) != 0;
			}
			set
			{
				bool flag = (InternalFlags & 0x800) != 0;
				if (flag != value)
				{
					if (value)
					{
						InternalFlags |= 2048;
					}
					else
					{
						InternalFlags &= -2049;
					}
					Changed(1016, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether any document Changed event handlers are called upon a
		/// change to this object.
		/// </summary>
		/// <value>
		/// A value of true means that any document Changed event handlers and any
		/// UndoManager are not called.  No observer objects are notified either.
		/// A value of false means that the notifications do take place.
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// <para>
		/// Warning!  You probably shouldn't be using this property.
		/// </para>
		/// <para>
		/// When this property is true, no views of this document will be updated
		/// as this object is changed, and no undo/redo information is kept.
		/// When you set the property to false again, you will need to make
		/// sure all the views are correct (you may wish to call <see cref="M:Northwoods.Go.GoObject.InvalidateViews" />).
		/// You should also be sure the user and the UndoManager will
		/// not be confused by the loss of any undo/redo information while
		/// this property was true.
		/// Setting this property does not raise any Changed events.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.SuspendsUpdates" />
		/// <seealso cref="P:Northwoods.Go.GoObject.SkipsUndoManager" />
		[Browsable(false)]
		public bool SuspendsUpdates
		{
			get
			{
				return (InternalFlags & 0x1000) != 0;
			}
			set
			{
				if (value)
				{
					InternalFlags |= 4096;
				}
				else
				{
					InternalFlags &= -4097;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the UndoManager is notified upon a change to this object.
		/// </summary>
		/// <value>
		/// A value of true means the <see cref="T:Northwoods.Go.GoUndoManager" />'s
		/// <see cref="M:Northwoods.Go.GoUndoManager.DocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> method is not called.
		/// A value of false means that if there is an <see cref="T:Northwoods.Go.GoUndoManager" />,
		/// it is notified so that it can record changes for undo and redo purposes.
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// You must be careful that any changes that occur while this property
		/// is true will not confuse the user when they perform Undo's and Redo's
		/// but the changes are not undone or redone.
		/// Setting this property does not raise any Changed events.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" />
		/// <seealso cref="P:Northwoods.Go.GoObject.SuspendsUpdates" />
		[Browsable(false)]
		public bool SkipsUndoManager
		{
			get
			{
				return (InternalFlags & 0x2000) != 0;
			}
			set
			{
				if (value)
				{
					InternalFlags |= 8192;
				}
				else
				{
					InternalFlags &= -8193;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the bounds are up to date.
		/// </summary>
		/// <remarks>
		/// This is typically set to true as some change is made to this
		/// object that requires recalculation of the bounds.
		/// This flag is automatically set to false and the
		/// <see cref="M:Northwoods.Go.GoObject.ComputeBounds" /> method then actually does that
		/// calculation on demand.
		/// Setting this property does not raise any Changed events.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Bounds" />
		[Browsable(false)]
		protected bool InvalidBounds
		{
			get
			{
				return (InternalFlags & 0x8000) != 0;
			}
			set
			{
				if (value)
				{
					InternalFlags |= 32768;
				}
				else
				{
					InternalFlags &= -32769;
				}
			}
		}

		private bool SkipsBoundsChanged
		{
			get
			{
				return (InternalFlags & 0x4000) != 0;
			}
			set
			{
				if (value)
				{
					InternalFlags |= 16384;
				}
				else
				{
					InternalFlags &= -16385;
				}
			}
		}

		/// <summary>
		/// Gets whether this object is in the process of being removed from its layer or group.
		/// </summary>
		/// <remarks>
		/// Sometimes when handling certain events you need to know whether the
		/// object is being removed from its group or layer, so that you can decide
		/// on alternative behaviors.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.Remove(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.Remove(Northwoods.Go.GoObject)" />
		[Browsable(false)]
		public bool BeingRemoved => (InternalFlags & 0x10000) != 0;

		/// <summary>
		/// Gets or sets whether this object is in the process of being initialized.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property is provided as a standard way to indicate that the object
		/// is not yet completely initialized, thereby allowing some methods to optimize
		/// their behavior.
		/// </para>
		/// <para>
		/// Most of the simpler objects do not need to use this flag.
		/// More complicated objects such as <see cref="T:Northwoods.Go.GoGroup" />s use this flag
		/// during construction and initialization to avoid repeated work in
		/// methods such as <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />.
		/// So if you temporarily set this property to true during the initialization of a
		/// complex or large node, be sure to call <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" /><b>(null)</b>
		/// after you set it back to false.
		/// </para>
		/// <para>
		/// This property is temporarily set to true in some constructors or
		/// initialization methods, in <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.CopyObject(Northwoods.Go.GoCopyDictionary)" />
		/// when calling <see cref="M:Northwoods.Go.GoGroup.CopyChildren(Northwoods.Go.GoGroup,Northwoods.Go.GoCopyDictionary)" />, and during an undo or a
		/// redo when calling <see cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// </para>
		/// <para>
		/// Property setters that have public side-effects (besides just changing the value
		/// of a field or other internal state) should not do those side-effects when this
		/// property is true.
		/// This can be important in avoiding unwanted side-effects during undo or redo.
		/// </para>
		/// <para>
		/// Setting this property does not call <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> or raise
		/// any <see cref="E:Northwoods.Go.GoDocument.Changed" /> events.
		/// </para>
		/// </remarks>
		[Browsable(false)]
		public bool Initializing
		{
			get
			{
				return (InternalFlags & 0x20000) != 0;
			}
			set
			{
				if (value)
				{
					InternalFlags |= 131072;
				}
				else
				{
					InternalFlags &= -131073;
				}
			}
		}

		internal int InternalFlags
		{
			get
			{
				return myInternalFlags;
			}
			set
			{
				myInternalFlags = value;
			}
		}

		/// <summary>
		/// Gets an enumerator over the list of observer objects of this object.
		/// </summary>
		/// <value>
		/// A <see cref="T:Northwoods.Go.GoCollectionEnumerator" /> that iterates over the <see cref="T:Northwoods.Go.GoObject" />s
		/// that are observers.  Most objects do not have any observer objects.
		/// Their use does impose a performance penalty in both space and time.
		/// </value>
		public GoCollectionEnumerator Observers
		{
			get
			{
				if (myObservers != null)
				{
					return myObservers.GetEnumerator();
				}
				return GoCollectionEnumerator.Empty;
			}
		}

		/// <summary>
		/// Gets the object that gets selection handles when this object is selected.
		/// </summary>
		/// <remarks>
		/// Sometimes when an object gets selected, you want to make it look like
		/// another object is what really got selected.
		/// Normally this happens for groups, where you don't want to have selection
		/// handles on the whole group, but only on some particular child object.
		/// This property allows you to specify which object should get selection
		/// handles when this object gains selection.
		/// The default value is this object itself.
		/// The selection object's <see cref="M:Northwoods.Go.GoObject.CanResize" /> method will control if that
		/// object gets resize selection handles and if the user can actually resize
		/// that object.
		/// You should be careful to make sure the value of this property is
		/// not confusing to the user.  Returning unrelated or varying objects
		/// may produce indeterminate behavior.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnGotSelection(Northwoods.Go.GoSelection)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		[Category("Appearance")]
		[Description("The object that will get the selection handles when this object is selected.")]
		public virtual GoObject SelectionObject => this;

		/// <summary>
		/// Gets the object that will be dragged instead of this selected object.
		/// </summary>
		/// <remarks>
		/// This property is used by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> and
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// If <see cref="P:Northwoods.Go.GoObject.DragsNode" /> is true, this will return the node that this
		/// object is part of, which may be the <see cref="P:Northwoods.Go.GoObject.Parent" /> or a grandparent.
		/// If this is a top-level object, the value will be this object itself.
		/// By default this will just return this object itself.
		/// You should be careful to make sure the value of this property is
		/// not confusing to the user.  Returning unrelated or varying objects
		/// may produce indeterminate behavior.
		/// </remarks>
		[Category("Behavior")]
		[Description("The object that will get dragged when this selected object is dragged.")]
		public virtual GoObject DraggingObject
		{
			get
			{
				if (DragsNode)
				{
					for (GoObject parent = Parent; parent != null; parent = parent.Parent)
					{
						if (parent is IGoNode)
						{
							return parent;
						}
						if (parent.Parent == null)
						{
							return parent;
						}
					}
				}
				return this;
			}
		}

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoControl" /> being used to edit this object.
		/// </summary>
		/// <value>
		/// This value is normally null, but should be non-null during the modal
		/// editing of this object using a <see cref="T:Northwoods.Go.GoControl" /> to host a
		/// <c>Control</c> in a <see cref="T:Northwoods.Go.GoView" />.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoObject.CreateEditor(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoBeginEdit(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" />
		[Browsable(false)]
		public virtual GoControl Editor => null;

		/// <summary>
		/// Called during the first phase of the copy process to produce a copy
		/// of this object within the given copy dictionary.
		/// </summary>
		/// <param name="env">
		/// The <see cref="T:Northwoods.Go.GoCopyDictionary" /> provides the context to be used for performing
		/// the copy.
		/// </param>
		/// <returns>
		/// A newly allocated copy of this object, or null.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If you just need to make a copy of a <see cref="T:Northwoods.Go.GoObject" />, call
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.Copy" />, or
		/// <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.AddCopy(Northwoods.Go.GoObject,System.Drawing.PointF)" />, or
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" /> if
		/// you have more than one object to copy.
		/// </para>
		/// <para>
		/// You should override this method in your derived classes when it has some
		/// fields that have reference objects that need to be copied.
		/// Your override method should first call <c>base.CopyObject(env)</c>
		/// to get the newly copied object.
		/// The standard implementation of this method for <see cref="T:Northwoods.Go.GoObject" />
		/// is to do a shallow copy, by <c>Object.MemberwiseClone()</c>.
		/// The result should be the object you return, after performing any
		/// other deeper copying of referenced objects that you deem necessary,
		/// and after removing references that should not be shared (such as to
		/// cached data structures).
		/// If <c>base.CopyObject(env)</c> returns null, it's either already copied,
		/// or mapped to an existing object in this document,
		/// or shouldn't be copied at all.
		/// </para>
		/// <para>
		/// The copied object does not belong to any layer or any group, nor does
		/// it have any observers; the ultimate caller
		/// (i.e. <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />)
		/// is responsible for adding it to the appropriate
		/// collection and <see cref="M:Northwoods.Go.GoObject.CopyObjectDelayed(Northwoods.Go.GoCopyDictionary,Northwoods.Go.GoObject)" /> is responsible
		/// for adding the appropriate observers.
		/// </para>
		/// <para>
		/// You probably should not be calling this method for any reason
		/// but the initial <c>base.CopyObject(env)</c> call in an override
		/// of this method; normally only
		/// <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" />
		/// is the caller.
		/// Within an override of this method, if you need to make a copy of another object,
		/// call <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" /> instead.
		/// </para>
		/// </remarks>
		public virtual GoObject CopyObject(GoCopyDictionary env)
		{
			GoObject goObject = (GoObject)env[this];
			if (goObject != null)
			{
				return null;
			}
			goObject = (GoObject)(env[this] = (GoObject)MemberwiseClone());
			goObject.myLayer = null;
			goObject.myParent = null;
			if (myObservers != null && myObservers.Count > 0)
			{
				env.Delayeds.Add(this);
			}
			goObject.myObservers = null;
			return goObject;
		}

		/// <summary>
		/// For objects that require a second pass to complete the copying,
		/// this method is called after the first pass of copying all of the objects
		/// in <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
		/// </summary>
		/// <param name="env"></param>
		/// <param name="newobj"></param>
		/// <remarks>
		/// <para>
		/// This method need only be overridden by objects that refer to other
		/// independent objects.  The problem usually is that during the
		/// call to <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" />, some of the objects that this
		/// object refers to might not yet have been copied.  So it's best to
		/// add this object to the <see cref="P:Northwoods.Go.GoCopyDictionary.Delayeds" />
		/// collection in the call to <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" />, and then
		/// fix things up during the second pass in the call to this method.
		/// </para>
		/// <para>
		/// For most objects, this method just adds the new observers that are
		/// copies of any old observers of this object.  For <see cref="T:Northwoods.Go.GoLink" />s,
		/// however, it also makes sure both ports refer to the copied ports, and
		/// if the ports were not copied, it removes the link.
		/// </para>
		/// <para>
		/// The base method should be called by any override that you define.
		/// You should not be calling this method from other code; normally only
		/// <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.FinishDelayedCopies" />
		/// is the caller.
		/// </para>
		/// </remarks>
		public virtual void CopyObjectDelayed(GoCopyDictionary env, GoObject newobj)
		{
			foreach (GoObject observer in Observers)
			{
				GoObject obj = env[observer] as GoObject;
				newobj.AddObserver(obj);
			}
		}

		/// <summary>
		/// This convenience method just makes a copy of the object itself,
		/// using a generic <see cref="T:Northwoods.Go.GoCopyDictionary" />.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// The easiest way to add a copy of an object to a document is to call
		/// <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.AddCopy(Northwoods.Go.GoObject,System.Drawing.PointF)" />.
		/// </para>
		/// <para>
		/// This method is implemented as:
		/// <pre><code>
		///   GoDocument doc = this.Document;
		///   GoCopyDictionary env;
		///   if (doc != null)
		///     env = doc.CreateCopyDictionary();
		///   else
		///     env = new GoCopyDictionary();
		///   return env.CopyComplete(this);
		/// </code></pre>
		/// <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" /> calls
		/// <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> to allow the object to decide how to copy itself.
		/// Although the use of a copy dictionary isn't really needed for the
		/// simplest objects, more complex objects (such as <see cref="T:Northwoods.Go.GoGroup" />s)
		/// may contain references that need to be resolved with a second pass.
		/// </para>
		/// <para>
		/// If you want to copy many objects, it will be more efficient to
		/// allocate your own <see cref="T:Northwoods.Go.GoCopyDictionary" />,
		/// call <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" /> on each object,
		/// and then finally call <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.FinishDelayedCopies" />.
		/// But if you are just going to be adding those copied objects into a <see cref="T:Northwoods.Go.GoDocument" />,
		/// just call <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" /> or
		/// the more general override of that method.
		/// </para>
		/// <para>
		/// If you want to make many copies of one object, it will be more efficient to
		/// allocate your own <see cref="T:Northwoods.Go.GoCopyDictionary" />,
		/// and then for each copy you want to make,
		/// call <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.CopyComplete(Northwoods.Go.GoObject)" /> on the object
		/// followed by a call to <c>Clear()</c> the dictionary.
		/// Clearing the copy dictionary will make sure each <see cref="M:Northwoods.Go.GoCopyDictionary.CopyComplete(Northwoods.Go.GoObject)" />
		/// call does not share any references with any other copy.
		/// </para>
		/// </remarks>
		public GoObject Copy()
		{
			GoDocument document = Document;
			GoCopyDictionary goCopyDictionary = (document == null) ? new GoCopyDictionary() : document.CreateCopyDictionary();
			return goCopyDictionary.CopyComplete(this);
		}

		internal void SetLayer(GoLayer value, GoObject mainObj, bool undoing)
		{
			GoGroup goGroup = this as GoGroup;
			if (goGroup != null)
			{
				GoObject[] array = goGroup.CopyArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetLayer(value, mainObj, undoing);
				}
			}
			GoLayer goLayer = myLayer;
			if (goLayer == value)
			{
				return;
			}
			if (value == null)
			{
				if (!undoing || this is GoControl)
				{
					OnLayerChanged(goLayer, null, mainObj);
					if (myObservers != null && !SuspendsUpdates)
					{
						RectangleF bounds = Bounds;
						GoObject[] array = myObservers.CopyArray();
						for (int i = 0; i < array.Length; i++)
						{
							array[i].OnObservedChanged(this, 903, 0, goLayer, bounds, 0, value, bounds);
						}
					}
				}
				myLayer = null;
				return;
			}
			myLayer = value;
			if (undoing)
			{
				return;
			}
			OnLayerChanged(goLayer, value, mainObj);
			if (myObservers != null && !SuspendsUpdates)
			{
				int subhint = (goLayer == null) ? 902 : 904;
				RectangleF bounds2 = Bounds;
				GoObject[] array = myObservers.CopyArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnObservedChanged(this, subhint, 0, goLayer, bounds2, 0, value, bounds2);
				}
			}
		}

		internal void SetParent(GoGroup value, bool undoing)
		{
			GoGroup goGroup = myParent;
			if (goGroup == value)
			{
				return;
			}
			if (value == null)
			{
				if (!undoing)
				{
					OnParentChanged(goGroup, null);
				}
				SetLayer(null, this, undoing);
				myParent = null;
			}
			else
			{
				myParent = value;
				SetLayer(value.Layer, this, undoing);
				if (!undoing)
				{
					OnParentChanged(goGroup, value);
				}
			}
		}

		/// <summary>
		/// Determines if this object is a child, perhaps indirectly, of the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This predicate will be false if <paramref name="obj" /> is this object itself or
		/// if it is not a <see cref="T:Northwoods.Go.GoGroup" />.
		/// </remarks>
		public bool IsChildOf(GoObject obj)
		{
			if (obj is GoGroup)
			{
				for (GoGroup parent = Parent; parent != null; parent = parent.Parent)
				{
					if (parent == obj)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Find the <see cref="T:Northwoods.Go.GoGroup" /> that is closest parent group for this object and another.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>
		/// This will return null if either argument is null or
		/// if there is no parent group that contains both objects.
		/// If <paramref name="a" /> is the same object as <paramref name="b" />, it will return it.
		/// If <paramref name="a" /> is a parent of <paramref name="b" />,
		/// it will return <paramref name="a" />. 
		/// If <paramref name="b" /> is a parent of <paramref name="a" />,
		/// it will return <paramref name="b" />. 
		/// </returns>
		public static GoObject FindCommonParent(GoObject a, GoObject b)
		{
			if (a == b)
			{
				return a;
			}
			if (a == null)
			{
				return null;
			}
			if (a.Parent == b)
			{
				return b;
			}
			if (b == null)
			{
				return null;
			}
			if (b.Parent == a)
			{
				return a;
			}
			if (a.Parent == b.Parent)
			{
				return a.Parent;
			}
			if (b.Parent == null)
			{
				for (GoObject goObject = a; goObject != null; goObject = goObject.Parent)
				{
					if (goObject == b)
					{
						return b;
					}
				}
			}
			else if (a.Parent == null)
			{
				for (GoObject goObject2 = b; goObject2 != null; goObject2 = goObject2.Parent)
				{
					if (goObject2 == a)
					{
						return a;
					}
				}
			}
			else
			{
				for (GoObject goObject3 = a; goObject3 != null; goObject3 = goObject3.Parent)
				{
					for (GoObject goObject4 = b; goObject4 != null; goObject4 = goObject4.Parent)
					{
						if (goObject4 == goObject3)
						{
							return goObject4;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// This convenience method just removes this object from its container, if there is any.
		/// </summary>
		/// <example>
		/// This is implemented as:
		/// <code>
		///   GoLayer layer = this.Layer;
		///   if (layer != null) {
		///     layer.Remove(this);
		///   } else {
		///     GoGroup parent = this.Parent;
		///     if (parent != null)
		///       parent.Remove(this);
		///   }
		/// </code>
		/// Removing an object from a layer also removes it from its parent group, if any.
		/// </example>
		public void Remove()
		{
			GoLayer layer = Layer;
			if (layer != null)
			{
				layer.Remove(this);
			}
			else
			{
				Parent?.Remove(this);
			}
		}

		/// <summary>
		/// Recalculates the actual bounding rectangle for this object when it might
		/// be invalid.
		/// </summary>
		/// <returns>
		/// The true bounding rectangle, in document coordinates.
		/// </returns>
		/// <remarks>
		/// This method is called if the <see cref="P:Northwoods.Go.GoObject.InvalidBounds" /> property
		/// is true, and some code needs the value of the <see cref="P:Northwoods.Go.GoObject.Bounds" />
		/// property or after the bounds have changed and <see cref="M:Northwoods.Go.GoObject.OnBoundsChanged(System.Drawing.RectangleF)" />
		/// or <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.OnChildBoundsChanged(Northwoods.Go.GoObject,System.Drawing.RectangleF)" /> have been called.
		/// The <see cref="P:Northwoods.Go.GoObject.InvalidBounds" /> property is set back to false
		/// just before calling this method.
		/// </remarks>
		protected virtual RectangleF ComputeBounds()
		{
			return Bounds;
		}

		/// <summary>
		/// Get the spot that is on the opposite side of a given spot.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <returns>
		/// The opposite spot for the standard nine object spots.
		/// For other values, it just returns that value.
		/// </returns>
		/// <remarks>
		/// This is typically used by methods that position objects in
		/// a group (such as <see cref="T:Northwoods.Go.GoGroup" />.<see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />)
		/// or that specify link points for ports
		/// (e.g. <see cref="T:Northwoods.Go.GoPort" />.<see cref="P:Northwoods.Go.GoPort.ToSpot" />).
		/// You may want to override this method to handle your own
		/// custom spot values, those with a value greater than
		/// </remarks>
		/// <see cref="F:Northwoods.Go.GoObject.LastSpot" />.
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		public virtual int SpotOpposite(int spot)
		{
			switch (spot)
			{
			default:
				return spot;
			case 1:
				return 1;
			case 2:
				return 8;
			case 32:
				return 128;
			case 4:
				return 16;
			case 64:
				return 256;
			case 8:
				return 2;
			case 128:
				return 32;
			case 16:
				return 4;
			case 256:
				return 64;
			}
		}

		/// <summary>
		/// Get the position of a spot in a rectangle.
		/// </summary>
		/// <param name="r">a <c>RectangleF</c></param>
		/// <param name="spot">a predefine or user-defined integer spot or handle ID.</param>
		/// <returns>the <c>PointF</c> position of that spot in the rectangle</returns>
		/// <remarks>
		/// This is typically used by methods that need to position objects within a group.
		/// You may want to override this method to handle your own
		/// custom spot values, those with a value greater than <see cref="F:Northwoods.Go.GoObject.LastSpot" />.
		/// If you do so, you will also need to override <see cref="M:Northwoods.Go.GoObject.SetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32,System.Drawing.PointF)" />
		/// and <see cref="M:Northwoods.Go.GoObject.SpotOpposite(System.Int32)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.SetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		public virtual PointF GetRectangleSpotLocation(RectangleF r, int spot)
		{
			PointF result = new PointF(r.X, r.Y);
			switch (spot)
			{
			case 1:
				result.X += r.Width / 2f;
				result.Y += r.Height / 2f;
				break;
			case 32:
				result.X += r.Width / 2f;
				break;
			case 4:
				result.X += r.Width;
				break;
			case 64:
				result.X += r.Width;
				result.Y += r.Height / 2f;
				break;
			case 8:
				result.X += r.Width;
				result.Y += r.Height;
				break;
			case 128:
				result.X += r.Width / 2f;
				result.Y += r.Height;
				break;
			case 16:
				result.Y += r.Height;
				break;
			case 256:
				result.Y += r.Height / 2f;
				break;
			}
			return result;
		}

		/// <summary>
		/// Modify a rectangle such that its new spot location is at a given point.
		/// </summary>
		/// <param name="r">a <c>RectangleF</c></param>
		/// <param name="spot">a predefine or user-defined integer spot or handle ID.</param>
		/// <param name="p">a <c>PointF</c> specifying the desired new location for the rectangle</param>
		/// <returns>the modified <c>RectangleF</c>; the original Width and Height are kept.</returns>
		/// <remarks>
		/// This is typically used by methods that need to position objects within a group.
		/// You may want to override this method to handle your own
		/// custom spot values, those with a value greater than <see cref="F:Northwoods.Go.GoObject.LastSpot" />.
		/// If you do so, you will also need to override <see cref="M:Northwoods.Go.GoObject.GetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32)" />
		/// and <see cref="M:Northwoods.Go.GoObject.SpotOpposite(System.Int32)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		public virtual RectangleF SetRectangleSpotLocation(RectangleF r, int spot, PointF p)
		{
			switch (spot)
			{
			case 1:
				r.X = p.X - r.Width / 2f;
				r.Y = p.Y - r.Height / 2f;
				break;
			default:
				r.X = p.X;
				r.Y = p.Y;
				break;
			case 32:
				r.X = p.X - r.Width / 2f;
				r.Y = p.Y;
				break;
			case 4:
				r.X = p.X - r.Width;
				r.Y = p.Y;
				break;
			case 64:
				r.X = p.X - r.Width;
				r.Y = p.Y - r.Height / 2f;
				break;
			case 8:
				r.X = p.X - r.Width;
				r.Y = p.Y - r.Height;
				break;
			case 128:
				r.X = p.X - r.Width / 2f;
				r.Y = p.Y - r.Height;
				break;
			case 16:
				r.X = p.X;
				r.Y = p.Y - r.Height;
				break;
			case 256:
				r.X = p.X;
				r.Y = p.Y - r.Height / 2f;
				break;
			}
			return r;
		}

		/// <summary>
		/// Get the position of a spot on this object.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <returns>
		/// A <c>PointF</c> value in document coordinates.
		/// </returns>
		/// <remarks>
		/// If you want to define your own custom spots, you may wish
		/// to override the <see cref="M:Northwoods.Go.GoObject.GetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32)" /> method
		/// and related methods.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,Northwoods.Go.GoObject,System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SpotOpposite(System.Int32)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Location" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Position" />
		public virtual PointF GetSpotLocation(int spot)
		{
			RectangleF bounds = Bounds;
			return GetRectangleSpotLocation(bounds, spot);
		}

		/// <summary>
		/// Move this object so this object's given spot is at the given location.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <param name="newp">
		/// The new <c>PointF</c> position for this object's spot to be at.
		/// </param>
		/// <remarks>
		/// If you want to define your own custom spots, you may wish
		/// to override the <see cref="M:Northwoods.Go.GoObject.SetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32,System.Drawing.PointF)" /> method
		/// and related methods.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.SetRectangleSpotLocation(System.Drawing.RectangleF,System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,Northwoods.Go.GoObject,System.Int32)" />
		public virtual void SetSpotLocation(int spot, PointF newp)
		{
			RectangleF bounds = Bounds;
			Bounds = SetRectangleSpotLocation(bounds, spot, newp);
		}

		/// <summary>
		/// Move this object so this object's given spot is the same as another
		/// object's spot's position.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <param name="obj">another <see cref="T:Northwoods.Go.GoObject" /></param>
		/// <param name="otherSpot">a spot on the other object, <paramref name="obj" /></param>
		/// <remarks>
		/// <para>
		/// Use this method when you want a spot on this object to be exactly at
		/// a spot on another object, by moving this object.
		/// </para>
		/// <para>
		/// Remember that this method just moves this object once.
		/// When other objects in a group resize or move, this object will
		/// not automatically follow.  You will need to override <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />
		/// to maintain whatever layout you want.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		public void SetSpotLocation(int spot, GoObject obj, int otherSpot)
		{
			PointF spotLocation = obj.GetSpotLocation(otherSpot);
			SetSpotLocation(spot, spotLocation);
		}

		/// <summary>
		/// Move this object so this object's given spot is the same as another
		/// object's spot's position and offset by a given distance.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <param name="obj">another <see cref="T:Northwoods.Go.GoObject" /></param>
		/// <param name="otherSpot">a spot on the other object, <paramref name="obj" /></param>
		/// <param name="offset"></param>
		/// <remarks>
		/// <para>
		/// Use this method when you want a spot on this object to be near,
		/// but not exactly at, a spot on another object, by moving this object.
		/// This is just like <see cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,Northwoods.Go.GoObject,System.Int32)" />,
		/// except that it first adds the <paramref name="offset" /> width and height
		/// to the point of the <paramref name="obj" />'s <paramref name="otherSpot" />.
		/// </para>
		/// <para>
		/// Remember that this method just moves this object once.
		/// When other objects in a group resize or move, this object will
		/// not automatically follow.  You will need to override <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />
		/// to maintain whatever layout you want.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		public void SetSpotLocation(int spot, GoObject obj, int otherSpot, SizeF offset)
		{
			PointF spotLocation = obj.GetSpotLocation(otherSpot);
			SetSpotLocation(spot, new PointF(spotLocation.X + offset.Width, spotLocation.Y + offset.Height));
		}

		/// <summary>
		/// Move this object so this object's given spot is the same as another
		/// object's spot's position and offset by a given X and Y distance.
		/// </summary>
		/// <param name="spot">
		/// A predefined or user-defined integer spot or handle ID.
		/// </param>
		/// <param name="obj">another <see cref="T:Northwoods.Go.GoObject" /></param>
		/// <param name="otherSpot">a spot on the other object, <paramref name="obj" /></param>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		/// <remarks>
		/// <para>
		/// Use this method when you want a spot on this object to be near,
		/// but not exactly at, a spot on another object, by moving this object.
		/// This is just like <see cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,Northwoods.Go.GoObject,System.Int32)" />,
		/// except that it first adds the <paramref name="dx" /> and <paramref name="dy" />
		/// offsets to the point of the <paramref name="obj" />'s <paramref name="otherSpot" />.
		/// </para>
		/// <para>
		/// Remember that this method just moves this object once.
		/// When other objects in a group resize or move, this object will
		/// not automatically follow.  You will need to override <see cref="M:Northwoods.Go.GoGroup.LayoutChildren(Northwoods.Go.GoObject)" />
		/// to maintain whatever layout you want.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetSpotLocation(System.Int32)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.SetSpotLocation(System.Int32,System.Drawing.PointF)" />
		public void SetSpotLocation(int spot, GoObject obj, int otherSpot, float dx, float dy)
		{
			PointF spotLocation = obj.GetSpotLocation(otherSpot);
			SetSpotLocation(spot, new PointF(spotLocation.X + dx, spotLocation.Y + dy));
		}

		/// <summary>
		/// Sets a new size for this object while maintaining the same location.
		/// </summary>
		/// <param name="s"></param>
		/// <remarks>
		/// If the natural location of an object is not the top-left corner,
		/// changing the size of an object will also change its location.
		/// However, there are times when you wish to change the size without
		/// changing the location.
		/// This method is more efficient than remembering the Location,
		/// setting the Size, and then setting the Location again.
		/// By default this assumes the <see cref="P:Northwoods.Go.GoObject.Location" /> is really the
		/// <see cref="F:Northwoods.Go.GoObject.TopLeft" /> spot of the object.
		/// If you override this method, you should also override the
		/// <see cref="P:Northwoods.Go.GoObject.Location" /> property.
		/// </remarks>
		public virtual void SetSizeKeepingLocation(SizeF s)
		{
			Size = s;
		}

		/// <summary>
		/// Compute a <c>SizeF</c> that fits in <paramref name="target" /> while maintaining
		/// the aspect ratio given by <paramref name="aspect" />.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="aspect">if both width and height are zero or negative, assume 1x1</param>
		/// <returns></returns>
		public static SizeF LargestSizeKeepingAspectRatio(SizeF target, SizeF aspect)
		{
			float num = Math.Max(0f, aspect.Width);
			float num2 = Math.Max(0f, aspect.Height);
			if (num == 0f && num2 == 0f)
			{
				num = 1f;
				num2 = 1f;
			}
			float num3 = Math.Max(0f, target.Width);
			float num4 = Math.Max(0f, target.Height);
			if (num == 0f)
			{
				return new SizeF(0f, num4);
			}
			if (num2 == 0f)
			{
				return new SizeF(num3, 0f);
			}
			if (num3 == 0f || num4 == 0f)
			{
				return new SizeF(num3, num4);
			}
			float num5 = num2 / num;
			float num6 = num4 / num3;
			if (num5 < num6)
			{
				return new SizeF(num3, num5 * num3);
			}
			return new SizeF(num4 / num5, num4);
		}

		/// <summary>
		/// Called to see if the user can see this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Visible</c>, if its parent
		/// is visible, and if this object is
		/// part of a layer, if <c>Layer.CanViewObjects</c> is true.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> and <see cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" />.
		/// Painting operations may also want to call <see cref="M:Northwoods.Go.GoObject.CanPrint" />
		/// if the painting is occurring for a view that is printing.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Visible" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanViewObjects" />
		public virtual bool CanView()
		{
			if (!Visible)
			{
				return false;
			}
			if (Parent != null)
			{
				return Parent.CanView();
			}
			if (Layer != null)
			{
				return Layer.CanViewObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the view can print this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Printable</c>, if its parent
		/// is printable, and if this object is
		/// part of a layer, whether <c>Layer.CanPrintObjects</c> is true.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />
		/// when <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.IsPrinting" /> is true for a view.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Printable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanPrintObjects" />
		public virtual bool CanPrint()
		{
			if (!Printable)
			{
				return false;
			}
			if (Parent != null)
			{
				return Parent.CanPrint();
			}
			if (Layer != null)
			{
				return Layer.CanPrintObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can select this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Selectable</c>, and if this object is
		/// part of a layer, if <c>Layer.CanSelectObjects</c>.
		/// This object's parent need not be selectable for this
		/// object to be selectable.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />
		/// and <see cref="M:Northwoods.Go.GoObject.Pick(System.Drawing.PointF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Selectable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanSelectObjects" />
		public virtual bool CanSelect()
		{
			if (!Selectable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanSelectObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can move this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Movable</c>, and, if this object is
		/// in a layer, if <c>Layer.CanMoveObjects</c>.
		/// This object's parent need not be movable for this
		/// object to be movable.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Movable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanMoveObjects" />
		public virtual bool CanMove()
		{
			if (!Movable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanMoveObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can copy this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Copyable</c>, and, if this object is
		/// in a layer, if <c>Layer.CanCopyObjects</c>.
		/// This object's parent need not be copyable for this
		/// object to be copyable.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Copyable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanCopyObjects" />
		public virtual bool CanCopy()
		{
			if (!Copyable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanCopyObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can resize this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <see cref="P:Northwoods.Go.GoObject.Resizable" />, and, if this object is
		/// in a layer, if <c>Layer.CanResizeObjects()</c> is true.
		/// This object's parent need not be resizable for this
		/// object to be resizable.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Resizable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanResizeObjects" />
		public virtual bool CanResize()
		{
			if (!Resizable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanResizeObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can reshape this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Reshapable</c>, and, if this object is
		/// in a layer, if <c>Layer.CanReshapeObjects</c>.
		/// This object's parent need not be reshapable for this
		/// object to be reshapable.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Reshapable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanReshapeObjects" />
		public virtual bool CanReshape()
		{
			if (!Reshapable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanReshapeObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can delete this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Deletable</c>, and, if this object is
		/// in a layer, if <c>Layer.CanDeleteObjects</c>.
		/// This object's parent need not be deletable for this
		/// object to be deletable.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Deletable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanDeleteObjects" />
		public virtual bool CanDelete()
		{
			if (!Deletable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanDeleteObjects();
			}
			return true;
		}

		/// <summary>
		/// Called to see if the user can edit this object.
		/// </summary>
		/// <remarks>
		/// This returns true if <c>Editable</c>, and, if this object is
		/// in a layer, if <c>Layer.CanEditObjects</c>.
		/// This object's parent need not be editable for this
		/// object to be editable.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoText" />.<see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoObject.Editable" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanEditObjects" />
		public virtual bool CanEdit()
		{
			if (!Editable)
			{
				return false;
			}
			if (Layer != null)
			{
				return Layer.CanEditObjects();
			}
			return true;
		}

		internal void SetBeingRemoved(bool value)
		{
			if (value)
			{
				InternalFlags |= 65536;
			}
			else
			{
				InternalFlags &= -65537;
			}
		}

		/// <summary>
		/// Notify this object's document or view that some part of this object's
		/// state is about to be changed.
		/// </summary>
		/// <param name="subhint"></param>
		/// <remarks>
		/// Normally this method is only called for the benefit of the
		/// <see cref="T:Northwoods.Go.GoUndoManager" /> to record larger or more complex state
		/// before a change than can easily or efficiently be passed as the
		/// "old" or "previous" value in a call to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// This method does nothing if <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> is true.
		/// If you intend to turn on <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> for a while,
		/// to reduce the overhead of repeated notifications,
		/// you probably should call this method first and make sure the
		/// <see cref="M:Northwoods.Go.GoObject.CopyOldValueForUndo(Northwoods.Go.GoChangedEventArgs)" /> and <see cref="M:Northwoods.Go.GoObject.CopyNewValueForRedo(Northwoods.Go.GoChangedEventArgs)" />
		/// methods can remember all of the relevant state before the unrecorded
		/// changes occur.
		/// </remarks>
		public virtual void Changing(int subhint)
		{
			if (!SuspendsUpdates)
			{
				Document?.RaiseChanging(901, subhint, this);
			}
		}

		/// <summary>
		/// Notify this object's document or view that some part of this object's
		/// state has been changed, via the <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" />
		/// event and using the <see cref="T:Northwoods.Go.GoChangedEventArgs" /> event args class.
		/// </summary>
		/// <param name="subhint">
		/// the value for <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.SubHint" />
		/// </param>
		/// <param name="oldI">
		/// An <c>integer</c> value.
		/// </param>
		/// <param name="oldVal">
		/// An <c>Object</c> value.
		/// </param>
		/// <param name="oldRect">
		/// A <c>RectangleF</c> value, also useful for <c>PointF</c>, <c>SizeF</c>, and <c>float</c> values.
		/// </param>
		/// <param name="newI">
		/// An <c>integer</c> value.
		/// </param>
		/// <param name="newVal">
		/// An <c>Object</c> value.
		/// </param>
		/// <param name="newRect">
		/// A <c>RectangleF</c> value, also useful for <c>PointF</c>, <c>SizeF</c>, and <c>float</c> values.
		/// </param>
		/// <remarks>
		/// <para>
		/// Any override of this method and any methods that this calls should not
		/// further modify this object, either directly or indirectly.
		/// </para>
		/// <para>
		/// Each of your property setters should call this method after actually
		/// changing the object's state.
		/// You should only call this method if the property's value actually changed.
		/// You should use the <paramref name="oldI" />, <paramref name="oldVal" />,
		/// and/or <paramref name="oldRect" /> parameters for passing the previous old
		/// property value.
		/// You should use the <paramref name="newI" />, <paramref name="newVal" />,
		/// and/or <paramref name="newRect" /> parameters for passing the new
		/// property value.
		/// The old and new values can be used by <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" />
		/// event handlers to update their state.
		/// A special case of such event handlers is the <see cref="T:Northwoods.Go.GoUndoManager" />,
		/// which records the old and new values so that it can perform undo and redo
		/// operations.
		/// </para>
		/// <para>
		/// If <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> is true (it is normally false of course),
		/// this method does nothing.  Your override of this method should check for
		/// <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> before invoking any updating functionality.
		/// If this object is part of a layer, it calls
		/// <see cref="M:Northwoods.Go.IGoLayerCollectionContainer.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// with a hint of <see cref="T:Northwoods.Go.GoLayer" />.<see cref="F:Northwoods.Go.GoLayer.ChangedObject" />.
		/// If there are any observers watching changes to this object,
		/// this method calls <see cref="M:Northwoods.Go.GoObject.OnObservedChanged(Northwoods.Go.GoObject,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> on each of this object's
		/// observers.
		/// </para>
		/// <para>
		/// A number of subhints are pre-defined:
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoObject" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.RepaintAll" /></term> <term>1000</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedBounds" /></term> <term>1001</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedVisible" /></term> <term>1003</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedSelectable" /></term> <term>1004</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedMovable" /></term> <term>1005</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedCopyable" /></term> <term>1006</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedResizable" /></term> <term>1007</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedReshapable" /></term> <term>1008</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedDeletable" /></term> <term>1009</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedEditable" /></term> <term>1010</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedAutoRescales" /></term> <term>1011</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedResizesRealtime" /></term> <term>1012</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedShadowed" /></term> <term>1013</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedAddedObserver" /></term> <term>1014</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedRemovedObserver" /></term> <term>1015</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedDragsNode" /></term> <term>1016</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedPrintable" /></term> <term>1017</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.ChangedInitializing" /></term> <term>1041</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoObject.LastChangedHint" /></term> <term>10000</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoGroup" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.InsertedObject" /></term> <term>1051</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.RemovedObject" /></term> <term>1052</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.ChangedZOrder" /></term> <term>1053</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.ReplacedObject" /></term> <term>1054</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.ChangedPickableBackground" /></term> <term>1055</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.AddedChildName" /></term> <term>1056</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGroup.RemovedChildName" /></term> <term>1057</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoShape" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoShape.ChangedPen" /></term> <term>1101</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoShape.ChangedBrush" /></term> <term>1102</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoDrawing" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.AddedPoint" /></term> <term>1151</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.RemovedPoint" /></term> <term>1152</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ModifiedPoint" /></term> <term>1153</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedData" /></term> <term>1154</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedFillMode" /></term> <term>1155</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedAngle" /></term> <term>1156</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedUnrotatedBounds" /></term> <term>1160</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedSmoothCurves" /></term> <term>1161</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedSameEndPoints" /></term> <term>1162</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedShowsBoundingHandle" /></term> <term>1163</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedReshapablePoints" /></term> <term>1164</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedReshapableRectangle" /></term> <term>1165</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedPickMargin" /></term> <term>1166</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDrawing.ChangedFigure" /></term> <term>1167</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoStroke" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.AddedPoint" /></term> <term>1201</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.RemovedPoint" /></term> <term>1202</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ModifiedPoint" /></term> <term>1203</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedAllPoints" /></term> <term>1204</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedStyle" /></term> <term>1205</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedCurviness" /></term> <term>1206</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedHighlightPen" /></term> <term>1236</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedHighlight" /></term> <term>1237</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedHighlightWhenSelected" /></term> <term>1238</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowHead" /></term> <term>1250</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowLength" /></term> <term>1251</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowShaftLength" /></term> <term>1252</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowWidth" /></term> <term>1253</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowFilled" /></term> <term>1254</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedToArrowStyle" /></term> <term>1255</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowHead" /></term> <term>1260</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowLength" /></term> <term>1261</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowShaftLength" /></term> <term>1262</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowWidth" /></term> <term>1263</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowFilled" /></term> <term>1264</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoStroke.ChangedFromArrowStyle" /></term> <term>1265</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoLink" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedLinkUserFlags" /></term> <term>1300</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedLinkUserObject" /></term> <term>1301</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedFromPort" /></term> <term>1302</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedToPort" /></term> <term>1303</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedOrthogonal" /></term> <term>1304</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedRelinkable" /></term> <term>1305</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedAbstractLink" /></term> <term>1306</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedAvoidsNodes" /></term> <term>1307</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedPartID" /></term> <term>1309</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedAdjustingStyle" /></term> <term>1310</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedToolTipText" /></term> <term>1311</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLink.ChangedDraggableOrthogonalSegments" /></term> <term>1312</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoLabeledLink" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedLink" /></term> <term>1311</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedFromLabel" /></term> <term>1312</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedMidLabel" /></term> <term>1313</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedToLabel" /></term> <term>1314</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedFromLabelCentered" /></term> <term>1315</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedMidLabelCentered" /></term> <term>1316</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLabeledLink.ChangedToLabelCentered" /></term> <term>1317</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoPolygon" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoPolygon.AddedPoint" /></term> <term>1401</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPolygon.RemovedPoint" /></term> <term>1402</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPolygon.ModifiedPoint" /></term> <term>1403</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPolygon.ChangedAllPoints" /></term> <term>1412</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPolygon.ChangedStyle" /></term> <term>1414</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoRoundedRectangle" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoRoundedRectangle.ChangedCorner" /></term> <term>1421</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoRoundedRectangle.ChangedRoundedCornerSpots" /></term> <term>1422</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoTriangle" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoTriangle.ChangedPointA" /></term> <term>1431</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTriangle.ChangedPointB" /></term> <term>1432</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTriangle.ChangedPointC" /></term> <term>1433</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTriangle.ChangedAllPoints" /></term> <term>1434</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoHexagon" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedDistanceLeft" /></term> <term>1442</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedDistanceRight" /></term> <term>1443</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedDistanceTop" /></term> <term>1444</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedDistanceBottom" /></term> <term>1445</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedOrientation" /></term> <term>1446</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedReshapeBehavior" /></term> <term>1447</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedReshapableCorner" /></term> <term>1448</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedKeepsLengthwiseSymmetry" /></term> <term>1449</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoHexagon.ChangedKeepsCrosswiseSymmetry" /></term> <term>1450</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoPie" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoPie.ChangedStartAngle" /></term> <term>1451</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPie.ChangedSweepAngle" /></term> <term>1452</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPie.ChangedResizableStartAngle" /></term> <term>1453</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPie.ChangedResizableEndAngle" /></term> <term>1454</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoTrapezoid" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedPointA" /></term> <term>1460</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedPointB" /></term> <term>1461</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedPointC" /></term> <term>1462</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedPointD" /></term> <term>1463</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedAllPoints" /></term> <term>1464</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTrapezoid.ChangedOrientation" /></term> <term>1465</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoParallelogram" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoParallelogram.ChangedSkew" /></term> <term>1466</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoParallelogram.ChangedReshapableSkew" /></term> <term>1467</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoParallelogram.ChangedDirection" /></term> <term>1468</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoOctagon" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoOctagon.ChangedCorner" /></term> <term>1469</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoOctagon.ChangedReshapableCorner" /></term> <term>1470</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoCylinder" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoCylinder.ChangedMinorRadius" /></term> <term>1481</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCylinder.ChangedOrientation" /></term> <term>1482</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCylinder.ChangedPerspective" /></term> <term>1483</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCylinder.ChangedResizableRadius" /></term> <term>1484</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoCube" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoCube.ChangedDepth" /></term> <term>1491</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCube.ChangedPerspective" /></term> <term>1492</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCube.ChangedReshapableDepth" /></term> <term>1493</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoText" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedText" /></term> <term>1501</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedFamilyName" /></term> <term>1502</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedFontSize" /></term> <term>1503</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedAlignment" /></term> <term>1504</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedTextColor" /></term> <term>1505</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedBackgroundColor" /></term> <term>1506</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedTransparentBackground" /></term> <term>1507</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedBold" /></term> <term>1508</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedItalic" /></term> <term>1509</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedUnderline" /></term> <term>1510</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedStrikeThrough" /></term> <term>1511</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedMultiline" /></term> <term>1512</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedBackgroundOpaqueWhenSelected" /></term> <term>1515</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedClipping" /></term> <term>1516</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedAutoResizes" /></term> <term>1518</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedWrapping" /></term> <term>1520</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedWrappingWidth" /></term> <term>1521</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedGdiCharSet" /></term> <term>1522</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedEditorStyle" /></term> <term>1523</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedMinimum" /></term> <term>1524</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedMaximum" /></term> <term>1525</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedDropDownList" /></term> <term>1526</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedChoices" /></term> <term>1527</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedRightToLeft" /></term> <term>1528</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedRightToLeftFromView" /></term> <term>1529</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedBordered" /></term> <term>1530</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedStringTrimming" /></term> <term>1531</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoText.ChangedEditableWhenSelected" /></term> <term>1532</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoImage" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedImage" /></term> <term>1601</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedResourceManager" /></term> <term>1602</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedName" /></term> <term>1603</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedAlignment" /></term> <term>1604</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedAutoResizes" /></term> <term>1605</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedImageList" /></term> <term>1606</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedIndex" /></term> <term>1607</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedThrowsExceptions" /></term> <term>1608</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoImage.ChangedNameIsUri" /></term> <term>1609</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoPort" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedPortUserFlags" /></term> <term>1700</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedPortUserObject" /></term> <term>1701</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedStyle" /></term> <term>1702</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedObject" /></term> <term>1703</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedValidFrom" /></term> <term>1704</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedValidTo" /></term> <term>1705</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedValidSelfNode" /></term> <term>1706</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedFromSpot" /></term> <term>1707</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedToSpot" /></term> <term>1708</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedAddedLink" /></term> <term>1709</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedRemovedLink" /></term> <term>1710</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedValidDuplicateLinks" /></term> <term>1711</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedEndSegmentLength" /></term> <term>1712</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedPartID" /></term> <term>1713</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedClearsLinksWhenRemoved" /></term> <term>1714</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoPort.ChangedValidSingleLink" /></term> <term>1715</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoGrid" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedStyle" /></term> <term>1801</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedOrigin" /></term> <term>1802</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedOriginRelative" /></term> <term>1803</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedCellSize" /></term> <term>1804</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedLineColor" /></term> <term>1805</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedLineWidth" /></term> <term>1806</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedLineDashStyle" /></term> <term>1807</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedSnapDrag" /></term> <term>1808</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedSnapResize" /></term> <term>1809</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedCellColors" /></term> <term>1810</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedUnboundedSpots" /></term> <term>1811</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedSnapDragWhole" /></term> <term>1812</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedSnapOpaque" /></term> <term>1814</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedSnapCellSpot" /></term> <term>1815</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedMajorLineColor" /></term> <term>1816</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedMajorLineWidth" /></term> <term>1817</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedMajorLineDashStyle" /></term> <term>1818</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedMajorLineFrequency" /></term> <term>1819</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedLineDashPattern" /></term> <term>1820</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGrid.ChangedMajorLineDashPattern" /></term> <term>1821</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoControl" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoControl.ChangedControlType" /></term> <term>1901</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoNode.ChangedNodeUserFlags" /></term> <term>2000</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoNode.ChangedNodeUserObject" /></term> <term>2001</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoNode.ChangedToolTipText" /></term> <term>2002</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoNode.ChangedPartID" /></term> <term>2004</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoNodeIcon" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoNodeIcon.ChangedMinimumIconSize" /></term> <term>2050</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoNodeIcon.ChangedMaximumIconSize" /></term> <term>2051</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoBasicNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedLabelSpot" /></term> <term>2101</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedShape" /></term> <term>2102</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedLabel" /></term> <term>2103</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedPort" /></term> <term>2104</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedMiddleLabelMargin" /></term> <term>2105</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBasicNode.ChangedAutoResizes" /></term> <term>2106</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoBoxNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxNode.ChangedBody" /></term> <term>2201</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxNode.ChangedPortBorderMargin" /></term> <term>2202</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxNode.ChangedPort" /></term> <term>2203</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoBoxPort" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxPort.ChangedLinkPointsSpread" /></term> <term>2211</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxPort.ChangedFromSides" /></term> <term>2212</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBoxPort.ChangedToSides" /></term> <term>2213</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoComment" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoComment.ChangedTopLeftMargin" /></term> <term>2301</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoComment.ChangedBottomRightMargin" /></term> <term>2302</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoComment.ChangedPartID" /></term> <term>2303</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoComment.ChangedBackground" /></term> <term>2304</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoComment.ChangedLabel" /></term> <term>2305</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoBalloon" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoBalloon.ChangedAnchor" /></term> <term>2310</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBalloon.ChangedBaseWidth" /></term> <term>2312</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBalloon.ChangedUnanchoredOffset" /></term> <term>2313</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoBalloon.ChangedReanchorable" /></term> <term>2314</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoGeneralNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.InsertedPort" /></term> <term>2401</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.RemovedPort" /></term> <term>2402</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ReplacedPort" /></term> <term>2403</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedTopLabel" /></term> <term>2404</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedBottomLabel" /></term> <term>2405</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedIcon" /></term> <term>2406</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedOrientation" /></term> <term>2407</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedFromEndSegmentLengthStep" /></term> <term>2408</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedToEndSegmentLengthStep" /></term> <term>2409</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedLeftPortsAlignment" /></term> <term>2410</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedRightPortsAlignment" /></term> <term>2411</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedLeftPortsLabelSpacing" /></term> <term>2412</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedRightPortsLabelSpacing" /></term> <term>2413</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedLeftPortLabelsInside" /></term> <term>2414</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNode.ChangedRightPortLabelsInside" /></term> <term>2415</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoGeneralNodePort" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNodePort.ChangedName" /></term> <term>2430</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNodePort.ChangedLabel" /></term> <term>2431</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNodePort.ChangedSideIndex" /></term> <term>2432</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoGeneralNodePort.ChangedLeftSide" /></term> <term>2433</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoListGroup" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedSpacing" /></term> <term>2501</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedAlignment" /></term> <term>2502</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedLinePen" /></term> <term>2503</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedBorderPen" /></term> <term>2504</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedBrush" /></term> <term>2505</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedCorner" /></term> <term>2506</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedTopLeftMargin" /></term> <term>2507</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedBottomRightMargin" /></term> <term>2508</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedOrientation" /></term> <term>2509</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedTopIndex" /></term> <term>2510</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoListGroup.ChangedMinimumItemSize" /></term> <term>2511</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoSimpleNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedText" /></term> <term>2601</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedIcon" /></term> <term>2602</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedLabel" /></term> <term>2603</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedInPort" /></term> <term>2604</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedOutPort" /></term> <term>2605</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSimpleNode.ChangedOrientation" /></term> <term>2606</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoIconicNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoIconicNode.ChangedDraggableLabel" /></term> <term>2651</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoIconicNode.ChangedIcon" /></term> <term>2652</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoIconicNode.ChangedLabel" /></term> <term>2653</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoIconicNode.ChangedPort" /></term> <term>2654</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoIconicNode.ChangedLabelOffset" /></term> <term>2655</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoSubGraph" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedLabel" /></term> <term>2702</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsible" /></term> <term>2703</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedBackgroundColor" /></term> <term>2704</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedOpacity" /></term> <term>2705</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedLabelSpot" /></term> <term>2706</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedTopLeftMargin" /></term> <term>2707</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedBorderPen" /></term> <term>2708</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCorner" /></term> <term>2710</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedPort" /></term> <term>2711</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedBottomRightMargin" /></term> <term>2712</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsedTopLeftMargin" /></term> <term>2713</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsedBottomRightMargin" /></term> <term>2714</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsedCorner" /></term> <term>2715</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsedLabelSpot" /></term> <term>2716</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedCollapsedObject" /></term> <term>2717</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedState" /></term> <term>2718</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedSavedBounds" /></term> <term>2719</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedSavedPaths" /></term> <term>2720</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedWasExpanded" /></term> <term>2721</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSubGraph.ChangedExpandedResizable" /></term> <term>2722</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoTextNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedLabel" /></term> <term>2801</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedBackground" /></term> <term>2802</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedTopPort" /></term> <term>2803</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedRightPort" /></term> <term>2804</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedBottomPort" /></term> <term>2805</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedLeftPort" /></term> <term>2806</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedTopLeftMargin" /></term> <term>2807</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedBottomRightMargin" /></term> <term>2808</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoTextNode.ChangedAutoResizes" /></term> <term>2809</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoButton" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedBackground" /></term> <term>2901</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedIcon" /></term> <term>2902</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedLabel" /></term> <term>2903</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedTopLeftMargin" /></term> <term>2904</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedBottomRightMargin" /></term> <term>2905</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedActionEnabled" /></term> <term>2906</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoButton.ChangedAutoRepeating" /></term> <term>2907</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoCollapsibleHandle" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoCollapsibleHandle.ChangedStyle" /></term> <term>2950</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoCollapsibleHandle.ChangedBordered" /></term> <term>2951</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoMultiTextNode" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.InsertedLeftPort" /></term> <term>3001</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.InsertedRightPort" /></term> <term>3002</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.RemovedLeftPort" /></term> <term>3003</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.RemovedRightPort" /></term> <term>3004</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ReplacedPort" /></term> <term>3005</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ChangedTopPort" /></term> <term>3006</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ChangedBottomPort" /></term> <term>3007</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ChangedItemWidth" /></term> <term>3008</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ChangedFromEndSegmentLengthStep" /></term> <term>3009</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoMultiTextNode.ChangedToEndSegmentLengthStep" /></term> <term>3010</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoSheet" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedTopLeftMargin" /></term> <term>3101</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedBottomRightMargin" /></term> <term>3102</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedBackgroundImageSpot" /></term> <term>3103</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedShowsMargins" /></term> <term>3104</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedMarginColor" /></term> <term>3105</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedPaper" /></term> <term>3110</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedBackgroundImage" /></term> <term>3111</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoSheet.ChangedGrid" /></term> <term>3112</term> </item>
		/// </list>
		/// Please note that this list may not be complete--in fact you are encouraged to
		/// add new subhints for your own properties and other changes.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.AddObserver(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.RemoveObserver(Northwoods.Go.GoObject)" />
		public virtual void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (SuspendsUpdates)
			{
				return;
			}
			if (InvalidBounds)
			{
				_ = Bounds;
			}
			Layer?.LayerCollectionContainer?.RaiseChanged(901, subhint, this, oldI, oldVal, oldRect, newI, newVal, newRect);
			if (myObservers != null)
			{
				GoObject[] array = myObservers.CopyArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnObservedChanged(this, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				}
			}
		}

		/// <summary>
		/// Cause all views to repaint this object when they get a chance.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> with a <see cref="F:Northwoods.Go.GoObject.RepaintAll" /> subhint.
		/// You will only need to call this if you have overridden <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />
		/// and the area that you paint extends beyond the regular <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// The primary usage is before making any change that affects only the paint bounds,
		/// as determined by <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />.
		/// </remarks>
		public void InvalidateViews()
		{
			if (Parent != null)
			{
				Parent.InvalidatePaintBounds();
			}
			Changed(1000, 0, null, NullRect, 0, null, NullRect);
		}

		/// <summary>
		/// Called when this object's layer is changed.
		/// </summary>
		/// <param name="oldlayer">
		/// This <see cref="T:Northwoods.Go.GoLayer" /> value is the old value of <see cref="P:Northwoods.Go.GoObject.Layer" />.
		/// The value will be null if the object did not belong to a layer but
		/// is being added to one.
		/// </param>
		/// <param name="newlayer">
		/// This <see cref="T:Northwoods.Go.GoLayer" /> value is the new value of <see cref="P:Northwoods.Go.GoObject.Layer" />.
		/// The value will be null if the object is being removed from a layer.
		/// </param>
		/// <param name="mainObj">
		/// This is the object that is being inserted or removed from a layer.
		/// </param>
		/// <remarks>
		/// When this object is being removed from its layer, this method is
		/// called before the actual value of <see cref="P:Northwoods.Go.GoObject.Layer" /> is set to null.
		/// Otherwise this method is called after the layer has been changed.
		/// Both <paramref name="oldlayer" /> and <paramref name="newlayer" />
		/// can be non-null when the object is being moved from one layer to another.
		/// Unlike <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// this method will be called even when this object is not part of a document,
		/// or when <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> is true.
		/// However, this method is not called upon undo or redo of a change of layer.
		/// By default this method does nothing.
		/// Any implementation of this method should not cause a change in layers.
		/// </remarks>
		protected virtual void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
		}

		/// <summary>
		/// Called when this object's parent is changed.
		/// </summary>
		/// <param name="oldgroup">
		/// This <see cref="T:Northwoods.Go.GoGroup" /> value is the old value of <see cref="P:Northwoods.Go.GoObject.Parent" />.
		/// The value will be null if the object did not belong to a group but
		/// is being added to one.
		/// </param>
		/// <param name="newgroup">
		/// This <see cref="T:Northwoods.Go.GoGroup" /> value is the new value of <see cref="P:Northwoods.Go.GoObject.Parent" />.
		/// The value will be null if the object is being removed from a group.
		/// </param>
		/// <remarks>
		/// When this object is being removed from its group, this method is
		/// called before the actual value of <see cref="P:Northwoods.Go.GoObject.Parent" /> is set to null.
		/// Otherwise this method is called after the parent has been changed.
		/// When this object is being removed from its group, its <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// will be set to null too.
		/// When this object is being added to a group, its <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// property will be set to be the same as its new parent's layer.
		/// Unlike <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// this method will be called even when this object is not part of a document,
		/// or when <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> is true.
		/// However, this method is not called upon undo or redo of a change of parent.
		/// By default this method does nothing.
		/// Any implementation of this method should not cause a change in parents
		/// or layers.
		/// </remarks>
		protected virtual void OnParentChanged(GoGroup oldgroup, GoGroup newgroup)
		{
		}

		/// <summary>
		/// Called after this object's bounds has changed.
		/// </summary>
		/// <param name="old">
		/// A <c>RectangleF</c> in document coordinates holding the previous bounds.
		/// </param>
		/// <remarks>
		/// By default this method does nothing.
		/// This method is called as part of the <see cref="P:Northwoods.Go.GoObject.Bounds" /> setter, after
		/// the property value has been saved.
		/// Unlike <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// this method will be called even when this object is not part of a document,
		/// or when <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> is true.
		/// However, this method is not called when the bounds are changed due to a call
		/// to <see cref="M:Northwoods.Go.GoObject.ComputeBounds" />, nor upon an undo or a redo of a change
		/// in bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.OnBoundsChanged(System.Drawing.RectangleF)" />
		/// <seealso cref="M:Northwoods.Go.GoGroup.OnChildBoundsChanged(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />
		protected virtual void OnBoundsChanged(RectangleF old)
		{
		}

		/// <summary>
		/// Called when an observed object has been changed.
		/// </summary>
		/// <param name="observed">
		/// The modified object being observed by this object.
		/// </param>
		/// <param name="subhint">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="oldI">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="oldVal">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="oldRect">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="newI">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="newVal">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <param name="newRect">The same as for the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method.</param>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> calls this method on each of the
		/// observing objects that were registered with the observed object
		/// by a call to <see cref="M:Northwoods.Go.GoObject.AddObserver(Northwoods.Go.GoObject)" />.
		/// If the observed's <see cref="P:Northwoods.Go.GoObject.SuspendsUpdates" /> property is true,
		/// this method will not be called on its observers.
		/// Unlike <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// this method will be called even when the observed object is not part of a document.
		/// By default this method does nothing.
		/// Any implementation of this method should be very careful about causing any
		/// changes to the observed object.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoGroup.OnChildBoundsChanged(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />
		protected virtual void OnObservedChanged(GoObject observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
		}

		/// <summary>
		/// Register an object as an observer of changes to this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// <para>
		/// Adding an observer object means that that observer's <see cref="M:Northwoods.Go.GoObject.OnObservedChanged(Northwoods.Go.GoObject,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// method will be called whenever this object is changed.
		/// If the <paramref name="obj" /> is already an observer for this object,
		/// this call does nothing.
		/// <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> will not copy the list of observers for this object;
		/// the caller of <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> is responsible for doing so, just as with
		/// adding the copied object to a layer and/or group to update the <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// and <see cref="P:Northwoods.Go.GoObject.Parent" /> properties.
		/// </para>
		/// <para>
		/// Use of the observer mechanism is less efficient in both space and time
		/// than overriding the <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> method of the observed object's class.
		/// However it can be useful when it is inconvenient to implement such an override.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.RemoveObserver(Northwoods.Go.GoObject)" />
		public virtual void AddObserver(GoObject obj)
		{
			if (obj != null)
			{
				if (myObservers == null)
				{
					myObservers = new GoCollection();
				}
				if (!myObservers.Contains(obj))
				{
					myObservers.Add(obj);
					Changed(1014, 0, null, NullRect, 0, obj, NullRect);
				}
			}
		}

		/// <summary>
		/// Make sure an object is not an observer of changes to this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <seealso cref="M:Northwoods.Go.GoObject.AddObserver(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnObservedChanged(Northwoods.Go.GoObject,System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		public virtual void RemoveObserver(GoObject obj)
		{
			if (obj != null && myObservers != null && myObservers.Contains(obj))
			{
				myObservers.Remove(obj);
				Changed(1015, 0, obj, NullRect, 0, null, NullRect);
			}
		}

		/// <summary>
		/// Render this object to a Graphics object.
		/// </summary>
		/// <param name="g">
		/// The <c>Graphics</c> object with which to draw.
		/// </param>
		/// <param name="view">
		/// The <see cref="T:Northwoods.Go.GoView" /> provides additional context for this painting
		/// operation.
		/// </param>
		/// <remarks>
		/// <para>
		/// This method may get called frequently and should not modify any object or view state.
		/// </para>
		/// <para>
		/// The default implementation paints nothing.
		/// Each derived class should override this method.
		/// </para>
		/// <para>
		/// The <paramref name="view" /> parameter is useful for customizing
		/// the drawing behavior based on the kind of view to which we are drawing.
		/// For example, the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.ShadowOffset" /> property specifies 
		/// the size of the drop shadow painted by objects whose <see cref="P:Northwoods.Go.GoObject.Shadowed" />
		/// property is true.
		/// </para>
		/// <para>
		/// If you override this method, you may also need to override <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />,
		/// and perhaps the other geometry methods, including <see cref="M:Northwoods.Go.GoObject.ComputeBounds" />,
		/// <see cref="M:Northwoods.Go.GoObject.ContainsPoint(System.Drawing.PointF)" />, <see cref="M:Northwoods.Go.GoObject.ContainedByRectangle(System.Drawing.RectangleF)" />,
		/// <see cref="M:Northwoods.Go.GoObject.GetNearestIntersectionPoint(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF@)" />,
		/// and other methods such as <see cref="M:Northwoods.Go.GoObject.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> and <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />.
		/// </para>
		/// <para>
		/// Furthermore, in the implementation of a property setter that causes the paint bounds
		/// to change, you will need to call <see cref="M:Northwoods.Go.GoObject.InvalidateViews" /> before actually
		/// changing your object's state, so that views will get a chance to notice the
		/// paint bounds before it changes.
		/// </para>
		/// </remarks>
		public virtual void Paint(Graphics g, GoView view)
		{
		}

		/// <summary>
		/// Expand a bounding rectangle to better represent where
		/// this object is painted.
		/// </summary>
		/// <param name="rect">
		/// A <c>RectangleF</c> in document coordinates.
		/// </param>
		/// <param name="view">
		/// The view in which the object is being painted.
		/// This may be null, if the particular view is not known.
		/// </param>
		/// <returns>
		/// A <c>RectangleF</c> in document coordinates that may be slightly
		/// larger than the <paramref name="rect" /> argument, to account for
		/// where this object may be painted.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The <see cref="P:Northwoods.Go.GoObject.Bounds" /> property provides the abstract position and
		/// size of an object.  However, the actual painted area is often somewhat
		/// larger, because of the thickness of a <c>Pen</c> or because of a
		/// shadow.
		/// </para>
		/// <para>
		/// The default behavior of this method is just to return the
		/// <paramref name="rect" /> value.
		/// </para>
		/// </remarks>
		public virtual RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			return rect;
		}

		internal bool IsApprox(float x, float y)
		{
			float num = 0.5f;
			GoDocument document = Document;
			if (document != null)
			{
				num = document.WorldEpsilon;
			}
			float num2 = x - y;
			if (num2 < num)
			{
				return num2 > 0f - num;
			}
			return false;
		}

		internal bool IsApprox(double x, double y)
		{
			double num = 0.5;
			GoDocument document = Document;
			if (document != null)
			{
				num = document.WorldEpsilon;
			}
			double num2 = x - y;
			if (num2 < num)
			{
				return num2 > 0.0 - num;
			}
			return false;
		}

		/// <summary>
		/// Determine if a given point is inside and on this object.
		/// </summary>
		/// <param name="p">
		/// A <c>PointF</c> in document coordinates.
		/// </param>
		/// <returns>
		/// True if the argument <paramref name="p" /> is considered to be "in"
		/// this object.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method tries to return true for points near a stroke or near or
		/// inside a possibly filled object such as an ellipse or a polygon.
		/// This method ignores any drop shadow, but normally includes the
		/// width of any <c>Pen</c>.
		/// </para>
		/// <para>
		/// The default behavior of this method is to return true if the
		/// point <paramref name="p" /> is within this object's <see cref="P:Northwoods.Go.GoObject.Bounds" />.
		/// If this object's <see cref="P:Northwoods.Go.GoObject.Width" /> or <see cref="P:Northwoods.Go.GoObject.Height" /> are zero,
		/// the given point <paramref name="p" /> has to be exactly on for this
		/// predicate to return true.
		/// However, some classes, such as <see cref="T:Northwoods.Go.GoStroke" /> that is inherently
		/// somewhat "one dimensional", may intentionally be more forgiving by
		/// supporting some margin nearby where a point can be considered to be
		/// "inside" the object.
		/// </para>
		/// </remarks>
		public virtual bool ContainsPoint(PointF p)
		{
			return ContainsRect(Bounds, p);
		}

		/// <summary>
		/// Determine if a given rectangle completely encloses this object.
		/// </summary>
		/// <param name="r">
		/// a <c>RectangleF</c> in document coordinates.
		/// </param>
		/// <returns>
		/// True if this object is considered to be "inside" the rectangle <paramref name="r" />.
		/// </returns>
		/// <remarks>
		/// The default behavior is to see if this object's <see cref="P:Northwoods.Go.GoObject.Bounds" /> are within
		/// the rectangle <paramref name="r" />.
		/// A zero width and/or height for this object is acceptable; but if either
		/// <c>r.Width</c> or <c>r.Height</c> are zero or negative, this will return false.
		/// </remarks>
		public virtual bool ContainedByRectangle(RectangleF r)
		{
			return ContainsRect(r, Bounds);
		}

		/// <summary>
		/// Find the closest point in this object to a given point that is on a line from that point.
		/// </summary>
		/// <param name="p1">
		/// the point we are looking to be closest to, on the line formed with <paramref name="p2" />
		/// </param>
		/// <param name="p2">
		/// forms a line with <paramref name="p1" />
		/// </param>
		/// <param name="result">
		/// the point of this object that is closest to <paramref name="p1" /> and that is on
		/// the infinite line from <paramref name="p1" /> to <paramref name="p2" />
		/// </param>
		/// <returns>
		/// true if the infinite line does intersect with this object; false otherwise
		/// </returns>
		public virtual bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			return GetNearestIntersectionPoint(Bounds, p1, p2, out result);
		}

		/// <summary>
		/// Find the closest point of a rectangle to a given point that is on a line from that point.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="p1">
		/// the point we are looking to be closest to, on the line formed with <paramref name="p2" />
		/// </param>
		/// <param name="p2">
		/// forms a line with <paramref name="p1" />
		/// </param>
		/// <param name="result">
		/// the point of this object that is closest to <paramref name="p1" /> and that is on
		/// the infinite line from <paramref name="p1" /> to <paramref name="p2" />
		/// </param>
		/// <returns>
		/// true if the infinite line does intersect with this object; false otherwise
		/// </returns>
		public static bool GetNearestIntersectionPoint(RectangleF rect, PointF p1, PointF p2, out PointF result)
		{
			PointF pointF = new PointF(rect.X, rect.Y);
			PointF pointF2 = new PointF(rect.X + rect.Width, rect.Y);
			PointF pointF3 = new PointF(rect.X, rect.Y + rect.Height);
			PointF pointF4 = new PointF(rect.X + rect.Width, rect.Y + rect.Height);
			float x = p1.X;
			float y = p1.Y;
			float num = 1E+21f;
			PointF pointF5 = default(PointF);
			if (GoStroke.NearestIntersectionOnLine(pointF, pointF2, p1, p2, out PointF result2))
			{
				float num2 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num2 < num)
				{
					num = num2;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF2, pointF4, p1, p2, out result2))
			{
				float num3 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num3 < num)
				{
					num = num3;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF4, pointF3, p1, p2, out result2))
			{
				float num4 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num4 < num)
				{
					num = num4;
					pointF5 = result2;
				}
			}
			if (GoStroke.NearestIntersectionOnLine(pointF3, pointF, p1, p2, out result2))
			{
				float num5 = (result2.X - x) * (result2.X - x) + (result2.Y - y) * (result2.Y - y);
				if (num5 < num)
				{
					num = num5;
					pointF5 = result2;
				}
			}
			result = pointF5;
			return num < 1E+21f;
		}

		internal static bool IntersectsLineSegment(RectangleF rect, PointF p1, PointF p2)
		{
			if (p1.X == p2.X)
			{
				if (rect.Left <= p1.X && p1.X <= rect.Right && Math.Min(p1.Y, p2.Y) <= rect.Bottom)
				{
					return Math.Max(p1.Y, p2.Y) >= rect.Top;
				}
				return false;
			}
			if (p1.Y == p2.Y)
			{
				if (rect.Top <= p1.Y && p1.Y <= rect.Bottom && Math.Min(p1.X, p2.X) <= rect.Right)
				{
					return Math.Max(p1.X, p2.X) >= rect.Left;
				}
				return false;
			}
			if (ContainsRect(rect, p1))
			{
				return true;
			}
			if (ContainsRect(rect, p2))
			{
				return true;
			}
			if (GoStroke.IntersectingLines(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top), p1, p2))
			{
				return true;
			}
			if (GoStroke.IntersectingLines(new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom), p1, p2))
			{
				return true;
			}
			if (GoStroke.IntersectingLines(new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom), p1, p2))
			{
				return true;
			}
			if (GoStroke.IntersectingLines(new PointF(rect.Left, rect.Bottom), new PointF(rect.Left, rect.Top), p1, p2))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Return an object at a point, perhaps only a selectable one.
		/// </summary>
		/// <param name="p">
		/// A <c>PointF</c> in document coordinates.
		/// </param>
		/// <param name="selectableOnly">
		/// Whether the return value must be selectable by the user.
		/// </param>
		/// <returns>
		/// An object under the point <paramref name="p" />.
		/// If <paramref name="selectableOnly" /> is true, the object
		/// returned will have its <see cref="M:Northwoods.Go.GoObject.CanSelect" /> property be true.
		/// This method returns null if no suitable object is found.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This will return null if this object is not visible or if
		/// the <paramref name="p" /> is not in this object.
		/// </para>
		/// <para>
		/// If <paramref name="selectableOnly" /> is false, it will return
		/// this object; if that parameter is true, it will return this
		/// object only if <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.
		/// </para>
		/// <para>
		/// Finally, if <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is false, and this object
		/// is part of a <see cref="T:Northwoods.Go.GoGroup" />, it proceeds up the chain
		/// of <see cref="P:Northwoods.Go.GoObject.Parent" /> objects until it finds one whose
		/// <see cref="M:Northwoods.Go.GoObject.CanSelect" /> property is true, and returns that.
		/// </para>
		/// <para>
		/// Failing all of those tests, this method will return null.
		/// </para>
		/// </remarks>
		public virtual GoObject Pick(PointF p, bool selectableOnly)
		{
			if (!CanView())
			{
				return null;
			}
			if (!ContainsPoint(p))
			{
				return null;
			}
			if (!selectableOnly)
			{
				return this;
			}
			if (CanSelect())
			{
				return this;
			}
			for (GoObject parent = Parent; parent != null; parent = parent.Parent)
			{
				if (parent.CanSelect())
				{
					return parent;
				}
			}
			return null;
		}

		/// <summary>
		/// Called when the user single clicks on this object.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this
		/// object handled the event and thus that the calling view
		/// need not continue calling the method up the chain of parents.
		/// </returns>
		/// <remarks>
		/// By default this method does nothing but return false.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoSingleClick(Northwoods.Go.GoInputEventArgs)" /> is the normal caller,
		/// which in turn is called by various tools.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnDoubleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		public virtual bool OnSingleClick(GoInputEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when the user double clicks on this object.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this
		/// object handled the event and thus that the calling view
		/// need not continue calling the method up the chain of parents.
		/// </returns>
		/// <remarks>
		/// By default this method does nothing but return false.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoDoubleClick(Northwoods.Go.GoInputEventArgs)" /> is the normal caller,
		/// which in turn is called by various tools.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		public virtual bool OnDoubleClick(GoInputEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when the user context clicks on this object.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this
		/// object handled the event and thus that the calling view
		/// need not continue calling the method up the chain of parents.
		/// </returns>
		/// <remarks>
		/// By default this method does nothing but return false.
		/// The context menu click is normally a right mouse click.
		/// This method is normally invoked by <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoContextClick(Northwoods.Go.GoInputEventArgs)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnDoubleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		public virtual bool OnContextClick(GoInputEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when the user hovers over this object.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this object handled the event and thus
		/// that the calling view need not continue calling the method
		/// up the chain of parent objects.
		/// </returns>
		/// <remarks>
		/// By default this method does nothing but return false.
		/// This method is normally invoked by the <see cref="T:Northwoods.Go.GoToolManager" /> tool,
		/// through the <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoHover(Northwoods.Go.GoInputEventArgs)" /> method.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoView)" />
		public virtual bool OnHover(GoInputEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when the document object immediately under the mouse changes
		/// as the mouse moves when no particular tool is running or when the
		/// <see cref="T:Northwoods.Go.GoToolDragging" /> tool is running.
		/// </summary>
		/// <param name="from">the object that had been under the mouse, or null if the mouse was not over any document object</param>
		/// <param name="to">the object that is now under the mouse, or null if the mouse is now in the view's background</param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this object handled the event and thus
		/// that the calling view need not continue calling the method
		/// up the chain of parent objects.
		/// </returns>
		/// <remarks>
		/// <para>
		/// By default this method does nothing but return false.
		/// This method is normally invoked by the <see cref="T:Northwoods.Go.GoToolManager" /> tool and
		/// the <see cref="T:Northwoods.Go.GoToolDragging" /> tool,
		/// through the <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" /> method.
		/// Thus the <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" /> event is not raised during
		/// a resize or a linking operation or the operation of other tools.
		/// This method is also not called directly on external drag-and-drop operations from
		/// other Controls.  However, you may be able to get called in such situations
		/// if <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> is true,
		/// because then a drag enter causes a drop which adds objects to the document
		/// that are dragged around by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnHover(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		public virtual bool OnEnterLeave(GoObject from, GoObject to, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when the user moves the mouse over this object when not
		/// dragging or resizing.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this
		/// object handled the event and thus that the calling view
		/// need not continue calling the method up the chain of parents.
		/// </returns>
		/// <remarks>
		/// <para>
		/// By default this method just calls <see cref="M:Northwoods.Go.GoObject.GetCursorName(Northwoods.Go.GoView)" />;
		/// if the return value is a string, it sets the <paramref name="view" />'s
		/// <see cref="P:Northwoods.Go.GoView.CursorName" /> property to that value;
		/// otherwise this method returns false.
		/// Your override of this method can decide to set the <c>Control.Cursor</c>
		/// property directly, if you do not want to use the <see cref="M:Northwoods.Go.GoObject.GetCursorName(Northwoods.Go.GoView)" />
		/// method.
		/// The view's <see cref="M:Northwoods.Go.GoView.DoBackgroundMouseOver(Northwoods.Go.GoInputEventArgs)" /> method
		/// is responsible for restoring the default cursor for the view when
		/// the pointer is no longer over an object that specifies a cursor.
		/// The view method <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" /> will call
		/// <see cref="M:Northwoods.Go.GoView.DoBackgroundMouseOver(Northwoods.Go.GoInputEventArgs)" />
		/// when all the calls to this object method <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// return false.
		/// </para>
		/// <para>
		/// Normally this method is called when no specific tool is in control
		/// for the <paramref name="view" />, i.e., when the
		/// <see cref="T:Northwoods.Go.GoToolManager" /> tool is the view's current tool.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" /> is the normal caller.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnHover(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.OnEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoView)" />
		public virtual bool OnMouseOver(GoInputEventArgs evt, GoView view)
		{
			string cursorName = GetCursorName(view);
			if (cursorName != null)
			{
				view.CursorName = cursorName;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called by <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> to determine the cursor, if any,
		/// to display over this object.
		/// </summary>
		/// <param name="view">may be null</param>
		/// <returns>By default this returns null: just show the view's default cursor.</returns>
		/// <seealso cref="M:Northwoods.Go.GoView.StandardizeCursorName(System.String)" />
		public virtual string GetCursorName(GoView view)
		{
			return null;
		}

		/// <summary>
		/// Called to get a String to display as a tooltip for this object.
		/// </summary>
		/// <param name="view">may be null</param>
		/// <returns>
		/// A <c>String</c>, or null to indicate no tooltip for this object.
		/// </returns>
		/// <remarks>
		/// By default this method does nothing but return null.
		/// A non-null <c>String</c> indicates this
		/// object handled the event and thus that the calling view
		/// need not continue calling the method up the chain of parents.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoToolTipObject(Northwoods.Go.GoObject)" /> is the normal caller.
		/// </remarks>
		public virtual string GetToolTip(GoView view)
		{
			return null;
		}

		/// <summary>
		/// Called to get a <see cref="T:Northwoods.Go.GoContextMenu" /> to display for this object.
		/// </summary>
		/// <param name="view">may be null</param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoContextMenu" />, or null to indicate no context menu for this object.
		/// </returns>
		/// <remarks>
		/// By default this just returns null.
		/// A non-null value indicates this object has handled the standard context click
		/// event and thus that the calling view need not continue calling this method
		/// up the chain of parents.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoContextClick(Northwoods.Go.GoInputEventArgs)" /> is the normal caller.
		/// </remarks>
		public virtual GoContextMenu GetContextMenu(GoView view)
		{
			return null;
		}

		/// <summary>
		/// Called to get a <b>ContextMenuStrip</b> to display for this object.
		/// </summary>
		/// <param name="view">may be null</param>
		/// <returns>
		/// a <b>System.Windows.Forms.ContextMenuStrip</b>, or null to indicate no context menu for this object.
		/// </returns>
		/// <remarks>
		/// By default this just returns null.
		/// A non-null value indicates this object has handled the standard context click
		/// event and thus that the calling view need not continue calling this method
		/// up the chain of parents.
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoContextClick(Northwoods.Go.GoInputEventArgs)" /> is the normal caller.
		/// </remarks>
		public virtual ContextMenuStrip GetContextMenuStrip(GoView view)
		{
			return null;
		}

		/// <summary>
		/// Called during a user's drag of the view's Selection when the mouse is
		/// over this object, to give this object a chance to veto a drop.
		/// </summary>
		/// <param name="evt">
		/// a <see cref="T:Northwoods.Go.GoObjectEventArgs" /> whose
		/// <see cref="T:Northwoods.Go.GoObjectEventArgs" />.<see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" /> property
		/// refers to the object at which the mouse might drop the selection;
		/// setting the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> property to
		/// <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" /> will
		/// also reject a drop, just as returning true does
		/// </param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate a drop of the view's Selection should not occur on this object.
		/// False to indicate that it might be OK, and to continue calling this method up
		/// the chain of parent objects to see if any parent wants to cancel the drop.
		/// </returns>
		/// <remarks>
		/// By default this does nothing but return false.
		/// This method is normally invoked by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool,
		/// through the <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoSelectionDropReject(Northwoods.Go.GoInputEventArgs)" /> method.
		/// </remarks>
		public virtual bool OnSelectionDropReject(GoObjectEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called after the user has dropped the selection on this object.
		/// </summary>
		/// <param name="evt">
		/// a <see cref="T:Northwoods.Go.GoObjectEventArgs" /> whose
		/// <see cref="T:Northwoods.Go.GoObjectEventArgs" />.<see cref="P:Northwoods.Go.GoObjectEventArgs.GoObject" /> property
		/// refers to the object at which the mouse dropped the selection
		/// </param>
		/// <param name="view"></param>
		/// <returns>
		/// True to indicate this object handled the event and thus
		/// that the calling view need not continue calling the method
		/// up the chain of parent objects.
		/// </returns>
		/// <remarks>
		/// By default this does nothing but return false.
		/// This method is normally invoked by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool,
		/// through the <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoSelectionDropped(Northwoods.Go.GoInputEventArgs)" /> method.
		/// One cannot cancel a drop at this time -- it has already happened.
		/// </remarks>
		public virtual bool OnSelectionDropped(GoObjectEventArgs evt, GoView view)
		{
			return false;
		}

		/// <summary>
		/// Called when this object becomes selected.
		/// </summary>
		/// <param name="sel">
		/// The <see cref="T:Northwoods.Go.GoSelection" /> of the <see cref="T:Northwoods.Go.GoView" /> in which
		/// this object has been selected.
		/// </param>
		/// <remarks>
		/// By default this method calls <see cref="M:Northwoods.Go.GoObject.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" /> on this
		/// object's <see cref="P:Northwoods.Go.GoObject.SelectionObject" />.
		/// Only document objects get selection handles--if this object does not belong
		/// to a document (for example by being a view object), no selection handles
		/// are created.
		/// Any implementation of this method should not change which objects are selected.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnLostSelection(Northwoods.Go.GoSelection)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		public virtual void OnGotSelection(GoSelection sel)
		{
			if (IsInDocument && CanView())
			{
				SelectionObject?.AddSelectionHandles(sel, this);
			}
		}

		/// <summary>
		/// Called when this object becomes unselected.
		/// </summary>
		/// <param name="sel">
		/// The <see cref="T:Northwoods.Go.GoSelection" /> of the <see cref="T:Northwoods.Go.GoView" /> in which
		/// this object had been selected.
		/// </param>
		/// <remarks>
		/// This removes any selection handles from this object's <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// by calling <see cref="M:Northwoods.Go.GoObject.RemoveSelectionHandles(Northwoods.Go.GoSelection)" /> on it.
		/// Any implementation of this method should not change which objects are selected.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.OnGotSelection(Northwoods.Go.GoSelection)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.RemoveSelectionHandles(Northwoods.Go.GoSelection)" />
		public virtual void OnLostSelection(GoSelection sel)
		{
			SelectionObject?.RemoveSelectionHandles(sel);
		}

		/// <summary>
		/// Add selection handles for this object for the given selection collection.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// This typically calls <see cref="T:Northwoods.Go.GoSelection" />.<see cref="M:Northwoods.Go.GoSelection.CreateBoundingHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" /> or
		/// <see cref="M:Northwoods.Go.GoSelection.CreateResizeHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Drawing.PointF,System.Int32,System.Boolean)" /> as needed.
		/// The standard implementation first calls <see cref="M:Northwoods.Go.GoObject.RemoveSelectionHandles(Northwoods.Go.GoSelection)" />
		/// to make sure no obsolete handles remain for this object.
		/// If <see cref="M:Northwoods.Go.GoObject.CanResize" /> is false, it just creates a bounding handle.
		/// Otherwise if <see cref="M:Northwoods.Go.GoObject.CanReshape" /> is true, it creates eight resize
		/// handles at the eight standard spots along the object's bounds.
		/// If <see cref="M:Northwoods.Go.GoObject.CanReshape" /> is false, it only creates the four resize
		/// handles at the object's corners.
		/// </remarks>
		public virtual void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			RemoveSelectionHandles(sel);
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			if (!(CanResize() && flag))
			{
				sel.CreateBoundingHandle(this, selectedObj);
				return;
			}
			RectangleF bounds = Bounds;
			float x = bounds.X;
			float x2 = bounds.X + bounds.Width / 2f;
			float x3 = bounds.X + bounds.Width;
			float y = bounds.Y;
			float y2 = bounds.Y + bounds.Height / 2f;
			float y3 = bounds.Y + bounds.Height;
			sel.CreateResizeHandle(this, selectedObj, new PointF(x, y), 2, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y), 4, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y3), 8, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x, y3), 16, filled: true);
			if (CanReshape() && flag2)
			{
				sel.CreateResizeHandle(this, selectedObj, new PointF(x2, y), 32, filled: true);
				sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y2), 64, filled: true);
				sel.CreateResizeHandle(this, selectedObj, new PointF(x2, y3), 128, filled: true);
				sel.CreateResizeHandle(this, selectedObj, new PointF(x, y2), 256, filled: true);
			}
		}

		/// <summary>
		/// Create a bounding handle for this object.
		/// </summary>
		/// <returns>
		/// This must return a <see cref="T:Northwoods.Go.GoObject" /> that implements <see cref="T:Northwoods.Go.IGoHandle" />
		/// and is an appropriate size and position for surrounding the object.
		/// </returns>
		/// <remarks>
		/// By default this allocates a new <see cref="T:Northwoods.Go.GoHandle" />.
		/// <see cref="T:Northwoods.Go.GoSelection" />.<see cref="M:Northwoods.Go.GoSelection.CreateBoundingHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" /> will call this method to
		/// allocate a handle and size and position it.
		/// <see cref="M:Northwoods.Go.GoSelection.CreateBoundingHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject)" /> is then responsible for
		/// specifying its properties, including its <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.Pen" /> and
		/// <see cref="P:Northwoods.Go.GoShape.Brush" /> if the handle is a <see cref="T:Northwoods.Go.GoShape" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CreateResizeHandle(System.Int32)" />
		public virtual IGoHandle CreateBoundingHandle()
		{
			GoHandle goHandle = new GoHandle();
			RectangleF a = Bounds;
			SizeF sizeF = new SizeF(1f, 1f);
			GoDocument document = Document;
			if (document != null)
			{
				SizeF worldScale = document.WorldScale;
				sizeF.Width /= worldScale.Width;
				sizeF.Height /= worldScale.Height;
			}
			InflateRect(ref a, sizeF.Width, sizeF.Height);
			goHandle.Bounds = a;
			return goHandle;
		}

		/// <summary>
		/// Create a resize handle for this object, given a handle ID.
		/// </summary>
		/// <param name="handleid"></param>
		/// <returns>
		/// This must return a <see cref="T:Northwoods.Go.GoObject" /> that implements <see cref="T:Northwoods.Go.IGoHandle" />.
		/// </returns>
		/// <remarks>
		/// By default this allocates a new <see cref="T:Northwoods.Go.GoHandle" />.
		/// <see cref="T:Northwoods.Go.GoSelection" />.<see cref="M:Northwoods.Go.GoSelection.CreateResizeHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Drawing.PointF,System.Int32,System.Boolean)" /> will call this method to
		/// allocate a handle which it will position.
		/// <see cref="M:Northwoods.Go.GoSelection.CreateResizeHandle(Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Drawing.PointF,System.Int32,System.Boolean)" /> also is responsible
		/// for specifying its other <see cref="T:Northwoods.Go.GoObject" /> properties,
		/// including its <see cref="T:Northwoods.Go.GoShape" />.<see cref="P:Northwoods.Go.GoShape.Pen" /> and <see cref="P:Northwoods.Go.GoShape.Brush" /> if
		/// the handle is a <see cref="T:Northwoods.Go.GoShape" />.
		/// You may find it convenient to adjust the shape (<see cref="T:Northwoods.Go.GoHandle" />.<see cref="P:Northwoods.Go.GoHandle.Style" />) or size
		/// of a particular handle by overriding this method.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CreateBoundingHandle" />
		public virtual IGoHandle CreateResizeHandle(int handleid)
		{
			return new GoHandle();
		}

		internal void MakeDiamondResizeHandle(IGoHandle handle, int spot)
		{
			GoHandle goHandle = handle.GoObject as GoHandle;
			if (goHandle != null)
			{
				goHandle.Style = GoHandleStyle.Diamond;
				if (!(goHandle.SelectedObject is IGoLink))
				{
					goHandle.Brush = GoShape.Brushes_Yellow;
				}
				RectangleF a = goHandle.Bounds;
				InflateRect(ref a, a.Width / 6f, a.Height / 6f);
				goHandle.Bounds = a;
				goHandle.CursorName = GoHandle.GetCursorNameForHandleID(spot);
			}
		}

		/// <summary>
		/// Remove all selection handles for this object for the given selection collection.
		/// </summary>
		/// <param name="sel"></param>
		/// <remarks>
		/// By default this just calls <see cref="T:Northwoods.Go.GoSelection" />.<see cref="M:Northwoods.Go.GoSelection.RemoveHandles(Northwoods.Go.GoObject)" />.
		/// </remarks>
		public virtual void RemoveSelectionHandles(GoSelection sel)
		{
			sel.RemoveHandles(this);
		}

		/// <summary>
		/// Called when a user moves this object.
		/// </summary>
		/// <param name="view">the <see cref="T:Northwoods.Go.GoView" /> whose <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> or
		/// <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> or similar method is calling this method</param>
		/// <param name="origLoc">the original <see cref="P:Northwoods.Go.GoObject.Location" /></param>
		/// <param name="newLoc">the new <see cref="P:Northwoods.Go.GoObject.Location" /></param>
		/// <remarks>
		/// This is normally called from <see cref="T:Northwoods.Go.GoView" /> methods <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />,
		/// and <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// By default it just sets this object's <see cref="P:Northwoods.Go.GoObject.Location" /> property
		/// to the result of a call to <see cref="M:Northwoods.Go.GoObject.ComputeMove(System.Drawing.PointF,System.Drawing.PointF)" />:
		/// <c>Location = ComputeMove(origLoc, newLoc)</c>
		/// However, you can override this method or <see cref="M:Northwoods.Go.GoObject.ComputeMove(System.Drawing.PointF,System.Drawing.PointF)" />
		/// to constrain the places that this object can be moved to by the user.
		/// You will still be able to move this object programmatically by
		/// setting the <see cref="P:Northwoods.Go.GoObject.Location" /> or <see cref="P:Northwoods.Go.GoObject.Position" /> property
		/// directly.
		/// If you want to constrain this object's movement both interactively and
		/// programmatically, you should override the <see cref="P:Northwoods.Go.GoObject.Bounds" /> property.
		/// Override <see cref="M:Northwoods.Go.GoObject.ComputeMove(System.Drawing.PointF,System.Drawing.PointF)" /> if you want to constrain the user's
		/// movement of this object without regard to the actual view or input events
		/// causing the move.
		/// </remarks>
		public virtual void DoMove(GoView view, PointF origLoc, PointF newLoc)
		{
			PointF pointF2 = Location = ComputeMove(origLoc, newLoc);
		}

		/// <summary>
		/// Calculate a new location for this object.
		/// </summary>
		/// <param name="origLoc"></param>
		/// <param name="newLoc"></param>
		/// <returns>
		/// A <c>PointF</c> in document coordinates.
		/// </returns>
		/// <remarks>
		/// This is normally called from <see cref="M:Northwoods.Go.GoObject.DoMove(Northwoods.Go.GoView,System.Drawing.PointF,System.Drawing.PointF)" /> and <see cref="T:Northwoods.Go.GoDocument" />'s
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />.
		/// </remarks>
		public virtual PointF ComputeMove(PointF origLoc, PointF newLoc)
		{
			return newLoc;
		}

		/// <summary>
		/// Called when a user resizes this object.
		/// </summary>
		/// <param name="view">
		/// the <see cref="T:Northwoods.Go.GoView" /> whose <see cref="T:Northwoods.Go.GoToolResizing" /> is calling this method
		/// </param>
		/// <param name="origRect">
		/// the original bounding rectangle
		/// </param>
		/// <param name="newPoint">
		/// the PointF, in document coordinates, to which the resize handle is being dragged
		/// </param>
		/// <param name="whichHandle">
		/// The <see cref="T:Northwoods.Go.IGoHandle" />.<see cref="P:Northwoods.Go.IGoHandle.HandleID" /> of the handle being dragged
		/// </param>
		/// <param name="evttype">
		/// <list type="bullet">
		/// <item><term><c>GoInputState.Start</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.Start" /></description></item>
		/// <item><term><c>GoInputState.Continue</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseMove" /></description></item>
		/// <item><term><c>GoInputState.Finish</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseUp" /></description></item>
		/// <item><term><c>GoInputState.Cancel</c></term><description>when the <see cref="M:Northwoods.Go.GoToolResizing.DoCancelMouse" /></description></item>
		/// </list>
		/// </param>
		/// <param name="min">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MinimumSize" />
		/// </param>
		/// <param name="max">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MaximumSize" />
		/// </param>
		/// <remarks>
		/// This is normally called from <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// By default it just calls <see cref="M:Northwoods.Go.GoObject.ComputeResize(System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,System.Drawing.SizeF,System.Drawing.SizeF,System.Boolean)" />, allowing
		/// reshaping if this object's <see cref="M:Northwoods.Go.GoObject.CanReshape" /> property is true
		/// and the <paramref name="view" />'s last input's <see cref="T:Northwoods.Go.GoInputEventArgs" />.<see cref="P:Northwoods.Go.GoInputEventArgs.Shift" />
		/// property is true.
		/// The resulting <c>RectangleF</c> value is used as this object's new
		/// <see cref="P:Northwoods.Go.GoObject.Bounds" /> if <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> is true or if
		/// <paramref name="evttype" /> is <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Finish" />.
		/// Override <see cref="M:Northwoods.Go.GoObject.ComputeResize(System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,System.Drawing.SizeF,System.Drawing.SizeF,System.Boolean)" /> if you want to constrain the user's
		/// resizing of this object without regard to the actual view or input events
		/// causing the resize.
		/// </remarks>
		public virtual void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (evttype == GoInputState.Cancel)
			{
				Bounds = origRect;
				return;
			}
			RectangleF rectangleF = ComputeResize(origRect, newPoint, whichHandle, min, max, CanReshape() && view.CanReshapeObjects() && !view.LastInput.Shift);
			if (ResizesRealtime)
			{
				Bounds = rectangleF;
				return;
			}
			Rectangle rect = view.ConvertDocToView(rectangleF);
			view.DrawXorBox(rect, evttype != GoInputState.Finish);
			if (evttype == GoInputState.Finish)
			{
				Bounds = rectangleF;
			}
		}

		/// <summary>
		/// Calculate a new bounding rectangle for this object.
		/// </summary>
		/// <param name="origRect">the original bounding rectangle</param>
		/// <param name="newPoint">the PointF, in document coordinates, to which to resize</param>
		/// <param name="handle">the handle ID that is being resized</param>
		/// <param name="min">the minimum size allowed</param>
		/// <param name="max">the maximum size allowed</param>
		/// <param name="reshape">whether or not to allow reshaping of the object</param>
		/// <returns>
		/// A new <c>RectangleF</c> bounding rectangle, in document coordinates.
		/// </returns>
		/// <remarks>
		/// This is normally called from <see cref="M:Northwoods.Go.GoObject.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" />.
		/// </remarks>
		public virtual RectangleF ComputeResize(RectangleF origRect, PointF newPoint, int handle, SizeF min, SizeF max, bool reshape)
		{
			float x = origRect.X;
			float y = origRect.Y;
			float num = origRect.X + origRect.Width;
			float num2 = origRect.Y + origRect.Height;
			float num3 = 1f;
			if (!reshape)
			{
				float num4 = origRect.Width;
				float num5 = origRect.Height;
				if (num4 <= 0f)
				{
					num4 = 1f;
				}
				if (num5 <= 0f)
				{
					num5 = 1f;
				}
				num3 = num5 / num4;
			}
			RectangleF result = origRect;
			switch (handle)
			{
			case 2:
				result.X = Math.Max(newPoint.X, num - max.Width);
				result.X = Math.Min(result.X, num - min.Width);
				result.Width = num - result.X;
				if (result.Width <= 0f)
				{
					result.Width = 1f;
				}
				result.Y = Math.Max(newPoint.Y, num2 - max.Height);
				result.Y = Math.Min(result.Y, num2 - min.Height);
				result.Height = num2 - result.Y;
				if (result.Height <= 0f)
				{
					result.Height = 1f;
				}
				if (!reshape)
				{
					float num7 = result.Height / result.Width;
					if (num3 < num7)
					{
						result.Height = num3 * result.Width;
						result.Y = num2 - result.Height;
					}
					else
					{
						result.Width = result.Height / num3;
						result.X = num - result.Width;
					}
				}
				break;
			case 4:
				result.Width = Math.Min(newPoint.X - x, max.Width);
				result.Width = Math.Max(result.Width, min.Width);
				result.Y = Math.Max(newPoint.Y, num2 - max.Height);
				result.Y = Math.Min(result.Y, num2 - min.Height);
				result.Height = num2 - result.Y;
				if (result.Height <= 0f)
				{
					result.Height = 1f;
				}
				if (!reshape)
				{
					float num6 = result.Height / result.Width;
					if (num3 < num6)
					{
						result.Height = num3 * result.Width;
						result.Y = num2 - result.Height;
					}
					else
					{
						result.Width = result.Height / num3;
					}
				}
				break;
			case 16:
				result.X = Math.Max(newPoint.X, num - max.Width);
				result.X = Math.Min(result.X, num - min.Width);
				result.Width = num - result.X;
				if (result.Width <= 0f)
				{
					result.Width = 1f;
				}
				result.Height = Math.Min(newPoint.Y - y, max.Height);
				result.Height = Math.Max(result.Height, min.Height);
				if (!reshape)
				{
					float num9 = result.Height / result.Width;
					if (num3 < num9)
					{
						result.Height = num3 * result.Width;
						break;
					}
					result.Width = result.Height / num3;
					result.X = num - result.Width;
				}
				break;
			case 8:
				result.Width = Math.Min(newPoint.X - x, max.Width);
				result.Width = Math.Max(result.Width, min.Width);
				result.Height = Math.Min(newPoint.Y - y, max.Height);
				result.Height = Math.Max(result.Height, min.Height);
				if (!reshape)
				{
					float num8 = result.Height / result.Width;
					if (num3 < num8)
					{
						result.Height = num3 * result.Width;
					}
					else
					{
						result.Width = result.Height / num3;
					}
				}
				break;
			case 32:
				result.Y = Math.Max(newPoint.Y, num2 - max.Height);
				result.Y = Math.Min(result.Y, num2 - min.Height);
				result.Height = num2 - result.Y;
				if (result.Height <= 0f)
				{
					result.Height = 1f;
				}
				break;
			case 256:
				result.X = Math.Max(newPoint.X, num - max.Width);
				result.X = Math.Min(result.X, num - min.Width);
				result.Width = num - result.X;
				if (result.Width <= 0f)
				{
					result.Width = 1f;
				}
				break;
			case 64:
				result.Width = Math.Min(newPoint.X - x, max.Width);
				result.Width = Math.Max(result.Width, min.Width);
				break;
			case 128:
				result.Height = Math.Min(newPoint.Y - y, max.Height);
				result.Height = Math.Max(result.Height, min.Height);
				break;
			}
			return result;
		}

		/// <summary>
		/// Start editing this object in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <remarks>
		/// Because no editor is suitable for all <see cref="T:Northwoods.Go.GoObject" />s,
		/// this method does nothing by default.
		/// An implementation of this method should probably start a
		/// transaction, call <see cref="M:Northwoods.Go.GoObject.CreateEditor(Northwoods.Go.GoView)" />, initialize the
		/// resulting <see cref="T:Northwoods.Go.GoControl" /> appropriately, remember it
		/// as the value of the <see cref="P:Northwoods.Go.GoObject.Editor" /> property, and add it to
		/// the view by setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" /> property.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.CreateEditor(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" />
		public virtual void DoBeginEdit(GoView view)
		{
		}

		/// <summary>
		/// Create a GoControl that implements an editor for this object in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// A <see cref="T:Northwoods.Go.GoControl" /> object to be added to a <see cref="T:Northwoods.Go.GoView" />
		/// that is responsible for displaying a <c>Control</c> to allow the user
		/// to edit this object.
		/// </returns>
		/// <remarks>
		/// The default behavior is to just return null.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.DoBeginEdit(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Editor" />
		public virtual GoControl CreateEditor(GoView view)
		{
			return null;
		}

		/// <summary>
		/// Stop editing this object in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <remarks>
		/// By default this method does nothing.
		/// However, an implementation of this method should probably
		/// update this object appropriately, remove the <see cref="T:Northwoods.Go.GoControl" />
		/// from the view by setting the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" />
		/// property to null, and finish the transaction.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.DoBeginEdit(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CreateEditor(Northwoods.Go.GoView)" />
		public virtual void DoEndEdit(GoView view)
		{
		}

		/// <summary>
		/// Return a <see cref="T:Northwoods.Go.GoPartInfo" /> that describes this object
		/// and that can be transmitted to the client (user agent) as
		/// JavaScript data structures that can be used by code running on the client.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="renderer">an <see cref="T:Northwoods.Go.IGoPartInfoRenderer" /> from which one can allocate a <see cref="T:Northwoods.Go.GoPartInfo" /></param>
		/// <returns>
		/// By default this just returns the result of calling <see cref="M:Northwoods.Go.IGoPartInfoRenderer.GetStandardPartInfo(Northwoods.Go.GoObject)" />.
		/// </returns>
		/// <remarks>
		/// You may want to override this method in order to add property/value information
		/// for each instance of a class.
		/// You can either call <see cref="M:Northwoods.Go.IGoPartInfoRenderer.CreatePartInfo" /> and
		/// set all of its properties as desired,
		/// or you can call the base method to get the standard <see cref="T:Northwoods.Go.GoPartInfo" />
		/// as produced by the <see cref="T:Northwoods.Go.IGoPartInfoRenderer" /> and modify that.
		/// However, it may be that the renderer does not normally produce any <see cref="T:Northwoods.Go.GoPartInfo" />
		/// for this kind of object, in which case the base method call will return null.
		/// You will need to check that return value to decide whether you can modify it
		/// or whether you need to call <see cref="M:Northwoods.Go.IGoPartInfoRenderer.CreatePartInfo" />
		/// if you still need to return a <see cref="T:Northwoods.Go.GoPartInfo" />.
		/// </remarks>
		public virtual GoPartInfo GetPartInfo(GoView view, IGoPartInfoRenderer renderer)
		{
			return renderer.GetStandardPartInfo(this);
		}

		/// <summary>
		/// Gets the size of a drop shadow for this object in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// A <c>SizeF</c> value that specifies the X and Y offset from the
		/// object's position.  A positive value for X places the drop shadow
		/// toward the right; a positive value for Y places it toward the bottom.
		/// Normally this value has small positive values for both <c>Width</c>
		/// (X) and <c>Height</c> (Y), resulting in a shadow that corresponds to
		/// a light source coming from the top left of the view.
		/// </returns>
		/// <remarks>
		/// By default this just returns <c>view.ShadowOffset</c>.
		/// You can override this to customize the size of ths drop shadow
		/// for this kind of object.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowBrush(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowPen(Northwoods.Go.GoView,System.Single)" />
		/// <seealso cref="P:Northwoods.Go.GoView.ShadowOffset" />
		public virtual SizeF GetShadowOffset(GoView view)
		{
			return view?.ShadowOffset ?? default(SizeF);
		}

		/// <summary>
		/// Get a Brush for painting a drop shadow in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// A <c>Brush</c>, typically a partially transparent gray.
		/// </returns>
		/// <remarks>
		/// By default this just returns <c>view.GetShadowBrush()</c>.
		/// You can override this to customize the shadow for this kind of object.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowOffset(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowPen(Northwoods.Go.GoView,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.GetShadowBrush(Northwoods.Go.GoObject)" />
		public virtual Brush GetShadowBrush(GoView view)
		{
			return view?.GetShadowBrush(this);
		}

		/// <summary>
		/// Get a Pen for painting a drop shadow in the given view.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="width"></param>
		/// <returns>
		/// A <c>Pen</c>, typically a partially transparent gray.
		/// </returns>
		/// <remarks>
		/// By default this just returns <c>view.GetShadowPen(width)</c>.
		/// You can override this to customize the shadow for this kind of object.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowOffset(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoObject.GetShadowBrush(Northwoods.Go.GoView)" />
		/// <seealso cref="M:Northwoods.Go.GoView.GetShadowPen(Northwoods.Go.GoObject,System.Single)" />
		public virtual Pen GetShadowPen(GoView view, float width)
		{
			return view?.GetShadowPen(this, width);
		}

		/// <summary>
		/// Preserve any mutable state needed to perform an <see cref="M:Northwoods.Go.IGoUndoableEdit.Undo" />.
		/// </summary>
		/// <param name="e">The particular <see cref="T:Northwoods.Go.GoChangedEventArgs" /> edit.</param>
		/// <remarks>
		/// This does not need to be overridden for changes to properties of type integer,
		/// single float, <c>RectangleF</c>, <c>PointF</c>, <c>SizeF</c>, boolean,
		/// or references to independent objects,
		/// as long as the old value is passed in the arguments to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CopyOldValueForUndo(Northwoods.Go.GoChangedEventArgs)" />
		public virtual void CopyOldValueForUndo(GoChangedEventArgs e)
		{
		}

		/// <summary>
		/// Preserve any mutable state needed to perform an <see cref="M:Northwoods.Go.IGoUndoableEdit.Redo" />.
		/// </summary>
		/// <param name="e">The particular <see cref="T:Northwoods.Go.GoChangedEventArgs" /> edit.</param>
		/// <remarks>
		/// This does not need to be overridden for changes to properties of type integer,
		/// single float, <c>RectangleF</c>, <c>PointF</c>, <c>SizeF</c>, boolean,
		/// or references to independent objects,
		/// as long as the new value is passed in the arguments to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CopyNewValueForRedo(Northwoods.Go.GoChangedEventArgs)" />
		public virtual void CopyNewValueForRedo(GoChangedEventArgs e)
		{
		}

		/// <summary>
		/// Perform an undo or redo, given a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> representing
		/// a change on this object.
		/// </summary>
		/// <param name="e">The particular <see cref="T:Northwoods.Go.GoChangedEventArgs" /> edit.</param>
		/// <param name="undo">If true, restore the old value, otherwise restore the new value.</param>
		/// <remarks>
		/// <para>
		/// If you override this method, be sure to call the base method for all
		/// <see cref="P:Northwoods.Go.GoChangedEventArgs.SubHint" /> values that your override method does
		/// not handle.
		/// </para>
		/// <para>
		/// Although properties should be designed so that setting one property
		/// does not modify other properties, this is sometimes not practical.
		/// Nevertheless it is important to avoid having side-effects when
		/// the value is changing due to an undo or redo.
		/// One way of doing this is to copy the needed code, but not the
		/// auxiliary side-effecting code, from the property setter to the
		/// <see cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> override.  Or you could define
		/// a method called from both the setter and the ChangeValue method,
		/// parameterized by whether the caller is the setter or not.
		/// </para>
		/// <para>
		/// But a more convenient way to achieve this is to check the
		/// <see cref="P:Northwoods.Go.GoObject.Initializing" /> property that is set to true when the
		/// <see cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> method is being called on this object.
		/// You should check this property before making "unrelated" side-effects.
		/// </para>
		/// </remarks>
		/// <example>
		/// Typical usage might be something like:
		/// <code>
		///   public bool Orthogonal {
		///     get { return myOrthogonal; }
		///     set {
		///       // only set the value and do other things if the value has changed
		///       bool old = myOrthogonal;
		///       if (old != value) {
		///         myOrthogonal = value;
		///         // maybe clear out some internal cached state too
		///         ResetStuff();
		///         // notify about the change
		///         Changed(ChangedOrthogonal, 0, old, NullRect, 0, value, NullRect);
		///         // when set to true, and when not undoing/redoing, recalculate the stroke
		///         if (value &amp;&amp; !this.Initializing) {
		///           ClearPoints();
		///           CalculateRoute();
		///         }
		///       }
		///     }
		///   }
		///
		///   public override void ChangeValue(GoChangedEventArgs e, bool undo) {
		///     switch (e.SubHint) {
		///       case ChangedOrthogonal: {
		///         this.Orthogonal = (bool)e.GetValue(undo);
		///         return; }
		///       default:
		///         base.ChangeValue(e, undo);
		///         return;
		///     }
		///   }
		/// </code>
		/// </example>
		/// <seealso cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public virtual void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1000:
				break;
			case 1001:
				Bounds = e.GetRect(undo);
				break;
			case 1003:
				Visible = (bool)e.GetValue(undo);
				break;
			case 1017:
				Printable = (bool)e.GetValue(undo);
				break;
			case 1004:
				Selectable = (bool)e.GetValue(undo);
				break;
			case 1005:
				Movable = (bool)e.GetValue(undo);
				break;
			case 1006:
				Copyable = (bool)e.GetValue(undo);
				break;
			case 1007:
				Resizable = (bool)e.GetValue(undo);
				break;
			case 1008:
				Reshapable = (bool)e.GetValue(undo);
				break;
			case 1009:
				Deletable = (bool)e.GetValue(undo);
				break;
			case 1010:
				Editable = (bool)e.GetValue(undo);
				break;
			case 1011:
				AutoRescales = (bool)e.GetValue(undo);
				break;
			case 1012:
				ResizesRealtime = (bool)e.GetValue(undo);
				break;
			case 1013:
				Shadowed = (bool)e.GetValue(undo);
				break;
			case 1014:
			{
				GoObject obj2 = e.NewValue as GoObject;
				if (undo)
				{
					RemoveObserver(obj2);
				}
				else
				{
					AddObserver(obj2);
				}
				break;
			}
			case 1015:
			{
				GoObject obj = e.OldValue as GoObject;
				if (undo)
				{
					AddObserver(obj);
				}
				else
				{
					RemoveObserver(obj);
				}
				break;
			}
			case 1016:
				DragsNode = (bool)e.GetValue(undo);
				break;
			case 1041:
				Initializing = (bool)e.GetValue(undo);
				break;
			default:
				throw new ArgumentException("Unknown GoChangedEventArgs.SubHint--override GoObject.ChangeValue to handle the case: " + e.SubHint.ToString(NumberFormatInfo.InvariantInfo));
			}
		}

		/// <summary>
		/// A static method for converting a float to a RectangleF, for calls to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		/// <param name="x"></param>
		public static RectangleF MakeRect(float x)
		{
			return new RectangleF(x, 0f, 0f, 0f);
		}

		/// <summary>
		/// A static method for converting a PointF to a RectangleF, for calls to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		/// <param name="p"></param>
		public static RectangleF MakeRect(PointF p)
		{
			return new RectangleF(p.X, p.Y, 0f, 0f);
		}

		/// <summary>
		/// A static method for converting a SizeF to a RectangleF, for calls to <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		/// <param name="s"></param>
		public static RectangleF MakeRect(SizeF s)
		{
			return new RectangleF(0f, 0f, s.Width, s.Height);
		}

		internal static void InflateRect(ref RectangleF a, float w, float h)
		{
			a.X -= w;
			a.Width += w * 2f;
			a.Y -= h;
			a.Height += h * 2f;
		}

		internal static bool ContainsRect(RectangleF a, PointF b)
		{
			if (a.X <= b.X && b.X <= a.X + a.Width && a.Y <= b.Y)
			{
				return b.Y <= a.Y + a.Height;
			}
			return false;
		}

		internal static bool ContainsRect(RectangleF a, RectangleF b)
		{
			if (a.X <= b.X && b.X + b.Width <= a.X + a.Width && a.Y <= b.Y && b.Y + b.Height <= a.Y + a.Height && a.Width >= 0f)
			{
				return a.Height >= 0f;
			}
			return false;
		}

		internal static RectangleF UnionRect(RectangleF a, RectangleF b)
		{
			float num = Math.Min(a.X, b.X);
			float num2 = Math.Min(a.Y, b.Y);
			float num3 = Math.Max(a.X + a.Width, b.X + b.Width);
			float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
			return new RectangleF(num, num2, num3 - num, num4 - num2);
		}

		internal static RectangleF UnionRect(RectangleF r, PointF p)
		{
			if (p.X < r.X)
			{
				r.Width = r.X + r.Width - p.X;
				r.X = p.X;
			}
			else if (p.X > r.X + r.Width)
			{
				r.Width = p.X - r.X;
			}
			if (p.Y < r.Y)
			{
				r.Height = r.Y + r.Height - p.Y;
				r.Y = p.Y;
			}
			else if (p.Y > r.Y + r.Height)
			{
				r.Height = p.Y - r.Y;
			}
			return r;
		}

		internal static RectangleF IntersectionRect(RectangleF a, RectangleF b)
		{
			float num = Math.Max(a.X, b.X);
			float num2 = Math.Max(a.Y, b.Y);
			float num3 = Math.Min(a.X + a.Width, b.X + b.Width);
			float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
			return new RectangleF(num, num2, Math.Max(0f, num3 - num), Math.Max(0f, num4 - num2));
		}

		internal static bool IntersectsRect(RectangleF a, RectangleF b)
		{
			float width = a.Width;
			if (width < 0f)
			{
				return false;
			}
			float height = a.Height;
			if (height < 0f)
			{
				return false;
			}
			float width2 = b.Width;
			if (width2 < 0f)
			{
				return false;
			}
			float height2 = b.Height;
			if (height2 < 0f)
			{
				return false;
			}
			float x = a.X;
			float x2 = b.X;
			width += x;
			width2 += x2;
			if (x > width2 || x2 > width)
			{
				return false;
			}
			float y = a.Y;
			float y2 = b.Y;
			height += y;
			height2 += y2;
			if (y > height2 || y2 > height)
			{
				return false;
			}
			return true;
		}

		internal static void Trace(string msg)
		{
			System.Diagnostics.Trace.WriteLine(msg);
		}
	}
}
