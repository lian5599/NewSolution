using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Northwoods.Go
{
	/// <summary>
	/// Provide a reduced-scale view of a document, showing the size and position of
	/// another view's viewport onto that same document, and support panning and
	/// zooming of that observed view.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The user can drag around the rectangle representing the observed view's
	/// viewport in order to scroll it and can resize it to change the scale of
	/// the observed view.  Clicking will move the observed view's viewport to
	/// that location in the view's document.  Doing a rubber-band drag will change
	/// the observed view's position and scale to match the box that was drawn,
	/// because by default it replaces the standard <see cref="T:Northwoods.Go.GoToolRubberBanding" />
	/// tool with an instance of <see cref="T:Northwoods.Go.GoToolZooming" />.
	/// </para>
	/// <para>
	/// The rectangle displaying the <see cref="P:Northwoods.Go.GoOverview.Observed" /> <see cref="T:Northwoods.Go.GoView" />'s
	/// <see cref="P:Northwoods.Go.GoView.DocExtent" /> is implemented by a <see cref="T:Northwoods.Go.GoOverviewRectangle" />,
	/// created by the <see cref="M:Northwoods.Go.GoOverview.CreateOverviewRectangle(Northwoods.Go.GoView)" /> method, and
	/// accessible by the <see cref="P:Northwoods.Go.GoOverview.OverviewRect" /> property.
	/// </para>
	/// <para>
	/// Most of the document-modifying-ability properties have been turned off:
	/// <see cref="P:Northwoods.Go.GoView.AllowCopy" />, <see cref="P:Northwoods.Go.GoView.AllowDelete" />,
	/// <see cref="P:Northwoods.Go.GoView.AllowInsert" />, <see cref="P:Northwoods.Go.GoView.AllowLink" />,
	/// <see cref="P:Northwoods.Go.GoView.AllowEdit" />, <see cref="P:Northwoods.Go.GoView.AllowDragOut" />,
	/// and <b>AllowDrop</b>.
	/// The user cannot pick any document object.
	/// The only pickable object is the <see cref="P:Northwoods.Go.GoOverview.OverviewRect" />, which is in
	/// this overview's default layer.
	/// Grids are also disabled for this view.
	/// </para>
	/// </remarks>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(GoOverview), "Northwoods.Go.GoOverview.bmp")]
	public class GoOverview : GoView
	{
		private GoView myObserved;

		private GoDocument myObservedDocument;

		private GoOverviewRectangle myOverviewRect;

		private GoToolZooming myZoomTool;

		[NonSerialized]
		private GoChangedEventHandler myDocChangedEventHandler;

		[NonSerialized]
		private EventHandler myViewResizedEventHandler;

		[NonSerialized]
		private PropertyChangedEventHandler myViewPropertyChangedEventHandler;

		/// <summary>
		/// Gets or sets the view that this overview is watching.
		/// </summary>
		/// <value>
		/// This property should not be set to itself or another GoOverview.
		/// </value>
		/// <remarks>
		/// This overview is useless until it has a <see cref="T:Northwoods.Go.GoView" /> to observe.
		/// When this property is set, this overview becomes a document <c>Changed</c>
		/// event handler for the observed view's document so that it can display
		/// that document.
		/// It also becomes a <c>PropertyChanged</c> event handler and a <c>Resize</c>
		/// event handler for the observed view so that it can track the observed
		/// view's extent (position and size) in its document, as well as any
		/// replacement of the observed view's document.
		/// </remarks>
		/// <seealso cref="T:Northwoods.Go.GoOverviewRectangle" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoView Observed
		{
			get
			{
				return myObserved;
			}
			set
			{
				if (value != this && !(value is GoOverview) && myObserved != value)
				{
					RemoveListeners();
					foreach (GoLayer layer in base.Layers)
					{
						RemoveAllGoControls(layer, remove: true);
					}
					myObserved = value;
					if (myObserved != null)
					{
						myZoomTool.ZoomedView = myObserved;
						myObservedDocument = myObserved.Document;
						AddListeners();
					}
					else
					{
						myZoomTool.ZoomedView = this;
						myObservedDocument = null;
						myOverviewRect = null;
					}
					InitializeLayersFromDocument();
					UpdateView();
					RaisePropertyChangedEvent("Observed");
				}
			}
		}

		/// <summary>
		/// No <see cref="P:Northwoods.Go.GoView.Grid" /> for an overview window.
		/// </summary>
		public override GoViewGridStyle GridStyle
		{
			get
			{
				return GoViewGridStyle.None;
			}
			set
			{
				throw new InvalidOperationException("GoOverview does not support GoView.Grid");
			}
		}

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoOverviewRectangle" /> representing the observed
		/// view's extent in its document.
		/// </summary>
		/// <remarks>
		/// This is the rectangle in this view that the user drags to
		/// change the <see cref="P:Northwoods.Go.GoView.DocPosition" /> of the observed view.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoOverview.Observed" />
		/// <seealso cref="M:Northwoods.Go.GoOverview.CreateOverviewRectangle(Northwoods.Go.GoView)" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GoOverviewRectangle OverviewRect => myOverviewRect;

		/// <summary>
		/// Overridden to change the default value to be SmoothingMode.AntiAlias.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(SmoothingMode.AntiAlias)]
		[Description("How nicely lines are drawn")]
		public override SmoothingMode SmoothingMode
		{
			get
			{
				return base.SmoothingMode;
			}
			set
			{
				base.SmoothingMode = value;
			}
		}

		/// <summary>
		/// Overridden to change the default value to be TextRenderingHint.AntiAlias.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(TextRenderingHint.AntiAlias)]
		[Description("How nicely text is rendered")]
		public override TextRenderingHint TextRenderingHint
		{
			get
			{
				return base.TextRenderingHint;
			}
			set
			{
				base.TextRenderingHint = value;
			}
		}

		/// <summary>
		/// Overridden to change the default value to be InterpolationMode.High.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(InterpolationMode.High)]
		[Description("How images are rendered when scaled or stretched")]
		public override InterpolationMode InterpolationMode
		{
			get
			{
				return base.InterpolationMode;
			}
			set
			{
				base.InterpolationMode = value;
			}
		}

		/// <summary>
		/// Overridden to change the default value to be CompositingQuality.AssumeLinear.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(CompositingQuality.AssumeLinear)]
		[Description("How pixels are composited")]
		public override CompositingQuality CompositingQuality
		{
			get
			{
				return base.CompositingQuality;
			}
			set
			{
				base.CompositingQuality = value;
			}
		}

		/// <summary>
		/// Overridden to change the default value to be PixelOffsetMode.HighSpeed.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(PixelOffsetMode.HighSpeed)]
		[Description("How pixels are offset")]
		public override PixelOffsetMode PixelOffsetMode
		{
			get
			{
				return base.PixelOffsetMode;
			}
			set
			{
				base.PixelOffsetMode = value;
			}
		}

		/// <summary>
		/// Make this view think the observed view's document is actually its own.
		/// </summary>
		/// <remarks>
		/// Setting this property is not useful except for changing the document
		/// that is shown when there is no <see cref="P:Northwoods.Go.GoOverview.Observed" /> view.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override GoDocument Document
		{
			get
			{
				if (myObservedDocument != null)
				{
					return myObservedDocument;
				}
				return base.Document;
			}
			set
			{
				base.Document = value;
			}
		}

		/// <summary>
		/// This should just track what the observed view shows.
		/// </summary>
		public override bool ShowsNegativeCoordinates
		{
			get
			{
				if (Observed != null)
				{
					return Observed.ShowsNegativeCoordinates;
				}
				return false;
			}
			set
			{
				base.ShowsNegativeCoordinates = value;
			}
		}

		/// <summary>
		/// Create a <see cref="T:Northwoods.Go.GoOverview" /> window capable of displaying the position
		/// of a different <see cref="T:Northwoods.Go.GoView" /> in its <see cref="T:Northwoods.Go.GoDocument" />.
		/// </summary>
		/// <remarks>
		/// You need to set the <see cref="P:Northwoods.Go.GoOverview.Observed" /> property to make this
		/// overview <c>Control</c> useful.
		/// </remarks>
		public GoOverview()
		{
			myZoomTool = new GoToolZooming(this);
			ReplaceMouseTool(typeof(GoToolRubberBanding), myZoomTool);
			base.AllowCopy = false;
			base.AllowDelete = false;
			base.AllowInsert = false;
			base.AllowLink = false;
			base.AllowEdit = false;
			base.AllowDragOut = false;
			InitAllowDrop(dnd: false);
			DragsRealtime = true;
			SmoothingMode = SmoothingMode.AntiAlias;
			TextRenderingHint = TextRenderingHint.AntiAlias;
			InterpolationMode = InterpolationMode.High;
			CompositingQuality = CompositingQuality.AssumeLinear;
			PixelOffsetMode = PixelOffsetMode.HighSpeed;
			DocScale = 0.125f;
		}

		/// <summary>
		/// Remove any event handlers from the <see cref="P:Northwoods.Go.GoOverview.Observed" /> view and document.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			RemoveListeners();
			myObserved = null;
		}

		private void AddListeners()
		{
			if (myDocChangedEventHandler == null)
			{
				myDocChangedEventHandler = base.SafeOnDocumentChanged;
				myViewResizedEventHandler = ComponentResized;
				myViewPropertyChangedEventHandler = ViewChanged;
			}
			if (myObservedDocument != null)
			{
				myObservedDocument.Changed += myDocChangedEventHandler;
			}
			if (myObserved != null)
			{
				myObserved.Resize += myViewResizedEventHandler;
				myObserved.PropertyChanged += myViewPropertyChangedEventHandler;
			}
		}

		private void RemoveListeners()
		{
			if (myObservedDocument != null)
			{
				myObservedDocument.Changed -= myDocChangedEventHandler;
			}
			if (myObserved != null)
			{
				myObserved.Resize -= myViewResizedEventHandler;
				myObserved.PropertyChanged -= myViewPropertyChangedEventHandler;
			}
		}

		/// <summary>
		/// Initialize the layers of this view.
		/// </summary>
		/// <remarks>
		/// This method makes sure this overview's layers are the same as that of its document.
		/// It also adds the result of <see cref="M:Northwoods.Go.GoOverview.CreateOverviewRectangle(Northwoods.Go.GoView)" /> to this
		/// view's default layer.
		/// </remarks>
		public override void InitializeLayersFromDocument()
		{
			base.InitializeLayersFromDocument();
			if (Observed != null)
			{
				myOverviewRect = CreateOverviewRectangle(Observed);
				myOverviewRect.Bounds = Observed.DocExtent;
				base.Layers.Default.Add(myOverviewRect);
				myOverviewRect.AddSelectionHandles(Selection, myOverviewRect);
			}
		}

		/// <summary>
		/// No <see cref="P:Northwoods.Go.GoView.Grid" /> for an overview window.
		/// </summary>
		/// <returns>null</returns>
		public override GoGrid CreateGrid()
		{
			return null;
		}

		/// <summary>
		/// Create an instance of <see cref="T:Northwoods.Go.GoOverviewRectangle" /> for the
		/// given view.
		/// </summary>
		/// <param name="observed"></param>
		/// <returns>An <see cref="T:Northwoods.Go.GoOverviewRectangle" /> that knows which view's extent it represents</returns>
		public virtual GoOverviewRectangle CreateOverviewRectangle(GoView observed)
		{
			return new GoOverviewRectangle();
		}

		/// <summary>
		/// Don't allow the user to select any objects except the OverviewRect
		/// and other view objects,
		/// even though that rectangle is a view object, not a document object.
		/// </summary>
		/// <param name="doc">ignored, assumed to be false</param>
		/// <param name="view"></param>
		/// <param name="p">a <c>PointF</c> in document coordinates</param>
		/// <param name="selectableOnly">whether to pick only objects that are selectable</param>
		/// <returns></returns>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoOverview.OverviewRect" /> gets picked when the point <paramref name="p" />
		/// is in along the rectangle's bounds, even though the the <see cref="P:Northwoods.Go.GoOverview.OverviewRect" />
		/// is not even a document object.
		/// </remarks>
		public override GoObject PickObject(bool doc, bool view, PointF p, bool selectableOnly)
		{
			return base.PickObject(doc: false, doc || view, p, selectableOnly);
		}

		/// <summary>
		/// Allow mouse clicks not on the OverviewRect, but elsewhere in the
		/// Overview, to cause the OverviewRect to be centered there, or as
		/// near as allowed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnBackgroundSingleClicked(GoInputEventArgs evt)
		{
			base.OnBackgroundSingleClicked(evt);
			if (OverviewRect != null)
			{
				RectangleF bounds = OverviewRect.Bounds;
				PointF newLoc = new PointF(evt.DocPoint.X - bounds.Width / 2f, evt.DocPoint.Y - bounds.Height / 2f);
				OverviewRect.Location = OverviewRect.ComputeMove(OverviewRect.Location, newLoc);
			}
		}

		/// <summary>
		/// Limit mouse over behavior for document objects to just show tooltips.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is basically to support tooltips, which are more valuable when
		/// the objects are so small.  Other mouse-over behavior, including
		/// hover and changing the cursor, is explicitly avoided, except over
		/// the <see cref="P:Northwoods.Go.GoOverview.OverviewRect" /> and other view objects.
		/// </remarks>
		public override bool DoMouseOver(GoInputEventArgs evt)
		{
			GoObject goObject = base.PickObject(doc: false, view: true, evt.DocPoint, selectableOnly: false);
			bool flag = false;
			while (goObject != null)
			{
				if (goObject.OnMouseOver(evt, this))
				{
					flag = true;
					break;
				}
				goObject = goObject.Parent;
			}
			if (!flag)
			{
				DoBackgroundMouseOver(evt);
			}
			goObject = base.PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			DoToolTipObject(goObject);
			return true;
		}

		internal void UpdateOverviewRect()
		{
			GoOverviewRectangle overviewRect = OverviewRect;
			if (overviewRect != null && !overviewRect.Initializing)
			{
				overviewRect.UpdateRectFromView();
			}
		}

		/// <summary>
		/// Handle basic changes to the observed view's DocPosition or DocScale,
		/// or when the observed view's Document got swapped for a different document.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void ViewChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DocPosition" || e.PropertyName == "DocScale")
			{
				UpdateOverviewRect();
			}
			else if (e.PropertyName == "Document" && sender is GoView)
			{
				if (myObservedDocument != null)
				{
					myObservedDocument.Changed -= myDocChangedEventHandler;
				}
				myObservedDocument = ((GoView)sender).Document;
				if (myObservedDocument != null)
				{
					myObservedDocument.Changed += myDocChangedEventHandler;
				}
				InitializeLayersFromDocument();
				UpdateOverviewRect();
			}
		}

		/// <summary>
		/// Handle changes in the observed view's (window) shape by changing the bounds
		/// of the OverviewRect.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void ComponentResized(object sender, EventArgs e)
		{
			UpdateOverviewRect();
		}
	}
}
