using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(LineGizmo))]
	public class LineGizmoInspector : GizmoDisplayInspector
	{
        private SerializedProperty _target;
        private SerializedProperty _targetOffset;

        private bool _targetAtr = false;
        private bool _targetOffsetAtr = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _target = serializedObject.FindProperty("targetTransform");
            _targetOffset = serializedObject.FindProperty("targetOffset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ResetAttributeStates();
            UpdateGizmo();

            EditorGUILayout.LabelField("Gizmo", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_alwaysDisplay);
            EditorGUILayout.Space();

            DisplayAdditionalPropertyFields();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DisplayAdditionalPropertyFields()
        {
            DisplayAttributeProperty(_color, _colorAtr, "GizmoColor");
            EditorGUILayout.Space();
            DisplayAttributeProperty(_origin, _originAtr, "GizmoOrigin");
            DisplayAttributeProperty(_posOffset, _posOffsetAtr, "GizmoOffset");
            EditorGUILayout.Space();
            DisplayAttributeProperty(_target, _targetAtr, "GizmoTarget");
            DisplayAttributeProperty(_targetOffset, _targetOffsetAtr, "GizmoTargetOffset");
        }

        protected override void ResetAttributeStates()
        {
            base.ResetAttributeStates();
            _targetAtr = false;
            _targetOffsetAtr = false;
        }

        protected override void EvaluateForAdditionalAttributes(GizmoAttribute attribute, FieldInfo field, MonoBehaviour mono)
        {
            if (attribute is GizmoTarget && field.FieldType == typeof(Transform))
            {
                _target.objectReferenceValue = (Transform)field.GetValue(mono);
                _targetAtr = true;
            }
            else if (attribute is GizmoTargetOffset && field.FieldType == typeof(Vector3))
            {
                _targetOffset.vector3Value = (Vector3)field.GetValue(mono);
                _targetOffsetAtr = true;
            }
        }
    }
}