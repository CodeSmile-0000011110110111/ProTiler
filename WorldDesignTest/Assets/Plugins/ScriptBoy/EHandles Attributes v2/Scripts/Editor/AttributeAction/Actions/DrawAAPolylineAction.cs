using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawAAPolylineAction : AttributeAction<DrawAAPolylineAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                DrawAAPolyline(property);
            }
        }

        private void DrawAAPolyline(SerializedProperty property)
        {
 
            if (TryGetPositions(property, out Vector3[] positions, attribute.loop))
            {
                using (new DrawingScope(attribute.color))
                {
                    Handles.DrawAAPolyLine(attribute.width, positions);
                }
            }
        }
    }
}