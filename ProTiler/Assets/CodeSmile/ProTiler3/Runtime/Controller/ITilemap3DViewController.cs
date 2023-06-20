// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Events;

namespace CodeSmile.ProTiler3.Controller
{
	public interface ITilemap3DViewController
	{
		public void OnMouseMove(MouseMoveEventData eventData);
		void DisableCursor();
		void EnableCursor();
	}
}
