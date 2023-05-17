// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Components;
using System;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class MonoBehaviourExt
	{
		public static void WaitForFramesElapsed(this MonoBehaviour self, Int32 framesToWait, Action callback) =>
			self.StartCoroutine(new WaitFramesCoroutine(framesToWait, callback));
	}
}
