using System.Reflection;
using UnityEngine;

namespace EHandles
{
    internal partial class SceneButtonsHandler
    {
        private class Button
        {
            private MonoBehaviour m_MonoBehaviour;
            private MethodInfo m_MethodInfo;
            private string m_Text;

            public Button(MonoBehaviour monoBehaviour, MethodInfo methodInfo, SceneButtonAttribute attribute)
            {
                m_MonoBehaviour = monoBehaviour;
                m_MethodInfo = methodInfo;
                m_Text = (attribute.text == null) ? methodInfo.Name : attribute.text;
            }

            public void Draw(Rect rect, GUIContent content, GUIStyle style)
            {
                content.text = m_Text;
                if (GUI.Button(rect, content, style))
                {
                    m_MethodInfo.Invoke(m_MonoBehaviour, null);
                }
            }

            public float CalcWidth(GUIContent content, GUIStyle style)
            {
                content.text = m_Text;
                return style.CalcSize(content).x;
            }
        }
    }
}