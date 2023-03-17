using CodeSmile;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor
{
	/// <summary>
	/// Displays a field with ReadOnlyFieldAttribute as readonly in Inspector.
	/// </summary>
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