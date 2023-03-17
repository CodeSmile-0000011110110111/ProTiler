// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	public sealed partial class EditorInputState
	{
		private readonly Dictionary<MouseButton, bool> m_ButtonPressed;
		private readonly Dictionary<KeyCode, bool> m_KeyPressed;
		private bool m_TouchDown;

		public bool IsTouchDown => m_TouchDown;

		public EditorInputState()
		{
			m_ButtonPressed = new Dictionary<MouseButton, bool>
			{
				{ MouseButton.LeftMouse, false },
				{ MouseButton.MiddleMouse, false },
				{ MouseButton.RightMouse, false },
			};

			m_KeyPressed = new Dictionary<KeyCode, bool>();
			var keyCodes = Enum.GetValues(typeof(KeyCode));
			foreach (KeyCode keyCode in keyCodes)
			{
				if (m_KeyPressed.ContainsKey(keyCode) == false)
					m_KeyPressed.Add(keyCode, false);
			}
		}

		public bool IsButtonDown(MouseButton button) => m_ButtonPressed[button];
		public bool IsKeyDown(KeyCode keyCode) => m_KeyPressed[keyCode];

		private void SetKeyPressed(bool down) => m_KeyPressed[Event.current.keyCode] = down;

		private void SetButtonPressed(bool down)
		{
			if (Event.current.button < m_ButtonPressed.Count)
				m_ButtonPressed[MouseButtonFromEvent()] = down;
		}

		private MouseButton MouseButtonFromEvent() => (MouseButton)Mathf.Clamp(Event.current.button, 0, 2);

		private void SetTouchPressed(bool down) => m_TouchDown = down;

		public void Update()
		{
			switch (Event.current.type)
			{
				case EventType.TouchDown:
					SetTouchPressed(true);
					break;
				case EventType.TouchUp:
					SetTouchPressed(false);
					break;
				case EventType.MouseDown:
					SetButtonPressed(true);
					break;
				case EventType.MouseUp:
					SetButtonPressed(false);
					break;
				case EventType.KeyDown:
					SetKeyPressed(true);
					break;
				case EventType.KeyUp:
					SetKeyPressed(false);
					break;
			}
		}
	}
}