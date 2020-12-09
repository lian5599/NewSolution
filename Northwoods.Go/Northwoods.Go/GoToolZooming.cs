using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This tool handles a user's drag in the background to draw a rubber-band box
	/// to specify a new document position and scale for a view.
	/// </summary>
	/// <remarks>
	/// The <see cref="P:Northwoods.Go.GoToolZooming.ZoomedView" /> value is the <see cref="T:Northwoods.Go.GoView" /> that is
	/// manipulated by this tool.  The rubber-band box that the user can draw is
	/// constrained to have the same aspect ratio as the <see cref="P:Northwoods.Go.GoToolZooming.ZoomedView" />'s
	/// <see cref="P:Northwoods.Go.GoView.DisplayRectangle" />.
	/// </remarks>
	[Serializable]
	public class GoToolZooming : GoToolRubberBanding
	{
		[NonSerialized]
		private GoView myZoomedView;

		/// <summary>
		/// Gets the view whose aspect ratio we want to maintain when drawing a zoom region,
		/// and whose document position and scale will be adjusted on a mouse up.
		/// </summary>
		/// <value>
		/// The initial value is the same as <see cref="P:Northwoods.Go.GoTool.View" />.
		/// </value>
		public GoView ZoomedView
		{
			get
			{
				return myZoomedView;
			}
			set
			{
				myZoomedView = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolZooming(GoView v)
			: base(v)
		{
			myZoomedView = v;
		}

		/// <summary>
		/// Allow this tool to start if the user isn't using the context button
		/// and if the mouse isn't over an object in the document.
		/// </summary>
		/// <returns></returns>
		public override bool CanStart()
		{
			if (base.FirstInput.IsContextButton)
			{
				return false;
			}
			return base.View.PickObject(doc: true, view: false, base.FirstInput.DocPoint, selectableOnly: true) == null;
		}

		/// <summary>
		/// Make the box keep the aspect ratio of the observed view.
		/// </summary>
		/// <returns></returns>
		public override Rectangle ComputeRubberBandBox()
		{
			Point viewPoint = base.FirstInput.ViewPoint;
			Point viewPoint2 = base.LastInput.ViewPoint;
			checked
			{
				int num = viewPoint2.X - viewPoint.X;
				int num2 = viewPoint2.Y - viewPoint.Y;
				GoView zoomedView = ZoomedView;
				if (zoomedView == null || zoomedView.DisplayRectangle.Height == 0 || num2 == 0)
				{
					return new Rectangle(Math.Min(viewPoint2.X, viewPoint.X), Math.Min(viewPoint2.Y, viewPoint.Y), Math.Abs(viewPoint2.X - viewPoint.X), Math.Abs(viewPoint2.Y - viewPoint.Y));
				}
				Rectangle displayRectangle = zoomedView.DisplayRectangle;
				float num3 = (float)displayRectangle.Width / (float)displayRectangle.Height;
				int num4;
				int num5;
				if (Math.Abs((float)num / (float)num2) < num3)
				{
					num4 = viewPoint.X + num;
					num5 = viewPoint.Y + (int)Math.Ceiling((float)Math.Abs(num) / num3) * ((num2 >= 0) ? 1 : (-1));
				}
				else
				{
					num4 = viewPoint.X + (int)Math.Ceiling((float)Math.Abs(num2) * num3) * ((num >= 0) ? 1 : (-1));
					num5 = viewPoint.Y + num2;
				}
				return new Rectangle(Math.Min(num4, viewPoint.X), Math.Min(num5, viewPoint.Y), Math.Abs(num4 - viewPoint.X), Math.Abs(num5 - viewPoint.Y));
			}
		}

		/// <summary>
		/// Instead of selecting objects within a rectangle, change the <see cref="P:Northwoods.Go.GoToolZooming.ZoomedView" />'s
		/// <see cref="P:Northwoods.Go.GoView.DocPosition" /> and <see cref="P:Northwoods.Go.GoView.DocScale" /> to match the
		/// given <paramref name="box" /> within this view.
		/// </summary>
		/// <param name="box">
		/// a rectangle whose aspect ratio matches the <see cref="P:Northwoods.Go.GoToolZooming.ZoomedView" />'s;
		/// if the width and height are less than the <see cref="P:Northwoods.Go.GoTool.DragSize" />,
		/// this method does nothing.
		/// </param>
		public override void DoRubberBand(Rectangle box)
		{
			Size dragSize = GoTool.DragSize;
			if (box.Width >= dragSize.Width || box.Height >= dragSize.Height)
			{
				GoView zoomedView = ZoomedView;
				if (zoomedView != null)
				{
					RectangleF rectangleF = base.View.ConvertViewToDoc(box);
					zoomedView.DocScale = (float)zoomedView.DisplayRectangle.Width / rectangleF.Width;
					zoomedView.DocPosition = new PointF(rectangleF.X, rectangleF.Y);
				}
			}
		}
	}
}
