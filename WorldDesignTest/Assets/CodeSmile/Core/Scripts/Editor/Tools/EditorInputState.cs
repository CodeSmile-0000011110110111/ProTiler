// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	public sealed class EditorInputState
	{
		private readonly Dictionary<MouseButton, bool> m_IsButtonPressed;
		private readonly Dictionary<KeyCode, bool> m_IsKeyPressed;
		private Dictionary<MouseButton, bool> m_WasButtonPressed;
		private Dictionary<KeyCode, bool> m_WasKeyPressed;
		private bool m_IsTouchDown;
		private bool m_WasTouchDown;

		public bool IsTouchDown => m_IsTouchDown;
		public bool WasTouchDown => m_WasTouchDown;
		public bool IsButtonOrTouchDown => IsButtonDown(MouseButton.LeftMouse) || m_IsTouchDown;
		public bool WasButtonOrTouchDown => WasButtonDown(MouseButton.LeftMouse) || m_WasTouchDown;

		public EditorInputState()
		{
			m_IsButtonPressed = new Dictionary<MouseButton, bool>
			{
				{ MouseButton.LeftMouse, false },
				{ MouseButton.MiddleMouse, false },
				{ MouseButton.RightMouse, false },
			};

			m_IsKeyPressed = new Dictionary<KeyCode, bool>();
			var keyCodes = Enum.GetValues(typeof(KeyCode));
			foreach (KeyCode keyCode in keyCodes)
			{
				if (m_IsKeyPressed.ContainsKey(keyCode) == false)
					m_IsKeyPressed.Add(keyCode, false);
			}

			UpdatePreviousStates();
		}

		public bool IsButtonDown(MouseButton button) => m_IsButtonPressed[button];
		public bool IsOnlyButtonDown(MouseButton button) => IsButtonDown(button) && ButtonDownCount() == 1;

		public int ButtonDownCount()
		{
			var count = 0;
			foreach (var state in m_IsButtonPressed.Values)
			{
				if (state)
					count++;
			}
			return count;
		}

		public bool IsKeyDown(KeyCode keyCode) => m_IsKeyPressed[keyCode];
		public bool WasButtonDown(MouseButton button) => m_WasButtonPressed[button];
		public bool WasKeyDown(KeyCode keyCode) => m_WasKeyPressed[keyCode];

		private void SetButtonPressed(bool down)
		{
			if (Event.current.button < m_IsButtonPressed.Count)
				m_IsButtonPressed[MouseButtonFromEvent()] = down;
		}

		private void SetKeyPressed(bool down) => m_IsKeyPressed[Event.current.keyCode] = down;

		private MouseButton MouseButtonFromEvent() => (MouseButton)Mathf.Clamp(Event.current.button, 0, 2);

		private void SetTouchPressed(bool down) => m_IsTouchDown = down;

		public void Update()
		{
			UpdatePreviousStates();

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

		private void UpdatePreviousStates()
		{
			m_WasKeyPressed = new Dictionary<KeyCode, bool>(m_IsKeyPressed);
			m_WasButtonPressed = new Dictionary<MouseButton, bool>(m_IsButtonPressed);
			m_WasTouchDown = m_IsTouchDown;
		}
	}
}