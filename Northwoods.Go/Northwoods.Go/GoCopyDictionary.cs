using System;
using System.Collections;

namespace Northwoods.Go
{
	/// <summary>
	/// This class is used to remember the mapping of original objects to copied
	/// objects in <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
	/// </summary>
	[Serializable]
	public class GoCopyDictionary : Hashtable
	{
		private IGoCollection mySourceCollection;

		private GoDocument myDestinationDocument;

		private GoCopyDelayedsCollection myDelayeds;

		/// <summary>
		/// Gets or sets the collection from which we are copying.
		/// </summary>
		public virtual IGoCollection SourceCollection
		{
			get
			{
				return mySourceCollection;
			}
			set
			{
				mySourceCollection = value;
			}
		}

		/// <summary>
		/// Gets or sets the document collection into which we are making copies.
		/// </summary>
		public virtual GoDocument DestinationDocument
		{
			get
			{
				return myDestinationDocument;
			}
			set
			{
				myDestinationDocument = value;
			}
		}

		/// <summary>
		/// Gets or sets the copied object in the destination for an object in the source.
		/// </summary>
		/// <remarks>
		/// This is an indexed property.
		/// If the key is null, getting this property returns null;
		/// When setting, if the key is null, nothing happens.
		/// </remarks>
		public override object this[object key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				return base[key];
			}
			set
			{
				if (key != null)
				{
					base[key] = value;
				}
			}
		}

		/// <summary>
		/// Get the collection that holds objects that need attention
		/// after the first phase of copying.
		/// </summary>
		public GoCopyDelayedsCollection Delayeds
		{
			get
			{
				if (myDelayeds == null)
				{
					myDelayeds = new GoCopyDelayedsCollection();
				}
				return myDelayeds;
			}
		}

		/// <summary>
		/// Return a copied object for an object (first pass only).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// First this method looks up <paramref name="obj" /> in its mapping
		/// table--if it finds an object, it returns it, because presumably it
		/// has already been copied.
		/// Otherwise it calls <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" /> in order to
		/// make a copy of <paramref name="obj" />.
		/// If <paramref name="obj" /> is null, this returns null.
		/// </para>
		/// <para>
		/// The primary use of this method is to be called by <see cref="T:Northwoods.Go.GoDocument" />'s
		/// <c>GoDocument.CopyFromCollection(IGoCollection, bool, bool, SizeF, GoCopyDictionary)</c> method.
		/// This is also typically called within a <see cref="M:Northwoods.Go.GoObject.CopyObject(Northwoods.Go.GoCopyDictionary)" />
		/// override method to make a copy of a referenced object that is not
		/// a child of the copied object.
		/// </para>
		/// <para>
		/// If you don't call this in the context of a
		/// <c>GoDocument.CopyFromCollection(IGoCollection, bool, bool, SizeF, GoCopyDictionary)</c> method,
		/// be sure to call <see cref="M:Northwoods.Go.GoCopyDictionary.FinishDelayedCopies" /> afterwards to perform the
		/// second copying pass, which resolves references and can do other copying work
		/// as implemented by <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.CopyObjectDelayed(Northwoods.Go.GoCopyDictionary,Northwoods.Go.GoObject)" />.
		/// </para>
		/// <para>
		/// The easiest way to make a full copy of an object in a document is to call
		/// <see cref="T:Northwoods.Go.GoDocument" />.<see cref="M:Northwoods.Go.GoDocument.AddCopy(Northwoods.Go.GoObject,System.Drawing.PointF)" />, which calls <c>CopyFromCollection</c>.
		/// However, if you want to create a copy of an object without adding it to a document,
		/// you can call <see cref="M:Northwoods.Go.GoCopyDictionary.CopyComplete(Northwoods.Go.GoObject)" />, which performs both copying passes.
		/// </para>
		/// </remarks>
		public virtual GoObject Copy(GoObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			GoObject goObject = this[obj] as GoObject;
			if (goObject == null)
			{
				goObject = obj.CopyObject(this);
			}
			return goObject;
		}

		/// <summary>
		/// Perform calls to <see cref="M:Northwoods.Go.GoObject.CopyObjectDelayed(Northwoods.Go.GoCopyDictionary,Northwoods.Go.GoObject)" />
		/// for any objects that were added to <see cref="P:Northwoods.Go.GoCopyDictionary.Delayeds" />
		/// during earlier calls to <see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" />.
		/// </summary>
		/// <remarks>
		/// This is primarily called by <see cref="T:Northwoods.Go.GoDocument" />'s
		/// <c>GoDocument.CopyFromCollection(IGoCollection, bool, bool, SizeF, GoCopyDictionary)</c> method.
		/// </remarks>
		public virtual void FinishDelayedCopies()
		{
			if (myDelayeds != null)
			{
				foreach (object delayed in Delayeds)
				{
					GoObject goObject = delayed as GoObject;
					if (goObject != null)
					{
						GoObject goObject2 = this[goObject] as GoObject;
						if (goObject2 != null)
						{
							goObject.CopyObjectDelayed(this, goObject2);
						}
					}
				}
				Delayeds.Clear();
			}
		}

		/// <summary>
		/// This convenience method performs both copy phases making a
		/// copy of a single object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>null, if <paramref name="obj" /> is null, else the result of <see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" />
		/// after a call to <see cref="M:Northwoods.Go.GoCopyDictionary.FinishDelayedCopies" /></returns>
		/// <seealso cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" />
		public GoObject CopyComplete(GoObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			GoObject result = Copy(obj);
			FinishDelayedCopies();
			return result;
		}
	}
}
