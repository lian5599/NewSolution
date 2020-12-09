using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Security;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// An object that displays a text string.
	/// </summary>
	[Serializable]
	public class GoText : GoObject
	{
		internal sealed class TextBoxControl : TextBox, IGoControlObject
		{
			private GoControl myGoControl;

			private GoView myGoView;

			public GoControl GoControl
			{
				get
				{
					return myGoControl;
				}
				set
				{
					if (myGoControl == value)
					{
						return;
					}
					myGoControl = value;
					if (value == null)
					{
						return;
					}
					GoText goText = value.EditedObject as GoText;
					if (goText == null)
					{
						return;
					}
					if (!goText.Multiline)
					{
						int num = goText.FindFirstLineBreak(goText.Text, 0);
						if (num >= 0)
						{
							Text = goText.Text.Substring(0, num);
						}
						else
						{
							Text = goText.Text;
						}
					}
					else
					{
						Text = goText.Text;
					}
					int alignment = goText.Alignment;
					if (alignment <= 16)
					{
						switch (alignment)
						{
						case 1:
							goto IL_00d7;
						case 4:
						case 8:
							goto IL_00e0;
						}
						goto IL_00ce;
					}
					if (alignment <= 64)
					{
						if (alignment != 32)
						{
							if (alignment != 64)
							{
								goto IL_00ce;
							}
							goto IL_00e0;
						}
					}
					else if (alignment != 128)
					{
						_ = 256;
						goto IL_00ce;
					}
					goto IL_00d7;
					IL_00e7:
					Multiline = (goText.Multiline || goText.Wrapping);
					base.AcceptsReturn = goText.Multiline;
					base.WordWrap = goText.Wrapping;
					RightToLeft = (goText.isRightToLeft(GoView) ? RightToLeft.Yes : RightToLeft.No);
					Font font = goText.Font;
					float num2 = font.Size;
					if (GoView != null)
					{
						num2 *= GoView.DocScale / GoView.WorldScale.Height;
					}
					Font = goText.makeFont(font.Name, num2, font.Style);
					return;
					IL_00d7:
					base.TextAlign = HorizontalAlignment.Center;
					goto IL_00e7;
					IL_00ce:
					base.TextAlign = HorizontalAlignment.Left;
					goto IL_00e7;
					IL_00e0:
					base.TextAlign = HorizontalAlignment.Right;
					goto IL_00e7;
				}
			}

			public GoView GoView
			{
				get
				{
					return myGoView;
				}
				set
				{
					myGoView = value;
				}
			}

			protected override bool ProcessDialogKey(Keys key)
			{
				if (HandleKey(key))
				{
					return true;
				}
				return base.ProcessDialogKey(key);
			}

			protected override void OnLeave(EventArgs evt)
			{
				AcceptText(force: true);
				base.OnLeave(evt);
			}

			private bool HandleKey(Keys key)
			{
				switch (key)
				{
				case Keys.Escape:
					GoControl?.DoEndEdit(GoView);
					GoView.RequestFocus();
					return true;
				case Keys.Tab:
				case Keys.Return:
					if (key == Keys.Return && base.AcceptsReturn)
					{
						return false;
					}
					if (AcceptText(force: false))
					{
						GoView.RequestFocus();
					}
					return true;
				default:
					return false;
				}
			}

			private bool AcceptText(bool force)
			{
				GoControl goControl = GoControl;
				if (goControl != null)
				{
					GoText goText = goControl.EditedObject as GoText;
					if ((goText?.DoEdit(GoView, goText.Text, Text) ?? true) || force)
					{
						goControl.DoEndEdit(GoView);
						return true;
					}
				}
				return false;
			}
		}

		internal sealed class NumericUpDownControl : NumericUpDown, IGoControlObject
		{
			private GoControl myGoControl;

			private GoView myGoView;

			public GoControl GoControl
			{
				get
				{
					return myGoControl;
				}
				set
				{
					if (myGoControl == value)
					{
						return;
					}
					myGoControl = value;
					if (value == null)
					{
						return;
					}
					GoText goText = value.EditedObject as GoText;
					if (goText != null)
					{
						RightToLeft = (goText.isRightToLeft(GoView) ? RightToLeft.Yes : RightToLeft.No);
						Font font = goText.Font;
						float num = font.Size;
						if (GoView != null)
						{
							num *= GoView.DocScale / GoView.WorldScale.Height;
						}
						Font = goText.makeFont(font.Name, num, font.Style);
						base.Minimum = goText.Minimum;
						base.Maximum = goText.Maximum;
						try
						{
							base.Value = decimal.Parse(goText.Text, CultureInfo.CurrentCulture);
						}
						catch (FormatException)
						{
							base.Value = base.Minimum;
						}
						catch (OverflowException)
						{
							base.Value = base.Minimum;
						}
					}
				}
			}

			public GoView GoView
			{
				get
				{
					return myGoView;
				}
				set
				{
					myGoView = value;
				}
			}

			protected override bool ProcessDialogKey(Keys key)
			{
				if (HandleKey(key))
				{
					return true;
				}
				return base.ProcessDialogKey(key);
			}

			protected override void OnLeave(EventArgs evt)
			{
				AcceptText(force: true);
				base.OnLeave(evt);
			}

			private bool HandleKey(Keys key)
			{
				switch (key)
				{
				case Keys.Escape:
					GoControl?.DoEndEdit(GoView);
					GoView.RequestFocus();
					return true;
				case Keys.Tab:
				case Keys.Return:
					if (AcceptText(force: false))
					{
						GoView.RequestFocus();
					}
					return true;
				default:
					return false;
				}
			}

			private bool AcceptText(bool force)
			{
				GoControl goControl = GoControl;
				if (goControl != null)
				{
					GoText goText = goControl.EditedObject as GoText;
					if ((goText?.DoEdit(GoView, goText.Text, base.Value.ToString(CultureInfo.CurrentCulture)) ?? true) || force)
					{
						goControl.DoEndEdit(GoView);
						return true;
					}
				}
				return false;
			}
		}

		internal sealed class ComboBoxControl : ComboBox, IGoControlObject
		{
			private GoControl myGoControl;

			private GoView myGoView;

			public GoControl GoControl
			{
				get
				{
					return myGoControl;
				}
				set
				{
					if (myGoControl == value)
					{
						return;
					}
					myGoControl = value;
					if (value == null)
					{
						return;
					}
					GoText goText = value.EditedObject as GoText;
					if (goText == null)
					{
						return;
					}
					RightToLeft = (goText.isRightToLeft(GoView) ? RightToLeft.Yes : RightToLeft.No);
					Font font = goText.Font;
					float num = font.Size;
					if (GoView != null)
					{
						num *= GoView.DocScale / GoView.WorldScale.Height;
					}
					Font = goText.makeFont(font.Name, num, font.Style);
					foreach (object choice in goText.Choices)
					{
						base.Items.Add(choice);
					}
					if (!goText.Multiline)
					{
						int num2 = goText.Text.IndexOf("\r\n");
						if (num2 >= 0)
						{
							Text = goText.Text.Substring(0, num2);
						}
						else
						{
							Text = goText.Text;
						}
					}
					else
					{
						Text = goText.Text;
					}
					if (goText.DropDownList)
					{
						base.DropDownStyle = ComboBoxStyle.DropDownList;
					}
					else
					{
						base.DropDownStyle = ComboBoxStyle.DropDown;
					}
				}
			}

			public GoView GoView
			{
				get
				{
					return myGoView;
				}
				set
				{
					myGoView = value;
				}
			}

			protected override bool ProcessDialogKey(Keys key)
			{
				if (HandleKey(key))
				{
					return true;
				}
				return base.ProcessDialogKey(key);
			}

			protected override void OnLeave(EventArgs evt)
			{
				AcceptText(force: true);
				base.OnLeave(evt);
			}

			private bool HandleKey(Keys key)
			{
				switch (key)
				{
				case Keys.Escape:
					GoControl?.DoEndEdit(GoView);
					GoView.RequestFocus();
					return true;
				case Keys.Tab:
				case Keys.Return:
					if (AcceptText(force: false))
					{
						GoView.RequestFocus();
					}
					return true;
				default:
					return false;
				}
			}

			private bool AcceptText(bool force)
			{
				GoControl goControl = GoControl;
				if (goControl != null)
				{
					GoText goText = goControl.EditedObject as GoText;
					if ((goText?.DoEdit(GoView, goText.Text, Text) ?? true) || force)
					{
						goControl.DoEndEdit(GoView);
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Text" /> property.
		/// </summary>
		public const int ChangedText = 1501;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.FamilyName" /> property.
		/// </summary>
		public const int ChangedFamilyName = 1502;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.FontSize" /> property.
		/// </summary>
		public const int ChangedFontSize = 1503;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Alignment" /> property.
		/// </summary>
		public const int ChangedAlignment = 1504;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.TextColor" /> property.
		/// </summary>
		public const int ChangedTextColor = 1505;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.BackgroundColor" /> property.
		/// </summary>
		public const int ChangedBackgroundColor = 1506;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> property.
		/// </summary>
		public const int ChangedTransparentBackground = 1507;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Bold" /> property.
		/// </summary>
		public const int ChangedBold = 1508;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Italic" /> property.
		/// </summary>
		public const int ChangedItalic = 1509;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Underline" /> property.
		/// </summary>
		public const int ChangedUnderline = 1510;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.StrikeThrough" /> property.
		/// </summary>
		public const int ChangedStrikeThrough = 1511;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Multiline" /> property.
		/// </summary>
		public const int ChangedMultiline = 1512;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.BackgroundOpaqueWhenSelected" /> property.
		/// </summary>
		public const int ChangedBackgroundOpaqueWhenSelected = 1515;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Clipping" /> property.
		/// </summary>
		public const int ChangedClipping = 1516;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.AutoResizes" /> property.
		/// </summary>
		public const int ChangedAutoResizes = 1518;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Wrapping" /> property.
		/// </summary>
		public const int ChangedWrapping = 1520;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.WrappingWidth" /> property.
		/// </summary>
		public const int ChangedWrappingWidth = 1521;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.GdiCharSet" /> property.
		/// </summary>
		public const int ChangedGdiCharSet = 1522;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.EditorStyle" /> property.
		/// </summary>
		public const int ChangedEditorStyle = 1523;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Minimum" /> property.
		/// </summary>
		public const int ChangedMinimum = 1524;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Maximum" /> property.
		/// </summary>
		public const int ChangedMaximum = 1525;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.DropDownList" /> property.
		/// </summary>
		public const int ChangedDropDownList = 1526;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Choices" /> property.
		/// </summary>
		public const int ChangedChoices = 1527;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.RightToLeft" /> property.
		/// </summary>
		public const int ChangedRightToLeft = 1528;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.RightToLeftFromView" /> property.
		/// </summary>
		public const int ChangedRightToLeftFromView = 1529;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.Bordered" /> property.
		/// </summary>
		public const int ChangedBordered = 1530;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.StringTrimming" /> property.
		/// </summary>
		public const int ChangedStringTrimming = 1531;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoText.EditableWhenSelected" /> property.
		/// </summary>
		public const int ChangedEditableWhenSelected = 1532;

		private const int flagTransparentBackground = 1;

		private const int flagBold = 2;

		private const int flagItalic = 4;

		private const int flagUnderline = 8;

		private const int flagStrikeThrough = 16;

		private const int flagMultiline = 32;

		private const int flagWrapping = 64;

		private const int flagClipping = 128;

		private const int flagAutoResizes = 256;

		private const int flagBackgroundOpaqueWhenSelected = 512;

		private const int flagEditableWhenSelected = 1024;

		private const int flagDropDownList = 2048;

		private const int flagRightToLeft = 268435456;

		private const int flagRightToLeftFromView = 536870912;

		private const int flagUpdating = 1073741824;

		private const int maskEditorStyle = 61440;

		private const int maskGdiCharSet = 16711680;

		private const int maskStringTrimming = 251658240;

		private const byte DEFAULT_CHARSET = 1;

		private const int flagBordered = 1048576;

		private static readonly char[] myNewlineArray = new char[2]
		{
			'\r',
			'\n'
		};

		private static string myDefaultFontName = FontFamily.GenericSansSerif.Name;

		private static StringFormat myStandardStringFormat = MakeStandardStringFormat();

		private static float myDefaultFontSize = 10f;

		private static Font myLastFont;

		private static readonly IList myEmptyChoices = ArrayList.FixedSize(new ArrayList());

		private static readonly Bitmap myEmptyBitmap = new Bitmap(10, 10);

		private string myString = "";

		private string myFamilyName = myDefaultFontName;

		private float myFontSize = myDefaultFontSize;

		private int myAlignment = 2;

		private Color myTextColor = Color.Black;

		private Color myBackgroundColor = Color.White;

		private int myInternalTextFlags = 536936705;

		private float myWrappingWidth = 150f;

		private int myNumLines = 1;

		private int myMinimum;

		private int myMaximum = 100;

		private IList myChoices = myEmptyChoices;

		[NonSerialized]
		private StringFormat myStringFormat;

		[NonSerialized]
		private Font myFont;

		[NonSerialized]
		private GoControl myEditor;

		/// <summary>
		/// Gets or sets the string that this text object displays.
		/// </summary>
		/// <value>
		/// This defaults to the empty string.
		/// Setting this property to a null/nothing value will result
		/// in using an empty string instead.
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </value>
		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The string that this text object displays.")]
		public virtual string Text
		{
			get
			{
				return myString;
			}
			set
			{
				string text = value;
				if (text == null)
				{
					text = "";
				}
				string text2 = myString;
				if (text2 != text)
				{
					myString = text;
					Changed(1501, 0, text2, GoObject.NullRect, 0, text, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets the font family face name.
		/// </summary>
		/// <value>
		/// Initially this is the value of <see cref="P:Northwoods.Go.GoText.DefaultFontFamilyName" />.
		/// Setting this value to null will result in the use of that default.
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoText.FontSize" />
		[Category("Appearance")]
		[Description("The font family face name.")]
		public virtual string FamilyName
		{
			get
			{
				return myFamilyName;
			}
			set
			{
				string text = value;
				if (text == null)
				{
					text = DefaultFontFamilyName;
				}
				string text2 = myFamilyName;
				if (text2 != text)
				{
					myFamilyName = text;
					ResetFont();
					Changed(1502, 0, text2, GoObject.NullRect, 0, text, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets the font size.
		/// </summary>
		/// <value>
		/// This value is in units of text points.
		/// Initially this is the value of <see cref="P:Northwoods.Go.GoText.DefaultFontSize" />.
		/// </value>
		/// <remarks>
		/// Setting this property when <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true
		/// will cause this object's bounds to be updated to fit the text.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.FamilyName" />
		[Category("Appearance")]
		[Description("The text font size, in points")]
		public virtual float FontSize
		{
			get
			{
				return myFontSize;
			}
			set
			{
				float num = myFontSize;
				if (value > 0f && num != value)
				{
					myFontSize = value;
					ResetFont();
					Changed(1503, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets how the text is aligned.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoObject.TopLeft" />; you can use any of the predefined
		/// <see cref="T:Northwoods.Go.GoObject" /> spot values.
		/// </value>
		/// <remarks>
		/// This property governs how each line of text is positioned in the bounding rectangle.
		/// In addition this property specifies the <see cref="P:Northwoods.Go.GoText.Location" /> for this object.
		/// The value of this property is not changed by the value of <see cref="P:Northwoods.Go.GoText.RightToLeft" />
		/// or <c>Control.RightToLeft</c> for any <see cref="T:Northwoods.Go.GoView" />.
		/// However, the <c>GoView.RightToLeft</c> or <see cref="P:Northwoods.Go.GoText.RightToLeft" /> property
		/// does affect where the text is drawn for that view.  When <c>GoView.RightToLeft</c>
		/// property value is <c>RightToLeft.Yes</c>, or when <see cref="P:Northwoods.Go.GoText.RightToLeftFromView" />
		/// is false and <see cref="P:Northwoods.Go.GoText.RightToLeft" /> is true,
		/// then text whose <see cref="P:Northwoods.Go.GoText.Alignment" /> is on the left (such as
		/// <see cref="F:Northwoods.Go.GoObject.TopLeft" />) will actually be drawn right-aligned.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(2)]
		[Description("The text alignment.")]
		public virtual int Alignment
		{
			get
			{
				return myAlignment;
			}
			set
			{
				int num = myAlignment;
				if (num != value)
				{
					myAlignment = value;
					Changed(1504, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <value>
		/// This defaults to <c>Color.Black</c>.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoText.BackgroundColor" />
		[Category("Appearance")]
		[Description("The color of the text.")]
		public virtual Color TextColor
		{
			get
			{
				return myTextColor;
			}
			set
			{
				Color color = myTextColor;
				if (color != value)
				{
					myTextColor = value;
					Changed(1505, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color for this text object, shown when <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> is false.
		/// </summary>
		/// <value>
		/// This defaults to <c>Color.White</c>.
		/// </value>
		/// <remarks>
		/// The background is not painted if <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> is true.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.TextColor" />
		/// <seealso cref="P:Northwoods.Go.GoText.BackgroundOpaqueWhenSelected" />
		[Category("Appearance")]
		[Description("The background color for this text object.")]
		public virtual Color BackgroundColor
		{
			get
			{
				return myBackgroundColor;
			}
			set
			{
				Color color = myBackgroundColor;
				if (color != value)
				{
					myBackgroundColor = value;
					Changed(1506, 0, color, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		private int InternalTextFlags
		{
			get
			{
				return myInternalTextFlags;
			}
			set
			{
				myInternalTextFlags = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the background color is painted.
		/// </summary>
		/// <value>
		/// This defaults to true--the background color is not painted, and only
		/// the characters are displayed.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoText.BackgroundColor" />
		/// <seealso cref="P:Northwoods.Go.GoText.BackgroundOpaqueWhenSelected" />
		/// <seealso cref="P:Northwoods.Go.GoText.TextColor" />
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the text is painted alone, or if the background is painted first.")]
		public virtual bool TransparentBackground
		{
			get
			{
				return (myInternalTextFlags & 1) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 1) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 1;
					}
					else
					{
						myInternalTextFlags &= -2;
					}
					Changed(1507, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the font style is bold.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Italic" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the font is bold.")]
		public virtual bool Bold
		{
			get
			{
				return (myInternalTextFlags & 2) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 2) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 2;
					}
					else
					{
						myInternalTextFlags &= -3;
					}
					ResetFont();
					Changed(1508, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the font style is italic.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Bold" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the font is italic.")]
		public virtual bool Italic
		{
			get
			{
				return (myInternalTextFlags & 4) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 4) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 4;
					}
					else
					{
						myInternalTextFlags &= -5;
					}
					ResetFont();
					Changed(1509, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the font style includes underline.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.StrikeThrough" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the font style includes an underline.")]
		public virtual bool Underline
		{
			get
			{
				return (myInternalTextFlags & 8) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 8) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 8;
					}
					else
					{
						myInternalTextFlags &= -9;
					}
					ResetFont();
					Changed(1510, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the font style includes strike-through.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Underline" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the font style includes a strike-through.")]
		public virtual bool StrikeThrough
		{
			get
			{
				return (myInternalTextFlags & 0x10) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x10) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 16;
					}
					else
					{
						myInternalTextFlags &= -17;
					}
					ResetFont();
					Changed(1511, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether a simple border is drawn around the text.
		/// </summary>
		/// <value>the default value is false, indicating no border</value>
		/// <remarks>
		/// The line drawn around the text is drawn using a solid Pen whose color
		/// is <see cref="P:Northwoods.Go.GoText.TextColor" />.  If you need more complex borders, wider
		/// margins around the text, or other visual effects, you will need to
		/// override <see cref="M:Northwoods.Go.GoText.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> or compose different objects using a <see cref="T:Northwoods.Go.GoGroup" />.
		/// Note that the <see cref="P:Northwoods.Go.GoObject.Bounds" /> of this object are not changed by
		/// setting this property.
		/// However, this property does affect the result of <see cref="M:Northwoods.Go.GoText.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />,
		/// just as the <see cref="P:Northwoods.Go.GoObject.Shadowed" /> property also affects that method.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether a simple border using the TextColor is drawn around the text.")]
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
					Changed(1530, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the GDI character set used for the text.
		/// </summary>
		/// <value>
		/// The default value is 1, the default character set.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(1)]
		[Description("The GDI character set.")]
		public virtual int GdiCharSet
		{
			get
			{
				return (myInternalTextFlags & 0xFF0000) >> 16;
			}
			set
			{
				int num = (myInternalTextFlags & 0xFF0000) >> 16;
				int num2 = value & 0xFF;
				if (num != num2)
				{
					myInternalTextFlags = ((myInternalTextFlags & -16711681) | (num2 << 16));
					ResetFont();
					Changed(1522, num, null, GoObject.NullRect, num2, null, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this text should be drawn from right to left,
		/// when the <see cref="P:Northwoods.Go.GoText.RightToLeftFromView" /> property is false.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// This property is ignored when <see cref="P:Northwoods.Go.GoText.RightToLeftFromView" /> is true.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether to draw text from right to left, when RightToLeftFromView is false")]
		public virtual bool RightToLeft
		{
			get
			{
				return (myInternalTextFlags & 0x10000000) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x10000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 268435456;
					}
					else
					{
						myInternalTextFlags &= -268435457;
					}
					Changed(1528, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the view's <c>RightToLeft</c> property governs
		/// how the string is drawn, or whether this text object's <see cref="P:Northwoods.Go.GoText.RightToLeft" />
		/// property takes precedence.
		/// </summary>
		/// <value>
		/// This defaults to true, which means the <see cref="T:Northwoods.Go.GoView" />'s <c>RightToLeft</c>
		/// property will affect the appearance of the text.
		/// However, for GoDiagram Web, since the ASP.NET Web Forms
		/// Controls don't support a <c>RightToLeft</c> property,
		/// this property is ignored and the value is assumed to be false.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether the view's RightToLeft property takes precedence over this text object's RightToLeft property")]
		public virtual bool RightToLeftFromView
		{
			get
			{
				return (myInternalTextFlags & 0x20000000) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x20000000) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 536870912;
					}
					else
					{
						myInternalTextFlags &= -536870913;
					}
					Changed(1529, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the text is displayed in multiple lines,
		/// according to any NewLine character sequences in
		/// the text string.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If this is false, any text after the first NewLine is ignored.
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Alignment" />
		/// <seealso cref="P:Northwoods.Go.GoText.Wrapping" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the text will be displayed as multiple lines of text.")]
		public virtual bool Multiline
		{
			get
			{
				return (myInternalTextFlags & 0x20) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x20) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 32;
					}
					else
					{
						myInternalTextFlags &= -33;
					}
					Changed(1512, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the text background is displayed when selected.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// When this property is true and this text object is selected, we set the
		/// <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> property to false.  Assuming there is a suitable
		/// <see cref="P:Northwoods.Go.GoText.BackgroundColor" />, the text will appear highlit.  When the text
		/// object is no longer selected, the <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> property is set to true.
		/// Under these circumstances the change to the <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> property
		/// is not recorded by the undo manager (if any).
		/// Note that setting this property to true will prevent the normal selection handles
		/// from appearing, so that when <see cref="P:Northwoods.Go.GoObject.Resizable" /> is true there won't be any
		/// resize handles for the user to manipulate.
		/// This feature should only be used when there is only one view on the document, or
		/// if it is OK for all views on the document to display the text highlight when the
		/// text object is selected in just one of the views.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoText.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the text background is displayed when selected, and transparent when not selected")]
		public virtual bool BackgroundOpaqueWhenSelected
		{
			get
			{
				return (myInternalTextFlags & 0x200) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x200) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 512;
					}
					else
					{
						myInternalTextFlags &= -513;
					}
					Changed(1515, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the text drawing is clipped by this object's bounds.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// <remarks>
		/// </remarks>
		/// You should turn on clipping when <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is false,
		/// unless you can be sure the text will always fit within the bounds
		/// of this object.
		/// You may also wish to set the <c>StringTrimming</c> property,
		/// for additional control over how the text is truncated or elided.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.StringTrimming" />
		/// <seealso cref="P:Northwoods.Go.GoText.AutoResizes" />
		/// <seealso cref="P:Northwoods.Go.GoObject.AutoRescales" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the text drawing is clipped by the bounds.")]
		public virtual bool Clipping
		{
			get
			{
				return (myInternalTextFlags & 0x80) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x80) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 128;
					}
					else
					{
						myInternalTextFlags &= -129;
					}
					Changed(1516, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the bounds are recalculated when the text changes.
		/// </summary>
		/// <value>
		/// This defaults to true.
		/// </value>
		/// <remarks>
		/// If both this property and <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> are false, and you change
		/// either the <see cref="P:Northwoods.Go.GoText.Text" /> string or the <see cref="P:Northwoods.Go.GoObject.Bounds" />,
		/// you must be careful not to make either the text too big or the bounds too small.
		/// It is then wise to turn on the <see cref="P:Northwoods.Go.GoText.Clipping" /> property.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Text" />
		/// <seealso cref="P:Northwoods.Go.GoText.Clipping" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the bounds are recalculated when the text changes.")]
		public virtual bool AutoResizes
		{
			get
			{
				return (myInternalTextFlags & 0x100) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x100) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 256;
					}
					else
					{
						myInternalTextFlags &= -257;
					}
					Changed(1518, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets how characters are removed when <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is false.
		/// </summary>
		/// <value>
		/// The default value is <c>StringTrimming.None</c>.
		/// </value>
		/// <remarks>
		/// If you set this property to a value other than <c>StringTrimming.None</c>,
		/// you should set <see cref="P:Northwoods.Go.GoText.AutoResizes" /> to false and <see cref="P:Northwoods.Go.GoText.Clipping" /> to true.
		/// Clipping may be needed even if this value is different than <c>StringTrimming.None</c>.
		/// If <see cref="P:Northwoods.Go.GoText.Multiline" /> is true, this applies to each virtual line,
		/// as separated by NewLine character sequences.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.Clipping" />
		/// <seealso cref="P:Northwoods.Go.GoText.AutoResizes" />
		[Category("Appearance")]
		[DefaultValue(StringTrimming.None)]
		[Description("How to trim text that does not fit.")]
		public virtual StringTrimming StringTrimming
		{
			get
			{
				return (StringTrimming)((myInternalTextFlags & 0xF000000) >> 24);
			}
			set
			{
				int num = (myInternalTextFlags & 0xF000000) >> 24;
				int num2 = (int)(value & (StringTrimming)15);
				if (num != num2)
				{
					myInternalTextFlags = ((myInternalTextFlags & -251658241) | (num2 << 24));
					ResetFont();
					Changed(1531, num, null, GoObject.NullRect, num2, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the text is wrapped when the text reaches the wrapping width.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoText.WrappingWidth" />
		/// <seealso cref="P:Northwoods.Go.GoText.Multiline" />
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the text is wrapped.")]
		public virtual bool Wrapping
		{
			get
			{
				return (myInternalTextFlags & 0x40) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x40) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 64;
					}
					else
					{
						myInternalTextFlags &= -65;
					}
					Changed(1520, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Gets or sets the width at which text is wrapped.
		/// </summary>
		/// <value>
		/// This distance is in document coordinates.
		/// This defaults to <c>150</c>.
		/// The new value must be positive and at least as wide as any single character.
		/// </value>
		/// <remarks>
		/// Text is wrapped to the next line when the <see cref="P:Northwoods.Go.GoText.Wrapping" /> property is true and
		/// the text gets wider than this value.
		/// If <see cref="P:Northwoods.Go.GoText.Multiline" /> is true, a NewLine character sequence (Carriage Return/
		/// LineFeed) forces a line break.
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(150)]
		[Description("The width at which wrapping occurs, if Wrapping is true.")]
		public virtual float WrappingWidth
		{
			get
			{
				return myWrappingWidth;
			}
			set
			{
				float num = myWrappingWidth;
				if (value > 0f && num != value)
				{
					myWrappingWidth = value;
					Changed(1521, 0, null, GoObject.MakeRect(num), 0, null, GoObject.MakeRect(value));
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// The kind of editor Control used when the user edits the text string in-place.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(GoTextEditorStyle.TextBox)]
		[Description("The kind of Control used when editing")]
		public GoTextEditorStyle EditorStyle
		{
			get
			{
				return (GoTextEditorStyle)((myInternalTextFlags & 0xF000) >> 12);
			}
			set
			{
				GoTextEditorStyle goTextEditorStyle = (GoTextEditorStyle)((myInternalTextFlags & 0xF000) >> 12);
				if (goTextEditorStyle != value)
				{
					myInternalTextFlags = ((myInternalTextFlags & -61441) | ((int)value << 12));
					Changed(1523, (int)goTextEditorStyle, null, GoObject.NullRect, (int)value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoText.EditorStyle" /> is <see cref="F:Northwoods.Go.GoTextEditorStyle.NumericUpDown" />,
		/// this specifies the minimum value.
		/// </summary>
		/// <value>
		/// This defaults to zero.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(0)]
		[Description("The minimum value that the user can choose")]
		public int Minimum
		{
			get
			{
				return myMinimum;
			}
			set
			{
				int num = myMinimum;
				if (num != value)
				{
					myMinimum = value;
					Changed(1524, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoText.EditorStyle" /> is <see cref="F:Northwoods.Go.GoTextEditorStyle.NumericUpDown" />,
		/// this specifies the maximum value.
		/// </summary>
		/// <value>
		/// This defaults to 100.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(100)]
		[Description("The maximum value that the user can choose")]
		public int Maximum
		{
			get
			{
				return myMaximum;
			}
			set
			{
				int num = myMaximum;
				if (num != value)
				{
					myMaximum = value;
					Changed(1525, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoText.EditorStyle" /> is <see cref="F:Northwoods.Go.GoTextEditorStyle.ComboBox" />,
		/// this controls whether the user must choose from the list of <see cref="P:Northwoods.Go.GoText.Choices" />,
		/// or whether the user can type in an arbitrary string.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the user is limited to values that are in the predefined list of Items.")]
		public bool DropDownList
		{
			get
			{
				return (myInternalTextFlags & 0x800) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x800) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 2048;
					}
					else
					{
						myInternalTextFlags &= -2049;
					}
					Changed(1526, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// When the <see cref="P:Northwoods.Go.GoText.EditorStyle" /> is <see cref="F:Northwoods.Go.GoTextEditorStyle.ComboBox" />,
		/// this holds the list of choices that are presented in the drop down list.
		/// </summary>
		/// <value>
		/// This defaults to an empty list.  The value may be shared by more than
		/// one <c>GoText</c> object.
		/// </value>
		/// <remarks>
		/// Modifying the items in the list is not controlled.  Any such changes are not
		/// recorded by the undo manager.  Only replacing the list is recorded for undo/redo,
		/// and even then, it is only the reference to an <c>IList</c> that is remembered
		/// and restored.
		/// </remarks>
		[Category("Behavior")]
		[Description("The list of items presented in a drop-down list when editing")]
		public IList Choices
		{
			get
			{
				if (myChoices == null)
				{
					return myEmptyChoices;
				}
				return myChoices;
			}
			set
			{
				IList list = (myChoices != null) ? myChoices : myEmptyChoices;
				IList list2 = value;
				if (list2 == null)
				{
					list2 = myEmptyChoices;
				}
				if (list != list2)
				{
					myChoices = list2;
					Changed(1527, 0, list, GoObject.NullRect, 0, list2, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the <c>Font</c> currently being used by this text object.
		/// </summary>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.AutoResizes" /> is true, setting this property will resize
		/// this object to fit the text;
		/// otherwise if <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, setting this property
		/// will change the font size so that the text fits the bounds.
		/// </remarks>
		[Browsable(false)]
		public Font Font
		{
			get
			{
				if (myFont == null)
				{
					FontStyle fontStyle = FontStyle.Regular;
					if (Bold)
					{
						fontStyle |= FontStyle.Bold;
					}
					if (Italic)
					{
						fontStyle |= FontStyle.Italic;
					}
					if (Underline)
					{
						fontStyle |= FontStyle.Underline;
					}
					if (StrikeThrough)
					{
						fontStyle |= FontStyle.Strikeout;
					}
					myFont = shareFont(FamilyName, FontSize, fontStyle);
				}
				return myFont;
			}
			set
			{
				if (value != null)
				{
					bool initializing = base.Initializing;
					base.Initializing = true;
					FamilyName = value.Name;
					FontSize = value.Size;
					Bold = ((value.Style & FontStyle.Bold) != 0);
					Italic = ((value.Style & FontStyle.Italic) != 0);
					Underline = ((value.Style & FontStyle.Underline) != 0);
					StrikeThrough = ((value.Style & FontStyle.Strikeout) != 0);
					GdiCharSet = value.GdiCharSet;
					myFont = value;
					base.Initializing = initializing;
					UpdateSizeOrScale();
				}
			}
		}

		/// <summary>
		/// Compute how many lines of text are displayed.
		/// </summary>
		/// <value>the number of physical lines of text that are drawn, including
		/// line breaks caused by newlines and by wrapping, if any</value>
		[Category("Appearance")]
		[Description("How many lines of text are being displayed")]
		public virtual int LineCount => myNumLines;

		/// <summary>
		/// A text object's natural location is determined by its Alignment.
		/// </summary>
		/// <remarks>
		/// The value of this property is not changed by the value of <see cref="P:Northwoods.Go.GoText.RightToLeft" />
		/// or <c>Control.RightToLeft</c> for any <see cref="T:Northwoods.Go.GoView" />.
		/// </remarks>
		public override PointF Location
		{
			get
			{
				return GetSpotLocation(Alignment);
			}
			set
			{
				SetSpotLocation(Alignment, value);
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> should only start editing
		/// if this label is part of an object that was already selected before it was clicked.
		/// </summary>
		/// <value>The default value is false</value>
		/// <remarks>
		/// This property only affects the behavior of <see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />,
		/// and is only effective when <see cref="P:Northwoods.Go.GoObject.Editable" /> is also true.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether a single click starts in-place editing only if the Editable text is part of a selected object")]
		public virtual bool EditableWhenSelected
		{
			get
			{
				return (myInternalTextFlags & 0x400) != 0;
			}
			set
			{
				bool flag = (myInternalTextFlags & 0x400) != 0;
				if (flag != value)
				{
					if (value)
					{
						myInternalTextFlags |= 1024;
					}
					else
					{
						myInternalTextFlags &= -1025;
					}
					Changed(1532, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="T:Northwoods.Go.GoControl" /> used to represent a text editing <c>Control</c>.
		/// </summary>
		/// <value>may be null, when not editing</value>
		/// <remarks>
		/// This is only set by <see cref="M:Northwoods.Go.GoText.DoBeginEdit(Northwoods.Go.GoView)" /> and <see cref="M:Northwoods.Go.GoText.DoEndEdit(Northwoods.Go.GoView)" />
		/// in conjunction with setting <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" />.
		/// </remarks>
		public override GoControl Editor => myEditor;

		/// <summary>
		/// Gets or sets the font family face name used when constructing a <see cref="T:Northwoods.Go.GoText" /> object.
		/// </summary>
		/// <value>
		/// This defaults to "Microsoft Sans Serif".  The value must not be null.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoText.DefaultFontSize" />
		[Description("The initial font face name for newly constructed GoText objects.")]
		public static string DefaultFontFamilyName
		{
			get
			{
				return myDefaultFontName;
			}
			set
			{
				if (value != null)
				{
					myDefaultFontName = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the font size used when constructing a <see cref="T:Northwoods.Go.GoText" /> object.
		/// </summary>
		/// <value>
		/// This defaults to <c>10</c>.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoText.DefaultFontFamilyName" />
		[Description("The initial font size for newly constructed GoText objects.")]
		public static float DefaultFontSize
		{
			get
			{
				return myDefaultFontSize;
			}
			set
			{
				if (value > 0f)
				{
					myDefaultFontSize = value;
				}
			}
		}

		/// <summary>
		/// The constructor produces an object that is not resizable by the user
		/// and does not automatically rescale the font size when the size is changed.
		/// However, it does automatically change the size to fit the text when the
		/// text is changed.
		/// </summary>
		public GoText()
		{
			base.InternalFlags &= -273;
		}

		/// <summary>
		/// This makes a copy of this text object.
		/// </summary>
		/// <param name="env"></param>
		/// <returns>The copied object.</returns>
		/// <remarks>
		/// The list of <see cref="P:Northwoods.Go.GoText.Choices" />, if any, is not copied--it is shared
		/// with the original text object.
		/// </remarks>
		public override GoObject CopyObject(GoCopyDictionary env)
		{
			GoText goText = (GoText)base.CopyObject(env);
			if (goText != null)
			{
				goText.myEditor = null;
			}
			return goText;
		}

		private void ResetFont()
		{
			if (myFont != null)
			{
				myFont = null;
			}
		}

		/// <summary>
		/// This method is responsible for alternative painting when the
		/// scale is small.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True if this method handled the Paint request; false if it did
		/// not and the regular painting should take place.
		/// </returns>
		/// <remarks>
		/// This paints nothing if the <paramref name="view" />'s
		/// <see cref="P:Northwoods.Go.GoView.DocScale" /> is less than the
		/// <see cref="P:Northwoods.Go.GoView.PaintNothingScale" />.
		/// It just draws a line if the scale is less than the
		/// <see cref="P:Northwoods.Go.GoView.PaintGreekScale" />.
		/// </remarks>
		public virtual bool PaintGreek(Graphics g, GoView view)
		{
			float docScale = view.DocScale;
			float num = view.PaintNothingScale;
			float num2 = view.PaintGreekScale;
			if (view.IsPrinting)
			{
				num /= 4f;
				num2 /= 4f;
			}
			float fontSize = FontSize;
			fontSize /= 10f;
			fontSize *= view.WorldScale.Height;
			num /= fontSize;
			num2 /= fontSize;
			if (docScale <= num)
			{
				return true;
			}
			checked
			{
				if (docScale <= num2)
				{
					RectangleF bounds = Bounds;
					Pen pen = GoShape.NewPen(TextColor, 1f / view.WorldScale.Height);
					int lineCount = LineCount;
					float num3 = bounds.Y;
					float num4 = bounds.Height / (float)(lineCount + 1);
					for (int i = 0; i < lineCount; i++)
					{
						num3 += num4;
						GoShape.DrawLine(g, view, pen, bounds.X, num3, bounds.X + bounds.Width, num3);
					}
					pen.Dispose();
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Draw a text string according to the attributes specified by this object's properties.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> is false, we fill the bounds with the
		/// <see cref="P:Northwoods.Go.GoText.BackgroundColor" />, and we paint a rectangular drop shadow if
		/// <see cref="P:Northwoods.Go.GoObject.Shadowed" />.
		/// If <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> is true, we draw the text (offset) using the shadow
		/// pen if <see cref="P:Northwoods.Go.GoObject.Shadowed" /> is true.  Then we draw the text using the
		/// <see cref="P:Northwoods.Go.GoText.TextColor" />.
		/// The value of <see cref="P:Northwoods.Go.GoText.Alignment" /> and either <see cref="P:Northwoods.Go.GoText.RightToLeft" /> or
		/// <c>GoView.RightToLeft</c> determine the actual alignment of the text.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			if (PaintGreek(g, view))
			{
				return;
			}
			RectangleF bounds = Bounds;
			if (!TransparentBackground)
			{
				if (Shadowed)
				{
					SizeF shadowOffset = GetShadowOffset(view);
					Brush shadowBrush = GetShadowBrush(view);
					GoShape.DrawRectangle(g, view, null, shadowBrush, bounds.X + shadowOffset.Width, bounds.Y + shadowOffset.Height, bounds.Width, bounds.Height);
				}
				Color backgroundColor = BackgroundColor;
				Brush brush = (backgroundColor == Color.White) ? GoShape.Brushes_White : new SolidBrush(BackgroundColor);
				GoShape.DrawRectangle(g, view, null, brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				if (backgroundColor != Color.White)
				{
					brush.Dispose();
				}
			}
			string text = Text;
			float num = 1f;
			if (view != null)
			{
				num /= view.WorldScale.Width;
			}
			if (Shadowed && TransparentBackground)
			{
				RectangleF rect = bounds;
				SizeF shadowOffset2 = GetShadowOffset(view);
				rect.X += shadowOffset2.Width;
				rect.Y += shadowOffset2.Height;
				if (Bordered)
				{
					Pen shadowPen = GetShadowPen(view, num);
					GoShape.DrawRectangle(g, view, shadowPen, null, rect.X - num / 2f, rect.Y, rect.Width + num, rect.Height);
				}
				if (text.Length > 0)
				{
					Brush shadowBrush2 = GetShadowBrush(view);
					paintText(text, g, view, rect, shadowBrush2);
				}
			}
			Color textColor = TextColor;
			if (Bordered)
			{
				Pen pen = GoShape.NewPen(textColor, num);
				GoShape.DrawRectangle(g, view, pen, null, bounds.X - num / 2f, bounds.Y, bounds.Width + num, bounds.Height);
				pen.Dispose();
			}
			if (text.Length > 0)
			{
				Brush brush2 = (textColor == Color.Black) ? GoShape.Brushes_Black : new SolidBrush(TextColor);
				paintText(text, g, view, bounds, brush2);
				if (textColor != Color.Black)
				{
					brush2.Dispose();
				}
			}
		}

		private void paintText(string str, Graphics g, GoView view, RectangleF rect, Brush textbrush)
		{
			if (str.Length == 0)
			{
				return;
			}
			Font font = Font;
			if (font == null)
			{
				return;
			}
			Font font2 = null;
			float lineHeight = getLineHeight(font);
			bool clipping = Clipping;
			Region region = null;
			Region region2 = null;
			if (clipping)
			{
				RectangleF rect2 = GoObject.IntersectionRect(rect, g.ClipBounds);
				region = g.Clip;
				region2 = (g.Clip = new Region(rect2));
			}
			if (!Multiline)
			{
				int num = FindFirstLineBreak(str, 0);
				if (num >= 0)
				{
					str = str.Substring(0, num);
				}
			}
			StringFormat stringFormat = getStringFormat(view);
			if (view.IsPrinting && AutoResizes)
			{
				font2 = findLargestFont(g, Bounds, font.Size - 1f, font.Size);
				font = font2;
			}
			float num2 = 0f - getLineLeading(font);
			int num3 = 0;
			int num4 = -1;
			int nextline = -1;
			bool flag = false;
			while (!flag)
			{
				num4 = FindFirstLineBreak(str, num3, ref nextline);
				if (num4 == -1)
				{
					num4 = str.Length;
					flag = true;
				}
				if (num3 <= num4)
				{
					string text = str.Substring(num3, checked(num4 - num3));
					if (text.Length > 0)
					{
						RectangleF rect3 = new RectangleF(rect.X, rect.Y + num2, rect.Width, rect.Height - num2 + 0.01f);
						drawString(text, g, view, font, textbrush, rect3, stringFormat);
						if (Wrapping)
						{
							int lines = 0;
							num2 += measureString(text, g, font, stringFormat, new SizeF(rect3.Width, rect3.Height), out lines).Height;
						}
						else
						{
							num2 += lineHeight;
						}
					}
					else
					{
						num2 += lineHeight;
					}
				}
				num3 = nextline;
			}
			font2?.Dispose();
			if (clipping && region != null)
			{
				g.Clip = region;
			}
			region2?.Dispose();
		}

		private int FindFirstLineBreak(string str, int start)
		{
			int nextline = 0;
			return FindFirstLineBreak(str, start, ref nextline);
		}

		private int FindFirstLineBreak(string str, int start, ref int nextline)
		{
			int num = str.IndexOfAny(myNewlineArray, start);
			checked
			{
				if (num >= 0)
				{
					if (str[num] == '\r' && num + 1 < str.Length && str[num + 1] == '\n')
					{
						nextline = num + 2;
					}
					else
					{
						nextline = num + 1;
					}
				}
				return num;
			}
		}

		internal void UpdateScale()
		{
			if (!base.Initializing && (InternalTextFlags & 0x40000000) == 0 && AutoRescales)
			{
				InternalTextFlags |= 1073741824;
				rescaleFont();
				InternalTextFlags &= -1073741825;
			}
		}

		internal void UpdateSize()
		{
			if (!base.Initializing && (InternalTextFlags & 0x40000000) == 0 && AutoResizes)
			{
				InternalTextFlags |= 1073741824;
				recalcBoundingRect();
				InternalTextFlags &= -1073741825;
			}
		}

		private void UpdateSizeOrScale()
		{
			if (AutoResizes)
			{
				UpdateSize();
			}
			else
			{
				UpdateScale();
			}
		}

		private bool fitsInBox(Graphics g, Font font, RectangleF rect)
		{
			float num = computeWidth(g, font);
			if (rect.Width < num)
			{
				return false;
			}
			float num2 = computeHeight(g, font, rect.Width);
			if (rect.Height < num2)
			{
				return false;
			}
			return true;
		}

		private Font findLargestFont(Graphics g, RectangleF rect, float minfsize, float maxfsize)
		{
			if (minfsize <= 0f)
			{
				minfsize = 0.01f;
			}
			string name = Font.Name;
			FontStyle style = Font.Style;
			float num = 10f;
			Font font = null;
			for (; num <= maxfsize; num += 1f)
			{
				if (!fitsInBox(g, font = makeFont(name, num, style), rect))
				{
					break;
				}
				if (font == null)
				{
					break;
				}
				font.Dispose();
			}
			font?.Dispose();
			num -= 0.1f;
			while (!fitsInBox(g, font = makeFont(name, num, style), rect) && num > minfsize + 0.1f && font != null)
			{
				font.Dispose();
				num -= 0.1f;
			}
			return font;
		}

		private void rescaleFont()
		{
			lock (myEmptyBitmap)
			{
				Graphics graphics = Graphics.FromImage(myEmptyBitmap);
				graphics.PageUnit = GraphicsUnit.Pixel;
				graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				Font font = findLargestFont(graphics, Bounds, 0f, 999f);
				if (font != null)
				{
					FontSize = font.Size;
					font.Dispose();
				}
				graphics.Dispose();
			}
		}

		private void recalcBoundingRect()
		{
			lock (myEmptyBitmap)
			{
				Graphics graphics = Graphics.FromImage(myEmptyBitmap);
				graphics.PageUnit = GraphicsUnit.Pixel;
				graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				float num = computeWidth(graphics, Font);
				float num2 = 10f;
				GoDocument document = base.Document;
				if (document != null)
				{
					num2 /= document.WorldScale.Width;
				}
				if (num < num2)
				{
					num = num2;
				}
				float num3 = computeHeight(graphics, Font, num);
				if (num != base.Width || num3 != base.Height)
				{
					SetSizeKeepingLocation(new SizeF(num, num3));
				}
				graphics.Dispose();
			}
		}

		private float computeHeight(Graphics g, Font font, float maxw)
		{
			string text = Text;
			float lineHeight = getLineHeight(font);
			if (text.Length == 0)
			{
				myNumLines = 1;
				return lineHeight;
			}
			if (!Multiline)
			{
				int num = FindFirstLineBreak(text, 0);
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
			}
			StringFormat stringFormat = getStringFormat(null);
			float num2 = 0f;
			myNumLines = 0;
			int num3 = 0;
			int num4 = -1;
			int nextline = 0;
			bool flag = false;
			checked
			{
				while (!flag)
				{
					num4 = FindFirstLineBreak(text, num3, ref nextline);
					if (num4 == -1)
					{
						num4 = text.Length;
						flag = true;
					}
					if (num3 <= num4)
					{
						string text2 = text.Substring(num3, num4 - num3);
						if (text2.Length > 0)
						{
							if (Wrapping)
							{
								SizeF area = new SizeF(maxw, 1E+09f);
								int lines = 0;
								num2 += measureString(text2, g, font, stringFormat, area, out lines).Height;
								myNumLines += lines;
							}
							else
							{
								num2 += lineHeight;
								myNumLines++;
							}
						}
						else
						{
							num2 += lineHeight;
							myNumLines++;
						}
					}
					num3 = nextline;
				}
				return num2;
			}
		}

		private float computeWidth(Graphics g, Font font)
		{
			string text = Text;
			if (text.Length == 0)
			{
				return 0f;
			}
			StringFormat fmt = myStandardStringFormat;
			if (Multiline)
			{
				float num = 0f;
				int num2 = 0;
				bool flag = false;
				int nextline = 0;
				while (!flag)
				{
					int num3 = FindFirstLineBreak(text, num2, ref nextline);
					if (num3 == -1)
					{
						num3 = text.Length;
						flag = true;
					}
					string str = text.Substring(num2, checked(num3 - num2));
					float stringWidth = getStringWidth(str, g, font, fmt);
					if (Wrapping && stringWidth > WrappingWidth)
					{
						return WrappingWidth;
					}
					if (stringWidth > num)
					{
						num = stringWidth;
					}
					num2 = nextline;
				}
				return num;
			}
			int num4 = FindFirstLineBreak(text, 0);
			if (num4 >= 0)
			{
				text = text.Substring(0, num4);
			}
			float stringWidth2 = getStringWidth(text, g, font, fmt);
			if (Wrapping && stringWidth2 > WrappingWidth)
			{
				return WrappingWidth;
			}
			return stringWidth2;
		}

		private Font shareFont(string name, float size, FontStyle style)
		{
			lock (typeof(GoText))
			{
				if (myLastFont != null && myLastFont.Name == name && myLastFont.Size == size && myLastFont.Style == style)
				{
					return myLastFont;
				}
				Font font = makeFont(name, size, style);
				if (font != null)
				{
					myLastFont = font;
				}
				return myLastFont;
			}
		}

		private Font makeFont(string name, float size, FontStyle style)
		{
			byte gdiCharSet = checked((byte)GdiCharSet);
			Font result = null;
			try
			{
				result = new Font(name, size, style, GraphicsUnit.Point, gdiCharSet);
				return result;
			}
			catch (Exception)
			{
				return result;
			}
		}

		private float getLineHeight(Font font)
		{
			return font.GetHeight();
		}

		private float getLineLeading(Font font)
		{
			FontFamily fontFamily = font.FontFamily;
			FontStyle style = font.Style;
			float size = font.Size;
			int lineSpacing = fontFamily.GetLineSpacing(style);
			int emHeight = fontFamily.GetEmHeight(style);
			return size * (float)checked(lineSpacing - emHeight) / (float)emHeight * 2f / 3f;
		}

		private float getStringWidth(string str, Graphics g, Font font, StringFormat fmt)
		{
			return g.MeasureString(str, font, default(PointF), fmt).Width;
		}

		private SizeF measureString(string str, Graphics g, Font font, StringFormat fmt, SizeF area, out int lines)
		{
			int charactersFitted = 0;
			return g.MeasureString(str, font, area, fmt, out charactersFitted, out lines);
		}

		private void drawString(string str, Graphics g, GoView view, Font font, Brush br, RectangleF rect, StringFormat fmt)
		{
			g.DrawString(str, font, br, rect, fmt);
		}

		internal bool isRightToLeft(GoView view)
		{
			if (RightToLeftFromView && view != null)
			{
				return view.RightToLeft == System.Windows.Forms.RightToLeft.Yes;
			}
			return RightToLeft;
		}

		private static StringFormat MakeStandardStringFormat()
		{
			StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
			stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			stringFormat.FormatFlags &= ~StringFormatFlags.LineLimit;
			return stringFormat;
		}

		private StringFormat getStringFormat(GoView view)
		{
			if (myStringFormat == null)
			{
				myStringFormat = new StringFormat(myStandardStringFormat);
			}
			myStringFormat.Trimming = StringTrimming;
			if (StringTrimming == StringTrimming.None)
			{
				myStringFormat.FormatFlags &= ~StringFormatFlags.LineLimit;
			}
			else
			{
				myStringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			}
			int alignment = Alignment;
			if (alignment <= 16)
			{
				switch (alignment)
				{
				case 1:
					goto IL_00bf;
				case 4:
				case 8:
					goto IL_00cd;
				}
				goto IL_00b1;
			}
			if (alignment <= 64)
			{
				if (alignment != 32)
				{
					if (alignment != 64)
					{
						goto IL_00b1;
					}
					goto IL_00cd;
				}
			}
			else if (alignment != 128)
			{
				_ = 256;
				goto IL_00b1;
			}
			goto IL_00bf;
			IL_00cd:
			myStringFormat.Alignment = StringAlignment.Far;
			goto IL_00d9;
			IL_00b1:
			myStringFormat.Alignment = StringAlignment.Near;
			goto IL_00d9;
			IL_00d9:
			if (isRightToLeft(view))
			{
				myStringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			else
			{
				myStringFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
			}
			if (Wrapping)
			{
				myStringFormat.FormatFlags &= ~StringFormatFlags.NoWrap;
			}
			else
			{
				myStringFormat.FormatFlags |= StringFormatFlags.NoWrap;
			}
			return myStringFormat;
			IL_00bf:
			myStringFormat.Alignment = StringAlignment.Center;
			goto IL_00d9;
		}

		/// <summary>
		/// This method can be called to change the size of this object without
		/// changing its location.
		/// </summary>
		/// <param name="s"></param>
		public override void SetSizeKeepingLocation(SizeF s)
		{
			RectangleF bounds = Bounds;
			bounds.Width = s.Width;
			bounds.Height = s.Height;
			PointF location = Location;
			RectangleF rectangleF2 = Bounds = SetRectangleSpotLocation(bounds, Alignment, location);
		}

		/// <summary>
		/// Consider any shadow when calculating the paint bounds.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public override RectangleF ExpandPaintBounds(RectangleF rect, GoView view)
		{
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
			GoObject.InflateRect(ref rect, Math.Max(rect.Height / 3f, 2f), 1f);
			return rect;
		}

		/// <summary>
		/// Implement a custom selection indicator if BackgroundOpaqueWhenSelected is true.
		/// </summary>
		/// <param name="sel"></param>
		/// <param name="selectedObj"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoText.BackgroundOpaqueWhenSelected" /> is true, selecting this object
		/// will cause the background to be shown; unselecting will make the
		/// background transparent again.
		/// This produces the effect of highlighting the text when selected,
		/// if the text normally has a transparent background and the
		/// <see cref="P:Northwoods.Go.GoText.BackgroundColor" /> is a suitable highlight color.
		/// Changes to the <see cref="P:Northwoods.Go.GoText.TransparentBackground" /> property are not recorded
		/// by the <see cref="T:Northwoods.Go.GoUndoManager" />.
		/// </remarks>
		public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
		{
			if (BackgroundOpaqueWhenSelected)
			{
				base.RemoveSelectionHandles(sel);
				bool skipsUndoManager = base.SkipsUndoManager;
				base.SkipsUndoManager = true;
				TransparentBackground = false;
				base.SkipsUndoManager = skipsUndoManager;
			}
			else
			{
				base.AddSelectionHandles(sel, selectedObj);
			}
		}

		/// <summary>
		/// Implement a custom selection indicator if BackgroundOpaqueWhenSelected is true.
		/// </summary>
		/// <param name="sel"></param>
		/// <seealso cref="M:Northwoods.Go.GoText.AddSelectionHandles(Northwoods.Go.GoSelection,Northwoods.Go.GoObject)" />
		public override void RemoveSelectionHandles(GoSelection sel)
		{
			if (BackgroundOpaqueWhenSelected)
			{
				bool skipsUndoManager = base.SkipsUndoManager;
				base.SkipsUndoManager = true;
				TransparentBackground = true;
				base.SkipsUndoManager = skipsUndoManager;
			}
			base.RemoveSelectionHandles(sel);
		}

		/// <summary>
		/// Handle any change in size.
		/// </summary>
		/// <param name="old"></param>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoObject.AutoRescales" /> is true, we rescale the font size
		/// so that the text fits in this object's bounds.
		/// </remarks>
		protected override void OnBoundsChanged(RectangleF old)
		{
			base.OnBoundsChanged(old);
			SizeF size = base.Size;
			if (old.Width != size.Width || old.Height != size.Height)
			{
				UpdateScale();
			}
		}

		/// <summary>
		/// Handle any change in layers.
		/// </summary>
		/// <param name="oldlayer"></param>
		/// <param name="newlayer"></param>
		/// <param name="mainObj"></param>
		/// <remarks>
		/// If the layer changes, make sure we are no longer editing the text.
		/// </remarks>
		protected override void OnLayerChanged(GoLayer oldlayer, GoLayer newlayer, GoObject mainObj)
		{
			base.OnLayerChanged(oldlayer, newlayer, mainObj);
			if (Editor != null)
			{
				GoView view = Editor.View;
				if (view != null)
				{
					DoEndEdit(view);
				}
			}
		}

		/// <summary>
		/// Start editing this text object on a single click if this object is editable.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="view"></param>
		/// <returns>
		/// True if it started editing; false if it could not.
		/// </returns>
		/// <remarks>
		/// To be able to call <see cref="M:Northwoods.Go.GoObject.DoBeginEdit(Northwoods.Go.GoView)" />, both <see cref="M:Northwoods.Go.GoObject.CanEdit" />
		/// and the <paramref name="view" />'s <see cref="M:Northwoods.Go.GoView.CanEditObjects" />
		/// property must both be true.
		/// </remarks>
		public override bool OnSingleClick(GoInputEventArgs evt, GoView view)
		{
			if (!CanEdit())
			{
				return false;
			}
			if (!view.CanEditObjects())
			{
				return false;
			}
			if (evt.Shift || evt.Control)
			{
				return false;
			}
			if (EditableWhenSelected)
			{
				GoTool goTool = view.Tool as GoTool;
				if (goTool != null && goTool.CurrentObjectWasSelected)
				{
					DoBeginEdit(view);
				}
			}
			else
			{
				DoBeginEdit(view);
			}
			return true;
		}

		/// <summary>
		/// Bring up a TextBox or other Control to allow the user to edit the text string in-place.
		/// </summary>
		/// <param name="view"></param>
		/// <remarks>
		/// <para>
		/// This is responsible for calling <see cref="M:Northwoods.Go.GoView.StartTransaction" />.
		/// (<see cref="M:Northwoods.Go.GoText.DoEndEdit(Northwoods.Go.GoView)" /> is responsible for finishing the transaction.)
		/// This basically sets both <see cref="P:Northwoods.Go.GoText.Editor" /> and
		/// <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" /> to the result
		/// from calling <see cref="M:Northwoods.Go.GoText.CreateEditor(Northwoods.Go.GoView)" />.
		/// However, this method does nothing if an edit is already in
		/// progress, when <see cref="P:Northwoods.Go.GoText.Editor" /> is not null.
		/// </para>
		/// <para>
		/// If you override this method, you can perform some customization of
		/// the resulting <c>Control</c> for the given <see cref="T:Northwoods.Go.GoView" /> by
		/// first calling the base method and then looking at the <c>Control</c>,
		/// as follows:
		/// <pre><code>
		/// public override void DoBeginEdit(GoView view) {
		///   base.DoBeginEdit(view);
		///   if (this.Editor == null) return;  // failed to create editor
		///   // get the Control for the Editor created in base method for this GoText
		///   System.Windows.Forms.Control ctrl = this.Editor.GetControl(view);
		///   if (ctrl != null) {
		///     ... simple Control customization ...
		///   }
		/// }
		/// </code></pre>
		/// </para>
		/// <para>
		/// However, many <c>Control</c>s require more sophisticated behavior,
		/// which is best implemented by inheriting from that particular <c>Control</c>
		/// class, implementing <see cref="T:Northwoods.Go.IGoControlObject" />, and handling the
		/// desired events.
		/// Normally that is done by overriding <see cref="M:Northwoods.Go.GoText.CreateEditor(Northwoods.Go.GoView)" />
		/// instead of overriding this method.
		/// </para>
		/// </remarks>
		public override void DoBeginEdit(GoView view)
		{
			if (view != null && Editor == null)
			{
				try
				{
					view.StartTransaction();
					RemoveSelectionHandles(view.Selection);
					myEditor = CreateEditor(view);
					Editor.EditedObject = this;
					view.EditControl = Editor;
					Editor.GetControl(view)?.Focus();
				}
				catch (VerificationException ex)
				{
					GoObject.Trace("GoText DoBeginEdit: " + ex.ToString());
					view.EditControl = null;
					myEditor = null;
					view.AbortTransaction();
				}
				catch (SecurityException ex2)
				{
					GoObject.Trace("GoText DoBeginEdit: " + ex2.ToString());
					view.EditControl = null;
					myEditor = null;
					view.AbortTransaction();
				}
			}
		}

		/// <summary>
		/// This creates a <see cref="T:Northwoods.Go.GoControl" /> that will create a <c>TextBox</c>,
		/// a <c>ComboBox</c>, or a <c>NumericUpDown</c> when the <see cref="T:Northwoods.Go.GoControl" />
		/// is painted in a <see cref="T:Northwoods.Go.GoView" />.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// This is responsible for creating a <see cref="T:Northwoods.Go.GoControl" /> with
		/// the appropriate <see cref="P:Northwoods.Go.GoControl.ControlType" /> and bounds.
		/// This method is called by <see cref="M:Northwoods.Go.GoText.DoBeginEdit(Northwoods.Go.GoView)" />.
		/// If you want to set some properties on the <c>Control</c> created
		/// by the <see cref="T:Northwoods.Go.GoControl" /> for a <see cref="T:Northwoods.Go.GoView" />, it is
		/// probably easiest to override the <see cref="M:Northwoods.Go.GoText.DoBeginEdit(Northwoods.Go.GoView)" /> method.
		/// </para>
		/// <para>
		/// For more general customization you will define a subclass of <c>Control</c>
		/// and override this method as follows:
		/// <pre><code>
		/// public override GoControl CreateEditor(GoView view) {
		///   GoControl editor = base.CreateEditor(view);
		///   if (editor != null) editor.ControlType = typeof(MyCustomControl);
		///   return editor;
		/// }
		/// </code></pre>
		/// You may also want to adjust the <see cref="P:Northwoods.Go.GoObject.Bounds" /> of the
		/// resulting <see cref="T:Northwoods.Go.GoControl" />.
		/// </para>
		/// <para>
		/// Your subclass of <c>Control</c>, <c>MyCustomControl</c>, which should implement
		/// the <see cref="T:Northwoods.Go.IGoControlObject" /> interface, should be responsible
		/// for initializing itself correctly for this <c>GoText</c> object.
		/// That initialization is normally done when <see cref="P:Northwoods.Go.IGoControlObject.GoControl" />
		/// is set.
		/// It will of course want to examine this <see cref="T:Northwoods.Go.GoText" /> object being edited;
		/// you can get that with the following expression:
		/// <pre><code>
		///   this.GoControl.EditedObject as GoText
		/// </code></pre>
		/// </para>
		/// <para>
		/// When your <c>Control</c> is finished editing, it may want to save its results
		/// by modifying this <see cref="T:Northwoods.Go.GoText" /> object.  In particular, if you want to
		/// change the <see cref="P:Northwoods.Go.GoText.Text" /> property, it is best to do something like:
		/// <pre><code>
		///   GoControl goctrl = this.GoControl;
		///   if (goctrl != null) {
		///     GoText gotext = goctrl.EditedObject as GoText;
		///     if (gotext == null || gotext.DoEdit(this.GoView, gotext.Text, newtextvalue)) {
		///       goctrl.DoEndEdit(this.GoView);
		///     }
		///   }
		/// </code></pre>
		/// This calls <see cref="M:Northwoods.Go.GoText.DoEdit(Northwoods.Go.GoView,System.String,System.String)" /> which permits validation using both the old and the
		/// proposed new text strings, before actually setting the <see cref="P:Northwoods.Go.GoText.Text" /> property.
		/// </para>
		/// <para>
		/// Your <see cref="T:Northwoods.Go.IGoControlObject" /> <c>Control</c> should terminate by calling:
		/// <pre><code>
		///   this.GoControl.DoEndEdit(this.GoView)
		/// </code></pre>
		/// in its event handlers that cause it to finish.
		/// </para>
		/// </remarks>
		public override GoControl CreateEditor(GoView view)
		{
			GoControl goControl = new GoControl();
			float num = 1f;
			float num2 = 1f;
			if (view != null)
			{
				num = view.WorldScale.Width;
				num2 = view.WorldScale.Height;
			}
			if (EditorStyle == GoTextEditorStyle.NumericUpDown)
			{
				goControl.ControlType = typeof(NumericUpDownControl);
				RectangleF bounds = Bounds;
				bounds.X -= 2f / num;
				bounds.Y -= 2f / num2;
				bounds.Width += 36f / num;
				bounds.Height += 8f / num2;
				goControl.Bounds = bounds;
				return goControl;
			}
			if (EditorStyle == GoTextEditorStyle.ComboBox)
			{
				goControl.ControlType = typeof(ComboBoxControl);
				RectangleF bounds2 = Bounds;
				bounds2.X -= 2f / num;
				bounds2.Y -= 2f / num2;
				bounds2.Width += 4f / num;
				bounds2.Height += 4f / num2;
				if (view != null)
				{
					StringFormat stringFormat = getStringFormat(view);
					float width = bounds2.Width;
					width *= view.DocScale;
					Graphics graphics = view.CreateGraphics();
					Font font = Font;
					float num3 = font.Size;
					if (view != null)
					{
						num3 *= view.DocScale / num2;
					}
					Font font2 = makeFont(font.Name, num3, font.Style);
					if (graphics != null)
					{
						foreach (object choice in Choices)
						{
							string str = choice.ToString();
							width = Math.Max(width, getStringWidth(str, graphics, font2, stringFormat));
						}
						graphics.Dispose();
					}
					font2.Dispose();
					width += 30f / num;
					bounds2.Width = width / view.DocScale;
				}
				goControl.Bounds = bounds2;
				return goControl;
			}
			goControl.ControlType = typeof(TextBoxControl);
			RectangleF bounds3 = Bounds;
			bounds3.X -= 2f / num;
			bounds3.Y -= 2f / num2;
			bounds3.Width += 4f / num;
			bounds3.Height += 4f / num2;
			if (Multiline || Wrapping)
			{
				bounds3.Height += getLineHeight(Font) * 2f;
			}
			int alignment;
			if (Wrapping)
			{
				alignment = Alignment;
				if (alignment <= 16)
				{
					switch (alignment)
					{
					case 1:
						goto IL_0351;
					case 4:
					case 8:
						goto IL_0384;
					}
					goto IL_031e;
				}
				if (alignment <= 64)
				{
					if (alignment != 32)
					{
						if (alignment != 64)
						{
							goto IL_031e;
						}
						goto IL_0384;
					}
				}
				else if (alignment != 128)
				{
					_ = 256;
					goto IL_031e;
				}
				goto IL_0351;
			}
			alignment = Alignment;
			if (alignment <= 16)
			{
				switch (alignment)
				{
				case 1:
					goto IL_0452;
				case 4:
				case 8:
					goto IL_0469;
				}
				goto IL_0432;
			}
			if (alignment <= 64)
			{
				if (alignment != 32)
				{
					if (alignment != 64)
					{
						goto IL_0432;
					}
					goto IL_0469;
				}
			}
			else if (alignment != 128)
			{
				_ = 256;
				goto IL_0432;
			}
			goto IL_0452;
			IL_0469:
			if (!isRightToLeft(view))
			{
				bounds3.X -= 30f / num;
			}
			goto IL_0487;
			IL_0384:
			if (!isRightToLeft(view))
			{
				bounds3.X = bounds3.X + bounds3.Width - WrappingWidth - 2f / num;
			}
			goto IL_03b2;
			IL_049c:
			goControl.Bounds = bounds3;
			return goControl;
			IL_0487:
			bounds3.Width += 30f / num;
			goto IL_049c;
			IL_0452:
			bounds3.X -= 15f / num;
			goto IL_0487;
			IL_031e:
			if (isRightToLeft(view))
			{
				bounds3.X = bounds3.X + bounds3.Width - WrappingWidth - 2f / num;
			}
			goto IL_03b2;
			IL_03b2:
			bounds3.Width = Math.Max(WrappingWidth + 4f / num, bounds3.Width);
			goto IL_049c;
			IL_0351:
			bounds3.X = bounds3.X + bounds3.Width / 2f - WrappingWidth / 2f - 2f / num;
			goto IL_03b2;
			IL_0432:
			if (isRightToLeft(view))
			{
				bounds3.X -= 30f / num;
			}
			goto IL_0487;
		}

		/// <summary>
		/// Stop editing this text object.
		/// </summary>
		/// <param name="view"></param>
		/// <remarks>
		/// <para>
		/// This sets <see cref="P:Northwoods.Go.GoText.Editor" /> to null and removes the <see cref="T:Northwoods.Go.GoControl" />
		/// from the view by setting <see cref="T:Northwoods.Go.GoView" />.<see cref="P:Northwoods.Go.GoView.EditControl" /> to null.
		/// This is also responsible for calling <see cref="M:Northwoods.Go.GoView.RaiseObjectEdited(Northwoods.Go.GoObject)" />
		/// and <see cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />.
		/// However, this method does nothing if no edit is in progress,
		/// when <see cref="P:Northwoods.Go.GoText.Editor" /> is already null.
		/// </para>
		/// <para>
		/// If you override this method, it might be easiest to first do
		/// whatever cleanup you need before calling the base method.
		/// </para>
		/// </remarks>
		public override void DoEndEdit(GoView view)
		{
			if (Editor != null)
			{
				Editor.EditedObject = null;
				if (view != null)
				{
					view.EditControl = null;
				}
				myEditor = null;
				if (view != null)
				{
					view.RaiseObjectEdited(this);
					view.FinishTransaction("Text Edit");
				}
			}
		}

		/// <summary>
		/// Called when a user tries to commit a text edit.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="oldtext"></param>
		/// <param name="newtext"></param>
		/// <returns>
		/// true to call <see cref="M:Northwoods.Go.GoText.DoEndEdit(Northwoods.Go.GoView)" /> to stop editing; false to continue editing.
		/// The default value is true.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This is normally called from the control that is setting a new string for
		/// this text object as a result of the user having finished editing the text.
		/// By default it just sets this object's <see cref="P:Northwoods.Go.GoText.Text" /> property
		/// to the result of a call to <see cref="M:Northwoods.Go.GoText.ComputeEdit(System.String,System.String)" />:
		/// <c>Text = ComputeEdit(oldtext, newtext)</c>.
		/// You can override this method to prevent the text value from being set,
		/// or you can override <see cref="M:Northwoods.Go.GoText.ComputeEdit(System.String,System.String)" /> to constrain the new
		/// text value.
		/// If you intend to bring up a message box or other focus-taking activity,
		/// call <see cref="T:Northwoods.Go.GoView" />.<see cref="M:Northwoods.Go.GoView.DoCancelMouse" /> first,
		/// to make sure no tool is started if the user had clicked elsewhere in the view.
		/// </para>
		/// <para>
		/// Call <see cref="M:Northwoods.Go.GoText.DoBeginEdit(Northwoods.Go.GoView)" /> to programmatically have the user start
		/// editing this text object; call <see cref="M:Northwoods.Go.GoText.DoEndEdit(Northwoods.Go.GoView)" /> to stop any
		/// editing that the user might be doing.  However, you should not be calling
		/// either of those methods from an override of this method.
		/// </para>
		/// </remarks>
		public virtual bool DoEdit(GoView view, string oldtext, string newtext)
		{
			string text2 = Text = ComputeEdit(oldtext, newtext);
			return true;
		}

		/// <summary>
		/// Calculate a new string value for this object as a result of an edit.
		/// </summary>
		/// <param name="oldtext"></param>
		/// <param name="newtext"></param>
		/// <returns>Normally, this just returns <paramref name="newtext" />.</returns>
		/// <remarks>
		/// This is normally called from <see cref="M:Northwoods.Go.GoText.DoEdit(Northwoods.Go.GoView,System.String,System.String)" />, and can be used for
		/// normalizing or canonicalizing the new string value, even when not
		/// interactively editing (i.e. no <see cref="T:Northwoods.Go.GoView" /> is not available).
		/// One typical usage is to make sure the new value is a unique string
		/// within some context such as the document.
		/// </remarks>
		public virtual string ComputeEdit(string oldtext, string newtext)
		{
			return newtext;
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1501:
				Text = (string)e.GetValue(undo);
				break;
			case 1502:
				FamilyName = (string)e.GetValue(undo);
				break;
			case 1503:
				FontSize = e.GetFloat(undo);
				break;
			case 1504:
				Alignment = e.GetInt(undo);
				break;
			case 1505:
				TextColor = (Color)e.GetValue(undo);
				break;
			case 1506:
				BackgroundColor = (Color)e.GetValue(undo);
				break;
			case 1507:
				TransparentBackground = (bool)e.GetValue(undo);
				break;
			case 1508:
				Bold = (bool)e.GetValue(undo);
				break;
			case 1509:
				Italic = (bool)e.GetValue(undo);
				break;
			case 1510:
				Underline = (bool)e.GetValue(undo);
				break;
			case 1511:
				StrikeThrough = (bool)e.GetValue(undo);
				break;
			case 1512:
				Multiline = (bool)e.GetValue(undo);
				break;
			case 1516:
				Clipping = (bool)e.GetValue(undo);
				break;
			case 1518:
				AutoResizes = (bool)e.GetValue(undo);
				break;
			case 1515:
				BackgroundOpaqueWhenSelected = (bool)e.GetValue(undo);
				break;
			case 1520:
				Wrapping = (bool)e.GetValue(undo);
				break;
			case 1521:
				WrappingWidth = e.GetFloat(undo);
				break;
			case 1522:
				GdiCharSet = e.GetInt(undo);
				break;
			case 1523:
				EditorStyle = (GoTextEditorStyle)e.GetInt(undo);
				break;
			case 1524:
				Minimum = e.GetInt(undo);
				break;
			case 1525:
				Maximum = e.GetInt(undo);
				break;
			case 1526:
				DropDownList = (bool)e.GetValue(undo);
				break;
			case 1527:
				Choices = (IList)e.GetValue(undo);
				break;
			case 1528:
				RightToLeft = (bool)e.GetValue(undo);
				break;
			case 1529:
				RightToLeftFromView = (bool)e.GetValue(undo);
				break;
			case 1530:
				Bordered = (bool)e.GetValue(undo);
				break;
			case 1531:
				StringTrimming = (StringTrimming)e.GetInt(undo);
				break;
			case 1532:
				EditableWhenSelected = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
