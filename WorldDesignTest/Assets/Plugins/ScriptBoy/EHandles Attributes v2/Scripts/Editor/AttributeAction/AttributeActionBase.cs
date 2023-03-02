using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Type = System.Type;
using System.Reflection;
using System.Linq;

namespace EHandles
{
    public abstract class AttributeActionBase : IAttributeAction
    {
        public static int selectedContolID;

        private AttributeSettings m_Settings;
        private Transform m_Transform;
        private FieldInfo m_FieldInfo;
        private Type m_FieldType;
       // private Quaternion m_Q;

        private bool m_CanDuplicate;

        protected AttributeSettings settings => m_Settings;
        protected Transform transform => m_Transform;
        protected FieldInfo fieldInfo => m_FieldInfo;
        protected Type fieldType => m_FieldType;
       // protected Quaternion q => m_Q;


        public void OnSceneGUI(SerializedProperty property, Attribute attribute, FieldInfo fieldInfo, Transform transform, AttributeSettings settings)
        {
            m_Transform = transform;
            m_FieldInfo = fieldInfo;
            m_FieldType = fieldInfo.FieldType;
            m_Settings = settings;

            if (settings.useLocalSpace)
            {
                var parentName = settings.localSpaceAttr.transformField;
                if (parentName != null)
                {
                    var parentProperty = property.GetNeighborProperty(parentName);
                    if (parentProperty.TryGetTransform(out var parent))
                    {
                        m_Transform = parent;
                    }

                }

               // m_Q = m_Transform.rotation;
            }
            else
            {
              //  m_Q = Quaternion.identity;
            }

            OnSceneGUI(property, attribute);
        }

        protected abstract void OnSceneGUI(SerializedProperty property, Attribute attribute);

        public bool TryGetPosition(SerializedProperty property, out Vector3 position)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Object obj = property.objectReferenceValue;
                if (obj != null)
                {
                    string type = property.type;
                    if (type == "PPtr<$Transform>")
                    {
                        position = (obj as Transform).position;
                        return true;
                    }
                    else if (type == "PPtr<$GameObject>")
                    {
                        position = (obj as GameObject).transform.position;
                        return true;
                    }
                }
            }
            else
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3: position = property.vector3Value; break;
                    case SerializedPropertyType.Vector2: position = property.vector2Value; break;
                    case SerializedPropertyType.Vector3Int: position = property.vector3IntValue; break;
                    case SerializedPropertyType.Vector2Int: position = property.vector2IntValue.Vector3(); break;
                    default: { position = Vector3.zero; return false; };
                }

                if (settings.useLocalSpace) position = m_Transform.TransformPoint(position);

                return true;
            }

            position = Vector3.zero;
            return false;
        }

        public bool TryGetPositions(SerializedProperty property, out Vector3[] positions, bool loop)
        {
            int length = property.arraySize;

            if (length < 1)
            {
                positions = null;
                return false;
            }

            List<Vector3> list = new List<Vector3>(length);
            Vector3 position;
            for (int i = 0; i < length; i++)
            {
                if (TryGetPosition(property.GetArrayElementAtIndex(i), out position))
                {
                    list.Add(position);
                }
                else
                {
                    positions = null;
                    return false;
                }
            }

            if (loop) list.Add(list[0]);

            positions = list.ToArray();
            return true;
        }

        public void SetPosition(SerializedProperty property, Vector3 position)
        {
            if (m_Settings.useArrayHotkeys && CheckArrayHotkeys(property))
            {
                return;
            }

            if (Hotkeys.snapPosition) position = EditorGridUtility.SnapToGrid(position);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var obj = property.objectReferenceValue;
                Undo.RecordObject(obj, "Move");
                if (property.type == "PPtr<$Transform>")
                {
                    (obj as Transform).position = position; ;
                }
                else if (property.type == "PPtr<$GameObject>")
                {
                    (obj as GameObject).transform.position = position; ;
                }
            }
            else
            {
                if (settings.useLocalSpace) position = m_Transform.InverseTransformPoint(position);

                switch (property.propertyType)
                {
                    case SerializedPropertyType.Vector3: property.vector3Value = position; break;
                    case SerializedPropertyType.Vector2: property.vector2Value = position; break;
                    case SerializedPropertyType.Vector3Int: property.vector3IntValue = position.Vector3Int(); break;
                    case SerializedPropertyType.Vector2Int: property.vector2IntValue = position.Vector2Int(); break;
                }
            }
        }

        private bool CheckArrayHotkeys(SerializedProperty property)
        {
            if (Hotkeys.duplicateArrayElement)
            {
                property.Duplicate();
                Hotkeys.duplicateArrayElement.Use();
                return true;
            }

            if (Hotkeys.deleteArrayElement)
            {
                property.Delete();
                GUIUtility.hotControl = selectedContolID = 0;
                Hotkeys.deleteArrayElement.Use();
                return true;
            }

            return false;
        }
    }
}