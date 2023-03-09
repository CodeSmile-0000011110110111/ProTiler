using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EHandles
{
    [FilePath(@"Assets\ScriptBoy\EHandles Attributes v2\Scripts\Editor\Hotkeys\Hotkeys.asset")]
    public class Hotkeys : ScriptableSingleton<Hotkeys>
    {
        [MenuItem("Tools/ScriptBoy/EHandles/Hotkeys", priority = 1)]
        private static void Select()
        {
            Selection.activeObject = instance;
        }

        private static List<KeyCode> s_PressedKeys = new List<KeyCode>();

        public static void UpdateHotkeys(Event e)
        {
            UpdatePressedKeys(e);
            instance.UpdateHotkeys();
        }

        private static void UpdatePressedKeys(Event e)
        {
            if (!e.isKey) return;

            var type = e.type;

            if (type == EventType.KeyDown && !s_PressedKeys.Contains(e.keyCode))
            {
                s_PressedKeys.Add(e.keyCode);
            }

            if (type == EventType.KeyUp && s_PressedKeys.Contains(e.keyCode))
            {
                s_PressedKeys.Remove(e.keyCode);
            }
        }

        public static void Clear()
        {
            s_PressedKeys.Clear();
        }

        private void UpdateHotkeys()
        {
            m_DuplicateArrayElement.Update(s_PressedKeys);
            m_DeleteArrayElement.Update(s_PressedKeys);
            m_SnapPosition.Update(s_PressedKeys);
        }

        [SerializeField] private Hotkey m_DuplicateArrayElement;
        [SerializeField] private Hotkey m_DeleteArrayElement;
        [SerializeField] private Hotkey m_SnapPosition;

        public static Hotkey duplicateArrayElement => instance.m_DuplicateArrayElement;
        public static Hotkey deleteArrayElement => instance.m_DeleteArrayElement;
        public static Hotkey snapPosition => instance.m_SnapPosition;
    }
}
