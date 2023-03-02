using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawCubeAction : AttributeAction<DrawCubeAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                int arraySize = property.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    DrawCube(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                DrawCube(property);
            }
        }

        private void DrawCube(SerializedProperty property)
        {
            if (TryGetPosition(property, out Vector3 position))
            {
                using (new DrawingScope(attribute.color))
                {
                    float handleSize = HandleUtility.GetHandleSize(position) * attribute.size;
                    Handles.CubeHandleCap(0, position, Quaternion.identity, handleSize, EventType.Repaint);
                }
            }
        }
    }
}