using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Northwoods.Go
{
	/// <summary>
	/// This class is used to hold a list of <see cref="T:Northwoods.Go.GoChangedEventArgs" /> that
	/// should be undone or redone altogether.
	/// </summary>
	[Serializable]
	public class GoUndoManagerCompoundEdit : IGoUndoableEdit
	{
		private List<IGoUndoableEdit> myEdits = new List<IGoUndoableEdit>();

		private bool myIsComplete;

		private string myName = "";

		/// <summary>
		/// Gets the user-visible string description of this compound edit.
		/// </summary>
		/// <remarks>
		/// This property is normally set to the value of
		/// <see cref="M:Northwoods.Go.GoUndoManager.GetPresentationName(System.String)" /> as part of
		/// <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" />.
		/// This property is used by the normal implementations of
		/// <see cref="P:Northwoods.Go.GoUndoManager.UndoPresentationName" /> and
		/// <see cref="P:Northwoods.Go.GoUndoManager.RedoPresentationName" />
		/// for generating undo and redo specific descriptions for this
		/// compound edit.
		/// </remarks>
		public virtual string PresentationName
		{
			get
			{
				return myName;
			}
			set
			{
				myName = value;
			}
		}

		/// <summary>
		/// Gets a list of all the <see cref="T:Northwoods.Go.IGoUndoableEdit" />s in this compound edit.
		/// </summary>
		/// <value>A <c>List&lt;T&gt;</c> of <see cref="T:Northwoods.Go.IGoUndoableEdit" />s</value>
		/// <remarks>
		/// Each item is normally an instance of <see cref="T:Northwoods.Go.GoChangedEventArgs" />.
		/// However, you may add your own <see cref="T:Northwoods.Go.IGoUndoableEdit" /> objects.
		/// </remarks>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual IList<IGoUndoableEdit> AllEdits => myEdits;

		/// <summary>
		/// Gets or sets whether we can add more undoable edits to this compound edit.
		/// </summary>
		/// <value>
		/// This is initially false.  It can only be set to true,
		/// which is what <see cref="M:Northwoods.Go.GoUndoManager.EndTransaction(System.Boolean,System.String,System.String)" /> does.
		/// </value>
		public virtual bool IsComplete
		{
			get
			{
				return myIsComplete;
			}
			set
			{
				if (value)
				{
					myIsComplete = true;
				}
			}
		}

		/// <summary>
		/// Clear all of the <see cref="T:Northwoods.Go.IGoUndoableEdit" />s and forget all references to them.
		/// </summary>
		public virtual void Clear()
		{
			checked
			{
				for (int num = myEdits.Count - 1; num >= 0; num--)
				{
					myEdits[num].Clear();
				}
				myEdits.Clear();
			}
		}

		/// <summary>
		/// This predicate returns true if you can call <see cref="M:Northwoods.Go.GoUndoManagerCompoundEdit.Undo" />--
		/// namely when <see cref="P:Northwoods.Go.GoUndoManagerCompoundEdit.IsComplete" /> is true.
		/// </summary>
		/// <returns></returns>
		public virtual bool CanUndo()
		{
			return IsComplete;
		}

		/// <summary>
		/// Undo all of the <see cref="T:Northwoods.Go.IGoUndoableEdit" />s, in reverse order.
		/// </summary>
		public virtual void Undo()
		{
			checked
			{
				if (CanUndo())
				{
					for (int num = myEdits.Count - 1; num >= 0; num--)
					{
						myEdits[num].Undo();
					}
				}
			}
		}

		/// <summary>
		/// This predicate returns true if you can call <see cref="M:Northwoods.Go.GoUndoManagerCompoundEdit.Redo" />--
		/// namely when <see cref="P:Northwoods.Go.GoUndoManagerCompoundEdit.IsComplete" /> is true.
		/// </summary>
		/// <returns></returns>
		public virtual bool CanRedo()
		{
			return IsComplete;
		}

		/// <summary>
		/// Redo all of the <see cref="T:Northwoods.Go.IGoUndoableEdit" />s, in forwards order.
		/// </summary>
		public virtual void Redo()
		{
			checked
			{
				if (CanRedo())
				{
					for (int i = 0; i <= myEdits.Count - 1; i++)
					{
						myEdits[i].Redo();
					}
				}
			}
		}

		/// <summary>
		/// Add an <see cref="T:Northwoods.Go.IGoUndoableEdit" /> to the end of the list.
		/// </summary>
		/// <param name="edit"></param>
		public virtual void AddEdit(IGoUndoableEdit edit)
		{
			if (!IsComplete)
			{
				myEdits.Add(edit);
			}
		}
	}
}
