using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawAAConvexPolygonAction : AttributeAction<DrawAAConvexPolygonAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                DrawAAConvexPolygon(property);
            }
        }

        private void DrawAAConvexPolygon(SerializedProperty property)
        {
            if (!TryGetPositions(property, out Vector3[] positions, false)) return;

            using (new DrawingScope(attribute.color))
            {
                Handles.DrawAAConvexPolygon(positions);
            }
        }
    }
}