using System;
using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	/// <summary>
	/// Adds a field to the specified Foldout-Group.
	/// If no group is defined, it is added to the last used group.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class FoldoutAttribute : PropertyAttribute
	{
		public string FoldoutGroup { get; }

		public FoldoutAttribute()
		{
			FoldoutGroup = string.Empty;
		}

		public FoldoutAttribute(string foldoutGroup)
        {
			FoldoutGroup = foldoutGroup;
        }
	}
}