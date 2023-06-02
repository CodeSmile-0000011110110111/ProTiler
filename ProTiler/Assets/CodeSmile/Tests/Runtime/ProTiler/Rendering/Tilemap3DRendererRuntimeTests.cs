// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using WorldPos = Unity.Mathematics.float3;
using CellSize = Unity.Mathematics.float3;
using ChunkSize = Unity.Mathematics.int2;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Runtime.ProTiler.Rendering
{
	public class Tilemap3DRendererRuntimeTests
	{
		private static IEnumerable CullingTestSource
		{
			get
			{
				yield return new TestCaseData(1, 1).Returns(null);
				yield return new TestCaseData(3, 3).Returns(null);
				yield return new TestCaseData(4, 4).Returns(null);
				yield return new TestCaseData(2, 2).Returns(null);
				yield return new TestCaseData(7, 5).Returns(null);
				yield return new TestCaseData(11, 13).Returns(null);
			}
		}

		private static TestTilemap3DRenderer GetTilemapRenderer()
		{
			var tilemap = Tilemap3DCreation.CreateTilemap3D(CellLayout.Rectangular,
				new[] { typeof(Tilemap3DModel), typeof(TestTilemap3DRenderer) });
			return tilemap.GetComponent<TestTilemap3DRenderer>();
		}

		// [UnityTest] [CreateEmptyScene]
		// public IEnumerator CreateSameAmountOfTileRenderersAsCullingIndicates()
		// {
		// 	var width = 3;
		// 	var length = 4;
		//
		// 	var model = Tilemap3DCreation.CreateRectangularTilemap3D();
		// 	var renderer = model.GetComponent<Tilemap3DRenderer>();
		// 	renderer.Culling = new TestCulling(width, length);
		//
		// 	var rendererFolder = model.transform.Find(Tilemap3DRenderer.ActiveRenderersFolderName);
		// 	Assert.That(rendererFolder != null);
		// 	Assert.That(rendererFolder.childCount, Is.Zero);
		//
		// 	yield return null;
		//
		// 	//Debug.Log(SceneManager.GetActiveScene().DumpAll());
		// 	Assert.That(rendererFolder.childCount, Is.EqualTo(width * length));
		// }

		[Test] [CreateDefaultScene]
		public void PoolCreatesActiveRenderersFolder()
		{
			var renderer = GetTilemapRenderer();

			Assert.That(renderer.ActiveRenderersFolder != null);
		}

		[Test] [CreateDefaultScene]
		public void PoolCreatesPooledRenderersFolder()
		{
			var renderer = GetTilemapRenderer();

			Assert.That(renderer.PooledRenderersFolder != null);
		}

		[UnityTest] [CreateEmptyScene]
		public IEnumerator ClearEmptiesBothActiveAndPooledTileRenderers()
		{
			var renderer = GetTilemapRenderer();
			var startingTileCount = 11;
			var coords = new GridCoord[startingTileCount];
			for (var i = 0; i < startingTileCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));
			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.EqualTo(startingTileCount));

			renderer.ClearTileRenderers();
			renderer.Culling = new ZeroCulling();

			yield return null;

			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.Zero);
			Assert.That(renderer.PooledRenderersFolder.childCount, Is.Zero);
		}

		[UnityTest] [CreateEmptyScene]
		public IEnumerator ClearTilemapEmptiesRendererFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();
			renderer.Culling = new TestCulling(3, 3);
			model.SetTile(new GridCoord(1, 1, 1), new Tile3D(1));

			yield return null;

			var rendererFolder = model.transform.Find(Tilemap3DRenderer.ActiveRenderersFolderName);
			Assert.That(rendererFolder != null);
			Assert.That(rendererFolder.childCount, Is.GreaterThan(0));

			model.ClearTilemap();
			renderer.Culling = new ZeroCulling();

			yield return null;

			rendererFolder = model.transform.Find(Tilemap3DRenderer.ActiveRenderersFolderName);
			Assert.That(rendererFolder != null);
			Assert.That(rendererFolder.childCount, Is.EqualTo(0));
		}

		[UnityTest] [CreateEmptyScene]
		public IEnumerator DestroyRendererEmptiesActiveRenderersFolder()
		{
			var renderer = GetTilemapRenderer();
			var parent = renderer.transform;

			renderer.DestroyInAnyMode();
			yield return null;

			var folder = parent.Find(Tilemap3DRenderer.ActiveRenderersFolderName);
			Assert.That(folder != null);
			Assert.That(folder.transform.childCount, Is.EqualTo(0));
		}

		[UnityTest] [CreateEmptyScene]
		public IEnumerator DestroyRendererEmptiesPooledRenderersFolder()
		{
			var renderer = GetTilemapRenderer();
			var parent = renderer.transform;

			renderer.DestroyInAnyMode();
			yield return null;

			var folder = parent.Find(Tilemap3DRenderer.PooledRenderersFolderName);
			Assert.That(folder != null);
			Assert.That(folder.childCount, Is.EqualTo(0));
		}

		[Test] [CreateDefaultScene]
		public void PoolCreatesTemplateGameObject()
		{
			var renderer = GetTilemapRenderer();

			Assert.That(renderer.TemplateGameObject != null);
			Assert.That(renderer.TemplateGameObject.GetComponent<Tile3DRenderer>() != null);
		}

		[Test] [CreateDefaultScene]
		public void PoolCreatesComponentPool()
		{
			var renderer = GetTilemapRenderer();

			Assert.That(renderer.ComponentPool != null);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)] [CreateDefaultScene]
		public void SetVisibleCoordsMakesTileRendererActive(Int32 visibleTilesCount)
		{
			var renderer = GetTilemapRenderer();
			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.EqualTo(0));

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));

			var activeRenderersCount = renderer.ActiveRenderersFolder.childCount;
			Assert.That(activeRenderersCount, Is.EqualTo(coords.Length));
			if (activeRenderersCount > 0)
				Assert.That(renderer.ActiveRenderersFolder.GetChild(0).gameObject.activeInHierarchy);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)] [CreateDefaultScene]
		public void SetVisibleCoordsMovesNonVisibleTileRenderersToPool(Int32 visibleTilesCount)
		{
			var renderer = GetTilemapRenderer();
			var startingTileCount = visibleTilesCount + 1;
			var coords = new GridCoord[startingTileCount];

			for (var i = 0; i < startingTileCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));
			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
			var lastPooledRendererCount = renderer.PooledRenderersFolder.childCount;

			coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));

			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.EqualTo(coords.Length));
			Assert.That(renderer.PooledRenderersFolder.childCount, Is.EqualTo(lastPooledRendererCount + 1));

			foreach (Transform pooledRenderer in renderer.PooledRenderersFolder)
				Assert.That(pooledRenderer.gameObject.activeInHierarchy == false);
		}

		[TestCase(1)] [TestCase(13)] [CreateDefaultScene]
		public void ComponentPoolGrowsOnDemand(Int32 visibleTilesCount)
		{
			var renderer = GetTilemapRenderer();

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));

			Assert.That(renderer.ActiveRenderersFolder.childCount, Is.EqualTo(visibleTilesCount));
		}

		[TestCase(1)] [TestCase(13)] [CreateDefaultScene]
		public void ActiveRenderersCountDoesNotGrow(Int32 visibleTilesCount)
		{
			var renderer = GetTilemapRenderer();

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));

			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i + 10, i + 20, i + 30);
			renderer.SetVisibleCoords(coords, new CellSize(1, 1, 1));

			Assert.That(renderer.ActiveRenderers.Count, Is.EqualTo(visibleTilesCount));
		}

		[TestCase(1, 1, 1)] [TestCase(3f, 4f, 5f)]
		[TestCase(7.89f, 8.88f, 9.87f)] [CreateDefaultScene]
		public void TileRenderersConvertGridToWorldPosition(Single cellSizeX, Single cellSizeY, Single cellSizeZ)
		{
			var renderer = GetTilemapRenderer();
			var cellSize = new CellSize(cellSizeX, cellSizeY, cellSizeZ);
			var visibleTilesCount = 3;

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, cellSize);

			for (var i = 0; i < visibleTilesCount; i++)
			{
				var tileRenderer = renderer.ActiveRenderersFolder.GetChild(i);
				Assert.That((WorldPos)tileRenderer.position, Is.EqualTo(Grid3DUtility.ToWorldPos(coords[i], cellSize)));
			}
		}

		[TestCase(1, 1, 1)] [TestCase(3f, 4f, 5f)]
		[TestCase(7.89f, 8.88f, 9.87f)] [CreateDefaultScene]
		public void TileRenderersConvertCellSizeToScale(Single cellSizeX, Single cellSizeY, Single cellSizeZ)
		{
			var renderer = GetTilemapRenderer();
			var cellSize = new CellSize(cellSizeX, cellSizeY, cellSizeZ);
			var visibleTilesCount = 3;

			var coords = new GridCoord[visibleTilesCount];
			for (var i = 0; i < visibleTilesCount; i++)
				coords[i] = new GridCoord(i, i, i);
			renderer.SetVisibleCoords(coords, cellSize);

			for (var i = 0; i < visibleTilesCount; i++)
			{
				var tileRenderer = renderer.ActiveRenderersFolder.GetChild(i);
				Assert.That((CellSize)tileRenderer.localScale, Is.EqualTo(cellSize));
			}
		}

		private class TestTilemap3DRenderer : Tilemap3DRenderer
		{
			internal Transform ActiveRenderersFolder => m_ActiveRenderersFolder;
			internal Transform PooledRenderersFolder => m_PooledRenderersFolder;
			internal GameObject TemplateGameObject => m_TemplateGameObject;
			internal ComponentPool<Tile3DRenderer> ComponentPool => m_ComponentPool;
			internal TileRenderers ActiveRenderers => m_ActiveRenderers;
		}

		private class ZeroCulling : Tilemap3DCullingBase
		{
			public override IEnumerable<GridCoord> GetVisibleCoords(ChunkSize chunkSize, CellSize cellSize) =>
				new GridCoord[0];
		}
	}
}
