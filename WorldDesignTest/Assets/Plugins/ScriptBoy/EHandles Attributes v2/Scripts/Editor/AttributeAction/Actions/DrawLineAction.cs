using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawLineAction : AttributeAction<DrawLineAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            DrawLine(property, attribute);
        }

        private void DrawLine(SerializedProperty property, DrawLineAttribute attribute)
        {
            if (!TryGetPosition(property, out Vector3 startPoint)) return;

            Vector3 endPoint = Vector3.zero;

            if (attribute.endPointField != null)
            {
                var endPointProperty = property.GetNeighborProperty(attribute.endPointField);

                if (endPointProperty == null)
                {
                    if (attribute.endPointField == "transform")
                    {
                        endPoint = (property.serializedObject.targetObject as MonoBehaviour).transform.position;
                    }
                    else
                    {
                        var msg = string.Format("Can't draw line from {0} to {1} : {1} is missing", property.name, attribute.endPointField);
                        Debug.LogError(msg);
                        return;
                    }
                }
                else if (!TryGetPosition(endPointProperty, out endPoint))
                {
                    return;
                }
            }

            using (new DrawingScope(attribute.color))
            {
                Handles.DrawLine(startPoint, endPoint);
            }
        }
    }
}