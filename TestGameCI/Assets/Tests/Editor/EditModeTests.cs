// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using Scripts;
using UnityEngine;

namespace Tests.Editor
{
	public class EditModeTests
	{
		[Test]
		public void EditModeTestsSimplePasses()
		{
			var go = new GameObject("playmode test", typeof(TestableClass));
			var tc = go.GetComponent<TestableClass>();
			Assert.IsNotNull(tc);
			Assert.IsTrue(tc.ReturnsTrue());
			Assert.IsTrue(true);
			Object.DestroyImmediate(go);
			Debug.Log("EditMode Tests Pass");
		}
	}
}
