using CodeSmile;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor.PropertyDrawers
{
	/// <summary>
	/// Displays a field with ReadOnlyFieldAttribute as readonly in Inspector.
	/// </summary>
	[ExcludeFromCodeCoverage]
	[CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
	public sealed class ReadOnlyFieldDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, label, true);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
