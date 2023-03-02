using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(GizmoDisplay))]
	public abstract class GizmoDisplayInspector : UnityEditor.Editor
	{
		private GizmoDisplay _gimzo;

        protected SerializedProperty _alwaysDisplay;
        protected SerializedProperty _displayMode;
        protected SerializedProperty _color;
        protected SerializedProperty _origin;
        protected SerializedProperty _posOffset;

        protected bool _colorAtr = false;
        protected bool _originAtr = false;
        protected bool _posOffsetAtr = false;


        protected virtual void OnEnable()
        {
            _gimzo = target as GizmoDisplay;

            _alwaysDisplay = serializedObject.FindProperty("alwaysDisplay");
            _displayMode = serializedObject.FindProperty("displayMode");
            _color = serializedObject.FindProperty("color");
            _origin = serializedObject.FindProperty("origin");
            _posOffset = serializedObject.FindProperty("offset");
        }

        public override void OnInspectorGUI()
		{
            serializedObject.Update();

            ResetAttributeStates();
            UpdateGizmo();

            EditorGUILayout.LabelField("Gizmo", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_alwaysDisplay);
            EditorGUILayout.PropertyField(_displayMode);
            EditorGUILayout.Space();

            DisplayAttributeProperty(_color, _colorAtr, "GizmoColor");
            DisplayAttributeProperty(_origin, _originAtr, "GizmoOrigin");

            DisplayAdditionalPropertyFields();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DisplayAttributeProperty(SerializedProperty property, bool attributeActive, string attributeName)
        {
            if (!attributeActive)
            {
                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(property.displayName);
                EditorGUILayout.LabelField($"Controlled through a [{attributeName}] Attribute", EditorStyles.helpBox);
                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual void ResetAttributeStates()
        {
            _colorAtr = false;
            _originAtr = false;
            _posOffsetAtr = false;
        }

        protected abstract void DisplayAdditionalPropertyFields();

        protected abstract void EvaluateForAdditionalAttributes(GizmoAttribute attribute, FieldInfo field, MonoBehaviour mono);

        protected void UpdateGizmo()
        {
            MonoBehaviour[] components = _gimzo.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour mono in components)
            {
                if (mono is GizmoDisplay) continue;

                Type monoType = mono.GetType();

                //Get all Fields
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                //Look for Attributes and set their values
                foreach (var field in objectFields)
                {
                    EvaluateField(field, mono);
                }
            }
        }

        private void EvaluateField(FieldInfo field, MonoBehaviour mono)
        {
            GizmoAttribute attribute = Attribute.GetCustomAttribute(field, typeof(GizmoAttribute)) as GizmoAttribute;

            if (attribute == null) return;

            if (attribute is GizmoColor && field.FieldType == typeof(Color))
            {
                _color.colorValue = (Color)field.GetValue(mono);
                _colorAtr = true;
            }
            else if (attribute is GizmoOrigin && field.FieldType == typeof(Transform))
            {
                _origin.objectReferenceValue = (Transform)field.GetValue(mono);
                _originAtr = true;
            }
            else if (attribute is GizmoOffset && field.FieldType == typeof(Vector3))
            {
                _posOffset.vector3Value = (Vector3)field.GetValue(mono);
                _posOffsetAtr = true;
            }
            else
            {
                EvaluateForAdditionalAttributes(attribute, field, mono);
            }
        }
    }
}