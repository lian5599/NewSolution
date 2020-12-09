using System;

namespace Northwoods.Go
{
	/// <summary>
	/// The label for a <see cref="T:Northwoods.Go.GoGeneralNodePort" />.
	/// </summary>
	/// <remarks>
	/// This is normally created by <see cref="T:Northwoods.Go.GoGeneralNode" />.<see cref="M:Northwoods.Go.GoGeneralNode.CreatePortLabel(System.Boolean)" />.
	/// </remarks>
	[Serializable]
	public class GoGeneralNodePortLabel : GoText
	{
		private GoGeneralNodePort myPort;

		/// <summary>
		/// Gets or sets the port corresponding to this label.
		/// </summary>
		/// <remarks>
		/// Setting this does not cause any Change notifications.
		/// This is normally set only by <see cref="T:Northwoods.Go.GoGeneralNode" />.<see cref="M:Northwoods.Go.GoGeneralNode.MakePort(System.Boolean)" />
		/// and by the <see cref="T:Northwoods.Go.GoGeneralNodePort" />.<see cref="P:Northwoods.Go.GoGeneralNodePort.Label" /> setter.
		/// </remarks>
		public GoGeneralNodePort Port
		{
			get
			{
				return myPort;
			}
			set
			{
				myPort = value;
			}
		}

		/// <summary>
		/// When this label's text string changes, we update the port's name too.
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (Text != value)
				{
					base.Text = value;
					if (Port != null)
					{
						Port.Name = Text;
					}
				}
			}
		}

		/// <summary>
		/// Create an editable, non-rescaling <see cref="T:Northwoods.Go.GoText" /> that
		/// does not yet know which <see cref="P:Northwoods.Go.GoGeneralNodePortLabel.Port" /> it is associated with.
		/// </summary>
		/// <remarks>
		/// <see cref="T:Northwoods.Go.GoGeneralNode" /> is responsible for allocating a
		/// <see cref="T:Northwoods.Go.GoGeneralNodePort" /> and associating it with this label.
		/// </remarks>
		public GoGeneralNodePortLabel()
		{
			Selectable = false;
			Editable = true;
			FontSize = GoText.DefaultFontSize - 2f;
		}
	}
}
