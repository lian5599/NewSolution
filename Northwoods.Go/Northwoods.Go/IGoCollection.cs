using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies a collection of <see cref="T:Northwoods.Go.GoObject" />.
	/// </summary>
	/// <remarks>
	/// Classes implementing <c>IGoCollection</c> need not maintain an indexing
	/// of its objects--in other words, they need not implement an <c>IList</c> of <c>GoObject</c>.
	/// But at any one time each collection implementing this interface should maintain
	/// an ordering of all of its items, so that the enumerator will iterate
	/// "forwards" over the list, and so that the <see cref="P:Northwoods.Go.IGoCollection.Backwards" /> enumerable's
	/// enumerator will iterate "backwards" over that list.
	/// This ordering should be guaranteed to remain stable as long as the collection
	/// is unchanged.  However, a call to <c>Add</c> or <c>Remove</c>
	/// may cause the ordering of the items to be altered.
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoCollection" />
	/// <seealso cref="T:Northwoods.Go.GoDocument" />
	/// <seealso cref="T:Northwoods.Go.GoLayer" />
	/// <seealso cref="T:Northwoods.Go.GoGroup" />
	public interface IGoCollection : ICollection<GoObject>, IEnumerable<GoObject>, IEnumerable
	{
		/// <summary>
		/// This predicate is true when there are no objects in this collection.
		/// </summary>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the <see cref="T:Northwoods.Go.GoObject" />s in reverse order.
		/// </summary>
		/// <remarks>
		/// The regular enumerator iterates over the <see cref="T:Northwoods.Go.GoObject" />s in this collection in forward order.
		/// </remarks>
		IEnumerable<GoObject> Backwards
		{
			get;
		}

		/// <summary>
		/// Returns a newly allocated array of all of the GoObjects in the collection.
		/// </summary>
		GoObject[] CopyArray();
	}
}
