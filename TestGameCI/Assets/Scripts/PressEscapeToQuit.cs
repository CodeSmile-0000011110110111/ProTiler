// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile
{
	public class PressEscapeToQuit : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}
	}
}