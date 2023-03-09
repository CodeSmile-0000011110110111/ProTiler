using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class RotationHandleAction : AttributeAction<RotationHandleAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                for (int i = 0, len = property.arraySize; i < len; i++)
                {
                    RotationHandle(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                RotationHandle(property);
            }
        }

        private void RotationHandle(SerializedProperty property)
        {
            if (property.TryGetTransform(out Transform t))
            {
                EditorGUI.BeginChangeCheck();
                Quaternion q = Handles.RotationHandle(t.rotation, t.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Rotate");
                    t.rotation = q;
                }
            }
        }
    }
}