using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to handle the user's dragging one end of a link in order
	/// to connect it up to another port.
	/// </summary>
	/// <remarks>
	/// An instance of this tool is in the default
	/// <see cref="T:Northwoods.Go.GoView" />'s <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
	/// </remarks>
	[Serializable]
	public class GoToolRelinking : GoToolLinking
	{
		[NonSerialized]
		private bool mySelectionHidden;

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolRelinking(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// The user can relink if the view allows it and if the handle
		/// found at the input event point has an ID that indicates it
		/// is relinkable.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoToolRelinking.PickRelinkHandle(System.Drawing.PointF)" /> to find a handle.
		/// The <see cref="P:Northwoods.Go.IGoHandle.HandleID" /> should be either
		/// <see cref="F:Northwoods.Go.GoLink.RelinkableFromHandle" /> or
		/// <see cref="F:Northwoods.Go.GoLink.RelinkableToHandle" />.  The ID also
		/// determines which end of the link is disconnected.
		/// This sets <see cref="P:Northwoods.Go.GoToolLinking.Link" /> and <see cref="P:Northwoods.Go.GoToolLinking.Forwards" />
		/// properties for initializing this tool before the call to <see cref="M:Northwoods.Go.GoToolRelinking.Start" />.
		/// </remarks>
		public override bool CanStart()
		{
			if (base.FirstInput.IsContextButton)
			{
				return false;
			}
			if (!base.View.CanLinkObjects())
			{
				return false;
			}
			IGoHandle goHandle = PickRelinkHandle(base.FirstInput.DocPoint);
			if (goHandle == null)
			{
				return false;
			}
			if (goHandle.HandleID == 1024)
			{
				base.CurrentObject = goHandle.HandledObject;
				IGoLink goLink = goHandle.SelectedObject as IGoLink;
				if (goLink is GoLink)
				{
					GoLink goLink2 = (GoLink)goLink;
					if (goLink2.AbstractLink != null)
					{
						goLink = goLink2.AbstractLink;
					}
				}
				if (goLink == null)
				{
					return false;
				}
				base.Link = goLink;
				base.Forwards = false;
				return true;
			}
			if (goHandle.HandleID == 1025)
			{
				base.CurrentObject = goHandle.HandledObject;
				IGoLink goLink3 = goHandle.SelectedObject as IGoLink;
				if (goLink3 is GoLink)
				{
					GoLink goLink4 = (GoLink)goLink3;
					if (goLink4.AbstractLink != null)
					{
						goLink3 = goLink4.AbstractLink;
					}
				}
				if (goLink3 == null)
				{
					return false;
				}
				base.Link = goLink3;
				base.Forwards = true;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Start relinking by by calling <see cref="M:Northwoods.Go.GoToolLinking.StartRelink(Northwoods.Go.IGoLink,System.Boolean,System.Drawing.PointF)" />
		/// and hiding any selection handles for the link.
		/// </summary>
		/// <remarks>
		/// This assumes that <see cref="P:Northwoods.Go.GoToolLinking.Link" /> and <see cref="P:Northwoods.Go.GoToolLinking.Forwards" />
		/// properties have been set, as is normally done by the <see cref="M:Northwoods.Go.GoToolRelinking.CanStart" /> method.
		/// </remarks>
		public override void Start()
		{
			base.Start();
			GoObject currentObject = base.CurrentObject;
			if (currentObject != null && base.Selection.GetHandleCount(currentObject) > 0)
			{
				mySelectionHidden = true;
				currentObject.RemoveSelectionHandles(base.Selection);
			}
			StartRelink(base.Link, base.Forwards, base.LastInput.DocPoint);
		}

		/// <summary>
		/// Find a resize handle at the given point.
		/// </summary>
		/// <param name="dc">a <c>PointF</c> in document coordinates</param>
		/// <returns>an <see cref="T:Northwoods.Go.IGoHandle" /> resize handle</returns>
		public virtual IGoHandle PickRelinkHandle(PointF dc)
		{
			return base.View.PickObject(doc: false, view: true, dc, selectableOnly: true) as IGoHandle;
		}

		/// <summary>
		/// Restore the selection handles on the link.
		/// </summary>
		public override void Stop()
		{
			if (mySelectionHidden)
			{
				mySelectionHidden = false;
				GoObject currentObject = base.CurrentObject;
				if (currentObject != null && currentObject.Document == base.View.Document)
				{
					GoObject goObject = base.Link.GoObject;
					if (!base.Selection.Contains(goObject))
					{
						base.Selection.Add(goObject);
					}
					else
					{
						currentObject.AddSelectionHandles(base.Selection, goObject);
					}
				}
			}
			base.CurrentObject = null;
			base.Stop();
		}
	}
}
