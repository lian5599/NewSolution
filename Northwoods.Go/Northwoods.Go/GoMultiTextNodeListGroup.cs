using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	[Serializable]
	internal sealed class GoMultiTextNodeListGroup : GoListGroup
	{
		public GoMultiTextNode MTN => (GoMultiTextNode)base.Parent;

		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
			set
			{
			}
		}

		public GoMultiTextNodeListGroup()
		{
			PickableBackground = true;
		}

		public override float LayoutItem(int i, RectangleF cell)
		{
			if (MTN != null && MTN.ItemWidth > 0f)
			{
				float itemWidth = MTN.ItemWidth;
				float num = cell.Y;
				GoObject goObject = this[i];
				if (goObject != null)
				{
					if (goObject.Visible)
					{
						goObject.Bounds = new RectangleF(cell.X, cell.Y, itemWidth, goObject.Height);
						num += goObject.Height;
					}
					else
					{
						goObject.Position = new PointF(cell.X, cell.Y);
					}
				}
				return num;
			}
			return base.LayoutItem(i, cell);
		}

		public override void Changed(int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (!base.SuspendsUpdates)
			{
				base.Changed(subhint, oldI, oldVal, oldRect, newI, newVal, newRect);
				switch (subhint)
				{
				case 1051:
					base.Parent?.LayoutChildren(null);
					break;
				case 1052:
					(base.Parent as GoMultiTextNode)?.RemoveOnlyPorts(oldI);
					break;
				}
			}
		}
	}
}
