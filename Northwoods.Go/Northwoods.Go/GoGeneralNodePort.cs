using System;
using System.Drawing;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A <see cref="T:Northwoods.Go.GoPort" /> that is part of a <see cref="T:Northwoods.Go.GoGeneralNode" />.
	/// </summary>
	/// <remarks>
	/// This is normally created by <see cref="T:Northwoods.Go.GoGeneralNode" />.<see cref="M:Northwoods.Go.GoGeneralNode.CreatePort(System.Boolean)" />.
	/// </remarks>
	[Serializable]
	public class GoGeneralNodePort : GoPort
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNodePort.Name" /> property.
		/// </summary>
		public const int ChangedName = 2430;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> property.
		/// </summary>
		public const int ChangedLabel = 2431;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNodePort.SideIndex" /> property.
		/// </summary>
		public const int ChangedSideIndex = 2432;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNodePort.LeftSide" /> property.
		/// </summary>
		public const int ChangedLeftSide = 2433;

		private bool myLeftSide = true;

		private int mySideIndex = -1;

		private string myName = "";

		private GoGeneralNodePortLabel myPortLabel;

		/// <summary>
		/// Gets or sets which side of the <see cref="T:Northwoods.Go.GoGeneralNode" /> this port should be on.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This should only be set before it is actually added to the node.
		/// If <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <b>Vertical</b>,
		/// this actually refers to the "top" side, not the "left" side.
		/// This property is also set by <see cref="M:Northwoods.Go.GoGeneralNode.InsertLeftPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />
		/// and <see cref="M:Northwoods.Go.GoGeneralNode.InsertRightPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.
		/// </para>
		/// <para>
		/// Changing this value will automatically set the <see cref="P:Northwoods.Go.GoPort.IsValidFrom" />
		/// and <see cref="P:Northwoods.Go.GoPort.IsValidTo" /> properties in the standard manner.
		/// </para>
		/// </remarks>
		public virtual bool LeftSide
		{
			get
			{
				return myLeftSide;
			}
			set
			{
				bool flag = myLeftSide;
				if (flag != value)
				{
					myLeftSide = value;
					Changed(2433, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						IsValidFrom = !value;
						IsValidTo = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the index of this port in the <see cref="T:Northwoods.Go.GoGeneralNode" /> list of ports on this side.
		/// </summary>
		/// <remarks>
		/// This should only be set by <see cref="T:Northwoods.Go.GoGeneralNode" /> code.
		/// </remarks>
		public virtual int SideIndex
		{
			get
			{
				return mySideIndex;
			}
			set
			{
				int num = mySideIndex;
				if (num != value)
				{
					mySideIndex = value;
					Changed(2432, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the name for this port, which is normally reflected by the label, if any.
		/// </summary>
		/// <remarks>
		/// This property is implemented separately from the <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" />
		/// to allow the port to be "named" even if there is no <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" />.
		/// </remarks>
		public virtual string Name
		{
			get
			{
				return myName;
			}
			set
			{
				string text = myName;
				if (!(text != value))
				{
					return;
				}
				myName = value;
				Changed(2430, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					if (Label != null)
					{
						Label.Text = value;
					}
					LinksOnPortChanged(2430, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the text object displays the name of this port.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the label.
		/// If non-null, the text object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the label after creating a port, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoGeneralNode.CreatePortLabel(System.Boolean)" /> method.
		/// </remarks>
		public virtual GoGeneralNodePortLabel Label
		{
			get
			{
				return myPortLabel;
			}
			set
			{
				GoGeneralNodePortLabel goGeneralNodePortLabel = myPortLabel;
				if (goGeneralNodePortLabel == value)
				{
					return;
				}
				if (goGeneralNodePortLabel != null)
				{
					goGeneralNodePortLabel.Port = null;
					if (goGeneralNodePortLabel.Parent != null)
					{
						goGeneralNodePortLabel.Parent.Remove(goGeneralNodePortLabel);
					}
				}
				myPortLabel = value;
				if (value != null)
				{
					value.Port = this;
					if (base.Parent != null)
					{
						base.Parent.Add(value);
					}
				}
				Changed(2431, 0, goGeneralNodePortLabel, GoObject.NullRect, 0, value, GoObject.NullRect);
				if (!base.Initializing)
				{
					LayoutLabel();
				}
			}
		}

		/// <summary>
		/// Return whether the <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> should go on the inside or
		/// on the outside of this port, relative to the <see cref="T:System.Drawing.Icon" />.
		/// </summary>
		/// <value>
		/// This defaults to the value of the parent <see cref="T:Northwoods.Go.GoGeneralNode" />'s
		/// <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortLabelsInside" /> or
		/// <see cref="P:Northwoods.Go.GoGeneralNode.RightPortLabelsInside" /> property.
		/// If there is no such parent, this returns false.
		/// </value>
		/// <remarks>
		/// You can override this property to customize the label positioning for individual ports.
		/// </remarks>
		public virtual bool LabelInside
		{
			get
			{
				GoGeneralNode goGeneralNode = base.Parent as GoGeneralNode;
				if (goGeneralNode != null)
				{
					if (LeftSide)
					{
						return goGeneralNode.LeftPortLabelsInside;
					}
					return goGeneralNode.RightPortLabelsInside;
				}
				return false;
			}
		}

		/// <summary>
		/// Return the desired distance between the port label and the port itself 
		/// </summary>
		/// <value>
		/// This defaults to the value of the parent <see cref="T:Northwoods.Go.GoGeneralNode" />'s
		/// <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsLabelSpacing" /> or
		/// <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsLabelSpacing" /> property.
		/// If there is no such parent, this returns 2.
		/// </value>
		/// <remarks>
		/// You can override this property to customize the spacing for individual ports.
		/// </remarks>
		public virtual float LabelSpacing
		{
			get
			{
				GoGeneralNode goGeneralNode = base.Parent as GoGeneralNode;
				if (goGeneralNode != null)
				{
					if (LeftSide)
					{
						return goGeneralNode.LeftPortsLabelSpacing;
					}
					return goGeneralNode.RightPortsLabelSpacing;
				}
				return 2f;
			}
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoGeneralNode.LayoutChildren(Northwoods.Go.GoObject)" /> and
		/// related methods to determine how wide this port is.
		/// </summary>
		public virtual float PortAndLabelWidth
		{
			get
			{
				if (!Visible)
				{
					return 0f;
				}
				GoGeneralNodePortLabel label = Label;
				GoGeneralNode goGeneralNode = base.Parent as GoGeneralNode;
				if (goGeneralNode != null && goGeneralNode.Orientation == Orientation.Vertical)
				{
					if (label != null && label.Visible)
					{
						return Math.Max(base.Width, label.Width);
					}
					return base.Width;
				}
				if (label != null && label.Visible)
				{
					return base.Width + LabelSpacing + label.Width;
				}
				return base.Width;
			}
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoGeneralNode.LayoutChildren(Northwoods.Go.GoObject)" /> and
		/// related methods to determine how tall this port is.
		/// </summary>
		public virtual float PortAndLabelHeight
		{
			get
			{
				if (!Visible)
				{
					return 0f;
				}
				GoGeneralNodePortLabel label = Label;
				GoGeneralNode goGeneralNode = base.Parent as GoGeneralNode;
				if (goGeneralNode != null && goGeneralNode.Orientation == Orientation.Vertical)
				{
					if (label != null && label.Visible)
					{
						return base.Height + LabelSpacing + label.Height;
					}
					return base.Height;
				}
				if (label != null && label.Visible)
				{
					return Math.Max(base.Height, label.Height);
				}
				return base.Height;
			}
		}

		/// <summary>
		/// Create a GoGeneralNodePort, for use in a <see cref="T:Northwoods.Go.GoGeneralNode" />.
		/// </summary>
		/// <remarks>
		/// The port is normally a medium sized gray triangle with no name.
		/// It does not create a <see cref="T:Northwoods.Go.GoGeneralNodePortLabel" />--
		/// <see cref="T:Northwoods.Go.GoGeneralNode" /> is responsible for creating it
		/// and associating the two with each other.
		/// </remarks>
		public GoGeneralNodePort()
		{
			Style = GoPortStyle.Triangle;
			Pen = GoShape.Pens_Gray;
			Brush = GoShape.Brushes_LightGray;
			base.Size = new SizeF(8f, 8f);
			IsValidFrom = false;
			IsValidTo = true;
		}

		/// <summary>
		/// Copy the port's label too, and make sure its <see cref="P:Northwoods.Go.GoGeneralNodePortLabel.Port" />
		/// property points to the copied port.
		/// </summary>
		/// <param name="env"></param>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoGeneralNodePort goGeneralNodePort = (GoGeneralNodePort)base.CopyObject(env);
			if (goGeneralNodePort != null && myPortLabel != null)
			{
				goGeneralNodePort.myPortLabel = (GoGeneralNodePortLabel)env.Copy(myPortLabel);
				if (goGeneralNodePort.myPortLabel != null)
				{
					goGeneralNodePort.myPortLabel.Port = goGeneralNodePort;
				}
			}
			return goGeneralNodePort;
		}

		/// <summary>
		/// Position the port label to the appropriate side of the port,
		/// so that it doesn't overlap the icon or the port itself.
		/// </summary>
		/// <remarks>
		/// This depends on the <see cref="P:Northwoods.Go.GoGeneralNodePort.LabelSpacing" /> property, and
		/// affects the link point for this port.
		/// </remarks>
		public virtual void LayoutLabel()
		{
			GoText label = Label;
			if (label == null)
			{
				return;
			}
			bool labelInside = LabelInside;
			GoGeneralNode goGeneralNode = base.Parent as GoGeneralNode;
			if (goGeneralNode != null && goGeneralNode.Orientation == Orientation.Vertical)
			{
				if (LeftSide)
				{
					label.Alignment = 1;
					PointF spotLocation = GetSpotLocation(labelInside ? 128 : 32);
					spotLocation.Y -= (labelInside ? (0f - LabelSpacing) : LabelSpacing);
					label.SetSpotLocation(labelInside ? 32 : 128, spotLocation);
				}
				else
				{
					label.Alignment = 1;
					PointF spotLocation2 = GetSpotLocation(labelInside ? 32 : 128);
					spotLocation2.Y += (labelInside ? (0f - LabelSpacing) : LabelSpacing);
					label.SetSpotLocation(labelInside ? 128 : 32, spotLocation2);
				}
			}
			else if (LeftSide)
			{
				label.Alignment = (labelInside ? 256 : 64);
				PointF spotLocation3 = GetSpotLocation(labelInside ? 64 : 256);
				spotLocation3.X -= (labelInside ? (0f - LabelSpacing) : LabelSpacing);
				label.SetSpotLocation(labelInside ? 256 : 64, spotLocation3);
			}
			else
			{
				label.Alignment = (labelInside ? 64 : 256);
				PointF spotLocation4 = GetSpotLocation(labelInside ? 256 : 64);
				spotLocation4.X += (labelInside ? (0f - LabelSpacing) : LabelSpacing);
				label.SetSpotLocation(labelInside ? 64 : 256, spotLocation4);
			}
		}

		/// <summary>
		/// Override the calculation of the link point to take into account
		/// the size of any port label, so the link does not overlap the label.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		public override PointF GetToLinkPoint(IGoLink link)
		{
			return GetLinkPoint(ToSpot);
		}

		/// <summary>
		/// Override the calculation of the link point to take into account
		/// the size of any port label, so the link does not overlap the label.
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		public override PointF GetFromLinkPoint(IGoLink link)
		{
			return GetLinkPoint(FromSpot);
		}

		/// <summary>
		/// This is called by <see cref="M:Northwoods.Go.GoGeneralNodePort.GetToLinkPoint(Northwoods.Go.IGoLink)" /> and <see cref="M:Northwoods.Go.GoGeneralNodePort.GetFromLinkPoint(Northwoods.Go.IGoLink)" />
		/// to calculate the appropriate point for a link to connect at, considering
		/// the width of the label and the <see cref="P:Northwoods.Go.GoGeneralNodePort.LabelSpacing" />.
		/// </summary>
		/// <param name="spot"></param>
		/// <returns>the link end point, a <b>PointF</b></returns>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoGeneralNodePort.LabelInside" /> property is true, this just returns
		/// <b>GetSpotLocation(spot)</b>.
		/// Otherwise it computes a point just outside of the label, so that the
		/// link does not cross over the label.
		/// </remarks>
		public virtual PointF GetLinkPoint(int spot)
		{
			if (LabelInside)
			{
				return GetSpotLocation(spot);
			}
			switch (spot)
			{
			case 256:
			{
				RectangleF bounds3 = Bounds;
				PointF result3 = new PointF(bounds3.X, bounds3.Y + bounds3.Height / 2f);
				GoGeneralNodePortLabel label3 = Label;
				if (label3 != null && label3.Visible)
				{
					result3.X -= label3.Width + LabelSpacing;
				}
				return result3;
			}
			case 64:
			{
				RectangleF bounds2 = Bounds;
				PointF result2 = new PointF(bounds2.X + bounds2.Width, bounds2.Y + bounds2.Height / 2f);
				GoGeneralNodePortLabel label2 = Label;
				if (label2 != null && label2.Visible)
				{
					result2.X += label2.Width + LabelSpacing;
				}
				return result2;
			}
			case 32:
			{
				RectangleF bounds4 = Bounds;
				PointF result4 = new PointF(bounds4.X + bounds4.Width / 2f, bounds4.Y);
				GoGeneralNodePortLabel label4 = Label;
				if (label4 != null && label4.Visible)
				{
					result4.Y -= label4.Height + LabelSpacing;
				}
				return result4;
			}
			case 128:
			{
				RectangleF bounds = Bounds;
				PointF result = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height);
				GoGeneralNodePortLabel label = Label;
				if (label != null && label.Visible)
				{
					result.Y += label.Height + LabelSpacing;
				}
				return result;
			}
			default:
				return GetSpotLocation(spot);
			}
		}

		/// <summary>
		/// Show the name of the port, in case the <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> is not present.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public override string GetToolTip(GoView view)
		{
			return Name;
		}

		/// <summary>
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 2430:
				Name = (string)e.GetValue(undo);
				break;
			case 2431:
				Label = (GoGeneralNodePortLabel)e.GetValue(undo);
				break;
			case 2432:
				SideIndex = e.GetInt(undo);
				break;
			case 2433:
				LeftSide = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
