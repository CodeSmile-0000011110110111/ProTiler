using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class TransformHandleAction : AttributeAction<TransformHandleAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                for (int i = 0, len = property.arraySize; i < len; i++)
                {
                    TransformHandle(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                TransformHandle(property);
            }
        }

        private void TransformHandle(SerializedProperty property)
        {
            if (property.TryGetTransform(out Transform transform))
            {
                switch (Tools.current)
                {
                    case Tool.Move:
                        Move(transform);
                        break;
                    case Tool.Rotate:
                        Rotate(transform);
                        break;
                    case Tool.Scale:
                        Scale(transform);
                        break;
                }
            }
        }

        private void Move(Transform t)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion q = (Tools.pivotRotation == PivotRotation.Local) ? t.rotation : Quaternion.identity;
            Vector3 position = Handles.PositionHandle(t.position, q);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(t, "Move");
                t.position = position;
            }
        }

        private void Rotate(Transform t)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion q = Handles.RotationHandle(t.rotation, t.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(t, "Rotate");
                t.rotation = q;
            }
        }

        private void Scale(Transform t)
        {
            float handleSize = HandleUtility.GetHandleSize(t.position);
            EditorGUI.BeginChangeCheck();
            Vector3 scale = Handles.ScaleHandle(t.localScale, t.position, t.rotation, handleSize);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(t, "Move");
                t.localScale = scale;
            }
        }
    }
}