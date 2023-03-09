using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal partial class SceneButtonsHandler
    {
        private class TargetScript
        {
            private MonoBehaviour m_MonoBehaviour;
            private GameObject m_GameObject;
            private GameObject gameObject
            {
                get
                {
                    if (m_GameObject == null) m_GameObject = m_MonoBehaviour.gameObject;
                    return m_GameObject;
                }
            }
            private Transform m_Transform;
            private Transform transform
            {
                get
                {
                    if (m_Transform == null) m_Transform = m_MonoBehaviour.transform;
                    return m_Transform;
                }
            }
            private Button[] m_Buttons;

            public TargetScript(MonoBehaviour monoBehaviour, Button[] buttons)
            {
                m_MonoBehaviour = monoBehaviour;
                m_Buttons = buttons;
            }

            public void DrawButtons(GUIContent content, GUIStyle style)
            {
                if (m_MonoBehaviour == null) return;
         
                float buttonWidth = CalcWidth(content, style);

                Rect rect = new Rect(HandleUtility.WorldToGUIPoint(transform.position), new Vector2(buttonWidth + 5, 17));
                rect.x -= buttonWidth / 2;

                if(Selection.Contains(gameObject)) rect.y += 20;

                for (int i = 0; i < m_Buttons.Length; i++)
                {
                    m_Buttons[i].Draw(rect, content, style);
                    rect.y += 18;
                }
            }

            private float CalcWidth(GUIContent content, GUIStyle style)
            {
                float maxWidth = 0;
                for (int i = 0; i < m_Buttons.Length; i++)
                {
                    float width = m_Buttons[i].CalcWidth(content, style);

                    if (width > maxWidth)
                    {
                        maxWidth = width;
                    }
                }
                return maxWidth;
            }
        }
    }
}