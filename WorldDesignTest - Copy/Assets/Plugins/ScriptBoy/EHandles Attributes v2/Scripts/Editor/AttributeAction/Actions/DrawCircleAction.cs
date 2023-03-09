using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal class DrawCircleAction : AttributeAction<DrawCircleAttribute>
    {
        protected override void OnSceneGUI(SerializedProperty property)
        {
            if (property.isArray)
            {
                int arraySize = property.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    DrawCircle(property.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                DrawCircle(property);
            }
        }

        private void DrawCircle(SerializedProperty property)
        {
            Vector3 position;
            float radius;

            if (property.propertyType == SerializedPropertyType.Float)
            {
                position = transform.position;
                radius = property.floatValue;
            }
            else
            {
                if (!TryGetPosition(property, out position)) return;

                 radius = attribute.radius;

                if (attribute.radiusField != null)
                {
                    var sizeProperty = property.GetNeighborProperty(attribute.radiusField);

                    if (sizeProperty == null)
                    {
                        Debug.LogError(attribute.radiusField + "is missing.");
                        return;
                    }
                    else if (!sizeProperty.TryGetNumber(ref radius))
                    {
                        Debug.LogError(attribute.radiusField + "is not a number.");
                        return;
                    }
                }
            }

            using (new DrawingScope(attribute.color))
            {
                Quaternion q = attribute.rotation.ToQuaternion();

                if (settings.useLocalSpace)
                {
                    if (property.TryGetTransform(out var t))
                    {
                        q = q * t.rotation;
                    }
                    else
                    {
                        q = q * transform.rotation;
                    }
                }

                Handles.CircleHandleCap(0, position, q, radius, EventType.Repaint);
            }
        }
    }
}