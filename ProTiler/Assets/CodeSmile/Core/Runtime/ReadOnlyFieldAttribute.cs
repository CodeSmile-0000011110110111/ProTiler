using System;
using UnityEngine;

namespace CodeSmile
{
	/// <summary>
	/// Displays a field as readonly in Inspector.
	/// Usage: [ReadOnlyField] public int myValue;
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ReadOnlyFieldAttribute : PropertyAttribute {}
}
