// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Runtime.ProTiler
{
	public class Tile3DRendererPoolRuntimeTests
	{
		private static TestTile3DRendererPool GetTileRendererPool() => GameObject.Find(nameof(TestTile3DRendererPool))
			.GetComponent<TestTile3DRendererPool>();

		[Test] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void PoolCreatesActiveRenderersFolder()
		{
			var pool = GetTileRendererPool();

			Assert.That(pool.ActiveRenderersFolder != null);
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void PoolCreatesPooledRenderersFolder()
		{
			var pool = GetTileRendererPool();

			Assert.That(pool.PooledRenderersFolder != null);
		}

		[UnityTest] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public IEnumerator DestroyPoolDestroysActiveRenderersFolder()
		{
			var pool = GetTileRendererPool();
			var parent = pool.transform;

			pool.DestroyInAnyMode();
			yield return null;

			Assert.That(parent.Find(Tile3DRendererPool.ActiveRenderersFolderName), Is.Null);
		}

		[UnityTest] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public IEnumerator DestroyPoolDestroysPooledRenderersFolder()
		{
			var pool = GetTileRendererPool();
			var parent = pool.transform;

			pool.DestroyInAnyMode();
			yield return null;

			Assert.That(parent.Find(Tile3DRendererPool.PooledRenderersFolderName), Is.Null);
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void PoolCreatesTemplateGameObject()
		{
			var pool = GetTileRendererPool();

			Assert.That(pool.TemplateGameObject != null);
			Assert.That(pool.TemplateGameObject.GetComponent<Tile3DRenderer>() != null);
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void PoolCreatesComponentPool()
		{
			var pool = GetTileRendererPool();

			Assert.That(pool.ComponentPool != null);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void SetVisibleCoordsMakesTileRendererActive(Int32 visibleTilesCount)
		{
			var pool = GetTileRendererPool();
			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(0));

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);

			var activeRenderersCount = pool.ActiveRenderersFolder.childCount;
			Assert.That(activeRenderersCount, Is.EqualTo(coords.Length));
			if (activeRenderersCount > 0)
				Assert.That(pool.ActiveRenderersFolder.GetChild(0).gameObject.activeInHierarchy);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void SetVisibleCoordsReturnsNonVisibleTileRenderersToPool(Int32 visibleTilesCount)
		{
			var pool = GetTileRendererPool();
			var startingTileCount = visibleTilesCount + 1;
			var coords = new GridCoord[startingTileCount];
			for (var i = 0; i < startingTileCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);
			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
			var lastPooledRendererCount = pool.PooledRenderersFolder.childCount;

			coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);

			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
			Assert.That(pool.PooledRenderersFolder.childCount, Is.EqualTo(lastPooledRendererCount + 1));

			foreach (Transform pooledRenderer in pool.PooledRenderersFolder)
				Assert.That(pooledRenderer.gameObject.activeInHierarchy == false);
		}

		[TestCase(Tile3DRendererPool.InitialPoolSize + 1)]
		[TestCase(Tile3DRendererPool.InitialPoolSize * 3)] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void ComponentPoolGrowsOnDemand(Int32 visibleTilesCount)
		{
			var pool = GetTileRendererPool();

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);

			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
		}

		[TestCase(1, 1, 1)] [TestCase(3f, 4f, 5f)]
		[TestCase(7.89f, 8.88f, 9.87f)] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void TileRenderersConvertGridToWorldPosition(Single cellSizeX, Single cellSizeY, Single cellSizeZ)
		{
			var pool = GetTileRendererPool();
			var cellSize = new CellSize(cellSizeX, cellSizeY, cellSizeZ);
			var visibleTilesCount = 3;

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, cellSize);

			for (var i = 0; i < visibleTilesCount; i++)
			{
				var tileRenderer = pool.ActiveRenderersFolder.GetChild(i);
				Assert.That(tileRenderer.position, Is.EqualTo(Grid3DUtility.ToWorldPos(coords[i], cellSize)));
			}
		}

		[TestCase(1, 1, 1)] [TestCase(3f, 4f, 5f)]
		[TestCase(7.89f, 8.88f, 9.87f)] [CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void TileRenderersConvertCellSizeToScale(Single cellSizeX, Single cellSizeY, Single cellSizeZ)
		{
			var pool = GetTileRendererPool();
			var cellSize = new CellSize(cellSizeX, cellSizeY, cellSizeZ);
			var visibleTilesCount = 3;

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, cellSize);

			for (var i = 0; i < visibleTilesCount; i++)
			{
				var tileRenderer = pool.ActiveRenderersFolder.GetChild(i);
				Assert.That(tileRenderer.localScale, Is.EqualTo(cellSize));
			}
		}

		private class TestTile3DRendererPool : Tile3DRendererPool
		{
			internal Transform ActiveRenderersFolder => m_ActiveRenderersFolder;
			internal Transform PooledRenderersFolder => m_PooledRenderersFolder;
			internal GameObject TemplateGameObject => m_TemplateGameObject;
			internal ComponentPool<Tile3DRenderer> ComponentPool => m_ComponentPool;
		}
	}
}
