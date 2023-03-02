using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawSphereAction : AttributeAction<DrawSphereAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                int arraySize = property.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    DrawSphere(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                DrawSphere(property);
            }
        }

        private void DrawSphere(SerializedProperty property)
        {
            if (!TryGetPosition(property, out Vector3 position)) return;

            using (new DrawingScope(attribute.color))
            {
                float handleSize = HandleUtility.GetHandleSize(position) * attribute.size;
                Handles.SphereHandleCap(0, position, Quaternion.identity, handleSize, EventType.Repaint);
            }
        }
    }
}