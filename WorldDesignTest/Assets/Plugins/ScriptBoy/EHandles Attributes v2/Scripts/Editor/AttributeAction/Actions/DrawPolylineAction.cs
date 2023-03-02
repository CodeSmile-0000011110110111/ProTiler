using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawPolylineAction : AttributeAction<DrawPolylineAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
               DrawPolyline(property);
            }
        }

        private void DrawPolyline(SerializedProperty property)
        {
            if (TryGetPositions(property, out Vector3[] positions, attribute.loop))
            {
                using (new DrawingScope(attribute.color))
                {
                    Handles.DrawAAPolyLine(positions);
                }
            }
        }
    }
}