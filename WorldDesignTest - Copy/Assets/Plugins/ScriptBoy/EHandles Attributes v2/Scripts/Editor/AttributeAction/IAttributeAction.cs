using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal interface IAttributeAction
    {
        void OnSceneGUI(SerializedProperty property,
            Attribute attribute,FieldInfo fieldInfo,
            Transform transform,AttributeSettings settings);
    }
}