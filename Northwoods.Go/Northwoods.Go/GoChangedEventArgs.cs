using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Northwoods.Go
{
	/// <summary>
	/// Holds information both for <see cref="E:Northwoods.Go.GoDocument.Changed" /> events and 
	/// for undo and redo handling in the undo manager.
	/// </summary>
	/// <remarks>
	/// In addition to providing before-and-after values for document change events,
	/// this also provides the storage for the undo/redo mechanism by implementing
	/// the <see cref="T:Northwoods.Go.IGoUndoableEdit" /> interface.
	/// </remarks>
	/// <seealso cref="E:Northwoods.Go.GoView.DocumentChanged" />
	[Serializable]
	public class GoChangedEventArgs : EventArgs, IGoUndoableEdit
	{
		private bool myIsBeforeChanging;

		private GoDocument myDocument;

		private int myHint;

		private int mySubHint;

		private object myObject;

		private int myOldInt;

		private object myOldValue;

		private RectangleF myOldRect;

		private int myNewInt;

		private object myNewValue;

		private RectangleF myNewRect;

		/// <summary>
		/// Gets the user-visible string description of this undoable edit.
		/// </summary>
		/// <remarks>
		/// Currently this is just the hint number, as a string.
		/// </remarks>
		public string PresentationName => myHint.ToString(CultureInfo.CurrentCulture);

		/// <summary>
		/// Gets or sets whether this event args/undoable edit was created by 
		/// a document Changed event that represents a call to
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanging(System.Int32,System.Int32,System.Object)" /> or by a call to
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// </summary>
		public bool IsBeforeChanging
		{
			get
			{
				return myIsBeforeChanging;
			}
			set
			{
				myIsBeforeChanging = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:Northwoods.Go.GoDocument" /> that raised the Changed event described
		/// by this <c>EventArgs</c>.
		/// </summary>
		/// <remarks>
		/// This value must be the same as the <see cref="T:Northwoods.Go.GoDocument" /> <c>sender</c>
		/// of a <see cref="E:Northwoods.Go.GoDocument.Changed" /> event.
		/// </remarks>
		public GoDocument Document
		{
			get
			{
				return myDocument;
			}
			set
			{
				myDocument = value;
			}
		}

		/// <summary>
		/// Gets or sets the general category of document Changed event.
		/// </summary>
		/// <remarks>
		/// Predefined GoDocument, GoLayerCollection, and GoLayer hints
		/// range from zero to one thousand.
		/// See the complete list in the documentation for <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// One frequently used hint is <see cref="F:Northwoods.Go.GoLayer.ChangedObject" />,
		/// which uses many different subhints describing the individual
		/// <see cref="P:Northwoods.Go.GoChangedEventArgs.GoObject" /> changes.
		/// </remarks>
		public int Hint
		{
			get
			{
				return myHint;
			}
			set
			{
				myHint = value;
			}
		}

		/// <summary>
		/// Gets or sets the more detailed kind of document Changed event, depending
		/// on the particular <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> value.
		/// </summary>
		/// <remarks>
		/// This property is commonly used to describe changes to individual objects
		/// when the <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> is <c>GoLayer.ChangedObject</c>,
		/// for example <c>GoObject.ChangedBounds</c>.
		/// See the complete list of predefined subhints for <see cref="P:Northwoods.Go.GoChangedEventArgs.GoObject" />
		/// changes in the documentation for <see cref="M:Northwoods.Go.GoObject.Changed(System.Int32,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />.
		/// However other <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> values may use this <c>SubHint</c> property for
		/// additional disambiguation too.
		/// </remarks>
		public int SubHint
		{
			get
			{
				return mySubHint;
			}
			set
			{
				mySubHint = value;
			}
		}

		/// <summary>
		/// Gets or sets the object that was changed by the document Changed event.
		/// </summary>
		/// <remarks>
		/// This may be null when the <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> implies the object, such
		/// as for property changes on the document itself.
		/// </remarks>
		public object Object
		{
			get
			{
				return myObject;
			}
			set
			{
				myObject = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="P:Northwoods.Go.GoChangedEventArgs.Object" /> as a <see cref="P:Northwoods.Go.GoChangedEventArgs.GoObject" />.
		/// </summary>
		/// <value>
		/// This should always be non-null when the <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" /> is
		/// <see cref="T:Northwoods.Go.GoLayer" />.<see cref="F:Northwoods.Go.GoLayer.ChangedObject" />.
		/// </value>
		public GoObject GoObject => myObject as GoObject;

		/// <summary>
		/// Gets or sets the previous or old integer value information for a change.
		/// </summary>
		public int OldInt
		{
			get
			{
				return myOldInt;
			}
			set
			{
				myOldInt = value;
			}
		}

		/// <summary>
		/// Gets or sets the previous or old arbitrary object value information
		/// for a change, including boolean values.
		/// </summary>
		public object OldValue
		{
			get
			{
				return myOldValue;
			}
			set
			{
				myOldValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the previous or old float, PositionF, SizeF, or RectangleF
		/// value information for a change.
		/// </summary>
		public RectangleF OldRect
		{
			get
			{
				return myOldRect;
			}
			set
			{
				myOldRect = value;
			}
		}

		/// <summary>
		/// Gets or sets the new integer value information for a change.
		/// </summary>
		public int NewInt
		{
			get
			{
				return myNewInt;
			}
			set
			{
				myNewInt = value;
			}
		}

		/// <summary>
		/// Gets or sets the new arbitrary object value information for a change,
		/// including boolean values.
		/// </summary>
		public object NewValue
		{
			get
			{
				return myNewValue;
			}
			set
			{
				myNewValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the new float, PositionF, SizeF, or RectangleF value
		/// information for a change.
		/// </summary>
		public RectangleF NewRect
		{
			get
			{
				return myNewRect;
			}
			set
			{
				myNewRect = value;
			}
		}

		/// <summary>
		/// The constructor produces an empty object, describing no event.
		/// </summary>
		public GoChangedEventArgs()
		{
		}

		/// <summary>
		/// This copy constructor makes a copy of the argument object.
		/// </summary>
		/// <param name="e"></param>
		public GoChangedEventArgs(GoChangedEventArgs e)
		{
			myIsBeforeChanging = e.IsBeforeChanging;
			myDocument = e.Document;
			myHint = e.Hint;
			mySubHint = e.SubHint;
			myObject = e.Object;
			myOldInt = e.OldInt;
			myOldValue = e.OldValue;
			myOldRect = e.OldRect;
			myNewInt = e.NewInt;
			myNewValue = e.NewValue;
			myNewRect = e.NewRect;
			if (myDocument != null)
			{
				myDocument.CopyOldValueForUndo(this);
				myDocument.CopyNewValueForRedo(this);
			}
		}

		/// <summary>
		/// Produce a description that may be useful in debugging event handling and the undo manager.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = PresentationName + ": " + SubHint.ToString(NumberFormatInfo.InvariantInfo);
			if (Object != null)
			{
				str = str + " " + Object.ToString();
				IGoLabeledPart goLabeledPart = Object as IGoLabeledPart;
				if (goLabeledPart != null)
				{
					str = str + " \"" + goLabeledPart.Text + "\"";
				}
			}
			if (IsBeforeChanging)
			{
				str += " (before)";
			}
			str += ";";
			if (OldInt != 0)
			{
				str = str + " " + OldInt.ToString(NumberFormatInfo.InvariantInfo);
			}
			if (OldValue != null)
			{
				str = str + " (" + OldValue.ToString() + ")";
			}
			if (OldRect != default(RectangleF))
			{
				str = str + " [" + OldRect.X.ToString(NumberFormatInfo.InvariantInfo) + "," + OldRect.Y.ToString(NumberFormatInfo.InvariantInfo) + " " + OldRect.Width.ToString(NumberFormatInfo.InvariantInfo) + "x" + OldRect.Height.ToString(NumberFormatInfo.InvariantInfo) + "]";
			}
			str += " -->";
			if (NewInt != 0)
			{
				str = str + " " + NewInt.ToString(NumberFormatInfo.InvariantInfo);
			}
			if (NewValue != null)
			{
				str = str + " (" + NewValue.ToString() + ")";
			}
			if (NewRect != default(RectangleF))
			{
				str = str + " [" + NewRect.X.ToString(NumberFormatInfo.InvariantInfo) + "," + NewRect.Y.ToString(NumberFormatInfo.InvariantInfo) + " " + NewRect.Width.ToString(NumberFormatInfo.InvariantInfo) + "x" + NewRect.Height.ToString(NumberFormatInfo.InvariantInfo) + "]";
			}
			return str;
		}

		/// <summary>
		/// Forget any references that this object may have.
		/// </summary>
		public void Clear()
		{
			myDocument = null;
			myObject = null;
			myOldValue = null;
			myNewValue = null;
		}

		/// <summary>
		/// This predicate returns true if you can call <see cref="M:Northwoods.Go.GoChangedEventArgs.Undo" />.
		/// </summary>
		/// <returns></returns>
		public bool CanUndo()
		{
			return Document != null;
		}

		/// <summary>
		/// Reverse the effects of this document change
		/// by calling <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// </summary>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoChangedEventArgs.CanUndo" /> must be true for this method to call <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// </remarks>
		public void Undo()
		{
			if (CanUndo())
			{
				GoDocument document = Document;
				document.Initializing = true;
				document.ChangeValue(this, undo: true);
				document.Initializing = false;
			}
		}

		/// <summary>
		/// This predicate returns true if you can call <see cref="M:Northwoods.Go.GoChangedEventArgs.Redo" />.
		/// </summary>
		/// <returns></returns>
		public bool CanRedo()
		{
			if (!IsBeforeChanging)
			{
				return Document != null;
			}
			return false;
		}

		/// <summary>
		/// Re-perform the document change after an <see cref="M:Northwoods.Go.GoChangedEventArgs.Undo" />
		/// by calling <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// </summary>
		/// <remarks>
		/// <see cref="M:Northwoods.Go.GoChangedEventArgs.CanRedo" /> must be true for this method to call <see cref="M:Northwoods.Go.GoDocument.ChangeValue(Northwoods.Go.GoChangedEventArgs,System.Boolean)" />.
		/// </remarks>
		public void Redo()
		{
			if (CanRedo())
			{
				GoDocument document = Document;
				document.Initializing = true;
				document.ChangeValue(this, undo: false);
				document.Initializing = false;
			}
		}

		/// <summary>
		/// Search for a <see cref="T:Northwoods.Go.GoChangedEventArgs" /> that matches this
		/// one's <see cref="P:Northwoods.Go.GoChangedEventArgs.Document" />, <see cref="P:Northwoods.Go.GoChangedEventArgs.Hint" />, <see cref="P:Northwoods.Go.GoChangedEventArgs.SubHint" />,
		/// and <see cref="P:Northwoods.Go.GoChangedEventArgs.Object" />, and whose <see cref="P:Northwoods.Go.GoChangedEventArgs.IsBeforeChanging" />
		/// property is true.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// The assumption is that there are always pairs of calls to
		/// <see cref="M:Northwoods.Go.GoDocument.RaiseChanging(System.Int32,System.Int32,System.Object)" /> and <see cref="M:Northwoods.Go.GoDocument.RaiseChanged(System.Int32,System.Int32,System.Object,System.Int32,System.Object,System.Drawing.RectangleF,System.Int32,System.Object,System.Drawing.RectangleF)" />,
		/// resulting in pairs of <see cref="T:Northwoods.Go.GoChangedEventArgs" />.
		/// This method is not called except when <see cref="M:Northwoods.Go.GoDocument.RaiseChanging(System.Int32,System.Int32,System.Object)" />
		/// should have produced recently a <see cref="P:Northwoods.Go.GoChangedEventArgs.IsBeforeChanging" /> event args.
		/// This searches <see cref="P:Northwoods.Go.GoUndoManager.CurrentEdit" /> backwards.
		/// </remarks>
		public GoChangedEventArgs FindBeforeChangingEdit()
		{
			if (IsBeforeChanging)
			{
				return null;
			}
			GoDocument document = Document;
			if (document == null)
			{
				return null;
			}
			GoUndoManager undoManager = document.UndoManager;
			if (undoManager == null)
			{
				return null;
			}
			GoUndoManagerCompoundEdit currentEdit = undoManager.CurrentEdit;
			if (currentEdit == null)
			{
				return null;
			}
			IList<IGoUndoableEdit> allEdits = currentEdit.AllEdits;
			checked
			{
				for (int num = allEdits.Count - 1; num >= 0; num--)
				{
					GoChangedEventArgs goChangedEventArgs = allEdits[num] as GoChangedEventArgs;
					if (goChangedEventArgs != null && goChangedEventArgs.IsBeforeChanging && goChangedEventArgs.Document == Document && goChangedEventArgs.Hint == Hint && goChangedEventArgs.SubHint == SubHint && goChangedEventArgs.Object == Object)
					{
						return goChangedEventArgs;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns <see cref="P:Northwoods.Go.GoChangedEventArgs.OldInt" />,
		/// otherwise it returns <see cref="P:Northwoods.Go.GoChangedEventArgs.NewInt" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>An <c>int</c></returns>
		public int GetInt(bool undo)
		{
			if (undo)
			{
				return OldInt;
			}
			return NewInt;
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns <see cref="P:Northwoods.Go.GoChangedEventArgs.OldValue" />,
		/// otherwise it returns <see cref="P:Northwoods.Go.GoChangedEventArgs.NewValue" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>An <c>Object</c></returns>
		public object GetValue(bool undo)
		{
			if (undo)
			{
				return OldValue;
			}
			return NewValue;
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns <see cref="P:Northwoods.Go.GoChangedEventArgs.OldRect" />,
		/// otherwise it returns <see cref="P:Northwoods.Go.GoChangedEventArgs.NewRect" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>A <c>RectangleF</c></returns>
		public RectangleF GetRect(bool undo)
		{
			if (undo)
			{
				return OldRect;
			}
			return NewRect;
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns the <c>X</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.OldRect" />,
		/// otherwise it returns the <c>X</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.NewRect" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>A <c>float</c></returns>
		public float GetFloat(bool undo)
		{
			if (undo)
			{
				return OldRect.X;
			}
			return NewRect.X;
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns the <c>Location</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.OldRect" />,
		/// otherwise it returns the <c>Location</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.NewRect" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>A <c>PointF</c></returns>
		public PointF GetPoint(bool undo)
		{
			if (undo)
			{
				return new PointF(OldRect.X, OldRect.Y);
			}
			return new PointF(NewRect.X, NewRect.Y);
		}

		/// <summary>
		/// If <paramref name="undo" /> is true, this returns the <c>Size</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.OldRect" />,
		/// otherwise it returns the <c>Size</c> part of <see cref="P:Northwoods.Go.GoChangedEventArgs.NewRect" />.
		/// </summary>
		/// <param name="undo"></param>
		/// <returns>A <c>SizeF</c></returns>
		public SizeF GetSize(bool undo)
		{
			if (undo)
			{
				return new SizeF(OldRect.Width, OldRect.Height);
			}
			return new SizeF(NewRect.Width, NewRect.Height);
		}
	}
}
