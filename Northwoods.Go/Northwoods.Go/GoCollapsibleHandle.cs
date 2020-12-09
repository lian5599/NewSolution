using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Northwoods.Go
{
	/// <summary>
	/// This small rectangle is drawn with different shapes so that users
	/// can click on this to expand or collapse the <see cref="T:Northwoods.Go.IGoCollapsible" />
	/// parent object.
	/// </summary>
	/// <remarks>
	/// Specify the <see cref="P:Northwoods.Go.GoCollapsibleHandle.Style" /> as one of the <see cref="T:Northwoods.Go.GoCollapsibleHandleStyle" />
	/// enum values.
	/// </remarks>
	[Serializable]
	public class GoCollapsibleHandle : GoRoundedRectangle
	{
		private const int flagBordered = 1048576;

		/// <summary>
		/// This is a <see cref="T:Northwoods.Go.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedStyle = 2950;

		/// <summary>
		/// This is a <see cref="T:Northwoods.Go.GoObject" />.<c>Changed</c> subhint.
		/// </summary>
		public const int ChangedBordered = 2951;

		private GoCollapsibleHandleStyle myStyle;

		/// <summary>
		/// Gets or sets the appearance style for this handle.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoCollapsibleHandleStyle.PlusMinus" />.
		/// </value>
		public virtual GoCollapsibleHandleStyle Style
		{
			get
			{
				return myStyle;
			}
			set
			{
				GoCollapsibleHandleStyle goCollapsibleHandleStyle = myStyle;
				if (goCollapsibleHandleStyle != value)
				{
					myStyle = value;
					Changed(2950, (int)goCollapsibleHandleStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to draw a rectangle's border, using a Pen of width 1.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		public virtual bool Bordered
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
					Changed(2951, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// The normal collapsible handle is 10x10 and has a Gold fill and Black lines.
		/// </summary>
		public GoCollapsibleHandle()
		{
			base.InternalFlags |= 1048576;
			Corner = new SizeF(0f, 0f);
			base.Size = new SizeF(10f, 10f);
			Brush = GoShape.Brushes_Gold;
			Pen = GoShape.Pens_Black;
			Selectable = false;
			Resizable = false;
			AutoRescales = false;
		}

		/// <summary>
		/// Draw a plus/minus/zero inside a rectangle.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoCollapsibleHandle.Bordered" /> is true, this draws a rounded rectangle.
		/// Then it calls <see cref="M:Northwoods.Go.GoCollapsibleHandle.PaintHandle(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			RectangleF bounds = Bounds;
			SizeF corner = Corner;
			if (Bordered)
			{
				Pen pen = GoShape.NewPen(GoShape.GetPenColor(Pen, Color.Black), 1f);
				GoShape.DrawRoundedRectangle(g, view, pen, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height, corner.Width, corner.Height);
				pen.Dispose();
			}
			else
			{
				GoShape.DrawRoundedRectangle(g, view, null, Brush, bounds.X, bounds.Y, bounds.Width, bounds.Height, corner.Width, corner.Height);
			}
			PaintHandle(g, view);
		}

		/// <summary>
		/// The path is the same as a regular rectangle if <see cref="P:Northwoods.Go.GoCollapsibleHandle.Bordered" />,
		/// but augmented with the shape(s) determined by the <see cref="P:Northwoods.Go.GoCollapsibleHandle.Style" />.
		/// </summary>
		/// <returns>a <c>GraphicsPath</c></returns>
		public override GraphicsPath MakePath()
		{
			GraphicsPath graphicsPath = (!Bordered) ? new GraphicsPath(FillMode.Winding) : base.MakePath();
			IGoCollapsible goCollapsible = FindCollapsible();
			if (goCollapsible != null)
			{
				RectangleF bounds = Bounds;
				if (goCollapsible.Collapsible)
				{
					float num = bounds.X + bounds.Width / 2f;
					float num2 = bounds.Y + bounds.Height / 2f;
					switch (Style)
					{
					case GoCollapsibleHandleStyle.PlusMinus:
					{
						float num9 = Bordered ? (bounds.Width / 4f) : 0f;
						float num10 = Bordered ? (bounds.Height / 4f) : 0f;
						graphicsPath.StartFigure();
						graphicsPath.AddLine(bounds.X + num9, num2, bounds.X + bounds.Width - num9, num2);
						if (!goCollapsible.IsExpanded)
						{
							graphicsPath.StartFigure();
							graphicsPath.AddLine(num, bounds.Y + num9, num, bounds.Y + bounds.Height - num10);
						}
						break;
					}
					case GoCollapsibleHandleStyle.TriangleRight:
					{
						float num7 = Bordered ? (bounds.Width / 6f) : 0f;
						float num8 = Bordered ? (bounds.Height / 6f) : 0f;
						PointF[] array3 = new PointF[3];
						if (goCollapsible.IsExpanded)
						{
							array3[0].X = bounds.X + num7;
							array3[0].Y = bounds.Y + num8;
							array3[1].X = bounds.X + bounds.Width - num7;
							array3[1].Y = num2;
							array3[2].X = bounds.X + num7;
							array3[2].Y = bounds.Y + bounds.Height - num8;
						}
						else
						{
							array3[0].X = bounds.X + num7;
							array3[0].Y = bounds.Y + num8;
							array3[1].X = bounds.X + bounds.Width - num7;
							array3[1].Y = bounds.Y + num8;
							array3[2].X = num;
							array3[2].Y = bounds.Y + bounds.Height - num8;
						}
						graphicsPath.StartFigure();
						graphicsPath.AddPolygon(array3);
						break;
					}
					case GoCollapsibleHandleStyle.TriangleUp:
					{
						float num5 = Bordered ? (bounds.Width / 6f) : 0f;
						float num6 = Bordered ? (bounds.Height / 6f) : 0f;
						PointF[] array2 = new PointF[3];
						if (goCollapsible.IsExpanded)
						{
							array2[0].X = bounds.X + num5;
							array2[0].Y = bounds.Y + bounds.Height - num6;
							array2[1].X = bounds.X + bounds.Width - num5;
							array2[1].Y = bounds.Y + bounds.Height - num6;
							array2[2].X = num;
							array2[2].Y = bounds.Y + num6;
						}
						else
						{
							array2[0].X = bounds.X + num5;
							array2[0].Y = bounds.Y + num6;
							array2[1].X = bounds.X + bounds.Width - num5;
							array2[1].Y = bounds.Y + num6;
							array2[2].X = num;
							array2[2].Y = bounds.Y + bounds.Height - num6;
						}
						graphicsPath.StartFigure();
						graphicsPath.AddPolygon(array2);
						break;
					}
					case GoCollapsibleHandleStyle.ChevronUp:
					{
						float num3 = Bordered ? (bounds.Width / 6f) : 0f;
						float num4 = Bordered ? (bounds.Height / 6f) : 0f;
						PointF[] array = new PointF[3];
						if (goCollapsible.IsExpanded)
						{
							array[0].X = bounds.X + num3;
							array[0].Y = num2;
							array[1].X = num;
							array[1].Y = bounds.Y + num4;
							array[2].X = bounds.X + bounds.Width - num3;
							array[2].Y = num2;
							graphicsPath.StartFigure();
							graphicsPath.AddLines(array);
							array[0].X = bounds.X + num3;
							array[0].Y = bounds.Y + bounds.Height - num4;
							array[1].X = num;
							array[1].Y = num2;
							array[2].X = bounds.X + bounds.Width - num3;
							array[2].Y = bounds.Y + bounds.Height - num4;
							graphicsPath.StartFigure();
							graphicsPath.AddLines(array);
						}
						else
						{
							array[0].X = bounds.X + num3;
							array[0].Y = bounds.Y + num4;
							array[1].X = num;
							array[1].Y = num2;
							array[2].X = bounds.X + bounds.Width - num3;
							array[2].Y = bounds.Y + num4;
							graphicsPath.StartFigure();
							graphicsPath.AddLines(array);
							array[0].X = bounds.X + num3;
							array[0].Y = num2;
							array[1].X = num;
							array[1].Y = bounds.Y + bounds.Height - num4;
							array[2].X = bounds.X + bounds.Width - num3;
							array[2].Y = num2;
							graphicsPath.StartFigure();
							graphicsPath.AddLines(array);
						}
						break;
					}
					}
				}
				else
				{
					graphicsPath.AddEllipse(bounds.X + bounds.Width / 4f, bounds.Y + bounds.Height / 4f, bounds.Width / 2f, bounds.Height / 2f);
				}
			}
			return graphicsPath;
		}

		/// <summary>
		/// The appearance of this handle depends on the <see cref="P:Northwoods.Go.IGoCollapsible.Collapsible" />
		/// and <see cref="P:Northwoods.Go.IGoCollapsible.IsExpanded" /> properties.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// If the parent is not <see cref="P:Northwoods.Go.IGoCollapsible.Collapsible" />,
		/// this draws a circle inside the rectangle.
		/// Otherwise, the appearance depends on the value of <see cref="P:Northwoods.Go.GoCollapsibleHandle.Style" />.
		/// </remarks>
		protected virtual void PaintHandle(Graphics g, GoView view)
		{
			IGoCollapsible goCollapsible = FindCollapsible();
			if (goCollapsible == null)
			{
				return;
			}
			RectangleF bounds = Bounds;
			if (goCollapsible.Collapsible)
			{
				float num = bounds.X + bounds.Width / 2f;
				float num2 = bounds.Y + bounds.Height / 2f;
				switch (Style)
				{
				case GoCollapsibleHandleStyle.PlusMinus:
				{
					float num9 = Bordered ? (bounds.Width / 4f) : 0f;
					float num10 = Bordered ? (bounds.Height / 4f) : 0f;
					GoShape.DrawLine(g, view, Pen, bounds.X + num9, num2, bounds.X + bounds.Width - num9, num2);
					if (!goCollapsible.IsExpanded)
					{
						GoShape.DrawLine(g, view, Pen, num, bounds.Y + num10, num, bounds.Y + bounds.Height - num10);
					}
					break;
				}
				case GoCollapsibleHandleStyle.TriangleRight:
				{
					float num7 = Bordered ? (bounds.Width / 6f) : 0f;
					float num8 = Bordered ? (bounds.Height / 6f) : 0f;
					PointF[] array3 = view.AllocTempPointArray(3);
					if (goCollapsible.IsExpanded)
					{
						array3[0].X = bounds.X + num7;
						array3[0].Y = bounds.Y + num8;
						array3[1].X = bounds.X + bounds.Width - num7;
						array3[1].Y = num2;
						array3[2].X = bounds.X + num7;
						array3[2].Y = bounds.Y + bounds.Height - num8;
					}
					else
					{
						array3[0].X = bounds.X + num7;
						array3[0].Y = bounds.Y + num8;
						array3[1].X = bounds.X + bounds.Width - num7;
						array3[1].Y = bounds.Y + num8;
						array3[2].X = num;
						array3[2].Y = bounds.Y + bounds.Height - num8;
					}
					SolidBrush solidBrush2 = new SolidBrush(GoShape.GetPenColor(Pen, Color.Black));
					GoShape.DrawPolygon(g, view, null, solidBrush2, array3);
					solidBrush2.Dispose();
					view.FreeTempPointArray(array3);
					break;
				}
				case GoCollapsibleHandleStyle.TriangleUp:
				{
					float num5 = Bordered ? (bounds.Width / 6f) : 0f;
					float num6 = Bordered ? (bounds.Height / 6f) : 0f;
					PointF[] array2 = view.AllocTempPointArray(3);
					if (goCollapsible.IsExpanded)
					{
						array2[0].X = bounds.X + num5;
						array2[0].Y = bounds.Y + bounds.Height - num6;
						array2[1].X = bounds.X + bounds.Width - num5;
						array2[1].Y = bounds.Y + bounds.Height - num6;
						array2[2].X = num;
						array2[2].Y = bounds.Y + num6;
					}
					else
					{
						array2[0].X = bounds.X + num5;
						array2[0].Y = bounds.Y + num6;
						array2[1].X = bounds.X + bounds.Width - num5;
						array2[1].Y = bounds.Y + num6;
						array2[2].X = num;
						array2[2].Y = bounds.Y + bounds.Height - num6;
					}
					SolidBrush solidBrush = new SolidBrush(GoShape.GetPenColor(Pen, Color.Black));
					GoShape.DrawPolygon(g, view, null, solidBrush, array2);
					solidBrush.Dispose();
					view.FreeTempPointArray(array2);
					break;
				}
				case GoCollapsibleHandleStyle.ChevronUp:
				{
					float num3 = Bordered ? (bounds.Width / 6f) : 0f;
					float num4 = Bordered ? (bounds.Height / 6f) : 0f;
					PointF[] array = view.AllocTempPointArray(3);
					if (goCollapsible.IsExpanded)
					{
						array[0].X = bounds.X + num3;
						array[0].Y = num2;
						array[1].X = num;
						array[1].Y = bounds.Y + num4;
						array[2].X = bounds.X + bounds.Width - num3;
						array[2].Y = num2;
						GoShape.DrawLines(g, view, Pen, array);
						array[0].X = bounds.X + num3;
						array[0].Y = bounds.Y + bounds.Height - num4;
						array[1].X = num;
						array[1].Y = num2;
						array[2].X = bounds.X + bounds.Width - num3;
						array[2].Y = bounds.Y + bounds.Height - num4;
						GoShape.DrawLines(g, view, Pen, array);
					}
					else
					{
						array[0].X = bounds.X + num3;
						array[0].Y = bounds.Y + num4;
						array[1].X = num;
						array[1].Y = num2;
						array[2].X = bounds.X + bounds.Width - num3;
						array[2].Y = bounds.Y + num4;
						GoShape.DrawLines(g, view, Pen, array);
						array[0].X = bounds.X + num3;
						array[0].Y = num2;
						array[1].X = num;
						array[1].Y = bounds.Y + bounds.Height - num4;
						array[2].X = bounds.X + bounds.Width - num3;
						array[2].Y = num2;
						GoShape.DrawLines(g, view, Pen, array);
					}
					view.FreeTempPointArray(array);
					break;
				}
				}
			}
			else
			{
				GoShape.DrawEllipse(g, view, Pen, null, bounds.X + bounds.Width / 4f, bounds.Y + bounds.Height / 4f, bounds.Width / 2f, bounds.Height / 2f);
			}
		}

		/// <summary>
		/// Search for a Parent object that implements <see cref="T:Northwoods.Go.IGoCollapsible" />.
		/// </summary>
		/// <returns>an <see cref="T:Northwoods.Go.IGoCollapsible" />, or null if none was found</returns>
		/// <remarks>
		/// Starting with this handle, this method searches the <c>Parent</c> chain for
		/// any <c>GoObject</c> that implements <see cref="T:Northwoods.Go.IGoCollapsible" />, and returns
		/// the first one that it finds.  Normally this will just be the <see cref="P:Northwoods.Go.GoObject.Parent" />,
		/// but it could be further up the parent chain.
		/// </remarks>
		public virtual IGoCollapsible FindCollapsible()
		{
			GoObject goObject = this;
			while (goObject != null && !(goObject is IGoCollapsible))
			{
				goObject = goObject.Parent;
			}
			return goObject as IGoCollapsible;
		}

		/// <summary>
		/// Implement the single-click behavior for this handle, to toggle the
		/// expansion state of the <see cref="T:Northwoods.Go.IGoCollapsible" /> that this handle is in.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>true if the parent <see cref="P:Northwoods.Go.IGoCollapsible.Collapsible" /> property is true</returns>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.IGoCollapsible.IsExpanded" /> is true, this calls <see cref="M:Northwoods.Go.IGoCollapsible.Collapse" />;
		/// otherwise this calls <see cref="M:Northwoods.Go.IGoCollapsible.Expand" />.
		/// If the view is non-null, this method calls <see cref="M:Northwoods.Go.GoView.StartTransaction" />
		/// and <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />, with a transaction name specified by
		/// the value of <see cref="F:Northwoods.Go.GoUndoManager.CollapsedName" /> or <see cref="F:Northwoods.Go.GoUndoManager.ExpandedName" />.
		/// </remarks>
		public override bool OnSingleClick(GoInputEventArgs evt, GoView view)
		{
			IGoCollapsible goCollapsible = FindCollapsible();
			if (goCollapsible == null)
			{
				return false;
			}
			string tname = null;
			try
			{
				view?.StartTransaction();
				if (goCollapsible.IsExpanded)
				{
					goCollapsible.Collapse();
					tname = "Collapsed";
				}
				else
				{
					goCollapsible.Expand();
					tname = "Expanded";
				}
			}
			finally
			{
				view?.FinishTransaction(tname);
			}
			return true;
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2950:
				Style = (GoCollapsibleHandleStyle)e.GetInt(undo);
				break;
			case 2951:
				Bordered = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
