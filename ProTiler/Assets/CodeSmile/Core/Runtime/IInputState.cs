// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile
{
	public interface IInputState
	{
		public Vector2 MousePosition { get; }
		public int MouseButtonsDownCount { get; }
		bool IsShiftKeyDown { get; }
		bool IsCtrlKeyDown { get; }
		bool IsAltKeyDown { get; }
		public bool IsButtonDown(MouseButton button);
		public bool IsOnlyButtonDown(MouseButton button);
		public bool IsKeyDown(KeyCode keyCode);

		public void Update();
	}
}