using UnityEngine;

namespace CodeSmile
{
	/// <summary>
	/// Displays a field as readonly in Inspector.
	/// Usage: [ReadOnlyField] public int myValue;
	/// </summary>
	public class ReadOnlyFieldAttribute : PropertyAttribute {}
}