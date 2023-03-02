using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EHandles
{
    [System.Serializable]
    public class Hotkey
    {
        [SerializeField] private KeyCode[] m_Keys;
        [SerializeField] private KeyCode[] m_AlternativeKeys;

        public bool pressed { get; private set; }
        public bool used { get; private set; }

        public void Update(List<KeyCode> pressedKeys)
        {
            pressed = IsPressed(m_Keys, pressedKeys) || IsPressed(m_AlternativeKeys, pressedKeys);
            if (!pressed) used = false;
        }

        private bool IsPressed(KeyCode[] keys, List<KeyCode> pressedKeys)
        {
            if (keys == null || keys.Length == 0) return false;

            foreach (var key in keys)
            {
                if (!pressedKeys.Contains(key)) return false;
            }

            return true;
        }

        public void Use()
        {
            used = true;
            Event.current.Use();
        }

        public static implicit operator bool(Hotkey hotkey)
        {
            return hotkey.pressed && !hotkey.used;
        }
    }
}