#if SUPPORTS_SCENE_VIEW_OVERLAYS
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal partial class EHandlesToolbar
    {
        private class OptionsWindow : EditorWindow
        {
            private SerializedObject m_SerializedObject;

            private SerializedProperty m_ShowHandles;
            private SerializedProperty m_ShowSceneButtons;

            private static void CloseAll()
            {
                foreach (var w in Resources.FindObjectsOfTypeAll<OptionsWindow>())
                {
                    Debug.Log(w);
                    w.Close();
                }
            }

            public static void ShowAtPosition(Rect buttonRect)
            {
                var window = CreateInstance<OptionsWindow>();
                buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
                window.ShowAsDropDown(buttonRect, new Vector2(200, 60));
            }

            private void OnEnable()
            {
                this.titleContent = new GUIContent("EHandles Settings");
                m_SerializedObject = new SerializedObject(EHandlesSettings.instance);

                m_ShowHandles = m_SerializedObject.FindProperty(nameof(m_ShowHandles));
                m_ShowSceneButtons = m_SerializedObject.FindProperty(nameof(m_ShowSceneButtons));
            }

            private void OnDisable()
            {
                m_SerializedObject.Dispose();
            }

            public void OnGUI()
            {
                if (m_SerializedObject == null) return;

                m_SerializedObject.Update();

                using (new GUILayout.VerticalScope(Styles.window))
                {
                    RightToggle(m_ShowHandles, Styles.content_ShowHandles);
                    RightToggle(m_ShowSceneButtons, Styles.content_ShowSceneButtons);
                }

                m_SerializedObject.ApplyModifiedProperties();

            }

            private void RightToggle(SerializedProperty serializedProperty, GUIContent lable)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(lable);
                GUILayout.FlexibleSpace();
                Rect rect = GUILayoutUtility.GetRect(Styles.content_Empty, Styles.toggle);
                EditorGUI.PropertyField(rect, serializedProperty, Styles.content_Empty);
                GUILayout.EndHorizontal();
            }

            private static class Styles
            {
                static Styles()
                {
                    window = new GUIStyle(GUI.skin.window);
                    window.padding = new RectOffset(10, 10, 10, 10);
                }

                public readonly static GUIStyle window;
                public readonly static GUIStyle toggle = GUI.skin.toggle;

                public readonly static GUIContent content_ShowHandles = new GUIContent("Show Handles");
                public readonly static GUIContent content_ShowSceneButtons = new GUIContent("Show SceneButtons");
                public readonly static GUIContent content_Empty = new GUIContent("");
            }
        }
    }
}
#endif