// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeSmile.InputState
{
	public class RuntimeInputState : IInputState
	{
		private readonly bool[] m_MouseButtonDown = new bool[(int)MouseButton.Count];
		private readonly bool[] m_MouseButtonUp = new bool[(int)MouseButton.Count];

		public Vector2 MousePosition
		{
			get
			{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
				return UnityEngine.InputSystem.Mouse.current.position.value;
#else // Legacy input system backend enabled
				return Input.mousePosition;
#endif
			}
		}
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
		public bool IsShiftKeyDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
				return UnityEngine.InputSystem.Keyboard.current.shiftKey.isPressed;
#else // Legacy input system backend enabled
				return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
			}
		}
		public bool IsCtrlKeyDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
				return UnityEngine.InputSystem.Keyboard.current.ctrlKey.isPressed;
#else // Legacy input system backend enabled
				return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
#endif
			}
		}
		public bool IsAltKeyDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
				return UnityEngine.InputSystem.Keyboard.current.altKey.isPressed;
#else // Legacy input system backend enabled
				return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
#endif
			}
		}

		public static int ToInt(MouseButton button) => math.clamp((int)button, 0, (int)MouseButton.Count);
		public static MouseButton ToMouseButton(int button) => (MouseButton)math.clamp(button, 0, (int)MouseButton.Count);

		public bool IsButtonDown(MouseButton button) => m_MouseButtonDown[ToInt(button)];

		public bool IsOnlyButtonDown(MouseButton button) => IsButtonDown(button) && MouseButtonsDownCount == 1;

		public bool IsKeyDown(KeyCode keyCode)
		{
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
			return keyCode switch
			{
				KeyCode.Backspace => UnityEngine.InputSystem.Keyboard.current.backspaceKey.isPressed,
				KeyCode.Delete => UnityEngine.InputSystem.Keyboard.current.deleteKey.isPressed,
				KeyCode.Tab => UnityEngine.InputSystem.Keyboard.current.tabKey.isPressed,
				KeyCode.Return => UnityEngine.InputSystem.Keyboard.current.enterKey.isPressed,
				KeyCode.Pause => UnityEngine.InputSystem.Keyboard.current.pauseKey.isPressed,
				KeyCode.Escape => UnityEngine.InputSystem.Keyboard.current.escapeKey.isPressed,
				KeyCode.Space => UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed,
				KeyCode.Keypad0 => UnityEngine.InputSystem.Keyboard.current.numpad0Key.isPressed,
				KeyCode.Keypad1 => UnityEngine.InputSystem.Keyboard.current.numpad1Key.isPressed,
				KeyCode.Keypad2 => UnityEngine.InputSystem.Keyboard.current.numpad2Key.isPressed,
				KeyCode.Keypad3 => UnityEngine.InputSystem.Keyboard.current.numpad3Key.isPressed,
				KeyCode.Keypad4 => UnityEngine.InputSystem.Keyboard.current.numpad4Key.isPressed,
				KeyCode.Keypad5 => UnityEngine.InputSystem.Keyboard.current.numpad5Key.isPressed,
				KeyCode.Keypad6 => UnityEngine.InputSystem.Keyboard.current.numpad6Key.isPressed,
				KeyCode.Keypad7 => UnityEngine.InputSystem.Keyboard.current.numpad7Key.isPressed,
				KeyCode.Keypad8 => UnityEngine.InputSystem.Keyboard.current.numpad8Key.isPressed,
				KeyCode.Keypad9 => UnityEngine.InputSystem.Keyboard.current.numpad9Key.isPressed,
				KeyCode.KeypadPeriod => UnityEngine.InputSystem.Keyboard.current.numpadPeriodKey.isPressed,
				KeyCode.KeypadDivide => UnityEngine.InputSystem.Keyboard.current.numpadDivideKey.isPressed,
				KeyCode.KeypadMultiply => UnityEngine.InputSystem.Keyboard.current.numpadMultiplyKey.isPressed,
				KeyCode.KeypadMinus => UnityEngine.InputSystem.Keyboard.current.numpadMinusKey.isPressed,
				KeyCode.KeypadPlus => UnityEngine.InputSystem.Keyboard.current.numpadPlusKey.isPressed,
				KeyCode.KeypadEnter => UnityEngine.InputSystem.Keyboard.current.numpadEnterKey.isPressed,
				KeyCode.KeypadEquals => UnityEngine.InputSystem.Keyboard.current.numpadEqualsKey.isPressed,
				KeyCode.UpArrow => UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed,
				KeyCode.DownArrow => UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed,
				KeyCode.RightArrow => UnityEngine.InputSystem.Keyboard.current.rightArrowKey.isPressed,
				KeyCode.LeftArrow => UnityEngine.InputSystem.Keyboard.current.leftArrowKey.isPressed,
				KeyCode.Insert => UnityEngine.InputSystem.Keyboard.current.insertKey.isPressed,
				KeyCode.Home => UnityEngine.InputSystem.Keyboard.current.homeKey.isPressed,
				KeyCode.End => UnityEngine.InputSystem.Keyboard.current.endKey.isPressed,
				KeyCode.PageUp => UnityEngine.InputSystem.Keyboard.current.pageUpKey.isPressed,
				KeyCode.PageDown => UnityEngine.InputSystem.Keyboard.current.pageDownKey.isPressed,
				KeyCode.F1 => UnityEngine.InputSystem.Keyboard.current.f1Key.isPressed,
				KeyCode.F2 => UnityEngine.InputSystem.Keyboard.current.f2Key.isPressed,
				KeyCode.F3 => UnityEngine.InputSystem.Keyboard.current.f3Key.isPressed,
				KeyCode.F4 => UnityEngine.InputSystem.Keyboard.current.f4Key.isPressed,
				KeyCode.F5 => UnityEngine.InputSystem.Keyboard.current.f5Key.isPressed,
				KeyCode.F6 => UnityEngine.InputSystem.Keyboard.current.f6Key.isPressed,
				KeyCode.F7 => UnityEngine.InputSystem.Keyboard.current.f7Key.isPressed,
				KeyCode.F8 => UnityEngine.InputSystem.Keyboard.current.f8Key.isPressed,
				KeyCode.F9 => UnityEngine.InputSystem.Keyboard.current.f9Key.isPressed,
				KeyCode.F10 => UnityEngine.InputSystem.Keyboard.current.f10Key.isPressed,
				KeyCode.F11 => UnityEngine.InputSystem.Keyboard.current.f11Key.isPressed,
				KeyCode.F12 => UnityEngine.InputSystem.Keyboard.current.f12Key.isPressed,
				KeyCode.Alpha0 => UnityEngine.InputSystem.Keyboard.current.digit0Key.isPressed,
				KeyCode.Alpha1 => UnityEngine.InputSystem.Keyboard.current.digit1Key.isPressed,
				KeyCode.Alpha2 => UnityEngine.InputSystem.Keyboard.current.digit2Key.isPressed,
				KeyCode.Alpha3 => UnityEngine.InputSystem.Keyboard.current.digit3Key.isPressed,
				KeyCode.Alpha4 => UnityEngine.InputSystem.Keyboard.current.digit4Key.isPressed,
				KeyCode.Alpha5 => UnityEngine.InputSystem.Keyboard.current.digit5Key.isPressed,
				KeyCode.Alpha6 => UnityEngine.InputSystem.Keyboard.current.digit6Key.isPressed,
				KeyCode.Alpha7 => UnityEngine.InputSystem.Keyboard.current.digit7Key.isPressed,
				KeyCode.Alpha8 => UnityEngine.InputSystem.Keyboard.current.digit8Key.isPressed,
				KeyCode.Alpha9 => UnityEngine.InputSystem.Keyboard.current.digit9Key.isPressed,
				//case KeyCode.Exclaim: break;
				//case KeyCode.DoubleQuote: return UnityEngine.InputSystem.Keyboard.current.quoteKey.isPressed;
				//case KeyCode.Hash: break;
				//case KeyCode.Dollar: break;
				//case KeyCode.Percent: break;
				//case KeyCode.Ampersand: break;
				//case KeyCode.Quote: return UnityEngine.InputSystem.Keyboard.current.backquoteKey.isPressed;
				//case KeyCode.LeftParen: break;
				//case KeyCode.RightParen: break;
				//case KeyCode.Asterisk: break;
				//case KeyCode.Plus: break;
				KeyCode.Comma => UnityEngine.InputSystem.Keyboard.current.commaKey.isPressed,
				KeyCode.Minus => UnityEngine.InputSystem.Keyboard.current.minusKey.isPressed,
				KeyCode.Period => UnityEngine.InputSystem.Keyboard.current.periodKey.isPressed,
				KeyCode.Slash => UnityEngine.InputSystem.Keyboard.current.slashKey.isPressed,
				//case KeyCode.Colon: break;
				KeyCode.Semicolon => UnityEngine.InputSystem.Keyboard.current.semicolonKey.isPressed,
				//case KeyCode.Less: break;
				KeyCode.Equals => UnityEngine.InputSystem.Keyboard.current.equalsKey.isPressed,
				//case KeyCode.Greater: break;
				//case KeyCode.Question: break;
				//case KeyCode.At: break;
				KeyCode.LeftBracket => UnityEngine.InputSystem.Keyboard.current.leftBracketKey.isPressed,
				KeyCode.Backslash => UnityEngine.InputSystem.Keyboard.current.backslashKey.isPressed,
				KeyCode.RightBracket => UnityEngine.InputSystem.Keyboard.current.rightBracketKey.isPressed,
				//case KeyCode.Caret: break;
				//case KeyCode.Underscore: break;
				KeyCode.BackQuote => UnityEngine.InputSystem.Keyboard.current.backquoteKey.isPressed,
				KeyCode.A => UnityEngine.InputSystem.Keyboard.current.aKey.isPressed,
				KeyCode.B => UnityEngine.InputSystem.Keyboard.current.bKey.isPressed,
				KeyCode.C => UnityEngine.InputSystem.Keyboard.current.cKey.isPressed,
				KeyCode.D => UnityEngine.InputSystem.Keyboard.current.dKey.isPressed,
				KeyCode.E => UnityEngine.InputSystem.Keyboard.current.eKey.isPressed,
				KeyCode.F => UnityEngine.InputSystem.Keyboard.current.fKey.isPressed,
				KeyCode.G => UnityEngine.InputSystem.Keyboard.current.gKey.isPressed,
				KeyCode.H => UnityEngine.InputSystem.Keyboard.current.hKey.isPressed,
				KeyCode.I => UnityEngine.InputSystem.Keyboard.current.iKey.isPressed,
				KeyCode.J => UnityEngine.InputSystem.Keyboard.current.jKey.isPressed,
				KeyCode.K => UnityEngine.InputSystem.Keyboard.current.kKey.isPressed,
				KeyCode.L => UnityEngine.InputSystem.Keyboard.current.lKey.isPressed,
				KeyCode.M => UnityEngine.InputSystem.Keyboard.current.mKey.isPressed,
				KeyCode.N => UnityEngine.InputSystem.Keyboard.current.nKey.isPressed,
				KeyCode.O => UnityEngine.InputSystem.Keyboard.current.oKey.isPressed,
				KeyCode.P => UnityEngine.InputSystem.Keyboard.current.pKey.isPressed,
				KeyCode.Q => UnityEngine.InputSystem.Keyboard.current.qKey.isPressed,
				KeyCode.R => UnityEngine.InputSystem.Keyboard.current.rKey.isPressed,
				KeyCode.S => UnityEngine.InputSystem.Keyboard.current.sKey.isPressed,
				KeyCode.T => UnityEngine.InputSystem.Keyboard.current.tKey.isPressed,
				KeyCode.U => UnityEngine.InputSystem.Keyboard.current.uKey.isPressed,
				KeyCode.V => UnityEngine.InputSystem.Keyboard.current.vKey.isPressed,
				KeyCode.W => UnityEngine.InputSystem.Keyboard.current.wKey.isPressed,
				KeyCode.X => UnityEngine.InputSystem.Keyboard.current.xKey.isPressed,
				KeyCode.Y => UnityEngine.InputSystem.Keyboard.current.yKey.isPressed,
				KeyCode.Z => UnityEngine.InputSystem.Keyboard.current.zKey.isPressed,
				//case KeyCode.LeftCurlyBracket: break;
				//case KeyCode.Pipe: break;
				//case KeyCode.RightCurlyBracket: break;
				//case KeyCode.Tilde: break;
				KeyCode.Numlock => UnityEngine.InputSystem.Keyboard.current.numLockKey.isPressed,
				KeyCode.CapsLock => UnityEngine.InputSystem.Keyboard.current.capsLockKey.isPressed,
				KeyCode.ScrollLock => UnityEngine.InputSystem.Keyboard.current.scrollLockKey.isPressed,
				KeyCode.RightShift => UnityEngine.InputSystem.Keyboard.current.rightShiftKey.isPressed,
				KeyCode.LeftShift => UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed,
				KeyCode.RightControl => UnityEngine.InputSystem.Keyboard.current.rightCtrlKey.isPressed,
				KeyCode.LeftControl => UnityEngine.InputSystem.Keyboard.current.leftCtrlKey.isPressed,
				KeyCode.RightAlt => UnityEngine.InputSystem.Keyboard.current.rightAltKey.isPressed,
				KeyCode.LeftAlt => UnityEngine.InputSystem.Keyboard.current.leftAltKey.isPressed,
				KeyCode.LeftMeta => UnityEngine.InputSystem.Keyboard.current.leftMetaKey.isPressed,
				KeyCode.LeftWindows => UnityEngine.InputSystem.Keyboard.current.leftWindowsKey.isPressed,
				KeyCode.RightMeta => UnityEngine.InputSystem.Keyboard.current.rightMetaKey.isPressed,
				KeyCode.RightWindows => UnityEngine.InputSystem.Keyboard.current.rightWindowsKey.isPressed,
				KeyCode.AltGr => UnityEngine.InputSystem.Keyboard.current.rightAltKey.isPressed,
				//case KeyCode.Help: break;
				KeyCode.Print => UnityEngine.InputSystem.Keyboard.current.printScreenKey.isPressed,
				//case KeyCode.SysReq: break;
				//case KeyCode.Break: break;
				KeyCode.Menu => UnityEngine.InputSystem.Keyboard.current.contextMenuKey.isPressed,
				_ => throw new ArgumentOutOfRangeException(nameof(keyCode), keyCode, 
					"legacy KeyCode to new InputSystem mapping may be incomplete, add key to switch if needed")
			};
#else
			return Input.GetKey(keyCode);
#endif
		}

		public void UpdateInputStates()
		{
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
			var anyKeyDown = Keyboard.current.anyKey.wasPressedThisFrame;
			if (anyKeyDown)
				throw new NotImplementedException("any key down new input system (runtime support)");
#endif
#if ENABLE_LEGACY_INPUT_MANAGER // Legacy input system backend enabled
			if (Input.anyKeyDown)
				throw new NotImplementedException("any key down legacy input system (runtime support)");
#endif

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // New input system backends are enabled.
			var mouse = UnityEngine.InputSystem.Mouse.current;
			m_MouseButtonDown[0] = mouse.leftButton.wasPressedThisFrame;
			m_MouseButtonDown[1] = mouse.rightButton.wasPressedThisFrame;
			m_MouseButtonDown[2] = mouse.middleButton.wasPressedThisFrame;
			m_MouseButtonDown[3] = mouse.backButton.wasPressedThisFrame;
			m_MouseButtonDown[4] = mouse.forwardButton.wasPressedThisFrame;
			m_MouseButtonUp[0] = mouse.leftButton.wasReleasedThisFrame;
			m_MouseButtonUp[1] = mouse.rightButton.wasReleasedThisFrame;
			m_MouseButtonUp[2] = mouse.middleButton.wasReleasedThisFrame;
			m_MouseButtonUp[3] = mouse.backButton.wasReleasedThisFrame;
			m_MouseButtonUp[4] = mouse.forwardButton.wasReleasedThisFrame;
#endif

			for (var button = 0; button < (int)MouseButton.Count; button++)
			{
#if ENABLE_LEGACY_INPUT_MANAGER // Legacy input system backend enabled
				m_MouseButtonDown[button] = Input.GetMouseButtonDown(button);
				m_MouseButtonUp[button] = Input.GetMouseButtonUp(button);
				var scrollDelta = Input.mouseScrollDelta.y;
#endif

				if (m_MouseButtonDown[button])
					OnMouseButtonDown?.Invoke((MouseButton)button);
				if (m_MouseButtonUp[button])
					OnMouseButtonUp?.Invoke((MouseButton)button);
				if (scrollDelta != 0f)
					OnScrollWheel?.Invoke(this, scrollDelta);
			}
		}
#pragma warning disable 0067
		public event Action<MouseButton> OnMouseButtonDown;
		public event Action<MouseButton> OnMouseButtonUp;
		public event Action<KeyCode> OnKeyDown;
		public event Action<KeyCode> OnKeyUp;
		public event Action<IInputState, float> OnScrollWheel;
#pragma warning restore 0067
	}
}