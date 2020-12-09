using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Resources;
using System.Windows.Forms;

namespace Northwoods.Go
{
	/// <summary>
	/// An object that displays a image such as a bitmap or JPEG file.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This <see cref="T:Northwoods.Go.GoObject" /> is responsible for painting an <b>Image</b>.
	/// The <see cref="P:Northwoods.Go.GoImage.ResourceManager" />, <see cref="P:Northwoods.Go.GoImage.Name" />,
	/// <see cref="P:Northwoods.Go.GoImage.ImageList" />, and <see cref="P:Northwoods.Go.GoImage.Index" /> properties
	/// are used by <see cref="M:Northwoods.Go.GoImage.LoadImage" /> in order to procure the
	/// actual <b>Image</b> that is painted.
	/// If you change any of these properties, <see cref="M:Northwoods.Go.GoImage.LoadImage" />
	/// will be called again to find a new image.
	/// </para>
	/// <para>
	/// When you set any of those properties,
	/// if <see cref="P:Northwoods.Go.GoImage.AutoResizes" /> is true (which is the default value),
	/// <see cref="M:Northwoods.Go.GoImage.LoadImage" /> will be called immediately to determine
	/// whether the new <b>Image</b> has a new <b>Size</b>, in order to
	/// resize this <b>GoImage</b> object to the <b>Image</b>'s natural size.
	/// However, when <see cref="P:Northwoods.Go.GoImage.AutoResizes" /> is false, <see cref="M:Northwoods.Go.GoImage.LoadImage" />
	/// will only get called when the <see cref="P:Northwoods.Go.GoImage.Image" /> property is needed
	/// by a call to <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
	/// Hence if you have a large diagram with many different <b>Image</b>s
	/// to be shown, it is advantageous to have GoDiagram only load images
	/// on demand due to painting.  You can achieve this effect by setting
	/// <see cref="P:Northwoods.Go.GoImage.AutoResizes" /> to false and by setting the
	/// <see cref="P:Northwoods.Go.GoObject.Size" /> explicitly to the size that you want,
	/// before setting the <see cref="P:Northwoods.Go.GoImage.Name" /> or <see cref="P:Northwoods.Go.GoImage.Index" /> properties.
	/// </para>
	/// <para>
	/// If <see cref="P:Northwoods.Go.GoImage.Index" /> is non-negative, this tries to display
	/// the corresponding <b>Image</b> in the <see cref="P:Northwoods.Go.GoImage.ImageList" />.
	/// If <see cref="P:Northwoods.Go.GoImage.ImageList" /> is null, this uses the value of
	/// <see cref="P:Northwoods.Go.GoImage.DefaultImageList" /> instead.
	/// </para>
	/// <para>
	/// If <see cref="P:Northwoods.Go.GoImage.Name" /> is non-null and non-empty, this tries
	/// to display the correspondingly named <b>Image</b> in the
	/// <see cref="P:Northwoods.Go.GoImage.ResourceManager" />.
	/// If <see cref="P:Northwoods.Go.GoImage.ResourceManager" /> is null, this uses the value of
	/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" /> instead.
	/// If no <b>Image</b> is found in any <b>ResourceManager</b> for the
	/// given name, the <see cref="P:Northwoods.Go.GoImage.Name" /> is treated as a URI to be gotten
	/// via a <b>WebClient</b> if <see cref="P:Northwoods.Go.GoImage.NameIsUri" /> is true, or as a
	/// file pathname for an image file stored on disk if <see cref="P:Northwoods.Go.GoImage.NameIsUri" />
	/// is false.
	/// </para>
	/// <para>
	/// Finally, even if <see cref="P:Northwoods.Go.GoImage.Image" /> is null, this object will
	/// still be able to paint an image if <see cref="P:Northwoods.Go.GoImage.Index" /> is non-negative
	/// and if it can find an image in the <see cref="T:Northwoods.Go.GoView" />'s <b>ImageList</b>.
	/// </para>
	/// <para>
	/// <b>Image</b>s loaded by <see cref="M:Northwoods.Go.GoImage.LoadImage" /> from a
	/// <b>ResourceManager</b> or <b>ImageList</b> or from a file on disk
	/// or from the web are cached in a static/shared hash table.
	/// Future calls to <see cref="M:Northwoods.Go.GoImage.LoadImage" /> with the same source and name/index
	/// will just share the same <b>Image</b>.
	/// You can clear the whole cache by calling <see cref="M:Northwoods.Go.GoImage.ClearCachedImages" />,
	/// or you can clear individual cache entries by calling one of the
	/// <b>ClearCachedImage</b> overloaded methods.
	/// Call <see cref="M:Northwoods.Go.GoImage.UnloadImage" /> to cause an individual <see cref="T:Northwoods.Go.GoImage" />
	/// to call <see cref="M:Northwoods.Go.GoImage.LoadImage" /> again the next time it needs to show an <b>Image</b>.
	/// </para>
	/// <para>
	/// A newly constructed <see cref="T:Northwoods.Go.GoImage" /> will get its initial value
	/// for <see cref="P:Northwoods.Go.GoImage.ImageList" /> from <see cref="P:Northwoods.Go.GoImage.DefaultImageList" /> and
	/// for <see cref="P:Northwoods.Go.GoImage.ResourceManager" /> from <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" />.
	/// These properties are not serialized and deserialized, although
	/// <see cref="P:Northwoods.Go.GoImage.Name" /> and <see cref="P:Northwoods.Go.GoImage.Index" /> are.
	/// To make sure copy-and-paste works successfully, you should either
	/// set <see cref="P:Northwoods.Go.GoImage.DefaultImageList" /> or <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" />,
	/// or fix up the value of <see cref="P:Northwoods.Go.GoImage.ImageList" /> or <see cref="P:Northwoods.Go.GoImage.ResourceManager" />
	/// in a <see cref="E:Northwoods.Go.GoView.ClipboardPasted" /> event handler or
	/// <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> override.
	/// </para>
	/// </remarks>
	[Serializable]
	public class GoImage : GoObject
	{
		[Serializable]
		internal sealed class GoImageInfo
		{
			public object Source;

			public int Index;

			public string Name;

			public CultureInfo Culture;

			public GoImageInfo()
			{
			}

			public GoImageInfo(GoImageInfo other)
			{
				Source = other.Source;
				Index = other.Index;
				Name = other.Name;
				Culture = other.Culture;
			}

			public override bool Equals(object obj)
			{
				GoImageInfo goImageInfo = obj as GoImageInfo;
				if (goImageInfo == null)
				{
					return false;
				}
				if (Source == goImageInfo.Source && Index == goImageInfo.Index && Name == goImageInfo.Name)
				{
					return Culture == goImageInfo.Culture;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((Source != null) ? Source.GetHashCode() : 0) ^ Index ^ ((Name != null) ? Name.GetHashCode() : 0) ^ Culture.GetHashCode();
			}
		}

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.Image" /> property.
		/// </summary>
		public const int ChangedImage = 1601;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.ResourceManager" /> property.
		/// </summary>
		public const int ChangedResourceManager = 1602;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.Name" /> property.
		/// </summary>
		public const int ChangedName = 1603;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.Alignment" /> property.
		/// </summary>
		public const int ChangedAlignment = 1604;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.AutoResizes" /> property.
		/// </summary>
		public const int ChangedAutoResizes = 1605;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <c>ImageList</c> property.
		/// </summary>
		public const int ChangedImageList = 1606;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <c>Index</c> property.
		/// </summary>
		public const int ChangedIndex = 1607;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.ThrowsExceptions" /> property.
		/// </summary>
		public const int ChangedThrowsExceptions = 1608;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> subhint identifying changes to the value of the <see cref="P:Northwoods.Go.GoImage.NameIsUri" /> property.
		/// </summary>
		public const int ChangedNameIsUri = 1609;

		private const int flagAutoResizes = 1048576;

		private const int flagThrowsExceptions = 2097152;

		private const int flagExceptionTraced = 4194304;

		private const int flagNameIsUri = 8388608;

		internal static GoImageInfo myInfo = new GoImageInfo();

		internal static Dictionary<GoImageInfo, WeakReference> myImages = new Dictionary<GoImageInfo, WeakReference>();

		internal static int myCounter;

		private static ImageList myDefaultImageList;

		private static ResourceManager myDefaultResourceManager;

		private int myAlignment = 2;

		[NonSerialized]
		private ResourceManager myResourceManager = DefaultResourceManager;

		private string myName;

		[NonSerialized]
		private ImageList myImageList = DefaultImageList;

		private int myIndex = -1;

		[NonSerialized]
		private Image myImage;

		/// <summary>
		/// The Image displayed by this GoImage.
		/// </summary>
		/// <value>
		/// If set to null, the next get will call <see cref="M:Northwoods.Go.GoImage.LoadImage" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" /> will draw the Image that is the value of this property.
		/// When an Image is not yet available, it calls <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// and remembers that returned Image as this property's value.
		/// The cached Image value is not serialized.
		/// </para>
		/// <para>
		/// When the <see cref="P:Northwoods.Go.GoImage.Index" /> is non-negative, the Image will be taken
		/// from an <b>ImageList</b>, which is supplied by <see cref="P:Northwoods.Go.GoImage.ImageList" />,
		/// and defaults to <see cref="P:Northwoods.Go.GoImage.DefaultImageList" />.
		/// </para>
		/// </remarks>
		[Category("Appearance")]
		[Description("The Image displayed by this GoImage.")]
		public virtual Image Image
		{
			get
			{
				if (myImage == null)
				{
					myImage = LoadImage();
				}
				return myImage;
			}
			set
			{
				if (myImage == null)
				{
					myImage = LoadImage();
				}
				Image image = myImage;
				if (image != value)
				{
					myImage = value;
					Changed(1601, 0, image, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets the ResourceManager used to look up and load images by Name.
		/// </summary>
		/// <value>
		/// When this value is changed, any cached Image is forgotten so that it can be
		/// reloaded from the new ResourceManager by <see cref="M:Northwoods.Go.GoImage.LoadImage" />.
		/// </value>
		/// <remarks>
		/// The ResourceManager is not serialized; after deserialization, you will
		/// need to reset this property.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The ResourceManager used to look up and load images by Name.")]
		public virtual ResourceManager ResourceManager
		{
			get
			{
				return myResourceManager;
			}
			set
			{
				ResourceManager resourceManager = myResourceManager;
				if (resourceManager != value)
				{
					myResourceManager = value;
					ResetImage();
					Changed(1602, 0, resourceManager, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets the ImageList used to hold images from which one is chosen by index.
		/// </summary>
		/// <value>
		/// When this value is changed, any cached Image is forgotten so that it can be
		/// redrawn from the new ImageList by <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />.
		/// </value>
		/// <remarks>
		/// The ImageList is not serialized; after deserialization, you will need to
		/// reset this property.
		/// If you modify the ImageList's collection of Images to replace an Image that
		/// a GoImage has already loaded, you will need to reset this <see cref="P:Northwoods.Go.GoImage.Image" />
		/// property by setting it to null.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoImage.Index" />
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The ImageList used to hold a collection of images, selected by Index.")]
		public virtual ImageList ImageList
		{
			get
			{
				return myImageList;
			}
			set
			{
				ImageList imageList = myImageList;
				if (imageList != value)
				{
					myImageList = value;
					ResetImage();
					Changed(1606, 0, imageList, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the image in an ImageList.
		/// </summary>
		/// <value>
		/// This zero-based value is an index into an ImageList.
		/// The default value is <c>-1</c>, indicating no image in any ImageList.
		/// </value>
		/// <remarks>
		/// If this value is non-negative, the image will come from the
		/// <see cref="P:Northwoods.Go.GoImage.ImageList" /> value, assuming it is within the bounds
		/// of the ImageList's collection of Images.
		/// If this value is non-negative but <see cref="P:Northwoods.Go.GoImage.ImageList" />
		/// is null, this object will try to draw using the view's ImageList,
		/// <see cref="P:Northwoods.Go.GoView.ImageList" />, if any.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(-1)]
		[Description("The index of the image in an ImageList.")]
		public virtual int Index
		{
			get
			{
				return myIndex;
			}
			set
			{
				int num = myIndex;
				if (num != value)
				{
					myIndex = value;
					ResetImage();
					Changed(1607, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets the resource name or filename for loading images.
		/// </summary>
		/// <value>
		/// A null value causes <see cref="M:Northwoods.Go.GoImage.LoadImage" /> to avoid looking for
		/// an image in a <c>ResourceManager</c> or as a file.
		/// </value>
		/// <remarks>
		/// When this value is changed, any cached Image is forgotten so that it can be
		/// reloaded from the ResourceManager, if any, or from the file system.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("The Resource name or filename for loading images.")]
		public virtual string Name
		{
			get
			{
				return myName;
			}
			set
			{
				string text = myName;
				if (text != value)
				{
					myName = value;
					ResetImage();
					Changed(1603, 0, text, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="P:Northwoods.Go.GoImage.Name" /> property is a URI instead of a disk file path.
		/// </summary>
		/// <value>The default is false.</value>
		/// <remarks>
		/// You might want to set this property to true before setting the <see cref="P:Northwoods.Go.GoImage.Name" /> property,
		/// so that <see cref="M:Northwoods.Go.GoImage.LoadImage" /> doesn't waste time trying to look up the <see cref="P:Northwoods.Go.GoImage.Name" />
		/// as if it were a pathname for a disk file.
		/// </remarks>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Whether the Name refers to a URI instead of to a disk file path.")]
		public virtual bool NameIsUri
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
					ResetImage();
					Changed(1609, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
					UpdateSize();
				}
			}
		}

		/// <summary>
		/// The natural location depends on this object's Alignment.
		/// </summary>
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
		/// Gets or sets the alignment for the image, which governs its <see cref="P:Northwoods.Go.GoImage.Location" />.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoObject.TopLeft" />.
		/// </value>
		[Category("Appearance")]
		[DefaultValue(2)]
		[Description("The image alignment")]
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
					Changed(1604, num, null, GoObject.NullRect, value, null, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the bounds are recalculated when the image changes.
		/// </summary>
		/// <value>
		/// The default value is true.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the bounds are recalculated when the image changes.")]
		public virtual bool AutoResizes
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
					Changed(1605, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to (re-)throw an exception that may occur while trying
		/// to load an Image.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// <para>
		/// You may want to set this to true to help debug why an image does not appear.
		/// <see cref="M:Northwoods.Go.GoImage.LoadImage" /> will first send the exception's message to
		/// trace listeners.  Then if this property is true, the catch handler will
		/// rethrow the exception.
		/// </para>
		/// <para>
		/// This flag may help you debug any image loading exceptions, but there can be
		/// other reasons for an image not to appear.  Perhaps the most common problem
		/// is that information, such as the <see cref="P:Northwoods.Go.GoImage.Name" />, is not present for
		/// <see cref="M:Northwoods.Go.GoImage.LoadImage" /> to be able to find the image data.  This will
		/// occur, for example, if you set the <see cref="P:Northwoods.Go.GoImage.Image" /> property alone
		/// and then serialize/deserialize this <c>GoImage</c>, because <c>Image</c>
		/// data and <c>ResourceManager</c>s are not serialized.
		/// </para>
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether LoadImage throws any exception that occurs trying to load an Image.")]
		public virtual bool ThrowsExceptions
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
					Changed(1608, 0, flag, GoObject.NullRect, 0, value, GoObject.NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the static/shared ImageList for newly constructed GoImage objects.
		/// </summary>
		/// <value>
		/// The initial value is null--no ImageList is used for any <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// calls unless the <see cref="P:Northwoods.Go.GoImage.ImageList" /> property is set for a given
		/// <c>GoImage</c> object.
		/// </value>
		[Description("The initial ImageList for newly constructed GoImage objects.")]
		public static ImageList DefaultImageList
		{
			get
			{
				return myDefaultImageList;
			}
			set
			{
				myDefaultImageList = value;
			}
		}

		/// <summary>
		/// Gets or sets the static/shared ResourceManager for newly constructed GoImage objects.
		/// </summary>
		/// <value>
		/// The initial value is null--no ResourceManager is used for any <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// calls unless the <see cref="P:Northwoods.Go.GoImage.ResourceManager" /> property is set for a given
		/// <c>GoImage</c> object.
		/// </value>
		[Description("The initial ResourceManager for newly constructed GoImage objects.")]
		public static ResourceManager DefaultResourceManager
		{
			get
			{
				return myDefaultResourceManager;
			}
			set
			{
				myDefaultResourceManager = value;
			}
		}

		/// <summary>
		/// The constructor produces an image that is not reshapable by
		/// the user and that automatically recalculates its size when the
		/// <c>Image</c> is replaced.
		/// </summary>
		public GoImage()
		{
			base.InternalFlags &= -33;
			base.InternalFlags |= 1048576;
		}

		private void ResetImage()
		{
			myImage = null;
		}

		/// <summary>
		/// Clear the <see cref="P:Northwoods.Go.GoImage.Image" /> reference that this object maintains,
		/// so that the next use of that property (for example, in <see cref="M:Northwoods.Go.GoImage.Paint(System.Drawing.Graphics,Northwoods.Go.GoView)" />)
		/// will result in a call to <see cref="M:Northwoods.Go.GoImage.LoadImage" />.
		/// </summary>
		/// <remarks>
		/// This is different from setting the <see cref="P:Northwoods.Go.GoImage.Image" /> property to null,
		/// because that change would actually modify the state of this <see cref="T:Northwoods.Go.GoImage" />,
		/// a change that could be undone and redone.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// <seealso cref="M:Northwoods.Go.GoImage.ClearCachedImages" />
		public virtual void UnloadImage()
		{
			ResetImage();
			InvalidateViews();
		}

		/// <summary>
		/// This method is responsible for getting an Image.
		/// </summary>
		/// <returns>
		/// An <c>Image</c>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The normal behavior is to try to use the <see cref="P:Northwoods.Go.GoImage.Index" />
		/// value, if non-negative, to find an Image in the <see cref="P:Northwoods.Go.GoImage.ImageList" />.
		/// If there is no ImageList, we try using the <see cref="P:Northwoods.Go.GoImage.DefaultImageList" />.
		/// </para>
		/// <para>
		/// If the index value is negative, we use the <see cref="P:Northwoods.Go.GoImage.Name" />
		/// to get an Image object from the <see cref="P:Northwoods.Go.GoImage.ResourceManager" />.
		/// If there is no ResourceManager, we try using the
		/// <see cref="P:Northwoods.Go.GoImage.DefaultResourceManager" />, and if that fails, we
		/// try treating the name as a filename.
		/// </para>
		/// <para>
		/// The resulting Image is normally cached by this object as the
		/// <see cref="P:Northwoods.Go.GoImage.Image" /> property.  If it loads the image from a file,
		/// the file may remain locked until the image is garbage collected.
		/// </para>
		/// <para>
		/// If an exception occurs trying to load an image, the message
		/// is sent to trace listeners.  Also, if <see cref="P:Northwoods.Go.GoImage.ThrowsExceptions" />
		/// is true, the catch handler will rethrow the exception.
		/// If the <see cref="P:Northwoods.Go.GoImage.Index" /> is out-of-bounds for the <see cref="P:Northwoods.Go.GoImage.ImageList" />,
		/// or if the <see cref="P:Northwoods.Go.GoImage.Name" /> is not found in the <see cref="P:Northwoods.Go.GoImage.ResourceManager" />,
		/// this will fail silently--no exception is thrown.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.UnloadImage" />
		public virtual Image LoadImage()
		{
			int index = Index;
			if (index >= 0)
			{
				Image image = null;
				ImageList imageList = ImageList;
				if (imageList != null)
				{
					image = FindCachedImage(imageList, index, null, CultureInfo.InvariantCulture);
					if (image != null)
					{
						return image;
					}
				}
				ImageList defaultImageList = DefaultImageList;
				if (defaultImageList != null && defaultImageList != imageList)
				{
					image = FindCachedImage(defaultImageList, index, null, CultureInfo.InvariantCulture);
					if (image != null)
					{
						return image;
					}
				}
				if (imageList != null && index < imageList.Images.Count)
				{
					image = imageList.Images[index];
				}
				if (image != null)
				{
					SaveCachedImage(imageList, index, null, CultureInfo.InvariantCulture, image);
					return image;
				}
				if (defaultImageList != null && defaultImageList != imageList && index < defaultImageList.Images.Count)
				{
					image = defaultImageList.Images[index];
				}
				if (image != null)
				{
					SaveCachedImage(defaultImageList, index, null, CultureInfo.InvariantCulture, image);
					return image;
				}
				if (image != null)
				{
					return image;
				}
			}
			string name = Name;
			if (name != null && name != "")
			{
				Image image2 = null;
				ResourceManager resourceManager = ResourceManager;
				if (resourceManager != null)
				{
					image2 = FindCachedImage(resourceManager, 0, name, CultureInfo.CurrentCulture);
					if (image2 != null)
					{
						return image2;
					}
				}
				ResourceManager defaultResourceManager = DefaultResourceManager;
				if (defaultResourceManager != null && defaultResourceManager != resourceManager)
				{
					image2 = FindCachedImage(defaultResourceManager, 0, name, CultureInfo.CurrentCulture);
					if (image2 != null)
					{
						return image2;
					}
				}
				image2 = FindCachedImage(null, 0, name, CultureInfo.InvariantCulture);
				if (image2 != null)
				{
					return image2;
				}
				if (resourceManager != null)
				{
					try
					{
						object @object = resourceManager.GetObject(name, CultureInfo.CurrentCulture);
						image2 = ConvertObjectToImage(@object);
					}
					catch (MissingManifestResourceException)
					{
					}
				}
				if (image2 != null)
				{
					SaveCachedImage(resourceManager, 0, name, CultureInfo.CurrentCulture, image2);
					return image2;
				}
				if (defaultResourceManager != null && defaultResourceManager != resourceManager)
				{
					try
					{
						object object2 = defaultResourceManager.GetObject(name, CultureInfo.CurrentCulture);
						image2 = ConvertObjectToImage(object2);
					}
					catch (MissingManifestResourceException)
					{
					}
				}
				if (image2 != null)
				{
					SaveCachedImage(defaultResourceManager, 0, name, CultureInfo.CurrentCulture, image2);
					return image2;
				}
				try
				{
					if (NameIsUri)
					{
						WebClient webClient = new WebClient();
						Stream stream = webClient.OpenRead(name);
						image2 = Image.FromStream(stream);
						stream.Close();
						webClient.Dispose();
					}
					else
					{
						image2 = Image.FromFile(name);
					}
					base.InternalFlags &= -4194305;
				}
				catch (Exception ex3)
				{
					if ((base.InternalFlags & 0x400000) == 0)
					{
						base.InternalFlags |= 4194304;
						GoObject.Trace("GoImage.LoadImage: " + ex3.ToString());
					}
					if (ThrowsExceptions)
					{
						throw;
					}
				}
				if (image2 != null)
				{
					SaveCachedImage(null, 0, name, CultureInfo.InvariantCulture, image2);
					return image2;
				}
			}
			return null;
		}

		private Image ConvertObjectToImage(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			Image image = obj as Image;
			if (image != null)
			{
				return image;
			}
			Icon icon = obj as Icon;
			if (icon != null)
			{
				return ConvertIconToImage(icon);
			}
			byte[] array = obj as byte[];
			if (array != null)
			{
				return Image.FromStream(new MemoryStream(array));
			}
			return null;
		}

		private static Image ConvertIconToImage(Icon icon)
		{
			return icon.ToBitmap();
		}

		private static Image FindCachedImage(object src, int idx, string name, CultureInfo cinfo)
		{
			lock (myImages)
			{
				myInfo.Source = src;
				myInfo.Index = idx;
				myInfo.Name = name;
				myInfo.Culture = cinfo;
				myImages.TryGetValue(myInfo, out WeakReference value);
				if (value != null && value.IsAlive)
				{
					Image image = value.Target as Image;
					if (image != null)
					{
						return image;
					}
				}
				return null;
			}
		}

		private static void SaveCachedImage(object src, int idx, string name, CultureInfo cinfo, Image img)
		{
			lock (myImages)
			{
				myInfo.Source = src;
				myInfo.Index = idx;
				myInfo.Name = name;
				myInfo.Culture = cinfo;
				myImages[new GoImageInfo(myInfo)] = new WeakReference(img);
				CleanInfos();
			}
		}

		/// <summary>
		/// Clear out GoDiagram's internal cache of loaded Images.
		/// </summary>
		/// <remarks>
		/// Calling this method does not modify any existing <see cref="T:Northwoods.Go.GoImage" />,
		/// so no Images will be "lost".  However, future calls to <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// will have to re-load all named images from <b>ResourceManager</b>s or from disk,
		/// or (in Windows Forms) re-load all indexed images from <b>ImageList</b>s.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.UnloadImage" />
		/// <seealso cref="M:Northwoods.Go.GoImage.ClearCachedImage(System.Resources.ResourceManager,System.String,System.Globalization.CultureInfo)" />
		/// <seealso cref="M:Northwoods.Go.GoImage.ClearCachedImage(System.String)" />
		public static void ClearCachedImages()
		{
			lock (myImages)
			{
				myImages.Clear();
			}
		}

		/// <summary>
		/// Remove any cached image identified by a ResourceManager and a name in a particular culture.
		/// </summary>
		/// <param name="resmgr">a <b>ResourceManager</b></param>
		/// <param name="name">the name of a particular image object;
		/// if this value is null, this clears all cached images associated
		/// with the <paramref name="resmgr" /></param>
		/// <param name="cinfo">a <b>CultureInfo</b></param>
		/// <remarks>
		/// Calling this method does not modify any existing <see cref="T:Northwoods.Go.GoImage" />,
		/// so no Images will be "lost".  However, future calls to <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// will have to re-load any image with the given name from the given ResourceManager.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.UnloadImage" />
		public static void ClearCachedImage(ResourceManager resmgr, string name, CultureInfo cinfo)
		{
			if (resmgr != null)
			{
				lock (myImages)
				{
					if (name != null)
					{
						myInfo.Source = resmgr;
						myInfo.Index = 0;
						myInfo.Name = name;
						myInfo.Culture = cinfo;
						myImages.Remove(myInfo);
					}
					else
					{
						List<GoImageInfo> list = new List<GoImageInfo>();
						foreach (KeyValuePair<GoImageInfo, WeakReference> myImage2 in myImages)
						{
							GoImageInfo key = myImage2.Key;
							if (key.Source == resmgr)
							{
								list.Add(key);
							}
						}
						foreach (GoImageInfo item in list)
						{
							myImages.Remove(item);
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove any cached image identified by a pathname on disk.
		/// </summary>
		/// <param name="path">a filename</param>
		/// <remarks>
		/// Calling this method does not modify any existing <see cref="T:Northwoods.Go.GoImage" />,
		/// so no Images will be "lost".  However, future calls to <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// will have to re-load from disk any image with the given file pathname.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.UnloadImage" />
		public static void ClearCachedImage(string path)
		{
			lock (myImages)
			{
				myInfo.Source = null;
				myInfo.Index = 0;
				myInfo.Name = path;
				myInfo.Culture = CultureInfo.InvariantCulture;
				myImages.Remove(myInfo);
			}
		}

		/// <summary>
		/// Remove any cached image identified by an ImageList and an index.
		/// </summary>
		/// <param name="imglist">an <b>ImageList</b></param>
		/// <param name="idx">the non-negative index of a particular image;
		/// but if this value is negative, this clears all cached images associated
		/// with the <paramref name="imglist" /></param>
		/// <remarks>
		/// Calling this method does not modify any existing <see cref="T:Northwoods.Go.GoImage" />,
		/// so no Images will be "lost".  However, future calls to <see cref="M:Northwoods.Go.GoImage.LoadImage" />
		/// will have to re-load from the given ImageList an image at the given index.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoImage.UnloadImage" />
		public static void ClearCachedImage(ImageList imglist, int idx)
		{
			if (imglist != null)
			{
				lock (myImages)
				{
					if (idx >= 0)
					{
						myInfo.Source = imglist;
						myInfo.Index = idx;
						myInfo.Name = null;
						myInfo.Culture = CultureInfo.InvariantCulture;
						myImages.Remove(myInfo);
					}
					else
					{
						List<GoImageInfo> list = new List<GoImageInfo>();
						foreach (KeyValuePair<GoImageInfo, WeakReference> myImage2 in myImages)
						{
							GoImageInfo key = myImage2.Key;
							if (key.Source == imglist)
							{
								list.Add(key);
							}
						}
						foreach (GoImageInfo item in list)
						{
							myImages.Remove(item);
						}
					}
				}
			}
		}

		internal static int CleanInfos()
		{
			if (checked(myCounter++) < 100)
			{
				return myImages.Count;
			}
			myCounter = 0;
			List<GoImageInfo> list = new List<GoImageInfo>();
			foreach (KeyValuePair<GoImageInfo, WeakReference> myImage2 in myImages)
			{
				if (!myImage2.Value.IsAlive)
				{
					list.Add(myImage2.Key);
				}
			}
			foreach (GoImageInfo item in list)
			{
				myImages.Remove(item);
			}
			return myImages.Count;
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

		private void UpdateSize()
		{
			if (!base.Initializing && AutoResizes)
			{
				try
				{
					UpdateSize2();
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		private void UpdateSize2()
		{
			Image image = Image;
			if (image != null)
			{
				SizeF size = base.Size;
				SizeF sizeF = new SizeF(image.Size.Width, image.Size.Height);
				if (size != sizeF)
				{
					SetSizeKeepingLocation(sizeF);
				}
			}
		}

		/// <summary>
		/// Draw this Image within the bounds of this object.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="view"></param>
		/// <remarks>
		/// If the value of <see cref="P:Northwoods.Go.GoImage.Image" /> is null and the value
		/// of <see cref="P:Northwoods.Go.GoImage.Index" /> is non-negative, we try to draw an <c>Image</c>
		/// from the view's <see cref="P:Northwoods.Go.GoView.ImageList" />.
		/// </remarks>
		public override void Paint(Graphics g, GoView view)
		{
			RectangleF bounds = Bounds;
			Image image = Image;
			int index = Index;
			if (image == null && index >= 0)
			{
				ImageList imageList = view.ImageList;
				if (imageList != null && index < imageList.Images.Count)
				{
					image = imageList.Images[index];
				}
			}
			if (image != null)
			{
				try
				{
					if (Shadowed)
					{
						SizeF shadowOffset = GetShadowOffset(view);
						ColorMatrix colorMatrix = new ColorMatrix();
						colorMatrix.Matrix00 = 0f;
						colorMatrix.Matrix11 = 0f;
						colorMatrix.Matrix22 = 0f;
						SolidBrush solidBrush = GetShadowBrush(view) as SolidBrush;
						if (solidBrush != null)
						{
							Color color = solidBrush.Color;
							colorMatrix.Matrix30 = (float)(int)color.R / 255f;
							colorMatrix.Matrix31 = (float)(int)color.G / 255f;
							colorMatrix.Matrix32 = (float)(int)color.B / 255f;
							colorMatrix.Matrix33 = (float)(int)color.A / 255f;
						}
						else
						{
							colorMatrix.Matrix30 = 0.5f;
							colorMatrix.Matrix31 = 0.5f;
							colorMatrix.Matrix32 = 0.5f;
							colorMatrix.Matrix33 = 0.5f;
						}
						ImageAttributes imageAttributes = new ImageAttributes();
						imageAttributes.SetColorMatrix(colorMatrix);
						g.DrawImage(image, checked(new Rectangle((int)(bounds.X + shadowOffset.Width), (int)(bounds.Y + shadowOffset.Height), (int)bounds.Width, (int)bounds.Height)), 0, 0, image.Size.Width, image.Size.Height, GraphicsUnit.Pixel, imageAttributes);
					}
					g.DrawImage(image, bounds);
					base.InternalFlags &= -4194305;
				}
				catch (Exception ex)
				{
					if ((base.InternalFlags & 0x400000) == 0)
					{
						base.InternalFlags |= 4194304;
						GoObject.Trace("GoImage.Paint: " + ex.ToString());
					}
					if (ThrowsExceptions)
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Consider any shadow when calculating the actual paint bounds.
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
			return rect;
		}

		/// <summary>
		/// Performs changes for undo and redo.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public override void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.SubHint)
			{
			case 1601:
				Image = (Image)e.GetValue(undo);
				break;
			case 1602:
				ResourceManager = (ResourceManager)e.GetValue(undo);
				break;
			case 1603:
				Name = (string)e.GetValue(undo);
				break;
			case 1604:
				Alignment = e.GetInt(undo);
				break;
			case 1605:
				AutoResizes = (bool)e.GetValue(undo);
				break;
			case 1606:
				ImageList = (ImageList)e.GetValue(undo);
				break;
			case 1607:
				Index = e.GetInt(undo);
				break;
			case 1608:
				ThrowsExceptions = (bool)e.GetValue(undo);
				break;
			case 1609:
				NameIsUri = (bool)e.GetValue(undo);
				break;
			default:
				base.ChangeValue(e, undo);
				break;
			}
		}
	}
}
