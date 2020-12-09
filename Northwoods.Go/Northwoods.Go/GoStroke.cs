using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An unfilled shape composed of lines or curves whose number and location can be specified.
	/// </summary>
	[Serializable]
	public class GoStroke : GoShape
	{
		[Serializable]
		internal sealed class ArrowInfo : ICloneable
		{
			internal const int flagFilled = 65536;

			internal const int flagStyleMask = 65535;

			internal float ArrowLength = 10f;

			internal float ShaftLength = 8f;

			internal float Width = 8f;

			private int myFlags = 65536;

			private PointF[] myPolyPoints;

			internal bool Filled
			{
				get
				{
					return (myFlags & 0x10000) != 0;
				}
				set
				{
					if (value)
					{
						myFlags |= 65536;
					}
					else
					{
						myFlags &= -65537;
					}
				}
			}

			internal GoStrokeArrowheadStyle Style
			{
				get
				{
					return (GoStrokeArrowheadStyle)(myFlags & 0xFFFF);
				}
				set
				{
					myFlags = ((myFlags & -65536) | (int)(value & (GoStrokeArrowheadStyle)65535));
				}
			}

			internal ArrowInfo()
			{
			}

			internal PointF[] GetPoly(int n)
			{
				if (myPolyPoints == null || myPolyPoints.Length < n)
				{
					myPolyPoints = new PointF[n];
				}
				return myPolyPoints;
			}

			public object Clone()
			{
				ArrowInfo arrowInfo = (ArrowInfo)MemberwiseClone();
				if (myPolyPoints != null)
				{
					arrowInfo.myPolyPoints = (PointF[])myPolyPoints.Clone();
				}
				return arrowInfo;
			}
		}

		private static float[] myIntersections = new float[50];

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int AddedPoint = 1201;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoStroke.AddedPoint" />.
		/// </summary>
		public const int ChangedAddPoint = 1201;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedPoint = 1202;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoStroke.RemovedPoint" />.
		/// </summary>
		public const int ChangedRemovePoint = 1202;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ModifiedPoint = 1203;

		/// <summary>
		/// This is a synonym for <see cref="F:Northwoods.Go.GoStroke.ModifiedPoint" />.
		/// </summary>
		public const int ChangedModifiedPoint = 1203;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ChangedAllPoints = 1204;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.Style" /> property.
		/// </summary>
		public const int ChangedStyle = 1205;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.Curviness" /> property.
		/// </summary>
		public const int ChangedCurviness = 1206;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.HighlightPen" /> property.
		/// </summary>
		public const int ChangedHighlightPen = 1236;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.Highlight" /> property.
		/// </summary>
		public const int ChangedHighlight = 1237;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" /> property.
		/// </summary>
		public const int ChangedHighlightWhenSelected = 1238;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrow" /> property.
		/// </summary>
		public const int ChangedToArrowHead = 1250;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrowLength" /> property.
		/// </summary>
		public const int ChangedToArrowLength = 1251;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" /> property.
		/// </summary>
		public const int ChangedToArrowShaftLength = 1252;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrowWidth" /> property.
		/// </summary>
		public const int ChangedToArrowWidth = 1253;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrowFilled" /> property.
		/// </summary>
		public const int ChangedToArrowFilled = 1254;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.ToArrowStyle" /> property.
		/// </summary>
		public const int ChangedToArrowStyle = 1255;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrow" /> property.
		/// </summary>
		public const int ChangedFromArrowHead = 1260;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrowLength" /> property.
		/// </summary>
		public const int ChangedFromArrowLength = 1261;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" /> property.
		/// </summary>
		public const int ChangedFromArrowShaftLength = 1262;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrowWidth" /> property.
		/// </summary>
		public const int ChangedFromArrowWidth = 1263;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrowFilled" /> property.
		/// </summary>
		public const int ChangedFromArrowFilled = 1264;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoStroke.FromArrowStyle" /> property.
		/// </summary>
		public const int ChangedFromArrowStyle = 1265;

		private const int LINE_FUZZ = 3;

		private const int DEFAULT_ARROW_POLYGON_SIDES = 4;

		private const float DEFAULT_ARROW_LENGTH = 10f;

		private const float DEFAULT_ARROW_SHAFT_LENGTH = 8f;

		private const float DEFAULT_ARROW_WIDTH = 8f;

		private const GoStrokeArrowheadStyle DEFAULT_ARROW_STYLE = GoStrokeArrowheadStyle.Polygon;

		private const int flagStrokeArrowStart = 1048576;

		private const int flagStrokeArrowEnd = 2097152;

		private const int flagStrokeHighlight = 4194304;

		private const int flagHighlightWhenSelected = 8388608;

		private GoStrokeStyle myStyle;

		private int myPointsCount;

		private PointF[] myPoints = new PointF[6];

		private ArrowInfo myToArrowInfo;

		private ArrowInfo myFromArrowInfo;

		private float myCurviness = 10f;

		private GoPenInfo myHighlightPenInfo;

		/// <summary>
		/// Gets or sets the kind of curve drawn using this stroke's points.
		/// </summary>
		/// <value>
		/// The initial value is <see cref="F:Northwoods.Go.GoStrokeStyle.Line" />
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoStrokeStyle.Line)]
		[Description("The kind of curve drawn using this stroke's points.")]
		public virtual GoStrokeStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				GoStrokeStyle goStrokeStyle = myStyle;
				if (goStrokeStyle != value)
				{
					myStyle = value;
					ResetPath();
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1205, 0, goStrokeStyle, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the number of points in this stroke.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoStroke.GetPoint(System.Int32)" />.
		/// <seealso cref="M:Northwoods.Go.GoStroke.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoStroke.AddPoint(System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoStroke.InsertPoint(System.Int32,System.Drawing.PointF)" />.
		/// <seealso cref="M:Northwoods.Go.GoStroke.RemovePoint(System.Int32)" />.
		[Category("Appearance")]
		[Description("The number of points in this stroke.")]
		public virtual int PointsCount => myPointsCount;

		/// <summary>
		/// Gets the index of the first point that should get a selection handle.
		/// </summary>
		/// <remarks>
		/// This normally returns <c>0</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.LastPickIndex" />
		/// <seealso cref="M:Northwoods.Go.GoStroke.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		[Category("Behavior")]
		[Description("The index of the first point getting a selection handle.")]
		public virtual int FirstPickIndex => 0;

		/// <summary>
		/// Gets the index of the last point that should get a selection handle.
		/// </summary>
		/// <remarks>
		/// This normally returns <c>PointsCount-1</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FirstPickIndex" />
		/// <seealso cref="M:Northwoods.Go.GoStroke.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		[Category("Behavior")]
		[Description("The index of the last point getting a selection handle.")]
		public virtual int LastPickIndex => checked(PointsCount - 1);

		/// <summary>
		/// Gets the approximate width of the region across the stroke,
		/// in addition to the pen width,
		/// that is still considered "inside" the stroke when picking.
		/// </summary>
		/// <value>This value defaults to 3, and should be non-negative.</value>
		/// <remarks>
		/// This allows users to pick thin links more easily.
		/// You can override this property in order to have it return a different value.
		/// </remarks>
		[Category("Behavior")]
		[Description("About how close users need to be to the stroke to pick it")]
		public virtual float PickMargin => 3f;

		/// <summary>
		/// Gets or sets whether an arrow is drawn at the end of this stroke.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// The arrow will be located near the last point of the stroke.
		/// By default this will be an arrowhead drawn pointing out
		/// with a thin pen and filled with the stroke's <see cref="T:System.Drawing.Brush" />,
		/// which defaults to <c>Brushes.Black</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an arrow is drawn at the end of this stroke.")]
		public virtual bool ToArrow
		{
			get
			{
				return (base.InternalFlags & 0x200000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x200000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 2097152;
					}
					else
					{
						base.InternalFlags &= -2097153;
					}
					ResetPath();
					Changed(1250, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the general shape of an arrowhead at the end of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoStrokeArrowheadStyle.Polygon" />.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		[Category("Appearance")]
		[DefaultValue(GoStrokeArrowheadStyle.Polygon)]
		[Description("Specifies the general shape of the arrowhead")]
		public virtual GoStrokeArrowheadStyle ToArrowStyle
		{
			get
			{
				if (myToArrowInfo != null)
				{
					return myToArrowInfo.Style;
				}
				return GoStrokeArrowheadStyle.Polygon;
			}
			set
			{
				GoStrokeArrowheadStyle goStrokeArrowheadStyle = (myToArrowInfo != null) ? myToArrowInfo.Style : GoStrokeArrowheadStyle.Polygon;
				if (goStrokeArrowheadStyle != value)
				{
					if (myToArrowInfo == null)
					{
						myToArrowInfo = new ArrowInfo();
					}
					myToArrowInfo.Style = value;
					ResetPath();
					Changed(1255, 0, goStrokeArrowheadStyle, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the point at the tip of the arrowhead at the end of this stroke.
		/// </summary>
		/// <value>
		/// The default value is the last point of this stroke.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The point at the tip of the arrowhead at the end of this stroke.")]
		public virtual PointF ToArrowEndPoint => GetPoint(checked(PointsCount - 1));

		/// <summary>
		/// Gets a point which specifies the direction the arrow is coming from.
		/// </summary>
		/// <value>
		/// The default value is the next-to-last point of this stroke.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("A point which specifies the direction the arrow is coming from.")]
		public virtual PointF ToArrowAnchorPoint
		{
			get
			{
				int pointsCount = PointsCount;
				return GetPoint(checked(pointsCount - 2));
			}
		}

		/// <summary>
		/// Gets or sets the length of the arrowhead at the end of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <c>10</c>.
		/// A negative value will produce an arrowhead that points inward instead of outward.
		/// </value>
		/// <remarks>
		/// The length is the distance measured along the shaft of the two "barbs" of
		/// the arrowhead from the end point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The length of the arrow at the end of this stroke, along the shaft from the end point to the widest point.")]
		public virtual float ToArrowLength
		{
			get
			{
				if (myToArrowInfo != null)
				{
					return myToArrowInfo.ArrowLength;
				}
				return 10f;
			}
			set
			{
				float num = (myToArrowInfo == null) ? 10f : myToArrowInfo.ArrowLength;
				if (num != value)
				{
					if (myToArrowInfo == null)
					{
						myToArrowInfo = new ArrowInfo();
					}
					myToArrowInfo.ArrowLength = value;
					ResetPath();
					Changed(1251, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the length of the shaft of the arrowhead at the end of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <c>8</c>.
		/// A negative value, along with a negative value for the arrow length, will produce an
		/// arrowhead that points inward instead of outward.
		/// </value>
		/// <remarks>
		/// The shaft length is the length of the arrowhead on the shaft.
		/// You can make the arrowhead diamond shaped by having the shaft length twice the
		/// arrow length.
		/// A shaft length of zero will result in an arrowhead with no interior to fill with
		/// the <see cref="T:System.Drawing.Brush" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The length of the arrow along the shaft at the end of this stroke.")]
		public virtual float ToArrowShaftLength
		{
			get
			{
				if (myToArrowInfo != null)
				{
					return myToArrowInfo.ShaftLength;
				}
				return 8f;
			}
			set
			{
				float num = (myToArrowInfo == null) ? 8f : myToArrowInfo.ShaftLength;
				if (num != value)
				{
					if (myToArrowInfo == null)
					{
						myToArrowInfo = new ArrowInfo();
					}
					myToArrowInfo.ShaftLength = value;
					ResetPath();
					Changed(1252, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum width of the arrowhead at the end of this stroke.
		/// </summary>
		/// <value>
		/// The value is measured in document coordinates and must be non-negative.
		/// The default value is <c>8</c>.
		/// </value>
		/// <remarks>
		/// Smaller values make the arrowhead sharper.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The width of the arrowhead at the widest point.")]
		public virtual float ToArrowWidth
		{
			get
			{
				if (myToArrowInfo != null)
				{
					return myToArrowInfo.Width;
				}
				return 8f;
			}
			set
			{
				float num = (myToArrowInfo == null) ? 8f : myToArrowInfo.Width;
				if (num != value)
				{
					if (myToArrowInfo == null)
					{
						myToArrowInfo = new ArrowInfo();
					}
					myToArrowInfo.Width = value;
					Changed(1253, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the arrowhead at the end of this stroke is filled with
		/// the brush.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowWidth" />
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the arrowhead is filled with the stroke's brush")]
		public virtual bool ToArrowFilled
		{
			get
			{
				if (myToArrowInfo != null)
				{
					return myToArrowInfo.Filled;
				}
				return true;
			}
			set
			{
				bool flag = myToArrowInfo == null || myToArrowInfo.Filled;
				if (flag != value)
				{
					if (myToArrowInfo == null)
					{
						myToArrowInfo = new ArrowInfo();
					}
					myToArrowInfo.Filled = value;
					Changed(1254, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether an arrow is drawn at the start of this stroke.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// The arrow will be located near the first point of the stroke.
		/// By default this will be an arrowhead drawn pointing out
		/// with a thin pen and filled with the stroke's <see cref="T:System.Drawing.Brush" />,
		/// which defaults to <c>Brushes.Black</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether an arrow is drawn at the start of this stroke.")]
		public virtual bool FromArrow
		{
			get
			{
				return (base.InternalFlags & 0x100000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x100000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 1048576;
					}
					else
					{
						base.InternalFlags &= -1048577;
					}
					ResetPath();
					Changed(1260, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the general shape of an arrowhead at the start of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoStrokeArrowheadStyle.Polygon" />.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.ToArrowStyle" />
		[Category("Appearance")]
		[DefaultValue(GoStrokeArrowheadStyle.Polygon)]
		[Description("Specifies the general shape of the arrowhead")]
		public virtual GoStrokeArrowheadStyle FromArrowStyle
		{
			get
			{
				if (myFromArrowInfo != null)
				{
					return myFromArrowInfo.Style;
				}
				return GoStrokeArrowheadStyle.Polygon;
			}
			set
			{
				GoStrokeArrowheadStyle goStrokeArrowheadStyle = (myFromArrowInfo != null) ? myFromArrowInfo.Style : GoStrokeArrowheadStyle.Polygon;
				if (goStrokeArrowheadStyle != value)
				{
					if (myFromArrowInfo == null)
					{
						myFromArrowInfo = new ArrowInfo();
					}
					myFromArrowInfo.Style = value;
					ResetPath();
					Changed(1265, 0, goStrokeArrowheadStyle, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the point at the tip of the arrowhead at the start of this stroke.
		/// </summary>
		/// <value>
		/// The default value is the last point of this stroke.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowFilled" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The point at the tip of the arrow at the start of this stroke.")]
		public virtual PointF FromArrowEndPoint => GetPoint(0);

		/// <summary>
		/// Gets a point which specifies the direction the arrow is coming from.
		/// </summary>
		/// <value>
		/// The default value is the second point of this stroke.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowFilled" />
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("A point specifying the direction from which comes the arrow at the start of this stroke.")]
		public virtual PointF FromArrowAnchorPoint => GetPoint(1);

		/// <summary>
		/// Gets or sets the length of the arrowhead at the start of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <c>10</c>.
		/// A negative value will produce an arrowhead that points inward instead of outward.
		/// </value>
		/// <remarks>
		/// The length is the distance measured along the shaft of the two "barbs" of
		/// the arrowhead from the end point.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("The length of the arrowhead at the start of this stroke, along the shaft from the end point to the widest point.")]
		public virtual float FromArrowLength
		{
			get
			{
				if (myFromArrowInfo != null)
				{
					return myFromArrowInfo.ArrowLength;
				}
				return 10f;
			}
			set
			{
				float num = (myFromArrowInfo == null) ? 10f : myFromArrowInfo.ArrowLength;
				if (num != value)
				{
					if (myFromArrowInfo == null)
					{
						myFromArrowInfo = new ArrowInfo();
					}
					myFromArrowInfo.ArrowLength = value;
					ResetPath();
					Changed(1261, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the length of the shaft of the arrowhead at the start of this stroke.
		/// </summary>
		/// <value>
		/// The default value is <c>8</c>.
		/// A negative value, along with a negative value for the arrow length, will produce an
		/// arrowhead that points inward instead of outward.
		/// </value>
		/// <remarks>
		/// The shaft length is the length of the arrowhead on the shaft.
		/// You can make the arrowhead diamond shaped by having the shaft length twice the
		/// arrow length.
		/// A shaft length of zero will result in an arrowhead with no interior to fill with
		/// the <see cref="T:System.Drawing.Brush" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The length of the arrow along the shaft at the start of this stroke.")]
		public virtual float FromArrowShaftLength
		{
			get
			{
				if (myFromArrowInfo != null)
				{
					return myFromArrowInfo.ShaftLength;
				}
				return 8f;
			}
			set
			{
				float num = (myFromArrowInfo == null) ? 8f : myFromArrowInfo.ShaftLength;
				if (num != value)
				{
					if (myFromArrowInfo == null)
					{
						myFromArrowInfo = new ArrowInfo();
					}
					myFromArrowInfo.ShaftLength = value;
					ResetPath();
					Changed(1262, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum width of the arrowhead at the start of this stroke.
		/// </summary>
		/// <value>
		/// The value is measured in document coordinates and must be non-negative.
		/// The default value is <c>8</c>.
		/// </value>
		/// <remarks>
		/// Smaller values make the arrowhead sharper.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowFilled" />
		[Category("Appearance")]
		[DefaultValue(8f)]
		[Description("The width at its widest point of the arrowhead at the start of this stroke.")]
		public virtual float FromArrowWidth
		{
			get
			{
				if (myFromArrowInfo != null)
				{
					return myFromArrowInfo.Width;
				}
				return 8f;
			}
			set
			{
				float num = (myFromArrowInfo == null) ? 8f : myFromArrowInfo.Width;
				if (num != value)
				{
					if (myFromArrowInfo == null)
					{
						myFromArrowInfo = new ArrowInfo();
					}
					myFromArrowInfo.Width = value;
					Changed(1263, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the arrowhead at the start of this stroke is filled with
		/// the brush.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrow" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowStyle" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowEndPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowAnchorPoint" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowShaftLength" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.FromArrowWidth" />
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the arrowhead is filled with the stroke's brush")]
		public virtual bool FromArrowFilled
		{
			get
			{
				if (myFromArrowInfo != null)
				{
					return myFromArrowInfo.Filled;
				}
				return true;
			}
			set
			{
				bool flag = myFromArrowInfo == null || myFromArrowInfo.Filled;
				if (flag != value)
				{
					if (myFromArrowInfo == null)
					{
						myFromArrowInfo = new ArrowInfo();
					}
					myFromArrowInfo.Filled = value;
					Changed(1264, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the line drawn as the highlight for the stroke.
		/// </summary>
		/// <value>
		/// The value must be non-negative.
		/// The default value is 0, meaning a one-pixel wide line at any scale.
		/// Values other than zero cause slower drawing.
		/// </value>
		/// <remarks>
		/// If there is no <see cref="T:System.Drawing.Pen" />, setting this property might have no effect.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.PenWidth" />
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to highlight the stroke.")]
		public virtual float HighlightPenWidth
		{
			get
			{
				return HighlightPenInfo?.Width ?? 0f;
			}
			set
			{
				GoPenInfo goPenInfo = HighlightPenInfo;
				float num = 0f;
				if (goPenInfo != null)
				{
					num = goPenInfo.Width;
				}
				else
				{
					goPenInfo = GoShape.PenInfo_Black;
				}
				if (num != value)
				{
					GoPenInfo goPenInfo2 = new GoPenInfo(goPenInfo);
					goPenInfo2.Width = value;
					HighlightPenInfo = goPenInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the <see cref="P:Northwoods.Go.GoStroke.HighlightPen" />.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoStroke.HighlightPen" />, or <b>Color.Empty</b> if there is no pen.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="T:System.Drawing.Pen" /> to null.
		/// </value>
		[Category("Appearance")]
		[Description("The color of the pen used to highlight the stroke.")]
		public virtual Color HighlightPenColor
		{
			get
			{
				return HighlightPenInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoPenInfo highlightPenInfo = HighlightPenInfo;
				GoPenInfo goPenInfo = null;
				if (highlightPenInfo != null)
				{
					if (highlightPenInfo.Color == value)
					{
						return;
					}
					if (value != Color.Empty)
					{
						goPenInfo = new GoPenInfo(highlightPenInfo);
						goPenInfo.Color = value;
					}
				}
				else if (value != Color.Empty)
				{
					goPenInfo = new GoPenInfo();
					goPenInfo.Width = HighlightPenWidth;
					goPenInfo.Color = value;
				}
				HighlightPenInfo = goPenInfo;
			}
		}

		/// <summary>
		/// Gets how rounded the corners are for adjacent line segments when the
		/// stroke style is <see cref="F:Northwoods.Go.GoStrokeStyle.RoundedLine" />.
		/// </summary>
		/// <value>
		/// This describes the maximum radius of rounded corners, in document coordinates.
		/// This also is used to determine the offset distance for the control points of
		/// a <see cref="F:Northwoods.Go.GoStrokeStyle.Bezier" /> style link when connecting two ports
		/// whose spots are both <see cref="F:Northwoods.Go.GoObject.NoSpot" />.
		/// This defaults to 10.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(10f)]
		[Description("How rounded corners are for strokes of style RoundedLine and how curved Bezier links are.")]
		public virtual float Curviness
		{
			get
			{
				return myCurviness;
			}
			set
			{
				float num = myCurviness;
				if (num != value)
				{
					myCurviness = value;
					ResetPath();
					Changed(1206, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this stroke displays a highlight along its path.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoStroke.HighlightPen" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether a highlight is shown along the path of this stroke.")]
		public virtual bool Highlight
		{
			get
			{
				return (base.InternalFlags & 0x400000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x400000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 4194304;
					}
					else
					{
						base.InternalFlags &= -4194305;
					}
					Changed(1237, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the pen used to draw a highlight for this stroke.
		/// </summary>
		/// <value>
		/// This property defaults to null.
		/// </value>
		/// <remarks>
		/// You must not modify the pen after you have assigned it.
		/// Setting <see cref="P:Northwoods.Go.GoStroke.Highlight" /> to true will not result in a visible
		/// markup until you specify a <c>Pen</c> for this property.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" />
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The pen used to draw the highlight.")]
		public virtual Pen HighlightPen
		{
			get
			{
				if (HighlightPenInfo != null)
				{
					return HighlightPenInfo.GetPen();
				}
				return null;
			}
			set
			{
				HighlightPenInfo = GoShape.GetPenInfo(value);
			}
		}

		internal GoPenInfo HighlightPenInfo
		{
			get
			{
				return myHighlightPenInfo;
			}
			set
			{
				GoPenInfo goPenInfo = myHighlightPenInfo;
				if (goPenInfo != value && (goPenInfo == null || !goPenInfo.Equals(value)))
				{
					InvalidateViews();
					myHighlightPenInfo = value;
					Changed(1236, 0, goPenInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (base.Parent != null)
					{
						base.Parent.InvalidatePaintBounds();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the highlight is shown when this stroke becomes selected.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// When this property is true and this stroke is selected, we set the
		/// <see cref="P:Northwoods.Go.GoStroke.Highlight" /> property to true.  Assuming there is a value for
		/// <see cref="P:Northwoods.Go.GoStroke.HighlightPen" />, the stroke will appear highlit.  When the stroke
		/// is no longer selected, the <see cref="P:Northwoods.Go.GoStroke.Highlight" /> property is set to false.
		/// Under these circumstances the change to the <see cref="P:Northwoods.Go.GoStroke.Highlight" /> property
		/// is not recorded by the undo manager (if any).
		/// This feature should only be used when there is only one view on the document, or
		/// if it is OK for all views on the document to display the highlight when the
		/// stroke is selected in just one of the views.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the highlight is shown when this stroke becomes selected.")]
		public virtual bool HighlightWhenSelected
		{
			get
			{
				return (base.InternalFlags & 0x800000) != 0;
			}
			set
			{
				bool flag = (base.InternalFlags & 0x800000) != 0;
				if (flag != value)
				{
					if (value)
					{
						base.InternalFlags |= 8388608;
					}
					else
					{
						base.InternalFlags &= -8388609;
					}
					Changed(1238, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The constructor produces a shape whose arrowheads are filled with black and
		/// that the user can resize in realtime.
		/// </summary>
		public GoStroke()
		{
			base.InternalFlags |= 512;
			Brush = GoShape.Brushes_Black;
		}

		/// <summary>
		/// Make sure the cloned stroke does not share any data references with the
		/// original stroke.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoStroke goStroke = (GoStroke)base.CopyObject(env);
			if (goStroke != null)
			{
				goStroke.myPoints = (PointF[])myPoints.Clone();
				if (myToArrowInfo != null)
				{
					goStroke.myToArrowInfo = (ArrowInfo)myToArrowInfo.Clone();
				}
				if (myFromArrowInfo != null)
				{
					goStroke.myFromArrowInfo = (ArrowInfo)myFromArrowInfo.Clone();
				}
			}
			return goStroke;
		}

		/// <summary>
		/// Add another point to the end of this stroke.
		/// </summary>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <returns>the zero-based index of the point that was added</returns>
		/// <seealso cref="M:Northwoods.Go.GoStroke.InsertPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoStroke.RemovePoint(System.Int32)" />
		public virtual int AddPoint(PointF p)
		{
			return InternalInsertPoint(myPointsCount, p);
		}

		/// <summary>
		/// This method is just a convenience overload of <see cref="M:Northwoods.Go.GoStroke.AddPoint(System.Drawing.PointF)" />.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>the zero-based index of the point that was added</returns>
		public int AddPoint(float x, float y)
		{
			return AddPoint(new PointF(x, y));
		}

		/// <summary>
		/// Add a point at a particular index, thereby increasing the number of points by one.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <remarks>
		/// This method throws an <c>ArgumentException</c> if the index <paramref name="i" />
		/// is less than zero.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoStroke.AddPoint(System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoStroke.RemovePoint(System.Int32)" />
		public virtual void InsertPoint(int i, PointF p)
		{
			InternalInsertPoint(i, p);
		}

		private int InternalInsertPoint(int i, PointF p)
		{
			if (i < 0)
			{
				throw new ArgumentException("GoStroke.InsertPoint given an invalid index, less than zero");
			}
			if (i > myPointsCount)
			{
				i = myPointsCount;
			}
			ResetPath();
			int num = myPoints.Length;
			checked
			{
				if (myPointsCount >= num)
				{
					PointF[] destinationArray = new PointF[Math.Max(num * 2, myPointsCount + 1)];
					Array.Copy(myPoints, 0, destinationArray, 0, num);
					myPoints = destinationArray;
				}
				if (myPointsCount > i)
				{
					Array.Copy(myPoints, i, myPoints, i + 1, myPointsCount - i);
				}
				myPointsCount++;
				myPoints[i] = p;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1201, i, null, GoObject.MakeRect(p), i, null, GoObject.MakeRect(p));
				return i;
			}
		}

		/// <summary>
		/// Remove the point at a particular index, thereby reducing the number of points by one.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <remarks>
		/// This method does nothing if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoStroke.AddPoint(System.Drawing.PointF)" />
		/// <seealso cref="M:Northwoods.Go.GoStroke.InsertPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.PointsCount" />
		public virtual void RemovePoint(int i)
		{
			InternalRemovePoint(i);
		}

		private void InternalRemovePoint(int i)
		{
			checked
			{
				if (i >= 0 && i < myPointsCount)
				{
					ResetPath();
					PointF p = myPoints[i];
					if (myPointsCount > i + 1)
					{
						Array.Copy(myPoints, i + 1, myPoints, i, myPointsCount - i - 1);
					}
					myPointsCount--;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1202, i, null, GoObject.MakeRect(p), i, null, GoObject.MakeRect(p));
				}
			}
		}

		/// <summary>
		/// Get the point at a particular index.
		/// </summary>
		/// <param name="i">the zero-based index</param>
		/// <returns>A <c>PointF</c> in document coordinates</returns>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoStroke.SetPoint(System.Int32,System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.PointsCount" />
		public virtual PointF GetPoint(int i)
		{
			if (i >= 0 && i < myPointsCount)
			{
				return myPoints[i];
			}
			throw new ArgumentException("GoStroke.GetPoint given an invalid index");
		}

		/// <summary>
		/// Replace the point at a particular index;
		/// </summary>
		/// <param name="i"></param>
		/// <param name="p">A <c>PointF</c> in document coordinates</param>
		/// <remarks>
		/// This method throws an exception if <paramref name="i" /> is out of bounds.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoStroke.GetPoint(System.Int32)" />
		/// <seealso cref="P:Northwoods.Go.GoStroke.PointsCount" />
		public virtual void SetPoint(int i, PointF p)
		{
			InternalSetPoint(i, p);
		}

		private void InternalSetPoint(int i, PointF p)
		{
			if (i >= 0 && i < myPointsCount)
			{
				PointF pointF = myPoints[i];
				if (pointF != p)
				{
					ResetPath();
					myPoints[i] = p;
					if (!base.Initializing)
					{
						base.InvalidBounds = true;
					}
					Changed(1203, i, null, GoObject.MakeRect(pointF), i, null, GoObject.MakeRect(p));
				}
				return;
			}
			throw new ArgumentException("GoStroke.SetPoint given an invalid index");
		}

		/// <summary>
		/// Remove all of the points for this stroke.
		/// </summary>
		/// <remarks>
		/// Afterwards, this stroke is not likely to participate usefully in many
		/// operations, such as painting, until more points are added by calling
		/// <see cref="M:Northwoods.Go.GoStroke.AddPoint(System.Drawing.PointF)" /> or <see cref="M:Northwoods.Go.GoStroke.SetPoints(System.Drawing.PointF[])" />.
		/// </remarks>
		public virtual void ClearPoints()
		{
			if (PointsCount > 0)
			{
				Changing(1204);
				ResetPath();
				myPointsCount = 0;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1204, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Replace all of the points for this stroke.
		/// </summary>
		/// <param name="points">A <c>PointF</c> array whose points are in document coordinates</param>
		/// <remarks>
		/// Afterwards, <see cref="P:Northwoods.Go.GoStroke.PointsCount" /> should equal the length of the
		/// <paramref name="points" /> array.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoStroke.CopyPointsArray" />
		public virtual void SetPoints(PointF[] points)
		{
			if (points != null)
			{
				Changing(1204);
				ResetPath();
				int num = points.Length;
				if (num > myPoints.Length)
				{
					myPoints = new PointF[num];
				}
				Array.Copy(points, 0, myPoints, 0, num);
				myPointsCount = num;
				if (!base.Initializing)
				{
					base.InvalidBounds = true;
				}
				Changed(1204, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Returns a copy of the array of points in this stroke.
		/// </summary>
		/// <value>
		/// A newly-allocated array of <c>PointF</c> values in document coordinates.
		/// </value>
		[Category("Appearance")]
		[Description("A copy of the array of points in this stroke.")]
		public virtual PointF[] CopyPointsArray()
		{
			PointF[] array = new PointF[myPointsCount];
			Array.Copy(myPoints, 0, array, 0, myPointsCount);
			return array;
		}

		/// <summary>
		/// When the resize handles are at each point of the stroke, the user's
		/// dragging of a resize handle should just change that point in the stroke.
		/// </summary>
		/// <param name="view">
		/// the <see cref="T:Northwoods.Go.GoView" /> whose <see cref="T:Northwoods.Go.GoToolResizing" /> is calling this method
		/// </param>
		/// <param name="origRect">
		/// the original Bounds of the object, but probably not useful for strokes
		/// </param>
		/// <param name="newPoint">
		/// the PointF, in document coordinates, to which the resize handle is being dragged
		/// </param>
		/// <param name="whichHandle">
		/// The <see cref="T:Northwoods.Go.IGoHandle" />.<see cref="P:Northwoods.Go.IGoHandle.HandleID" /> of the handle being dragged;
		/// for strokes this usually means that a value greater or equal to <see cref="F:Northwoods.Go.GoObject.LastHandle" />
		/// is actually the index of a point in the stroke, when one subtracts <see cref="F:Northwoods.Go.GoObject.LastHandle" />.
		/// </param>
		/// <param name="evttype">
		/// <list type="bullet">
		/// <item><term><c>GoInputState.Start</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.Start" /></description></item>
		/// <item><term><c>GoInputState.Continue</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseMove" /></description></item>
		/// <item><term><c>GoInputState.Finish</c></term><description>when called from <see cref="M:Northwoods.Go.GoToolResizing.DoMouseUp" /></description></item>
		/// <item><term><c>GoInputState.Cancel</c></term><description>when the <see cref="M:Northwoods.Go.GoToolResizing.DoCancelMouse" /></description></item>
		/// </list>
		/// </param>
		/// <param name="min">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MinimumSize" />,
		/// but probably not useful for strokes
		/// </param>
		/// <param name="max">
		/// the value of <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="P:Northwoods.Go.GoToolResizing.MaximumSize" />,
		/// but probably not useful for strokes
		/// </param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoObject.ResizesRealtime" /> is true, this method always calls
		/// <see cref="M:Northwoods.Go.GoStroke.SetPoint(System.Int32,System.Drawing.PointF)" />.
		/// Otherwise it only calls <see cref="M:Northwoods.Go.GoStroke.SetPoint(System.Int32,System.Drawing.PointF)" /> when the <paramref name="evttype" />
		/// is <see cref="F:Northwoods.Go.GoInputState.Finish" /> or <see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle, GoInputState evttype, SizeF min, SizeF max)
		{
			if (whichHandle >= 8192 && (ResizesRealtime || evttype == GoInputState.Finish || evttype == GoInputState.Cancel))
			{
				SetPoint(checked(whichHandle - 8192), newPoint);
			}
			else
			{
				base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
			}
		}

		/// <summary>
		/// Display the appropriate selected appearance, normally resize selection handles
		/// at each point of the stroke.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" /> is true, we just set <see cref="P:Northwoods.Go.GoStroke.Highlight" />
		/// to true.
		/// Otherwise, if this stroke is resizable and reshapable, we add resize selection
		/// handles at each stroke point, with handle IDs equal to <see cref="F:Northwoods.Go.GoObject.LastHandle" />
		/// plus the index of the point.
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			sel.RemoveHandles(this);
			if (HighlightWhenSelected)
			{
				bool skipsUndoManager = base.SkipsUndoManager;
				base.SkipsUndoManager = true;
				Highlight = true;
				base.SkipsUndoManager = skipsUndoManager;
				return;
			}
			GoView view = sel.View;
			bool flag = view?.CanResizeObjects() ?? true;
			bool flag2 = view?.CanReshapeObjects() ?? true;
			int lastPickIndex = LastPickIndex;
			if (CanResize() && flag)
			{
				if (CanReshape() && flag2)
				{
					checked
					{
						for (int i = FirstPickIndex; i <= lastPickIndex; i++)
						{
							PointF point = GetPoint(i);
							sel.CreateResizeHandle(this, selectedObj, point, 8192 + i, filled: true);
						}
					}
				}
				else
				{
					base.AddSelectionHandles(sel, selectedObj);
				}
			}
			else
			{
				for (int j = FirstPickIndex; j <= lastPickIndex; j = checked(j + 1))
				{
					PointF point2 = GetPoint(j);
					sel.CreateResizeHandle(this, selectedObj, point2, 0, filled: false);
				}
			}
		}

		/// <summary>
		/// Besides removing all selection handles, we also turn off any highlighting
		/// if <see cref="P:Northwoods.Go.GoStroke.HighlightWhenSelected" /> is true.
		/// </summary>
		/// <param name="sel"></param>
		public override void RemoveSelectionHandles(GoSelection sel)
		{
			if (HighlightWhenSelected)
			{
				bool skipsUndoManager = base.SkipsUndoManager;
				base.SkipsUndoManager = true;
				Highlight = false;
				base.SkipsUndoManager = skipsUndoManager;
			}
			base.RemoveSelectionHandles(sel);
		}

		/// <summary>
		/// Changing the bounds of a stroke may change all of the stroke's points.
		/// </summary>
		/// <param name="old">the earlier bounds, a <c>RectangleF</c> in document coordinates</param>
		/// <remarks>
		/// All of the points are modified to reflect the translation and
		/// scaling of the new bounding rectangle from the old one.
		/// </remarks>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			RectangleF bounds = Bounds;
			if (old.Width == bounds.Width && old.Height == bounds.Height)
			{
				float num = bounds.X - old.X;
				float num2 = bounds.Y - old.Y;
				if (num != 0f || num2 != 0f)
				{
					TranslatePoints(myPoints, num, num2);
					base.InvalidBounds = false;
				}
			}
			else
			{
				Changing(1204);
				RescalePoints(myPoints, old, bounds);
				base.InvalidBounds = false;
				Changed(1204, 0, null, old, 0, null, bounds);
			}
		}

		/// <summary>
		/// The bounding rectangle of a stroke is computed as the smallest
		/// rectangle that includes all of the stroke's points.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// If there are only zero or one points, the size will be zero.
		/// The computed bounds for Bezier strokes are not necessarily the closest fitting,
		/// and do not include any arrowheads.
		/// </remarks>
		protected override RectangleF ComputeBounds()
		{
			int pointsCount = PointsCount;
			if (pointsCount <= 0)
			{
				PointF position = base.Position;
				return new RectangleF(position.X, position.Y, 0f, 0f);
			}
			PointF point = GetPoint(0);
			float num = point.X;
			float num2 = point.Y;
			float num3 = point.X;
			float num4 = point.Y;
			checked
			{
				if (Style == GoStrokeStyle.Bezier && pointsCount >= 4)
				{
					for (int i = 3; i < pointsCount; i += 3)
					{
						PointF point2 = GetPoint(i - 3);
						PointF point3 = GetPoint(i - 2);
						if (i + 3 >= pointsCount)
						{
							i = pointsCount - 1;
						}
						PointF point4 = GetPoint(i - 1);
						PointF point5 = GetPoint(i);
						RectangleF rectangleF = BezierBounds(point2, point3, point4, point5, 0.1f);
						num = Math.Min(num, rectangleF.X);
						num2 = Math.Min(num2, rectangleF.Y);
						num3 = Math.Max(num3, rectangleF.X + rectangleF.Width);
						num4 = Math.Max(num4, rectangleF.Y + rectangleF.Height);
					}
				}
				else
				{
					for (int j = 1; j < pointsCount; j++)
					{
						point = GetPoint(j);
						num = Math.Min(num, point.X);
						num2 = Math.Min(num2, point.Y);
						num3 = Math.Max(num3, point.X);
						num4 = Math.Max(num4, point.Y);
					}
				}
				return new RectangleF(num, num2, num3 - num, num4 - num2);
			}
		}

		/// <summary>
		/// Consider Pen width, miter limit, drop shadow and highlight in computing the paint bounds.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
			GoPenInfo penInfo = base.PenInfo;
			if (penInfo != null)
			{
				float num = Math.Max(penInfo.Width, 1f) / 2f * penInfo.MiterLimit + 1f;
				GoPenInfo highlightPenInfo = HighlightPenInfo;
				if (highlightPenInfo != null)
				{
					float val = Math.Max(highlightPenInfo.Width, 1f) / 2f * highlightPenInfo.MiterLimit + 1f;
					num = Math.Max(num, val);
				}
				if (ToArrow)
				{
					num = Math.Max(num, Math.Abs(ToArrowLength));
					num = Math.Max(num, Math.Abs(ToArrowWidth));
				}
				if (FromArrow)
				{
					num = Math.Max(num, Math.Abs(FromArrowLength));
					num = Math.Max(num, Math.Abs(FromArrowWidth));
				}
				GoObject.InflateRect(ref rect, num, num);
				if (Shadowed)
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
			}
			return rect;
		}

		/// <summary>
		/// A point is in a stroke only if it is near one of its segments.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoStroke.GetSegmentNearPoint(System.Drawing.PointF)" /> to see if the
		/// point <paramref name="p" /> is near a segment.
		/// This is sensitive to the <see cref="P:Northwoods.Go.GoStroke.PickMargin" /> value.
		/// </remarks>
		public override bool ContainsPoint(PointF p)
		{
			return GetSegmentNearPoint(p) >= 0;
		}

		/// <summary>
		/// Return the index of the first point of a segment of this stroke
		/// that is close to a given point.
		/// </summary>
		/// <param name="pnt">A <c>PointF</c> in document coordinates</param>
		/// <returns>
		/// The zero-based index of the first point of a segment,
		/// or <c>-1</c> if no segment is near <paramref name="pnt" />.
		/// </returns>
		/// <remarks>
		/// This is sensitive to the <see cref="P:Northwoods.Go.GoStroke.PickMargin" /> value.
		/// For Bezier style strokes, this returns the index of the first of
		/// each set of points, e.g. 0, 3, 7, ....
		/// </remarks>
		public int GetSegmentNearPoint(PointF pnt)
		{
			RectangleF bounds = Bounds;
			float num = Math.Max(PenWidth, 0.1f);
			float num2 = Math.Max(PickMargin, 0f);
			num += num2;
			if (pnt.X < bounds.X - num || pnt.X > bounds.X + bounds.Width + num || pnt.Y < bounds.Y - num || pnt.Y > bounds.Y + bounds.Height + num)
			{
				return -1;
			}
			int pointsCount = PointsCount;
			if (pointsCount <= 1)
			{
				return -1;
			}
			num -= num2 / 2f;
			checked
			{
				if (Style == GoStrokeStyle.Bezier && pointsCount >= 4)
				{
					num *= Math.Max(1f, Math.Max(bounds.Width, bounds.Height) / 1000f);
					for (int i = 3; i < pointsCount; i += 3)
					{
						int result = i - 3;
						PointF point = GetPoint(i - 3);
						PointF point2 = GetPoint(i - 2);
						if (i + 3 >= pointsCount)
						{
							i = pointsCount - 1;
						}
						PointF point3 = GetPoint(i - 1);
						PointF point4 = GetPoint(i);
						if (BezierContainsPoint(point, point2, point3, point4, num, pnt))
						{
							return result;
						}
					}
				}
				else
				{
					for (int j = 0; j < pointsCount - 1; j++)
					{
						PointF point5 = GetPoint(j);
						PointF point6 = GetPoint(j + 1);
						if (LineContainsPoint(point5, point6, num, pnt))
						{
							return j;
						}
					}
				}
				return -1;
			}
		}

		internal static bool LineContainsPoint(PointF a, PointF b, float fuzz, PointF p)
		{
			float x;
			float x2;
			if (a.X < b.X)
			{
				x = a.X;
				x2 = b.X;
			}
			else
			{
				x = b.X;
				x2 = a.X;
			}
			float y;
			float y2;
			if (a.Y < b.Y)
			{
				y = a.Y;
				y2 = b.Y;
			}
			else
			{
				y = b.Y;
				y2 = a.Y;
			}
			if (a.X == b.X)
			{
				if (y <= p.Y && p.Y <= y2 && a.X - fuzz <= p.X)
				{
					return p.X <= a.X + fuzz;
				}
				return false;
			}
			if (a.Y == b.Y)
			{
				if (x <= p.X && p.X <= x2 && a.Y - fuzz <= p.Y)
				{
					return p.Y <= a.Y + fuzz;
				}
				return false;
			}
			float num = x2 + fuzz;
			float num2 = x - fuzz;
			if (num2 <= p.X && p.X <= num)
			{
				float num3 = y2 + fuzz;
				float num4 = y - fuzz;
				if (num4 <= p.Y && p.Y <= num3)
				{
					if (num - num2 > num3 - num4)
					{
						if (!(a.X - b.X > fuzz) && !(b.X - a.X > fuzz))
						{
							return true;
						}
						float num5 = (b.Y - a.Y) / (b.X - a.X) * (p.X - a.X) + a.Y;
						if (num5 - fuzz <= p.Y && p.Y <= num5 + fuzz)
						{
							return true;
						}
					}
					else
					{
						if (!(a.Y - b.Y > fuzz) && !(b.Y - a.Y > fuzz))
						{
							return true;
						}
						float num6 = (b.X - a.X) / (b.Y - a.Y) * (p.Y - a.Y) + a.X;
						if (num6 - fuzz <= p.X && p.X <= num6 + fuzz)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static bool BezierContainsPoint(PointF s, PointF c1, PointF c2, PointF e, float epsilon, PointF p)
		{
			if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2))
			{
				PointF c3 = new PointF((s.X + c1.X) / 2f, (s.Y + c1.Y) / 2f);
				PointF pointF = new PointF((c1.X + c2.X) / 2f, (c1.Y + c2.Y) / 2f);
				PointF c4 = new PointF((c2.X + e.X) / 2f, (c2.Y + e.Y) / 2f);
				PointF c5 = new PointF((c3.X + pointF.X) / 2f, (c3.Y + pointF.Y) / 2f);
				PointF c6 = new PointF((pointF.X + c4.X) / 2f, (pointF.Y + c4.Y) / 2f);
				PointF pointF2 = new PointF((c5.X + c6.X) / 2f, (c5.Y + c6.Y) / 2f);
				if (!BezierContainsPoint(s, c3, c5, pointF2, epsilon, p))
				{
					return BezierContainsPoint(pointF2, c6, c4, e, epsilon, p);
				}
				return true;
			}
			return LineContainsPoint(s, e, epsilon, p);
		}

		internal static void BezierMidPoint(PointF b0, PointF b1, PointF b2, PointF b3, out PointF v, out PointF w)
		{
			PointF pointF = new PointF((b0.X + b1.X) / 2f, (b0.Y + b1.Y) / 2f);
			PointF pointF2 = new PointF((b1.X + b2.X) / 2f, (b1.Y + b2.Y) / 2f);
			PointF pointF3 = new PointF((b2.X + b3.X) / 2f, (b2.Y + b3.Y) / 2f);
			v = new PointF((pointF.X + pointF2.X) / 2f, (pointF.Y + pointF2.Y) / 2f);
			w = new PointF((pointF2.X + pointF3.X) / 2f, (pointF2.Y + pointF3.Y) / 2f);
		}

		internal static RectangleF LineBounds(PointF a, PointF b)
		{
			float num = Math.Min(a.X, b.X);
			float num2 = Math.Min(a.Y, b.Y);
			float num3 = Math.Max(a.X, b.X);
			float num4 = Math.Max(a.Y, b.Y);
			return new RectangleF(num, num2, num3 - num, num4 - num2);
		}

		internal static RectangleF BezierBounds(PointF s, PointF c1, PointF c2, PointF e, float epsilon)
		{
			if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2))
			{
				PointF c3 = new PointF((s.X + c1.X) / 2f, (s.Y + c1.Y) / 2f);
				PointF pointF = new PointF((c1.X + c2.X) / 2f, (c1.Y + c2.Y) / 2f);
				PointF c4 = new PointF((c2.X + e.X) / 2f, (c2.Y + e.Y) / 2f);
				PointF c5 = new PointF((c3.X + pointF.X) / 2f, (c3.Y + pointF.Y) / 2f);
				PointF c6 = new PointF((pointF.X + c4.X) / 2f, (pointF.Y + c4.Y) / 2f);
				PointF pointF2 = new PointF((c5.X + c6.X) / 2f, (c5.Y + c6.Y) / 2f);
				return GoObject.UnionRect(BezierBounds(s, c3, c5, pointF2, epsilon), BezierBounds(pointF2, c6, c4, e, epsilon));
			}
			return LineBounds(s, e);
		}

		internal static bool BezierNearestIntersectionOnLine(PointF s, PointF c1, PointF c2, PointF e, PointF p1, PointF p2, float epsilon, out PointF result)
		{
			float num = 1E+21f;
			PointF pointF = default(PointF);
			PointF result2;
			if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2))
			{
				PointF c3 = new PointF((s.X + c1.X) / 2f, (s.Y + c1.Y) / 2f);
				PointF pointF2 = new PointF((c1.X + c2.X) / 2f, (c1.Y + c2.Y) / 2f);
				PointF c4 = new PointF((c2.X + e.X) / 2f, (c2.Y + e.Y) / 2f);
				PointF c5 = new PointF((c3.X + pointF2.X) / 2f, (c3.Y + pointF2.Y) / 2f);
				PointF c6 = new PointF((pointF2.X + c4.X) / 2f, (pointF2.Y + c4.Y) / 2f);
				PointF pointF3 = new PointF((c5.X + c6.X) / 2f, (c5.Y + c6.Y) / 2f);
				if (BezierNearestIntersectionOnLine(s, c3, c5, pointF3, p1, p2, epsilon, out result2))
				{
					float num2 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
					if (num2 < num)
					{
						num = num2;
						pointF = result2;
					}
				}
				if (BezierNearestIntersectionOnLine(pointF3, c6, c4, e, p1, p2, epsilon, out result2))
				{
					float num3 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
					if (num3 < num)
					{
						num = num3;
						pointF = result2;
					}
				}
			}
			else if (NearestIntersectionOnLine(s, e, p1, p2, out result2))
			{
				float num4 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
				if (num4 < num)
				{
					num = num4;
					pointF = result2;
				}
			}
			result = pointF;
			return num < 1E+21f;
		}

		/// <summary>
		/// The closest intersection point of a stroke with a line is the
		/// closest such point for each of its segments.
		/// </summary>
		/// <param name="p1">
		/// the point we are looking to be closest to, on the line formed with <paramref name="p2" />
		/// </param>
		/// <param name="p2">
		/// forms a line with <paramref name="p1" />
		/// </param>
		/// <param name="result">
		/// the point of this object that is closest to <paramref name="p1" /> and that is on
		/// the infinite line from <paramref name="p1" /> to <paramref name="p2" />
		/// </param>
		/// <returns>
		/// true if the infinite line does intersect with this object; false otherwise
		/// </returns>
		/// <remarks>
		/// This currently does not take into account any Pen width.
		/// </remarks>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			int pointsCount = PointsCount;
			float num = 1E+21f;
			PointF pointF = default(PointF);
			checked
			{
				PointF result2;
				if (Style == GoStrokeStyle.Bezier && pointsCount >= 4)
				{
					float epsilon = PenWidth / 2f;
					for (int i = 3; i < pointsCount; i += 3)
					{
						PointF point = GetPoint(i - 3);
						PointF point2 = GetPoint(i - 2);
						if (i + 3 >= pointsCount)
						{
							i = pointsCount - 1;
						}
						PointF point3 = GetPoint(i - 1);
						PointF point4 = GetPoint(i);
						if (BezierNearestIntersectionOnLine(point, point2, point3, point4, p1, p2, epsilon, out result2))
						{
							float num2 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num2 < num)
							{
								num = num2;
								pointF = result2;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < pointsCount - 1; j++)
					{
						PointF point5 = GetPoint(j);
						PointF point6 = GetPoint(j + 1);
						if (NearestIntersectionOnLine(point5, point6, p1, p2, out result2))
						{
							float num3 = (result2.X - p1.X) * (result2.X - p1.X) + (result2.Y - p1.Y) * (result2.Y - p1.Y);
							if (num3 < num)
							{
								num = num3;
								pointF = result2;
							}
						}
					}
				}
				result = pointF;
				return num < 1E+21f;
			}
		}

		/// <summary>
		/// Return a point on a straight line segment that is closest to a given point.
		/// </summary>
		/// <param name="a">One end of the line.</param>
		/// <param name="b">The other end of the line.</param>
		/// <param name="p">The point to be closest to.</param>
		/// <param name="result">
		/// A <c>PointF</c> that is on the finite length straight line segment from
		/// <paramref name="a" /> to <paramref name="b" />
		/// </param>
		/// <returns>
		/// true if the point <paramref name="p" /> is on a perpendicular line to the line segment;
		/// false if the point <paramref name="p" /> is beyond either end of the line segment.
		/// When this returns false, the <paramref name="result" /> will be either
		/// <paramref name="a" /> or <paramref name="b" />.
		/// </returns>
		public static bool NearestPointOnLine(PointF a, PointF b, PointF p, out PointF result)
		{
			float x = a.X;
			float y = a.Y;
			float x2 = b.X;
			float y2 = b.Y;
			float x3 = p.X;
			float y3 = p.Y;
			if (x == x2)
			{
				float num;
				float num2;
				if (y < y2)
				{
					num = y;
					num2 = y2;
				}
				else
				{
					num = y2;
					num2 = y;
				}
				float num3 = y3;
				if (num3 < num)
				{
					result = new PointF(x, num);
					return false;
				}
				if (num3 > num2)
				{
					result = new PointF(x, num2);
					return false;
				}
				result = new PointF(x, num3);
				return true;
			}
			if (y == y2)
			{
				float num4;
				float num5;
				if (x < x2)
				{
					num4 = x;
					num5 = x2;
				}
				else
				{
					num4 = x2;
					num5 = x;
				}
				float num6 = x3;
				if (num6 < num4)
				{
					result = new PointF(num4, y);
					return false;
				}
				if (num6 > num5)
				{
					result = new PointF(num5, y);
					return false;
				}
				result = new PointF(num6, y);
				return true;
			}
			float num7 = (x2 - x) * (x2 - x) + (y2 - y) * (y2 - y);
			float num8 = ((x - x3) * (x - x2) + (y - y3) * (y - y2)) / num7;
			if (num8 < 0f)
			{
				result = a;
				return false;
			}
			if (num8 > 1f)
			{
				result = b;
				return false;
			}
			float x4 = x + num8 * (x2 - x);
			float y4 = y + num8 * (y2 - y);
			result = new PointF(x4, y4);
			return true;
		}

		/// <summary>
		/// Find the intersection point of the finite line segment A-B and the infinite line P-Q
		/// that is closest to point P.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="p"></param>
		/// <param name="q"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool NearestIntersectionOnLine(PointF a, PointF b, PointF p, PointF q, out PointF result)
		{
			float x = a.X;
			float y = a.Y;
			float x2 = b.X;
			float y2 = b.Y;
			float x3 = p.X;
			float y3 = p.Y;
			float x4 = q.X;
			float y4 = q.Y;
			if (x3 == x4)
			{
				if (x == x2)
				{
					NearestPointOnLine(a, b, p, out result);
					return false;
				}
				float y5 = (y2 - y) / (x2 - x) * (x3 - x) + y;
				return NearestPointOnLine(a, b, new PointF(x3, y5), out result);
			}
			float num = (y4 - y3) / (x4 - x3);
			if (x == x2)
			{
				float num2 = num * (x - x3) + y3;
				if (num2 < Math.Min(y, y2))
				{
					result = new PointF(x, Math.Min(y, y2));
					return false;
				}
				if (num2 > Math.Max(y, y2))
				{
					result = new PointF(x, Math.Max(y, y2));
					return false;
				}
				result = new PointF(x, num2);
				return true;
			}
			float num3 = (y2 - y) / (x2 - x);
			if (num == num3)
			{
				NearestPointOnLine(a, b, p, out result);
				return false;
			}
			float num4 = (num3 * x - num * x3 + y3 - y) / (num3 - num);
			if (num3 == 0f)
			{
				if (num4 < Math.Min(x, x2))
				{
					result = new PointF(Math.Min(x, x2), y);
					return false;
				}
				if (num4 > Math.Max(x, x2))
				{
					result = new PointF(Math.Max(x, x2), y);
					return false;
				}
				result = new PointF(num4, y);
				return true;
			}
			float y6 = num3 * (num4 - x) + y;
			return NearestPointOnLine(a, b, new PointF(num4, y6), out result);
		}

		/// <summary>
		/// Return the angle of the line going from the origin to a point.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>
		/// an angle in degrees, with <c>0</c> along the positive X axis, and
		/// with <c>90.0</c> along the positive Y axis.
		/// </returns>
		public static float GetAngle(float x, float y)
		{
			float num;
			if (x == 0f)
			{
				num = ((!(y > 0f)) ? 270f : 90f);
			}
			else if (y == 0f)
			{
				num = ((!(x > 0f)) ? 180f : 0f);
			}
			else
			{
				num = (float)(Math.Atan(Math.Abs(y / x)) * 180.0 / Math.PI);
				if (x < 0f)
				{
					num = ((!(y < 0f)) ? (180f - num) : (num + 180f));
				}
				else if (y < 0f)
				{
					num = 360f - num;
				}
			}
			return num;
		}

		internal static bool IntersectingLines(PointF a1, PointF a2, PointF b1, PointF b2)
		{
			checked
			{
				if (ComparePointWithLine(a1, a2, b1) * ComparePointWithLine(a1, a2, b2) <= 0)
				{
					return ComparePointWithLine(b1, b2, a1) * ComparePointWithLine(b1, b2, a2) <= 0;
				}
				return false;
			}
		}

		internal static int ComparePointWithLine(PointF a1, PointF a2, PointF p)
		{
			float num = a2.X - a1.X;
			float num2 = a2.Y - a1.Y;
			float num3 = p.X - a1.X;
			float num4 = p.Y - a1.Y;
			float num5 = num3 * num2 - num4 * num;
			if (num5 == 0f)
			{
				num5 = num3 * num + num4 * num2;
				if (num5 > 0f)
				{
					num3 -= num;
					num4 -= num2;
					num5 = num3 * num + num4 * num2;
					if (num5 < 0f)
					{
						num5 = 0f;
					}
				}
			}
			if (!(num5 < 0f))
			{
				if (!(num5 > 0f))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}

		internal bool IntersectsStroke(GoObject stroke)
		{
			return StrokesIntersect(this, stroke, GoObject.NullRect);
		}

		internal bool IntersectsRectangle(RectangleF rect)
		{
			return StrokesIntersect(this, null, rect);
		}

		internal static bool StrokesIntersect(GoObject a, GoObject b, RectangleF br)
		{
			RectangleF bounds = a.Bounds;
			if (b != null)
			{
				br = b.Bounds;
			}
			if (!GoObject.IntersectsRect(bounds, br))
			{
				return false;
			}
			GoStroke goStroke = a as GoStroke;
			if (goStroke == null && a is GoLabeledLink)
			{
				goStroke = ((GoLabeledLink)a).RealLink;
			}
			GoStroke goStroke2 = b as GoStroke;
			if (goStroke2 == null && b is GoLabeledLink)
			{
				goStroke2 = ((GoLabeledLink)b).RealLink;
			}
			checked
			{
				if (goStroke != null)
				{
					float w = Math.Max(goStroke.PenWidth, goStroke.HighlightPenWidth);
					for (int i = 0; i < goStroke.PointsCount - 1; i++)
					{
						PointF point = goStroke.GetPoint(i);
						PointF point2 = goStroke.GetPoint(i + 1);
						if (goStroke2 != null)
						{
							float w2 = Math.Max(goStroke2.PenWidth, goStroke2.HighlightPenWidth);
							for (int j = 0; j < goStroke2.PointsCount - 1; j++)
							{
								PointF point3 = goStroke2.GetPoint(j);
								PointF point4 = goStroke2.GetPoint(j + 1);
								if (GoObject.IntersectsRect(RectFromLine(point, point2, w), RectFromLine(point3, point4, w2)))
								{
									return true;
								}
							}
						}
						else if (GoObject.IntersectsRect(br, RectFromLine(point, point2, w)))
						{
							return true;
						}
					}
					return false;
				}
				if (goStroke2 != null)
				{
					float w3 = Math.Max(goStroke2.PenWidth, goStroke2.HighlightPenWidth);
					for (int k = 0; k < goStroke2.PointsCount - 1; k++)
					{
						PointF point5 = goStroke2.GetPoint(k);
						PointF point6 = goStroke2.GetPoint(k + 1);
						if (GoObject.IntersectsRect(bounds, RectFromLine(point5, point6, w3)))
						{
							return true;
						}
					}
					return false;
				}
				return true;
			}
		}

		internal static RectangleF RectFromLine(PointF a, PointF b, float w)
		{
			if (a.X == b.X)
			{
				float y = a.Y;
				float y2 = b.Y;
				if (y > y2)
				{
					y = b.Y;
					y2 = a.Y;
				}
				return new RectangleF(a.X - w / 2f, y, w, y2 - y);
			}
			if (a.Y == b.Y)
			{
				float x = a.X;
				float x2 = b.X;
				if (x > x2)
				{
					x = b.X;
					x2 = a.X;
				}
				return new RectangleF(x, a.Y - w / 2f, x2 - x, w);
			}
			float y3 = a.Y;
			float y4 = b.Y;
			if (y3 > y4)
			{
				y3 = b.Y;
				y4 = a.Y;
			}
			float x3 = a.X;
			float x4 = b.X;
			if (x3 > x4)
			{
				x3 = b.X;
				x4 = a.X;
			}
			return new RectangleF(x3, y3, x4 - x3, y4 - y3);
		}

		internal static void TranslatePoints(PointF[] v, float dx, float dy)
		{
			for (int i = 0; i < v.Length; i = checked(i + 1))
			{
				PointF pointF = v[i];
				pointF.X += dx;
				pointF.Y += dy;
				v[i] = pointF;
			}
		}

		internal static void RescalePoints(PointF[] v, RectangleF oldr, RectangleF newr)
		{
			float num = 1f;
			if (oldr.Width != 0f)
			{
				num = newr.Width / oldr.Width;
			}
			float num2 = 1f;
			if (oldr.Height != 0f)
			{
				num2 = newr.Height / oldr.Height;
			}
			for (int i = 0; i < v.Length; i = checked(i + 1))
			{
				PointF pointF = v[i];
				float x = newr.X + (pointF.X - oldr.X) * num;
				float y = newr.Y + (pointF.Y - oldr.Y) * num2;
				v[i] = new PointF(x, y);
			}
		}

		/// <summary>
		/// Gets the number of points to be used in the polygon representing an arrowhead.
		/// </summary>
		/// <param name="atEnd">true for the "To" end, false for the "From" end</param>
		/// <returns>
		/// By default this returns 4.  The value must be at least 3.
		/// </returns>
		public virtual int GetArrowheadPointsCount(bool atEnd)
		{
			return 4;
		}

		/// <summary>
		/// Modify an array of points to hold the four points of a polygon outlining an arrowhead
		/// </summary>
		/// <param name="anchor">The arrowhead anchor position, giving the spot the arrow is coming from</param>
		/// <param name="endPoint">The arrowhead end point</param>
		/// <param name="atEnd">true if for the "To" end, false if for the "From" end</param>
		/// <param name="poly">An array of <c>PointF</c> of at least length four to hold the results</param>
		/// <remarks>
		/// <para>
		/// By default the four points are as follows:
		/// <list type="bullet">
		/// <item> [0] the inner point on the shaft, arrow shaft length from the end point </item>
		/// <item> [1] and [3] the barbs of the arrowhead, arrow width apart </item>
		/// <item> [2] the tip of the arrowhead, at the end point </item>
		/// </list>
		/// If necessary, the length of the arrowhead is actually scaled down to
		/// fit in the distance between the <paramref name="anchor" /> and the
		/// <paramref name="endPoint" />.
		/// </para>
		/// <para>
		/// This method is normally called by <see cref="M:Northwoods.Go.GoStroke.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> to produce a point array to
		/// be passed to <see cref="M:Northwoods.Go.GoStroke.DrawArrowhead(System.Drawing.Graphics,Northwoods.Go.GoView,System.Drawing.Pen,System.Drawing.Brush,System.Boolean,System.Single,System.Single,System.Drawing.PointF[])" /> in the following fashion:
		/// <pre><code>
		///   if (this.ToArrow &amp;&amp; this.PointsCount &gt;= 2) {
		///     PointF[] toPoly = new PointF[GetArrowheadPointsCount(true)];
		///     PointF anchor = this.ToArrowAnchorPoint;
		///     PointF end = this.ToArrowEndPoint;
		///     CalculateArrowhead(anchor, end, true, toPoly);
		///     ... now use the toPoly array ...
		///   }
		/// </code></pre>
		/// </para>
		/// </remarks>
		public virtual void CalculateArrowhead(PointF anchor, PointF endPoint, bool atEnd, PointF[] poly)
		{
			float x = endPoint.X;
			float y = endPoint.Y;
			float num = x - anchor.X;
			float num2 = y - anchor.Y;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			int num4 = poly.Length;
			for (int i = 0; i < num4; i = checked(i + 1))
			{
				poly[i].X = x;
				poly[i].Y = y;
			}
			if (IsApprox(num3, 0f) || num4 < 3)
			{
				return;
			}
			float num5 = num / num3;
			float num6 = num2 / num3;
			float num7;
			float num8;
			GoStrokeArrowheadStyle goStrokeArrowheadStyle;
			float num9;
			if (atEnd)
			{
				num7 = ToArrowLength;
				num8 = ToArrowShaftLength;
				num9 = ToArrowWidth;
				goStrokeArrowheadStyle = ToArrowStyle;
			}
			else
			{
				num7 = FromArrowLength;
				num8 = FromArrowShaftLength;
				num9 = FromArrowWidth;
				goStrokeArrowheadStyle = FromArrowStyle;
			}
			num9 /= 2f;
			float num10 = Math.Max(num7, num8);
			if (num10 > 0f && num3 < num10 && Style != GoStrokeStyle.Bezier)
			{
				float num11 = num3 / num10;
				num7 *= num11;
				num8 *= num11;
				num9 *= num11;
			}
			if (num4 >= 6)
			{
				float num12 = 0f - num8;
				float num13 = 0f;
				float num14 = 0f - num7;
				float num15 = 0f - num9;
				float num16 = 0f - num7 + num8;
				float num17 = 0f - num9;
				float num18 = 0f - num7 + num8;
				float num19 = num9;
				float num20 = 0f - num7;
				float num21 = num9;
				poly[0].X = x + (num5 * num12 - num6 * num13);
				poly[0].Y = y + (num6 * num12 + num5 * num13);
				poly[1].X = x + (num5 * num14 - num6 * num15);
				poly[1].Y = y + (num6 * num14 + num5 * num15);
				poly[2].X = x + (num5 * num16 - num6 * num17);
				poly[2].Y = y + (num6 * num16 + num5 * num17);
				poly[3].X = x;
				poly[3].Y = y;
				poly[4].X = x + (num5 * num18 - num6 * num19);
				poly[4].Y = y + (num6 * num18 + num5 * num19);
				poly[5].X = x + (num5 * num20 - num6 * num21);
				poly[5].Y = y + (num6 * num20 + num5 * num21);
				return;
			}
			switch (num4)
			{
			case 5:
			{
				float num41 = 0f - num8;
				float num42 = 0f;
				float num43 = 0f - num7;
				float num44 = 0f - num9;
				float num45 = 0f - num7 + num8;
				float num46 = 0f - num9;
				poly[0].X = x + (num5 * num41 - num6 * num42);
				poly[0].Y = y + (num6 * num41 + num5 * num42);
				poly[1].X = x + (num5 * num43 - num6 * num44);
				poly[1].Y = y + (num6 * num43 + num5 * num44);
				poly[2].X = x + (num5 * num45 - num6 * num46);
				poly[2].Y = y + (num6 * num45 + num5 * num46);
				poly[3].X = x;
				poly[3].Y = y;
				poly[4].X = x;
				poly[4].Y = y;
				break;
			}
			case 4:
				if (goStrokeArrowheadStyle != GoStrokeArrowheadStyle.X)
				{
					float num26 = 0f - num8;
					float num27 = 0f;
					float num28 = 0f - num7;
					float num29 = 0f - num9;
					float num30 = 0f - num7;
					float num31 = num9;
					if (atEnd)
					{
						switch (ToArrowStyle)
						{
						case GoStrokeArrowheadStyle.Slash:
							num28 = (0f - num7) / 2f;
							num30 = (0f - num7) * 3f / 2f;
							break;
						case GoStrokeArrowheadStyle.BackSlash:
							num28 = (0f - num7) * 3f / 2f;
							num30 = (0f - num7) / 2f;
							break;
						}
					}
					else
					{
						switch (FromArrowStyle)
						{
						case GoStrokeArrowheadStyle.Slash:
							num28 = (0f - num7) / 2f;
							num30 = (0f - num7) * 3f / 2f;
							break;
						case GoStrokeArrowheadStyle.BackSlash:
							num28 = (0f - num7) * 3f / 2f;
							num30 = (0f - num7) / 2f;
							break;
						}
					}
					poly[0].X = x + (num5 * num26 - num6 * num27);
					poly[0].Y = y + (num6 * num26 + num5 * num27);
					poly[1].X = x + (num5 * num28 - num6 * num29);
					poly[1].Y = y + (num6 * num28 + num5 * num29);
					poly[2].X = x;
					poly[2].Y = y;
					poly[3].X = x + (num5 * num30 - num6 * num31);
					poly[3].Y = y + (num6 * num30 + num5 * num31);
				}
				else
				{
					float num32 = num7 / 2f;
					float num33 = 0f - num8 - num32;
					float num34 = 0f - num9;
					float num35 = 0f - num8 + num32;
					float num36 = 0f - num9;
					float num37 = 0f - num8 - num32;
					float num38 = num9;
					float num39 = 0f - num8 + num32;
					float num40 = num9;
					poly[0].X = x + (num5 * num33 - num6 * num34);
					poly[0].Y = y + (num6 * num33 + num5 * num34);
					poly[1].X = x + (num5 * num35 - num6 * num36);
					poly[1].Y = y + (num6 * num35 + num5 * num36);
					poly[2].X = x + (num5 * num37 - num6 * num38);
					poly[2].Y = y + (num6 * num37 + num5 * num38);
					poly[3].X = x + (num5 * num39 - num6 * num40);
					poly[3].Y = y + (num6 * num39 + num5 * num40);
				}
				break;
			case 3:
			{
				float num22 = 0f - num8;
				float num23 = 0f;
				float num24 = 0f - num7;
				float num25 = 0f - num9;
				if (atEnd)
				{
					switch (ToArrowStyle)
					{
					case GoStrokeArrowheadStyle.Slash:
						num24 = (0f - num7) / 2f;
						break;
					case GoStrokeArrowheadStyle.BackSlash:
						num24 = (0f - num7) * 3f / 2f;
						break;
					}
				}
				else
				{
					switch (FromArrowStyle)
					{
					case GoStrokeArrowheadStyle.Slash:
						num24 = (0f - num7) / 2f;
						break;
					case GoStrokeArrowheadStyle.BackSlash:
						num24 = (0f - num7) * 3f / 2f;
						break;
					}
				}
				poly[0].X = x + (num5 * num22 - num6 * num23);
				poly[0].Y = y + (num6 * num22 + num5 * num23);
				poly[1].X = x + (num5 * num24 - num6 * num25);
				poly[1].Y = y + (num6 * num24 + num5 * num25);
				poly[2].X = x;
				poly[2].Y = y;
				break;
			}
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoStroke.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> to actually draw an arrowhead.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="atEnd">true if for the "To" end, false if for the "From" end</param>
		/// <param name="offsetw">the offset is normally used for drawing a shadow</param>
		/// <param name="offseth">the offset is normally used for drawing a shadow</param>
		/// <param name="poly">The result of a call to <see cref="M:Northwoods.Go.GoStroke.CalculateArrowhead(System.Drawing.PointF,System.Drawing.PointF,System.Boolean,System.Drawing.PointF[])" /></param>
		/// <remarks>
		/// By default this will be called with a thin, solid pen derived from
		/// <see cref="P:Northwoods.Go.GoShape.Pen" /> and this stroke's <see cref="P:Northwoods.Go.GoShape.Brush" />.
		/// When the arrowhead style is <see cref="F:Northwoods.Go.GoStrokeArrowheadStyle.Circle" />, the
		/// polygon's [0] and [2] points are used to position the circle.
		/// When the arrowhead style is one of the "crossed" styles, the
		/// polygon's [1] and [3] points are used as the points for the crossing line.
		/// </remarks>
		protected virtual void DrawArrowhead(Graphics g, GoView view, Pen pen, Brush brush, bool atEnd, float offsetw, float offseth, PointF[] poly)
		{
			Brush brush2 = null;
			if (poly[0] != poly[2] && (atEnd ? ToArrowFilled : FromArrowFilled))
			{
				brush2 = brush;
			}
			checked
			{
				switch (atEnd ? ToArrowStyle : FromArrowStyle)
				{
				case GoStrokeArrowheadStyle.Polygon:
					if (offsetw != 0f || offseth != 0f)
					{
						int num4 = poly.Length;
						for (int i = 0; i < num4; i++)
						{
							poly[i].X += offsetw;
							poly[i].Y += offseth;
						}
						GoShape.DrawPolygon(g, view, pen, brush2, poly);
						for (int j = 0; j < num4; j++)
						{
							poly[j].X -= offsetw;
							poly[j].Y -= offseth;
						}
					}
					else
					{
						GoShape.DrawPolygon(g, view, pen, brush2, poly);
					}
					break;
				case GoStrokeArrowheadStyle.Circle:
				{
					float x5 = poly[0].X;
					float y5 = poly[0].Y;
					float x6 = poly[2].X;
					float y6 = poly[2].Y;
					float num = (x5 + x6) / 2f + offsetw;
					float num2 = (y5 + y6) / 2f + offseth;
					float num3 = (float)Math.Sqrt((x5 - x6) * (x5 - x6) + (y5 - y6) * (y5 - y6));
					GoShape.DrawEllipse(g, view, pen, brush2, num - num3 / 2f, num2 - num3 / 2f, num3, num3);
					break;
				}
				case GoStrokeArrowheadStyle.Cross:
				case GoStrokeArrowheadStyle.Slash:
				case GoStrokeArrowheadStyle.BackSlash:
				{
					float x3 = poly[1].X + offsetw;
					float y3 = poly[1].Y + offseth;
					float x4 = poly[3].X + offsetw;
					float y4 = poly[3].Y + offseth;
					GoShape.DrawLine(g, view, pen, x3, y3, x4, y4);
					break;
				}
				case GoStrokeArrowheadStyle.X:
				{
					float x = poly[0].X + offsetw;
					float y = poly[0].Y + offseth;
					float x2 = poly[3].X + offsetw;
					float y2 = poly[3].Y + offseth;
					GoShape.DrawLine(g, view, pen, x, y, x2, y2);
					x = poly[1].X + offsetw;
					y = poly[1].Y + offseth;
					x2 = poly[2].X + offsetw;
					y2 = poly[2].Y + offseth;
					GoShape.DrawLine(g, view, pen, x, y, x2, y2);
					break;
				}
				}
			}
		}

		/// <summary>
		/// A stroke is drawn as a sequence of straight line segments or
		/// Bezier curves, with optional arrows at the ends.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// The kind of stroke is specified by <see cref="P:Northwoods.Go.GoStroke.Style" />, and the
		/// parameters of the stroke's segments are specified by the stroke's
		/// points.
		/// Each segment uses the points it needs until there are not enough
		/// more for an additional segment.
		/// If <see cref="P:Northwoods.Go.GoObject.Shadowed" /> is true, the stroke is drawn
		/// with the <see cref="M:Northwoods.Go.GoObject.GetShadowPen(Northwoods.Go.GoView,System.Single)" /> at a offset given by
		/// <see cref="M:Northwoods.Go.GoObject.GetShadowOffset(Northwoods.Go.GoView)" />.
		/// If <see cref="P:Northwoods.Go.GoStroke.HighlightPen" /> is non-null, that pen is used to
		/// drawn the stroke first.
		/// Finally <see cref="P:Northwoods.Go.GoShape.Pen" /> is used to draw the stroke.
		/// If <see cref="P:Northwoods.Go.GoStroke.FromArrow" /> or <see cref="P:Northwoods.Go.GoStroke.ToArrow" /> are true,
		/// the corresponding arrowheads are drawn according to the values
		/// of the ArrowAnchorPoint, ArrowLength, ArrowShaftLength,
		/// ArrowWidth, and ArrowFilled.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			Pen pen = Pen;
			if (pen == null)
			{
				return;
			}
			Pen pen2 = pen;
			Brush brush = Brush;
			int pointsCount = PointsCount;
			PointF[] array = null;
			PointF[] array2 = null;
			if (FromArrow && pointsCount >= 2)
			{
				if (myFromArrowInfo == null)
				{
					myFromArrowInfo = new ArrowInfo();
				}
				array = myFromArrowInfo.GetPoly(GetArrowheadPointsCount(atEnd: false));
				PointF fromArrowAnchorPoint = FromArrowAnchorPoint;
				PointF fromArrowEndPoint = FromArrowEndPoint;
				CalculateArrowhead(fromArrowAnchorPoint, fromArrowEndPoint, atEnd: false, array);
			}
			if (ToArrow && pointsCount >= 2)
			{
				if (myToArrowInfo == null)
				{
					myToArrowInfo = new ArrowInfo();
				}
				array2 = myToArrowInfo.GetPoly(GetArrowheadPointsCount(atEnd: true));
				PointF toArrowAnchorPoint = ToArrowAnchorPoint;
				PointF toArrowEndPoint = ToArrowEndPoint;
				CalculateArrowhead(toArrowAnchorPoint, toArrowEndPoint, atEnd: true, array2);
			}
			if (Shadowed && Pen != null)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				Pen shadowPen = GetShadowPen(view, PenWidth);
				if (shadowPen != null)
				{
					GraphicsPath path = GetPath(shadowOffset.Width, shadowOffset.Height, array, array2);
					GoShape.DrawPath(g, view, shadowPen, null, path);
					DisposePath(path);
				}
				Brush shadowBrush = GetShadowBrush(view);
				if (array != null)
				{
					DrawArrowhead(g, view, shadowPen, shadowBrush, atEnd: false, shadowOffset.Width, shadowOffset.Height, array);
				}
				if (array2 != null)
				{
					DrawArrowhead(g, view, shadowPen, shadowBrush, atEnd: true, shadowOffset.Width, shadowOffset.Height, array2);
				}
			}
			GraphicsPath path2 = GetPath(0f, 0f, array, array2);
			Pen highlightPen = HighlightPen;
			if (highlightPen != null && Highlight)
			{
				GoShape.DrawPath(g, view, highlightPen, null, path2);
			}
			if (pen != null)
			{
				GoShape.DrawPath(g, view, pen, null, path2);
			}
			DisposePath(path2);
			if (array != null || array2 != null)
			{
				if (pen2.DashStyle != 0 || pen2.Width > 1f)
				{
					pen2 = new Pen(GoShape.GetPenColor(pen2, Color.Black));
					if (array != null)
					{
						DrawArrowhead(g, view, pen2, brush, atEnd: false, 0f, 0f, array);
					}
					if (array2 != null)
					{
						DrawArrowhead(g, view, pen2, brush, atEnd: true, 0f, 0f, array2);
					}
					pen2.Dispose();
				}
				else
				{
					if (array != null)
					{
						DrawArrowhead(g, view, pen2, brush, atEnd: false, 0f, 0f, array);
					}
					if (array2 != null)
					{
						DrawArrowhead(g, view, pen2, brush, atEnd: true, 0f, 0f, array2);
					}
				}
			}
			if (base.Layer != null && view != null && (Style == GoStrokeStyle.RoundedLineWithJumpOvers || Style == GoStrokeStyle.RoundedLineWithJumpGaps))
			{
				GoLayer.GoLayerCache goLayerCache = base.Layer.FindCache(view);
				if (goLayerCache != null && !goLayerCache.Strokes.Contains(this))
				{
					goLayerCache.Strokes.Add(this);
				}
			}
		}

		/// <summary>
		/// Returns a <c>GraphicsPath</c> representation of what will be drawn.
		/// </summary>
		/// <returns></returns>
		/// <remarks>The path will not include the arrowheads.</remarks>
		public override GraphicsPath MakePath()
		{
			return (GraphicsPath)GetPath(0f, 0f, null, null).Clone();
		}

		private GraphicsPath GetPath(float offx, float offy, PointF[] fromPoly, PointF[] toPoly)
		{
			if (offx != 0f || offy != 0f || Style == GoStrokeStyle.RoundedLineWithJumpOvers || Style == GoStrokeStyle.RoundedLineWithJumpGaps)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				addStroke(graphicsPath, offx, offy, fromPoly, toPoly);
				return graphicsPath;
			}
			if (myPath == null)
			{
				myPath = new GraphicsPath();
				addStroke(myPath, 0f, 0f, fromPoly, toPoly);
			}
			return myPath;
		}

		private void addStroke(GraphicsPath path, float offx, float offy, PointF[] fromPoly, PointF[] toPoly)
		{
			int pointsCount = PointsCount;
			checked
			{
				if (Style == GoStrokeStyle.Bezier && pointsCount >= 4)
				{
					for (int i = 3; i < pointsCount; i += 3)
					{
						PointF pointF = (fromPoly == null || i - 3 != 0 || !(FromArrowShaftLength > 0f) || !(fromPoly[2] == GetPoint(0))) ? GetPoint(i - 3) : fromPoly[0];
						PointF point = GetPoint(i - 2);
						if (i + 3 >= pointsCount)
						{
							i = pointsCount - 1;
						}
						PointF point2 = GetPoint(i - 1);
						PointF pointF2 = (toPoly == null || i != pointsCount - 1 || !(ToArrowShaftLength > 0f) || !(toPoly[2] == GetPoint(i))) ? GetPoint(i) : toPoly[0];
						path.AddBezier(pointF.X + offx, pointF.Y + offy, point.X + offx, point.Y + offy, point2.X + offx, point2.Y + offy, pointF2.X + offx, pointF2.Y + offy);
					}
				}
				else
				{
					if (pointsCount < 2)
					{
						return;
					}
					if (pointsCount == 2 || Style == GoStrokeStyle.Line || Style == GoStrokeStyle.Bezier || (IsApprox(Curviness, 0f) && (Style != GoStrokeStyle.RoundedLineWithJumpOvers || Style != GoStrokeStyle.RoundedLineWithJumpGaps)))
					{
						for (int j = 1; j < pointsCount; j++)
						{
							PointF pointF3 = (fromPoly == null || j - 1 != 0 || !(FromArrowShaftLength > 0f) || !(fromPoly[2] == GetPoint(0))) ? GetPoint(j - 1) : fromPoly[0];
							PointF pointF4 = (toPoly == null || j != pointsCount - 1 || !(ToArrowShaftLength > 0f) || !(toPoly[2] == GetPoint(j))) ? GetPoint(j) : toPoly[0];
							path.AddLine(pointF3.X + offx, pointF3.Y + offy, pointF4.X + offx, pointF4.Y + offy);
						}
						return;
					}
					PointF pointF5 = GetPoint(0);
					if (fromPoly != null && FromArrowShaftLength > 0f && fromPoly[2] == pointF5)
					{
						pointF5 = fromPoly[0];
					}
					int num = 1;
					PointF pointF6;
					while (true)
					{
						if (num < pointsCount)
						{
							num = furthestPoint(pointF5, num, num > 1);
							pointF6 = GetPoint(num);
							if (num >= pointsCount - 1)
							{
								break;
							}
							int num2 = furthestPoint(pointF6, num + 1, num < pointsCount - 3);
							PointF pointF7 = GetPoint(num2);
							if (toPoly != null && num2 == pointsCount - 1 && ToArrowShaftLength > 0f && toPoly[2] == pointF7)
							{
								pointF7 = toPoly[0];
							}
							pointF5 = addLineAndCorner(path, offx, offy, pointF5, pointF6, pointF7);
							num = num2;
							continue;
						}
						return;
					}
					if (toPoly != null && ToArrowShaftLength > 0f && toPoly[2] == pointF6)
					{
						pointF6 = toPoly[0];
					}
					if (pointF5 != pointF6)
					{
						addLine(path, offx, offy, pointF5, pointF6);
					}
				}
			}
		}

		private int furthestPoint(PointF a, int i, bool oneway)
		{
			int pointsCount = PointsCount;
			PointF pointF = a;
			checked
			{
				while (IsApprox(a.X, pointF.X) && IsApprox(a.Y, pointF.Y))
				{
					if (i >= pointsCount)
					{
						return pointsCount - 1;
					}
					pointF = GetPoint(i++);
				}
				if (!IsApprox(a.X, pointF.X) && !IsApprox(a.Y, pointF.Y))
				{
					return i - 1;
				}
				PointF pointF2 = pointF;
				while ((IsApprox(a.X, pointF.X) && IsApprox(pointF.X, pointF2.X) && (!oneway || ((a.Y >= pointF.Y) ? (pointF.Y >= pointF2.Y) : (pointF.Y <= pointF2.Y)))) || (IsApprox(a.Y, pointF.Y) && IsApprox(pointF.Y, pointF2.Y) && (!oneway || ((a.X >= pointF.X) ? (pointF.X >= pointF2.X) : (pointF.X <= pointF2.X)))))
				{
					if (i >= pointsCount)
					{
						return pointsCount - 1;
					}
					pointF2 = GetPoint(i++);
				}
				return i - 2;
			}
		}

		private PointF addLineAndCorner(GraphicsPath path, float offx, float offy, PointF a, PointF b, PointF c)
		{
			if (IsApprox(a.Y, b.Y) && IsApprox(b.X, c.X))
			{
				float val = Math.Min(Math.Abs(Curviness), Math.Abs(b.X - a.X) / 2f);
				float num = Math.Min(val, Math.Abs(c.Y - b.Y) / 2f);
				val = num;
				if (IsApprox(val, 0f))
				{
					addLine(path, offx, offy, a, b);
					return b;
				}
				PointF to = b;
				PointF result = b;
				float sweepAngle = 90f;
				RectangleF rect = new RectangleF(0f, 0f, val * 2f, num * 2f);
				float startAngle;
				if (b.X > a.X)
				{
					to.X = b.X - val;
					if (c.Y > b.Y)
					{
						result.Y = b.Y + num;
						startAngle = 270f;
						rect.X = b.X - val * 2f;
						rect.Y = b.Y;
					}
					else
					{
						result.Y = b.Y - num;
						startAngle = 90f;
						sweepAngle = -90f;
						rect.X = b.X - val * 2f;
						rect.Y = b.Y - num * 2f;
					}
				}
				else
				{
					to.X = b.X + val;
					if (c.Y > b.Y)
					{
						result.Y = b.Y + num;
						startAngle = 270f;
						sweepAngle = -90f;
						rect.X = b.X;
						rect.Y = b.Y;
					}
					else
					{
						result.Y = b.Y - num;
						startAngle = 90f;
						rect.X = b.X;
						rect.Y = b.Y - num * 2f;
					}
				}
				addLine(path, offx, offy, a, to);
				rect.X += offx;
				rect.Y += offy;
				path.AddArc(rect, startAngle, sweepAngle);
				return result;
			}
			if (IsApprox(a.X, b.X) && IsApprox(b.Y, c.Y))
			{
				float val2 = Math.Min(Math.Abs(Curviness), Math.Abs(b.Y - a.Y) / 2f);
				float num2 = Math.Min(val2, Math.Abs(c.X - b.X) / 2f);
				val2 = num2;
				if (IsApprox(num2, 0f))
				{
					addLine(path, offx, offy, a, b);
					return b;
				}
				PointF to2 = b;
				PointF result2 = b;
				float sweepAngle2 = 90f;
				RectangleF rect2 = new RectangleF(0f, 0f, num2 * 2f, val2 * 2f);
				float startAngle2;
				if (b.Y > a.Y)
				{
					to2.Y = b.Y - val2;
					if (c.X > b.X)
					{
						result2.X = b.X + num2;
						startAngle2 = 180f;
						sweepAngle2 = -90f;
						rect2.Y = b.Y - val2 * 2f;
						rect2.X = b.X;
					}
					else
					{
						result2.X = b.X - num2;
						startAngle2 = 0f;
						rect2.Y = b.Y - val2 * 2f;
						rect2.X = b.X - num2 * 2f;
					}
				}
				else
				{
					to2.Y = b.Y + val2;
					if (c.X > b.X)
					{
						result2.X = b.X + num2;
						startAngle2 = 180f;
						rect2.Y = b.Y;
						rect2.X = b.X;
					}
					else
					{
						result2.X = b.X - num2;
						startAngle2 = 0f;
						sweepAngle2 = -90f;
						rect2.Y = b.Y;
						rect2.X = b.X - num2 * 2f;
					}
				}
				addLine(path, offx, offy, a, to2);
				rect2.X += offx;
				rect2.Y += offy;
				path.AddArc(rect2, startAngle2, sweepAngle2);
				return result2;
			}
			addLine(path, offx, offy, a, b);
			return b;
		}

		private void addLine(GraphicsPath path, float offx, float offy, PointF from, PointF to)
		{
			if (Style != GoStrokeStyle.RoundedLineWithJumpOvers && Style != GoStrokeStyle.RoundedLineWithJumpGaps)
			{
				path.AddLine(from.X + offx, from.Y + offy, to.X + offx, to.Y + offy);
				return;
			}
			float num = 10f;
			float num2 = num / 2f;
			checked
			{
				lock (myIntersections)
				{
					float[] array = myIntersections;
					int intersections = getIntersections(from, to, array);
					PointF pointF = from;
					if (intersections > 0)
					{
						if (IsApprox(from.Y, to.Y))
						{
							if (from.X < to.X)
							{
								int num3 = 0;
								while (num3 < intersections)
								{
									float num4 = Math.Max(from.X, Math.Min(array[num3++] - num2, to.X - num));
									path.AddLine(pointF.X + offx, pointF.Y + offy, num4 + offx, to.Y + offy);
									pointF = new PointF(num4 + offx, to.Y + offy);
									float num5 = Math.Min(num4 + num, to.X);
									while (num3 < intersections)
									{
										float num6 = array[num3];
										if (!(num6 < num5 + num))
										{
											break;
										}
										num3++;
										num5 = Math.Min(num6 + num2, to.X);
									}
									PointF pointF2 = new PointF((num4 + num5) / 2f, to.Y - num);
									PointF pointF3 = new PointF(num5, to.Y);
									if (Style == GoStrokeStyle.RoundedLineWithJumpOvers)
									{
										path.AddBezier(pointF.X, pointF.Y, pointF.X, pointF2.Y, pointF3.X, pointF2.Y, pointF3.X, pointF3.Y);
									}
									else
									{
										path.StartFigure();
									}
									pointF = pointF3;
								}
							}
							else
							{
								int num7 = intersections - 1;
								while (num7 >= 0)
								{
									float num8 = Math.Min(from.X, Math.Max(array[num7--] + num2, to.X + num));
									path.AddLine(pointF.X + offx, pointF.Y + offy, num8 + offx, to.Y + offy);
									pointF = new PointF(num8 + offx, to.Y + offy);
									float num9 = Math.Max(num8 - num, to.X);
									while (num7 >= 0)
									{
										float num10 = array[num7];
										if (!(num10 > num9 - num))
										{
											break;
										}
										num7--;
										num9 = Math.Max(num10 - num2, to.X);
									}
									PointF pointF4 = new PointF((num8 + num9) / 2f, to.Y - num);
									PointF pointF5 = new PointF(num9, to.Y);
									if (Style == GoStrokeStyle.RoundedLineWithJumpOvers)
									{
										path.AddBezier(pointF.X, pointF.Y, pointF.X, pointF4.Y, pointF5.X, pointF4.Y, pointF5.X, pointF5.Y);
									}
									else
									{
										path.StartFigure();
									}
									pointF = pointF5;
								}
							}
						}
						else if (IsApprox(from.X, to.X))
						{
							if (from.Y < to.Y)
							{
								int num11 = 0;
								while (num11 < intersections)
								{
									float num12 = Math.Max(from.Y, Math.Min(array[num11++] - num2, to.Y - num));
									path.AddLine(pointF.X + offx, pointF.Y + offy, to.X + offx, num12 + offy);
									pointF = new PointF(to.X + offx, num12 + offy);
									float num13 = Math.Min(num12 + num, to.Y);
									while (num11 < intersections)
									{
										float num14 = array[num11];
										if (!(num14 < num13 + num))
										{
											break;
										}
										num11++;
										num13 = Math.Min(num14 + num2, to.Y);
									}
									PointF pointF6 = new PointF(to.X - num, (num12 + num13) / 2f);
									PointF pointF7 = new PointF(to.X, num13);
									if (Style == GoStrokeStyle.RoundedLineWithJumpOvers)
									{
										path.AddBezier(pointF.X, pointF.Y, pointF6.X, pointF.Y, pointF6.X, pointF7.Y, pointF7.X, pointF7.Y);
									}
									else
									{
										path.StartFigure();
									}
									pointF = pointF7;
								}
							}
							else
							{
								int num15 = intersections - 1;
								while (num15 >= 0)
								{
									float num16 = Math.Min(from.Y, Math.Max(array[num15--] + num2, to.Y + num));
									path.AddLine(pointF.X + offx, pointF.Y + offy, to.X + offx, num16 + offy);
									pointF = new PointF(to.X + offx, num16 + offy);
									float num17 = Math.Max(num16 - num, to.Y);
									while (num15 >= 0)
									{
										float num18 = array[num15];
										if (!(num18 > num17 - num))
										{
											break;
										}
										num15--;
										num17 = Math.Max(num18 - num2, to.Y);
									}
									PointF pointF8 = new PointF(to.X - num, (num16 + num17) / 2f);
									PointF pointF9 = new PointF(to.X, num17);
									if (Style == GoStrokeStyle.RoundedLineWithJumpOvers)
									{
										path.AddBezier(pointF.X, pointF.Y, pointF8.X, pointF.Y, pointF8.X, pointF9.Y, pointF9.X, pointF9.Y);
									}
									else
									{
										path.StartFigure();
									}
									pointF = pointF9;
								}
							}
						}
					}
					path.AddLine(pointF.X + offx, pointF.Y + offy, to.X + offx, to.Y + offy);
				}
			}
		}

		private int getIntersections(PointF A, PointF B, float[] v)
		{
			GoDocument document = base.Document;
			if (document == null)
			{
				return 0;
			}
			float num = Math.Min(A.X, B.X);
			float num2 = Math.Min(A.Y, B.Y);
			float num3 = Math.Max(A.X, B.X);
			float num4 = Math.Max(A.Y, B.Y);
			RectangleF r = new RectangleF(num, num2, num3 - num, num4 - num2);
			int num5 = 0;
			foreach (GoLayer layer in document.Layers)
			{
				if (layer.CanViewObjects())
				{
					GoLayer.GoLayerCache goLayerCache = layer.FindCache(r);
					if (goLayerCache != null)
					{
						List<GoStroke> list = null;
						foreach (GoStroke stroke in goLayerCache.Strokes)
						{
							if (stroke.Layer == null)
							{
								if (list == null)
								{
									list = new List<GoStroke>();
								}
								list.Add(stroke);
							}
							else
							{
								if (stroke == this)
								{
									if (list != null)
									{
										foreach (GoStroke item in list)
										{
											GoCollection.fastRemove(goLayerCache.Strokes, item);
										}
									}
									Array.Sort(v, 0, num5, Comparer<float>.Default);
									return num5;
								}
								num5 = getIntersections2(A, B, v, num5, stroke);
							}
						}
						if (list != null)
						{
							foreach (GoStroke item2 in list)
							{
								GoCollection.fastRemove(goLayerCache.Strokes, item2);
							}
						}
					}
					else
					{
						foreach (GoObject item3 in layer)
						{
							GoStroke goStroke = item3 as GoStroke;
							if (goStroke == null)
							{
								GoLabeledLink goLabeledLink = item3 as GoLabeledLink;
								if (goLabeledLink == null)
								{
									continue;
								}
								goStroke = goLabeledLink.RealLink;
							}
							if ((goStroke.Style == GoStrokeStyle.RoundedLineWithJumpOvers || goStroke.Style == GoStrokeStyle.RoundedLineWithJumpGaps) && goStroke.CanView())
							{
								if (goStroke == this)
								{
									Array.Sort(v, 0, num5, Comparer<float>.Default);
									return num5;
								}
								num5 = getIntersections2(A, B, v, num5, goStroke);
							}
						}
					}
				}
			}
			Array.Sort(v, 0, num5, Comparer<float>.Default);
			return num5;
		}

		private int getIntersections2(PointF A, PointF B, float[] v, int numints, GoStroke link)
		{
			checked
			{
				if (link.CanView())
				{
					int pointsCount = link.PointsCount;
					for (int i = 1; i < pointsCount; i++)
					{
						PointF point = link.GetPoint(i - 1);
						PointF point2 = link.GetPoint(i);
						PointF result = default(PointF);
						if (getOrthoSegmentIntersection(A, B, point, point2, ref result) && numints < v.Length)
						{
							if (IsApprox(A.Y, B.Y))
							{
								v[numints++] = result.X;
							}
							else
							{
								v[numints++] = result.Y;
							}
						}
					}
				}
				return numints;
			}
		}

		private bool getOrthoSegmentIntersection(PointF A, PointF B, PointF C, PointF D, ref PointF result)
		{
			if (!IsApprox(A.X, B.X))
			{
				if (IsApprox(C.X, D.X) && Math.Min(A.X, B.X) < C.X && Math.Max(A.X, B.X) > C.X && Math.Min(C.Y, D.Y) < A.Y && Math.Max(C.Y, D.Y) > A.Y)
				{
					result.X = C.X;
					result.Y = A.Y;
					return true;
				}
			}
			else if (IsApprox(C.Y, D.Y) && Math.Min(A.Y, B.Y) < C.Y && Math.Max(A.Y, B.Y) > C.Y && Math.Min(C.X, D.X) < A.X && Math.Max(C.X, D.X) > A.X)
			{
				result.X = A.X;
				result.Y = C.Y;
				return true;
			}
			result.X = 0f;
			result.Y = 0f;
			return false;
		}

		/// <summary>
		/// Copies state to permit an undo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1204)
			{
				if (e.IsBeforeChanging)
				{
					PointF[] array2 = (PointF[])(e.OldValue = CopyPointsArray());
				}
			}
			else
			{
				base.CopyOldValueForUndo(e);
			}
		}

		/// <summary>
		/// Copies state to permit a redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void CopyNewValueForRedo(GoChangedEventArgs e)
		{
			int subHint = e.SubHint;
			if (subHint == 1204)
			{
				if (!e.IsBeforeChanging)
				{
					PointF[] array2 = (PointF[])(e.NewValue = CopyPointsArray());
				}
			}
			else
			{
				base.CopyNewValueForRedo(e);
			}
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1001:
			{
				base.ChangeValue(e, undo);
				RectangleF rect = e.GetRect(!undo);
				RectangleF rect2 = e.GetRect(undo);
				if (rect.Width == rect2.Width && rect.Height == rect2.Height)
				{
					float dx = rect2.X - rect.X;
					float dy = rect2.Y - rect.Y;
					TranslatePoints(myPoints, dx, dy);
				}
				break;
			}
			case 1201:
				if (undo)
				{
					InternalRemovePoint(e.OldInt);
				}
				else
				{
					InternalInsertPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
				}
				break;
			case 1202:
				if (undo)
				{
					InternalInsertPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
				}
				else
				{
					InternalRemovePoint(e.OldInt);
				}
				break;
			case 1203:
				if (undo)
				{
					InternalSetPoint(e.OldInt, new PointF(e.OldRect.X, e.OldRect.Y));
				}
				else
				{
					InternalSetPoint(e.OldInt, new PointF(e.NewRect.X, e.NewRect.Y));
				}
				break;
			case 1250:
				ToArrow = (bool)e.GetValue(undo);
				break;
			case 1251:
				ToArrowLength = e.GetFloat(undo);
				break;
			case 1252:
				ToArrowShaftLength = e.GetFloat(undo);
				break;
			case 1253:
				ToArrowWidth = e.GetFloat(undo);
				break;
			case 1254:
				ToArrowFilled = (bool)e.GetValue(undo);
				break;
			case 1255:
				ToArrowStyle = (GoStrokeArrowheadStyle)e.GetValue(undo);
				break;
			case 1260:
				FromArrow = (bool)e.GetValue(undo);
				break;
			case 1261:
				FromArrowLength = e.GetFloat(undo);
				break;
			case 1262:
				FromArrowShaftLength = e.GetFloat(undo);
				break;
			case 1263:
				FromArrowWidth = e.GetFloat(undo);
				break;
			case 1264:
				FromArrowFilled = (bool)e.GetValue(undo);
				break;
			case 1265:
				FromArrowStyle = (GoStrokeArrowheadStyle)e.GetValue(undo);
				break;
			case 1205:
				Style = (GoStrokeStyle)e.GetValue(undo);
				break;
			case 1206:
				Curviness = e.GetFloat(undo);
				break;
			case 1236:
			{
				object value = e.GetValue(undo);
				if (value is Pen)
				{
					HighlightPen = (Pen)value;
				}
				else if (value is GoPenInfo)
				{
					HighlightPenInfo = (GoPenInfo)value;
				}
				break;
			}
			case 1237:
				Highlight = (bool)e.GetValue(undo);
				break;
			case 1238:
				HighlightWhenSelected = (bool)e.GetValue(undo);
				break;
			case 1204:
			{
				PointF[] points = (PointF[])e.GetValue(undo);
				SetPoints(points);
				break;
			}
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
