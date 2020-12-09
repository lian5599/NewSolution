using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// The tool used to implement the user's drawing a new link or reconnecting an existing link.
	/// </summary>
	[Serializable]
	public abstract class GoToolLinking : GoTool
	{
		[Serializable]
		internal class GoTemporaryPort : GoPort
		{
			private GoPort myTargetPort;

			public override GoObject PortObject
			{
				get
				{
					if (Target != null)
					{
						return Target.PortObject;
					}
					return base.PortObject;
				}
			}

			public override int FromSpot
			{
				get
				{
					if (Target != null)
					{
						return Target.FromSpot;
					}
					return base.FromSpot;
				}
			}

			public override int ToSpot
			{
				get
				{
					if (Target != null)
					{
						return Target.ToSpot;
					}
					return base.ToSpot;
				}
			}

			public override float EndSegmentLength
			{
				get
				{
					if (Target != null)
					{
						return Target.EndSegmentLength;
					}
					return base.EndSegmentLength;
				}
			}

			internal GoPort Target
			{
				get
				{
					return myTargetPort;
				}
				set
				{
					myTargetPort = value;
				}
			}

			internal GoTemporaryPort()
			{
				PortObject = null;
				FromSpot = 0;
				ToSpot = 0;
				base.Size = default(SizeF);
			}

			public override float GetFromLinkDir(IGoLink link)
			{
				if (Target != null)
				{
					return Target.GetFromLinkDir(link);
				}
				return base.GetFromLinkDir(link);
			}

			public override PointF GetFromLinkPoint(IGoLink link)
			{
				if (Target != null)
				{
					return Target.GetFromLinkPoint(link);
				}
				return base.GetFromLinkPoint(link);
			}

			public override float GetToLinkDir(IGoLink link)
			{
				if (Target != null)
				{
					return Target.GetToLinkDir(link);
				}
				return base.GetToLinkDir(link);
			}

			public override PointF GetToLinkPoint(IGoLink link)
			{
				if (Target != null)
				{
					return Target.GetToLinkPoint(link);
				}
				return base.GetToLinkPoint(link);
			}

			public override PointF GetLinkPointFromPoint(PointF p)
			{
				if (Target != null)
				{
					return Target.GetLinkPointFromPoint(p);
				}
				return base.GetLinkPointFromPoint(p);
			}
		}

		/// <summary>
		/// This value associated with a port in the <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" />
		/// indicates that it is valid to make a link between <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" />
		/// and that port.
		/// </summary>
		/// <value>
		/// <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> calls <see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> with
		/// <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> and each visible port in the document.
		/// If the predicate returned true, the port is associated with this Valid
		/// value in the <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" /> hash table.
		/// </value>
		public static readonly object Valid = "Valid";

		/// <summary>
		/// This value associated with a port in the <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" />
		/// indicates that it is not valid to make a link between <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" />
		/// and that port.
		/// </summary>
		/// <value>
		/// <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> calls <see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> with
		/// <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> and each visible port in the document.
		/// If the predicate returned false, the port is associated with this Invalid
		/// value in the <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" /> hash table.
		/// </value>
		public static readonly object Invalid = "Invalid";

		private bool myForwardsOnly;

		[NonSerialized]
		private bool myLinkingNew = true;

		[NonSerialized]
		private bool myForwards = true;

		[NonSerialized]
		private IGoPort myOrigStartPort;

		[NonSerialized]
		private IGoPort myOrigEndPort;

		[NonSerialized]
		private IGoPort myTempStartPort;

		[NonSerialized]
		private IGoPort myTempEndPort;

		[NonSerialized]
		private IGoLink myTempLink;

		[NonSerialized]
		private Dictionary<IGoPort, object> myValidPortsCache;

		/// <summary>
		/// Gets or sets whether the user's linking operation started at the "From" port.
		/// </summary>
		/// <remarks>
		/// When this property is true, the <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> and
		/// <see cref="P:Northwoods.Go.GoToolLinking.StartPort" /> ports were or are at the "From" end of the
		/// <see cref="P:Northwoods.Go.GoToolLinking.Link" />.
		/// </remarks>
		public bool Forwards
		{
			get
			{
				return myForwards;
			}
			set
			{
				myForwards = value;
			}
		}

		/// <summary>
		/// Gets or sets the port from which the user is starting or modifying a link.
		/// </summary>
		/// <remarks>
		/// When creating a new link, the <see cref="T:Northwoods.Go.GoToolLinkingNew" /> tool sets this
		/// property to the port under the initial mouse point.
		/// When reconnecting an existing link, the <see cref="T:Northwoods.Go.GoToolRelinking" /> tool
		/// sets this property to the port at the other end of the link from the resize
		/// handle that the user is moving.
		/// This will be a port that already existed in the document prior to the
		/// linking operation.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.OriginalEndPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.StartPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.Link" />
		public IGoPort OriginalStartPort
		{
			get
			{
				return myOrigStartPort;
			}
			set
			{
				myOrigStartPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the port at the end of an existing link that is being reconnected.
		/// </summary>
		/// <remarks>
		/// When creating a new link, this property is not relevant.
		/// When reconnecting an existing link, the <see cref="T:Northwoods.Go.GoToolRelinking" /> tool
		/// sets this property to the port at the end of the existing link that the
		/// user is disconnecting from.
		/// This will be a port that already existed in the document prior to the
		/// linking operation.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.EndPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.Link" />
		public IGoPort OriginalEndPort
		{
			get
			{
				return myOrigEndPort;
			}
			set
			{
				myOrigEndPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the temporary starting port.
		/// </summary>
		/// <remarks>
		/// When creating a new link or when reconnecting an existing link, the tool
		/// sets this property to the value of <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryPort(Northwoods.Go.IGoPort,System.Drawing.PointF,System.Boolean,System.Boolean)" />.
		/// This will be a new port that only exists in this view.
		/// If <see cref="P:Northwoods.Go.GoToolLinking.Forwards" /> is true, this port will correspond to
		/// the <see cref="P:Northwoods.Go.IGoLink.FromPort" /> of the link; otherwise it will
		/// correspond to the <see cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.EndPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.Link" />
		public IGoPort StartPort
		{
			get
			{
				return myTempStartPort;
			}
			set
			{
				myTempStartPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the temporary ending port.
		/// </summary>
		/// <remarks>
		/// When creating a new link or when reconnecting an existing link, the tool
		/// sets this property to the value of <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryPort(Northwoods.Go.IGoPort,System.Drawing.PointF,System.Boolean,System.Boolean)" />.
		/// This will be a new port that only exists in this view.
		/// If <see cref="P:Northwoods.Go.GoToolLinking.Forwards" /> is true, this port will correspond to
		/// the <see cref="P:Northwoods.Go.IGoLink.ToPort" /> of the link; otherwise it will
		/// correspond to the <see cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.StartPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.OriginalEndPort" />
		/// <seealso cref="P:Northwoods.Go.GoToolLinking.Link" />
		public IGoPort EndPort
		{
			get
			{
				return myTempEndPort;
			}
			set
			{
				myTempEndPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the link that the user is manipulating for this linking operation.
		/// </summary>
		/// <remarks>
		/// When creating a new link, the <see cref="T:Northwoods.Go.GoToolLinkingNew" /> tool sets this
		/// property to the value of <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />, a new link that
		/// only exists in this view.
		/// When reconnecting an existing link, the <see cref="T:Northwoods.Go.GoToolRelinking" /> tool
		/// sets this property to the existing link in the document.
		/// </remarks>
		public IGoLink Link
		{
			get
			{
				return myTempLink;
			}
			set
			{
				myTempLink = value;
			}
		}

		/// <summary>
		/// Gets the hashtable of all known ports that are valid for this particular linking operation.
		/// </summary>
		/// <remarks>
		/// This collection is initially empty for each linking operation.
		/// As the <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> method is called,
		/// the port is added to this collection, with a value depending on
		/// whether <see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> returns true.
		/// The value is <see cref="F:Northwoods.Go.GoToolLinking.Valid" /> if it returned true,
		/// <see cref="F:Northwoods.Go.GoToolLinking.Invalid" /> if it returned false.
		/// The cacheing is done because the computation to determine valid links can
		/// be expensive.
		/// You can turn off the cacheing by setting this property to null.
		/// </remarks>
		public Dictionary<IGoPort, object> ValidPortsCache
		{
			get
			{
				if (myValidPortsCache == null)
				{
					myValidPortsCache = new Dictionary<IGoPort, object>();
				}
				return myValidPortsCache;
			}
			set
			{
				myValidPortsCache = value;
			}
		}

		/// <summary>
		/// Gets or sets whether users must draw their new links starting at the "from" port
		/// and going to the "to" port.
		/// </summary>
		/// <value>
		/// This value defaults to false, which will allow users to draw links "backwards".
		/// </value>
		public virtual bool ForwardsOnly
		{
			get
			{
				return myForwardsOnly;
			}
			set
			{
				myForwardsOnly = value;
			}
		}

		/// <summary>
		/// The standard tool constructor.
		/// </summary>
		/// <param name="v"></param>
		protected GoToolLinking(GoView v)
			: base(v)
		{
		}

		/// <summary>
		/// Cleaning up from any kind of linking operation involves
		/// removing any temporary ports or link from the view and
		/// stopping the current transaction.
		/// </summary>
		public override void Stop()
		{
			base.View.StopAutoScroll();
			Forwards = true;
			OriginalStartPort = null;
			OriginalEndPort = null;
			if (Link != null)
			{
				GoObject goObject = Link.GoObject;
				if (goObject != null && goObject.IsInView)
				{
					goObject.Remove();
				}
			}
			Link = null;
			if (StartPort != null)
			{
				GoObject goObject2 = StartPort.GoObject;
				if (goObject2 != null && goObject2.IsInView)
				{
					goObject2.Remove();
				}
			}
			StartPort = null;
			if (EndPort != null)
			{
				GoObject goObject3 = EndPort.GoObject;
				if (goObject3 != null && goObject3.IsInView)
				{
					goObject3.Remove();
				}
			}
			EndPort = null;
			if (ValidPortsCache != null)
			{
				ValidPortsCache.Clear();
			}
			StopTransaction();
		}

		/// <summary>
		/// A mouse move during a linking operation involves
		/// calling <see cref="M:Northwoods.Go.GoToolLinking.DoLinking(System.Drawing.PointF)" /> and autoscrolling the view.
		/// </summary>
		public override void DoMouseMove()
		{
			DoLinking(base.LastInput.DocPoint);
			base.View.DoAutoScroll(base.LastInput.ViewPoint);
		}

		/// <summary>
		/// A mouse up event ends the linking operation.
		/// </summary>
		/// <remarks>
		/// Depending on whether the user is drawing a new link or relinking,
		/// whether the user is drawing <see cref="P:Northwoods.Go.GoToolLinking.Forwards" /> or not, and
		/// whether <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> found a valid port at
		/// a reasonable distance, this method will call either
		/// <see cref="M:Northwoods.Go.GoToolLinking.DoNewLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />, <see cref="M:Northwoods.Go.GoToolLinking.DoRelink(Northwoods.Go.IGoLink,Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />,
		/// <see cref="M:Northwoods.Go.GoToolLinking.DoNoNewLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />, or <see cref="M:Northwoods.Go.GoToolLinking.DoNoRelink(Northwoods.Go.IGoLink,Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />.
		/// </remarks>
		public override void DoMouseUp()
		{
			IGoPort goPort = PickNearestPort(base.LastInput.DocPoint);
			if (goPort != null)
			{
				if (myLinkingNew)
				{
					if (Forwards)
					{
						DoNewLink(OriginalStartPort, goPort);
					}
					else
					{
						DoNewLink(goPort, OriginalStartPort);
					}
				}
				else if (Forwards)
				{
					DoRelink(Link, OriginalStartPort, goPort);
				}
				else
				{
					DoRelink(Link, goPort, OriginalStartPort);
				}
			}
			else
			{
				IGoPort goPort2 = PickPort(base.LastInput.DocPoint);
				if (myLinkingNew)
				{
					if (Forwards)
					{
						DoNoNewLink(OriginalStartPort, goPort2);
					}
					else
					{
						DoNoNewLink(goPort2, OriginalStartPort);
					}
				}
				else if (Forwards)
				{
					DoNoRelink(Link, OriginalStartPort, goPort2);
				}
				else
				{
					DoNoRelink(Link, goPort2, OriginalStartPort);
				}
			}
			StopTool();
		}

		/// <summary>
		/// Clean up the link state before stopping this tool.
		/// </summary>
		/// <remarks>
		/// Depending on whether the user is drawing a new link or relinking,
		/// whether the user is drawing <see cref="P:Northwoods.Go.GoToolLinking.Forwards" /> or not,
		/// this method will call either
		/// <see cref="M:Northwoods.Go.GoToolLinking.DoNoNewLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />, <see cref="M:Northwoods.Go.GoToolLinking.DoNoRelink(Northwoods.Go.IGoLink,Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />,
		/// or <see cref="M:Northwoods.Go.GoToolLinking.DoCancelRelink(Northwoods.Go.IGoLink,Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />.
		/// </remarks>
		public override void DoCancelMouse()
		{
			if (myLinkingNew)
			{
				if (Forwards)
				{
					DoNoNewLink(StartPort, null);
				}
				else
				{
					DoNoNewLink(null, StartPort);
				}
			}
			else if (OriginalEndPort == null)
			{
				if (Forwards)
				{
					DoNoRelink(Link, StartPort, null);
				}
				else
				{
					DoNoRelink(Link, null, StartPort);
				}
			}
			else if (Forwards)
			{
				DoCancelRelink(Link, OriginalStartPort, OriginalEndPort);
			}
			else
			{
				DoCancelRelink(Link, OriginalEndPort, OriginalStartPort);
			}
			base.View.CursorName = "default";
			StopTool();
		}

		/// <summary>
		/// Find a port in the document at the given point.
		/// </summary>
		/// <param name="dc">a <c>PointF</c> in document coordinates</param>
		/// <returns>
		/// an <see cref="T:Northwoods.Go.IGoPort" />, or null if none was found at <paramref name="dc" />,
		/// or null if the object found is a child of the <see cref="P:Northwoods.Go.GoToolLinking.Link" />.
		/// The latter case is to avoid picking any port that is part of
		/// the link, such as when the link is a <see cref="T:Northwoods.Go.GoLabeledLink" />
		/// and the <see cref="P:Northwoods.Go.GoLabeledLink.FromLabel" /> or the <see cref="P:Northwoods.Go.GoLabeledLink.ToLabel" />
		/// is a port or contains one.
		/// </returns>
		public virtual IGoPort PickPort(PointF dc)
		{
			GoObject goObject = base.View.PickObject(doc: true, view: false, dc, selectableOnly: false);
			if (goObject != null && Link != null && goObject.IsChildOf(Link.GoObject))
			{
				return null;
			}
			return goObject as IGoPort;
		}

		/// <summary>
		/// Start the process of drawing a new link from a given port.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="dc"></param>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.GoToolLinking.IsValidFromPort(Northwoods.Go.IGoPort)" /> is true, the user will be
		/// linking in the <see cref="P:Northwoods.Go.GoToolLinking.Forwards" /> direction--i.e. from the
		/// source to the destination.
		/// This method calls <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryPort(Northwoods.Go.IGoPort,System.Drawing.PointF,System.Boolean,System.Boolean)" /> to create both
		/// the <see cref="P:Northwoods.Go.GoToolLinking.StartPort" /> and the <see cref="P:Northwoods.Go.GoToolLinking.EndPort" />, and
		/// it calls <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> to create the <see cref="P:Northwoods.Go.GoToolLinking.Link" />.
		/// This starts a transaction.
		/// On WinForms it also changes the cursor to a Hand.
		/// </remarks>
		public virtual void StartNewLink(IGoPort port, PointF dc)
		{
			if (port != null)
			{
				StartTransaction();
				myLinkingNew = true;
				if (IsValidFromPort(port))
				{
					Forwards = true;
					StartPort = CreateTemporaryPort(port, port.GoObject.Center, forToPort: false, atEnd: false);
					EndPort = CreateTemporaryPort(port, dc, forToPort: true, atEnd: true);
					Link = CreateTemporaryLink(StartPort, EndPort);
				}
				else
				{
					Forwards = false;
					StartPort = CreateTemporaryPort(port, port.GoObject.Center, forToPort: true, atEnd: false);
					EndPort = CreateTemporaryPort(port, dc, forToPort: false, atEnd: true);
					Link = CreateTemporaryLink(EndPort, StartPort);
				}
				base.View.CursorName = "hand";
			}
		}

		/// <summary>
		/// This predicate is called to decide if it is OK for a user to start
		/// drawing a link from a given port.
		/// </summary>
		/// <param name="fromPort"></param>
		/// <returns>By default this returns the result of calling <see cref="M:Northwoods.Go.IGoPort.CanLinkFrom" /></returns>
		public virtual bool IsValidFromPort(IGoPort fromPort)
		{
			return fromPort.CanLinkFrom();
		}

		/// <summary>
		/// This predicate is called to decide if it is OK for a user to start
		/// drawing a link backwards at a given port that will be the destination
		/// for the link.
		/// </summary>
		/// <param name="toPort"></param>
		/// <returns>By default this is true if <see cref="P:Northwoods.Go.GoToolLinking.ForwardsOnly" /> is false
		/// <see cref="M:Northwoods.Go.IGoPort.CanLinkTo" /> is true</returns>
		public virtual bool IsValidToPort(IGoPort toPort)
		{
			if (!ForwardsOnly)
			{
				return toPort.CanLinkTo();
			}
			return false;
		}

		/// <summary>
		/// Start the process of reconnecting an existing link at a given port.
		/// </summary>
		/// <param name="oldlink"></param>
		/// <param name="forwards">true if the "To" port is being reconnected; false if it is the "From" end</param>
		/// <param name="dc"></param>
		/// <remarks>
		/// This starts a transaction.
		/// On WinForms it also changes the cursor to a Hand.
		/// </remarks>
		public virtual void StartRelink(IGoLink oldlink, bool forwards, PointF dc)
		{
			if (oldlink == null)
			{
				return;
			}
			GoObject goObject = oldlink.GoObject;
			if (goObject == null || goObject.Layer == null)
			{
				return;
			}
			StartTransaction();
			myLinkingNew = false;
			Link = oldlink;
			checked
			{
				if (forwards)
				{
					Forwards = true;
					OriginalStartPort = oldlink.FromPort;
					OriginalEndPort = oldlink.ToPort;
					PointF pnt = dc;
					if (OriginalStartPort != null)
					{
						pnt = OriginalStartPort.GoObject.Center;
					}
					else if (oldlink is GoLink)
					{
						GoLink goLink = (GoLink)oldlink;
						if (goLink.PointsCount > 0)
						{
							pnt = goLink.GetPoint(0);
						}
					}
					else if (oldlink is GoLabeledLink)
					{
						GoLabeledLink goLabeledLink = (GoLabeledLink)oldlink;
						if (goLabeledLink.RealLink.PointsCount > 0)
						{
							pnt = goLabeledLink.RealLink.GetPoint(0);
						}
					}
					StartPort = CreateTemporaryPort(OriginalStartPort, pnt, forToPort: false, atEnd: false);
					oldlink.FromPort = StartPort;
					EndPort = CreateTemporaryPort(OriginalEndPort, dc, forToPort: true, atEnd: true);
					oldlink.ToPort = EndPort;
				}
				else
				{
					Forwards = false;
					OriginalStartPort = oldlink.ToPort;
					OriginalEndPort = oldlink.FromPort;
					PointF pnt2 = dc;
					if (OriginalStartPort != null)
					{
						pnt2 = OriginalStartPort.GoObject.Center;
					}
					else if (oldlink is GoLink)
					{
						GoLink goLink2 = (GoLink)oldlink;
						if (goLink2.PointsCount > 0)
						{
							pnt2 = goLink2.GetPoint(goLink2.PointsCount - 1);
						}
					}
					else if (oldlink is GoLabeledLink)
					{
						GoLabeledLink goLabeledLink2 = (GoLabeledLink)oldlink;
						if (goLabeledLink2.RealLink.PointsCount > 0)
						{
							pnt2 = goLabeledLink2.RealLink.GetPoint(goLabeledLink2.RealLink.PointsCount - 1);
						}
					}
					StartPort = CreateTemporaryPort(OriginalStartPort, pnt2, forToPort: true, atEnd: false);
					oldlink.ToPort = StartPort;
					EndPort = CreateTemporaryPort(OriginalEndPort, dc, forToPort: false, atEnd: true);
					oldlink.FromPort = EndPort;
				}
				base.View.CursorName = "hand";
			}
		}

		/// <summary>
		/// This is responsible for creating a temporary port for the linking process.
		/// </summary>
		/// <param name="port">an existing port that the temporary port should be like; this may be null</param>
		/// <param name="pnt">the <c>PointF</c> in document coordinates for where the temporary port should be</param>
		/// <param name="forToPort">true if this is meant to be the <see cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// instead of the <see cref="P:Northwoods.Go.IGoLink.FromPort" /></param>
		/// <param name="atEnd">true if this is meant to be the <see cref="P:Northwoods.Go.GoToolLinking.EndPort" />
		/// instead of <see cref="P:Northwoods.Go.GoToolLinking.StartPort" /></param>
		/// <returns>a <see cref="T:Northwoods.Go.GoPort" /> in the view at <paramref name="pnt" /></returns>
		/// <remarks>
		/// This creates a new <see cref="T:Northwoods.Go.GoPort" /> that is similar to the <paramref name="port" />.
		/// By default the temporary port's <see cref="P:Northwoods.Go.GoPort.Style" /> is <see cref="F:Northwoods.Go.GoPortStyle.None" />,
		/// so that it is not seen by the user.
		/// It is added to the default layer of the view.
		/// </remarks>
		protected virtual IGoPort CreateTemporaryPort(IGoPort port, PointF pnt, bool forToPort, bool atEnd)
		{
			GoTemporaryPort goTemporaryPort = new GoTemporaryPort();
			goTemporaryPort.Target = (port as GoPort);
			if (port != null && port.GoObject != null)
			{
				goTemporaryPort.Size = port.GoObject.Size;
			}
			goTemporaryPort.Center = pnt;
			goTemporaryPort.Style = GoPortStyle.None;
			base.View.Layers.Default.Add(goTemporaryPort);
			return goTemporaryPort;
		}

		/// <summary>
		/// This is responsible for creating a temporary link when the user is drawing a new link.
		/// </summary>
		/// <param name="fromPort"></param>
		/// <param name="toPort"></param>
		/// <returns>a <see cref="T:Northwoods.Go.IGoLink" /> in the view</returns>
		/// <remarks>
		/// By default this just creates a copy of the <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.NewLinkPrototype" />
		/// using <paramref name="fromPort" /> and <paramref name="toPort" /> as the ends of the link.
		/// The temporary link has its <see cref="P:Northwoods.Go.GoLink.AdjustingStyle" /> set to 
		/// <see cref="F:Northwoods.Go.GoLinkAdjustingStyle.Calculate" />.
		/// The temporary link is added to the default layer of the view, but unlike
		/// temporary ports, is visible.
		/// </remarks>
		protected virtual IGoLink CreateTemporaryLink(IGoPort fromPort, IGoPort toPort)
		{
			IGoLink goLink = base.View.Document.CreateCopyDictionary().CopyComplete(base.View.NewLinkPrototype) as IGoLink;
			if (goLink != null && goLink.GoObject != null)
			{
				goLink.FromPort = fromPort;
				goLink.ToPort = toPort;
				GoObject goObject = goLink.GoObject;
				if (goObject is GoLink)
				{
					((GoLink)goObject).AdjustingStyle = GoLinkAdjustingStyle.Calculate;
				}
				else if (goObject is GoLabeledLink)
				{
					((GoLabeledLink)goObject).AdjustingStyle = GoLinkAdjustingStyle.Calculate;
				}
				base.View.Layers.Default.Add(goObject);
				return goLink;
			}
			return null;
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoToolLinking.DoMouseMove" /> to find the nearest
		/// valid port and adjust the temporary link according to where the given point is.
		/// </summary>
		/// <param name="dc">a <c>PointF</c> in document coordinates, the mouse position</param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> to find the closest valid port
		/// that we might link to, and passes the result to <see cref="M:Northwoods.Go.GoToolLinking.ImitatePort(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />
		/// to get the <see cref="P:Northwoods.Go.GoToolLinking.EndPort" /> to behave like that port.
		/// It also sets the <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Bounds" />
		/// of the <see cref="P:Northwoods.Go.GoToolLinking.EndPort" /> so that the temporary link appears to either
		/// follow the mouse pointer or connect to a nearby valid port.
		/// </remarks>
		public virtual void DoLinking(PointF dc)
		{
			if (EndPort != null)
			{
				GoObject goObject = EndPort.GoObject;
				if (goObject != null)
				{
					IGoPort goPort = PickNearestPort(dc);
					ImitatePort(EndPort, goPort);
					RectangleF rectangleF2 = goObject.Bounds = ((goPort != null && goPort.GoObject != null) ? goPort.GoObject.Bounds : new RectangleF(dc.X, dc.Y, 0f, 0f));
				}
			}
		}

		/// <summary>
		/// Change the <paramref name="endport" />'s behavior for link routing to be
		/// like the given port <paramref name="iport" />.
		/// </summary>
		/// <param name="endport">the value of <see cref="P:Northwoods.Go.GoToolLinking.EndPort" /></param>
		/// <param name="iport">the value of <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /></param>
		/// <remarks>
		/// This is just called by <see cref="M:Northwoods.Go.GoToolLinking.DoLinking(System.Drawing.PointF)" /> in order to get
		/// the temporary <see cref="P:Northwoods.Go.GoToolLinking.Link" /> to route in the way expected as if
		/// it really were being connected to <paramref name="iport" />.
		/// Remember that the <paramref name="endport" /> might be either the
		/// "FromPort" or the "ToPort" of a link, depending on the direction the
		/// link is being drawn, as given by the value of <see cref="P:Northwoods.Go.GoToolLinking.Forwards" />.
		/// </remarks>
		protected virtual void ImitatePort(IGoPort endport, IGoPort iport)
		{
			GoTemporaryPort goTemporaryPort = endport as GoTemporaryPort;
			if (goTemporaryPort != null)
			{
				goTemporaryPort.Target = (iport as GoPort);
			}
		}

		/// <summary>
		/// This predicate is called during the process of finding the nearest port
		/// that the user can link to.
		/// </summary>
		/// <param name="fromPort"></param>
		/// <param name="toPort"></param>
		/// <returns>Basically this is implemented as <c>fromPort.IsValidLink(toPort)</c></returns>
		/// <remarks>
		/// <para>
		/// If both <paramref name="fromPort" /> and <paramref name="toPort" /> are null, this returns false.
		/// If only one of the parameters is null, then this returns the value of
		/// <c>fromPort.CanLinkFrom()</c> or <c>toPort.CanLinkTo()</c>, on the non-null port parameter.
		/// This predicate also disallows interactive linking of a link to a port that is part of
		/// the link--e.g. linking to a port that is a label on a <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// The results of these calls are stored in the <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" />
		/// hash table.  The ports are associated with <see cref="F:Northwoods.Go.GoToolLinking.Valid" /> or <see cref="F:Northwoods.Go.GoToolLinking.Invalid" />
		/// values depending on whether this predicate returned true or false.
		/// Note that to check for links from a port to itself, this predicate may
		/// be called with the same value for both arguments.
		/// </para>
		/// <para>
		/// <see cref="T:Northwoods.Go.GoPort" /> has more options for link validity checking; see <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> for more details.
		/// If you override this method, you may well need to do so for both
		/// <see cref="T:Northwoods.Go.GoToolLinkingNew" /> and <see cref="T:Northwoods.Go.GoToolRelinking" />,
		/// and then replace both of the standard linking tools in your <see cref="T:Northwoods.Go.GoView" />
		/// by calling <see cref="M:Northwoods.Go.GoView.ReplaceMouseTool(System.Type,Northwoods.Go.IGoTool)" />.
		/// </para>
		/// </remarks>
		public virtual bool IsValidLink(IGoPort fromPort, IGoPort toPort)
		{
			if (fromPort == null && toPort == null)
			{
				return false;
			}
			if (fromPort != null && toPort == null)
			{
				return fromPort.CanLinkFrom();
			}
			if (fromPort == null && toPort != null)
			{
				return toPort.CanLinkTo();
			}
			if (Link != null)
			{
				if (fromPort != null && fromPort.GoObject != null && fromPort.GoObject.IsChildOf(Link.GoObject))
				{
					return false;
				}
				if (toPort != null && toPort.GoObject != null && toPort.GoObject.IsChildOf(Link.GoObject))
				{
					return false;
				}
			}
			return fromPort.IsValidLink(toPort);
		}

		/// <summary>
		/// Find the valid document port nearest to a given point.
		/// </summary>
		/// <param name="dc">a <c>PointF</c> in document coordinates</param>
		/// <returns></returns>
		/// <remarks>
		/// A nearby port (as determined by the distance between <paramref name="dc" />
		/// and the result of <see cref="M:Northwoods.Go.GoToolLinking.PortPoint(Northwoods.Go.IGoPort,System.Drawing.PointF)" />) must be within
		/// the <see cref="P:Northwoods.Go.GoView.PortGravity" /> distance for it to qualify,
		/// and it must be in a <see cref="T:Northwoods.Go.GoLayer" /> that is visible.
		/// This uses the <see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> predicate, passing it the
		/// <see cref="P:Northwoods.Go.GoToolLinking.OriginalStartPort" /> along with each port to be considered.
		/// The results of <see cref="M:Northwoods.Go.GoToolLinking.IsValidLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> are cached in <see cref="P:Northwoods.Go.GoToolLinking.ValidPortsCache" />,
		/// for the cases where determining valid links is computationally expensive.
		/// This cache is valid only for the duration of this linking tool as the
		/// current tool; it is cleared each time this tool is stopped.
		/// </remarks>
		public virtual IGoPort PickNearestPort(PointF dc)
		{
			IGoPort goPort = null;
			float portGravity = base.View.PortGravity;
			RectangleF r = new RectangleF(dc.X - portGravity, dc.Y - portGravity, portGravity * 2f, portGravity * 2f);
			float bestDist = portGravity * portGravity;
			checked
			{
				foreach (GoLayer backward in base.View.Layers.Backwards)
				{
					if (backward.IsInDocument && backward.CanViewObjects())
					{
						GoLayer.GoLayerCache goLayerCache = backward.FindCache(r);
						if (goLayerCache != null)
						{
							List<GoObject> objects = goLayerCache.Objects;
							for (int num = objects.Count - 1; num >= 0; num--)
							{
								GoObject obj = objects[num];
								goPort = pickNearestPort1(obj, dc, goPort, ref bestDist);
							}
						}
						else
						{
							foreach (GoObject backward2 in backward.Backwards)
							{
								goPort = pickNearestPort1(backward2, dc, goPort, ref bestDist);
							}
						}
					}
				}
				return goPort;
			}
		}

		private IGoPort pickNearestPort1(GoObject obj, PointF dc, IGoPort bestPort, ref float bestDist)
		{
			IGoPort goPort = obj as IGoPort;
			if (goPort != null)
			{
				PointF pointF = PortPoint(goPort, dc);
				float num = dc.X - pointF.X;
				float num2 = dc.Y - pointF.Y;
				float num3 = num * num + num2 * num2;
				if (num3 < bestDist)
				{
					object value = null;
					if (ValidPortsCache != null)
					{
						ValidPortsCache.TryGetValue(goPort, out value);
					}
					if (value == Valid)
					{
						bestPort = goPort;
						bestDist = num3;
					}
					else if (value != Invalid)
					{
						if ((Forwards && IsValidLink(OriginalStartPort, goPort)) || (!Forwards && IsValidLink(goPort, OriginalStartPort)))
						{
							if (ValidPortsCache != null)
							{
								ValidPortsCache[goPort] = Valid;
							}
							bestPort = goPort;
							bestDist = num3;
						}
						else if (ValidPortsCache != null)
						{
							ValidPortsCache[goPort] = Invalid;
						}
					}
				}
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					bestPort = pickNearestPort1(item, dc, bestPort, ref bestDist);
				}
				return bestPort;
			}
			return bestPort;
		}

		/// <summary>
		/// Return a <c>PointF</c> representing the position of the port.
		/// </summary>
		/// <param name="port">an <see cref="T:Northwoods.Go.IGoPort" /> whose distance is being considered</param>
		/// <param name="dc">the point nearest which we are searching for a port</param>
		/// <returns>normally, <c>port.GoObject.Center</c></returns>
		/// <remarks>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.PickNearestPort(System.Drawing.PointF)" /> for each
		/// port in the document.
		/// For large ports, if the <paramref name="port" /> is a <see cref="T:Northwoods.Go.GoPort" />,
		/// this just returns the point <paramref name="dc" /> if it is inside the port.
		/// If it is outside of the port,
		/// this uses the result of <see cref="M:Northwoods.Go.GoPort.GetLinkPointFromPoint(System.Drawing.PointF)" />, which should
		/// be more accurate than the center of the port.
		/// </remarks>
		public virtual PointF PortPoint(IGoPort port, PointF dc)
		{
			GoPort goPort = port.GoObject as GoPort;
			if (goPort != null)
			{
				GoObject goObject = goPort.PortObject;
				if (goObject == null || goObject.Layer == null)
				{
					goObject = goPort;
				}
				SizeF size = goObject.Size;
				float num = 10f / base.View.WorldScale.Width;
				if (size.Width < num && size.Height < num)
				{
					return goObject.Center;
				}
				if (goObject.ContainsPoint(dc))
				{
					return dc;
				}
				return goPort.GetLinkPointFromPoint(dc);
			}
			return port.GoObject.Center;
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.DoMouseUp" /> in order to create a new link.
		/// </summary>
		/// <param name="fromPort"></param>
		/// <param name="toPort"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoView.CreateLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> and <see cref="M:Northwoods.Go.GoView.RaiseLinkCreated(Northwoods.Go.GoObject)" />.
		/// This method is responsible for setting <see cref="P:Northwoods.Go.GoTool.TransactionResult" />, so
		/// that <see cref="M:Northwoods.Go.GoToolLinking.Stop" />'s call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will
		/// properly call <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// as desired.
		/// </remarks>
		public virtual void DoNewLink(IGoPort fromPort, IGoPort toPort)
		{
			IGoLink goLink = base.View.CreateLink(fromPort, toPort);
			if (goLink != null)
			{
				base.TransactionResult = "New Link";
				base.View.RaiseLinkCreated(goLink.GoObject);
			}
			else
			{
				base.TransactionResult = null;
			}
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.DoMouseUp" /> or <see cref="M:Northwoods.Go.GoToolLinking.DoCancelMouse" />
		/// when no new link was drawn by the user.
		/// </summary>
		/// <param name="fromPort"></param>
		/// <param name="toPort"></param>
		/// <remarks>
		/// This method is responsible for setting <see cref="P:Northwoods.Go.GoTool.TransactionResult" />, so
		/// that <see cref="M:Northwoods.Go.GoToolLinking.Stop" />'s call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will
		/// properly call <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// as desired.
		/// </remarks>
		public virtual void DoNoNewLink(IGoPort fromPort, IGoPort toPort)
		{
			base.TransactionResult = null;
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.DoMouseUp" /> in order to reconnect the existing link.
		/// </summary>
		/// <param name="oldlink"></param>
		/// <param name="fromPort"></param>
		/// <param name="toPort"></param>
		/// <remarks>
		/// This makes sure <paramref name="oldlink" /> refers to <paramref name="fromPort" />
		/// and <paramref name="toPort" /> and then calls <see cref="M:Northwoods.Go.GoView.RaiseLinkRelinked(Northwoods.Go.GoObject)" />.
		/// This method is responsible for setting <see cref="P:Northwoods.Go.GoTool.TransactionResult" />, so
		/// that <see cref="M:Northwoods.Go.GoToolLinking.Stop" />'s call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will
		/// properly call <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// as desired.
		/// </remarks>
		public virtual void DoRelink(IGoLink oldlink, IGoPort fromPort, IGoPort toPort)
		{
			oldlink.FromPort = fromPort;
			oldlink.ToPort = toPort;
			GoSubGraphBase.ReparentToCommonSubGraph(oldlink.GoObject, fromPort?.GoObject, toPort?.GoObject, behind: true, base.View.Document.LinksLayer);
			base.TransactionResult = "Relink";
			base.View.RaiseLinkRelinked(oldlink.GoObject);
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.DoMouseUp" /> or <see cref="M:Northwoods.Go.GoToolLinking.DoCancelMouse" />
		/// when an existing link was purposely not reconnected by the user to any port.
		/// </summary>
		/// <param name="oldlink"></param>
		/// <param name="fromPort">might be null</param>
		/// <param name="toPort">might be null</param>
		/// <remarks>
		/// <para>
		/// If the <paramref name="oldlink" /> is <see cref="P:Northwoods.Go.GoObject.Movable" />,
		/// this is considered a relinking to no port, rather than a deletion of the link.
		/// </para>
		/// <para>
		/// Because this case (when <see cref="P:Northwoods.Go.GoObject.Movable" /> is false)
		/// effectively results in an object being removed from the
		/// document, this method calls <see cref="M:Northwoods.Go.GoView.RaiseSelectionDeleting(System.ComponentModel.CancelEventArgs)" />
		/// and <see cref="M:Northwoods.Go.GoView.RaiseSelectionDeleted" />.
		/// If the <see cref="M:Northwoods.Go.GoView.RaiseSelectionDeleting(System.ComponentModel.CancelEventArgs)" /> event results in
		/// a cancellation, this calls <see cref="M:Northwoods.Go.GoToolLinking.DoCancelMouse" /> instead of
		/// removing the link.
		/// This method does not remove the link if <see cref="M:Northwoods.Go.GoObject.CanDelete" />
		/// is false.
		/// </para>
		/// <para>
		/// This method is responsible for setting <see cref="P:Northwoods.Go.GoTool.TransactionResult" />, so
		/// that <see cref="M:Northwoods.Go.GoToolLinking.Stop" />'s call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will
		/// properly call <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// as desired.
		/// </para>
		/// </remarks>
		public virtual void DoNoRelink(IGoLink oldlink, IGoPort fromPort, IGoPort toPort)
		{
			base.TransactionResult = null;
			GoObject goObject = oldlink.GoObject;
			if (goObject == null || goObject.Layer == null)
			{
				return;
			}
			if (goObject.Movable)
			{
				oldlink.FromPort = fromPort;
				oldlink.ToPort = toPort;
				base.TransactionResult = "Relink";
				base.View.RaiseLinkRelinked(oldlink.GoObject);
			}
			else if (goObject.CanDelete() && base.View.CanDeleteObjects())
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				base.View.RaiseSelectionDeleting(cancelEventArgs);
				if (!cancelEventArgs.Cancel)
				{
					goObject.Remove();
					base.View.RaiseSelectionDeleted();
					base.TransactionResult = "Relink";
				}
				else
				{
					DoCancelMouse();
				}
			}
			else if (OriginalEndPort == null)
			{
				base.View.CursorName = "default";
				base.TransactionResult = null;
				StopTool();
			}
			else
			{
				DoCancelMouse();
			}
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoToolLinking.DoCancelMouse" /> when a relinking was cancelled
		/// by the user.
		/// </summary>
		/// <param name="oldlink"></param>
		/// <param name="fromPort">the original From port</param>
		/// <param name="toPort">the original To port</param>
		/// <remarks>
		/// This method is responsible for setting <see cref="P:Northwoods.Go.GoTool.TransactionResult" />, so
		/// that <see cref="M:Northwoods.Go.GoToolLinking.Stop" />'s call to <see cref="M:Northwoods.Go.GoTool.StopTransaction" /> will
		/// properly call <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoView.AbortTransaction" />,
		/// as desired.
		/// </remarks>
		public virtual void DoCancelRelink(IGoLink oldlink, IGoPort fromPort, IGoPort toPort)
		{
			oldlink.FromPort = fromPort;
			oldlink.ToPort = toPort;
			base.TransactionResult = null;
		}
	}
}
