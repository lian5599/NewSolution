using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// A GoSubGraph is a <see cref="T:Northwoods.Go.GoNode" /> that supports having a graph inside of it
	/// that the user can edit, and that the user can collapse and expand.
	/// </summary>
	[Serializable]
	public class GoSubGraph : GoSubGraphBase, IGoCollapsible
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2702;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.Collapsible" /> property.
		/// </summary>
		public const int ChangedCollapsible = 2703;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.BackgroundColor" /> property.
		/// </summary>
		public const int ChangedBackgroundColor = 2704;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.Opacity" /> property.
		/// </summary>
		public const int ChangedOpacity = 2705;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.LabelSpot" /> property.
		/// </summary>
		public const int ChangedLabelSpot = 2706;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.TopLeftMargin" /> property.
		/// </summary>
		public const int ChangedTopLeftMargin = 2707;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" /> property.
		/// </summary>
		public const int ChangedBorderPen = 2708;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.Corner" /> property.
		/// </summary>
		public const int ChangedCorner = 2710;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.Port" /> property.
		/// </summary>
		public const int ChangedPort = 2711;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.BottomRightMargin" /> property.
		/// </summary>
		public const int ChangedBottomRightMargin = 2712;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedTopLeftMargin" /> property.
		/// </summary>
		public const int ChangedCollapsedTopLeftMargin = 2713;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedBottomRightMargin" /> property.
		/// </summary>
		public const int ChangedCollapsedBottomRightMargin = 2714;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedCorner" /> property.
		/// </summary>
		public const int ChangedCollapsedCorner = 2715;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedLabelSpot" /> property.
		/// </summary>
		public const int ChangedCollapsedLabelSpot = 2716;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> property.
		/// </summary>
		public const int ChangedCollapsedObject = 2717;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.State" /> property.
		/// </summary>
		public const int ChangedState = 2718;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.SavedBounds" /> property.
		/// </summary>
		public const int ChangedSavedBounds = 2719;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.SavedPaths" /> property.
		/// </summary>
		public const int ChangedSavedPaths = 2720;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.WasExpanded" /> property.
		/// </summary>
		public const int ChangedWasExpanded = 2721;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoSubGraph.ExpandedResizable" /> property.
		/// </summary>
		public const int ChangedExpandedResizable = 2722;

		private const int flagCollapsible = 33554432;

		private const int flagExpandedResizable = 67108864;

		private const int flagWasExpanded = 134217728;

		private GoSubGraphState myState;

		private GoSubGraphHandle myHandle;

		private GoText myLabel;

		private GoPort myPort;

		private GoObject myCollapsedObject;

		private Color myBackgroundColor = Color.LightBlue;

		private float myOpacity = 25f;

		private int myLabelSpot = 32;

		private int myCollapsedLabelSpot = 1;

		private SizeF myCorner;

		private SizeF myCollapsedCorner;

		private SizeF myTopLeftMargin = new SizeF(4f, 4f);

		private SizeF myBottomRightMargin = new SizeF(4f, 4f);

		private SizeF myCollapsedTopLeftMargin;

		private SizeF myCollapsedBottomRightMargin;

		private GoShape.GoPenInfo myBorderPenInfo;

		private Dictionary<GoObject, RectangleF> myBoundsHashtable = new Dictionary<GoObject, RectangleF>();

		private Dictionary<GoObject, PointF[]> myPathsHashtable = new Dictionary<GoObject, PointF[]>();

		[NonSerialized]
		private RectangleF mySavedBoundsInsideMargins;

		[NonSerialized]
		private RectangleF myOriginalResizeDecorationBounds;

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoSubGraphHandle" /> representing and controlling the subgraph.
		/// </summary>
		public GoSubGraphHandle Handle => myHandle;

		/// <summary>
		/// Gets or sets the standard text label for this node.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public override GoText Label
		{
			get
			{
				return myLabel;
			}
			set
			{
				GoText goText = myLabel;
				if (goText != value)
				{
					if (goText != null)
					{
						Remove(goText);
					}
					myLabel = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2702, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the port represents the subgraph as a whole. 
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoPort Port
		{
			get
			{
				return myPort;
			}
			set
			{
				GoPort goPort = myPort;
				if (goPort != value)
				{
					if (goPort != null)
					{
						Remove(goPort);
					}
					myPort = value;
					if (value != null)
					{
						InsertBefore(null, value);
					}
					Changed(2711, 0, goPort, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the object shown when the subgraph is collapsed.
		/// </summary>
		/// <value>
		/// By default, <see cref="M:Northwoods.Go.GoSubGraph.CreateCollapsedObject" /> returns null,
		/// so this property is null.
		/// </value>
		/// <remarks>
		/// You will probably want this object to be not Selectable.
		/// Setting this property to a value will set its <see cref="P:Northwoods.Go.GoObject.Visible" />
		/// and <see cref="P:Northwoods.Go.GoObject.Printable" /> properties to have the same values
		/// as the old collapsed object, if any.
		/// If there was no previous collapsed object, this setter sets those
		/// two properties according to whether the subgraph is collapsed or not.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject CollapsedObject
		{
			get
			{
				return myCollapsedObject;
			}
			set
			{
				GoObject goObject = myCollapsedObject;
				if (goObject == value)
				{
					return;
				}
				if (goObject != null)
				{
					Remove(goObject);
				}
				myCollapsedObject = value;
				if (value != null)
				{
					InsertBefore(null, value);
				}
				Changed(2717, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing && CollapsedObject != null)
				{
					if (goObject != null)
					{
						CollapsedObject.Visible = goObject.Visible;
						CollapsedObject.Printable = goObject.Printable;
					}
					else
					{
						CollapsedObject.Visible = !IsExpanded;
						CollapsedObject.Printable = !IsExpanded;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum radial width and height of each corner.
		/// </summary>
		/// <value>
		/// The default value is 0x0.  Both the width and the height must be
		/// non-negative; only when both are positive will there be arcs at each corner.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoSubGraph.CollapsedCorner" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum radial width and height of each corner")]
		public virtual SizeF Corner
		{
			get
			{
				return myCorner;
			}
			set
			{
				SizeF sizeF = myCorner;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCorner = value;
					Changed(2710, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum radial width and height of each corner when the node is collapsed.
		/// </summary>
		/// <value>
		/// The default value is 0x0.  Both the width and the height must be
		/// non-negative; only when both are positive will there be arcs at each corner.
		/// </value>
		/// <remarks>
		/// Of course, when there is a <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> the border is not
		/// drawn, because that collapsed-object is shown instead.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoSubGraph.Corner" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum radial width and height of each corner of a collapsed node")]
		public virtual SizeF CollapsedCorner
		{
			get
			{
				return myCollapsedCorner;
			}
			set
			{
				SizeF sizeF = myCollapsedCorner;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCollapsedCorner = value;
					Changed(2715, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the pen used to draw a rectangular outline just inside the
		/// edges of this node.
		/// </summary>
		/// <value>
		/// You must not modify the pen after you have assigned it.
		/// The <c>Pen</c> value may be null, in which case no outline is drawn.
		/// This value defaults to null.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoSubGraph.PaintDecoration(System.Drawing.Graphics,Northwoods.Go.GoView)" /> does not draw any border when
		/// there is a <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> to show instead.
		/// </remarks>
		[Category("Appearance")]
		[Description("The pen used to draw an outline for this node.")]
		public virtual Pen BorderPen
		{
			get
			{
				if (BorderPenInfo != null)
				{
					return BorderPenInfo.GetPen();
				}
				return null;
			}
			set
			{
				BorderPenInfo = GoShape.GetPenInfo(value);
			}
		}

		internal GoShape.GoPenInfo BorderPenInfo
		{
			get
			{
				return myBorderPenInfo;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = myBorderPenInfo;
				if (goPenInfo != value && (goPenInfo == null || !goPenInfo.Equals(value)))
				{
					myBorderPenInfo = value;
					Changed(2708, 0, goPenInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />, or <b>Color.Empty</b> if there is no pen.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" /> to null.
		/// </value>
		[Category("Appearance")]
		[Description("The color of the pen used to draw the border of the group")]
		public Color BorderPenColor
		{
			get
			{
				return BorderPenInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoShape.GoPenInfo borderPenInfo = BorderPenInfo;
				GoShape.GoPenInfo goPenInfo = null;
				if (borderPenInfo != null)
				{
					if (borderPenInfo.Color == value)
					{
						return;
					}
					if (value != Color.Empty)
					{
						goPenInfo = new GoShape.GoPenInfo(borderPenInfo);
						goPenInfo.Color = value;
					}
				}
				else if (value != Color.Empty)
				{
					goPenInfo = new GoShape.GoPenInfo();
					goPenInfo.Width = BorderPenWidth;
					goPenInfo.Color = value;
				}
				BorderPenInfo = goPenInfo;
			}
		}

		/// <summary>
		/// Gets or sets the width of the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </summary>
		/// <value>
		/// The width of the <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />, or 0 if there is no pen.
		/// The default value is zero.
		/// </value>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />, setting this property might have no effect.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to draw the outline of the shape.")]
		public float BorderPenWidth
		{
			get
			{
				return BorderPenInfo?.Width ?? 0f;
			}
			set
			{
				GoShape.GoPenInfo goPenInfo = BorderPenInfo;
				float num = 0f;
				if (goPenInfo != null)
				{
					num = goPenInfo.Width;
				}
				else
				{
					goPenInfo = GoShape.PenInfo_Black;
				}
				if (num != value)
				{
					GoShape.GoPenInfo goPenInfo2 = new GoShape.GoPenInfo(goPenInfo);
					goPenInfo2.Width = value;
					BorderPenInfo = goPenInfo2;
				}
			}
		}

		/// <summary>
		/// The location is the position of the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />.
		/// </summary>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, the location is just the <see cref="P:Northwoods.Go.GoObject.Position" />.
		/// </remarks>
		public override PointF Location
		{
			get
			{
				if (Handle != null)
				{
					return Handle.Position;
				}
				return base.Position;
			}
			set
			{
				if (Handle != null)
				{
					SizeF sizeF = GoTool.SubtractPoints(Handle.Position, base.Position);
					base.Position = new PointF(value.X - sizeF.Width, value.Y - sizeF.Height);
				}
				else
				{
					base.Position = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the spot at which the label, if any, should be positioned
		/// relative to the rest of the node.
		/// </summary>
		/// <value>This defaults to <c>GoObject.MiddleTop</c></value>
		[Category("Appearance")]
		[DefaultValue(32)]
		[Description("The spot where the label should be positioned")]
		public virtual int LabelSpot
		{
			get
			{
				return myLabelSpot;
			}
			set
			{
				int num = myLabelSpot;
				if (num != value)
				{
					myLabelSpot = value;
					Changed(2706, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the spot at which the label, if any, should be positioned
		/// relative to the rest of the node.
		/// </summary>
		/// <value>This defaults to <c>GoObject.Middle</c></value>
		[Category("Appearance")]
		[DefaultValue(1)]
		[Description("The spot where the label should be positioned when the node is collapsed")]
		public virtual int CollapsedLabelSpot
		{
			get
			{
				return myCollapsedLabelSpot;
			}
			set
			{
				int num = myCollapsedLabelSpot;
				if (num != value)
				{
					myCollapsedLabelSpot = value;
					Changed(2716, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the top and left sides of the subgraph children
		/// with the top and left sides of the background.
		/// </summary>
		/// <value>
		/// This defaults to 4x4 units, in document coordinates.
		/// The margin width and height must each be non-negative.
		/// The margin should be large enough to accommodate any border drawn by the
		/// <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the left side and the top")]
		public virtual SizeF TopLeftMargin
		{
			get
			{
				return myTopLeftMargin;
			}
			set
			{
				SizeF sizeF = myTopLeftMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myTopLeftMargin = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(2707, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the bottom and right sides of the subgraph children
		/// with the bottom and right sides of the background.
		/// </summary>
		/// <value>
		/// This defaults to 4x4 units, in document coordinates.
		/// The margin width and height must each be non-negative.
		/// The margin should be large enough to accommodate any border drawn by the
		/// <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the right side and the bottom")]
		public virtual SizeF BottomRightMargin
		{
			get
			{
				return myBottomRightMargin;
			}
			set
			{
				SizeF sizeF = myBottomRightMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myBottomRightMargin = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(2712, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the top and left sides of the subgraph children
		/// with the top and left sides of the background when the node is collapsed.
		/// </summary>
		/// <value>
		/// This defaults to 0x0 units, in document coordinates.
		/// The margin width and height must each be non-negative.
		/// The margin should be large enough to accommodate any border drawn by the
		/// <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the left side and the top of a collapsed subgraph")]
		public virtual SizeF CollapsedTopLeftMargin
		{
			get
			{
				return myCollapsedTopLeftMargin;
			}
			set
			{
				SizeF sizeF = myCollapsedTopLeftMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCollapsedTopLeftMargin = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(2713, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between the bottom and right sides of the subgraph children
		/// with the bottom and right sides of the background when the node is collapsed.
		/// </summary>
		/// <value>
		/// This defaults to 0x0 units, in document coordinates.
		/// The margin width and height must each be non-negative.
		/// The margin should be large enough to accommodate any border drawn by the
		/// <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" />.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The margin around the text inside the background at the right side and the bottom of a collapsed subgraph")]
		public virtual SizeF CollapsedBottomRightMargin
		{
			get
			{
				return myCollapsedBottomRightMargin;
			}
			set
			{
				SizeF sizeF = myCollapsedBottomRightMargin;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCollapsedBottomRightMargin = value;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(2714, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this subgraph is in an expanded state.
		/// </summary>
		/// <value>
		/// The initial value is true.
		/// </value>
		/// <remarks>
		/// The getter is implemented as <c>this.State == GoSubGraphState.Expanded</c>.
		/// The setter is implemented as a call to either <see cref="M:Northwoods.Go.GoSubGraph.Collapse" />
		/// or <see cref="M:Northwoods.Go.GoSubGraph.Expand" />, depending on the value of <see cref="P:Northwoods.Go.GoSubGraph.State" />
		/// and whether the new value is false or true.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether this subgraph is in an expanded state")]
		public bool IsExpanded
		{
			get
			{
				return State == GoSubGraphState.Expanded;
			}
			set
			{
				if (!value && State == GoSubGraphState.Expanded)
				{
					Collapse();
				}
				else if (value && State == GoSubGraphState.Collapsed)
				{
					Expand();
				}
			}
		}

		/// <summary>
		/// Gets or sets the expanded/collapsed state of this subgraph.
		/// </summary>
		/// <value>This <see cref="T:Northwoods.Go.GoSubGraphState" /> enum value defaults to <see cref="F:Northwoods.Go.GoSubGraphState.Expanded" /></value>
		/// <remarks>
		/// Setting this property must only be done in pairs in the implementation of
		/// an override of <see cref="M:Northwoods.Go.GoSubGraph.Collapse" />:
		/// <code>
		///   sg.State = GoSubGraphState.Collapsing;
		///   ... move/resize children ...
		///   sg.State = GoSubGraphState.Collapsed;
		///   sg.LayoutChildren(null);
		/// </code>
		/// or in an override of <see cref="M:Northwoods.Go.GoSubGraph.Expand" />:
		/// <code>
		///   sg.State = GoSubGraphState.Expanding;
		///   ... move/resize children ...
		///   sg.State = GoSubGraphState.Expanded;
		///   sg.LayoutChildren(null);
		/// </code>
		/// </remarks>
		protected internal GoSubGraphState State
		{
			get
			{
				return myState;
			}
			set
			{
				GoSubGraphState goSubGraphState = myState;
				if (goSubGraphState != value)
				{
					myState = value;
					Changed(2718, (int)goSubGraphState, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the dictionary that remembers the sizes and relative positions
		/// of all of the child objects when the subgraph is collapsed.
		/// </summary>
		/// <value>
		/// The dictionary may be empty when the subgraph is in an expanded state.
		/// </value>
		/// <remarks>
		/// The dictionary maps child non-link objects to RectangleF structures.
		/// Although the values are in document coordinates,
		/// the (X,Y) position is not an absolute position
		/// but a position relative to the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />'s position.
		/// Modifications to this dictionary have no effect except upon
		/// a call to <see cref="M:Northwoods.Go.GoSubGraph.Expand" />, and are not recorded by the undo manager.
		/// </remarks>
		public Dictionary<GoObject, RectangleF> SavedBounds => myBoundsHashtable;

		/// <summary>
		/// Gets the dictionary that remembers the points of <see cref="T:Northwoods.Go.GoLink" />
		/// and <see cref="T:Northwoods.Go.GoLabeledLink" /> strokes when the subgraph is collapsed.
		/// </summary>
		/// <value>
		/// The dictionary may be empty when the subgraph is in an expanded state.
		/// </value>
		/// <remarks>
		/// The dictionary maps child link strokes to PointF arrays.
		/// Although the values are in document coordinates,
		/// the (X,Y) positions are not an absolute positions
		/// but positions relative to the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />'s position.
		/// Modifications to this dictionary have no effect except upon
		/// a call to <see cref="M:Northwoods.Go.GoSubGraph.Expand" />, and are not recorded by the undo manager.
		/// </remarks>
		public Dictionary<GoObject, PointF[]> SavedPaths => myPathsHashtable;

		private bool ExpandedResizable
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x4000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 67108864;
					}
					else
					{
						base.InternalFlags &= -67108865;
					}
					Changed(2722, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this subgraph had been in an expanded state
		/// when the parent subgraph was collapsed.
		/// </summary>
		/// <remarks>
		/// This is set by <see cref="M:Northwoods.Go.GoSubGraph.CollapseChild(Northwoods.Go.GoObject,System.Drawing.RectangleF)" /> and <see cref="M:Northwoods.Go.GoSubGraph.ExpandChild(Northwoods.Go.GoObject,System.Drawing.PointF)" />
		/// of the parent <see cref="T:Northwoods.Go.GoSubGraph" />.
		/// </remarks>
		public bool WasExpanded
		{
			get
			{
				return (base.InternalFlags & 0x8000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x8000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 134217728;
					}
					else
					{
						base.InternalFlags &= -134217729;
					}
					Changed(2721, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user is allowed to toggle whether this node
		/// is expanded or collapsed.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// When the value is true, the handle will be drawn as either a "-" or a "+".
		/// When this value is false, the handle will be drawn in a different fashion.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user is allowed to expand and collapse this subgraph")]
		public virtual bool Collapsible
		{
			get
			{
				return (base.InternalFlags & 0x2000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x2000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 33554432;
					}
					else
					{
						base.InternalFlags &= -33554433;
					}
					Changed(2703, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color for the group.
		/// </summary>
		/// <value>
		/// The default color is a <c>LightBlue</c>.
		/// </value>
		[Category("Appearance")]
		[Description("The background color for the group; the opacity is specified separately")]
		public virtual Color BackgroundColor
		{
			get
			{
				return myBackgroundColor;
			}
			set
			{
				Color color = myBackgroundColor;
				if (color != value)
				{
					myBackgroundColor = value;
					Changed(2704, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the percentage of opaqueness that the background color
		/// should be painted with.
		/// </summary>
		/// <value>
		/// The default value is 20.  The value must be between zero and 100.
		/// </value>
		/// <remarks>
		/// When the opacity is zero, the <see cref="P:Northwoods.Go.GoSubGraph.BackgroundColor" /> is not
		/// drawn at all.
		/// When the opacity is 100, the user cannot see what is behind the group's
		/// whole area.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(20f)]
		[Description("The opaqueness of the background; the background color is specified separately")]
		public virtual float Opacity
		{
			get
			{
				return myOpacity;
			}
			set
			{
				float num = myOpacity;
				if (num != value && value >= 0f && value <= 100f)
				{
					myOpacity = value;
					Changed(2705, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Create an empty subgraph, with an empty label.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoSubGraph.CreateHandle" />, <see cref="M:Northwoods.Go.GoSubGraph.CreateCollapsedObject" />,
		/// <see cref="M:Northwoods.Go.GoSubGraph.CreateLabel" />, and <see cref="M:Northwoods.Go.GoSubGraph.CreatePort" /> to get initial values
		/// for <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, <see cref="P:Northwoods.Go.GoSubGraph.Label" />,
		/// and <see cref="P:Northwoods.Go.GoSubGraph.Port" />.
		/// </remarks>
		public GoSubGraph()
		{
			base.InternalFlags |= 33685504;
			base.InternalFlags &= -17;
			myHandle = CreateHandle();
			Add(myHandle);
			myCollapsedObject = CreateCollapsedObject();
			Add(myCollapsedObject);
			myLabel = CreateLabel();
			Add(myLabel);
			myPort = CreatePort();
			InsertBefore(null, myPort);
			base.Initializing = false;
			LayoutChildren(null);
		}

		/// <summary>
		/// This creates the initial text label--an editable, bold, single-line
		/// <see cref="T:Northwoods.Go.GoText" /> that wraps.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel() {
		///    GoText t = new GoText();
		///    t.Selectable = false;
		///    t.Alignment = MiddleBottom;
		///    t.Wrapping = true;
		///    t.Bold = true;
		///    t.Editable = true;
		///    return t;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel()
		{
			return new GoText
			{
				Selectable = false,
				Alignment = 128,
				Wrapping = true,
				Bold = true,
				Editable = true
			};
		}

		/// <summary>
		/// This method is called to create the "handle" representing the whole subgraph
		/// that the user can select to drag or click to collapse or expand the subgraph.
		/// </summary>
		/// <returns>a <see cref="T:Northwoods.Go.GoSubGraphHandle" /></returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoSubGraphHandle CreateHandle() {
		///    GoSubGraphHandle h = new GoSubGraphHandle();
		///    return h;
		///  }
		/// </code>
		/// </example>
		protected virtual GoSubGraphHandle CreateHandle()
		{
			return new GoSubGraphHandle();
		}

		/// <summary>
		/// This method is called to create a port that represents the subgraph as a whole.
		/// </summary>
		/// <returns>By default this returns null--the subgraph does not have a port</returns>
		protected virtual GoPort CreatePort()
		{
			return null;
		}

		/// <summary>
		/// This method is called to create an object that is displayed when the node is collapsed.
		/// </summary>
		/// <returns>By default this returns null--the subgraph has no such object</returns>
		/// <remarks>
		/// It looks best when all the child nodes are the same size as the collapsed object.
		/// If your child nodes are different sizes, the collapsed object should be about the
		/// size of the largest one.
		/// If the subgraph starts in the expanded state, you should probably make sure that
		/// the <see cref="P:Northwoods.Go.GoObject.Visible" /> and <see cref="P:Northwoods.Go.GoObject.Printable" /> properties
		/// are set to false.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />
		protected virtual GoObject CreateCollapsedObject()
		{
			return null;
		}

		/// <summary>
		/// This override is needed to make sure the expanded position information
		/// is copied correctly when this subgraph is collapsed at the time of the copy.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			GoSubGraph goSubGraph = (GoSubGraph)newgroup;
			goSubGraph.myHandle = null;
			goSubGraph.myLabel = null;
			goSubGraph.myPort = null;
			goSubGraph.myCollapsedObject = null;
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current = goGroupEnumerator.Current;
					env.Copy(current);
				}
			}
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current2 = goGroupEnumerator.Current;
					GoObject goObject = (GoObject)env[current2];
					goSubGraph.Add(goObject);
					if (current2 == myHandle)
					{
						goSubGraph.myHandle = (GoSubGraphHandle)goObject;
					}
					else if (current2 == myLabel)
					{
						goSubGraph.myLabel = (GoText)goObject;
					}
					else if (current2 == myPort)
					{
						goSubGraph.myPort = (GoPort)goObject;
					}
					else if (current2 == myCollapsedObject)
					{
						goSubGraph.myCollapsedObject = goObject;
					}
				}
			}
			goSubGraph.myBoundsHashtable = new Dictionary<GoObject, RectangleF>();
			foreach (KeyValuePair<GoObject, RectangleF> item in myBoundsHashtable)
			{
				GoObject key = item.Key;
				GoObject goObject2 = (GoObject)env[key];
				if (goObject2 != null)
				{
					RectangleF value = item.Value;
					goSubGraph.myBoundsHashtable[goObject2] = value;
				}
			}
			goSubGraph.myPathsHashtable = new Dictionary<GoObject, PointF[]>();
			foreach (KeyValuePair<GoObject, PointF[]> item2 in myPathsHashtable)
			{
				GoObject key2 = item2.Key;
				GoObject goObject3 = (GoObject)env[key2];
				if (goObject3 != null)
				{
					PointF[] value2 = item2.Value;
					goSubGraph.myPathsHashtable[goObject3] = (PointF[])value2.Clone();
				}
			}
		}

		/// <summary>
		/// This override of <see cref="M:Northwoods.Go.GoGroup.Add(Northwoods.Go.GoObject)" /> makes sure the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />
		/// stays on top.
		/// </summary>
		/// <param name="obj"></param>
		public override void Add(GoObject obj)
		{
			if (Handle != null && Count >= 1)
			{
				InsertBefore(Handle, obj);
			}
			else
			{
				base.Add(obj);
			}
		}

		/// <summary>
		/// If any part is removed from this group,
		/// be sure to remove any references in local fields.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			bool result = base.Remove(obj);
			if (obj == myHandle)
			{
				myHandle = null;
			}
			else if (obj == myLabel)
			{
				myLabel = null;
			}
			else if (obj == myPort)
			{
				myPort = null;
			}
			else if (obj == myCollapsedObject)
			{
				myCollapsedObject = null;
			}
			if (SavedBounds.ContainsKey(obj))
			{
				SavedBounds.Remove(obj);
			}
			if (SavedPaths.ContainsKey(obj))
			{
				SavedPaths.Remove(obj);
			}
			return result;
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public override string FindName(GoObject child)
		{
			if (child == Handle)
			{
				return "Handle";
			}
			if (child == Label)
			{
				return "Label";
			}
			if (child == Port)
			{
				return "Port";
			}
			if (child == CollapsedObject)
			{
				return "CollapsedObject";
			}
			return base.FindName(child);
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override GoObject FindChild(string name)
		{
			if (name == "Handle")
			{
				return Handle;
			}
			if (name == "Label")
			{
				return Label;
			}
			if (name == "Port")
			{
				return Port;
			}
			if (name == "CollapsedObject")
			{
				return CollapsedObject;
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// The background of the group is displayed as a rounded rectangle with a
		/// <see cref="P:Northwoods.Go.GoSubGraph.BackgroundColor" /> and an <see cref="P:Northwoods.Go.GoSubGraph.Opacity" />,
		/// to make it easier to tell which nodes are part of which subgraph.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// This method calls <see cref="M:Northwoods.Go.GoSubGraph.PaintDecoration(System.Drawing.Graphics,Northwoods.Go.GoView)" /> in order to draw the rounded
		/// rectangle, but only if <see cref="M:Northwoods.Go.GoSubGraph.PaintsDecoration(Northwoods.Go.GoView)" /> returns true.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			if (PaintsDecoration(view))
			{
				PaintDecoration(g, view);
			}
			base.Paint(g, view);
		}

		/// <summary>
		/// Decide whether <see cref="M:Northwoods.Go.GoSubGraph.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> should call <see cref="M:Northwoods.Go.GoSubGraph.PaintDecoration(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// </summary>
		/// <param name="view"></param>
		/// <returns>
		/// true if there is no <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />,
		/// or if the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> is not visible
		/// (i.e. <see cref="M:Northwoods.Go.GoObject.CanView" /> is false,
		/// or if <see cref="M:Northwoods.Go.GoObject.CanPrint" /> is false when the view <see cref="P:Northwoods.Go.GoView.IsPrinting" />)
		/// </returns>
		public virtual bool PaintsDecoration(GoView view)
		{
			if (CollapsedObject != null)
			{
				if (!view.IsPrinting)
				{
					return !CollapsedObject.CanView();
				}
				return !CollapsedObject.CanPrint();
			}
			return true;
		}

		/// <summary>
		/// Paint a rounded rectangle using the current <see cref="P:Northwoods.Go.GoSubGraph.BackgroundColor" />,
		/// <see cref="P:Northwoods.Go.GoSubGraph.Opacity" />, <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" /> and <see cref="P:Northwoods.Go.GoSubGraph.Corner" />
		/// (or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedCorner" />).
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// When the <see cref="P:Northwoods.Go.GoSubGraph.Opacity" /> is zero, the background color is not painted at all.
		/// This calls the <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" /> method to determine the
		/// size and position of the border and background to be painted.
		/// The <see cref="P:Northwoods.Go.GoSubGraph.BorderPen" /> is drawn just inside the border bounds.
		/// This is called by <see cref="M:Northwoods.Go.GoSubGraph.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> before painting all of the child objects,
		/// when there is no <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> shown.
		/// </remarks>
		protected virtual void PaintDecoration(Graphics g, GoView view)
		{
			SizeF corner = (!IsExpanded) ? CollapsedCorner : Corner;
			RectangleF a = ComputeBorder();
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			if (Opacity > 0f)
			{
				GoRoundedRectangle.MakeRoundedRectangularPath(graphicsPath, 0f, 0f, a, corner);
				if (Shadowed)
				{
					SizeF shadowOffset = GetShadowOffset(view);
					GraphicsPath graphicsPath2 = new GraphicsPath(FillMode.Winding);
					GoRoundedRectangle.MakeRoundedRectangularPath(graphicsPath2, shadowOffset.Width, shadowOffset.Height, a, corner);
					Region region = new Region(graphicsPath2);
					region.Exclude(graphicsPath);
					Brush shadowBrush = GetShadowBrush(view);
					g.FillRegion(shadowBrush, region);
					region.Dispose();
					graphicsPath2.Dispose();
				}
				Brush brush = new SolidBrush(Color.FromArgb(checked((int)Math.Round(Opacity / 100f * 255f)), BackgroundColor));
				GoShape.DrawPath(g, view, null, brush, graphicsPath);
				brush.Dispose();
				graphicsPath.Reset();
			}
			if (BorderPen != null)
			{
				float num = (BorderPenInfo != null) ? BorderPenInfo.Width : BorderPen.Width;
				GoObject.InflateRect(ref a, (0f - num) / 2f, (0f - num) / 2f);
				GoRoundedRectangle.MakeRoundedRectangularPath(graphicsPath, 0f, 0f, a, corner);
				GoShape.DrawPath(g, view, BorderPen, null, graphicsPath);
			}
			graphicsPath.Dispose();
		}

		/// <summary>
		/// Account for the margins, and any shadow.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			SizeF sizeF;
			SizeF sizeF2;
			if (IsExpanded)
			{
				sizeF = TopLeftMargin;
				sizeF2 = BottomRightMargin;
			}
			else
			{
				sizeF = CollapsedTopLeftMargin;
				sizeF2 = CollapsedBottomRightMargin;
			}
			float num = Math.Max(1f, sizeF.Width);
			float num2 = Math.Max(1f, sizeF2.Width);
			float num3 = Math.Max(1f, sizeF.Height);
			float num4 = Math.Max(1f, sizeF2.Height);
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (shadowOffset.Width < 0f)
				{
					num = Math.Max(num, 0f - shadowOffset.Width);
				}
				else
				{
					num2 = Math.Max(num2, shadowOffset.Width);
				}
				if (shadowOffset.Height < 0f)
				{
					num3 = Math.Max(num3, 0f - shadowOffset.Height);
				}
				else
				{
					num4 = Math.Max(num4, shadowOffset.Height);
				}
			}
			return new RectangleF(rect.X - num, rect.Y - num3, rect.Width + num + num2, rect.Height + num3 + num4);
		}

		/// <summary>
		/// Unlike a regular group, the bounding rectangle is computed by ignoring all
		/// children that are not Visible.
		/// </summary>
		/// <returns>the union of all of the visible children and the result of <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" /></returns>
		/// <remarks>
		/// The Bounds should include the bordered area, as returned by <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" />,
		/// plus any other visible objects that may happen to extend beyond that bordered area.
		/// </remarks>
		protected override RectangleF ComputeBounds()
		{
			RectangleF a = default(RectangleF);
			bool flag = false;
			foreach (GoObject item in GetEnumerator())
			{
				if (item.Visible)
				{
					RectangleF bounds = item.Bounds;
					if (!flag)
					{
						a = bounds;
						flag = true;
					}
					else
					{
						a = GoObject.UnionRect(a, bounds);
					}
				}
			}
			if (!flag)
			{
				a = Bounds;
			}
			return GoObject.UnionRect(a, ComputeBorder());
		}

		/// <summary>
		/// The border goes around the rectangle returned by <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" />
		/// augmented by margins all around.
		/// </summary>
		/// <returns>The result of calling <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" /> and
		/// adding the margins, <see cref="P:Northwoods.Go.GoSubGraph.TopLeftMargin" /> and <see cref="P:Northwoods.Go.GoSubGraph.BottomRightMargin" />
		/// if <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" />, or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedTopLeftMargin" /> and
		/// <see cref="P:Northwoods.Go.GoSubGraph.CollapsedBottomRightMargin" /> if collapsed.</returns>
		public virtual RectangleF ComputeBorder()
		{
			RectangleF result = ComputeInsideMargins(null);
			if (result.Width > 0f && result.Height > 0f)
			{
				SizeF sizeF;
				SizeF sizeF2;
				if (IsExpanded)
				{
					sizeF = TopLeftMargin;
					sizeF2 = BottomRightMargin;
				}
				else
				{
					sizeF = CollapsedTopLeftMargin;
					sizeF2 = CollapsedBottomRightMargin;
				}
				result.X -= sizeF.Width;
				result.Y -= sizeF.Height;
				result.Width += sizeF.Width + sizeF2.Width;
				result.Height += sizeF.Height + sizeF2.Height;
			}
			else
			{
				result = Bounds;
			}
			return result;
		}

		private static RectangleF BoundsWithoutMargins(GoSubGraph sg)
		{
			RectangleF bounds = sg.Bounds;
			SizeF sizeF;
			SizeF sizeF2;
			if (sg.IsExpanded)
			{
				sizeF = sg.TopLeftMargin;
				sizeF2 = sg.BottomRightMargin;
			}
			else
			{
				sizeF = sg.CollapsedTopLeftMargin;
				sizeF2 = sg.CollapsedBottomRightMargin;
			}
			bounds.X += sizeF.Width;
			bounds.Y += sizeF.Height;
			bounds.Width -= sizeF.Width + sizeF2.Width;
			bounds.Height -= sizeF.Height + sizeF2.Height;
			return bounds;
		}

		/// <summary>
		/// Determine the area occupied by the subgraph's child nodes, excluding
		/// any margins.
		/// </summary>
		/// <param name="ignore">
		/// a child object to ignore, in addition to children skipped
		/// because <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMarginsSkip(Northwoods.Go.GoObject)" /> returned true
		/// </param>
		/// <returns>
		/// the region that is the inside of the subgraph,
		/// excluding the margins and any objects skipped by <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMarginsSkip(Northwoods.Go.GoObject)" />
		/// </returns>
		/// <remarks>
		/// <para>
		/// If all child objects are skipped, as when there are no nodes
		/// in the subgraph, this will try to provide a reasonable value
		/// for the area inside the margins, by using the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />
		/// if any.  Otherwise the return value may well have a <c>Width</c>
		/// and <c>Height</c> of zero.
		/// </para>
		/// <para>
		/// This is called by <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" /> with a null argument.
		/// <see cref="M:Northwoods.Go.GoSubGraph.LayoutCollapsedObject" /> calls this method passing the
		/// <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, and <see cref="M:Northwoods.Go.GoSubGraph.LayoutLabel" /> calls
		/// this method passing the <see cref="P:Northwoods.Go.GoSubGraph.Label" />, so that those layout
		/// methods can determine the area without the collapsed-object and
		/// label, respectively, affecting the calculations.
		/// </para>
		/// </remarks>
		public virtual RectangleF ComputeInsideMargins(GoObject ignore)
		{
			RectangleF rectangleF = default(RectangleF);
			bool flag = false;
			bool flag2 = false;
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current = goGroupEnumerator.Current;
					if ((ignore == null || current != ignore) && !ComputeInsideMarginsSkip(current))
					{
						RectangleF rectangleF2 = current.Bounds;
						if (!current.Visible)
						{
							GoSubGraph goSubGraph = current as GoSubGraph;
							if (goSubGraph != null)
							{
								rectangleF2 = BoundsWithoutMargins(goSubGraph);
							}
							else if (current.SelectionObject != null)
							{
								rectangleF2 = current.SelectionObject.Bounds;
							}
						}
						if (!flag)
						{
							rectangleF = rectangleF2;
							flag = true;
						}
						else
						{
							flag2 = true;
							rectangleF = GoObject.UnionRect(rectangleF, rectangleF2);
						}
					}
				}
			}
			if (!flag)
			{
				rectangleF = ((CollapsedObject == null) ? mySavedBoundsInsideMargins : CollapsedObject.Bounds);
			}
			else if (flag && !flag2)
			{
				mySavedBoundsInsideMargins = rectangleF;
			}
			return rectangleF;
		}

		/// <summary>
		/// When collapsed, update saved bounds for expand when empty.
		/// </summary>
		/// <param name="old"></param>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			if (State == GoSubGraphState.Collapsed)
			{
				RectangleF value = SavedBounds[this];
				value.X += base.Left - old.X;
				value.Y += base.Top - old.Y;
				SavedBounds[this] = value;
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" /> to
		/// see if a given child of this group should be ignored.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>
		/// true for special objects whose position
		/// depends on the bounds of all the other children or for those
		/// objects that should not affect the size of the subgraph.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Child objects such as the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, <see cref="P:Northwoods.Go.GoSubGraph.Port" />,
		/// an invisible <see cref="P:Northwoods.Go.GoSubGraph.Label" /> or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />
		/// and invisible links or internal links connected to this
		/// subgraph's <see cref="P:Northwoods.Go.GoSubGraph.Port" /> should not be included in
		/// the computation of the <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" /> method,
		/// because they may be outside of the inside area where the child
		/// nodes are.
		/// Any override of this method to return true for additional
		/// objects may also override one of the <c>Layout...</c> methods
		/// to make sure those objects are positioned in the desired manner.
		/// </para>
		/// <para>
		/// Originally this method was called from <see cref="M:Northwoods.Go.GoSubGraph.ComputeBounds" />,
		/// but we no longer tie the Bounds with the bordered area.  The latter
		/// is now governed by <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" />, which calls
		/// <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" />, which calls this method to
		/// decide which objects are really part of the subgraph and thus should
		/// be inside the border.
		/// </para>
		/// </remarks>
		protected virtual bool ComputeInsideMarginsSkip(GoObject child)
		{
			if (child == Handle)
			{
				return true;
			}
			if (child == Label)
			{
				return !child.Visible;
			}
			if (child == Port)
			{
				return true;
			}
			if (child == CollapsedObject)
			{
				return !child.Visible;
			}
			IGoLink goLink = child as IGoLink;
			if (goLink != null)
			{
				if (!child.Visible || !IsExpanded)
				{
					return true;
				}
				if (Port != null && (goLink.FromPort == Port || goLink.ToPort == Port))
				{
					return true;
				}
				if (goLink.FromPort != null && goLink.FromPort.GoObject.IsInView)
				{
					return true;
				}
				if (goLink.ToPort != null && goLink.ToPort.GoObject.IsInView)
				{
					return true;
				}
			}
			else if (CollapsedObject != null && !child.Visible && !IsExpanded)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resize selection handles go around the border, not along the Bounds.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// The resize handles are placed around the rectangle returned by
		/// <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" />.  The <see cref="M:Northwoods.Go.GoObject.CanReshape" />
		/// predicate is ignored at the present time--it is assumed to be true.
		/// If this subgraph is not resizable, a bounding handle will go around the
		/// whole subgraph, not just the bordered area.
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			RemoveSelectionHandles(sel);
			bool flag = sel.View?.CanResizeObjects() ?? true;
			if (!(CanResize() && flag))
			{
				sel.CreateBoundingHandle(this, selectedObj);
				return;
			}
			RectangleF rectangleF = ComputeBorder();
			float x = rectangleF.X;
			float x2 = rectangleF.X + rectangleF.Width / 2f;
			float x3 = rectangleF.X + rectangleF.Width;
			float y = rectangleF.Y;
			float y2 = rectangleF.Y + rectangleF.Height / 2f;
			float y3 = rectangleF.Y + rectangleF.Height;
			sel.CreateResizeHandle(this, selectedObj, new PointF(x, y), 2, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y), 4, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y3), 8, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x, y3), 16, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x2, y), 32, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x3, y2), 64, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x2, y3), 128, filled: true);
			sel.CreateResizeHandle(this, selectedObj, new PointF(x, y2), 256, filled: true);
		}

		/// <summary>
		/// Handle resizing by changing the margins instead of resizing the children.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect">ignored--this remembers the initial value of <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" /></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min">ignored--limited by <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" /></param>
		/// <param name="max"></param>
		/// <remarks>
		/// This currently ignores the <see cref="P:Northwoods.Go.GoObject.Reshapable" /> property, which is assumed to be true.
		/// Although collapsed nodes are not normally meant to be resized, when this node is collapsed, this method
		/// will update the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedTopLeftMargin" /> and the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedBottomRightMargin" />
		/// properties instead of the <see cref="P:Northwoods.Go.GoSubGraph.TopLeftMargin" /> and the <see cref="P:Northwoods.Go.GoSubGraph.BottomRightMargin" />
		/// properties.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (evttype == GoInputState.Start)
			{
				myOriginalResizeDecorationBounds = ComputeBorder();
			}
			origRect = myOriginalResizeDecorationBounds;
			RectangleF rectangleF = ComputeInsideMargins(null);
			RectangleF r = (evttype != 0) ? ComputeResize(origRect, newPoint, whichHandle, new SizeF(rectangleF.Width, rectangleF.Height), max, reshape: true) : origRect;
			if (ResizesRealtime || evttype == GoInputState.Cancel)
			{
				SizeF sizeF = new SizeF(Math.Max(0f, r.Right - rectangleF.Right), Math.Max(0f, r.Bottom - rectangleF.Bottom));
				SizeF sizeF2 = new SizeF(Math.Max(0f, rectangleF.X - r.X), Math.Max(0f, rectangleF.Y - r.Y));
				if (IsExpanded)
				{
					BottomRightMargin = sizeF;
					TopLeftMargin = sizeF2;
				}
				else
				{
					CollapsedBottomRightMargin = sizeF;
					CollapsedTopLeftMargin = sizeF2;
				}
				LayoutChildren(null);
				return;
			}
			Rectangle rect = view.ConvertDocToView(r);
			view.DrawXorBox(rect, evttype != GoInputState.Finish);
			if (evttype == GoInputState.Finish)
			{
				SizeF sizeF3 = new SizeF(Math.Max(0f, r.Right - rectangleF.Right), Math.Max(0f, r.Bottom - rectangleF.Bottom));
				SizeF sizeF4 = new SizeF(Math.Max(0f, rectangleF.X - r.X), Math.Max(0f, rectangleF.Y - r.Y));
				if (IsExpanded)
				{
					BottomRightMargin = sizeF3;
					TopLeftMargin = sizeF4;
				}
				else
				{
					CollapsedBottomRightMargin = sizeF3;
					CollapsedTopLeftMargin = sizeF4;
				}
				LayoutChildren(null);
			}
		}

		/// <summary>
		/// This method does not move any of the children except possibly the
		/// <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, the <see cref="P:Northwoods.Go.GoSubGraph.Label" />, the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />,
		/// and the <see cref="P:Northwoods.Go.GoSubGraph.Port" />.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// In other words, this method does not move any of the child nodes or
		/// links of this subgraph--just the known auxiliary objects.
		/// This calls <see cref="M:Northwoods.Go.GoSubGraph.LayoutCollapsedObject" />, <see cref="M:Northwoods.Go.GoSubGraph.LayoutLabel" />,
		/// <see cref="M:Northwoods.Go.GoSubGraph.LayoutHandle" />, and <see cref="M:Northwoods.Go.GoSubGraph.LayoutPort" />, in that order.
		/// This method does nothing if <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true,
		/// nor if the <see cref="P:Northwoods.Go.GoSubGraph.State" /> is <see cref="F:Northwoods.Go.GoSubGraphState.Collapsing" />
		/// or <see cref="F:Northwoods.Go.GoSubGraphState.Expanding" />,
		/// nor if the <see cref="P:Northwoods.Go.GoSubGraph.Handle" /> or the <see cref="P:Northwoods.Go.GoSubGraph.Port" /> is the
		/// child object causing the call to this method.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (!base.Initializing && State != GoSubGraphState.Collapsing && State != GoSubGraphState.Expanding && (childchanged != Handle || childchanged == null) && (childchanged != Port || childchanged == null))
			{
				LayoutCollapsedObject();
				LayoutLabel();
				LayoutHandle();
				if (Handle != null && Label != null && Handle.Position == Label.Position)
				{
					LayoutLabel();
				}
				LayoutPort();
			}
		}

		/// <summary>
		/// This positions the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> (if any) within the group.
		/// </summary>
		/// <remarks>
		/// By default the position of the CollapsedObject is set to be in the top-left
		/// corner of the rectangle returned by <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMargins(Northwoods.Go.GoObject)" />
		/// if <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" /> is true, or else by <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedRectangle(System.Drawing.SizeF)" />
		/// if this subgraph is collapsed.
		/// </remarks>
		public virtual void LayoutCollapsedObject()
		{
			GoObject collapsedObject = CollapsedObject;
			if (collapsedObject != null)
			{
				RectangleF rectangleF;
				if (IsExpanded)
				{
					rectangleF = ComputeInsideMargins(collapsedObject);
				}
				else
				{
					SizeF s = ComputeCollapsedSize(visible: true);
					rectangleF = ComputeCollapsedRectangle(s);
				}
				collapsedObject.Position = new PointF(rectangleF.X, rectangleF.Y);
			}
		}

		/// <summary>
		/// This positions the <see cref="P:Northwoods.Go.GoSubGraph.Label" /> within the group, according to the <see cref="P:Northwoods.Go.GoSubGraph.LabelSpot" />
		/// or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedLabelSpot" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When the <see cref="P:Northwoods.Go.GoSubGraph.LabelSpot" /> refers to a corner, the <see cref="P:Northwoods.Go.GoSubGraph.Label" /> is
		/// actually positioned above or below the subgraph, much like headers or footers in
		/// a page layout.  Furthermore, when the <see cref="P:Northwoods.Go.GoSubGraph.LabelSpot" /> is
		/// <see cref="F:Northwoods.Go.GoObject.TopLeft" />, if there is a <see cref="P:Northwoods.Go.GoSubGraph.Handle" /> and it is
		/// at exactly the same position, the label is shifted right to avoid the overlap.
		/// If this node is not <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" />, the label is positioned according
		/// to the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedLabelSpot" />.
		/// If there is a <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> the label is positioned relative
		/// to that collapsed object; otherwise it is positioned relative to the result of
		/// <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedRectangle(System.Drawing.SizeF)" />.
		/// </para>
		/// <para>
		/// You can override this method to locate the label in customized places.
		/// But note that the default behavior of <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMarginsSkip(Northwoods.Go.GoObject)" /> is to
		/// include the <see cref="P:Northwoods.Go.GoSubGraph.Label" /> in the background/border decoration as computed by
		/// <see cref="M:Northwoods.Go.GoSubGraph.ComputeBorder" />.  If you want the label to be
		/// positioned outside of the border decoration, you will need to
		/// override <see cref="M:Northwoods.Go.GoSubGraph.ComputeInsideMarginsSkip(Northwoods.Go.GoObject)" /> as follows:
		/// <pre><code>
		/// protected override bool ComputeInsideMarginsSkip(GoObject child) {
		///   if (child == this.Label &amp;&amp; this.IsExpanded) return true;
		///   return base.ComputeInsideMarginsSkip(child);
		/// }
		/// </code></pre>
		/// This example override will cause the <see cref="P:Northwoods.Go.GoSubGraph.Label" /> not to necessarily
		/// be inside the border when the subgraph is expanded.
		/// </para>
		/// </remarks>
		public virtual void LayoutLabel()
		{
			GoText label = Label;
			if (label == null)
			{
				return;
			}
			checked
			{
				int spot;
				RectangleF r;
				if (IsExpanded)
				{
					int num = 0;
					if (Handle != null)
					{
						num++;
					}
					if (Label != null)
					{
						num++;
					}
					if (Port != null)
					{
						num++;
					}
					if (CollapsedObject != null)
					{
						num++;
					}
					if (num == Count)
					{
						return;
					}
					spot = LabelSpot;
					r = ComputeInsideMargins(label);
				}
				else
				{
					spot = CollapsedLabelSpot;
					GoObject collapsedObject = CollapsedObject;
					if (collapsedObject != null)
					{
						r = collapsedObject.Bounds;
					}
					else
					{
						SizeF s = ComputeCollapsedSize(visible: true);
						r = ComputeCollapsedRectangle(s);
					}
				}
				PointF rectangleSpotLocation = GetRectangleSpotLocation(r, spot);
				PositionLabel(label, spot, rectangleSpotLocation);
			}
		}

		private void PositionLabel(GoText lab, int spot, PointF pt)
		{
			switch (spot)
			{
			case 2:
				lab.Alignment = spot;
				lab.SetSpotLocation(16, pt);
				if (Handle != null && Handle.Position == lab.Position)
				{
					pt.X += Handle.Width + 2f;
					lab.SetSpotLocation(16, pt);
				}
				break;
			case 4:
				lab.Alignment = spot;
				lab.SetSpotLocation(8, pt);
				break;
			case 8:
				lab.Alignment = spot;
				lab.SetSpotLocation(4, pt);
				break;
			case 16:
				lab.Alignment = spot;
				lab.SetSpotLocation(2, pt);
				break;
			default:
				lab.Alignment = SpotOpposite(spot);
				lab.SetSpotLocation(SpotOpposite(spot), pt);
				break;
			}
		}

		/// <summary>
		/// This positions the handle within the group.
		/// </summary>
		/// <remarks>
		/// By default this places the handle at the top left corner of the group, inside the margin.
		/// This method does nothing if the node is not <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" />.
		/// </remarks>
		/// <example>
		/// To place the handle in the margin at the top-left corner, rather than
		/// the default position inside the margins:
		/// <code>
		///   public override void LayoutHandle() {
		///     if (!this.IsExpanded) return;
		///     GoSubGraphHandle h = this.Handle;
		///     if (h != null) {
		///       RectangleF b = ComputeBorder();
		///       // top-left, in the margins
		///       h.Position = new PointF(b.X, b.Y);
		///     }
		///   }
		/// </code>
		/// </example>
		/// <example>
		/// <code>
		///   // To place the handle at the top-right corner, you could implement these overrides:
		///   public override void LayoutHandle() {
		///     if (!this.IsExpanded) return;
		///     GoSubGraphHandle h = this.Handle;
		///     if (h != null) {
		///       RectangleF b = ComputeInsideMargins(null);
		///       // top-right, inside margin
		///       h.SetSpotLocation(TopRight, GetRectangleSpotLocation(b, TopRight));
		///     }
		///   }
		///
		///   // Make sure the collapsed subgraph body (including any CollapsedObject)
		///   // is positioned to the left of the handle.
		///   protected override RectangleF ComputeCollapsedRectangle(SizeF s) {
		///     PointF hpos = ComputeReferencePoint();
		///     return new RectangleF(hpos.X + this.Handle.Width - s.Width, hpos.Y, s.Width, s.Height);
		///   }
		/// </code>
		/// For more examples, see the classes in the SubGraphApp sample.
		/// </example>
		public virtual void LayoutHandle()
		{
			if (IsExpanded)
			{
				GoSubGraphHandle handle = Handle;
				if (handle != null)
				{
					RectangleF rectangleF = ComputeInsideMargins(null);
					handle.Position = new PointF(rectangleF.X, rectangleF.Y);
				}
			}
		}

		/// <summary>
		/// This positions the port within the group, normally exactly where the <see cref="P:Northwoods.Go.GoSubGraph.Handle" /> is.
		/// </summary>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, the port gets the bounds of the <see cref="P:Northwoods.Go.GoSubGraph.Label" />.
		/// If there is no <see cref="P:Northwoods.Go.GoSubGraph.Label" />, the port is positioned at the top left corner, inside the margin.
		/// If you override this, you should be sure to position the port within the bounds of the subgraph.
		/// </remarks>
		/// <example>
		/// For example, if you want the port's Bounds to be the same as the bordered area,
		/// override this method as follows:
		/// <code>
		///   public override void LayoutPort() {
		///     GoPort p = this.Port;
		///     if (p != null &amp;&amp; p.CanView()) {
		///       RectangleF r = ComputeBorder();
		///       p.Bounds = r;
		///     }
		///   }
		/// </code>
		/// For more examples, see the classes in the SubGraphApp sample.
		/// </example>
		public virtual void LayoutPort()
		{
			GoPort port = Port;
			if (port != null)
			{
				if (Handle != null)
				{
					RectangleF rectangleF = port.Bounds = Handle.Bounds;
					return;
				}
				if (Label != null)
				{
					port.Bounds = Label.Bounds;
					return;
				}
				RectangleF rectangleF2 = ComputeInsideMargins(null);
				port.Position = new PointF(rectangleF2.X, rectangleF2.Y);
			}
		}

		/// <summary>
		/// Determine the size that this subgraph should be when collapsed, excluding any collapsed margin.
		/// </summary>
		/// <param name="visible">true when the size of any <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> should be considered</param>
		/// <returns>the size of <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, if it is not null/nothing and <paramref name="visible" /> is true,
		/// and the union of the sizes of all the children, including the collapsed sizes of subgraphs</returns>
		/// <remarks>
		/// <para>
		/// Unlike <see cref="M:Northwoods.Go.GoSubGraph.ComputeBounds" />, this method is used to calculate the expected size of
		/// this node when collapsed, so that <see cref="M:Northwoods.Go.GoSubGraph.Collapse" /> can call <see cref="M:Northwoods.Go.GoSubGraph.CollapseChild(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />
		/// and <see cref="M:Northwoods.Go.GoSubGraph.FinishCollapse(System.Drawing.RectangleF)" /> with the intended collapsed bounds.
		/// The size of each child (that is not skipped by <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedSizeSkip(Northwoods.Go.GoObject)" />)
		/// is given by the <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.SelectionObject" />,
		/// except for nested subgraphs, where <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedSize(System.Boolean)" /> is called recursively.
		/// This method does not augment the size to include the <see cref="P:Northwoods.Go.GoSubGraph.CollapsedTopLeftMargin" />
		/// or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedBottomRightMargin" />.
		/// </para>
		/// <para>
		/// Note that the standard behavior of this method makes sure the resulting size is wide
		/// enough to hold each child node and tall enough to hold each child node--it is not just
		/// the size of any <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />.
		/// The reason is that the standard behavior wants to insure that all links connected to child nodes
		/// will end at or in the collapsed subgraph, not potentially far away from the subgraph.
		/// </para>
		/// </remarks>
		public virtual SizeF ComputeCollapsedSize(bool visible)
		{
			SizeF result = new SizeF(0f, 0f);
			if (visible && CollapsedObject != null)
			{
				return CollapsedObject.Size;
			}
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					GoObject current = goGroupEnumerator.Current;
					if (!ComputeCollapsedSizeSkip(current))
					{
						GoSubGraph goSubGraph = current as GoSubGraph;
						SizeF sizeF;
						if (goSubGraph == null)
						{
							sizeF = ((current.SelectionObject == null) ? current.Size : current.SelectionObject.Size);
						}
						else if (goSubGraph.IsExpanded)
						{
							sizeF = goSubGraph.ComputeCollapsedSize(visible: false);
						}
						else
						{
							RectangleF rectangleF = BoundsWithoutMargins(goSubGraph);
							sizeF = new SizeF(rectangleF.Width, rectangleF.Height);
						}
						result.Width = Math.Max(result.Width, sizeF.Width);
						result.Height = Math.Max(result.Height, sizeF.Height);
					}
				}
				return result;
			}
		}

		/// <summary>
		/// This predicate is called by <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedSize(System.Boolean)" /> to decide if a
		/// child object should take part in the size calculation.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>
		/// true if the object is the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, <see cref="P:Northwoods.Go.GoSubGraph.Label" />,
		/// <see cref="P:Northwoods.Go.GoSubGraph.Port" />, <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, or a link.
		/// If there is a <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />, all invisible objects are skipped.
		/// </returns>
		protected virtual bool ComputeCollapsedSizeSkip(GoObject child)
		{
			if (child == Handle)
			{
				return true;
			}
			if (child == Label)
			{
				return true;
			}
			if (child == Port)
			{
				return true;
			}
			if (child == CollapsedObject)
			{
				return true;
			}
			if (child is IGoLink)
			{
				return true;
			}
			if (CollapsedObject != null && !child.Visible)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Return the point from which the relative positions of the children are calculated.
		/// </summary>
		/// <returns>
		/// a <c>PointF</c> in document coordinates,
		/// normally the <see cref="P:Northwoods.Go.GoObject.Position" /> of the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />
		/// </returns>
		protected virtual PointF ComputeReferencePoint()
		{
			if (Handle != null)
			{
				return Handle.Position;
			}
			return base.Position;
		}

		/// <summary>
		/// Given a desired collapsed size, return the expected collapsed area, ignoring any collapsed margins.
		/// </summary>
		/// <param name="s">
		/// a <c>SizeF</c> in document coordinates, normally the result of a call to <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedSize(System.Boolean)" />
		/// </param>
		/// <returns>
		/// a <c>RectangleF</c> in document coordinates that is passed to calls to
		/// <see cref="M:Northwoods.Go.GoSubGraph.SaveChildBounds(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />, <see cref="M:Northwoods.Go.GoSubGraph.CollapseChild(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />, and <see cref="M:Northwoods.Go.GoSubGraph.FinishCollapse(System.Drawing.RectangleF)" />.
		/// </returns>
		/// <remarks>
		/// This depends on the result of a call to <see cref="M:Northwoods.Go.GoSubGraph.ComputeReferencePoint" />.
		/// See also the example in the description for <see cref="M:Northwoods.Go.GoSubGraph.LayoutHandle" />.
		/// </remarks>
		protected virtual RectangleF ComputeCollapsedRectangle(SizeF s)
		{
			PointF pointF = ComputeReferencePoint();
			return new RectangleF(pointF.X, pointF.Y, s.Width, s.Height);
		}

		/// <summary>
		/// Reposition all of the children so that this node takes a minimum amount of space.
		/// </summary>
		/// <remarks>
		/// This method will first set the <see cref="P:Northwoods.Go.GoSubGraph.State" /> to <see cref="F:Northwoods.Go.GoSubGraphState.Collapsing" />.
		/// It then calls <see cref="M:Northwoods.Go.GoSubGraph.PrepareCollapse" />, which collapses any nested subgraphs.
		/// It will call <see cref="M:Northwoods.Go.GoSubGraph.ComputeReferencePoint" /> to determine the reference position
		/// for the collapsed subgraph.
		/// Then it will call <see cref="M:Northwoods.Go.GoSubGraph.ComputeCollapsedSize(System.Boolean)" /> to determine the desired size of
		/// the collapsed subgraph,
		/// will call <see cref="M:Northwoods.Go.GoSubGraph.SaveChildBounds(Northwoods.Go.GoObject,System.Drawing.RectangleF)" /> to remember the expanded size and
		/// relative position of each child,
		/// will call <see cref="M:Northwoods.Go.GoSubGraph.CollapseChild(Northwoods.Go.GoObject,System.Drawing.RectangleF)" /> to actually move and perhaps resize
		/// each child, and then will call <see cref="M:Northwoods.Go.GoSubGraph.FinishCollapse(System.Drawing.RectangleF)" />.
		/// It then sets the state to <see cref="F:Northwoods.Go.GoSubGraphState.Collapsed" />.
		/// Afterwards, <see cref="P:Northwoods.Go.GoSubGraph.IsExpanded" /> will be false, and this method
		/// calls <c>LayoutChildren(null)</c> to give <see cref="M:Northwoods.Go.GoSubGraph.LayoutLabel" />,
		/// <see cref="M:Northwoods.Go.GoSubGraph.LayoutHandle" />, and <see cref="M:Northwoods.Go.GoSubGraph.LayoutPort" /> a chance.
		/// This method does nothing if this node is already collapsed or if
		/// <see cref="P:Northwoods.Go.GoSubGraph.Collapsible" /> is false.
		/// </remarks>
		public virtual void Collapse()
		{
			if (State == GoSubGraphState.Expanded && Collapsible)
			{
				GoDocument document = base.Document;
				bool old = false;
				if (document != null)
				{
					old = document.SuspendsRouting;
					document.SuspendsRouting = true;
				}
				State = GoSubGraphState.Collapsing;
				Changed(1041, 0, false, GoObject.NullRect, 0, true, GoObject.NullRect);
				Changing(2720);
				Changing(2719);
				SavedBounds[this] = Bounds;
				MarkPortsInside(this, inside: true);
				PrepareCollapse();
				SizeF s = ComputeCollapsedSize(visible: true);
				RectangleF sgrect = ComputeCollapsedRectangle(s);
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current = goGroupEnumerator.Current;
						SaveChildBounds(current, sgrect);
					}
				}
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current2 = goGroupEnumerator.Current;
						CollapseChild(current2, sgrect);
					}
				}
				FinishCollapse(sgrect);
				State = GoSubGraphState.Collapsed;
				LayoutChildren(null);
				base.InvalidBounds = true;
				Changed(2719, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
				Changed(2720, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
				Changed(1041, 0, true, GoObject.NullRect, 0, false, GoObject.NullRect);
				document?.ResumeRouting(old, null);
			}
		}

		/// <summary>
		/// Do any work before collapsing this subgraph.
		/// </summary>
		/// <remarks>
		/// By default this does nothing.
		/// </remarks>
		protected virtual void PrepareCollapse()
		{
		}

		/// <summary>
		/// Save the original size and relative position for all non-links and
		/// the paths of all links.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="sgrect">the expected bounds of the collapsed subgraph, without the collapsed margin</param>
		/// <remarks>
		/// This saves the points of link strokes, relative to the <paramref name="sgrect" />'s position,
		/// in the <see cref="P:Northwoods.Go.GoSubGraph.SavedPaths" /> hash table.
		/// For other objects, besides the <see cref="P:Northwoods.Go.GoSubGraph.Handle" />, <see cref="P:Northwoods.Go.GoSubGraph.Port" />,
		/// <see cref="P:Northwoods.Go.GoSubGraph.Label" />, or <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />,
		/// this saves the current size and position relative
		/// to the <paramref name="sgrect" /> in the <see cref="P:Northwoods.Go.GoSubGraph.SavedBounds" /> hash table.
		/// </remarks>
		protected virtual void SaveChildBounds(GoObject child, RectangleF sgrect)
		{
			if (child == Handle || child == Label || child == Port || child == CollapsedObject)
			{
				return;
			}
			PointF pointF = ComputeReferencePoint();
			if (child is GoLink || child is GoLabeledLink)
			{
				GoStroke goStroke = null;
				goStroke = ((!(child is GoLink)) ? ((GoLabeledLink)child).RealLink : ((GoLink)child));
				if (goStroke != null)
				{
					PointF[] array = goStroke.CopyPointsArray();
					for (int i = 0; i < array.Length; i = checked(i + 1))
					{
						PointF pointF2 = array[i];
						pointF2.X -= pointF.X;
						pointF2.Y -= pointF.Y;
						array[i] = pointF2;
					}
					SavedPaths[child] = array;
				}
			}
			else
			{
				RectangleF bounds = child.Bounds;
				SavedBounds[child] = new RectangleF(bounds.X - pointF.X, bounds.Y - pointF.Y, bounds.Width, bounds.Height);
			}
		}

		/// <summary>
		/// Center all the objects that aren't links and make everything invisible
		/// (besides Label and Handle and Port and CollapsedObject).
		/// </summary>
		/// <param name="child"></param>
		/// <param name="sgrect">the expected bounds of the collapsed subgraph, without the collapsed margin</param>
		/// <remarks>
		/// <para>
		/// Child subgraphs are collapsed first, of course, and if they had been expanded,
		/// their <see cref="P:Northwoods.Go.GoSubGraph.WasExpanded" /> property is set to true.
		/// If the child object has a <see cref="P:Northwoods.Go.GoObject.SelectionObject" />, that
		/// is centered; otherwise the whole child is centered.
		/// </para>
		/// <para>
		/// You may want to override this method in order to customize the
		/// sizing of child objects when the subgraph is collapsed.  If you do
		/// this, you will also want to override <see cref="M:Northwoods.Go.GoSubGraph.ExpandChild(Northwoods.Go.GoObject,System.Drawing.PointF)" /> to
		/// restore the original size, as saved in the <see cref="P:Northwoods.Go.GoSubGraph.SavedBounds" />
		/// hash table.
		/// </para>
		/// </remarks>
		protected virtual void CollapseChild(GoObject child, RectangleF sgrect)
		{
			if (child == Handle || child == Label || child == Port || child == CollapsedObject)
			{
				return;
			}
			if (!(child is IGoLink))
			{
				GoSubGraph goSubGraph = child as GoSubGraph;
				if (goSubGraph != null && goSubGraph.IsExpanded)
				{
					goSubGraph.WasExpanded = true;
					goSubGraph.Collapse();
				}
				PointF pointF = new PointF(sgrect.X + sgrect.Width / 2f, sgrect.Y + sgrect.Height / 2f);
				child.Position = new PointF(pointF.X - child.Width / 2f, pointF.Y - child.Height / 2f);
			}
			child.Visible = false;
			child.Printable = false;
		}

		/// <summary>
		/// This method is called towards the end of the process of a <see cref="M:Northwoods.Go.GoSubGraph.Collapse" /> call.
		/// </summary>
		/// <param name="sgrect">the expected bounds of the collapsed subgraph, without the collapsed margin</param>
		/// <remarks>
		/// By default this method makes any <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" /> visible and makes the whole subgraph not resizable.
		/// </remarks>
		protected virtual void FinishCollapse(RectangleF sgrect)
		{
			if (CollapsedObject != null)
			{
				CollapsedObject.Visible = true;
				CollapsedObject.Printable = true;
			}
			if (Resizable)
			{
				ExpandedResizable = true;
				Resizable = false;
			}
		}

		private void MarkPortsInside(GoGroup parent, bool inside)
		{
			foreach (GoObject item in parent)
			{
				GoPort goPort = item as GoPort;
				if (goPort != null)
				{
					goPort.InsideCollapsedSubGraph = inside;
				}
				else
				{
					GoGroup goGroup = item as GoGroup;
					if (goGroup != null && !(goGroup is GoSubGraph))
					{
						MarkPortsInside(goGroup, inside);
					}
				}
			}
		}

		/// <summary>
		/// Restore the position of all of the children so that this subgraph completely visible again.
		/// </summary>
		/// <remarks>
		/// The <see cref="M:Northwoods.Go.GoSubGraph.Collapse" /> call saved the original bounds of all of the non-link children.
		/// These saved relative positions are used to move all of the children back again,
		/// and making them all visible too.
		/// This method will first set the <see cref="P:Northwoods.Go.GoSubGraph.State" /> to <see cref="F:Northwoods.Go.GoSubGraphState.Expanding" />, and then
		/// call <see cref="M:Northwoods.Go.GoSubGraph.PrepareExpand" /> for any preparatory work.
		/// It will call <see cref="M:Northwoods.Go.GoSubGraph.ComputeReferencePoint" /> to determine the reference position
		/// for the expanded subgraph.
		/// It calls <see cref="M:Northwoods.Go.GoSubGraph.ExpandChild(Northwoods.Go.GoObject,System.Drawing.PointF)" /> on each child to move them back to their
		/// original positions,
		/// and will call <see cref="M:Northwoods.Go.GoSubGraph.FinishExpand(System.Drawing.PointF)" /> for any clean-up work. 
		/// It then sets the state to <see cref="F:Northwoods.Go.GoSubGraphState.Expanded" />.
		/// This method does nothing if this node is already expanded or
		/// if <see cref="P:Northwoods.Go.GoSubGraph.Collapsible" /> is false.
		/// This method also calls <c>LayoutChildren(null)</c> afterwards, mostly to make
		/// sure the <see cref="P:Northwoods.Go.GoSubGraph.Handle" /> and <see cref="P:Northwoods.Go.GoSubGraph.Label" /> are positioned
		/// correctly again, especially if the objects changed while the node was collapsed.
		/// </remarks>
		public virtual void Expand()
		{
			if (State == GoSubGraphState.Collapsed && Collapsible)
			{
				GoDocument document = base.Document;
				bool old = false;
				if (document != null)
				{
					old = document.SuspendsRouting;
					document.SuspendsRouting = true;
				}
				State = GoSubGraphState.Expanding;
				Changed(1041, 0, false, GoObject.NullRect, 0, true, GoObject.NullRect);
				Changing(2720);
				Changing(2719);
				MarkPortsInside(this, inside: false);
				PrepareExpand();
				PointF hpos = ComputeReferencePoint();
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current = goGroupEnumerator.Current;
						if (!(current is IGoLink))
						{
							ExpandChild(current, hpos);
						}
					}
				}
				using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
				{
					while (goGroupEnumerator.MoveNext())
					{
						GoObject current2 = goGroupEnumerator.Current;
						if (current2 is IGoLink)
						{
							ExpandChild(current2, hpos);
						}
					}
				}
				bool num = SavedBounds.Count <= 1;
				RectangleF rectangleF = default(RectangleF);
				if (SavedBounds.ContainsKey(this))
				{
					rectangleF = SavedBounds[this];
				}
				FinishExpand(hpos);
				State = GoSubGraphState.Expanded;
				LayoutChildren(null);
				base.InvalidBounds = true;
				if (num)
				{
					base.Position = new PointF(rectangleF.X, rectangleF.Y);
				}
				Changed(2719, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
				Changed(2720, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
				Changed(1041, 0, true, GoObject.NullRect, 0, false, GoObject.NullRect);
				document?.ResumeRouting(old, null);
			}
		}

		/// <summary>
		/// Do any work before expanding the children.
		/// </summary>
		/// <remarks>
		/// By default this does nothing.
		/// </remarks>
		protected virtual void PrepareExpand()
		{
		}

		/// <summary>
		/// Restore a child to its original position.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="hpos">the reference point for restoring the new position</param>
		/// <remarks>
		/// This uses <see cref="P:Northwoods.Go.GoSubGraph.SavedBounds" /> for getting the saved
		/// relative position of a child, and this uses <see cref="P:Northwoods.Go.GoSubGraph.SavedPaths" />
		/// for getting the relative points of link strokes.
		/// Nested subgraphs that are <see cref="P:Northwoods.Go.GoSubGraph.WasExpanded" /> (i.e. were collapsed by <see cref="M:Northwoods.Go.GoSubGraph.CollapseChild(Northwoods.Go.GoObject,System.Drawing.RectangleF)" />) are expanded.
		/// This method also makes sure each child is made <see cref="P:Northwoods.Go.GoObject.Visible" /> again.
		/// </remarks>
		protected virtual void ExpandChild(GoObject child, PointF hpos)
		{
			if (child == CollapsedObject)
			{
				return;
			}
			child.Visible = true;
			child.Printable = true;
			if (child is GoLink || child is GoLabeledLink)
			{
				GoStroke goStroke = null;
				goStroke = ((!(child is GoLink)) ? ((GoLabeledLink)child).RealLink : ((GoLink)child));
				if (goStroke != null && SavedPaths.ContainsKey(child))
				{
					PointF[] array = SavedPaths[child];
					PointF[] array2 = (PointF[])array.Clone();
					for (int i = 0; i < array.Length; i = checked(i + 1))
					{
						PointF pointF = array2[i];
						pointF.X += hpos.X;
						pointF.Y += hpos.Y;
						array2[i] = pointF;
					}
					goStroke.SetPoints(array2);
					child.Document?.DelayedRoutings.Remove(child);
				}
			}
			else if (SavedBounds.ContainsKey(child))
			{
				RectangleF rectangleF = SavedBounds[child];
				GoSubGraph goSubGraph = child as GoSubGraph;
				if (goSubGraph != null && goSubGraph.WasExpanded)
				{
					goSubGraph.WasExpanded = false;
					goSubGraph.Expand();
				}
				child.Position = new PointF(hpos.X + rectangleF.X, hpos.Y + rectangleF.Y);
			}
		}

		/// <summary>
		/// This method is called at the end of the process of an <see cref="M:Northwoods.Go.GoSubGraph.Expand" /> call.
		/// </summary>
		/// <param name="hpos">the reference point for restoring the new position</param>
		/// <remarks>
		/// By default this just clears all saved information in the <see cref="P:Northwoods.Go.GoSubGraph.SavedBounds" />
		/// and <see cref="P:Northwoods.Go.GoSubGraph.SavedPaths" /> hash tables, it makes any <see cref="P:Northwoods.Go.GoSubGraph.CollapsedObject" />
		/// not visible, and it restores whether the expanded subgraph is resizable.
		/// </remarks>
		protected virtual void FinishExpand(PointF hpos)
		{
			if (CollapsedObject != null)
			{
				CollapsedObject.Visible = false;
				CollapsedObject.Printable = false;
			}
			if (ExpandedResizable)
			{
				ExpandedResizable = false;
				Resizable = true;
			}
			SavedBounds.Clear();
			SavedPaths.Clear();
		}

		/// <summary>
		/// This convenience method calls either <see cref="M:Northwoods.Go.GoSubGraph.Collapse" /> or <see cref="M:Northwoods.Go.GoSubGraph.Expand" />
		/// depending on whether it is expanded or collapsed.
		/// </summary>
		public void Toggle()
		{
			if (State == GoSubGraphState.Expanded)
			{
				Collapse();
			}
			else if (State == GoSubGraphState.Collapsed)
			{
				Expand();
			}
		}

		/// <summary>
		/// This method calls <see cref="M:Northwoods.Go.GoSubGraph.Expand" /> on this node and then recursively
		/// proceeds through all of the children, expanding them.
		/// </summary>
		public virtual void ExpandAll()
		{
			Expand();
			using (GoGroupEnumerator goGroupEnumerator = GetEnumerator())
			{
				while (goGroupEnumerator.MoveNext())
				{
					(goGroupEnumerator.Current as GoSubGraph)?.ExpandAll();
				}
			}
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			switch (e.SubHint)
			{
			case 2719:
				if (e.IsBeforeChanging)
				{
					e.OldValue = new Dictionary<GoObject, RectangleF>(myBoundsHashtable);
				}
				break;
			case 2720:
				if (e.IsBeforeChanging)
				{
					e.OldValue = new Dictionary<GoObject, PointF[]>(myPathsHashtable);
				}
				break;
			default:
				base.CopyOldValueForUndo(e);
				break;
			}
		}

		/// <summary>
		/// Copies state to permit a redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyNewValueForRedo(GoChangedEventArgs e)
		{
			switch (e.SubHint)
			{
			case 2719:
				if (!e.IsBeforeChanging)
				{
					e.NewValue = new Dictionary<GoObject, RectangleF>(myBoundsHashtable);
				}
				break;
			case 2720:
				if (!e.IsBeforeChanging)
				{
					e.NewValue = new Dictionary<GoObject, PointF[]>(myPathsHashtable);
				}
				break;
			default:
				base.CopyNewValueForRedo(e);
				break;
			}
		}

		/// <summary>
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2718:
				State = (GoSubGraphState)e.GetInt(undo);
				break;
			case 2702:
				Label = (GoText)e.GetValue(undo);
				break;
			case 2703:
				Collapsible = (bool)e.GetValue(undo);
				break;
			case 2704:
				BackgroundColor = (Color)e.GetValue(undo);
				break;
			case 2705:
				Opacity = e.GetFloat(undo);
				break;
			case 2706:
				LabelSpot = e.GetInt(undo);
				break;
			case 2707:
				TopLeftMargin = e.GetSize(undo);
				break;
			case 2712:
				BottomRightMargin = e.GetSize(undo);
				break;
			case 2713:
				CollapsedTopLeftMargin = e.GetSize(undo);
				break;
			case 2714:
				CollapsedBottomRightMargin = e.GetSize(undo);
				break;
			case 2708:
			{
				object value = e.GetValue(undo);
				if (value is Pen)
				{
					BorderPen = (Pen)value;
				}
				else if (value is GoShape.GoPenInfo)
				{
					BorderPenInfo = (GoShape.GoPenInfo)value;
				}
				break;
			}
			case 2710:
				Corner = e.GetSize(undo);
				break;
			case 2715:
				CollapsedCorner = e.GetSize(undo);
				break;
			case 2711:
				Port = (GoPort)e.GetValue(undo);
				break;
			case 2716:
				CollapsedLabelSpot = e.GetInt(undo);
				break;
			case 2717:
				CollapsedObject = (GoObject)e.GetValue(undo);
				break;
			case 2719:
			{
				Dictionary<GoObject, RectangleF> dictionary2 = (Dictionary<GoObject, RectangleF>)e.GetValue(undo);
				if (dictionary2 != null)
				{
					myBoundsHashtable = new Dictionary<GoObject, RectangleF>(dictionary2);
				}
				else
				{
					myBoundsHashtable.Clear();
				}
				break;
			}
			case 2720:
			{
				Dictionary<GoObject, PointF[]> dictionary = (Dictionary<GoObject, PointF[]>)e.GetValue(undo);
				if (dictionary != null)
				{
					myPathsHashtable = new Dictionary<GoObject, PointF[]>(dictionary);
				}
				else
				{
					myPathsHashtable.Clear();
				}
				break;
			}
			case 2721:
				WasExpanded = (bool)e.GetValue(undo);
				break;
			case 2722:
				ExpandedResizable = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
