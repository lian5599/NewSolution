using System.Collections.Generic;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// This interface represents an abstract connection point for a link to
	/// be attached to a node.
	/// </summary>
	/// <remarks>
	/// Every <c>IGoPort</c> also implements <see cref="P:Northwoods.Go.IGoGraphPart.GoObject" />,
	/// <see cref="P:Northwoods.Go.IGoGraphPart.UserFlags" />, and <see cref="P:Northwoods.Go.IGoGraphPart.UserObject" />.
	/// </remarks>
	/// <seealso cref="T:Northwoods.Go.GoPort" />
	/// <seealso cref="T:Northwoods.Go.IGoNode" />
	/// <seealso cref="T:Northwoods.Go.IGoLink" />
	public interface IGoPort : IGoGraphPart
	{
		/// <summary>
		/// Gets an enumerator over all of the links connected at this port.
		/// </summary>
		IEnumerable<IGoLink> Links
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the links coming into this port.
		/// </summary>
		/// <remarks>
		/// Each source link's <see cref="P:Northwoods.Go.IGoLink.ToPort" /> will be this port.
		/// </remarks>
		IEnumerable<IGoLink> SourceLinks
		{
			get;
		}

		/// <summary>
		/// Gets an enumerator over all of the links going out of this port.
		/// </summary>
		/// <remarks>
		/// Each destination link's <see cref="P:Northwoods.Go.IGoLink.FromPort" /> will be this port.
		/// </remarks>
		IEnumerable<IGoLink> DestinationLinks
		{
			get;
		}

		/// <summary>
		/// Gets the total number of links connected at this port.
		/// </summary>
		int LinksCount
		{
			get;
		}

		/// <summary>
		/// Gets the number of links coming into this port.
		/// </summary>
		/// <remarks>
		/// This is the number of links whose <see cref="P:Northwoods.Go.IGoLink.ToPort" />
		/// is this port.
		/// </remarks>
		int SourceLinksCount
		{
			get;
		}

		/// <summary>
		/// Gets the number of links going out of this port.
		/// </summary>
		/// <remarks>
		/// This is the number of links whose <see cref="P:Northwoods.Go.IGoLink.FromPort" />
		/// is this port.
		/// </remarks>
		int DestinationLinksCount
		{
			get;
		}

		/// <summary>
		/// Gets the node that this port is part of.
		/// </summary>
		/// <remarks>
		/// If this port is not part of any node, this should return null.
		/// If this port is part of a node that in turn is part of another node,
		/// this should return the highest-level (most encompassing) node.
		/// </remarks>
		IGoNode Node
		{
			get;
		}

		/// <summary>
		/// Add a link whose <see cref="P:Northwoods.Go.IGoLink.ToPort" /> should be this port.
		/// </summary>
		/// <param name="l"></param>
		void AddSourceLink(IGoLink l);

		/// <summary>
		/// Add a link whose <see cref="P:Northwoods.Go.IGoLink.FromPort" /> should be this port.
		/// </summary>
		/// <param name="l"></param>
		void AddDestinationLink(IGoLink l);

		/// <summary>
		/// Remove a link from the collection of links connected to this port.
		/// </summary>
		/// <param name="l"></param>
		void RemoveLink(IGoLink l);

		/// <summary>
		/// This predicate is true if the given link is connected to this port.
		/// </summary>
		/// <param name="l"></param>
		/// <returns></returns>
		bool ContainsLink(IGoLink l);

		/// <summary>
		/// Remove all links connected at this port.
		/// </summary>
		void ClearLinks();

		/// <summary>
		/// Returns a newly allocated array containing references to all of the
		/// links connected to this port.
		/// </summary>
		IGoLink[] CopyLinksArray();

		/// <summary>
		/// This predicate should be true when it is valid to create a link from
		/// this port to another port.
		/// </summary>
		/// <param name="toPort"></param>
		/// <returns></returns>
		bool IsValidLink(IGoPort toPort);

		/// <summary>
		/// This predicate should be true if, by itself, there is no known
		/// reason why one couldn't create a valid link from this port to some port.
		/// </summary>
		bool CanLinkFrom();

		/// <summary>
		/// This predicate should be true if, by itself, there is no known
		/// reason why one couldn't create a valid link from some port to this one.
		/// </summary>
		bool CanLinkTo();

		/// <summary>
		/// This method is called when a link connected to this port is changed.
		/// </summary>
		/// <param name="link"></param>
		/// <param name="subhint"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		void OnLinkChanged(IGoLink link, int subhint, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect);
	}
}
