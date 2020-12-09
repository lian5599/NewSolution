namespace Northwoods.Go
{
	/// <summary>
	/// This interface specifies the methods the view uses to control this tool
	/// and the methods used to handle the canonicalized input events processed through
	/// the view.
	/// </summary>
	/// <remarks>
	/// All existing tools are actually subclasses of the abstract class <see cref="T:Northwoods.Go.GoTool" />.
	/// </remarks>
	public interface IGoTool
	{
		/// <summary>
		/// Gets the view for which this tool is handling canonicalized  input events.
		/// </summary>
		GoView View
		{
			get;
			set;
		}

		/// <summary>
		/// This predicate is used by the view to decide if this tool can be started.
		/// </summary>
		/// <returns>true if the view can make this tool the current one and call
		/// the <see cref="M:Northwoods.Go.IGoTool.Start" /> method</returns>
		bool CanStart();

		/// <summary>
		/// This method is called by the view when this tool becomes the currently active tool.
		/// </summary>
		/// <remarks>
		/// Tool implementations should perform their per-use initialization here, such
		/// as starting a document transaction and setting up internal data structures.
		/// </remarks>
		void Start();

		/// <summary>
		/// This method is called by the view when this tool stops being the current tool.
		/// </summary>
		/// <remarks>
		/// Tool implementations should perform their per-use cleanup here, such as
		/// finishing a document transaction.
		/// </remarks>
		void Stop();

		/// <summary>
		/// The view will call this method upon a mouse down event.
		/// </summary>
		void DoMouseDown();

		/// <summary>
		/// The view will call this method upon a mouse drag event.
		/// </summary>
		void DoMouseMove();

		/// <summary>
		/// The view will call this method upon a mouse up event.
		/// </summary>
		void DoMouseUp();

		/// <summary>
		/// The view will call this method as the mouse wheel is rotated.
		/// </summary>
		void DoMouseWheel();

		/// <summary>
		/// The view will call this method upon a mouse hover event.
		/// </summary>
		void DoMouseHover();

		/// <summary>
		/// The view will call this method when the we wish to cancel the
		/// current tool's operation.
		/// </summary>
		void DoCancelMouse();

		/// <summary>
		/// The view will call this method upon a key down event.
		/// </summary>
		void DoKeyDown();
	}
}
