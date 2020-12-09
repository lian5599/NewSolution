using System;
using System.Drawing;

namespace Northwoods.Go
{
	[Serializable]
	internal class GoDragImage : GoImage
	{
		private SizeF myOffset;

		public override PointF Location
		{
			get
			{
				return new PointF(base.Left + Offset.Width, base.Top + Offset.Height);
			}
			set
			{
				base.Position = new PointF(value.X - Offset.Width, value.Y - Offset.Height);
			}
		}

		public SizeF Offset
		{
			get
			{
				return myOffset;
			}
			set
			{
				myOffset = value;
			}
		}
	}
}
