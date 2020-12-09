using System;
using System.ComponentModel;
using System.Drawing;

namespace Northwoods.Go
{
	/// <summary>
	/// Node classes that expect to hold nodes and links should inherit from this class.
	/// </summary>
	/// <remarks>
	/// This base class for <see cref="T:Northwoods.Go.GoSubGraph" /> does nothing but
	/// support graph traversal and the reparenting of links.
	/// Unlike <see cref="T:Northwoods.Go.GoSubGraph" />, it does not support collapse/expand
	/// (<see cref="T:Northwoods.Go.IGoCollapsible" />), a Label, a Port, or a drawn border with margins.
	/// Instead, if you want to implement your own kinds of subgraphs, this may provide
	/// a handy base class without the constraints and assumptions that <see cref="T:Northwoods.Go.GoSubGraph" />
	/// was designed for.
	/// </remarks>
	[Serializable]
	public class GoSubGraphBase : GoNode
	{
		/// <summary>
		/// Gets an enumerator over all of the nodes that have links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// destination links coming into any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Sources" />
		[Description("Gets an enumerator over all of the nodes that have links coming into this node.")]
		public virtual GoNodeNodeEnumerator ExternalSources => GetNodeEnumerator(Search.NodesIn | Search.NotSelf);

		/// <summary>
		/// Gets an enumerator over all of the nodes that have links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that have any
		/// source links going out of any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Destinations" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.Nodes" />
		[Description("Gets an enumerator over all of the nodes that have links going out of this node.")]
		public virtual GoNodeNodeEnumerator ExternalDestinations => GetNodeEnumerator(Search.NodesOut | Search.NotSelf);

		/// <summary>
		/// Gets an enumerator over all of the nodes that are connected to this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all nodes that have ports that are
		/// connected in either direction to any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.Sources" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.Destinations" />
		[Description("Gets an enumerator over all of the nodes that are connected to this node.")]
		public virtual GoNodeNodeEnumerator ExternalNodes => GetNodeEnumerator(Search.NodesIn | Search.NodesOut | Search.NotSelf);

		/// <summary>
		/// Gets an enumerator over all of the links coming into this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that come into
		/// any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.SourceLinks" />
		[Description("Gets an enumerator over all of the links coming into this node.")]
		public virtual GoNodeLinkEnumerator ExternalSourceLinks => GetLinkEnumerator(Search.LinksIn | Search.NotSelf);

		/// <summary>
		/// Gets an enumerator over all of the links going out of this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that go out of
		/// any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.DestinationLinks" />
		[Description("Gets an enumerator over all of the links going out of this node.")]
		public virtual GoNodeLinkEnumerator ExternalDestinationLinks => GetLinkEnumerator(Search.LinksOut | Search.NotSelf);

		/// <summary>
		/// Gets an enumerator over all of the links connected to this node.
		/// </summary>
		/// <remarks>
		/// The enumerator iterates over the set of all links that come into
		/// or go out of any port of this node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.IGoNode.SourceLinks" />
		/// <seealso cref="P:Northwoods.Go.IGoNode.DestinationLinks" />
		[Description("Gets an enumerator over all of the links connected to this node.")]
		public virtual GoNodeLinkEnumerator ExternalLinks => GetLinkEnumerator(Search.LinksIn | Search.LinksOut | Search.NotSelf);

		/// <summary>
		/// This is a static/shared method that is convenient for finding if an object
		/// is part of a <see cref="T:Northwoods.Go.GoSubGraphBase" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>the <see cref="T:Northwoods.Go.GoSubGraphBase" /> that <paramref name="obj" /> is part of,
		/// or null if it is not a child of a subgraph</returns>
		public static GoSubGraphBase FindParentSubGraph(GoObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			for (GoObject parent = obj.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is GoSubGraphBase)
				{
					return (GoSubGraphBase)parent;
				}
			}
			return null;
		}

		private GoNodeNodeEnumerator GetNodeEnumerator(Search s)
		{
			return new GoNodeNodeEnumerator(this, s);
		}

		private GoNodeLinkEnumerator GetLinkEnumerator(Search s)
		{
			return new GoNodeLinkEnumerator(this, s);
		}

		/// <summary>
		/// Change the <see cref="P:Northwoods.Go.GoObject.Parent" /> of an object to be the common subgraph that
		/// contains two given objects, or if there is no such subgraph, add the object to the given layer.
		/// </summary>
		/// <param name="obj">the <see cref="T:Northwoods.Go.GoObject" /> to reparent</param>
		/// <param name="child1">an object that may belong to a <see cref="T:Northwoods.Go.GoSubGraphBase" /></param>
		/// <param name="child2">another object that may belong to a <see cref="T:Northwoods.Go.GoSubGraphBase" /></param>
		/// <param name="behind">whether to add the <paramref name="obj" /> at the beginning of the list
		/// of the subgraph's children (thus behind all other subgraph children), or at the end of the list
		/// (thus appearing in front of all the other subgraph children)</param>
		/// <param name="layer">
		/// the <see cref="T:Northwoods.Go.GoLayer" /> to which the <paramref name="obj" /> should be added,
		/// if there is no common parent <see cref="T:Northwoods.Go.GoSubGraphBase" /> for <paramref name="child1" /> and
		/// <paramref name="child2" />
		/// </param>
		/// <remarks>
		/// All of the arguments to this method should be non-null.
		/// </remarks>
		public static void ReparentToCommonSubGraph(GoObject obj, GoObject child1, GoObject child2, bool behind, GoLayer layer)
		{
			GoSubGraphBase a = FindParentSubGraph(child1);
			GoSubGraphBase b = FindParentSubGraph(child2);
			GoObject goObject = GoObject.FindCommonParent(a, b);
			while (goObject != null && !(goObject is GoSubGraphBase))
			{
				goObject = goObject.Parent;
			}
			GoSubGraphBase goSubGraphBase = goObject as GoSubGraphBase;
			if (obj.Parent == goSubGraphBase && obj.Layer != null)
			{
				return;
			}
			if (obj.Parent == null && obj.Layer == null)
			{
				if (goSubGraphBase != null)
				{
					if (behind)
					{
						goSubGraphBase.InsertBefore(null, obj);
					}
					else
					{
						goSubGraphBase.InsertAfter(null, obj);
					}
				}
				else
				{
					layer.Add(obj);
				}
			}
			else
			{
				GoCollection goCollection = new GoCollection();
				goCollection.Add(obj);
				if (goSubGraphBase != null)
				{
					goSubGraphBase.AddCollection(goCollection, reparentLinks: false);
				}
				else
				{
					layer.AddCollection(goCollection, reparentLinks: false);
				}
			}
		}

		/// <summary>
		/// Make sure each link, either directly in the given collection, or connected to the nodes in
		/// the given collection, belong to the appropriate <see cref="T:Northwoods.Go.GoSubGraphBase" />.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="behind">whether to add the <paramref name="coll" /> at the beginning of the list
		/// of the subgraph's children (thus behind all other subgraph children), or at the end of the list
		/// (thus appearing in front of all the other subgraph children)</param>
		/// <param name="layer">the <see cref="T:Northwoods.Go.GoLayer" /> for links whose ports do not both belong to a <see cref="T:Northwoods.Go.GoSubGraphBase" /></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoSubGraphBase.ReparentToCommonSubGraph(Northwoods.Go.GoObject,Northwoods.Go.GoObject,Northwoods.Go.GoObject,System.Boolean,Northwoods.Go.GoLayer)" /> for each <see cref="T:Northwoods.Go.IGoLink" /> in the
		/// <paramref name="coll" /> collection, or for each <see cref="T:Northwoods.Go.IGoLink" /> connected to each <see cref="T:Northwoods.Go.IGoNode" />
		/// in the <paramref name="coll" /> collection.
		/// </remarks>
		public static void ReparentAllLinksToSubGraphs(IGoCollection coll, bool behind, GoLayer layer)
		{
			GoCollection goCollection = new GoCollection();
			foreach (GoObject item in coll)
			{
				goCollection.Add(item);
			}
			foreach (GoObject item2 in goCollection)
			{
				IGoNode goNode = item2 as IGoNode;
				if (goNode != null)
				{
					foreach (IGoLink link in goNode.Links)
					{
						if (link != null && link.FromPort != null && link.ToPort != null)
						{
							ReparentToCommonSubGraph(link.GoObject, link.FromPort.GoObject, link.ToPort.GoObject, behind, layer);
						}
					}
				}
				else
				{
					IGoPort goPort = item2 as IGoPort;
					if (goPort != null)
					{
						foreach (IGoLink link2 in goPort.Links)
						{
							if (link2 != null && link2.FromPort != null && link2.ToPort != null)
							{
								ReparentToCommonSubGraph(link2.GoObject, link2.FromPort.GoObject, link2.ToPort.GoObject, behind, layer);
							}
						}
					}
					else
					{
						IGoLink goLink = item2 as IGoLink;
						if (goLink != null && goLink.FromPort != null && goLink.ToPort != null)
						{
							ReparentToCommonSubGraph(goLink.GoObject, goLink.FromPort.GoObject, goLink.ToPort.GoObject, behind, layer);
						}
					}
				}
			}
		}

		/// <summary>
		/// Unlike the standard behavior provided by <see cref="T:Northwoods.Go.GoGroup" />'s <see cref="M:Northwoods.Go.GoGroup.PickObjects(System.Drawing.PointF,System.Boolean,Northwoods.Go.IGoCollection,System.Int32)" />,
		/// subgraphs allow the picking of more than one child, if they overlap each other at the given point.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="selectableOnly"></param>
		/// <param name="coll"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		/// <remarks>
		/// If <see cref="M:Northwoods.Go.GoObject.CanView" /> is false for this group, no children are added to the collection.
		/// </remarks>
		public override IGoCollection PickObjects(PointF p, bool selectableOnly, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection();
			}
			if (coll.Count >= max)
			{
				return coll;
			}
			if (!GoObject.ContainsRect(Bounds, p))
			{
				return coll;
			}
			if (!CanView())
			{
				return coll;
			}
			foreach (GoObject backward in base.Backwards)
			{
				GoSubGraphBase goSubGraphBase = backward as GoSubGraphBase;
				if (goSubGraphBase != null)
				{
					goSubGraphBase.PickObjects(p, selectableOnly, coll, max);
				}
				else
				{
					GoObject goObject = backward.Pick(p, selectableOnly);
					if (goObject != null)
					{
						coll.Add(goObject);
						if (coll.Count >= max)
						{
							return coll;
						}
					}
				}
			}
			if (PickableBackground && (!selectableOnly || CanSelect()))
			{
				coll.Add(this);
			}
			return coll;
		}
	}
}
