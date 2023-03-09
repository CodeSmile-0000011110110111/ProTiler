using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class PositionHandleAction : AttributeAction<PositionHandleAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                for (int i = 0, len = property.arraySize; i < len; i++)
                {
                    PositionHandle(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                PositionHandle(property);
            }
        }

        private void PositionHandle(SerializedProperty property)
        {
            if (TryGetPosition(property, out Vector3 position))
            {
                Quaternion q = Quaternion.identity;

                if (Tools.pivotRotation == PivotRotation.Local)
                {
                    if (property.TryGetTransform(out var t))
                    {
                        q = t.rotation;
                    }
                    else
                    {
                        q = transform.rotation;
                    }
                }

                EditorGUI.BeginChangeCheck();
                position = Handles.PositionHandle(position, q);
                if (EditorGUI.EndChangeCheck())
                {
                    SetPosition(property, position);
                }
            }
        }
    }
}