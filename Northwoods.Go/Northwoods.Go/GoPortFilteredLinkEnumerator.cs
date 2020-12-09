using System;
using System.Collections;
using System.Collections.Generic;

namespace Northwoods.Go
{
	/// <summary>
	/// Iterate over a subset of the links connected to this port,
	/// those either coming into this port or those leaving this port.
	/// </summary>
	/// <remarks>
	/// Although this structure is public, it is rarely used explicitly.
	/// Instead you should use the <c>foreach</c> (or <c>For Each</c>) construct to
	/// iterate through the links of a port.  In C#:
	/// <pre><code>
	/// foreach (IGoLink link in aPort.DestinationLinks) {
	///   . . .
	/// }
	/// </code></pre>
	/// In VB.NET:
	/// <pre><code>
	/// Dim link As IGoLink
	/// For Each link in aPort.DestinationLinks
	///   . . .
	/// Next
	/// </code></pre>
	/// <para>
	/// Remember that you must not modify the graph structure of links and nodes connected to this port,
	/// by adding or removing any links or nodes, while you are iterating over them.
	/// This structure type does not support comparison for equality.
	/// </para>
	/// </remarks>
	public struct GoPortFilteredLinkEnumerator : IEnumerator<IGoLink>, IDisposable, IEnumerator, IEnumerable<IGoLink>, IEnumerable
	{
		private IGoPort myPort;

		private List<IGoLink> myArray;

		private int myIndex;

		private bool myDest;

		object IEnumerator.Current => Current;

		IGoLink IEnumerator<IGoLink>.Current => Current;

		/// <summary>
		///  Gets the current port in the collection of links connected at this port.
		/// </summary>
		public IGoLink Current
		{
			get
			{
				if (myIndex >= 0 && myIndex < myArray.Count)
				{
					return myArray[myIndex];
				}
				throw new InvalidOperationException("GoPort.GoPortFilteredLinkEnumerator is not at a valid position for the List");
			}
		}

		/// <summary>
		/// Gets the first <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to this port.
		/// </summary>
		public IGoLink First
		{
			get
			{
				for (int i = 0; i < myArray.Count; i = checked(i + 1))
				{
					IGoLink goLink = myArray[i];
					if (myDest)
					{
						if (goLink.FromPort == myPort)
						{
							return goLink;
						}
					}
					else if (goLink.ToPort == myPort)
					{
						return goLink;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets the last <see cref="T:Northwoods.Go.IGoLink" />, or null if there are no links connected to this port.
		/// </summary>
		public IGoLink Last
		{
			get
			{
				checked
				{
					for (int i = myArray.Count - 1; i >= 0; i++)
					{
						IGoLink goLink = myArray[i];
						if (myDest)
						{
							if (goLink.FromPort == myPort)
							{
								return goLink;
							}
						}
						else if (goLink.ToPort == myPort)
						{
							return goLink;
						}
					}
					return null;
				}
			}
		}

		internal GoPortFilteredLinkEnumerator(IGoPort p, List<IGoLink> a, bool dest)
		{
			myPort = p;
			myArray = a;
			myIndex = -1;
			myDest = dest;
			Reset();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			GoPortFilteredLinkEnumerator goPortFilteredLinkEnumerator = this;
			goPortFilteredLinkEnumerator.Reset();
			return goPortFilteredLinkEnumerator;
		}

		IEnumerator<IGoLink> IEnumerable<IGoLink>.GetEnumerator()
		{
			GoPortFilteredLinkEnumerator goPortFilteredLinkEnumerator = this;
			goPortFilteredLinkEnumerator.Reset();
			return goPortFilteredLinkEnumerator;
		}

		/// <summary>
		/// Gets an enumerator for iterating over the IGoLinks connected to this port.
		/// </summary>
		public GoPortFilteredLinkEnumerator GetEnumerator()
		{
			GoPortFilteredLinkEnumerator result = this;
			result.Reset();
			return result;
		}

		/// <summary>
		/// Advance the enumerator to the next IGoLink.
		/// </summary>
		/// <returns>True if there is a next IGoLink; false if it has finished iterating over the collection.</returns>
		public bool MoveNext()
		{
			checked
			{
				if (myIndex + 1 < myArray.Count)
				{
					myIndex++;
					IGoLink goLink = myArray[myIndex];
					if (myDest)
					{
						if (goLink.FromPort != myPort)
						{
							return MoveNext();
						}
					}
					else if (goLink.ToPort != myPort)
					{
						return MoveNext();
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Reset the enumerator to its original position.
		/// </summary>
		public void Reset()
		{
			myIndex = -1;
		}

		/// <summary>
		/// There are no resources to clean up.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
