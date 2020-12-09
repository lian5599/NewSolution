using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This class implements the rectangle shown and dragged around in the overview window.
	/// It is also responsible for keeping track of changes to the view, so that it can resize itself.
	/// </summary>
	[Serializable]
	public class GoOverviewRectangle : GoRectangle
	{
		/// <summary>
		/// Gets the view whose bounds this rectangle is representing in the overview.
		/// </summary>
		public GoView ObservedView => (base.View as GoOverview)?.Observed;

		/// <summary>
		/// Create a <see cref="T:Northwoods.Go.GoRectangle" /> that knows about the view that it represents.
		/// </summary>
		/// <remarks>
		/// The overview rectangle must not be <see cref="P:Northwoods.Go.GoObject.Reshapable" />.
		/// </remarks>
		public GoOverviewRectangle()
		{
			ResizesRealtime = true;
			Reshapable = false;
			PenColor = Color.DarkCyan;
			PenWidth = 0f;
		}

		/// <summary>
		/// Make this GoRectangle's position and size correspond to the
		/// observed view's position and size in the document
		/// </summary>
		/// <remarks>
		/// This method also scrolls this overview window, if needed,
		/// to make the rectangle visible.
		/// This method should be a no-op when <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true.
		/// </remarks>
		public virtual void UpdateRectFromView()
		{
			GoView observedView = ObservedView;
			if (observedView != null && !base.Initializing)
			{
				base.Initializing = true;
				Bounds = observedView.DocExtent;
				if (base.View != null)
				{
					base.View.ScrollRectangleToVisible(Bounds);
				}
				base.Initializing = false;
			}
		}

		/// <summary>
		/// Treat this rectangle as being hollow--the user can only pick the rectangle when close to the edge.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool ContainsPoint(PointF p)
		{
			GoView view = base.View;
			if (view == null)
			{
				return false;
			}
			RectangleF a = Bounds;
			float num = 4f / view.DocScale;
			GoObject.InflateRect(ref a, num, num);
			if (!GoObject.ContainsRect(a, p))
			{
				return false;
			}
			GoObject.InflateRect(ref a, -2f * num, -2f * num);
			return !GoObject.ContainsRect(a, p);
		}

		/// <summary>
		/// Limit where this rectangle can be dragged, to avoid misleading the user
		/// into believing they could scroll even futher.
		/// </summary>
		/// <param name="origLoc"></param>
		/// <param name="newLoc"></param>
		/// <returns></returns>
		public override PointF ComputeMove(PointF origLoc, PointF newLoc)
		{
			GoView observedView = ObservedView;
			if (observedView == null)
			{
				return newLoc;
			}
			PointF documentTopLeft = observedView.DocumentTopLeft;
			SizeF documentSize = observedView.DocumentSize;
			if (newLoc.X + base.Width > documentTopLeft.X + documentSize.Width)
			{
				newLoc.X = documentTopLeft.X + documentSize.Width - base.Width;
			}
			if (newLoc.X < documentTopLeft.X)
			{
				newLoc.X = documentTopLeft.X;
			}
			if (newLoc.Y + base.Height > documentTopLeft.Y + documentSize.Height)
			{
				newLoc.Y = documentTopLeft.Y + documentSize.Height - base.Height;
			}
			if (newLoc.Y < documentTopLeft.Y)
			{
				newLoc.Y = documentTopLeft.Y;
			}
			if (!observedView.ShowsNegativeCoordinates)
			{
				if (newLoc.X < 0f)
				{
					newLoc.X = 0f;
				}
				if (newLoc.Y < 0f)
				{
					newLoc.Y = 0f;
				}
			}
			return newLoc;
		}

		/// <summary>
		/// As the user drags this rectangle around, change the observed view's
		/// DocPosition property.
		/// </summary>
		/// <param name="old"></param>
		/// <remarks>
		/// This basically just does <c>ObservedView.DocPosition = Position</c>,
		/// although it can also set the ObservedView.DocScale property
		/// when this rectangle's size changes.
		/// It ignores changes caused by a change in the observed view
		/// by not changing the ObservedView's DocPosition or DocScale when
		/// <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true.
		/// </remarks>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			GoView observedView = ObservedView;
			if (observedView == null)
			{
				return;
			}
			if (base.View != null)
			{
				AddSelectionHandles(base.View.Selection, this);
			}
			if (!base.Initializing)
			{
				base.Initializing = true;
				observedView.DocPosition = base.Position;
				if (old.Width != base.Width || old.Height != base.Height)
				{
					Size size = observedView.DisplayRectangle.Size;
					observedView.DocScale = Math.Min((float)size.Width / base.Width, (float)size.Height / base.Height);
				}
				base.Initializing = false;
			}
		}

		/// <summary>
		/// Display a "move" cursor when the mouse is over the edge of this rectangle.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public override string GetCursorName(GoView view)
		{
			return "move";
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoOverviewRectangle.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <param name="sel"></param>
		public override void OnGotSelection(GoSelection sel)
		{
			AddSelectionHandles(sel, this);
		}

		/// <summary>
		/// The overview rectangle should only appear selected if the <see cref="T:Northwoods.Go.GoOverview" />
		/// supports resizing (i.e. <see cref="P:Northwoods.Go.GoView.AllowSelect" /> and <see cref="P:Northwoods.Go.GoView.AllowResize" /> are true),
		/// and even then the handles will not be seen since their <see cref="T:Northwoods.Go.GoHandle" />.<see cref="T:Northwoods.Go.GoHandleStyle" />
		/// is <see cref="F:Northwoods.Go.GoHandleStyle.None" />.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			GoView view = sel.View;
			if (view != null && view.CanSelectObjects() && view.CanResizeObjects())
			{
				view.ResizeHandleSize = new SizeF(4f / view.DocScale, 4f / view.DocScale);
				RemoveSelectionHandles(sel);
				RectangleF bounds = Bounds;
				GoHandle goHandle = sel.CreateResizeHandle(this, selectedObj, new PointF(bounds.Left, bounds.Top), 2, filled: true) as GoHandle;
				if (goHandle != null)
				{
					goHandle.Style = GoHandleStyle.None;
				}
				goHandle = (sel.CreateResizeHandle(this, selectedObj, new PointF(bounds.Right, bounds.Top), 4, filled: true) as GoHandle);
				if (goHandle != null)
				{
					goHandle.Style = GoHandleStyle.None;
				}
				goHandle = (sel.CreateResizeHandle(this, selectedObj, new PointF(bounds.Right, bounds.Bottom), 8, filled: true) as GoHandle);
				if (goHandle != null)
				{
					goHandle.Style = GoHandleStyle.None;
				}
				goHandle = (sel.CreateResizeHandle(this, selectedObj, new PointF(bounds.Left, bounds.Bottom), 16, filled: true) as GoHandle);
				if (goHandle != null)
				{
					goHandle.Style = GoHandleStyle.None;
				}
			}
		}
	}
}
