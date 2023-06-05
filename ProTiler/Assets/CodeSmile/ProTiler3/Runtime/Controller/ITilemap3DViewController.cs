// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Events;

namespace CodeSmile.ProTiler.Controller
{
	public interface ITilemap3DViewController
	{
		public void OnMouseMove(MouseMoveEventData eventData);
		void DisableCursor();
		void EnableCursor();
	}
}
