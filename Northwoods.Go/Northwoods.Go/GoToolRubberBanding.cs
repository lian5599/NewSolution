using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to handle a user's background drag to do a multiple selection.
	/// </summary>
	/// <remarks>
	/// No transaction is performed by this tool, although it is possible
	/// (but unconventional) that <see cref="M:Northwoods.Go.GoToolRubberBanding.DoRubberBand(System.Drawing.Rectangle)" /> might be
	/// overridden to perform one.
	/// This tool is normally used as a modeless tool, one of the view's mouse tools,
	/// that can be started upon a mouse move (<see cref="P:Northwoods.Go.GoView.MouseMoveTools" />).
	/// When the <see cref="P:Northwoods.Go.GoToolRubberBanding.Modal" /> property is set to true,
	/// this tool waits for a mouse down at which <see cref="M:Northwoods.Go.GoToolRubberBanding.CanStart" />
	/// returns true before drawing the rubber-band box.
	/// </remarks>
	[Serializable]
	public class GoToolRubberBanding : GoTool
	{
		private bool myModal;

		private bool myAutoScrolling;

		[NonSerialized]
		private Rectangle myBox;

		[NonSerialized]
		private bool myActive;

		/// <summary>
		/// Gets or sets the rectangle that the user has drawn so far.
		/// </summary>
		/// <value>
		/// This <c>Rectangle</c> is in view coordinates.
		/// You should call <see cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Rectangle)" /> to convert
		/// these view coordinates into document coordinates so that you can
		/// select any objects within the rectangle corresponding to this box.
		/// It is initially a zero size rectangle at the mouse down point.
		/// </value>
		/// <remarks>
		/// This is normally set to the value last computed by <see cref="M:Northwoods.Go.GoToolRubberBanding.ComputeRubberBandBox" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoToolRubberBanding.DoRubberBand(System.Drawing.Rectangle)" />
		public Rectangle Box
		{
			get
			{
				return myBox;
			}
			set
			{
				myBox = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this tool should wait for a mouse-down before
		/// drawing a rubber-band box.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		public virtual bool Modal
		{
			get
			{
				return myModal;
			}
			set
			{
				myModal = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this tool is in the process of drawing a
		/// rubber-band box.
		/// </summary>
		/// <value>
		/// The value is set to true in <see cref="M:Northwoods.Go.GoToolRubberBanding.DoMouseDown" /> if <see cref="M:Northwoods.Go.GoToolRubberBanding.CanStart" /> is true,
		/// and in <see cref="M:Northwoods.Go.GoToolRubberBanding.DoMouseMove" /> if <see cref="P:Northwoods.Go.GoToolRubberBanding.Modal" /> is false.
		/// </value>
		public bool Active
		{
			get
			{
				return myActive;
			}
			set
			{
				myActive = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this tool automatically scrolls the view when the
		/// mouse is near the edge, thus extending the size of the <see cref="P:Northwoods.Go.GoToolRubberBanding.Box" />.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// Setting this to true also sets <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DrawsXorMode" /> to false
		/// to avoid having the XOR-mode lines mess up other controls.
		/// </remarks>
		public virtual bool AutoScrolling
		{
			get
			{
				return myAutoScrolling;
			}
			set
			{
				myAutoScrolling = value;
				if (value && base.View != null)
				{
					base.View.DrawsXorMode = false;
				}
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolRubberBanding(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// This tool can start if the user can select objects in this view and the
		/// input event point is not over a selectable document object.
		/// </summary>
		/// <returns></returns>
		public override bool CanStart()
		{
			if (!base.View.CanSelectObjects())
			{
				return false;
			}
			if (base.LastInput.IsContextButton)
			{
				return false;
			}
			if (!IsBeyondDragSize())
			{
				return false;
			}
			return base.View.PickObject(doc: true, view: false, base.FirstInput.DocPoint, selectableOnly: true) == null;
		}

		/// <summary>
		/// Remove the rubber band box from the view.
		/// </summary>
		public override void Stop()
		{
			base.View.DrawXorBox(Box, drawnew: false);
			base.View.StopAutoScroll();
			Active = false;
		}

		private void Activate()
		{
			Active = true;
			Box = new Rectangle(base.FirstInput.ViewPoint.X, base.FirstInput.ViewPoint.Y, 0, 0);
			if (!base.FirstInput.Shift && !base.Selection.IsEmpty)
			{
				base.Selection.Clear();
				base.View.Refresh();
			}
		}

		/// <summary>
		/// This starts keeping track of the <see cref="P:Northwoods.Go.GoToolRubberBanding.Box" />'s bounds.
		/// </summary>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.GoToolRubberBanding.CanStart" /> is true, this sets <see cref="P:Northwoods.Go.GoToolRubberBanding.Active" /> to true.
		/// Unless the <see cref="P:Northwoods.Go.GoInputEventArgs.Shift" /> modifier is true,
		/// we also clear the view's selection.
		/// </remarks>
		public override void DoMouseDown()
		{
			if (CanStart())
			{
				Activate();
			}
		}

		/// <summary>
		/// As the mouse is dragged, we display the rubber band box.
		/// </summary>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoToolRubberBanding.Modal" /> is true, we skip all mouse moves and mouse ups
		/// until a mouse down occurs when <see cref="M:Northwoods.Go.GoToolRubberBanding.CanStart" /> returns true.
		/// If <see cref="P:Northwoods.Go.GoToolRubberBanding.Modal" /> is false, a mouse move sets <see cref="P:Northwoods.Go.GoToolRubberBanding.Active" />
		/// to true and starts keeping track of the rubber-band <see cref="P:Northwoods.Go.GoToolRubberBanding.Box" />.
		/// </remarks>
		public override void DoMouseMove()
		{
			if (!Active)
			{
				if (!Modal)
				{
					Activate();
				}
				return;
			}
			Box = ComputeRubberBandBox();
			base.View.DrawXorBox(Box, drawnew: true);
			if (AutoScrolling)
			{
				base.View.DoAutoScroll(base.LastInput.ViewPoint);
			}
		}

		/// <summary>
		/// When the mouse is released, we remove the rubber band box, call <see cref="M:Northwoods.Go.GoToolRubberBanding.DoRubberBand(System.Drawing.Rectangle)" />,
		/// and stop this tool if <see cref="P:Northwoods.Go.GoToolRubberBanding.Modal" /> is false.
		/// </summary>
		public override void DoMouseUp()
		{
			if (Active)
			{
				Box = ComputeRubberBandBox();
				DoRubberBand(Box);
			}
			StopTool();
		}

		/// <summary>
		/// This method is called to compute the latest bounds of the <see cref="P:Northwoods.Go.GoToolRubberBanding.Box" />.
		/// </summary>
		/// <returns>a <c>Rectangle</c> in view coordinates</returns>
		public virtual Rectangle ComputeRubberBandBox()
		{
			PointF docPoint = base.FirstInput.DocPoint;
			PointF docPoint2 = base.LastInput.DocPoint;
			RectangleF r = new RectangleF(Math.Min(docPoint2.X, docPoint.X), Math.Min(docPoint2.Y, docPoint.Y), Math.Abs(docPoint2.X - docPoint.X), Math.Abs(docPoint2.Y - docPoint.Y));
			return base.View.ConvertDocToView(r);
		}

		/// <summary>
		/// This method is called as part of the mouse up event, normally to select
		/// the objects within the <paramref name="box" />.
		/// </summary>
		/// <param name="box">a <c>Rectangle</c> describing what the user outlined, in view coordinates</param>
		/// <remarks>
		/// By default this will call <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />, after converting
		/// the <paramref name="box" /> into document coordinates.
		/// If the box is too small in width and height, this acts like a normal mouse click instead.
		/// </remarks>
		public virtual void DoRubberBand(Rectangle box)
		{
			if (!IsBeyondDragSize())
			{
				DoSelect(base.LastInput);
				DoClick(base.LastInput);
			}
			else
			{
				RectangleF rect = base.View.ConvertViewToDoc(box);
				base.View.SelectInRectangle(rect);
			}
		}
	}
}
