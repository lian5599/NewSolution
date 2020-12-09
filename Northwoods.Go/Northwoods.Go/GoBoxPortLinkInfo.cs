using System;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// Information used when sorting the links connected at a <see cref="T:Northwoods.Go.GoBoxPort" />.
	/// </summary>
	/// <remarks>
	/// This is used by <see cref="M:Northwoods.Go.GoBoxPort.SortLinkInfos(Northwoods.Go.GoBoxPortLinkInfo[])" />,
	/// <see cref="M:Northwoods.Go.GoBoxPort.AssignLinkPoints(Northwoods.Go.GoBoxPortLinkInfo[])" />, and
	/// <see cref="M:Northwoods.Go.GoBoxPort.AssignEndSegmentLengths(Northwoods.Go.GoBoxPortLinkInfo[])" />
	/// only when <see cref="P:Northwoods.Go.GoBoxPort.LinkPointsSpread" /> is true.
	/// This information is transient, just used when calculating link points.
	/// </remarks>
	[Serializable]
	public sealed class GoBoxPortLinkInfo
	{
		private IGoLink myLink;

		private float myAngle;

		private int mySide;

		private int myCountOnSide;

		private int myIndexOnSide;

		private PointF myLinkPoint;

		private float myEndSegmentLength;

		/// <summary>
		/// The <see cref="T:Northwoods.Go.IGoLink" /> whose link point is being sorted around the <see cref="T:Northwoods.Go.GoBoxPort" />.
		/// </summary>
		public IGoLink Link
		{
			get
			{
				return myLink;
			}
			set
			{
				myLink = value;
			}
		}

		/// <summary>
		/// The effective angle at which the link connects with another node;
		/// this value corresponds to the result of calling <see cref="M:Northwoods.Go.GoBoxPort.GetAngle(Northwoods.Go.IGoLink)" />.
		/// </summary>
		/// <value>
		/// The angle in degrees from this port to the other port.
		/// </value>
		public float Angle
		{
			get
			{
				return myAngle;
			}
			set
			{
				myAngle = value;
			}
		}

		/// <summary>
		/// The side at which the link connects;
		/// this value corresponds to the result of calling <see cref="M:Northwoods.Go.GoBoxPort.GetDirection(Northwoods.Go.IGoLink)" />.
		/// </summary>
		/// <value>
		/// One of <see cref="F:Northwoods.Go.GoObject.MiddleRight" />,
		/// <see cref="F:Northwoods.Go.GoObject.MiddleBottom" />, <see cref="F:Northwoods.Go.GoObject.MiddleLeft" />,
		/// <see cref="F:Northwoods.Go.GoObject.MiddleTop" />.
		/// </value>
		public int Side
		{
			get
			{
				return mySide;
			}
			set
			{
				mySide = value;
			}
		}

		/// <summary>
		/// How many links are connected on this side;
		/// computed after calling <see cref="M:Northwoods.Go.GoBoxPort.SortLinkInfos(Northwoods.Go.GoBoxPortLinkInfo[])" /> and before calling <see cref="M:Northwoods.Go.GoBoxPort.AssignLinkPoints(Northwoods.Go.GoBoxPortLinkInfo[])" />.
		/// </summary>
		public int CountOnSide
		{
			get
			{
				return myCountOnSide;
			}
			set
			{
				myCountOnSide = value;
			}
		}

		/// <summary>
		/// The index of this link on this side;
		/// computed after calling <see cref="M:Northwoods.Go.GoBoxPort.SortLinkInfos(Northwoods.Go.GoBoxPortLinkInfo[])" /> and before calling <see cref="M:Northwoods.Go.GoBoxPort.AssignLinkPoints(Northwoods.Go.GoBoxPortLinkInfo[])" />.
		/// </summary>
		public int IndexOnSide
		{
			get
			{
				return myIndexOnSide;
			}
			set
			{
				myIndexOnSide = value;
			}
		}

		/// <summary>
		/// The document point at which the link should terminate;
		/// should be set in <see cref="M:Northwoods.Go.GoBoxPort.AssignLinkPoints(Northwoods.Go.GoBoxPortLinkInfo[])" />.
		/// </summary>
		public PointF LinkPoint
		{
			get
			{
				return myLinkPoint;
			}
			set
			{
				myLinkPoint = value;
			}
		}

		/// <summary>
		/// The value of length of the last segment for this link at this port;
		/// should be set in <see cref="M:Northwoods.Go.GoBoxPort.AssignEndSegmentLengths(Northwoods.Go.GoBoxPortLinkInfo[])" />.
		/// </summary>
		public float EndSegmentLength
		{
			get
			{
				return myEndSegmentLength;
			}
			set
			{
				myEndSegmentLength = value;
			}
		}

		internal GoBoxPortLinkInfo(IGoLink l, float a, int s)
		{
			myLink = l;
			myAngle = a;
			mySide = s;
		}
	}
}
