using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class PositionHandleProAction : AttributeAction<PositionHandleProAttribute>
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
                int controlID = property.GetID();

                if (selectedContolID == controlID)
                {
                    if (Hotkeys.deleteArrayElement)
                    {
                        Hotkeys.deleteArrayElement.Use();
                        property.Delete();
                        GUIUtility.hotControl = selectedContolID = 0;
                        return;
                    }

                    HandleUtility.AddDefaultControl(selectedContolID);

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

                    var EVENT = Event.current;
                    if (EVENT.type == EventType.MouseDown && EVENT.button == 0)
                    {
                        selectedContolID = 0;
                        EVENT.Use();
                    }
                }
                else
                {
                    using (new DrawingScope(attribute.buttonColor))
                    {
                        float handleSize = HandleUtility.GetHandleSize(position) * attribute.buttonSize;
                        if (Handles.Button(position, Quaternion.identity, handleSize, 0, Handles.SphereHandleCap))
                        {
                            selectedContolID = controlID;
                        }
                    }
                }
            }
        }
    }
}