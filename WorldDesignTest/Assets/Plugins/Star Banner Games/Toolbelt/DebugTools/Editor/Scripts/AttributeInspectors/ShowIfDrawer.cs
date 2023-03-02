using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomPropertyDrawer(typeof(ShowIfAttribute))]
	public class ShowIfDrawer : PropertyDrawer
	{
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = attribute as ShowIfAttribute;

            bool condition = CheckCondition(showIf.Condition, property.serializedObject);

            if (showIf.Invert) condition = !condition;

            if (condition) EditorGUI.PropertyField(position, property, label);
        }

        private bool CheckCondition(string condition, SerializedObject so)
        {
            try
            {
                SerializedProperty prop = so.FindProperty(condition);

                if (prop.propertyType == SerializedPropertyType.Boolean)
                {
                    return prop.boolValue;
                }
                else
                {
                    Debug.LogWarning($"[ShowIfAttribute]: Property \"{prop.name}\" is not a Boolean!");
                    return false;
                }
            }
            catch
            {
                Debug.LogWarning($"[ShowIfAttribute]: Property \"{condition}\" not found");
                return false;
            }
        }
    }
}