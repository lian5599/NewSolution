using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// A more complicated node that has an icon, optional labels at the top and bottom,
	/// and variable numbers of labeled ports on either side.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Be sure to call the <b>Initialize</b> method to create all of the standard parts
	/// by calling the various <b>Create...</b> methods.  You can override those methods
	/// to customize the appearance or behavior of those parts.
	/// </para>
	/// <para>
	/// The positions of the <see cref="T:Northwoods.Go.GoGeneralNodePort" />s relative to the icon are determined by the
	/// <see cref="M:Northwoods.Go.GoGeneralNode.LayoutLeftPorts" /> and <see cref="M:Northwoods.Go.GoGeneralNode.LayoutRightPorts" /> methods.
	/// If you want to customize how the ports are laid out, you can
	/// override these methods.  For simple customization,
	/// you can set the <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsAlignment" /> and <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsAlignment" />
	/// properties to specify whether the port should go outside, straddle, or go inside
	/// of the icon's edge.
	/// </para>
	/// <para>
	/// The positions of the <see cref="T:Northwoods.Go.GoGeneralNodePortLabel" />s relative to the port are
	/// determined by the <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="M:Northwoods.Go.GoGeneralNodePort.LayoutLabel" /> method.
	/// However, you can set the <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortLabelsInside" />, <see cref="P:Northwoods.Go.GoGeneralNode.RightPortLabelsInside" />,
	/// <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsLabelSpacing" />, and <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsLabelSpacing" /> properties on
	/// the node to customize the default layout for all port labels.
	/// </para>
	/// <para>
	/// If you don't want the ports to have labels at all, override <see cref="M:Northwoods.Go.GoGeneralNode.CreatePortLabel(System.Boolean)" />
	/// to return null/Nothing.
	/// </para>
	/// <para>
	/// If you want ports at the top and at the bottom of the icon,
	/// with optional labels at either left or right sides, set the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" />
	/// to <c>Orientation.Vertical</c>.
	/// "Left" ports will go on top; "right" ports will go underneath the icon.
	/// </para>
	/// <para>
	/// You can add ports at any time by calling <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
	/// and then calling either <see cref="M:Northwoods.Go.GoGeneralNode.Add(Northwoods.Go.GoObject)" /> or <see cref="M:Northwoods.Go.GoGeneralNode.InsertLeftPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />
	/// or <see cref="M:Northwoods.Go.GoGeneralNode.InsertRightPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.  You can remove ports at any time by calling
	/// <see cref="M:Northwoods.Go.GoGroup.Remove(Northwoods.Go.GoObject)" />.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> is normally a <see cref="T:Northwoods.Go.GoNodeIcon" />,
	/// but can be any kind of <see cref="T:Northwoods.Go.GoObject" />.
	/// Setting the <see cref="P:Northwoods.Go.GoNode.Location" />, <see cref="P:Northwoods.Go.GoNode.Resizable" />,
	/// <see cref="P:Northwoods.Go.GoNode.Reshapable" /> and <see cref="P:Northwoods.Go.GoNode.Shadowed" />
	/// properties actually sets the same properties on the
	/// <see cref="P:Northwoods.Go.GoGeneralNode.SelectionObject" />, which is the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoGeneralNode : GoNode, IGoNodeIconConstraint
	{
		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int InsertedPort = 2401;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int RemovedPort = 2402;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint.
		/// </summary>
		public const int ReplacedPort = 2403;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.TopLabel" /> property.
		/// </summary>
		public const int ChangedTopLabel = 2404;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.BottomLabel" /> property.
		/// </summary>
		public const int ChangedBottomLabel = 2405;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> property.
		/// </summary>
		public const int ChangedIcon = 2406;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> property.
		/// </summary>
		public const int ChangedOrientation = 2407;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.FromEndSegmentLengthStep" /> property.
		/// </summary>
		public const int ChangedFromEndSegmentLengthStep = 2408;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.ToEndSegmentLengthStep" /> property.
		/// </summary>
		public const int ChangedToEndSegmentLengthStep = 2409;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsAlignment" /> property.
		/// </summary>
		public const int ChangedLeftPortsAlignment = 2410;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsAlignment" /> property.
		/// </summary>
		public const int ChangedRightPortsAlignment = 2411;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsLabelSpacing" /> property.
		/// </summary>
		public const int ChangedLeftPortsLabelSpacing = 2412;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsLabelSpacing" /> property.
		/// </summary>
		public const int ChangedRightPortsLabelSpacing = 2413;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortLabelsInside" /> property.
		/// </summary>
		public const int ChangedLeftPortLabelsInside = 2414;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoGeneralNode.RightPortLabelsInside" /> property.
		/// </summary>
		public const int ChangedRightPortLabelsInside = 2415;

		private GoText myTopLabel;

		private GoText myBottomLabel;

		private GoObject myIcon;

		private List<GoGeneralNodePort> myLeftPorts = new List<GoGeneralNodePort>();

		private List<GoGeneralNodePort> myRightPorts = new List<GoGeneralNodePort>();

		private Orientation myOrientation;

		private float myFromEndSegmentLengthStep = 8f;

		private float myToEndSegmentLengthStep = 4f;

		private int myLeftPortsAlignment = 8;

		private int myRightPortsAlignment = 2;

		private float myLeftPortsLabelSpacing = 2f;

		private float myRightPortsLabelSpacing = 2f;

		private bool myLeftPortLabelsInside;

		private bool myRightPortLabelsInside;

		/// <summary>
		/// Assume the icon needs to be tall enough to hold all of the
		/// ports on its left and right sides without overlapping them or
		/// extending them out beyond the icon.
		/// </summary>
		/// <remarks>
		/// If the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> is actually a <see cref="T:Northwoods.Go.GoNodeIcon" />,
		/// this uses the maximum of the <see cref="P:Northwoods.Go.GoNodeIcon.MinimumIconSize" />
		/// and the total height (or width if <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" />
		/// is <c>Orientation.Vertical</c>) of all of ports on each side.
		/// However, when the icon's <see cref="P:Northwoods.Go.GoObject.AutoRescales" />
		/// property is false, this does not take into account the total
		/// height/width of the ports.
		/// </remarks>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The minimum size for the icon")]
		public virtual SizeF MinimumIconSize
		{
			get
			{
				checked
				{
					if (Orientation == Orientation.Horizontal)
					{
						float width = 20f;
						float num = 20f;
						GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
						if (goNodeIcon != null)
						{
							width = goNodeIcon.MinimumIconSize.Width;
							num = goNodeIcon.MinimumIconSize.Height;
						}
						if (Icon != null && Icon.AutoRescales)
						{
							int leftPortsCount = LeftPortsCount;
							float num2 = 0f;
							for (int i = 0; i < leftPortsCount; i++)
							{
								GoGeneralNodePort leftPort = GetLeftPort(i);
								if (leftPort != null && leftPort.Visible)
								{
									num2 += leftPort.PortAndLabelHeight;
								}
							}
							num = Math.Max(num, num2);
							leftPortsCount = RightPortsCount;
							num2 = 0f;
							for (int j = 0; j < leftPortsCount; j++)
							{
								GoGeneralNodePort rightPort = GetRightPort(j);
								if (rightPort != null && rightPort.Visible)
								{
									num2 += rightPort.PortAndLabelHeight;
								}
							}
							num = Math.Max(num, num2);
						}
						return new SizeF(width, num);
					}
					float num3 = 20f;
					float height = 20f;
					GoNodeIcon goNodeIcon2 = Icon as GoNodeIcon;
					if (goNodeIcon2 != null)
					{
						num3 = goNodeIcon2.MinimumIconSize.Width;
						height = goNodeIcon2.MinimumIconSize.Height;
					}
					if (Icon != null && Icon.AutoRescales)
					{
						int leftPortsCount2 = LeftPortsCount;
						float num4 = 0f;
						for (int k = 0; k < leftPortsCount2; k++)
						{
							GoGeneralNodePort leftPort2 = GetLeftPort(k);
							if (leftPort2 != null && leftPort2.Visible)
							{
								num4 += leftPort2.PortAndLabelWidth;
							}
						}
						num3 = Math.Max(num3, num4);
						leftPortsCount2 = RightPortsCount;
						num4 = 0f;
						for (int l = 0; l < leftPortsCount2; l++)
						{
							GoGeneralNodePort rightPort2 = GetRightPort(l);
							if (rightPort2 != null && rightPort2.Visible)
							{
								num4 += rightPort2.PortAndLabelWidth;
							}
						}
						num3 = Math.Max(num3, num4);
					}
					return new SizeF(num3, height);
				}
			}
			set
			{
				GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
				if (goNodeIcon != null)
				{
					goNodeIcon.MinimumIconSize = value;
				}
			}
		}

		/// <summary>
		/// Assume a 1000x2000 maximum size for the icon.
		/// </summary>
		/// <value>
		/// The value comes from the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> if it is an instance of <see cref="T:Northwoods.Go.GoNodeIcon" />;
		/// otherwise the value defaults to 1000x2000.
		/// Setting this value sets the <see cref="P:Northwoods.Go.GoNodeIcon.MaximumIconSize" /> property of the
		/// <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> if it is a <see cref="T:Northwoods.Go.GoNodeIcon" />; it is a no-op otherwise.
		/// </value>
		[Category("Appearance")]
		[TypeConverter(typeof(GoSizeFConverter))]
		[Description("The maximum size for the icon")]
		public virtual SizeF MaximumIconSize
		{
			get
			{
				return (Icon as GoNodeIcon)?.MaximumIconSize ?? new SizeF(1000f, 2000f);
			}
			set
			{
				GoNodeIcon goNodeIcon = Icon as GoNodeIcon;
				if (goNodeIcon != null)
				{
					goNodeIcon.MaximumIconSize = value;
				}
			}
		}

		/// <summary>
		/// The selection object, what the user sees as being selected and what the user
		/// actually resizes, is the icon.
		/// </summary>
		public override GoObject SelectionObject
		{
			get
			{
				GoObject icon = Icon;
				if (icon != null)
				{
					return icon;
				}
				return this;
			}
		}

		/// <summary>
		/// Participate in standard textual node searches and editing,
		/// using the bottom label in preference to the top label if both
		/// are present.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public override GoText Label
		{
			get
			{
				if (BottomLabel != null)
				{
					return BottomLabel;
				}
				if (TopLabel != null)
				{
					return TopLabel;
				}
				return null;
			}
			set
			{
				if (BottomLabel != null)
				{
					BottomLabel = value;
				}
				else if (TopLabel != null)
				{
					TopLabel = value;
				}
				else
				{
					BottomLabel = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoObject" />, normally a <see cref="T:Northwoods.Go.GoImage" />,
		/// acting as the central icon for this node.
		/// </summary>
		/// <value>
		/// The new value may be null, to simply remove the icon.
		/// If non-null, the icon object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the icon after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoGeneralNode.CreateIcon(System.Resources.ResourceManager,System.String)" /> method(s).
		/// The new shape will have its Center location,
		/// Selectable, Resizable, Reshapable, ResizesRealtime, and Shadowed
		/// properties copied from the old shape.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoObject Icon
		{
			get
			{
				return myIcon;
			}
			set
			{
				GoObject goObject = myIcon;
				if (goObject != value)
				{
					CopyPropertiesFromSelectionObject(goObject, value);
					if (goObject != null)
					{
						Remove(goObject);
					}
					myIcon = value;
					if (value != null)
					{
						InsertBefore(null, value);
					}
					Changed(2406, 0, goObject, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the icon as a <see cref="T:Northwoods.Go.GoImage" />.
		/// </summary>
		/// <value>
		/// If, in an exceptional case, the icon does not exist or in fact is
		/// not a <see cref="T:Northwoods.Go.GoImage" />, this value is null.
		/// </value>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoImage Image => Icon as GoImage;

		/// <summary>
		/// Gets this node's icon, assuming it is a <see cref="T:Northwoods.Go.GoShape" />,
		/// as it would be if you call <see cref="M:Northwoods.Go.GoGeneralNode.Initialize(System.Resources.ResourceManager,System.String,System.String,System.String,System.Int32,System.Int32)" />
		/// with a null String icon name.
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoShape Shape => Icon as GoShape;

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDrawing" />.<see cref="P:Northwoods.Go.GoDrawing.Figure" />
		/// of the <see cref="P:Northwoods.Go.GoGeneralNode.Shape" /> IF the icon is a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </summary>
		/// <value>
		/// This returns <see cref="T:Northwoods.Go.GoFigure" />.<see cref="F:Northwoods.Go.GoFigure.None" />
		/// in the typical case where the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> is a <see cref="T:Northwoods.Go.GoImage" />.
		/// Setting this property has no effect if the <see cref="P:Northwoods.Go.GoGeneralNode.Shape" /> is not a <see cref="T:Northwoods.Go.GoDrawing" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(GoFigure.None)]
		[Description("The GoFigure defining the shape of the Icon, if it is a GoDrawing")]
		public virtual GoFigure Figure
		{
			get
			{
				return (Shape as GoDrawing)?.Figure ?? GoFigure.None;
			}
			set
			{
				GoDrawing goDrawing = Shape as GoDrawing;
				if (goDrawing != null)
				{
					goDrawing.Figure = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the general orientation of the node and how links connect to it.
		/// </summary>
		/// <value>
		/// This defaults to <c>Orientation.Horizontal</c>
		/// </value>
		[Category("Appearance")]
		[DefaultValue(Orientation.Horizontal)]
		[Description("The general orientation of the node and how links connect to it")]
		public virtual Orientation Orientation
		{
			get
			{
				return myOrientation;
			}
			set
			{
				Orientation orientation = myOrientation;
				if (orientation != value)
				{
					myOrientation = value;
					Changed(2407, (int)orientation, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						OnOrientationChanged(orientation);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the label at the top of the node.
		/// </summary>
		/// <value>
		/// This must be a <see cref="T:Northwoods.Go.GoText" /> object.
		/// The value may be set to null, to remove any existing label.
		/// If non-null, the text object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the label after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoGeneralNode.CreateLabel(System.Boolean,System.String)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoText TopLabel
		{
			get
			{
				return myTopLabel;
			}
			set
			{
				GoText goText = myTopLabel;
				if (goText != value)
				{
					if (goText != null)
					{
						Remove(goText);
					}
					myTopLabel = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2404, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the label at the bottom of the node.
		/// </summary>
		/// <value>
		/// This must be a <see cref="T:Northwoods.Go.GoText" /> object.
		/// The value may be set to null, to remove any existing label.
		/// If non-null, the text object should have the <see cref="P:Northwoods.Go.GoObject.Selectable" />
		/// property set to false.
		/// </value>
		/// <remarks>
		/// Instead of setting the label after creating a node, you may find it
		/// easier to override the <see cref="M:Northwoods.Go.GoGeneralNode.CreateLabel(System.Boolean,System.String)" /> method.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual GoText BottomLabel
		{
			get
			{
				return myBottomLabel;
			}
			set
			{
				GoText goText = myBottomLabel;
				if (goText != value)
				{
					if (goText != null)
					{
						Remove(goText);
					}
					myBottomLabel = value;
					if (value != null)
					{
						Add(value);
					}
					Changed(2405, 0, goText, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the incremental distance at when the ports'
		/// <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" /> is extended by <see cref="T:Northwoods.Go.GoGeneralNodePort" />
		/// to help reduce the amount of overlapping orthogonal links coming out of this node.
		/// </summary>
		/// <value>
		/// This defaults to 8.  Set this to zero to have <see cref="M:Northwoods.Go.GoPort.GetFromEndSegmentLength(Northwoods.Go.IGoLink)" />
		/// just return the value of <see cref="T:Northwoods.Go.GoPort" />.<see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />
		/// for the "from" end of links.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(8f)]
		public virtual float FromEndSegmentLengthStep
		{
			get
			{
				return myFromEndSegmentLengthStep;
			}
			set
			{
				float num = myFromEndSegmentLengthStep;
				if (num != value)
				{
					myFromEndSegmentLengthStep = value;
					Changed(2408, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the incremental distance at when the ports'
		/// <see cref="P:Northwoods.Go.GoPort.EndSegmentLength" /> is extended by <see cref="T:Northwoods.Go.GoGeneralNodePort" />
		/// to help reduce the amount of overlapping orthogonal links going into this node.
		/// </summary>
		/// <value>
		/// This defaults to 4.  Set this to zero to have <see cref="M:Northwoods.Go.GoPort.GetToEndSegmentLength(Northwoods.Go.IGoLink)" />
		/// just return the value of <see cref="T:Northwoods.Go.GoPort" />.<see cref="P:Northwoods.Go.GoPort.EndSegmentLength" />
		/// for the "to" end of links.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(4f)]
		public virtual float ToEndSegmentLengthStep
		{
			get
			{
				return myToEndSegmentLengthStep;
			}
			set
			{
				float num = myToEndSegmentLengthStep;
				if (num != value)
				{
					myToEndSegmentLengthStep = value;
					Changed(2409, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignments of the "left ports" relative to the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// </summary>
		/// <value>
		/// The default value is <b>GoObject.BottomRight</b>.
		/// If the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <b>Horizontal</b>, the right side of the port
		/// is lined up with the left edge of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />;
		/// otherwise for a <b>Vertical</b> orientation, the bottom side of the port
		/// is lined up with the top edge of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(8)]
		[Description("The alignment of the left ports relative to the Icon")]
		public virtual int LeftPortsAlignment
		{
			get
			{
				return myLeftPortsAlignment;
			}
			set
			{
				int num = myLeftPortsAlignment;
				if (num != value)
				{
					myLeftPortsAlignment = value;
					Changed(2410, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignments of the "right ports" relative to the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// </summary>
		/// <value>
		/// The default value is <b>GoObject.TopLeft</b>.
		/// If the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <b>Horizontal</b>, the left side of the port
		/// is lined up with the right edge of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />;
		/// otherwise for a <b>Vertical</b> orientation, the top side of the port
		/// is lined up with the bottom edge of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(2)]
		[Description("The alignment of the right ports relative to the Icon")]
		public virtual int RightPortsAlignment
		{
			get
			{
				return myRightPortsAlignment;
			}
			set
			{
				int num = myRightPortsAlignment;
				if (num != value)
				{
					myRightPortsAlignment = value;
					Changed(2411, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> and its <see cref="T:Northwoods.Go.GoGeneralNodePortLabel" />.
		/// </summary>
		/// <value>
		/// The default value is 2.  A negative value will result in overlap of the label and the port.
		/// </value>
		/// <remarks>
		/// You can customize the value for individual ports by overriding the
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="P:Northwoods.Go.GoGeneralNodePort.LabelSpacing" /> property.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(2f)]
		[Description("The distance between a left port and its label")]
		public virtual float LeftPortsLabelSpacing
		{
			get
			{
				return myLeftPortsLabelSpacing;
			}
			set
			{
				float num = myLeftPortsLabelSpacing;
				if (num != value)
				{
					myLeftPortsLabelSpacing = value;
					Changed(2412, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the space between a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> and its <see cref="T:Northwoods.Go.GoGeneralNodePortLabel" />.
		/// </summary>
		/// <value>
		/// The default value is 2.  A negative value will result in overlap of the label and the port.
		/// </value>
		/// <remarks>
		/// You can customize the value for individual ports by overriding the
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="P:Northwoods.Go.GoGeneralNodePort.LabelSpacing" /> property.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(2f)]
		[Description("The distance between a right port and its label")]
		public virtual float RightPortsLabelSpacing
		{
			get
			{
				return myRightPortsLabelSpacing;
			}
			set
			{
				float num = myRightPortsLabelSpacing;
				if (num != value)
				{
					myRightPortsLabelSpacing = value;
					Changed(2413, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the labels for the left ports should go inside the ports
		/// (over the icon) or on the outside.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// Note that when the value is true, long labels or narrow icons may result
		/// in overlapping labels.
		/// </value>
		/// <remarks>
		/// You can customize the value for individual ports by overriding the
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="P:Northwoods.Go.GoGeneralNodePort.LabelInside" /> property.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether left port labels go inside over the icon")]
		public virtual bool LeftPortLabelsInside
		{
			get
			{
				return myLeftPortLabelsInside;
			}
			set
			{
				bool flag = myLeftPortLabelsInside;
				if (flag != value)
				{
					myLeftPortLabelsInside = value;
					Changed(2414, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the labels for the right ports should go inside the ports
		/// (over the icon) or on the outside.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// Note that when the value is true, long labels or narrow icons may result
		/// in overlapping labels.
		/// </value>
		/// <remarks>
		/// You can customize the value for individual ports by overriding the
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="P:Northwoods.Go.GoGeneralNodePort.LabelInside" /> property.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether right port labels go inside over the icon")]
		public virtual bool RightPortLabelsInside
		{
			get
			{
				return myRightPortLabelsInside;
			}
			set
			{
				bool flag = myRightPortLabelsInside;
				if (flag != value)
				{
					myRightPortLabelsInside = value;
					Changed(2415, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					if (!base.Initializing)
					{
						LayoutChildren(null);
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of ports on the left side of this node.
		/// </summary>
		public int LeftPortsCount => myLeftPorts.Count;

		/// <summary>
		/// Gets the number of ports on the right side of this node.
		/// </summary>
		public int RightPortsCount => myRightPorts.Count;

		/// <summary>
		/// Initialize an empty GoGeneralNode to have an icon, some labels, and some ports.
		/// </summary>
		/// <param name="res">
		/// Provides the <c>ResourceManager</c> holding an <c>Image</c> resource named by
		/// <paramref name="iconname" />.  If this parameter is null,
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is used instead.
		/// </param>
		/// <param name="iconname">
		/// <para>
		/// The name of the <c>Image</c> resource in the <c>ResourceManager</c>
		/// given by <paramref name="res" />, or else a file name if no resource manager
		/// can be used (i.e., when both <paramref name="res" /> is null and
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> is null).
		/// </para>
		/// <para>
		/// If the value is an empty string, the <c>Image</c> will be blank;
		/// you can set <see cref="P:Northwoods.Go.GoGeneralNode.Image" />.<see cref="P:Northwoods.Go.GoImage.Name" /> to show or change
		/// the image displayed by the <see cref="T:Northwoods.Go.GoImage" /> that is the <see cref="P:Northwoods.Go.GoGeneralNode.Image" />.
		/// </para>
		/// <para>
		/// If the value is null, the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> is not a <see cref="T:Northwoods.Go.GoImage" />
		/// but a <see cref="T:Northwoods.Go.GoDrawing" />; you can then set the <see cref="P:Northwoods.Go.GoGeneralNode.Figure" />
		/// to change the shape shown as the icon.
		/// </para>
		/// </param>
		/// <param name="top">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoGeneralNode.TopLabel" />.
		/// If this value is null, no label is created at the top of this node.
		/// </param>
		/// <param name="bottom">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoGeneralNode.BottomLabel" />.
		/// If this value is null, no label is created at the bottom of this node.
		/// </param>
		/// <param name="numinports">
		/// The number of ports to create on the left side (or top side) of this node.
		/// </param>
		/// <param name="numoutports">
		/// The number of ports to create on the right side (or bottom side) of this node.
		/// </param>
		public virtual void Initialize(ResourceManager res, string iconname, string top, string bottom, int numinports, int numoutports)
		{
			base.Initializing = true;
			Icon = CreateIcon(res, iconname);
			initializeCommon(top, bottom, numinports, numoutports);
		}

		/// <summary>
		/// Initialize an empty GoGeneralNode to have an icon, some labels, and some ports.
		/// </summary>
		/// <param name="imglist">
		/// Provide the <c>ImageList</c> whose <paramref name="imgindex" /> specifies
		/// the actual image to use for the icon.  If this is null, the
		/// <see cref="P:Northwoods.Go.GoView.ImageList" /> property is used by <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// </param>
		/// <param name="imgindex">
		/// The zero-based index of the <c>Image</c> contained in an <c>ImageList</c>,
		/// given either by <paramref name="imglist" /> or by <see cref="P:Northwoods.Go.GoView.ImageList" />.
		/// </param>
		/// <param name="top">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoGeneralNode.TopLabel" />.
		/// If this value is null, no label is created at the top of this node.
		/// </param>
		/// <param name="bottom">
		/// The initial string value for the <see cref="P:Northwoods.Go.GoGeneralNode.BottomLabel" />.
		/// If this value is null, no label is created at the bottom of this node.
		/// </param>
		/// <param name="numinports">
		/// The number of ports to create on the left side (or top side) of this node.
		/// </param>
		/// <param name="numoutports">
		/// The number of ports to create on the right side (or bottom side) of this node.
		/// </param>
		public virtual void Initialize(ImageList imglist, int imgindex, string top, string bottom, int numinports, int numoutports)
		{
			base.Initializing = true;
			Icon = CreateIcon(imglist, imgindex);
			initializeCommon(top, bottom, numinports, numoutports);
		}

		private void initializeCommon(string top, string bottom, int numinports, int numoutports)
		{
			TopLabel = CreateLabel(top: true, top);
			BottomLabel = CreateLabel(top: false, bottom);
			checked
			{
				for (int i = 0; i < numinports; i++)
				{
					GoGeneralNodePort p = MakePort(input: true);
					AddLeftPort(p);
				}
				for (int j = 0; j < numoutports; j++)
				{
					GoGeneralNodePort p2 = MakePort(input: false);
					AddRightPort(p2);
				}
				base.PropertiesDelegatedToSelectionObject = true;
				base.Initializing = false;
				LayoutChildren(null);
			}
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoImage" /> or a <see cref="T:Northwoods.Go.GoDrawing" /> to act as the node's icon.
		/// </summary>
		/// <param name="res"></param>
		/// <param name="iconname">
		/// a null value causes no <see cref="T:Northwoods.Go.GoNodeIcon" /> to be allocated,
		/// but instead a <see cref="T:Northwoods.Go.GoDrawing" /> initialized to look like a rectangle.
		/// </param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoNodeIcon" /> that obeys this node's <see cref="P:Northwoods.Go.GoGeneralNode.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoGeneralNode.MaximumIconSize" /> properties
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(ResourceManager res, String iconname) {
		///    if (iconname != null) {
		///      GoNodeIcon ni = new GoNodeIcon();
		///      if (res != null)
		///        ni.ResourceManager = res;
		///      ni.Name = iconname;
		///      ni.MinimumIconSize = new SizeF(20, 20);
		///      ni.MaximumIconSize = new SizeF(1000, 2000);
		///      ni.Size = ni.MinimumIconSize;
		///      return ni;
		///    } else {
		///      GoDrawing rect = new GoDrawing(GoFigure.Rectangle);
		///      rect.Selectable = false;
		///      rect.Resizable = false;
		///      rect.Size = new SizeF(20, 20);
		///      return rect;
		///    }
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ResourceManager res, string iconname)
		{
			if (iconname != null)
			{
				GoNodeIcon goNodeIcon = new GoNodeIcon();
				if (res != null)
				{
					goNodeIcon.ResourceManager = res;
				}
				goNodeIcon.Name = iconname;
				goNodeIcon.MinimumIconSize = new SizeF(20f, 20f);
				goNodeIcon.MaximumIconSize = new SizeF(1000f, 2000f);
				goNodeIcon.Size = goNodeIcon.MinimumIconSize;
				return goNodeIcon;
			}
			return new GoDrawing(GoFigure.Rectangle)
			{
				Selectable = false,
				Resizable = false,
				Size = new SizeF(20f, 20f)
			};
		}

		/// <summary>
		/// Create and initialize a <see cref="T:Northwoods.Go.GoImage" /> to act as the node's icon.
		/// </summary>
		/// <param name="imglist"></param>
		/// <param name="imgindex"></param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoNodeIcon" /> that obeys this node's <see cref="P:Northwoods.Go.GoGeneralNode.MinimumIconSize" />
		/// and <see cref="P:Northwoods.Go.GoGeneralNode.MaximumIconSize" /> properties
		/// </returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoObject CreateIcon(System.Windows.Forms.ImageList imglist, int imgindex) {
		///    GoNodeIcon ni = new GoNodeIcon();
		///    ni.ImageList = imglist;
		///    ni.Index = imgindex;
		///    ni.MinimumIconSize = new SizeF(20, 20);
		///    ni.MaximumIconSize = new SizeF(1000, 2000);
		///    ni.Size = ni.MinimumIconSize;
		///    return ni;
		///  }
		/// </code>
		/// </example>
		protected virtual GoObject CreateIcon(ImageList imglist, int imgindex)
		{
			GoNodeIcon obj = new GoNodeIcon
			{
				ImageList = imglist,
				Index = imgindex,
				MinimumIconSize = new SizeF(20f, 20f),
				MaximumIconSize = new SizeF(1000f, 2000f)
			};
			obj.Size = obj.MinimumIconSize;
			return obj;
		}

		/// <summary>
		/// Create and initialize a text label for either the top or the bottom.
		/// </summary>
		/// <param name="top"></param>
		/// <param name="text">a null value causes no label to be allocated</param>
		/// <returns>an editable, non-selectable, middle-aligned, non-rescaling <see cref="T:Northwoods.Go.GoText" /> object</returns>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoText CreateLabel(bool top, String text) {
		///    GoText l = null;
		///    if (text != null) {
		///      l = new GoText();
		///      l.Text = text;
		///      l.Selectable = false;
		///      if (this.Orientation == Orientation.Vertical) {
		///        if (top)
		///          l.Alignment = MiddleRight;
		///        else
		///          l.Alignment = MiddleLeft;
		///      } else {
		///        if (top)
		///          l.Alignment = MiddleBottom;
		///        else
		///          l.Alignment = MiddleTop;
		///      }
		///      l.Editable = true;
		///      this.Editable = true;
		///    }
		///    return l;
		///  }
		/// </code>
		/// </example>
		protected virtual GoText CreateLabel(bool top, string text)
		{
			GoText goText = null;
			if (text != null)
			{
				goText = new GoText();
				goText.Text = text;
				goText.Selectable = false;
				if (Orientation == Orientation.Vertical)
				{
					if (top)
					{
						goText.Alignment = 64;
					}
					else
					{
						goText.Alignment = 256;
					}
				}
				else if (top)
				{
					goText.Alignment = 128;
				}
				else
				{
					goText.Alignment = 32;
				}
				goText.Editable = true;
				Editable = true;
			}
			return goText;
		}

		/// <summary>
		/// Create and initialize a port that may go on either the left side or the right side.
		/// </summary>
		/// <param name="input">true to mean the left side or the top side</param>
		/// <returns>a <see cref="T:Northwoods.Go.GoGeneralNodePort" /></returns>
		/// <remarks>
		/// This method is called by <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" /> to create a port,
		/// which it then associates with a port label.
		/// If the orientation is <b>Vertical</b>, the port will be either
		/// on the top side or on the bottom side of the node.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoGeneralNodePort CreatePort(bool input) {
		///    GoGeneralNodePort p = new GoGeneralNodePort();
		///    p.LeftSide = input;
		///    p.IsValidFrom = !input;
		///    p.IsValidTo = input;
		///    return p;
		///  }
		/// </code>
		/// </example>
		protected virtual GoGeneralNodePort CreatePort(bool input)
		{
			return new GoGeneralNodePort
			{
				LeftSide = input,
				IsValidFrom = !input,
				IsValidTo = input
			};
		}

		/// <summary>
		/// Create a label for a port.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>a <see cref="T:Northwoods.Go.GoGeneralNodePortLabel" /></returns>
		/// <remarks>
		/// This method is called by <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" /> to create a
		/// label for a port.
		/// </remarks>
		/// <example>
		/// If you override this method, you may want the definition to do
		/// some of the things that the standard definition does:
		/// <code>
		///  protected virtual GoGeneralNodePortLabel CreatePortLabel(bool input) {
		///    GoGeneralNodePortLabel l = new GoGeneralNodePortLabel();
		///    return l;
		///  }
		/// </code>
		/// If you don't want any port labels, you can override this method to return null.
		/// </example>
		protected virtual GoGeneralNodePortLabel CreatePortLabel(bool input)
		{
			return new GoGeneralNodePortLabel();
		}

		/// <summary>
		/// Create and initialize a new port and its label.
		/// </summary>
		/// <param name="input">true to mean the left side or the top side</param>
		/// <returns></returns>
		/// <remarks>
		/// Pass the result of this method to <see cref="M:Northwoods.Go.GoGeneralNode.Add(Northwoods.Go.GoObject)" />,
		/// <see cref="M:Northwoods.Go.GoGeneralNode.InsertLeftPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />, or <see cref="M:Northwoods.Go.GoGeneralNode.InsertRightPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.
		/// </remarks>
		public virtual GoGeneralNodePort MakePort(bool input)
		{
			GoGeneralNodePort goGeneralNodePort = CreatePort(input);
			if (goGeneralNodePort != null)
			{
				GoGeneralNodePortLabel goGeneralNodePortLabel2 = goGeneralNodePort.Label = CreatePortLabel(input);
				if (goGeneralNodePortLabel2 != null)
				{
					goGeneralNodePortLabel2.Port = goGeneralNodePort;
				}
				if (Orientation == Orientation.Vertical)
				{
					goGeneralNodePort.ToSpot = 32;
					goGeneralNodePort.FromSpot = 128;
				}
				else
				{
					goGeneralNodePort.ToSpot = 256;
					goGeneralNodePort.FromSpot = 64;
				}
				if (input)
				{
					goGeneralNodePort.Name = LeftPortsCount.ToString(CultureInfo.CurrentCulture);
				}
				else
				{
					goGeneralNodePort.Name = RightPortsCount.ToString(CultureInfo.CurrentCulture);
				}
				PointF position;
				if (Icon != null)
				{
					position = Icon.Position;
					if (Orientation == Orientation.Vertical)
					{
						if (input)
						{
							position.Y -= goGeneralNodePort.Height;
						}
						else
						{
							position.Y = Icon.Bottom;
						}
					}
					else if (input)
					{
						position.X -= goGeneralNodePort.Width;
					}
					else
					{
						position.X = Icon.Right;
					}
				}
				else
				{
					position = base.Position;
				}
				goGeneralNodePort.Position = position;
				if (goGeneralNodePortLabel2 != null)
				{
					goGeneralNodePortLabel2.Position = position;
				}
			}
			return goGeneralNodePort;
		}

		/// <summary>
		/// Make copies of the icon, label and the left and right ports.
		/// </summary>
		/// <param name="newgroup"></param>
		/// <param name="env"></param>
		protected override void CopyChildren(GoGroup newgroup, GoCopyDictionary env)
		{
			GoGeneralNode goGeneralNode = (GoGeneralNode)newgroup;
			base.CopyChildren(newgroup, env);
			goGeneralNode.myLeftPorts = new List<GoGeneralNodePort>();
			goGeneralNode.myRightPorts = new List<GoGeneralNodePort>();
			goGeneralNode.myIcon = (GoObject)env[myIcon];
			goGeneralNode.myTopLabel = (GoText)env[myTopLabel];
			goGeneralNode.myBottomLabel = (GoText)env[myBottomLabel];
			checked
			{
				for (int i = 0; i < myLeftPorts.Count; i++)
				{
					GoGeneralNodePort goGeneralNodePort = myLeftPorts[i];
					if (goGeneralNodePort != null)
					{
						GoGeneralNodePort goGeneralNodePort2 = (GoGeneralNodePort)env[goGeneralNodePort];
						if (goGeneralNodePort2 != null)
						{
							goGeneralNode.myLeftPorts.Add(goGeneralNodePort2);
							goGeneralNodePort2.SideIndex = goGeneralNode.myLeftPorts.Count - 1;
							goGeneralNodePort2.LeftSide = true;
						}
					}
				}
				for (int j = 0; j < myRightPorts.Count; j++)
				{
					GoGeneralNodePort goGeneralNodePort3 = myRightPorts[j];
					if (goGeneralNodePort3 != null)
					{
						GoGeneralNodePort goGeneralNodePort4 = (GoGeneralNodePort)env[goGeneralNodePort3];
						if (goGeneralNodePort4 != null)
						{
							goGeneralNode.myRightPorts.Add(goGeneralNodePort4);
							goGeneralNodePort4.SideIndex = goGeneralNode.myRightPorts.Count - 1;
							goGeneralNodePort4.LeftSide = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Position the parts of this node.
		/// </summary>
		/// <param name="childchanged"></param>
		/// <remarks>
		/// The icon is the primary part--the node labels are placed just above
		/// and below the icon.
		/// The ports go on their respective sides of the icon, positioned
		/// contiguously and centered vertically.  The ports are always next to
		/// the icon, and their respective labels are expected to be outside,
		/// as determined by <see cref="M:Northwoods.Go.GoGeneralNodePort.LayoutLabel" />.
		/// If there are too many ports to fit alongside the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />,
		/// the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> is stretched, unless the icon's
		/// <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> property is false.
		/// Of course when <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <c>Orientation.Vertical</c>,
		/// all of the left/right and up/down directions are exchanged.
		/// When <see cref="P:Northwoods.Go.GoObject.Initializing" /> is true, this method does nothing.
		/// </remarks>
		public override void LayoutChildren(GoObject childchanged)
		{
			if (!base.Initializing)
			{
				base.Initializing = true;
				LayoutIcon();
				LayoutLeftPorts();
				LayoutRightPorts();
				LayoutLabels();
				base.InvalidBounds = true;
				base.Initializing = false;
			}
		}

		/// <summary>
		/// Resize the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" /> so that it meets the <see cref="P:Northwoods.Go.GoGeneralNode.MinimumIconSize" /> requirement.
		/// </summary>
		/// <remarks>
		/// This only happens when <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true for the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// <see cref="P:Northwoods.Go.GoGeneralNode.MinimumIconSize" /> is computed to be large enough to hold all of the ports.
		/// If you want the Icon to automatically shrink in size when ports are removed,
		/// override this method as follows:
		/// <pre><code>
		///     protected override void LayoutIcon() {
		///       GoObject icon = this.Icon;
		///       if (icon != null &amp;&amp; icon.AutoRescales) {
		///         SizeF minIconSize = this.MinimumIconSize;
		///         float newW = minIconSize.Width;
		///         float newH = minIconSize.Height;
		///         icon.Bounds = new RectangleF(icon.Left - (newW - icon.Width)/2, icon.Top - (newH - icon.Height)/2, newW, newH);
		///       }
		///     }
		/// </code></pre>
		/// </remarks>
		protected virtual void LayoutIcon()
		{
			GoObject icon = Icon;
			if (icon != null && icon.AutoRescales)
			{
				SizeF minimumIconSize = MinimumIconSize;
				float num = Math.Max(minimumIconSize.Width, icon.Width);
				float num2 = Math.Max(minimumIconSize.Height, icon.Height);
				icon.Bounds = new RectangleF(icon.Left - (num - icon.Width) / 2f, icon.Top - (num2 - icon.Height) / 2f, num, num2);
			}
		}

		/// <summary>
		/// Position all of the left ports along the left (or top) edge of the icon.
		/// </summary>
		/// <remarks>
		/// Ports are placed adjacent to each other, centered along the icon's edge.
		/// The positioning depends on the values of
		/// <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsAlignment" />, <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsLabelSpacing" />,
		/// <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortLabelsInside" />, and the result of calling
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="M:Northwoods.Go.GoGeneralNodePort.LayoutLabel" />.
		/// </remarks>
		protected virtual void LayoutLeftPorts()
		{
			GoObject icon = Icon;
			int leftPortsCount = LeftPortsCount;
			if (leftPortsCount <= 0)
			{
				return;
			}
			float num3;
			float num4;
			float num5;
			int num6;
			checked
			{
				if (Orientation == Orientation.Horizontal)
				{
					float num = (TopLabel != null) ? TopLabel.Height : 0f;
					float num2 = 0f;
					for (int i = 0; i < leftPortsCount; i++)
					{
						GoGeneralNodePort leftPort = GetLeftPort(i);
						if (leftPort.Visible)
						{
							num2 += leftPort.PortAndLabelHeight;
						}
					}
					num3 = 0f;
					num4 = 0f;
					if (icon != null)
					{
						num3 = icon.Left;
						num4 = icon.Top + (icon.Height - num2) / 2f;
					}
					else
					{
						num3 = base.Left;
						num4 = base.Top + num;
					}
					num5 = 0f;
					num6 = 64;
					int leftPortsAlignment = LeftPortsAlignment;
					if (leftPortsAlignment <= 16)
					{
						switch (leftPortsAlignment)
						{
						case 1:
							goto IL_011e;
						case 4:
						case 8:
							goto IL_0123;
						}
						goto IL_0115;
					}
					if (leftPortsAlignment <= 64)
					{
						if (leftPortsAlignment != 32)
						{
							if (leftPortsAlignment != 64)
							{
								goto IL_0115;
							}
							goto IL_0123;
						}
					}
					else if (leftPortsAlignment != 128)
					{
						_ = 256;
						goto IL_0115;
					}
					goto IL_011e;
				}
				float num7 = (TopLabel != null) ? TopLabel.Width : 0f;
				float num8 = 0f;
				for (int j = 0; j < leftPortsCount; j++)
				{
					GoGeneralNodePort leftPort2 = GetLeftPort(j);
					if (leftPort2.Visible)
					{
						num8 += leftPort2.PortAndLabelWidth;
					}
				}
				float num9 = 0f;
				float num10 = 0f;
				if (icon != null)
				{
					num9 = icon.Left + (icon.Width - num8) / 2f;
					num10 = icon.Top;
				}
				else
				{
					num9 = base.Left + num7;
					num10 = base.Top;
				}
				float num11 = 0f;
				int num12 = 128;
				switch (LeftPortsAlignment)
				{
				default:
					num12 = 32;
					break;
				case 1:
				case 64:
				case 256:
					num12 = 1;
					break;
				case 8:
				case 16:
				case 128:
					num12 = 128;
					break;
				}
				for (int k = 0; k < leftPortsCount; k++)
				{
					GoGeneralNodePort leftPort3 = GetLeftPort(k);
					if (leftPort3.Visible)
					{
						num11 += leftPort3.PortAndLabelWidth / 2f;
						leftPort3.SetSpotLocation(num12, new PointF(num9 + num11, num10));
						leftPort3.LayoutLabel();
						num11 += leftPort3.PortAndLabelWidth / 2f;
					}
				}
				return;
			}
			IL_0127:
			for (int l = 0; l < leftPortsCount; l = checked(l + 1))
			{
				GoGeneralNodePort leftPort4 = GetLeftPort(l);
				if (leftPort4.Visible)
				{
					num5 += leftPort4.PortAndLabelHeight / 2f;
					leftPort4.SetSpotLocation(num6, new PointF(num3, num4 + num5));
					leftPort4.LayoutLabel();
					num5 += leftPort4.PortAndLabelHeight / 2f;
				}
			}
			return;
			IL_011e:
			num6 = 1;
			goto IL_0127;
			IL_0123:
			num6 = 64;
			goto IL_0127;
			IL_0115:
			num6 = 256;
			goto IL_0127;
		}

		/// <summary>
		/// Position all of the right ports along the right (or bottom) edge of the icon.
		/// </summary>
		/// <remarks>
		/// Ports are placed adjacent to each other, centered along the icon's edge.
		/// The positioning depends on the values of
		/// <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsAlignment" />, <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsLabelSpacing" />,
		/// <see cref="P:Northwoods.Go.GoGeneralNode.RightPortLabelsInside" />, and the result of calling
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="M:Northwoods.Go.GoGeneralNodePort.LayoutLabel" />.
		/// </remarks>
		protected virtual void LayoutRightPorts()
		{
			GoObject icon = Icon;
			int rightPortsCount = RightPortsCount;
			if (rightPortsCount <= 0)
			{
				return;
			}
			float num3;
			float num4;
			float num5;
			int num6;
			checked
			{
				if (Orientation == Orientation.Horizontal)
				{
					float num = (TopLabel != null) ? TopLabel.Height : 0f;
					float num2 = 0f;
					for (int i = 0; i < rightPortsCount; i++)
					{
						GoGeneralNodePort rightPort = GetRightPort(i);
						if (rightPort.Visible)
						{
							num2 += rightPort.PortAndLabelHeight;
						}
					}
					num3 = 0f;
					num4 = 0f;
					if (icon != null)
					{
						num3 = icon.Right;
						num4 = icon.Top + (icon.Height - num2) / 2f;
					}
					else
					{
						num3 = base.Right;
						num4 = base.Top + num;
					}
					num5 = 0f;
					num6 = 256;
					int rightPortsAlignment = RightPortsAlignment;
					if (rightPortsAlignment <= 16)
					{
						switch (rightPortsAlignment)
						{
						case 1:
							goto IL_0121;
						case 4:
						case 8:
							goto IL_0126;
						}
						goto IL_0118;
					}
					if (rightPortsAlignment <= 64)
					{
						if (rightPortsAlignment != 32)
						{
							if (rightPortsAlignment != 64)
							{
								goto IL_0118;
							}
							goto IL_0126;
						}
					}
					else if (rightPortsAlignment != 128)
					{
						_ = 256;
						goto IL_0118;
					}
					goto IL_0121;
				}
				float num7 = (TopLabel != null) ? TopLabel.Width : 0f;
				float num8 = 0f;
				for (int j = 0; j < rightPortsCount; j++)
				{
					GoGeneralNodePort rightPort2 = GetRightPort(j);
					if (rightPort2.Visible)
					{
						num8 += rightPort2.PortAndLabelWidth;
					}
				}
				float num9 = 0f;
				float num10 = 0f;
				if (icon != null)
				{
					num9 = icon.Left + (icon.Width - num8) / 2f;
					num10 = icon.Bottom;
				}
				else
				{
					num9 = base.Left + num7;
					num10 = base.Bottom;
				}
				float num11 = 0f;
				int num12 = 64;
				switch (RightPortsAlignment)
				{
				default:
					num12 = 32;
					break;
				case 1:
				case 64:
				case 256:
					num12 = 1;
					break;
				case 8:
				case 16:
				case 128:
					num12 = 128;
					break;
				}
				for (int k = 0; k < rightPortsCount; k++)
				{
					GoGeneralNodePort rightPort3 = GetRightPort(k);
					if (rightPort3.Visible)
					{
						num11 += rightPort3.PortAndLabelWidth / 2f;
						rightPort3.SetSpotLocation(num12, new PointF(num9 + num11, num10));
						rightPort3.LayoutLabel();
						num11 += rightPort3.PortAndLabelWidth / 2f;
					}
				}
				return;
			}
			IL_012a:
			for (int l = 0; l < rightPortsCount; l = checked(l + 1))
			{
				GoGeneralNodePort rightPort4 = GetRightPort(l);
				if (rightPort4.Visible)
				{
					num5 += rightPort4.PortAndLabelHeight / 2f;
					rightPort4.SetSpotLocation(num6, new PointF(num3, num4 + num5));
					rightPort4.LayoutLabel();
					num5 += rightPort4.PortAndLabelHeight / 2f;
				}
			}
			return;
			IL_0121:
			num6 = 1;
			goto IL_012a;
			IL_0126:
			num6 = 64;
			goto IL_012a;
			IL_0118:
			num6 = 256;
			goto IL_012a;
		}

		/// <summary>
		/// Position the <see cref="P:Northwoods.Go.GoGeneralNode.TopLabel" /> and the <see cref="P:Northwoods.Go.GoGeneralNode.BottomLabel" />
		/// above and below the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />.
		/// </summary>
		/// <remarks>
		/// When the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <b>Vertical</b>,
		/// the <see cref="P:Northwoods.Go.GoGeneralNode.TopLabel" /> will be on the left side of the <see cref="P:Northwoods.Go.GoGeneralNode.Icon" />,
		/// and the <see cref="P:Northwoods.Go.GoGeneralNode.BottomLabel" /> will be on the right side.
		/// </remarks>
		protected virtual void LayoutLabels()
		{
			GoObject icon = Icon;
			GoObject topLabel = TopLabel;
			GoObject bottomLabel = BottomLabel;
			int num;
			int num2;
			if (Orientation == Orientation.Horizontal)
			{
				num = 32;
				num2 = 128;
			}
			else
			{
				num = 256;
				num2 = 64;
			}
			if (topLabel != null)
			{
				if (icon != null)
				{
					topLabel.SetSpotLocation(num2, icon, num);
				}
				else
				{
					topLabel.SetSpotLocation(num, this, num);
				}
			}
			if (bottomLabel != null)
			{
				if (icon != null)
				{
					bottomLabel.SetSpotLocation(num, icon, num2);
				}
				else
				{
					bottomLabel.SetSpotLocation(num2, this, num2);
				}
			}
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		/// <remarks>
		/// LeftPorts and RightPorts have generated names that are of the form:
		/// "Lnnn" or "Rnnn", where "nnn" is the index of that port.
		/// </remarks>
		public override string FindName(GoObject child)
		{
			if (child == TopLabel)
			{
				return "TopLabel";
			}
			if (child == BottomLabel)
			{
				return "BottomLabel";
			}
			if (child == Icon)
			{
				return "Icon";
			}
			GoGeneralNodePort goGeneralNodePort = child as GoGeneralNodePort;
			if (goGeneralNodePort != null)
			{
				int num = myLeftPorts.IndexOf(goGeneralNodePort);
				if (num >= 0)
				{
					return "L" + num.ToString(NumberFormatInfo.InvariantInfo);
				}
				num = myRightPorts.IndexOf(goGeneralNodePort);
				if (num >= 0)
				{
					return "R" + num.ToString(NumberFormatInfo.InvariantInfo);
				}
			}
			return base.FindName(child);
		}

		/// <summary>
		/// The properties referring to parts of this node
		/// are also the names of those parts.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// LeftPorts and RightPorts have generated names that are of the form:
		/// "Lnnn" or "Rnnn", where "nnn" is the index of that port.
		/// </remarks>
		public override GoObject FindChild(string name)
		{
			if (name == "TopLabel")
			{
				return TopLabel;
			}
			if (name == "BottomLabel")
			{
				return BottomLabel;
			}
			if (name == "Icon")
			{
				return Icon;
			}
			if (name.Length >= 2)
			{
				if (name[0] == 'L')
				{
					int num = -1;
					try
					{
						num = int.Parse(name.Substring(1), NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
					if (num >= 0 && num < myLeftPorts.Count)
					{
						return myLeftPorts[num];
					}
				}
				else if (name[0] == 'R')
				{
					int num2 = -1;
					try
					{
						num2 = int.Parse(name.Substring(1), NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
					if (num2 >= 0 && num2 < myRightPorts.Count)
					{
						return myRightPorts[num2];
					}
				}
			}
			return base.FindChild(name);
		}

		/// <summary>
		/// Determine how to change the whole node when the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> changes.
		/// </summary>
		/// <param name="old">the former <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> value</param>
		/// <remarks>
		/// By default, changing the <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> property will reposition
		/// all of the ports appropriately and change the ports' <see cref="P:Northwoods.Go.GoPort.ToSpot" />
		/// and <see cref="P:Northwoods.Go.GoPort.FromSpot" /> properties.
		/// When <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <c>Orientation.Horizontal</c>, "left" ports
		/// are in fact on the left side, "right" ports are on the right side, source links
		/// come into the left ports on the left side, and destination links go out of the
		/// right ports from the right side.
		/// When <see cref="P:Northwoods.Go.GoGeneralNode.Orientation" /> is <c>Orientation.Vertical</c>, "left" ports are
		/// on top of the node, "right" ports are on the bottom, and links come in from the
		/// top and go out from the bottom.
		/// </remarks>
		public virtual void OnOrientationChanged(Orientation old)
		{
			int leftPortsCount = LeftPortsCount;
			checked
			{
				for (int i = 0; i < leftPortsCount; i++)
				{
					GoGeneralNodePort leftPort = GetLeftPort(i);
					if (Orientation == Orientation.Vertical)
					{
						leftPort.ToSpot = 32;
						leftPort.FromSpot = 128;
					}
					else
					{
						leftPort.ToSpot = 256;
						leftPort.FromSpot = 64;
					}
				}
				leftPortsCount = RightPortsCount;
				for (int j = 0; j < leftPortsCount; j++)
				{
					GoGeneralNodePort rightPort = GetRightPort(j);
					if (Orientation == Orientation.Vertical)
					{
						rightPort.ToSpot = 32;
						rightPort.FromSpot = 128;
					}
					else
					{
						rightPort.ToSpot = 256;
						rightPort.FromSpot = 64;
					}
				}
				LayoutChildren(null);
			}
		}

		/// <summary>
		/// Return a port by its index position on the left side of this node.
		/// </summary>
		/// <param name="i">a zero-based, non-negative index</param>
		/// <returns>null if <paramref name="i" /> is greater than or equal to <see cref="P:Northwoods.Go.GoGeneralNode.LeftPortsCount" /></returns>
		public virtual GoGeneralNodePort GetLeftPort(int i)
		{
			if (i < 0 || i >= myLeftPorts.Count)
			{
				return null;
			}
			return myLeftPorts[i];
		}

		/// <summary>
		/// Return a port by its index position on the right side of this node.
		/// </summary>
		/// <param name="i">a zero-based, non-negative index</param>
		/// <returns>null if <paramref name="i" /> is greater than or equal to <see cref="P:Northwoods.Go.GoGeneralNode.RightPortsCount" /></returns>
		public virtual GoGeneralNodePort GetRightPort(int i)
		{
			if (i < 0 || i >= myRightPorts.Count)
			{
				return null;
			}
			return myRightPorts[i];
		}

		private void initializePort(GoGeneralNodePort p)
		{
			if (p != null && p.Parent == null)
			{
				base.Add(p);
				if (p.Label != null)
				{
					base.Add(p.Label);
				}
			}
		}

		/// <summary>
		/// If the new child is a <see cref="T:Northwoods.Go.GoGeneralNodePort" />,
		/// and if <see cref="P:Northwoods.Go.GoObject.Initializing" /> is false,
		/// automatically add it (and its <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" />)
		/// to the appropriate side, by calling either <see cref="M:Northwoods.Go.GoGeneralNode.InsertLeftPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />
		/// or <see cref="M:Northwoods.Go.GoGeneralNode.InsertRightPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.
		/// </summary>
		/// <param name="obj"></param>
		public override void Add(GoObject obj)
		{
			if (!base.Initializing)
			{
				GoGeneralNodePort goGeneralNodePort = obj as GoGeneralNodePort;
				if (goGeneralNodePort != null)
				{
					if (goGeneralNodePort.LeftSide)
					{
						InsertLeftPort(LeftPortsCount, goGeneralNodePort);
					}
					else
					{
						InsertRightPort(RightPortsCount, goGeneralNodePort);
					}
					return;
				}
			}
			base.Add(obj);
		}

		/// <summary>
		/// Add a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> created by a call to <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
		/// at the end of the list of ports on the left side of this node.
		/// </summary>
		/// <param name="p"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoGeneralNode.InsertLeftPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.
		/// </remarks>
		public void AddLeftPort(GoGeneralNodePort p)
		{
			InsertLeftPort(LeftPortsCount, p);
		}

		/// <summary>
		/// Add a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> created by a call to <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
		/// at the end of the list of ports on the right side of this node.
		/// </summary>
		/// <param name="p"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoGeneralNode.InsertRightPort(System.Int32,Northwoods.Go.GoGeneralNodePort)" />.
		/// </remarks>
		public void AddRightPort(GoGeneralNodePort p)
		{
			InsertRightPort(RightPortsCount, p);
		}

		/// <summary>
		/// Insert a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> created by a call to <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
		/// to be at a particular index position on the left side of this node.
		/// </summary>
		/// <param name="i">if beyond the number of ports, adds it at the end</param>
		/// <param name="p"></param>
		/// <remarks>
		/// This also adds the port's <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> to the node.
		/// This sets <see cref="P:Northwoods.Go.GoGeneralNodePort.LeftSide" /> to true, and
		/// sets <see cref="P:Northwoods.Go.GoGeneralNodePort.SideIndex" /> to the index <paramref name="i" />.
		/// </remarks>
		public virtual void InsertLeftPort(int i, GoGeneralNodePort p)
		{
			checked
			{
				if (p != null && i >= 0)
				{
					p.LeftSide = true;
					if (i < LeftPortsCount)
					{
						myLeftPorts.Insert(i, p);
						AdjustSideIndices(left: true, i);
					}
					else
					{
						myLeftPorts.Add(p);
						i = LeftPortsCount - 1;
						p.SideIndex = i;
					}
					initializePort(p);
					Changed(2401, -(i + 1), p, GoObject.NullRect, -(i + 1), p, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Insert a <see cref="T:Northwoods.Go.GoGeneralNodePort" /> created by a call to <see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
		/// to be at a particular index position on the right side of this node.
		/// </summary>
		/// <param name="i">if beyond the number of ports, adds it at the end</param>
		/// <param name="p"></param>
		/// <remarks>
		/// This also adds the port's <see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> to the node.
		/// This sets <see cref="P:Northwoods.Go.GoGeneralNodePort.LeftSide" /> to false, and
		/// sets <see cref="P:Northwoods.Go.GoGeneralNodePort.SideIndex" /> to the index <paramref name="i" />.
		/// </remarks>
		public virtual void InsertRightPort(int i, GoGeneralNodePort p)
		{
			if (p != null && i >= 0)
			{
				p.LeftSide = false;
				if (i < RightPortsCount)
				{
					myRightPorts.Insert(i, p);
					AdjustSideIndices(left: false, i);
				}
				else
				{
					myRightPorts.Add(p);
					i = checked(RightPortsCount - 1);
					p.SideIndex = i;
				}
				initializePort(p);
				Changed(2401, i, p, GoObject.NullRect, i, p, GoObject.NullRect);
			}
		}

		private void AdjustSideIndices(bool left, int idx)
		{
			List<GoGeneralNodePort> list = left ? myLeftPorts : myRightPorts;
			for (int i = idx; i < list.Count; i = checked(i + 1))
			{
				GoGeneralNodePort goGeneralNodePort = list[i];
				if (goGeneralNodePort != null)
				{
					goGeneralNodePort.SideIndex = i;
				}
			}
		}

		/// <summary>
		/// When a port is removed, make sure we also remove its label and adjust
		/// the positions of all of the other ports.
		/// </summary>
		/// <param name="obj"></param>
		public override bool Remove(GoObject obj)
		{
			GoGeneralNodePort goGeneralNodePort = obj as GoGeneralNodePort;
			checked
			{
				if (goGeneralNodePort != null)
				{
					int num = myLeftPorts.IndexOf(goGeneralNodePort);
					if (num >= 0)
					{
						myLeftPorts.RemoveAt(num);
						if (goGeneralNodePort.Label != null)
						{
							Remove(goGeneralNodePort.Label);
						}
						base.Remove(goGeneralNodePort);
						Changed(2402, -(num + 1), goGeneralNodePort, GoObject.NullRect, -(num + 1), goGeneralNodePort, GoObject.NullRect);
						AdjustSideIndices(left: true, num);
						return true;
					}
					int num2 = myRightPorts.IndexOf(goGeneralNodePort);
					if (num2 >= 0)
					{
						myRightPorts.RemoveAt(num2);
						if (goGeneralNodePort.Label != null)
						{
							Remove(goGeneralNodePort.Label);
						}
						base.Remove(goGeneralNodePort);
						Changed(2402, num2, goGeneralNodePort, GoObject.NullRect, num2, goGeneralNodePort, GoObject.NullRect);
						AdjustSideIndices(left: false, num2);
						return true;
					}
				}
				bool result = base.Remove(obj);
				if (obj == myTopLabel)
				{
					myTopLabel = null;
					return result;
				}
				if (obj == myBottomLabel)
				{
					myBottomLabel = null;
					return result;
				}
				if (obj == myIcon)
				{
					myIcon = null;
				}
				return result;
			}
		}

		/// <summary>
		/// Remove the port and its label at a particular index position on the
		/// left side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		public virtual void RemoveLeftPort(int i)
		{
			if (i >= 0 && i < LeftPortsCount)
			{
				GoGeneralNodePort obj = myLeftPorts[i];
				Remove(obj);
			}
		}

		/// <summary>
		/// Remove the port and its label at a particular index position on the
		/// right side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		public virtual void RemoveRightPort(int i)
		{
			if (i >= 0 && i < RightPortsCount)
			{
				GoGeneralNodePort obj = myRightPorts[i];
				Remove(obj);
			}
		}

		/// <summary>
		/// Replace the port and its label at a particular index position on the
		/// left side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		/// <param name="p"></param>
		public virtual void SetLeftPort(int i, GoGeneralNodePort p)
		{
			GoGeneralNodePort leftPort = GetLeftPort(i);
			if (leftPort == p)
			{
				return;
			}
			if (leftPort != null)
			{
				if (p != null)
				{
					p.Bounds = leftPort.Bounds;
				}
				if (leftPort.Label != null)
				{
					Remove(leftPort.Label);
				}
				base.Remove(leftPort);
			}
			myLeftPorts[i] = p;
			p.LeftSide = true;
			p.SideIndex = i;
			initializePort(p);
			checked
			{
				Changed(2403, -(i + 1), leftPort, GoObject.NullRect, -(i + 1), p, GoObject.NullRect);
			}
		}

		/// <summary>
		/// Replace the port and its label at a particular index position on the
		/// right side of this node.
		/// </summary>
		/// <param name="i">a zero-based non-negative index</param>
		/// <param name="p"></param>
		public virtual void SetRightPort(int i, GoGeneralNodePort p)
		{
			GoGeneralNodePort rightPort = GetRightPort(i);
			if (rightPort == p)
			{
				return;
			}
			if (rightPort != null)
			{
				if (p != null)
				{
					p.Bounds = rightPort.Bounds;
				}
				if (rightPort.Label != null)
				{
					Remove(rightPort.Label);
				}
				base.Remove(rightPort);
			}
			myRightPorts[i] = p;
			p.LeftSide = false;
			p.SideIndex = i;
			initializePort(p);
			Changed(2403, i, rightPort, GoObject.NullRect, i, p, GoObject.NullRect);
		}

		/// <summary>
		/// Handle this class's property changes for undo and redo
		/// </summary>
		/// <param name="e"></param>
		/// <param name="undo"></param>
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			checked
			{
				switch (e.SubHint)
				{
				case 2401:
				{
					int oldInt2 = e.OldInt;
					if (oldInt2 < 0)
					{
						oldInt2 = -oldInt2 - 1;
						if (undo)
						{
							if (oldInt2 < LeftPortsCount)
							{
								myLeftPorts.RemoveAt(oldInt2);
							}
						}
						else if (oldInt2 < LeftPortsCount)
						{
							myLeftPorts.Insert(oldInt2, (GoGeneralNodePort)e.OldValue);
						}
						else
						{
							myLeftPorts.Add((GoGeneralNodePort)e.OldValue);
						}
					}
					else if (undo)
					{
						if (oldInt2 < RightPortsCount)
						{
							myRightPorts.RemoveAt(oldInt2);
						}
					}
					else if (oldInt2 < RightPortsCount)
					{
						myRightPorts.Insert(oldInt2, (GoGeneralNodePort)e.OldValue);
					}
					else
					{
						myRightPorts.Add((GoGeneralNodePort)e.OldValue);
					}
					break;
				}
				case 2402:
				{
					int oldInt3 = e.OldInt;
					if (oldInt3 < 0)
					{
						oldInt3 = -oldInt3 - 1;
						if (undo)
						{
							if (oldInt3 < LeftPortsCount)
							{
								myLeftPorts.Insert(oldInt3, (GoGeneralNodePort)e.OldValue);
							}
							else
							{
								myLeftPorts.Add((GoGeneralNodePort)e.OldValue);
							}
						}
						else if (oldInt3 < LeftPortsCount)
						{
							myLeftPorts.RemoveAt(oldInt3);
						}
					}
					else if (undo)
					{
						if (oldInt3 < RightPortsCount)
						{
							myRightPorts.Insert(oldInt3, (GoGeneralNodePort)e.OldValue);
						}
						else
						{
							myRightPorts.Add((GoGeneralNodePort)e.OldValue);
						}
					}
					else if (oldInt3 < RightPortsCount)
					{
						myRightPorts.RemoveAt(oldInt3);
					}
					break;
				}
				case 2403:
				{
					int oldInt = e.OldInt;
					if (oldInt < 0)
					{
						oldInt = -oldInt - 1;
						if (oldInt < LeftPortsCount)
						{
							myLeftPorts[oldInt] = (GoGeneralNodePort)e.GetValue(undo);
						}
					}
					else if (oldInt < RightPortsCount)
					{
						myRightPorts[oldInt] = (GoGeneralNodePort)e.GetValue(undo);
					}
					break;
				}
				case 2404:
					TopLabel = (GoText)e.GetValue(undo);
					break;
				case 2405:
					BottomLabel = (GoText)e.GetValue(undo);
					break;
				case 2406:
					Icon = (GoObject)e.GetValue(undo);
					break;
				case 2407:
					Orientation = unchecked((Orientation)e.GetInt(undo));
					break;
				case 2408:
					FromEndSegmentLengthStep = e.GetFloat(undo);
					break;
				case 2409:
					ToEndSegmentLengthStep = e.GetFloat(undo);
					break;
				case 2410:
					LeftPortsAlignment = e.GetInt(undo);
					break;
				case 2411:
					RightPortsAlignment = e.GetInt(undo);
					break;
				case 2412:
					LeftPortsLabelSpacing = e.GetFloat(undo);
					break;
				case 2413:
					RightPortsLabelSpacing = e.GetFloat(undo);
					break;
				case 2414:
					LeftPortLabelsInside = (bool)e.GetValue(undo);
					break;
				case 2415:
					RightPortLabelsInside = (bool)e.GetValue(undo);
					break;
				default:
					base.ChangeValue(e, undo);
					break;
				}
			}
		}
	}
}
