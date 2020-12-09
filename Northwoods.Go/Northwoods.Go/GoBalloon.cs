using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// A GoBalloon is a kind of <see cref="T:Northwoods.Go.GoComment" /> that is associated with another object.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoComment.Background" /> object is implemented as a <see cref="T:Northwoods.Go.GoPolygon" />.
	/// It is shaped to "point" at the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> object.
	/// The actual <b>PointF</b> point is calculated by <see cref="M:Northwoods.Go.GoBalloon.ComputeAnchorPoint" />.
	/// When there is no <see cref="P:Northwoods.Go.GoBalloon.Anchor" />, it uses <see cref="P:Northwoods.Go.GoBalloon.UnanchoredOffset" />
	/// to determine a point near the <see cref="P:Northwoods.Go.GoComment.Label" />.
	/// </para>
	/// <para>
	/// When <see cref="P:Northwoods.Go.GoBalloon.Reanchorable" /> is true, an additional selection handle is
	/// placed at the "point", to allow users to interactively choose another object.
	/// You can override <see cref="M:Northwoods.Go.GoBalloon.PickNewAnchor(System.Drawing.PointF,Northwoods.Go.GoView,Northwoods.Go.GoInputState)" /> to determine which objects
	/// may be chosen as new anchors for this balloon comment.  By default it allows
	/// any selectable document object, but you could just have it return objects
	/// of a certain type, depending on the state of your application.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoBalloon : GoComment, IGoRoutable
	{
		private const int flagReanchorable = 33554432;

		private const int flagNoClearAnchors = 67108864;

		/// <summary>
		/// This is a special handle ID, to allow the user to reanchor the balloon.
		/// </summary>
		public const int AnchorHandle = 1026;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> property.
		/// </summary>
		public const int ChangedAnchor = 2310;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBalloon.Corner" /> property.
		/// </summary>
		internal const int ChangedCorner = 2311;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBalloon.BaseWidth" /> property.
		/// </summary>
		public const int ChangedBaseWidth = 2312;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBalloon.UnanchoredOffset" /> property.
		/// </summary>
		public const int ChangedUnanchoredOffset = 2313;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoBalloon.Reanchorable" /> property.
		/// </summary>
		public const int ChangedReanchorable = 2314;

		private GoObject myAnchor;

		private SizeF myCorner = new SizeF(4f, 4f);

		private float myBaseWidth = 30f;

		private SizeF myUnanchoredOffset = new SizeF(-30f, -30f);

		[NonSerialized]
		private GoObject myTemporaryAnchor;

		/// <summary>
		/// Gets or sets the offset from the <see cref="P:Northwoods.Go.GoComment.Label" /> for the result of <see cref="M:Northwoods.Go.GoBalloon.ComputeAnchorPoint" />
		/// when there is no <see cref="P:Northwoods.Go.GoBalloon.Anchor" />.
		/// </summary>
		/// <value>
		/// This defaults to -30 x -30.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoBalloon.ComputeAnchorPoint" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The relative distance from the top-left corner of the Label to the point, when there is no Anchor object")]
		public SizeF UnanchoredOffset
		{
			get
			{
				return myUnanchoredOffset;
			}
			set
			{
				SizeF sizeF = myUnanchoredOffset;
				if (sizeF != value)
				{
					myUnanchoredOffset = value;
					Changed(2313, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						UpdateRoute();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the base of the triangular region of the balloon,
		/// near the text label.
		/// </summary>
		/// <value>
		/// This value defaults to 30.  The value must be greater than zero.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(30)]
		[Description("The width of the base of the balloon's pointer")]
		public virtual float BaseWidth
		{
			get
			{
				return myBaseWidth;
			}
			set
			{
				float num = myBaseWidth;
				if (num != value && value > 0f)
				{
					myBaseWidth = value;
					Changed(2312, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						UpdateRoute();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum radial width and height of each corner.
		/// </summary>
		/// <value>
		/// The default value is 4x4.
		/// </value>
		private SizeF Corner
		{
			get
			{
				return myCorner;
			}
			set
			{
				SizeF sizeF = myCorner;
				if (sizeF != value && value.Width >= 0f && value.Height >= 0f)
				{
					myCorner = value;
					Changed(2311, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						UpdateRoute();
					}
				}
			}
		}

		/// <summary>
		/// This is the object that the balloon always seems to be pointing to.
		/// </summary>
		[Description("The object that the balloon comment is pointing at")]
		public virtual GoObject Anchor
		{
			get
			{
				return myAnchor;
			}
			set
			{
				GoObject goObject = myAnchor;
				if (goObject != value)
				{
					goObject?.RemoveObserver(this);
					myAnchor = value;
					value?.AddObserver(this);
					Changed(2310, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing && !base.BeingRemoved)
					{
						UpdateRoute();
					}
				}
			}
		}

		internal bool NoClearAnchors
		{
			get
			{
				return (base.InternalFlags & 0x4000000) != 0;
			}
			set
			{
				if (value)
				{
					base.InternalFlags |= 67108864;
				}
				else
				{
					base.InternalFlags &= -67108865;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user may reanchor this balloon comment,
		/// by reattaching it or leaving it unattached.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the user may reanchor this balloon.")]
		public virtual bool Reanchorable
		{
			get
			{
				return (base.InternalFlags & 0x2000000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x2000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 33554432;
					}
					else
					{
						base.InternalFlags &= -33554433;
					}
					Changed(2314, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> object may or may not be copied;
		/// let <see cref="M:Northwoods.Go.GoBalloon.CopyObjectDelayed(Northwoods.Go.GoCopyDictionary,Northwoods.Go.GoObject)" /> handle it.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoBalloon obj = (GoBalloon)base.CopyObject(env);
			if (obj != null)
			{
				env.Delayeds.Add(this);
			}
			return obj;
		}

		/// <summary>
		/// Make sure the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> is the copied object, and update the
		/// background polygon's shape appropriately.
		/// </summary>
		/// <param name="env"></param>
		/// <param name="newobj"></param>
		public override void CopyObjectDelayed(GoCopyDictionary env, GoObject newobj)
		{
			base.CopyObjectDelayed(env, newobj);
			GoBalloon obj = (GoBalloon)newobj;
			obj.myAnchor = (env[myAnchor] as GoObject);
			obj.UpdateRoute();
		}

		/// <summary>
		/// Change the background from a rectangle to a polygon that stretches out towards
		/// the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> object.
		/// </summary>
		/// <returns></returns>
		protected override GoObject CreateBackground()
		{
			return new GoPolygon
			{
				Shadowed = true,
				Selectable = false,
				Pen = GoShape.Pens_LightGray,
				Brush = GoShape.Brushes_LemonChiffon
			};
		}

		/// <summary>
		/// The bounds of a balloon comment are just the bounds of the label plus the margins.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// The bounds do not include the part of the background polygon that points to the anchor.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoBalloon.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />
		protected override RectangleF ComputeBounds()
		{
			GoText label = Label;
			if (label == null)
			{
				return base.ComputeBounds();
			}
			SizeF topLeftMargin = TopLeftMargin;
			SizeF bottomRightMargin = BottomRightMargin;
			return new RectangleF(label.Left - topLeftMargin.Width, label.Top - topLeftMargin.Height, label.Width + topLeftMargin.Width + bottomRightMargin.Width, label.Height + topLeftMargin.Height + bottomRightMargin.Height);
		}

		/// <summary>
		/// Since the bounds include only part of the background polygon, the area that
		/// needs repainting is really determined by the polygon.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns>a rectangle in document coordinates that includes the background object</returns>
		/// <seealso cref="M:Northwoods.Go.GoBalloon.ComputeBounds" />
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			GoObject background = Background;
			if (background != null)
			{
				rect = GoObject.UnionRect(rect, background.Bounds);
				rect = background.ExpandPaintBounds(rect, view);
			}
			return rect;
		}

		/// <summary>
		/// Moving a balloon should change the size and shape, because the
		/// anchor might not be moving along.
		/// </summary>
		/// <param name="old"></param>
		protected override void MoveChildren(RectangleF old)
		{
			base.MoveChildren(old);
			UpdateRoute();
		}

		/// <summary>
		/// Make sure the background polygon is moved along with the text label
		/// and shaped so that it points at the anchor.
		/// </summary>
		/// <param name="childchanged"></param>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (base.Initializing)
			{
				return;
			}
			GoText label = Label;
			if (label == null)
			{
				return;
			}
			GoPolygon goPolygon = Background as GoPolygon;
			if (goPolygon != null && childchanged != goPolygon)
			{
				SizeF topLeftMargin = TopLeftMargin;
				SizeF bottomRightMargin = BottomRightMargin;
				RectangleF rectangleF = new RectangleF(label.Left - topLeftMargin.Width, label.Top - topLeftMargin.Height, label.Width + topLeftMargin.Width + bottomRightMargin.Width, label.Height + topLeftMargin.Height + bottomRightMargin.Height);
				SizeF corner = Corner;
				float num = corner.Width;
				if (num > rectangleF.Width / 2f)
				{
					num = rectangleF.Width / 2f;
				}
				float num2 = corner.Height;
				if (num2 > rectangleF.Height / 2f)
				{
					num2 = rectangleF.Height / 2f;
				}
				float x = rectangleF.X;
				float y = rectangleF.Y;
				float x2 = x + num;
				float y2 = y + num2;
				float num3 = x + rectangleF.Width / 2f;
				float num4 = y + rectangleF.Height / 2f;
				float x3 = x + rectangleF.Width - num;
				float y3 = y + rectangleF.Height - num2;
				float num5 = x + rectangleF.Width;
				float num6 = y + rectangleF.Height;
				RectangleF bounds = goPolygon.Bounds;
				bool suspendsUpdates = goPolygon.SuspendsUpdates;
				if (!suspendsUpdates)
				{
					goPolygon.Changing(1412);
				}
				goPolygon.SuspendsUpdates = true;
				goPolygon.ClearPoints();
				float num7 = Math.Min(rectangleF.Width - num, BaseWidth);
				float num8 = Math.Min(rectangleF.Height - num2, BaseWidth);
				float left = label.Left;
				float top = label.Top;
				float right = label.Right;
				float bottom = label.Bottom;
				PointF center = label.Center;
				PointF pointF = (myTemporaryAnchor == null) ? ComputeAnchorPoint() : myTemporaryAnchor.Center;
				label.GetNearestIntersectionPoint(pointF, center, out PointF result);
				if (result.Y <= top && result.X < num3)
				{
					goPolygon.AddPoint(x, y);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(x + num7, y);
				}
				else
				{
					goPolygon.AddPoint(x2, y);
				}
				if (result.Y <= top && result.X >= num3)
				{
					goPolygon.AddPoint(num5 - num7, y);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(num5, y);
				}
				else
				{
					goPolygon.AddPoint(x3, y);
				}
				if ((result.X >= right) & (result.Y < num4))
				{
					goPolygon.AddPoint(num5, y);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(num5, y + num8);
				}
				else
				{
					goPolygon.AddPoint(num5, y2);
				}
				if ((result.X >= right) & (result.Y >= num4))
				{
					goPolygon.AddPoint(num5, num6 - num8);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(num5, num6);
				}
				else
				{
					goPolygon.AddPoint(num5, y3);
				}
				if (result.Y >= bottom && result.X >= num3)
				{
					goPolygon.AddPoint(num5, num6);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(num5 - num7, num6);
				}
				else
				{
					goPolygon.AddPoint(x3, num6);
				}
				if (result.Y >= bottom && result.X < num3)
				{
					goPolygon.AddPoint(x + num7, num6);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(x, num6);
				}
				else
				{
					goPolygon.AddPoint(x2, num6);
				}
				if (result.X <= left && result.Y >= num4)
				{
					goPolygon.AddPoint(x, num6);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(x, num6 - num8);
				}
				else
				{
					goPolygon.AddPoint(x, y3);
				}
				if (result.X <= left && result.Y < num4)
				{
					goPolygon.AddPoint(x, y + num8);
					goPolygon.AddPoint(pointF);
					goPolygon.AddPoint(x, y);
				}
				else
				{
					goPolygon.AddPoint(x, y2);
				}
				goPolygon.SuspendsUpdates = suspendsUpdates;
				if (!suspendsUpdates)
				{
					goPolygon.Changed(1412, 0, null, bounds, 0, null, goPolygon.Bounds);
				}
			}
		}

		/// <summary>
		/// Determine the end point of the balloon, near the <see cref="P:Northwoods.Go.GoBalloon.Anchor" />.
		/// </summary>
		/// <returns>
		/// Normally this returns the <see cref="M:Northwoods.Go.GoObject.GetNearestIntersectionPoint(System.Drawing.PointF,System.Drawing.PointF,System.Drawing.PointF@)" />
		/// of the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> from the <see cref="P:Northwoods.Go.GoComment.Label" />'s center.
		/// However, if there is no anchor, this method returns the point at the
		/// <see cref="P:Northwoods.Go.GoBalloon.UnanchoredOffset" /> from the label's Position.
		/// </returns>
		public virtual PointF ComputeAnchorPoint()
		{
			PointF p = (Label == null) ? base.Center : Label.Center;
			PointF center;
			if (Anchor != null)
			{
				center = Anchor.Center;
				if (Anchor.GetNearestIntersectionPoint(p, center, out PointF result))
				{
					return result;
				}
				return center;
			}
			center = ((Label != null) ? new PointF(Label.Left + UnanchoredOffset.Width, Label.Top + UnanchoredOffset.Height) : new PointF(p.X + UnanchoredOffset.Width, p.Y + UnanchoredOffset.Height));
			return center;
		}

		/// <summary>
		/// If the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> moves or resizes, we need to update the
		/// background polygon's shape.
		/// </summary>
		/// <param name="observed"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		protected override void OnObservedChanged(GoObject observed, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			base.OnObservedChanged(observed, subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
			if (observed != Anchor)
			{
				return;
			}
			switch (subhint)
			{
			case 1001:
				UpdateRoute();
				break;
			case 903:
				if (!NoClearAnchors)
				{
					Anchor = null;
				}
				break;
			}
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoBalloon.LayoutChildren(Northwoods.Go.GoObject)" /> to determine
		/// the new shape for this balloon's polygon background.
		/// </summary>
		public virtual void CalculateRoute()
		{
			LayoutChildren(null);
		}

		/// <summary>
		/// Request the reshaping of the balloon's polygon background.
		/// </summary>
		/// <remarks>
		/// If this is part of a <see cref="T:Northwoods.Go.GoDocument" />,
		/// this calls <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.UpdateRoute(Northwoods.Go.IGoRoutable)" /> in order to maybe delay
		/// the call to <see cref="M:Northwoods.Go.GoBalloon.CalculateRoute" /> (depending on the value of <see cref="P:Northwoods.Go.GoDocument.RoutingTime" />).
		/// If there is no <see cref="T:Northwoods.Go.GoDocument" />, this just calls <see cref="M:Northwoods.Go.GoBalloon.CalculateRoute" /> immediately.
		/// </remarks>
		public virtual void UpdateRoute()
		{
			GoDocument document = base.Document;
			if (document != null)
			{
				document.UpdateRoute(this);
			}
			else
			{
				CalculateRoute();
			}
		}

		/// <summary>
		/// If this balloon is removed from the document,
		/// set the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> property to null.
		/// </summary>
		/// <param name="oldlayer"></param>
		/// <param name="newlayer"></param>
		/// <param name="mainObj"></param>
		protected override void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
			base.OnLayerChanged(oldlayer, newlayer, mainObj);
			if (oldlayer != null && newlayer == null && !NoClearAnchors)
			{
				Anchor = null;
			}
		}

		/// <summary>
		/// This method is called repeatedly by <see cref="M:Northwoods.Go.GoBalloon.DoResize(Northwoods.Go.GoView,System.Drawing.RectangleF,System.Drawing.PointF,System.Int32,Northwoods.Go.GoInputState,System.Drawing.SizeF,System.Drawing.SizeF)" /> while
		/// the user is dragging the <see cref="F:Northwoods.Go.GoBalloon.AnchorHandle" /> resize handle.
		/// </summary>
		/// <param name="p">the point (in document coordinates) currently specified by the user's mouse</param>
		/// <param name="view"></param>
		/// <param name="evttype"></param>
		/// <remarks>
		/// When <paramref name="evttype" /> is <c>GoInputState.</c><see cref="F:Northwoods.Go.GoInputState.Finish" />,
		/// this calls <see cref="M:Northwoods.Go.GoView.PickObject(System.Boolean,System.Boolean,System.Drawing.PointF,System.Boolean)" /> to
		/// find the selectable document object at the given point <paramref name="p" />.
		/// The <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> is set to that object.
		/// If no object is found at that point, the <see cref="P:Northwoods.Go.GoBalloon.Anchor" /> is set to null
		/// and the <see cref="P:Northwoods.Go.GoBalloon.UnanchoredOffset" /> is set to the offset between that
		/// point and the <see cref="P:Northwoods.Go.GoComment.Label" />'s Position.
		/// If the user tries to reanchor this balloon to itself, no change is made.
		/// This method does nothing unless the <paramref name="evttype" /> is <see cref="F:Northwoods.Go.GoInputState.Finish" />.
		/// </remarks>
		protected virtual void PickNewAnchor(PointF p, GoView view, GoInputState evttype)
		{
			if (evttype != GoInputState.Finish)
			{
				return;
			}
			GoObject goObject = view.PickObject(doc: true, view: false, p, selectableOnly: true);
			if (goObject == this)
			{
				return;
			}
			Anchor = goObject;
			if (goObject == null)
			{
				if (Label != null)
				{
					UnanchoredOffset = GoTool.SubtractPoints(p, Label.Position);
				}
				else
				{
					UnanchoredOffset = GoTool.SubtractPoints(p, base.Position);
				}
			}
		}

		/// <summary>
		/// When resizing the <see cref="F:Northwoods.Go.GoBalloon.AnchorHandle" />, call
		/// <see cref="M:Northwoods.Go.GoBalloon.PickNewAnchor(System.Drawing.PointF,Northwoods.Go.GoView,Northwoods.Go.GoInputState)" /> to consider other objects as anchors for this balloon.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="origRect"></param>
		/// <param name="newPoint"></param>
		/// <param name="whichHandle"></param>
		/// <param name="evttype"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <remarks>
		/// <para>
		/// <see cref="M:Northwoods.Go.GoBalloon.PickNewAnchor(System.Drawing.PointF,Northwoods.Go.GoView,Northwoods.Go.GoInputState)" /> is called each time this method is called,
		/// as long as the handle is <see cref="F:Northwoods.Go.GoBalloon.AnchorHandle" />.
		/// The implementation of <see cref="M:Northwoods.Go.GoBalloon.PickNewAnchor(System.Drawing.PointF,Northwoods.Go.GoView,Northwoods.Go.GoInputState)" /> will actually set <see cref="P:Northwoods.Go.GoBalloon.Anchor" />
		/// when <paramref name="evttype" /> is <see cref="F:Northwoods.Go.GoInputState.Finish" />.
		/// </para>
		/// <para>
		/// Instead of reusing the resizing mechanism to reanchor a balloon,
		/// we may replace this mechanism with a separate reanchoring tool,
		/// if the need for more flexibility becomes apparent.
		/// </para>
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (whichHandle == 1026)
			{
				switch (evttype)
				{
				case GoInputState.Start:
					break;
				case GoInputState.Continue:
					if (myTemporaryAnchor == null)
					{
						GoObject goObject = new GoRectangle();
						PointF pointF = ComputeAnchorPoint();
						goObject.Bounds = new RectangleF(pointF.X, pointF.Y, 0f, 0f);
						myTemporaryAnchor = goObject;
					}
					myTemporaryAnchor.Position = newPoint;
					PickNewAnchor(newPoint, view, evttype);
					UpdateRoute();
					break;
				case GoInputState.Finish:
					myTemporaryAnchor = null;
					PickNewAnchor(newPoint, view, evttype);
					UpdateRoute();
					break;
				case GoInputState.Cancel:
					myTemporaryAnchor = null;
					PickNewAnchor(newPoint, view, evttype);
					break;
				}
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// When <see cref="P:Northwoods.Go.GoBalloon.Reanchorable" /> is true, add a special handle that lets the user change the <see cref="P:Northwoods.Go.GoBalloon.Anchor" />
		/// by dragging the end point of the balloon to another object.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			base.AddSelectionHandles(sel, selectedObj);
			if (Reanchorable)
			{
				IGoHandle handle = sel.CreateResizeHandle(this, selectedObj, ComputeAnchorPoint(), 1026, filled: true);
				MakeDiamondResizeHandle(handle, 1);
			}
		}

		/// <summary>
		/// Perform the usual undo and redo changes.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2310:
				Anchor = (GoObject)e.GetValue(undo);
				break;
			case 2311:
				Corner = e.GetSize(undo);
				break;
			case 2312:
				BaseWidth = e.GetFloat(undo);
				break;
			case 2313:
				UnanchoredOffset = e.GetSize(undo);
				break;
			case 2314:
				Reanchorable = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
