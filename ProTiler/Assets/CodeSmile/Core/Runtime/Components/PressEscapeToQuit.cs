// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Components
{
	[ExcludeFromCodeCoverage]
	public class PressEscapeToQuit : MonoBehaviour
	{
		[ExcludeFromCodeCoverage]
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}
	}
}
