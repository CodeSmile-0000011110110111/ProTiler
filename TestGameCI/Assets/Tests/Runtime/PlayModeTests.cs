// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using Scripts;
using UnityEngine;

namespace Tests.Runtime
{
	public class PlayModeTests
	{
		[Test]
		public void PlayModeTestsSimplePasses()
		{
			var go = new GameObject("playmode test", typeof(TestableClass));
			var tc = go.GetComponent<TestableClass>();
			Assert.IsNotNull(tc);
			Assert.IsTrue(tc.ReturnsTrue());
			Assert.IsTrue(true);
			Object.Destroy(go);
			Debug.Log("PlayMode Tests Pass");
		}
	}
}
