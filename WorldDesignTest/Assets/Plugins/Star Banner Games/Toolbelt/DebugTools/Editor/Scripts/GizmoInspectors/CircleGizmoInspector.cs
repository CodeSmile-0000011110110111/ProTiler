using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(CircleGizmo))]
	public class CircleGizmoInspector : GizmoDisplayInspector
	{
        private SerializedProperty _applyRot;
        private SerializedProperty _rotOffset;
        private SerializedProperty _radius;
        private SerializedProperty _segments;

        private bool _rotOffsetAtr = false;
        private bool _radiusAtr = false;
        private bool _segmentAtr = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _applyRot = serializedObject.FindProperty("applyTransformRotation");
            _rotOffset = serializedObject.FindProperty("rotationOffset");
            _radius = serializedObject.FindProperty("radius");
            _segments = serializedObject.FindProperty("segments");
        }

        protected override void DisplayAdditionalPropertyFields()
        {
            EditorGUILayout.PropertyField(_applyRot);

            EditorGUILayout.Space();

            DisplayAttributeProperty(_posOffset, _posOffsetAtr, "GizmoOffset");
            DisplayAttributeProperty(_rotOffset, _rotOffsetAtr, "GizmoRotationOffset");

            EditorGUILayout.Space();

            DisplayAttributeProperty(_radius, _radiusAtr, "GizmoRadius");
            DisplayAttributeProperty(_segments, _segmentAtr, "GizmoSegments");
        }

        protected override void ResetAttributeStates()
        {
            base.ResetAttributeStates();
            _rotOffsetAtr = false;
            _radiusAtr = false;
            _segmentAtr = false;
        }

        protected override void EvaluateForAdditionalAttributes(GizmoAttribute attribute, FieldInfo field, MonoBehaviour mono)
        {
            if (attribute is GizmoRotationOffset && field.FieldType == typeof(Vector3))
            {
                _rotOffset.vector3Value = (Vector3)field.GetValue(mono);
                _rotOffsetAtr = true;
            }
            else if (attribute is GizmoRadius && field.FieldType == typeof(float))
            {
                _radius.floatValue = (float)field.GetValue(mono);
                _radiusAtr = true;
            }
            else if (attribute is GizmoSegments && field.FieldType == typeof(int))
            {
                _segments.intValue = (int)field.GetValue(mono);
                _segmentAtr = true;
            }
        }
    }
}