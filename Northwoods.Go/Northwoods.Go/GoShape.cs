using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// An abstract geometrical shape that uses an optional <c>Pen</c> to draw an outline and
	/// an optional <c>Brush</c> to fill the outline.
	/// </summary>
	/// <remarks>
	/// <para>
	/// There are several very commonly used classes inheriting from <c>GoShape</c>:
	/// <see cref="T:Northwoods.Go.GoRectangle" />, <see cref="T:Northwoods.Go.GoRoundedRectangle" />, and <see cref="T:Northwoods.Go.GoEllipse" />.
	/// The following classes allow you to determine the shape:
	/// <see cref="T:Northwoods.Go.GoStroke" /> for straight and curved multi-segment lines,
	/// <see cref="T:Northwoods.Go.GoPolygon" /> for closed figures using straight or curved sides,
	/// and most generally, <see cref="T:Northwoods.Go.GoDrawing" />.
	/// <see cref="T:Northwoods.Go.GoDrawing" /> also supports many predefined shapes that you can
	/// specify with the <see cref="T:Northwoods.Go.GoFigure" /> enumeration.
	/// Other shape classes provide properties to easily specify particular characteristics,
	/// and they may provide specialized resizing behavior.
	/// </para>
	/// <para>
	/// Although it is most common to use the predefined <b>Brush</b>es given by
	/// the <b>System.Drawing.Brushes</b> class, you can also set the <see cref="P:Northwoods.Go.GoShape.Brush" />
	/// property to any <b>SolidBrush</b> or <b>HatchBrush</b>, as well as many kinds of
	/// simple <b>LinearGradientBrush</b> or <b>PathGradientBrush</b>.
	/// Be sure to construct and completely initialize the <b>Brush</b> before assigning
	/// it to the <see cref="P:Northwoods.Go.GoShape.Brush" /> property -- you may not modify a <b>Brush</b>
	/// after assigning it to a <b>Brush</b> property defined by GoDiagram.
	/// </para>
	/// <para>
	/// It is also very common to assign the <see cref="P:Northwoods.Go.GoShape.BrushColor" />, <see cref="P:Northwoods.Go.GoShape.BrushStyle" />,
	/// <see cref="P:Northwoods.Go.GoShape.BrushForeColor" />, <see cref="P:Northwoods.Go.GoShape.BrushMidColor" />, and <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" />
	/// properties.
	/// </para>
	/// <para>
	/// For somewhat more complex effects, there are several <b>Fill...</b> methods defined on <see cref="T:Northwoods.Go.GoShape" />
	/// that set the <see cref="P:Northwoods.Go.GoShape.Brush" /> property to commonly used <b>LinearGradientBrush</b>es
	/// or <b>PathGradientBrush</b>es.
	/// </para>
	/// <para>
	/// As with brushes, you can set the <see cref="P:Northwoods.Go.GoShape.Pen" /> property to any of the predefined
	/// <b>System.Drawing.Pens</b>, or you can construct your own <b>Pen</b>.
	/// Be sure to construct and completely initialize the <b>Pen</b> before assigning
	/// it to the <see cref="P:Northwoods.Go.GoShape.Pen" /> property -- you may not modify a <b>Pen</b>
	/// after assigning it to a <b>Pen</b> property defined by GoDiagram.
	/// </para>
	/// <para>
	/// It is also very common to assign the <see cref="P:Northwoods.Go.GoShape.PenColor" /> and <see cref="P:Northwoods.Go.GoShape.PenWidth" /> properties.
	/// </para>
	/// </remarks>
	[Serializable]
	public abstract class GoShape : GoObject
	{
		[Serializable]
		internal sealed class GoPenInfo
		{
			[NonSerialized]
			private Pen myPen;

			private Color myColor;

			private float myWidth;

			private GoPenInfoEx myEx;

			public Color Color
			{
				get
				{
					return myColor;
				}
				set
				{
					myColor = value;
				}
			}

			public float Width
			{
				get
				{
					return myWidth;
				}
				set
				{
					myWidth = value;
				}
			}

			public float MiterLimit
			{
				get
				{
					if (myEx != null)
					{
						return myEx.myMiterLimit;
					}
					return 10f;
				}
			}

			public GoPenInfo()
			{
			}

			public GoPenInfo(Pen p)
			{
				SetPen(p);
			}

			public GoPenInfo(GoPenInfo other)
			{
				if (other != null)
				{
					myColor = other.myColor;
					myWidth = other.myWidth;
					myEx = ((other.myEx != null) ? new GoPenInfoEx(other.myEx) : null);
				}
			}

			public override bool Equals(object obj)
			{
				GoPenInfo goPenInfo = obj as GoPenInfo;
				if (goPenInfo == null)
				{
					return false;
				}
				if (myColor == goPenInfo.myColor && myWidth == goPenInfo.myWidth)
				{
					if (myEx == null)
					{
						return goPenInfo.myEx == null;
					}
					return myEx.Equals(goPenInfo.myEx);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return myColor.GetHashCode() ^ myWidth.GetHashCode() ^ ((myEx != null) ? myEx.GetHashCode() : 0);
			}

			public bool SetPen(Pen p)
			{
				myPen = p;
				myColor = GetPenColor(p, Color.Empty);
				myWidth = GetPenWidth(p);
				if (p.DashStyle != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myDashStyle = p.DashStyle;
				}
				if (p.DashCap != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myDashCap = p.DashCap;
				}
				if (p.DashOffset != 0f)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myDashOffset = p.DashOffset;
				}
				if (p.DashStyle == DashStyle.Custom)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myDashPattern = p.DashPattern;
				}
				if (p.Alignment != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myAlignment = p.Alignment;
				}
				if (p.EndCap != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myEndCap = p.EndCap;
				}
				if (p.StartCap != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myStartCap = p.StartCap;
				}
				if (p.LineJoin != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myLineJoin = p.LineJoin;
				}
				if (p.MiterLimit != 10f)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myMiterLimit = p.MiterLimit;
				}
				float[] compoundArray = p.CompoundArray;
				if (compoundArray != null && compoundArray.Length != 0)
				{
					if (myEx == null)
					{
						myEx = new GoPenInfoEx();
					}
					myEx.myCompoundFractions = compoundArray;
				}
				if (myColor == Color.Empty)
				{
					myColor = Color.Black;
					return false;
				}
				return true;
			}

			public Pen GetPen()
			{
				if (myPen == null)
				{
					myPen = new Pen(Color, Width);
					if (myEx != null)
					{
						myPen.DashStyle = myEx.myDashStyle;
						myPen.DashCap = myEx.myDashCap;
						myPen.DashOffset = myEx.myDashOffset;
						if (myEx.myDashStyle == DashStyle.Custom)
						{
							myPen.DashPattern = myEx.myDashPattern;
						}
						myPen.Alignment = myEx.myAlignment;
						myPen.EndCap = myEx.myEndCap;
						myPen.StartCap = myEx.myStartCap;
						myPen.LineJoin = myEx.myLineJoin;
						myPen.MiterLimit = myEx.myMiterLimit;
						if (myEx.myCompoundFractions != null)
						{
							myPen.CompoundArray = myEx.myCompoundFractions;
						}
					}
				}
				return myPen;
			}
		}

		[Serializable]
		internal sealed class GoPenInfoEx
		{
			internal DashStyle myDashStyle;

			internal DashCap myDashCap;

			internal float myDashOffset;

			internal float[] myDashPattern;

			internal PenAlignment myAlignment;

			internal LineCap myEndCap;

			internal LineCap myStartCap;

			internal LineJoin myLineJoin;

			internal float myMiterLimit = 10f;

			internal float[] myCompoundFractions;

			public GoPenInfoEx()
			{
			}

			public GoPenInfoEx(GoPenInfoEx other)
			{
				myDashStyle = other.myDashStyle;
				myDashCap = other.myDashCap;
				myDashOffset = other.myDashOffset;
				if (other.myDashPattern != null)
				{
					myDashPattern = (float[])other.myDashPattern.Clone();
				}
				myAlignment = other.myAlignment;
				myEndCap = other.myEndCap;
				myStartCap = other.myStartCap;
				myLineJoin = other.myLineJoin;
				myMiterLimit = other.myMiterLimit;
				if (other.myCompoundFractions != null)
				{
					myCompoundFractions = (float[])other.myCompoundFractions.Clone();
				}
			}

			public override bool Equals(object obj)
			{
				GoPenInfoEx goPenInfoEx = obj as GoPenInfoEx;
				if (goPenInfoEx == null)
				{
					return false;
				}
				if (myDashStyle != goPenInfoEx.myDashStyle || myDashCap != goPenInfoEx.myDashCap || myDashOffset != goPenInfoEx.myDashOffset || myAlignment != goPenInfoEx.myAlignment || myEndCap != goPenInfoEx.myEndCap || myStartCap != goPenInfoEx.myStartCap || myLineJoin != goPenInfoEx.myLineJoin || myMiterLimit != goPenInfoEx.myMiterLimit)
				{
					return false;
				}
				checked
				{
					if (myDashStyle == DashStyle.Custom && (myDashPattern != null || goPenInfoEx.myDashPattern != null))
					{
						if (myDashPattern == null || goPenInfoEx.myDashPattern == null)
						{
							return false;
						}
						if (myDashPattern.Length != goPenInfoEx.myDashPattern.Length)
						{
							return false;
						}
						for (int i = 0; i < myDashPattern.Length; i++)
						{
							if (myDashPattern[i] != goPenInfoEx.myDashPattern[i])
							{
								return false;
							}
						}
					}
					if (myCompoundFractions != null || goPenInfoEx.myCompoundFractions != null)
					{
						if (myCompoundFractions == null || goPenInfoEx.myCompoundFractions == null)
						{
							return false;
						}
						if (myCompoundFractions.Length != goPenInfoEx.myCompoundFractions.Length)
						{
							return false;
						}
						for (int j = 0; j < myCompoundFractions.Length; j++)
						{
							if (myCompoundFractions[j] != goPenInfoEx.myCompoundFractions[j])
							{
								return false;
							}
						}
					}
					return true;
				}
			}

			public override int GetHashCode()
			{
				int num = (int)myDashStyle ^ (int)myDashCap ^ myDashOffset.GetHashCode() ^ (int)myAlignment ^ (int)myEndCap ^ (int)myStartCap ^ (int)myLineJoin ^ myMiterLimit.GetHashCode();
				if (myDashStyle == DashStyle.Custom && myDashPattern != null)
				{
					num ^= myDashPattern.GetHashCode();
				}
				if (myCompoundFractions != null)
				{
					num ^= myCompoundFractions.GetHashCode();
				}
				return num;
			}
		}

		internal enum GoBrushType
		{
			None,
			Solid,
			Hatch,
			Texture,
			LinearGradient,
			LinearGradientGamma,
			PathGradient
		}

		[Serializable]
		internal sealed class GoBrushInfo
		{
			[NonSerialized]
			private Brush myBrush;

			private Color myColor;

			private GoBrushInfoEx myEx;

			public bool HasBrush => myBrush != null;

			public Color Color
			{
				get
				{
					return myColor;
				}
				set
				{
					myColor = value;
				}
			}

			public GoBrushType BrushType
			{
				get
				{
					if (myEx == null)
					{
						return GoBrushType.Solid;
					}
					return myEx.myBrushType;
				}
			}

			public GoBrushStyle BrushStyle
			{
				get
				{
					if (myEx == null)
					{
						return GoBrushStyle.Solid;
					}
					return myEx.myBrushStyle;
				}
				set
				{
					if (myEx == null && value != GoBrushStyle.Solid)
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myBrushStyle = value;
						switch (value)
						{
						case GoBrushStyle.None:
							myEx.myBrushType = GoBrushType.None;
							break;
						case GoBrushStyle.Solid:
							myEx.myBrushType = GoBrushType.Solid;
							break;
						case GoBrushStyle.Texture:
							myEx.myBrushType = GoBrushType.Texture;
							break;
						case GoBrushStyle.SimpleGradientVertical:
						case GoBrushStyle.SimpleGradientHorizontal:
						case GoBrushStyle.SimpleGradientForwardDiagonal:
						case GoBrushStyle.SimpleGradientBackwardDiagonal:
							myEx.myBrushType = GoBrushType.LinearGradientGamma;
							myEx.myWrapMode = WrapMode.TileFlipXY;
							break;
						case GoBrushStyle.MiddleGradientVertical:
						case GoBrushStyle.MiddleGradientHorizontal:
						case GoBrushStyle.MiddleGradientForwardDiagonal:
						case GoBrushStyle.MiddleGradientBackwardDiagonal:
							myEx.myBrushType = GoBrushType.LinearGradientGamma;
							myEx.myWrapMode = WrapMode.TileFlipXY;
							break;
						case GoBrushStyle.SingleEdgeGradientTop:
						case GoBrushStyle.SingleEdgeGradientLeft:
						case GoBrushStyle.SingleEdgeGradientRight:
						case GoBrushStyle.SingleEdgeGradientBottom:
							myEx.myBrushType = GoBrushType.LinearGradientGamma;
							myEx.myWrapMode = WrapMode.TileFlipXY;
							break;
						case GoBrushStyle.DoubleEdgeGradientVertical:
						case GoBrushStyle.DoubleEdgeGradientHorizontal:
						case GoBrushStyle.DoubleEdgeGradientForwardDiagonal:
						case GoBrushStyle.DoubleEdgeGradientBackwardDiagonal:
							myEx.myBrushType = GoBrushType.LinearGradientGamma;
							myEx.myWrapMode = WrapMode.TileFlipXY;
							break;
						case GoBrushStyle.ShapeFringeGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							myEx.myWrapMode = WrapMode.Clamp;
							break;
						case GoBrushStyle.ShapeSimpleGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							myEx.myWrapMode = WrapMode.Clamp;
							break;
						case GoBrushStyle.ShapeHighlightGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							myEx.myWrapMode = WrapMode.TileFlipXY;
							break;
						case GoBrushStyle.RectangleGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							myEx.myWrapMode = WrapMode.TileFlipX;
							break;
						case GoBrushStyle.EllipseGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							myEx.myWrapMode = WrapMode.TileFlipY;
							break;
						case GoBrushStyle.CustomLinearGradient:
							myEx.myBrushType = GoBrushType.LinearGradient;
							break;
						case GoBrushStyle.CustomPathGradient:
							myEx.myBrushType = GoBrushType.PathGradient;
							break;
						default:
							myEx.myBrushType = GoBrushType.Hatch;
							break;
						}
					}
				}
			}

			public Color ForeColor
			{
				get
				{
					if (myEx == null)
					{
						return Color.Empty;
					}
					return myEx.myForeColor;
				}
				set
				{
					if (myEx == null && value != Color.Empty)
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myForeColor = value;
					}
				}
			}

			public Color MidColor
			{
				get
				{
					if (myEx == null)
					{
						return Color.Empty;
					}
					return myEx.myMidColor;
				}
				set
				{
					if (myEx == null && value != Color.Empty)
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myMidColor = value;
					}
				}
			}

			public PointF StartOrFocusScales
			{
				get
				{
					if (myEx == null)
					{
						return default(PointF);
					}
					return myEx.myStartOrFocusScales;
				}
				set
				{
					if (myEx == null && value != default(PointF))
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myStartOrFocusScales = value;
					}
				}
			}

			public PointF Point
			{
				get
				{
					if (myEx == null)
					{
						return default(PointF);
					}
					return myEx.myPoint;
				}
				set
				{
					if (myEx == null && value != default(PointF))
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myPoint = value;
					}
				}
			}

			public float MidBlendFactor
			{
				get
				{
					if (myEx == null)
					{
						return float.NaN;
					}
					return myEx.myMidBlendFactor;
				}
				set
				{
					if (myEx == null && value != float.NaN)
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myMidBlendFactor = value;
					}
				}
			}

			public float MidFraction
			{
				get
				{
					if (myEx == null)
					{
						return float.NaN;
					}
					return myEx.myMidPosition;
				}
				set
				{
					if (myEx == null && value != float.NaN)
					{
						myEx = new GoBrushInfoEx();
					}
					if (myEx != null)
					{
						myEx.myMidPosition = value;
					}
				}
			}

			public GoBrushInfo()
			{
			}

			public GoBrushInfo(Brush b)
			{
				SetBrush(b, null);
			}

			public GoBrushInfo(GoBrushInfo other)
			{
				if (other != null)
				{
					myColor = other.myColor;
					myEx = ((other.myEx != null) ? new GoBrushInfoEx(other.myEx) : null);
				}
			}

			public override bool Equals(object obj)
			{
				GoBrushInfo goBrushInfo = obj as GoBrushInfo;
				if (goBrushInfo == null)
				{
					return false;
				}
				if (myColor == goBrushInfo.myColor)
				{
					if (myEx == null)
					{
						return goBrushInfo.myEx == null;
					}
					return myEx.Equals(goBrushInfo.myEx);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return myColor.GetHashCode() ^ ((myEx != null) ? myEx.GetHashCode() : 0);
			}

			public void SetBrush(Brush b, object shapeorpath)
			{
				myBrush = b;
				if (b is SolidBrush)
				{
					SolidBrush solidBrush = (SolidBrush)b;
					myColor = solidBrush.Color;
					myEx = null;
				}
				else if (b is HatchBrush)
				{
					HatchBrush hatchBrush = (HatchBrush)b;
					myColor = hatchBrush.BackgroundColor;
					if (myEx == null)
					{
						myEx = new GoBrushInfoEx();
					}
					myEx.myBrushType = GoBrushType.Hatch;
					myEx.myBrushStyle = ToBrushStyle(hatchBrush.HatchStyle);
					myEx.myForeColor = hatchBrush.ForegroundColor;
				}
				else if (b is TextureBrush)
				{
					TextureBrush textureBrush = (TextureBrush)b;
					if (myEx == null)
					{
						myEx = new GoBrushInfoEx();
					}
					myEx.myBrushType = GoBrushType.Texture;
					myEx.myBrushStyle = GoBrushStyle.Texture;
					myEx.myImage = textureBrush.Image;
					myEx.myWrapMode = textureBrush.WrapMode;
					float[] elements = textureBrush.Transform.Elements;
					myEx.myStartOrFocusScales = new PointF(elements[0], elements[1]);
					myEx.myPoint = new PointF(elements[2], elements[3]);
				}
				else if (b is LinearGradientBrush)
				{
					LinearGradientBrush linearGradientBrush = (LinearGradientBrush)b;
					if (myEx == null)
					{
						myEx = new GoBrushInfoEx();
					}
					myEx.myBrushType = GoBrushType.LinearGradient;
					myEx.myBrushStyle = GoBrushStyle.CustomLinearGradient;
					bool flag = false;
					Blend blend = null;
					try
					{
						blend = linearGradientBrush.Blend;
						if (blend != null && blend.Positions.Length > 3)
						{
							flag = ((byte)((flag ? 1 : 0) | 1) != 0);
						}
					}
					catch (Exception)
					{
						blend = null;
					}
					ColorBlend colorBlend = null;
					try
					{
						colorBlend = linearGradientBrush.InterpolationColors;
						if (colorBlend != null && colorBlend.Positions.Length > 3)
						{
							flag = ((byte)((flag ? 1 : 0) | 1) != 0);
						}
					}
					catch (Exception)
					{
						colorBlend = null;
					}
					if (linearGradientBrush.GammaCorrection)
					{
						myEx.myBrushType = GoBrushType.LinearGradientGamma;
					}
					else
					{
						myEx.myBrushType = GoBrushType.LinearGradient;
					}
					if (!flag)
					{
						myBrush = null;
					}
					myColor = linearGradientBrush.LinearColors[1];
					myEx.myForeColor = linearGradientBrush.LinearColors[0];
					RectangleF rectangleF = default(RectangleF);
					GoShape goShape = shapeorpath as GoShape;
					if (goShape != null)
					{
						rectangleF = goShape.Bounds;
					}
					else
					{
						GraphicsPath graphicsPath = shapeorpath as GraphicsPath;
						if (graphicsPath != null)
						{
							rectangleF = graphicsPath.GetBounds();
						}
					}
					Matrix transform = linearGradientBrush.Transform;
					RectangleF rectangle = linearGradientBrush.Rectangle;
					PointF[] array = new PointF[2]
					{
						new PointF(rectangle.X, rectangle.Y),
						new PointF(rectangle.X + rectangle.Width, rectangle.Y)
					};
					transform.TransformPoints(array);
					PointF pointF = array[0];
					PointF pointF2 = array[1];
					if (Math.Abs(pointF.X - pointF2.X) < 0.001f)
					{
						pointF.X = rectangleF.Width / 4f;
						pointF2.X = pointF.X;
					}
					if (Math.Abs(pointF.Y - pointF2.Y) < 0.001f)
					{
						pointF.Y = rectangleF.Height / 4f;
						pointF2.Y = pointF.Y;
					}
					if (Math.Abs(pointF.X - pointF2.X) > 0.001f && Math.Abs(pointF.Y - pointF2.Y) > 0.001f)
					{
						float num = pointF.X - (rectangle.X + rectangle.Width / 2f);
						float num2 = pointF.Y - (rectangle.Y + rectangle.Height / 2f);
						float num3 = pointF2.X - (rectangle.X + rectangle.Width / 2f);
						float num4 = pointF2.Y - (rectangle.Y + rectangle.Height / 2f);
						if (num != num3 && num2 != num4)
						{
							float num5 = (num4 - num2) / (num3 - num);
							float num6 = num5 + 1f / num5;
							float num7 = (num / num5 + num2) / num6;
							float num8 = num5 * num7;
							float num9 = (num3 / num5 + num4) / num6;
							float num10 = num5 * num9;
							pointF.X = num7 + (rectangle.X + rectangle.Width / 2f);
							pointF.Y = num8 + (rectangle.Y + rectangle.Height / 2f);
							pointF2.X = num9 + (rectangle.X + rectangle.Width / 2f);
							pointF2.Y = num10 + (rectangle.Y + rectangle.Height / 2f);
						}
					}
					if (rectangleF.Width > 0.001f)
					{
						myEx.myStartOrFocusScales.X = pointF.X / rectangleF.Width;
						myEx.myPoint.X = pointF2.X / rectangleF.Width;
					}
					else
					{
						myEx.myStartOrFocusScales.X = 0f;
						myEx.myPoint.X = 0f;
					}
					if (rectangleF.Height > 0.001f)
					{
						myEx.myStartOrFocusScales.Y = pointF.Y / rectangleF.Height;
						myEx.myPoint.Y = pointF2.Y / rectangleF.Height;
					}
					else
					{
						myEx.myStartOrFocusScales.Y = 0f;
						myEx.myPoint.Y = 0f;
					}
					if (myEx.myStartOrFocusScales == myEx.myPoint)
					{
						myEx.myStartOrFocusScales = new PointF(0f, 0f);
						myEx.myPoint = new PointF(0f, 1f);
					}
					myEx.myWrapMode = linearGradientBrush.WrapMode;
					myEx.myMidPosition = float.NaN;
					myEx.myMidBlendFactor = float.NaN;
					if (colorBlend != null && colorBlend.Colors.Length > 1)
					{
						myEx.myMidColor = colorBlend.Colors[colorBlend.Colors.Length / 2];
						myEx.myMidPosition = colorBlend.Positions[colorBlend.Positions.Length / 2];
					}
					else if (blend != null && blend.Factors.Length > 1)
					{
						myEx.myMidBlendFactor = blend.Factors[blend.Factors.Length / 2];
						myEx.myMidPosition = blend.Positions[blend.Positions.Length / 2];
					}
					if (flag)
					{
						return;
					}
					bool flag2 = myEx.myPoint.X - myEx.myStartOrFocusScales.X > 0.1f;
					bool flag3 = Math.Abs(myEx.myPoint.X - myEx.myStartOrFocusScales.X) <= 0.1f;
					bool flag4 = myEx.myPoint.X - myEx.myStartOrFocusScales.X < -0.1f;
					bool flag5 = myEx.myPoint.Y - myEx.myStartOrFocusScales.Y > 0.1f;
					bool flag6 = Math.Abs(myEx.myPoint.Y - myEx.myStartOrFocusScales.Y) <= 0.1f;
					bool flag7 = myEx.myPoint.Y - myEx.myStartOrFocusScales.Y < -0.1f;
					if (flag3 && flag5)
					{
						if (myEx.myMidColor != Color.Empty)
						{
							myEx.myBrushStyle = GoBrushStyle.MiddleGradientVertical;
						}
						else if (!float.IsNaN(myEx.myMidPosition))
						{
							if (Math.Abs(myEx.myPoint.X - 0.5f) < 0.1f)
							{
								myEx.myBrushStyle = GoBrushStyle.DoubleEdgeGradientVertical;
							}
							else
							{
								myEx.myBrushStyle = GoBrushStyle.SingleEdgeGradientTop;
							}
						}
						else
						{
							myEx.myBrushStyle = GoBrushStyle.SimpleGradientVertical;
						}
					}
					else if (flag2 && flag6)
					{
						if (myEx.myMidColor != Color.Empty)
						{
							myEx.myBrushStyle = GoBrushStyle.MiddleGradientHorizontal;
						}
						else if (!float.IsNaN(myEx.myMidPosition))
						{
							if (Math.Abs(myEx.myPoint.X - 0.5f) < 0.1f)
							{
								myEx.myBrushStyle = GoBrushStyle.DoubleEdgeGradientHorizontal;
							}
							else
							{
								myEx.myBrushStyle = GoBrushStyle.SingleEdgeGradientLeft;
							}
						}
						else
						{
							myEx.myBrushStyle = GoBrushStyle.SimpleGradientHorizontal;
						}
					}
					else if (flag2 && flag5)
					{
						if (myEx.myMidColor != Color.Empty)
						{
							myEx.myBrushStyle = GoBrushStyle.MiddleGradientForwardDiagonal;
						}
						else if (!float.IsNaN(myEx.myMidPosition))
						{
							if (Math.Abs(myEx.myPoint.X - 0.5f) < 0.1f && Math.Abs(myEx.myPoint.Y - 0.5f) < 0.1f)
							{
								myEx.myBrushStyle = GoBrushStyle.DoubleEdgeGradientForwardDiagonal;
							}
						}
						else
						{
							myEx.myBrushStyle = GoBrushStyle.SimpleGradientForwardDiagonal;
						}
					}
					else if (flag4 && flag5)
					{
						if (myEx.myMidColor != Color.Empty)
						{
							myEx.myBrushStyle = GoBrushStyle.MiddleGradientBackwardDiagonal;
						}
						else if (!float.IsNaN(myEx.myMidPosition))
						{
							if (Math.Abs(myEx.myPoint.X - 0.5f) < 0.1f && Math.Abs(myEx.myPoint.Y - 0.5f) < 0.1f)
							{
								myEx.myBrushStyle = GoBrushStyle.DoubleEdgeGradientBackwardDiagonal;
							}
						}
						else
						{
							myEx.myBrushStyle = GoBrushStyle.SimpleGradientBackwardDiagonal;
						}
					}
					else if (flag4 && flag6)
					{
						if (!float.IsNaN(myEx.myMidPosition))
						{
							myEx.myBrushStyle = GoBrushStyle.SingleEdgeGradientRight;
						}
					}
					else if (flag3 && flag7 && !float.IsNaN(myEx.myMidPosition))
					{
						myEx.myBrushStyle = GoBrushStyle.SingleEdgeGradientBottom;
					}
				}
				else if (b is PathGradientBrush)
				{
					PathGradientBrush pathGradientBrush = (PathGradientBrush)b;
					if (myEx == null)
					{
						myEx = new GoBrushInfoEx();
					}
					myEx.myBrushType = GoBrushType.PathGradient;
					myEx.myBrushStyle = GoBrushStyle.CustomPathGradient;
					bool flag8 = false;
					Blend blend2 = null;
					try
					{
						blend2 = pathGradientBrush.Blend;
						if (blend2 != null && blend2.Positions.Length > 3)
						{
							flag8 = ((byte)((flag8 ? 1 : 0) | 1) != 0);
						}
					}
					catch (Exception)
					{
						blend2 = null;
					}
					ColorBlend colorBlend2 = null;
					try
					{
						colorBlend2 = pathGradientBrush.InterpolationColors;
						if (colorBlend2 != null && colorBlend2.Positions.Length > 3)
						{
							flag8 = ((byte)((flag8 ? 1 : 0) | 1) != 0);
						}
					}
					catch (Exception)
					{
						colorBlend2 = null;
					}
					Color[] array2 = null;
					try
					{
						array2 = pathGradientBrush.SurroundColors;
						if (array2 != null && array2.Length > 1)
						{
							flag8 = ((byte)((flag8 ? 1 : 0) | 1) != 0);
						}
					}
					catch (Exception)
					{
						array2 = null;
					}
					if (!flag8)
					{
						myBrush = null;
					}
					try
					{
						myColor = pathGradientBrush.CenterColor;
					}
					catch (Exception)
					{
						myColor = Color.Empty;
					}
					if (array2 != null)
					{
						myEx.myForeColor = array2[0];
					}
					else
					{
						myEx.myForeColor = Color.Empty;
					}
					RectangleF rectangle2 = pathGradientBrush.Rectangle;
					PointF pointF3;
					try
					{
						pointF3 = pathGradientBrush.CenterPoint;
					}
					catch (Exception)
					{
						pointF3 = new PointF(rectangle2.X + rectangle2.Width / 2f, rectangle2.Y + rectangle2.Height / 2f);
					}
					myEx.myPoint = new PointF((rectangle2.Width == 0f) ? 0.5f : ((pointF3.X - rectangle2.X) / rectangle2.Width), (rectangle2.Height == 0f) ? 0.5f : ((pointF3.Y - rectangle2.Y) / rectangle2.Height));
					myEx.myWrapMode = pathGradientBrush.WrapMode;
					myEx.myMidPosition = float.NaN;
					myEx.myMidBlendFactor = float.NaN;
					if (colorBlend2 != null && colorBlend2.Colors.Length > 1)
					{
						myEx.myMidColor = colorBlend2.Colors[colorBlend2.Colors.Length / 2];
						myEx.myMidPosition = colorBlend2.Positions[colorBlend2.Positions.Length / 2];
					}
					else if (blend2 != null && blend2.Factors.Length > 1)
					{
						myEx.myMidBlendFactor = blend2.Factors[blend2.Factors.Length / 2];
						myEx.myMidPosition = blend2.Positions[blend2.Positions.Length / 2];
					}
					myEx.myStartOrFocusScales = pathGradientBrush.FocusScales;
					if (!flag8)
					{
						switch (myEx.myWrapMode)
						{
						case WrapMode.Tile:
						case WrapMode.Clamp:
							myEx.myBrushStyle = GoBrushStyle.ShapeFringeGradient;
							break;
						case WrapMode.TileFlipXY:
							myEx.myBrushStyle = GoBrushStyle.ShapeHighlightGradient;
							break;
						case WrapMode.TileFlipX:
							myEx.myBrushStyle = GoBrushStyle.RectangleGradient;
							break;
						case WrapMode.TileFlipY:
							myEx.myBrushStyle = GoBrushStyle.EllipseGradient;
							break;
						}
					}
				}
				else
				{
					if (myEx == null)
					{
						myEx = new GoBrushInfoEx();
					}
					myEx.myBrushType = GoBrushType.None;
					myEx.myBrushStyle = GoBrushStyle.None;
				}
			}

			public Brush GetBrush(object shapeorpath)
			{
				if (myBrush == null)
				{
					if (myEx == null || myEx.myBrushType == GoBrushType.Solid)
					{
						myBrush = new SolidBrush(myColor);
					}
					else
					{
						if (myEx.myBrushType == GoBrushType.None)
						{
							return null;
						}
						if (myEx.myBrushType == GoBrushType.Hatch)
						{
							myBrush = new HatchBrush(ToHatchStyle(myEx.myBrushStyle), myEx.myForeColor, myColor);
						}
						else if (myEx.myBrushType == GoBrushType.Texture)
						{
							if (myEx.myImage == null)
							{
								myBrush = new SolidBrush(myColor);
							}
							else
							{
								TextureBrush textureBrush = new TextureBrush(myEx.myImage, myEx.myWrapMode);
								Matrix matrix2 = textureBrush.Transform = new Matrix(myEx.myStartOrFocusScales.X, myEx.myStartOrFocusScales.Y, myEx.myPoint.X, myEx.myPoint.Y, 0f, 0f);
								myBrush = textureBrush;
							}
						}
						else
						{
							if (myEx.myBrushType == GoBrushType.LinearGradient || myEx.myBrushType == GoBrushType.LinearGradientGamma)
							{
								RectangleF rectangleF = new RectangleF(0f, 0f, 100f, 100f);
								GoShape goShape = shapeorpath as GoShape;
								if (goShape != null)
								{
									rectangleF = goShape.Bounds;
								}
								else
								{
									GraphicsPath graphicsPath = shapeorpath as GraphicsPath;
									if (graphicsPath != null)
									{
										rectangleF = graphicsPath.GetBounds();
									}
								}
								if ((rectangleF.Width < 0.001f && rectangleF.Height < 0.001f) || rectangleF.Width > 9999999f || rectangleF.Height > 9999999f)
								{
									myBrush = new SolidBrush(myColor);
									return myBrush;
								}
								PointF pointF = new PointF(myEx.myStartOrFocusScales.X * rectangleF.Width, myEx.myStartOrFocusScales.Y * rectangleF.Height);
								PointF pointF2 = new PointF(myEx.myPoint.X * rectangleF.Width, myEx.myPoint.Y * rectangleF.Height);
								if (pointF == pointF2)
								{
									pointF = new PointF(0f, 0f);
									pointF2 = new PointF(0f, rectangleF.Height);
								}
								LinearGradientBrush linearGradientBrush = new LinearGradientBrush(pointF, pointF2, myEx.myForeColor, myColor);
								linearGradientBrush.WrapMode = myEx.myWrapMode;
								if (myEx.myMidColor != Color.Empty && !float.IsNaN(myEx.myMidPosition))
								{
									ColorBlend colorBlend = new ColorBlend();
									colorBlend.Colors = new Color[3]
									{
										myEx.myForeColor,
										myEx.myMidColor,
										myColor
									};
									colorBlend.Positions = new float[3]
									{
										0f,
										myEx.myMidPosition,
										1f
									};
									linearGradientBrush.InterpolationColors = colorBlend;
								}
								else if (!float.IsNaN(myEx.myMidBlendFactor) && !float.IsNaN(myEx.myMidPosition))
								{
									Blend blend = new Blend();
									blend.Factors = new float[3]
									{
										0f,
										myEx.myMidBlendFactor,
										1f
									};
									blend.Positions = new float[3]
									{
										0f,
										myEx.myMidPosition,
										1f
									};
									linearGradientBrush.Blend = blend;
								}
								linearGradientBrush.GammaCorrection = (myEx.myBrushType == GoBrushType.LinearGradientGamma);
								return linearGradientBrush;
							}
							if (myEx.myBrushType == GoBrushType.PathGradient)
							{
								GraphicsPath graphicsPath2 = shapeorpath as GraphicsPath;
								GoShape goShape2 = shapeorpath as GoShape;
								if (graphicsPath2 == null && goShape2 != null)
								{
									graphicsPath2 = goShape2.GetPath();
								}
								if (graphicsPath2 != null)
								{
									RectangleF bounds = graphicsPath2.GetBounds();
									if (bounds.Width < 0.001f || bounds.Height < 0.001f || bounds.Width > 9999999f || bounds.Height > 9999999f)
									{
										myBrush = new SolidBrush(myColor);
										return myBrush;
									}
									PathGradientBrush pathGradientBrush;
									if (myEx.myWrapMode == WrapMode.TileFlipX)
									{
										if (goShape2 != null)
										{
											bounds = goShape2.Bounds;
										}
										pathGradientBrush = new PathGradientBrush(new PointF[4]
										{
											new PointF(bounds.X, bounds.Y),
											new PointF(bounds.Right, bounds.Y),
											new PointF(bounds.Right, bounds.Bottom),
											new PointF(bounds.X, bounds.Bottom)
										});
									}
									else if (myEx.myWrapMode == WrapMode.TileFlipY)
									{
										if (goShape2 != null)
										{
											bounds = goShape2.Bounds;
										}
										float num = bounds.Width / 2f;
										float num2 = bounds.Height / 2f;
										if (num > 0.1f && num2 > 0.1f)
										{
											double num3 = Math.Atan2(num2, num);
											float num4 = (float)Math.Sin(num3);
											float num5 = (float)Math.Cos(num3);
											bounds.Inflate(num5 * num, num4 * num2);
										}
										GraphicsPath graphicsPath3 = new GraphicsPath();
										graphicsPath3.AddEllipse(bounds);
										pathGradientBrush = new PathGradientBrush(graphicsPath3);
									}
									else
									{
										pathGradientBrush = new PathGradientBrush(graphicsPath2);
									}
									if (myEx.myWrapMode != WrapMode.Clamp)
									{
										PointF pointF4 = pathGradientBrush.CenterPoint = new PointF(bounds.X + myEx.myPoint.X * bounds.Width, bounds.Y + myEx.myPoint.Y * bounds.Height);
									}
									pathGradientBrush.WrapMode = myEx.myWrapMode;
									if (myEx.myMidColor != Color.Empty && !float.IsNaN(myEx.myMidPosition))
									{
										ColorBlend colorBlend2 = new ColorBlend();
										colorBlend2.Colors = new Color[3]
										{
											myEx.myForeColor,
											myEx.myMidColor,
											myColor
										};
										colorBlend2.Positions = new float[3]
										{
											0f,
											myEx.myMidPosition,
											1f
										};
										pathGradientBrush.InterpolationColors = colorBlend2;
									}
									else
									{
										if (!float.IsNaN(myEx.myMidBlendFactor) && !float.IsNaN(myEx.myMidPosition))
										{
											Blend blend2 = new Blend();
											blend2.Factors = new float[3]
											{
												0f,
												myEx.myMidBlendFactor,
												1f
											};
											blend2.Positions = new float[3]
											{
												0f,
												myEx.myMidPosition,
												1f
											};
											pathGradientBrush.Blend = blend2;
										}
										pathGradientBrush.CenterColor = myColor;
										pathGradientBrush.SurroundColors = new Color[1]
										{
											myEx.myForeColor
										};
									}
									pathGradientBrush.FocusScales = myEx.myStartOrFocusScales;
									return pathGradientBrush;
								}
							}
						}
					}
				}
				return myBrush;
			}
		}

		[Serializable]
		internal sealed class GoBrushInfoEx
		{
			internal GoBrushType myBrushType = GoBrushType.Solid;

			internal GoBrushStyle myBrushStyle = GoBrushStyle.Solid;

			internal Color myForeColor;

			internal Image myImage;

			internal WrapMode myWrapMode;

			internal PointF myStartOrFocusScales;

			internal PointF myPoint;

			internal float myMidBlendFactor = float.NaN;

			internal float myMidPosition = float.NaN;

			internal Color myMidColor;

			public GoBrushInfoEx()
			{
			}

			public GoBrushInfoEx(GoBrushInfoEx other)
			{
				myBrushType = other.myBrushType;
				myBrushStyle = other.myBrushStyle;
				myForeColor = other.myForeColor;
				myImage = other.myImage;
				myWrapMode = other.myWrapMode;
				myStartOrFocusScales = other.myStartOrFocusScales;
				myPoint = other.myPoint;
				myMidBlendFactor = other.myMidBlendFactor;
				myMidPosition = other.myMidPosition;
				myMidColor = other.myMidColor;
			}

			public override bool Equals(object obj)
			{
				GoBrushInfoEx goBrushInfoEx = obj as GoBrushInfoEx;
				if (goBrushInfoEx == null)
				{
					return false;
				}
				if (myBrushType == goBrushInfoEx.myBrushType && myBrushStyle == goBrushInfoEx.myBrushStyle && myForeColor == goBrushInfoEx.myForeColor && myImage == goBrushInfoEx.myImage && myWrapMode == goBrushInfoEx.myWrapMode && myStartOrFocusScales == goBrushInfoEx.myStartOrFocusScales && myPoint == goBrushInfoEx.myPoint && myMidBlendFactor == goBrushInfoEx.myMidBlendFactor && myMidPosition == goBrushInfoEx.myMidPosition)
				{
					return myMidColor == goBrushInfoEx.myMidColor;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (int)myBrushType ^ (int)myBrushStyle ^ myForeColor.GetHashCode() ^ ((myImage != null) ? myImage.GetHashCode() : 0) ^ (int)myWrapMode ^ myStartOrFocusScales.GetHashCode() ^ myPoint.GetHashCode() ^ myMidBlendFactor.GetHashCode() ^ myMidPosition.GetHashCode() ^ myMidColor.GetHashCode();
			}
		}

		internal static GoPenInfo PenInfo_Black;

		internal static GoBrushInfo BrushInfo_Black;

		internal static GoBrushInfo BrushInfo_Gray;

		internal static GoBrushInfo BrushInfo_LightGray;

		internal static GoBrushInfo BrushInfo_White;

		internal static readonly Pen Pens_Black = Pens.Black;

		internal static readonly Pen Pens_Gray = Pens.Gray;

		internal static readonly Pen Pens_LightGray = Pens.LightGray;

		internal static readonly Pen SystemPens_Control = SystemPens.Control;

		internal static readonly Pen SystemPens_ControlDarkDark = SystemPens.ControlDarkDark;

		internal static readonly Pen SystemPens_ControlDark = SystemPens.ControlDark;

		internal static readonly Pen SystemPens_ControlLightLight = SystemPens.ControlLightLight;

		internal static readonly Pen SystemPens_WindowFrame = SystemPens.WindowFrame;

		internal static readonly Brush Brushes_Black = Brushes.Black;

		internal static readonly Brush Brushes_Gray = Brushes.Gray;

		internal static readonly Brush Brushes_LightGray = Brushes.LightGray;

		internal static readonly Brush Brushes_White = Brushes.White;

		internal static readonly Brush Brushes_Yellow = Brushes.Yellow;

		internal static readonly Brush Brushes_LemonChiffon = Brushes.LemonChiffon;

		internal static readonly Brush Brushes_Gold = Brushes.Gold;

		internal static readonly Brush SystemBrushes_Control = SystemBrushes.Control;

		internal const int HATCH_START = 256;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoShape.Pen" /> property.
		/// </summary>
		public const int ChangedPen = 1101;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoShape.Brush" /> property.
		/// </summary>
		public const int ChangedBrush = 1102;

		private GoPenInfo myPenInfo = GetPenInfo(Pens_Black);

		private GoBrushInfo myBrushInfo;

		[NonSerialized]
		internal GraphicsPath myPath;

		[NonSerialized]
		internal Brush myBrush;

		/// <summary>
		/// Whenever the size and/or position is changed, we clear out any cached data.
		/// </summary>
		public override RectangleF Bounds
		{
			get
			{
				return base.Bounds;
			}
			set
			{
				ResetPath();
				base.Bounds = value;
			}
		}

		/// <summary>
		/// Gets or sets the pen used to draw the outline of this shape.
		/// </summary>
		/// <value>
		/// The <c>Pen</c> value may be null, in which case no outline is drawn.
		/// This value defaults to <c>Pens.Black</c>.
		/// </value>
		/// <remarks>
		/// You must not modify the pen after you have assigned it.
		/// It is common to use the predefined brushes that are members of the
		/// <c>Pens</c> class.
		/// Currently serialization is limited to standard pens.
		/// </remarks>
		[Category("Appearance")]
		[Description("The pen used to draw the outline of this shape.")]
		public virtual Pen Pen
		{
			get
			{
				if (myPenInfo != null)
				{
					return myPenInfo.GetPen();
				}
				return null;
			}
			set
			{
				PenInfo = GetPenInfo(value);
			}
		}

		internal GoPenInfo PenInfo
		{
			get
			{
				return myPenInfo;
			}
			set
			{
				GoPenInfo goPenInfo = myPenInfo;
				if (goPenInfo != value && (goPenInfo == null || !goPenInfo.Equals(value)))
				{
					InvalidateViews();
					myPenInfo = value;
					Changed(1101, 0, goPenInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (base.Parent != null)
					{
						base.Parent.InvalidatePaintBounds();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used to fill the outline of this shape.
		/// </summary>
		/// <value>
		/// The <c>Brush</c> value may be null, in which case no background is painted.
		/// This value defaults to null.
		/// </value>
		/// <remarks>
		/// <para>
		/// LinearGradientBrushes and TextureBrushes are drawn with their origin at the Position (top-left corner) of this shape.
		/// </para>
		/// <para>
		/// You must not modify the brush after you have assigned it.
		/// It is common to use the predefined brushes that are members of the
		/// <c>Brushes</c> class.
		/// For the simplest linear gradient and path gradient effects,
		/// you can just set the <see cref="P:Northwoods.Go.GoShape.BrushColor" />, <see cref="P:Northwoods.Go.GoShape.BrushForeColor" />,
		/// <see cref="P:Northwoods.Go.GoShape.BrushMidColor" />, and <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> properties.
		/// For common linear gradient and path gradient effects,
		/// more complicated than you get by setting those <b>Brush...</b> properties,
		/// we suggest that you use one of the <b>Fill...</b> methods.
		/// Finally, for the most sophisticated or complex kinds of gradient brushes,
		/// you may need to override this property to return the kind of value you need.
		/// </para>
		/// <para>
		/// When a linear gradient brush or a path gradient brush is drawn very small,
		/// due to a combination of small size and small <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush or not draw anything at all,
		/// both for efficiency as well as to avoid GDI+ errors.
		/// When a path gradient brush is drawn very large,
		/// due to a combination of large size and large <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush, both for efficiency as well as to avoid GDI+ errors.
		/// </para>
		/// <para>
		/// Currently serialization is limited to standard solid, hatch,
		/// and texture brushes and many kinds of simple linear gradient and
		/// path gradient brushes.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[Description("The brush used to fill the outline of this shape.")]
		public virtual Brush Brush
		{
			get
			{
				if (myBrushInfo != null)
				{
					if (myBrush == null)
					{
						myBrush = myBrushInfo.GetBrush(this);
					}
					return myBrush;
				}
				return null;
			}
			set
			{
				BrushInfo = GetBrushInfo(value, this);
			}
		}

		internal GoBrushInfo BrushInfo
		{
			get
			{
				return myBrushInfo;
			}
			set
			{
				GoBrushInfo goBrushInfo = myBrushInfo;
				if (goBrushInfo != value && (goBrushInfo == null || !goBrushInfo.Equals(value)))
				{
					myBrushInfo = value;
					myBrush = null;
					Changed(1102, 0, goBrushInfo, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the pen.
		/// </summary>
		/// <value>
		/// The width of the <see cref="P:Northwoods.Go.GoShape.Pen" />, or 0 if there is no pen.
		/// The default value is zero.
		/// </value>
		/// <remarks>
		/// If there is no <see cref="P:Northwoods.Go.GoShape.Pen" />, setting this property might have no effect.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The width of the pen used to draw the outline of the shape.")]
		public virtual float PenWidth
		{
			get
			{
				return PenInfo?.Width ?? 0f;
			}
			set
			{
				GoPenInfo goPenInfo = PenInfo;
				float num = 0f;
				if (goPenInfo != null)
				{
					num = goPenInfo.Width;
				}
				else
				{
					goPenInfo = PenInfo_Black;
				}
				if (num != value)
				{
					GoPenInfo goPenInfo2 = new GoPenInfo(goPenInfo);
					goPenInfo2.Width = value;
					PenInfo = goPenInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the pen.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoShape.Pen" />, or <b>Color.Empty</b> if there is no pen.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoShape.Pen" /> to null.
		/// </value>
		[Category("Appearance")]
		[Description("The color of the pen used to draw the outline of the shape")]
		public virtual Color PenColor
		{
			get
			{
				return PenInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoPenInfo penInfo = PenInfo;
				GoPenInfo goPenInfo = null;
				if (penInfo != null)
				{
					if (penInfo.Color == value)
					{
						return;
					}
					if (value != Color.Empty)
					{
						goPenInfo = new GoPenInfo(penInfo);
						goPenInfo.Color = value;
					}
				}
				else if (value != Color.Empty)
				{
					goPenInfo = new GoPenInfo();
					goPenInfo.Width = PenWidth;
					goPenInfo.Color = value;
				}
				PenInfo = goPenInfo;
			}
		}

		/// <summary>
		/// Gets or sets the main or background color of the brush.
		/// </summary>
		/// <value>
		/// The <b>Color</b> of the <see cref="P:Northwoods.Go.GoShape.Brush" />, or <b>Color.Empty</b> if there is no brush.
		/// Setting the new value to a non-empty <b>Color</b> when <see cref="P:Northwoods.Go.GoShape.Brush" /> is null
		/// will set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to a <b>SolidBrush</b> of that new color.
		/// Setting the new value to <b>Color.Empty</b> will set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to null if
		/// the old brush was a <b>SolidBrush</b>.
		/// </value>
		/// <remarks>
		/// This refers to the color of a <b>SolidBrush</b>, the background color of
		/// a <b>HatchBrush</b>, the ending color of a <b>LinearGradientBrush</b>, or the
		/// center color of a <b>PathGradientBrush</b>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The background color of a solid, hatch, or gradient brush")]
		public virtual Color BrushColor
		{
			get
			{
				return BrushInfo?.Color ?? Color.Empty;
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.Color != value)
				{
					if (value == Color.Empty && brushInfo.BrushType == GoBrushType.Solid)
					{
						BrushInfo = null;
						return;
					}
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.Color = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.Color = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the foreground color of a brush.
		/// </summary>
		/// <value>
		/// Setting this value will have no effect for brushes that are not
		/// <b>HatchBrush</b>, <b>LinearGradientBrush</b>, or <b>PathGradientBrush</b>.
		/// </value>
		/// <remarks>
		/// This refers to the foreground color of a <b>HatchBrush</b>,
		/// the starting color of a <b>LinearGradientBrush</b>, or the
		/// surrounding color of a <b>PathGradientBrush</b>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The foreground color of a hatch or gradient brush")]
		public virtual Color BrushForeColor
		{
			get
			{
				return BrushInfo?.ForeColor ?? Color.Empty;
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.ForeColor != value)
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.ForeColor = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.ForeColor = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the middle color of a gradient brush.
		/// </summary>
		/// <value>
		/// For a <b>LinearGradientBrush</b> or <b>PathGradientBrush</b>, the value may be <b>Color.Empty</b>
		/// if no middle color is needed.
		/// This defaults to <b>Color.Empty</b>.
		/// Setting this value will have no effect for other kinds of brushes.
		/// </value>
		[Category("Appearance")]
		[Description("The middle color of a linear gradient brush")]
		public virtual Color BrushMidColor
		{
			get
			{
				return BrushInfo?.MidColor ?? Color.Empty;
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null && brushInfo.MidColor != value)
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.MidColor = value;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && value != Color.Empty)
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.MidColor = value;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the fractional distance between the end colors at which the middle color is drawn.
		/// </summary>
		/// <remarks>
		/// This property is ignored if the value is <b>Single.NaN</b>.
		/// The meaning of this property depends on the value <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> property;
		/// many brush styles ignore this property.
		/// </remarks>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> is a linear gradient or a path gradient
		/// that supports three colors.
		/// Changing the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[Description("The fraction (0-1) of the distance between the end colors at which the middle color is drawn")]
		public virtual float BrushMidFraction
		{
			get
			{
				return BrushInfo?.MidFraction ?? float.NaN;
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				float num = value;
				if (!float.IsNaN(num))
				{
					if (num < 0f)
					{
						num = 0f;
					}
					else if (num > 1f)
					{
						num = 1f;
					}
				}
				if (brushInfo != null && brushInfo.MidFraction != num)
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.MidFraction = num;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && num != float.NaN)
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.MidFraction = num;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the start point for linear gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>PointF</b> whose X and Y values are normally between 0 and 1, and should be between -1 and 2.
		/// An X value of zero corresponds to the left side of the shape; an X value of 1 corresponds to the right side.
		/// A Y value of zero corresponds to the top side of the shape; a Y value of 1 corresponds to the bottom side.
		/// Negative values or values greater than 1 denote points that are outside of the shape.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> is a linear gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The normalized/fractional start point for linear gradients, typically each value from around 0 to 1")]
		public virtual PointF BrushStartPoint
		{
			get
			{
				return BrushInfo?.StartOrFocusScales ?? default(PointF);
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				PointF pointF = value;
				if (pointF.X < -1f)
				{
					pointF.X = -1f;
				}
				else if (pointF.X > 2f)
				{
					pointF.X = 2f;
				}
				if (pointF.Y < -1f)
				{
					pointF.Y = -1f;
				}
				else if (pointF.Y > 2f)
				{
					pointF.Y = 2f;
				}
				if (brushInfo != null && brushInfo.Point != pointF)
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.StartOrFocusScales = pointF;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && pointF != default(PointF))
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.StartOrFocusScales = pointF;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the end point for linear gradients or the focus point for path gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>PointF</b> whose X and Y values are normally between 0 and 1, and should be between -1 and 2.
		/// An X value of zero corresponds to the left side of the shape; an X value of 1 corresponds to the right side.
		/// A Y value of zero corresponds to the top side of the shape; a Y value of 1 corresponds to the bottom side.
		/// Negative values or values greater than 1 denote points that are outside of the shape.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> is a linear gradient or a path gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoPointFConverter))]
		[Description("The normalized/fractional end point for linear gradients or focus point for path gradients, typically value each around from 0 to 1")]
		public virtual PointF BrushPoint
		{
			get
			{
				return BrushInfo?.Point ?? default(PointF);
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				PointF pointF = value;
				if (pointF.X < -1f)
				{
					pointF.X = -1f;
				}
				else if (pointF.X > 2f)
				{
					pointF.X = 2f;
				}
				if (pointF.Y < -1f)
				{
					pointF.Y = -1f;
				}
				else if (pointF.Y > 2f)
				{
					pointF.Y = 2f;
				}
				if (brushInfo != null && brushInfo.Point != pointF)
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.Point = pointF;
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && pointF != default(PointF))
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.Point = pointF;
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of the focus area for path gradients,
		/// as normalized fractions of the size of the shape.
		/// </summary>
		/// <value>
		/// A <b>SizeF</b> whose Width and Height values must be between 0 and 1.
		/// </value>
		/// <remarks>
		/// This property is not meaningful unless the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> is a path gradient.
		/// Changing the <see cref="P:Northwoods.Go.GoShape.BrushStyle" /> may also change the value of this property.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The fractional size of the focus area of a path gradient, each value from 0 to 1")]
		public virtual SizeF BrushFocusScales
		{
			get
			{
				GoBrushInfo brushInfo = BrushInfo;
				if (brushInfo != null)
				{
					return new SizeF(brushInfo.StartOrFocusScales.X, brushInfo.StartOrFocusScales.Y);
				}
				return default(SizeF);
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				SizeF sz = value;
				if (sz.Width < 0f)
				{
					sz.Width = 0f;
				}
				else if (sz.Width > 1f)
				{
					sz.Width = 1f;
				}
				if (sz.Height < 0f)
				{
					sz.Height = 0f;
				}
				else if (sz.Height > 1f)
				{
					sz.Height = 1f;
				}
				if (brushInfo != null && (brushInfo.StartOrFocusScales.X != sz.Width || brushInfo.StartOrFocusScales.Y != sz.Height))
				{
					GoBrushInfo goBrushInfo = new GoBrushInfo(brushInfo);
					goBrushInfo.StartOrFocusScales = new PointF(sz.Width, sz.Height);
					BrushInfo = goBrushInfo;
				}
				else if (brushInfo == null && sz != default(SizeF))
				{
					GoBrushInfo goBrushInfo2 = new GoBrushInfo(brushInfo);
					goBrushInfo2.BrushStyle = GoBrushStyle.None;
					goBrushInfo2.StartOrFocusScales = new PointF(sz.Width, sz.Height);
					BrushInfo = goBrushInfo2;
				}
			}
		}

		/// <summary>
		/// Gets or sets the kind of brush used by this shape.
		/// </summary>
		/// <value>
		/// A <see cref="T:Northwoods.Go.GoBrushStyle" />.
		/// The default value depends on the shape type.
		/// However, for most shapes the default is <see cref="F:Northwoods.Go.GoBrushStyle.None" />
		/// because <see cref="P:Northwoods.Go.GoShape.Brush" /> is null.
		/// </value>
		/// <remarks>
		/// <para>
		/// Changing this property to a new <see cref="T:Northwoods.Go.GoBrushStyle" /> that is a
		/// gradient will also reset some of the brush properties to
		/// default values established by the corresponding <b>Fill...</b> methods.
		/// In particular, the value of <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" />, <see cref="P:Northwoods.Go.GoShape.BrushPoint" />,
		/// <see cref="P:Northwoods.Go.GoShape.BrushStartPoint" />, and/or <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> may change.
		/// However, setting this property will not change the <see cref="P:Northwoods.Go.GoShape.BrushColor" />,
		/// <see cref="P:Northwoods.Go.GoShape.BrushForeColor" /> or <see cref="P:Northwoods.Go.GoShape.BrushMidColor" /> properties,
		/// although some or all of those properties might not be used by certain
		/// brush styles.  You will normally want to set this property first, before
		/// setting other <b>Brush...</b> properties.
		/// </para>
		/// <para>
		/// When a linear gradient brush or a path gradient brush is drawn very small,
		/// due to a combination of small size and small <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush or not draw anything at all,
		/// both for efficiency as well as to avoid GDI+ errors.
		/// When a path gradient brush is drawn very large,
		/// due to a combination of large size and large <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.DocScale" />,
		/// GoDiagram may substitute a solid brush, both for efficiency as well as to avoid GDI+ errors.
		/// </para>
		/// <para>
		/// Caution: using gradient brush styles, particularly path gradients,
		/// can be computationally expensive to paint.  This is especially true
		/// for large shapes.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[Description("The kind of simple brush used by this shape, including some linear and path gradients")]
		public virtual GoBrushStyle BrushStyle
		{
			get
			{
				return BrushInfo?.BrushStyle ?? GoBrushStyle.None;
			}
			set
			{
				GoBrushInfo brushInfo = BrushInfo;
				if ((brushInfo == null || brushInfo.BrushStyle != value) && (brushInfo != null || value != 0))
				{
					BrushInfo = ModifyBrushStyle(brushInfo, value);
				}
			}
		}

		/// <summary>
		/// Makes sure the copied shape does not share any internal data references with the
		/// original one.
		/// </summary>
		/// <param name="env"></param>
		/// <returns></returns>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoShape goShape = (GoShape)base.CopyObject(env);
			if (goShape != null)
			{
				goShape.myPath = null;
				goShape.myBrush = null;
			}
			return goShape;
		}

		/// <summary>
		/// This static method converts a <see cref="T:Northwoods.Go.GoBrushStyle" /> enumerated value representing a hatch style
		/// to a <c>System.Drawing.Drawing2D.HatchStyle</c> value.
		/// </summary>
		/// <param name="s">must be one of the <c>GoBrushStyle.Hatch...</c> enum values</param>
		/// <returns>a <c>HatchStyle</c></returns>
		public static HatchStyle ToHatchStyle(GoBrushStyle s)
		{
			return (HatchStyle)checked(s - 256);
		}

		/// <summary>
		/// This static method converts a <c>System.Drawing.Drawing2D.HatchStyle</c> enumerated value
		/// to the corresponding <see cref="T:Northwoods.Go.GoBrushStyle" /> value.
		/// </summary>
		/// <param name="s">a <c>HatchStyle</c></param>
		/// <returns>a <see cref="T:Northwoods.Go.GoBrushStyle" /> named "Hatch..."</returns>
		public static GoBrushStyle ToBrushStyle(HatchStyle s)
		{
			return (GoBrushStyle)checked(s + 256);
		}

		private static void RgbToHsb(float red, float green, float blue, out float hue, out float saturation, out float brightness)
		{
			brightness = Math.Max(red, Math.Max(green, blue));
			float num = Math.Min(red, Math.Min(green, blue));
			if (brightness == num)
			{
				hue = 0f;
			}
			else if (brightness == red)
			{
				if (green < blue)
				{
					hue = 60f * ((green - blue) / (brightness - num)) + 360f;
				}
				else
				{
					hue = 60f * ((green - blue) / (brightness - num));
				}
			}
			else if (brightness == green)
			{
				hue = 60f * ((blue - red) / (brightness - num)) + 120f;
			}
			else
			{
				hue = 60f * ((red - green) / (brightness - num)) + 240f;
			}
			if (brightness == 0f)
			{
				saturation = 0f;
			}
			else
			{
				saturation = (brightness - num) / brightness;
			}
		}

		private static void HsbToRgb(float hue, float saturation, float brightness, out float red, out float green, out float blue)
		{
			if (saturation == 0f)
			{
				red = brightness;
				green = brightness;
				blue = brightness;
				return;
			}
			int num = checked((int)(Math.Floor((double)hue / 60.0) % 6.0));
			float num2 = hue / 60f - (float)num;
			float num3 = brightness * (1f - saturation);
			float num4 = brightness * (1f - num2 * saturation);
			float num5 = brightness * (1f - (1f - num2) * saturation);
			switch (num)
			{
			default:
				red = brightness;
				green = num5;
				blue = num3;
				break;
			case 1:
				red = num4;
				green = brightness;
				blue = num3;
				break;
			case 2:
				red = num3;
				green = brightness;
				blue = num5;
				break;
			case 3:
				red = num3;
				green = num4;
				blue = brightness;
				break;
			case 4:
				red = num5;
				green = num3;
				blue = brightness;
				break;
			case 5:
				red = brightness;
				green = num3;
				blue = num4;
				break;
			}
		}

		private static Color VeryLight(Color c)
		{
			RgbToHsb((float)(int)c.R / 255f, (float)(int)c.G / 255f, (float)(int)c.B / 255f, out float hue, out float saturation, out float brightness);
			HsbToRgb(hue, Math.Min(saturation / 2f, 0.1f), Math.Max(1f - (1f - brightness) / 2f, 0.9f), out float red, out float green, out float blue);
			return checked(Color.FromArgb(c.A, (byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f)));
		}

		private static Color LighterSofter(Color c)
		{
			RgbToHsb((float)(int)c.R / 255f, (float)(int)c.G / 255f, (float)(int)c.B / 255f, out float hue, out float saturation, out float brightness);
			brightness *= 1.4f;
			if (brightness < 0f)
			{
				brightness = 0f;
			}
			else if (brightness > 1f)
			{
				brightness = 1f;
			}
			saturation /= 1.4f;
			if (saturation < 0f)
			{
				saturation = 0f;
			}
			else if (saturation > 1f)
			{
				saturation = 1f;
			}
			HsbToRgb(hue, saturation, brightness, out float red, out float green, out float blue);
			return checked(Color.FromArgb(c.A, (byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f)));
		}

		private static Color DarkerIntenser(Color c)
		{
			RgbToHsb((float)(int)c.R / 255f, (float)(int)c.G / 255f, (float)(int)c.B / 255f, out float hue, out float saturation, out float brightness);
			brightness /= 1.4f;
			if (brightness < 0f)
			{
				brightness = 0f;
			}
			else if (brightness > 1f)
			{
				brightness = 1f;
			}
			saturation *= 1.4f;
			if (saturation < 0f)
			{
				saturation = 0f;
			}
			else if (saturation > 1f)
			{
				saturation = 1f;
			}
			HsbToRgb(hue, saturation, brightness, out float red, out float green, out float blue);
			return checked(Color.FromArgb(c.A, (byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f)));
		}

		private int CanonicalizeLinearGradientSpot(int spot)
		{
			switch (spot)
			{
			case 32:
				return 32;
			case 256:
				return 256;
			case 2:
				return 2;
			case 4:
				return 4;
			default:
				throw new ArgumentOutOfRangeException("spot", "spot must be one of: MiddleTop, MiddleLeft, TopLeft, or TopRight");
			}
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a simple vertical linear gradient using the given <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillSimpleGradient(Color color)
		{
			FillSimpleGradient(VeryLight(color), color, 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a simple linear gradient using the given <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillSimpleGradient(Color color, int spot)
		{
			FillSimpleGradient(VeryLight(color), color, spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a simple linear gradient starting with the <paramref name="start" /> color
		/// and ending with the <paramref name="end" /> color.
		/// </summary>
		/// <param name="start">the <b>Color</b> at the starting <paramref name="spot" /></param>
		/// <param name="end">the <b>Color</b> at the ending spot</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillSimpleGradient(Color start, Color end, int spot)
		{
			spot = CanonicalizeLinearGradientSpot(spot);
			GoBrushInfo goBrushInfo = null;
			switch (spot)
			{
			default:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SimpleGradientVertical);
				break;
			case 256:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SimpleGradientHorizontal);
				break;
			case 2:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SimpleGradientForwardDiagonal);
				break;
			case 4:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SimpleGradientBackwardDiagonal);
				break;
			}
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = end;
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a vertical linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// having the middle with a color lighter than <paramref name="color" />, and ending with a darker color than the <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShadedGradient(Color color)
		{
			FillMiddleGradient(VeryLight(color), LighterSofter(color), DarkerIntenser(color), 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// having the middle with a color lighter than <paramref name="color" />, and ending with a darker color than the <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShadedGradient(Color color, int spot)
		{
			FillMiddleGradient(VeryLight(color), LighterSofter(color), DarkerIntenser(color), spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with the <paramref name="start" /> color,
		/// having the middle with a color lighter than <paramref name="color" />, and ending with a darker color than the <paramref name="color" />.
		/// </summary>
		/// <param name="start">the <b>Color</b> at the starting <paramref name="spot" /></param>
		/// <param name="color">a <b>Color</b> used to generate the colors in the middle and at the end of the gradient</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShadedGradient(Color start, Color color, int spot)
		{
			FillMiddleGradient(start, LighterSofter(color), DarkerIntenser(color), spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a vertical linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// and having the last half of the gradient be the solid color <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillHalfGradient(Color color)
		{
			FillMiddleGradient(VeryLight(color), color, color, 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// and having the last half of the gradient be the solid color <paramref name="color" />.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived, keeping the same hue</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillHalfGradient(Color color, int spot)
		{
			FillMiddleGradient(VeryLight(color), color, color, spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with the <paramref name="start" /> color,
		/// and having the last half of the gradient be the solid color <paramref name="mid" />.
		/// </summary>
		/// <param name="start">the <b>Color</b> at the starting <paramref name="spot" /></param>
		/// <param name="mid">the <b>Color</b> from the middle to the end of the gradient</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillHalfGradient(Color start, Color mid, int spot)
		{
			FillMiddleGradient(start, mid, mid, spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a vertical linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// having the middle with a color lighter than <paramref name="color" />, and ending with the starting color.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillMiddleGradient(Color color)
		{
			FillMiddleGradient(VeryLight(color), LighterSofter(color), VeryLight(color), 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// having the middle with a color lighter than <paramref name="color" />, and ending with the starting color.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient colors are derived</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillMiddleGradient(Color color, int spot)
		{
			FillMiddleGradient(VeryLight(color), LighterSofter(color), VeryLight(color), spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with a very light version of the <paramref name="color" /> color,
		/// having the middle be the color <paramref name="mid" /> color, and ending with the starting color.
		/// </summary>
		/// <param name="color">a <b>Color</b> from which the gradient start and end colors are derived</param>
		/// <param name="mid">the <b>Color</b> at the middle of the gradient</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillMiddleGradient(Color color, Color mid, int spot)
		{
			FillMiddleGradient(VeryLight(color), mid, VeryLight(color), spot);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with the <paramref name="start" /> color,
		/// having the middle be the <paramref name="mid" /> color, and ending with the <paramref name="end" /> color.
		/// </summary>
		/// <param name="start">the <b>Color</b> at the starting <paramref name="spot" /></param>
		/// <param name="mid">the <b>Color</b> in the middle of the gradient</param>
		/// <param name="end">the <b>Color</b> at the ending spot</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillMiddleGradient(Color start, Color mid, Color end, int spot)
		{
			spot = CanonicalizeLinearGradientSpot(spot);
			GoBrushInfo goBrushInfo = null;
			switch (spot)
			{
			default:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.MiddleGradientVertical);
				break;
			case 256:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.MiddleGradientHorizontal);
				break;
			case 2:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.MiddleGradientForwardDiagonal);
				break;
			case 4:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.MiddleGradientBackwardDiagonal);
				break;
			}
			goBrushInfo.ForeColor = start;
			goBrushInfo.MidColor = mid;
			goBrushInfo.Color = end;
			goBrushInfo.MidFraction = 0.5f;
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a vertical linear gradient starting with the <paramref name="start" /> color,
		/// with most of the center being <b>Color.White</b>.
		/// </summary>
		/// <param name="start">a <b>Color</b> for the edge</param>
		/// <remarks>
		/// Only filled areas inside the shape near the MiddleTop will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color edging.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillSingleEdge(Color start)
		{
			FillSingleEdge(start, Color.White, 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with the <paramref name="start" /> color,
		/// having the center being the <paramref name="center" /> color.
		/// </summary>
		/// <param name="start">a <b>Color</b> for the edge</param>
		/// <param name="center">a <b>Color</b> for the rest of the area</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.MiddleRight</b>, <b>GoObject.MiddleBottom</b>, that shows the <paramref name="start" /> color</param>
		/// <remarks>
		/// Only filled areas inside the shape near the <paramref name="spot" /> will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color edging.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillSingleEdge(Color start, Color center, int spot)
		{
			if (spot != 32 && spot != 256 && spot != 64 && spot != 128)
			{
				throw new ArgumentOutOfRangeException("spot", "spot must be one of: MiddleTop, MiddleLeft, MiddleRight, or MiddleBottom");
			}
			GoBrushInfo goBrushInfo = null;
			switch (spot)
			{
			default:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SingleEdgeGradientTop);
				break;
			case 256:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SingleEdgeGradientLeft);
				break;
			case 64:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SingleEdgeGradientRight);
				break;
			case 128:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.SingleEdgeGradientBottom);
				break;
			}
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.MidFraction = 0.1f;
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a vertical linear gradient starting with the <paramref name="start" /> color,
		/// with most of the center being <b>Color.White</b>, and ending with the starting color.
		/// </summary>
		/// <param name="start">a <b>Color</b> for the edge</param>
		/// <remarks>
		/// Only filled areas inside the shape near the top or bottom will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color edging.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillDoubleEdge(Color start)
		{
			FillDoubleEdge(start, Color.White, 32);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a linear gradient starting with the <paramref name="start" /> color,
		/// having the center the <paramref name="center" /> color, and ending with the starting color.
		/// </summary>
		/// <param name="start">a <b>Color</b> for the edges</param>
		/// <param name="center">a <b>Color</b> for the rest of the area</param>
		/// <param name="spot">one of four <see cref="T:Northwoods.Go.GoObject" /> spots: <b>GoObject.MiddleTop</b>, <b>GoObject.MiddleLeft</b>, <b>GoObject.TopLeft</b>, <b>GoObject.TopRight</b>, from which to start the gradient, towards the opposite spot</param>
		/// <remarks>
		/// Only filled areas inside the shape near the <paramref name="spot" /> or near the opposite spot will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color edging.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillDoubleEdge(Color start, Color center, int spot)
		{
			spot = CanonicalizeLinearGradientSpot(spot);
			GoBrushInfo goBrushInfo = null;
			switch (spot)
			{
			default:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.DoubleEdgeGradientVertical);
				break;
			case 256:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.DoubleEdgeGradientHorizontal);
				break;
			case 2:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.DoubleEdgeGradientForwardDiagonal);
				break;
			case 4:
				goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.DoubleEdgeGradientBackwardDiagonal);
				break;
			}
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.MidFraction = 0.2f;
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient starting with the <paramref name="start" /> color,
		/// with most of the center being <b>Color.White</b>.
		/// </summary>
		/// <param name="start">a <b>Color</b></param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// Only filled areas inside the shape near the edges of the bounding rectangle will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color fringe.
		/// Its thickness is always proportional to the width and height of the shape.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeFringe(Color start)
		{
			FillShapeFringe(start, Color.White);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient starting with the <paramref name="start" /> color,
		/// with most of the center being the <paramref name="center" /> color.
		/// </summary>
		/// <param name="start">a <b>Color</b></param>
		/// <param name="center">a <b>Color</b></param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// Only filled areas inside the shape near the edges of the bounding rectangle will get the starting color.
		/// You can change the <see cref="P:Northwoods.Go.GoShape.BrushMidFraction" /> property to adjust the thickness of the <paramref name="start" /> color fringe.
		/// Its thickness is always proportional to the width and height of the shape.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeFringe(Color start, Color center)
		{
			GoBrushInfo goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.ShapeFringeGradient);
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.MidFraction = 0.2f;
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient with the <paramref name="color" /> color
		/// surrounding a very light version of that color.
		/// </summary>
		/// <param name="color">a <b>Color</b> that is the starting color of the gradient;
		/// the center color is a very light color of the same hue</param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// Only filled areas inside the shape near the edges of the bounding rectangle will get the starting color.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeGradient(Color color)
		{
			FillShapeGradient(color, VeryLight(color));
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient with the <paramref name="center" /> color
		/// surrounded by the <paramref name="start" /> color.
		/// </summary>
		/// <param name="start">the <b>Color</b> drawn inside the edges of the shape</param>
		/// <param name="center">the <b>Color</b> filling the center of the shape</param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// Only filled areas inside the shape near the edges of the bounding rectangle will get the starting color.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeGradient(Color start, Color center)
		{
			GoBrushInfo goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.ShapeSimpleGradient);
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.StartOrFocusScales = new PointF(0.15f, 0.15f);
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient that is mostly the <paramref name="start" /> color,
		/// with a white "highlight".
		/// </summary>
		/// <param name="start">a <b>Color</b></param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.15f, 0.15f)</b>, near the top-left corner.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0.1f, 0.1f)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeHighlight(Color start)
		{
			FillShapeHighlight(start, Color.White);
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a path shaped gradient that is mostly the <paramref name="start" /> color,
		/// with a "highlight" given by the <paramref name="center" /> color.
		/// </summary>
		/// <param name="start">the <b>Color</b> drawn inside the edges of the shape</param>
		/// <param name="center">the <b>Color</b> filling the center of the shape</param>
		/// <remarks>
		/// Path gradients only work well for convex shapes having only a single figure.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.15f, 0.15f)</b>, near the top-left corner.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0.1f, 0.1f)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillShapeHighlight(Color start, Color center)
		{
			GoBrushInfo goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.ShapeHighlightGradient);
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.Point = new PointF(0.15f, 0.15f);
			goBrushInfo.StartOrFocusScales = new PointF(0.1f, 0.1f);
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a gradient occupying this shape's <see cref="P:Northwoods.Go.GoObject.Bounds" />,
		/// with the <paramref name="color" /> color along the four edges of the rectangle,
		/// surrounding a very light version of that color.
		/// </summary>
		/// <param name="color">the <b>Color</b> drawn inside the edges of the rectangular bounds</param>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoShape.Brush" /> is a <b>PathGradientBrush</b> using the rectangular bounds of this shape, clipped by the actual shape.
		/// Hence you may not see the <paramref name="color" /> very much, except where the shape is close to the bounds.
		/// This use of a <b>PathGradientBrush</b> does not suffer from the effects of GDI+ filling concave or multi-figure shapes.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.5f, 0.5f)</b>, the middle of the shape.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0, 0)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillRectangleGradient(Color color)
		{
			FillRectangleGradient(color, VeryLight(color));
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a gradient occupying this shape's <see cref="P:Northwoods.Go.GoObject.Bounds" />,
		/// with the <paramref name="start" /> color along the four edges of the rectangle, and with the
		/// <paramref name="center" /> color at the center.
		/// </summary>
		/// <param name="start">the <b>Color</b> drawn inside the edges of the rectangular bounds</param>
		/// <param name="center">the <b>Color</b> filling the center of the shape</param>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoShape.Brush" /> is a <b>PathGradientBrush</b> using the rectangular bounds of this shape, clipped by the actual shape.
		/// Hence you may not see the <paramref name="start" /> color very much, except where the shape is close to the bounds.
		/// This use of a <b>PathGradientBrush</b> does not suffer from the effects of GDI+ filling concave or multi-figure shapes.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.5f, 0.5f)</b>, the middle of the shape.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0, 0)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillRectangleGradient(Color start, Color center)
		{
			GoBrushInfo goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.RectangleGradient);
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.Point = new PointF(0.5f, 0.5f);
			goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
			BrushInfo = goBrushInfo;
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a gradient occupying this shape's <see cref="P:Northwoods.Go.GoObject.Bounds" />,
		/// with the <paramref name="color" /> color along the four edges of the rectangle,
		/// surrounding a very light version of that color.
		/// </summary>
		/// <param name="color">the <b>Color</b> drawn inside the edges of the rectangular bounds</param>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoShape.Brush" /> is a <b>PathGradientBrush</b> using the rectangular bounds of this shape, clipped by the actual shape.
		/// Hence you may not see the <paramref name="color" /> very much, except where the shape is close to the bounds.
		/// This use of a <b>PathGradientBrush</b> does not suffer from the effects of GDI+ filling concave or multi-figure shapes.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.5f, 0.5f)</b>, the middle of the shape.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0, 0)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillEllipseGradient(Color color)
		{
			FillEllipseGradient(color, VeryLight(color));
		}

		/// <summary>
		/// Set the <see cref="P:Northwoods.Go.GoShape.Brush" /> to be a gradient occupying this shape's <see cref="P:Northwoods.Go.GoObject.Bounds" />,
		/// with the <paramref name="start" /> color along the four edges of the rectangle, and with the
		/// <paramref name="center" /> color at the center.
		/// </summary>
		/// <param name="start">the <b>Color</b> drawn inside the edges of the rectangular bounds</param>
		/// <param name="center">the <b>Color</b> filling the center of the shape</param>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoShape.Brush" /> is a <b>PathGradientBrush</b> using the rectangular bounds of this shape, clipped by the actual shape.
		/// Hence you may not see the <paramref name="start" /> color very much, except where the shape is close to the bounds.
		/// This use of a <b>PathGradientBrush</b> does not suffer from the effects of GDI+ filling concave or multi-figure shapes.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushPoint" /> property to change where the center color is centered;
		/// the default is <b>new PointF(0.5f, 0.5f)</b>, the middle of the shape.
		/// You can set the <see cref="P:Northwoods.Go.GoShape.BrushFocusScales" /> property to change the size of the center color area;
		/// the default is <b>new SizeF(0, 0)</b>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoShape.Brush" />
		public void FillEllipseGradient(Color start, Color center)
		{
			GoBrushInfo goBrushInfo = ModifyBrushStyle(BrushInfo, GoBrushStyle.EllipseGradient);
			goBrushInfo.ForeColor = start;
			goBrushInfo.Color = center;
			goBrushInfo.Point = new PointF(0.5f, 0.5f);
			goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
			BrushInfo = goBrushInfo;
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
				base.ChangeValue(e, undo);
				ResetPath();
				break;
			case 1101:
			{
				object value2 = e.GetValue(undo);
				if (value2 is Pen)
				{
					Pen = (Pen)value2;
				}
				else if (value2 is GoPenInfo)
				{
					PenInfo = (GoPenInfo)value2;
				}
				break;
			}
			case 1102:
			{
				object value = e.GetValue(undo);
				if (value is Brush)
				{
					Brush = (Brush)value;
				}
				else if (value is GoBrushInfo)
				{
					BrushInfo = (GoBrushInfo)value;
				}
				break;
			}
			default:
				base.ChangeValue(e, undo);
				break;
			}
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
			GoPenInfo penInfo = PenInfo;
			if (penInfo != null)
			{
				float num = Math.Max(penInfo.Width, 1f) / 2f * penInfo.MiterLimit + 1f;
				GoObject.InflateRect(ref rect, num, num);
			}
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
			return rect;
		}

		internal static Pen NewPen(Color c, float w)
		{
			return new Pen(c, w);
		}

		internal static Color GetPenColor(Pen p, Color def)
		{
			if (p == null)
			{
				return def;
			}
			try
			{
				return p.Color;
			}
			catch (Exception)
			{
				return def;
			}
		}

		internal static float GetPenWidth(Pen pen)
		{
			if (pen == null)
			{
				return 0f;
			}
			try
			{
				return pen.Width;
			}
			catch (Exception)
			{
				return 1f;
			}
		}

		/// <summary>
		/// Get the effective width of a pen in a view.
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="view"></param>
		/// <returns>
		/// Zero if <paramref name="pen" /> is null.
		/// If the pen's <c>Width</c> is zero,
		/// this returns the width of one pixel in the <paramref name="view" />,
		/// if the view is non-null, inversely proportional to the <see cref="P:Northwoods.Go.GoView.DocScale" />.
		/// Otherwise this just returns the pen's <c>Width</c>.
		/// </returns>
		public static float GetPenWidth(Pen pen, GoView view)
		{
			if (pen == null)
			{
				return 0f;
			}
			float width = GetPenInfo(pen).Width;
			if (width == 0f && view != null && view.DocScale > 0f)
			{
				return 1f / view.DocScale;
			}
			return width;
		}

		internal static GoBrushInfo ModifyBrushStyle(GoBrushInfo info, GoBrushStyle style)
		{
			GoBrushInfo goBrushInfo = new GoBrushInfo(info);
			goBrushInfo.BrushStyle = style;
			switch (style)
			{
			case GoBrushStyle.SimpleGradientVertical:
			case GoBrushStyle.MiddleGradientVertical:
			case GoBrushStyle.SingleEdgeGradientTop:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(0f, 1f);
				break;
			case GoBrushStyle.SimpleGradientHorizontal:
			case GoBrushStyle.MiddleGradientHorizontal:
			case GoBrushStyle.SingleEdgeGradientLeft:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(1f, 0f);
				break;
			case GoBrushStyle.SimpleGradientForwardDiagonal:
			case GoBrushStyle.MiddleGradientForwardDiagonal:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(1f, 1f);
				break;
			case GoBrushStyle.SimpleGradientBackwardDiagonal:
			case GoBrushStyle.MiddleGradientBackwardDiagonal:
				goBrushInfo.StartOrFocusScales = new PointF(1f, 0f);
				goBrushInfo.Point = new PointF(0f, 1f);
				break;
			case GoBrushStyle.SingleEdgeGradientRight:
				goBrushInfo.StartOrFocusScales = new PointF(1f, 0f);
				goBrushInfo.Point = new PointF(0f, 0f);
				break;
			case GoBrushStyle.SingleEdgeGradientBottom:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 1f);
				goBrushInfo.Point = new PointF(0f, 0f);
				break;
			case GoBrushStyle.DoubleEdgeGradientVertical:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(0f, 0.5f);
				break;
			case GoBrushStyle.DoubleEdgeGradientHorizontal:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(0.5f, 0f);
				break;
			case GoBrushStyle.DoubleEdgeGradientForwardDiagonal:
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				break;
			case GoBrushStyle.DoubleEdgeGradientBackwardDiagonal:
				goBrushInfo.StartOrFocusScales = new PointF(1f, 0f);
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				break;
			case GoBrushStyle.ShapeFringeGradient:
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				break;
			case GoBrushStyle.ShapeSimpleGradient:
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				goBrushInfo.StartOrFocusScales = new PointF(0.15f, 0.15f);
				break;
			case GoBrushStyle.ShapeHighlightGradient:
				goBrushInfo.Point = new PointF(0.15f, 0.15f);
				goBrushInfo.StartOrFocusScales = new PointF(0.1f, 0.1f);
				break;
			case GoBrushStyle.RectangleGradient:
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				break;
			case GoBrushStyle.EllipseGradient:
				goBrushInfo.Point = new PointF(0.5f, 0.5f);
				goBrushInfo.StartOrFocusScales = new PointF(0f, 0f);
				break;
			}
			switch (style)
			{
			case GoBrushStyle.MiddleGradientVertical:
			case GoBrushStyle.MiddleGradientHorizontal:
			case GoBrushStyle.MiddleGradientForwardDiagonal:
			case GoBrushStyle.MiddleGradientBackwardDiagonal:
				goBrushInfo.MidBlendFactor = 1f;
				goBrushInfo.MidFraction = 0.5f;
				break;
			case GoBrushStyle.SingleEdgeGradientTop:
			case GoBrushStyle.SingleEdgeGradientLeft:
			case GoBrushStyle.SingleEdgeGradientRight:
			case GoBrushStyle.SingleEdgeGradientBottom:
				goBrushInfo.MidBlendFactor = 1f;
				goBrushInfo.MidFraction = 0.1f;
				break;
			case GoBrushStyle.DoubleEdgeGradientVertical:
			case GoBrushStyle.DoubleEdgeGradientHorizontal:
			case GoBrushStyle.DoubleEdgeGradientForwardDiagonal:
			case GoBrushStyle.DoubleEdgeGradientBackwardDiagonal:
				goBrushInfo.MidBlendFactor = 1f;
				goBrushInfo.MidFraction = 0.2f;
				break;
			case GoBrushStyle.ShapeFringeGradient:
				goBrushInfo.MidBlendFactor = 1f;
				goBrushInfo.MidFraction = 0.2f;
				break;
			case GoBrushStyle.RectangleGradient:
			case GoBrushStyle.EllipseGradient:
				goBrushInfo.MidBlendFactor = 0.5f;
				goBrushInfo.MidFraction = 0.5f;
				break;
			default:
				goBrushInfo.MidBlendFactor = float.NaN;
				goBrushInfo.MidFraction = float.NaN;
				break;
			}
			return goBrushInfo;
		}

		/// <summary>
		/// Paint this shape, using this shape's <see cref="P:Northwoods.Go.GoShape.Brush" /> and <see cref="P:Northwoods.Go.GoShape.Pen" />,
		/// with a shadow if <see cref="P:Northwoods.Go.GoObject.Shadowed" /> is true.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		public override void Paint(Graphics g, GoView view)
		{
			PaintPath(g, view, GetPath());
		}

		/// <summary>
		/// Produce a <c>GraphicsPath</c> representing this shape.
		/// </summary>
		/// <returns>a newly allocated <c>GraphicsPath</c></returns>
		/// <remarks>
		/// The result of this method is not necessarily the whole path
		/// used by the <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> method.
		/// In fact, <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> or <see cref="M:Northwoods.Go.GoObject.ContainsPoint(System.Drawing.PointF)" />
		/// or other methods might never call this method.
		/// </remarks>
		public virtual GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = new GraphicsPath(FillMode.Winding);
			graphicsPath.AddRectangle(Bounds);
			return graphicsPath;
		}

		/// <summary>
		/// Gets a cached <c>GraphicsPath</c> produced by a call to <see cref="M:Northwoods.Go.GoShape.MakePath" />.
		/// </summary>
		/// <returns></returns>
		protected GraphicsPath GetPath()
		{
			if (myPath == null)
			{
				myPath = MakePath();
			}
			return myPath;
		}

		/// <summary>
		/// Clear any cached <c>GraphicsPath</c>.
		/// </summary>
		/// <remarks>
		/// Calling this method will result in <see cref="M:Northwoods.Go.GoShape.MakePath" /> to be called
		/// when needed by a call to <see cref="M:Northwoods.Go.GoShape.GetPath" />.
		/// </remarks>
		protected virtual void ResetPath()
		{
			if (myPath != null)
			{
				myPath.Dispose();
				myPath = null;
			}
			if (myBrush != null)
			{
				if (myBrushInfo != null && !myBrushInfo.HasBrush)
				{
					myBrush.Dispose();
				}
				myBrush = null;
			}
		}

		internal void DisposePath(GraphicsPath path)
		{
			if (path != myPath)
			{
				path.Dispose();
			}
		}

		internal void PaintPath(Graphics g, GoView view, GraphicsPath path)
		{
			Brush brush = Brush;
			Pen pen = Pen;
			if (Shadowed)
			{
				SizeF shadowOffset = GetShadowOffset(view);
				if (brush != null)
				{
					Brush shadowBrush = GetShadowBrush(view);
					try
					{
						view.TranslateTransform(g, shadowOffset.Width, shadowOffset.Height);
						DrawPath(g, view, null, shadowBrush, path);
					}
					finally
					{
						view.TranslateTransform(g, 0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
				else if (pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					try
					{
						view.TranslateTransform(g, shadowOffset.Width, shadowOffset.Height);
						DrawPath(g, view, shadowPen, null, path);
					}
					finally
					{
						view.TranslateTransform(g, 0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
			}
			DrawPath(g, view, pen, brush, path);
		}

		internal void PaintPolygon(Graphics g, GoView view, PointF[] pts)
		{
			Brush brush = Brush;
			Pen pen = Pen;
			SizeF shadowOffset = GetShadowOffset(view);
			if (Shadowed)
			{
				if (brush != null)
				{
					Brush shadowBrush = GetShadowBrush(view);
					try
					{
						view.TranslateTransform(g, shadowOffset.Width, shadowOffset.Height);
						DrawPolygon(g, view, null, shadowBrush, pts);
					}
					finally
					{
						view.TranslateTransform(g, 0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
				else if (pen != null)
				{
					Pen shadowPen = GetShadowPen(view, PenWidth);
					try
					{
						view.TranslateTransform(g, shadowOffset.Width, shadowOffset.Height);
						DrawPolygon(g, view, shadowPen, null, pts);
					}
					finally
					{
						view.TranslateTransform(g, 0f - shadowOffset.Width, 0f - shadowOffset.Height);
					}
				}
			}
			DrawPolygon(g, view, pen, brush, pts);
		}

		/// <summary>
		/// This is a convenience method for shifting a point on the edge of the
		/// given rectangle's bounds out by some distance.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="rect"></param>
		/// <param name="shift"></param>
		/// <returns>
		/// if <paramref name="p" /> had been on the edge of the rectangle, the
		/// value will be shifted out by the <paramref name="shift" /> in either or
		/// both directions; otherwise the unmodified <paramref name="p" />
		/// is returned
		/// </returns>
		public static PointF ExpandPointOnEdge(PointF p, RectangleF rect, float shift)
		{
			if (p.X <= rect.X)
			{
				p.X -= shift;
			}
			else if (p.X >= rect.X + rect.Width)
			{
				p.X += shift;
			}
			if (p.Y <= rect.Y)
			{
				p.Y -= shift;
			}
			else if (p.Y >= rect.Y + rect.Height)
			{
				p.Y += shift;
			}
			return p;
		}

		/// <summary>
		/// Consider the Pen width when determining if a point is in this shape.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool ContainsPoint(PointF p)
		{
			RectangleF a = Bounds;
			float penWidth = PenWidth;
			GoObject.InflateRect(ref a, penWidth / 2f, penWidth / 2f);
			return GoObject.ContainsRect(a, p);
		}

		/// <summary>
		/// Consider the Pen width when determining if this shape is inside a rectangle.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public override bool ContainedByRectangle(RectangleF r)
		{
			RectangleF a = Bounds;
			float penWidth = PenWidth;
			GoObject.InflateRect(ref a, penWidth / 2f, penWidth / 2f);
			if (r.Width > 0f && r.Height > 0f && a.Width >= 0f && a.Height >= 0f && a.X >= r.X && a.Y >= r.Y && a.X + a.Width <= r.X + r.Width)
			{
				return a.Y + a.Height <= r.Y + r.Height;
			}
			return false;
		}

		/// <summary>
		/// Consider the Pen width in determining the closest point of this object
		/// that intersects the given line.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool GetNearestIntersectionPoint(PointF p1, PointF p2, out PointF result)
		{
			RectangleF a = Bounds;
			float penWidth = PenWidth;
			GoObject.InflateRect(ref a, penWidth / 2f, penWidth / 2f);
			return GoObject.GetNearestIntersectionPoint(a, p1, p2, out result);
		}

		/// <summary>
		/// Draw a straight line.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public static void DrawLine(Graphics g, GoView view, Pen pen, float x1, float y1, float x2, float y2)
		{
			if (pen != null)
			{
				g.DrawLine(pen, x1, y1, x2, y2);
			}
		}

		/// <summary>
		/// Draw a connected sequence of straight lines.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="points"></param>
		public static void DrawLines(Graphics g, GoView view, Pen pen, PointF[] points)
		{
			if (pen != null)
			{
				g.DrawLines(pen, points);
			}
		}

		/// <summary>
		/// Draw a Bezier curve.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="x3"></param>
		/// <param name="y3"></param>
		/// <param name="x4"></param>
		/// <param name="y4"></param>
		public static void DrawBezier(Graphics g, GoView view, Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			try
			{
				if (pen != null)
				{
					g.DrawBezier(pen, x1, y1, x2, y2, x3, y3, x4, y4);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		/// <summary>
		/// Draw an open arc-shaped section of an ellipse.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="startangle"></param>
		/// <param name="sweepangle"></param>
		public static void DrawArc(Graphics g, GoView view, Pen pen, float x, float y, float width, float height, float startangle, float sweepangle)
		{
			try
			{
				if (pen != null)
				{
					g.DrawArc(pen, x, y, width, height, startangle, sweepangle);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		/// <summary>
		/// Draw a possibly filled rectangle or square.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public static void DrawRectangle(Graphics g, GoView view, Pen pen, Brush brush, float x, float y, float width, float height)
		{
			try
			{
				if (brush != null)
				{
					LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
					if (linearGradientBrush != null)
					{
						if (IsExtremelySmall(width, height, view))
						{
							Brush brush2 = new SolidBrush(linearGradientBrush.LinearColors[0]);
							g.FillRectangle(brush2, x, y, width, height);
							brush2.Dispose();
						}
						else
						{
							g.TranslateTransform(x, y);
							g.FillRectangle(brush, 0f, 0f, width, height);
							g.TranslateTransform(0f - x, 0f - y);
						}
					}
					else if (brush is PathGradientBrush && IsExtremelySized(width, height, view))
					{
						Brush brush3 = new SolidBrush(((PathGradientBrush)brush).CenterColor);
						g.FillRectangle(brush3, x, y, width, height);
						brush3.Dispose();
					}
					else if (brush is TextureBrush)
					{
						g.TranslateTransform(x, y);
						g.FillRectangle(brush, 0f, 0f, width, height);
						g.TranslateTransform(0f - x, 0f - y);
					}
					else
					{
						g.FillRectangle(brush, x, y, width, height);
					}
				}
				if (pen != null)
				{
					g.DrawRectangle(pen, x, y, width, height);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		/// <summary>
		/// Draw a possibly filled rounded rectangle.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w">the rectangle width</param>
		/// <param name="h">the rectangle height</param>
		/// <param name="cw">the desired corner radius width, actually limited by half the width <paramref name="w" /></param>
		/// <param name="ch">the desired corner radius height, actually limited by half the height <paramref name="h" /></param>
		public static void DrawRoundedRectangle(Graphics g, GoView view, Pen pen, Brush brush, float x, float y, float w, float h, float cw, float ch)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			GoRoundedRectangle.MakeRoundedRectangularPath(graphicsPath, 0f, 0f, new RectangleF(x, y, w, h), new SizeF(cw, ch));
			DrawPath(g, view, pen, brush, graphicsPath);
			graphicsPath.Dispose();
		}

		/// <summary>
		/// Draw a possibly filled ellipse or circle.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public static void DrawEllipse(Graphics g, GoView view, Pen pen, Brush brush, float x, float y, float width, float height)
		{
			try
			{
				if (brush != null)
				{
					LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
					if (linearGradientBrush != null)
					{
						if (IsExtremelySmall(width, height, view))
						{
							Brush brush2 = new SolidBrush(linearGradientBrush.LinearColors[0]);
							g.FillEllipse(brush2, x, y, width, height);
							brush2.Dispose();
						}
						else
						{
							g.TranslateTransform(x, y);
							g.FillEllipse(brush, 0f, 0f, width, height);
							g.TranslateTransform(0f - x, 0f - y);
						}
					}
					else if (brush is PathGradientBrush && IsExtremelySized(width, height, view))
					{
						Brush brush3 = new SolidBrush(((PathGradientBrush)brush).CenterColor);
						g.FillEllipse(brush3, x, y, width, height);
						brush3.Dispose();
					}
					else if (brush is TextureBrush)
					{
						g.TranslateTransform(x, y);
						g.FillEllipse(brush, 0f, 0f, width, height);
						g.TranslateTransform(0f - x, 0f - y);
					}
					else
					{
						g.FillEllipse(brush, x, y, width, height);
					}
				}
				if (pen != null)
				{
					g.DrawEllipse(pen, x, y, width, height);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		/// <summary>
		/// Draw a possibly filled pie-shaped section of an ellipse.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="startangle"></param>
		/// <param name="sweepangle"></param>
		public static void DrawPie(Graphics g, GoView view, Pen pen, Brush brush, float x, float y, float width, float height, float startangle, float sweepangle)
		{
			try
			{
				if (brush != null)
				{
					LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
					if (linearGradientBrush != null)
					{
						if (IsExtremelySmall(width, height, view))
						{
							Brush brush2 = new SolidBrush(linearGradientBrush.LinearColors[0]);
							g.FillPie(brush2, x, y, width, height, startangle, sweepangle);
							brush2.Dispose();
						}
						else
						{
							g.TranslateTransform(x, y);
							g.FillPie(brush, 0f, 0f, width, height, startangle, sweepangle);
							g.TranslateTransform(0f - x, 0f - y);
						}
					}
					else if (brush is PathGradientBrush && IsExtremelySized(width, height, view))
					{
						Brush brush3 = new SolidBrush(((PathGradientBrush)brush).CenterColor);
						g.FillPie(brush3, x, y, width, height, startangle, sweepangle);
						brush3.Dispose();
					}
					else if (brush is TextureBrush)
					{
						g.TranslateTransform(x, y);
						g.FillPie(brush, 0f, 0f, width, height, startangle, sweepangle);
						g.TranslateTransform(0f - x, 0f - y);
					}
					else
					{
						g.FillPie(brush, x, y, width, height, startangle, sweepangle);
					}
				}
				if (pen != null)
				{
					g.DrawPie(pen, x, y, width, height, startangle, sweepangle);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		/// <summary>
		/// Draw a possibly filled polygon.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="points"></param>
		public static void DrawPolygon(Graphics g, GoView view, Pen pen, Brush brush, PointF[] points)
		{
			checked
			{
				try
				{
					if (brush != null && points.Length > 2)
					{
						LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
						if (linearGradientBrush != null)
						{
							PointF pointF = points[0];
							float x = pointF.X;
							float x2 = pointF.X;
							float y = pointF.Y;
							float y2 = pointF.Y;
							for (int i = 1; i < points.Length; i++)
							{
								pointF = points[i];
								if (pointF.X < x)
								{
									x = pointF.X;
								}
								else if (pointF.X > x2)
								{
									x2 = pointF.X;
								}
								if (pointF.Y < y)
								{
									y = pointF.Y;
								}
								else if (pointF.Y > y2)
								{
									y2 = pointF.Y;
								}
							}
							if (IsExtremelySmall(x2 - x, y2 - y, view))
							{
								Brush brush2 = new SolidBrush(linearGradientBrush.LinearColors[0]);
								g.FillPolygon(brush2, points);
								brush2.Dispose();
							}
							else
							{
								g.TranslateTransform(x, y);
								PointF[] array = (PointF[])points.Clone();
								for (int j = 0; j < array.Length; j++)
								{
									array[j].X -= x;
									array[j].Y -= y;
								}
								g.FillPolygon(brush, array);
								g.TranslateTransform(0f - x, 0f - y);
							}
						}
						else if (brush is PathGradientBrush)
						{
							PathGradientBrush pathGradientBrush = (PathGradientBrush)brush;
							PointF pointF2 = points[0];
							float x3 = pointF2.X;
							float x4 = pointF2.X;
							float y3 = pointF2.Y;
							float y4 = pointF2.Y;
							for (int k = 1; k < points.Length; k++)
							{
								pointF2 = points[k];
								if (pointF2.X < x3)
								{
									x3 = pointF2.X;
								}
								else if (pointF2.X > x4)
								{
									x4 = pointF2.X;
								}
								if (pointF2.Y < y3)
								{
									y3 = pointF2.Y;
								}
								else if (pointF2.Y > y4)
								{
									y4 = pointF2.Y;
								}
							}
							if (IsExtremelySized(x4 - x3, y4 - y3, view))
							{
								Brush brush3 = new SolidBrush(pathGradientBrush.CenterColor);
								g.FillPolygon(brush3, points);
								brush3.Dispose();
							}
							else
							{
								g.FillPolygon(brush, points);
							}
						}
						else if (brush is TextureBrush)
						{
							PointF pointF3 = points[0];
							float x5 = pointF3.X;
							float y5 = pointF3.Y;
							for (int l = 1; l < points.Length; l++)
							{
								pointF3 = points[l];
								if (pointF3.X < x5)
								{
									x5 = pointF3.X;
								}
								if (pointF3.Y < y5)
								{
									y5 = pointF3.Y;
								}
							}
							g.TranslateTransform(x5, y5);
							PointF[] array2 = (PointF[])points.Clone();
							for (int m = 0; m < array2.Length; m++)
							{
								array2[m].X -= x5;
								array2[m].Y -= y5;
							}
							g.FillPolygon(brush, array2);
							g.TranslateTransform(0f - x5, 0f - y5);
						}
						else
						{
							g.FillPolygon(brush, points);
						}
					}
					if (pen != null)
					{
						g.DrawPolygon(pen, points);
					}
				}
				catch (OutOfMemoryException)
				{
				}
				catch (ArgumentException)
				{
				}
			}
		}

		/// <summary>
		/// Draw a general <c>GraphicsPath</c>, filling with the <paramref name="brush" /> and outlining with the <paramref name="pen" />.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <param name="pen"></param>
		/// <param name="brush"></param>
		/// <param name="path"></param>
		public static void DrawPath(Graphics g, GoView view, Pen pen, Brush brush, GraphicsPath path)
		{
			try
			{
				if (brush != null)
				{
					LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
					if (linearGradientBrush != null)
					{
						RectangleF bounds = path.GetBounds();
						if (IsExtremelySmall(bounds.Width, bounds.Height, view))
						{
							Brush brush2 = new SolidBrush(linearGradientBrush.LinearColors[0]);
							g.FillPath(brush2, path);
							brush2.Dispose();
						}
						else
						{
							g.TranslateTransform(bounds.X, bounds.Y);
							Matrix matrix = new Matrix(1f, 0f, 0f, 1f, 0f - bounds.X, 0f - bounds.Y);
							path.Transform(matrix);
							g.FillPath(brush, path);
							matrix.Translate(bounds.X * 2f, bounds.Y * 2f);
							path.Transform(matrix);
							matrix.Dispose();
							g.TranslateTransform(0f - bounds.X, 0f - bounds.Y);
						}
					}
					else if (brush is PathGradientBrush)
					{
						PathGradientBrush pathGradientBrush = (PathGradientBrush)brush;
						RectangleF bounds2 = path.GetBounds();
						if (IsExtremelySized(bounds2.Width, bounds2.Height, view))
						{
							Brush brush3 = new SolidBrush(pathGradientBrush.CenterColor);
							g.FillPath(brush3, path);
							brush3.Dispose();
						}
						else
						{
							g.FillPath(brush, path);
						}
					}
					else if (brush is TextureBrush)
					{
						RectangleF bounds3 = path.GetBounds();
						g.TranslateTransform(bounds3.X, bounds3.Y);
						Matrix matrix2 = new Matrix(1f, 0f, 0f, 1f, 0f - bounds3.X, 0f - bounds3.Y);
						path.Transform(matrix2);
						g.FillPath(brush, path);
						matrix2.Translate(bounds3.X * 2f, bounds3.Y * 2f);
						path.Transform(matrix2);
						matrix2.Dispose();
						g.TranslateTransform(0f - bounds3.X, 0f - bounds3.Y);
					}
					else
					{
						g.FillPath(brush, path);
					}
				}
				if (pen != null)
				{
					g.DrawPath(pen, path);
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentException)
			{
			}
		}

		private static bool IsExtremelySmall(float w, float h, GoView view)
		{
			SizeF worldScale = view.WorldScale;
			float docScale = view.DocScale;
			return w * h * docScale * docScale * worldScale.Width * worldScale.Height < 2f;
		}

		private static bool IsExtremelySized(float w, float h, GoView view)
		{
			SizeF worldScale = view.WorldScale;
			float docScale = view.DocScale;
			float num = w * h * docScale * docScale * worldScale.Width * worldScale.Height;
			if (!(num < 2f))
			{
				return num > 9999999f;
			}
			return true;
		}

		internal static GoPenInfo GetPenInfo(Pen p)
		{
			if (p == null)
			{
				return null;
			}
			if (PenInfo_Black == null)
			{
				PenInfo_Black = new GoPenInfo(Pens_Black);
			}
			if (p == Pens_Black)
			{
				return PenInfo_Black;
			}
			GoPenInfo goPenInfo = new GoPenInfo();
			goPenInfo.SetPen(p);
			return goPenInfo;
		}

		internal static GoBrushInfo GetBrushInfo(Brush b, object shapeorpath)
		{
			if (b == null)
			{
				return null;
			}
			if (BrushInfo_Black == null)
			{
				BrushInfo_Black = new GoBrushInfo(Brushes_Black);
				BrushInfo_Gray = new GoBrushInfo(Brushes_Gray);
				BrushInfo_LightGray = new GoBrushInfo(Brushes_LightGray);
				BrushInfo_White = new GoBrushInfo(Brushes_White);
			}
			if (b == Brushes_Black)
			{
				return BrushInfo_Black;
			}
			if (b == Brushes_Gray)
			{
				return BrushInfo_Gray;
			}
			if (b == Brushes_LightGray)
			{
				return BrushInfo_LightGray;
			}
			if (b == Brushes_White)
			{
				return BrushInfo_White;
			}
			GoBrushInfo goBrushInfo = new GoBrushInfo();
			goBrushInfo.SetBrush(b, shapeorpath);
			return goBrushInfo;
		}
	}
}
