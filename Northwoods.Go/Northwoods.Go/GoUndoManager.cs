using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Northwoods.Go
{
	/// <summary>
	/// The GoUndoManager class observes and records document changes and supports
	/// undo and redo operations.
	/// </summary>
	[Serializable]
	public class GoUndoManager
	{
		/// <summary>
		/// The unlocalized name for a drag copy operation, "Copy Selection".
		/// </summary>
		public const string CopySelectionName = "Copy Selection";

		/// <summary>
		/// The unlocalized name for a drag move operation, "Move Selection".
		/// </summary>
		public const string MoveSelectionName = "Move Selection";

		/// <summary>
		/// The unlocalized name for a delete operation, "Delete Selection".
		/// </summary>
		public const string DeleteSelectionName = "Delete Selection";

		/// <summary>
		/// The unlocalized name for a linking operation, "New Link".
		/// </summary>
		public const string NewLinkName = "New Link";

		/// <summary>
		/// The unlocalized name for a relinking operation, "Relink".
		/// </summary>
		public const string RelinkName = "Relink";

		/// <summary>
		/// The unlocalized name for a resize operation, "Resize".
		/// </summary>
		public const string ResizeName = "Resize";

		/// <summary>
		/// The unlocalized name for a clipboard copy operation, "Copy".
		/// </summary>
		public const string CopyName = "Copy";

		/// <summary>
		/// The unlocalized name for a clipboard cut operation, "Cut".
		/// </summary>
		public const string CutName = "Cut";

		/// <summary>
		/// The unlocalized name for a clipboard paste operation, "Paste".
		/// </summary>
		public const string PasteName = "Paste";

		/// <summary>
		/// The unlocalized name for a drag drop operation, "Drop".
		/// </summary>
		public const string DropName = "Drop";

		/// <summary>
		/// The unlocalized name for a text edit operation, "Text Edit".
		/// </summary>
		public const string TextEditName = "Text Edit";

		/// <summary>
		/// The unlocalized name for a SubGraph collapsing, "Collapsed SubGraph".
		/// </summary>
		public const string CollapsedSubGraphName = "Collapsed SubGraph";

		/// <summary>
		/// The unlocalized name for a SubGraph expansion, "Expanded SubGraph".
		/// </summary>
		public const string ExpandedSubGraphName = "Expanded SubGraph";

		/// <summary>
		/// The unlocalized name for a complete SubGraph expansion, "Expanded All SubGraphs".
		/// </summary>
		public const string ExpandedAllSubGraphsName = "Expanded All SubGraphs";

		/// <summary>
		/// The unlocalized name for an IGoCollapsible collapse, "Collapsed".
		/// </summary>
		public const string CollapsedName = "Collapsed";

		/// <summary>
		/// The unlocalized name for an IGoCollapsible expansion, "Expanded".
		/// </summary>
		public const string ExpandedName = "Expanded";

		/// <summary>
		/// The unlocalized name for a GoToolCreating addition of an object, "Drag Created".
		/// </summary>
		public const string DragCreatedName = "Drag Created";

		private List<IGoUndoableEdit> myCompoundEdits = new List<IGoUndoableEdit>();

		private int myMaximumEditCount = -1;

		private int myCurrentEditIndex = -1;

		private GoUndoManagerCompoundEdit myIncompleteEdit;

		private int myLevel;

		private bool myIsUndoing;

		private bool myIsRedoing;

		private List<GoDocument> myDocuments = new List<GoDocument>();

		private bool myChecksTransactionLevel;

		[NonSerialized]
		private ResourceManager myResourceManager;

		/// <summary>
		/// Gets the current GoUndoManagerCompoundEdit to be undone, or null if there is none.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.CanUndo" />
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.Undo" />
		/// <seealso cref="P:Northwoods.Go.GoUndoManager.AllEdits" />
		/// <seealso cref="P:Northwoods.Go.GoUndoManager.UndoEditIndex" />
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual IGoUndoableEdit EditToUndo
		{
			get
			{
				if (myCurrentEditIndex >= 0 && myCurrentEditIndex <= checked(myCompoundEdits.Count - 1))
				{
					return myCompoundEdits[myCurrentEditIndex];
				}
				return null;
			}
		}

		/// <summary>
		/// This predicate is true during a call to <see cref="M:Northwoods.Go.GoUndoManager.Undo" />.
		/// </summary>
		/// <remarks>
		/// When this property is true, <see cref="M:Northwoods.Go.GoUndoManager.CanUndo" /> and
		/// <see cref="M:Northwoods.Go.GoUndoManager.CanRedo" /> will be false.
		/// To avoid confusion, all document change events are ignored
		/// when this property is true.
		/// </remarks>
		public virtual bool IsUndoing => myIsUndoing;

		/// <summary>
		/// Gets the user-visible string description of the next undo action.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoUndoableEdit.PresentationName" />.
		public virtual string UndoPresentationName
		{
			get
			{
				IGoUndoableEdit editToUndo = EditToUndo;
				if (editToUndo != null)
				{
					return editToUndo.PresentationName;
				}
				return "";
			}
		}

		/// <summary>
		/// Gets the current GoUndoManagerCompoundEdit to be redone, or null if there is none.
		/// </summary>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.CanRedo" />
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.Redo" />
		/// <seealso cref="P:Northwoods.Go.GoUndoManager.AllEdits" />
		/// <seealso cref="P:Northwoods.Go.GoUndoManager.UndoEditIndex" />
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual IGoUndoableEdit EditToRedo
		{
			get
			{
				checked
				{
					if (myCurrentEditIndex < myCompoundEdits.Count - 1)
					{
						return myCompoundEdits[myCurrentEditIndex + 1];
					}
					return null;
				}
			}
		}

		/// <summary>
		/// This predicate is true during a call to <see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </summary>
		/// <remarks>
		/// When this property is true, <see cref="M:Northwoods.Go.GoUndoManager.CanUndo" /> and
		/// <see cref="M:Northwoods.Go.GoUndoManager.CanRedo" /> will be false.
		/// To avoid confusion, all document change events are ignored
		/// when this property is true.
		/// </remarks>
		public virtual bool IsRedoing => myIsRedoing;

		/// <summary>
		/// Gets the user-visible string description of the next redo action.
		/// </summary>
		/// <seealso cref="P:Northwoods.Go.IGoUndoableEdit.PresentationName" />.
		public virtual string RedoPresentationName
		{
			get
			{
				IGoUndoableEdit editToRedo = EditToRedo;
				if (editToRedo != null)
				{
					return editToRedo.PresentationName;
				}
				return "";
			}
		}

		/// <summary>
		///  Gets or sets the ResourceManager used to get presentation names.
		/// </summary>
		/// <value>
		/// The default value is null, which means the standard presentation names
		/// are not replaced by any substitute strings from resource managers.
		/// </value>
		public ResourceManager ResourceManager
		{
			get
			{
				return myResourceManager;
			}
			set
			{
				myResourceManager = value;
			}
		}

		/// <summary>
		/// Gets a list of all of the compound edits.
		/// </summary>
		/// <value>This will be an <c>List</c> of <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />s</value>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public IList<IGoUndoableEdit> AllEdits => myCompoundEdits;

		/// <summary>
		/// Gets or sets the maximum number of compound edits that this undo manager will remember.
		/// </summary>
		/// <value>
		/// If the value is negative, no limit is assumed.
		/// A new value of zero is treated as if the new value were one.
		/// The initial value is -1.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property is useful in helping limit the memory consumption of typical application usage.
		/// But note that this only limits the number of compound edits, not the size of any individual
		/// <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />, which may still have an unlimited number of
		/// <see cref="T:Northwoods.Go.GoChangedEventArgs" />s.
		/// </para>
		/// <para>
		/// Decreasing this value will not necessarily remove any existing edits
		/// if there currently exist more edits in <see cref="P:Northwoods.Go.GoUndoManager.AllEdits" /> than the new value would allow.
		/// </para>
		/// </remarks>
		public int MaximumEditCount
		{
			get
			{
				return myMaximumEditCount;
			}
			set
			{
				if (value == 0)
				{
					value = 1;
				}
				myMaximumEditCount = value;
			}
		}

		/// <summary>
		/// Gets the index into AllEdits for the current undoable edit.
		/// </summary>
		/// <value>
		/// -1 if there's no undoable edit to be undone.
		/// </value>
		public int UndoEditIndex => myCurrentEditIndex;

		/// <summary>
		/// Gets the current compound edit for recording additional document change events.
		/// </summary>
		/// <remarks>
		/// This is initialized and augmented by <see cref="M:Northwoods.Go.GoUndoManager.DocumentChanged(System.Object,Northwoods.Go.GoChangedEventArgs)" />,
		/// before it is added to <see cref="P:Northwoods.Go.GoUndoManager.AllEdits" />.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public GoUndoManagerCompoundEdit CurrentEdit
		{
			get
			{
				return myIncompleteEdit;
			}
			set
			{
				myIncompleteEdit = value;
			}
		}

		/// <summary>
		/// Gets a list of documents for which this UndoManager is recording undo/redo
		/// information.
		/// </summary>
		/// <remarks>
		/// You can manipulate this list explicitly by calling
		/// <see cref="M:Northwoods.Go.GoUndoManager.AddDocument(Northwoods.Go.GoDocument)" /> and <see cref="M:Northwoods.Go.GoUndoManager.RemoveDocument(Northwoods.Go.GoDocument)" />.
		/// Setting <see cref="P:Northwoods.Go.GoDocument.UndoManager" /> automatically calls these methods.
		/// <see cref="M:Northwoods.Go.GoUndoManager.Undo" /> and <see cref="M:Northwoods.Go.GoUndoManager.Redo" /> use this list to call
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" /> with notices about starting and
		/// ending undo and redo actions, and about starting/finishing/aborting transactions.
		/// </remarks>
		public virtual IEnumerable<GoDocument> Documents => myDocuments;

		/// <summary>
		/// Gets the current transaction level.
		/// </summary>
		/// <value>
		/// This value is zero when no transaction is in progress.
		/// The initial value is zero.
		/// <see cref="M:Northwoods.Go.GoUndoManager.StartTransaction" /> will increment this value;
		/// <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" /> will decrement it.
		/// When this value is greater than zero, <see cref="M:Northwoods.Go.GoUndoManager.CanUndo" />
		/// and <see cref="M:Northwoods.Go.GoUndoManager.CanRedo" /> will be false, because
		/// additional logically related document change events may occur.
		/// </value>
		public int TransactionLevel => myLevel;

		/// <summary>
		///  Gets or sets whether this undo manager will output warnings to Trace listeners
		///  when document changes occur outside of a transaction.
		/// </summary>
		/// <value>
		/// This defaults to false.
		/// </value>
		public bool ChecksTransactionLevel
		{
			get
			{
				return myChecksTransactionLevel;
			}
			set
			{
				myChecksTransactionLevel = value;
			}
		}

		/// <summary>
		/// Clear all of the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />s and reset all other state.
		/// </summary>
		/// <remarks>
		/// However, this does not forget the documents that this undo manager is
		/// managing, nor does it forget the assigned ResourceManager, if any.
		/// </remarks>
		public virtual void Clear()
		{
			checked
			{
				for (int num = myCompoundEdits.Count - 1; num >= 0; num--)
				{
					myCompoundEdits[num].Clear();
				}
				myCompoundEdits.Clear();
				myCurrentEditIndex = -1;
				myIncompleteEdit = null;
				myLevel = 0;
				myIsUndoing = false;
				myIsRedoing = false;
			}
		}

		/// <summary>
		/// This predicate is true when one can call <see cref="M:Northwoods.Go.GoUndoManager.Undo" />.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// In order to be able to perform an undo, a transaction must not
		/// be in progress, nor an undo or a redo.
		/// Furthermore there must be an <see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" /> that itself
		/// is ready to be undone, because its <see cref="M:Northwoods.Go.IGoUndoableEdit.CanUndo" />
		/// predicate is true.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.Undo" />
		public virtual bool CanUndo()
		{
			if (TransactionLevel > 0)
			{
				return false;
			}
			if (IsUndoing)
			{
				return false;
			}
			if (IsRedoing)
			{
				return false;
			}
			return EditToUndo?.CanUndo() ?? false;
		}

		/// <summary>
		/// Restore the state of some documents to before the current <see cref="T:Northwoods.Go.IGoUndoableEdit" />.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.IGoUndoableEdit.Undo" /> on the current <see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" />.
		/// This will raise a <see cref="E:Northwoods.Go.GoDocument.Changed" /> event with a hint of
		/// <see cref="F:Northwoods.Go.GoDocument.StartingUndo" /> before actually performing the undo, and will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedUndo" /> afterwards.
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToUndo" /> before calling Undo.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.CanUndo" />
		public virtual void Undo()
		{
			checked
			{
				if (CanUndo())
				{
					IGoUndoableEdit editToUndo = EditToUndo;
					try
					{
						foreach (GoDocument document in Documents)
						{
							document.RaiseChanged(107, 0, editToUndo, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
						}
						myIsUndoing = true;
						myCurrentEditIndex--;
						editToUndo.Undo();
					}
					catch (Exception ex)
					{
						GoObject.Trace("Undo: " + ex.ToString());
						throw;
					}
					finally
					{
						myIsUndoing = false;
						foreach (GoDocument document2 in Documents)
						{
							document2.RaiseChanged(108, 0, editToUndo, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
						}
					}
				}
			}
		}

		/// <summary>
		/// This predicate is true when one can call <see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// In order to be able to perform a redo, a transaction must not
		/// be in progress, nor an undo or a redo.
		/// Furthermore there must be an <see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" /> that itself
		/// is ready to be redone, because its <see cref="M:Northwoods.Go.IGoUndoableEdit.CanRedo" />
		/// predicate is true.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.Redo" />
		public virtual bool CanRedo()
		{
			if (TransactionLevel > 0)
			{
				return false;
			}
			if (IsUndoing)
			{
				return false;
			}
			if (IsRedoing)
			{
				return false;
			}
			return EditToRedo?.CanRedo() ?? false;
		}

		/// <summary>
		/// Restore the state of some documents to after the current <see cref="T:Northwoods.Go.IGoUndoableEdit" />.
		/// </summary>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.IGoUndoableEdit.Redo" /> on the current <see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" />.
		/// This will raise a <see cref="E:Northwoods.Go.GoDocument.Changed" /> event with a hint of
		/// <see cref="F:Northwoods.Go.GoDocument.StartingRedo" /> before actually performing the redo, and will raise a
		/// Changed event with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedRedo" /> afterwards.
		/// The <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> that was the value of
		/// <see cref="P:Northwoods.Go.GoUndoManager.EditToRedo" /> before calling Redo.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.CanRedo" />
		public virtual void Redo()
		{
			checked
			{
				if (CanRedo())
				{
					IGoUndoableEdit editToRedo = EditToRedo;
					try
					{
						foreach (GoDocument document in Documents)
						{
							document.RaiseChanged(109, 0, editToRedo, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
						}
						myIsRedoing = true;
						myCurrentEditIndex++;
						editToRedo.Redo();
					}
					catch (Exception ex)
					{
						GoObject.Trace("Redo: " + ex.ToString());
						throw;
					}
					finally
					{
						myIsRedoing = false;
						foreach (GoDocument document2 in Documents)
						{
							document2.RaiseChanged(110, 0, editToRedo, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
						}
					}
				}
			}
		}

		/// <summary>
		/// Look up a string in a <c>ResourceManager</c>.
		/// </summary>
		/// <param name="tname">a transaction name, such as one of the <see cref="F:Northwoods.Go.GoUndoManager.MoveSelectionName" /> constants</param>
		/// <returns>a user-visible presentation name</returns>
		/// <remarks>
		/// This method first tries the <see cref="P:Northwoods.Go.GoUndoManager.ResourceManager" /> property.
		/// Otherwise it just returns <paramref name="tname" />.
		/// </remarks>
		public virtual string GetPresentationName(string tname)
		{
			if (tname == null)
			{
				return "";
			}
			string text = null;
			if (ResourceManager != null)
			{
				text = ResourceManager.GetString(tname, CultureInfo.CurrentCulture);
			}
			if (text == null)
			{
				text = tname;
			}
			return text;
		}

		/// <summary>
		/// Make sure this undo manager knows about a <see cref="T:Northwoods.Go.GoDocument" /> for which
		/// it is receiving document Changed event notifications.
		/// </summary>
		/// <param name="doc"></param>
		/// <remarks>
		/// This just adds <paramref name="doc" /> to the list of <see cref="P:Northwoods.Go.GoUndoManager.Documents" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.RemoveDocument(Northwoods.Go.GoDocument)" />
		public virtual void AddDocument(GoDocument doc)
		{
			if (!myDocuments.Contains(doc))
			{
				myDocuments.Add(doc);
			}
		}

		/// <summary>
		/// Call this method to inform this undo manager that it no longer will be
		/// notified of document Changed events.
		/// </summary>
		/// <param name="doc"></param>
		/// <remarks>
		/// This just removes <paramref name="doc" /> from the list of <see cref="P:Northwoods.Go.GoUndoManager.Documents" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.AddDocument(Northwoods.Go.GoDocument)" />
		public virtual void RemoveDocument(GoDocument doc)
		{
			myDocuments.Remove(doc);
		}

		/// <summary>
		/// Create an <see cref="T:Northwoods.Go.IGoUndoableEdit" /> for a <see cref="T:Northwoods.Go.GoDocument" /> Changed event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// This calls <see cref="M:Northwoods.Go.GoUndoManager.SkipEvent(Northwoods.Go.GoChangedEventArgs)" /> if for some reason we should ignore
		/// the <paramref name="e" />.
		/// This then creates a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> and adds it to the
		/// <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" />, a <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" /> which it allocates
		/// if needed.
		/// This method always ignores all Changed events while we are performing an
		/// <see cref="M:Northwoods.Go.GoUndoManager.Undo" /> or <see cref="M:Northwoods.Go.GoUndoManager.Redo" />.
		/// </remarks>
		public virtual void DocumentChanged(object sender, GoChangedEventArgs e)
		{
			if (!IsUndoing && !IsRedoing && !SkipEvent(e))
			{
				GoUndoManagerCompoundEdit goUndoManagerCompoundEdit = CurrentEdit;
				if (goUndoManagerCompoundEdit == null || goUndoManagerCompoundEdit.IsComplete)
				{
					goUndoManagerCompoundEdit = (CurrentEdit = new GoUndoManagerCompoundEdit());
				}
				GoChangedEventArgs goChangedEventArgs = new GoChangedEventArgs(e);
				goUndoManagerCompoundEdit.AddEdit(goChangedEventArgs);
				if (ChecksTransactionLevel && TransactionLevel <= 0)
				{
					GoObject.Trace("Change not within a transaction: " + goChangedEventArgs.ToString());
				}
			}
		}

		/// <summary>
		/// This predicate is responsible for deciding if a <see cref="T:Northwoods.Go.GoChangedEventArgs" />
		/// is not interesting enough to be recorded.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		/// <remarks>
		/// This returns true:
		/// if the <see cref="P:Northwoods.Go.GoChangedEventArgs.Document" />'s <see cref="P:Northwoods.Go.GoDocument.SkipsUndoManager" />
		/// property is true, or if the <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> is
		/// <see cref="F:Northwoods.Go.GoDocument.RepaintAll" />, or if it's describing a changed object, and
		/// the object's <see cref="P:Northwoods.Go.GoObject.SkipsUndoManager" /> property is true or if
		/// the <see cref="P:Northwoods.Go.GoChangedEventArgs.SubHint" /> is <see cref="F:Northwoods.Go.GoObject.RepaintAll" />.
		/// </remarks>
		public virtual bool SkipEvent(GoChangedEventArgs evt)
		{
			if (evt.Document == null || evt.Document.SkipsUndoManager || (evt.Hint >= 0 && evt.Hint < 200))
			{
				return true;
			}
			if (evt.Hint == 901 && (evt.GoObject == null || evt.GoObject.SkipsUndoManager || evt.SubHint == 1000))
			{
				return true;
			}
			if (evt.Hint >= 208 && evt.Hint <= 215 && evt.OldValue.GetType() == typeof(bool))
				//the change of allow move,without notification ,bylzy
				//evt.Hint < ChangedAllowMove && evt.Hint > ChangedAllowEdit
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Begin a transaction, where the changes are held by a <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />.
		/// </summary>
		/// <returns>true if starting a top-level transaction</returns>
		/// <remarks>
		/// Transactions can be nested:
		/// <list type="numbered">
		/// <item><c>StartTransaction</c> returns true</item>
		/// <item><c>StartTransaction</c> returns false</item>
		/// <item><c>FinishTransaction</c> returns false</item>
		/// <item><c>FinishTransaction</c> returns true</item>
		/// </list>
		/// Nested transactions will share the same compound edit as the top-level one.
		/// This will raise a <see cref="E:Northwoods.Go.GoDocument.Changed" /> event for each of the <see cref="P:Northwoods.Go.GoUndoManager.Documents" />,
		/// with a hint of <see cref="F:Northwoods.Go.GoDocument.StartedTransaction" />.
		/// </remarks>
		/// <seealso cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" />
		public virtual bool StartTransaction()
		{
			checked
			{
				myLevel++;
				bool flag = myLevel == 1;
				if (flag)
				{
					foreach (GoDocument document in Documents)
					{
						document.RaiseChanged(104, 0, null, 0, null, GoObject.NullRect, 0, null, GoObject.NullRect);
					}
					return flag;
				}
				return flag;
			}
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" />, aborting the current transaction.
		/// </summary>
		/// <returns>the value of the call to <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" /></returns>
		public bool AbortTransaction()
		{
			return EndTransaction(commit: false, null, null);
		}

		/// <summary>
		/// Just call <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" />, committing the current transaction,
		/// with the presentation name for the given transaction name.
		/// </summary>
		/// <param name="tname">
		/// the transaction name;
		/// this value is passed to <see cref="M:Northwoods.Go.GoUndoManager.GetPresentationName(System.String)" /> before being passed to <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" />
		/// </param>
		/// <returns>the value of the call to <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" /></returns>
		public bool FinishTransaction(string tname)
		{
			if (tname == null)
			{
				tname = "";
			}
			return EndTransaction(commit: true, tname, GetPresentationName(tname));
		}

		/// <summary>
		/// Stop the current transaction, either aborting it or committing it.
		/// </summary>
		/// <param name="commit">true to terminate the transaction normally;
		/// false to abort it</param>
		/// <param name="tname">the internal locale-neutral name for the transaction</param>
		/// <param name="pname">a string describing the transaction,
		/// used for the <see cref="P:Northwoods.Go.IGoUndoableEdit.PresentationName" /></param>
		/// <returns>true for a committed top-level transaction</returns>
		/// <remarks>
		/// <para>
		/// If this call stops a top-level transaction, a value of false for
		/// <paramref name="commit" /> just clears the information in the
		/// <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" />.
		/// If <paramref name="commit" /> is true for a top-level transaction,
		/// we mark the <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> complete,
		/// call <see cref="M:Northwoods.Go.GoUndoManager.CommitCompoundEdit(Northwoods.Go.GoUndoManagerCompoundEdit)" />,
		/// and add the resulting <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />
		/// to the list of compound edits that this undo manager is recording.
		/// </para>
		/// <para>
		/// Committing a transaction when there have been some undos without
		/// corresponding redos will throw away the compound edits holding
		/// changes that happened after the current state, before adding this
		/// new compound edit to the undo manager's list of edits.
		/// </para>
		/// <para>
		/// This method raises a <see cref="E:Northwoods.Go.GoDocument.Changed" /> event
		/// for each of this undo manager's <see cref="P:Northwoods.Go.GoUndoManager.Documents" />,
		/// with a hint of <see cref="F:Northwoods.Go.GoDocument.FinishedTransaction" />,
		/// and with a <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />
		/// that is the <see cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />
		/// that has been added to the list of <see cref="P:Northwoods.Go.GoUndoManager.AllEdits" />.
		/// Furthermore the <see cref="T:Northwoods.Go.GoChangedEventArgs" />.<see cref="P:Northwoods.Go.GoChangedEventArgs.OldValue" />
		/// will be the <paramref name="tname" />, and the <see cref="P:Northwoods.Go.GoChangedEventArgs.NewValue" />
		/// will be the <paramref name="pname" />.
		/// Similarly, if the transaction is aborted, either because <paramref name="commit" />
		/// is false or because there is no <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> to commit,
		/// all of the <see cref="P:Northwoods.Go.GoUndoManager.Documents" /> get a <see cref="F:Northwoods.Go.GoDocument.AbortedTransaction" />
		/// Changed event.  The values passed in the <see cref="T:Northwoods.Go.GoChangedEventArgs" />
		/// may all be null, however.
		/// </para>
		/// </remarks>
		public virtual bool EndTransaction(bool commit, string tname, string pname)
		{
			bool num = myLevel == 1;
			checked
			{
				if (myLevel > 0)
				{
					myLevel--;
				}
				if (num)
				{
					GoUndoManagerCompoundEdit currentEdit = CurrentEdit;
					if (commit && currentEdit != null)
					{
						GoUndoManagerCompoundEdit goUndoManagerCompoundEdit = CommitCompoundEdit(currentEdit);
						goUndoManagerCompoundEdit.IsComplete = true;
						if (pname != null)
						{
							goUndoManagerCompoundEdit.PresentationName = pname;
						}
						for (int num2 = myCompoundEdits.Count - 1; num2 > myCurrentEditIndex; num2--)
						{
							myCompoundEdits[num2].Clear();
							myCompoundEdits.RemoveAt(num2);
						}
						if (MaximumEditCount > 0 && myCompoundEdits.Count >= MaximumEditCount)
						{
							myCompoundEdits[0].Clear();
							myCompoundEdits.RemoveAt(0);
							myCurrentEditIndex--;
						}
						myCompoundEdits.Add(goUndoManagerCompoundEdit);
						myCurrentEditIndex++;
						foreach (GoDocument document in Documents)
						{
							document.RaiseChanged(105, 0, goUndoManagerCompoundEdit, 0, tname, GoObject.NullRect, 0, pname, GoObject.NullRect);
						}
					}
					else
					{
						foreach (GoDocument document2 in Documents)
						{
							document2.RaiseChanged(106, 0, currentEdit, 0, tname, GoObject.NullRect, 0, pname, GoObject.NullRect);
						}
					}
					CurrentEdit = null;
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// This method is called by <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" /> when committing a
		/// compound edit. 
		/// </summary>
		/// <param name="cedit">the <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /></param>
		/// <returns>By default, the unmodified <paramref name="cedit" />.</returns>
		public virtual GoUndoManagerCompoundEdit CommitCompoundEdit(GoUndoManagerCompoundEdit cedit)
		{
			return cedit;
		}
	}
}
