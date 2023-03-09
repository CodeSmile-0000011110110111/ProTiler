using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal static class AttributeExtensions
    {
        public static void OnSceneGUI(this Attribute attribute, SerializedProperty property,
            FieldInfo fieldInfo, Transform transform, AttributeSettings settings)
        {
            var action = AttributeActionCollector.GetAction(attribute.GetType());
            action?.OnSceneGUI(property, attribute, fieldInfo, transform, settings);
        }
    }
}