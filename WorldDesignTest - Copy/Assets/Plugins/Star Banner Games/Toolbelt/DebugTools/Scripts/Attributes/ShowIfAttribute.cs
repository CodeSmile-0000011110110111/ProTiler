using System;
using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	/// <summary>
	/// Assign the name of a boolean to this attribute.
	/// This field will only display if the boolean is true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowIfAttribute : PropertyAttribute
	{
		public string Condition { get; }
		public bool Invert { get; }

		public ShowIfAttribute(string condition, bool invert=false)
		{
			Condition = condition;
			Invert = invert;
		}
	}
}