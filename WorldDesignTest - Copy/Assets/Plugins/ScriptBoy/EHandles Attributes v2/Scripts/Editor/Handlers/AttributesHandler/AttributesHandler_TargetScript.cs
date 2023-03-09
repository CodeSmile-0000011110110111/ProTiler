using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal partial class AttributesHandler
    {
        internal class TargetScript
        {
            private MonoBehaviour m_Script;
            private SerializedObject m_SerializedObject;
            private Transform m_Transform;

            public TargetScript(MonoBehaviour monoBehaviour, SerializedObject serializedObject)
            {
                m_Script = monoBehaviour;
                m_SerializedObject = serializedObject;
                m_Transform = monoBehaviour.transform;
            }

            public void OnSceneGUI()
            {
                if (m_Script == null) return;

                m_SerializedObject.Update();

                var property = m_SerializedObject.GetIterator();
                while (property.NextVisible(true))
                {
                    if (EHandlesSettings.IsValidProperty(property))
                    {
                        var field = property.GetFieldInfo();
                        if (field == null) continue;

                        var settings = new AttributeSettings(field.GetCustomAttributes<SettingsAttribute>(false));

                        foreach (var attribute in field.GetCustomAttributes<Attribute>(false))
                        {
                            attribute.OnSceneGUI(property, field, m_Transform, settings);
                        }
                    }
                }

                m_SerializedObject.ApplyModifiedProperties();
            }
        }
    }
}