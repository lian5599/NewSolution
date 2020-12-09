namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies how a document change (an edit) can be
	/// managed by the UndoManager.
	/// </summary>
	/// <seealso cref="T:Northwoods.Go.GoChangedEventArgs" />
	/// <seealso cref="T:Northwoods.Go.GoUndoManagerCompoundEdit" />
	public interface IGoUndoableEdit
	{
		/// <summary>
		/// Gets the user-visible string description of this undoable edit.
		/// </summary>
		string PresentationName
		{
			get;
		}

		/// <summary>
		/// Forget about any state remembered in this edit.
		/// </summary>
		void Clear();

		/// <summary>
		/// Determine if this edit is ready to be and can be undone.
		/// </summary>
		/// <returns></returns>
		bool CanUndo();

		/// <summary>
		/// Restore the previous state of this edit.
		/// </summary>
		void Undo();

		/// <summary>
		/// Determine if this edit is ready to be and can be redone.
		/// </summary>
		/// <returns></returns>
		bool CanRedo();

		/// <summary>
		/// Restore the new state of this edit after having been undone.
		/// </summary>
		void Redo();
	}
}
