// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using CodeSmile.Tile;
using NUnit.Framework;
using UnityEngine;

public class ObjectPoolTests
{
	private GameObject m_Prefab;
	private Transform m_Parent;

	[SetUp]
	public void SetUp()
	{
		m_Prefab = new GameObject("test");
		m_Prefab.AddComponent<TileProxy>();
		m_Parent = new GameObject("parent").transform;
	}

	[TearDown]
	public void TearDown()
	{
		m_Prefab.DestroyInAnyMode();
		m_Parent.gameObject.DestroyInAnyMode();
	}

	[Test]
	public void CreateUpdateDisposeTest()
	{
		var poolSize = 100;
		var pool = new ObjectPool<TileProxy>(m_Prefab, m_Parent, poolSize);
		Assert.AreEqual(poolSize, pool.Count);

		poolSize += 10;
		pool.UpdatePoolSize(poolSize);
		Assert.AreEqual(poolSize, pool.Count);

		poolSize -= 30;
		pool.UpdatePoolSize(poolSize);
		Assert.AreEqual(poolSize, pool.Count);

		pool.Dispose();
		Assert.AreEqual(0, pool.Count);
	}
}