// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Pooling;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Tests.Editor
{
	public class ComponentPoolTests
	{
		[Test] [EmptyScene()][CreateGameObject("Parent")]
		public void NewThrowsOnInvalidParams()
		{
			var instance = GameObject.Find("Parent");
			var prefab = TestAssets.LoadTestPrefab();

			Assert.Throws<ArgumentNullException>(() => { new ComponentPool<Transform>(null, instance, 1); });
			Assert.Throws<ArgumentNullException>(() => { new ComponentPool<Transform>(prefab, null, 2); });
			Assert.Throws<ArgumentException>(() => { new ComponentPool<Transform>(instance, instance, 3); });
			Assert.Throws<ArgumentException>(() => { new ComponentPool<Transform>(prefab, prefab, 4); });
		}


		[Test] [EmptyScene()][CreateGameObject("Parent")]
		public void PoolSizeZero()
		{
			var parent = GameObject.Find("Parent");
			var prefab = TestAssets.LoadTestPrefab();
			var poolSize = 0;

			using (var pool = new ComponentPool<Transform>(prefab, parent, poolSize))
			{
			}
		}

		[Test] [EmptyScene()][CreateGameObject("Parent")]
		public void CreateAndDispose()
		{
			var parent = GameObject.Find("Parent");
			var prefab = TestAssets.LoadTestPrefab();
			var poolSize = 0;

			using (var pool = new ComponentPool<Transform>(prefab, parent, poolSize))
			{
				Assert.That(poolSize == pool.Count);
				Assert.That(poolSize == pool.AllInstances.Count);
				Assert.That(poolSize == parent.transform.childCount);
			}

			Assert.AreEqual(0, parent.transform.childCount);
		}

		[Test] [EmptyScene()][CreateGameObject("Parent")]
		public void TakeAndReturnPooledObject()
		{
			var parent = GameObject.Find("Parent");
			var prefab = TestAssets.LoadTestPrefab();
			var poolSize = 10;

			using (var pool = new ComponentPool<Transform>(prefab, parent, poolSize))
			{
				Assert.AreEqual(poolSize, pool.Count);
				Assert.AreEqual(poolSize, pool.AllInstances.Count);
				Assert.AreEqual(poolSize, parent.transform.childCount);
			}

			Assert.AreEqual(0, parent.transform.childCount);
		}

	}
}
