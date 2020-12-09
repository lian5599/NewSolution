#define TRACE
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Northwoods.Go
{
	/// <summary>
	/// This class represents a model for a <see cref="T:Northwoods.Go.GoView" />, holding <see cref="T:Northwoods.Go.GoObject" />s
	/// to be displayed, organized into <see cref="T:Northwoods.Go.GoLayer" />s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To show something in a <see cref="T:Northwoods.Go.GoView" />, you will need to construct and initialize a
	/// <see cref="T:Northwoods.Go.GoObject" /> (typically a class inheriting from <see cref="T:Northwoods.Go.GoNode" /> or
	/// <see cref="T:Northwoods.Go.GoLink" /> or <see cref="T:Northwoods.Go.GoLabeledLink" />), and then add it to the view's
	/// <see cref="P:Northwoods.Go.GoView.Document" />, which will be an instance of <c>GoDocument</c>.
	/// </para>
	/// <para>
	/// Please note that a <c>GoDocument</c> does not require the existence of any <see cref="T:Northwoods.Go.GoView" />s.
	/// A document may have zero, one, or many views displaying it simultaneously.
	/// A document and all of its <see cref="T:Northwoods.Go.GoObject" />s must not have any references to any
	/// <see cref="T:Northwoods.Go.GoView" /> or to any other <c>Control</c>s.
	/// </para>
	/// <para>
	/// All documents and all of their objects must be serializable.
	/// Please read more about this topic in the User Guide.
	/// To help debug serialization issues, call the <see cref="M:Northwoods.Go.GoDocument.TestSerialization" /> method.
	/// </para>
	/// <para>
	/// <c>GoDocument</c> implements <see cref="T:Northwoods.Go.IGoCollection" />, so you can call methods such as
	/// <see cref="M:Northwoods.Go.GoDocument.Add(Northwoods.Go.GoObject)" />, <see cref="M:Northwoods.Go.GoDocument.Remove(Northwoods.Go.GoObject)" />, <see cref="M:Northwoods.Go.GoDocument.Contains(Northwoods.Go.GoObject)" />, <see cref="M:Northwoods.Go.GoDocument.Clear" />, and
	/// <see cref="M:Northwoods.Go.GoDocument.CopyArray" />, use properties such as <see cref="P:Northwoods.Go.GoDocument.Count" />, and enumerate both
	/// forwards and <see cref="P:Northwoods.Go.GoDocument.Backwards" />.
	/// </para>
	/// <para>
	/// A <c>GoDocument</c> organizes all of the <see cref="T:Northwoods.Go.GoObject" />s that it contains into
	/// <see cref="T:Northwoods.Go.GoLayer" />s.  The layers are held in the <see cref="P:Northwoods.Go.GoDocument.Layers" /> collection.
	/// Initially a document will only have one layer, the <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" />.
	/// If you want to keep nodes in one layer and links in a different layer, it is conventional
	/// to put the nodes in the <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" /> and the link in the <see cref="P:Northwoods.Go.GoDocument.LinksLayer" />.
	/// You will need to create a new layer and assign it to that property.
	/// If you want have all links appear behind all nodes, you can do:
	/// </para>
	/// <code lang="CS"> 
	/// doc.LinksLayer = doc.Layers.CreateNewLayerBefore(doc.Layers.Default)
	/// </code>
	/// <para>
	/// Since the layers are painted in the order in which they appear in the <see cref="P:Northwoods.Go.GoDocument.Layers" />
	/// collection, the above code will ensure that all links are painted before any nodes or
	/// other objects that are in the <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" />.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoObject.Bounds" /> of each object are in document coordinates.
	/// This means that the coordinates are single precision floating point values;
	/// the position is a <c>PointF</c>, the size is a <c>SizeF</c>, and the bounds is a
	/// <c>RectangleF</c>.  Each <see cref="T:Northwoods.Go.GoView" /> can look at some or all of the 
	/// <see cref="T:Northwoods.Go.GoObject" />s in a rectangular area determined by the view.  The size
	/// of the view, the view's <see cref="P:Northwoods.Go.GoView.DocPosition" />, and the view's
	/// <see cref="P:Northwoods.Go.GoView.DocScale" /> all together form the view's <see cref="P:Northwoods.Go.GoView.DocExtent" />.
	/// If a <see cref="T:Northwoods.Go.GoObject" />'s <see cref="P:Northwoods.Go.GoObject.Bounds" /> is outside of a view's
	/// <see cref="P:Northwoods.Go.GoView.DocExtent" />, it won't be seen in that view.
	/// </para>
	/// <para>
	/// Independent of any view, a <c>GoDocument</c> has a <see cref="P:Northwoods.Go.GoDocument.Size" /> and a
	/// <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> position.  Together they form the document's <see cref="P:Northwoods.Go.GoDocument.Bounds" />.
	/// Normally the document's bounds is large enough to include the bounds of all of its
	/// objects.  As objects are added or moved or resized, <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />
	/// extends the <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> position and the <see cref="P:Northwoods.Go.GoDocument.Size" /> if needed.
	/// (However, if <see cref="P:Northwoods.Go.GoDocument.FixedSize" /> is true, <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />
	/// does nothing.)  The document's bounds affect whether or not a particular view's
	/// scroll bars are needed and how far a user might be able to scroll.  But note that
	/// each view has additional properties for governing scrolling extents.
	/// </para>
	/// <para>
	/// The document's bounds do not automatically shrink as objects are removed or are
	/// moved away from the edges of the bounds.  To iterate over all of the objects and
	/// compute the actual area that they currently occupy, call <see cref="M:Northwoods.Go.GoDocument.ComputeBounds" />.
	/// You can also call the static method <see cref="M:Northwoods.Go.GoDocument.ComputeBounds(Northwoods.Go.IGoCollection,Northwoods.Go.GoView)" />
	/// to determine the extent of any collection of objects.  That method takes an optional
	/// <see cref="T:Northwoods.Go.GoView" /> argument to allow consideration of how much area each object
	/// actually takes when painted or printed in that view, since for each object the
	/// result of <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" /> may be larger than then the
	/// object's <see cref="P:Northwoods.Go.GoObject.Bounds" />.
	/// </para>
	/// <para>
	/// There are a number of different possibilities for finding an object in a document:
	/// by location, by name, or by identifier.
	/// </para>
	/// <para>
	/// To find the front-most object at a given point,
	/// call the <see cref="M:Northwoods.Go.GoDocument.PickObject(System.Drawing.PointF,System.Boolean)" /> method.  If you might want to find all of the
	/// objects at a particular point, including those hidden behind/underneath, call the
	/// <see cref="M:Northwoods.Go.GoDocument.PickObjects(System.Drawing.PointF,System.Boolean,Northwoods.Go.IGoCollection,System.Int32)" /> method.  If you want to find all of the objects that are
	/// either completely or partially surrounded by a rectangular area,
	/// call <see cref="M:Northwoods.Go.GoDocument.PickObjectsInRectangle(System.Drawing.RectangleF,Northwoods.Go.GoPickInRectangleStyle,Northwoods.Go.IGoCollection,System.Int32)" />.
	/// </para>
	/// <para>
	/// To find a node by name, call one of the <c>FindNode</c> methods.
	/// <c>FindNode</c> actually will find any <see cref="T:Northwoods.Go.IGoLabeledPart" />, not just nodes.
	/// <see cref="P:Northwoods.Go.IGoLabeledPart.Text" /> is the property that provides the text string.
	/// </para>
	/// <para>
	/// To find a node or port or link by integer identifier, you'll first need to set
	/// <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> to true.  Then you'll be able to call
	/// <see cref="M:Northwoods.Go.GoDocument.FindPart(System.Int32)" /> to find any <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> within the
	/// document, at any level of nesting.
	/// <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> is the property that provides the integer
	/// identifier.
	/// </para>
	/// <para>
	/// There are many properties that control whether a user can perform certain operations.
	/// These properties (and corresponding predicates) are defined by the interface
	/// <see cref="T:Northwoods.Go.IGoLayerAbilities" />.  Those include: select, move, copy, resize,
	/// reshape, delete, insert, link, and edit.  Setting one of these <c>Allow...</c>
	/// properties will affect all <see cref="T:Northwoods.Go.GoView" />s displaying this document.
	/// (Setting the same properties on a view will only affect that view;
	/// setting the same properties on a layer will only affect all objects in that layer.)
	/// Call the <see cref="M:Northwoods.Go.GoDocument.SetModifiable(System.Boolean)" /> method for a convenient way to set all of
	/// the properties that allow modification of the document.
	/// </para>
	/// <para>
	/// The <see cref="P:Northwoods.Go.GoDocument.IsModified" /> property indicates whether the document has been
	/// modified.  Read its documentation to learn more about how the <see cref="T:Northwoods.Go.GoUndoManager" />
	/// affects its state, for both getting and setting.
	/// </para>
	/// <para>
	/// Whenever a change to the document occurs, the <see cref="E:Northwoods.Go.GoDocument.Changed" /> event is raised.
	/// Each time one sets a property or adds or removes an object, <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
	/// is called, which in turn calls <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" /> to call all registered
	/// event handlers.  <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" /> is also responsible for maintaining the
	/// <see cref="P:Northwoods.Go.GoDocument.IsModified" /> state, for calling <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />,
	/// and for maintaining <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />s.
	/// </para>
	/// <para>
	/// If you add a property in a class inheriting from <c>GoDocument</c>,
	/// be sure to call <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> if and only if the value has changed.
	/// You will also need to override <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> to perform the recorded
	/// change for that particular property.
	/// Similarly, if you add a property to a class inheriting from <see cref="T:Northwoods.Go.GoObject" />,
	/// be sure to call <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> if and only if the value has changed.
	/// You will also need to override <see cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> in that class
	/// to perform the recorded change for that particular property.
	/// </para>
	/// <para>
	/// To record undo/redo information, assign the <see cref="P:Northwoods.Go.GoDocument.UndoManager" /> property.
	/// This will automatically enable undo/redo functionality in all views displaying
	/// this document.  If you want to make some transient changes to objects and do not
	/// want those changes to be recorded in the <see cref="T:Northwoods.Go.GoUndoManager" />, temporarily
	/// set <see cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" /> to true.
	/// </para>
	/// <para>
	/// Any code that makes changes to a document or to a document object should be
	/// preceded with a call to <see cref="M:Northwoods.Go.GoDocument.StartTransaction" /> and followed by a call to
	/// <see cref="M:Northwoods.Go.GoDocument.FinishTransaction(System.String)" /> or <see cref="M:Northwoods.Go.GoDocument.AbortTransaction" />.  This groups
	/// together all of the changes so that an undo or a redo will perform them all at once.
	/// Transactions can be nested.
	/// </para>
	/// <para>
	/// When you want to (programmatically) make copies of objects and add them to a document,
	/// call <see cref="M:Northwoods.Go.GoDocument.AddCopy(Northwoods.Go.GoObject,System.Drawing.PointF)" /> for a single object or call
	/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" /> for multiple objects.  Calling the latter
	/// method (or its overloaded variant, for more options) is needed if you want to maintain
	/// relationships between the objects being copied, for example, the links between nodes.
	/// </para>
	/// <para>
	/// <c>GoDocument</c> also provides some options for controlling the kinds of links that
	/// users may draw: the <see cref="P:Northwoods.Go.GoDocument.ValidCycle" /> property.
	/// </para>
	/// <para>
	/// For document persistence, you will need to implement code to read/write to your
	/// desired medium.  If you are reading/writing XML, you may find GoXML useful.
	/// In particular, <c>GoXmlBindingTransformer</c> may be very convenient to use.
	/// We also offer a sample, DataSetDemo, demonstrating persistence to and from a DataSet.
	/// </para>
	/// <para>
	/// Read the User Guide and FAQ for more details.
	/// </para>
	/// </remarks>
	/// <example>
	///  Typical programmatic usage might be something like:
	/// <code lang="CS"> 
	/// GoBasicNode node1 = new GoBasicNode();
	/// node1.LabelSpot = GoObject.Middle;
	/// node1.Text = "basic node 1";
	/// node1.Shape.BrushColor = Color.LightGreen;
	/// node1.Location = new PointF(75, 50);
	/// goView1.Document.Add(node1);
	///
	/// GoBasicNode node2 = new GoBasicNode();
	/// node2.LabelSpot = GoObject.Middle;
	/// node2.Text = "basic node 2";
	/// node2.Shape.BrushColor = Color.LightYellow;
	/// node2.Location = new PointF(200, 50);
	/// goView1.Document.Add(node2);
	///
	/// GoLink link = new GoLink();
	/// link.ToArrow = true;
	/// link.FromPort = node1.Port;
	/// link.ToPort = node2.Port;
	/// goView1.Document.Add(link);
	/// </code>
	/// </example>
	[Serializable]
	public class GoDocument : IGoCollection, ICollection<GoObject>, IEnumerable<GoObject>, IEnumerable, IGoLayerCollectionContainer, IGoLayerAbilities
	{
		internal class SegInfo
		{
			public float Layer;

			public float First;

			public float Last;

			public float ColumnMin;

			public float ColumnMax;

			public int Index;

			public IGoLink Link;

			public int Turns;
		}

		internal sealed class SegInfoComparer : IComparer<SegInfo>
		{
			private static SegInfoComparer myDefaultComparer = new SegInfoComparer();

			public static SegInfoComparer Default => myDefaultComparer;

			private SegInfoComparer()
			{
			}

			public int Compare(SegInfo a, SegInfo b)
			{
				if (a == null || b == null || a == b)
				{
					return 0;
				}
				if (a.Layer < b.Layer)
				{
					return -1;
				}
				if (a.Layer > b.Layer)
				{
					return 1;
				}
				if (a.ColumnMin < b.ColumnMin)
				{
					return -1;
				}
				if (a.ColumnMin > b.ColumnMin)
				{
					return 1;
				}
				if (a.ColumnMax < b.ColumnMax)
				{
					return -1;
				}
				if (a.ColumnMax > b.ColumnMax)
				{
					return 1;
				}
				return 0;
			}
		}

		internal sealed class SegInfoComparer2 : IComparer<SegInfo>
		{
			private bool myFirst;

			private static SegInfoComparer2 myDefaultFirstComparer = new SegInfoComparer2(f: true);

			private static SegInfoComparer2 myDefaultLastComparer = new SegInfoComparer2(f: false);

			public static SegInfoComparer2 DefaultFirst => myDefaultFirstComparer;

			public static SegInfoComparer2 DefaultLast => myDefaultLastComparer;

			private SegInfoComparer2(bool f)
			{
				myFirst = f;
			}

			public int Compare(SegInfo a, SegInfo b)
			{
				if (a == null || b == null || a == b)
				{
					return 0;
				}
				Trace.Assert(a.Layer == b.Layer);
				if (myFirst)
				{
					if (a.First < b.First)
					{
						return -1;
					}
					if (a.First > b.First)
					{
						return 1;
					}
				}
				else
				{
					if (a.Last < b.Last)
					{
						return -1;
					}
					if (a.Last > b.Last)
					{
						return 1;
					}
				}
				if (a.Turns < b.Turns)
				{
					return 1;
				}
				if (a.Turns > b.Turns)
				{
					return -1;
				}
				if (a.ColumnMin < b.ColumnMin)
				{
					return -1;
				}
				if (a.ColumnMin > b.ColumnMin)
				{
					return 1;
				}
				if (a.ColumnMax < b.ColumnMax)
				{
					return -1;
				}
				if (a.ColumnMax > b.ColumnMax)
				{
					return 1;
				}
				return 0;
			}
		}

		/// <summary>
		/// This is an empty <c>RectangleF</c>, which is convenient when calling <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		protected static readonly RectangleF NullRect = default(RectangleF);

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint to tell all of the <see cref="T:Northwoods.Go.GoView" />s
		/// on this document to call <see cref="M:Northwoods.Go.GoView.UpdateView" />.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> are not meaningful.
		/// </remarks>
		public const int RepaintAll = 100;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint to tell all of the <see cref="T:Northwoods.Go.GoView" />s
		/// on this document to call <see cref="M:Northwoods.Go.GoView.BeginUpdate" />.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> are not meaningful.
		/// </remarks>
		public const int BeginUpdateAllViews = 101;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint to tell all of the <see cref="T:Northwoods.Go.GoView" />s
		/// on this document to call <see cref="M:Northwoods.Go.GoView.EndUpdate" />.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> are not meaningful.
		/// </remarks>
		public const int EndUpdateAllViews = 102;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint to tell all of the <see cref="T:Northwoods.Go.GoView" />s
		/// on this document to call <c>Control.Update</c>.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> are not meaningful.
		/// </remarks>
		public const int UpdateAllViews = 103;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.StartTransaction" /> just after a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.StartTransaction" />.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> are not meaningful.
		/// </remarks>
		public const int StartedTransaction = 104;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint
		/// called by <see cref="M:Northwoods.Go.GoDocument.FinishTransaction(System.String)" /> just after a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.FinishTransaction(System.String)" />.
		/// </summary>
		/// <remarks>
		/// Because there is no real document state change associated with this event case,
		/// the Object and the old and new values (integer, Object, and RectangleF) passed to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// and stored in a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> have a different meaning.
		/// The <see cref="P:Northwoods.Go.GoChangedEventArgs.Object" /> is the <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> after having been committed,
		/// the <see cref="P:Northwoods.Go.GoChangedEventArgs.OldValue" /> is the transaction name,
		/// and the <see cref="P:Northwoods.Go.GoChangedEventArgs.NewValue" /> is the presentation name.
		/// </remarks>
		public const int FinishedTransaction = 105;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.AbortTransaction" /> just after a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.AbortTransaction" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoChangedEventArgs.Object" /> is the <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" />,
		/// the <see cref="P:Northwoods.Go.GoChangedEventArgs.OldValue" /> is the transaction name,
		/// and the <see cref="P:Northwoods.Go.GoChangedEventArgs.NewValue" /> is the presentation name.
		/// </remarks>
		public const int AbortedTransaction = 106;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.Undo" /> just before a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Undo" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" /> before calling Undo.
		/// </remarks>
		public const int StartingUndo = 107;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.Undo" /> just after a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Undo" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" /> before calling Undo.
		/// </remarks>
		public const int FinishedUndo = 108;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.Redo" /> just before a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" /> before calling Redo.
		/// </remarks>
		public const int StartingRedo = 109;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint,
		/// called by <see cref="M:Northwoods.Go.GoDocument.Redo" /> just after a call to <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </summary>
		/// <remarks>
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" /> before calling Redo.
		/// </remarks>
		public const int FinishedRedo = 110;

		internal const int FirstStateChangedHint = 200;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.Name" /> property.
		/// </summary>
		public const int ChangedName = 201;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.Size" /> property.
		/// </summary>
		public const int ChangedSize = 202;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> property.
		/// </summary>
		public const int ChangedTopLeft = 203;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.FixedSize" /> property.
		/// </summary>
		public const int ChangedFixedSize = 204;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.PaperColor" /> property.
		/// </summary>
		public const int ChangedPaperColor = 205;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.DataFormat" /> property.
		/// </summary>
		public const int ChangedDataFormat = 206;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowSelect" /> property.
		/// </summary>
		public const int ChangedAllowSelect = 207;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowMove" /> property.
		/// </summary>
		public const int ChangedAllowMove = 208;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowCopy" /> property.
		/// </summary>
		public const int ChangedAllowCopy = 209;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowResize" /> property.
		/// </summary>
		public const int ChangedAllowResize = 210;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowReshape" /> property.
		/// </summary>
		public const int ChangedAllowReshape = 211;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowDelete" /> property.
		/// </summary>
		public const int ChangedAllowDelete = 212;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowInsert" /> property.
		/// </summary>
		public const int ChangedAllowInsert = 213;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowLink" /> property.
		/// </summary>
		public const int ChangedAllowLink = 214;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.AllowEdit" /> property.
		/// </summary>
		public const int ChangedAllowEdit = 215;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		public const int AllArranged = 220;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.UserFlags" /> property.
		/// </summary>
		public const int ChangedUserFlags = 221;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.UserObject" /> property.
		/// </summary>
		public const int ChangedUserObject = 222;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.LinksLayer" /> property.
		/// </summary>
		public const int ChangedLinksLayer = 223;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> property.
		/// </summary>
		public const int ChangedMaintainsPartID = 224;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.ValidCycle" /> property.
		/// </summary>
		public const int ChangedValidCycle = 225;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.LastPartID" /> property.
		/// </summary>
		public const int ChangedLastPartID = 226;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.WorldScale" /> property.
		/// </summary>
		internal const int ChangedWorldScale = 227;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.RoutingTime" /> property.
		/// </summary>
		public const int ChangedRoutingTime = 228;

		/// <summary>
		/// This is a <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint identifying changes to the value of the <see cref="P:Northwoods.Go.GoDocument.Initializing" /> property.
		/// </summary>
		public const int ChangedInitializing = 241;

		/// <summary>
		/// This <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint is a synonym for <see cref="T:Northwoods.Go.GoLayer" />.<see cref="F:Northwoods.Go.GoLayer.ChangedObject" />.
		/// </summary>
		public const int ChangedObject = 901;

		/// <summary>
		/// This is the last system-defined <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> hint.
		/// </summary>
		/// <remarks>
		/// Subclasses of GoDocument should use values larger than this one
		/// to identify document, layer collection, and layer change events.
		/// GoLayerCollection hints are 800-899.
		/// GoLayer hints are 900-999.
		/// GoObject hints are 1000-9999, even though they are stored as subhints and thus cannot conflict.
		/// </remarks>
		public const int LastHint = 10000;

		private const float RIGHT = 0f;

		private const float DOWN = 90f;

		private const float LEFT = 180f;

		private const float UP = 270f;

		private static Dictionary<IGoNode, bool> myCycleMap = new Dictionary<IGoNode, bool>();

		internal static bool myCaching = true;

		private int myUserFlags;

		private object myUserObject;

		private GoLayerCollection myLayers = new GoLayerCollection();

		private GoLayer myLinksLayer;

		private string myName = "";

		private SizeF myDocumentSize;

		private PointF myDocumentTopLeft;

		private SizeF myWorldScale = new SizeF(1f, 1f);

		private float myWorldEpsilon = 0.5f;

		private bool myFixedSize;

		private Color myPaperColor = Color.Empty;

		private string myDataFormat;

		private bool myAllowSelect = true;

		private bool myAllowMove = true;

		private bool myAllowCopy = true;

		private bool myAllowResize = true;

		private bool myAllowReshape = true;

		private bool myAllowDelete = true;

		private bool myAllowInsert = true;

		private bool myAllowLink = true;

		private bool myAllowEdit = true;

		private bool mySuspendsUpdates;

		private bool mySkipsUndoManager;

		private bool mySerializesUndoManager;

		private bool myInitializing;

		[NonSerialized]
		private GoChangedEventHandler myChangedEvent;

		[NonSerialized]
		private GoChangedEventArgs myChangedEventArgs;

		[NonSerialized]
		private bool myIsModified;

		[NonSerialized]
		private GoUndoManager myUndoManager;

		private GoUndoManager mySerializedUndoManager;

		private int myUndoEditIndex = -2;

		private GoDocumentValidCycle myValidCycle;

		private bool mySuspendsRouting;

		private GoCopyDelayedsCollection myDelayedRoutings = new GoCopyDelayedsCollection();

		private GoRoutingTime myRoutingTime = GoRoutingTime.Delayed;

		[NonSerialized]
		private GoPositionArray myPositions;

		[NonSerialized]
		private GoObject mySkippedAvoidable;

		private float myLinkSpacing = 4f;

		private bool myAvoidsOrthogonalLinks;

		private bool myMaintainsPartID;

		private int myLastPartID = -1;

		[NonSerialized]
		private Dictionary<int, IGoIdentifiablePart> myParts;

		/// <summary>
		/// This predicate is true when there are no objects in this collection.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.IsEmpty" />
		[Browsable(false)]
		public bool IsEmpty => Count == 0;

		/// <summary>
		/// Gets the total number of objects in all document layers.
		/// </summary>
		[Description("The total number of objects in all document layers.")]
		public virtual int Count
		{
			get
			{
				int num = 0;
				foreach (GoLayer layer in Layers)
				{
					num = checked(num + layer.Count);
				}
				return num;
			}
		}

		/// <summary>
		/// This collection is never read-only programmatically,
		/// but might not be modifiable by the user: <see cref="M:Northwoods.Go.GoDocument.SetModifiable(System.Boolean)" />.
		/// </summary>
		public virtual bool IsReadOnly => false;

		IEnumerable<GoObject> IGoCollection.Backwards => Layers.GetObjectEnumerator(forward: false);

		/// <summary>
		/// Gets an enumerable whose enumerator will iterate over the GoObjects in reverse order.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoCollection.Backwards" />
		[Browsable(false)]
		public virtual GoLayerCollectionObjectEnumerator Backwards => Layers.GetObjectEnumerator(forward: false);

		/// <summary>
		/// Gets the collection of layers belonging to this document.
		/// </summary>
		/// <remarks>
		/// This value is the list of this document's layers.
		/// Use <see cref="T:Northwoods.Go.GoLayerCollection" /> methods for creating new
		/// document layers, removing them, or operating on particular layers,
		/// such as the <see cref="P:Northwoods.Go.GoLayerCollection.Default" /> one.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.DefaultLayer" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.LinksLayer" />
		/// <seealso cref="T:Northwoods.Go.GoLayer" />
		/// <seealso cref="T:Northwoods.Go.GoView" />
		[Browsable(false)]
		public virtual GoLayerCollection Layers => myLayers;

		/// <summary>
		/// Gets or sets the layer that is considered the default layer for document
		/// operations that do not specify a layer.
		/// </summary>
		/// <value>
		/// The <see cref="T:Northwoods.Go.GoLayer" /> value must not be null and must already
		/// belong to this document.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoDocument.LinksLayer" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.Layers" />
		[Description("The default layer used when adding objects to the document.")]
		public virtual GoLayer DefaultLayer
		{
			get
			{
				return Layers.Default;
			}
			set
			{
				Layers.Default = value;
			}
		}

		/// <summary>
		/// Gets or sets the layer that is normally used for holding links.
		/// </summary>
		/// <value>
		/// The <see cref="T:Northwoods.Go.GoLayer" /> value must not be null and must already
		/// belong to this document.  By default the links layer is the same as
		/// the default layer, because the document only has a single layer.
		/// </value>
		/// <remarks>
		/// It is common to want to display all links behind all nodes, or all
		/// links in front of all nodes.  Either policy can be implemented easily
		/// by creating a new document layer and assigning it to this property.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.DefaultLayer" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.Layers" />
		[Description("The default layer used when adding links to the document.")]
		public virtual GoLayer LinksLayer
		{
			get
			{
				return myLinksLayer;
			}
			set
			{
				GoLayer goLayer = myLinksLayer;
				if (goLayer != value)
				{
					if (value == null || value.Document != this)
					{
						throw new ArgumentException("The new value for GoDocument.LinksLayer must belong to this document.");
					}
					myLinksLayer = value;
					RaiseChanged(223, 0, null, 0, goLayer, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets a name for this document.
		/// </summary>
		/// <value>
		/// The name may be any string.  The default value is an empty string.
		/// A new value must not be null.
		/// </value>
		/// <remarks>
		/// Normally this property is used to hold a user-visible name for a document.
		/// Additional properties are then used to hold information about how to
		/// load and store the information in the document.
		/// </remarks>
		[DefaultValue("")]
		[Description("The user-visible name for this document.")]
		public virtual string Name
		{
			get
			{
				return myName;
			}
			set
			{
				string text = myName;
				if (value != null && text != value)
				{
					myName = value;
					RaiseChanged(201, 0, null, 0, text, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of this document.
		/// </summary>
		/// <value>
		/// The <c>SizeF</c> value is in document coordinates and should have non-negative
		/// width and height.
		/// </value>
		/// <remarks>
		/// The default behavior is that this property automatically expands to include all
		/// of the objects in the document.  This policy is implemented in
		/// <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />.  Set <see cref="P:Northwoods.Go.GoDocument.FixedSize" /> to avoid this
		/// default policy, or override <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" /> to implement your
		/// own policy.
		/// This property automatically affects what a view can show and where the user can
		/// scroll to.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.Bounds" />
		[Description("The size of this document.")]
		[TypeConverter(typeof(GoSizeFConverter))]
		public virtual SizeF Size
		{
			get
			{
				return myDocumentSize;
			}
			set
			{
				if (value.Width < 0f)
				{
					if (value.Width == -23f)
					{
						if (value.Height == -23f)
						{
							myCaching = true;
						}
						else if (value.Height == -24f)
						{
							myCaching = false;
							foreach (GoLayer layer in Layers)
							{
								layer.ResetCaches();
							}
						}
					}
					else if (value.Width == -24f)
					{
						WorldScale = new SizeF(value.Height, value.Height);
					}
					else if (value.Width == -26f)
					{
						WorldScale = new SizeF(value.Height, WorldScale.Height);
					}
					else if (value.Width == -27f)
					{
						WorldScale = new SizeF(WorldScale.Width, value.Height);
					}
					else if (value.Width == -25f)
					{
						if (myPositions != null)
						{
							myPositions.CellSize = new SizeF(value.Height / WorldScale.Width, value.Height / WorldScale.Height);
						}
					}
					else if (value.Width == -28f)
					{
						if (myPositions != null)
						{
							myPositions.WholeDocument = (value.Height > 0f);
						}
					}
					else if (value.Width == -29f)
					{
						if (myPositions != null)
						{
							myPositions.SmallMargin = value.Height;
						}
					}
					else if (value.Width == -30f)
					{
						if (myPositions != null)
						{
							myPositions.LargeMargin = value.Height;
						}
					}
					else if (value.Width == -31f)
					{
						myLinkSpacing = value.Height;
					}
					else if (value.Width == -32f)
					{
						myAvoidsOrthogonalLinks = true;
					}
					else if (value.Width == -33f)
					{
						myAvoidsOrthogonalLinks = false;
					}
				}
				else
				{
					SizeF sizeF = myDocumentSize;
					if (value.Width >= 0f && value.Height >= 0f && sizeF != value)
					{
						myDocumentSize = value;
						RaiseChanged(202, 0, null, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the top-left corner position of this document.
		/// </summary>
		/// <value>
		/// The <c>PointF</c> value is in document coordinates.
		/// Initially this value is (0, 0).
		/// </value>
		/// <remarks>
		/// The default behavior is that this property automatically moves toward
		/// negative coordinates to include all of the objects in the document.
		/// This policy is implemented in <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />.
		/// Set <see cref="P:Northwoods.Go.GoDocument.FixedSize" /> to avoid this default policy,
		/// or override <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" /> to implement your own policy.
		/// This property automatically affects what a view can show and where the user can
		/// scroll to.
		/// Note that the <see cref="P:Northwoods.Go.GoView.ShowsNegativeCoordinates" /> property has
		/// no effect on any document.  That property constrains what the user can see,
		/// even if the document includes objects at negative positions.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.Bounds" />
		[Description("The top-left corner position of this document.")]
		[TypeConverter(typeof(GoPointFConverter))]
		public virtual PointF TopLeft
		{
			get
			{
				return myDocumentTopLeft;
			}
			set
			{
				PointF pointF = myDocumentTopLeft;
				if (pointF != value)
				{
					myDocumentTopLeft = value;
					RaiseChanged(203, 0, null, 0, null, GoObject.MakeRect(pointF), 0, null, GoObject.MakeRect(value));
				}
			}
		}

		/// <summary>
		/// This property is just a combination of <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> and <see cref="P:Northwoods.Go.GoDocument.Size" />
		/// as a <c>RectangleF</c>.
		/// </summary>
		/// <remarks>
		/// To limit the scrolling extent of views where objects in the document have been
		/// deleted or gathered together, you may want to do:
		/// <code>doc.Bounds = doc.ComputeBounds()</code>
		/// </remarks>
		[Browsable(false)]
		public RectangleF Bounds
		{
			get
			{
				PointF topLeft = TopLeft;
				SizeF size = Size;
				return new RectangleF(topLeft.X, topLeft.Y, size.Width, size.Height);
			}
			set
			{
				TopLeft = new PointF(value.X, value.Y);
				Size = new SizeF(value.Width, value.Height);
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" /> should update
		/// the <see cref="P:Northwoods.Go.GoDocument.Size" /> and <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> properties as objects
		/// are added or moved.
		/// </summary>
		/// <value>
		/// The default value is false.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoDocument.Size" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.TopLeft" />
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether adding or moving objects in the document leaves the document size and top-left unchanged.")]
		public virtual bool FixedSize
		{
			get
			{
				return myFixedSize;
			}
			set
			{
				bool flag = myFixedSize;
				if (flag != value)
				{
					myFixedSize = value;
					RaiseChanged(204, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets a horizontal and vertical scaling factor for converting document
		/// coordinates in other units to and from pixel units.
		/// </summary>
		/// <value>
		/// The default value is 1.0f--document units have the same size as pixels
		/// (when the view's <see cref="P:Northwoods.Go.GoView.DocScale" /> is 1.0f).
		/// </value>
		/// <example>
		/// For example, for an object with a <see cref="P:Northwoods.Go.GoObject.Width" /> equal to 1
		/// to be (nominally) one centimeter wide, set the world scale as follows:
		/// <code>
		/// Bitmap bm = new Bitmap(10, 10);
		/// Graphics g = Graphics.FromImage(bm);
		/// g.PageUnit = GraphicsUnit.Pixel;
		/// myDoc.WorldScale = new SizeF(g.DpiX / 2.54f, g.DpiY / 2.54f);
		/// g.Dispose();
		/// bm.Dispose();
		/// </code>
		/// </example>
		internal SizeF WorldScale
		{
			get
			{
				return myWorldScale;
			}
			set
			{
				SizeF sizeF = myWorldScale;
				if (sizeF != value && value.Width > 0f && value.Height > 0f)
				{
					myWorldScale = value;
					myWorldEpsilon = 0.5f / value.Width;
					if (myPositions != null)
					{
						SizeF cellSize = myPositions.CellSize;
						myPositions.CellSize = new SizeF(cellSize.Width * sizeF.Width / value.Width, cellSize.Height * sizeF.Height / value.Height);
					}
					InvalidatePositionArray(null);
					RaiseChanged(227, 0, null, 0, null, GoObject.MakeRect(sizeF), 0, null, GoObject.MakeRect(value));
					if (!Initializing)
					{
						Bounds = ComputeBounds();
					}
				}
			}
		}

		internal float WorldEpsilon => myWorldEpsilon;

		/// <summary>
		/// Gets or sets the color for this document's background.
		/// </summary>
		/// <value>
		/// The default value is <c>Color.Empty</c>.
		/// </value>
		/// <remarks>
		/// Documents can have their own background, independent of any background
		/// color provided by a view.  The normal behavior is that a view will
		/// use the document's <c>PaperColor</c> property when that color is
		/// not <c>Color.Empty</c>, but will otherwise use the view's <c>BackColor</c>
		/// property.  However, there may be times when both or neither color is
		/// used in a rendering of the document.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.PaintPaperColor(System.Drawing.Graphics,System.Drawing.RectangleF)" />
		[Category("Appearance")]
		[Description("The color of the document's background.")]
		public virtual Color PaperColor
		{
			get
			{
				return myPaperColor;
			}
			set
			{
				Color color = myPaperColor;
				if (color != value)
				{
					myPaperColor = value;
					RaiseChanged(205, 0, null, 0, color, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can select objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from selecting objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// selectable by the user because the object or its layer disallows it,
		/// or because the view disallows it, or because the object is not visible.
		/// Your code can always select objects programmatically by calling
		/// <c>aView.Selection.Select(obj)</c> or <c>aView.Selection.Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanSelectObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowSelect" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Selectable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can select objects in this document.")]
		public virtual bool AllowSelect
		{
			get
			{
				return myAllowSelect;
			}
			set
			{
				bool flag = myAllowSelect;
				if (flag != value)
				{
					myAllowSelect = value;
					RaiseChanged(207, 0, this, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can move selected objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from moving objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// movable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always move objects programmatically by calling
		/// <c>obj.Position = newPos</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanMoveObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowMove" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Movable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can move selected objects in this document.")]
		public virtual bool AllowMove
		{
			get
			{
				return myAllowMove;
			}
			set
			{
				bool flag = myAllowMove;
				if (flag != value)
				{
					myAllowMove = value;
					RaiseChanged(208, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can copy selected objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from copying objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// copyable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always copy objects programmatically by calling
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanCopyObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowCopy" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Copyable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can copy selected objects in this document.")]
		public virtual bool AllowCopy
		{
			get
			{
				return myAllowCopy;
			}
			set
			{
				bool flag = myAllowCopy;
				if (flag != value)
				{
					myAllowCopy = value;
					RaiseChanged(209, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can resize selected objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from resizing objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// resizable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always resize objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanResizeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowResize" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Resizable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can resize selected objects in this document.")]
		public virtual bool AllowResize
		{
			get
			{
				return myAllowResize;
			}
			set
			{
				bool flag = myAllowResize;
				if (flag != value)
				{
					myAllowResize = value;
					RaiseChanged(210, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can reshape resizable objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from reshaping objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// reshapable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always reshape objects programmatically by calling
		/// <c>obj.Size = newSize</c> or <c>obj.Bounds = newRect</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanReshapeObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowReshape" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Reshapable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can reshape resizable objects in this document.")]
		public virtual bool AllowReshape
		{
			get
			{
				return myAllowReshape;
			}
			set
			{
				bool flag = myAllowReshape;
				if (flag != value)
				{
					myAllowReshape = value;
					RaiseChanged(211, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can delete selected objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from deleting objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// deletable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always delete objects programmatically by calling
		/// <c>obj.Remove()</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanDeleteObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowDelete" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Deletable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can delete selected objects in this document.")]
		public virtual bool AllowDelete
		{
			get
			{
				return myAllowDelete;
			}
			set
			{
				bool flag = myAllowDelete;
				if (flag != value)
				{
					myAllowDelete = value;
					RaiseChanged(212, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can insert objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from inserting objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// insertable by the user because the layer or view disallows it.
		/// Your code can always insert objects programmatically by calling
		/// <c>Add(obj)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanInsertObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowInsert" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can insert objects into this document.")]
		public virtual bool AllowInsert
		{
			get
			{
				return myAllowInsert;
			}
			set
			{
				bool flag = myAllowInsert;
				if (flag != value)
				{
					myAllowInsert = value;
					RaiseChanged(213, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can link objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from linking objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// linkable by the user because the ports disallow it,
		/// or because the view disallows it.
		/// Your code can always link objects programmatically by calling
		/// <c>LinksLayers.Add(newLink)</c>, where <c>newLink</c> is
		/// a newly created instance of a class like <see cref="T:Northwoods.Go.GoLink" /> or
		/// <see cref="T:Northwoods.Go.GoLabeledLink" /> whose <see cref="P:Northwoods.Go.IGoLink.FromPort" /> and
		/// <see cref="P:Northwoods.Go.IGoLink.ToPort" /> properties have been set to ports in
		/// the same document.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanLinkObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowLink" />
		/// <seealso cref="P:Northwoods.Go.GoView.AllowLink" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can link ports in this document.")]
		public virtual bool AllowLink
		{
			get
			{
				return myAllowLink;
			}
			set
			{
				bool flag = myAllowLink;
				if (flag != value)
				{
					myAllowLink = value;
					RaiseChanged(214, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user can edit objects in this document.
		/// </summary>
		/// <remarks>
		/// A false value prevents the user from editing objects in this document
		/// by the normal mechanisms.
		/// Even when this property value is true, some objects might not be
		/// editable by the user because the object or its layer disallows it,
		/// or because the view disallows it.
		/// Your code can always edit objects programmatically by calling
		/// <c>obj.DoBeginEdit(aView)</c>.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.CanEditObjects" />
		/// <seealso cref="P:Northwoods.Go.GoLayer.AllowEdit" />
		/// <seealso cref="P:Northwoods.Go.GoObject.Editable" />
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Whether the user can edit objects in this document.")]
		public virtual bool AllowEdit
		{
			get
			{
				return myAllowEdit;
			}
			set
			{
				bool flag = myAllowEdit;
				if (flag != value)
				{
					myAllowEdit = value;
					RaiseChanged(215, 0, null, 0, flag, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether this document is considered changed from an earlier state.
		/// </summary>
		/// <value>
		/// true if this document has been marked as having been modified,
		/// if the <see cref="P:Northwoods.Go.GoDocument.UndoManager" /> has recorded any changes, or
		/// if an undo has been performed without a corresponding redo.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property is set to true in <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" />.
		/// Setting this property to a new value calls <see cref="M:Northwoods.Go.GoDocument.InvalidateViews" />,
		/// but does not raise a Changed event.
		/// </para>
		/// <para>
		/// Although you can set this property at any time, the value of
		/// <c>GoDocument.IsModified</c> will continue to be true as long as
		/// there have been changes made to the document and you are using an
		/// <see cref="P:Northwoods.Go.GoDocument.UndoManager" />.
		/// Any modifications to a GoDocument or one of its parts will result
		/// in setting <c>GoDocument.IsModified</c> to true and in adding a
		/// <see cref="T:Northwoods.Go.GoChangedEventArgs" /> to the
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> list,
		/// which holds all of the changes for undo/redo.
		/// </para>
		/// <para>
		/// When using an UndoManager, you should be making all changes within a transaction.
		/// After finishing or aborting a transaction, you can set <c>GoDocument.IsModified</c>
		/// to false, and then it will remain false until another change is made to the document.
		/// </para>
		/// <para>
		/// When there is no <see cref="P:Northwoods.Go.GoDocument.UndoManager" />, this property is
		/// implemented as only a simple boolean state variable.
		/// </para>
		/// </remarks>
		public bool IsModified
		{
			get
			{
				if (UndoManager == null)
				{
					return myIsModified;
				}
				//if (UndoManager.CurrentEdit != null)//bylzy
				//{
				//	return true;
				//}
				if (myIsModified)
				{
					return myUndoEditIndex != UndoManager.UndoEditIndex;
				}
				return false;
			}
			set
			{
				bool num = myIsModified;
				myIsModified = value;
				if (!value && UndoManager != null)
				{
					myUndoEditIndex = UndoManager.UndoEditIndex;
				}
				if (num != value)
				{
					InvalidateViews();
				}
			}
		}

		/// <summary>
		/// Gets or sets whether any Changed event handlers are called upon a
		/// document or document object change.
		/// </summary>
		/// <value>
		/// A value of true means that any Changed event handlers and any
		/// UndoManager are not called.
		/// A value of false means that the notifications do take place.
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// <para>
		/// Warning!  You probably shouldn't be using this property.
		/// </para>
		/// <para>
		/// When this property is true, no views of this document will be updated
		/// as the document is changed, and no undo/redo information is kept.
		/// When you set the property to false again, you will need to make
		/// sure all the views are correct (you may wish to call <see cref="M:Northwoods.Go.GoDocument.InvalidateViews" />)
		/// and that the <see cref="P:Northwoods.Go.GoDocument.UndoManager" />
		/// (if any) is in a satisfactory state (you may wish to call
		/// <see cref="M:Northwoods.Go.GoUndoManager.Clear" />,
		/// so that it cannot be confused by the loss of any undo/redo
		/// information while this property was true).
		/// You may also need to update the document's bounds (<see cref="P:Northwoods.Go.GoDocument.TopLeft" />
		/// and <see cref="P:Northwoods.Go.GoDocument.Size" />) and perhaps call <see cref="M:Northwoods.Go.GoDocument.EnsureUniquePartID" />.
		/// No Changed event is raised when this property is set.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.SuspendsUpdates" />
		/// <seealso cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" />
		[Browsable(false)]
		public bool SuspendsUpdates
		{
			get
			{
				return mySuspendsUpdates;
			}
			set
			{
				mySuspendsUpdates = value;
				if (!value)
				{
					InvalidatePositionArray(null);
					foreach (GoLayer layer in Layers)
					{
						layer.ResetCaches();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="P:Northwoods.Go.GoDocument.UndoManager" /> is notified upon a
		/// document or document object change.
		/// </summary>
		/// <value>
		/// A value of true means the <see cref="P:Northwoods.Go.GoDocument.UndoManager" />'s
		/// <see cref="M:Northwoods.Go.GoUndoManager.DocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> method is not called.
		/// A value of false means that if there is an <see cref="P:Northwoods.Go.GoDocument.UndoManager" />,
		/// it is notified so that it can record changes for undo and redo purposes.
		/// The default value is false.
		/// </value>
		/// <remarks>
		/// You must be careful that any changes that occur while this property
		/// is true will not confuse the user when they perform Undo's and Redo's
		/// but the changes are not undone or redone.
		/// No Changed event is raised when this property is set.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />
		/// <seealso cref="P:Northwoods.Go.GoObject.SkipsUndoManager" />
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.Clear" />
		[Browsable(false)]
		public bool SkipsUndoManager
		{
			get
			{
				return mySkipsUndoManager;
			}
			set
			{
				mySkipsUndoManager = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this document is in the process of being initialized.
		/// </summary>
		/// <remarks>
		/// This property is provided as a standard way to indicate that the document
		/// is not yet completely initialized, thereby allowing some methods to optimize
		/// their behavior.
		/// During an undo or a redo, this property will be set to true.
		/// See the documentation for <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// Setting this property does not call <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </remarks>
		[Browsable(false)]
		public bool Initializing
		{
			get
			{
				return myInitializing;
			}
			set
			{
				myInitializing = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="P:Northwoods.Go.GoDocument.UndoManager" /> is serialized
		/// when the document is.
		/// </summary>
		/// <value>
		/// The default value is false for Windows Forms but is true for Web Forms.
		/// </value>
		/// <remarks>
		/// If you set this to true, you will need to make sure that all
		/// <see cref="T:Northwoods.Go.GoChangedEventArgs" /> that are remembered as changes by the
		/// undo manager are themselves serializable.
		/// This might be a problem if you are tracking a field
		/// that has a non-serializable value, since those values are naturally
		/// remembered by the <see cref="T:Northwoods.Go.GoChangedEventArgs" />.
		/// No Changed event is raised when this property is set.
		/// </remarks>
		[Description("Whether the UndoManager is serialized along with the document")]
		public bool SerializesUndoManager
		{
			get
			{
				return mySerializesUndoManager;
			}
			set
			{
				mySerializesUndoManager = value;
				if (value)
				{
					mySerializedUndoManager = myUndoManager;
				}
				else
				{
					mySerializedUndoManager = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets this document's data format name for the clipboard.
		/// </summary>
		/// <value>
		/// Normally this <c>String</c> will be the fully qualified name
		/// of this document <c>Type</c>.
		/// This value must not be null.
		/// </value>
		/// <remarks>
		/// The default value for this property ensures that the user will not
		/// be able to copy the selection from one kind of document and paste
		/// it into another kind of document, but that they can do so when the
		/// document classes are the same.  If you want to be able to copy and
		/// paste between views where the documents are of different classes,
		/// you will have to assign the same values for this property for
		/// both documents.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" />
		/// <seealso cref="M:Northwoods.Go.GoView.PasteFromClipboard" />
		[Description("The data format name used for the clipboard.")]
		public virtual string DataFormat
		{
			get
			{
				if (myDataFormat == null)
				{
					myDataFormat = GetType().FullName;
				}
				return myDataFormat;
			}
			set
			{
				if (myDataFormat == null)
				{
					myDataFormat = GetType().FullName;
				}
				string text = myDataFormat;
				if (value != null && text != value)
				{
					myDataFormat = value;
					RaiseChanged(206, 0, null, 0, text, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets the UndoManager for this document.
		/// </summary>
		/// <value>
		/// This value may be null, when there is no <see cref="T:Northwoods.Go.GoUndoManager" />.
		/// By default this value is null.
		/// </value>
		/// <remarks>
		/// Of course, when there is no UndoManager, the user cannot
		/// perform an Undo or a Redo.
		/// No Changed event is raised when this property is set.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" />
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("The UndoManager for this document.")]
		public virtual GoUndoManager UndoManager
		{
			get
			{
				return myUndoManager;
			}
			set
			{
				if (myUndoManager != value)
				{
					if (myUndoManager != null)
					{
						myUndoManager.RemoveDocument(this);
					}
					myUndoManager = value;
					if (SerializesUndoManager)
					{
						mySerializedUndoManager = value;
					}
					myIsModified = false;
					myUndoEditIndex = -2;
					if (myUndoManager != null)
					{
						myUndoManager.AddDocument(this);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets an integer value associated with this document.
		/// </summary>
		/// <value>
		/// The initial value is zero.
		/// </value>
		/// <seealso cref="P:Northwoods.Go.GoDocument.UserObject" />
		[DefaultValue(0)]
		[Description("An integer value associated with this document.")]
		public virtual int UserFlags
		{
			get
			{
				return myUserFlags;
			}
			set
			{
				int num = myUserFlags;
				if (num != value)
				{
					myUserFlags = value;
					RaiseChanged(221, 0, null, num, null, NullRect, value, null, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets an object associated with this document.
		/// </summary>
		/// <value>
		/// The initial value is null.  The value should be serializable.
		/// </value>
		/// <remarks>
		/// In some cases you may be able to avoid creating a subclass of <see cref="T:Northwoods.Go.GoDocument" />
		/// by using this property to hold your application specific state.
		/// However, in general it would be best to derive a subclass holding your custom
		/// fields, because you will be able to control access to your document state
		/// more clearly, and because you will be able to override various methods to customize
		/// behavior more cleanly.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.UserFlags" />
		[DefaultValue(null)]
		[Description("An object associated with this document.")]
		public virtual object UserObject
		{
			get
			{
				return myUserObject;
			}
			set
			{
				object obj = myUserObject;
				if (obj != value)
				{
					myUserObject = value;
					RaiseChanged(222, 0, null, 0, obj, NullRect, 0, value, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether a valid link can be made between two ports that may produce a
		/// directed or undirected cycle in this document.
		/// </summary>
		/// <value>
		/// This defaults to <see cref="F:Northwoods.Go.GoDocumentValidCycle.All" />, resulting in no checks for a
		/// new link possibly producing any kind of cycle or loop.
		/// </value>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" /> uses this property in the following manners:
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoDocumentValidCycle" /></term></listheader>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.All" /></term>
		/// <term>produces no checking for cycles.</term>
		/// </item>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.NotDirected" /></term>
		/// <term>checks for possible cycle consisting of directed links.
		/// This check is slower and consumes more memory than the "Fast" version,
		/// but will not fail even if there are cycles or loops in the graph.</term>
		/// </item>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.NotDirectedFast" /></term>
		/// <term>checks for possible cycle consisting of directed links,
		/// without concern for any existing directed cycles elsewhere in the graph.
		/// Thus using this mode may result in infinite recursion and stack overflows if
		/// there happen to be any directed cycles accessible from either port in a
		/// call to <see cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />.</term>
		/// </item>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.NotUndirected" /></term>
		/// <term>checks for possible cycle consisting of links in either direction.</term>
		/// </item>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.DestinationTree" /></term>
		/// <term>checks for possible links that would cause the graph no longer to be a tree,
		/// with each node having at most one source link.</term>
		/// </item>
		/// <item>
		/// <term><see cref="F:Northwoods.Go.GoDocumentValidCycle.SourceTree" /></term>
		/// <term>checks for possible links that would cause the graph no longer to be a tree,
		/// with each node having at most one destination link.</term>
		/// </item>
		/// </list>
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(GoDocumentValidCycle.All)]
		[Description("Whether a valid link can produce a cycle in the graph.")]
		public virtual GoDocumentValidCycle ValidCycle
		{
			get
			{
				return myValidCycle;
			}
			set
			{
				GoDocumentValidCycle goDocumentValidCycle = myValidCycle;
				if (goDocumentValidCycle != value)
				{
					myValidCycle = value;
					RaiseChanged(225, 0, null, (int)goDocumentValidCycle, null, NullRect, (int)value, 0, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether <see cref="T:Northwoods.Go.IGoRoutable" /> <see cref="T:Northwoods.Go.GoObject" />s should
		/// have their <see cref="T:Northwoods.Go.IGoRoutable" />.<see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" />
		/// method called immediately, or later when this property is set back to false.
		/// </summary>
		/// <value>
		/// The default value of this property is false.
		/// It is temporarily set to true in methods such as <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />,
		/// where the moving of multiple objects might cause repeated routing calls for the
		/// same objects.
		/// </value>
		/// <remarks>
		/// You probably shouldn't be using this property.
		/// No Changed event is raised when this property is set.
		/// If you set this property to false, you are also responsible for calling
		/// <see cref="M:Northwoods.Go.GoDocument.DoDelayedRouting(Northwoods.Go.IGoCollection)" /> in order to properly deal with any
		/// <see cref="T:Northwoods.Go.IGoRoutable" /> objects that have been added to the
		/// <see cref="P:Northwoods.Go.GoDocument.DelayedRoutings" /> collection.
		/// </remarks>
		[Browsable(false)]
		public bool SuspendsRouting
		{
			get
			{
				return mySuspendsRouting;
			}
			set
			{
				mySuspendsRouting = value;
			}
		}

		/// <summary>
		/// A collection of <see cref="T:Northwoods.Go.GoObject" />s that implement the <see cref="T:Northwoods.Go.IGoRoutable" />
		/// interface and whose <see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" /> needs to be called.
		/// </summary>
		/// <remarks>
		/// Modifications of this collection do not produce any <see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// nor are they recorded by the <see cref="T:Northwoods.Go.GoUndoManager" />.
		/// Normally the only modifications to this collection are implemented by the
		/// <see cref="T:Northwoods.Go.GoDocument" /> class; in particular by the <see cref="M:Northwoods.Go.GoDocument.UpdateRoute(Northwoods.Go.IGoRoutable)" />
		/// and <see cref="M:Northwoods.Go.GoDocument.DoDelayedRouting(Northwoods.Go.IGoCollection)" /> methods.
		/// </remarks>
		public GoCopyDelayedsCollection DelayedRoutings => myDelayedRoutings;

		/// <summary>
		/// Gets or sets when <see cref="T:Northwoods.Go.IGoRoutable" /> objects should have their
		/// routes recalculated.
		/// </summary>
		/// <value>
		/// The default value is <see cref="F:Northwoods.Go.GoRoutingTime.Delayed" />,
		/// so that routable objects have their routes recalculated by
		/// <see cref="M:Northwoods.Go.GoDocument.DoDelayedRouting(Northwoods.Go.IGoCollection)" />, such as at the end of the implementations of
		/// <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />,
		/// <see cref="M:Northwoods.Go.GoView.EditPaste" /> and (in full Windows Forms) <c>GoView.DoExternalDrop</c>.
		/// </value>
		[Category("Behavior")]
		[DefaultValue(GoRoutingTime.Delayed)]
		[Description("When IGoRoutable objects are routed")]
		public virtual GoRoutingTime RoutingTime
		{
			get
			{
				return myRoutingTime;
			}
			set
			{
				GoRoutingTime goRoutingTime = myRoutingTime;
				if (goRoutingTime != value)
				{
					myRoutingTime = value;
					RaiseChanged(228, 0, null, (int)goRoutingTime, null, NullRect, (int)value, null, NullRect);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the document should make sure each <see cref="T:Northwoods.Go.IGoIdentifiablePart" />
		/// has a unique <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Setting this property to true will call <see cref="M:Northwoods.Go.GoDocument.EnsureUniquePartID" />.
		/// When this property is false, any <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />'s are invalid.
		/// When this property is true, adding any object to this document will cause every
		/// <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> whose <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> is
		/// -1 to be assigned a unique part ID, a non-negative integer.
		/// </para>
		/// <para>
		/// If you add any <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> with a non-negative integer to
		/// a document, all parts (including parts nested inside groups) must have unique
		/// <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />s already.
		/// Since PartIDs are integers, the number of unique part IDs for the lifetime of a
		/// document is limited to the number of positive integers.
		/// </para>
		/// </remarks>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Whether all the IGoIdentifiableParts in this document have a unique PartID")]
		public bool MaintainsPartID
		{
			get
			{
				return myMaintainsPartID;
			}
			set
			{
				bool flag = myMaintainsPartID;
				if (flag == value)
				{
					return;
				}
				myMaintainsPartID = value;
				RaiseChanged(224, 0, null, 0, flag, NullRect, 0, value, NullRect);
				if (!Initializing)
				{
					if (value)
					{
						EnsureUniquePartID();
					}
					else
					{
						myParts = null;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the last PartID value that this document assigned to an
		/// <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> when <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is true.
		/// </summary>
		/// <value>
		/// Initially the value is -1.
		/// </value>
		/// <remarks>
		/// You should not set this property except when loading/deserializing an
		/// existing document.
		/// Setting this property at other times may cause inconsistent results, just as
		/// setting the <see cref="T:Northwoods.Go.IGoIdentifiablePart" />.<see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />
		/// at other times can cause problems.
		/// </remarks>
		public int LastPartID
		{
			get
			{
				return myLastPartID;
			}
			set
			{
				int num = myLastPartID;
				if (num != value)
				{
					myLastPartID = value;
					RaiseChanged(226, 0, null, num, null, NullRect, value, null, NullRect);
				}
			}
		}

		/// <summary>
		/// The Changed event is raised whenever a document or a part of a document is modified.
		/// </summary>
		/// <remarks>
		/// Any Changed event handlers should not modify this document or any part of this document.
		/// </remarks>
		public event GoChangedEventHandler Changed
		{
			add
			{
				myChangedEvent = (GoChangedEventHandler)Delegate.Combine(myChangedEvent, value);
			}
			remove
			{
				myChangedEvent = (GoChangedEventHandler)Delegate.Remove(myChangedEvent, value);
			}
		}

		/// <summary>
		/// Create a document containing one empty layer.
		/// </summary>
		public GoDocument()
		{
			myLayers.init(this);
			myLinksLayer = myLayers.Default;
		}

		/// <summary>
		/// Make a copy of this document.
		/// </summary>
		/// <returns>
		/// A new <c>GoDocument</c> containing copies of all the layers
		/// and all of their <see cref="T:Northwoods.Go.GoObject" />s.
		/// </returns>
		/// <remarks>
		/// The <see cref="P:Northwoods.Go.GoDocument.UndoManager" />, if any, is not copied or shared
		/// with the new document.
		/// </remarks>
		public virtual GoDocument Copy()
		{
			GoDocument goDocument = (GoDocument)MemberwiseClone();
			goDocument.myChangedEvent = null;
			goDocument.myChangedEventArgs = null;
			goDocument.myPositions = null;
			goDocument.mySkippedAvoidable = null;
			goDocument.myParts = null;
			goDocument.myUndoManager = null;
			goDocument.mySerializedUndoManager = null;
			goDocument.myUndoEditIndex = -2;
			goDocument.myLayers = new GoLayerCollection();
			goDocument.myLinksLayer = null;
			goDocument.myLayers.init(goDocument);
			goDocument.DefaultLayer.Identifier = DefaultLayer.Identifier;
			goDocument.MergeLayersFrom(this);
			if (goDocument.Layers.Count > 1)
			{
				GoLayer @default = Layers.Default;
				GoLayer goLayer = Layers.NextLayer(@default, -1);
				if (goLayer != null && goLayer.Identifier != null)
				{
					GoLayer dest = goDocument.Layers.Find(goLayer.Identifier);
					goDocument.Layers.MoveAfter(dest, goDocument.Layers.Default);
				}
				else
				{
					GoLayer goLayer2 = Layers.NextLayer(@default, 1);
					if (goLayer2 != null && goLayer2.Identifier != null)
					{
						GoLayer dest2 = goDocument.Layers.Find(goLayer2.Identifier);
						goDocument.Layers.MoveBefore(dest2, goDocument.Layers.Default);
					}
				}
			}
			goDocument.CopyFromCollection(this);
			goDocument.myIsModified = false;
			return goDocument;
		}

		/// <summary>
		/// Test serializing and deserializing this document, to help discover unserializable objects by
		/// getting <c>SerializationException</c>s when debugging serialization errors such as during a copy-and-paste.
		/// </summary>
		/// <returns>a copy of this document, including all of its contents and any referenced objects</returns>
		/// <remarks>
		/// <para>
		/// Serialization of a <see cref="T:Northwoods.Go.GoDocument" /> and its <see cref="T:Northwoods.Go.GoObject" />s is important
		/// for copy-and-paste and drag-and-drop (in Windows Forms) and for session state (in ASP.NET Web Forms).
		/// Sometimes copy-and-paste will result in missing objects, due to unserializable objects
		/// or serialization errors.
		/// </para>
		/// <para>
		/// You can call this method to help debug serialization problems.
		/// This method serializes this document into a <c>MemoryStream</c>
		/// and then immediately deserializes it back into a newly copied document.
		/// If there is a problem, you are likely to get a <c>SerializationException</c>,
		/// which should give you a clue as to which class needs to be attributed <c>Serializable</c>
		/// or which fields need to be attributed <c>NonSerialized</c>.
		/// Because this may consume significant time and space without actually changing anything,
		/// do not call this method in a production environment except when investigating a serialization problem.
		/// </para>
		/// </remarks>
		/// <example>
		/// This method is implemented as:
		/// <code>
		/// public GoDocument TestSerialization() {
		///   MemoryStream memstream = new MemoryStream();
		///   IFormatter oformatter = new BinaryFormatter();
		///   oformatter.Serialize(memstream, this);
		///   memstream.Position = 0;
		///   IFormatter iformatter = new BinaryFormatter();
		///   return iformatter.Deserialize(memstream) as GoDocument;
		/// }
		/// </code>
		/// </example>
		public GoDocument TestSerialization()
		{
			MemoryStream memoryStream = new MemoryStream();
			((IFormatter)new BinaryFormatter()).Serialize((Stream)memoryStream, (object)this);
			memoryStream.Position = 0L;
			return ((IFormatter)new BinaryFormatter()).Deserialize((Stream)memoryStream) as GoDocument;
		}

		/// <summary>
		/// Add an object to the <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" />,
		/// or to the <see cref="P:Northwoods.Go.GoDocument.LinksLayer" /> if the object is an <see cref="T:Northwoods.Go.IGoLink" />.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// It is an error if the <paramref name="obj" /> belongs to a different document
		/// or to a <see cref="T:Northwoods.Go.GoGroup" />.
		/// If the object already belongs to this document, nothing happens.
		/// If the object is a link, it adds the link to the <see cref="P:Northwoods.Go.GoDocument.LinksLayer" />
		/// rather than to the <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" />, as is conventional.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.AddCopy(Northwoods.Go.GoObject,System.Drawing.PointF)" />
		public virtual void Add(GoObject obj)
		{
			if (obj is IGoLink)
			{
				LinksLayer.Add(obj);
			}
			else
			{
				DefaultLayer.Add(obj);
			}
		}

		/// <summary>
		/// Remove an object from this document.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// It is an error if the <paramref name="obj" /> belongs to a different document.
		/// If it already has been removed from this document, this method does nothing.
		/// </remarks>
		public virtual bool Remove(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			GoLayer layer = obj.Layer;
			if (layer == null)
			{
				return false;
			}
			if (layer.Document != this)
			{
				throw new ArgumentException("Cannot remove object that does not belong to this document");
			}
			return layer.Remove(obj);
		}

		/// <summary>
		/// Determine if an object belongs to this document.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>
		/// This implementation currently depends on the object's <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// property to see if it is part of this document.
		/// </remarks>
		public virtual bool Contains(GoObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			GoLayer layer = obj.Layer;
			if (layer != null)
			{
				return layer.Document == this;
			}
			return false;
		}

		/// <summary>
		/// Remove all objects from all layers in this document.
		/// </summary>
		/// <remarks>
		/// All layers remain in the <see cref="P:Northwoods.Go.GoDocument.Layers" /> collection, but
		/// each layer will be <see cref="M:Northwoods.Go.GoLayer.Clear" />ed.
		/// </remarks>
		public virtual void Clear()
		{
			myParts = null;
			InvalidatePositionArray(null);
			foreach (GoLayer layer in Layers)
			{
				layer.Clear();
			}
		}

		/// <summary>
		/// Gets a newly allocated array of all of the GoObjects in this collection.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.IGoCollection.CopyArray" />
		public GoObject[] CopyArray()
		{
			GoObject[] array = new GoObject[Count];
			CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Copy references to all of the document objects into the given array of <see cref="T:Northwoods.Go.GoObject" />s.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public virtual void CopyTo(GoObject[] array, int index)
		{
			foreach (GoLayer layer in Layers)
			{
				foreach (GoObject item in layer)
				{
					if (index >= array.Length)
					{
						return;
					}
					array.SetValue(item, checked(index++));
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Layers.GetObjectEnumerator(forward: true);
		}

		IEnumerator<GoObject> IEnumerable<GoObject>.GetEnumerator()
		{
			return Layers.GetObjectEnumerator(forward: true);
		}

		/// <summary>
		/// Get an Enumerator that iterates over all layers to include all document objects.
		/// </summary>
		/// <returns></returns>
		public virtual GoLayerCollectionObjectEnumerator GetEnumerator()
		{
			return Layers.GetObjectEnumerator(forward: true);
		}

		/// <summary>
		/// Find the top-most (front-most) document object at a given point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> location in document coordinates.</param>
		/// <param name="selectableOnly"></param>
		/// <returns></returns>
		/// <remarks>
		/// This iterates over the collection of layers, backwards from top to bottom,
		/// calling <see cref="M:Northwoods.Go.GoLayer.PickObject(System.Drawing.PointF,System.Boolean)" />.
		/// If <paramref name="selectableOnly" /> is true but <see cref="M:Northwoods.Go.GoDocument.CanSelectObjects" />
		/// is false, this method returns null.
		/// Please note that if an object is found, it might not be a top-level object.
		/// In fact, when <paramref name="selectableOnly" /> is false, it is very likely
		/// that if any object is found at the given point, it will be a child of some
		/// group.
		/// </remarks>
		public virtual GoObject PickObject(PointF p, bool selectableOnly)
		{
			if (selectableOnly && !CanSelectObjects())
			{
				return null;
			}
			foreach (GoLayer backward in Layers.Backwards)
			{
				GoObject goObject = backward.PickObject(p, selectableOnly);
				if (goObject != null)
				{
					return goObject;
				}
			}
			return null;
		}

		/// <summary>
		/// Return a collection of objects that can be picked at a particular point.
		/// </summary>
		/// <param name="p">A <c>PointF</c> location in document coordinates.</param>
		/// <param name="selectableOnly">If true, only consider objects for which <see cref="M:Northwoods.Go.GoObject.CanSelect" /> is true.</param>
		/// <param name="coll">An <see cref="T:Northwoods.Go.IGoCollection" /> that is modified by adding results and then returned.
		/// If this value is null, a <see cref="T:Northwoods.Go.GoCollection" /> is allocated and returned.</param>
		/// <param name="max">A limit on the number of objects to be found and added to the result collection.</param>
		/// <returns>The <paramref name="coll" /> argument, or a newly allocated one if that was null.</returns>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoLayer.PickObjects(System.Drawing.PointF,System.Boolean,Northwoods.Go.IGoCollection,System.Int32)" /> on each of the layers of this document
		/// in reverse order, from front to back.
		/// Please note that when objects are found, they might not be a top-level objects.
		/// In fact, when <paramref name="selectableOnly" /> is false, it is very likely
		/// that if any object is found at the given point, it will be a child of some group.
		/// </remarks>
		public virtual IGoCollection PickObjects(PointF p, bool selectableOnly, IGoCollection coll, int max)
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
				return coll;
			}
			foreach (GoLayer backward in Layers.Backwards)
			{
				if (coll.Count >= max)
				{
					return coll;
				}
				backward.PickObjects(p, selectableOnly, coll, max);
			}
			return coll;
		}

		/// <summary>
		/// Return a collection of objects that are surrounded by a given rectangle.
		/// </summary>
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
		/// If <paramref name="pickstyle" /> is <c>GoPickInRectangleStyle.SelectableOnlyContained</c> and <see cref="M:Northwoods.Go.GoDocument.CanSelectObjects" /> is false,
		/// this will not add any objects to the result collection.
		/// </remarks>
		public IGoCollection PickObjectsInRectangle(RectangleF rect, GoPickInRectangleStyle pickstyle, IGoCollection coll, int max)
		{
			if (coll == null)
			{
				coll = new GoCollection
				{
					InternalChecksForDuplicates = false
				};
			}
			if (PickStyleSelectableOnly(pickstyle) && !CanSelectObjects())
			{
				return coll;
			}
			foreach (GoLayer layer in Layers)
			{
				if (coll.Count >= max)
				{
					return coll;
				}
				layer.PickObjectsInRectangle(rect, pickstyle, coll, max);
			}
			return coll;
		}

		internal static bool PickStyleContained(GoPickInRectangleStyle s)
		{
			return (s & GoPickInRectangleStyle.AnyContained) != 0;
		}

		internal static bool PickStyleIntersectsBounds(GoPickInRectangleStyle s)
		{
			return (s & GoPickInRectangleStyle.AnyIntersectsBounds) != 0;
		}

		internal static bool PickStyleAny(GoPickInRectangleStyle s)
		{
			return s < GoPickInRectangleStyle.SelectableOnlyContained;
		}

		internal static bool PickStyleSelectableOnly(GoPickInRectangleStyle s)
		{
			return s >= GoPickInRectangleStyle.SelectableOnlyContained;
		}

		/// <summary>
		/// Called when a document object's bounds changes to possibly update the document's bounds.
		/// </summary>
		/// <param name="obj"></param>
		/// <remarks>
		/// This method does nothing if <see cref="P:Northwoods.Go.GoDocument.FixedSize" /> is true.
		/// Otherwise it increases the <see cref="P:Northwoods.Go.GoDocument.Size" /> property and moves
		/// the <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> point farther towards negative coordinates
		/// as needed to include the <paramref name="obj" />'s bounds.
		/// By default this method never shrinks the document.
		/// Note also that this method is not called while <see cref="P:Northwoods.Go.GoDocument.SuspendsUpdates" /> is true.
		/// If you do add objects or modify their bounds while <see cref="P:Northwoods.Go.GoDocument.SuspendsUpdates" />
		/// is true, afterwards you can explicitly set <see cref="P:Northwoods.Go.GoDocument.TopLeft" /> and <see cref="P:Northwoods.Go.GoDocument.Size" />
		/// to accommodate the new or modified document objects.
		/// </remarks>
		public virtual void UpdateDocumentBounds(GoObject obj)
		{
			if (obj != null && !FixedSize)
			{
				SizeF size = Size;
				PointF topLeft = TopLeft;
				RectangleF bounds = obj.Bounds;
				float num = Math.Min(topLeft.X, bounds.X);
				float num2 = Math.Min(topLeft.Y, bounds.Y);
				float num3 = Math.Max(topLeft.X + size.Width, bounds.X + bounds.Width);
				float num4 = Math.Max(topLeft.Y + size.Height, bounds.Y + bounds.Height);
				float num5 = num3 - num;
				float num6 = num4 - num2;
				if (num < topLeft.X || num2 < topLeft.Y)
				{
					TopLeft = new PointF(num, num2);
				}
				if (num5 > size.Width || num6 > size.Height)
				{
					Size = new SizeF(num5, num6);
				}
			}
		}

		/// <summary>
		/// Search this document for an object whose <see cref="P:Northwoods.Go.IGoLabeledPart.Text" />
		/// property matches a given string, possibly considering the initial part of
		/// the part's text string and possibly comparing in a case-insensitive manner.
		/// </summary>
		/// <param name="s">the <c>String</c> to search for</param>
		/// <param name="prefix">
		/// true to just find parts beginning with <paramref name="s" />;
		/// false to compare the whole string
		/// </param>
		/// <param name="ignorecase">
		/// true to upcase both the <paramref name="s" /> search string as well
		/// as each part's text string;
		/// false to do a case-sensitive comparison
		/// </param>
		/// <param name="insidesubgraph">
		/// true to recurse into subgraphs;
		/// false to only consider top-level <see cref="T:Northwoods.Go.IGoLabeledPart" />'s
		/// </param>
		/// <returns>
		/// A <see cref="T:Northwoods.Go.GoObject" /> that implements <see cref="T:Northwoods.Go.IGoLabeledPart" />
		/// with a matching text string, or null if no such part exists in this document.
		/// </returns>
		public virtual GoObject FindNode(string s, bool prefix, bool ignorecase, bool insidesubgraph)
		{
			if (s == null)
			{
				return null;
			}
			string text = s;
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (ignorecase)
			{
				text = text.ToUpper(currentCulture);
			}
			return FindNodeInternal(this, text, prefix, ignorecase, insidesubgraph, currentCulture);
		}

		private GoObject FindNodeInternal(IGoCollection coll, string search, bool prefix, bool ignorecase, bool insidesubgraph, CultureInfo ci)
		{
			foreach (GoObject item in coll)
			{
				IGoLabeledPart goLabeledPart = item as IGoLabeledPart;
				if (goLabeledPart != null)
				{
					string text = goLabeledPart.Text;
					if (ignorecase)
					{
						text = text.ToUpper(ci);
					}
					if (prefix)
					{
						if (text.StartsWith(search))
						{
							return item;
						}
					}
					else if (text == search)
					{
						return item;
					}
					if (insidesubgraph)
					{
						GoSubGraphBase goSubGraphBase = item as GoSubGraphBase;
						if (goSubGraphBase != null)
						{
							GoObject goObject = FindNodeInternal(goSubGraphBase, search, prefix, ignorecase, insidesubgraph, ci);
							if (goObject != null)
							{
								return goObject;
							}
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Find a top-level object whose <see cref="P:Northwoods.Go.IGoLabeledPart.Text" /> property value
		/// matches a given string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="prefix"></param>
		/// <param name="ignorecase"></param>
		/// <returns>
		/// The result of calling <c>FindNode(s, prefix, ignorecase, false)</c>
		/// </returns>
		/// <remarks>
		/// This method's functionality is implemented by <see cref="M:Northwoods.Go.GoDocument.FindNode(System.String,System.Boolean,System.Boolean,System.Boolean)" />.
		/// </remarks>
		public GoObject FindNode(string s, bool prefix, bool ignorecase)
		{
			return FindNode(s, prefix, ignorecase, insidesubgraph: false);
		}

		/// <summary>
		/// Find a top-level object whose <see cref="P:Northwoods.Go.IGoLabeledPart.Text" /> property value is
		/// exactly the same as a given string.
		/// </summary>
		/// <param name="s">the <c>String</c> to search for</param>
		/// <returns>
		/// The result of <c>FindNode(s, false, false, false)</c>.
		/// </returns>
		/// <remarks>
		/// This method's functionality is implemented by <see cref="M:Northwoods.Go.GoDocument.FindNode(System.String,System.Boolean,System.Boolean,System.Boolean)" />.
		/// </remarks>
		public GoObject FindNode(string s)
		{
			return FindNode(s, prefix: false, ignorecase: false, insidesubgraph: false);
		}

		/// <summary>
		/// Get the smallest rectangle that includes the bounds of all of the
		/// objects in a collection.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="view">May be null.</param>
		/// <returns>
		/// A <c>RectangleF</c> that encloses all of the objects in the
		/// collection, which might not include the (0, 0) origin point
		/// </returns>
		/// <remarks>
		/// This method uses <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" /> to include
		/// areas beyond the immediate <see cref="P:Northwoods.Go.GoObject.Bounds" />, perhaps
		/// affected by the <paramref name="view" />.
		/// This ignores non-visible objects, by checking <see cref="M:Northwoods.Go.GoObject.CanView" />.
		/// If the view <see cref="P:Northwoods.Go.GoView.IsPrinting" />, this checks <see cref="M:Northwoods.Go.GoObject.CanPrint" /> instead.
		/// </remarks>
		public static RectangleF ComputeBounds(IGoCollection coll, GoView view)
		{
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			bool flag2 = view?.IsPrinting ?? false;
			foreach (GoObject item in coll)
			{
				if (!(flag2 ? (!item.CanPrint()) : (!item.CanView())))
				{
					RectangleF bounds = item.Bounds;
					RectangleF rectangleF = item.ExpandPaintBounds(bounds, view);
					if (!flag)
					{
						flag = true;
						num = rectangleF.X;
						num2 = rectangleF.Y;
						num3 = rectangleF.X + rectangleF.Width;
						num4 = rectangleF.Y + rectangleF.Height;
					}
					else
					{
						if (rectangleF.X < num)
						{
							num = rectangleF.X;
						}
						if (rectangleF.Y < num2)
						{
							num2 = rectangleF.Y;
						}
						if (rectangleF.X + rectangleF.Width > num3)
						{
							num3 = rectangleF.X + rectangleF.Width;
						}
						if (rectangleF.Y + rectangleF.Height > num4)
						{
							num4 = rectangleF.Y + rectangleF.Height;
						}
					}
				}
			}
			if (flag)
			{
				return new RectangleF(num, num2, num3 - num, num4 - num2);
			}
			return default(RectangleF);
		}

		/// <summary>
		/// Call <see cref="M:Northwoods.Go.GoDocument.ComputeBounds(Northwoods.Go.IGoCollection,Northwoods.Go.GoView)" />
		/// on this document but with no view.
		/// </summary>
		/// <returns></returns>
		public RectangleF ComputeBounds()
		{
			return ComputeBounds(this, null);
		}

		/// <summary>
		/// Called to see if the user can select objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowSelect</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoView.SelectInRectangle(System.Drawing.RectangleF)" />
		/// and <see cref="M:Northwoods.Go.GoDocument.PickObject(System.Drawing.PointF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowSelect" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanSelectObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanSelect" />
		public virtual bool CanSelectObjects()
		{
			return AllowSelect;
		}

		/// <summary>
		/// Called to see if the user can move selected objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowMove</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowMove" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanMoveObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanMove" />
		public virtual bool CanMoveObjects()
		{
			return AllowMove;
		}

		/// <summary>
		/// Called to see if the user can copy selected objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowCopy</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowCopy" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanCopyObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanCopy" />
		public virtual bool CanCopyObjects()
		{
			return AllowCopy;
		}

		/// <summary>
		/// Called to see if the user can resize selected objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowResize</c>,
		/// This property is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowResize" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanResizeObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanResize" />
		public virtual bool CanResizeObjects()
		{
			return AllowResize;
		}

		/// <summary>
		/// Called to see if the user can reshape resizable objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowReshape</c>,
		/// This property is used by <see cref="T:Northwoods.Go.GoToolResizing" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowReshape" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanReshapeObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanReshape" />
		public virtual bool CanReshapeObjects()
		{
			return AllowReshape;
		}

		/// <summary>
		/// Called to see if the user can delete selected objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowDelete</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoView.DeleteSelection(Northwoods.Go.GoSelection)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowDelete" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanDeleteObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanDelete" />
		public virtual bool CanDeleteObjects()
		{
			return AllowDelete;
		}

		/// <summary>
		/// Called to see if the user can insert objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowInsert</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoView.EditPaste" /> and by
		/// <see cref="T:Northwoods.Go.GoToolDragging" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowInsert" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanInsertObjects" />
		public virtual bool CanInsertObjects()
		{
			return AllowInsert;
		}

		/// <summary>
		/// Called to see if the user can link objects in this document.
		/// </summary>
		/// <remarks>
		/// This just returns <c>AllowLink</c>.
		/// This property is used by <see cref="T:Northwoods.Go.GoToolLinking" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowLink" />
		/// <seealso cref="M:Northwoods.Go.GoPort.CanLinkFrom" />
		/// <seealso cref="M:Northwoods.Go.GoPort.CanLinkTo" />
		public virtual bool CanLinkObjects()
		{
			return AllowLink;
		}

		/// <summary>
		/// Called to see if the user can edit objects in this document.
		/// </summary>
		/// <remarks>
		/// By default this just returns <c>AllowEdit</c>,
		/// This property is used by methods such as <see cref="M:Northwoods.Go.GoText.OnSingleClick(Northwoods.Go.GoInputEventArgs,Northwoods.Go.GoView)" />.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.AllowEdit" />
		/// <seealso cref="M:Northwoods.Go.GoLayer.CanEditObjects" />
		/// <seealso cref="M:Northwoods.Go.GoObject.CanEdit" />
		public virtual bool CanEditObjects()
		{
			return AllowEdit;
		}

		/// <summary>
		/// This method sets some properties that determine whether the user can
		/// modify the document from any view.
		/// </summary>
		/// <param name="b"></param>
		/// <remarks>
		/// By default this just sets the <see cref="P:Northwoods.Go.GoDocument.AllowMove" />, <see cref="P:Northwoods.Go.GoDocument.AllowResize" />, 
		/// <see cref="P:Northwoods.Go.GoDocument.AllowReshape" />, <see cref="P:Northwoods.Go.GoDocument.AllowDelete" />, <see cref="P:Northwoods.Go.GoDocument.AllowInsert" />, 
		/// <see cref="P:Northwoods.Go.GoDocument.AllowLink" />, and <see cref="P:Northwoods.Go.GoDocument.AllowEdit" /> properties.
		/// You may want to override this in order to also control other properties you
		/// may have defined that govern the user's ability to modify this document.
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
		/// Called when any part of this document has changed, to invoke all Changed event handlers.
		/// </summary>
		/// <param name="evt"></param>
		/// <remarks>
		/// This method is called after this document of a part of this document has been modified.
		/// To avoid confusion, this method and any method that it calls should not modify the
		/// document.
		/// Besides invoking all Changed event handlers, this also calls
		/// <see cref="M:Northwoods.Go.GoUndoManager.DocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> if there is an <see cref="P:Northwoods.Go.GoDocument.UndoManager" />
		/// and sets <see cref="P:Northwoods.Go.GoDocument.IsModified" /> to true, unless <see cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" /> is true.
		/// This method calls <see cref="M:Northwoods.Go.GoDocument.UpdateDocumentBounds(Northwoods.Go.GoObject)" />
		/// if an object is inserted into a layer or it its bounds change.
		/// An insertion or removal of an object from a layer, if <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is true
		/// and the object is an <see cref="T:Northwoods.Go.IGoIdentifiablePart" />,
		/// will also update the object's <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> and
		/// the document's internal table of part ID mappings.
		/// </remarks>
		protected virtual void OnChanged(GoChangedEventArgs evt)
		{
			if (myChangedEvent != null)
			{
				myChangedEvent(this, evt);
			}
			int hint = evt.Hint;
			if (!SkipsUndoManager)
			{
				UndoManager?.DocumentChanged(this, evt);
				if ((hint < 0 || hint >= 200) && (hint != 901 || evt.SubHint != 1000)&& evt.SubHint != 1013 && evt.SubHint != 1702)
				{
					IsModified = true;
				}
			}
			switch (hint)
			{
			case 901:
				if (evt.SubHint == 1001)
				{
					GoObject goObject = evt.GoObject;
					UpdateDocumentBounds(goObject);
					InvalidatePositionArray(goObject);
					if (goObject.IsTopLevel)
					{
						goObject.Layer?.UpdateCaches(goObject, evt);
					}
				}
				else if (evt.SubHint == 1051)
				{
					GoObject goObject2 = evt.NewValue as GoObject;
					if (goObject2 != null)
					{
						if (goObject2.Layer != null)
						{
							goObject2.Layer.ResetPickCache();
						}
						if (MaintainsPartID)
						{
							AddAllParts(goObject2);
						}
					}
				}
				else if (evt.SubHint == 1052)
				{
					GoObject goObject3 = evt.OldValue as GoObject;
					if (goObject3 != null)
					{
						if (goObject3.Layer != null)
						{
							goObject3.Layer.ResetPickCache();
						}
						RemoveAllParts(goObject3);
					}
				}
				else if (evt.SubHint == 1004)
				{
					GoObject goObject4 = evt.GoObject;
					if (goObject4 != null && goObject4.Layer != null)
					{
						goObject4.Layer.ResetPickCache();
					}
				}
				break;
			case 902:
			{
				GoObject goObject6 = evt.GoObject;
				if (MaintainsPartID)
				{
					AddAllParts(goObject6);
				}
				UpdateDocumentBounds(goObject6);
				InvalidatePositionArray(goObject6);
				break;
			}
			case 903:
			{
				GoObject goObject5 = evt.GoObject;
				RemoveAllParts(goObject5);
				InvalidatePositionArray(goObject5);
				break;
			}
			case 801:
				InvalidatePositionArray(null);
				break;
			case 802:
				InvalidatePositionArray(null);
				if (evt.Object == LinksLayer)
				{
					LinksLayer = DefaultLayer;
				}
				break;
			}
		}

		/// <summary>
		/// Any change to a document or to a part of a document may call this method
		/// to invoke the <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" /> method, after the change has occurred.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="subhint"></param>
		/// <param name="obj"></param>
		/// <param name="oldI"></param>
		/// <param name="oldVal"></param>
		/// <param name="oldRect"></param>
		/// <param name="newI"></param>
		/// <param name="newVal"></param>
		/// <param name="newRect"></param>
		/// <remarks>
		/// <para>
		/// This implementation tries to reuse a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> instance
		/// that it initializes with the information in the parameters before calling
		/// <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" />.
		/// This method is often called by <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </para>
		/// <para>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoDocument" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.RepaintAll" /></term> <term>100</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.BeginUpdateAllViews" /></term> <term>101</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.EndUpdateAllViews" /></term> <term>102</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.UpdateAllViews" /></term> <term>103</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.StartedTransaction" /></term> <term>104</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.FinishedTransaction" /></term> <term>105</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.AbortedTransaction" /></term> <term>106</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.StartingUndo" /></term> <term>107</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.FinishedUndo" /></term> <term>108</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.StartingRedo" /></term> <term>109</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.FinishedRedo" /></term> <term>110</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedName" /></term> <term>201</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedSize" /></term> <term>202</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedTopLeft" /></term> <term>203</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedFixedSize" /></term> <term>204</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedPaperColor" /></term> <term>205</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedDataFormat" /></term> <term>206</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowSelect" /></term> <term>207</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowMove" /></term> <term>208</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowCopy" /></term> <term>209</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowResize" /></term> <term>210</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowReshape" /></term> <term>211</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowDelete" /></term> <term>212</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowInsert" /></term> <term>213</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowLink" /></term> <term>214</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedAllowEdit" /></term> <term>215</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.AllArranged" /></term> <term>220</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedUserFlags" /></term> <term>221</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedUserObject" /></term> <term>222</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedLinksLayer" /></term> <term>223</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedMaintainsPartID" /></term> <term>224</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedValidCycle" /></term> <term>225</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedLastPartID" /></term> <term>226</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedRoutingTime" /></term> <term>228</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.ChangedInitializing" /></term> <term>241</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoDocument.LastHint" /></term> <term>10000</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoLayerCollection" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" /></term> <term>801</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayerCollection.RemovedLayer" /></term> <term>802</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayerCollection.MovedLayer" /></term> <term>803</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayerCollection.ChangedDefault" /></term> <term>804</term> </item>
		/// </list>
		/// <list type="table">
		/// <listheader><term><see cref="T:Northwoods.Go.GoLayer" /></term></listheader>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedObject" /></term> <term>901 See also the GoObject.Changed method: <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /></term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.InsertedObject" /></term> <term>902</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.RemovedObject" /></term> <term>903</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedObjectLayer" /></term> <term>904</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowView" /></term> <term>910</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowSelect" /></term> <term>911</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowMove" /></term> <term>912</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowCopy" /></term> <term>913</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowResize" /></term> <term>914</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowReshape" /></term> <term>915</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowDelete" /></term> <term>916</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowInsert" /></term> <term>917</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowLink" /></term> <term>918</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowEdit" /></term> <term>919</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedAllowPrint" /></term> <term>920</term> </item>
		/// <item> <term><see cref="F:Northwoods.Go.GoLayer.ChangedIdentifier" /></term> <term>930</term> </item>
		/// </list>
		/// Please note that this list may not be complete--in fact you are encouraged to
		/// add new subhints for your own properties and other changes.
		/// </para>
		/// </remarks>
		public virtual void RaiseChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect)
		{
			invokeOnChanged(hint, subhint, obj, oldI, oldVal, oldRect, newI, newVal, newRect, before: false);
		}

		/// <summary>
		/// Call this method to invoke the <see cref="M:Northwoods.Go.GoDocument.OnChanged(Northwoods.Go.GoChangedEventArgs)" /> method before
		/// any change occurs.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="subhint"></param>
		/// <param name="obj"></param>
		/// <remarks>
		/// You should call this method before making any changes for which the
		/// call to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> does not have the complete previous
		/// (old) state.
		/// You should also implement <see cref="M:Northwoods.Go.GoDocument.CopyOldValueForUndo(Northwoods.Go.GoChangedEventArgs)" /> and
		/// <see cref="M:Northwoods.Go.GoDocument.CopyNewValueForRedo(Northwoods.Go.GoChangedEventArgs)" /> to record the state information for
		/// the particular hint passed to this method.
		/// When the call to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> has the complete before-and-after
		/// state as arguments, calling this method is not necessary.
		/// You will typically call <see cref="M:Northwoods.Go.GoDocument.RaiseChanging(System.Int32,System.Int32,System.Object)" /> followed by a call
		/// to <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> when making a bunch of changes while
		/// <see cref="P:Northwoods.Go.GoDocument.SuspendsUpdates" /> is true, if you still want to maintain
		/// the ability to undo and redo all of those changes.
		/// </remarks>
		public virtual void RaiseChanging(int hint, int subhint, object obj)
		{
			invokeOnChanged(hint, subhint, obj, 0, null, NullRect, 0, null, NullRect, before: true);
		}

		private void invokeOnChanged(int hint, int subhint, object obj, int oldI, object oldVal, RectangleF oldRect, int newI, object newVal, RectangleF newRect, bool before)
		{
			if (!SuspendsUpdates)
			{
				GoChangedEventArgs goChangedEventArgs = null;
				lock (this)
				{
					goChangedEventArgs = myChangedEventArgs;
					myChangedEventArgs = null;
				}
				if (goChangedEventArgs == null)
				{
					goChangedEventArgs = new GoChangedEventArgs();
					goChangedEventArgs.Document = this;
				}
				goChangedEventArgs.IsBeforeChanging = before;
				goChangedEventArgs.Hint = hint;
				goChangedEventArgs.SubHint = subhint;
				goChangedEventArgs.Object = obj;
				goChangedEventArgs.OldInt = oldI;
				goChangedEventArgs.OldValue = oldVal;
				goChangedEventArgs.OldRect = oldRect;
				goChangedEventArgs.NewInt = newI;
				goChangedEventArgs.NewValue = newVal;
				goChangedEventArgs.NewRect = newRect;
				OnChanged(goChangedEventArgs);
				myChangedEventArgs = goChangedEventArgs;
				goChangedEventArgs.Object = null;
				goChangedEventArgs.OldValue = null;
				goChangedEventArgs.NewValue = null;
			}
		}

		/// <summary>
		/// Cause all views to completely repaint sometime in the future.
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="F:Northwoods.Go.GoDocument.RepaintAll" /> hint.
		/// This is only useful for Windows Forms.
		/// </remarks>
		public void InvalidateViews()
		{
			RaiseChanged(100, 0, null, 0, null, NullRect, 0, null, NullRect);
		}

		/// <summary>
		/// Cause all views to stop painting.
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="F:Northwoods.Go.GoDocument.BeginUpdateAllViews" /> hint.
		/// <see cref="M:Northwoods.Go.GoView.OnDocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> will detect this hint and
		/// call <c>GoView.BeginUpdate</c>.
		/// Note that preventing views from painting does not keep them from
		/// receiving and handling <see cref="E:Northwoods.Go.GoDocument.Changed" /> events,
		/// nor does it interfere with any other event handlers or with any
		/// <see cref="P:Northwoods.Go.GoDocument.UndoManager" />.
		/// This is only useful for Windows Forms.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.EndUpdateViews" />
		public void BeginUpdateViews()
		{
			RaiseChanged(101, 0, null, 0, null, NullRect, 0, null, NullRect);
		}

		/// <summary>
		/// Cause all views to continue painting normally.
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="F:Northwoods.Go.GoDocument.EndUpdateAllViews" /> hint.
		/// <see cref="M:Northwoods.Go.GoView.OnDocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> will detect this hint and
		/// call <c>GoView.EndUpdate</c>.
		/// This is only useful for Windows Forms.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.BeginUpdateViews" />
		public void EndUpdateViews()
		{
			RaiseChanged(102, 0, null, 0, null, NullRect, 0, null, NullRect);
		}

		/// <summary>
		/// Cause all views to repaint their invalidated areas.
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="F:Northwoods.Go.GoDocument.UpdateAllViews" /> hint.
		/// <see cref="M:Northwoods.Go.GoView.OnDocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" /> will detect this hint and
		/// call <c>Control.Update</c>.
		/// This is only useful for Windows Forms.
		/// </remarks>
		public void UpdateViews()
		{
			RaiseChanged(103, 0, null, 0, null, NullRect, 0, null, NullRect);
		}

		/// <summary>
		/// Allocate a <see cref="T:Northwoods.Go.GoCopyDictionary" /> for use in a call to
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />
		/// </summary>
		/// <returns>this must not return null</returns>
		/// <example>
		/// By default this also sets the <see cref="P:Northwoods.Go.GoCopyDictionary.DestinationDocument" /> to
		/// this document:
		/// <code>
		///   GoCopyDictionary env = new GoCopyDictionary();
		///   env.DestinationDocument = this;
		///   return env;
		/// </code>
		/// </example>
		public virtual GoCopyDictionary CreateCopyDictionary()
		{
			return new GoCopyDictionary
			{
				DestinationDocument = this
			};
		}

		/// <summary>
		/// Add a copy of an object into this document at the given point.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="loc"></param>
		/// <returns>The newly inserted object.</returns>
		/// <remarks>
		/// <para>
		/// This creates a singleton collection and calls
		/// <see cref="M:Northwoods.Go.GoDocument.CopyFromCollection(Northwoods.Go.IGoCollection,System.Boolean,System.Boolean,System.Drawing.SizeF,Northwoods.Go.GoCopyDictionary)" />
		/// with the appropriate offset.
		/// This method is different from <see cref="M:Northwoods.Go.GoDocument.Add(Northwoods.Go.GoObject)" /> in that the latter method
		/// causes the document to own the object; i.e., the object's <see cref="P:Northwoods.Go.GoObject.Layer" />
		/// property is modified to be this document's default layer.
		/// But this method does not modify the <paramref name="obj" /> and may add the
		/// copy to a different layer, if <paramref name="obj" /> belongs to a layer and
		/// <c>CopyFromCollection</c> can find a matching layer in this document.
		/// </para>
		/// <para>
		/// If you want to make a copy of a <see cref="T:Northwoods.Go.GoObject" /> but do not
		/// want to add it to a document, you can call <see cref="T:Northwoods.Go.GoCopyDictionary" />.<see cref="M:Northwoods.Go.GoCopyDictionary.CopyComplete(Northwoods.Go.GoObject)" />
		/// or <see cref="T:Northwoods.Go.GoObject" />.<see cref="M:Northwoods.Go.GoObject.Copy" />.
		/// </para>
		/// </remarks>
		public GoObject AddCopy(GoObject obj, PointF loc)
		{
			PointF location = obj.Location;
			GoCollection goCollection = new GoCollection();
			goCollection.Add(obj);
			SizeF offset = GoTool.SubtractPoints(loc, location);
			return CopyFromCollection(goCollection, copyableOnly: false, dragging: false, offset, null)[obj] as GoObject;
		}

		/// <summary>
		/// Make copies of a collection of objects in this document.
		/// </summary>
		/// <param name="coll"></param>
		/// <returns>A <see cref="T:Northwoods.Go.GoCopyDictionary" /> describing the results of the copy.</returns>
		public GoCopyDictionary CopyFromCollection(IGoCollection coll)
		{
			return CopyFromCollection(coll, copyableOnly: false, dragging: false, default(SizeF), null);
		}

		/// <summary>
		/// Add a copy of the objects in the given collection into this document, with locations
		/// offset from their existing locations.
		/// </summary>
		/// <param name="coll">the <see cref="T:Northwoods.Go.IGoCollection" /> of <see cref="T:Northwoods.Go.GoObject" />s</param>
		/// <param name="copyableOnly">whether to heed the <see cref="M:Northwoods.Go.GoObject.CanCopy" />
		/// property for each object that might be copied, and whether to heed the
		/// <see cref="M:Northwoods.Go.GoLayer.CanInsertObjects" /> predicate for the corresponding destination layer</param>
		/// <param name="dragging">whether to copy the <see cref="P:Northwoods.Go.GoObject.DraggingObject" /> instead
		/// of the object itself</param>
		/// <param name="offset">the <c>SizeF</c> offset in document coordinates</param>
		/// <param name="env">
		/// the copy dictionary which can control and record the results of the copy;
		/// if null it calls <see cref="M:Northwoods.Go.GoDocument.CreateCopyDictionary" /> to get one
		/// </param>
		/// <returns>the <see cref="T:Northwoods.Go.GoCopyDictionary" /> holding the results of the copy</returns>
		/// <remarks>
		/// This is a two-pass process.
		/// In the first pass, this method copies everything, by calling
		/// <see cref="M:Northwoods.Go.GoCopyDictionary.Copy(Northwoods.Go.GoObject)" /> on each object and then adding the
		/// object to the appropriate layer in this document.
		/// The layer is chosen to be one that matches the layer of the original object,
		/// by calling <see cref="M:Northwoods.Go.GoLayerCollection.Find(System.Object)" />.  If no such layer is found,
		/// it adds the new object to the default layer.
		/// The second pass checks for any objects that were delayed during the copy.
		/// Any such objects have their <see cref="M:Northwoods.Go.GoObject.CopyObjectDelayed(Northwoods.Go.GoCopyDictionary,Northwoods.Go.GoObject)" />
		/// method called to fix up whatever is needed.
		/// </remarks>
		public virtual GoCopyDictionary CopyFromCollection(IGoCollection coll, bool copyableOnly, bool dragging, SizeF offset, GoCopyDictionary env)
		{
			if (env == null)
			{
				env = CreateCopyDictionary();
			}
			env.SourceCollection = coll;
			Dictionary<GoObject, bool> dictionary = new Dictionary<GoObject, bool>();
			GoCollection goCollection = null;
			GoCollection goCollection2 = new GoCollection();
			goCollection2.InternalChecksForDuplicates = false;
			GoCollection goCollection3 = null;
			foreach (GoObject item in coll)
			{
				GoObject goObject = dragging ? item.DraggingObject : item;
				if (goObject != null && (!copyableOnly || goObject.CanCopy()) && !alreadyCopied(dictionary, goObject))
				{
					if (goCollection != null && goObject is GoGroup)
					{
						foreach (GoObject item2 in goCollection)
						{
							if (item2.IsChildOf(goObject))
							{
								dictionary.Remove(item2);
								if (goCollection3 == null)
								{
									goCollection3 = new GoCollection();
									goCollection3.InternalChecksForDuplicates = false;
								}
								goCollection3.Add(item2);
								goCollection2.Remove(item2);
							}
						}
						if (goCollection3 != null && !goCollection3.IsEmpty)
						{
							foreach (GoObject item3 in goCollection3)
							{
								goCollection.Remove(item3);
							}
							goCollection3.Clear();
						}
					}
					dictionary.Add(goObject, value: true);
					if (!goObject.IsTopLevel)
					{
						if (goCollection == null)
						{
							goCollection = new GoCollection();
							goCollection.InternalChecksForDuplicates = false;
						}
						goCollection.Add(goObject);
					}
					goCollection2.Add(goObject);
				}
			}
			PointF pointF = default(PointF);
			foreach (GoObject item4 in goCollection2)
			{
				GoObject goObject2 = env[item4] as GoObject;
				if (goObject2 == null)
				{
					goObject2 = env.Copy(item4);
					if (goObject2 != null)
					{
						pointF = goObject2.Location;
						goObject2.Location = goObject2.ComputeMove(pointF, new PointF(pointF.X + offset.Width, pointF.Y + offset.Height));
						GoLayer layer = item4.Layer;
						GoLayer goLayer = null;
						if (layer != null)
						{
							goLayer = ((layer.Document != this) ? Layers.Find(layer.Identifier) : layer);
						}
						if (goLayer == null)
						{
							goLayer = DefaultLayer;
						}
						if (!copyableOnly || goLayer.CanInsertObjects())
						{
							goLayer.Add(goObject2);
						}
					}
				}
			}
			env.FinishDelayedCopies();
			return env;
		}

		private static bool alreadyCopied(Dictionary<GoObject, bool> copieds, GoObject o)
		{
			for (GoObject goObject = o; goObject != null; goObject = goObject.Parent)
			{
				if (copieds.TryGetValue(goObject, out bool _))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Make sure this document has all of the identified layers of another document.
		/// </summary>
		/// <param name="other"></param>
		/// <remarks>
		/// Only non-null <see cref="P:Northwoods.Go.GoLayer.Identifier" />s of the layers of the
		/// <paramref name="other" /> document are checked for presence in this document.
		/// No <see cref="T:Northwoods.Go.GoObject" />s are inserted or removed from any layer.
		/// No pre-existing layers are removed or reordered.
		/// The <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" /> property is changed to match that of
		/// the other document too, if its <see cref="P:Northwoods.Go.GoDocument.DefaultLayer" /> has an identifier.
		/// The <see cref="P:Northwoods.Go.GoDocument.LinksLayer" /> is also updated to match that of the other
		/// document, if that layer has an identifier.
		/// The principal use of this method is to initialize a clipboard document--
		/// <see cref="M:Northwoods.Go.GoView.CopyToClipboard(Northwoods.Go.IGoCollection)" />.
		/// </remarks>
		public virtual void MergeLayersFrom(GoDocument other)
		{
			foreach (GoLayer layer in other.Layers)
			{
				object identifier = layer.Identifier;
				if (identifier != null && Layers.Find(identifier) == null)
				{
					Layers.CreateNewLayerAfter(Layers.Top).Identifier = identifier;
				}
			}
			object identifier2 = other.DefaultLayer.Identifier;
			GoLayer goLayer = Layers.Find(identifier2);
			if (goLayer != null)
			{
				DefaultLayer = goLayer;
			}
			object identifier3 = other.LinksLayer.Identifier;
			GoLayer goLayer2 = Layers.Find(identifier3);
			if (goLayer2 != null)
			{
				LinksLayer = goLayer2;
			}
		}

		/// <summary>
		/// This predicate is true if this document has an UndoManager whose CanUndo predicate is true.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoUndoManager.CanUndo" /></returns>
		public virtual bool CanUndo()
		{
			return UndoManager?.CanUndo() ?? false;
		}

		/// <summary>
		/// Call the UndoManager's <see cref="M:Northwoods.Go.GoUndoManager.Undo" /> method if <see cref="M:Northwoods.Go.GoDocument.CanUndo" /> is true.
		/// </summary>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Undo" /> will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.StartingUndo" /> before actually
		/// performing the undo, and will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedUndo" /> afterwards.
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" /> before calling
		/// <see cref="M:Northwoods.Go.GoUndoManager.Undo" />.
		/// </remarks>
		public virtual void Undo()
		{
			if (CanUndo())
			{
				UndoManager?.Undo();
			}
		}

		/// <summary>
		/// This predicate is true if this document has an UndoManager whose CanRedo predicate is true.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoUndoManager.CanRedo" /></returns>
		public virtual bool CanRedo()
		{
			return UndoManager?.CanRedo() ?? false;
		}

		/// <summary>
		/// Call the UndoManager's <see cref="M:Northwoods.Go.GoUndoManager.Redo" /> method if <see cref="M:Northwoods.Go.GoDocument.CanRedo" /> is true.
		/// </summary>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.Redo" /> will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.StartingRedo" /> before actually
		/// performing the redo, and will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedRedo" /> afterwards.
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" /> before calling
		/// <see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </remarks>
		public virtual void Redo()
		{
			if (CanRedo())
			{
				UndoManager?.Redo();
			}
		}

		/// <summary>
		/// Call the UndoManager's <see cref="M:Northwoods.Go.GoUndoManager.StartTransaction" /> method.
		/// </summary>
		/// <returns>the value of <see cref="M:Northwoods.Go.GoUndoManager.StartTransaction" /></returns>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.StartTransaction" />,
		/// will raise a Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.StartedTransaction" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.FinishTransaction(System.String)" />
		public virtual bool StartTransaction()
		{
			return UndoManager?.StartTransaction() ?? false;
		}

		/// <summary>
		/// Call the UndoManager's <see cref="M:Northwoods.Go.GoUndoManager.FinishTransaction(System.String)" /> method.
		/// </summary>
		/// <param name="tname">a String describing the transaction</param>
		/// <returns><see cref="M:Northwoods.Go.GoUndoManager.FinishTransaction(System.String)" /></returns>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="M:Northwoods.Go.GoUndoManager.FinishTransaction(System.String)" />,
		/// will raise a Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedTransaction" />,
		/// and with a <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// that is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />
		/// that was the value of <see cref="T:Northwoods.Go.GoUndoManager" />.<see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> before calling
		/// <see cref="M:Northwoods.Go.GoUndoManager.FinishTransaction(System.String)" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.StartTransaction" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.AbortTransaction" />
		public virtual bool FinishTransaction(string tname)
		{
			return UndoManager?.FinishTransaction(tname) ?? false;
		}

		/// <summary>
		/// Call the UndoManager's <see cref="M:Northwoods.Go.GoUndoManager.AbortTransaction" /> method.
		/// </summary>
		/// <returns><see cref="M:Northwoods.Go.GoUndoManager.AbortTransaction" /></returns>
		/// <remarks>
		/// After calling <see cref="M:Northwoods.Go.GoUndoManager.AbortTransaction" />, if that call returned true,
		/// this raises a Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.AbortedTransaction" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoDocument.FinishTransaction(System.String)" />
		public virtual bool AbortTransaction()
		{
			return UndoManager?.AbortTransaction() ?? false;
		}

		private List<KeyValuePair<GoObject, object>> InternalCopy()
		{
			List<KeyValuePair<GoObject, object>> list = new List<KeyValuePair<GoObject, object>>();
			using (GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = GetEnumerator())
			{
				while (goLayerCollectionObjectEnumerator.MoveNext())
				{
					GoObject current = goLayerCollectionObjectEnumerator.Current;
					if (!(current is GoLink) && !(current is GoLabeledLink))
					{
						RectangleF bounds = current.Bounds;
						list.Add(new KeyValuePair<GoObject, object>(current, bounds));
					}
				}
			}
			using (GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = GetEnumerator())
			{
				while (goLayerCollectionObjectEnumerator.MoveNext())
				{
					GoObject current2 = goLayerCollectionObjectEnumerator.Current;
					GoLink goLink = current2 as GoLink;
					if (goLink != null)
					{
						PointF[] value = goLink.CopyPointsArray();
						list.Add(new KeyValuePair<GoObject, object>(current2, value));
					}
					else
					{
						GoLabeledLink goLabeledLink = current2 as GoLabeledLink;
						if (goLabeledLink != null)
						{
							PointF[] value2 = goLabeledLink.RealLink.CopyPointsArray();
							list.Add(new KeyValuePair<GoObject, object>(current2, value2));
						}
					}
				}
				return list;
			}
		}

		private void InternalArrange(List<KeyValuePair<GoObject, object>> copy)
		{
			if (copy != null)
			{
				bool suspendsRouting = SuspendsRouting;
				try
				{
					if (!suspendsRouting)
					{
						DelayedRoutings.Clear();
					}
					SuspendsRouting = true;
					for (int i = 0; i < copy.Count; i = checked(i + 1))
					{
						GoObject key = copy[i].Key;
						GoLink goLink = key as GoLink;
						if (goLink != null)
						{
							PointF[] points = (PointF[])copy[i].Value;
							goLink.SetPoints(points);
						}
						else
						{
							GoLabeledLink goLabeledLink = key as GoLabeledLink;
							if (goLabeledLink != null)
							{
								PointF[] points2 = (PointF[])copy[i].Value;
								goLabeledLink.RealLink.SetPoints(points2);
							}
							else
							{
								RectangleF rectangleF2 = key.Bounds = (RectangleF)copy[i].Value;
								(key as IGoRoutable)?.UpdateRoute();
							}
						}
					}
				}
				finally
				{
					SuspendsRouting = suspendsRouting;
					if (!suspendsRouting)
					{
						DoDelayedRouting(null);
					}
				}
			}
		}

		/// <summary>
		/// This is called during the construction of a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> in
		/// order to record the older/previous value for a document change.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// This method needs to be overridden for a particular kind of changed event only
		/// when the previous value state is not held in arguments to the call to
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />, or when those arguments are actually references to
		/// objects containing the real previous state that might be overwritten or lost by
		/// subsequent changes.
		/// </remarks>
		public virtual void CopyOldValueForUndo(GoChangedEventArgs e)
		{
			switch (e.Hint)
			{
			case 901:
				e.GoObject.CopyOldValueForUndo(e);
				break;
			case 220:
				if (e.IsBeforeChanging)
				{
					List<KeyValuePair<GoObject, object>> list2 = (List<KeyValuePair<GoObject, object>>)(e.OldValue = InternalCopy());
				}
				break;
			}
		}

		/// <summary>
		/// This is called during the construction of a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> in
		/// order to record the newer/next value for a document change.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// This method needs to be overridden for a particular kind of changed event only
		/// when the next value state is not held in arguments to the call to
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />, or when those arguments are actually references to
		/// objects containing the real newer state that might be overwritten or lost by
		/// subsequent changes.
		/// </remarks>
		public virtual void CopyNewValueForRedo(GoChangedEventArgs e)
		{
			switch (e.Hint)
			{
			case 901:
				e.GoObject.CopyNewValueForRedo(e);
				break;
			case 220:
				if (!e.IsBeforeChanging)
				{
					List<KeyValuePair<GoObject, object>> list2 = (List<KeyValuePair<GoObject, object>>)(e.NewValue = InternalCopy());
				}
				break;
			}
		}

		/// <summary>
		/// This method is called by <see cref="T:Northwoods.Go.GoChangedEventArgs" /> in order to perform
		/// the Undo or Redo or a particular document change.
		/// </summary>
		/// <param name="e">this value's <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> property
		/// identifies the kind of document change</param>
		/// <param name="undo">true if this method should restore the older/previous state
		/// from before the change event; false if this method should restore the newer/next
		/// state from after the change event</param>
		/// <remarks>
		/// <para>
		/// This handles changes to the document, such as <see cref="F:Northwoods.Go.GoDocument.ChangedAllowMove" />;
		/// to the collection of document layers, such as <see cref="F:Northwoods.Go.GoLayerCollection.InsertedLayer" />;
		/// and to any document layer, such as <see cref="F:Northwoods.Go.GoLayer.InsertedObject" />.
		/// For a <see cref="F:Northwoods.Go.GoLayer.ChangedObject" /> event hint, this just calls
		/// <see cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> on the <see cref="P:Northwoods.Go.GoChangedEventArgs.GoObject" />,
		/// to handle all the changes for objects such as <see cref="F:Northwoods.Go.GoObject.ChangedMovable" />.
		/// This method will raise an <c>ArgumentException</c> if the argument
		/// <paramref name="e" />'s <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> value is not recognized.
		/// </para>
		/// <para>
		/// If you add a property to a class inheriting from <see cref="T:Northwoods.Go.GoDocument" />,
		/// you may want to override this method in order to handle undo/redo.
		/// Be sure to call the base method for all <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" />
		/// values that your override does not handle.
		/// </para>
		/// <para>
		/// Although properties should be designed so that setting one property
		/// does not modify other properties, this is sometimes not practical.
		/// Nevertheless it is important to avoid having side-effects when
		/// the value is changing due to an undo or redo.
		/// One way of doing this is to copy the needed code, but not the
		/// auxiliary side-effecting code, from the property setter to the
		/// <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> override.  Or similarly, you could define
		/// a method called from both the setter and the ChangeValue method,
		/// parameterized by whether the caller is the setter or not.
		/// </para>
		/// <para>
		/// But a more convenient way to achieve this is to check the
		/// <see cref="P:Northwoods.Go.GoDocument.Initializing" /> property that is set to true when the
		/// <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" /> method is being called on this document.
		/// You should check this property before making "unrelated" side-effects.
		/// </para>
		/// </remarks>
		/// <example>
		/// Typical usage might be something like:
		/// <code>
		///   public bool MaintainsPartID {
		///     get { return myMaintainsPartID; }
		///     set {
		///       // only set the value and do other things if the value has changed
		///       bool old = myMaintainsPartID;
		///       if (old != value) {
		///         myMaintainsPartID = value;
		///         // notify about the change
		///         RaiseChanged(ChangedMaintainsPartID, 0, null, 0, old, NullRect, 0, value, NullRect);
		///         // when set to true, and when not undoing/redoing, make sure all parts have unique IDs
		///         if (!this.Initializing) {
		///           if (value)
		///             EnsureUniquePartID();
		///           else
		///             ClearPartIDTable();
		///         }
		///       }
		///     }
		///   }
		///
		///   public override void ChangeValue(GoChangedEventArgs e, bool undo) {
		///     switch (e.SubHint) {
		///       case ChangedMaintainsPartID:
		///         this.MaintainsPartID = (bool)e.GetValue(undo);
		///         return;
		///       default:
		///         base.ChangeValue(e, undo);
		///         return;
		///     }
		///   }
		/// </code>
		/// </example>
		/// <seealso cref="M:Northwoods.Go.GoObject.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />
		public virtual void ChangeValue(GoChangedEventArgs e, bool undo)
		{
			switch (e.Hint)
			{
			case 901:
			{
				GoObject goObject2 = e.GoObject;
				bool initializing = goObject2.Initializing;
				goObject2.Initializing = true;
				goObject2.ChangeValue(e, undo);
				goObject2.Initializing = initializing;
				break;
			}
			case 902:
			{
				GoLayer goLayer7 = (GoLayer)e.NewValue;
				GoObject goObject5 = e.GoObject;
				if (undo)
				{
					goLayer7.removeFromLayer(goObject5, undoing: true);
				}
				else
				{
					goLayer7.addToLayer(goObject5, undoing: true);
				}
				break;
			}
			case 903:
			{
				GoLayer goLayer4 = (GoLayer)e.OldValue;
				GoObject goObject3 = e.GoObject;
				if (undo)
				{
					goLayer4.addToLayer(goObject3, undoing: true);
					goLayer4.moveInLayerInternal(e.OldInt, goObject3, checked(goLayer4.Count - 1));
				}
				else
				{
					goLayer4.removeFromLayer(goObject3, undoing: true);
				}
				break;
			}
			case 904:
			{
				GoObject goObject4 = e.GoObject;
				GoLayer goLayer5 = (GoLayer)e.OldValue;
				GoLayer goLayer6 = (GoLayer)e.NewValue;
				if (undo)
				{
					goLayer5.changeLayer(goObject4, goLayer6, undoing: true);
				}
				else
				{
					goLayer6.changeLayer(goObject4, goLayer5, undoing: true);
				}
				break;
			}
			case 905:
			{
				GoObject goObject = (GoObject)e.OldValue;
				GoLayer layer = goObject.Layer;
				int oldInt2 = e.OldInt;
				int newInt2 = e.NewInt;
				if (undo)
				{
					layer.moveInLayer(oldInt2, goObject, newInt2, undoing: true);
				}
				else
				{
					layer.moveInLayer(newInt2, goObject, oldInt2, undoing: true);
				}
				break;
			}
			case 202:
				Size = e.GetSize(undo);
				break;
			case 204:
				FixedSize = (bool)e.GetValue(undo);
				break;
			case 203:
				TopLeft = e.GetPoint(undo);
				break;
			case 201:
				Name = (string)e.GetValue(undo);
				break;
			case 205:
				PaperColor = (Color)e.GetValue(undo);
				break;
			case 206:
				DataFormat = (string)e.GetValue(undo);
				break;
			case 220:
			{
				List<KeyValuePair<GoObject, object>> copy = (List<KeyValuePair<GoObject, object>>)e.GetValue(undo);
				InternalArrange(copy);
				InvalidateViews();
				break;
			}
			case 221:
				UserFlags = e.GetInt(undo);
				break;
			case 222:
				UserObject = e.GetValue(undo);
				break;
			case 223:
				LinksLayer = (GoLayer)e.GetValue(undo);
				break;
			case 207:
				AllowSelect = (bool)e.GetValue(undo);
				break;
			case 208:
				AllowMove = (bool)e.GetValue(undo);
				break;
			case 209:
				AllowCopy = (bool)e.GetValue(undo);
				break;
			case 210:
				AllowResize = (bool)e.GetValue(undo);
				break;
			case 211:
				AllowReshape = (bool)e.GetValue(undo);
				break;
			case 212:
				AllowDelete = (bool)e.GetValue(undo);
				break;
			case 213:
				AllowInsert = (bool)e.GetValue(undo);
				break;
			case 214:
				AllowLink = (bool)e.GetValue(undo);
				break;
			case 215:
				AllowEdit = (bool)e.GetValue(undo);
				break;
			case 910:
				((GoLayer)e.Object).AllowView = (bool)e.GetValue(undo);
				break;
			case 911:
				((GoLayer)e.Object).AllowSelect = (bool)e.GetValue(undo);
				break;
			case 912:
				((GoLayer)e.Object).AllowMove = (bool)e.GetValue(undo);
				break;
			case 913:
				((GoLayer)e.Object).AllowCopy = (bool)e.GetValue(undo);
				break;
			case 914:
				((GoLayer)e.Object).AllowResize = (bool)e.GetValue(undo);
				break;
			case 915:
				((GoLayer)e.Object).AllowReshape = (bool)e.GetValue(undo);
				break;
			case 916:
				((GoLayer)e.Object).AllowDelete = (bool)e.GetValue(undo);
				break;
			case 917:
				((GoLayer)e.Object).AllowInsert = (bool)e.GetValue(undo);
				break;
			case 918:
				((GoLayer)e.Object).AllowLink = (bool)e.GetValue(undo);
				break;
			case 919:
				((GoLayer)e.Object).AllowEdit = (bool)e.GetValue(undo);
				break;
			case 930:
				((GoLayer)e.Object).Identifier = e.GetValue(undo);
				break;
			case 801:
			{
				GoLayer goLayer3 = (GoLayer)e.Object;
				if (undo)
				{
					Layers.Remove(goLayer3);
					break;
				}
				GoLayer dest = (GoLayer)e.OldValue;
				if (e.SubHint == 1)
				{
					Layers.InsertAfter(dest, goLayer3);
				}
				else
				{
					Layers.InsertBefore(dest, goLayer3);
				}
				break;
			}
			case 802:
			{
				GoLayer goLayer = (GoLayer)e.Object;
				if (undo)
				{
					GoLayer goLayer2 = (GoLayer)e.OldValue;
					if (goLayer2 == null)
					{
						Layers.InsertAfter(Layers.Top, goLayer);
					}
					else
					{
						Layers.InsertBefore(goLayer2, goLayer);
					}
				}
				else
				{
					Layers.Remove(goLayer);
				}
				break;
			}
			case 803:
			{
				GoLayer moving = (GoLayer)e.OldValue;
				int oldInt = e.OldInt;
				int newInt = e.NewInt;
				if (undo)
				{
					Layers.moveInCollection(oldInt, moving, newInt, undoing: true);
				}
				else
				{
					Layers.moveInCollection(newInt, moving, oldInt, undoing: true);
				}
				break;
			}
			case 804:
			{
				GoLayer @default = (GoLayer)e.GetValue(undo);
				Layers.Default = @default;
				break;
			}
			case 224:
				MaintainsPartID = (bool)e.GetValue(undo);
				break;
			case 226:
				LastPartID = e.GetInt(undo);
				break;
			case 225:
				ValidCycle = (GoDocumentValidCycle)e.GetInt(undo);
				break;
			case 227:
				WorldScale = e.GetSize(undo);
				break;
			case 228:
				RoutingTime = (GoRoutingTime)e.GetInt(undo);
				break;
			case 241:
				Initializing = (bool)e.GetValue(undo);
				break;
			default:
				if (e.Hint >= 10000)
				{
					throw new ArgumentException("Unknown GoChangedEventArgs hint--override GoDocument.ChangeValue to handle the Hint: " + e.Hint.ToString(NumberFormatInfo.InvariantInfo));
				}
				break;
			}
		}

		/// <summary>
		/// This method returns true if adding a link from <paramref name="a" /> to <paramref name="b" />
		/// would result in a cycle of directed links going through the node <paramref name="a" />.
		/// </summary>
		/// <param name="a">the node to start from</param>
		/// <param name="b">the node that the proposed link would connect to</param>
		/// <returns></returns>
		/// <remarks>
		/// This method ignores any reflexive links--i.e. links whose ports are both part of the same node.
		/// This assumes that there are no directed cycles already present in the graph.
		/// If there are any such cycles, this recursive method may cause stack overflows or
		/// infinite recursion.
		/// However, this method is faster than <see cref="M:Northwoods.Go.GoDocument.MakesDirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />.
		/// Set <see cref="P:Northwoods.Go.GoDocument.ValidCycle" /> to <see cref="F:Northwoods.Go.GoDocumentValidCycle.NotDirectedFast" />
		/// when you can be sure there are never any cycles in the graph; otherwise set it
		/// to <see cref="F:Northwoods.Go.GoDocumentValidCycle.NotDirected" />, which is slower but can handle
		/// existing cycles in the graph.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.ValidCycle" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesDirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesUndirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />
		public static bool MakesDirectedCycleFast(IGoNode a, IGoNode b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null)
			{
				return false;
			}
			if (b == null)
			{
				return false;
			}
			foreach (IGoNode destination in b.Destinations)
			{
				if (destination != b && MakesDirectedCycleFast(a, destination))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This method returns true if adding a link from <paramref name="a" /> to <paramref name="b" />
		/// would result in a cycle of directed links going through the node <paramref name="a" />.
		/// </summary>
		/// <param name="a">the node to start from</param>
		/// <param name="b">the node that the proposed link would connect to</param>
		/// <returns></returns>
		/// <remarks>
		/// This method ignores any reflexive links--i.e. links whose ports are both part of the same node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.ValidCycle" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesDirectedCycleFast(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesUndirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />
		public static bool MakesDirectedCycle(IGoNode a, IGoNode b)
		{
			if (a == b)
			{
				return true;
			}
			lock (myCycleMap)
			{
				myCycleMap.Clear();
				myCycleMap.Add(a, value: true);
				bool result = MakesDirectedCycle1(a, b, myCycleMap);
				myCycleMap.Clear();
				return result;
			}
		}

		private static bool MakesDirectedCycle1(IGoNode a, IGoNode b, Dictionary<IGoNode, bool> map)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null)
			{
				return false;
			}
			if (b == null)
			{
				return false;
			}
			if (map.TryGetValue(b, out bool _))
			{
				return false;
			}
			map.Add(b, value: true);
			foreach (IGoNode destination in b.Destinations)
			{
				if (destination != b && MakesDirectedCycle1(a, destination, map))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This method returns true if adding a link from <paramref name="a" /> to <paramref name="b" />
		/// would result in a cycle or loop of links, regardless of link direction,
		/// going through the node <paramref name="a" />.
		/// </summary>
		/// <param name="a">the node to start from</param>
		/// <param name="b">the node that the proposed link would connect to</param>
		/// <returns></returns>
		/// <remarks>
		/// This method ignores any reflexive links--i.e. links whose ports are both part of the same node.
		/// </remarks>
		/// <seealso cref="P:Northwoods.Go.GoDocument.ValidCycle" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesDirectedCycle(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoDocument.MakesDirectedCycleFast(Northwoods.Go.IGoNode,Northwoods.Go.IGoNode)" />
		/// <seealso cref="M:Northwoods.Go.GoPort.IsValidLink(Northwoods.Go.IGoPort)" />
		public static bool MakesUndirectedCycle(IGoNode a, IGoNode b)
		{
			if (a == b)
			{
				return true;
			}
			lock (myCycleMap)
			{
				myCycleMap.Clear();
				myCycleMap.Add(a, value: true);
				bool result = MakesUndirectedCycle1(a, b, myCycleMap);
				myCycleMap.Clear();
				return result;
			}
		}

		private static bool MakesUndirectedCycle1(IGoNode a, IGoNode b, Dictionary<IGoNode, bool> map)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null)
			{
				return false;
			}
			if (b == null)
			{
				return false;
			}
			if (map.TryGetValue(b, out bool _))
			{
				return false;
			}
			map.Add(b, value: true);
			foreach (IGoNode node in b.Nodes)
			{
				if (node != b && MakesUndirectedCycle1(a, node, map))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// If <see cref="P:Northwoods.Go.GoDocument.SuspendsRouting" /> is true add the given <see cref="T:Northwoods.Go.IGoRoutable" />
		/// <see cref="T:Northwoods.Go.GoObject" /> to the <see cref="P:Northwoods.Go.GoDocument.DelayedRoutings" /> collection;
		/// otherwise just call <see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" /> immediately.
		/// </summary>
		/// <param name="obj"></param>
		public virtual void UpdateRoute(IGoRoutable obj)
		{
			if (SuspendsRouting && (RoutingTime & GoRoutingTime.Delayed) != 0)
			{
				DelayedRoutings.Add(obj);
			}
			else
			{
				obj.CalculateRoute();
			}
		}

		internal void ResumeRouting(bool old, IGoCollection moved)
		{
			if (SuspendsRouting && !old)
			{
				bool flag = false;
				if (moved != null)
				{
					foreach (GoObject item in moved)
					{
						if (item.Document == this)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag || moved == null)
				{
					DoDelayedRouting(moved);
				}
			}
			SuspendsRouting = old;
		}

		/// <summary>
		/// Call <see cref="T:Northwoods.Go.IGoRoutable" />.<see cref="M:Northwoods.Go.IGoRoutable.CalculateRoute" />
		/// on each of the objects in the <see cref="P:Northwoods.Go.GoDocument.DelayedRoutings" /> collection,
		/// and clear that collection.
		/// </summary>
		/// <param name="moved">an <see cref="T:Northwoods.Go.IGoCollection" />, or null</param>
		/// <remarks>
		/// This method is called by <see cref="M:Northwoods.Go.GoView.MoveSelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />, <see cref="M:Northwoods.Go.GoView.CopySelection(Northwoods.Go.GoSelection,System.Drawing.SizeF,System.Boolean)" />,
		/// <see cref="M:Northwoods.Go.GoView.EditPaste" /> and (in full Windows Forms) <c>GoView.DoExternalDrop</c>.
		/// </remarks>
		public virtual void DoDelayedRouting(IGoCollection moved)
		{
			if (moved != null && !moved.IsEmpty)
			{
				if ((RoutingTime & GoRoutingTime.AfterNodesDragged) == GoRoutingTime.AfterNodesDragged)
				{
					GoCollection goCollection = null;
					foreach (GoObject item in moved)
					{
						if (item.Document == this && IsAvoidable(item))
						{
							if (goCollection == null)
							{
								goCollection = new GoCollection();
							}
							else
							{
								goCollection.Clear();
							}
							RectangleF a = item.Bounds;
							GoObject.InflateRect(ref a, 1f, 1f);
							PickObjectsInRectangle(a, GoPickInRectangleStyle.AnyIntersectsBounds, goCollection, 999999);
							foreach (GoObject item2 in goCollection)
							{
								IGoRoutable goRoutable = item2 as IGoRoutable;
								if (goRoutable != null)
								{
									GoLink stroke = GetStroke(item2);
									if (stroke != null)
									{
										if (stroke.IntersectsRectangle(a))
										{
											DelayedRoutings.Add(goRoutable);
										}
									}
									else
									{
										DelayedRoutings.Add(goRoutable);
									}
								}
							}
						}
					}
				}
				if ((RoutingTime & GoRoutingTime.AfterLinksDragged) == GoRoutingTime.AfterLinksDragged)
				{
					foreach (GoObject item3 in moved)
					{
						IGoRoutable goRoutable2 = item3 as IGoRoutable;
						if (goRoutable2 != null)
						{
							DelayedRoutings.Add(goRoutable2);
						}
					}
				}
			}
			while (!DelayedRoutings.IsEmpty)
			{
				object[] array = DelayedRoutings.CopyArray();
				DelayedRoutings.Clear();
				object[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					(array2[i] as IGoRoutable)?.CalculateRoute();
				}
				if (myAvoidsOrthogonalLinks)
				{
					AvoidOrthogonalOverlaps(array);
				}
			}
		}

		private void AvoidOrthogonalOverlaps(object[] routables)
		{
			GoCollection goCollection = new GoCollection();
			for (int i = 0; i < routables.Length; i++)
			{
				GoObject obj = (GoObject)routables[i];
				goCollection.Add(obj);
			}
			RectangleF a = ComputeBounds(goCollection, null);
			SizeF sizeF = (myPositions != null) ? myPositions.CellSize : new SizeF(8f, 8f);
			GoObject.InflateRect(ref a, sizeF.Width, sizeF.Height);
			goCollection.Clear();
			PickObjectsInRectangle(a, GoPickInRectangleStyle.AnyIntersectsBounds, goCollection, 999999);
			GoCollection goCollection2 = new GoCollection();
			foreach (GoObject item in goCollection)
			{
				if (item is IGoRoutable)
				{
					GoLink stroke = GetStroke(item);
					if (stroke != null && GoObject.IntersectsRect(stroke.Bounds, a))
					{
						goCollection2.Add(stroke);
					}
				}
			}
			if (AdjustOverlapsH(goCollection2) > 0 || AdjustOverlapsV(goCollection2) > 0)
			{
				AdjustOverlapsH(goCollection2);
				AdjustOverlapsV(goCollection2);
			}
		}

		private static GoLink GetStroke(GoObject obj)
		{
			GoLink goLink = obj as GoLink;
			if (goLink != null)
			{
				return goLink;
			}
			return (obj as GoLabeledLink)?.RealLink;
		}

		private static bool IsApprox(float a, float b)
		{
			float num = a - b;
			if (num > -1f)
			{
				return num < 1f;
			}
			return false;
		}

		private int AdjustOverlapsH(IGoCollection routables)
		{
			float num = (myPositions != null) ? myPositions.CellSize.Height : 8f;
			int num2 = 0;
			List<SegInfo> list = new List<SegInfo>();
			checked
			{
				foreach (GoObject routable in routables)
				{
					GoLink goLink = (GoLink)routable;
					if (goLink != null && goLink.Orthogonal && goLink.Style != GoStrokeStyle.Bezier)
					{
						for (int i = 2; i < goLink.PointsCount - 3; i++)
						{
							PointF point = goLink.GetPoint(i);
							PointF point2 = goLink.GetPoint(i + 1);
							if (IsApprox(point.Y, point2.Y) && !IsApprox(point.X, point2.X))
							{
								SegInfo segInfo = new SegInfo();
								segInfo.Layer = (float)Math.Floor(point.Y / num);
								PointF point3 = goLink.GetPoint(0);
								PointF point4 = goLink.GetPoint(goLink.PointsCount - 1);
								segInfo.First = point3.X * point3.X + point3.Y;
								segInfo.Last = point4.X * point4.X + point4.Y;
								segInfo.ColumnMin = Math.Min(point.X, point2.X);
								segInfo.ColumnMax = Math.Max(point.X, point2.X);
								segInfo.Index = i;
								segInfo.Link = (routable as IGoLink);
								if (i + 2 < goLink.PointsCount)
								{
									PointF point5 = goLink.GetPoint(i - 1);
									PointF point6 = goLink.GetPoint(i + 2);
									int turns = 0;
									if (point5.Y < point.Y)
									{
										turns = ((point6.Y < point.Y) ? 3 : ((!(point.X < point2.X)) ? 1 : 2));
									}
									else if (point5.Y > point.Y)
									{
										turns = ((!(point6.Y > point.Y)) ? ((!(point2.X < point.X)) ? 1 : 2) : 0);
									}
									segInfo.Turns = turns;
								}
								list.Add(segInfo);
							}
						}
					}
				}
				if (list.Count > 1)
				{
					list.Sort(SegInfoComparer.Default);
					int num3 = 0;
					while (num3 < list.Count)
					{
						float layer = list[num3].Layer;
						int j;
						for (j = num3 + 1; j < list.Count && list[j].Layer == layer; j++)
						{
						}
						if (j - num3 > 1)
						{
							int num4 = num3;
							while (num4 < j)
							{
								float num5 = list[num4].ColumnMax;
								int k;
								for (k = num3 + 1; k < j && list[k].ColumnMin < num5; k++)
								{
									num5 = Math.Max(num5, list[k].ColumnMax);
								}
								int num6 = k - num4;
								if (num6 > 1)
								{
									list.Sort(num4, num6, SegInfoComparer2.DefaultLast);
									int num7 = 1;
									float last = list[num4].Last;
									for (int l = num4; l < k; l++)
									{
										SegInfo segInfo2 = list[l];
										if (segInfo2.Last != last)
										{
											num7++;
											last = segInfo2.Last;
										}
									}
									list.Sort(num4, num6, SegInfoComparer2.DefaultFirst);
									int num8 = 1;
									last = list[num4].First;
									for (int m = num4; m < k; m++)
									{
										SegInfo segInfo3 = list[m];
										if (segInfo3.First != last)
										{
											num8++;
											last = segInfo3.First;
										}
									}
									bool flag;
									int num9;
									if (num7 < num8)
									{
										flag = false;
										num9 = num7;
										last = list[num4].Last;
										list.Sort(num4, num6, SegInfoComparer2.DefaultLast);
									}
									else
									{
										flag = true;
										num9 = num8;
										last = list[num4].First;
									}
									float num10 = layer * num + num / 2f;
									int num11 = 0;
									for (int n = num4; n < k; n++)
									{
										SegInfo segInfo4 = list[n];
										if ((flag ? segInfo4.First : segInfo4.Last) != last)
										{
											num11++;
											last = (flag ? segInfo4.First : segInfo4.Last);
										}
										GoLink stroke = GetStroke(segInfo4.Link.GoObject);
										PointF point7 = stroke.GetPoint(segInfo4.Index);
										PointF point8 = stroke.GetPoint(segInfo4.Index + 1);
										float num12 = myLinkSpacing * ((float)num11 - (float)(num9 - 1) / 2f);
										if (!stroke.AvoidsNodes || IsUnoccupied2(point7.X, num10 + num12, point8.X, num10 + num12))
										{
											num2++;
											stroke.SetPoint(segInfo4.Index, new PointF(point7.X, num10 + num12));
											stroke.SetPoint(segInfo4.Index + 1, new PointF(point8.X, num10 + num12));
										}
									}
								}
								num4 = k;
							}
						}
						num3 = j;
					}
				}
				return num2;
			}
		}

		private int AdjustOverlapsV(IGoCollection routables)
		{
			float num = (myPositions != null) ? myPositions.CellSize.Width : 8f;
			int num2 = 0;
			List<SegInfo> list = new List<SegInfo>();
			checked
			{
				foreach (GoObject routable in routables)
				{
					GoLink goLink = (GoLink)routable;
					if (goLink != null && goLink.Orthogonal && goLink.Style != GoStrokeStyle.Bezier)
					{
						for (int i = 2; i < goLink.PointsCount - 3; i++)
						{
							PointF point = goLink.GetPoint(i);
							PointF point2 = goLink.GetPoint(i + 1);
							if (IsApprox(point.X, point2.X) && !IsApprox(point.Y, point2.Y))
							{
								SegInfo segInfo = new SegInfo();
								segInfo.Layer = (float)Math.Floor(point.X / num);
								PointF point3 = goLink.GetPoint(0);
								PointF point4 = goLink.GetPoint(goLink.PointsCount - 1);
								segInfo.First = point3.X + point3.Y * point3.Y;
								segInfo.Last = point4.X + point4.Y * point4.Y;
								segInfo.ColumnMin = Math.Min(point.Y, point2.Y);
								segInfo.ColumnMax = Math.Max(point.Y, point2.Y);
								segInfo.Index = i;
								segInfo.Link = (routable as IGoLink);
								if (i + 2 < goLink.PointsCount)
								{
									PointF point5 = goLink.GetPoint(i - 1);
									PointF point6 = goLink.GetPoint(i + 2);
									int turns = 0;
									if (point5.X < point.X)
									{
										turns = ((point6.X < point.X) ? 3 : ((!(point.Y < point2.Y)) ? 1 : 2));
									}
									else if (point5.X > point.X)
									{
										turns = ((!(point6.X > point.X)) ? ((!(point2.Y < point.Y)) ? 1 : 2) : 0);
									}
									segInfo.Turns = turns;
								}
								list.Add(segInfo);
							}
						}
					}
				}
				if (list.Count > 1)
				{
					list.Sort(SegInfoComparer.Default);
					int num3 = 0;
					while (num3 < list.Count)
					{
						float layer = list[num3].Layer;
						int j;
						for (j = num3 + 1; j < list.Count && list[j].Layer == layer; j++)
						{
						}
						if (j - num3 > 1)
						{
							int num4 = num3;
							while (num4 < j)
							{
								float num5 = list[num4].ColumnMax;
								int k;
								for (k = num3 + 1; k < j && list[k].ColumnMin < num5; k++)
								{
									num5 = Math.Max(num5, list[k].ColumnMax);
								}
								int num6 = k - num4;
								if (num6 > 1)
								{
									list.Sort(num4, num6, SegInfoComparer2.DefaultLast);
									int num7 = 1;
									float last = list[num4].Last;
									for (int l = num4; l < k; l++)
									{
										SegInfo segInfo2 = list[l];
										if (segInfo2.Last != last)
										{
											num7++;
											last = segInfo2.Last;
										}
									}
									list.Sort(num4, num6, SegInfoComparer2.DefaultFirst);
									int num8 = 1;
									last = list[num4].First;
									for (int m = num4; m < k; m++)
									{
										SegInfo segInfo3 = list[m];
										if (segInfo3.First != last)
										{
											num8++;
											last = segInfo3.First;
										}
									}
									bool flag;
									int num9;
									if (num7 < num8)
									{
										flag = false;
										num9 = num7;
										last = list[num4].Last;
										list.Sort(num4, num6, SegInfoComparer2.DefaultLast);
									}
									else
									{
										flag = true;
										num9 = num8;
										last = list[num4].First;
									}
									float num10 = layer * num + num / 2f;
									int num11 = 0;
									for (int n = num4; n < k; n++)
									{
										SegInfo segInfo4 = list[n];
										if ((flag ? segInfo4.First : segInfo4.Last) != last)
										{
											num11++;
											last = (flag ? segInfo4.First : segInfo4.Last);
										}
										GoLink stroke = GetStroke(segInfo4.Link.GoObject);
										PointF point7 = stroke.GetPoint(segInfo4.Index);
										PointF point8 = stroke.GetPoint(segInfo4.Index + 1);
										float num12 = myLinkSpacing * ((float)num11 - (float)(num9 - 1) / 2f);
										if (!stroke.AvoidsNodes || IsUnoccupied2(num10 + num12, point7.Y, num10 + num12, point8.Y))
										{
											num2++;
											stroke.SetPoint(segInfo4.Index, new PointF(num10 + num12, point7.Y));
											stroke.SetPoint(segInfo4.Index + 1, new PointF(num10 + num12, point8.Y));
										}
									}
								}
								num4 = k;
							}
						}
						num3 = j;
					}
				}
				return num2;
			}
		}

		private bool IsUnoccupied2(float px, float py, float qx, float qy)
		{
			float num = Math.Min(px, qx);
			float num2 = Math.Min(py, qy);
			float num3 = Math.Max(px, qx);
			float num4 = Math.Max(py, qy);
			return IsUnoccupied(new RectangleF(num, num2, num3 - num, num4 - num2), null);
		}

		/// <summary>
		/// This predicate determines whether the given object is considered when
		/// trying to route links whose <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" /> property
		/// is true.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>
		/// By default this returns true when the <paramref name="obj" /> is an <see cref="T:Northwoods.Go.IGoNode" />.
		/// </returns>
		/// <remarks>
		/// This is called on all top-level objects in the document.
		/// If the top-level object is a <see cref="T:Northwoods.Go.GoSubGraphBase" />,
		/// it is called on its non-subgraph children.
		/// </remarks>
		public virtual bool IsAvoidable(GoObject obj)
		{
			return obj is IGoNode;
		}

		/// <summary>
		/// Return the effective bounds of an object that should be avoided when
		/// routing links whose <see cref="P:Northwoods.Go.GoLink.AvoidsNodes" /> property is true.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>
		/// By default this returns the bounds of the <paramref name="obj" />,
		/// expanded by <see cref="M:Northwoods.Go.GoObject.ExpandPaintBounds(System.Drawing.RectangleF,Northwoods.Go.GoView)" />.
		/// </returns>
		/// <remarks>
		/// This is called for top-level non-<see cref="T:Northwoods.Go.GoSubGraphBase" /> objects and
		/// <see cref="T:Northwoods.Go.GoSubGraphBase" /> children for which <see cref="M:Northwoods.Go.GoDocument.IsAvoidable(Northwoods.Go.GoObject)" /> returned true.
		/// </remarks>
		public virtual RectangleF GetAvoidableRectangle(GoObject obj)
		{
			RectangleF bounds = obj.Bounds;
			obj.ExpandPaintBounds(bounds, null);
			return bounds;
		}

		/// <summary>
		/// Returns true if there are any "avoidable" objects within or
		/// intersecting the given rectangular region.
		/// </summary>
		/// <param name="r">a RectangleF in document coordinates</param>
		/// <param name="skip">
		/// an object that should be ignored when checking for collisions,
		/// usually the object in the document that you are considering moving;
		/// may be <c>null</c> to consider all avoidable objects
		/// </param>
		/// <returns></returns>
		/// <remarks>
		/// This only considers document objects for which <see cref="M:Northwoods.Go.GoDocument.IsAvoidable(Northwoods.Go.GoObject)" />
		/// returns true, with the "occupied" space determined by <see cref="M:Northwoods.Go.GoDocument.GetAvoidableRectangle(Northwoods.Go.GoObject)" />.
		/// </remarks>
		public bool IsUnoccupied(RectangleF r, GoObject skip)
		{
			if (skip != mySkippedAvoidable)
			{
				InvalidatePositionArray(null);
				mySkippedAvoidable = skip;
			}
			return GetPositions(clearunoccupied: false, skip).IsUnoccupied(r.X, r.Y, r.Width, r.Height);
		}

		internal GoPositionArray GetPositions()
		{
			return GetPositions(clearunoccupied: true, null);
		}

		internal GoPositionArray GetPositions(bool clearunoccupied, GoObject skip)
		{
			if (myPositions == null)
			{
				myPositions = new GoPositionArray();
			}
			if (myPositions.Invalid)
			{
				RectangleF a = ComputeBounds();
				GoObject.InflateRect(ref a, 200f * WorldEpsilon, 200f * WorldEpsilon);
				myPositions.Initialize(a);
				using (GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = GetEnumerator())
				{
					while (goLayerCollectionObjectEnumerator.MoveNext())
					{
						GoObject current = goLayerCollectionObjectEnumerator.Current;
						GetPositions1(current, skip);
					}
				}
				myPositions.Invalid = false;
			}
			else if (clearunoccupied)
			{
				myPositions.ClearAllUnoccupied();
			}
			return myPositions;
		}

		private void GetPositions1(GoObject obj, GoObject skip)
		{
			if (obj == skip)
			{
				return;
			}
			GoSubGraphBase goSubGraphBase = obj as GoSubGraphBase;
			if (goSubGraphBase != null)
			{
				foreach (GoObject item in goSubGraphBase)
				{
					GetPositions1(item, skip);
				}
			}
			else
			{
				if (!IsAvoidable(obj))
				{
					return;
				}
				RectangleF avoidableRectangle = GetAvoidableRectangle(obj);
				float width = myPositions.CellSize.Width;
				float height = myPositions.CellSize.Height;
				float num = avoidableRectangle.X + avoidableRectangle.Width;
				float num2 = avoidableRectangle.Y + avoidableRectangle.Height;
				for (float num3 = avoidableRectangle.X; num3 < num; num3 += width)
				{
					for (float num4 = avoidableRectangle.Y; num4 < num2; num4 += height)
					{
						myPositions.SetOccupied(num3, num4);
					}
					myPositions.SetOccupied(num3, num2);
				}
				for (float num5 = avoidableRectangle.Y; num5 < num2; num5 += height)
				{
					myPositions.SetOccupied(num, num5);
				}
				myPositions.SetOccupied(num, num2);
			}
		}

		private void InvalidatePositionArray(GoObject obj)
		{
			mySkippedAvoidable = null;
			if (myPositions != null && !myPositions.Invalid && (obj == null || IsAvoidable(obj)))
			{
				myPositions.Invalid = true;
			}
		}

		/// <summary>
		/// Returns an <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> in this document with the given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>null if no such part is known</returns>
		/// <remarks>
		/// If <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is false, the value may not be correct.
		/// If <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" /> is true, you must not set the <see cref="T:Northwoods.Go.IGoIdentifiablePart" />
		/// <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> of any object while it is part of a document,
		/// or else this method may produce incorrect results.
		/// </remarks>
		public IGoIdentifiablePart FindPart(int id)
		{
			if (myParts != null)
			{
				if (!myParts.TryGetValue(id, out IGoIdentifiablePart value))
				{
					return null;
				}
				return value;
			}
			return null;
		}

		internal void AddPart(IGoIdentifiablePart p)
		{
			if (myParts == null)
			{
				myParts = new Dictionary<int, IGoIdentifiablePart>(1000);
			}
			int partID = p.PartID;
			checked
			{
				IGoIdentifiablePart value;
				if (partID == -1)
				{
					int num = ++myLastPartID;
					while (myParts.ContainsKey(num))
					{
						num = ++myLastPartID;
					}
					myParts[num] = p;
					p.PartID = num;
				}
				else if (!myParts.TryGetValue(partID, out value))
				{
					myParts[partID] = p;
				}
				else if (value != null && value.PartID != partID)
				{
					myParts[partID] = p;
					value.PartID = -1;
					AddPart(value);
				}
			}
		}

		internal void RemovePart(IGoIdentifiablePart p)
		{
			if (myParts != null)
			{
				myParts.Remove(p.PartID);
			}
		}

		/// <summary>
		/// Make sure every <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> in this
		/// document has a unique <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" />.
		/// </summary>
		/// <remarks>
		/// Although this is called automatically when setting <see cref="P:Northwoods.Go.GoDocument.MaintainsPartID" />
		/// to true, you may need to call this method explicitly after adding some objects
		/// to this document while <see cref="P:Northwoods.Go.GoDocument.SuspendsUpdates" /> is true.
		/// You may also want to call this method explicitly after loading a document
		/// from persistent storage if there is a possibility of any inconsistencies.
		/// This method will also make sure that the value of <see cref="P:Northwoods.Go.GoDocument.LastPartID" />
		/// is at least as large as any <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> of
		/// any <see cref="T:Northwoods.Go.IGoIdentifiablePart" /> in this document.
		/// If you set any <see cref="P:Northwoods.Go.IGoIdentifiablePart.PartID" /> to a non-negative integer
		/// before adding the object to the document, you are responsible for making
		/// sure that the value is in fact unique within the document.
		/// </remarks>
		public virtual void EnsureUniquePartID()
		{
			if (myParts == null)
			{
				myParts = new Dictionary<int, IGoIdentifiablePart>(1000);
			}
			List<KeyValuePair<int, IGoIdentifiablePart>> list = new List<KeyValuePair<int, IGoIdentifiablePart>>();
			foreach (KeyValuePair<int, IGoIdentifiablePart> myPart in myParts)
			{
				int key = myPart.Key;
				if (myPart.Value.PartID != key)
				{
					list.Add(myPart);
				}
			}
			foreach (KeyValuePair<int, IGoIdentifiablePart> item in list)
			{
				int key2 = item.Key;
				IGoIdentifiablePart value = item.Value;
				int partID = value.PartID;
				if (!myParts.ContainsKey(partID))
				{
					myParts.Remove(key2);
					myParts[partID] = value;
				}
				else
				{
					value.PartID = key2;
				}
			}
			int num = -1;
			using (GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = GetEnumerator())
			{
				while (goLayerCollectionObjectEnumerator.MoveNext())
				{
					GoObject current3 = goLayerCollectionObjectEnumerator.Current;
					num = Math.Max(num, MaxPartID(current3));
				}
			}
			myLastPartID = Math.Max(myLastPartID, num);
			using (GoLayerCollectionObjectEnumerator goLayerCollectionObjectEnumerator = GetEnumerator())
			{
				while (goLayerCollectionObjectEnumerator.MoveNext())
				{
					GoObject current4 = goLayerCollectionObjectEnumerator.Current;
					AddAllParts(current4);
				}
			}
		}

		internal int MaxPartID(GoObject obj)
		{
			int num = -1;
			IGoIdentifiablePart goIdentifiablePart = obj as IGoIdentifiablePart;
			if (goIdentifiablePart != null)
			{
				num = Math.Max(num, goIdentifiablePart.PartID);
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					num = Math.Max(num, MaxPartID(item));
				}
				return num;
			}
			return num;
		}

		internal void AddAllParts(GoObject obj)
		{
			IGoIdentifiablePart goIdentifiablePart = obj as IGoIdentifiablePart;
			if (goIdentifiablePart != null)
			{
				AddPart(goIdentifiablePart);
			}
			GoGroup goGroup = obj as GoGroup;
			if (goGroup != null)
			{
				foreach (GoObject item in goGroup.GetEnumerator())
				{
					AddAllParts(item);
				}
			}
		}

		internal void RemoveAllParts(GoObject obj)
		{
			if (myParts != null)
			{
				IGoIdentifiablePart goIdentifiablePart = obj as IGoIdentifiablePart;
				if (goIdentifiablePart != null)
				{
					RemovePart(goIdentifiablePart);
				}
				GoGroup goGroup = obj as GoGroup;
				if (goGroup != null)
				{
					foreach (GoObject item in goGroup.GetEnumerator())
					{
						RemoveAllParts(item);
					}
				}
			}
		}
	}
}
