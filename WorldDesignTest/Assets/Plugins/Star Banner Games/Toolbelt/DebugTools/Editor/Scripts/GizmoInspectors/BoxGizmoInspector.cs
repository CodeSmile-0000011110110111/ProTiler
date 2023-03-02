using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(BoxGizmo))]
	public class BoxGizmoInspector : GizmoDisplayInspector
	{
        private SerializedProperty _applyRot;
        private SerializedProperty _rotOffset;
        private SerializedProperty _size;

        private bool _rotOffsetAtr = false;
        private bool _sizeAtr = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _applyRot = serializedObject.FindProperty("applyTransformRotation");
            _rotOffset = serializedObject.FindProperty("rotationOffset");
            _size = serializedObject.FindProperty("size");
        }

        protected override void DisplayAdditionalPropertyFields()
        {
            EditorGUILayout.PropertyField(_applyRot);

            EditorGUILayout.Space();

            DisplayAttributeProperty(_posOffset, _posOffsetAtr, "GizmoOffset");
            DisplayAttributeProperty(_rotOffset, _rotOffsetAtr, "GizmoRotationOffset");

            EditorGUILayout.Space();

            DisplayAttributeProperty(_size, _sizeAtr, "GizmoSize");
        }

        protected override void ResetAttributeStates()
        {
            base.ResetAttributeStates();
            _rotOffsetAtr = false;
            _sizeAtr = false;
        }

        protected override void EvaluateForAdditionalAttributes(GizmoAttribute attribute, FieldInfo field, MonoBehaviour mono)
        {
            if (attribute is GizmoRotationOffset && field.FieldType == typeof(Vector3))
            {
                _rotOffset.vector3Value = (Vector3)field.GetValue(mono);
                _rotOffsetAtr = true;
            }
            else if (attribute is GizmoSize && field.FieldType == typeof(Vector3))
            {
                _size.vector3Value = (Vector3)field.GetValue(mono);
                _sizeAtr = true;
            }
        }
    }
}