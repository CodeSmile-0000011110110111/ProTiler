// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Pooling;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Core.Tests.Editor
{
	public class ComponentPoolTests
	{
		private const string PrefabAssetPath = "Assets/ComponentPoolTestPrefab.prefab";

		private GameObject m_Prefab;
		private GameObject m_Parent;

		[SetUp]
		public void SetUp()
		{
			m_Parent = new GameObject("parent");
			var prefab = new GameObject("prefab");
			m_Prefab = PrefabUtility.SaveAsPrefabAsset(prefab, PrefabAssetPath, out var success);
			Assert.IsTrue(success);
			Assert.NotNull(m_Prefab);
		}

		[TearDown]
		public void TearDown()
		{
			if (m_Prefab != null)
				Assert.IsTrue(AssetDatabase.DeleteAsset(PrefabAssetPath));
			if (m_Parent != null)
				Object.DestroyImmediate(m_Parent);
		}

		[Test]
		public void NewThrowsOnInvalidParams()
		{
			Assert.Throws<ArgumentNullException>(() => { new ComponentPool<Transform>(null, m_Parent, 1); });
			Assert.Throws<ArgumentNullException>(() => { new ComponentPool<Transform>(m_Prefab, null, 2); });
			Assert.Throws<ArgumentException>(() => { new ComponentPool<Transform>(m_Parent, m_Parent, 3); });
			Assert.Throws<ArgumentException>(() => { new ComponentPool<Transform>(m_Prefab, m_Prefab, 4); });
		}

		[Test]
		public void CreateAndDispose()
		{
			var poolSize = 10;

			using (var pool = new ComponentPool<Transform>(m_Prefab, m_Parent, poolSize))
			{
				Assert.AreEqual(poolSize, pool.Count);
				Assert.AreEqual(poolSize, pool.AllInstances.Count);
				Assert.AreEqual(poolSize, m_Parent.transform.childCount);
			}

			Assert.AreEqual(0, m_Parent.transform.childCount);
		}

	}
}
