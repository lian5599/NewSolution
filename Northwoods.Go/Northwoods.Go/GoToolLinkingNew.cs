using System;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to handle a user's drawing a new link between two ports.
	/// </summary>
	/// <remarks>
	/// An instance of this modeless tool is in the default
	/// <see cref="T:Northwoods.Go.GoView" />'s <see cref="P:Northwoods.Go.GoView.MouseMoveTools" /> list.
	/// In other words, this tool is not invoked until the user starts a
	/// dragging gesture with the mouse.
	/// </remarks>
	[Serializable]
	public class GoToolLinkingNew : GoToolLinking
	{
		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		public GoToolLinkingNew(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// The user can draw a new link if the view allows it
		/// and if the port at the input event point is a valid
		/// source port or a valid destination port.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoToolLinking.PickPort(System.Drawing.PointF)" /> to find a port
		/// at the mouse down point.  At least one of the
		/// <see cref="M:Northwoods.Go.GoToolLinking.IsValidFromPort(Northwoods.Go.IGoPort)" /> and
		/// <see cref="M:Northwoods.Go.GoToolLinking.IsValidToPort(Northwoods.Go.IGoPort)" /> predicates
		/// must be true for the linking to start.
		/// This sets <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> property for
		/// initializing this tool before the call to <see cref="M:Northwoods.Go.GoToolLinkingNew.Start" />.
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
			IGoPort goPort2 = base.OriginalStartPort = PickPort(base.FirstInput.DocPoint);
			if (goPort2 != null)
			{
				if (!IsValidFromPort(goPort2))
				{
					return IsValidToPort(goPort2);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// This is just a matter of calling <see cref="M:Northwoods.Go.GoToolLinking.StartNewLink(Northwoods.Go.IGoPort,System.Drawing.PointF)" />.
		/// </summary>
		/// <remarks>
		/// This assumes that the <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> property
		/// has been set, as is normally done by <see cref="M:Northwoods.Go.GoToolLinkingNew.CanStart" />.
		/// </remarks>
		public override void Start()
		{
			base.Start();
			StartNewLink(base.OriginalStartPort, base.LastInput.DocPoint);
		}
	}
}
