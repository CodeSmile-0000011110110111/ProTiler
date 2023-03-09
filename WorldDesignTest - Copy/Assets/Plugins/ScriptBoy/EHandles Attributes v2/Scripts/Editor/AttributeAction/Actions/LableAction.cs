using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class LableAction : AttributeAction<LabelAttribute>
    {
        private static GUIStyle style = new GUIStyle(GUI.skin.label);

        protected override void OnSceneGUI(SerializedProperty property)
        {
            var text = (attribute.text == null) ? property.name : attribute.text;

            style.normal.textColor = style.hover.textColor = attribute.color.ToUnityColor();

            if (property.isArray)
            {
                int arraySize = property.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    DrawLabel(property.GetArrayElementAtIndex(i), $"  {text}[{i}]");
                }
            }
            else
            {
                DrawLabel(property, "  " + text);
            }
        }

        private void DrawLabel(SerializedProperty property, string text)
        {
            if (TryGetPosition(property, out Vector3 position))
            {
                Handles.Label(position, text, style);
            }
        }
    }
}