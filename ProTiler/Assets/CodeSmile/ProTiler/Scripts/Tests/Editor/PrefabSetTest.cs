// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Collections;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class PrefabSetTest
	{
		[Test]
		public void AddAndGetTests()
		{
			var set = new Tile3DPrefabSet();
			var go1 = new GameObject("one");
			var go2 = new GameObject("two");

			Assert.AreEqual(1, set.Count);

			var newIndex = set.TryAddPrefab(go1);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, newIndex);
			newIndex = set.TryAddPrefab(go1);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, newIndex);

			newIndex = set.TryAddPrefab(go2);
			Assert.AreEqual(3, set.Count);
			Assert.AreEqual(2, newIndex);
			newIndex = set.TryAddPrefab(go2);
			Assert.AreEqual(3, set.Count);
			Assert.AreEqual(2, newIndex);

			newIndex = set.TryAddPrefab(go1);
			Assert.AreEqual(3, set.Count);
			Assert.AreEqual(1, newIndex);

			Assert.AreEqual(go1, set[1]);
			Assert.AreEqual(go2, set[2]);
			Assert.AreEqual(go1.GetInstanceID(), set[1].GetInstanceID());
			Assert.AreEqual(go2.GetInstanceID(), set[2].GetInstanceID());

			Object.DestroyImmediate(go1);
			Object.DestroyImmediate(go2);
		}
	}
}
