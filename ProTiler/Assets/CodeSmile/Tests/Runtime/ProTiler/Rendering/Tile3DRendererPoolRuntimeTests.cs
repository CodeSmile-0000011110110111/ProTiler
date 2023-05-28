// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
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

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		[CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void SetVisibleCoordsMakesTileRendererActive(Int32 visibleTilesCount)
		{
			var pool = GetTileRendererPool();
			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(0));

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);

			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
		}


		[TestCase(Tile3DRendererPool.InitialPoolSize + 1)]
		[TestCase(Tile3DRendererPool.InitialPoolSize * 3)]
		[CreateEmptyScene]
		[CreateGameObject(nameof(TestTile3DRendererPool), typeof(TestTile3DRendererPool))]
		public void IncreasingTileCountGrowsComponentPoolSize(Int32 visibleTilesCount)
		{
			var pool = GetTileRendererPool();

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			pool.SetVisibleCoords(coords, CellSize.one);

			Assert.That(pool.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
		}

		// set position
		// set scale

		private class TestTile3DRendererPool : Tile3DRendererPool
		{
			internal Transform ActiveRenderersFolder => m_ActiveRenderersFolder;
			internal Transform PooledRenderersFolder => m_PooledRenderersFolder;
			internal GameObject TemplateGameObject => m_TemplateGameObject;
			internal ComponentPool<Tile3DRenderer> ComponentPool => m_ComponentPool;
		}
	}
}
