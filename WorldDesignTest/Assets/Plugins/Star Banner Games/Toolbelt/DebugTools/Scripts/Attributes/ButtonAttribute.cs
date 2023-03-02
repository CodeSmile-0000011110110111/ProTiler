using System;

namespace SBG.Toolbelt.DebugTools
{
	/// <summary>
	/// Add this Attribute to a Method.
	/// This allows you to call the Method through a Button in the Inspector.
	/// Only works on Methods without parameters.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ButtonAttribute : Attribute
	{
		public string Text { get; }
		public int Height { get; }

		public ButtonAttribute()
		{
			Height = 20;
		}

		public ButtonAttribute(string buttonText)
		{
			Text = buttonText;
			Height = 20;
		}

		public ButtonAttribute(string buttonText, int buttonHeight)
		{
			Text = buttonText;
			Height = buttonHeight;
			if (Height < 1) Height = 1;
		}
	}
}