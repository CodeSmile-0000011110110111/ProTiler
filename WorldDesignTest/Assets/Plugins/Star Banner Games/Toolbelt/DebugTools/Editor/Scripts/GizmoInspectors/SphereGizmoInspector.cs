using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(SphereGizmo))]
	public class SphereGizmoInspector : GizmoDisplayInspector
	{
        private SerializedProperty _radius;

        private bool _radiusAtr = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _radius = serializedObject.FindProperty("radius");
        }

        protected override void DisplayAdditionalPropertyFields()
        {
            EditorGUILayout.Space();

            DisplayAttributeProperty(_posOffset, _posOffsetAtr, "GizmoOffset");

            EditorGUILayout.Space();

            DisplayAttributeProperty(_radius, _radiusAtr, "GizmoRadius");
        }

        protected override void ResetAttributeStates()
        {
            base.ResetAttributeStates();
            _radiusAtr = false;
        }

        protected override void EvaluateForAdditionalAttributes(GizmoAttribute attribute, FieldInfo field, MonoBehaviour mono)
        {
            if (attribute is GizmoRadius && field.FieldType == typeof(float))
            {
                _radius.floatValue = (float)field.GetValue(mono);
                _radiusAtr = true;
            }
        }
    }
}