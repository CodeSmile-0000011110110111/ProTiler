// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.InputState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Editor.InputState
{
	/*
	public sealed class EditorInputState : IInputState
	{
		public event Action<MouseButton> OnMouseButtonDown;
		public event Action<MouseButton> OnMouseButtonUp;
		public event Action<KeyCode> OnKeyDown;
		public event Action<KeyCode> OnKeyUp;
		public event Action<IInputState, float> OnScrollWheel;

		private readonly bool[] m_MouseButtonDown = new bool[(int)MouseButton.Count];
		private readonly Dictionary<KeyCode, bool> m_IsKeyDown;

		// ReSharper disable once PossibleNullReferenceException
		public Vector2 MousePosition => Event.current.mousePosition;
		public int MouseButtonsDownCount
		{
			get
			{
				var count = 0;
				for (var i = 0; i < (int)MouseButton.Count; i++)
					count += m_MouseButtonDown[i] ? 1 : 0;
				return count;
			}
		}

		public bool IsShiftKeyDown => Event.current.shift;
		public bool IsCtrlKeyDown => Event.current.control;
		public bool IsAltKeyDown => Event.current.alt;

		public EditorInputState()
		{
			m_IsKeyDown = new Dictionary<KeyCode, bool>();
			var keyCodes = Enum.GetValues(typeof(KeyCode));
			foreach (KeyCode keyCode in keyCodes)
			{
				if (m_IsKeyDown.ContainsKey(keyCode) == false)
					m_IsKeyDown.Add(keyCode, false);
			}
		}

		public bool IsButtonDown(MouseButton button) => m_MouseButtonDown[RuntimeInputState.ToInt(button)];

		public bool IsOnlyButtonDown(MouseButton button) => IsButtonDown(button) && MouseButtonsDownCount == 1;

		public void Update()
		{
			// ReSharper disable once PossibleNullReferenceException
			switch (Event.current.type)
			{
				case EventType.TouchDown:
				case EventType.MouseDown:
				{
					var button = RuntimeInputState.ToMouseButton(Event.current.button);
					SetMouseButtonDownState(button, true);
					OnMouseButtonDown?.Invoke(button);
					break;
				}
				case EventType.TouchUp:
				case EventType.MouseUp:
				{
					var button = RuntimeInputState.ToMouseButton(Event.current.button);
					SetMouseButtonDownState(button, false);
					OnMouseButtonUp?.Invoke(button);
					break;
				}
				case EventType.ScrollWheel:
				{
					var delta = Event.current.delta.y;
					OnScrollWheel?.Invoke(this, delta);
					break;
				}
				case EventType.KeyDown:
				{
					var keyCode = Event.current.keyCode;
					SetKeyDownState(keyCode, true);
					OnKeyDown?.Invoke(keyCode);
					break;
				}
				case EventType.KeyUp:
				{
					var keyCode = Event.current.keyCode;
					SetKeyDownState(keyCode, false);
					OnKeyUp?.Invoke(keyCode);
					break;
				}
			}
		}

		public bool IsKeyDown(KeyCode keyCode) => m_IsKeyDown[keyCode];

		private void SetMouseButtonDownState(MouseButton button, bool pressed)
		{
			// ReSharper disable once PossibleNullReferenceException
			if ((int)button >= 0 && button < MouseButton.Count)
				m_MouseButtonDown[(int)button] = pressed;
		}

		// ReSharper disable once PossibleNullReferenceException
		private void SetKeyDownState(KeyCode keyCode, bool pressed) => m_IsKeyDown[keyCode] = pressed;
	}
*/
}
