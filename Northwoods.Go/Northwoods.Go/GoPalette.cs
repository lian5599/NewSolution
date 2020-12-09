using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A palette is a view holding a number of objects in a grid that the user
	/// can drag into another document.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The spacing and positioning of the items is controlled by the
	/// <see cref="P:Northwoods.Go.GoView.Grid" />.<see cref="P:Northwoods.Go.GoView.GridCellSizeWidth" /> and
	/// <see cref="P:Northwoods.Go.GoView.GridCellSizeHeight" />
	/// control the spacing between items--smaller values will allow the objects to be
	/// placed closer together without overlapping, but will increase the likelihood of
	/// unaligned columns (or rows) if the objects have different sizes.
	/// <see cref="P:Northwoods.Go.GoView.GridOriginX" /> and <see cref="P:Northwoods.Go.GoView.GridOriginY" />
	/// control the amount of "margin" space at the top and left of the view.
	/// However, if <see cref="P:Northwoods.Go.GoPalette.AlignsSelectionObject" /> is true and the selection objects tend
	/// to be smaller than the items, this margin should be larger to accomodate those
	/// wider item parts (unless you want to allow them to be clipped).
	/// </para>
	/// <para>
	/// You can control whether the objects are sorted, and in what order they
	/// are sorted, by setting the <see cref="P:Northwoods.Go.GoPalette.Sorting" /> property.
	/// You can control the way the objects are laid out in a grid, with the
	/// scrollbar (if needed) either vertical or horizontal, by setting the
	/// <see cref="P:Northwoods.Go.GoPalette.Orientation" /> property.
	/// Because <see cref="M:Northwoods.Go.GoPalette.LayoutItems" /> is designed to take this view's
	/// scale and size into accout, this class is not designed to share its
	/// document with other views.
	/// </para>
	/// <para>
	/// Palettes do not support sheets or <see cref="P:Northwoods.Go.GoView.SheetStyle" />.
	/// Since users are not expected to move or resize any of the objects,
	/// the grid <see cref="P:Northwoods.Go.GoView.GridSnapDrag" /> and <see cref="P:Northwoods.Go.GoView.GridSnapResize" />
	/// behavior is ignored.
	/// </para>
	/// </remarks>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(GoPalette), "Northwoods.Go.GoPalette.bmp")]
	public class GoPalette : GoView
	{
		[Serializable]
		internal sealed class AlphaComparer : IComparer<GoObject>
		{
			internal AlphaComparer()
			{
			}

			public int Compare(GoObject x, GoObject y)
			{
				IGoLabeledPart goLabeledPart = x as IGoLabeledPart;
				IGoLabeledPart goLabeledPart2 = y as IGoLabeledPart;
				if (goLabeledPart != null)
				{
					if (goLabeledPart2 != null)
					{
						return string.Compare(goLabeledPart.Text, goLabeledPart2.Text, ignoreCase: true, CultureInfo.CurrentCulture);
					}
					return 1;
				}
				if (goLabeledPart2 != null)
				{
					return -1;
				}
				return 0;
			}
		}

		internal static readonly IComparer<GoObject> AlphabeticNodeTextComparer = new AlphaComparer();

		private Orientation myOrientation = Orientation.Vertical;

		private SortOrder mySorting = SortOrder.Ascending;

		private IComparer<GoObject> myComparer = AlphabeticNodeTextComparer;

		private bool myAlignsSelectionObject = true;

		private bool myAutomaticLayout = true;

		/// <summary>
		/// SheetStyle is always <c>GoViewSheetStyle.None</c>, and cannot be set.
		/// </summary>
		public override GoViewSheetStyle SheetStyle
		{
			get
			{
				return GoViewSheetStyle.None;
			}
			set
			{
				throw new InvalidOperationException("GoPalette does not support GoView.Sheet");
			}
		}

		/// <summary>
		/// Palettes do not support <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size SheetRoom
		{
			get
			{
				return base.SheetRoom;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets how to fill the palette by positioning its items.
		/// </summary>
		/// <value>
		/// The default value is <c>Orientation.Vertical</c>.
		/// </value>
		/// <remarks>
		/// This property determines whether the automatic layout of the items in
		/// the palette fills and adds rows (<c>Orientation.Vertical</c>) or fills
		/// and adds columns (<c>Orientation.Horizontal</c>).
		/// A vertical orientation means that there is no horizontal scroll bar,
		/// and that the vertical scroll bar shows itself when it is needed because
		/// the rows do not all fit in the display area.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(Orientation.Vertical)]
		[Description("How to fill the palette by positioning its items.")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				if (myOrientation != value)
				{
					myOrientation = value;
					if (value == Orientation.Vertical)
					{
						ShowHorizontalScrollBar = GoViewScrollBarVisibility.Hide;
						ShowVerticalScrollBar = GoViewScrollBarVisibility.IfNeeded;
					}
					else
					{
						ShowHorizontalScrollBar = GoViewScrollBarVisibility.IfNeeded;
						ShowVerticalScrollBar = GoViewScrollBarVisibility.Hide;
					}
					LayoutItems();
					RaisePropertyChangedEvent("Orientation");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to align each item's <see cref="P:Northwoods.Go.GoObject.SelectionObject" />
		/// instead of the whole item.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// A true value causes node icons to be aligned instead of the top-left
		/// corners of the objects.  The node labels, then, are unlikely to be aligned.
		/// To reduce overlap <see cref="M:Northwoods.Go.GoPalette.LayoutItems" /> may skip cells in order to place
		/// an item.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether to grid-align each whole item or each item's SelectionObject")]
		public virtual bool AlignsSelectionObject
		{
			get
			{
				return myAlignsSelectionObject;
			}
			set
			{
				if (myAlignsSelectionObject != value)
				{
					myAlignsSelectionObject = value;
					LayoutItems();
					RaisePropertyChangedEvent("AlignsSelectionObject");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether and how LayoutItems sorts before it positions all of the items.
		/// </summary>
		/// <value>
		/// If the value is not <c>SortOrder.None</c>, <see cref="P:Northwoods.Go.GoPalette.Comparer" /> is used to
		/// determine the sort order.  Otherwise, <see cref="M:Northwoods.Go.GoPalette.LayoutItems" /> places the
		/// palette items in a grid in an order of its own choosing.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(SortOrder.Ascending)]
		[Description("Whether the items in the palette are sorted before being positioned.")]
		public virtual SortOrder Sorting
		{
			get
			{
				return mySorting;
			}
			set
			{
				if (mySorting != value)
				{
					mySorting = value;
					LayoutItems();
					RaisePropertyChangedEvent("Sorting");
				}
			}
		}

		/// <summary>
		/// Gets or sets the way the palette items are compared during sorting.
		/// </summary>
		/// <value>
		/// This defaults to a comparer that compares the <see cref="P:Northwoods.Go.IGoLabeledPart.Text" /> strings.
		/// If the object is not a <see cref="T:Northwoods.Go.IGoLabeledPart" />, the default comparer
		/// compares as if it were null.
		/// Attempting to set this property to null will restore the default comparer.
		/// </value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IComparer<GoObject> Comparer
		{
			get
			{
				return myComparer;
			}
			set
			{
				IComparer<GoObject> comparer = myComparer;
				if (value == null)
				{
					value = AlphabeticNodeTextComparer;
				}
				if (comparer != value)
				{
					myComparer = value;
					LayoutItems();
					RaisePropertyChangedEvent("Comparer");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoPalette.LayoutItems" /> actually positions all the items.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether to automatically position all of the items in a grid")]
		public virtual bool AutomaticLayout
		{
			get
			{
				return myAutomaticLayout;
			}
			set
			{
				if (myAutomaticLayout != value)
				{
					myAutomaticLayout = value;
					LayoutItems();
					RaisePropertyChangedEvent("AutomaticLayout");
				}
			}
		}

		/// <summary>
		/// Create a <see cref="T:Northwoods.Go.GoPalette" /> window that can show a collection of
		/// <see cref="T:Northwoods.Go.GoObject" /> items arranged in a grid that the user can drag from.
		/// </summary>
		/// <remarks>
		/// This kind of view allows no modifications by users, but does allow copying
		/// objects by dragging objects out of the view or through clipboard copy.
		/// The grid cell size defaults to 60 by 60, with an initial origin of 20,5.
		/// </remarks>
		public GoPalette()
		{
			ShowsNegativeCoordinates = false;
			SetModifiable(b: false);
			AutoScrollRegion = default(Size);
			if (InitAllowDrop(dnd: true))
			{
				base.AllowCopy = true;
			}
			else
			{
				base.AllowCopy = false;
			}
			base.GridCellSize = new SizeF(60f, 60f);
			base.GridOrigin = new PointF(20f, 5f);
			(FindMouseTool(typeof(GoToolDragging)) as GoToolDragging).HidesSelectionHandles = false;
		}

		/// <summary>
		/// Palettes do not support <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// </summary>
		/// <returns>null</returns>
		public override GoSheet CreateSheet()
		{
			return null;
		}

		/// <summary>
		/// Position all of this document's objects in a grid, according to the
		/// orientation and sort order.
		/// </summary>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoPalette.Orientation" /> is <c>Orientation.Vertical</c>, for
		/// example, this method will position all of the objects in the document
		/// in the grid, specified by <see cref="P:Northwoods.Go.GoView.GridCellSizeWidth" />,
		/// <see cref="P:Northwoods.Go.GoView.GridCellSizeHeight" />, <see cref="P:Northwoods.Go.GoView.GridOriginX" />,
		/// and <see cref="P:Northwoods.Go.GoView.GridOriginY" />, starting at the top left corner,
		/// and proceeding to fill each row before adding rows.
		/// The order in which document objects are taken as palette items is
		/// determined by the <see cref="P:Northwoods.Go.GoPalette.Sorting" /> and the <see cref="P:Northwoods.Go.GoPalette.Comparer" />
		/// comparison method.
		/// </remarks>
		public virtual void LayoutItems()
		{
			if (!AutomaticLayout)
			{
				return;
			}
			bool flag = Orientation == Orientation.Vertical;
			ICollection<GoObject> collection = Document;
			if (Sorting != 0 && Comparer != null)
			{
				GoObject[] array = Document.CopyArray();
				Array.Sort(array, 0, array.Length, Comparer);
				if (Sorting == SortOrder.Descending)
				{
					Array.Reverse(array, 0, array.Length);
				}
				collection = array;
			}
			SizeF docExtentSize = DocExtentSize;
			SizeF gridCellSize = GridCellSize;
			PointF gridOrigin = GridOrigin;
			bool alignsSelectionObject = AlignsSelectionObject;
			bool flag2 = true;
			PointF pointF = gridOrigin;
			float num = Math.Min(gridOrigin.X, 0f);
			float num2 = Math.Min(gridOrigin.Y, 0f);
			foreach (GoObject item in collection)
			{
				GoObject goObject = item;
				if (alignsSelectionObject)
				{
					goObject = item.SelectionObject;
					if (goObject == null)
					{
						goObject = item;
					}
				}
				goObject.Position = pointF;
				if (flag)
				{
					pointF = ShiftRight(item, goObject, num, pointF, gridCellSize);
					if (!flag2 && item.Right >= docExtentSize.Width)
					{
						num = Math.Min(gridOrigin.X, 0f);
						pointF.X = gridOrigin.X;
						pointF.Y = Math.Max(pointF.Y + gridCellSize.Height, num2);
						goObject.Position = pointF;
						pointF = ShiftRight(item, goObject, num, pointF, gridCellSize);
					}
					pointF.X += gridCellSize.Width;
				}
				else
				{
					pointF = ShiftDown(item, goObject, num2, pointF, gridCellSize);
					if (!flag2 && item.Bottom >= docExtentSize.Height)
					{
						num2 = Math.Min(gridOrigin.Y, 0f);
						pointF.Y = gridOrigin.Y;
						pointF.X = Math.Max(pointF.X + gridCellSize.Width, num);
						goObject.Position = pointF;
						pointF = ShiftDown(item, goObject, num2, pointF, gridCellSize);
					}
					pointF.Y += gridCellSize.Height;
				}
				num = Math.Max(num, item.Right);
				num2 = Math.Max(num2, item.Bottom);
				flag2 = false;
			}
			Document.Bounds = ComputeDocumentBounds();
		}

		private PointF ShiftDown(GoObject obj, GoObject selobj, float maxrow, PointF pnt, SizeF cellsize)
		{
			while (obj.Top < maxrow)
			{
				pnt.Y += cellsize.Height;
				float top = obj.Top;
				selobj.Top = pnt.Y;
				if (obj.Top <= top)
				{
					break;
				}
			}
			return pnt;
		}

		private PointF ShiftRight(GoObject obj, GoObject selobj, float maxcol, PointF pnt, SizeF cellsize)
		{
			while (obj.Left < maxcol)
			{
				pnt.X += cellsize.Width;
				float left = obj.Left;
				selobj.Left = pnt.X;
				if (obj.Left <= left)
				{
					break;
				}
			}
			return pnt;
		}

		/// <summary>
		/// When an object is inserted or removed from the document, call <see cref="M:Northwoods.Go.GoPalette.LayoutItems" />.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnDocumentChanged(object sender, GoChangedEventArgs e)
		{
			base.OnDocumentChanged(sender, e);
			if (e.Hint == 902 || e.Hint == 903)
			{
				LayoutItems();
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoView.Grid" /> changes, call <see cref="M:Northwoods.Go.GoPalette.LayoutItems" />.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="subhint"></param>
		/// <param name="x"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		public override void RaiseChanged(int hint, int subhint, object x, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			base.RaiseChanged(hint, subhint, x, oldI, oldVal, oldRect, newI, newVal, newRect);
			if (x != null && x == Grid && hint == 901)
			{
				switch (subhint)
				{
				case 1802:
				case 1803:
				case 1804:
					LayoutItems();
					break;
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoView.DocScale" /> property changes, call <see cref="M:Northwoods.Go.GoPalette.LayoutItems" />.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnPropertyChanged(PropertyChangedEventArgs evt)
		{
			base.OnPropertyChanged(evt);
			if (evt.PropertyName == "DocScale")
			{
				LayoutItems();
			}
		}

		/// <summary>
		/// When the view is resized, call <see cref="M:Northwoods.Go.GoPalette.LayoutItems" />.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnSizeChanged(EventArgs evt)
		{
			base.OnSizeChanged(evt);
			LayoutItems();
		}
	}
}
