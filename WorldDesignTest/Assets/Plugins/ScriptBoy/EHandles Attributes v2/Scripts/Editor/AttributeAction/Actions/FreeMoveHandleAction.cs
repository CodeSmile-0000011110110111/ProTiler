using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class FreeMoveHandleAction : AttributeAction<FreeMoveHandleAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                for (int i = 0, len = property.arraySize; i < len; i++)
                {
                    FreeMoveHandle(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                FreeMoveHandle(property);
            }
        }

        private void FreeMoveHandle(SerializedProperty property)
        {
            if (TryGetPosition(property, out var position))
            {
                float handleSize = ((attribute.size == 0) ? 0.1f : attribute.size) * HandleUtility.GetHandleSize(position);

                using (new DrawingScope(attribute.color))
                {
                    EditorGUI.BeginChangeCheck();
                    var q = Quaternion.identity;
                    position = Handles.FreeMoveHandle(position, handleSize, Vector3.zero, Handles.SphereHandleCap);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetPosition(property, position);
                    }
                }
            }
        }
    }
}