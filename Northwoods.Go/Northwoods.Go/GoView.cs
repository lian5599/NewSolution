using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// GoView is a Control that provides display and editing of abstract graphs or
	/// networks of 2D graphical objects.
	/// </summary>
	/// <remarks>
	/// Read the User Guide and FAQ for more details.
	/// </remarks>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(GoView), "Northwoods.Go.GoView.bmp")]
	public class GoView : Control, INotifyPropertyChanged, IGoLayerCollectionContainer, IGoLayerAbilities
	{
		[Serializable]
		internal sealed class PrintInfo
		{
			internal RectangleF DocRect;

			internal float HorizScale;

			internal float VertScale;

			internal SizeF PrintSize;

			internal int NumPagesAcross;

			internal int NumPagesDown;

			internal int CurPage;
		}

		private sealed class DiagramLicense
		{
			[NonSerialized]
			internal Font myFont;

			[NonSerialized]
			internal SolidBrush myBrush;

			public DiagramLicense(GoView view)
			{
				string text = "Northwoods.Go.GoView.LicenseKey has not yet been set; running with an evaluation license.";
				checked
				{
					try
					{
						string fullName = Assembly.GetExecutingAssembly().FullName;
						string text2 = fullName.Substring(0, fullName.IndexOf(','));
						if (text2 == "Northwoods.Go" || text2 == "Northwoods.GoRT")
						{
							text2 = "Northwoods.GoWin";
						}
						string text3 = "";
						string text4 = "";
						Random random = new Random();
						int val = 80 + random.Next(100);
						val = Math.Min(255, Math.Max(0, val));
						int val2 = 80 + random.Next(100);
						val2 = Math.Min(255, Math.Max(0, val2));
						int val3 = 80 + random.Next(100);
						val3 = Math.Min(255, Math.Max(0, val3));
						val3 &= -8;
						myBrush = new SolidBrush(Color.FromArgb(255, val, val2, val3));
						myFont = new Font(FontFamily.GenericSansSerif, 8 + random.Next(4));
						if (text4 == null || text4 == "")
						{
							text4 = LicenseKey;
							if (text4 == null || text4 == "")
							{
								text4 = FindSavedLicenseKey(1, text2);
								if (text4 == null || text4 == "")
								{
									text4 = FindSavedLicenseKey(2, text2);
								}
							}
						}
						if (text4 != null && text4 != "")
						{
							string text5 = null;
							text5 = "Northwoods.GoXam";
							text = "Component assembly has been modified [" + fullName + "]";
							string text6 = fullName.Substring(fullName.LastIndexOf('=') + 1);
							string value = string.Format(CultureInfo.InvariantCulture, "{0:X}", new object[1]
							{
								2766387127u
							}) + "01" + string.Format(CultureInfo.InvariantCulture, "{0:X}", new object[1]
							{
								6410216
							});
							if (text6.Equals(value, StringComparison.OrdinalIgnoreCase))
							{
								text = "Invalid GoView.LicenseKey value: " + text4;
								string versionName = VersionName;
								text = text + "\n  component: " + text2 + " " + versionName + " [" + fullName + "]";
								string[] array = versionName.Split('.');
								int num = int.Parse(array[0], NumberFormatInfo.InvariantInfo);
								int num2 = int.Parse(array[1], NumberFormatInfo.InvariantInfo);
								string value2 = null;
								string value3 = null;
								text3 = myCAN;
								try
								{
									value2 = Environment.MachineName;
								}
								catch (Exception)
								{
								}
								try
								{
									value3 = Environment.UserName;
								}
								catch (Exception)
								{
								}
								string text7 = "";
								byte[] bytes = Encoding.UTF8.GetBytes(text5);
								ICryptoTransform transform = new AesCryptoServiceProvider().CreateDecryptor(bytes, bytes);
								using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(text4)))
								{
									byte[] array2 = new byte[text4.Length + 1];
									using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
									{
										cryptoStream.Read(array2, 0, array2.Length);
										int i;
										for (i = 0; i < array2.Length && array2[i] != 0; i++)
										{
										}
										text7 = Encoding.UTF8.GetString(array2, 0, i);
									}
								}
								string[] array3 = text7.Split('|');
								if (array3.Length > 7)
								{
									string text8 = array3[0];
									string text9 = array3[3];
									int num3 = int.Parse(array3[4], NumberFormatInfo.InvariantInfo);
									int num4 = int.Parse(array3[5], NumberFormatInfo.InvariantInfo);
									string text10 = array3[6];
									string text11 = array3[7];
									if (text9.Equals(text2, StringComparison.OrdinalIgnoreCase) && (num < num3 || (num == num3 && num2 <= num4)) && (text8.Equals(text3, StringComparison.OrdinalIgnoreCase) || text10.Equals(value2, StringComparison.OrdinalIgnoreCase) || text11.Equals(value3, StringComparison.OrdinalIgnoreCase)))
									{
										val3 |= 7;
										myBrush = new SolidBrush(Color.FromArgb(255, val, val2, val3));
										return;
									}
									text = text + "\n  licensed component: " + text9 + " " + num3.ToString(CultureInfo.InvariantCulture) + "." + num4.ToString(CultureInfo.InvariantCulture);
									text = text + "\n  licensed application: " + text8;
									if (!text9.Equals(text2, StringComparison.OrdinalIgnoreCase))
									{
										text = text + "\nPlease set GoView.LicenseKey to a run-time license key for " + text2;
									}
									if (num >= num3 && (num != num3 || num2 > num4))
									{
										text = text + "\nPlease upgrade the GoView.LicenseKey to at least version " + num.ToString(CultureInfo.InvariantCulture) + "." + num2.ToString(CultureInfo.InvariantCulture);
									}
									if (!text8.Equals(text3, StringComparison.OrdinalIgnoreCase))
									{
										text = text + "\nPlease set GoView.LicenseKey to a run-time license key for your application: " + text3;
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
					string text12 = ((text.IndexOf("evaluation", StringComparison.OrdinalIgnoreCase) > 0) ? "\n" : "\nNorthwoods.Go license error:\n") + text + "\nSee GoDiagram User Guide for details";
					GoObject.Trace(text12);
					try
					{
						EventLog.WriteEntry("Northwoods", text12, EventLogEntryType.Warning);
					}
					catch
					{
					}
				}
			}

			public void Dispose()
			{
				if (myFont != null)
				{
					myFont.Dispose();
					myFont = null;
				}
				if (myBrush != null)
				{
					myBrush.Dispose();
					myBrush = null;
				}
			}

			internal void Dispose(GoView view)
			{
				PaintEventArgs myPaintEventArgs = view.myPaintEventArgs;
				Rectangle clipRectangle = myPaintEventArgs.ClipRectangle;
				myPaintEventArgs.Graphics.DrawImage(view.myBuffer, clipRectangle, clipRectangle, GraphicsUnit.Pixel);
				if ((myBrush.Color.B & 7) < 4)
				{
					myPaintEventArgs.Graphics.DrawString("", myFont, myBrush, 10f, 35f);
				}
			}

			private static string FindSavedLicenseKey(int where, string compname)
			{
				string text = null;
				try
				{
					RegistryKey registryKey = null;
					switch (where)
					{
					case 2:
						registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Northwoods Software\\GoDiagram");
						if (registryKey != null)
						{
							text = (registryKey.GetValue(compname) as string);
						}
						break;
					case 1:
						registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Northwoods Software\\GoDiagram");
						if (registryKey != null)
						{
							text = (registryKey.GetValue(compname) as string);
						}
						break;
					case 0:
					{
						using (TextReader textReader = new StreamReader(compname + ".lic"))
						{
							text = textReader.ReadLine();
						}
						break;
					}
					}
				}
				catch (Exception)
				{
				}
				if (text != null)
				{
					return text.Trim();
				}
				return "";
			}
		}

		internal sealed class DefaultLayer
		{
			private Random _Random = new Random();

			internal const string S1 = "Northwoods.Go.GoView.LicenseKey has not yet been set; running with an evaluation license.";

			internal const string S2 = "Component assembly has been modified ";

			internal const string S3 = "{0:X}";

			internal const string S4 = "Invalid GoView.LicenseKey value: ";

			internal const string S5 = "\n  component: ";

			internal const string S6 = "\nno Deployment.Current";

			internal const string S7 = "\nno Application.Current";

			internal const string S8 = "\n  licensed component: ";

			internal const string S9 = "\n  licensed application: ";

			internal const string S10 = "\nPlease set GoView.LicenseKey to a run-time license key for ";

			internal const string S11 = "\nPlease upgrade the GoView.LicenseKey to at least version ";

			internal const string S12 = "\nPlease set GoView.LicenseKey to a run-time license key for your application: ";

			internal const string S13 = "evaluation";

			internal const string S14 = "\nNorthwoods.Go license error:\n";

			internal const string S15 = "\nSee GoDiagram User Guide for details";

			internal const string S17 = "SOFTWARE\\Northwoods Software\\GoDiagram";

			internal const string S18 = ".lic";

			internal const string S19 = "\nPlease use an application class that inherits from System.Windows.Application";

			internal const string S40 = "\nno appname available";

			internal const string S41 = "\nApplication name from GetCurrentProcess: ";

			internal const string S42 = "\nApplication name from Request.ApplicationPath: ";

			internal const string S45 = "\nSuccessful Activation";

			internal const string S46 = "\nApplication name from GetCallingAssembly: ";

			internal const string SNL = "\n";

			internal const string SDOT = ".";

			internal const string SSBO = "[";

			internal const string SSBC = "]";

			internal const string SSP = " ";

			internal const char CEQ = '=';

			internal const char CDOT = '.';

			internal const char CCOM = ',';

			internal const char CVB = '|';

			internal const char CSLASH = '/';
		}

		internal static float myVersion = -1f;

		internal static int myVersionMajor;

		internal static int myVersionMinor;

		internal static string myVersionAssembly = "";

		private static string myLicenseKey;

		internal static string myCAN = "";

		private static Dictionary<string, GoDocument> myClipboard = new Dictionary<string, GoDocument>();

		private bool myUpdatingScrollBars = true;

		private ScrollEventHandler myVertScrollHandler;

		private ScrollEventHandler myHorizScrollHandler;

		private GoViewScrollBarVisibility myShowVertScroll = GoViewScrollBarVisibility.IfNeeded;

		private GoViewScrollBarVisibility myShowHorizScroll = GoViewScrollBarVisibility.IfNeeded;

		private Control myTopLeftCorner;

		private Control myTopRightCorner;

		private Control myBottomRightCorner;

		private Control myBottomLeftCorner;

		private Control myTopBar;

		private Control myRightBar;

		private Control myBottomBar;

		private Control myLeftBar;

		[NonSerialized]
		private EventHandler mySafeOnDocumentChangedDelegate;

		[NonSerialized]
		private Queue<GoChangedEventArgs> myQueuedEvents;

		private bool myAllowDragOut = true;

		private GoObject myExternalDragImage;

		[NonSerialized]
		private bool myPretendsInternalDrag;

		private bool myExternalDragDropsOnEnter;

		[NonSerialized]
		internal PaintEventArgs myPaintEventArgs;

		private int mySuppressPaint;

		private Size myAutoScrollRegion = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);

		private int myAutoScrollTime = 100;

		private int myAutoScrollDelay = 1000;

		[NonSerialized]
		private System.Threading.Timer myAutoScrollTimer;

		[NonSerialized]
		private bool myAutoScrollTimerEnabled;

		[NonSerialized]
		private Point myAutoScrollPoint;

		[NonSerialized]
		private bool myActioning;

		[NonSerialized]
		private bool myPanning;

		[NonSerialized]
		private Point myPanningOrigin;

		[NonSerialized]
		private ToolTip myToolTip;

		private string myToolTipText = "";

		[NonSerialized]
		private Cursor myDefaultCursor;

		[NonSerialized]
		private System.Threading.Timer myHoverTimer;

		[NonSerialized]
		private bool myHoverTimerEnabled;

		private int myHoverDelay = 1000;

		private Point myHoverPoint;

		[NonSerialized]
		private FormWindowState myWindowState;

		[NonSerialized]
		private PrintInfo myPrintInfo;

		private float myPrintScale = 0.8f;

		private bool myPrintsViewObjects;

		private bool myDrawsXorMode;

		[NonSerialized]
		private Rectangle myPrevXorRect;

		[NonSerialized]
		private bool myPrevXorRectValid;

		[NonSerialized]
		private GoRectangle myMarquee;

		[NonSerialized]
		private GoControl myEditControl;

		[NonSerialized]
		private List<GoControl> myGoControls;

		[NonSerialized]
		private Control myModalControl;

		[NonSerialized]
		private bool myCancelMouseDown;

		private ImageList myImageList;

		private BorderStyle myBorderStyle = BorderStyle.Fixed3D;

		private Border3DStyle myBorder3DStyle = Border3DStyle.Etched;

		private Size myBorderSize = SystemInformation.Border3DSize;

		private GoDocument myDocument;

		[NonSerialized]
		private GoChangedEventHandler myDocChangedEventHandler;

		private GoSelection mySelection;

		private int myMaximumSelectionCount = 999999;

		private GoPickInRectangleStyle mySelectInRectangleStyle = GoPickInRectangleStyle.SelectableOnlyContained;

		private Color myPrimarySelectionColor = Color.Chartreuse;

		private Color mySecondarySelectionColor = Color.Cyan;

		private Color myNoFocusSelectionColor = Color.LightGray;

		private SizeF myResizeHandleSize = new SizeF(6f, 6f);

		private float myResizeHandlePenWidth = 1f;

		private float myBoundingHandlePenWidth = 2f;

		private bool myHidesSelection;

		private GoViewDisableKeys myDisableKeys = GoViewDisableKeys.ArrowMove;

		private float myArrowMoveSmall = 1f;

		private float myArrowMoveLarge = 10f;

		private GoLayerCollection myLayers;

		private GoLayer myBackgroundLayer;

		private Size myScrollSmallChange = new Size(16, 16);

		private Size myAutoPanRegion = new Size(16, 16);

		private bool myShowsNegativeCoordinates = true;

		private PointF myOrigin;

		private float myHorizScale = 1f;

		private float myVertScale = 1f;

		private Size myViewSize = new Size(-1, -1);

		private Rectangle myDisplayRectangle = new Rectangle(1, 1, -999999, -999999);

		[NonSerialized]
		private float myHorizWorld = 1f;

		[NonSerialized]
		private float myVertWorld = 1f;

		private PointF myPreviousCenter;

		private SmoothingMode mySmoothingMode = SmoothingMode.HighQuality;

		private TextRenderingHint myTextRenderingHint = TextRenderingHint.ClearTypeGridFit;

		private InterpolationMode myInterpolationMode = InterpolationMode.HighQualityBicubic;

		private CompositingQuality myCompositingQuality = CompositingQuality.AssumeLinear;

		private PixelOffsetMode myPixelOffsetMode = PixelOffsetMode.HighQuality;

		private bool myAllowSelect = true;

		private bool myAllowMove = true;

		private bool myAllowCopy = true;

		private bool myAllowResize = true;

		private bool myAllowReshape = true;

		private bool myAllowDelete = true;

		private bool myAllowInsert = true;

		private bool myAllowLink = true;

		private bool myAllowEdit = true;

		private bool myAllowMouse = true;

		private bool myAllowKey = true;

		private DiagramLicense myCurrentResult;

		[NonSerialized]
		internal Bitmap myBuffer;

		[NonSerialized]
		private SolidBrush myBackgroundBrush;

		[NonSerialized]
		private PointF[][] myTempArrays;

		[NonSerialized]
		private List<GoObject> myUpdateHandles = new List<GoObject>();

		[NonSerialized]
		private bool myIsRenderingBitmap;

		private GoInputEventArgs myFirstInput = new GoInputEventArgs();

		private GoInputEventArgs myLastInput = new GoInputEventArgs();

		private IGoTool myTool;

		private IGoTool myDefaultTool;

		private List<IGoTool> myMouseDownTools;

		private List<IGoTool> myMouseMoveTools;

		private List<IGoTool> myMouseUpTools;

		private bool myDragsRealtime;

		private bool myDragRoutesRealtime = true;

		private bool mySelectionDropRejectOverValid;

		private GoObject mySelectionDropRejectOver;

		private float myPortGravity = 100f;

		private GoObject myNewLinkPrototype = new GoLink();

		private GoGrid myBackgroundGrid;

		private GoSheet mySheet;

		private GoViewSheetStyle mySheetStyle;

		private Size mySheetRoom = new Size(10, 10);

		private SizeF myShadowOffset = new SizeF(5f, 5f);

		private Color myShadowColor = Color.FromArgb(127, Color.Gray);

		[NonSerialized]
		private SolidBrush myShadowBrush;

		[NonSerialized]
		private Pen myShadowPen;

		private float myPaintNothingScale = 0.13f;

		private float myPaintGreekScale = 0.24f;

		/// <summary>
		/// Return the version of GoDiagram being used.
		/// </summary>
		public static float Version
		{
			get
			{
				if (myVersion < 0f)
				{
					string text = VersionName;
					int num = text.IndexOf('.');
					if (num >= 0)
					{
						int num2 = text.IndexOf('.', checked(num + 1));
						if (num2 > 0)
						{
							text = text.Substring(0, num2);
						}
					}
					try
					{
						myVersion = float.Parse(text, NumberFormatInfo.InvariantInfo);
					}
					catch (ArithmeticException)
					{
					}
				}
				return myVersion;
			}
		}

		/// <summary>
		/// Return the name of the version of GoDiagram being used.
		/// </summary>
		public static string VersionName
		{
			get
			{
				checked
				{
					if (myVersionAssembly == null || myVersionAssembly.Length == 0)
					{
						string fullName = Assembly.GetExecutingAssembly().FullName;
						myVersionAssembly = fullName.Substring(0, fullName.IndexOf(','));
						string versionName = VersionName;
						int num = versionName.IndexOf('.');
						int num2 = versionName.IndexOf('.', num + 1);
						myVersionMajor = int.Parse(versionName.Substring(0, num), NumberFormatInfo.InvariantInfo);
						myVersionMinor = int.Parse(versionName.Substring(num + 1, num2 - (num + 1)), NumberFormatInfo.InvariantInfo);
					}
					return "6.1.0.4";
				}
			}
			set
			{
			}
		}

		/// <summary>
		/// This static/shared property holds the runtime license key that permits distribution
		/// of applications using this control without displaying a licensing watermark.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This should always be set before any <see cref="T:Northwoods.Go.GoView" /> is created.
		/// </para>
		/// <para>
		/// For more details, read the GoDiagram Win/Web Intro documentation.
		/// </para>
		/// </remarks>
		public static string LicenseKey
		{
			get
			{
				return myLicenseKey;
			}
			set
			{
				if (myLicenseKey != value)
				{
					string text = myLicenseKey;
					myLicenseKey = value;
					if (text != null)
					{
						GoObject.Trace(("Caution: resetting GoView.LicenseKey from:\n  " + text + "\n  to:\n  " + myLicenseKey) ?? "(null)");
					}
					string fullName = Assembly.GetCallingAssembly().FullName;
					myCAN = fullName.Substring(0, fullName.IndexOf(','));
				}
			}
		}

		/// <summary>
		/// Gets or sets the vertical scroll bar used by the view when not all objects can be
		/// displayed at once in the given client area.
		/// </summary>
		/// <value>
		/// The <c>VScrollBar</c> control may be invisible and/or disabled, or null.  Setting this
		/// property will set up <see cref="M:Northwoods.Go.GoView.HandleScroll(System.Object,System.Windows.Forms.ScrollEventArgs)" /> as a scroll event handler for the scroll bar.
		/// The value comes from either the <see cref="P:Northwoods.Go.GoView.RightBar" /> property or the
		/// <see cref="P:Northwoods.Go.GoView.LeftBar" /> property.
		/// </value>
		/// <remarks>
		/// The scroll bar has the <c>LargeChange</c> and <c>SmallChange</c> properties, which affect how
		/// much is scrolled at a time.  The <c>LargeChange</c> property is computed given the height of
		/// the view's client area minus the <c>SmallChange</c> value.  The <c>SmallChange</c> property
		/// value is taken from the <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> property height.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.ShowVerticalScrollBar" />
		/// <seealso cref="P:Northwoods.Go.GoView.HorizontalScrollBar" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollPage(System.Single,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		/// <seealso cref="P:Northwoods.Go.GoView.LeftBar" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public VScrollBar VerticalScrollBar
		{
			get
			{
				if (RightBar is VScrollBar)
				{
					return RightBar as VScrollBar;
				}
				return LeftBar as VScrollBar;
			}
			set
			{
				if (RightBar is VScrollBar)
				{
					RightBar = value;
				}
				else
				{
					LeftBar = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the visibility policy for the vertical scroll bar.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoViewScrollBarVisibility.IfNeeded" />.
		/// </value>
		/// <seealso cref="T:Northwoods.Go.GoViewScrollBarVisibility" />
		/// <seealso cref="P:Northwoods.Go.GoView.VerticalScrollBar" />
		[Category("Appearance")]
		[DefaultValue(GoViewScrollBarVisibility.IfNeeded)]
		[Description("The visibility policy for the vertical scroll bar.")]
		public virtual GoViewScrollBarVisibility ShowVerticalScrollBar
		{
			get
			{
				return myShowVertScroll;
			}
			set
			{
				if (myShowVertScroll != value)
				{
					myShowVertScroll = value;
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("ShowVerticalScrollBar");
				}
			}
		}

		/// <summary>
		/// Gets or sets the horizontal scroll bar used by the view when not all objects can be
		/// displayed at once in the given client area.
		/// </summary>
		/// <value>
		/// The <c>HScrollBar</c> control may be invisible and/or disabled, or null.
		/// The value comes from either the <see cref="P:Northwoods.Go.GoView.BottomBar" /> property or the
		/// <see cref="P:Northwoods.Go.GoView.TopBar" /> property.
		/// </value>
		/// <remarks>
		/// The scroll bar has the LargeChange and SmallChange properties, which affect how
		/// much is scrolled at a time.  The <c>LargeChange</c> property is computed given the width of
		/// the view's client area minus the <c>SmallChange</c> value.  The <c>SmallChange</c> property
		/// value is taken from the <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> property width.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.ShowHorizontalScrollBar" />
		/// <seealso cref="P:Northwoods.Go.GoView.VerticalScrollBar" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollPage(System.Single,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		/// <seealso cref="P:Northwoods.Go.GoView.BottomBar" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HScrollBar HorizontalScrollBar
		{
			get
			{
				if (BottomBar is HScrollBar)
				{
					return BottomBar as HScrollBar;
				}
				return TopBar as HScrollBar;
			}
			set
			{
				if (BottomBar is HScrollBar)
				{
					BottomBar = value;
				}
				else
				{
					TopBar = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the visibility policy for the horizontal scroll bar.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoViewScrollBarVisibility.IfNeeded" />.
		/// </value>
		/// <seealso cref="T:Northwoods.Go.GoViewScrollBarVisibility" />
		/// <seealso cref="P:Northwoods.Go.GoView.HorizontalScrollBar" />
		[Category("Appearance")]
		[DefaultValue(GoViewScrollBarVisibility.IfNeeded)]
		[Description("The visibility policy for the horizontal scroll bar.")]
		public virtual GoViewScrollBarVisibility ShowHorizontalScrollBar
		{
			get
			{
				return myShowHorizScroll;
			}
			set
			{
				if (myShowHorizScroll != value)
				{
					myShowHorizScroll = value;
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("ShowHorizontalScrollBar");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that fits in the top-left corner adjacent to both
		/// the <see cref="P:Northwoods.Go.GoView.TopBar" /> and the <see cref="P:Northwoods.Go.GoView.LeftBar" />.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a blank, default <c>Control</c>.
		/// </value>
		/// <remarks>
		/// The position and size of the control are set automatically to fit the adjacent
		/// bars' width and height.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control TopLeftCorner
		{
			get
			{
				return myTopLeftCorner;
			}
			set
			{
				Control control = myTopLeftCorner;
				if (control != value)
				{
					if (value != null && value.Parent != null)
					{
						throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
					}
					if (control != null)
					{
						base.Controls.Remove(control);
					}
					myTopLeftCorner = value;
					if (value != null)
					{
						base.Controls.Add(value);
					}
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("TopLeftCorner");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that fits in the top-right corner adjacent to both
		/// the <see cref="P:Northwoods.Go.GoView.TopBar" /> and the <see cref="P:Northwoods.Go.GoView.RightBar" />.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a blank, default <c>Control</c>.
		/// </value>
		/// <remarks>
		/// The position and size of the control are set automatically to fit the adjacent
		/// bars' width and height.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control TopRightCorner
		{
			get
			{
				return myTopRightCorner;
			}
			set
			{
				Control control = myTopRightCorner;
				if (control != value)
				{
					if (value != null && value.Parent != null)
					{
						throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
					}
					if (control != null)
					{
						base.Controls.Remove(control);
					}
					myTopRightCorner = value;
					if (value != null)
					{
						base.Controls.Add(value);
					}
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("TopRightCorner");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that fits in the bottom-right corner adjacent to both
		/// the <see cref="P:Northwoods.Go.GoView.BottomBar" /> and the <see cref="P:Northwoods.Go.GoView.RightBar" />.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a blank, default <c>Control</c>.
		/// </value>
		/// <remarks>
		/// The position and size of the control are set automatically to fit the adjacent
		/// bars' width and height.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control BottomRightCorner
		{
			get
			{
				return myBottomRightCorner;
			}
			set
			{
				Control control = myBottomRightCorner;
				if (control != value)
				{
					if (value != null && value.Parent != null)
					{
						throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
					}
					if (control != null)
					{
						base.Controls.Remove(control);
					}
					myBottomRightCorner = value;
					if (value != null)
					{
						base.Controls.Add(value);
					}
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("BottomRightCorner");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that fits in the bottom-left corner adjacent to both
		/// the <see cref="P:Northwoods.Go.GoView.BottomBar" /> and the <see cref="P:Northwoods.Go.GoView.LeftBar" />.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a blank, default <c>Control</c>.
		/// </value>
		/// <remarks>
		/// The position and size of the control are set automatically to fit the adjacent
		/// bars' width and height.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control BottomLeftCorner
		{
			get
			{
				return myBottomLeftCorner;
			}
			set
			{
				Control control = myBottomLeftCorner;
				if (control != value)
				{
					if (value != null && value.Parent != null)
					{
						throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
					}
					if (control != null)
					{
						base.Controls.Remove(control);
					}
					myBottomLeftCorner = value;
					if (value != null)
					{
						base.Controls.Add(value);
					}
					LayoutScrollBars(update: true);
					RaisePropertyChangedEvent("BottomLeftCorner");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that runs along the top edge of the view.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is null.
		/// </value>
		/// <remarks>
		/// The position and width of the control are set automatically by
		/// <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />; the height is not modified.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control TopBar
		{
			get
			{
				return myTopBar;
			}
			set
			{
				Control control = myTopBar;
				if (control == value)
				{
					return;
				}
				if (value != null && value.Parent != null)
				{
					throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
				}
				if (control != null)
				{
					HScrollBar hScrollBar = control as HScrollBar;
					if (hScrollBar != null)
					{
						hScrollBar.Scroll -= myHorizScrollHandler;
					}
					base.Controls.Remove(control);
				}
				myTopBar = value;
				if (value != null)
				{
					base.Controls.Add(value);
					HScrollBar hScrollBar2 = value as HScrollBar;
					if (hScrollBar2 != null)
					{
						hScrollBar2.SmallChange = ScrollSmallChange.Width;
						hScrollBar2.Scroll += myHorizScrollHandler;
					}
				}
				LayoutScrollBars(update: true);
				RaisePropertyChangedEvent("TopBar");
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that runs along the right edge of the view.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a vertical scroll bar.
		/// </value>
		/// <remarks>
		/// The position and height of the control are set automatically by
		/// <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />; the width is not modified.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control RightBar
		{
			get
			{
				return myRightBar;
			}
			set
			{
				Control control = myRightBar;
				if (control == value)
				{
					return;
				}
				if (value != null && value.Parent != null)
				{
					throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
				}
				if (control != null)
				{
					VScrollBar vScrollBar = control as VScrollBar;
					if (vScrollBar != null)
					{
						vScrollBar.Scroll -= myVertScrollHandler;
					}
					base.Controls.Remove(control);
				}
				myRightBar = value;
				if (value != null)
				{
					base.Controls.Add(value);
					VScrollBar vScrollBar2 = value as VScrollBar;
					if (vScrollBar2 != null)
					{
						vScrollBar2.SmallChange = ScrollSmallChange.Height;
						vScrollBar2.Scroll += myVertScrollHandler;
					}
				}
				LayoutScrollBars(update: true);
				RaisePropertyChangedEvent("RightBar");
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that runs along the bottom edge of the view.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is a horizontal scroll bar.
		/// </value>
		/// <remarks>
		/// The position and width of the control are set automatically by
		/// <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />; the height is not modified.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control BottomBar
		{
			get
			{
				return myBottomBar;
			}
			set
			{
				Control control = myBottomBar;
				if (control == value)
				{
					return;
				}
				if (value != null && value.Parent != null)
				{
					throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
				}
				if (control != null)
				{
					HScrollBar hScrollBar = control as HScrollBar;
					if (hScrollBar != null)
					{
						hScrollBar.Scroll -= myHorizScrollHandler;
					}
					base.Controls.Remove(control);
				}
				myBottomBar = value;
				if (value != null)
				{
					base.Controls.Add(value);
					HScrollBar hScrollBar2 = value as HScrollBar;
					if (hScrollBar2 != null)
					{
						hScrollBar2.SmallChange = ScrollSmallChange.Width;
						hScrollBar2.Scroll += myHorizScrollHandler;
					}
				}
				LayoutScrollBars(update: true);
				RaisePropertyChangedEvent("BottomBar");
			}
		}

		/// <summary>
		/// Gets or sets the <c>Control</c> that runs along the left edge of the view.
		/// </summary>
		/// <value>
		/// Any <c>Control</c> may be used here; the initial value is null.
		/// </value>
		/// <remarks>
		/// The position and height of the control are set automatically by
		/// <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />; the width is not modified.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Control LeftBar
		{
			get
			{
				return myLeftBar;
			}
			set
			{
				Control control = myLeftBar;
				if (control == value)
				{
					return;
				}
				if (value != null && value.Parent != null)
				{
					throw new ArgumentException("new Control already belongs to a Control; remove it from there first");
				}
				if (control != null)
				{
					VScrollBar vScrollBar = control as VScrollBar;
					if (vScrollBar != null)
					{
						vScrollBar.Scroll -= myVertScrollHandler;
					}
					base.Controls.Remove(control);
				}
				myLeftBar = value;
				if (value != null)
				{
					base.Controls.Add(value);
					VScrollBar vScrollBar2 = value as VScrollBar;
					if (vScrollBar2 != null)
					{
						vScrollBar2.SmallChange = ScrollSmallChange.Height;
						vScrollBar2.Scroll += myVertScrollHandler;
					}
				}
				LayoutScrollBars(update: true);
				RaisePropertyChangedEvent("LeftBar");
			}
		}

		/// <summary>
		/// Gets or sets the document that this view is displaying.
		/// </summary>
		/// <value>
		/// The initial value is created by a call to <see cref="M:Northwoods.Go.GoView.CreateDocument" />.
		/// The value must not be null.
		/// </value>
		/// <remarks>
		/// <para>
		/// The document serves as the container of graphical objects that you want
		/// to display.
		/// Normally you should create graphical objects (instances of subclasses of
		/// <see cref="T:Northwoods.Go.GoObject" />) and add them to the document, in order to make them
		/// visible to the user.
		/// Although often there will be one view for each document, there are can be
		/// more than one view displaying the same document, or sometimes no views at all
		/// for a document.
		/// Each view will have its own state, such as scroll position and selection.
		/// The document holds all of the state that should be shared by all views.
		/// </para>
		/// <para>
		/// Setting this property to a different document will stop any ongoing editing
		/// in this view, clear out the selection, make this view's OnDocumentChanged method
		/// the event handler for the new document, and call
		/// <see cref="M:Northwoods.Go.GoView.InitializeLayersFromDocument" /> to set up the document layers in
		/// this view.
		/// </para>
		/// </remarks>
		/// <seealso cref="T:Northwoods.Go.GoDocument" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoDocument Document
		{
			get
			{
				return myDocument;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException("New value for GoView.Document must not be null");
				}
				GoDocument document = Document;
				if (value != document)
				{
					if (document != null && myDocChangedEventHandler != null)
					{
						document.Changed -= myDocChangedEventHandler;
					}
					if (Tool != null)
					{
						DoCancelMouse();
					}
					myCancelMouseDown = false;
					DoEndEdit();
					if (Selection != null)
					{
						Selection.Clear();
					}
					myDocument = value;
					value.Changed += myDocChangedEventHandler;
					RaisePropertyChangedEvent("Document");
					InitializeLayersFromDocument();
				}
			}
		}

		/// <summary>
		/// Gets the dimensions of the document.
		/// </summary>
		/// <value>
		/// The <c>SizeF</c> value measures the document in document coordinates.
		/// </value>
		/// <remarks>
		/// <para>
		/// This value is normally the same as <c>Document.Size</c>.
		/// However, a view may decide to change the extent of the document that
		/// the view displays.
		/// </para>
		/// <para>
		/// For example, the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> controls
		/// whether the effective document size includes not only the whole document
		/// but also any part of the <see cref="P:Northwoods.Go.GoView.Sheet" /> at the current <see cref="P:Northwoods.Go.GoView.DocScale" />
		/// given the size of this view control.
		/// </para>
		/// <para>
		/// As another example, the <see cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" />
		/// property, when false, restricts the view to only showing non-negative
		/// positions in the document.  In addition to restricting the
		/// <see cref="P:Northwoods.Go.GoView.DocumentTopLeft" /> property to non-negative positions,
		/// it adjusts this property accordingly.  This property also leaves room
		/// for any shadows, as specified by <see cref="P:Northwoods.Go.GoView.ShadowOffset" />.
		/// </para>
		/// <para>
		/// This property is different from the result of <see cref="M:Northwoods.Go.GoView.ComputeDocumentBounds" />
		/// because the latter method only takes into account what objects there actually are
		/// in the document, whereas this property will have the same value even if the
		/// document is empty.
		/// </para>
		/// <para>
		/// A different document size is used when printing in WinForms, <c>PrintDocumentSize</c>.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.DocumentTopLeft" />
		/// <seealso cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.Size" />
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The apparent size of the document, which may be different from GoDocument.Size")]
		public virtual SizeF DocumentSize
		{
			get
			{
				SizeF size = Document.Size;
				size.Width += Math.Abs(ShadowOffset.Width);
				size.Height += Math.Abs(ShadowOffset.Height);
				if (SheetStyle != 0)
				{
					if (Sheet != null)
					{
						PointF center = Sheet.Center;
						Size size2 = DisplayRectangle.Size;
						SizeF sizeF = ConvertViewToDoc(size2);
						RectangleF a = new RectangleF(center.X - sizeF.Width, center.Y - sizeF.Height, sizeF.Width * 2f, sizeF.Height * 2f);
						PointF topLeft = Document.TopLeft;
						RectangleF rectangleF = GoObject.UnionRect(GoObject.UnionRect(a, new RectangleF(topLeft.X, topLeft.Y, size.Width, size.Height)), Sheet.Bounds);
						return new SizeF(rectangleF.Width, rectangleF.Height);
					}
					SizeF docExtentSize = DocExtentSize;
					return new SizeF(size.Width + docExtentSize.Width, size.Height + docExtentSize.Height);
				}
				if (!ShowsNegativeCoordinates)
				{
					PointF topLeft2 = Document.TopLeft;
					if (topLeft2.X < 0f)
					{
						size.Width += topLeft2.X;
					}
					if (topLeft2.Y < 0f)
					{
						size.Height += topLeft2.Y;
					}
				}
				return size;
			}
		}

		/// <summary>
		/// Gets the top-left position of the document.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value specifies the top-left corner of the document in
		/// document coordinates.
		/// </value>
		/// <remarks>
		/// <para>
		/// This value is normally the same as <c>Document.TopLeft</c>.
		/// However, a view may decide to change the extent of the document that
		/// the view displays.
		/// </para>
		/// <para>
		/// For example, the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> controls
		/// whether the effective document bounds includes not only the whole document
		/// but also any part of the <see cref="P:Northwoods.Go.GoView.Sheet" /> at the current <see cref="P:Northwoods.Go.GoView.DocScale" />
		/// given the size of this view control.
		/// </para>
		/// <para>
		/// As another example, the <see cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" />
		/// property, when false, restricts the view to only showing non-negative
		/// positions in the document by always returning the (0, 0) point.
		/// This property also leaves room
		/// for any shadows, as specified by <see cref="P:Northwoods.Go.GoView.ShadowOffset" />.
		/// </para>
		/// <para>
		/// A different document top-left position is used when printing in WinForms,
		/// <c>PrintDocumentTopLeft</c>.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.DocumentSize" />
		/// <seealso cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.TopLeft" />
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The apparent top-left corner of the document, which may be different from GoDocument.TopLeft")]
		public virtual PointF DocumentTopLeft
		{
			get
			{
				checked
				{
					if (SheetStyle != 0)
					{
						if (Sheet != null)
						{
							SizeF size = Document.Size;
							size.Width += Math.Abs(ShadowOffset.Width);
							size.Height += Math.Abs(ShadowOffset.Height);
							PointF center = Sheet.Center;
							Size size2 = DisplayRectangle.Size;
							if (HorizontalScrollBar != null && HorizontalScrollBar.Visible && VerticalScrollBar != null)
							{
								size2.Width += VerticalScrollBar.Width;
							}
							if (VerticalScrollBar != null && VerticalScrollBar.Visible && HorizontalScrollBar != null)
							{
								size2.Height += HorizontalScrollBar.Height;
							}
							SizeF sizeF = ConvertViewToDoc(size2);
							RectangleF a = new RectangleF(center.X - sizeF.Width, center.Y - sizeF.Height, sizeF.Width * 2f, sizeF.Height * 2f);
							PointF topLeft = Document.TopLeft;
							RectangleF rectangleF = GoObject.UnionRect(GoObject.UnionRect(a, new RectangleF(topLeft.X, topLeft.Y, size.Width, size.Height)), Sheet.Bounds);
							return new PointF(rectangleF.X, rectangleF.Y);
						}
						PointF topLeft2 = Document.TopLeft;
						SizeF docExtentSize = DocExtentSize;
						return new PointF(topLeft2.X - docExtentSize.Width / 2f, topLeft2.Y - docExtentSize.Height / 2f);
					}
					if (ShowsNegativeCoordinates)
					{
						PointF topLeft3 = Document.TopLeft;
						SizeF shadowOffset = ShadowOffset;
						if (shadowOffset.Width < 0f)
						{
							topLeft3.X += shadowOffset.Width;
						}
						if (shadowOffset.Height < 0f)
						{
							topLeft3.Y += shadowOffset.Height;
						}
						return topLeft3;
					}
					return default(PointF);
				}
			}
		}

		/// <summary>
		/// Controls whether any parts of the document at negative coordinates can be seen
		/// or scrolled to by the user, when the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is None.
		/// </summary>
		/// <value>
		/// If this value is true, the user will be able to scroll to negative coordinate
		/// positions in the document.
		/// If this value is false, the user cannot see objects located at negative coordinates.
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// <para>
		/// When this value is false, it limits the values of the <see cref="P:Northwoods.Go.GoView.DocumentSize" />
		/// and <see cref="P:Northwoods.Go.GoView.DocumentTopLeft" /> properties.
		/// However, when the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// and there is a <see cref="P:Northwoods.Go.GoView.Sheet" />, the view ignores this property.
		/// </para>
		/// <para>
		/// For <see cref="T:Northwoods.Go.GoPalette" /> the default value is false.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Whether any parts of the document at negative coordinates can be seen or scrolled to.")]
		public virtual bool ShowsNegativeCoordinates
		{
			get
			{
				return myShowsNegativeCoordinates;
			}
			set
			{
				if (myShowsNegativeCoordinates != value)
				{
					myShowsNegativeCoordinates = value;
					RaisePropertyChangedEvent("ShowsNegativeCoordinates");
				}
			}
		}

		internal SizeF WorldScale => new SizeF(myHorizWorld, myVertWorld);

		/// <summary>
		/// Gets or sets the position in the document that this view is displaying.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates and corresponds to
		/// this view's top-left corner's position in the document.
		/// Initially the value is (0, 0).
		/// </value>
		/// <remarks>
		/// When setting this property, it first adjusts the value by calling
		/// <see cref="M:Northwoods.Go.GoView.LimitDocPosition(System.Drawing.PointF)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Point)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertDocToView(System.Drawing.PointF)" />
		/// <seealso cref="P:Northwoods.Go.GoView.DocExtentSize" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisplayRectangle" />
		[Category("Appearance")]
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The position in the document that this view is displaying.")]
		public virtual PointF DocPosition
		{
			get
			{
				return myOrigin;
			}
			set
			{
				PointF left = myOrigin;
				PointF right = LimitDocPosition(value);
				if (left != right)
				{
					myOrigin = right;
					RaisePropertyChangedEvent("DocPosition");
					RectangleF docExtent = DocExtent;
					myPreviousCenter = new PointF(docExtent.X + docExtent.Width / 2f, docExtent.Y + docExtent.Height / 2f);
				}
			}
		}

		/// <summary>
		/// Gets the size of this view in its document.
		/// </summary>
		/// <value>
		/// The <c>SizeF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// The value depends on the actual size of the client area and the scale
		/// at which the document is being shown.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.DocPosition" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisplayRectangle" />
		[Browsable(false)]
		public virtual SizeF DocExtentSize
		{
			get
			{
				Size size = DisplayRectangle.Size;
				return ConvertViewToDoc(size);
			}
		}

		/// <summary>
		/// Gets the extent of the view in its document, both position and size.
		/// </summary>
		/// <value>
		/// The <c>RectangleF</c> value is in document coordinates.
		/// </value>
		/// <remarks>
		/// This convenience method returns <c>new RectangleF(this.DocPosition, this.DocExtentSize)</c>.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.DocPosition" />
		/// <seealso cref="P:Northwoods.Go.GoView.DocExtentSize" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisplayRectangle" />
		[Browsable(false)]
		public RectangleF DocExtent
		{
			get
			{
				PointF docPosition = DocPosition;
				SizeF docExtentSize = DocExtentSize;
				return new RectangleF(docPosition.X, docPosition.Y, docExtentSize.Width, docExtentSize.Height);
			}
		}

		/// <summary>
		/// Gets or sets the center point of the view in the document, in document coordinates.
		/// </summary>
		/// <value>
		/// This returns the center of the rectangle returned by <see cref="P:Northwoods.Go.GoView.DocExtent" />.
		/// When set, this modifies <see cref="P:Northwoods.Go.GoView.DocPosition" /> so that the center of the view
		/// is at the new point.
		/// </value>
		/// <remarks>
		/// The view might not be able to be scrolled far enough to be able to actually
		/// make the new point be the center of the view, depending on the behavior of
		/// <see cref="M:Northwoods.Go.GoView.LimitDocPosition(System.Drawing.PointF)" />, which is normally limited by the values of
		/// <see cref="P:Northwoods.Go.GoView.DocumentTopLeft" /> and <see cref="P:Northwoods.Go.GoView.DocumentSize" />.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointF DocExtentCenter
		{
			get
			{
				PointF docPosition = DocPosition;
				SizeF docExtentSize = DocExtentSize;
				return new PointF(docPosition.X + docExtentSize.Width / 2f, docPosition.Y + docExtentSize.Height / 2f);
			}
			set
			{
				SizeF docExtentSize = DocExtentSize;
				DocPosition = new PointF(value.X - docExtentSize.Width / 2f, value.Y - docExtentSize.Height / 2f);
			}
		}

		/// <summary>
		/// Gets or sets the scale at which this view displays its document.
		/// </summary>
		/// <value>
		/// <para>
		/// A value of <c>1.0f</c> specifies that one unit in document coordinates corresponds
		/// to one pixel in view coordinates.  Values smaller than one make objects appear
		/// smaller on the screen.  Larger values make it appear that you have zoomed into
		/// the diagram.
		/// </para>
		/// <para>
		/// The <c>float</c> value must be greater than zero.  The default value is <c>1.0f</c>.
		/// </para>
		/// </value>
		/// <remarks>
		/// When setting this property, it first limits the value by calling
		/// <see cref="M:Northwoods.Go.GoView.LimitDocScale(System.Single)" />.
		/// A different value is used when printing, <c>PrintScale</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Size)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertDocToView(System.Drawing.SizeF)" />
		/// <seealso cref="P:Northwoods.Go.GoView.DocExtentSize" />
		[Category("Appearance")]
		[DefaultValue(1f)]
		[Description("The scale at which this view displays its document.")]
		public virtual float DocScale
		{
			get
			{
				return myHorizScale;
			}
			set
			{
				float num = myHorizScale;
				float num2 = Math.Max(9E-09f, LimitDocScale(value));
				if (num != num2 || myVertScale != num2)
				{
					myHorizScale = num2;
					myVertScale = num2;
					RaisePropertyChangedEvent("DocScale");
					RectangleF docExtent = DocExtent;
					myPreviousCenter = new PointF(docExtent.X + docExtent.Width / 2f, docExtent.Y + docExtent.Height / 2f);
					if (myMarquee != null)
					{
						myMarquee.Pen = null;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the distance to scroll when scrolling a small amount.
		/// </summary>
		/// <value>
		/// The <c>Size</c> value must have positive <c>Width</c> and <c>Height</c>, indicating the
		/// amount in view coordinates to scroll horizontally or vertically in either direction.
		/// </value>
		/// <remarks>
		/// Setting this property also modifies the <c>SmallChange</c> properties of the scroll bars,
		/// if there are any.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		[Category("Behavior")]
		[Description("The distance to scroll when scrolling a small amount.")]
		public virtual Size ScrollSmallChange
		{
			get
			{
				return myScrollSmallChange;
			}
			set
			{
				if (myScrollSmallChange != value)
				{
					if (value.Width <= 0 || value.Height <= 0)
					{
						throw new ArgumentException("New Size value for GoView.ScrollSmallChange must have positive dimensions");
					}
					myScrollSmallChange = value;
					HScrollBar horizontalScrollBar = HorizontalScrollBar;
					if (horizontalScrollBar != null && horizontalScrollBar.SmallChange != myScrollSmallChange.Width)
					{
						horizontalScrollBar.SmallChange = myScrollSmallChange.Width;
					}
					VScrollBar verticalScrollBar = VerticalScrollBar;
					if (verticalScrollBar != null && verticalScrollBar.SmallChange != myScrollSmallChange.Height)
					{
						verticalScrollBar.SmallChange = myScrollSmallChange.Height;
					}
					RaisePropertyChangedEvent("ScrollSmallChange");
				}
			}
		}

		/// <summary>
		/// Gets or sets the margin in the view where a mouse drag will automatically cause the view to scroll.
		/// </summary>
		/// <value>
		/// The <c>Size</c> value must have non-negative <c>Width</c> and <c>Height</c>, indicating in view
		/// coordinates the distance from the edge of the display area.
		/// Initially the value is 16x16.
		/// </value>
		/// <remarks>
		/// When the mouse drag point is within <c>AutoScrollRegion.Width</c> of the left or right sides,
		/// the view will automatically scroll horizontally in that direction.  When the point is within
		/// <c>AutoScrollRegion.Height</c> of the top or bottom, the view will automatically scroll
		/// vertically in that direction.  You can specify a distance of zero to disable autoscrolling
		/// in a direction; a value of 0x0 turns off autoscrolling altogether.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoAutoScroll(System.Drawing.Point)" />
		/// <seealso cref="P:Northwoods.Go.GoView.ScrollSmallChange" />
		[Category("Behavior")]
		[Description("The margin in the view where a mouse drag will automatically cause the view to scroll.")]
		public virtual Size AutoScrollRegion
		{
			get
			{
				return myAutoScrollRegion;
			}
			set
			{
				if (myAutoScrollRegion != value)
				{
					if (value.Width < 0 || value.Height < 0)
					{
						throw new ArgumentException("New Size value for GoView.AutoScrollRegion must have non-negative dimensions");
					}
					myAutoScrollRegion = value;
					RaisePropertyChangedEvent("AutoScrollRegion");
				}
			}
		}

		/// <summary>
		/// Gets or sets how quickly to change the <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// when the mouse is in the <see cref="P:Northwoods.Go.GoView.AutoScrollRegion" />.
		/// </summary>
		/// <value>
		/// The time is in milliseconds.
		/// The default is 100 (one tenth of a second).
		/// The value must not be negative.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(100)]
		[Description("How long to wait before changing the DocPosition during autoscrolling.")]
		public int AutoScrollTime
		{
			get
			{
				return myAutoScrollTime;
			}
			set
			{
				if (myAutoScrollTime != value && value >= 0)
				{
					myAutoScrollTime = value;
					RaisePropertyChangedEvent("AutoScrollTime");
				}
			}
		}

		/// <summary>
		/// Gets or sets how long to wait before autoscrolling.
		/// </summary>
		/// <value>
		/// The time is in milliseconds.
		/// The default is 1000 (one second).
		/// The value must not be negative.
		/// </value>
		/// <remarks>
		/// This is helpful in avoiding autoscrolling when the user is dragging something
		/// into the view and doesn't yet intend to autoscroll.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(1000)]
		[Description("How long to wait in the autoscroll margin before performing any autoscrolling.")]
		public int AutoScrollDelay
		{
			get
			{
				return myAutoScrollDelay;
			}
			set
			{
				if (myAutoScrollDelay != value && value >= 0)
				{
					myAutoScrollDelay = value;
					RaisePropertyChangedEvent("AutoScrollDelay");
				}
			}
		}

		/// <summary>
		/// Gets or sets the region around the original pan point where automatic panning does not occur.
		/// </summary>
		/// <value>
		/// The value defaults to 16x16; any new values must not be negative.
		/// </value>
		/// <remarks>
		/// This is used by <see cref="M:Northwoods.Go.GoView.ComputeAutoPanDocPosition(System.Drawing.Point,System.Drawing.Point)" /> to decide whether the
		/// current mouse point is close enough to the original pan point that no scrolling
		/// should occur.
		/// </remarks>
		[Category("Behavior")]
		[Description("The area around the original pan point outside of which the mouse will automatically cause the view to scroll.")]
		public virtual Size AutoPanRegion
		{
			get
			{
				return myAutoPanRegion;
			}
			set
			{
				if (myAutoPanRegion != value)
				{
					if (value.Width < 0 || value.Height < 0)
					{
						throw new ArgumentException("New Size value for GoView.AutoPanRegion must have non-negative dimensions");
					}
					myAutoPanRegion = value;
					RaisePropertyChangedEvent("AutoPanRegion");
				}
			}
		}

		/// <summary>
		/// Gets the collection of layers that this view displays.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value is the list of layers that this view displays.  The view will paint
		/// the layers in order.  Picking objects at a given point searches this same list
		/// of layers, but in reverse order.
		/// </para>
		/// <para>
		/// A view's collection of layers includes both view layers that it owns and document
		/// layers that it refers to.
		/// It is normal for the view to have references to all of the document's layers,
		/// in the same order as the document's layers, with a view layer on top for
		/// selection handles and the like.
		/// However, each view can have its own set of layers to display.
		/// The <see cref="M:Northwoods.Go.GoView.InitializeLayersFromDocument" /> method is responsible for setting
		/// up the <see cref="T:Northwoods.Go.GoLayer" />s when the view is assigned a document.  It can use
		/// methods such as <see cref="M:Northwoods.Go.GoLayerCollection.CreateNewLayerAfter(Northwoods.Go.GoLayer)" /> to create
		/// view layers or <see cref="M:Northwoods.Go.GoLayerCollection.InsertDocumentLayerAfter(Northwoods.Go.GoLayer,Northwoods.Go.GoLayer)" /> to insert
		/// references to document layers.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.InitializeLayersFromDocument" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundLayer" />
		/// <seealso cref="T:Northwoods.Go.GoLayerCollection" />
		/// <seealso cref="T:Northwoods.Go.GoLayer" />
		/// <seealso cref="T:Northwoods.Go.GoDocument" />
		[Browsable(false)]
		public GoLayerCollection Layers => myLayers;

		/// <summary>
		/// Gets or sets the layer that is normally used for holding view-specific objects
		/// that are behind all of the document layers.
		/// </summary>
		/// <value>
		/// The <see cref="T:Northwoods.Go.GoLayer" /> value must not be null and must already
		/// belong to this view.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoView.Layers" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoLayer BackgroundLayer
		{
			get
			{
				return myBackgroundLayer;
			}
			set
			{
				if (myBackgroundLayer != value)
				{
					if (value == null || value.View != this)
					{
						throw new ArgumentException("The new value for GoView.BackgroundLayer must belong to this view.");
					}
					myBackgroundLayer = value;
					RaisePropertyChangedEvent("BackgroundLayer");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can select objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from selecting objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// selectable by the user because the document or the object disallows it
		/// or because the object is not visible.
		/// Your code can always select objects programmatically by calling
		/// <c>Selection.Select(obj)</c> or <c>Selection.Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanSelectObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowSelect" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can select objects, if visible.")]
		public bool AllowSelect
		{
			get
			{
				return myAllowSelect;
			}
			set
			{
				if (myAllowSelect != value)
				{
					myAllowSelect = value;
					RaisePropertyChangedEvent("AllowSelect");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can move objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from moving objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// movable by the user because the document or the object disallows it.
		/// Your code can always move objects programmatically by calling
		/// <c>obj.Position = newPos</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanMoveObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowMove" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can move selected objects.")]
		public bool AllowMove
		{
			get
			{
				return myAllowMove;
			}
			set
			{
				if (myAllowMove != value)
				{
					myAllowMove = value;
					RaisePropertyChangedEvent("AllowMove");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can copy objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from copying objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// copyable by the user because the document or the object disallows it.
		/// Your code can always copy objects programmatically by calling
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanCopyObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowCopy" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can copy selected objects.")]
		public bool AllowCopy
		{
			get
			{
				return myAllowCopy;
			}
			set
			{
				if (myAllowCopy != value)
				{
					myAllowCopy = value;
					RaisePropertyChangedEvent("AllowCopy");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can resize objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from resizing objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// resizable by the user because the document or the object disallows it.
		/// Your code can always resize objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanResizeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowResize" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can resize selected objects.")]
		public bool AllowResize
		{
			get
			{
				return myAllowResize;
			}
			set
			{
				if (myAllowResize != value)
				{
					myAllowResize = value;
					RaisePropertyChangedEvent("AllowResize");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can reshape objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from reshaping objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// reshapable by the user because the document or the object disallows it.
		/// Your code can always reshape objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanReshapeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowReshape" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can reshape objects, if resizable.")]
		public bool AllowReshape
		{
			get
			{
				return myAllowReshape;
			}
			set
			{
				if (myAllowReshape != value)
				{
					myAllowReshape = value;
					RaisePropertyChangedEvent("AllowReshape");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can delete objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from deleting objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// deletable by the user because the document or the object disallows it.
		/// Your code can always delete objects programmatically by calling
		/// <c>obj.Remove()</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanDeleteObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowDelete" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can delete selected objects.")]
		public bool AllowDelete
		{
			get
			{
				return myAllowDelete;
			}
			set
			{
				if (myAllowDelete != value)
				{
					myAllowDelete = value;
					RaisePropertyChangedEvent("AllowDelete");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can insert objects into this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from inserting objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// insertable by the user because the document or the object disallows it.
		/// Your code can always insert objects programmatically by calling
		/// <c>Document.Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanInsertObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowInsert" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can insert new objects.")]
		public bool AllowInsert
		{
			get
			{
				return myAllowInsert;
			}
			set
			{
				if (myAllowInsert != value)
				{
					myAllowInsert = value;
					RaisePropertyChangedEvent("AllowInsert");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can link objects together in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from linking objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// linkable by the user because the document or the object disallows it.
		/// Your code can always link objects programmatically by calling
		/// <c>Document.LinksLayers.Add(newLink)</c>, where <c>newLink</c> is
		/// a newly created instance of a class like <see cref="T:Northwoods.Go.GoLink" /> or
		/// <see cref="T:Northwoods.Go.GoLabeledLink" /> whose <see cref="P:Northwoods.Go.IGoLink.FromPort" /> and
		/// <see cref="P:Northwoods.Go.IGoLink.ToPort" /> properties have been set to ports in
		/// this view's document.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanLinkObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowLink" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can link ports.")]
		public bool AllowLink
		{
			get
			{
				return myAllowLink;
			}
			set
			{
				if (myAllowLink != value)
				{
					myAllowLink = value;
					RaisePropertyChangedEvent("AllowLink");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can edit objects in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from editing objects in this view
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// editable by the user because the document or the object disallows it.
		/// Your code can always edit objects programmatically by calling
		/// <c>obj.DoBeginEdit(aView)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditObjects" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowEdit" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can edit objects.")]
		public bool AllowEdit
		{
			get
			{
				return myAllowEdit;
			}
			set
			{
				if (myAllowEdit != value)
				{
					myAllowEdit = value;
					RaisePropertyChangedEvent("AllowEdit");
				}
			}
		}

		/// <summary>
		/// Gets the view's selection object.
		/// </summary>
		/// <value>
		/// The view's <see cref="T:Northwoods.Go.GoSelection" /> collection is created by a call to
		/// <see cref="M:Northwoods.Go.GoView.CreateSelection" /> during construction.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoView.SelectAll" />
		/// <seealso cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />
		/// <seealso cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// <seealso cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// <seealso cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />
		/// <seealso cref="E:Northwoods.Go.GoView.ObjectGotSelection" />
		/// <seealso cref="E:Northwoods.Go.GoView.ObjectLostSelection" />
		[Browsable(false)]
		public virtual GoSelection Selection => mySelection;

		/// <summary>
		/// Gets or sets the maximum number of objects allowed in the selection.
		/// </summary>
		/// <value>
		/// The initial value is 999999.
		/// The value must be non-negative.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property is checked by the <see cref="P:Northwoods.Go.GoView.Selection" />'s <see cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" /> method.
		/// If the selection's <see cref="P:Northwoods.Go.GoCollection.Count" /> is greater than or equal to this value,
		/// <see cref="M:Northwoods.Go.GoSelection.Add(Northwoods.Go.GoObject)" /> will not add any objects to this collection.
		/// </para>
		/// <para>
		/// If the new value is larger than the current number of selected objects,
		/// objects are removed from the <see cref="P:Northwoods.Go.GoView.Selection" /> until the count is less than this new value.
		/// Such removals are done between <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> and
		/// <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events.
		/// </para>
		/// </remarks>
		[Category("Selection")]
		[DefaultValue(999999)]
		[Description("The maximum number of selected objects")]
		public virtual int MaximumSelectionCount
		{
			get
			{
				return myMaximumSelectionCount;
			}
			set
			{
				if (value == myMaximumSelectionCount || value < 0)
				{
					return;
				}
				myMaximumSelectionCount = value;
				RaisePropertyChangedEvent("MaximumSelectionCount");
				if (Selection.Count > value)
				{
					RaiseSelectionStarting();
					while (Selection.Count > value)
					{
						GoObject last = Selection.Last;
						Selection.Remove(last);
					}
					RaiseSelectionFinished();
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:Northwoods.Go.GoToolContext.SingleSelection" /> property
		/// of the <see cref="T:Northwoods.Go.GoToolContext" /> tool that is in the <see cref="P:Northwoods.Go.GoView.MouseDownTools" /> list.
		/// </summary>
		/// <value>
		/// The default value is true: a context click changes the selection so that
		/// only the object at the click point is selected.  Setting this to false permits
		/// context menu actions to occur on a multiple selection.
		/// </value>
		[Category("Selection")]
		[DefaultValue(true)]
		[Description("Whether a context click changes the selection to be only the object at that click point")]
		public virtual bool ContextClickSingleSelection
		{
			get
			{
				return ((GoToolContext)FindMouseTool(typeof(GoToolContext), subclass: true))?.SingleSelection ?? true;
			}
			set
			{
				GoToolContext goToolContext = (GoToolContext)FindMouseTool(typeof(GoToolContext), subclass: true);
				if (goToolContext != null)
				{
					goToolContext.SingleSelection = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoPickInRectangleStyle" /> used by
		/// <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" /> to determine whether an object is within
		/// a rectangle.
		/// </summary>
		/// <value>
		/// This value defaults to <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyContained" />.
		/// The value must be either
		/// <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyContained" /> or
		/// <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyIntersectsBounds" />.
		/// </value>
		[Category("Selection")]
		[DefaultValue(GoPickInRectangleStyle.SelectableOnlyContained)]
		[Description("Controls how SelectInRectangle decides an object is in a rectangle")]
		public virtual GoPickInRectangleStyle SelectInRectangleStyle
		{
			get
			{
				return mySelectInRectangleStyle;
			}
			set
			{
				if (mySelectInRectangleStyle != value && (value == GoPickInRectangleStyle.SelectableOnlyContained || value == GoPickInRectangleStyle.SelectableOnlyIntersectsBounds))
				{
					mySelectInRectangleStyle = value;
					RaisePropertyChangedEvent("SelectInRectangleStyle");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the selection disappears when this view loses focus.
		/// </summary>
		/// <value>
		/// If this value is true, all selection handles are removed from this view when
		/// it loses focus.
		/// If this value is false, all selection handles are changed to use the
		/// <see cref="P:Northwoods.Go.GoView.NoFocusSelectionColor" /> when this view loses focus.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoSelection.OnLostFocus" />
		[Category("Selection")]
		[DefaultValue(false)]
		[Description("Whether the selection disappears when this view loses focus.")]
		public virtual bool HidesSelection
		{
			get
			{
				return myHidesSelection;
			}
			set
			{
				if (myHidesSelection != value)
				{
					myHidesSelection = value;
					RaisePropertyChangedEvent("HidesSelection");
				}
			}
		}

		/// <summary>
		/// Gets or sets the handle color for the primary selection.
		/// </summary>
		/// <value>
		/// The default value is a bright green color.
		/// </value>
		/// <seealso cref="T:Northwoods.Go.GoSelection" />
		/// <seealso cref="P:Northwoods.Go.GoView.NoFocusSelectionColor" />
		[Category("Selection")]
		[Description("The handle color for the primary selection.")]
		public virtual Color PrimarySelectionColor
		{
			get
			{
				return myPrimarySelectionColor;
			}
			set
			{
				if (myPrimarySelectionColor != value)
				{
					myPrimarySelectionColor = value;
					RaisePropertyChangedEvent("PrimarySelectionColor");
				}
			}
		}

		/// <summary>
		/// Gets or sets the handle color for selected objects other than the primary selection.
		/// </summary>
		/// <value>
		/// The default value is a light blue color.
		/// </value>
		/// <seealso cref="T:Northwoods.Go.GoSelection" />
		/// <seealso cref="P:Northwoods.Go.GoView.PrimarySelectionColor" />
		[Category("Selection")]
		[Description("The handle color for objects other than the primary selection.")]
		public virtual Color SecondarySelectionColor
		{
			get
			{
				return mySecondarySelectionColor;
			}
			set
			{
				if (mySecondarySelectionColor != value)
				{
					mySecondarySelectionColor = value;
					RaisePropertyChangedEvent("SecondarySelectionColor");
				}
			}
		}

		/// <summary>
		/// Gets or sets the handle color for selected objects when this view does not have focus.
		/// </summary>
		/// <value>
		/// The default value is a light gray color.
		/// </value>
		/// <remarks>
		/// If this color is the same as <see cref="P:Northwoods.Go.GoView.PrimarySelectionColor" />,
		/// the selection handles will not be re-created as the view gains or loses focus.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoSelection.OnLostFocus" />
		/// <seealso cref="P:Northwoods.Go.GoView.PrimarySelectionColor" />
		[Category("Selection")]
		[Description("The handle color for objects when the view does not have focus.")]
		public virtual Color NoFocusSelectionColor
		{
			get
			{
				return myNoFocusSelectionColor;
			}
			set
			{
				if (myNoFocusSelectionColor != value)
				{
					myNoFocusSelectionColor = value;
					RaisePropertyChangedEvent("NoFocusSelectionColor");
				}
			}
		}

		/// <summary>
		/// The default size for new resize handles for resizable selected objects.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value is in document coordinates.
		/// The initial value is 6x6.
		/// </value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SizeF ResizeHandleSize
		{
			get
			{
				return myResizeHandleSize;
			}
			set
			{
				if (myResizeHandleSize != value)
				{
					myResizeHandleSize = value;
					RaisePropertyChangedEvent("ResizeHandleSize");
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.ResizeHandleSize" />.
		/// </summary>
		[Category("Selection")]
		[DefaultValue(6f)]
		[Description("The default size for new resize handles.")]
		public float ResizeHandleWidth
		{
			get
			{
				return ResizeHandleSize.Width;
			}
			set
			{
				ResizeHandleSize = new SizeF(value, ResizeHandleSize.Height);
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.ResizeHandleSize" />.
		/// </summary>
		[Category("Selection")]
		[DefaultValue(6f)]
		[Description("The default size for new resize handles.")]
		public float ResizeHandleHeight
		{
			get
			{
				return ResizeHandleSize.Height;
			}
			set
			{
				ResizeHandleSize = new SizeF(ResizeHandleSize.Width, value);
			}
		}

		/// <summary>
		/// Gets or sets the width of the pen used in resize handles for a selected object.
		/// </summary>
		/// <value>
		/// The initial value is 1.
		/// </value>
		[Category("Selection")]
		[DefaultValue(1f)]
		[Description("The width of the pen used to draw the standard resize handle")]
		public virtual float ResizeHandlePenWidth
		{
			get
			{
				return myResizeHandlePenWidth;
			}
			set
			{
				if (myResizeHandlePenWidth != value)
				{
					myResizeHandlePenWidth = value;
					RaisePropertyChangedEvent("ResizeHandlePenWidth");
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the pen used in bounding handles, surrounding a selected object.
		/// </summary>
		/// <value>
		/// The initial value is 2.
		/// </value>
		[Category("Selection")]
		[DefaultValue(2f)]
		[Description("The width of the pen used to draw the standard bounding handle")]
		public virtual float BoundingHandlePenWidth
		{
			get
			{
				return myBoundingHandlePenWidth;
			}
			set
			{
				if (myBoundingHandlePenWidth != value)
				{
					myBoundingHandlePenWidth = value;
					RaisePropertyChangedEvent("BoundingHandlePenWidth");
				}
			}
		}

		/// <summary>
		/// Gets or sets which key commands are disabled in the <see cref="T:Northwoods.Go.GoToolManager" />'s
		/// <see cref="M:Northwoods.Go.GoToolManager.DoKeyDown" /> method.
		/// </summary>
		/// <value>
		/// This property is initially <c>GoViewDisableKeys.ArrowMove</c>:
		/// all standard keystroke commands are enabled except for
		/// moving selected movable objects using the arrow keys.
		/// See the list of flags, which can be bit-wise combined, in the description
		/// for <see cref="T:Northwoods.Go.GoViewDisableKeys" />.
		/// </value>&gt;
		[Category("Behavior")]
		[DefaultValue(GoViewDisableKeys.ArrowMove)]
		[Description("Controls which keys do not have their normal effect.")]
		public GoViewDisableKeys DisableKeys
		{
			get
			{
				return myDisableKeys;
			}
			set
			{
				if (myDisableKeys != value)
				{
					myDisableKeys = value;
					RaisePropertyChangedEvent("DisableKeys");
				}
			}
		}

		/// <summary>
		/// Gets or sets the distance a selected object is moved using the arrow keys with the Ctrl key held down.
		/// </summary>
		/// <value>The default value is 1 document unit.</value>
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		/// <seealso cref="F:Northwoods.Go.GoViewDisableKeys.ArrowMove" />
		[Category("Behavior")]
		[DefaultValue(1)]
		[Description("Controls the distance the arrow keys will move an object with the Ctrl key modifier")]
		public float ArrowMoveSmall
		{
			get
			{
				return myArrowMoveSmall;
			}
			set
			{
				if (myArrowMoveSmall != value)
				{
					myArrowMoveSmall = value;
					RaisePropertyChangedEvent("ArrowMoveSmall");
				}
			}
		}

		/// <summary>
		/// Gets or sets the distance a selected object is moved using the arrow keys.
		/// </summary>
		/// <value>The default value is 10 document units.</value>
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		/// <seealso cref="F:Northwoods.Go.GoViewDisableKeys.ArrowMove" />
		[Category("Behavior")]
		[DefaultValue(10)]
		[Description("Controls the distance the arrow keys will move an object")]
		public float ArrowMoveLarge
		{
			get
			{
				return myArrowMoveLarge;
			}
			set
			{
				if (myArrowMoveLarge != value)
				{
					myArrowMoveLarge = value;
					RaisePropertyChangedEvent("ArrowMoveLarge");
				}
			}
		}

		/// <summary>
		/// This property is true when this GoView is producing a Bitmap.
		/// </summary>
		/// <value>
		/// The value is false when rendering onto the screen or not rendering at all.
		/// </value>
		public bool IsRenderingBitmap => myIsRenderingBitmap;

		/// <summary>
		/// Gets or sets whether a rubber-band box should be drawn in XOR mode.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// When this value is false, <see cref="M:Northwoods.Go.GoView.DrawXorBox(System.Drawing.Rectangle,System.Boolean)" /> will
		/// draw a rectangle using the normal drawing mechanisms.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether rubber-band drawing (DrawXorBox) actually draws in XOR mode")]
		public virtual bool DrawsXorMode
		{
			get
			{
				return myDrawsXorMode;
			}
			set
			{
				if (myDrawsXorMode != value)
				{
					myDrawsXorMode = value;
					RaisePropertyChangedEvent("DrawsXorMode");
				}
			}
		}

		/// <summary>
		/// Gets or sets the ImageList that GoImage objects can draw from.
		/// </summary>
		/// <value>
		/// This value defaults to null.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The ImageList from which GoImage objects can draw an image.")]
		public virtual ImageList ImageList
		{
			get
			{
				return myImageList;
			}
			set
			{
				if (myImageList != value)
				{
					myImageList = value;
					RaisePropertyChangedEvent("ImageList");
				}
			}
		}

		/// <summary>
		/// Gets or sets the border style for this view.
		/// </summary>
		/// <value>
		/// The default value is <c>BorderStyle.Fixed3D</c>.
		/// </value>
		/// <remarks>
		/// The border surrounds the view's display area and its scrollbars, if any.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(BorderStyle.Fixed3D)]
		[Description("The border style for this view.")]
		public virtual BorderStyle BorderStyle
		{
			get
			{
				return myBorderStyle;
			}
			set
			{
				if (myBorderStyle != value)
				{
					myBorderStyle = value;
					UpdateBorderWidths();
					RaisePropertyChangedEvent("BorderStyle");
				}
			}
		}

		/// <summary>
		/// Gets or sets the 3D border style for this view.
		/// </summary>
		/// <value>
		/// The default value is <c>Border3DStyle.Etched</c>.
		/// </value>
		/// <remarks>
		/// When the <see cref="P:Northwoods.Go.GoView.BorderStyle" /> value is <c>BorderStyle.Fixed3D</c>,
		/// this property specifies what kind of 3D border is displayed.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.BorderStyle" />
		[Category("Appearance")]
		[DefaultValue(Border3DStyle.Etched)]
		[Description("The 3D border style for this view, when BorderStyle is Fixed3D.")]
		public virtual Border3DStyle Border3DStyle
		{
			get
			{
				return myBorder3DStyle;
			}
			set
			{
				if (myBorder3DStyle != value)
				{
					myBorder3DStyle = value;
					RaisePropertyChangedEvent("Border3DStyle");
				}
			}
		}

		/// <summary>
		/// Gets the area where the view displays its document.
		/// </summary>
		/// <value>
		/// The <c>Rectangle</c> value specifies an area in control coordinates
		/// relative to the top left corner of this control.
		/// </value>
		/// <remarks>
		/// The display rectangle is normally smaller than the <c>Control.Size</c>,
		/// because of the scroll bars and the border along the edges.
		/// Note the difference with <see cref="P:Northwoods.Go.GoView.DocExtent" />, which gets an area
		/// in a document in document coordinates depending on the <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// and <see cref="P:Northwoods.Go.GoView.DocScale" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Rectangle)" />
		[Browsable(false)]
		public override Rectangle DisplayRectangle
		{
			get
			{
				if (myViewSize.Width >= 0 && myViewSize.Height >= 0)
				{
					return new Rectangle(myDisplayRectangle.X, myDisplayRectangle.Y, myViewSize.Width, myViewSize.Height);
				}
				if (myDisplayRectangle.Width < 0 || myDisplayRectangle.Height < 0)
				{
					return new Rectangle(0, 0, base.Width, base.Height);
				}
				return myDisplayRectangle;
			}
		}

		/// <summary>
		/// Gets or sets how nicely lines are drawn.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.TextRenderingHint" />
		/// <seealso cref="P:Northwoods.Go.GoView.InterpolationMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.CompositingQuality" />
		/// <seealso cref="P:Northwoods.Go.GoView.PixelOffsetMode" />
		[Category("Appearance")]
		[DefaultValue(SmoothingMode.HighQuality)]
		[Description("How nicely lines are drawn")]
		public virtual SmoothingMode SmoothingMode
		{
			get
			{
				return mySmoothingMode;
			}
			set
			{
				if (mySmoothingMode != value && value != SmoothingMode.Invalid)
				{
					mySmoothingMode = value;
					RaisePropertyChangedEvent("SmoothingMode");
				}
			}
		}

		/// <summary>
		/// Gets or sets how nicely text is rendered.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.SmoothingMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.InterpolationMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.CompositingQuality" />
		/// <seealso cref="P:Northwoods.Go.GoView.PixelOffsetMode" />
		[Category("Appearance")]
		[DefaultValue(TextRenderingHint.ClearTypeGridFit)]
		[Description("How nicely text is rendered")]
		public virtual TextRenderingHint TextRenderingHint
		{
			get
			{
				return myTextRenderingHint;
			}
			set
			{
				if (myTextRenderingHint != value)
				{
					myTextRenderingHint = value;
					RaisePropertyChangedEvent("TextRenderingHint");
				}
			}
		}

		/// <summary>
		/// Gets or sets how images are rendered when scaled or stretched.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.SmoothingMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.TextRenderingHint" />
		/// <seealso cref="P:Northwoods.Go.GoView.CompositingQuality" />
		/// <seealso cref="P:Northwoods.Go.GoView.PixelOffsetMode" />
		[Category("Appearance")]
		[DefaultValue(InterpolationMode.HighQualityBicubic)]
		[Description("How images are rendered when scaled or stretched")]
		public virtual InterpolationMode InterpolationMode
		{
			get
			{
				return myInterpolationMode;
			}
			set
			{
				if (myInterpolationMode != value && value != InterpolationMode.Invalid)
				{
					myInterpolationMode = value;
					RaisePropertyChangedEvent("InterpolationMode");
				}
			}
		}

		/// <summary>
		/// Gets or sets how pixels are composited for all drawing operations.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.SmoothingMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.TextRenderingHint" />
		/// <seealso cref="P:Northwoods.Go.GoView.InterpolationMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.PixelOffsetMode" />
		[Category("Appearance")]
		[DefaultValue(CompositingQuality.AssumeLinear)]
		[Description("How pixels are composited")]
		public virtual CompositingQuality CompositingQuality
		{
			get
			{
				return myCompositingQuality;
			}
			set
			{
				if (myCompositingQuality != value && value != CompositingQuality.Invalid)
				{
					myCompositingQuality = value;
					RaisePropertyChangedEvent("CompositingQuality");
				}
			}
		}

		/// <summary>
		/// Gets or sets how pixels are positioned for all drawing operations.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.SmoothingMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.TextRenderingHint" />
		/// <seealso cref="P:Northwoods.Go.GoView.InterpolationMode" />
		/// <seealso cref="P:Northwoods.Go.GoView.CompositingQuality" />
		[Category("Appearance")]
		[DefaultValue(PixelOffsetMode.HighQuality)]
		[Description("How pixels are offset")]
		public virtual PixelOffsetMode PixelOffsetMode
		{
			get
			{
				return myPixelOffsetMode;
			}
			set
			{
				if (myPixelOffsetMode != value && value != PixelOffsetMode.Invalid)
				{
					myPixelOffsetMode = value;
					RaisePropertyChangedEvent("PixelOffsetMode");
				}
			}
		}

		internal bool SuppressingPaint => mySuppressPaint > 0;

		/// <summary>
		/// Gets or sets whether the user can type keystroke commands in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from entering key commands in this view
		/// by the normal mechanisms.
		/// Your code can always handle keystrokes programmatically by adding a
		/// <c>KeyDown</c> event handler to this control or by overriding
		/// <see cref="M:Northwoods.Go.GoView.OnKeyDown(System.Windows.Forms.KeyEventArgs)" />.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can type keystroke commands in this view.")]
		public bool AllowKey
		{
			get
			{
				return myAllowKey;
			}
			set
			{
				if (myAllowKey != value)
				{
					myAllowKey = value;
					RaisePropertyChangedEvent("AllowKey");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can use the mouse in this view.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from using the mouse in this view
		/// by the normal mechanisms.
		/// Your code can always handle mouse events programmatically by adding
		/// mouse event handlers to this control or by overriding
		/// <see cref="M:Northwoods.Go.GoView.OnMouseDown(System.Windows.Forms.MouseEventArgs)" />, <see cref="M:Northwoods.Go.GoView.OnMouseMove(System.Windows.Forms.MouseEventArgs)" />,
		/// <see cref="M:Northwoods.Go.GoView.OnMouseUp(System.Windows.Forms.MouseEventArgs)" />, or <see cref="M:Northwoods.Go.GoView.OnDoubleClick(System.EventArgs)" />.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can use the mouse in this view.")]
		public bool AllowMouse
		{
			get
			{
				return myAllowMouse;
			}
			set
			{
				if (myAllowMouse != value)
				{
					myAllowMouse = value;
					RaisePropertyChangedEvent("AllowMouse");
				}
			}
		}

		/// <summary>
		/// Gets the canonical event args information for the last mouse down.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.LastInput" />
		/// <seealso cref="T:Northwoods.Go.GoTool" />
		[Browsable(false)]
		public GoInputEventArgs FirstInput => myFirstInput;

		/// <summary>
		/// Gets the canonical event args information for the last mouse or keyboard input.
		/// </summary>
		/// <remarks>
		/// The last input event args information is used by the tools, the view, and objects
		/// to decide how to behave.  Typically you will use <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />
		/// to see where an event occurred, or <see cref="P:Northwoods.Go.GoInputEventArgs.Control" /> to see
		/// if the Ctrl key was held down at the time of the event.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.FirstInput" />
		/// <seealso cref="T:Northwoods.Go.GoTool" />
		[Browsable(false)]
		public GoInputEventArgs LastInput => myLastInput;

		/// <summary>
		/// Gets or sets the default tool.
		/// </summary>
		/// <value>
		/// The value must not be null.  Initially this is assigned the value of <see cref="M:Northwoods.Go.GoView.CreateDefaultTool" />.
		/// </value>
		/// <remarks>
		/// When the <see cref="P:Northwoods.Go.GoView.Tool" /> property is set to null, we actually reset
		/// <see cref="P:Northwoods.Go.GoView.Tool" /> to be the value of this <see cref="P:Northwoods.Go.GoView.DefaultTool" /> property.
		/// By default this value is an instance of <see cref="T:Northwoods.Go.GoToolManager" />, which handles
		/// standard keyboard commands and invokes the appropriate tool upon mouse down/move/up
		/// events.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.Tool" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IGoTool DefaultTool
		{
			get
			{
				return myDefaultTool;
			}
			set
			{
				if (myDefaultTool != value)
				{
					if (value == null)
					{
						throw new ArgumentException("New value for GoView.DefaultTool must not be null");
					}
					myDefaultTool = value;
					RaisePropertyChangedEvent("DefaultTool");
				}
			}
		}

		/// <summary>
		/// Gets or sets the current tool being used by this view.
		/// </summary>
		/// <remarks>
		/// <para>
		/// As standard input events occur, the event args information is canonicalized
		/// into an instance of <see cref="T:Northwoods.Go.GoInputEventArgs" /> and then the current Tool's
		/// appropriate method is called.
		/// </para>
		/// <para>
		/// Setting this property to null results in setting it to the value of
		/// <see cref="P:Northwoods.Go.GoView.DefaultTool" />.
		/// A tool that has finished will probably need to reset this property to
		/// the <see cref="P:Northwoods.Go.GoView.DefaultTool" />,
		/// typically by calling the <see cref="T:Northwoods.Go.GoTool" />.<see cref="M:Northwoods.Go.GoTool.StopTool" /> method.
		/// </para>
		/// <para>
		/// If the tool is explicitly set as a result of some user-interface command,
		/// the tool is being used in a "modal" fashion.
		/// </para>
		/// <para>
		/// If the tool is set as a result of the <see cref="T:Northwoods.Go.GoToolManager" /> searching
		/// through the lists of tools to be started as a result of a mouse down, a mouse
		/// move, or a mouse up, then the tool is being used in a "mode-less" fashion.
		/// </para>
		/// </remarks>
		/// <seealso cref="T:Northwoods.Go.GoToolManager" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseDownTools" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseMoveTools" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseUpTools" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IGoTool Tool
		{
			get
			{
				return myTool;
			}
			set
			{
				if (myTool != value)
				{
					if (myTool != null)
					{
						myTool.Stop();
					}
					if (value == null)
					{
						myTool = DefaultTool;
					}
					else
					{
						myTool = value;
					}
					if (myTool != null)
					{
						myTool.Start();
					}
					RaisePropertyChangedEvent("Tool");
				}
			}
		}

		/// <summary>
		/// Gets a list of "mode-less" tools to be considered for becoming the current Tool upon a mouse down event.
		/// </summary>
		/// <value>
		/// The <c>IList</c> of <see cref="T:Northwoods.Go.IGoTool" /> may be modified.
		/// </value>
		/// <remarks>
		/// <para>
		/// <see cref="T:Northwoods.Go.GoToolManager" />, an instance of which is normally the <see cref="P:Northwoods.Go.GoView.DefaultTool" />,
		/// iterates through this list when a mouse down event occurs.  The first tool that it finds
		/// whose <see cref="M:Northwoods.Go.IGoTool.CanStart" /> method returns true becomes this view's current
		/// <see cref="P:Northwoods.Go.GoView.Tool" />.  If no such tool is found, the <see cref="T:Northwoods.Go.GoToolManager" /> continues
		/// its normal behavior.
		/// </para>
		/// <para>
		/// By default this returns a list containing instances of the <see cref="T:Northwoods.Go.GoToolAction" />,
		/// <see cref="T:Northwoods.Go.GoToolContext" />, <see cref="T:Northwoods.Go.GoToolPanning" />,
		/// <see cref="T:Northwoods.Go.GoToolRelinking" />, and <see cref="T:Northwoods.Go.GoToolResizing" /> tools, in that order.
		/// The order of the tools matters, because even if several tools can start, only the first one
		/// actually is started.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.MouseMoveTools" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseUpTools" />
		[Browsable(false)]
		public virtual IList<IGoTool> MouseDownTools
		{
			get
			{
				if (myMouseDownTools == null)
				{
					myMouseDownTools = new List<IGoTool>();
					myMouseDownTools.Add(new GoToolAction(this));
					myMouseDownTools.Add(new GoToolContext(this));
					myMouseDownTools.Add(new GoToolPanning(this));
					myMouseDownTools.Add(new GoToolRelinking(this));
					myMouseDownTools.Add(new GoToolResizing(this));
				}
				return myMouseDownTools;
			}
		}

		/// <summary>
		/// Gets a list of "mode-less" tools to be considered for becoming the current Tool upon a mouse move event.
		/// </summary>
		/// <value>
		/// The <c>IList</c> of <see cref="T:Northwoods.Go.IGoTool" /> may be modified.
		/// </value>
		/// <remarks>
		/// <para>
		/// <see cref="T:Northwoods.Go.GoToolManager" />, an instance of which is normally the <see cref="P:Northwoods.Go.GoView.DefaultTool" />,
		/// iterates through this list when a mouse move event occurs.  The first tool that it finds
		/// whose <see cref="M:Northwoods.Go.IGoTool.CanStart" /> method returns true becomes this view's current
		/// <see cref="P:Northwoods.Go.GoView.Tool" />.  If no such tool is found, the <see cref="T:Northwoods.Go.GoToolManager" /> continues
		/// its normal behavior.
		/// </para>
		/// <para>
		/// By default this returns a list containing instances of the
		/// <see cref="T:Northwoods.Go.GoToolLinkingNew" />, <see cref="T:Northwoods.Go.GoToolDragging" />,
		/// and <see cref="T:Northwoods.Go.GoToolRubberBanding" /> tools, in that order.
		/// The order of the tools matters, because even if several tools can start, only the first one
		/// actually is started.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.MouseDownTools" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseUpTools" />
		[Browsable(false)]
		public virtual IList<IGoTool> MouseMoveTools
		{
			get
			{
				if (myMouseMoveTools == null)
				{
					myMouseMoveTools = new List<IGoTool>();
					myMouseMoveTools.Add(new GoToolLinkingNew(this));
					myMouseMoveTools.Add(new GoToolDragging(this));
					myMouseMoveTools.Add(new GoToolRubberBanding(this));
				}
				return myMouseMoveTools;
			}
		}

		/// <summary>
		/// Gets a list of "mode-less" tools to be considered for becoming the current Tool upon a mouse up event.
		/// </summary>
		/// <value>
		/// The <c>IList</c> of <see cref="T:Northwoods.Go.IGoTool" /> may be modified.
		/// </value>
		/// <remarks>
		/// <para>
		/// <see cref="T:Northwoods.Go.GoToolManager" />, an instance of which is normally the <see cref="P:Northwoods.Go.GoView.DefaultTool" />,
		/// iterates through this list when a mouse up event occurs.  The first tool that it finds
		/// whose <see cref="M:Northwoods.Go.IGoTool.CanStart" /> method returns true becomes this view's current
		/// <see cref="P:Northwoods.Go.GoView.Tool" />.  If no such tool is found, the <see cref="T:Northwoods.Go.GoToolManager" /> continues
		/// its normal behavior.
		/// </para>
		/// <para>
		/// By default this returns a list containing only an instance of the
		/// <see cref="T:Northwoods.Go.GoToolSelecting" /> tool.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.MouseDownTools" />
		/// <seealso cref="P:Northwoods.Go.GoView.MouseMoveTools" />
		[Browsable(false)]
		public virtual IList<IGoTool> MouseUpTools
		{
			get
			{
				if (myMouseUpTools == null)
				{
					myMouseUpTools = new List<IGoTool>();
					myMouseUpTools.Add(new GoToolSelecting(this));
				}
				return myMouseUpTools;
			}
		}

		/// <summary>
		/// Gets the Type of <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" />, and when set,
		/// creates an instance of the class as the new value of <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" />.
		/// </summary>
		/// <value>the <c>Type</c> of the value of <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" /></value>
		/// <remarks>
		/// This property has been superseded by the <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" /> property,
		/// but it remains for compatibility.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type NewLinkClass
		{
			get
			{
				return NewLinkPrototype?.GetType();
			}
			set
			{
				if (value != null)
				{
					NewLinkPrototype = (GoObject)Activator.CreateInstance(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a <see cref="T:Northwoods.Go.GoObject" /> that is copied when a new link is created.
		/// </summary>
		/// <value>
		/// The <see cref="T:Northwoods.Go.GoObject" /> must implement the <see cref="T:Northwoods.Go.IGoLink" /> interface
		/// with a no-argument constructor.
		/// The value must not be null, either.  You can disable user-drawn links by setting
		/// <see cref="P:Northwoods.Go.GoView.AllowLink" /> to false instead.
		/// </value>
		/// <remarks>
		/// <para>
		/// The <see cref="M:Northwoods.Go.GoView.CreateLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> method uses this property to determine
		/// what kind of link to create.  It is also used by the <see cref="T:Northwoods.Go.GoToolLinking" />
		/// tool's <see cref="M:Northwoods.Go.GoToolLinking.CreateTemporaryLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" /> method.
		/// </para>
		/// <para>
		/// With this property, you can more easily initialize the properties of each new link
		/// to be created, by setting the properties once on this prototype object, rather
		/// than each time in a <see cref="E:Northwoods.Go.GoView.LinkCreated" /> event handler.
		/// For coding purposes, it may be more convenient to refer to either
		/// the <see cref="P:Northwoods.Go.GoView.NewGoLink" /> property to set <see cref="T:Northwoods.Go.GoLink" /> properties
		/// if this object is a <see cref="T:Northwoods.Go.GoLink" />,
		/// or to the <see cref="P:Northwoods.Go.GoView.NewGoLabeledLink" /> property to set <see cref="T:Northwoods.Go.GoLabeledLink" />
		/// properties, if this object is a <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// </para>
		/// </remarks>
		[Category("Behavior")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("An instance of a link to be copied when linking.")]
		public virtual GoObject NewLinkPrototype
		{
			get
			{
				return myNewLinkPrototype;
			}
			set
			{
				if (myNewLinkPrototype != value && value != null)
				{
					if (!(value is IGoLink))
					{
						throw new ArgumentException("New prototype object for GoView.NewLinkPrototype must implement IGoLink");
					}
					myNewLinkPrototype = value;
					RaisePropertyChangedEvent("NewLinkPrototype");
				}
			}
		}

		/// <summary>
		/// This convenience property safely casts the value of <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" />
		/// as a <see cref="T:Northwoods.Go.GoLink" />.
		/// </summary>
		/// <value>this may be null if <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" /> is not
		/// an instance of <see cref="T:Northwoods.Go.GoLink" /></value>
		[Category("Behavior")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The value of NewLinkPrototype, if it is a GoLink.")]
		public GoLink NewGoLink
		{
			get
			{
				return NewLinkPrototype as GoLink;
			}
			set
			{
				NewLinkPrototype = value;
			}
		}

		/// <summary>
		/// This convenience property safely casts the value of <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" />
		/// as a <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// </summary>
		/// <value>this may be null if <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" /> is not
		/// an instance of <see cref="T:Northwoods.Go.GoLabeledLink" /></value>
		[Category("Behavior")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The value of NewLinkPrototype, if it is a GoLabeledLink.")]
		public GoLabeledLink NewGoLabeledLink
		{
			get
			{
				return NewLinkPrototype as GoLabeledLink;
			}
			set
			{
				NewLinkPrototype = value;
			}
		}

		/// <summary>
		/// Gets or sets the distance at which potential links will snap to valid ports.
		/// </summary>
		/// <value>
		/// The <c>float</c> value must be positive, indicating the document coordinate
		/// distance from the mouse to the prospective port.
		/// The default value is <c>100.0f</c>.
		/// </value>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoToolLinking" /> tool uses this property to decide when the
		/// user's drag of a new link or relink is "close enough" to a valid port that the
		/// tool should draw the proposed link directly to that port.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(100f)]
		[Description("The distance at which potential links will snap to valid ports.")]
		public virtual float PortGravity
		{
			get
			{
				return myPortGravity;
			}
			set
			{
				if (myPortGravity != value)
				{
					if (value <= 0f)
					{
						throw new ArgumentException("New distance value for GoView.PortGravity must be positive");
					}
					myPortGravity = value;
					RaisePropertyChangedEvent("PortGravity");
				}
			}
		}

		/// <summary>
		/// Gets or sets how long a mouse should stay at one spot before a
		/// hover event occurs.
		/// </summary>
		/// <value>
		/// The time is in milliseconds.  The default is 1000 (one second).
		/// </value>
		[Category("Behavior")]
		[DefaultValue(1000)]
		[Description("How long a mouse should stay at one spot before a hover event occurs.")]
		public int HoverDelay
		{
			get
			{
				return myHoverDelay;
			}
			set
			{
				if (myHoverDelay != value)
				{
					myHoverDelay = value;
					RaisePropertyChangedEvent("HoverDelay");
				}
			}
		}

		/// <summary>
		/// Gets or sets the default Cursor for this view.
		/// </summary>
		/// <value>
		/// Setting this property to null will reset this value to the
		/// current <c>Cursor</c> property for this control.
		/// </value>
		/// <remarks>
		/// The normal behavior for a view is that when the mouse is not over
		/// any object, or if it is over no object for which
		/// <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> returns true, then 
		/// the view's <c>Cursor</c> property is set to the value of
		/// its <see cref="P:Northwoods.Go.GoView.DefaultCursor" /> property.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />
		/// <seealso cref="M:Northwoods.Go.GoView.DoBackgroundMouseOver(Northwoods.Go.GoInputEventArgs)" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Cursor DefaultCursor
		{
			get
			{
				if (myDefaultCursor == null)
				{
					return base.DefaultCursor;
				}
				return myDefaultCursor;
			}
			set
			{
				if (myDefaultCursor != value)
				{
					myDefaultCursor = value;
					RaisePropertyChangedEvent("DefaultCursor");
				}
			}
		}

		/// <summary>
		/// We override this property in order to remember the <see cref="P:Northwoods.Go.GoView.DefaultCursor" />
		/// if it had not been set explicitly.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Cursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				SetCursor(value);
			}
		}

		/// <summary>
		/// Gets or sets the view's <c>Cursor</c> given a standard cursor name,
		/// rather than a <c>Windows.Forms.Cursor</c> object.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoView.StandardizeCursorName(System.String)" />
		[Category("Behavior")]
		[DefaultValue("default")]
		[Description("The standard cursor name for the default cursor for this view")]
		public virtual string CursorName
		{
			get
			{
				Cursor cursor = Cursor;
				if (cursor == DefaultCursor)
				{
					return "default";
				}
				if (cursor == Cursors.AppStarting)
				{
					return "appstarting";
				}
				if (cursor == Cursors.Arrow)
				{
					return "arrow";
				}
				if (cursor == Cursors.Cross)
				{
					return "crosshair";
				}
				if (cursor == Cursors.Hand)
				{
					return "pointer";
				}
				if (cursor == Cursors.Help)
				{
					return "help";
				}
				if (cursor == Cursors.HSplit)
				{
					return "row-resize";
				}
				if (cursor == Cursors.IBeam)
				{
					return "text";
				}
				if (cursor == Cursors.No)
				{
					return "not-allowed";
				}
				if (cursor == Cursors.NoMove2D)
				{
					return "nomove2d";
				}
				if (cursor == Cursors.NoMoveHoriz)
				{
					return "nomovehoriz";
				}
				if (cursor == Cursors.NoMoveVert)
				{
					return "nomovevert";
				}
				if (cursor == Cursors.PanEast)
				{
					return "paneast";
				}
				if (cursor == Cursors.PanNE)
				{
					return "panne";
				}
				if (cursor == Cursors.PanNorth)
				{
					return "pannorth";
				}
				if (cursor == Cursors.PanNW)
				{
					return "pannw";
				}
				if (cursor == Cursors.PanSE)
				{
					return "panse";
				}
				if (cursor == Cursors.PanSouth)
				{
					return "pansouth";
				}
				if (cursor == Cursors.PanSW)
				{
					return "pansw";
				}
				if (cursor == Cursors.PanWest)
				{
					return "panwest";
				}
				if (cursor == Cursors.SizeAll)
				{
					return "move";
				}
				if (cursor == Cursors.SizeNESW)
				{
					return "ne-resize";
				}
				if (cursor == Cursors.SizeNS)
				{
					return "s-resize";
				}
				if (cursor == Cursors.SizeNWSE)
				{
					return "se-resize";
				}
				if (cursor == Cursors.SizeWE)
				{
					return "e-resize";
				}
				if (cursor == Cursors.UpArrow)
				{
					return "uparrow";
				}
				if (cursor == Cursors.VSplit)
				{
					return "col-resize";
				}
				if (cursor == Cursors.WaitCursor)
				{
					return "wait";
				}
				return null;
			}
			set
			{
				Cursor cursor = null;
				switch (StandardizeCursorName(value))
				{
				case "default":
					cursor = DefaultCursor;
					break;
				case "pointer":
					cursor = Cursors.Hand;
					break;
				case "hand":
					cursor = Cursors.Hand;
					break;
				case "move":
					cursor = Cursors.SizeAll;
					break;
				case "n-resize":
					cursor = Cursors.SizeNS;
					break;
				case "ne-resize":
					cursor = Cursors.SizeNESW;
					break;
				case "nw-resize":
					cursor = Cursors.SizeNWSE;
					break;
				case "s-resize":
					cursor = Cursors.SizeNS;
					break;
				case "se-resize":
					cursor = Cursors.SizeNWSE;
					break;
				case "sw-resize":
					cursor = Cursors.SizeNESW;
					break;
				case "e-resize":
					cursor = Cursors.SizeWE;
					break;
				case "w-resize":
					cursor = Cursors.SizeWE;
					break;
				case "crosshair":
					cursor = Cursors.Cross;
					break;
				case "col-resize":
					cursor = Cursors.VSplit;
					break;
				case "row-resize":
					cursor = Cursors.HSplit;
					break;
				case "text":
					cursor = Cursors.IBeam;
					break;
				case "help":
					cursor = Cursors.Help;
					break;
				case "wait":
					cursor = Cursors.WaitCursor;
					break;
				case "not-allowed":
					cursor = Cursors.No;
					break;
				case "appstarting":
					cursor = Cursors.AppStarting;
					break;
				case "arrow":
					cursor = Cursors.Arrow;
					break;
				case "nomove2d":
					cursor = Cursors.NoMove2D;
					break;
				case "nomovehoriz":
					cursor = Cursors.NoMoveHoriz;
					break;
				case "nomovevert":
					cursor = Cursors.NoMoveVert;
					break;
				case "paneast":
					cursor = Cursors.PanEast;
					break;
				case "panne":
					cursor = Cursors.PanNE;
					break;
				case "pannorth":
					cursor = Cursors.PanNorth;
					break;
				case "pannw":
					cursor = Cursors.PanNW;
					break;
				case "panse":
					cursor = Cursors.PanSE;
					break;
				case "pansouth":
					cursor = Cursors.PanSouth;
					break;
				case "pansw":
					cursor = Cursors.PanSW;
					break;
				case "panwest":
					cursor = Cursors.PanWest;
					break;
				case "uparrow":
					cursor = Cursors.UpArrow;
					break;
				}
				SetCursor(cursor);
			}
		}

		/// <summary>
		/// Gets or sets the ToolTip component for this view.
		/// </summary>
		/// <value>
		/// By default each view has a <c>ToolTip</c> component allocated.
		/// If the value is null, all tooltips are disabled for this view.
		/// </value>
		/// <remarks>
		/// Setting this property to null to turn off all tooltip related activity
		/// will reduce the overhead involved when moving the mouse over the view.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoToolTipObject(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ToolTip ToolTip
		{
			get
			{
				return myToolTip;
			}
			set
			{
				if (myToolTip != value)
				{
					myToolTip = value;
					RaisePropertyChangedEvent("ToolTip");
				}
			}
		}

		/// <summary>
		/// Gets or sets a string to be displayed in a tooltip when no
		/// tooltip text is shown for a particular <see cref="T:Northwoods.Go.GoObject" />.
		/// </summary>
		/// <value>
		/// The initial value is the empty string, which means don't display a tooltip.
		/// </value>
		/// <remarks>
		/// Note that this is different from the same property that you may find
		/// on some <see cref="T:Northwoods.Go.GoObject" /> classes such as <see cref="T:Northwoods.Go.GoNode" />
		/// (<see cref="P:Northwoods.Go.GoNode.ToolTipText" />), since here an empty string is
		/// the same as a value of null/Nothing, meaning not to display a tooltip.
		/// For <see cref="T:Northwoods.Go.GoObject" />s a value from <see cref="M:Northwoods.Go.GoObject.GetToolTip(Northwoods.Go.GoView)" />
		/// of an empty string will display an empty tooltip, and only a value of
		/// null/Nothing will indicate no tooltip for that particular <see cref="T:Northwoods.Go.GoObject" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoToolTipObject(Northwoods.Go.GoObject)" />
		[Category("Behavior")]
		[DefaultValue("")]
		[Description("A string to be displayed in a tooltip.")]
		public virtual string ToolTipText
		{
			get
			{
				return myToolTipText;
			}
			set
			{
				if (myToolTipText != value)
				{
					myToolTipText = value;
					RaisePropertyChangedEvent("ToolTipText");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether a user's drag of the selection occurs continuously.
		/// [In Web Forms there is no continuous visual feedback of a drag.]
		/// </summary>
		/// <value>
		/// If this value is true, the objects in the selection are actually moved
		/// continuously, following the mouse.
		/// If this value is false, the user actually drags around an image of
		/// the selection, and the move only happens on mouse up.
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// One advantage of a false value for this property is that only one undo record
		/// per moved object is generated for a user's moving operation--namely the
		/// final one on mouse up.
		/// Another advantage, when this property is false, is that a user's drag of
		/// some objects to another window will not actually modify the source document
		/// due to objects moved before the pointer leaves the view.
		/// However, many applications will need this property to be true so as to provide
		/// accurate continuous feedback to the user regarding where a move or copy will
		/// occur, particularly when grids or other snapping behaviors are involved.
		/// If you have many links that are expensive to route, but have to have this
		/// property set to true, you might want to consider setting <see cref="P:Northwoods.Go.GoView.DragRoutesRealtime" />
		/// to false, to delay routing until the end of a drag.
		/// This property is used by <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// This property should be set to true when <c>ExternalDragDropsOnEnter</c> is set to true.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether a user's drag of the selection occurs continuously instead of dragging an outline.")]
		public virtual bool DragsRealtime
		{
			get
			{
				return myDragsRealtime;
			}
			set
			{
				if (myDragsRealtime != value)
				{
					myDragsRealtime = value;
					RaisePropertyChangedEvent("DragsRealtime");
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the routing of links happens continuously
		/// when <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> is true.
		/// [In Web Forms there is no continuous visual feedback of the rerouting of links.]
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		/// <remarks>
		/// Setting this to false will improve dragging performance, particularly
		/// when there are <see cref="P:Northwoods.Go.GoLink.Orthogonal" /> links that
		/// <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" />, at the expense of not maintaining
		/// apparent link connections during the drag.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether a user's drag of the selection continuously reroutes links.")]
		public virtual bool DragRoutesRealtime
		{
			get
			{
				return myDragRoutesRealtime;
			}
			set
			{
				myDragRoutesRealtime = value;
			}
		}

		/// <summary>
		/// Gets and sets whether this view accepts drop events of drag-and-drop operations.
		/// </summary>
		/// <value>The default value is true</value>
		/// <remarks>
		/// If there is a <c>SecurityException</c> when setting this property,
		/// the <see cref="P:Northwoods.Go.GoView.AllowDragOut" /> property is set to false.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether to accept drag-and-drop events")]
		public override bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}
			set
			{
				try
				{
					InitAllowDrop2(value);
				}
				catch (VerificationException)
				{
					AllowDragOut = false;
				}
				catch (SecurityException)
				{
					AllowDragOut = false;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can drag the selection out of this view to another window.
		/// </summary>
		/// <value>
		/// If this value is true, the user can drag selected objects out of this view to another window.
		/// If this value is false, the user cannot do so, although drags within this view may still
		/// be allowed.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property is used by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool.
		/// </para>
		/// <para>
		/// You can make a view so that the user cannot drag within it, but the user can copy out
		/// from it to another window, by setting <c>AllowDrop</c> to false and
		/// <see cref="P:Northwoods.Go.GoView.AllowDragOut" /> and <see cref="P:Northwoods.Go.GoView.AllowCopy" />to true.
		/// <see cref="T:Northwoods.Go.GoPalette" /> does this.
		/// </para>
		/// <para>
		/// You can make a view so that the user can drag-and-drop around within it, but the user
		/// cannot drag anything out from it, by setting <c>AllowDrop</c> to true and
		/// <see cref="P:Northwoods.Go.GoView.AllowDragOut" /> to false.  The behaviors that are allowed within the view
		/// are additionally governed by <see cref="P:Northwoods.Go.GoView.AllowCopy" /> and <see cref="P:Northwoods.Go.GoView.AllowMove" />.
		/// </para>
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can drag the selection out of this view to another window.")]
		public bool AllowDragOut
		{
			get
			{
				return myAllowDragOut;
			}
			set
			{
				if (myAllowDragOut != value)
				{
					myAllowDragOut = value;
					RaisePropertyChangedEvent("AllowDragOut");
				}
			}
		}

		internal bool PretendsInternalDrag
		{
			get
			{
				return myPretendsInternalDrag;
			}
			set
			{
				myPretendsInternalDrag = value;
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> should call <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" />
		/// immediately, to actually create the dropped objects and add them to the document,
		/// and so that the user's drag-and-drop from another window will use an instance
		/// of a <see cref="T:Northwoods.Go.GoToolDragging" /> as the current <see cref="P:Northwoods.Go.GoView.Tool" /> for dragging
		/// around those newly dropped objects.
		/// </summary>
		/// <value>By default this is false.  If you set this to true, you will also need
		/// to set <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> to true.</value>
		/// <remarks>
		/// The current <see cref="P:Northwoods.Go.GoView.Selection" /> is deleted in <see cref="M:Northwoods.Go.GoView.OnDragLeave(System.EventArgs)" />
		/// or when the drag-and-drop is cancelled.
		/// When this value is true, no external drag image is used
		/// (i.e., <see cref="M:Northwoods.Go.GoView.GetExternalDragImage(System.Windows.Forms.DragEventArgs)" /> is not called).
		/// This property only makes sense when <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> is true.
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether the user drags newly dropped objects on a drag enter.")]
		public bool ExternalDragDropsOnEnter
		{
			get
			{
				return myExternalDragDropsOnEnter;
			}
			set
			{
				if (myExternalDragDropsOnEnter != value)
				{
					myExternalDragDropsOnEnter = value;
					RaisePropertyChangedEvent("ExternalDragDropsOnEnter");
				}
			}
		}

		/// <summary>
		/// Gets whether any painting that is ongoing is part of a printing operation.
		/// </summary>
		/// <value>
		/// This value is true when indirectly invoked by the <see cref="M:Northwoods.Go.GoView.Print" /> method.
		/// </value>
		[Browsable(false)]
		public virtual bool IsPrinting => myPrintInfo != null;

		/// <summary>
		/// Gets the size of the document to be printed.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value represents the size of the portion of the document to be printed,
		/// in document coordinates.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is normally just the size needed to include all of the objects in the document,
		/// by calling <c>ComputeDocumentBounds()</c>.
		/// This avoids printing a lot of empty pages
		/// if the document size is much larger than needed, perhaps because some objects have been
		/// deleted, but the document size has not been shrunk accordingly.
		/// </para>
		/// <para>
		/// When the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// only one page is printed, corresponding to where the sheet is.  In these cases,
		/// this property has the value of the sheet's <see cref="P:Northwoods.Go.GoSheet.MarginBounds" /> size.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.PrintDocumentTopLeft" />
		/// <seealso cref="P:Northwoods.Go.GoView.DocumentSize" />
		[Category("Printing")]
		public virtual SizeF PrintDocumentSize
		{
			get
			{
				if (SheetStyle != 0 && Sheet != null)
				{
					RectangleF marginBounds = Sheet.MarginBounds;
					return new SizeF(marginBounds.Width, marginBounds.Height);
				}
				RectangleF rectangleF = ComputeDocumentBounds();
				PointF a = new PointF(rectangleF.X + rectangleF.Width, rectangleF.Y + rectangleF.Height);
				PointF printDocumentTopLeft = PrintDocumentTopLeft;
				return GoTool.SubtractPoints(a, printDocumentTopLeft);
			}
		}

		/// <summary>
		/// Gets the top-left position of the document to be printed.
		/// </summary>
		/// <value>
		/// This <c>PointF</c> value represents the top-left document coordinate of the portion
		/// of the document to be printed.
		/// </value>
		/// <remarks>
		/// <para>
		/// This is normally the top-left point of the bounds returned by <see cref="M:Northwoods.Go.GoView.ComputeDocumentBounds" />,
		/// but may be overridden for custom behaviors.
		/// When <see cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" /> is false, this just returns the origin.
		/// </para>
		/// <para>
		/// When the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// only one page is printed, corresponding to where the sheet is.  In these cases,
		/// this property has the value of the sheet's <see cref="P:Northwoods.Go.GoSheet.MarginBounds" /> location.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.PrintDocumentSize" />
		/// <seealso cref="P:Northwoods.Go.GoView.DocumentTopLeft" />
		[Category("Printing")]
		public virtual PointF PrintDocumentTopLeft
		{
			get
			{
				if (SheetStyle != 0 && Sheet != null)
				{
					RectangleF marginBounds = Sheet.MarginBounds;
					return new PointF(marginBounds.X, marginBounds.Y);
				}
				if (ShowsNegativeCoordinates)
				{
					return ComputeDocumentBounds().Location;
				}
				return default(PointF);
			}
		}

		/// <summary>
		/// Gets or sets the scale at which we should print.
		/// </summary>
		/// <value>
		/// The value must be positive.
		/// The default value is less than 1.0f.
		/// </value>
		/// <remarks>
		/// This serves a purpose very much like that of <see cref="P:Northwoods.Go.GoView.DocScale" />, but for
		/// printing rather than normal painting.
		/// If there is a <see cref="P:Northwoods.Go.GoView.Sheet" />, its <see cref="P:Northwoods.Go.GoObject.Size" />,
		/// <see cref="P:Northwoods.Go.GoSheet.TopLeftMargin" />, and <see cref="P:Northwoods.Go.GoSheet.BottomRightMargin" />
		/// are rescaled accordingly.  No grid properties of the sheet are modified.
		/// </remarks>
		[Category("Printing")]
		[DefaultValue(0.8f)]
		[Description("The scale at which we should print.")]
		public virtual float PrintScale
		{
			get
			{
				return myPrintScale;
			}
			set
			{
				float num = myPrintScale;
				if (num != value)
				{
					if (value <= 0f)
					{
						throw new ArgumentException("New value for GoView.PrintScale must be positive");
					}
					myPrintScale = value;
					RaisePropertyChangedEvent("PrintScale");
					GoSheet sheet = Sheet;
					if (sheet != null)
					{
						float num2 = num / value;
						sheet.Size = new SizeF(sheet.Width * num2, sheet.Height * num2);
						sheet.TopLeftMargin = new SizeF(sheet.TopLeftMargin.Width * num2, sheet.TopLeftMargin.Height * num2);
						sheet.BottomRightMargin = new SizeF(sheet.BottomRightMargin.Width * num2, sheet.BottomRightMargin.Height * num2);
					}
					UpdateExtent();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoView.PrintView(System.Drawing.Graphics,System.Drawing.RectangleF)" /> should print objects that
		/// belong to view layers as well as all document objects.
		/// </summary>
		/// <value>By default this is false.</value>
		/// <remarks>
		/// You can disable printing layers of objects and individual objects by setting
		/// <see cref="T:Northwoods.Go.GoLayer" />.<see cref="P:Northwoods.Go.GoLayer.AllowPrint" /> or
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Printable" /> to false.
		/// </remarks>
		[Category("Printing")]
		[DefaultValue(false)]
		[Description("Whether to print view objects such as selection handles, as well as document objects")]
		public virtual bool PrintsViewObjects
		{
			get
			{
				return myPrintsViewObjects;
			}
			set
			{
				if (myPrintsViewObjects != value)
				{
					myPrintsViewObjects = value;
					RaisePropertyChangedEvent("PrintsViewObjects");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoGrid" /> which is held by
		/// this view's <see cref="P:Northwoods.Go.GoView.BackgroundLayer" /> and which by default implements
		/// the various grid properties of this view.
		/// </summary>
		/// <value>
		/// The initial value is provided by <see cref="M:Northwoods.Go.GoView.CreateGrid" />.
		/// </value>
		/// <remarks>
		/// The various "Grid..." properties get and set the corresponding
		/// properties of this <see cref="T:Northwoods.Go.GoGrid" /> object.
		/// If a <see cref="P:Northwoods.Go.GoView.Sheet" /> is visible, it too has a <see cref="T:Northwoods.Go.GoGrid" />
		/// that is independent of this grid.  The sheet's grid just covers
		/// the sheet, whereas this grid is unbounded.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoGrid BackgroundGrid
		{
			get
			{
				return myBackgroundGrid;
			}
			set
			{
				GoGrid goGrid = myBackgroundGrid;
				if (goGrid == value)
				{
					return;
				}
				GoLayer goLayer = BackgroundLayer;
				if (goGrid != null)
				{
					if (goGrid.Layer != null)
					{
						goLayer = goGrid.Layer;
					}
					goGrid.Remove();
				}
				myBackgroundGrid = value;
				goLayer.Add(myBackgroundGrid);
				if (goGrid != null && value != null)
				{
					value.Visible = goGrid.Visible;
					value.Printable = goGrid.Printable;
					value.Selectable = goGrid.Selectable;
				}
				RaisePropertyChangedEvent("Grid");
			}
		}

		/// <summary>
		/// Gets or sets the primary <see cref="T:Northwoods.Go.GoGrid" /> used by this view.
		/// </summary>
		/// <value>This will normally be either the <see cref="P:Northwoods.Go.GoView.Sheet" />'s <see cref="P:Northwoods.Go.GoSheet.Grid" />
		/// or the <see cref="P:Northwoods.Go.GoView.BackgroundGrid" />.
		/// </value>
		/// <remarks>
		/// Because the initial value of <see cref="P:Northwoods.Go.GoView.Sheet" /> is null/nothing,
		/// the initial value of this property will be the <see cref="P:Northwoods.Go.GoView.BackgroundGrid" />.
		/// Setting the <see cref="P:Northwoods.Go.GoView.Sheet" /> property to a <see cref="T:Northwoods.Go.GoSheet" />,
		/// either explicitly, or implicitly by setting <see cref="P:Northwoods.Go.GoView.BackgroundHasSheet" /> to true,
		/// will cause this property's value to refer to the <see cref="P:Northwoods.Go.GoView.Sheet" />'s
		/// <see cref="P:Northwoods.Go.GoSheet.Grid" />.
		/// Setting <see cref="P:Northwoods.Go.GoView.Sheet" /> to null/nothing again, or setting
		/// <see cref="P:Northwoods.Go.GoView.BackgroundHasSheet" /> to false, will cause this
		/// property to refer back to the original <see cref="P:Northwoods.Go.GoView.BackgroundGrid" />.
		/// Since this property is used by many of this view's "<c>Grid...</c>" properties,
		/// all of those properties may suddenly change value as this <see cref="T:Northwoods.Go.GoGrid" />
		/// reference is changed.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoGrid Grid
		{
			get
			{
				GoGrid goGrid = null;
				GoSheet sheet = Sheet;
				if (sheet != null)
				{
					goGrid = sheet.Grid;
				}
				if (goGrid == null)
				{
					goGrid = BackgroundGrid;
				}
				return goGrid;
			}
			set
			{
				GoSheet sheet = Sheet;
				if (sheet != null)
				{
					sheet.Grid = value;
				}
				else
				{
					BackgroundGrid = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the origin for the <see cref="P:Northwoods.Go.GoView.Grid" />.
		/// </summary>
		/// <value>
		/// This <c>PointF</c> value is a document coordinate point.
		/// The default value is (0, 0).
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoView.GridStyle" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.Origin" />
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PointF GridOrigin
		{
			get
			{
				if (Grid != null)
				{
					return Grid.Origin;
				}
				return new PointF(0f, 0f);
			}
			set
			{
				if (Grid != null)
				{
					Grid.Origin = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of each cell in the <see cref="P:Northwoods.Go.GoView.Grid" />.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value describes the size of each cell in document coordinates.
		/// The <c>Width</c> and <c>Height</c> must be positive.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoView.GridStyle" />
		/// <seealso cref="P:Northwoods.Go.GoGrid.CellSize" />
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SizeF GridCellSize
		{
			get
			{
				if (Grid != null)
				{
					return Grid.CellSize;
				}
				return new SizeF(50f, 50f);
			}
			set
			{
				if (Grid != null)
				{
					Grid.CellSize = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(GoViewGridStyle.None)]
		[Description("The appearance style of the grid.")]
		public virtual GoViewGridStyle GridStyle
		{
			get
			{
				if (Grid != null)
				{
					return Grid.Style;
				}
				return GoViewGridStyle.None;
			}
			set
			{
				if (Grid != null)
				{
					Grid.Style = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The origin for the grid.")]
		public float GridOriginX
		{
			get
			{
				if (Grid != null)
				{
					return Grid.Origin.X;
				}
				return 0f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.Origin = new PointF(value, Grid.Origin.Y);
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The origin for the grid.")]
		public float GridOriginY
		{
			get
			{
				if (Grid != null)
				{
					return Grid.Origin.Y;
				}
				return 0f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.Origin = new PointF(Grid.Origin.X, value);
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(false)]
		[Description("Whether the Origin moves along with the GoGrid's Position")]
		public bool GridOriginRelative
		{
			get
			{
				if (Grid != null)
				{
					return Grid.OriginRelative;
				}
				return false;
			}
			set
			{
				if (Grid != null)
				{
					Grid.OriginRelative = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(50f)]
		[Description("The size of each cell in the grid.")]
		public float GridCellSizeWidth
		{
			get
			{
				if (Grid != null)
				{
					return Grid.CellSize.Width;
				}
				return 50f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.CellSize = new SizeF(value, Grid.CellSize.Height);
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(50f)]
		[Description("The size of each cell in the grid.")]
		public float GridCellSizeHeight
		{
			get
			{
				if (Grid != null)
				{
					return Grid.CellSize.Height;
				}
				return 50f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.CellSize = new SizeF(Grid.CellSize.Width, value);
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[Description("The color used in drawing the grid lines.")]
		public Color GridLineColor
		{
			get
			{
				if (Grid != null)
				{
					return Grid.LineColor;
				}
				return Color.LightGray;
			}
			set
			{
				if (Grid != null)
				{
					Grid.LineColor = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(10)]
		[Description("The directions in which the grid is infinite, a bitwise-OR of the standard eight GoObject spots")]
		public int GridUnboundedSpots
		{
			get
			{
				if (Grid != null)
				{
					return Grid.UnboundedSpots;
				}
				return 10;
			}
			set
			{
				if (Grid != null)
				{
					Grid.UnboundedSpots = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[Description("The color used in drawing the grid lines.")]
		public Color GridMajorLineColor
		{
			get
			{
				if (Grid != null)
				{
					return Grid.MajorLineColor;
				}
				return Color.Gray;
			}
			set
			{
				if (Grid != null)
				{
					Grid.MajorLineColor = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The width of the pen used in drawing the grid lines.")]
		public float GridLineWidth
		{
			get
			{
				if (Grid != null)
				{
					return Grid.LineWidth;
				}
				return 0f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.LineWidth = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(DashStyle.Solid)]
		[Description("The pen dash style used in drawing the grid lines.")]
		public DashStyle GridLineDashStyle
		{
			get
			{
				if (Grid != null)
				{
					return Grid.LineDashStyle;
				}
				return DashStyle.Solid;
			}
			set
			{
				if (Grid != null)
				{
					Grid.LineDashStyle = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[Description("The pattern of dashes used in drawing the grid lines, when the LineDashStyle is DashStyle.Custom")]
		public float[] GridLineDashPattern
		{
			get
			{
				if (Grid != null)
				{
					return Grid.LineDashPattern;
				}
				return GoGrid.DefaultLineDashPattern;
			}
			set
			{
				if (Grid != null)
				{
					Grid.LineDashPattern = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(0f)]
		[Description("The width of the pen used in drawing the grid lines.")]
		public float GridMajorLineWidth
		{
			get
			{
				if (Grid != null)
				{
					return Grid.MajorLineWidth;
				}
				return 0f;
			}
			set
			{
				if (Grid != null)
				{
					Grid.MajorLineWidth = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(DashStyle.Solid)]
		[Description("The pen dash style used in drawing the grid lines.")]
		public DashStyle GridMajorLineDashStyle
		{
			get
			{
				if (Grid != null)
				{
					return Grid.MajorLineDashStyle;
				}
				return DashStyle.Solid;
			}
			set
			{
				if (Grid != null)
				{
					Grid.MajorLineDashStyle = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[Description("The pattern of dashes used in drawing the grid lines, when the MajorLineDashStyle is DashStyle.Custom")]
		public float[] GridMajorLineDashPattern
		{
			get
			{
				if (Grid != null)
				{
					return Grid.MajorLineDashPattern;
				}
				return GoGrid.DefaultMajorLineDashPattern;
			}
			set
			{
				if (Grid != null)
				{
					Grid.MajorLineDashPattern = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[Description("How often major lines should be drawn; zero means never.")]
		public Size GridMajorLineFrequency
		{
			get
			{
				if (Grid != null)
				{
					return Grid.MajorLineFrequency;
				}
				return new Size(0, 0);
			}
			set
			{
				if (Grid != null)
				{
					Grid.MajorLineFrequency = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(GoViewSnapStyle.None)]
		[Description("The interactive dragging behavior for positioning objects.")]
		public virtual GoViewSnapStyle GridSnapDrag
		{
			get
			{
				if (Grid != null)
				{
					return Grid.SnapDrag;
				}
				return GoViewSnapStyle.None;
			}
			set
			{
				if (Grid != null)
				{
					Grid.SnapDrag = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(GoViewSnapStyle.None)]
		[Description("The interactive resizing behavior for resizing objects.")]
		public virtual GoViewSnapStyle GridSnapResize
		{
			get
			{
				if (Grid != null)
				{
					return Grid.SnapResize;
				}
				return GoViewSnapStyle.None;
			}
			set
			{
				if (Grid != null)
				{
					Grid.SnapResize = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(true)]
		[Description("Whether GoView.SnapPoint should look at any grid that might be behind this one.")]
		public bool GridSnapOpaque
		{
			get
			{
				if (Grid != null)
				{
					return Grid.SnapOpaque;
				}
				return true;
			}
			set
			{
				if (Grid != null)
				{
					Grid.SnapOpaque = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Grid" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundGrid" />
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Sheet" />
		[Category("Grid")]
		[DefaultValue(2)]
		[Description("Which cell spot should be returned by GoGrid.FindNearestGridPoint.")]
		public int GridSnapCellSpot
		{
			get
			{
				if (Grid != null)
				{
					return Grid.SnapCellSpot;
				}
				return 2;
			}
			set
			{
				if (Grid != null)
				{
					Grid.SnapCellSpot = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[DefaultValue(1)]
		[Description("The spot at which the BackgroundImage is located relative to the MarginBounds")]
		public int SheetBackgroundImageSpot
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.BackgroundImageSpot;
				}
				return 1;
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.BackgroundImageSpot = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The bounds of the sheet of paper; should be set programmatically at run-time")]
		public RectangleF SheetPaperBounds
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.Paper.Bounds;
				}
				return default(RectangleF);
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.Paper.Bounds = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The bounding rectangle inside the margins")]
		public RectangleF SheetMarginBounds
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.MarginBounds;
				}
				return default(RectangleF);
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The margins along the left side and top; should be set programmatically at run-time")]
		public SizeF SheetTopLeftMargin
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.TopLeftMargin;
				}
				return default(SizeF);
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.TopLeftMargin = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The margins along the right side and the bottom; should be set programmatically at run-time")]
		public SizeF SheetBottomRightMargin
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.BottomRightMargin;
				}
				return default(SizeF);
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.BottomRightMargin = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[DefaultValue(true)]
		[Description("Whether this paints the marginal areas with the MarginColor")]
		public bool SheetShowsMargins
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.ShowsMargins;
				}
				return true;
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.ShowsMargins = value;
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to <see cref="P:Northwoods.Go.GoView.Sheet" /> properties.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[Description("The color for the margins, including alpha transparency")]
		public Color SheetMarginColor
		{
			get
			{
				if (Sheet != null)
				{
					return Sheet.MarginColor;
				}
				return GoSheet.DefaultMarginColor;
			}
			set
			{
				if (Sheet != null)
				{
					Sheet.MarginColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoObject" /> that represents the sheet of paper the user sees,
		/// when the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="T:Northwoods.Go.GoViewSheetStyle" />.<see cref="F:Northwoods.Go.GoViewSheetStyle.None" />.
		/// </summary>
		/// <value>
		/// The initial value is null/nothing.
		/// </value>
		/// <remarks>
		/// Setting this property will implicitly change the value of <see cref="P:Northwoods.Go.GoView.Grid" />
		/// from a reference to <see cref="P:Northwoods.Go.GoView.BackgroundGrid" /> to a reference to
		/// the new <see cref="P:Northwoods.Go.GoView.Sheet" />'s <see cref="P:Northwoods.Go.GoSheet.Grid" />, if there had been no
		/// <see cref="P:Northwoods.Go.GoView.Sheet" /> before, or else from the old sheet's grid to the new sheet's grid.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		/// <seealso cref="P:Northwoods.Go.GoView.Grid" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoSheet Sheet
		{
			get
			{
				return mySheet;
			}
			set
			{
				GoSheet goSheet = mySheet;
				if (goSheet == value)
				{
					return;
				}
				GoLayer goLayer = BackgroundLayer;
				if (goSheet != null)
				{
					if (goSheet.Layer != null)
					{
						goLayer = goSheet.Layer;
					}
					goSheet.Remove();
				}
				mySheet = value;
				goLayer.Add(mySheet);
				if (goSheet != null && value != null)
				{
					value.Visible = goSheet.Visible;
					value.Printable = goSheet.Printable;
					value.Selectable = goSheet.Selectable;
				}
				RaisePropertyChangedEvent("Sheet");
			}
		}

		/// <summary>
		/// Gets or sets whether there is a value for <see cref="P:Northwoods.Go.GoView.Sheet" />, that is a
		/// <see cref="T:Northwoods.Go.GoSheet" /> in this view's <see cref="P:Northwoods.Go.GoView.BackgroundLayer" />.
		/// </summary>
		/// <value>
		/// Initially this is false, since there is initially no <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// Setting this to true when there is no <see cref="P:Northwoods.Go.GoView.Sheet" /> will call
		/// <see cref="M:Northwoods.Go.GoView.CreateSheet" /> and assign the <see cref="P:Northwoods.Go.GoView.Sheet" /> property.
		/// Setting this to false when there is a <see cref="P:Northwoods.Go.GoView.Sheet" /> will
		/// set the <see cref="P:Northwoods.Go.GoView.Sheet" /> property to null/nothing.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property just controls whether or not a <see cref="T:Northwoods.Go.GoSheet" />
		/// exists in the background layer as the <see cref="P:Northwoods.Go.GoView.Sheet" /> property.
		/// It does not control whether the sheet is actually visible at this time,
		/// nor the positioning of the sheet in the view, nor what appearance the sheet
		/// takes--that is controlled by the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> property
		/// and other <c>Sheet...</c> and <see cref="T:Northwoods.Go.GoSheet" /> properties.
		/// </para>
		/// <para>
		/// When there is a <see cref="P:Northwoods.Go.GoView.Sheet" />, all of the <c>Grid...</c>
		/// properties refer to the sheet's <see cref="P:Northwoods.Go.GoSheet.Grid" /> rather
		/// than to the view's <see cref="P:Northwoods.Go.GoView.BackgroundGrid" />.
		/// </para>
		/// </remarks>
		[Category("Sheet")]
		[DefaultValue(false)]
		[Description("Whether the background layer has a GoSheet and whether the Grid properties refer to the BackgroundGrid or to the Sheet's Grid; also set SheetStyle to make it visible")]
		public virtual bool BackgroundHasSheet
		{
			get
			{
				return Sheet != null;
			}
			set
			{
				if (value)
				{
					if (Sheet == null)
					{
						Sheet = CreateSheet();
						if (Sheet != null)
						{
							myPreviousCenter = Sheet.Center;
						}
						UpdateExtent();
					}
				}
				else
				{
					Sheet = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets how the view adjusts its <see cref="P:Northwoods.Go.GoView.DocScale" /> and <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// as the view's size changes.
		/// </summary>
		/// <value>
		/// The default value is <see cref="T:Northwoods.Go.GoViewSheetStyle" />.<see cref="F:Northwoods.Go.GoViewSheetStyle.None" />:
		/// the <see cref="P:Northwoods.Go.GoView.Sheet" /> (if any exists) is not visible.
		/// </value>
		/// <remarks>
		/// The value of this property affects whether the <see cref="P:Northwoods.Go.GoView.Sheet" /> is visible
		/// and the behavior of <see cref="M:Northwoods.Go.GoView.UpdateExtent" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		[Category("Sheet")]
		[DefaultValue(GoViewSheetStyle.None)]
		[Description("How to rescale and reposition the view as the view changes size")]
		public virtual GoViewSheetStyle SheetStyle
		{
			get
			{
				return mySheetStyle;
			}
			set
			{
				if (mySheetStyle != value)
				{
					mySheetStyle = value;
					RaisePropertyChangedEvent("SheetStyle");
					if (Sheet != null)
					{
						Sheet.Visible = (value != GoViewSheetStyle.None);
						myPreviousCenter = Sheet.Center;
					}
					UpdateExtent();
				}
			}
		}

		/// <summary>
		/// Gets or sets the how much of the background to show on each side of the sheet of paper,
		/// when the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />.
		/// </summary>
		/// <value>
		/// The value is in view coordinates; the default value is 10x10 pixels.
		/// </value>
		/// <remarks>
		/// The value of this property affects the behavior of <see cref="M:Northwoods.Go.GoView.UpdateExtent" />
		/// for certain values of <see cref="P:Northwoods.Go.GoView.SheetStyle" />.
		/// </remarks>
		[Category("Sheet")]
		[Description("The extra space on each side of the sheet of paper to be shown, depending on the SheetStyle")]
		public virtual Size SheetRoom
		{
			get
			{
				return mySheetRoom;
			}
			set
			{
				if (mySheetRoom != value)
				{
					mySheetRoom = value;
					RaisePropertyChangedEvent("SheetRoom");
					UpdateExtent();
				}
			}
		}

		/// <summary>
		/// Gets or sets the graphical object representing a <c>Control</c> used to edit an object
		/// in this view in a modal fashion.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoControl" /> object is added to the default view layer.  The
		/// assumption is that the editor that the <see cref="T:Northwoods.Go.GoControl" /> represents is
		/// used in a modal fashion--the corresponding <c>Control</c> is added to the view
		/// and gets focus.  When the <see cref="T:Northwoods.Go.GoControl" /> is removed from the view's layer,
		/// the corresponding <c>Control</c> loses focus and is removed from the view.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.IsEditing" />
		/// <seealso cref="M:Northwoods.Go.GoView.DoEndEdit" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GoControl EditControl
		{
			get
			{
				return myEditControl;
			}
			set
			{
				GoControl goControl = myEditControl;
				if (goControl != value)
				{
					if (goControl != null && goControl.View == this)
					{
						goControl.Remove();
					}
					if (value != null)
					{
						myEditControl = value;
						Layers.Default.Add(value);
						myModalControl = value.GetControl(this);
					}
				}
			}
		}

		/// <summary>
		/// Gets whether the user is currently editing an object.
		/// </summary>
		/// <value>
		/// This is true when <see cref="P:Northwoods.Go.GoView.EditControl" /> is non-null.
		/// </value>
		/// <seealso cref="M:Northwoods.Go.GoView.DoEndEdit" />
		[Browsable(false)]
		public virtual bool IsEditing => EditControl != null;

		/// <summary>
		/// Gets or sets the offset distance for drop shadows.
		/// </summary>
		/// <value>
		/// This <c>SizeF</c> value specifies the offset, where positive values move
		/// the shadow to the right and to the bottom.
		/// The default value is (5, 5).
		/// </value>
		/// <remarks>
		/// The shadow only appears for those objects that have the <see cref="P:Northwoods.Go.GoObject.Shadowed" />
		/// property set to true.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.ShadowColor" />
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SizeF ShadowOffset
		{
			get
			{
				return myShadowOffset;
			}
			set
			{
				if (myShadowOffset != value)
				{
					myShadowOffset = value;
					RaisePropertyChangedEvent("ShadowOffset");
				}
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to the ShadowOffset.
		/// </summary>
		[Category("Shadows")]
		[DefaultValue(5f)]
		[Description("The offset distance for drop shadows.")]
		public float ShadowWidth
		{
			get
			{
				return ShadowOffset.Width;
			}
			set
			{
				ShadowOffset = new SizeF(value, ShadowOffset.Height);
			}
		}

		/// <summary>
		/// Design-time and PropertyGrid access to the ShadowOffset.
		/// </summary>
		[Category("Shadows")]
		[DefaultValue(5f)]
		[Description("The offset distance for drop shadows.")]
		public float ShadowHeight
		{
			get
			{
				return ShadowOffset.Height;
			}
			set
			{
				ShadowOffset = new SizeF(ShadowOffset.Width, value);
			}
		}

		/// <summary>
		/// Gets or sets the color used for drawing drop shadows.
		/// </summary>
		/// <value>
		/// The default color is a partly transparent gray, so that users can see the objects
		/// underneath the shadow.
		/// </value>
		/// <remarks>
		/// The shadow only appears for those objects that have the <see cref="P:Northwoods.Go.GoObject.Shadowed" />
		/// property set to true.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.ShadowOffset" />
		[Category("Shadows")]
		[Description("The color used for drawing drop shadows.")]
		public virtual Color ShadowColor
		{
			get
			{
				return myShadowColor;
			}
			set
			{
				if (myShadowColor != value)
				{
					myShadowColor = value;
					RaisePropertyChangedEvent("ShadowColor");
				}
			}
		}

		/// <summary>
		/// Gets or sets the scale at which greeked objects paint nothing.
		/// </summary>
		/// <value>
		/// This value defaults to <c>0.15f</c>, and should be less than or equal to
		/// <see cref="P:Northwoods.Go.GoView.PaintGreekScale" />.
		/// </value>
		/// <remarks>
		/// Not all objects use greeking to simplify and speed up painting at small scales,
		/// but those that do use this property to govern when to make that decision.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.PaintGreekScale" />
		[Category("Appearance")]
		[DefaultValue(0.13f)]
		[Description("The scale at which greeked objects paint nothing.")]
		public virtual float PaintNothingScale
		{
			get
			{
				return myPaintNothingScale;
			}
			set
			{
				if (myPaintNothingScale != value)
				{
					myPaintNothingScale = value;
					RaisePropertyChangedEvent("PaintNothingScale");
				}
			}
		}

		/// <summary>
		/// Gets or sets the scale at which greeked objects paint something simple.
		/// </summary>
		/// <value>
		/// This value defaults to <c>0.24f</c> and should be greater than or equal to
		/// <see cref="P:Northwoods.Go.GoView.PaintNothingScale" />.
		/// </value>
		/// <remarks>
		/// Not all objects use greeking to simplify and speed up painting at small scales,
		/// but those that do use this property to govern when to make that decision.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.PaintNothingScale" />
		[Category("Appearance")]
		[DefaultValue(0.24f)]
		[Description("The scale at which greeked objects paint something simple.")]
		public virtual float PaintGreekScale
		{
			get
			{
				return myPaintGreekScale;
			}
			set
			{
				if (myPaintGreekScale != value)
				{
					myPaintGreekScale = value;
					RaisePropertyChangedEvent("PaintGreekScale");
				}
			}
		}

		/// <summary>
		/// The event that is raised after the user has finished moving the selection.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoView.Selection" /> is the collection of objects that were moved,
		/// although the effective selection, as determined by
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.<see cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" />
		/// may be a rather larger set of objects.
		/// </remarks>
		[Category("GoView")]
		[Description("The user has finished moving the selection.")]
		public event EventHandler SelectionMoved;

		/// <summary>
		/// The event that is raised after the user has copied the selection.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoView.Selection" /> is the collection of objects that were copied,
		/// although the effective selection, as determined by
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.<see cref="M:Northwoods.Go.GoToolDragging.ComputeEffectiveSelection(Northwoods.Go.IGoCollection,System.Boolean)" />
		/// may be a rather larger set of objects if
		/// <see cref="P:Northwoods.Go.GoToolDragging.CopiesEffectiveSelection" /> is true.
		/// </remarks>
		[Category("GoView")]
		[Description("The user has copied the selection.")]
		public event EventHandler SelectionCopied;

		/// <summary>
		/// The cancellable event that is raised just before the user has deleted the selected objects.
		/// </summary>
		/// <remarks>
		/// You can look at the <see cref="P:Northwoods.Go.GoView.Selection" /> collection to examine
		/// the objects the user is about to remove from the document.
		/// </remarks>
		[Category("GoView")]
		[Description("Raised before the user has deleted the selected objects (a CancelEventHandler)")]
		public event CancelEventHandler SelectionDeleting;

		/// <summary>
		/// The event that is raised after the user has deleted the selected objects.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoView.Selection" /> collection has already been cleared
		/// at the time of this event.  If you need access to the collection of
		/// objects to be deleted before the deletion occurs, consider implementing
		/// a <see cref="E:Northwoods.Go.GoView.SelectionDeleting" /> event handler.
		/// </remarks>
		[Category("GoView")]
		[Description("The user has deleted the selected objects.")]
		public event EventHandler SelectionDeleted;

		/// <summary>
		/// The event that is raised when an object gets added to this view's selection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If there is a lot of work to be done in updating various Controls to reflect
		/// what is currently selected, you may be able to reduce the frequency of
		/// updates for some user actions by implementing <see cref="E:Northwoods.Go.GoView.SelectionStarting" />
		/// and <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> event handlers.
		/// </para>
		/// <para>
		/// All event handlers for this event should not modify the selection.
		/// </para>
		/// </remarks>
		[Category("GoView")]
		[Description("An object got added to this view's selection (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler ObjectGotSelection;

		/// <summary>
		/// The event that is raised when an object is removed from this view's selection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If there is a lot of work to be done in updating various Controls to reflect
		/// what is currently selected, you may be able to reduce the frequency of
		/// updates for some user actions by implementing <see cref="E:Northwoods.Go.GoView.SelectionStarting" />
		/// and <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> event handlers.
		/// </para>
		/// <para>
		/// All event handlers for this event should not modify the selection.
		/// </para>
		/// </remarks>
		[Category("GoView")]
		[Description("An object was removed from this view's selection (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler ObjectLostSelection;

		/// <summary>
		/// The event that is raised before a number of objects are selected or de-selected.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is paired with the <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> event in order to permit
		/// optimizations to skip updating other controls that depend on what is currently selected.
		/// You still need to implement <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> and <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" />
		/// event handlers in order to detect the selection or de-selection of individual objects.
		/// Methods such as <see cref="M:Northwoods.Go.GoView.SelectAll" />, <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />,
		/// <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />, and <see cref="M:Northwoods.Go.GoView.EditPaste" />
		/// will raise this event before (and the
		/// <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> event after) making changes to the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </para>
		/// <para>
		/// These two events are not raised when individual objects are added or removed
		/// from the <see cref="P:Northwoods.Go.GoView.Selection" />, such as when the user clicks on an object.
		/// These two events have been added to allow certain optimizations to be implemented
		/// when many <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> and <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" />
		/// events might occur "at once".
		/// </para>
		/// <para>
		/// All event handlers for this event should not modify the selection.
		/// </para>
		/// </remarks>
		[Category("GoView")]
		[Description("Raised before a number of objects are selected or de-selected.")]
		public event EventHandler SelectionStarting;

		/// <summary>
		/// The event that is raised after a number of objects are selected or de-selected.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is paired with the <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> event in order to permit
		/// optimizations to skip updating other controls that depend on what is currently selected.
		/// You still need to implement <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> and <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" />
		/// event handlers in order to detect the selection or de-selection of individual objects.
		/// Methods such as <see cref="M:Northwoods.Go.GoView.SelectAll" />, <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />,
		/// <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />, and <see cref="M:Northwoods.Go.GoView.EditPaste" />
		/// will raise this event after (and the
		/// <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> event before) making changes to the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </para>
		/// <para>
		/// These two events are not raised when individual objects are added or removed
		/// from the <see cref="P:Northwoods.Go.GoView.Selection" />, such as when the user clicks on an object.
		/// </para>
		/// <para>
		/// All event handlers for this event should not modify the selection.
		/// </para>
		/// </remarks>
		[Category("GoView")]
		[Description("Raised after a number of objects are selected or de-selected.")]
		public event EventHandler SelectionFinished;

		/// <summary>
		/// The event that is raised after the user finishes resizing an object.
		/// </summary>
		/// <remarks>
		/// The resized object is available as the <see cref="T:Northwoods.Go.GoSelectionEventArgs" />.<see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> property.
		/// </remarks>
		[Category("GoView")]
		[Description("The user finished resizing an object (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler ObjectResized;

		/// <summary>
		/// Rather than having separate events whenever any view property changed,
		/// all such notifications occur through this single event.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoView.OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs)" />
		/// <seealso cref="M:Northwoods.Go.GoView.RaisePropertyChangedEvent(System.String)" />
		[Category("GoView")]
		[Description("A property value has been changed (a PropertyChangedEventHandler)")]
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// All <see cref="T:Northwoods.Go.GoDocument" />.<see cref="E:Northwoods.Go.GoDocument.Changed" /> events get re-raised through this event,
		/// for the convenience of writing event handlers on the document's view.
		/// </summary>
		/// <remarks>
		/// Because the <see cref="T:Northwoods.Go.GoChangedEventArgs" /> that are passed on through
		/// this event are actually raised by <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and distributed by <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" />, there is no
		/// <c>GoView.RaiseDocumentChanged</c> method as would be conventional.
		/// As with all events, it is more efficient to override
		/// <see cref="M:Northwoods.Go.GoView.OnDocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> on <see cref="T:Northwoods.Go.GoView" />
		/// or <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" /> on <see cref="T:Northwoods.Go.GoDocument" />
		/// than it is to register delegates as event handlers on this event.
		/// </remarks>
		[Category("GoView")]
		[Description("A GoDocument.Changed event occurred (a GoChangedEventHandler)")]
		public event GoChangedEventHandler DocumentChanged;

		/// <summary>
		/// The event that is raised after the user draws a new link.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoObject" /> that is provided by the <see cref="T:Northwoods.Go.GoSelectionEventArgs" />.<see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> property
		/// will be an <see cref="T:Northwoods.Go.IGoLink" />, which is typically either a <see cref="T:Northwoods.Go.GoLink" />
		/// or a <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// </remarks>
		[Category("GoView")]
		[Description("The user drew a new link (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler LinkCreated;

		/// <summary>
		/// The event that is raised after the user reconnects an existing link.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoObject" /> that is provided by the <see cref="T:Northwoods.Go.GoSelectionEventArgs" />.<see cref="P:Northwoods.Go.GoSelectionEventArgs.GoObject" /> property
		/// will be an <see cref="T:Northwoods.Go.IGoLink" />, which is typically either a <see cref="T:Northwoods.Go.GoLink" />
		/// or a <see cref="T:Northwoods.Go.GoLabeledLink" />.
		/// </remarks>
		[Category("GoView")]
		[Description("The user reconnected an existing link (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler LinkRelinked;

		/// <summary>
		/// A document object was single clicked by the user.
		/// </summary>
		/// <remarks>
		/// This event is raised for the first document object found at the
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.  Unlike <see cref="M:Northwoods.Go.GoObject.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />,
		/// which proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until a call
		/// returns true, this event occurs only once.  Thus you will often need
		/// to look at the <see cref="P:Northwoods.Go.GoObject.ParentNode" /> or <see cref="P:Northwoods.Go.GoObject.TopLevelObject" />
		/// in order to find the most meaningful object to really handle the click.
		/// </remarks>
		/// <example>
		/// <code>
		///     MyView.ObjectSingleClicked += new GoObjectEventHandler(MyView_ObjectSingleClicked);
		///   ...
		///   private void MyView_ObjectSingleClicked(Object sender, GoObjectEventArgs evt) {
		///     GoBasicNode bn = evt.GoObject.ParentNode as GoBasicNode;
		///     if (bn != null) {
		///       MessageBox.Show("Clicked on " + bn.Text);
		///     }
		///   }
		/// </code>
		/// </example>
		[Category("GoView")]
		[Description("A document object was single clicked by the user (a GoObjectEventHandler)")]
		public event GoObjectEventHandler ObjectSingleClicked;

		/// <summary>
		/// A document object was double clicked by the user.
		/// </summary>
		/// <remarks>
		/// This event is raised for the first document object found at the
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.  Unlike <see cref="M:Northwoods.Go.GoObject.OnDoubleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />,
		/// which proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until a call
		/// returns true, this event occurs only once.  Thus you will often need
		/// to look at the <see cref="P:Northwoods.Go.GoObject.ParentNode" /> or <see cref="P:Northwoods.Go.GoObject.TopLevelObject" />
		/// in order to find the most meaningful object to really handle the click.
		/// </remarks>
		/// <example>    
		/// <code>
		///     MyView.ObjectDoubleClicked += new GoObjectEventHandler(MyView_ObjectDoubleClicked);
		///   ...
		///   private void MyView_ObjectDoubleClicked(Object sender, GoObjectEventArgs evt) {
		///     GoBasicNode bn = evt.GoObject.ParentNode as GoBasicNode;
		///     if (bn != null) {
		///       MessageBox.Show("Double-clicked on " + bn.Text);
		///     }
		///   }
		/// </code>
		/// </example>
		[Category("GoView")]
		[Description("A document object was double clicked by the user (a GoObjectEventHandler)")]
		public event GoObjectEventHandler ObjectDoubleClicked;

		/// <summary>
		/// A document object was context clicked by the user.
		/// </summary>
		/// <remarks>
		/// This event is raised for the first document object found at the
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.  Unlike <see cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />,
		/// which proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until a call
		/// returns true, this event occurs only once.  Thus you will often need
		/// to look at the <see cref="P:Northwoods.Go.GoObject.ParentNode" /> or <see cref="P:Northwoods.Go.GoObject.TopLevelObject" />
		/// in order to find the most meaningful object to really handle the click.
		/// </remarks>
		/// <example>
		/// <code>
		///     MyView.ObjectContextClicked += new GoObjectEventHandler(MyView_ObjectContextClicked);
		///   ...
		///   private void MyView_ObjectContextClicked(Object sender, GoObjectEventArgs evt) {
		///     GoBasicNode bn = evt.GoObject.ParentNode as GoBasicNode;
		///     if (bn != null) {
		///       MessageBox.Show("Context-clicked on " + bn.Text);
		///     }
		///   }
		/// </code>
		/// </example>
		[Category("GoView")]
		[Description("A document object was context clicked by the user (a GoObjectEventHandler)")]
		public event GoObjectEventHandler ObjectContextClicked;

		/// <summary>
		/// The user performed a single click on the background, not over any document object.
		/// </summary>
		/// <remarks>
		/// The location of the click is given by the <see cref="T:Northwoods.Go.GoInputEventArgs" />.<see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" /> property.
		/// </remarks>
		[Category("GoView")]
		[Description("The user performed a single click on the background, not over any document object (a GoInputEventHandler)")]
		public event GoInputEventHandler BackgroundSingleClicked;

		/// <summary>
		/// The user performed a double click on the background, not over any document object.
		/// </summary>
		/// <remarks>
		/// The location of the click is given by the <see cref="T:Northwoods.Go.GoInputEventArgs" />.<see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" /> property.
		/// </remarks>
		[Category("GoView")]
		[Description("The user performed a double click on the background, not over any document object (a GoInputEventHandler)")]
		public event GoInputEventHandler BackgroundDoubleClicked;

		/// <summary>
		/// The user performed a context click in the background, not over any document object.
		/// </summary>
		/// <remarks>
		/// The location of the click is given by the <see cref="T:Northwoods.Go.GoInputEventArgs" />.<see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" /> property.
		/// </remarks>
		[Category("GoView")]
		[Description("The user performed a context click on the background, not over any document object (a GoInputEventHandler)")]
		public event GoInputEventHandler BackgroundContextClicked;

		/// <summary>
		/// The mouse entered and/or left a document object,
		/// when no particular tool is running (a mouse-over) or when dragging.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event is raised when the <see cref="T:Northwoods.Go.GoToolManager" /> or the
		/// <see cref="T:Northwoods.Go.GoToolDragging" /> tool notices that a mouse move
		/// causes a change in the current document object that the mouse is over.
		/// The <see cref="T:Northwoods.Go.GoObjectEnterLeaveEventArgs" /> indicates the object that the
		/// mouse had been over, and the object that it is now over.  Either object may be
		/// null, indicating that it had been or now is over the background of this view.
		/// </para>
		/// <para>
		/// Because drag-and-drop is a modal operation, an external drag-and-drop from a
		/// different Control will not normally raise any <c>ObjectEnterLeave</c> events.
		/// However, you may be able to get such events if you set
		/// <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> to true and
		/// <see cref="P:Northwoods.Go.GoView.DragsRealtime" /> to true, since then a drag enter may cause
		/// some <see cref="T:Northwoods.Go.GoObject" />s to be added to the document by
		/// <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> and <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" /> and
		/// then to be dragged around by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool.
		/// </para>
		/// </remarks>
		[Category("GoView")]
		[Description("The mouse entered and/or left a document object during a mouse-over or a drag (a GoObjectEnterLeaveEventHandler)")]
		public event GoObjectEnterLeaveEventHandler ObjectEnterLeave;

		/// <summary>
		/// The event that is raised during a drag's mouse over a document object;
		/// setting the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> property to
		/// <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" /> will
		/// reject a drop of the selection at this input event point.
		/// </summary>
		/// <remarks>
		/// This event is raised for the first document object found at the
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />, excluding objects that are part
		/// of the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// Unlike the <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnSelectionDropReject(Northwoods.Go.GoObjectEventArgs,Northwoods.Go.GoView)" /> method,
		/// which proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until a call
		/// returns true, this event occurs only once per mouse move.
		/// Thus you will often need to look at the <see cref="P:Northwoods.Go.GoObject.ParentNode" />
		/// or <see cref="P:Northwoods.Go.GoObject.TopLevelObject" /> in order to find the most
		/// meaningful object to really handle the event.
		/// </remarks>
		[Category("GoView")]
		[Description("The view's Selection has been dragged over a document object (a GoObjectEventHandler); to reject a drop, set .InputState to Cancel")]
		public event GoObjectEventHandler ObjectSelectionDropReject;

		/// <summary>
		/// The event that is raised during a drag's mouse move over the background;
		/// setting the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> property to
		/// <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" /> will
		/// reject a drop of the selection at this input event point.
		/// </summary>
		[Category("GoView")]
		[Description("The view's Selection has been dragged over the background (a GoInputEventHandler); to reject a drop, set .InputState to Cancel")]
		public event GoInputEventHandler BackgroundSelectionDropReject;

		/// <summary>
		/// The user dropped the view's Selection onto a document object.
		/// </summary>
		/// <remarks>
		/// This event is raised for the first document object found at the
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />, excluding objects that are part
		/// of the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// Unlike the <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnSelectionDropped(Northwoods.Go.GoObjectEventArgs,Northwoods.Go.GoView)" /> method,
		/// which proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain until a call
		/// returns true, this event occurs only once.  Thus you will often need
		/// to look at the <see cref="P:Northwoods.Go.GoObject.ParentNode" /> or <see cref="P:Northwoods.Go.GoObject.TopLevelObject" />
		/// in order to find the most meaningful object to really handle the drop.
		/// </remarks>
		[Category("GoView")]
		[Description("The user dropped the view's Selection onto a document object (a GoObjectEventHandler)")]
		public event GoObjectEventHandler ObjectSelectionDropped;

		/// <summary>
		/// The user dropped the view's selection onto the background of the view, not onto any document object.
		/// </summary>
		[Category("GoView")]
		[Description("The user dropped the view's selection onto the background of the view (a GoInputEventHandler)")]
		public event GoInputEventHandler BackgroundSelectionDropped;

		/// <summary>
		/// A document object was hovered over by the user.
		/// </summary>
		[Category("GoView")]
		[Description("The user hovered over a document object (a GoObjectEventHandler)")]
		public event GoObjectEventHandler ObjectHover;

		/// <summary>
		/// The user hovered over the background, not over any document object.
		/// </summary>
		[Category("GoView")]
		[Description("The user hovered over the background (a GoInputEventHandler)")]
		public event GoInputEventHandler BackgroundHover;

		/// <summary>
		/// The event that is raised after the user has copied something into the clipboard
		/// from this document.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoView.Selection" /> has been copied (serialized) into the clipboard.
		/// </remarks>
		[Category("GoView")]
		[Description("The user copied something into the clipboard from this view's document")]
		public event EventHandler ClipboardCopied;

		/// <summary>
		/// The event that is raised after the user has pasted a copy of the clipboard
		/// into this document.
		/// </summary>
		/// <remarks>
		/// The newly copied objects that have been added by deserializing from the clipboard
		/// to this document are held in the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		[Category("GoView")]
		[Description("The user has pasted a copy of the clipboard into this view's document")]
		public event EventHandler ClipboardPasted;

		/// <summary>
		/// The event that is raised after the user has dropped a copy of some objects
		/// into this document.
		/// </summary>
		/// <remarks>
		/// The newly copied objects that have been added to this document are
		/// held in the <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		[Category("GoView")]
		[Description("The user has dropped some objects into this document (a GoInputEventHandler)")]
		public event GoInputEventHandler ExternalObjectsDropped;

		/// <summary>
		/// The event that is raised after the user finishes editing an object.
		/// </summary>
		[Category("GoView")]
		[Description("The user has finished editing an object (a GoSelectionEventHandler)")]
		public event GoSelectionEventHandler ObjectEdited;

		/// <summary>
		/// Create a GoView displaying an empty GoDocument.
		/// </summary>
		public GoView()
		{
			init(null);
		}

		/// <summary>
		/// Create a GoView displaying the contents of a given GoDocument.
		/// </summary>
		/// <param name="doc">The <see cref="T:Northwoods.Go.GoDocument" /> to display.</param>
		public GoView(GoDocument doc)
		{
			init(doc);
		}

		private void init(GoDocument doc)
		{
			_ = Version;
			if (myCurrentResult == null)
			{
				try
				{
					myCurrentResult = new DiagramLicense(this);
				}
				catch (Exception)
				{
					myCurrentResult = null;
				}
			}
			myDocChangedEventHandler = SafeOnDocumentChanged;
			myDocument = doc;
			myLayers = new GoLayerCollection();
			myLayers.init(this);
			myBackgroundLayer = myLayers.CreateNewLayerBefore(null);
			myBackgroundLayer.Identifier = -1;
			if (myDocument == null)
			{
				myDocument = CreateDocument();
			}
			myDocument.Changed += myDocChangedEventHandler;
			InitializeLayersFromDocument();
			myBackgroundGrid = CreateGrid();
			BackgroundLayer.Add(myBackgroundGrid);
			mySelection = CreateSelection();
			myDefaultTool = CreateDefaultTool();
			myTool = DefaultTool;
			myTool.Start();
			SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, value: true);
			VScrollBar vScrollBar = new VScrollBar();
			HScrollBar hScrollBar = new HScrollBar();
			vScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
			hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
			myTopBar = null;
			myRightBar = vScrollBar;
			myBottomBar = hScrollBar;
			myLeftBar = null;
			myTopLeftCorner = new Control();
			myTopLeftCorner.BackColor = SystemColors.Control;
			myTopRightCorner = new Control();
			myTopRightCorner.BackColor = SystemColors.Control;
			myBottomRightCorner = new Control();
			myBottomRightCorner.BackColor = SystemColors.Control;
			myBottomLeftCorner = new Control();
			myBottomLeftCorner.BackColor = SystemColors.Control;
			base.Controls.Add(vScrollBar);
			base.Controls.Add(hScrollBar);
			base.Controls.Add(myBottomRightCorner);
			base.Controls.Add(myBottomLeftCorner);
			base.Controls.Add(myTopRightCorner);
			base.Controls.Add(myTopLeftCorner);
			vScrollBar.SmallChange = ScrollSmallChange.Height;
			hScrollBar.SmallChange = ScrollSmallChange.Width;
			myToolTip = new ToolTip();
			myVertScrollHandler = HandleScroll;
			vScrollBar.Scroll += myVertScrollHandler;
			myHorizScrollHandler = HandleScroll;
			hScrollBar.Scroll += myHorizScrollHandler;
			vScrollBar.RightToLeft = RightToLeft.No;
			hScrollBar.RightToLeft = RightToLeft.No;
			InitAllowDrop(dnd: true);
			BackColor = Color.White;
		}

		/// <summary>
		/// Free up any Windows resources that this view caches.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (myAutoScrollTimer != null)
				{
					myAutoScrollTimer.Dispose();
					myAutoScrollTimer = null;
				}
				if (myHoverTimer != null)
				{
					myHoverTimer.Dispose();
					myHoverTimer = null;
				}
				if (myCurrentResult != null)
				{
					myCurrentResult.Dispose();
					myCurrentResult = null;
				}
				if (myModalControl != null)
				{
					myModalControl.Dispose();
					myModalControl = null;
				}
				if (Document != null)
				{
					foreach (GoLayer layer in Document.Layers)
					{
						layer.ResetCaches();
					}
				}
			}
			base.Dispose(disposing);
			myDocument.Changed -= myDocChangedEventHandler;
			if (myBuffer != null)
			{
				myBuffer.Dispose();
				myBuffer = null;
			}
			if (myBackgroundBrush != null)
			{
				myBackgroundBrush.Dispose();
				myBackgroundBrush = null;
			}
			if (myShadowBrush != null)
			{
				myShadowBrush.Dispose();
				myShadowBrush = null;
			}
			if (myShadowPen != null)
			{
				myShadowPen.Dispose();
				myShadowPen = null;
			}
		}

		/// <summary>
		/// This lays out the scroll bars, too, if needed.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// This calls <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.UpdateExtent" />, and <see cref="M:Northwoods.Go.GoView.UpdateView" />.
		/// </para>
		/// </remarks>
		protected override void OnSizeChanged(EventArgs evt)
		{
			LayoutScrollBars(update: false);
			base.OnSizeChanged(evt);
			UpdateExtent();
			UpdateView();
		}

		/// <summary>
		/// This lays out the scroll bars, too, if needed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This calls both <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" /> and <see cref="M:Northwoods.Go.GoView.UpdateExtent" />.
		/// </para>
		/// </remarks>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			myUpdatingScrollBars = false;
			LayoutScrollBars(update: true);
			UpdateExtent();
		}

		/// <summary>
		/// In case the size changed while it was not visible, make sure we update the scroll bars.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This calls <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.UpdateExtent" />, and <see cref="M:Northwoods.Go.GoView.UpdateView" />.
		/// </para>
		/// </remarks>
		protected override void OnVisibleChanged(EventArgs evt)
		{
			base.OnVisibleChanged(evt);
			if (base.Visible)
			{
				LayoutScrollBars(update: false);
				UpdateExtent();
				UpdateView();
			}
		}

		/// <summary>
		/// Position and size the scrollbars and corner.
		/// </summary>
		/// <param name="update">
		/// Whether to call <see cref="M:Northwoods.Go.GoView.UpdateScrollBars" /> afterwards.
		/// </param>
		/// <remarks>
		/// By default this places the vertical scroll bar at the right edge of the view,
		/// the horizontal scroll bar at the bottom edge, and the corner in the bottom right.
		/// All of these controls are inside the view's border.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.VerticalScrollBar" />
		/// <seealso cref="P:Northwoods.Go.GoView.HorizontalScrollBar" />
		/// <seealso cref="P:Northwoods.Go.GoView.BottomRightCorner" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisplayRectangle" />
		public virtual void LayoutScrollBars(bool update)
		{
			if (myUpdatingScrollBars)
			{
				return;
			}
			Control control = null;
			Control control2 = null;
			Control control3 = null;
			Control control4 = null;
			Control control5 = null;
			Control control6 = null;
			Control control7 = null;
			Control control8 = null;
			if (TopBar != null && TopBar.Visible)
			{
				control = TopBar;
			}
			if (RightBar != null && RightBar.Visible)
			{
				control2 = RightBar;
			}
			if (BottomBar != null && BottomBar.Visible)
			{
				control3 = BottomBar;
			}
			if (LeftBar != null && LeftBar.Visible)
			{
				control4 = LeftBar;
			}
			if (TopLeftCorner != null)
			{
				control5 = TopLeftCorner;
			}
			if (TopRightCorner != null)
			{
				control6 = TopRightCorner;
			}
			if (BottomRightCorner != null)
			{
				control7 = BottomRightCorner;
			}
			if (BottomLeftCorner != null)
			{
				control8 = BottomLeftCorner;
			}
			Rectangle clientRectangle = base.ClientRectangle;
			checked
			{
				Point point = new Point(clientRectangle.X + myBorderSize.Width, clientRectangle.Y + myBorderSize.Height);
				Point point2 = new Point(clientRectangle.X + clientRectangle.Width - myBorderSize.Width, clientRectangle.Y + myBorderSize.Height);
				Point point3 = new Point(clientRectangle.X + clientRectangle.Width - myBorderSize.Width, clientRectangle.Y + clientRectangle.Height - myBorderSize.Height);
				Point point4 = new Point(clientRectangle.X + myBorderSize.Width, clientRectangle.Y + clientRectangle.Height - myBorderSize.Height);
				if (control != null)
				{
					point.Y += control.Height;
					point2.Y += control.Height;
				}
				if (control2 != null)
				{
					point2.X -= control2.Width;
					point3.X -= control2.Width;
				}
				if (control3 != null)
				{
					point3.Y -= control3.Height;
					point4.Y -= control3.Height;
				}
				if (control4 != null)
				{
					point4.X += control4.Width;
					point.X += control4.Width;
				}
				myDisplayRectangle = new Rectangle(point.X, point.Y, point3.X - point.X, point3.Y - point.Y);
				if (control5 != null)
				{
					if (control != null && control4 != null)
					{
						control5.Bounds = new Rectangle(point.X - control4.Width, point.Y - control.Height, control4.Width, control.Height);
						control5.Visible = true;
					}
					else
					{
						control5.Visible = false;
					}
				}
				if (control6 != null)
				{
					if (control != null && control2 != null)
					{
						control6.Bounds = new Rectangle(point2.X, point2.Y - control.Height, control2.Width, control.Height);
						control6.Visible = true;
					}
					else
					{
						control6.Visible = false;
					}
				}
				if (control7 != null)
				{
					if (control3 != null && control2 != null)
					{
						control7.Bounds = new Rectangle(point3.X, point3.Y, control2.Width, control3.Height);
						control7.Visible = true;
					}
					else
					{
						control7.Visible = false;
					}
				}
				if (control8 != null)
				{
					if (control3 != null && control4 != null)
					{
						control8.Bounds = new Rectangle(point4.X - control4.Width, point4.Y, control4.Width, control3.Height);
						control8.Visible = true;
					}
					else
					{
						control8.Visible = false;
					}
				}
				if (control != null)
				{
					Rectangle bounds = new Rectangle(point.X, point.Y - control.Height, point2.X - point.X, control.Height);
					if (control5 == null && control4 != null)
					{
						bounds.X -= control4.Width;
						bounds.Width += control4.Width;
					}
					if (control6 == null && control2 != null)
					{
						bounds.Width += control2.Width;
					}
					control.Bounds = bounds;
				}
				if (control2 != null)
				{
					Rectangle rectangle2 = control2.Bounds = new Rectangle(point2.X, point2.Y, control2.Width, point3.Y - point2.Y);
				}
				if (control3 != null)
				{
					Rectangle bounds2 = new Rectangle(point4.X, point4.Y, point3.X - point4.X, control3.Height);
					if (control8 == null && control4 != null)
					{
						bounds2.X -= control4.Width;
						bounds2.Width += control4.Width;
					}
					if (control7 == null && control2 != null)
					{
						bounds2.Width += control2.Width;
					}
					control3.Bounds = bounds2;
				}
				if (control4 != null)
				{
					Rectangle rectangle4 = control4.Bounds = new Rectangle(point.X - control4.Width, point.Y, control4.Width, point4.Y - point.Y);
				}
				VScrollBar verticalScrollBar = VerticalScrollBar;
				if (verticalScrollBar != null && verticalScrollBar.Visible)
				{
					verticalScrollBar.LargeChange = Math.Max(ScrollSmallChange.Height, verticalScrollBar.Height - ScrollSmallChange.Height);
				}
				HScrollBar horizontalScrollBar = HorizontalScrollBar;
				if (horizontalScrollBar != null && horizontalScrollBar.Visible)
				{
					horizontalScrollBar.LargeChange = Math.Max(ScrollSmallChange.Width, horizontalScrollBar.Width - ScrollSmallChange.Width);
				}
				if (update)
				{
					UpdateScrollBars();
				}
			}
		}

		/// <summary>
		/// This method is called when this GoView's Size (or Width or Height) is changed,
		/// to allow the view to scroll or rescale itself.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The behavior depends on the value of <see cref="P:Northwoods.Go.GoView.SheetStyle" />
		/// and perhaps on the size of the <see cref="P:Northwoods.Go.GoView.Sheet" />.
		/// When the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is <see cref="T:Northwoods.Go.GoViewSheetStyle" />.<see cref="F:Northwoods.Go.GoViewSheetStyle.WholeSheet" />,
		/// the <see cref="P:Northwoods.Go.GoView.DocScale" /> and <see cref="P:Northwoods.Go.GoView.DocPosition" /> are
		/// set to have all of the <see cref="P:Northwoods.Go.GoView.Sheet" /> be visible in the view.
		/// A <see cref="P:Northwoods.Go.GoView.SheetStyle" /> of <see cref="F:Northwoods.Go.GoViewSheetStyle.SheetWidth" />
		/// causes the scale to be set so that the full width of the <see cref="P:Northwoods.Go.GoView.Sheet" /> is visible.
		/// A <see cref="P:Northwoods.Go.GoView.SheetStyle" /> of <see cref="F:Northwoods.Go.GoViewSheetStyle.SheetHeight" />
		/// causes the scale to be set so that the full height of the <see cref="P:Northwoods.Go.GoView.Sheet" /> is visible.
		/// A <see cref="P:Northwoods.Go.GoView.SheetStyle" /> of <see cref="F:Northwoods.Go.GoViewSheetStyle.Sheet" />
		/// does not cause the scale to be changed.
		/// Finally, when <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// or when there is no <see cref="P:Northwoods.Go.GoView.Sheet" />, this method does nothing.
		/// The <see cref="P:Northwoods.Go.GoView.Sheet" />'s <see cref="P:Northwoods.Go.GoObject.Bounds" /> are needed to decide how
		/// to scroll and scale the view, but the <see cref="P:Northwoods.Go.GoView.Sheet" /> need not be
		/// <see cref="P:Northwoods.Go.GoObject.Visible" />.
		/// </para>
		/// <para>
		/// For all of the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> values other than <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// this method calls <see cref="M:Northwoods.Go.GoView.RescaleWithCenter(System.Single,System.Drawing.PointF)" /> to try to keep centered
		/// the document point that had been in the center of the view.
		/// </para>
		/// </remarks>
		public virtual void UpdateExtent()
		{
			Control control = null;
			try
			{
				control = base.Parent;
				while (control != null && !(control is Form))
				{
					control = control.Parent;
				}
			}
			catch (VerificationException)
			{
			}
			catch (SecurityException)
			{
			}
			if (control != null && control is Form)
			{
				Form form = (Form)control;
				if (form.WindowState == FormWindowState.Minimized)
				{
					myWindowState = FormWindowState.Minimized;
					return;
				}
				if (myWindowState == FormWindowState.Minimized)
				{
					myWindowState = form.WindowState;
					return;
				}
			}
			if (SheetStyle == GoViewSheetStyle.None || Sheet == null)
			{
				return;
			}
			RectangleF a = Sheet.Bounds;
			SizeF sizeF = ConvertViewToDoc(SheetRoom);
			GoObject.InflateRect(ref a, sizeF.Width + Math.Abs(ShadowOffset.Width), sizeF.Height + Math.Abs(ShadowOffset.Height));
			Size size = DisplayRectangle.Size;
			float newscale = DocScale;
			switch (SheetStyle)
			{
			case GoViewSheetStyle.WholeSheet:
				if (a.Width > 0f && a.Height > 0f)
				{
					newscale = Math.Min((float)size.Width / a.Width, (float)size.Height / a.Height);
				}
				break;
			case GoViewSheetStyle.SheetWidth:
				if (a.Width > 0f)
				{
					newscale = (float)size.Width / a.Width;
				}
				break;
			case GoViewSheetStyle.SheetHeight:
				if (a.Height > 0f)
				{
					newscale = (float)size.Height / a.Height;
				}
				break;
			}
			RescaleWithCenter(newscale, myPreviousCenter);
		}

		/// <summary>
		/// This method is called by the constructor to create a document for this view.
		/// </summary>
		/// <returns>A <see cref="T:Northwoods.Go.GoDocument" /></returns>
		/// <remarks>
		/// By default this just creates a new instance of <see cref="T:Northwoods.Go.GoDocument" />.
		/// Often you will want to override this method so that creating a
		/// particular kind of view will automatically create the right kind
		/// of document.
		/// This method is not called when the constructor is passed a non-null
		/// <see cref="T:Northwoods.Go.GoDocument" />.
		/// </remarks>
		public virtual GoDocument CreateDocument()
		{
			return new GoDocument();
		}

		/// <summary>
		/// This method is responsible for setting up the view's collection of layers
		/// to include all of the document's layers, in order, followed by the
		/// view's default layer.
		/// </summary>
		/// <remarks>
		/// This removes all old document layers and clears out all view layers
		/// from this view's <see cref="T:Northwoods.Go.GoLayerCollection" />, adds all of the
		/// new document's layers, and leaves the default view layer in front of all layers.
		/// Any <see cref="P:Northwoods.Go.GoView.BackgroundGrid" /> or <see cref="P:Northwoods.Go.GoView.Sheet" />
		/// is kept in the <see cref="P:Northwoods.Go.GoView.BackgroundLayer" />.
		/// </remarks>
		public virtual void InitializeLayersFromDocument()
		{
			if (Layers == null)
			{
				return;
			}
			BeginUpdate();
			GoLayer @default = Layers.Default;
			GoLayer backgroundLayer = BackgroundLayer;
			GoGrid backgroundGrid = BackgroundGrid;
			GoSheet sheet = Sheet;
			GoLayer[] array = Layers.CopyArray();
			foreach (GoLayer goLayer in array)
			{
				if (goLayer.IsInView)
				{
					goLayer.Clear();
				}
				else
				{
					Layers.Remove(goLayer);
				}
			}
			DocPosition = default(PointF);
			foreach (GoLayer layer in Document.Layers)
			{
				Layers.InsertDocumentLayerAfter(null, layer);
			}
			Layers.MoveAfter(null, @default);
			Layers.MoveBefore(null, backgroundLayer);
			backgroundLayer.Add(backgroundGrid);
			backgroundLayer.Add(sheet);
			EndUpdate();
		}

		/// <summary>
		/// Given a point in this view, calculate the corresponding point in the view's document.
		/// </summary>
		/// <param name="p">
		/// A <c>Point</c> in view coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>PointF</c> in document coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current position in the document, and the current
		/// view scale, into account when computing the transformation from view coordinates
		/// to document coordinates.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertDocToView(System.Drawing.PointF)" />
		public virtual PointF ConvertViewToDoc(Point p)
		{
			PointF docPosition = DocPosition;
			return checked(new PointF((float)(p.X - myDisplayRectangle.X) / myHorizWorld / myHorizScale + docPosition.X, (float)(p.Y - myDisplayRectangle.Y) / myVertWorld / myVertScale + docPosition.Y));
		}

		/// <summary>
		/// Given a size in this view, calculate the corresponding size in the view's document.
		/// </summary>
		/// <param name="s">
		/// A <c>Size</c> in view coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>SizeF</c> in document coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current view scale into account when computing the
		/// transformation from view coordinates to document coordinates.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertDocToView(System.Drawing.SizeF)" />
		public virtual SizeF ConvertViewToDoc(Size s)
		{
			return new SizeF((float)s.Width / myHorizWorld / myHorizScale, (float)s.Height / myVertWorld / myVertScale);
		}

		/// <summary>
		/// Given a rectangle in this view, calculate the corresponding rectangle in the view's document.
		/// </summary>
		/// <param name="r">
		/// A <c>Rectangle</c> in view coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>RectangleF</c> in document coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current position in the document, and the current
		/// view scale, into account when computing the transformation from view coordinates
		/// to document coordinates.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertDocToView(System.Drawing.RectangleF)" />
		public virtual RectangleF ConvertViewToDoc(Rectangle r)
		{
			PointF docPosition = DocPosition;
			return checked(new RectangleF((float)(r.X - myDisplayRectangle.X) / myHorizWorld / myHorizScale + docPosition.X, (float)(r.Y - myDisplayRectangle.Y) / myVertWorld / myVertScale + docPosition.Y, (float)r.Width / myHorizWorld / myHorizScale, (float)r.Height / myVertWorld / myVertScale));
		}

		/// <summary>
		/// Given a point in this document, calculate the corresponding point in this view.
		/// </summary>
		/// <param name="p">
		/// A <c>PointF</c> in document coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>Point</c> in view coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current position in the document, and the current
		/// view scale, into account when computing the transformation from document coordinates
		/// to view coordinates.
		/// Note that because documents are often larger than the views,
		/// many object positions will often have corresponding view positions outside the
		/// view's client area.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Point)" />
		public virtual Point ConvertDocToView(PointF p)
		{
			PointF docPosition = DocPosition;
			return checked(new Point((int)Math.Floor((p.X - docPosition.X) * myHorizScale * myHorizWorld) + myDisplayRectangle.X, (int)Math.Floor((p.Y - docPosition.Y) * myVertScale * myVertWorld) + myDisplayRectangle.Y));
		}

		/// <summary>
		/// Given a size in this document, calculate the corresponding size in this view.
		/// </summary>
		/// <param name="s">
		/// A <c>SizeF</c> in document coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>Size</c> in view coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current view scale into account when computing the
		/// transformation from document coordinates to view coordinates.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Size)" />
		public virtual Size ConvertDocToView(SizeF s)
		{
			return checked(new Size((int)Math.Ceiling(s.Width * myHorizScale * myHorizWorld), (int)Math.Ceiling(s.Height * myVertScale * myVertWorld)));
		}

		/// <summary>
		/// Given a rectangle in this document, calculate the corresponding rectangle in this view.
		/// </summary>
		/// <param name="r">
		/// A <c>RectangleF</c> in document coordinates.
		/// </param>
		/// <returns>
		/// The corresponding <c>Rectangle</c> in view coordinates.
		/// </returns>
		/// <remarks>
		/// This method takes this view's current position in the document, and the current
		/// view scale, into account when computing the transformation from document coordinates
		/// to view coordinates.
		/// Note that because documents are often larger than the views,
		/// many object positions will often have corresponding view positions outside the
		/// view's client area.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ConvertViewToDoc(System.Drawing.Rectangle)" />
		public virtual Rectangle ConvertDocToView(RectangleF r)
		{
			PointF docPosition = DocPosition;
			return checked(new Rectangle((int)Math.Floor((r.X - docPosition.X) * myHorizScale * myHorizWorld) + myDisplayRectangle.X, (int)Math.Floor((r.Y - docPosition.Y) * myVertScale * myVertWorld) + myDisplayRectangle.Y, (int)Math.Ceiling(r.Width * myHorizScale * myHorizWorld), (int)Math.Ceiling(r.Height * myVertScale * myVertWorld)));
		}

		/// <summary>
		/// This method is called when setting the <see cref="P:Northwoods.Go.GoView.DocPosition" /> property to make
		/// sure the view only takes reasonable, desired positions.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		/// <remarks>
		/// By default this method tries to keep the view within the document.
		/// However, when the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// there are no limits--the <see cref="P:Northwoods.Go.GoView.DocumentTopLeft" /> and
		/// <see cref="P:Northwoods.Go.GoView.DocumentSize" /> properties automatically adjust themselves
		/// to accomodate viewing any part of the document or the <see cref="P:Northwoods.Go.GoView.Sheet" />
		/// at any <see cref="P:Northwoods.Go.GoView.DocScale" /> given the size of this view control.
		/// </remarks>
		public virtual PointF LimitDocPosition(PointF p)
		{
			if (SheetStyle == GoViewSheetStyle.None)
			{
				PointF documentTopLeft = DocumentTopLeft;
				SizeF documentSize = DocumentSize;
				SizeF docExtentSize = DocExtentSize;
				float num = documentTopLeft.X + documentSize.Width - docExtentSize.Width;
				if (num < documentTopLeft.X)
				{
					p.X = documentTopLeft.X;
				}
				else if (p.X > num && num > documentTopLeft.X)
				{
					p.X = num;
				}
				else if (p.X < documentTopLeft.X)
				{
					p.X = documentTopLeft.X;
				}
				float num2 = documentTopLeft.Y + documentSize.Height - docExtentSize.Height;
				if (num2 < documentTopLeft.Y)
				{
					p.Y = documentTopLeft.Y;
				}
				else if (p.Y > num2 && num2 > documentTopLeft.Y)
				{
					p.Y = num2;
				}
				else if (p.Y < documentTopLeft.Y)
				{
					p.Y = documentTopLeft.Y;
				}
			}
			return p;
		}

		/// <summary>
		/// Change this view's DocPosition so that the given rectangle is visible.
		/// </summary>
		/// <param name="contentRect">the area, in document coordinates, to try to scroll into view</param>
		/// <remarks>
		/// Usually you call this method with the bounds of an object, to make
		/// that object visible to the user and not scrolled off somewhere.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollPage(System.Single,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		public virtual void ScrollRectangleToVisible(RectangleF contentRect)
		{
			RectangleF docExtent = DocExtent;
			if (!GoObject.ContainsRect(docExtent, contentRect))
			{
				float x = (!(contentRect.Width < docExtent.Width)) ? contentRect.X : (contentRect.X + contentRect.Width / 2f - docExtent.Width / 2f);
				float y = (!(contentRect.Height < docExtent.Height)) ? contentRect.Y : (contentRect.Y + contentRect.Height / 2f - docExtent.Height / 2f);
				DocPosition = new PointF(x, y);
			}
		}

		/// <summary>
		/// Programmatically scroll the view by a "page" (a large change).
		/// </summary>
		/// <param name="dx">the number of pages to change the X coordinate of the <see cref="P:Northwoods.Go.GoView.DocPosition" />; positive increases, negative decreases</param>
		/// <param name="dy">the number of pages to change the Y coordinate of the <see cref="P:Northwoods.Go.GoView.DocPosition" />; positive increases, negative decreases</param>
		/// <remarks>
		/// This method does not depend on the existence of any scrollbars,
		/// but does depend on the values of <see cref="P:Northwoods.Go.GoView.DocExtentSize" /> and <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> to determine
		/// a new value for <see cref="P:Northwoods.Go.GoView.DocPosition" /> that is a "line" less than one full "page" away from the old position
		/// times the factor provided by the parameters for each direction.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollRectangleToVisible(System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void ScrollPage(float dx, float dy)
		{
			PointF docPosition = DocPosition;
			SizeF docExtentSize = DocExtentSize;
			PointF documentTopLeft = DocumentTopLeft;
			SizeF documentSize = DocumentSize;
			Size scrollSmallChange = ScrollSmallChange;
			Size size = ConvertDocToView(docExtentSize);
			Size s = checked(new Size(Math.Max(scrollSmallChange.Width, size.Width - scrollSmallChange.Width), Math.Max(scrollSmallChange.Height, size.Height - scrollSmallChange.Height)));
			SizeF sizeF = ConvertViewToDoc(s);
			float num = dx * sizeF.Width;
			docPosition.X += num;
			if (num >= 0f)
			{
				docPosition.X = Math.Min(docPosition.X, Math.Max(documentTopLeft.X, documentTopLeft.X + documentSize.Width - docExtentSize.Width));
			}
			else
			{
				docPosition.X = Math.Max(docPosition.X, documentTopLeft.X);
			}
			float num2 = dy * sizeF.Height;
			docPosition.Y += num2;
			if (num2 >= 0f)
			{
				docPosition.Y = Math.Min(docPosition.Y, Math.Max(documentTopLeft.Y, documentTopLeft.Y + documentSize.Height - docExtentSize.Height));
			}
			else
			{
				docPosition.Y = Math.Max(docPosition.Y, documentTopLeft.Y);
			}
			DocPosition = docPosition;
		}

		/// <summary>
		/// Programmatically scroll the view by a "line" (a small change).
		/// </summary>
		/// <param name="dx">the number of lines to change the X coordinate of the <see cref="P:Northwoods.Go.GoView.DocPosition" />; positive increases, negative decreases</param>
		/// <param name="dy">the number of lines to change the Y coordinate of the <see cref="P:Northwoods.Go.GoView.DocPosition" />; positive increases, negative decreases</param>
		/// <remarks>
		/// This method does not depend on the existence of any scrollbars,
		/// but does depend on the value of <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> to determine
		/// a new value for <see cref="P:Northwoods.Go.GoView.DocPosition" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollPage(System.Single,System.Single)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ScrollRectangleToVisible(System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void ScrollLine(float dx, float dy)
		{
			PointF docPosition = DocPosition;
			SizeF docExtentSize = DocExtentSize;
			PointF documentTopLeft = DocumentTopLeft;
			SizeF documentSize = DocumentSize;
			Size scrollSmallChange = ScrollSmallChange;
			Size s = new Size(scrollSmallChange.Width, scrollSmallChange.Height);
			SizeF sizeF = ConvertViewToDoc(s);
			float num = dx * sizeF.Width;
			docPosition.X += num;
			if (num > 0f)
			{
				docPosition.X = Math.Min(docPosition.X, Math.Max(documentTopLeft.X, documentTopLeft.X + documentSize.Width - docExtentSize.Width));
			}
			else
			{
				docPosition.X = Math.Max(docPosition.X, documentTopLeft.X);
			}
			float num2 = dy * sizeF.Height;
			docPosition.Y += num2;
			if (num2 > 0f)
			{
				docPosition.Y = Math.Min(docPosition.Y, Math.Max(documentTopLeft.Y, documentTopLeft.Y + documentSize.Height - docExtentSize.Height));
			}
			else
			{
				docPosition.Y = Math.Max(docPosition.Y, documentTopLeft.Y);
			}
			DocPosition = docPosition;
		}

		/// <summary>
		/// Determine if there is room for the view to be scrolled in a given direction.
		/// </summary>
		/// <param name="down"></param>
		/// <param name="vertical"></param>
		/// <returns></returns>
		public virtual bool CanScroll(bool down, bool vertical)
		{
			PointF docPosition = DocPosition;
			SizeF docExtentSize = DocExtentSize;
			PointF documentTopLeft = DocumentTopLeft;
			SizeF documentSize = DocumentSize;
			if (vertical)
			{
				if (down)
				{
					docPosition.Y += 1f;
					docPosition.Y = Math.Min(docPosition.Y, Math.Max(documentTopLeft.Y, documentTopLeft.Y + documentSize.Height - docExtentSize.Height));
				}
				else
				{
					docPosition.Y -= 1f;
					docPosition.Y = Math.Max(docPosition.Y, documentTopLeft.Y);
				}
			}
			else if (down)
			{
				docPosition.X += 1f;
				docPosition.X = Math.Min(docPosition.X, Math.Max(documentTopLeft.X, documentTopLeft.X + documentSize.Width - docExtentSize.Width));
			}
			else
			{
				docPosition.X -= 1f;
				docPosition.X = Math.Max(docPosition.X, documentTopLeft.X);
			}
			return docPosition != DocPosition;
		}

		/// <summary>
		/// This method is called when setting the DocScale property to make
		/// sure that the view only displays objects at a reasonable scale.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		/// <remarks>
		/// By default this limits the value to between 0.01f and 10.0f.
		/// </remarks>
		public virtual float LimitDocScale(float s)
		{
			float width = WorldScale.Width;
			float num = 0.01f / width;
			float num2 = 10f / width;
			if (s < num)
			{
				s = num;
			}
			if (s > num2)
			{
				s = num2;
			}
			return s;
		}

		/// <summary>
		/// Determine the actual extent of all of the objects in the document
		/// as seen by this view.
		/// </summary>
		/// <returns>A <c>RectangleF</c> in document coordinates</returns>
		/// <remarks>
		/// This is called by methods such as <see cref="M:Northwoods.Go.GoView.RescaleToFit" />, that
		/// want to know how much area is taken up by visible document objects.
		/// <see cref="P:Northwoods.Go.GoView.DocumentSize" /> is different in that that property is
		/// likely to be less changeable as objects are moved or deleted.
		/// </remarks>
		public virtual RectangleF ComputeDocumentBounds()
		{
			return GoDocument.ComputeBounds(Document, this);
		}

		/// <summary>
		/// Change the <see cref="P:Northwoods.Go.GoView.DocScale" /> and <see cref="P:Northwoods.Go.GoView.DocPosition" /> properties
		/// so that all document objects are visible.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This calls <see cref="M:Northwoods.Go.GoView.ComputeDocumentBounds" /> to determine the size and position of
		/// this view's document.
		/// If the document is very large, the <see cref="M:Northwoods.Go.GoView.LimitDocScale(System.Single)" />
		/// method might prevent the whole document from fitting.
		/// <see cref="M:Northwoods.Go.GoView.LimitDocPosition(System.Drawing.PointF)" /> may similarly prevent the whole document from fitting.
		/// </para>
		/// <para>
		/// Calling this method will not necessarily cause the scroll bars to
		/// disappear, because the scroll bars normally show the extent of the
		/// document, which is normally greater than the extent of the actual
		/// objects in the document.
		/// This method does not modify the size and position of the view's document.
		/// Depending on your application, you may wish to shrink the size of the document
		/// in order to cause the scroll bars to disappear.
		/// </para>
		/// <para>
		/// The value of the new <see cref="P:Northwoods.Go.GoView.DocScale" /> will not be larger than one.
		/// In other words, if the document is small and thus fits easily in the view,
		/// it will not be scaled up beyond normal to expand to fit the view.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>
		///   goView1.Document.Bounds = goView1.ComputeDocumentBounds();
		///   goView1.RescaleToFit();
		/// </code>
		/// </example>
		public virtual void RescaleToFit()
		{
			Size size = DisplayRectangle.Size;
			RectangleF rectangleF = ComputeDocumentBounds();
			SizeF shadowOffset = ShadowOffset;
			rectangleF.Width += Math.Abs(shadowOffset.Width);
			rectangleF.Height += Math.Abs(shadowOffset.Height);
			if (shadowOffset.Width < 0f)
			{
				rectangleF.X += shadowOffset.Width;
			}
			if (shadowOffset.Height < 0f)
			{
				rectangleF.Y += shadowOffset.Height;
			}
			DocPosition = new PointF(rectangleF.X, rectangleF.Y);
			float num = 1f;
			if (rectangleF.Width > 0f && rectangleF.Height > 0f)
			{
				num = Math.Min((float)size.Width / rectangleF.Width, (float)size.Height / rectangleF.Height);
			}
			if (num > 1f)
			{
				num = 1f;
			}
			DocScale = num;
		}

		/// <summary>
		/// Change the <see cref="P:Northwoods.Go.GoView.DocScale" />, and adjust the <see cref="P:Northwoods.Go.GoView.DocPosition" /> so that
		/// the <paramref name="docPt" /> remains at the center of the view.
		/// </summary>
		/// <param name="newscale">the new value for <see cref="P:Northwoods.Go.GoView.DocScale" /></param>
		/// <param name="docPt">a PointF in document coordinates</param>
		/// <remarks>
		/// <para>
		/// The given <paramref name="docPt" /> may not actually remain at the center
		/// of the view if the <see cref="M:Northwoods.Go.GoView.LimitDocPosition(System.Drawing.PointF)" /> method limits where the
		/// view can be scrolled to.
		/// </para>
		/// <para>
		/// <see cref="M:Northwoods.Go.GoView.UpdateExtent" /> calls this method, even if the scale is not changing.
		/// </para>
		/// </remarks>
		public virtual void RescaleWithCenter(float newscale, PointF docPt)
		{
			DocScale = newscale;
			RectangleF docExtent = DocExtent;
			DocPosition = new PointF(docPt.X - docExtent.Width / 2f, docPt.Y - docExtent.Height / 2f);
		}

		private bool ShouldSerializeScrollSmallChange()
		{
			return ScrollSmallChange != new Size(16, 16);
		}

		private void ResetScrollSmallChange()
		{
			ScrollSmallChange = new Size(16, 16);
		}

		private bool ShouldSerializeAutoScrollRegion()
		{
			return AutoScrollRegion != new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
		}

		private void ResetAutoScrollRegion()
		{
			AutoScrollRegion = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
		}

		/// <summary>
		/// Start or continue automatically scrolling the view during a mouse drag.
		/// </summary>
		/// <param name="viewPnt">the current mouse point, in view coordinates</param>
		/// <remarks>
		/// <para>
		/// As soon <see cref="M:Northwoods.Go.GoView.ComputeAutoScrollDocPosition(System.Drawing.Point)" /> returns a new
		/// <see cref="P:Northwoods.Go.GoView.DocPosition" /> value, this method starts a <c>Timer</c>
		/// that waits for <see cref="P:Northwoods.Go.GoView.AutoScrollDelay" /> milliseconds.
		/// After waiting, it repeatedly sets <see cref="P:Northwoods.Go.GoView.DocPosition" />
		/// to the latest <see cref="M:Northwoods.Go.GoView.ComputeAutoScrollDocPosition(System.Drawing.Point)" /> value,
		/// until the position does not change (presumably because the 
		/// <see cref="P:Northwoods.Go.GoView.LastInput" />'s view point is no longer in the autoscroll
		/// margin).
		/// Setting this view's <see cref="P:Northwoods.Go.GoView.DocPosition" /> occurs each
		/// <see cref="P:Northwoods.Go.GoView.AutoScrollTime" /> milliseconds.
		/// </para>
		/// <para>
		/// This method is normally called by those tools that want to support
		/// auto-scrolling during a mouse move, such as
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.<see cref="M:Northwoods.Go.GoToolDragging.DoMouseMove" /> or
		/// <see cref="T:Northwoods.Go.GoToolLinking" />.<see cref="M:Northwoods.Go.GoToolLinking.DoMouseMove" />.
		/// The timer is stopped when the mouse leaves this view.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoAutoPan(System.Drawing.Point,System.Drawing.Point)" />
		/// <seealso cref="M:Northwoods.Go.GoView.StopAutoScroll" />
		public virtual void DoAutoScroll(Point viewPnt)
		{
			myPanning = false;
			myAutoScrollPoint = viewPnt;
			DoInternalAutoScroll();
		}

		/// <summary>
		/// Start or continue scrolling the view according to the relative position of
		/// the <paramref name="viewPnt" /> compared to the <paramref name="originPnt" />.
		/// </summary>
		/// <param name="originPnt">the original panning point, in view coordinates</param>
		/// <param name="viewPnt">the current mouse point, in view coordinates</param>
		/// <remarks>
		/// <para>
		/// This uses the same mechanisms as <see cref="M:Northwoods.Go.GoView.DoAutoScroll(System.Drawing.Point)" /> -- do not
		/// try to auto-scroll and auto-pan at the same time.
		/// Automatic panning occurs in the area outside of the region specified by
		/// <see cref="P:Northwoods.Go.GoView.AutoPanRegion" /> surrounding the <paramref name="originPnt" />.
		/// </para>
		/// <para>
		/// This is normally called from <see cref="T:Northwoods.Go.GoToolPanning" />.<see cref="M:Northwoods.Go.GoToolPanning.DoMouseMove" />.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.StopAutoScroll" />
		/// <seealso cref="P:Northwoods.Go.GoView.AutoScrollDelay" />
		/// <seealso cref="P:Northwoods.Go.GoView.AutoScrollTime" />
		public virtual void DoAutoPan(Point originPnt, Point viewPnt)
		{
			myPanning = true;
			myPanningOrigin = originPnt;
			myAutoScrollPoint = viewPnt;
			DoInternalAutoScroll();
		}

		internal void DoAutoAction()
		{
			if (!myActioning)
			{
				myActioning = true;
				DoInternalAutoScroll();
			}
		}

		private void DoInternalAutoScroll()
		{
			if (myAutoScrollTimer == null)
			{
				myAutoScrollTimer = new System.Threading.Timer(autoScrollCallback, new EventHandler(autoScrollTick), -1, -1);
				myAutoScrollTimerEnabled = false;
			}
			if (myActioning)
			{
				if (!myAutoScrollTimerEnabled)
				{
					myAutoScrollTimerEnabled = true;
					myAutoScrollTimer.Change(AutoScrollDelay, -1);
				}
			}
			else if ((myPanning ? ComputeAutoPanDocPosition(myPanningOrigin, myAutoScrollPoint) : ComputeAutoScrollDocPosition(myAutoScrollPoint)) != DocPosition)
			{
				if (!myAutoScrollTimerEnabled)
				{
					myAutoScrollTimerEnabled = true;
					if (!Focused)
					{
						myAutoScrollTimer.Change(AutoScrollDelay, -1);
					}
					else
					{
						myAutoScrollTimer.Change(AutoScrollTime, -1);
					}
				}
			}
			else if (!myPanning)
			{
				StopAutoScroll();
			}
		}

		/// <summary>
		/// Stop any ongoing auto-scroll or auto-pan action.
		/// </summary>
		/// <remarks>
		/// This stops the Timer used to get repeating events to consider scrolling.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.DoAutoScroll(System.Drawing.Point)" />
		/// <seealso cref="M:Northwoods.Go.GoView.DoAutoPan(System.Drawing.Point,System.Drawing.Point)" />
		public void StopAutoScroll()
		{
			myActioning = false;
			if (myAutoScrollTimer != null)
			{
				myAutoScrollTimerEnabled = false;
				myAutoScrollTimer.Change(-1, -1);
			}
		}

		private void autoScrollCallback(object obj)
		{
			if (base.IsHandleCreated)
			{
				Invoke((EventHandler)obj);
			}
		}

		private void autoScrollTick(object sender, EventArgs evt)
		{
			if (!myAutoScrollTimerEnabled)
			{
				return;
			}
			if (myActioning)
			{
				GoToolAction goToolAction = Tool as GoToolAction;
				if (goToolAction != null && goToolAction.ActionObject != null)
				{
					goToolAction.AutoAdjust();
					myAutoScrollTimer.Change(AutoScrollTime, -1);
				}
				return;
			}
			PointF pointF = myPanning ? ComputeAutoPanDocPosition(myPanningOrigin, myAutoScrollPoint) : ComputeAutoScrollDocPosition(myAutoScrollPoint);
			if (pointF == DocPosition)
			{
				myAutoScrollTimer.Change(AutoScrollDelay, -1);
				return;
			}
			if (DocPosition != pointF && DrawsXorMode)
			{
				DrawXorBox(myPrevXorRect, drawnew: false);
			}
			DocPosition = pointF;
			myAutoScrollTimer.Change(AutoScrollTime, -1);
		}

		/// <summary>
		/// This method is called to determine the next position in the document for this view,
		/// given a point at which the user is dragging the mouse.
		/// </summary>
		/// <param name="viewPnt">
		/// The mouse point, in view coordinates.
		/// </param>
		/// <remarks>
		/// This uses the <see cref="P:Northwoods.Go.GoView.AutoScrollRegion" /> and <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" />
		/// properties to calculate a new <see cref="P:Northwoods.Go.GoView.DocPosition" />.
		/// The closer the point is to the edge of the view, the larger a multiple of
		/// the <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> is used as a scroll step in that direction.
		/// </remarks>
		public virtual PointF ComputeAutoScrollDocPosition(Point viewPnt)
		{
			PointF docPosition = DocPosition;
			Point p = ConvertDocToView(docPosition);
			Size autoScrollRegion = AutoScrollRegion;
			int width = ScrollSmallChange.Width;
			int height = ScrollSmallChange.Height;
			Rectangle displayRectangle = DisplayRectangle;
			checked
			{
				if (viewPnt.X >= displayRectangle.X && viewPnt.X < displayRectangle.X + autoScrollRegion.Width)
				{
					p.X -= width;
					if (viewPnt.X < displayRectangle.X + unchecked(autoScrollRegion.Width / 2))
					{
						p.X -= width;
					}
					if (viewPnt.X < displayRectangle.X + unchecked(autoScrollRegion.Width / 4))
					{
						p.X -= 4 * width;
					}
				}
				else if (viewPnt.X <= displayRectangle.X + displayRectangle.Width && viewPnt.X > displayRectangle.X + displayRectangle.Width - autoScrollRegion.Width)
				{
					p.X += width;
					if (viewPnt.X > displayRectangle.X + displayRectangle.Width - unchecked(autoScrollRegion.Width / 2))
					{
						p.X += width;
					}
					if (viewPnt.X > displayRectangle.X + displayRectangle.Width - unchecked(autoScrollRegion.Width / 4))
					{
						p.X += 4 * width;
					}
				}
				if (viewPnt.Y >= displayRectangle.Y && viewPnt.Y < displayRectangle.Y + autoScrollRegion.Height)
				{
					p.Y -= height;
					if (viewPnt.Y < displayRectangle.Y + unchecked(autoScrollRegion.Height / 2))
					{
						p.Y -= height;
					}
					if (viewPnt.Y < displayRectangle.Y + unchecked(autoScrollRegion.Height / 4))
					{
						p.Y -= 4 * height;
					}
				}
				else if (viewPnt.Y <= displayRectangle.Y + displayRectangle.Height && viewPnt.Y > displayRectangle.Y + displayRectangle.Height - autoScrollRegion.Height)
				{
					p.Y += height;
					if (viewPnt.Y > displayRectangle.Y + displayRectangle.Height - unchecked(autoScrollRegion.Height / 2))
					{
						p.Y += height;
					}
					if (viewPnt.Y > displayRectangle.Y + displayRectangle.Height - unchecked(autoScrollRegion.Height / 4))
					{
						p.Y += 4 * height;
					}
				}
				return ConvertViewToDoc(p);
			}
		}

		private bool ShouldSerializeAutoPanRegion()
		{
			return AutoPanRegion != new Size(16, 16);
		}

		private void ResetAutoPanRegion()
		{
			AutoPanRegion = new Size(16, 16);
		}

		/// <summary>
		/// This method is called to determine the next position in the document for this view,
		/// given a point at which the user is holding the mouse during a pan operation.
		/// </summary>
		/// <param name="originPnt">
		/// The original panning point, in view coordinates.
		/// </param>
		/// <param name="viewPnt">
		/// The mouse point, in view coordinates.
		/// </param>
		/// <remarks>
		/// This uses the <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> property to calculate a new <see cref="P:Northwoods.Go.GoView.DocPosition" />.
		/// When the current mouse point is within the <see cref="P:Northwoods.Go.GoView.AutoPanRegion" /> width or height
		/// distance from the <paramref name="originPnt" />, no scrolling occurs.
		/// When the current mouse point is outside of this region, between the <see cref="P:Northwoods.Go.GoView.AutoPanRegion" />
		/// distance (width or height) and three times that distance, automatic scrolling proceeds
		/// at the smallest scrolling increment, <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" />.
		/// The farther away the <paramref name="viewPnt" /> is from the <paramref name="originPnt" />, the larger a multiple of
		/// the <see cref="P:Northwoods.Go.GoView.ScrollSmallChange" /> is used as a scroll step in that direction.
		/// </remarks>
		public virtual PointF ComputeAutoPanDocPosition(Point originPnt, Point viewPnt)
		{
			PointF docPosition = DocPosition;
			Point p = ConvertDocToView(docPosition);
			Size autoPanRegion = AutoPanRegion;
			int width = ScrollSmallChange.Width;
			int height = ScrollSmallChange.Height;
			Rectangle displayRectangle = DisplayRectangle;
			checked
			{
				int num = viewPnt.X - originPnt.X;
				int num2 = viewPnt.Y - originPnt.Y;
				int width2 = autoPanRegion.Width;
				int height2 = autoPanRegion.Height;
				int num3 = 2 * width2;
				int num4 = 2 * height2;
				if (num < -num3)
				{
					int num5;
					unchecked
					{
						num5 = checked((num + width2) * (num + width2)) / 100;
					}
					p.X -= Math.Min(displayRectangle.Width, width * num5);
				}
				else if (num < -width2)
				{
					p.X -= width;
				}
				else if (num > num3)
				{
					int num6;
					unchecked
					{
						num6 = checked((num - width2) * (num - width2)) / 100;
					}
					p.X += Math.Min(displayRectangle.Width, width * num6);
				}
				else if (num > width2)
				{
					p.X += width;
				}
				if (num2 < -num4)
				{
					int num7;
					unchecked
					{
						num7 = checked((num2 + height2) * (num2 + height2)) / 100;
					}
					p.Y -= Math.Min(displayRectangle.Height, height * num7);
				}
				else if (num2 < -height2)
				{
					p.Y -= height;
				}
				else if (num2 > num4)
				{
					int num8;
					unchecked
					{
						num8 = checked((num2 - height2) * (num2 - height2)) / 100;
					}
					p.Y += Math.Min(displayRectangle.Height, height * num8);
				}
				else if (num2 > height2)
				{
					p.Y += height;
				}
				return ConvertViewToDoc(p);
			}
		}

		/// <summary>
		/// Called to see if the user can select objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowSelect &amp;&amp; Document.CanSelectObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.SelectAll" /> and
		/// <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowSelect" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanSelectObjects" />
		public virtual bool CanSelectObjects()
		{
			if (AllowSelect)
			{
				return Document.CanSelectObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can move objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowMove &amp;&amp; Document.CanMoveObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> and
		/// by <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowMove" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanMoveObjects" />
		public virtual bool CanMoveObjects()
		{
			if (AllowMove)
			{
				return Document.CanMoveObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can copy objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowCopy &amp;&amp; Document.CanCopyObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// and <see cref="M:Northwoods.Go.GoView.EditCopy" /> and by <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowCopy" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanCopyObjects" />
		public virtual bool CanCopyObjects()
		{
			if (AllowCopy)
			{
				return Document.CanCopyObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can resize objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowResize &amp;&amp; Document.CanResizeObjects</c>.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowResize" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanResizeObjects" />
		public virtual bool CanResizeObjects()
		{
			if (AllowResize)
			{
				return Document.CanResizeObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can reshape objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowReshape &amp;&amp; Document.CanReshapeObjects</c>.
		/// <seealso cref="P:Northwoods.Go.GoView.AllowReshape" />
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanReshapeObjects" />
		public virtual bool CanReshapeObjects()
		{
			if (AllowReshape)
			{
				return Document.CanReshapeObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can delete objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowDelete &amp;&amp; Document.CanDeleteObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" /> and
		/// <see cref="M:Northwoods.Go.GoView.EditCut" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowDelete" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanDeleteObjects" />
		public virtual bool CanDeleteObjects()
		{
			if (AllowDelete)
			{
				return Document.CanDeleteObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can insert objects into this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowInsert &amp;&amp; Document.CanInsertObjects</c>.
		/// This predicate is used by methods such as <see cref="M:Northwoods.Go.GoView.EditPaste" /> and by
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowInsert" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanInsertObjects" />
		public virtual bool CanInsertObjects()
		{
			if (AllowInsert)
			{
				return Document.CanInsertObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can link objects together in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowLink &amp;&amp; Document.CanLinkObjects</c>.
		/// This predicate is used by <see cref="T:Northwoods.Go.GoToolLinking" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowLink" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanLinkObjects" />
		public virtual bool CanLinkObjects()
		{
			if (AllowLink)
			{
				return Document.CanLinkObjects();
			}
			return false;
		}

		/// <summary>
		/// Called to see if the user can edit objects in this view for this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowEdit &amp;&amp; Document.CanEditObjects</c>.
		/// This predicate is used by methods such as <see cref="T:Northwoods.Go.GoText" />'s <see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.AllowEdit" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanEditObjects" />
		public virtual bool CanEditObjects()
		{
			if (AllowEdit)
			{
				return Document.CanEditObjects();
			}
			return false;
		}

		/// <summary>
		/// This method sets some properties that determine whether the user can
		/// modify the document from this view.
		/// </summary>
		/// <param name="b"></param>
		/// <remarks>
		/// By default this just sets the <see cref="P:Northwoods.Go.GoView.AllowMove" />, <see cref="P:Northwoods.Go.GoView.AllowResize" />, 
		/// <see cref="P:Northwoods.Go.GoView.AllowReshape" />, <see cref="P:Northwoods.Go.GoView.AllowDelete" />, <see cref="P:Northwoods.Go.GoView.AllowInsert" />, 
		/// <see cref="P:Northwoods.Go.GoView.AllowLink" />, and <see cref="P:Northwoods.Go.GoView.AllowEdit" /> properties.
		/// You may want to override this in order to also control other properties you
		/// may have defined that govern the user's ability to modify the view's document.
		/// </remarks>
		public virtual void SetModifiable(bool b)
		{
			AllowMove = b;
			AllowResize = b;
			AllowReshape = b;
			AllowDelete = b;
			AllowInsert = b;
			AllowLink = b;
			AllowEdit = b;
		}

		/// <summary>
		/// Find a visible object at a given point.
		/// </summary>
		/// <param name="doc">If true, consider objects in document layers.</param>
		/// <param name="view">If true, consider objects in view layers.</param>
		/// <param name="p">The <c>PointF</c> in document coordinates at which to search.</param>
		/// <param name="selectableOnly">
		/// If true, skip over any objects whose <see cref="M:Northwoods.Go.GoObject.CanSelect" /> property is false.
		/// </param>
		/// <returns>
		/// A <see cref="T:Northwoods.Go.GoObject" /> that contains the <paramref name="p" />, or null if
		/// no such object exists.
		/// </returns>
		/// <remarks>
		/// This method never actually selects any object--use <see cref="T:Northwoods.Go.GoSelection" />
		/// instead.
		/// Please note that if an object is found, it might not be a top-level object.
		/// In fact, when <paramref name="selectableOnly" /> is false, it is very likely
		/// that if any object is found at the given point, it will be a child of some
		/// group.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoLayer.PickObject(System.Drawing.PointF,System.Boolean)" />
		public virtual GoObject PickObject(bool doc, bool view, PointF p, bool selectableOnly)
		{
			if (selectableOnly && !CanSelectObjects())
			{
				return null;
			}
			foreach (GoLayer backward in Layers.Backwards)
			{
				if ((doc && backward.IsInDocument) || (view && backward.IsInView))
				{
					GoObject goObject = backward.PickObject(p, selectableOnly);
					if (goObject != null)
					{
						return goObject;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Return a collection of objects that can be picked at a particular point.
		/// </summary>
		/// <param name="doc">If true, consider objects in document layers.</param>
		/// <param name="view">If true, consider objects in view layers.</param>
		/// <param name="p">A <c>PointF</c> location in document coordinates.</param>
		/// <param name="selectableOnly">If true, only consider objects for which <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.</param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoLayer.PickObjects(System.Drawing.PointF,System.Boolean,Northwoods.Go.IGoCollection,System.Int32)" /> on each of the <see cref="T:Northwoods.Go.GoLayer" />s used by this view
		/// and as filtered by the <paramref name="doc" /> and <paramref name="view" /> arguments,
		/// in reverse order, from front to back.
		/// Please note that when objects are found, they might not be a top-level objects.
		/// In fact, when <paramref name="selectableOnly" /> is false, it is very likely
		/// that if any object is found at the given point, it will be a child of some group.
		/// </remarks>
		public virtual IGoCollection PickObjects(bool doc, bool view, PointF p, bool selectableOnly, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection
				{
					InternalChecksForDuplicates = false
				};
			}
			if (selectableOnly && !CanSelectObjects())
			{
				return null;
			}
			foreach (GoLayer backward in Layers.Backwards)
			{
				if (coll.Count >= max)
				{
					return coll;
				}
				if ((doc && backward.IsInDocument) || (view && backward.IsInView))
				{
					backward.PickObjects(p, selectableOnly, coll, max);
				}
			}
			return coll;
		}

		/// <summary>
		/// Return a collection of objects that are surrounded by a given rectangle.
		/// </summary>
		/// <param name="doc">If true, consider objects in document layers.</param>
		/// <param name="view">If true, consider objects in view layers.</param>
		/// <param name="rect">A <c>RectangleF</c> in document coordinates.</param>
		/// <param name="pickstyle">
		/// If <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyContained" />
		/// or <see cref="F:Northwoods.Go.GoPickInRectangleStyle.SelectableOnlyIntersectsBounds" />,
		/// only consider objects for which <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.
		/// </param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// This basically calls <see cref="T:Northwoods.Go.GoLayer" />.<see cref="M:Northwoods.Go.GoLayer.PickObjectsInRectangle(System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />
		/// on each layer in this document.
		/// If <paramref name="pickstyle" /> is <c>GoPickInRectangleStyle.SelectableOnlyContained</c> and <see cref="M:Northwoods.Go.GoView.CanSelectObjects" /> is false,
		/// this will not add any objects to the result collection.
		/// </remarks>
		public IGoCollection PickObjectsInRectangle(bool doc, bool view, RectangleF rect, GoPickInRectangleStyle pickstyle, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection
				{
					InternalChecksForDuplicates = false
				};
			}
			if (GoDocument.PickStyleSelectableOnly(pickstyle) && !CanSelectObjects())
			{
				return coll;
			}
			foreach (GoLayer layer in Layers)
			{
				if (coll.Count >= max)
				{
					return coll;
				}
				if ((doc && layer.IsInDocument) || (view && layer.IsInView))
				{
					layer.PickObjectsInRectangle(rect, pickstyle, coll, max);
				}
			}
			return coll;
		}

		/// <summary>
		/// This method is called by the constructor to create a selection collection for this view.
		/// </summary>
		/// <returns>A <see cref="T:Northwoods.Go.GoSelection" />.</returns>
		/// <remarks>
		/// If you need to use your own subclass of <see cref="T:Northwoods.Go.GoSelection" />, this is
		/// the place to create it.
		/// </remarks>
		public virtual GoSelection CreateSelection()
		{
			return new GoSelection(this);
		}

		/// <summary>
		/// Add all eligible document objects to this view's selection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method only selects document objects.
		/// It heeds this view's <see cref="M:Northwoods.Go.GoView.CanSelectObjects" /> predicate,
		/// the <see cref="T:Northwoods.Go.GoLayer" />'s <see cref="M:Northwoods.Go.GoLayer.CanViewObjects" /> and <see cref="M:Northwoods.Go.GoLayer.CanSelectObjects" /> predicates,
		/// and the <see cref="T:Northwoods.Go.GoObject" />'s <see cref="M:Northwoods.Go.GoObject.CanView" /> and <see cref="M:Northwoods.Go.GoObject.CanSelect" /> predicates.
		/// </para>
		/// <para>
		/// This raises the <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> and <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events.
		/// </para>
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void SelectAll()
		{
			if (CanSelectObjects())
			{
				try
				{
					CursorName = "wait";
					List<GoObject> list = new List<GoObject>();
					foreach (GoLayer layer in Layers)
					{
						if (layer.IsInDocument && layer.CanViewObjects() && layer.CanSelectObjects())
						{
							foreach (GoObject item in layer)
							{
								if (item.CanView() && item.CanSelect())
								{
									list.Add(item);
								}
							}
						}
					}
					if (list.Count > 0)
					{
						RaiseSelectionStarting();
						foreach (GoObject item2 in list)
						{
							Selection.Add(item2);
						}
						RaiseSelectionFinished();
					}
				}
				finally
				{
					CursorName = "default";
				}
			}
		}

		/// <summary>
		/// Add all eligible document objects that are within a given rectangle to this view's selection.
		/// </summary>
		/// <param name="rect">A <c>RectangleF</c> in document coordinates.</param>
		/// <remarks>
		/// <para>
		/// This method only selects document objects.
		/// It basically just calls <see cref="T:Northwoods.Go.GoLayer" />.<see cref="M:Northwoods.Go.GoLayer.PickObjectsInRectangle(System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />
		/// for each document layer that this view uses, passing in the value of
		/// <see cref="P:Northwoods.Go.GoView.SelectInRectangleStyle" /> to control how the inclusion decision is made.
		/// If <see cref="M:Northwoods.Go.GoView.CanSelectObjects" /> is false, this will not select anything.
		/// This method is called by <see cref="T:Northwoods.Go.GoToolRubberBanding" />'s
		/// <see cref="M:Northwoods.Go.GoToolRubberBanding.DoRubberBand(System.Drawing.Rectangle)" /> method.
		/// </para>
		/// <para>
		/// This raises the <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> and <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events.
		/// </para>
		/// </remarks>
		public virtual void SelectInRectangle(RectangleF rect)
		{
			if (CanSelectObjects())
			{
				try
				{
					CursorName = "wait";
					GoCollection goCollection = new GoCollection();
					goCollection.InternalChecksForDuplicates = false;
					foreach (GoLayer layer in Layers)
					{
						if (layer.IsInDocument)
						{
							layer.PickObjectsInRectangle(rect, SelectInRectangleStyle, goCollection, MaximumSelectionCount);
						}
					}
					Selection.AddRange(goCollection);
				}
				finally
				{
					CursorName = "default";
				}
			}
		}

		/// <summary>
		/// Move a collection of objects by a given offset.
		/// </summary>
		/// <param name="sel">
		/// The collection of objects to be moved; if null, this view's <see cref="P:Northwoods.Go.GoView.Selection" /> is used.
		/// </param>
		/// <param name="offset">
		/// The distance the objects should be moved.
		/// </param>
		/// <param name="grid">
		/// Whether to adjust the computed destination location by calling <see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />.
		/// This is normally true, to allow any relevant grids to decide how to control movement of objects.
		/// </param>
		/// <remarks>
		/// <para>
		/// This method heeds the <see cref="M:Northwoods.Go.GoView.CanMoveObjects" /> property if <paramref name="sel" />
		/// is this view's <see cref="P:Northwoods.Go.GoView.Selection" />, and the <see cref="M:Northwoods.Go.GoObject.CanMove" /> predicate
		/// for each <see cref="T:Northwoods.Go.GoObject" /> that is not an <see cref="T:Northwoods.Go.IGoLink" />.
		/// To accomplish the move, each object's <see cref="M:Northwoods.Go.GoObject.DoMove(Northwoods.Go.GoView,System.Drawing.PointF,System.Drawing.PointF)" /> is called,
		/// to support object-specific move behavior.
		/// All actions take place within a <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="F:Northwoods.Go.GoUndoManager.MoveSelectionName" /> transaction.
		/// This does not raise the <c>SelectionMoved</c> event;
		/// that event is raised by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool's <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" /> method.
		/// If there are any duplicates in the selection, objects may get moved multiple times.
		/// Note that an object may be moved twice if both it and one of its parents are
		/// in the selection.
		/// </para>
		/// <para>
		/// When <paramref name="grid" /> is true, each of the non-link objects in the selection <paramref name="sel" />
		/// is "snapped" to the proper location, by calling <see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />.
		/// It is always each object's <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Location" />
		/// that is snapped to some point.
		/// However, links that are in the selection are never "snapped"; they are always moved by the
		/// distance that the first movable non-link object in the selection is moved, which may have
		/// been grid-snapped.
		/// This policy supports moving links without recalculating their paths, as long as their connected
		/// ports/nodes are all moved together by the same distance, whether those nodes are "snapped" or not.
		/// </para>
		/// </remarks>
		public virtual void MoveSelection(GoSelection sel, SizeF offset, bool grid)
		{
			if (sel == null)
			{
				sel = Selection;
			}
			if ((sel != Selection || CanMoveObjects()) && !sel.IsEmpty)
			{
				GoDocument document = Document;
				string tname = null;
				bool suspendsRouting = document.SuspendsRouting;
				try
				{
					StartTransaction();
					document.SuspendsRouting = true;
					GoObject goObject = null;
					foreach (GoObject item in sel)
					{
						if (!(item is IGoLink) && item.CanMove())
						{
							goObject = item;
							break;
						}
					}
					SizeF sizeF = offset;
					if (goObject != null)
					{
						PointF location = goObject.Location;
						PointF p = new PointF(location.X + offset.Width, location.Y + offset.Height);
						if (grid)
						{
							p = SnapPoint(p, goObject);
						}
						sizeF.Width = p.X - location.X;
						sizeF.Height = p.Y - location.Y;
					}
					foreach (GoObject item2 in sel)
					{
						if (item2 is IGoLink)
						{
							item2.DoMove(this, item2.Position, new PointF(item2.Left + sizeF.Width, item2.Top + sizeF.Height));
						}
					}
					foreach (GoObject item3 in sel)
					{
						if (!(item3 is IGoLink) && item3.CanMove())
						{
							PointF location2 = item3.Location;
							PointF pointF = new PointF(location2.X + sizeF.Width, location2.Y + sizeF.Height);
							if (grid)
							{
								pointF = SnapPoint(pointF, item3);
							}
							item3.DoMove(this, location2, pointF);
						}
					}
					if (sizeF.Width != 0f || sizeF.Height != 0f)
					{
						document.ResumeRouting(suspendsRouting, sel);
					}
					tname = "Move Selection";
				}
				finally
				{
					document.SuspendsRouting = suspendsRouting;
					FinishTransaction(tname);
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionMoved" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionMoved(EventArgs evt)
		{
			if (this.SelectionMoved != null)
			{
				this.SelectionMoved(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionMoved(System.EventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.SelectionMoved" /> event.
		/// </summary>
		public void RaiseSelectionMoved()
		{
			OnSelectionMoved(EventArgs.Empty);
		}

		/// <summary>
		/// Make copies of the objects in a collection and add them to this view's document at the given offset.
		/// </summary>
		/// <param name="sel">
		/// The collection of objects to be copied; if null, this view's <see cref="P:Northwoods.Go.GoView.Selection" /> is used.
		/// </param>
		/// <param name="offset">
		/// The distance the objects should be moved from the original's location.
		/// </param>
		/// <param name="grid">
		/// Whether to adjust the computed destination location by calling <see cref="M:Northwoods.Go.GoView.SnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject)" />.
		/// This is normally true, to allow any relevant grids to decide how to control movement of objects.
		/// </param>
		/// <remarks>
		/// This method heeds the <see cref="M:Northwoods.Go.GoView.CanCopyObjects" /> property if <paramref name="sel" />
		/// is this view's <see cref="P:Northwoods.Go.GoView.Selection" />, and it heeds each object's <see cref="M:Northwoods.Go.GoObject.CanCopy" />
		/// property as well.
		/// The <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.DraggingObject" /> property allows
		/// individual objects to decide to copy the parent group instead of the object itself.
		/// To accomplish the copy, this method calls <see cref="T:Northwoods.Go.GoDocument" />'s <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />.
		/// To accomplish the move, each object's <see cref="M:Northwoods.Go.GoObject.DoMove(Northwoods.Go.GoView,System.Drawing.PointF,System.Drawing.PointF)" /> is called,
		/// to support object-specific move behavior.
		/// All actions take place within a <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="F:Northwoods.Go.GoUndoManager.CopySelectionName" /> transaction.
		/// The newly copied top-level objects become this view's new <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// This does not raise the <c>SelectionCopied</c> event;
		/// that event is raised by the <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" /> method of a <see cref="T:Northwoods.Go.GoToolDragging" /> tool.
		/// </remarks>
		public virtual void CopySelection(GoSelection sel, SizeF offset, bool grid)
		{
			if (sel == null)
			{
				sel = Selection;
			}
			if ((sel != Selection || CanCopyObjects()) && !sel.IsEmpty)
			{
				GoDocument document = Document;
				string tname = null;
				bool suspendsRouting = document.SuspendsRouting;
				try
				{
					StartTransaction();
					document.SuspendsRouting = true;
					GoCollection goCollection = new GoCollection();
					goCollection.InternalChecksForDuplicates = false;
					goCollection.AddRange(sel);
					Layers.SortByZOrder(goCollection);
					GoCopyDictionary goCopyDictionary = document.CreateCopyDictionary();
					document.CopyFromCollection(goCollection, copyableOnly: true, dragging: true, offset, goCopyDictionary);
					Selection.Clear();
					RaiseSelectionStarting();
					foreach (GoObject item in goCollection)
					{
						GoObject goObject = goCopyDictionary[item] as GoObject;
						if (goObject != null && goObject.IsTopLevel && goObject.Document == document)
						{
							Selection.Add(goObject);
						}
					}
					RaiseSelectionFinished();
					if (grid)
					{
						GoObject goObject2 = null;
						foreach (GoObject item2 in Selection)
						{
							if (!(item2 is IGoLink))
							{
								goObject2 = item2;
								break;
							}
						}
						SizeF sizeF = offset;
						if (goObject2 != null)
						{
							PointF location = goObject2.Location;
							PointF pointF = SnapPoint(location, goObject2);
							sizeF.Width = pointF.X - location.X;
							sizeF.Height = pointF.Y - location.Y;
							foreach (GoObject item3 in Selection)
							{
								if (item3 is IGoLink)
								{
									item3.Position = new PointF(item3.Left + sizeF.Width, item3.Top + sizeF.Height);
								}
							}
						}
						foreach (GoObject item4 in Selection)
						{
							if (!(item4 is IGoLink))
							{
								PointF location2 = item4.Location;
								PointF newLoc = SnapPoint(location2, item4);
								item4.DoMove(this, location2, newLoc);
							}
						}
					}
					document.ResumeRouting(suspendsRouting, Selection);
					tname = "Copy Selection";
				}
				finally
				{
					document.SuspendsRouting = suspendsRouting;
					FinishTransaction(tname);
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionCopied" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionCopied(EventArgs evt)
		{
			if (this.SelectionCopied != null)
			{
				this.SelectionCopied(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionCopied(System.EventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.SelectionCopied" /> event.
		/// </summary>
		public void RaiseSelectionCopied()
		{
			OnSelectionCopied(EventArgs.Empty);
		}

		/// <summary>
		/// Delete a collection of objects.
		/// </summary>
		/// <param name="sel">
		/// The collection of objects to be deleted; if null, this view's <see cref="P:Northwoods.Go.GoView.Selection" /> is used.
		/// </param>
		/// <remarks>
		/// This method heeds the <see cref="M:Northwoods.Go.GoView.CanDeleteObjects" /> property if <paramref name="sel" />
		/// is this view's <see cref="P:Northwoods.Go.GoView.Selection" />, and it heeds each object's <see cref="M:Northwoods.Go.GoObject.CanDelete" />
		/// property as well.
		/// All actions take place within a <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="F:Northwoods.Go.GoUndoManager.DeleteSelectionName" /> transaction.
		/// This method also calls <see cref="M:Northwoods.Go.GoView.RaiseSelectionDeleting(System.ComponentModel.CancelEventArgs)" />, <see cref="M:Northwoods.Go.GoView.RaiseSelectionStarting" />,
		/// <see cref="M:Northwoods.Go.GoView.RaiseSelectionFinished" />, and <see cref="M:Northwoods.Go.GoView.RaiseSelectionDeleted" />.
		/// </remarks>
		public virtual void DeleteSelection(GoSelection sel)
		{
			if (sel == null)
			{
				sel = Selection;
			}
			checked
			{
				if ((sel != Selection || CanDeleteObjects()) && !sel.IsEmpty)
				{
					string tname = null;
					try
					{
						StartTransaction();
						CancelEventArgs cancelEventArgs = new CancelEventArgs();
						RaiseSelectionDeleting(cancelEventArgs);
						if (!cancelEventArgs.Cancel)
						{
							RaiseSelectionStarting();
							sel.RemoveAllSelectionHandles();
							GoObject[] array = sel.CopyArray();
							Layers.SortByZOrder(array);
							for (int num = array.Length - 1; num >= 0; num--)
							{
								GoObject goObject = array[num];
								if (goObject != null && goObject.CanDelete())
								{
									goObject.Remove();
									sel.Remove(goObject);
								}
							}
							RaiseSelectionFinished();
							tname = "Delete Selection";
							RaiseSelectionDeleted();
						}
					}
					finally
					{
						FinishTransaction(tname);
					}
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionDeleting" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionDeleting(CancelEventArgs evt)
		{
			if (this.SelectionDeleting != null)
			{
				this.SelectionDeleting(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionDeleting(System.ComponentModel.CancelEventArgs)" /> with the given <c>CancelEventArgs</c>
		/// to raise a <see cref="E:Northwoods.Go.GoView.SelectionDeleting" /> event.
		/// </summary>
		public void RaiseSelectionDeleting(CancelEventArgs evt)
		{
			OnSelectionDeleting(evt);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionDeleted" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionDeleted(EventArgs evt)
		{
			if (this.SelectionDeleted != null)
			{
				this.SelectionDeleted(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionDeleted(System.EventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.SelectionDeleted" /> event.
		/// </summary>
		public void RaiseSelectionDeleted()
		{
			OnSelectionDeleted(EventArgs.Empty);
		}

		/// <summary>
		/// This predicate is true when the user can perform the <see cref="M:Northwoods.Go.GoView.EditDelete" /> action.
		/// </summary>
		/// <remarks>
		/// This returns false if the <see cref="P:Northwoods.Go.GoView.Selection" /> is empty,
		/// if <see cref="M:Northwoods.Go.GoView.CanDeleteObjects" /> is false,
		/// or if the primary selection's <see cref="M:Northwoods.Go.GoObject.CanDelete" /> property is false.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.EditDelete" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanEditDelete()
		{
			if (!CanDeleteObjects())
			{
				return false;
			}
			if (Selection.IsEmpty)
			{
				return false;
			}
			if (!Selection.Primary.CanDelete())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Remove all selected objects from this view's document.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" /> with this view's
		/// <see cref="P:Northwoods.Go.GoView.Selection" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditDelete" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void EditDelete()
		{
			try
			{
				CursorName = "wait";
				DeleteSelection(Selection);
			}
			finally
			{
				CursorName = "default";
			}
		}

		/// <summary>
		/// Begin having the user edit an object.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This method does nothing if <see cref="M:Northwoods.Go.GoView.CanEditObjects" /> is false
		/// or if <paramref name="obj" />'s <see cref="M:Northwoods.Go.GoObject.CanEdit" /> property
		/// is false.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoObject.DoBeginEdit(Northwoods.Go.GoView)" />
		public virtual void EditObject(GoObject obj)
		{
			if (obj != null && CanEditObjects() && obj.CanEdit())
			{
				obj.DoBeginEdit(this);
			}
		}

		/// <summary>
		/// This predicate is true when the user can perform the <see cref="M:Northwoods.Go.GoView.EditEdit" /> action.
		/// </summary>
		/// <remarks>
		/// This returns false if the <see cref="P:Northwoods.Go.GoView.Selection" /> is empty,
		/// if <see cref="M:Northwoods.Go.GoView.CanEditObjects" /> is false,
		/// or if the primary selection's <see cref="M:Northwoods.Go.GoObject.CanEdit" /> property is false.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.EditObject(Northwoods.Go.GoObject)" />
		/// <seealso cref="M:Northwoods.Go.GoView.EditEdit" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanEditEdit()
		{
			if (!CanEditObjects())
			{
				return false;
			}
			if (Selection.IsEmpty)
			{
				return false;
			}
			if (!Selection.Primary.CanEdit())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Have the user edit the primary selection.
		/// </summary>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoView.EditObject(Northwoods.Go.GoObject)" /> with the view's
		/// <see cref="P:Northwoods.Go.GoSelection.Primary" /> selection.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditEdit" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void EditEdit()
		{
			EditObject(Selection.Primary);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectGotSelection(GoSelectionEventArgs evt)
		{
			if (this.ObjectGotSelection != null)
			{
				this.ObjectGotSelection(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnObjectGotSelection(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise an <see cref="E:Northwoods.Go.GoView.ObjectGotSelection" /> event.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This is called by methods in <see cref="T:Northwoods.Go.GoSelection" /> for each object added to the selection.
		/// </remarks>
		public void RaiseObjectGotSelection(GoObject obj)
		{
			OnObjectGotSelection(new GoSelectionEventArgs(obj));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectLostSelection(GoSelectionEventArgs evt)
		{
			if (this.ObjectLostSelection != null)
			{
				this.ObjectLostSelection(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnObjectLostSelection(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise an <see cref="E:Northwoods.Go.GoView.ObjectLostSelection" /> event.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This is called by methods in <see cref="T:Northwoods.Go.GoSelection" /> for each object removed from the selection.
		/// </remarks>
		public void RaiseObjectLostSelection(GoObject obj)
		{
			OnObjectLostSelection(new GoSelectionEventArgs(obj));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionStarting(EventArgs evt)
		{
			if (this.SelectionStarting != null)
			{
				this.SelectionStarting(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionStarting(System.EventArgs)" />.
		/// </summary>
		/// <remarks>
		/// This is called by methods before they select or unselect a variable number of objects.
		/// </remarks>
		public void RaiseSelectionStarting()
		{
			OnSelectionStarting(EventArgs.Empty);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnSelectionFinished(EventArgs evt)
		{
			if (this.SelectionFinished != null)
			{
				this.SelectionFinished(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnSelectionFinished(System.EventArgs)" />.
		/// </summary>
		/// <remarks>
		/// This is called by methods after they select or unselect a variable number of objects.
		/// </remarks>
		public void RaiseSelectionFinished()
		{
			OnSelectionFinished(EventArgs.Empty);
		}

		private bool ShouldSerializePrimarySelectionColor()
		{
			return PrimarySelectionColor != Color.Chartreuse;
		}

		private void ResetPrimarySelectionColor()
		{
			PrimarySelectionColor = Color.Chartreuse;
		}

		private bool ShouldSerializeSecondarySelectionColor()
		{
			return SecondarySelectionColor != Color.Cyan;
		}

		private void ResetSecondarySelectionColor()
		{
			SecondarySelectionColor = Color.Cyan;
		}

		private bool ShouldSerializeNoFocusSelectionColor()
		{
			return NoFocusSelectionColor != Color.LightGray;
		}

		private void ResetNoFocusSelectionColor()
		{
			NoFocusSelectionColor = Color.LightGray;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectResized" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectResized(GoSelectionEventArgs evt)
		{
			if (this.ObjectResized != null)
			{
				this.ObjectResized(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnObjectResized(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise an <see cref="E:Northwoods.Go.GoView.ObjectResized" /> event.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This method is called by <see cref="T:Northwoods.Go.GoToolResizing" />.<see cref="M:Northwoods.Go.GoToolResizing.DoMouseUp" />.
		/// </remarks>
		public void RaiseObjectResized(GoObject obj)
		{
			OnObjectResized(new GoSelectionEventArgs(obj));
		}

		/// <summary>
		/// Change the primary selection to be the next object whose label
		/// starts with a given letter or digit.
		/// </summary>
		/// <param name="c"></param>
		/// <returns>true if it found another part</returns>
		/// <remarks>
		/// This method iterates through the <see cref="T:Northwoods.Go.IGoLabeledPart" />s
		/// of this view's document, starting with the current primary
		/// selection (if it is an <see cref="T:Northwoods.Go.IGoLabeledPart" />), looking
		/// for the next part that satisfies the <see cref="M:Northwoods.Go.GoView.MatchesNodeLabel(Northwoods.Go.IGoLabeledPart,System.Char)" />
		/// predicate.
		/// If it finds such an object, it makes it this view's only selected object.
		/// This method is called by <see cref="T:Northwoods.Go.GoToolManager" /> when the user
		/// types a letter or digit, if the value of <see cref="P:Northwoods.Go.GoView.DisableKeys" />
		/// does not have the <see cref="F:Northwoods.Go.GoViewDisableKeys.SelectsByFirstChar" /> flag set.
		/// (This method does not check for that flag.)
		/// This method does nothing if <see cref="M:Northwoods.Go.GoView.CanSelectObjects" /> is false.
		/// </remarks>
		public virtual bool SelectNextNode(char c)
		{
			if (!CanSelectObjects())
			{
				return false;
			}
			IGoLabeledPart goLabeledPart = null;
			GoObject primary = Selection.Primary;
			if (primary != null && primary is IGoLabeledPart)
			{
				goLabeledPart = (IGoLabeledPart)primary;
			}
			GoLayerCollectionObjectEnumerator enumerator = Document.GetEnumerator();
			if (goLabeledPart != null)
			{
				while (enumerator.MoveNext() && enumerator.Current != goLabeledPart)
				{
				}
			}
			while (enumerator.MoveNext())
			{
				GoObject current = enumerator.Current;
				IGoLabeledPart goLabeledPart2 = current as IGoLabeledPart;
				if (goLabeledPart2 != null && MatchesNodeLabel(goLabeledPart2, c))
				{
					Selection.Select(current);
					ScrollRectangleToVisible(current.Bounds);
					return true;
				}
			}
			enumerator = Document.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GoObject current2 = enumerator.Current;
				IGoLabeledPart goLabeledPart3 = current2 as IGoLabeledPart;
				if (goLabeledPart3 == goLabeledPart)
				{
					break;
				}
				if (goLabeledPart3 != null && MatchesNodeLabel(goLabeledPart3, c))
				{
					Selection.Select(current2);
					ScrollRectangleToVisible(current2.Bounds);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This predicate is called by <see cref="M:Northwoods.Go.GoView.SelectNextNode(System.Char)" /> to
		/// decide if a part's text label starts with a given character.
		/// </summary>
		/// <param name="part"></param>
		/// <param name="c">a <c>Char</c>, normally a letter or digit</param>
		/// <returns>
		/// true if the <paramref name="part" />'s <see cref="P:Northwoods.Go.IGoLabeledPart.Text" />'s
		/// first character is the same as <paramref name="c" />, ignoring case
		/// </returns>
		public virtual bool MatchesNodeLabel(IGoLabeledPart part, char c)
		{
			if (part == null)
			{
				return false;
			}
			string text = part.Text;
			if (text == null)
			{
				return false;
			}
			if (text.Length == 0)
			{
				return false;
			}
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return char.ToUpper(text[0], currentCulture) == char.ToUpper(c, currentCulture);
		}

		internal PointF[] AllocTempPointArray(int len)
		{
			if (myTempArrays == null || len >= myTempArrays.Length)
			{
				myTempArrays = new PointF[Math.Max(checked(len + 1), 10)][];
			}
			PointF[] array = myTempArrays[len];
			if (array == null)
			{
				array = new PointF[len];
			}
			else
			{
				myTempArrays[len] = null;
			}
			return array;
		}

		internal void FreeTempPointArray(PointF[] a)
		{
			int num = a.Length;
			if (myTempArrays != null && num < myTempArrays.Length)
			{
				myTempArrays[num] = a;
			}
		}

		/// <summary>
		/// Return a <c>Bitmap</c> displaying all of the visible objects in the given collection.
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		/// <remarks>
		/// The intended size of the <c>Bitmap</c> is determined by the result of calling
		/// the <see cref="T:Northwoods.Go.GoDocument" /> method
		/// <see cref="M:Northwoods.Go.GoDocument.ComputeBounds(Northwoods.Go.IGoCollection,Northwoods.Go.GoView)" /> on the collection.
		/// If the bitmap is too wide or too tall, the objects are drawn at a smaller scale.
		/// The bitmap is first filled with the paper color.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection,System.Drawing.RectangleF,System.Single,System.Boolean)" />
		/// <seealso cref="M:Northwoods.Go.GoView.GetBitmap" />
		public Bitmap GetBitmapFromCollection(IGoCollection coll)
		{
			RectangleF bounds = GoDocument.ComputeBounds(coll, this);
			return GetBitmapFromCollection(coll, bounds, paper: true);
		}

		/// <summary>
		/// This calls <see cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection,System.Drawing.RectangleF,System.Single,System.Boolean)" />
		/// with a scale that will cause the resulting bitmap not to be too large.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="bounds">a <c>RectangleF</c> in document coordinates</param>
		/// <param name="paper">whether to call <see cref="M:Northwoods.Go.GoView.PaintPaperColor(System.Drawing.Graphics,System.Drawing.RectangleF)" /> for the background</param>
		/// <returns>A <c>Bitmap</c></returns>
		/// <remarks>
		/// If the bitmap is too wide or too tall, the objects are drawn at a smaller scale.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.GetBitmap" />
		public Bitmap GetBitmapFromCollection(IGoCollection coll, RectangleF bounds, bool paper)
		{
			float scale = 1f;
			float num = 2000f / WorldScale.Width;
			float num2 = 2000f / WorldScale.Height;
			if (bounds.Width > num || bounds.Height > num2)
			{
				scale = Math.Min(num / bounds.Width, num2 / bounds.Height);
			}
			return GetBitmapFromCollection(coll, bounds, scale, paper);
		}

		/// <summary>
		/// Return a Bitmap displaying all of the visible objects in the given collection
		/// that paint within the given bounds.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="bounds">The portion of the rendering, in document coordinates</param>
		/// <param name="scale">The scale at which to draw; 1.0 is normal</param>
		/// <param name="paper">Whether to call <see cref="M:Northwoods.Go.GoView.PaintPaperColor(System.Drawing.Graphics,System.Drawing.RectangleF)" /> for the background</param>
		/// <returns>A <c>Bitmap</c></returns>
		/// <remarks>
		/// This method creates a <c>Bitmap</c> whose size is determined by
		/// <paramref name="bounds" /> multiplied by the <paramref name="scale" /> (minimum 1x1),
		/// that displays what is painted starting at the rectangle's top-left position in the document.
		/// It calls <see cref="M:Northwoods.Go.GoView.PaintPaperColor(System.Drawing.Graphics,System.Drawing.RectangleF)" /> to fill in the bitmap and then calls
		/// <see cref="M:Northwoods.Go.GoObject.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> for each object in <paramref name="coll" />
		/// whose <see cref="M:Northwoods.Go.GoObject.CanView" /> property is true.
		/// <para>
		/// Unlike the other <see cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection)" /> methods,
		/// this method does not automatically limit the size of the Bitmap based on the
		/// <paramref name="bounds" /> and <paramref name="scale" /> values.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.GetBitmap" />
		public virtual Bitmap GetBitmapFromCollection(IGoCollection coll, RectangleF bounds, float scale, bool paper)
		{
			if (scale < 9E-09f)
			{
				scale = 9E-09f;
			}
			checked
			{
				int num = (int)Math.Ceiling(bounds.Width * scale);
				int num2 = (int)Math.Ceiling(bounds.Height * scale);
				if (num < 1)
				{
					num = 1;
				}
				if (num2 < 1)
				{
					num2 = 1;
				}
				lock (typeof(GoView))
				{
					Bitmap bitmap = new Bitmap(num, num2);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.PageUnit = GraphicsUnit.Pixel;
					graphics.SmoothingMode = SmoothingMode;
					graphics.TextRenderingHint = TextRenderingHint;
					graphics.InterpolationMode = InterpolationMode;
					graphics.CompositingQuality = CompositingQuality;
					graphics.PixelOffsetMode = PixelOffsetMode;
					graphics.ScaleTransform(scale, scale);
					graphics.TranslateTransform(0f - bounds.X, 0f - bounds.Y);
					PointF pointF = myOrigin;
					float num3 = myHorizScale;
					float num4 = myVertScale;
					Size size = myBorderSize;
					Rectangle rectangle = myDisplayRectangle;
					bool flag = myIsRenderingBitmap;
					myOrigin = new PointF(bounds.X, bounds.Y);
					myHorizScale = scale;
					myVertScale = scale;
					myBorderSize = new Size(0, 0);
					myViewSize = new Size(num, num2);
					myDisplayRectangle = new Rectangle(0, 0, num, num2);
					myIsRenderingBitmap = true;
					try
					{
						if (paper)
						{
							RectangleF a = bounds;
							GoObject.InflateRect(ref a, 1f, 1f);
							PaintPaperColor(graphics, a);
						}
						foreach (GoObject item in coll)
						{
							if (item.CanView())
							{
								item.Paint(graphics, this);
							}
						}
					}
					finally
					{
						myOrigin = pointF;
						myHorizScale = num3;
						myVertScale = num4;
						myBorderSize = size;
						myViewSize = new Size(-1, -1);
						myDisplayRectangle = rectangle;
						myIsRenderingBitmap = flag;
					}
					graphics.Dispose();
					return bitmap;
				}
			}
		}

		/// <summary>
		/// Draw a line on the screen in XOR mode.
		/// </summary>
		/// <param name="ax"></param>
		/// <param name="ay"></param>
		/// <param name="bx"></param>
		/// <param name="by"></param>
		/// <remarks>
		/// The parameters are all in view coordinates.
		/// You should call this method twice for each set of argument values--
		/// once to draw the line and once to restore the original screen image.
		/// </remarks>
		public void DrawXorLine(int ax, int ay, int bx, int by)
		{
			Point p = new Point(ax, ay);
			Point p2 = new Point(bx, by);
			Point start = PointToScreen(p);
			Point end = PointToScreen(p2);
			Color color = Document.PaperColor;
			if (color == Color.Empty)
			{
				color = BackColor;
			}
			ControlPaint.DrawReversibleLine(start, end, color);
		}

		/// <summary>
		/// Draw a rectangle on the screen in XOR mode.
		/// </summary>
		/// <param name="rect"></param>
		/// <remarks>
		/// The parameter is in view coordinates.
		/// You should call this method twice for each set of rectangular coordinates--
		/// once to draw the rectangle and once to restore the original screen image.
		/// </remarks>
		public void DrawXorRectangle(Rectangle rect)
		{
			Rectangle rectangle = RectangleToScreen(rect);
			Color color = Document.PaperColor;
			if (color == Color.Empty)
			{
				color = BackColor;
			}
			ControlPaint.DrawReversibleFrame(rectangle, color, FrameStyle.Thick);
		}

		/// <summary>
		/// This convenience method erases any previous XOR-drawn rectangle and then
		/// may draw a new one with the given dimensions.
		/// </summary>
		/// <param name="rect">The size and location of the rectangle to draw, in view coordinates.</param>
		/// <param name="drawnew">Whether to draw the new rectangle.</param>
		/// <remarks>
		/// <para>
		/// This always erases any earlier rectangle drawn by this method.
		/// It only draws a new rectangle if <paramref name="drawnew" /> is true.
		/// </para>
		/// <para>
		/// If <see cref="P:Northwoods.Go.GoView.DrawsXorMode" /> is false, this draws a rectangle using a
		/// checkerboard pen, instead of drawing in XOR mode.
		/// </para>
		/// </remarks>
		public virtual void DrawXorBox(Rectangle rect, bool drawnew)
		{
			if (myPrevXorRectValid)
			{
				myPrevXorRectValid = false;
				try
				{
					DrawXorRectangle(myPrevXorRect);
				}
				catch (VerificationException)
				{
					DrawsXorMode = false;
				}
				catch (SecurityException)
				{
					DrawsXorMode = false;
				}
			}
			if (drawnew)
			{
				if (DrawsXorMode)
				{
					try
					{
						DrawXorRectangle(rect);
						myPrevXorRect = rect;
						myPrevXorRectValid = true;
					}
					catch (VerificationException)
					{
						DrawsXorMode = false;
					}
					catch (SecurityException)
					{
						DrawsXorMode = false;
					}
					return;
				}
				if (myMarquee == null)
				{
					myMarquee = new GoRectangle();
					myMarquee.Brush = null;
					myMarquee.Pen = null;
				}
				if (myMarquee.Pen == null)
				{
					Pen pen = new Pen(new HatchBrush(HatchStyle.SmallCheckerBoard, Color.Black, Color.White), 2f / DocScale);
					myMarquee.Pen = pen;
				}
				myMarquee.Bounds = ConvertViewToDoc(rect);
				Layers.Top.Add(myMarquee);
			}
			else if (myMarquee != null)
			{
				myMarquee.Remove();
			}
		}

		private void UpdateBorderWidths()
		{
			Size size = myBorderSize;
			switch (BorderStyle)
			{
			case BorderStyle.None:
				size = default(Size);
				break;
			case BorderStyle.FixedSingle:
				size = SystemInformation.BorderSize;
				break;
			default:
				size = SystemInformation.Border3DSize;
				break;
			}
			if (size != myBorderSize)
			{
				myBorderSize = size;
				LayoutScrollBars(update: false);
			}
		}

		internal void AddGoControl(GoControl g, Control c)
		{
			if (myGoControls == null)
			{
				myGoControls = new List<GoControl>();
			}
			myGoControls.Add(g);
			base.Controls.Add(c);
		}

		internal void RemoveGoControl(GoControl g, Control c)
		{
			if (myGoControls != null)
			{
				myGoControls.Remove(g);
				base.Controls.Remove(c);
			}
		}

		internal void RemoveAllGoControls(GoLayer layer, bool remove)
		{
			if (myGoControls == null)
			{
				return;
			}
			GoControl[] array = myGoControls.ToArray();
			foreach (GoControl goControl in array)
			{
				if (goControl.Layer != layer)
				{
					continue;
				}
				Control control = goControl.FindControl(this);
				if (control != null)
				{
					if (remove)
					{
						base.Controls.Remove(control);
						myGoControls.Remove(goControl);
					}
					else
					{
						control.Visible = false;
					}
				}
			}
		}

		/// <summary>
		/// Called by the system when needing to fix up invalidated parts of this view.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnPaint(PaintEventArgs evt)
		{
			try
			{
				onPaintCanvas(evt);
				if (myGoControls != null && myGoControls.Count > 0)
				{
					Rectangle displayRectangle = DisplayRectangle;
					foreach (GoControl myGoControl in myGoControls)
					{
						Control control = myGoControl.FindControl(this);
						if (control != null)
						{
							Rectangle rectangle = ConvertDocToView(myGoControl.Bounds);
							if (!displayRectangle.IntersectsWith(rectangle))
							{
								control.Bounds = rectangle;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				GoObject.Trace("OnPaint: " + ex.ToString());
				throw;
			}
			base.OnPaint(evt);
		}

		internal void TranslateTransform(Graphics g, float w, float h)
		{
			g.TranslateTransform(w, h);
		}

		private void onPaintCanvas(PaintEventArgs evt)
		{
			if (SuppressingPaint)
			{
				return;
			}
			myPaintEventArgs = evt;
			Graphics graphics = evt.Graphics;
			graphics.PageUnit = GraphicsUnit.Pixel;
			Rectangle clipRectangle = evt.ClipRectangle;
			if (clipRectangle.Width <= 0 || clipRectangle.Height <= 0)
			{
				return;
			}
			Rectangle clientRectangle = base.ClientRectangle;
			if (clientRectangle.Width <= 0 || clientRectangle.Height <= 0)
			{
				return;
			}
			if (myBuffer == null || myBuffer.Width < clientRectangle.Width || myBuffer.Height < clientRectangle.Height)
			{
				if (myBuffer != null)
				{
					myBuffer.Dispose();
				}
				myBuffer = new Bitmap(clientRectangle.Width, clientRectangle.Height, graphics);
			}
			Graphics graphics2 = Graphics.FromImage(myBuffer);
			graphics2.PageUnit = GraphicsUnit.Pixel;
			PaintBorder(graphics2, clientRectangle, clipRectangle);
			Rectangle rectangle = Rectangle.Intersect(clipRectangle, DisplayRectangle);
			graphics2.IntersectClip(rectangle);
			RectangleF clipRect = ConvertViewToDoc(rectangle);
			graphics2.TranslateTransform(myDisplayRectangle.X, myDisplayRectangle.Y);
			graphics2.ScaleTransform(myHorizScale * myHorizWorld, myVertScale * myVertWorld);
			PointF docPosition = DocPosition;
			graphics2.TranslateTransform(0f - docPosition.X, 0f - docPosition.Y);
			UpdateDelayedSelectionHandles();
			PaintView(graphics2, clipRect);
			myCurrentResult.Dispose(this);
			graphics2.Dispose();
		}

		internal void PaintBorder(Graphics g, Rectangle rect, Rectangle clipRect)
		{
			checked
			{
				switch (BorderStyle)
				{
				case BorderStyle.None:
					break;
				case BorderStyle.FixedSingle:
					if (clipRect.X <= rect.X + myBorderSize.Width || clipRect.Y <= rect.Y + myBorderSize.Height || clipRect.X + clipRect.Width >= rect.X + rect.Width - myBorderSize.Width || clipRect.Y + clipRect.Height >= rect.Y + rect.Height - myBorderSize.Height)
					{
						g.DrawRectangle(GoShape.SystemPens_WindowFrame, rect);
					}
					break;
				default:
					if (clipRect.X <= rect.X + myBorderSize.Width || clipRect.Y <= rect.Y + myBorderSize.Height || clipRect.X + clipRect.Width >= rect.X + rect.Width - myBorderSize.Width || clipRect.Y + clipRect.Height >= rect.Y + rect.Height - myBorderSize.Height)
					{
						ControlPaint.DrawBorder3D(g, rect, Border3DStyle);
					}
					break;
				}
			}
		}

		/// <summary>
		/// Produce a rendering of this view.
		/// </summary>
		/// <returns>a <c>Bitmap</c> containing the appearance of this view in its current state</returns>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoView.DocPosition" /> and <see cref="P:Northwoods.Go.GoView.DocScale" /> determine what part of this
		/// view's <see cref="P:Northwoods.Go.GoView.Document" /> and this view's state are painted in a bitmap.
		/// The borders and scrollbars are not included.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection,System.Drawing.RectangleF,System.Single,System.Boolean)" />
		public virtual Bitmap GetBitmap()
		{
			lock (typeof(GoView))
			{
				bool flag = myIsRenderingBitmap;
				myIsRenderingBitmap = true;
				Rectangle displayRectangle = DisplayRectangle;
				Bitmap bitmap = new Bitmap(displayRectangle.Width, displayRectangle.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.PageUnit = GraphicsUnit.Pixel;
				graphics.ScaleTransform(DocScale, DocScale);
				PointF docPosition = DocPosition;
				graphics.TranslateTransform(0f - docPosition.X, 0f - docPosition.Y);
				RectangleF rectangleF = new RectangleF(docPosition, ConvertViewToDoc(displayRectangle.Size));
				graphics.SetClip(rectangleF);
				UpdateDelayedSelectionHandles();
				PaintView(graphics, rectangleF);
				graphics.Dispose();
				myIsRenderingBitmap = flag;
				return bitmap;
			}
		}

		/// <summary>
		/// Paint all of the objects of this view or its document that are visible in
		/// the given rectangle.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="clipRect">A <c>RectangleF</c> in document coordinates.</param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoView.PaintPaperColor(System.Drawing.Graphics,System.Drawing.RectangleF)" />,
		/// <see cref="M:Northwoods.Go.GoView.PaintBackgroundDecoration(System.Drawing.Graphics,System.Drawing.RectangleF)" />, and
		/// <see cref="M:Northwoods.Go.GoView.PaintObjects(System.Boolean,System.Boolean,System.Drawing.Graphics,System.Drawing.RectangleF)" /> for both document and view objects.
		/// </remarks>
		protected virtual void PaintView(Graphics g, RectangleF clipRect)
		{
			PaintPaperColor(g, clipRect);
			PaintBackgroundDecoration(g, clipRect);
			g.SmoothingMode = SmoothingMode;
			g.TextRenderingHint = TextRenderingHint;
			g.InterpolationMode = InterpolationMode;
			g.CompositingQuality = CompositingQuality;
			g.PixelOffsetMode = PixelOffsetMode;
			PaintObjects(doc: true, view: true, g, clipRect);
		}

		/// <summary>
		/// Fill in the document paper color or view background color.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="clipRect"></param>
		/// <remarks>
		/// If this view's document's <see cref="P:Northwoods.Go.GoDocument.PaperColor" /> is
		/// <c>Color.Empty</c> we use this view's <c>BackColor</c> instead.
		/// However, if the <see cref="P:Northwoods.Go.GoView.SheetStyle" /> is not <see cref="F:Northwoods.Go.GoViewSheetStyle.None" />,
		/// this method just fills in this control's <c>BackColor</c>, because
		/// the <see cref="P:Northwoods.Go.GoDocument.PaperColor" /> is used for the <see cref="P:Northwoods.Go.GoView.Sheet" /> instead.
		/// </remarks>
		protected virtual void PaintPaperColor(Graphics g, RectangleF clipRect)
		{
			Color color = (SheetStyle == GoViewSheetStyle.None) ? Document.PaperColor : Color.Empty;
			if (color == Color.Empty)
			{
				color = BackColor;
			}
			if (myBackgroundBrush == null || myBackgroundBrush.Color != color)
			{
				if (myBackgroundBrush != null)
				{
					myBackgroundBrush.Dispose();
				}
				myBackgroundBrush = new SolidBrush(color);
			}
			g.FillRectangle(myBackgroundBrush, clipRect);
		}

		/// <summary>
		/// Draw any decoration that should appear behind all of the objects.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="clipRect"></param>
		/// <remarks>
		/// By default this method draws this <c>Control</c>'s <c>BackgroundImage</c>, if any.
		/// </remarks>
		protected virtual void PaintBackgroundDecoration(Graphics g, RectangleF clipRect)
		{
			Image backgroundImage = BackgroundImage;
			if (backgroundImage != null)
			{
				RectangleF rectangleF = clipRect;
				rectangleF.Width = Math.Min(rectangleF.Width, 32767f);
				rectangleF.Height = Math.Min(rectangleF.Height, 32767f);
				g.DrawImage(backgroundImage, rectangleF, rectangleF, GraphicsUnit.Pixel);
			}
		}

		private void UpdateDelayedSelectionHandles()
		{
			if (myUpdateHandles != null)
			{
				GoSelection selection = Selection;
				foreach (GoObject myUpdateHandle in myUpdateHandles)
				{
					if (selection.GetHandleCount(myUpdateHandle) > 0)
					{
						if (myUpdateHandle.CanView())
						{
							IGoHandle anExistingHandle = selection.GetAnExistingHandle(myUpdateHandle);
							myUpdateHandle.AddSelectionHandles(selection, anExistingHandle.SelectedObject);
						}
						else
						{
							myUpdateHandle.RemoveSelectionHandles(selection);
						}
					}
				}
				myUpdateHandles.Clear();
			}
		}

		/// <summary>
		/// Paint all the document and/or view objects.
		/// </summary>
		/// <param name="doc">If true, paint document objects.</param>
		/// <param name="view">If true, paint view objects.</param>
		/// <param name="g"></param>
		/// <param name="clipRect"></param>
		/// <seealso cref="M:Northwoods.Go.GoLayer.Paint(System.Drawing.Graphics,Northwoods.Go.GoView,System.Drawing.RectangleF)" />
		protected virtual void PaintObjects(bool doc, bool view, Graphics g, RectangleF clipRect)
		{
			foreach (GoLayer layer in Layers)
			{
				if ((doc && layer.IsInDocument) || (view && layer.IsInView))
				{
					layer.Paint(g, this, clipRect);
				}
			}
		}

		/// <summary>
		/// Call the <c>Control.Focus()</c> method, catching any
		/// <c>SecurityException</c> to make sure <see cref="M:Northwoods.Go.GoView.OnGotFocus(System.EventArgs)" /> is called anyway.
		/// </summary>
		public virtual void RequestFocus()
		{
			try
			{
				RequestFocus2();
			}
			catch (VerificationException ex)
			{
				GoObject.Trace("Focus: " + ex.ToString());
				OnGotFocus(EventArgs.Empty);
			}
			catch (SecurityException ex2)
			{
				GoObject.Trace("Focus: " + ex2.ToString());
				OnGotFocus(EventArgs.Empty);
			}
		}

		private void RequestFocus2()
		{
			Focus();
		}

		/// <summary>
		/// When a view gets focus, make sure the selection appearance can be updated, if needed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnGotFocus(EventArgs evt)
		{
			base.OnGotFocus(evt);
			CleanUpModalControl();
			if (Selection != null)
			{
				Selection.OnGotFocus();
			}
		}

		/// <summary>
		/// When a view loses focus, make sure the selection appearance can be updated, if needed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnLostFocus(EventArgs evt)
		{
			base.OnLostFocus(evt);
			if (Selection != null)
			{
				Selection.OnLostFocus();
			}
		}

		/// <summary>
		/// Cause the whole view, including scroll bars, to be redrawn.
		/// </summary>
		/// <remarks>
		/// This calls <c>Invalidate()</c> after calling <see cref="M:Northwoods.Go.GoView.UpdateScrollBars" />.
		/// This does not call <see cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" /> to resize/reposition
		/// any bars or corner controls.
		/// Nor does this call <see cref="M:Northwoods.Go.GoView.UpdateExtent" /> to possibly scroll or rescale the view.
		/// </remarks>
		public virtual void UpdateView()
		{
			UpdateBorderWidths();
			UpdateScrollBars();
			Invalidate();
		}

		/// <summary>
		/// Disable painting of this view.
		/// </summary>
		/// <remarks>
		/// This must be followed by a call to <see cref="M:Northwoods.Go.GoView.EndUpdate" />.
		/// Pairs of calls to <c>BeginUpdate()</c> and <c>EndUpdate()</c> can be nested.
		/// </remarks>
		public virtual void BeginUpdate()
		{
			checked
			{
				mySuppressPaint++;
			}
		}

		/// <summary>
		/// Re-enable painting of this view.
		/// </summary>
		/// <remarks>
		/// This must be preceded by a call to <see cref="M:Northwoods.Go.GoView.BeginUpdate" />.
		/// Pairs of calls to <c>BeginUpdate()</c> and <c>EndUpdate()</c> can be nested.
		/// The outermost/final call to this method will call <see cref="M:Northwoods.Go.GoView.UpdateView" />,
		/// which will invalidate the whole view to cause it to be repainted later.
		/// </remarks>
		public virtual void EndUpdate()
		{
			checked
			{
				if (mySuppressPaint > 0)
				{
					mySuppressPaint--;
					UpdateView();
				}
			}
		}

		/// <summary>
		/// Redraw the whole view if the background color has changed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnBackColorChanged(EventArgs evt)
		{
			base.OnBackColorChanged(evt);
			UpdateView();
		}

		/// <summary>
		/// Redraw the whole view if the background image has changed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnBackgroundImageChanged(EventArgs evt)
		{
			base.OnBackgroundImageChanged(evt);
			UpdateView();
		}

		/// <summary>
		/// Redraw the whole view if the Control style has changed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnStyleChanged(EventArgs evt)
		{
			base.OnStyleChanged(evt);
			UpdateView();
		}

		/// <summary>
		/// Redraw the whole view if the system colors have changed.
		/// </summary>
		/// <param name="evt"></param>
		protected override void OnSystemColorsChanged(EventArgs evt)
		{
			base.OnSystemColorsChanged(evt);
			UpdateView();
		}

		/// <summary>
		/// Update the scroll bars for this view, changing the minimum/maximum/value
		/// and visibility as appropriate.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoView.LayoutScrollBars(System.Boolean)" />
		public virtual void UpdateScrollBars()
		{
			if (myUpdatingScrollBars)
			{
				return;
			}
			HScrollBar horizontalScrollBar = HorizontalScrollBar;
			VScrollBar verticalScrollBar = VerticalScrollBar;
			if (verticalScrollBar == null && horizontalScrollBar == null)
			{
				return;
			}
			PointF documentTopLeft = DocumentTopLeft;
			SizeF documentSize = DocumentSize;
			checked
			{
				int num = (int)Math.Floor(documentTopLeft.X * myHorizScale * myHorizWorld);
				int num2 = (int)Math.Floor(documentTopLeft.Y * myVertScale * myVertWorld);
				int num3 = (int)Math.Floor((documentTopLeft.X + documentSize.Width) * myHorizScale * myHorizWorld);
				int num4 = (int)Math.Floor((documentTopLeft.Y + documentSize.Height) * myVertScale * myVertWorld);
				PointF docPosition = DocPosition;
				int num5 = (int)Math.Floor(docPosition.X * myHorizScale * myHorizWorld);
				int num6 = (int)Math.Floor(docPosition.Y * myVertScale * myVertWorld);
				Size size = base.Size;
				size.Width -= 2 * myBorderSize.Width;
				if (size.Width < 0)
				{
					size.Width = 0;
				}
				size.Height -= 2 * myBorderSize.Height;
				if (size.Height < 0)
				{
					size.Height = 0;
				}
				bool flag = num4 - num2 > size.Height || num6 > num2 || num6 < num4 - size.Height;
				bool flag2 = verticalScrollBar != null && (ShowVerticalScrollBar == GoViewScrollBarVisibility.Show || unchecked(ShowVerticalScrollBar == GoViewScrollBarVisibility.IfNeeded && flag));
				bool flag3 = num3 - num > size.Width || num5 > num || num5 < num3 - size.Width;
				bool flag4 = horizontalScrollBar != null && (ShowHorizontalScrollBar == GoViewScrollBarVisibility.Show || unchecked(ShowHorizontalScrollBar == GoViewScrollBarVisibility.IfNeeded && flag3));
				if (flag2)
				{
					if (verticalScrollBar != null)
					{
						size.Width -= verticalScrollBar.Width;
					}
					size.Width = Math.Max(0, size.Width);
				}
				if (flag4)
				{
					if (horizontalScrollBar != null)
					{
						size.Height -= horizontalScrollBar.Height;
					}
					size.Height = Math.Max(0, size.Height);
				}
				flag = (num4 - num2 > size.Height || num6 > num2 || num6 < num4 - size.Height);
				flag2 = (verticalScrollBar != null && (ShowVerticalScrollBar == GoViewScrollBarVisibility.Show || unchecked(ShowVerticalScrollBar == GoViewScrollBarVisibility.IfNeeded && flag)));
				flag3 = (num3 - num > size.Width || num5 > num || num5 < num3 - size.Width);
				flag4 = (horizontalScrollBar != null && (ShowHorizontalScrollBar == GoViewScrollBarVisibility.Show || unchecked(ShowHorizontalScrollBar == GoViewScrollBarVisibility.IfNeeded && flag3)));
				myUpdatingScrollBars = true;
				bool flag5 = false;
				if (verticalScrollBar != null)
				{
					int num7 = num4 - size.Height;
					if (num6 > num7 && num7 > num2)
					{
						num6 = num7;
					}
					if (num6 < num2)
					{
						num6 = num2;
					}
					int val = Math.Max(num4, num6 + size.Height) - 12;
					val = Math.Max(val, num6);
					if (verticalScrollBar.Minimum != num2)
					{
						verticalScrollBar.Minimum = num2;
					}
					if (verticalScrollBar.Value > val && verticalScrollBar.Value != num6)
					{
						verticalScrollBar.Value = num6;
					}
					if (verticalScrollBar.Maximum != val)
					{
						verticalScrollBar.Maximum = val;
					}
					if (verticalScrollBar.Value != num6)
					{
						verticalScrollBar.Value = num6;
					}
					if (verticalScrollBar.Visible != flag2)
					{
						flag5 = true;
					}
					verticalScrollBar.Visible = flag2;
					verticalScrollBar.Enabled = flag;
				}
				if (horizontalScrollBar != null)
				{
					int num8 = num3 - size.Width;
					if (num5 > num8 && num8 > num)
					{
						num5 = num8;
					}
					if (num5 < num)
					{
						num5 = num;
					}
					int val2 = Math.Max(num3, num5 + size.Width) - 12;
					val2 = Math.Max(val2, num5);
					if (horizontalScrollBar.Minimum != num)
					{
						horizontalScrollBar.Minimum = num;
					}
					if (horizontalScrollBar.Value > val2 && horizontalScrollBar.Value != num5)
					{
						horizontalScrollBar.Value = num5;
					}
					if (horizontalScrollBar.Maximum != val2)
					{
						horizontalScrollBar.Maximum = val2;
					}
					if (horizontalScrollBar.Value != num5)
					{
						horizontalScrollBar.Value = num5;
					}
					if (horizontalScrollBar.Visible != flag4)
					{
						flag5 = true;
					}
					horizontalScrollBar.Visible = flag4;
					horizontalScrollBar.Enabled = flag3;
				}
				myUpdatingScrollBars = false;
				if (flag5)
				{
					LayoutScrollBars(update: false);
				}
			}
		}

		/// <summary>
		/// This is the event handler for both scroll bars.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">
		/// This is a <c>ScrollEventArgs</c>
		/// </param>
		/// <remarks>
		/// This method sets the <see cref="P:Northwoods.Go.GoView.DocPosition" /> property according to
		/// the new value.
		/// </remarks>
		public virtual void HandleScroll(object sender, ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
			{
				int newValue = e.NewValue;
				RequestFocus();
				PointF docPosition = DocPosition;
				SizeF sizeF = ConvertViewToDoc(new Size(newValue, newValue));
				if (sender == VerticalScrollBar)
				{
					docPosition.Y = sizeF.Height;
					DocPosition = docPosition;
				}
				else if (sender == HorizontalScrollBar)
				{
					docPosition.X = sizeF.Width;
					DocPosition = docPosition;
				}
			}
		}

		/// <summary>
		/// Handle changes to any view layers or view objects.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="subhint"></param>
		/// <param name="x"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// By default this method just invalidates part or all of this view
		/// whenever an object is modified, inserted, or removed from a view layer,
		/// or when a view layer is modified, added, or removed.
		/// </remarks>
		public virtual void RaiseChanged(int hint, int subhint, object x, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			if (hint <= 904)
			{
				switch (hint)
				{
				case 901:
				case 904:
				{
					if (SuppressingPaint)
					{
						break;
					}
					GoObject goObject = x as GoObject;
					if (goObject == null)
					{
						break;
					}
					RectangleF bounds = goObject.Bounds;
					bounds = goObject.ExpandPaintBounds(bounds, this);
					Rectangle rectangle = ConvertDocToView(bounds);
					rectangle.Inflate(2, 2);
					if (hint == 901 && subhint == 1001)
					{
						oldRect = goObject.ExpandPaintBounds(oldRect, this);
						Rectangle rectangle2 = ConvertDocToView(oldRect);
						rectangle2.Inflate(2, 2);
						if (rectangle.IntersectsWith(rectangle2))
						{
							Invalidate(Rectangle.Union(rectangle, rectangle2));
							break;
						}
						Invalidate(rectangle);
						Invalidate(rectangle2);
					}
					else
					{
						Invalidate(rectangle);
					}
					break;
				}
				case 902:
					if (!SuppressingPaint)
					{
						GoObject goObject2 = x as GoObject;
						if (goObject2 != null)
						{
							RectangleF bounds2 = goObject2.Bounds;
							bounds2 = goObject2.ExpandPaintBounds(bounds2, this);
							Rectangle rc = ConvertDocToView(bounds2);
							rc.Inflate(2, 2);
							Invalidate(rc);
						}
					}
					break;
				case 903:
					if (!SuppressingPaint)
					{
						GoObject goObject3 = x as GoObject;
						if (goObject3 != null)
						{
							RectangleF bounds3 = goObject3.Bounds;
							bounds3 = goObject3.ExpandPaintBounds(bounds3, this);
							Rectangle rc2 = ConvertDocToView(bounds3);
							rc2.Inflate(2, 2);
							Invalidate(rc2);
						}
					}
					break;
				case 801:
				case 803:
					UpdateView();
					break;
				case 802:
				{
					GoLayer goLayer = x as GoLayer;
					if (goLayer != null)
					{
						RemoveAllGoControls(goLayer, remove: true);
					}
					UpdateView();
					break;
				}
				}
			}
			else if (hint != 910)
			{
				_ = 930;
			}
			else
			{
				GoLayer goLayer2 = x as GoLayer;
				if (goLayer2 != null && newVal == (object)false)
				{
					RemoveAllGoControls(goLayer2, remove: false);
				}
				UpdateView();
			}
		}

		/// <summary>
		/// Call all <see cref="E:Northwoods.Go.GoView.PropertyChanged" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This also calls <see cref="M:Northwoods.Go.GoView.UpdateView" />, unless the property is
		/// known to be a minor one.
		/// If you override this method, be sure to call the base method too.
		/// This is called by <see cref="M:Northwoods.Go.GoView.RaisePropertyChangedEvent(System.String)" />
		/// </remarks>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs evt)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, evt);
			}
			if (evt.PropertyName != "Tool")
			{
				UpdateView();
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.PropertyChanged" /> event for the given property name.
		/// </summary>
		/// <param name="propname"></param>
		/// <remarks>
		/// This just calls <see cref="M:Northwoods.Go.GoView.OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs)" />.
		/// </remarks>
		public void RaisePropertyChangedEvent(string propname)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propname));
		}

		/// <summary>
		/// Handle changes to the view's document or any of the document's layers or objects.
		/// </summary>
		/// <param name="sender">A <see cref="T:Northwoods.Go.GoDocument" />.</param>
		/// <param name="e">A <see cref="T:Northwoods.Go.GoChangedEventArgs" /> describing the change.</param>
		/// <remarks>
		/// <para>
		/// By default this method just invalidates part or all of this view
		/// whenever an object is modified, inserted, or removed from a layer in this view's
		/// document, or when a document layer is modified, added, or removed, or when a
		/// document is changed.
		/// It also passes the event args on to all <see cref="E:Northwoods.Go.GoView.DocumentChanged" /> event
		/// handlers registered with this view.
		/// </para>
		/// <para>
		/// When the <see cref="P:Northwoods.Go.GoView.Document" />'s <see cref="P:Northwoods.Go.GoDocument.PaperColor" /> changes,
		/// we also try to update the brush of the <see cref="P:Northwoods.Go.GoView.Sheet" />'s <see cref="P:Northwoods.Go.GoSheet.Paper" />.
		/// </para>
		/// </remarks>
		protected virtual void OnDocumentChanged(object sender, GoChangedEventArgs e)
		{
			GoObject goObject = e.GoObject;
			checked
			{
				if (e.IsBeforeChanging)
				{
					if (e.Hint == 901 && !SuppressingPaint && goObject != null)
					{
						RectangleF bounds = goObject.Bounds;
						bounds = goObject.ExpandPaintBounds(bounds, this);
						Rectangle rc = ConvertDocToView(bounds);
						rc.Inflate(2, 2);
						Invalidate(rc);
					}
				}
				else
				{
					switch (e.Hint)
					{
					case 901:
						if (!SuppressingPaint)
						{
							RectangleF bounds3 = goObject.Bounds;
							bounds3 = goObject.ExpandPaintBounds(bounds3, this);
							Rectangle rc3 = ConvertDocToView(bounds3);
							rc3.Inflate(2, 2);
							Invalidate(rc3);
						}
						if (e.SubHint == 1001)
						{
							if (Selection.GetHandleCount(goObject) > 0 && !myUpdateHandles.Contains(goObject))
							{
								myUpdateHandles.Add(goObject);
							}
							if (!SuppressingPaint)
							{
								RectangleF oldRect = e.OldRect;
								oldRect = goObject.ExpandPaintBounds(oldRect, this);
								Rectangle rc4 = ConvertDocToView(oldRect);
								rc4.Inflate(2, 2);
								Invalidate(rc4);
							}
						}
						else if (e.SubHint == 1204 || e.SubHint == 1412)
						{
							if (Selection.GetHandleCount(goObject) > 0 && !myUpdateHandles.Contains(goObject))
							{
								myUpdateHandles.Add(goObject);
							}
						}
						else if (e.SubHint == 1003)
						{
							updateSelectionHandles(goObject);
						}
						else if (e.SubHint == 1052)
						{
							removeFromSelection(e.OldValue as GoObject);
						}
						break;
					case 902:
					case 903:
					case 904:
					case 905:
						if (e.Hint == 903)
						{
							removeFromSelection(goObject);
						}
						if (!SuppressingPaint)
						{
							RectangleF bounds2 = goObject.Bounds;
							bounds2 = goObject.ExpandPaintBounds(bounds2, this);
							Rectangle rc2 = ConvertDocToView(bounds2);
							rc2.Inflate(2, 2);
							Invalidate(rc2);
						}
						break;
					case 202:
						UpdateScrollBars();
						break;
					case 203:
						UpdateScrollBars();
						break;
					case 227:
					{
						GoDocument goDocument = sender as GoDocument;
						if (goDocument != null)
						{
							SizeF worldScale = goDocument.WorldScale;
							myHorizWorld = worldScale.Width;
							myVertWorld = worldScale.Height;
							UpdateView();
						}
						break;
					}
					case 103:
						Update();
						break;
					case 101:
						BeginUpdate();
						break;
					case 102:
						EndUpdate();
						break;
					case 801:
					{
						GoLayer doclayer = (GoLayer)e.Object;
						GoLayer dest = (GoLayer)e.OldValue;
						if (e.SubHint == 1)
						{
							Layers.InsertDocumentLayerAfter(dest, doclayer);
						}
						else
						{
							Layers.InsertDocumentLayerBefore(dest, doclayer);
						}
						Selection.AddAllSelectionHandles();
						UpdateView();
						break;
					}
					case 802:
					{
						GoLayer layer = (GoLayer)e.Object;
						Layers.Remove(layer);
						RemoveAllGoControls(layer, remove: true);
						Selection.AddAllSelectionHandles();
						UpdateView();
						break;
					}
					case 803:
					{
						GoLayer moving = (GoLayer)e.OldValue;
						int newInt = e.NewInt;
						if (newInt <= 0)
						{
							GoLayer dest2 = Document.Layers.LayerAt(newInt + 1);
							Layers.MoveBefore(dest2, moving);
						}
						else
						{
							GoLayer dest3 = Document.Layers.LayerAt(newInt - 1);
							Layers.MoveAfter(dest3, moving);
						}
						UpdateView();
						break;
					}
					case 910:
					{
						GoLayer goLayer = e.Object as GoLayer;
						if (goLayer != null && e.NewValue == (object)false)
						{
							RemoveAllGoControls(goLayer, remove: false);
						}
						Selection.AddAllSelectionHandles();
						UpdateView();
						break;
					}
					case 100:
					case 108:
					case 110:
					case 220:
						Selection.AddAllSelectionHandles();
						UpdateView();
						break;
					case 205:
					{
						GoSheet sheet = Sheet;
						if (sheet != null && sheet.Paper != null)
						{
							Color color = Document.PaperColor;
							if (color == Color.Empty)
							{
								color = Color.White;
							}
							sheet.Paper.BrushColor = color;
						}
						UpdateView();
						break;
					}
					}
				}
				if (this.DocumentChanged != null)
				{
					this.DocumentChanged(sender, e);
				}
			}
		}

		internal void SafeOnDocumentChanged(object sender, GoChangedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				if (mySafeOnDocumentChangedDelegate == null)
				{
					mySafeOnDocumentChangedDelegate = InternalOnDocumentChanged;
				}
				if (myQueuedEvents == null)
				{
					myQueuedEvents = new Queue<GoChangedEventArgs>();
				}
				GoChangedEventArgs item = new GoChangedEventArgs(e);
				lock (myQueuedEvents)
				{
					myQueuedEvents.Enqueue(item);
				}
				Invoke(mySafeOnDocumentChangedDelegate);
			}
			else
			{
				OnDocumentChanged(sender, e);
			}
		}

		private void InternalOnDocumentChanged(object sender, EventArgs e)
		{
			if (myQueuedEvents != null)
			{
				GoChangedEventArgs goChangedEventArgs = null;
				lock (myQueuedEvents)
				{
					goChangedEventArgs = myQueuedEvents.Dequeue();
				}
				if (goChangedEventArgs != null)
				{
					OnDocumentChanged(goChangedEventArgs.Document, goChangedEventArgs);
				}
			}
		}

		private void updateSelectionHandles(GoObject obj)
		{
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					updateSelectionHandles(item);
				}
			}
			GoObject selectionObject = obj.SelectionObject;
			if (selectionObject != null)
			{
				GoSelection selection = Selection;
				if (selection.Contains(obj) && obj.CanView())
				{
					selectionObject.AddSelectionHandles(selection, obj);
				}
				else
				{
					selectionObject.RemoveSelectionHandles(selection);
				}
			}
		}

		private void removeFromSelection(GoObject obj)
		{
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					removeFromSelection(item);
				}
			}
			Selection.Remove(obj);
		}

		/// <summary>
		/// When this view has an <see cref="P:Northwoods.Go.GoView.EditControl" /> that is showing
		/// a <c>Control</c> that has focus, return false to permit that Control
		/// to process all keys.
		/// </summary>
		/// <param name="m"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		/// <remarks>
		/// When the user is editing text in-place with a Control such as a TextBox,
		/// let that Control handle keys such as "Del" and "Ctrl-V" instead of
		/// interpreting them as accelerators for menu commands and ignoring them
		/// because the view doesn't have focus.
		/// </remarks>
		protected override bool ProcessCmdKey(ref Message m, Keys keyData)
		{
			GoControl editControl = EditControl;
			if (editControl != null)
			{
				Control control = editControl.GetControl(this);
				if (control != null && control.Focused)
				{
					return false;
				}
			}
			return base.ProcessCmdKey(ref m, keyData);
		}

		/// <summary>
		/// Let this view handle the arrow keys.
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		protected override bool IsInputKey(Keys k)
		{
			if (k == Keys.Down || k == Keys.Up || k == Keys.Left || k == Keys.Right)
			{
				GoViewDisableKeys disableKeys = DisableKeys;
				if ((disableKeys & GoViewDisableKeys.ArrowMove) == 0 || (disableKeys & GoViewDisableKeys.ArrowScroll) == 0)
				{
					return true;
				}
			}
			return base.IsInputKey(k);
		}

		/// <summary>
		/// This method is the view's key event handler.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method makes sure the <see cref="P:Northwoods.Go.GoView.LastInput" /> property value
		/// has up-to-date information describing this keyboard input event.
		/// It then calls <see cref="M:Northwoods.Go.GoView.DoKeyDown" />, and then finally calls
		/// the base method to invoke all of the <c>KeyDown</c> event handlers.
		/// </remarks>
		protected override void OnKeyDown(KeyEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowKey)
			{
				lastInput.Buttons = MouseButtons.None;
				lastInput.Modifiers = evt.Modifiers;
				lastInput.Delta = 0;
				lastInput.Key = evt.KeyCode;
				lastInput.KeyEventArgs = evt;
				lastInput.InputState = GoInputState.Start;
				DoKeyDown();
			}
			base.OnKeyDown(evt);
			lastInput.KeyEventArgs = null;
		}

		/// <summary>
		/// Handle a canonicalized keyboard input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a keyboard input event.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoKeyDown" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.OnKeyDown(System.Windows.Forms.KeyEventArgs)" /> and any
		/// other code that wishes to simulate a canonicalized keyboard event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowKey" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoKeyDown()
		{
			Tool.DoKeyDown();
		}

		/// <summary>
		/// This method is the view's mouse down event handler.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method makes sure the <see cref="P:Northwoods.Go.GoView.LastInput" /> and
		/// <see cref="P:Northwoods.Go.GoView.FirstInput" /> canonicalized input property values
		/// have up-to-date information describing this mouse input event.
		/// It then calls <see cref="M:Northwoods.Go.GoView.DoMouseDown" /> to allow tools
		/// to handle the input event, and then finally calls
		/// the base method to invoke all of the <c>MouseDown</c> event handlers.
		/// </remarks>
		protected override void OnMouseDown(MouseEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				lastInput.ViewPoint = new Point(evt.X, evt.Y);
				lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
				lastInput.Buttons = evt.Button;
				lastInput.Modifiers = Control.ModifierKeys;
				lastInput.Delta = evt.Delta;
				lastInput.Key = Keys.None;
				lastInput.MouseEventArgs = evt;
				lastInput.InputState = GoInputState.Start;
				GoInputEventArgs firstInput = FirstInput;
				firstInput.ViewPoint = lastInput.ViewPoint;
				firstInput.DocPoint = lastInput.DocPoint;
				firstInput.Buttons = lastInput.Buttons;
				firstInput.Modifiers = lastInput.Modifiers;
				firstInput.Delta = lastInput.Delta;
				firstInput.Key = lastInput.Key;
				firstInput.MouseEventArgs = evt;
				firstInput.InputState = GoInputState.Start;
				DoMouseDown();
			}
			base.OnMouseDown(evt);
			lastInput.MouseEventArgs = null;
			FirstInput.MouseEventArgs = null;
		}

		/// <summary>
		/// This method is the view's mouse move event handler.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method makes sure the <see cref="P:Northwoods.Go.GoView.LastInput" /> property value
		/// has up-to-date information describing this mouse input event.
		/// It then calls <see cref="M:Northwoods.Go.GoView.DoMouseMove" /> to allow tools
		/// to handle the input event, and then finally calls
		/// the base method to invoke all of the <c>MouseMove</c> event handlers.
		/// </remarks>
		protected override void OnMouseMove(MouseEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			Point viewPoint = new Point(evt.X, evt.Y);
			if (AllowMouse)
			{
				lastInput.ViewPoint = viewPoint;
				lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
				lastInput.Buttons = evt.Button;
				lastInput.Modifiers = Control.ModifierKeys;
				lastInput.Delta = evt.Delta;
				lastInput.Key = Keys.None;
				lastInput.MouseEventArgs = evt;
				lastInput.InputState = GoInputState.Continue;
				DoMouseMove();
			}
			base.OnMouseMove(evt);
			lastInput.MouseEventArgs = null;
		}

		/// <summary>
		/// This method is the view's mouse up event handler.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method makes sure the <see cref="P:Northwoods.Go.GoView.LastInput" /> property value
		/// has up-to-date information describing this mouse input event.
		/// It then calls <see cref="M:Northwoods.Go.GoView.DoMouseUp" /> to allow tools
		/// to handle the input event, and then finally calls
		/// the base method to invoke all of the <c>MouseUp</c> event handlers.
		/// </remarks>
		protected override void OnMouseUp(MouseEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				lastInput.ViewPoint = new Point(evt.X, evt.Y);
				lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
				lastInput.Buttons = evt.Button;
				lastInput.Modifiers = Control.ModifierKeys;
				lastInput.Delta = evt.Delta;
				lastInput.Key = Keys.None;
				lastInput.MouseEventArgs = evt;
				lastInput.InputState = GoInputState.Finish;
				DoMouseUp();
			}
			base.OnMouseUp(evt);
			lastInput.MouseEventArgs = null;
		}

		/// <summary>
		/// For a double-click, this method gets called instead of a second OnMouseUp.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This is another way the view produces a mouse-up canonicalized input event.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.OnMouseUp(System.Windows.Forms.MouseEventArgs)" />
		/// <seealso cref="M:Northwoods.Go.GoView.DoMouseUp" />
		protected override void OnDoubleClick(EventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				lastInput.MouseEventArgs = new MouseEventArgs(lastInput.Buttons, 2, lastInput.ViewPoint.X, lastInput.ViewPoint.Y, lastInput.Delta);
				lastInput.DoubleClick = true;
				lastInput.InputState = GoInputState.Finish;
				DoMouseUp();
			}
			base.OnDoubleClick(evt);
			lastInput.DoubleClick = false;
			lastInput.MouseEventArgs = null;
		}

		/// <summary>
		/// This is the mouse wheel event handler, that handles scrolling and zooming.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method makes sure the <see cref="P:Northwoods.Go.GoView.LastInput" /> property value
		/// has up-to-date information describing this mouse input event.
		/// It then calls <see cref="M:Northwoods.Go.GoView.DoMouseWheel" /> to allow tools
		/// to handle the input event, and then finally calls
		/// the base method to invoke all of the <c>MouseWheel</c> event handlers.
		/// </remarks>
		protected override void OnMouseWheel(MouseEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				lastInput.ViewPoint = new Point(evt.X, evt.Y);
				lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
				lastInput.Buttons = evt.Button;
				lastInput.Modifiers = Control.ModifierKeys;
				lastInput.Delta = evt.Delta;
				lastInput.Key = Keys.None;
				lastInput.MouseEventArgs = evt;
				lastInput.InputState = GoInputState.Start;
				DoMouseWheel();
			}
			base.OnMouseWheel(evt);
			lastInput.MouseEventArgs = null;
		}

		/// <summary>
		/// Handle a canonicalized mouse down input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a mouse down input event.
		/// We also assume that <see cref="P:Northwoods.Go.GoView.FirstInput" /> has a copy
		/// of the canonicalized input event.
		/// By default this just gets focus and calls <see cref="M:Northwoods.Go.IGoTool.DoMouseDown" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.OnMouseDown(System.Windows.Forms.MouseEventArgs)" /> and any
		/// other code that wishes to simulate a canonicalized mouse down event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowMouse" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoMouseDown()
		{
			bool focused = Focused;
			RequestFocus();
			if (focused || !myCancelMouseDown)
			{
				Tool.DoMouseDown();
			}
			myCancelMouseDown = false;
		}

		/// <summary>
		/// Handle a canonicalized mouse move input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a mouse move input event.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoMouseMove" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.OnMouseMove(System.Windows.Forms.MouseEventArgs)" /> and any
		/// other code that wishes to simulate a canonicalized mouse move event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowMouse" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoMouseMove()
		{
			Tool.DoMouseMove();
		}

		/// <summary>
		/// Handle a canonicalized mouse up input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a mouse up input event.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoMouseUp" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.OnMouseUp(System.Windows.Forms.MouseEventArgs)" /> and any
		/// other code that wishes to simulate a canonicalized mouse up event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowMouse" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoMouseUp()
		{
			Tool.DoMouseUp();
		}

		/// <summary>
		/// Handle a canonicalized mouse wheel input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assume <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing an input event describing the rotation of the
		/// mouse wheel.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoMouseWheel" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.OnMouseWheel(System.Windows.Forms.MouseEventArgs)" /> and any
		/// other code that wishes to simulate a canonicalized mouse wheel event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowMouse" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoMouseWheel()
		{
			Tool.DoMouseWheel();
		}

		/// <summary>
		/// Handle a canonicalized mouse hover input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a mouse-hovering-somewhere event.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoMouseHover" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <see cref="M:Northwoods.Go.GoView.DetectHover(System.Drawing.Point)" /> and any
		/// other code that wishes to simulate a canonicalized mouse hover event.
		/// This is not called when <see cref="P:Northwoods.Go.GoView.AllowMouse" /> is false.
		/// </para>
		/// </remarks>
		public virtual void DoMouseHover()
		{
			Tool.DoMouseHover();
		}

		/// <summary>
		/// Handle a canonicalized cancel input event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method assumes <see cref="P:Northwoods.Go.GoView.LastInput" /> has information
		/// representing a mouse down input event.
		/// By default this just calls <see cref="M:Northwoods.Go.IGoTool.DoMouseDown" />
		/// on the current <see cref="P:Northwoods.Go.GoView.Tool" />.
		/// </para>
		/// <para>
		/// This is normally called by <c>OnQueryContinueDrag</c>
		/// and most tools when the user types an <c>Escape</c>, and by any
		/// other code that wishes to simulate cancelling a mouse operation.
		/// For example, this is called when the <see cref="P:Northwoods.Go.GoView.Document" />
		/// is changed, to try to clean up any input operation that might
		/// be in progress.
		/// </para>
		/// </remarks>
		public virtual void DoCancelMouse()
		{
			myCancelMouseDown = true;
			Tool.DoCancelMouse();
		}

		/// <summary>
		/// Create an instance of the default <see cref="P:Northwoods.Go.GoView.DefaultTool" /> for this view.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// By default this creates an instance of <see cref="T:Northwoods.Go.GoToolManager" />:
		/// <code>
		///   return new GoToolManager(this);
		/// </code>
		/// </example>
		/// <seealso cref="P:Northwoods.Go.GoView.Tool" />
		public virtual IGoTool CreateDefaultTool()
		{
			return new GoToolManager(this);
		}

		/// <summary>
		/// Replace one of the "mode-less" tools used by this view.
		/// </summary>
		/// <param name="tooltype">the <c>Type</c> of the tool to be replaced;
		/// this should not be a base class of the actual tool instance type</param>
		/// <param name="newtool">the tool to use instead of the existing one of
		/// <c>Type</c> <paramref name="tooltype" />;
		/// if null, the old tool is only removed</param>
		/// <returns>the tool that was replaced, or null if no such instance was found</returns>
		/// <remarks>
		/// When you want to customize an existing "mode-less" tool, and when setting one of its properties
		/// is insufficient, you may need to define your own subclass of that tool or define
		/// your own tool inheriting from <see cref="T:Northwoods.Go.GoTool" />.
		/// In order for the view to use your tool, you'll need to create an instance of
		/// your tool class for the view, and then you can either set <see cref="P:Northwoods.Go.GoView.Tool" />
		/// explicitly, or let <see cref="T:Northwoods.Go.GoToolManager" /> find your tool in one of the mouse tool
		/// lists, such as <see cref="P:Northwoods.Go.GoView.MouseDownTools" />.
		/// For the latter case, you could just add an instance of your tool to one of those lists.
		/// But often you will not want to allow the instance of the original tool class to be used.
		/// This method makes it easy to replace an existing tool with a different one.
		/// This method searches all of the lists of mode-less tools:
		/// <seealso cref="P:Northwoods.Go.GoView.MouseDownTools" />, <seealso cref="P:Northwoods.Go.GoView.MouseMoveTools" />, <seealso cref="P:Northwoods.Go.GoView.MouseUpTools" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.FindMouseTool(System.Type)" />
		/// <example>
		/// You have defined a new subclass of <see cref="T:Northwoods.Go.GoToolLinkingNew" />, called <c>CustomLinkTool</c>.
		/// For each view that you want to use of this new tool instead of the standard way
		/// for users to draw new links, call
		/// <c>aView.ReplaceMouseTool(typeof(GoToolLinkingNew), new CustomLinkTool(aView))</c>
		/// </example>
		public virtual IGoTool ReplaceMouseTool(Type tooltype, IGoTool newtool)
		{
			IList<IGoTool> mouseDownTools = MouseDownTools;
			checked
			{
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype)
					{
						IGoTool result = mouseDownTools[i];
						if (newtool == null)
						{
							mouseDownTools.RemoveAt(i);
							return result;
						}
						mouseDownTools[i] = newtool;
						return result;
					}
				}
				mouseDownTools = MouseMoveTools;
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype)
					{
						IGoTool result2 = mouseDownTools[i];
						if (newtool == null)
						{
							mouseDownTools.RemoveAt(i);
							return result2;
						}
						mouseDownTools[i] = newtool;
						return result2;
					}
				}
				mouseDownTools = MouseUpTools;
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype)
					{
						IGoTool result3 = mouseDownTools[i];
						if (newtool == null)
						{
							mouseDownTools.RemoveAt(i);
							return result3;
						}
						mouseDownTools[i] = newtool;
						return result3;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Find one of the tools used by this view that is an instance of a given tool type.
		/// </summary>
		/// <param name="tooltype"></param>
		/// <returns>null if no mouse tool's type is exactly <paramref name="tooltype" /></returns>
		/// <remarks>
		/// This is most useful when you want to modify one of the standard tools, typically by
		/// setting one of its properties.
		/// This method searches all of the lists of mode-less tools:
		/// <see cref="P:Northwoods.Go.GoView.MouseDownTools" />, <see cref="P:Northwoods.Go.GoView.MouseMoveTools" />, <see cref="P:Northwoods.Go.GoView.MouseUpTools" />.
		/// Note that the class of the tool that is found must match exactly with the
		/// given <paramref name="tooltype" />--it cannot inherit from that type.
		/// This is implemented by calling <c>FindMouseTool(tooltype, false)</c>.
		/// </remarks>
		/// <example>
		/// (myView.FindMouseTool(typeof(GoToolContext)) as GoToolContext).SingleSelection = false;
		/// (myView.FindMouseTool(typeof(GoToolLinkingNew)) as GoToolLinkingNew).ForwardsOnly = true;
		/// </example>
		/// <example>
		/// GoToolDragging dragtool = FindMouseTool(typeof(GoToolDragging)) as GoToolDragging;
		/// if (dragtool != null) {
		///   ... dragtool.ComputeEffectiveSelection(clipsel, false) ...
		/// }
		/// </example>
		/// <seealso cref="M:Northwoods.Go.GoView.FindMouseTool(System.Type,System.Boolean)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ReplaceMouseTool(System.Type,Northwoods.Go.IGoTool)" />
		public IGoTool FindMouseTool(Type tooltype)
		{
			return FindMouseTool(tooltype, subclass: false);
		}

		/// <summary>
		/// Find one of the tools used by this view that is an instance of a given tool type
		/// or of a subclass of that type.
		/// </summary>
		/// <param name="tooltype"></param>
		/// <param name="subclass">true if an instances of subclass of <paramref name="tooltype" /> is acceptable</param>
		/// <returns>null if no mouse tool's type is the same or is a subclass of <paramref name="tooltype" /></returns>
		/// <remarks>
		/// This is most useful when you want to modify one of the standard tools, typically by
		/// setting one of its properties.
		/// This method searches all of the lists of mode-less tools:
		/// <see cref="P:Northwoods.Go.GoView.MouseDownTools" />, <see cref="P:Northwoods.Go.GoView.MouseMoveTools" />, <see cref="P:Northwoods.Go.GoView.MouseUpTools" />.
		/// For examples, see <see cref="M:Northwoods.Go.GoView.FindMouseTool(System.Type)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.FindMouseTool(System.Type)" />
		/// <seealso cref="M:Northwoods.Go.GoView.ReplaceMouseTool(System.Type,Northwoods.Go.IGoTool)" />
		public virtual IGoTool FindMouseTool(Type tooltype, bool subclass)
		{
			IList<IGoTool> mouseDownTools = MouseDownTools;
			checked
			{
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype || (subclass && mouseDownTools[i].GetType().IsSubclassOf(tooltype)))
					{
						return mouseDownTools[i];
					}
				}
				mouseDownTools = MouseMoveTools;
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype || (subclass && mouseDownTools[i].GetType().IsSubclassOf(tooltype)))
					{
						return mouseDownTools[i];
					}
				}
				mouseDownTools = MouseUpTools;
				for (int i = 0; i < mouseDownTools.Count; i++)
				{
					if (mouseDownTools[i].GetType() == tooltype || (subclass && mouseDownTools[i].GetType().IsSubclassOf(tooltype)))
					{
						return mouseDownTools[i];
					}
				}
				return null;
			}
		}

		/// <summary>
		/// The <see cref="T:Northwoods.Go.GoToolLinking" /> class calls this method to create a new link between ports.
		/// </summary>
		/// <param name="fromPort">if null, this method will return null</param>
		/// <param name="toPort">if null, this method will return null</param>
		/// <returns></returns>
		/// <remarks>
		/// By default this method creates a copy of the <see cref="P:Northwoods.Go.GoView.NewLinkPrototype" /> object,
		/// assigns the <see cref="P:Northwoods.Go.IGoLink.FromPort" /> and <see cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// properties.
		/// How it adds the new link to this view's document depends on the ports.
		/// If both of the ports belong to <see cref="T:Northwoods.Go.GoSubGraphBase" />s, it adds the new link
		/// to the common parent <see cref="T:Northwoods.Go.GoSubGraphBase" /> for both ports.
		/// Otherwise it adds the new link to the document's <see cref="P:Northwoods.Go.GoDocument.LinksLayer" />.
		/// If the new link is <see cref="T:Northwoods.Go.IGoRoutable" />, its <see cref="M:Northwoods.Go.IGoRoutable.UpdateRoute" />
		/// method is called.
		/// The <c>LinkCreated</c> event is <i>not</i> raised here;
		/// it is raised by <see cref="M:Northwoods.Go.GoToolLinking.DoNewLink(Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />.
		/// </remarks>
		public virtual IGoLink CreateLink(IGoPort fromPort, IGoPort toPort)
		{
			if (fromPort == null || toPort == null || fromPort.GoObject == null || toPort.GoObject == null)
			{
				return null;
			}
			if (NewLinkPrototype == null)
			{
				return null;
			}
			IGoLink goLink = Document.CreateCopyDictionary().CopyComplete(NewLinkPrototype) as IGoLink;
			if (goLink != null && goLink.GoObject != null)
			{
				goLink.FromPort = fromPort;
				goLink.ToPort = toPort;
				GoSubGraphBase.ReparentToCommonSubGraph(goLink.GoObject, fromPort.GoObject, toPort.GoObject, behind: true, Document.LinksLayer);
				(goLink as IGoRoutable)?.UpdateRoute();
				return goLink;
			}
			return null;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.LinkCreated" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnLinkCreated(GoSelectionEventArgs evt)
		{
			if (this.LinkCreated != null)
			{
				this.LinkCreated(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnLinkCreated(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise a <see cref="E:Northwoods.Go.GoView.LinkCreated" /> event.
		/// </summary>
		/// <param name="obj"></param>
		public void RaiseLinkCreated(GoObject obj)
		{
			OnLinkCreated(new GoSelectionEventArgs(obj));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.LinkRelinked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnLinkRelinked(GoSelectionEventArgs evt)
		{
			if (this.LinkRelinked != null)
			{
				this.LinkRelinked(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnLinkRelinked(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise a <see cref="E:Northwoods.Go.GoView.LinkRelinked" /> event.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This method is called by <see cref="M:Northwoods.Go.GoToolLinking.DoRelink(Northwoods.Go.IGoLink,Northwoods.Go.IGoPort,Northwoods.Go.IGoPort)" />.
		/// </remarks>
		public void RaiseLinkRelinked(GoObject obj)
		{
			OnLinkRelinked(new GoSelectionEventArgs(obj));
		}

		/// <summary>
		/// Perform the behavior that normally occurs upon a single click.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the single click occured on a document object
		/// whose <see cref="M:Northwoods.Go.GoObject.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// By default this picks the document object at the event's
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.
		/// If an object is found, it raises the <c>ObjectSingleClicked</c> event and
		/// calls <see cref="M:Northwoods.Go.GoObject.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> on the object and
		/// on its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any) until it returns
		/// true.
		/// If no object is found at the event's point, it raises the
		/// <see cref="E:Northwoods.Go.GoView.BackgroundSingleClicked" /> event.
		/// This is normally called from the <see cref="T:Northwoods.Go.GoTool" />.<see cref="M:Northwoods.Go.GoTool.DoClick(Northwoods.Go.GoInputEventArgs)" />
		/// method, which is called by those tools that treat clicks in the
		/// standard fashion.
		/// </remarks>
		public virtual bool DoSingleClick(GoInputEventArgs evt)
		{
			GoObject goObject = PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			if (goObject != null)
			{
				RaiseObjectSingleClicked(goObject, evt);
				while (goObject != null)
				{
					if (goObject.OnSingleClick(evt, this))
					{
						return true;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				RaiseBackgroundSingleClicked(evt);
			}
			return false;
		}

		/// <summary>
		/// Perform the behavior that normally occurs upon a double click.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the double click occured on a document object
		/// whose <see cref="M:Northwoods.Go.GoObject.OnDoubleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// By default this picks the document object at the event's
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.
		/// If an object is found, it raises the <c>ObjectDoubleClicked</c> event and
		/// calls <see cref="M:Northwoods.Go.GoObject.OnDoubleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> on the object and
		/// on its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any) until it returns
		/// true.
		/// If no object is found at the event's point, it raises the
		/// <see cref="E:Northwoods.Go.GoView.BackgroundDoubleClicked" /> event.
		/// This is normally called from the <see cref="T:Northwoods.Go.GoTool" />.<see cref="M:Northwoods.Go.GoTool.DoClick(Northwoods.Go.GoInputEventArgs)" />
		/// method, which is called by those tools that treat clicks in the
		/// standard fashion.
		/// </remarks>
		public virtual bool DoDoubleClick(GoInputEventArgs evt)
		{
			GoObject goObject = PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			if (goObject != null)
			{
				RaiseObjectDoubleClicked(goObject, evt);
				while (goObject != null)
				{
					if (goObject.OnDoubleClick(evt, this))
					{
						return true;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				RaiseBackgroundDoubleClicked(evt);
			}
			return false;
		}

		/// <summary>
		/// Perform the behavior that normally occurs upon a context click.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the context click occured on a document object
		/// whose <see cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// <para>
		/// By default this picks the document object at the event's
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.
		/// If an object is found, it raises the <c>ObjectContextClicked</c> event and
		/// calls <see cref="M:Northwoods.Go.GoObject.OnContextClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> on the object and
		/// on its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any) until it returns
		/// true.
		/// </para>
		/// <para>
		/// If no object is found at the event's point, it raises the
		/// <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" /> event.
		/// </para>
		/// <para>
		/// This is normally called from a context mouse button handling tool.
		/// </para>
		/// </remarks>
		public virtual bool DoContextClick(GoInputEventArgs evt)
		{
			if (!this.Document.AllowEdit) return true;//motifyByLzy
			GoObject goObject = PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			if (goObject != null)
			{
				RaiseObjectContextClicked(goObject, evt);
				while (goObject != null)
				{
					ContextMenuStrip contextMenuStrip = goObject.GetContextMenuStrip(this);
					if (contextMenuStrip != null)
					{
						contextMenuStrip.Show(this, evt.ViewPoint);
						return true;
					}
					GoContextMenu contextMenu = goObject.GetContextMenu(this);
					if (contextMenu != null)
					{
						contextMenu.Show(this, evt.ViewPoint);
						return true;
					}
					if (goObject.OnContextClick(evt, this))
					{
						return true;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				RaiseBackgroundContextClicked(evt);
			}
			return false;
		}

		/// <summary>
		/// Perform the standard mouse wheel behavior for views.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// When the Control key is held down, rotating the mouse wheel changes the
		/// <see cref="P:Northwoods.Go.GoView.DocScale" /> to "zoom" the view in or out at the current mouse point.
		/// Otherwise rotating the mouse wheel scrolls the view by calling <see cref="M:Northwoods.Go.GoView.ScrollLine(System.Single,System.Single)" />.
		/// If the Shift key is held down, the scrolling is horizontal instead of vertical.
		/// </para>
		/// <para>
		/// This is normally called from the <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoMouseWheel" />
		/// method, to handle mouse wheel turns in the standard manner.
		/// </para>
		/// </remarks>
		public virtual void DoWheel(GoInputEventArgs evt)
		{
			if (evt.Delta == 0)
			{
				return;
			}
			if (EditControl != null)
			{
				RequestFocus();
			}
			if (evt.Control)
			{
				PointF docPosition = DocPosition;
				DocScale *= 1f + (float)evt.Delta / 2400f;
				PointF pointF = ConvertViewToDoc(evt.ViewPoint);
				DocPosition = new PointF(docPosition.X + evt.DocPoint.X - pointF.X, docPosition.Y + evt.DocPoint.Y - pointF.Y);
			}
			else
			{
				int num = checked(-evt.Delta) / 120;
				if (evt.Shift)
				{
					ScrollLine(num, 0f);
				}
				else
				{
					ScrollLine(0f, num);
				}
			}
		}

		/// <summary>
		/// Perform the behavior that normally occurs upon a mouse hover.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the hover occured on a document object
		/// whose <see cref="M:Northwoods.Go.GoObject.OnHover(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// <para>
		/// By default this picks the document object at the event's
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.
		/// If an object is found, it raises the ObjectHover event and
		/// calls <see cref="M:Northwoods.Go.GoObject.OnHover(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> on the object and on
		/// its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any) until it returns true.
		/// If no object is found at the event's point, it raises the
		/// BackgroundHover event.
		/// </para>
		/// <para>
		/// This is normally called from the <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoMouseHover" />
		/// method, to process the standard behavior when the mouse hovers somewhere in the view.
		/// </para>
		/// </remarks>
		public virtual bool DoHover(GoInputEventArgs evt)
		{
			GoObject goObject = PickObject(doc: true, view: false, evt.DocPoint, selectableOnly: false);
			if (goObject != null)
			{
				RaiseObjectHover(goObject, evt);
				while (goObject != null)
				{
					if (goObject.OnHover(evt, this))
					{
						return true;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				RaiseBackgroundHover(evt);
			}
			return false;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectSingleClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectSingleClicked(GoObjectEventArgs evt)
		{
			if (this.ObjectSingleClicked != null)
			{
				this.ObjectSingleClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectSingleClicked" /> event for a given object and canonicalized input event.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public void RaiseObjectSingleClicked(GoObject obj, GoInputEventArgs evt)
		{
			OnObjectSingleClicked(new GoObjectEventArgs(obj, evt));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectDoubleClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectDoubleClicked(GoObjectEventArgs evt)
		{
			if (this.ObjectDoubleClicked != null)
			{
				this.ObjectDoubleClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectDoubleClicked" /> event for a given object and canonicalized input event.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public void RaiseObjectDoubleClicked(GoObject obj, GoInputEventArgs evt)
		{
			OnObjectDoubleClicked(new GoObjectEventArgs(obj, evt));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectContextClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectContextClicked(GoObjectEventArgs evt)
		{
			if (this.ObjectContextClicked != null)
			{
				this.ObjectContextClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectContextClicked" /> event for a given object and canonicalized input event.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public void RaiseObjectContextClicked(GoObject obj, GoInputEventArgs evt)
		{
			OnObjectContextClicked(new GoObjectEventArgs(obj, evt));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundSingleClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnBackgroundSingleClicked(GoInputEventArgs evt)
		{
			if (this.BackgroundSingleClicked != null)
			{
				this.BackgroundSingleClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.BackgroundSingleClicked" /> event for a given canonicalized input event.
		/// </summary>
		/// <param name="evt"></param>
		public void RaiseBackgroundSingleClicked(GoInputEventArgs evt)
		{
			OnBackgroundSingleClicked(evt);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundDoubleClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnBackgroundDoubleClicked(GoInputEventArgs evt)
		{
			if (this.BackgroundDoubleClicked != null)
			{
				this.BackgroundDoubleClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.BackgroundDoubleClicked" /> event for a given canonicalized input event.
		/// </summary>
		/// <param name="evt"></param>
		public void RaiseBackgroundDoubleClicked(GoInputEventArgs evt)
		{
			OnBackgroundDoubleClicked(evt);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This event is often used to provide a context menu for the view when no
		/// object is at the click point.
		/// We recommend not using the <c>Control.ContextMenu</c> property, but bringing
		/// up a context menu explicitly when handling this event.
		/// </remarks>
		protected virtual void OnBackgroundContextClicked(GoInputEventArgs evt)
		{
			if (this.BackgroundContextClicked != null)
			{
				this.BackgroundContextClicked(this, evt);
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.BackgroundContextClicked" /> event for a given canonicalized input event.
		/// </summary>
		/// <param name="evt"></param>
		public void RaiseBackgroundContextClicked(GoInputEventArgs evt)
		{
			OnBackgroundContextClicked(evt);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectEnterLeave(GoObjectEnterLeaveEventArgs evt)
		{
			if (this.ObjectEnterLeave != null)
			{
				this.ObjectEnterLeave(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" /> event for a given object.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="evt"></param>
		public void RaiseObjectEnterLeave(GoObject from, GoObject to, GoInputEventArgs evt)
		{
			OnObjectEnterLeave(new GoObjectEnterLeaveEventArgs(from, to, evt));
		}

		/// <summary>
		/// Perform the behavior that normally occurs when there is a change in the
		/// document object that the mouse is over.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// By default this raises the <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" /> event by calling <see cref="M:Northwoods.Go.GoView.RaiseObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" />.
		/// It calls <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /> for the
		/// <paramref name="from" /> object (if not null) and the <paramref name="to" /> object
		/// (if not null) and proceeds up the <see cref="P:Northwoods.Go.GoObject.Parent" /> chain for both objects.
		/// This allows groups such as nodes the ability to easily detect when the mouse enters
		/// or leaves the node, and to detect when the mouse moves over different child objects of a node.
		/// If both <paramref name="from" /> and <paramref name="to" /> are part of the same group,
		/// this method does not call <see cref="M:Northwoods.Go.GoObject.OnEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /> twice on the common parent group.
		/// A true value from a call to <see cref="M:Northwoods.Go.GoObject.OnEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoView)" /> will stop that
		/// chain of calls up the <see cref="P:Northwoods.Go.GoObject.Parent" /> tree.
		/// </para>
		/// <para>
		/// This is normally called from the <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoMouseMove" /> method,
		/// and from the <see cref="T:Northwoods.Go.GoToolDragging" />.<see cref="M:Northwoods.Go.GoToolDragging.DoMouseMove" /> method.
		/// Therefore this <see cref="E:Northwoods.Go.GoView.ObjectEnterLeave" /> event is not raised during
		/// a resize or a linking or a rubber-band selection operation.
		/// However, you may extend existing tools or define new tools to call this method
		/// under the circumstances and with the objects that your policy would indicate.
		/// </para>
		/// </remarks>
		public virtual void DoObjectEnterLeave(GoObject from, GoObject to, GoInputEventArgs evt)
		{
			RaiseObjectEnterLeave(from, to, evt);
			GoObject goObject = GoObject.FindCommonParent(from, to);
			while (from != null && from != goObject && !from.OnEnterLeave(from, to, this))
			{
				from = from.Parent;
			}
			bool flag = false;
			while (to != null && to != goObject)
			{
				if (to.OnEnterLeave(from, to, this))
				{
					flag = true;
					break;
				}
				to = to.Parent;
			}
			if (!flag)
			{
				while (goObject != null && !goObject.OnEnterLeave(from, to, this))
				{
					goObject = goObject.Parent;
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropReject" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectSelectionDropReject(GoObjectEventArgs evt)
		{
			if (this.ObjectSelectionDropReject != null)
			{
				this.ObjectSelectionDropReject(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnObjectSelectionDropReject(Northwoods.Go.GoObjectEventArgs)" /> with the given <see cref="T:Northwoods.Go.GoObjectEventArgs" />
		/// to raise a <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropReject" /> event.
		/// </summary>
		/// <remarks>
		/// The caller needs to allocate a <see cref="T:Northwoods.Go.GoObjectEventArgs" /> and afterwards
		/// examine the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> to see if an event handler
		/// had set the value to <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public void RaiseObjectSelectionDropReject(GoObjectEventArgs evt)
		{
			OnObjectSelectionDropReject(evt);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropReject" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnBackgroundSelectionDropReject(GoInputEventArgs evt)
		{
			if (this.BackgroundSelectionDropReject != null)
			{
				this.BackgroundSelectionDropReject(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnBackgroundSelectionDropReject(Northwoods.Go.GoInputEventArgs)" /> with the given <see cref="T:Northwoods.Go.GoInputEventArgs" />
		/// to raise a <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropReject" /> event.
		/// </summary>
		/// <remarks>
		/// The caller needs to allocate a <see cref="T:Northwoods.Go.GoInputEventArgs" /> and afterwards
		/// examine the <see cref="P:Northwoods.Go.GoInputEventArgs.InputState" /> to see if an event handler
		/// had set the value to <see cref="T:Northwoods.Go.GoInputState" />.<see cref="F:Northwoods.Go.GoInputState.Cancel" />.
		/// </remarks>
		public void RaiseBackgroundSelectionDropReject(GoInputEventArgs evt)
		{
			OnBackgroundSelectionDropReject(evt);
		}

		/// <summary>
		/// Invoke <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropReject" /> or
		/// <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropReject" /> event handlers and
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnSelectionDropReject(Northwoods.Go.GoObjectEventArgs,Northwoods.Go.GoView)" /> methods
		/// to see if any want to prevent a drop.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if no drop should occur; false if a drop is acceptable.</returns>
		/// <remarks>
		/// This is normally called from the <see cref="T:Northwoods.Go.GoToolDragging" /> tool's
		/// <see cref="M:Northwoods.Go.GoToolDragging.DoMouseMove" /> and <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" /> methods,
		/// to decide whether a drop is permissible at the given input event point.
		/// In Windows Forms, it is also called by <c>DoExternalDrop</c> when dropping <c>GoObject</c>s,
		/// for completeness, when the <see cref="T:Northwoods.Go.GoToolDragging" /> tool is not used
		/// for normal external drag-and-drops.
		/// This method must be called before calling <see cref="M:Northwoods.Go.GoView.DoSelectionDropped(Northwoods.Go.GoInputEventArgs)" />,
		/// but it may be called many times during a drag before a drop actually happens.
		/// </remarks>
		public virtual bool DoSelectionDropReject(GoInputEventArgs evt)
		{
			GoObject goObject = PickObjectExcluding(doc: true, view: false, evt.DocPoint, selectableOnly: false, Selection);
			mySelectionDropRejectOverValid = true;
			mySelectionDropRejectOver = goObject;
			if (goObject != null)
			{
				GoObjectEventArgs goObjectEventArgs = new GoObjectEventArgs(goObject, evt);
				RaiseObjectSelectionDropReject(goObjectEventArgs);
				if (goObjectEventArgs.InputState == GoInputState.Cancel)
				{
					return true;
				}
				while (goObject != null)
				{
					if (goObject.OnSelectionDropReject(goObjectEventArgs, this) || goObjectEventArgs.InputState == GoInputState.Cancel)
					{
						return true;
					}
					if (goObjectEventArgs.GoObject == null)
					{
						return false;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				GoInputEventArgs goInputEventArgs = new GoInputEventArgs(evt);
				RaiseBackgroundSelectionDropReject(goInputEventArgs);
				if (goInputEventArgs.InputState == GoInputState.Cancel)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropped" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectSelectionDropped(GoObjectEventArgs evt)
		{
			if (this.ObjectSelectionDropped != null)
			{
				this.ObjectSelectionDropped(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropped" /> event for a given object and canonicalized input event.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public void RaiseObjectSelectionDropped(GoObject obj, GoInputEventArgs evt)
		{
			OnObjectSelectionDropped(new GoObjectEventArgs(obj, evt));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropped" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// </remarks>
		protected virtual void OnBackgroundSelectionDropped(GoInputEventArgs evt)
		{
			if (this.BackgroundSelectionDropped != null)
			{
				this.BackgroundSelectionDropped(this, evt);
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropped" /> event for a given canonicalized input event.
		/// </summary>
		/// <param name="evt"></param>
		public void RaiseBackgroundSelectionDropped(GoInputEventArgs evt)
		{
			OnBackgroundSelectionDropped(evt);
		}

		internal GoObject PickObjectExcluding(bool doc, bool view, PointF p, bool selectableOnly, IGoCollection ignore)
		{
			if (selectableOnly && !CanSelectObjects())
			{
				return null;
			}
			GoCollection goCollection = new GoCollection();
			goCollection.InternalChecksForDuplicates = false;
			foreach (GoLayer backward in Layers.Backwards)
			{
				if ((doc && backward.IsInDocument) || (view && backward.IsInView))
				{
					goCollection.Clear();
					backward.PickObjects(p, selectableOnly, goCollection, 999999);
					foreach (GoObject item in goCollection)
					{
						bool flag = false;
						foreach (GoObject item2 in ignore)
						{
							if (item == item2 || item.IsChildOf(item2))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return item;
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Perform the behavior that normally occurs when the <see cref="P:Northwoods.Go.GoView.Selection" /> has been
		/// dropped in this view.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the selection was dropped on a document object
		/// whose <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnSelectionDropped(Northwoods.Go.GoObjectEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// <para>
		/// By default this picks the document object at the event's
		/// <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />, ignoring all objects that are in
		/// the <see cref="P:Northwoods.Go.GoView.Selection" /> or are part of any selected object.
		/// If an object is found, it raises the <see cref="E:Northwoods.Go.GoView.ObjectSelectionDropped" /> event and
		/// calls <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.OnSelectionDropped(Northwoods.Go.GoObjectEventArgs,Northwoods.Go.GoView)" /> on the object and
		/// on its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any) until it returns
		/// true.
		/// </para>
		/// <para>
		/// If no object is found at the event's point, it raises the
		/// <see cref="E:Northwoods.Go.GoView.BackgroundSelectionDropped" /> event.
		/// </para>
		/// <para>
		/// This is normally called from the <see cref="T:Northwoods.Go.GoToolDragging" /> tool's
		/// <see cref="M:Northwoods.Go.GoToolDragging.DoMouseUp" /> method.
		/// For external drag-and-drops from other Controls, in Windows Forms,
		/// this method is also called.  If <c>ExternalDragDropsOnEnter</c> is true,
		/// this method is called by the <see cref="T:Northwoods.Go.GoToolDragging" /> tool, just as
		/// for an internal drag-and-drop.  If <c>ExternalDragDropsOnEnter</c> is false,
		/// <c>DoExternalDrop</c> calls this method, since the regular <see cref="T:Northwoods.Go.GoToolDragging" />
		/// tool is not used on a normal external drop.
		/// This method should only be called when <see cref="M:Northwoods.Go.GoView.DoSelectionDropReject(Northwoods.Go.GoInputEventArgs)" /> returns false.
		/// </para>
		/// </remarks>
		public virtual bool DoSelectionDropped(GoInputEventArgs evt)
		{
			GoObject goObject = (!mySelectionDropRejectOverValid) ? PickObjectExcluding(doc: true, view: false, evt.DocPoint, selectableOnly: false, Selection) : mySelectionDropRejectOver;
			mySelectionDropRejectOverValid = false;
			if (goObject != null)
			{
				GoObjectEventArgs goObjectEventArgs = new GoObjectEventArgs(goObject, evt);
				OnObjectSelectionDropped(goObjectEventArgs);
				while (goObject != null)
				{
					if (goObject.OnSelectionDropped(goObjectEventArgs, this))
					{
						return true;
					}
					if (goObjectEventArgs.GoObject == null)
					{
						return false;
					}
					goObject = goObject.Parent;
				}
			}
			else
			{
				RaiseBackgroundSelectionDropped(evt);
			}
			return false;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectHover" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectHover(GoObjectEventArgs evt)
		{
			if (this.ObjectHover != null)
			{
				this.ObjectHover(this, evt);
			}
		}

		/// <summary>
		/// Raise an <see cref="E:Northwoods.Go.GoView.ObjectHover" /> event for a given object and canonicalized input event.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="evt"></param>
		public void RaiseObjectHover(GoObject obj, GoInputEventArgs evt)
		{
			OnObjectHover(new GoObjectEventArgs(obj, evt));
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.BackgroundHover" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// If you want to get notification of mouse moves immediately, rather than
		/// after a delay, you'll need to override <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" /> or
		/// one of the methods that it calls, such as <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />.
		/// </remarks>
		protected virtual void OnBackgroundHover(GoInputEventArgs evt)
		{
			if (this.BackgroundHover != null)
			{
				this.BackgroundHover(this, evt);
			}
		}

		/// <summary>
		/// Raise a <see cref="E:Northwoods.Go.GoView.BackgroundHover" /> event for a given canonicalized input event.
		/// </summary>
		/// <param name="evt"></param>
		public void RaiseBackgroundHover(GoInputEventArgs evt)
		{
			OnBackgroundHover(evt);
		}

		/// <summary>
		/// Start or restart a timer to see if the mouse has moved; if at the end
		/// of the timer the mouse has not moved, <see cref="M:Northwoods.Go.GoView.DoMouseHover" />
		/// is called.
		/// </summary>
		/// <param name="viewPnt">a <c>Point</c> in view coordinates</param>
		/// <remarks>
		/// This is called whenever a tool wants to deliver hover events.
		/// The time the mouse must rest motionless is determined by
		/// <see cref="P:Northwoods.Go.GoView.HoverDelay" />.
		/// A mouse leave event will stop the hover timer.
		/// </remarks>
		public virtual void DetectHover(Point viewPnt)
		{
			if (myHoverTimer == null)
			{
				myHoverTimer = new System.Threading.Timer(hoverCallback, new EventHandler(hoverTick), -1, -1);
				myHoverTimerEnabled = false;
			}
			if (myHoverPoint != viewPnt)
			{
				StopHoverTimer();
			}
			if (!myHoverTimerEnabled)
			{
				myHoverTimerEnabled = true;
				myHoverTimer.Change(HoverDelay, -1);
			}
			myHoverPoint = viewPnt;
		}

		private void StopHoverTimer()
		{
			if (myHoverTimer != null)
			{
				myHoverTimerEnabled = false;
				myHoverTimer.Change(-1, -1);
			}
		}

		private void hoverCallback(object obj)
		{
			try
			{
				if (base.IsHandleCreated)
				{
					Invoke((EventHandler)obj);
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
		}

		private void hoverTick(object sender, EventArgs e)
		{
			try
			{
				if (myHoverTimerEnabled)
				{
					GoInputEventArgs lastInput = LastInput;
					lastInput.ViewPoint = myHoverPoint;
					lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
					lastInput.Buttons = Control.MouseButtons;
					lastInput.Modifiers = Control.ModifierKeys;
					lastInput.Delta = 0;
					lastInput.Key = Keys.None;
					lastInput.InputState = GoInputState.Continue;
					DoMouseHover();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" /> if the mouse
		/// enters this view on a document object.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (AllowMouse)
			{
				GoInputEventArgs lastInput = LastInput;
				Point mousePosition = Control.MousePosition;
				lastInput.ViewPoint = PointToClient(mousePosition);
				lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
				lastInput.Buttons = Control.MouseButtons;
				lastInput.Modifiers = Control.ModifierKeys;
				lastInput.Delta = 0;
				lastInput.Key = Keys.None;
				lastInput.MouseEventArgs = null;
				lastInput.InputState = GoInputState.Start;
				GoObject goObject = PickObject(doc: true, view: false, lastInput.DocPoint, selectableOnly: false);
				if (goObject != null)
				{
					DoObjectEnterLeave(null, goObject, LastInput);
				}
			}
		}

		/// <summary>
		/// Turn off any mouse-related timers.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoView.DoObjectEnterLeave(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoInputEventArgs)" /> if the current <see cref="P:Northwoods.Go.GoView.Tool" />
		/// is a <see cref="T:Northwoods.Go.GoToolManager" /> with a <see cref="P:Northwoods.Go.GoTool.CurrentObject" />
		/// that is non-null.
		/// </remarks>
		protected override void OnMouseLeave(EventArgs e)
		{
			StopHoverTimer();
			StopAutoScroll();
			base.OnMouseLeave(e);
			if (AllowMouse)
			{
				GoObject goObject = (Tool as GoToolManager)?.CurrentObject;
				if (goObject != null)
				{
					DoObjectEnterLeave(goObject, null, LastInput);
				}
			}
		}

		/// <summary>
		/// Perform the immediate behavior normally associated with the mouse moving without
		/// a mouse button being pressed.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if the mouse-over occured on a document object
		/// whose <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method returned true</returns>
		/// <remarks>
		/// This is called to handle mouse moves immediately.  If you don't need the
		/// immediate response, but would prefer getting an event after the mouse has
		/// rested at one spot for a while, use the <see cref="M:Northwoods.Go.GoView.DoHover(Northwoods.Go.GoInputEventArgs)" /> method,
		/// the <see cref="M:Northwoods.Go.GoObject.OnHover(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method on <see cref="T:Northwoods.Go.GoObject" />,
		/// or the <see cref="E:Northwoods.Go.GoView.ObjectHover" /> or <see cref="E:Northwoods.Go.GoView.BackgroundHover" /> events.
		/// By default this picks the topmost/frontmost view or document object
		/// at the event's <see cref="P:Northwoods.Go.GoInputEventArgs.DocPoint" />.
		/// It calls <see cref="M:Northwoods.Go.GoView.DoToolTipObject(Northwoods.Go.GoObject)" /> on the result, even if it is null.
		/// If an object is found, it calls <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />
		/// on the object and on its <see cref="P:Northwoods.Go.GoObject.Parent" />s (if any)
		/// until it returns true.
		/// If no object is found at the event's point, or if no object's
		/// <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> returns true, it calls
		/// <see cref="M:Northwoods.Go.GoView.DoBackgroundMouseOver(Northwoods.Go.GoInputEventArgs)" />.  The assumption is that any object
		/// that changes the <c>Cursor</c> will return true from the
		/// <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> method.
		/// This is normally called from <see cref="T:Northwoods.Go.GoToolManager" />.<see cref="M:Northwoods.Go.GoToolManager.DoMouseMove" />,
		/// when no other more specific tools are in effect.
		/// </remarks>
		public virtual bool DoMouseOver(GoInputEventArgs evt)
		{
			GoObject goObject = PickObject(doc: true, view: true, evt.DocPoint, selectableOnly: false);
			DoToolTipObject(goObject);
			bool flag = false;
			while (goObject != null)
			{
				if (goObject.OnMouseOver(evt, this))
				{
					flag = true;
					break;
				}
				goObject = goObject.Parent;
			}
			if (!flag)
			{
				DoBackgroundMouseOver(evt);
			}
			DetectHover(evt.ViewPoint);
			return flag;
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" /> when no call to
		/// <see cref="M:Northwoods.Go.GoObject.OnMouseOver(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" /> has returned true.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// By default this just sets the view's <c>Cursor</c> property to
		/// the value of <see cref="P:Northwoods.Go.GoView.DefaultCursor" />.
		/// </remarks>
		public virtual void DoBackgroundMouseOver(GoInputEventArgs evt)
		{
			SetCursor(DefaultCursor);
		}

		internal void SetCursor(Cursor c)
		{
			try
			{
				SetCursor2(c);
			}
			catch (VerificationException)
			{
			}
			catch (SecurityException)
			{
			}
		}

		private void SetCursor2(Cursor c)
		{
			if (c != null && base.Cursor != c)
			{
				base.Cursor = c;
			}
		}

		/// <summary>
		/// Convert a cursor name to a standardized name.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// Standard cursor names are basically those used by CSS2.
		/// This list includes:
		/// "default", "pointer", "move", "crosshair", "text", "help", "wait", "not-allowed",
		/// and "*-resize" where "*" is one of: "n", "ne", "nw", "s", "se", "sw", "e", "w", "col", or "row".
		/// Other names are lowercased and returned, allowing for the user of
		/// other cursors supported by Windows Forms (e.g. "pannorth") that are
		/// not recognized by CSS2, or those cursors defined by CSS for WebForms or SVG
		/// but that are not standard in Windows Forms.
		/// </para>
		/// <para>
		/// This method will recognize the names of some
		/// <c>System.Windows.Forms.Cursors</c> properties and substitute
		/// the corresponding standard cursor names.  These currently include:
		/// "Cross" (becomes "crosshair"),
		/// "Hand" (becomes "pointer"),
		/// "SizeAll" (becomes "move"),
		/// "SizeNESW" (becomes "ne-resize"),
		/// "SizeNS" (becomes "s-resize"),
		/// "SizeNWSE" (becomes "se-resize"),
		/// "SizeWE" (becomes "e-resize"),
		/// "VSplit" (becomes "col-resize"),
		/// "HSplit" (becomes "row-resize"),
		/// "No" (becomes "not-allowed"),
		/// "WaitCursor" (becomes "wait"),
		/// and "IBeam" (becomes "text").
		/// </para>
		/// </remarks>
		public virtual string StandardizeCursorName(string s)
		{
			if (s == null)
			{
				return "default";
			}
			string text = s.ToLower(CultureInfo.InvariantCulture);
			switch (text)
			{
			case "default":
				return "default";
			case "auto":
				return "default";
			case "cross":
				return "crosshair";
			case "hand":
				return "pointer";
			case "sizeall":
				return "move";
			case "sizenesw":
				return "ne-resize";
			case "sizens":
				return "s-resize";
			case "sizenwse":
				return "se-resize";
			case "sizewe":
				return "e-resize";
			case "vsplit":
				return "col-resize";
			case "hsplit":
				return "row-resize";
			case "no":
				return "not-allowed";
			case "waitcursor":
				return "wait";
			case "ibeam":
				return "text";
			default:
				return text;
			}
		}

		/// <summary>
		/// This method is responsible for finding a tooltip string for an object and then
		/// displaying it in a ToolTip.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// <para>
		/// This calls <see cref="M:Northwoods.Go.GoObject.GetToolTip(Northwoods.Go.GoView)" /> on the given object
		/// and its <see cref="P:Northwoods.Go.GoObject.Parent" />'s until it gets a non-null
		/// <c>String</c> return value.
		/// By default this method does nothing if this view has no <see cref="P:Northwoods.Go.GoView.ToolTip" /> <c>Control</c>.
		/// </para>
		/// <para>
		/// This method is normally called by <see cref="M:Northwoods.Go.GoView.DoMouseOver(Northwoods.Go.GoInputEventArgs)" />.
		/// </para>
		/// </remarks>
		public virtual void DoToolTipObject(GoObject obj)
		{
			if (ToolTip == null)
			{
				return;
			}
			string toolTip = ToolTip.GetToolTip(this);
			string text = null;
			while (obj != null)
			{
				text = obj.GetToolTip(this);
				if (text != null)
				{
					break;
				}
				obj = obj.Parent;
			}
			if (text == null)
			{
				text = ToolTipText;
			}
			if (text == null)
			{
				text = "";
			}
			if (text != toolTip)
			{
				ToolTip.SetToolTip(this, text);
			}
		}

		/// <summary>
		/// This predicate is true when the user can perform the <see cref="M:Northwoods.Go.GoView.EditCopy" /> action.
		/// </summary>
		/// <remarks>
		/// This returns false if the <see cref="P:Northwoods.Go.GoView.Selection" /> is empty,
		/// if <see cref="M:Northwoods.Go.GoView.CanCopyObjects" /> is false,
		/// or if the primary selection's <see cref="M:Northwoods.Go.GoObject.CanCopy" /> property is false.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" />
		/// <seealso cref="M:Northwoods.Go.GoView.EditCopy" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanEditCopy()
		{
			if (!CanCopyObjects())
			{
				return false;
			}
			if (Selection.IsEmpty)
			{
				return false;
			}
			if (!Selection.Primary.CanCopy())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Copy the <see cref="P:Northwoods.Go.GoView.Selection" /> to the clipboard.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method does nothing if <see cref="M:Northwoods.Go.GoView.CanCopyObjects" /> is false.
		/// After calling <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" />,
		/// this calls <see cref="M:Northwoods.Go.GoView.RaiseClipboardCopied" />.
		/// All the actions occur within a transaction.
		/// </para>
		/// <para>
		/// All of the objects in the selection will be copied by serializing them.
		/// If any of the objects, or any of the objects that they refer to, are not
		/// serializable, there will be a serialization exception, and the clipboard
		/// might not a copy of the selection.
		/// <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" /> will log any exceptions to any trace listeners.
		/// The User Guide discusses serialization and deserialization in more detail.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditCopy" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void EditCopy()
		{
			if (CanCopyObjects())
			{
				string tname = null;
				try
				{
					CursorName = "wait";
					StartTransaction();
					CopyToClipboard(Selection);
					tname = "Copy";
					RaiseClipboardCopied();
				}
				catch (Exception ex)
				{
					GoObject.Trace("EditCopy: " + ex.ToString());
					//throw;bylzy
				}
				finally
				{
					GoUndoManager undoManager = Document.UndoManager;
					if (undoManager != null && undoManager.CurrentEdit == null)
					{
						undoManager.CurrentEdit = new GoUndoManagerCompoundEdit();
					}
					FinishTransaction(tname);
					CursorName = "default";
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ClipboardCopied" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This is called after <see cref="M:Northwoods.Go.GoView.EditCopy" /> and <see cref="M:Northwoods.Go.GoView.EditCut" />
		/// call to <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" />.
		/// </remarks>
		protected virtual void OnClipboardCopied(EventArgs evt)
		{
			if (this.ClipboardCopied != null)
			{
				this.ClipboardCopied(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnClipboardCopied(System.EventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.ClipboardCopied" /> event.
		/// </summary>
		public void RaiseClipboardCopied()
		{
			OnClipboardCopied(EventArgs.Empty);
		}

		/// <summary>
		/// Put a copy of a collection of objects in the clipboard.
		/// </summary>
		/// <param name="coll"></param>
		/// <remarks>
		/// <para>
		/// The collection is copied into a new document of the same
		/// class as the <see cref="P:Northwoods.Go.GoView.Document" /> property value.
		/// (That document class must have a public zero-argument constructor.)
		/// This method then calls <see cref="M:Northwoods.Go.GoDocument.MergeLayersFrom(Northwoods.Go.GoDocument)" /> this
		/// view's document and then calls <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />
		/// to make a copy of all of the objects in <paramref name="coll" /> in
		/// the new document in the appropriate layers.
		/// Note that in order to maintain the distinct layers of the objects being copied,
		/// each document layer needs to have a unique identifier, <see cref="P:Northwoods.Go.GoLayer.Identifier" />.
		/// The new document is then inserted into the clipboard using that
		/// document's <see cref="P:Northwoods.Go.GoDocument.DataFormat" />.
		/// </para>
		/// <para>
		/// To facilitate pasting into regular (not GoDiagram) documents, this method
		/// calls <see cref="M:Northwoods.Go.GoView.CreateDataObject(Northwoods.Go.IGoCollection,Northwoods.Go.GoDocument)" /> to support additional data formats.
		/// If the <paramref name="coll" /> is null or empty, the clipboard is cleared
		/// of any previous data values.
		/// </para>
		/// <para>
		/// All of the objects in the collection will be copied by serializing them.
		/// If any of the objects, or any of the objects that they refer to, are not
		/// serializable, there will be a serialization exception, and the clipboard
		/// might not a copy of the argument collection.
		/// Exceptions will be logged to any trace listeners.
		/// The User Guide discusses serialization and deserialization in more detail.
		/// </para>
		/// </remarks>
		public virtual void CopyToClipboard(IGoCollection coll)
		{
			if (coll == null || coll.IsEmpty)
			{
				try
				{
					Clipboard.SetDataObject(new DataObject());
				}
				catch (VerificationException ex)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex.ToString());
					myClipboard.Clear();
				}
				catch (SecurityException ex2)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex2.ToString());
					myClipboard.Clear();
				}
				catch (Exception ex3)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex3.ToString());
					throw;
				}
				return;
			}
			GoDocument document = Document;
			if (document != null)
			{
				GoDocument goDocument = (GoDocument)Activator.CreateInstance(document.GetType());
				goDocument.UndoManager = null;
				goDocument.MergeLayersFrom(document);
				GoCollection goCollection = new GoCollection();
				goCollection.InternalChecksForDuplicates = false;
				goCollection.AddRange(coll);
				document.Layers.SortByZOrder(goCollection);
				goDocument.CopyFromCollection(goCollection, copyableOnly: true, dragging: true, default(SizeF), null);
				try
				{
					Clipboard.SetDataObject(CreateDataObject(coll, goDocument), copy: false, 5, 100);
				}
				catch (VerificationException ex4)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex4.ToString());
					myClipboard.Clear();
					myClipboard[goDocument.DataFormat] = goDocument;
				}
				catch (SecurityException ex5)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex5.ToString());
					myClipboard.Clear();
					myClipboard[goDocument.DataFormat] = goDocument;
				}
				catch (Exception ex6)
				{
					GoObject.Trace("GoView.CopyToClipboard: " + ex6.ToString());
					throw;
				}
			}
		}

		/// <summary>
		/// This method is called from <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" /> to
		/// produce a <c>DataObject</c> representing the available data formats
		/// and their values for the objects in the clipboard.
		/// </summary>
		/// <param name="coll">the original collection passed to <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" /></param>
		/// <param name="clipdoc">the <see cref="T:Northwoods.Go.GoDocument" /> holding a copy of <paramref name="coll" />
		/// that normally should be the primary value in the clipboard, with a <c>DataFormat</c>
		/// as specified by <see cref="P:Northwoods.Go.GoDocument.DataFormat" />.</param>
		/// <returns></returns>
		/// <remarks>
		/// In addition to using the <see cref="P:Northwoods.Go.GoDocument.DataFormat" /> format
		/// to store the <paramref name="clipdoc" />,
		/// this calls <see cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection)" /> to draw
		/// the collection into a bitmap and to insert it into the clipboard
		/// as an alternative Bitmap data format.
		/// Finally, each object in the clipboard document that implements
		/// <see cref="T:Northwoods.Go.IGoLabeledPart" /> provides a text value.  The text strings are
		/// all concatenated together, separated by NewLines, to provide an alternative
		/// textual data format value in the clipboard for those applications that
		/// cannot handle bitmaps either.
		/// </remarks>
		protected virtual DataObject CreateDataObject(IGoCollection coll, GoDocument clipdoc)
		{
			DataObject dataObject = new DataObject();
			dataObject.SetData(clipdoc.DataFormat, clipdoc);
			Bitmap bitmapFromCollection = GetBitmapFromCollection(clipdoc);
			dataObject.SetData(DataFormats.Bitmap, autoConvert: true, bitmapFromCollection);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (GoObject item in clipdoc)
			{
				string text = null;
				IGoLabeledPart goLabeledPart = item as IGoLabeledPart;
				if (goLabeledPart != null)
				{
					text = goLabeledPart.Text;
				}
				else
				{
					GoText goText = item as GoText;
					if (goText != null)
					{
						text = goText.Text;
					}
				}
				if (text != null)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(Environment.NewLine);
					}
					stringBuilder.Append(text);
				}
			}
			if (stringBuilder.Length > 0)
			{
				dataObject.SetData(DataFormats.UnicodeText, autoConvert: true, stringBuilder.ToString());
			}
			return dataObject;
		}

		/// <summary>
		/// This predicate is true when the user can perform the <see cref="M:Northwoods.Go.GoView.EditCut" /> action.
		/// </summary>
		/// <remarks>
		/// This returns false if the <see cref="P:Northwoods.Go.GoView.Selection" /> is empty,
		/// if <see cref="M:Northwoods.Go.GoView.CanCopyObjects" /> is false,
		/// if <see cref="M:Northwoods.Go.GoView.CanDeleteObjects" /> is false,
		/// or if the primary selection's <see cref="M:Northwoods.Go.GoObject.CanCopy" />
		/// or <see cref="M:Northwoods.Go.GoObject.CanDelete" /> properties are false.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.EditCut" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanEditCut()
		{
			if (!CanCopyObjects())
			{
				return false;
			}
			if (!CanDeleteObjects())
			{
				return false;
			}
			if (Selection.IsEmpty)
			{
				return false;
			}
			GoObject primary = Selection.Primary;
			if (!primary.CanCopy())
			{
				return false;
			}
			if (!primary.CanDelete())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Copy the current <see cref="P:Northwoods.Go.GoView.Selection" /> to the clipboard and then delete it.
		/// </summary>
		/// <remarks>
		/// This method does nothing if <see cref="M:Northwoods.Go.GoView.CanCopyObjects" /> or
		/// <see cref="M:Northwoods.Go.GoView.CanDeleteObjects" /> is false.
		/// After calling <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" /> and <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />,
		/// this calls <see cref="M:Northwoods.Go.GoView.RaiseClipboardCopied" />.
		/// All the actions occur within a transaction.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditCut" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void EditCut()
		{
			if (CanCopyObjects() && CanDeleteObjects())
			{
				string tname = null;
				try
				{
					CursorName = "wait";
					StartTransaction();
					CopyToClipboard(Selection);
					DeleteSelection(Selection);
					tname = "Cut";
					RaiseClipboardCopied();
				}
				catch (Exception ex)
				{
					GoObject.Trace("EditCut: " + ex.ToString());
					throw;
				}
				finally
				{
					FinishTransaction(tname);
					CursorName = "default";
				}
			}
		}

		/// <summary>
		/// This predicate is true when the user can perform the <see cref="M:Northwoods.Go.GoView.EditPaste" /> action.
		/// </summary>
		/// <remarks>
		/// This returns false if <see cref="M:Northwoods.Go.GoView.CanInsertObjects" /> is false,
		/// or if the clipboard doesn't have data supporting the <see cref="P:Northwoods.Go.GoDocument.DataFormat" /> format.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.PasteFromClipboard" />
		/// <seealso cref="M:Northwoods.Go.GoView.EditPaste" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanEditPaste()
		{
			if (!CanInsertObjects())
			{
				return false;
			}
			GoDocument document = Document;
			if (document == null)
			{
				return false;
			}
			try
			{
				return Clipboard.GetDataObject()?.GetDataPresent(document.DataFormat) ?? false;
			}
			catch (ExternalException ex)
			{
				GoObject.Trace("GoView.CanEditPaste: " + ex.ToString());
				return myClipboard.ContainsKey(document.DataFormat);
			}
			catch (VerificationException ex2)
			{
				GoObject.Trace("GoView.CanEditPaste: " + ex2.ToString());
				return myClipboard.ContainsKey(document.DataFormat);
			}
			catch (SecurityException ex3)
			{
				GoObject.Trace("GoView.CanEditPaste: " + ex3.ToString());
				return myClipboard.ContainsKey(document.DataFormat);
			}
			catch (Exception ex4)
			{
				GoObject.Trace("GoView.CanEditPaste: " + ex4.ToString());
				throw;
			}
		}

		/// <summary>
		/// Paste and select a copy of the clipboard's GoObjects into this view's document.
		/// </summary>
		/// <remarks>
		/// <para>
		/// After calling <see cref="M:Northwoods.Go.GoView.PasteFromClipboard" />, it selects all of the
		/// newly inserted top-level objects, and then calls <see cref="M:Northwoods.Go.GoView.RaiseClipboardPasted" />.
		/// This method does nothing if <see cref="M:Northwoods.Go.GoView.CanInsertObjects" /> is false.
		/// All the actions occur within a transaction.
		/// The selection is built between <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> and
		/// <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events.
		/// </para>
		/// <para>
		/// All of the pasted objects will be copied by deserializing them.
		/// If any of the objects, or any of the objects that they refer to, are not
		/// (de-)serializable, there will be a serialization exception, and the paste
		/// might fail.
		/// <see cref="M:Northwoods.Go.GoView.PasteFromClipboard" /> will log any exceptions to any trace listeners.
		/// The User Guide discusses serialization and deserialization in more detail.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CanEditPaste" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void EditPaste()
		{
			if (CanInsertObjects())
			{
				GoDocument document = Document;
				string tname = null;
				bool suspendsRouting = document.SuspendsRouting;
				try
				{
					CursorName = "wait";
					StartTransaction();
					document.SuspendsRouting = true;
					GoCopyDictionary goCopyDictionary = PasteFromClipboard();
					if (goCopyDictionary != null)
					{
						bool flag = false;
						RaiseSelectionStarting();
						IGoCollection sourceCollection = goCopyDictionary.SourceCollection;
						if (sourceCollection != null)
						{
							foreach (GoObject item in sourceCollection)
							{
								GoObject goObject = goCopyDictionary[item] as GoObject;
								if (goObject != null && goObject.IsTopLevel && goObject.Document == document)
								{
									if (!flag)
									{
										flag = true;
										Selection.Clear();
									}
									Selection.Add(goObject);
								}
							}
						}
						else
						{
							IDictionaryEnumerator enumerator2 = goCopyDictionary.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								GoObject goObject2 = enumerator2.Key as GoObject;
								if (goObject2 != null && goObject2.IsTopLevel)
								{
									GoObject goObject3 = enumerator2.Value as GoObject;
									if (goObject3 != null && goObject3.IsTopLevel && goObject3.Document == document)
									{
										if (!flag)
										{
											flag = true;
											Selection.Clear();
										}
										Selection.Add(goObject3);
									}
								}
							}
						}
						RaiseSelectionFinished();
					}
					tname = "Paste";
					RaiseClipboardPasted();
					document.ResumeRouting(suspendsRouting, Selection);
				}
				catch (Exception ex)
				{
					GoObject.Trace("EditPaste: " + ex.ToString());
					throw;
				}
				finally
				{
					document.SuspendsRouting = suspendsRouting;
					FinishTransaction(tname);
					CursorName = "default";
				}
			}
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ClipboardPasted" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This is called after a call to <see cref="M:Northwoods.Go.GoView.PasteFromClipboard" /> in <see cref="M:Northwoods.Go.GoView.EditPaste" />.
		/// </remarks>
		protected virtual void OnClipboardPasted(EventArgs evt)
		{
			if (this.ClipboardPasted != null)
			{
				this.ClipboardPasted(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnClipboardPasted(System.EventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.ClipboardPasted" /> event.
		/// </summary>
		public void RaiseClipboardPasted()
		{
			OnClipboardPasted(EventArgs.Empty);
		}

		/// <summary>
		/// Copy the GoObjects in the clipboard into this view's document.
		/// </summary>
		/// <returns>The <see cref="T:Northwoods.Go.GoCopyDictionary" /> representing the results of the copy.</returns>
		/// <remarks>
		/// <para>
		/// This assumes the clipboard has a value in this view's document's
		/// <see cref="P:Northwoods.Go.GoDocument.DataFormat" />.
		/// The value will be a <see cref="T:Northwoods.Go.GoDocument" />; we just call
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" /> to copy the objects
		/// from the clipboard document into this document.
		/// </para>
		/// <para>
		/// All of the pasted objects will be copied by deserializing them.
		/// If any of the objects, or any of the objects that they refer to, are not
		/// (de-)serializable, there will be a serialization exception, and the paste
		/// might fail.
		/// Exceptions will be logged to any trace listeners.
		/// The User Guide discusses serialization and deserialization in more detail.
		/// </para>
		/// </remarks>
		public virtual GoCopyDictionary PasteFromClipboard()
		{
			GoDocument document = Document;
			if (document == null)
			{
				return null;
			}
			GoDocument value = null;
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject == null)
				{
					return null;
				}
				value = (dataObject.GetData(document.DataFormat) as GoDocument);
			}
			catch (VerificationException ex)
			{
				GoObject.Trace("GoView.PasteFromClipboard: " + ex.ToString());
				myClipboard.TryGetValue(document.DataFormat, out value);
			}
			catch (SecurityException ex2)
			{
				GoObject.Trace("GoView.PasteFromClipboard: " + ex2.ToString());
				myClipboard.TryGetValue(document.DataFormat, out value);
			}
			catch (Exception ex3)
			{
				GoObject.Trace("GoView.PasteFromClipboard: " + ex3.ToString());
				throw;
			}
			if (value != null)
			{
				return document.CopyFromCollection(value, copyableOnly: false, dragging: false, new SizeF(1f, 1f), null);
			}
			return null;
		}

		internal bool InitAllowDrop(bool dnd)
		{
			try
			{
				InitAllowDrop2(dnd);
			}
			catch (VerificationException ex)
			{
				AllowDragOut = false;
				GoObject.Trace("GoView.init: " + ex.ToString());
				return false;
			}
			catch (SecurityException ex2)
			{
				AllowDragOut = false;
				GoObject.Trace("GoView.init: " + ex2.ToString());
				return false;
			}
			return true;
		}

		private void InitAllowDrop2(bool dnd)
		{
			base.AllowDrop = dnd;
		}

		/// <summary>
		/// Handle the DragOver event by canonicalizing the input event and behaving
		/// differently for internal drags than for external drags coming from other windows.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This sets up <see cref="P:Northwoods.Go.GoView.LastInput" /> and then it either calls
		/// <see cref="M:Northwoods.Go.GoView.DoInternalDrag(System.Windows.Forms.DragEventArgs)" /> for drags that started in this view
		/// (because the current tool is an instance of <see cref="T:Northwoods.Go.GoToolDragging" />)
		/// or it calls <see cref="M:Northwoods.Go.GoView.DoExternalDrag(System.Windows.Forms.DragEventArgs)" /> for dragging from another <c>Control</c>.
		/// </remarks>
		protected override void OnDragOver(DragEventArgs evt)
		{
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				try
				{
					Point p = new Point(evt.X, evt.Y);
					lastInput.ViewPoint = PointToClient(p);
					lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
					lastInput.Buttons = Control.MouseButtons;
					lastInput.Modifiers = Control.ModifierKeys;
					lastInput.Delta = 0;
					lastInput.Key = Keys.None;
					lastInput.DragEventArgs = evt;
					lastInput.InputState = GoInputState.Continue;
					if (IsInternalDragDrop(evt))
					{
						DoInternalDrag(evt);
					}
					else if (PretendsInternalDrag)
					{
						DoMouseMove();
					}
					else
					{
						DoExternalDrag(evt);
					}
				}
				catch (Exception ex)
				{
					GoObject.Trace("OnDragOver: " + ex.ToString());
					throw;
				}
			}
			base.OnDragOver(evt);
			lastInput.DragEventArgs = null;
		}

		/// <summary>
		/// This predicate is true when a drag-and-drop started within this view.
		/// </summary>
		/// <param name="evt">may be null</param>
		/// <returns>
		/// true if the current <see cref="P:Northwoods.Go.GoView.Tool" /> is an instance of <see cref="T:Northwoods.Go.GoToolDragging" />,
		/// and if the view is not in the midst of a drag caused by an external drag
		/// when <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> was true.
		/// </returns>
		/// <remarks>
		/// The only tool that calls <c>Control.DoDragDrop</c> in the standard
		/// implementation is <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		public virtual bool IsInternalDragDrop(DragEventArgs evt)
		{
			if (Tool is GoToolDragging)
			{
				return !PretendsInternalDrag;
			}
			return false;
		}

		/// <summary>
		/// Handle dragging that started from this view.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// By default this just calls <see cref="M:Northwoods.Go.GoView.DoMouseMove" />.
		/// This is infrequently overridden to change the behavior for all internal drags
		/// before a drop occurs--normally it is cleaner if one modifies the
		/// <see cref="T:Northwoods.Go.GoToolDragging" /> class or creates another new tool.
		/// </para>
		/// <para>
		/// This is called by <see cref="M:Northwoods.Go.GoView.OnDragOver(System.Windows.Forms.DragEventArgs)" /> if <see cref="M:Northwoods.Go.GoView.IsInternalDragDrop(System.Windows.Forms.DragEventArgs)" /> is true.
		/// </para>
		/// </remarks>
		protected virtual void DoInternalDrag(DragEventArgs evt)
		{
			DoMouseMove();
		}

		/// <summary>
		/// Handle dragging from another Control.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// By default this sets the <c>DragEventArgs.Effect</c> according to whether
		/// <see cref="M:Northwoods.Go.GoView.CanInsertObjects" /> is true.
		/// It also starts autoscrolling, if appropriate.
		/// Override this method to change the behavior when a drag comes in
		/// from another window, before a drop occurs.
		/// To examine the data, you may wish to use code such as:
		/// </para>
		/// <para>
		/// This is called by <see cref="M:Northwoods.Go.GoView.OnDragOver(System.Windows.Forms.DragEventArgs)" /> if <see cref="M:Northwoods.Go.GoView.IsInternalDragDrop(System.Windows.Forms.DragEventArgs)" /> is false.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>
		///   GoSelection sel = evt.Data.GetData(typeof(GoSelection)) as GoSelection;
		///   if (sel != null) {
		///     ... look at the selection's GoView, or at the individual selected objects ...
		///   }
		/// </code>
		/// </example>
		protected virtual void DoExternalDrag(DragEventArgs evt)
		{
			FollowExternalDragImage(LastInput.DocPoint);
			if (CanInsertObjects())
			{
				evt.Effect = DragDropEffects.Copy;
				DoAutoScroll(LastInput.ViewPoint);
			}
			else
			{
				evt.Effect = DragDropEffects.None;
			}
		}

		private void ShowExternalDragImage(GoObject img)
		{
			myExternalDragImage = img;
			Layers.Default.Add(img);
		}

		private void FollowExternalDragImage(PointF pt)
		{
			if (myExternalDragImage != null)
			{
				myExternalDragImage.Location = pt;
			}
		}

		private void HideExternalDragImage()
		{
			if (myExternalDragImage != null)
			{
				myExternalDragImage.Remove();
				myExternalDragImage = null;
			}
		}

		/// <summary>
		/// Return an image to be displayed during a drag coming from another window.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoImage" /> that is added to this view,
		/// or null not to have any such image
		/// </returns>
		/// <remarks>
		/// This is called by <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> when an external drag
		/// enters this window, unless <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> is true.
		/// The default implementation just looks at the <c>DragEventArgs.Data</c>.
		/// If the data is a <see cref="T:Northwoods.Go.GoSelection" />,
		/// it calls <see cref="M:Northwoods.Go.GoView.GetBitmapFromCollection(Northwoods.Go.IGoCollection,System.Drawing.RectangleF,System.Boolean)" />,
		/// using <see cref="M:Northwoods.Go.GoDocument.ComputeBounds(Northwoods.Go.IGoCollection,Northwoods.Go.GoView)" /> to determine the size
		/// of the bitmap that is returned, and not drawing any paper background color.
		/// By default all other data formats are ignored, causing this method to return null.
		/// You can return other kinds of <see cref="T:Northwoods.Go.GoObject" />s besides <see cref="T:Northwoods.Go.GoImage" />.
		/// The object (if non-null) gets added to this view to be dragged around by the
		/// user to represent whatever might be dropped, and is discarded when the
		/// drag is finished whether or not a drop occurs.
		/// The relative position of the objects compared to the mouse position is
		/// obtained from the selection's <see cref="P:Northwoods.Go.GoSelection.HotSpot" />.
		/// The object returned by this method has defined its <see cref="P:Northwoods.Go.GoObject.Location" />
		/// so as to return the HotSpot point.
		/// The actual side-effects of a drop do not involve the object returned
		/// by this method.
		/// </remarks>
		protected virtual GoObject GetExternalDragImage(DragEventArgs evt)
		{
			GoSelection goSelection = evt.Data.GetData(typeof(GoSelection)) as GoSelection;
			if (goSelection == null)
			{
				goSelection = (evt.Data.GetData(Selection.GetType()) as GoSelection);
			}
			if (goSelection != null)
			{
				GoObject primary = goSelection.Primary;
				GoSelection coll = goSelection;
				GoToolDragging goToolDragging = FindMouseTool(typeof(GoToolDragging), subclass: true) as GoToolDragging;
				if (goToolDragging != null)
				{
					coll = goToolDragging.ComputeEffectiveSelection(goSelection, move: false);
				}
				RectangleF bounds = GoDocument.ComputeBounds(coll, null);
				Image bitmapFromCollection = GetBitmapFromCollection(coll, bounds, paper: false);
				GoDragImage obj = new GoDragImage
				{
					Image = bitmapFromCollection
				};
				SizeF sizeF = GoTool.SubtractPoints(primary.Position, bounds.Location);
				obj.Offset = new SizeF(sizeF.Width + goSelection.HotSpot.Width, sizeF.Height + goSelection.HotSpot.Height);
				return obj;
			}
			return null;
		}

		/// <summary>
		/// Show an image for an external drag.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// When <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> and <see cref="M:Northwoods.Go.GoView.CanInsertObjects" /> are true,
		/// this method calls <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" /> to create new objects in the document
		/// and then starts a <see cref="T:Northwoods.Go.GoToolDragging" /> tool to allow the user to get customized
		/// feedback during the external drag into this view.
		/// When <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> is false,
		/// or when the call to <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" /> does not produce newly created objects
		/// in the <see cref="P:Northwoods.Go.GoView.Selection" />,
		/// this calls <see cref="M:Northwoods.Go.GoView.GetExternalDragImage(System.Windows.Forms.DragEventArgs)" /> to produce an object
		/// that is added to the view (not to the document) and appears to be dragged around by the
		/// user during the dragging-in operation.
		/// </remarks>
		protected override void OnDragEnter(DragEventArgs evt)
		{
			if (AllowMouse)
			{
				if (ExternalDragDropsOnEnter && !IsInternalDragDrop(evt) && CanInsertObjects())
				{
					StartTransaction();
					GoInputEventArgs lastInput = LastInput;
					Point p = new Point(evt.X, evt.Y);
					lastInput.ViewPoint = PointToClient(p);
					lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
					lastInput.Buttons = Control.MouseButtons;
					lastInput.Modifiers = Control.ModifierKeys;
					lastInput.Delta = 0;
					lastInput.Key = Keys.None;
					lastInput.DragEventArgs = evt;
					lastInput.InputState = GoInputState.Start;
					GoInputEventArgs firstInput = FirstInput;
					firstInput.ViewPoint = lastInput.ViewPoint;
					firstInput.DocPoint = lastInput.DocPoint;
					firstInput.Buttons = lastInput.Buttons;
					firstInput.Modifiers = lastInput.Modifiers;
					firstInput.Delta = lastInput.Delta;
					firstInput.Key = lastInput.Key;
					firstInput.DragEventArgs = lastInput.DragEventArgs;
					firstInput.InputState = lastInput.InputState;
					IGoCollection goCollection = DoExternalDrop(evt);
					if (goCollection != null && !goCollection.IsEmpty)
					{
						PretendsInternalDrag = true;
						evt.Effect = DragDropEffects.Copy;
						GoToolDragging goToolDragging = FindMouseTool(typeof(GoToolDragging), subclass: true) as GoToolDragging;
						if (goToolDragging == null)
						{
							goToolDragging = new GoToolDragging(this);
						}
						goToolDragging.CurrentObject = Selection.Primary;
						goToolDragging.MoveOffset = Selection.HotSpot;
						goToolDragging.SelectsWhenStarts = false;
						bool allowDragOut = AllowDragOut;
						AllowDragOut = false;
						Tool = goToolDragging;
						AllowDragOut = allowDragOut;
						base.OnDragEnter(evt);
						return;
					}
					AbortTransaction();
				}
				if (!IsInternalDragDrop(evt))
				{
					GoObject externalDragImage = GetExternalDragImage(evt);
					if (externalDragImage != null)
					{
						ShowExternalDragImage(externalDragImage);
						FollowExternalDragImage(LastInput.DocPoint);
					}
				}
			}
			base.OnDragEnter(evt);
		}

		/// <summary>
		/// Turn off any mouse-related timers and remove any external drag image or objects.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// If the view is handling a drag that caused objects to be dropped
		/// in <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> when <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" />
		/// was true, this will delete those dropped objects by calling <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />.
		/// </remarks>
		protected override void OnDragLeave(EventArgs e)
		{
			StopHoverTimer();
			StopAutoScroll();
			if (AllowMouse)
			{
				if (PretendsInternalDrag)
				{
					PretendsInternalDrag = false;
					DeleteSelection(Selection);
					AbortTransaction();
					Tool = null;
				}
				else if (IsInternalDragDrop(null))
				{
					(Tool as GoToolDragging)?.ClearDragSelection();
				}
				else
				{
					HideExternalDragImage();
				}
			}
			base.OnDragLeave(e);
		}

		/// <summary>
		/// If the user types an <c>Escape</c> during a drag, we cancel the drag
		/// by calling <see cref="M:Northwoods.Go.GoView.DoCancelMouse" />.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// If the view is handling a drag that caused objects to be dropped
		/// in <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> when <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" />
		/// was true, this will delete those dropped objects by calling <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />,
		/// before calling <see cref="M:Northwoods.Go.GoView.DoCancelMouse" />.
		/// </remarks>
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs evt)
		{
			if (AllowMouse)
			{
				try
				{
					if (evt.EscapePressed)
					{
						StopHoverTimer();
						StopAutoScroll();
						if (PretendsInternalDrag)
						{
							PretendsInternalDrag = false;
							DeleteSelection(Selection);
							AbortTransaction();
						}
						DoCancelMouse();
					}
				}
				catch (Exception ex)
				{
					GoObject.Trace("OnQueryContinueDrag: " + ex.ToString());
					throw;
				}
			}
			base.OnQueryContinueDrag(evt);
		}

		/// <summary>
		/// Handle the DragDrop event by canonicalizing the input event and behaving
		/// differently for internal drag-drops than for drops coming from other windows.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This sets up <see cref="P:Northwoods.Go.GoView.LastInput" /> and then it either calls
		/// <see cref="M:Northwoods.Go.GoView.DoInternalDrop(System.Windows.Forms.DragEventArgs)" /> for drags that started in this view
		/// (because the current tool is an instance of <see cref="T:Northwoods.Go.GoToolDragging" />)
		/// or it calls <see cref="M:Northwoods.Go.GoView.DoExternalDrop(System.Windows.Forms.DragEventArgs)" /> for dragging from another <c>Control</c>.
		/// </remarks>
		protected override void OnDragDrop(DragEventArgs evt)
		{
			StopHoverTimer();
			StopAutoScroll();
			GoInputEventArgs lastInput = LastInput;
			if (AllowMouse)
			{
				try
				{
					Point p = new Point(evt.X, evt.Y);
					lastInput.ViewPoint = PointToClient(p);
					lastInput.DocPoint = ConvertViewToDoc(lastInput.ViewPoint);
					lastInput.Buttons = Control.MouseButtons;
					lastInput.Modifiers = Control.ModifierKeys;
					lastInput.Delta = 0;
					lastInput.Key = Keys.None;
					lastInput.DragEventArgs = evt;
					lastInput.InputState = GoInputState.Finish;
					if (IsInternalDragDrop(evt))
					{
						DoInternalDrop(evt);
					}
					else
					{
						HideExternalDragImage();
						if (PretendsInternalDrag)
						{
							DoMouseUp();
							FinishTransaction("Drop");
						}
						else
						{
							DoExternalDrop(evt);
						}
					}
					PretendsInternalDrag = false;
				}
				catch (Exception ex)
				{
					GoObject.Trace("OnDragDrop: " + ex.ToString());
					throw;
				}
			}
			base.OnDragDrop(evt);
			lastInput.DragEventArgs = null;
		}

		/// <summary>
		/// Handle drag-dropping from within this view.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// <para>
		/// By default this just calls <see cref="M:Northwoods.Go.GoView.DoMouseUp" />.
		/// Override this method to handle all drag-drops that started in this view.
		/// But it is more common to modify the <see cref="T:Northwoods.Go.GoToolDragging" /> tool
		/// or to define a new tool.
		/// </para>
		/// <para>
		/// This is called by <see cref="M:Northwoods.Go.GoView.OnDragDrop(System.Windows.Forms.DragEventArgs)" /> when <see cref="M:Northwoods.Go.GoView.IsInternalDragDrop(System.Windows.Forms.DragEventArgs)" /> is true.
		/// </para>
		/// </remarks>
		protected virtual void DoInternalDrop(DragEventArgs evt)
		{
			DoMouseUp();
		}

		/// <summary>
		/// Handle drag-drops from another Control.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>
		/// an <see cref="T:Northwoods.Go.IGoCollection" /> holding the objects that were copied, if any,
		/// or null otherwise
		/// </returns>
		/// <remarks>
		/// <para>
		/// By default this method gets the drop data as a <see cref="T:Northwoods.Go.GoSelection" />,
		/// and copies the selected objects into this view's document with an
		/// offset that places the primary selection at the drop point.
		/// After calling <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" /> to add a copy
		/// of each object in the selection to this view's document, it selects
		/// all of them in this view and raises an event by calling
		/// <see cref="M:Northwoods.Go.GoView.RaiseExternalObjectsDropped(Northwoods.Go.GoInputEventArgs)" />.
		/// </para>
		/// <para>
		/// This method is commonly overridden in order to handle different
		/// data formats, depending on the expected actual source <c>Control</c>.
		/// </para>
		/// <para>
		/// This method is called by <see cref="M:Northwoods.Go.GoView.OnDragDrop(System.Windows.Forms.DragEventArgs)" /> normally
		/// if <see cref="M:Northwoods.Go.GoView.IsInternalDragDrop(System.Windows.Forms.DragEventArgs)" /> is false, but will also
		/// be called by <see cref="M:Northwoods.Go.GoView.OnDragEnter(System.Windows.Forms.DragEventArgs)" /> if <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" />
		/// is true.  In the latter case, the view's <see cref="P:Northwoods.Go.GoView.Selection" />
		/// should be the newly dropped objects created by this method.
		/// The selection is built between <see cref="E:Northwoods.Go.GoView.SelectionStarting" /> and
		/// <see cref="E:Northwoods.Go.GoView.SelectionFinished" /> events.
		/// </para>
		/// <para>
		/// After the <c>GoObject</c>s have been created and added to the document
		/// as part of the drop and after those objects have been selected,
		/// and when <see cref="P:Northwoods.Go.GoView.ExternalDragDropsOnEnter" /> is false,
		/// this method checks the value of <see cref="M:Northwoods.Go.GoView.DoSelectionDropReject(Northwoods.Go.GoInputEventArgs)" />
		/// to see if it the drop is permitted or should be cancelled.
		/// If it should be cancelled, the selection is deleted.
		/// Otherwise, this method calls <see cref="M:Northwoods.Go.GoView.DoSelectionDropped(Northwoods.Go.GoInputEventArgs)" />
		/// and then <see cref="M:Northwoods.Go.GoView.RaiseExternalObjectsDropped(Northwoods.Go.GoInputEventArgs)" />.
		/// </para>
		/// </remarks>
		protected virtual IGoCollection DoExternalDrop(DragEventArgs evt)
		{
			GoSelection goSelection = evt.Data.GetData(typeof(GoSelection)) as GoSelection;
			if (goSelection == null)
			{
				goSelection = (evt.Data.GetData(Selection.GetType()) as GoSelection);
			}
			if (goSelection != null)
			{
				GoDocument document = Document;
				if (document != null)
				{
					PointF docPoint = LastInput.DocPoint;
					GoObject primary = goSelection.Primary;
					if (primary != null)
					{
						string tname = null;
						bool suspendsRouting = document.SuspendsRouting;
						GoCollection goCollection = new GoCollection();
						goCollection.InternalChecksForDuplicates = false;
						try
						{
							StartTransaction();
							document.SuspendsRouting = true;
							SizeF offset = GoTool.SubtractPoints(docPoint, new PointF(primary.Left + goSelection.HotSpot.Width, primary.Top + goSelection.HotSpot.Height));
							GoCopyDictionary goCopyDictionary = document.CopyFromCollection(goSelection, copyableOnly: false, dragging: true, offset, null);
							foreach (object value in goCopyDictionary.Values)
							{
								GoObject goObject = value as GoObject;
								if (goObject != null && goObject.IsTopLevel && goObject.Document == document)
								{
									goCollection.Add(goObject);
									PointF location = goObject.Location;
									PointF newLoc = SnapPoint(location, goObject);
									goObject.DoMove(this, location, newLoc);
								}
							}
							GoSelection selection = Selection;
							selection.Clear();
							RaiseSelectionStarting();
							GoObject goObject2 = (GoObject)goCopyDictionary[primary];
							if (goObject2 != null && goCollection.Contains(goObject2))
							{
								selection.Add(goObject2);
							}
							foreach (GoObject item in goCollection)
							{
								selection.Add(item);
							}
							RaiseSelectionFinished();
							selection.HotSpot = goSelection.HotSpot;
							if (!ExternalDragDropsOnEnter)
							{
								if (DoSelectionDropReject(LastInput))
								{
									DeleteSelection(Selection);
									tname = null;
								}
								else
								{
									DoSelectionDropped(LastInput);
									tname = "Drop";
									RaiseExternalObjectsDropped(LastInput);
								}
							}
							else
							{
								tname = "Drop";
								RaiseExternalObjectsDropped(LastInput);
							}
							document.ResumeRouting(suspendsRouting, null);
							return goCollection;
						}
						finally
						{
							document.SuspendsRouting = suspendsRouting;
							FinishTransaction(tname);
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ExternalObjectsDropped" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnExternalObjectsDropped(GoInputEventArgs evt)
		{
			if (this.ExternalObjectsDropped != null)
			{
				this.ExternalObjectsDropped(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnExternalObjectsDropped(Northwoods.Go.GoInputEventArgs)" /> to raise a <see cref="E:Northwoods.Go.GoView.ExternalObjectsDropped" /> event.
		/// </summary>
		public void RaiseExternalObjectsDropped(GoInputEventArgs evt)
		{
			OnExternalObjectsDropped(evt);
		}

		/// <summary>
		/// Print this view's document.
		/// </summary>
		/// <remarks>
		/// This is typically called from your File-Print command.
		/// It constructs a <c>PrintDocument</c> that calls
		/// <see cref="M:Northwoods.Go.GoView.PrintDocumentPage(System.Object,System.Drawing.Printing.PrintPageEventArgs)" /> repeatedly to render the printed image.
		/// This <c>PrintDocument</c> is passed to <see cref="M:Northwoods.Go.GoView.PrintShowDialog(System.Drawing.Printing.PrintDocument)" />
		/// to display a <c>PrintDialog</c> so the user can select printing
		/// parameters.
		/// </remarks>
		public virtual void Print()
		{
			try
			{
				PrintDocument printDocument = new PrintDocument();
				printDocument.PrintPage += PrintDocumentPage;
				printDocument.DocumentName = Document.Name;
				if (PrintShowDialog(printDocument) != DialogResult.Cancel)
				{
					printDocument.Print();
				}
			}
			catch (Exception ex)
			{
				GoObject.Trace("Print: " + ex.ToString());
				throw;
			}
			finally
			{
				myPrintInfo = null;
			}
		}

		/// <summary>
		/// Display a printing choices dialog.
		/// </summary>
		/// <param name="pd">a <c>PrintDocument</c> that may be modified</param>
		/// <returns>
		/// a <c>DialogResult</c>; any value other than <c>DialogResult.Cancel</c>
		/// will start the process of printing this document
		/// </returns>
		/// <remarks>
		/// You could override this method to always return <c>DialogResult.OK</c>
		/// if you don't want to display any <c>PrintDialog</c>.
		/// Or, in order to have the printing orientation default to landscape,
		/// with no margins, you could override this method as follows:
		/// </remarks>
		/// <example>
		/// <code>
		///   protected override DialogResult PrintShowDialog(PrintDocument pd) {
		///     pd.DefaultPageSettings.Landscape = true;
		///     pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
		///     return base.PrintShowDialog(pd);
		///   }
		/// </code>
		/// </example>
		/// <seealso cref="M:Northwoods.Go.GoView.PrintPreviewShowDialog(System.Drawing.Printing.PrintDocument)" />
		protected virtual DialogResult PrintShowDialog(PrintDocument pd)
		{
			PrintDialog printDialog = new PrintDialog();
			printDialog.Document = pd;
			printDialog.AllowSomePages = true;
			printDialog.PrinterSettings.MinimumPage = 1;
			printDialog.PrinterSettings.FromPage = 1;
			printDialog.PrinterSettings.ToPage = printDialog.PrinterSettings.MaximumPage;
			return printDialog.ShowDialog();
		}

		/// <summary>
		/// Display a print preview window for this view's document.
		/// </summary>
		/// <remarks>
		/// It constructs a <c>PrintDocument</c> that calls
		/// <see cref="M:Northwoods.Go.GoView.PrintDocumentPage(System.Object,System.Drawing.Printing.PrintPageEventArgs)" /> repeatedly to render the printed image.
		/// This <c>PrintDocument</c> is passed to <see cref="M:Northwoods.Go.GoView.PrintPreviewShowDialog(System.Drawing.Printing.PrintDocument)" />.
		/// </remarks>
		public virtual void PrintPreview()
		{
			try
			{
				PrintDocument printDocument = new PrintDocument();
				printDocument.PrintPage += PrintDocumentPage;
				printDocument.DocumentName = Document.Name;
				PrintPreviewShowDialog(printDocument);
			}
			catch (Exception ex)
			{
				GoObject.Trace("PrintPreview: " + ex.ToString());
				throw;
			}
			finally
			{
				myPrintInfo = null;
			}
		}

		/// <summary>
		/// Display the print preview window.
		/// </summary>
		/// <param name="pd"></param>
		/// <remarks>
		/// This just brings up a <c>PrintPreviewDialog</c>, using
		/// the <paramref name="pd" /> as the <c>PrintDocument</c>.
		/// You may want to override this method in order to modify the <paramref name="pd" />
		/// <c>PrintDocument</c> to alter the default appearance and behavior of the
		/// print preview window.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.PrintShowDialog(System.Drawing.Printing.PrintDocument)" />
		protected virtual void PrintPreviewShowDialog(PrintDocument pd)
		{
			PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
			printPreviewDialog.UseAntiAlias = true;
			printPreviewDialog.Document = pd;
			printPreviewDialog.ShowDialog();
		}

		/// <summary>
		/// This is called repeatedly by the printing process in order to produce the
		/// printed image.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoView.PrintDecoration(System.Drawing.Graphics,System.Drawing.Printing.PrintPageEventArgs,System.Int32,System.Int32,System.Int32,System.Int32)" /> and <see cref="M:Northwoods.Go.GoView.PrintView(System.Drawing.Graphics,System.Drawing.RectangleF)" />.
		/// </remarks>
		public virtual void PrintDocumentPage(object sender, PrintPageEventArgs e)
		{
			Graphics graphics = e.Graphics;
			checked
			{
				if (myPrintInfo == null)
				{
					myPrintInfo = new PrintInfo();
					myPrintInfo.DocRect = new RectangleF(PrintDocumentTopLeft, PrintDocumentSize);
					myPrintInfo.HorizScale = PrintScale;
					myPrintInfo.VertScale = myPrintInfo.HorizScale;
					Rectangle marginBounds = e.MarginBounds;
					myPrintInfo.PrintSize = new SizeF((float)marginBounds.Width / myPrintInfo.HorizScale, (float)marginBounds.Height / myPrintInfo.VertScale);
					if (myPrintInfo.PrintSize.Width > 0f && myPrintInfo.PrintSize.Height > 0f)
					{
						myPrintInfo.NumPagesAcross = (int)Math.Ceiling(myPrintInfo.DocRect.Width / myPrintInfo.PrintSize.Width);
						myPrintInfo.NumPagesDown = (int)Math.Ceiling(myPrintInfo.DocRect.Height / myPrintInfo.PrintSize.Height);
						switch (e.PageSettings.PrinterSettings.PrintRange)
						{
						default:
							myPrintInfo.CurPage = 0;
							break;
						case PrintRange.Selection:
							myPrintInfo.CurPage = 0;
							break;
						case PrintRange.SomePages:
							myPrintInfo.CurPage = e.PageSettings.PrinterSettings.FromPage - 1;
							break;
						}
					}
				}
				if (myPrintInfo.NumPagesAcross <= 0 || myPrintInfo.NumPagesDown <= 0)
				{
					e.HasMorePages = false;
					return;
				}
			}
			int num = myPrintInfo.CurPage % myPrintInfo.NumPagesAcross;
			int num2 = myPrintInfo.CurPage / myPrintInfo.NumPagesAcross;
			PointF pointF = myOrigin;
			float num3 = myHorizScale;
			float num4 = myVertScale;
			Size size = myBorderSize;
			Rectangle rectangle = myDisplayRectangle;
			myOrigin = new PointF(myPrintInfo.DocRect.X + (float)num * myPrintInfo.PrintSize.Width, myPrintInfo.DocRect.Y + (float)num2 * myPrintInfo.PrintSize.Height);
			myHorizScale = myPrintInfo.HorizScale;
			myVertScale = myPrintInfo.VertScale;
			myBorderSize = new Size(e.MarginBounds.X, e.MarginBounds.Y);
			myDisplayRectangle = e.MarginBounds;
			RectangleF rectangleF = new RectangleF(myOrigin.X, myOrigin.Y, Math.Min(myPrintInfo.PrintSize.Width, myPrintInfo.DocRect.Width), Math.Min(myPrintInfo.PrintSize.Height, myPrintInfo.DocRect.Height));
			Rectangle rect = ConvertDocToView(rectangleF);
			try
			{
				PrintDecoration(graphics, e, num, myPrintInfo.NumPagesAcross, num2, myPrintInfo.NumPagesDown);
				graphics.IntersectClip(e.MarginBounds);
				graphics.IntersectClip(rect);
				graphics.TranslateTransform(myDisplayRectangle.X, myDisplayRectangle.Y);
				graphics.ScaleTransform(myHorizScale * myHorizWorld, myVertScale * myVertWorld);
				graphics.TranslateTransform(0f - myOrigin.X, 0f - myOrigin.Y);
				UpdateDelayedSelectionHandles();
				PrintView(graphics, rectangleF);
			}
			finally
			{
				myOrigin = pointF;
				myHorizScale = num3;
				myVertScale = num4;
				myBorderSize = size;
				myDisplayRectangle = rectangle;
			}
			int num5 = 0;
			checked
			{
				switch (e.PageSettings.PrinterSettings.PrintRange)
				{
				default:
					num5 = myPrintInfo.NumPagesAcross * myPrintInfo.NumPagesDown - 1;
					break;
				case PrintRange.Selection:
					num5 = myPrintInfo.NumPagesAcross * myPrintInfo.NumPagesDown - 1;
					break;
				case PrintRange.SomePages:
					num5 = e.PageSettings.PrinterSettings.ToPage - 1;
					break;
				}
				e.HasMorePages = (myPrintInfo.CurPage < num5);
				if (e.HasMorePages)
				{
					myPrintInfo.CurPage++;
				}
				else
				{
					myPrintInfo = null;
				}
			}
		}

		/// <summary>
		/// Render for printing everything you want to display from the view.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="clipRect"></param>
		/// <remarks>
		/// By default this does not print the document paper color or the
		/// view background color.
		/// This calls <see cref="M:Northwoods.Go.GoView.PaintBackgroundDecoration(System.Drawing.Graphics,System.Drawing.RectangleF)" /> and <see cref="M:Northwoods.Go.GoView.PaintObjects(System.Boolean,System.Boolean,System.Drawing.Graphics,System.Drawing.RectangleF)" />,
		/// the latter painting all document objects, and all view objects only if
		/// <see cref="P:Northwoods.Go.GoView.PrintsViewObjects" /> is true.
		/// You can disable printing layers of objects and individual objects by setting
		/// <see cref="T:Northwoods.Go.GoLayer" />.<see cref="P:Northwoods.Go.GoLayer.AllowPrint" /> or
		/// <see cref="T:Northwoods.Go.GoObject" />.<see cref="P:Northwoods.Go.GoObject.Printable" /> to false.
		/// </remarks>
		protected virtual void PrintView(Graphics g, RectangleF clipRect)
		{
			PaintBackgroundDecoration(g, clipRect);
			g.SmoothingMode = SmoothingMode;
			g.TextRenderingHint = TextRenderingHint;
			g.InterpolationMode = InterpolationMode;
			g.CompositingQuality = CompositingQuality;
			g.PixelOffsetMode = PixelOffsetMode;
			PaintObjects(doc: true, PrintsViewObjects, g, clipRect);
		}

		/// <summary>
		/// Draw any printed decorations on the paper that would not
		/// normally be part of a window view.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="e"></param>
		/// <param name="hpnum">the current horizontal page number</param>
		/// <param name="hpmax">the total number of pages to be printed in the horizontal direction</param>
		/// <param name="vpnum">the current vertical page number</param>
		/// <param name="vpmax">the total number of pages to be printed in the vertical direction</param>
		/// <remarks>
		/// By default this just draws little corners for more precise cuts
		/// when piecing all of the printed sheets together.
		/// </remarks>
		protected virtual void PrintDecoration(Graphics g, PrintPageEventArgs e, int hpnum, int hpmax, int vpnum, int vpmax)
		{
			float num = e.MarginBounds.X;
			float num2 = e.MarginBounds.Y;
			float num3 = e.MarginBounds.Width;
			float num4 = e.MarginBounds.Height;
			float num5 = num + num3;
			float num6 = num2 + num4;
			g.DrawLine(GoShape.Pens_Black, num, num2, num + 10f, num2);
			g.DrawLine(GoShape.Pens_Black, num, num2, num, num2 + 10f);
			g.DrawLine(GoShape.Pens_Black, num5, num2, num5 - 10f, num2);
			g.DrawLine(GoShape.Pens_Black, num5, num2, num5, num2 + 10f);
			g.DrawLine(GoShape.Pens_Black, num, num6, num + 10f, num6);
			g.DrawLine(GoShape.Pens_Black, num, num6, num, num6 - 10f);
			g.DrawLine(GoShape.Pens_Black, num5, num6, num5 - 10f, num6);
			g.DrawLine(GoShape.Pens_Black, num5, num6, num5, num6 - 10f);
		}

		/// <summary>
		/// This method is called by the constructor to create a grid object for this view.
		/// </summary>
		/// <returns>A <see cref="T:Northwoods.Go.GoGrid" /></returns>
		/// <remarks>
		/// If you need to use your own subclass of <see cref="T:Northwoods.Go.GoGrid" />, this is
		/// one natural place to create it if you create more than one view.
		/// Otherwise you can just set the <see cref="P:Northwoods.Go.GoView.Grid" /> property.
		/// The constructor will automatically call this method and add the
		/// resulting grid to this view's background layer, as if:
		/// <c>this.BackgroundLayer.Add(g);</c>
		/// </remarks>
		/// <example>
		/// By default this does:
		/// <code>
		///   GoGrid g = new GoGrid();
		///   g.OriginRelative = false;
		///   g.UnboundedSpots = (GoObject.TopLeft | GoObject.BottomRight);
		///   g.Selectable = false;
		///   return g;
		/// </code>
		/// </example>
		public virtual GoGrid CreateGrid()
		{
			return new GoGrid
			{
				OriginRelative = false,
				UnboundedSpots = 10,
				Selectable = false
			};
		}

		private bool ShouldSerializeGridLineColor()
		{
			return GridLineColor != Color.LightGray;
		}

		private void ResetGridLineColor()
		{
			GridLineColor = Color.LightGray;
		}

		private bool ShouldSerializeGridMajorLineColor()
		{
			return GridMajorLineColor != Color.Gray;
		}

		private void ResetGridMajorLineColor()
		{
			GridMajorLineColor = Color.Gray;
		}

		private bool ShouldSerializeGridLineDashPattern()
		{
			float[] gridLineDashPattern = GridLineDashPattern;
			float[] defaultLineDashPattern = GoGrid.DefaultLineDashPattern;
			if (gridLineDashPattern.Length != defaultLineDashPattern.Length)
			{
				return true;
			}
			for (int i = 0; i < gridLineDashPattern.Length; i = checked(i + 1))
			{
				if (gridLineDashPattern[i] != defaultLineDashPattern[i])
				{
					return true;
				}
			}
			return false;
		}

		private void ResetGridLineDashPattern()
		{
			GridLineDashPattern = GoGrid.DefaultLineDashPattern;
		}

		private bool ShouldSerializeGridMajorLineDashPattern()
		{
			float[] gridMajorLineDashPattern = GridMajorLineDashPattern;
			float[] defaultMajorLineDashPattern = GoGrid.DefaultMajorLineDashPattern;
			if (gridMajorLineDashPattern.Length != defaultMajorLineDashPattern.Length)
			{
				return true;
			}
			for (int i = 0; i < gridMajorLineDashPattern.Length; i = checked(i + 1))
			{
				if (gridMajorLineDashPattern[i] != defaultMajorLineDashPattern[i])
				{
					return true;
				}
			}
			return false;
		}

		private void ResetGridMajorLineDashPattern()
		{
			GridMajorLineDashPattern = GoGrid.DefaultMajorLineDashPattern;
		}

		private bool ShouldSerializeGridMajorLineFrequency()
		{
			return GridMajorLineFrequency != new Size(0, 0);
		}

		private void ResetGridMajorLineFrequency()
		{
			GridMajorLineFrequency = new Size(0, 0);
		}

		private bool ShouldSerializeSheetMarginColor()
		{
			return SheetMarginColor != GoSheet.DefaultMarginColor;
		}

		private void ResetSheetMarginColor()
		{
			SheetMarginColor = GoSheet.DefaultMarginColor;
		}

		/// <summary>
		/// Find the nearest <see cref="P:Northwoods.Go.GoView.Grid" /> point to a given point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> in document coordinates.</param>
		/// <returns>A <c>PointF</c> grid point in document coordinates.</returns>
		/// <remarks>This just calls <c>this.Grid.FindNearestGridPoint</c>.</remarks>
		public PointF FindNearestGridPoint(PointF p)
		{
			if (Grid != null)
			{
				return Grid.FindNearestGridPoint(p, null);
			}
			return p;
		}

		/// <summary>
		/// Find the nearest grid point to a given point when <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />
		/// or <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" /> need to snap object locations to a grid.
		/// </summary>
		/// <param name="p">A <c>PointF</c> in document coordinates.</param>
		/// <param name="obj">the <see cref="T:Northwoods.Go.GoObject" /> being moved or resized</param>
		/// <returns>
		/// A <c>PointF</c> grid point in document coordinates, or else
		/// the value of <paramref name="p" /> if the point is not in or near any grid.
		/// </returns>
		/// <remarks>
		/// This method looks at all top-level <see cref="T:Northwoods.Go.IGoDragSnapper" /> objects, from front to back,
		/// in all layers that are part of this view, to see which one has the grid point
		/// closest to the given point <paramref name="p" />.
		/// This calls <see cref="T:Northwoods.Go.IGoDragSnapper" />.<see cref="M:Northwoods.Go.IGoDragSnapper.CanSnapPoint(System.Drawing.PointF,Northwoods.Go.GoObject,Northwoods.Go.GoView)" />
		/// to ask whether a grid object can participate in this calculation.
		/// If a grid's <see cref="P:Northwoods.Go.IGoDragSnapper.SnapOpaque" /> property is true, it stops the
		/// search for any additional <see cref="T:Northwoods.Go.IGoDragSnapper" /> objects that are underneath it.
		/// </remarks>
		public virtual PointF SnapPoint(PointF p, GoObject obj)
		{
			PointF pointF = p;
			float nearestDist = 1E+21f;
			checked
			{
				foreach (GoLayer backward in Layers.Backwards)
				{
					if (backward.CanViewObjects())
					{
						GoLayer.GoLayerCache goLayerCache = backward.FindCache(p);
						if (goLayerCache != null)
						{
							List<IGoDragSnapper> snappers = goLayerCache.Snappers;
							for (int num = snappers.Count - 1; num >= 0; num--)
							{
								GoObject o = (GoObject)snappers[num];
								pointF = snapPoint1(p, obj, o, pointF, ref nearestDist);
								if (nearestDist < 0f)
								{
									return pointF;
								}
							}
						}
						else
						{
							foreach (GoObject backward2 in backward.Backwards)
							{
								pointF = snapPoint1(p, obj, backward2, pointF, ref nearestDist);
								if (nearestDist < 0f)
								{
									return pointF;
								}
							}
						}
					}
				}
				return pointF;
			}
		}

		private PointF snapPoint1(PointF p, GoObject obj, GoObject o, PointF nearest, ref float nearestDist)
		{
			IGoDragSnapper goDragSnapper = o as IGoDragSnapper;
			if (goDragSnapper != null && goDragSnapper.CanSnapPoint(p, obj, this))
			{
				PointF pointF = goDragSnapper.SnapPoint(p, obj, this);
				float num = (p.X - pointF.X) * (p.X - pointF.X) + (p.Y - pointF.Y) * (p.Y - pointF.Y);
				if (num < nearestDist)
				{
					nearest = pointF;
					nearestDist = num;
				}
				if (goDragSnapper.SnapOpaque)
				{
					nearestDist = -1f;
					return nearest;
				}
			}
			GoGroup goGroup = o as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					nearest = snapPoint1(p, obj, item, nearest, ref nearestDist);
					if (nearestDist < 0f)
					{
						return nearest;
					}
				}
				return nearest;
			}
			return nearest;
		}

		/// <summary>
		/// This method is called by the constructor to create a page object for this view.
		/// </summary>
		/// <returns>
		/// a <see cref="T:Northwoods.Go.GoGroup" /> including a bounded, shadowed <see cref="T:Northwoods.Go.GoGrid" /> filled with the
		/// <see cref="P:Northwoods.Go.GoDocument.PaperColor" /> positioned at the origin.
		/// </returns>
		/// <remarks>
		/// If you need to use your own subclass of <see cref="T:Northwoods.Go.GoSheet" />, this is
		/// one natural place to create it if you create more than one view.
		/// Otherwise you can just set the <see cref="P:Northwoods.Go.GoView.Sheet" /> property.
		/// <para>
		/// The <see cref="P:Northwoods.Go.GoView.BackgroundHasSheet" /> setter will automatically call this method
		/// and set the <see cref="P:Northwoods.Go.GoView.Sheet" /> property, which will add that
		/// sheet to this view's background layer, as if:
		/// <c>this.BackgroundLayer.Add(s);</c>
		/// </para>
		/// </remarks>
		/// <example>
		/// By default this does:
		/// <code>
		///   public virtual GoSheet CreateSheet() {
		///     GoSheet sheet = new GoSheet();
		///     sheet.Visible = (this.SheetStyle != GoViewSheetStyle.None);
		///     sheet.Printable = false;
		///     sheet.Selectable = false;
		///     // the following three lines are executed only in WinForms, in a try-catch:
		///       PrintDocument pd = new PrintDocument();
		///       PageSettings ps = pd.DefaultPageSettings;
		///       sheet.UpdateBounds(ps, this.PrintScale);
		///     GoRectangle paper = sheet.Paper;
		///     Color color = this.Document.PaperColor;
		///     if (color == Color.Empty)
		///       color = Color.White;
		///     paper.BrushColor = color;
		///     return sheet;
		///   }
		/// </code>
		/// </example>
		///
		/// <seealso cref="P:Northwoods.Go.GoView.BackgroundHasSheet" />
		public virtual GoSheet CreateSheet()
		{
			GoSheet goSheet = new GoSheet();
			goSheet.Visible = (SheetStyle != GoViewSheetStyle.None);
			goSheet.Printable = false;
			goSheet.Selectable = false;
			try
			{
				PageSettings defaultPageSettings = new PrintDocument().DefaultPageSettings;
				goSheet.UpdateBounds(defaultPageSettings, PrintScale);
			}
			catch (Exception ex)
			{
				GoObject.Trace("CreateSheet: " + ex.ToString());
				goSheet.Bounds = new RectangleF(0f, 0f, 850f, 1100f);
			}
			GoRectangle paper = goSheet.Paper;
			Color color = Document.PaperColor;
			if (color == Color.Empty)
			{
				color = Color.White;
			}
			paper.BrushColor = color;
			return goSheet;
		}

		private bool ShouldSerializeSheetRoom()
		{
			return SheetRoom != new Size(10, 10);
		}

		private void ResetSheetRoom()
		{
			SheetRoom = new Size(10, 10);
		}

		private void CleanUpModalControl()
		{
			if (myEditControl != null && myModalControl != null)
			{
				GoControl goControl = myEditControl;
				Control comp = myModalControl;
				myEditControl = null;
				myModalControl = null;
				goControl.DisposeControl(comp, this);
			}
		}

		/// <summary>
		/// Stop the user's editing an object using the <see cref="P:Northwoods.Go.GoView.EditControl" />.
		/// </summary>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoView.EditControl" /> is non-null, we call <see cref="M:Northwoods.Go.GoControl.DoEndEdit(Northwoods.Go.GoView)" /> on it,
		/// which presumably will call <see cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" /> on the
		/// <see cref="P:Northwoods.Go.GoControl.EditedObject" />.
		/// The responsibility for calling <see cref="M:Northwoods.Go.GoView.RaiseObjectEdited(Northwoods.Go.GoObject)" />
		/// and for finishing any transaction rests with the individual implementations
		/// of <see cref="M:Northwoods.Go.GoObject.DoEndEdit(Northwoods.Go.GoView)" />.
		/// </remarks>
		public virtual void DoEndEdit()
		{
			EditControl?.DoEndEdit(this);
		}

		/// <summary>
		/// Invoke all <see cref="E:Northwoods.Go.GoView.ObjectEdited" /> event handlers.
		/// </summary>
		/// <param name="evt"></param>
		protected virtual void OnObjectEdited(GoSelectionEventArgs evt)
		{
			if (this.ObjectEdited != null)
			{
				this.ObjectEdited(this, evt);
			}
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoView.OnObjectEdited(Northwoods.Go.GoSelectionEventArgs)" /> for the given object
		/// to raise an <see cref="E:Northwoods.Go.GoView.ObjectEdited" /> event.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This method is called by <see cref="M:Northwoods.Go.GoText.DoEndEdit(Northwoods.Go.GoView)" />.
		/// </remarks>
		public void RaiseObjectEdited(GoObject obj)
		{
			OnObjectEdited(new GoSelectionEventArgs(obj));
		}

		private bool ShouldSerializeShadowColor()
		{
			return ShadowColor != Color.FromArgb(127, Color.Gray);
		}

		private void ResetShadowColor()
		{
			ShadowColor = Color.FromArgb(127, Color.Gray);
		}

		/// <summary>
		/// Get a Brush for filling in an object's drop-shadow.
		/// </summary>
		/// <param name="obj">the object casting a shadow; ignored by the default implementation</param>
		/// <returns>A <c>SolidBrush</c> whose color is <see cref="P:Northwoods.Go.GoView.ShadowColor" /></returns>
		public virtual SolidBrush GetShadowBrush(GoObject obj)
		{
			if (myShadowBrush == null || myShadowBrush.Color != ShadowColor)
			{
				if (myShadowBrush != null)
				{
					myShadowBrush.Dispose();
				}
				myShadowBrush = new SolidBrush(ShadowColor);
			}
			return myShadowBrush;
		}

		/// <summary>
		/// Get a Pen for drawing an object's drop-shadow.
		/// </summary>
		/// <param name="obj">the object casting a shadow; ignored by the default implementation</param>
		/// <param name="width">the desired width of the Pen</param>
		/// <returns>A <c>Pen</c> whose color is <see cref="P:Northwoods.Go.GoView.ShadowColor" /></returns>
		public virtual Pen GetShadowPen(GoObject obj, float width)
		{
			if (myShadowPen == null || myShadowPen.Color != ShadowColor || GoShape.GetPenWidth(myShadowPen) != width)
			{
				if (myShadowPen != null)
				{
					myShadowPen.Dispose();
				}
				myShadowPen = GoShape.NewPen(ShadowColor, width);
			}
			return myShadowPen;
		}

		/// <summary>
		/// This just calls <c>Document.CanUndo()</c>.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoDocument.CanUndo" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoView.Undo" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanUndo()
		{
			return Document.CanUndo();
		}

		/// <summary>
		/// This just calls <c>Document.Undo()</c>.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoView.Redo" />
		/// <seealso cref="M:Northwoods.Go.GoView.CanUndo" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void Undo()
		{
			if (CanUndo())
			{
				Document.Undo();
			}
		}

		/// <summary>
		/// This just calls <c>Document.CanRedo()</c>.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoDocument.CanRedo" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoView.Redo" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual bool CanRedo()
		{
			return Document.CanRedo();
		}

		/// <summary>
		/// This just calls <c>Document.Redo()</c>.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoView.Undo" />
		/// <seealso cref="M:Northwoods.Go.GoView.CanRedo" />
		/// <seealso cref="P:Northwoods.Go.GoView.DisableKeys" />
		public virtual void Redo()
		{
			if (CanRedo())
			{
				Document.Redo();
			}
		}

		/// <summary>
		/// This just calls <c>Document.StartTransaction()</c>.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoDocument.StartTransaction" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />
		/// <seealso cref="M:Northwoods.Go.GoView.AbortTransaction" />
		public virtual bool StartTransaction()
		{
			return Document.StartTransaction();
		}

		/// <summary>
		/// This just calls <c>Document.FinishTransaction(tname)</c>.
		/// </summary>
		/// <param name="tname"></param>
		/// <returns><see cref="M:Northwoods.Go.GoDocument.FinishTransaction(System.String)" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoView.StartTransaction" />
		/// <seealso cref="M:Northwoods.Go.GoView.AbortTransaction" />
		public virtual bool FinishTransaction(string tname)
		{
			return Document.FinishTransaction(tname);
		}

		/// <summary>
		/// This just calls <c>Document.AbortTransaction()</c>.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoDocument.AbortTransaction" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoView.StartTransaction" />
		/// <seealso cref="M:Northwoods.Go.GoView.FinishTransaction(System.String)" />
		public virtual bool AbortTransaction()
		{
			return Document.AbortTransaction();
		}
	}
}
