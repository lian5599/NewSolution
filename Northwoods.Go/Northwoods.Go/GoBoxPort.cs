using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A rectangular port that is smart about where each of its
	/// links is connected, centered or distributed on each side,
	/// meeting the edge of the port orthogonally.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Although this class is normally used as part of a <see cref="T:Northwoods.Go.GoBoxNode" />
	/// to act as a port that surrounds another object, you can use
	/// GoBoxPort as part of other nodes, such as <see cref="T:Northwoods.Go.GoIconicNode" />.
	/// </para>
	/// <para>
	/// Normally links can connect at any of the four sides.
	/// If you want to permit links at only particular sides, you can set the
	/// <see cref="P:Northwoods.Go.GoBoxPort.FromSides" /> and/or <see cref="P:Northwoods.Go.GoBoxPort.ToSides" /> properties.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoBoxPort : GoPort
	{
		[Serializable]
		internal sealed class EndPositionComparer : IComparer<GoBoxPortLinkInfo>
		{
			internal EndPositionComparer()
			{
			}

			public int Compare(GoBoxPortLinkInfo a, GoBoxPortLinkInfo b)
			{
				if (a == null || b == null || a == b)
				{
					return 0;
				}
				if (a.Side < b.Side)
				{
					return -1;
				}
				if (a.Side > b.Side)
				{
					return 1;
				}
				if (a.Angle < b.Angle)
				{
					return -1;
				}
				if (a.Angle > b.Angle)
				{
					return 1;
				}
				return 0;
			}
		}

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxPort.LinkPointsSpread" /> property.
		/// </summary>
		public const int ChangedLinkPointsSpread = 2211;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxPort.FromSides" /> property.
		/// </summary>
		public const int ChangedFromSides = 2212;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBoxPort.ToSides" /> property.
		/// </summary>
		public const int ChangedToSides = 2213;

		private static IComparer<GoBoxPortLinkInfo> myComparer = new EndPositionComparer();

		private bool myLinkPointsSpread;

		private int myFromSides;

		private int myToSides;

		[NonSerialized]
		private GoBoxPortLinkInfo[] mySortedLinks;

		[NonSerialized]
		private bool myRespreading;

		[NonSerialized]
		private bool myReSort;

		/// <summary>
		/// Gets or sets whether the link points of the links connected to this port
		/// are spread evenly along the side of the port, or if they are all at the
		/// mid-point of the side.
		/// </summary>
		/// <value>
		/// The default value is false--all of the links connect at the mid-point of each side.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the link points are distributed evenly along each side")]
		public virtual bool LinkPointsSpread
		{
			get
			{
				return myLinkPointsSpread;
			}
			set
			{
				bool flag = myLinkPointsSpread;
				if (flag != value)
				{
					myLinkPointsSpread = value;
					Changed(2211, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(2211, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the spot(s) indicating the side(s) of the port from which links come out.
		/// </summary>
		/// <value>
		/// The default value is <see cref="T:Northwoods.Go.GoObject" />.<see cref="F:Northwoods.Go.GoObject.NoSpot" />, indicating no restriction.
		/// The permitted values are <c>NoSpot</c> or a bitwise combination of any number of these side spots:
		/// <c>MiddleLeft</c>, <c>MiddleRight</c>, <c>MiddleTop</c>, <c>MiddleBottom</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The one side or one of two opposite sides at which links may come from")]
		public virtual int FromSides
		{
			get
			{
				return myFromSides;
			}
			set
			{
				int num = myFromSides;
				if (num != value)
				{
					myFromSides = value;
					Changed(2212, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(2212, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the spot(s) indicating the side(s) of the port to which links go.
		/// </summary>
		/// <value>
		/// The default value is <see cref="T:Northwoods.Go.GoObject" />.<see cref="F:Northwoods.Go.GoObject.NoSpot" />, indicating no restriction.
		/// The permitted values are <c>NoSpot</c> or a bitwise combination of any number of these side spots:
		/// <c>MiddleLeft</c>, <c>MiddleRight</c>, <c>MiddleTop</c>, <c>MiddleBottom</c>.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The one side or one of two opposite sides at which links may go to")]
		public virtual int ToSides
		{
			get
			{
				return myToSides;
			}
			set
			{
				int num = myToSides;
				if (num != value)
				{
					myToSides = value;
					Changed(2213, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LinksOnPortChanged(2213, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					}
				}
			}
		}

		/// <summary>
		/// Construct a rectangular port that is a solid gray.
		/// </summary>
		public GoBoxPort()
		{
			Style = GoPortStyle.Rectangle;
			Pen = null;
			Brush = GoShape.Brushes_Gray;
			FromSpot = 1;
			ToSpot = 1;
		}

		/// <summary>
		/// Don't copy internally cached information.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoBoxPort goBoxPort = (GoBoxPort)base.CopyObject(env);
			if (goBoxPort != null)
			{
				goBoxPort.mySortedLinks = null;
				goBoxPort.myRespreading = false;
				goBoxPort.myReSort = false;
			}
			return goBoxPort;
		}

		/// <summary>
		/// Unlike most ports, this kind of port will draw a drop-shadow, if expected of the parent
		/// <see cref="T:Northwoods.Go.GoBoxNode" />.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			if (Style != 0 && base.Parent != null && base.Parent.Shadowed)
			{
				RectangleF bounds = Bounds;
				SizeF shadowOffset = base.Parent.GetShadowOffset(view);
				if (Brush != null)
				{
					Brush shadowBrush = base.Parent.GetShadowBrush(view);
					GoShape.DrawRectangle(g, view, null, shadowBrush, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
				else if (Pen != null)
				{
					Pen shadowPen = base.Parent.GetShadowPen(view, GoShape.GetPenWidth(Pen));
					GoShape.DrawRectangle(g, view, shadowPen, null, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
			}
			base.Paint(g, view);
		}

		/// <summary>
		/// The expanded paint bounds for a shape includes any Pen width and miter
		/// limit and any drop shadow.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			rect = base.ExpandPaintBounds(rect, view);
			if (Style != 0 && base.Parent != null && base.Parent.Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (shadowOffset.Width < 0f)
				{
					rect.X += shadowOffset.Width;
					rect.Width -= shadowOffset.Width;
				}
				else
				{
					rect.Width += shadowOffset.Width;
				}
				if (shadowOffset.Height < 0f)
				{
					rect.Y += shadowOffset.Height;
					rect.Height -= shadowOffset.Height;
				}
				else
				{
					rect.Height += shadowOffset.Height;
				}
			}
			return rect;
		}

		/// <summary>
		/// Determine the link point by judging where the link is going
		/// to, and at what angle.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>a <c>PointF</c> on the edge of the port, in document coordinates</returns>
		public override PointF GetFromLinkPoint(IGoLink link)
		{
			GoObject goObject = PortObject;
			if (goObject == null || goObject.Layer == null)
			{
				goObject = this;
			}
			if (FromSpot != 0 && FromSpot != 1)
			{
				return goObject.GetSpotLocation(FromSpot);
			}
			if (link == null || link.GoObject == null)
			{
				return goObject.Center;
			}
			if (LinkPointsSpread)
			{
				GoBoxPortLinkInfo[] linkInfos = GetLinkInfos();
				foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkInfos)
				{
					if (goBoxPortLinkInfo.Link == link)
					{
						return goBoxPortLinkInfo.LinkPoint;
					}
				}
			}
			int fromSides = FromSides;
			switch (fromSides)
			{
			case 128:
				return goObject.GetSpotLocation(128);
			case 256:
				return goObject.GetSpotLocation(256);
			case 32:
				return goObject.GetSpotLocation(32);
			case 64:
				return goObject.GetSpotLocation(64);
			default:
			{
				float num = GetAngle(link);
				switch (fromSides)
				{
				case 160:
					if (num > 180f)
					{
						return goObject.GetSpotLocation(32);
					}
					return goObject.GetSpotLocation(128);
				case 320:
					if (num > 90f && num <= 270f)
					{
						return goObject.GetSpotLocation(256);
					}
					return goObject.GetSpotLocation(64);
				default:
				{
					float num2 = (float)(Math.Atan2(goObject.Height, goObject.Width) * 180.0 / Math.PI);
					switch (fromSides)
					{
					case 288:
						if (num > num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(32);
					case 96:
						if (num > 180f - num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(64);
					case 192:
						if (num > num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(128);
						}
						return goObject.GetSpotLocation(64);
					case 384:
						if (num > 180f - num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(128);
					case 352:
						if (num > 90f && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						if (num > 180f + num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(64);
					case 224:
						if (num > 180f && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						if (num > num2 && num <= 180f)
						{
							return goObject.GetSpotLocation(128);
						}
						return goObject.GetSpotLocation(64);
					case 448:
						if (num > num2 && num <= 180f - num2)
						{
							return goObject.GetSpotLocation(128);
						}
						if (num > 180f - num2 && num <= 270f)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(64);
					case 416:
						if (num > 180f - num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						if (num > 180f + num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(128);
					default:
					{
						if (IsOrthogonal(link) && fromSides != 480)
						{
							num -= 15f;
							if (num < 0f)
							{
								num += 360f;
							}
						}
						int num3 = 1;
						num3 = ((num > num2 && num < 180f - num2) ? 128 : ((num >= 180f - num2 && num <= 180f + num2) ? 256 : ((!(num > 180f + num2) || !(num < 360f - num2)) ? 64 : 32)));
						return goObject.GetSpotLocation(num3);
					}
					}
				}
				}
			}
			}
		}

		/// <summary>
		/// Determine the link point by judging where the link is coming
		/// from, and at what angle.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>a <c>PointF</c> on the edge of the port, in document coordinates</returns>
		public override PointF GetToLinkPoint(IGoLink link)
		{
			GoObject goObject = PortObject;
			if (goObject == null || goObject.Layer == null)
			{
				goObject = this;
			}
			if (ToSpot != 0 && ToSpot != 1)
			{
				return goObject.GetSpotLocation(ToSpot);
			}
			if (link == null || link.GoObject == null)
			{
				return goObject.Center;
			}
			if (LinkPointsSpread)
			{
				GoBoxPortLinkInfo[] linkInfos = GetLinkInfos();
				foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkInfos)
				{
					if (goBoxPortLinkInfo.Link == link)
					{
						return goBoxPortLinkInfo.LinkPoint;
					}
				}
			}
			int toSides = ToSides;
			switch (toSides)
			{
			case 128:
				return goObject.GetSpotLocation(128);
			case 256:
				return goObject.GetSpotLocation(256);
			case 32:
				return goObject.GetSpotLocation(32);
			case 64:
				return goObject.GetSpotLocation(64);
			default:
			{
				float num = GetAngle(link);
				switch (toSides)
				{
				case 160:
					if (num > 180f)
					{
						return goObject.GetSpotLocation(32);
					}
					return goObject.GetSpotLocation(128);
				case 320:
					if (num > 90f && num <= 270f)
					{
						return goObject.GetSpotLocation(256);
					}
					return goObject.GetSpotLocation(64);
				default:
				{
					float num2 = (float)(Math.Atan2(goObject.Height, goObject.Width) * 180.0 / Math.PI);
					switch (toSides)
					{
					case 288:
						if (num > num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(32);
					case 96:
						if (num > 180f - num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(64);
					case 192:
						if (num > num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(128);
						}
						return goObject.GetSpotLocation(64);
					case 384:
						if (num > 180f - num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(128);
					case 352:
						if (num > 90f && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						if (num > 180f + num2 && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(64);
					case 224:
						if (num > 180f && num <= 360f - num2)
						{
							return goObject.GetSpotLocation(32);
						}
						if (num > num2 && num <= 180f)
						{
							return goObject.GetSpotLocation(128);
						}
						return goObject.GetSpotLocation(64);
					case 448:
						if (num > num2 && num <= 180f - num2)
						{
							return goObject.GetSpotLocation(128);
						}
						if (num > 180f - num2 && num <= 270f)
						{
							return goObject.GetSpotLocation(256);
						}
						return goObject.GetSpotLocation(64);
					case 416:
						if (num > 180f - num2 && num <= 180f + num2)
						{
							return goObject.GetSpotLocation(256);
						}
						if (num > 180f + num2)
						{
							return goObject.GetSpotLocation(32);
						}
						return goObject.GetSpotLocation(128);
					default:
					{
						if (IsOrthogonal(link) && toSides != 480)
						{
							num += 15f;
							if (num >= 360f)
							{
								num -= 360f;
							}
						}
						int num3 = 1;
						num3 = ((num > num2 && num < 180f - num2) ? 128 : ((num >= 180f - num2 && num <= 180f + num2) ? 256 : ((!(num > 180f + num2) || !(num < 360f - num2)) ? 64 : 32)));
						return goObject.GetSpotLocation(num3);
					}
					}
				}
				}
			}
			}
		}

		/// <summary>
		/// Make sure the link is going out perpendicularly from the
		/// sides of the port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>the angle in degrees</returns>
		public override float GetFromLinkDir(IGoLink link)
		{
			if (FromSpot != 0 && FromSpot != 1)
			{
				return GetLinkDir(FromSpot);
			}
			GoObject goObject = PortObject;
			if (goObject == null || goObject.Layer == null)
			{
				goObject = this;
			}
			int num = FromSides;
			if (LinkPointsSpread && !myRespreading)
			{
				GoBoxPortLinkInfo[] linkInfos = GetLinkInfos();
				foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkInfos)
				{
					if (goBoxPortLinkInfo.Link == link)
					{
						num = goBoxPortLinkInfo.Side;
					}
				}
			}
			switch (num)
			{
			case 128:
				return 90f;
			case 256:
				return 180f;
			case 32:
				return 270f;
			case 64:
				return 0f;
			default:
			{
				float num2 = GetAngle(link);
				switch (num)
				{
				case 160:
					if (num2 > 180f)
					{
						return 270f;
					}
					return 90f;
				case 320:
					if (num2 > 90f && num2 <= 270f)
					{
						return 180f;
					}
					return 0f;
				default:
				{
					float num3 = (float)(Math.Atan2(goObject.Height, goObject.Width) * 180.0 / Math.PI);
					switch (num)
					{
					case 288:
						if (num2 > num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						return 270f;
					case 96:
						if (num2 > 180f - num3 && num2 <= 360f - num3)
						{
							return 270f;
						}
						return 0f;
					case 192:
						if (num2 > num3 && num2 <= 180f + num3)
						{
							return 90f;
						}
						return 0f;
					case 384:
						if (num2 > 180f - num3 && num2 <= 360f - num3)
						{
							return 180f;
						}
						return 90f;
					case 352:
						if (num2 > 90f && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3 && num2 <= 360f - num3)
						{
							return 270f;
						}
						return 0f;
					case 224:
						if (num2 > 180f && num2 <= 360f - num3)
						{
							return 270f;
						}
						if (num2 > num3 && num2 <= 180f)
						{
							return 90f;
						}
						return 0f;
					case 448:
						if (num2 > num3 && num2 <= 180f - num3)
						{
							return 90f;
						}
						if (num2 > 180f - num3 && num2 <= 270f)
						{
							return 180f;
						}
						return 0f;
					case 416:
						if (num2 > 180f - num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3)
						{
							return 270f;
						}
						return 90f;
					default:
						if (IsOrthogonal(link) && num != 480)
						{
							num2 -= 15f;
							if (num2 < 0f)
							{
								num2 += 360f;
							}
						}
						if (num2 > num3 && num2 < 180f - num3)
						{
							return 90f;
						}
						if (num2 >= 180f - num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3 && num2 < 360f - num3)
						{
							return 270f;
						}
						return 0f;
					}
				}
				}
			}
			}
		}

		/// <summary>
		/// Make sure the link is coming in perpendicularly to the
		/// sides of the port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>the angle in degrees</returns>
		public override float GetToLinkDir(IGoLink link)
		{
			if (ToSpot != 0 && ToSpot != 1)
			{
				return GetLinkDir(ToSpot);
			}
			GoObject goObject = PortObject;
			if (goObject == null || goObject.Layer == null)
			{
				goObject = this;
			}
			int num = ToSides;
			if (LinkPointsSpread && !myRespreading)
			{
				GoBoxPortLinkInfo[] linkInfos = GetLinkInfos();
				foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkInfos)
				{
					if (goBoxPortLinkInfo.Link == link)
					{
						num = goBoxPortLinkInfo.Side;
					}
				}
			}
			switch (num)
			{
			case 128:
				return 90f;
			case 256:
				return 180f;
			case 32:
				return 270f;
			case 64:
				return 0f;
			default:
			{
				float num2 = GetAngle(link);
				switch (num)
				{
				case 160:
					if (num2 > 180f)
					{
						return 270f;
					}
					return 90f;
				case 320:
					if (num2 > 90f && num2 <= 270f)
					{
						return 180f;
					}
					return 0f;
				default:
				{
					float num3 = (float)(Math.Atan2(goObject.Height, goObject.Width) * 180.0 / Math.PI);
					switch (num)
					{
					case 288:
						if (num2 > num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						return 270f;
					case 96:
						if (num2 > 180f - num3 && num2 <= 360f - num3)
						{
							return 270f;
						}
						return 0f;
					case 192:
						if (num2 > num3 && num2 <= 180f + num3)
						{
							return 90f;
						}
						return 0f;
					case 384:
						if (num2 > 180f - num3 && num2 <= 360f - num3)
						{
							return 180f;
						}
						return 90f;
					case 352:
						if (num2 > 90f && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3 && num2 <= 360f - num3)
						{
							return 270f;
						}
						return 0f;
					case 224:
						if (num2 > 180f && num2 <= 360f - num3)
						{
							return 270f;
						}
						if (num2 > num3 && num2 <= 180f)
						{
							return 90f;
						}
						return 0f;
					case 448:
						if (num2 > num3 && num2 <= 180f - num3)
						{
							return 90f;
						}
						if (num2 > 180f - num3 && num2 <= 270f)
						{
							return 180f;
						}
						return 0f;
					case 416:
						if (num2 > 180f - num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3)
						{
							return 270f;
						}
						return 90f;
					default:
						if (IsOrthogonal(link) && num != 480)
						{
							num2 += 15f;
							if (num2 >= 360f)
							{
								num2 -= 360f;
							}
						}
						if (num2 > num3 && num2 < 180f - num3)
						{
							return 90f;
						}
						if (num2 >= 180f - num3 && num2 <= 180f + num3)
						{
							return 180f;
						}
						if (num2 > 180f + num3 && num2 < 360f - num3)
						{
							return 270f;
						}
						return 0f;
					}
				}
				}
			}
			}
		}

		/// <summary>
		/// This convenience method decides if the given link is supposed to
		/// be drawn with all of its segments orthogonal.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>true if the link is believed to be drawn with orthogonal segments</returns>
		public virtual bool IsOrthogonal(IGoLink link)
		{
			return GoLink.IsOrtho(link);
		}

		/// <summary>
		/// Determine the angle the port at the other end makes with this port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>the angle in degrees</returns>
		public virtual float GetAngle(IGoLink link)
		{
			if (link == null)
			{
				return 0f;
			}
			IGoPort goPort = link.GetOtherPort(this);
			if (goPort == null)
			{
				if (link.FromPort != null && link.FromPort.GoObject != null && link.FromPort.GoObject.Bounds == Bounds)
				{
					goPort = link.ToPort;
				}
				else if (link.ToPort != null && link.ToPort.GoObject != null && link.ToPort.GoObject.Bounds == Bounds)
				{
					goPort = link.FromPort;
				}
			}
			if (goPort == null)
			{
				return 0f;
			}
			GoObject goObject = goPort.GoObject;
			if (goObject == null)
			{
				return 0f;
			}
			PointF pointF = goObject.Center;
			PointF center = base.Center;
			GoLink goLink = link as GoLink;
			if (goLink == null)
			{
				GoLabeledLink goLabeledLink = link as GoLabeledLink;
				if (goLabeledLink != null)
				{
					goLink = goLabeledLink.RealLink;
				}
			}
			if (goLink != null && goLink.PointsCount > 0)
			{
				pointF = ((goLink.FromPort != goPort) ? goLink.GetPoint(checked(goLink.PointsCount - 1)) : goLink.GetPoint(0));
			}
			return GoStroke.GetAngle(pointF.X - center.X, pointF.Y - center.Y);
		}

		/// <summary>
		/// Determine the direction for a link at this port.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>the angle in degrees</returns>
		public virtual float GetDirection(IGoLink link)
		{
			if (link == null)
			{
				return 0f;
			}
			if (link.FromPort == this)
			{
				return GetFromLinkDir(link);
			}
			return GetToLinkDir(link);
		}

		/// <summary>
		/// Be a little smarter about customizing the end-segment-length
		/// value used for the multiple links connected to the same side,
		/// to reduce the chances of overlapping segments of orthogonal
		/// links in some circumstances.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>
		/// By default, the value of <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />,
		/// or a larger value to help position the adjacent segment of
		/// standard orthogonal links at different positions.
		/// </returns>
		public override float GetFromEndSegmentLength(IGoLink link)
		{
			return GetEndSegmentLength(link);
		}

		/// <summary>
		/// Be a little smarter about customizing the end-segment-length
		/// value used for the multiple links connected to the same side,
		/// to reduce the chances of overlapping segments of orthogonal
		/// links in some circumstances.
		/// </summary>
		/// <param name="link"></param>
		/// <returns>
		/// By default, the value of <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />,
		/// or a larger value to help position the adjacent segment of
		/// standard orthogonal links at different positions.
		/// </returns>
		public override float GetToEndSegmentLength(IGoLink link)
		{
			return GetEndSegmentLength(link);
		}

		private float GetEndSegmentLength(IGoLink link)
		{
			float endSegmentLength = EndSegmentLength;
			if (link == null)
			{
				return endSegmentLength;
			}
			if (LinkPointsSpread)
			{
				GoBoxPortLinkInfo[] linkInfos = GetLinkInfos();
				foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkInfos)
				{
					if (goBoxPortLinkInfo.Link == link)
					{
						return goBoxPortLinkInfo.EndSegmentLength;
					}
				}
			}
			return endSegmentLength;
		}

		internal float GetEndSegmentLength(GoBoxPortLinkInfo info)
		{
			float endSegmentLength = EndSegmentLength;
			int num = info.IndexOnSide;
			if (num < 0)
			{
				return endSegmentLength;
			}
			int countOnSide = info.CountOnSide;
			if (countOnSide <= 1)
			{
				return endSegmentLength;
			}
			IGoLink link = info.Link;
			if (!GoLink.IsOrtho(link))
			{
				return endSegmentLength;
			}
			IGoPort otherPort = link.GetOtherPort(this);
			if (otherPort == null)
			{
				return endSegmentLength;
			}
			GoObject goObject = otherPort.GoObject;
			if (goObject == null)
			{
				return endSegmentLength;
			}
			PointF center = goObject.Center;
			PointF center2 = base.Center;
			checked
			{
				if (info.Side == 256 || info.Side == 128)
				{
					num = countOnSide - 1 - num;
				}
				float num2 = 8f;
				bool flag = info.Side == 256 || info.Side == 64;
				if (flag ? (center.Y < center2.Y) : (center.X < center2.X))
				{
					return endSegmentLength + (float)num * num2;
				}
				if (flag ? (center.Y == center2.Y) : (center.X == center2.X))
				{
					return endSegmentLength;
				}
				return endSegmentLength + (float)(countOnSide - 1 - num) * num2;
			}
		}

		internal PointF GetSideLinkPoint(GoBoxPortLinkInfo info)
		{
			GoObject goObject = PortObject;
			if (goObject == null || goObject.Layer == null)
			{
				goObject = this;
			}
			PointF spotLocation;
			PointF spotLocation2;
			switch (info.Side)
			{
			case 128:
				spotLocation = goObject.GetSpotLocation(8);
				spotLocation2 = goObject.GetSpotLocation(16);
				break;
			case 256:
				spotLocation = goObject.GetSpotLocation(16);
				spotLocation2 = goObject.GetSpotLocation(2);
				break;
			case 32:
				spotLocation = goObject.GetSpotLocation(2);
				spotLocation2 = goObject.GetSpotLocation(4);
				break;
			default:
				spotLocation = goObject.GetSpotLocation(4);
				spotLocation2 = goObject.GetSpotLocation(8);
				break;
			}
			float num = spotLocation2.X - spotLocation.X;
			float num2 = spotLocation2.Y - spotLocation.Y;
			float num3 = ((float)info.IndexOnSide + 1f) / ((float)info.CountOnSide + 1f);
			return new PointF(spotLocation.X + num * num3, spotLocation.Y + num2 * num3);
		}

		internal GoBoxPortLinkInfo[] GetLinkInfos()
		{
			bool flag = myReSort;
			myReSort = false;
			if (mySortedLinks == null || mySortedLinks.Length != LinksCount)
			{
				mySortedLinks = new GoBoxPortLinkInfo[LinksCount];
				flag = true;
			}
			checked
			{
				if (flag && !myRespreading)
				{
					bool flag2 = myRespreading;
					myRespreading = true;
					int num = 0;
					foreach (IGoLink link in Links)
					{
						float direction = GetDirection(link);
						float num2 = GetAngle(link);
						int num3;
						if (direction != 0f)
						{
							num3 = ((direction == 90f) ? 128 : ((direction != 180f) ? 32 : 256));
						}
						else
						{
							num3 = 64;
							if (num2 > 180f)
							{
								num2 -= 360f;
							}
						}
						GoBoxPortLinkInfo goBoxPortLinkInfo = mySortedLinks[num];
						if (goBoxPortLinkInfo == null)
						{
							mySortedLinks[num] = new GoBoxPortLinkInfo(link, num2, num3);
						}
						else
						{
							goBoxPortLinkInfo.Link = link;
							goBoxPortLinkInfo.Angle = num2;
							goBoxPortLinkInfo.Side = num3;
						}
						num++;
					}
					SortLinkInfos(mySortedLinks);
					int num4 = mySortedLinks.Length;
					int num5 = -1;
					int num6 = 0;
					for (num = 0; num < num4; num++)
					{
						GoBoxPortLinkInfo goBoxPortLinkInfo2 = mySortedLinks[num];
						if (goBoxPortLinkInfo2.Side != num5)
						{
							num5 = goBoxPortLinkInfo2.Side;
							num6 = 0;
						}
						goBoxPortLinkInfo2.IndexOnSide = num6;
						num6++;
					}
					num5 = -1;
					num6 = 0;
					for (num = num4 - 1; num >= 0; num--)
					{
						GoBoxPortLinkInfo goBoxPortLinkInfo3 = mySortedLinks[num];
						if (goBoxPortLinkInfo3.Side != num5)
						{
							num5 = goBoxPortLinkInfo3.Side;
							num6 = goBoxPortLinkInfo3.IndexOnSide + 1;
						}
						goBoxPortLinkInfo3.CountOnSide = num6;
					}
					AssignLinkPoints(mySortedLinks);
					AssignEndSegmentLengths(mySortedLinks);
					myRespreading = flag2;
				}
				return mySortedLinks;
			}
		}

		/// <summary>
		/// Sort an array of angle and side information about the links connected to this port.
		/// </summary>
		/// <param name="linkinfos">an array of <see cref="T:Northwoods.Go.GoBoxPortLinkInfo" /> that is modified</param>
		/// <remarks>
		/// By default this just sorts by <see cref="P:Northwoods.Go.GoBoxPortLinkInfo.Side" /> group, and by
		/// <see cref="P:Northwoods.Go.GoBoxPortLinkInfo.Angle" /> for each side.
		/// </remarks>
		protected virtual void SortLinkInfos(GoBoxPortLinkInfo[] linkinfos)
		{
			Array.Sort(linkinfos, 0, linkinfos.Length, myComparer);
		}

		/// <summary>
		/// Given a sorted array of angle and side information about the links connected to this port,
		/// assign the actual <see cref="T:Northwoods.Go.GoBoxPortLinkInfo" />.<see cref="P:Northwoods.Go.GoBoxPortLinkInfo.LinkPoint" />.
		/// </summary>
		/// <param name="linkinfos">an array of <see cref="T:Northwoods.Go.GoBoxPortLinkInfo" /></param>
		/// <remarks>
		/// By default this just spreads the link points evenly along each side.
		/// </remarks>
		protected virtual void AssignLinkPoints(GoBoxPortLinkInfo[] linkinfos)
		{
			foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkinfos)
			{
				goBoxPortLinkInfo.LinkPoint = GetSideLinkPoint(goBoxPortLinkInfo);
			}
		}

		/// <summary>
		/// Given a sorted array of angle, side, and link-point information about the links
		/// connected to this port, assign the actual
		/// <see cref="T:Northwoods.Go.GoBoxPortLinkInfo" />.<see cref="P:Northwoods.Go.GoBoxPortLinkInfo.EndSegmentLength" />.
		/// </summary>
		/// <param name="linkinfos"></param>
		/// <remarks>
		/// By default this just specifies shorter values for links at the ends of each side,
		/// and longer values for links in the middle of each side.
		/// </remarks>
		protected virtual void AssignEndSegmentLengths(GoBoxPortLinkInfo[] linkinfos)
		{
			foreach (GoBoxPortLinkInfo goBoxPortLinkInfo in linkinfos)
			{
				goBoxPortLinkInfo.EndSegmentLength = GetEndSegmentLength(goBoxPortLinkInfo);
			}
		}

		/// <summary>
		/// Reroute all of the links connected at this port,
		/// if <see cref="P:Northwoods.Go.GoBoxPort.LinkPointsSpread" /> is true, so that
		/// all of the link points are up-to-date.
		/// </summary>
		public virtual void UpdateAllLinkPoints()
		{
			bool linkPointsSpread = LinkPointsSpread;
			if (linkPointsSpread)
			{
				LinksOnPortChanged(2211, 0, linkPointsSpread, GoObject.NullRect, 0, linkPointsSpread, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Sets an internal flag that causes the link positions to be recalculated when link routing occurs.
		/// </summary>
		public override void LinksOnPortChanged(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			base.LinksOnPortChanged(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			if (LinkPointsSpread && !myReSort)
			{
				myReSort = true;
			}
		}

		/// <summary>
		/// Handle undo and redo changes.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2211:
				LinkPointsSpread = (bool)e.GetValue(undo);
				break;
			case 2212:
				FromSides = e.GetInt(undo);
				break;
			case 2213:
				ToSides = e.GetInt(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
