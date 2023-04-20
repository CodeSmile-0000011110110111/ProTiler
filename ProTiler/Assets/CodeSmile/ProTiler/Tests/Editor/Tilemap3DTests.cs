// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tilemap3DTests
	{
		[SetUp]
		public void SetUp() {}

		[TearDown]
		public void TearDown() {}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapCreation()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.NotNull(tilemap);
			Assert.NotNull(tilemap.Grid);
			Assert.NotNull(tilemap.Chunks);
			Assert.AreEqual(0, tilemap.Chunks.Count);
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 1);
			Assert.Contains(tilemap.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap, tilemap.Grid.GetComponentsInChildren<Tilemap3D>());
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapMultipleCreation()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 2);
			Assert.AreEqual(tilemap1.Grid, tilemap2.Grid);
			Assert.AreEqual(tilemap1.Grid.gameObject, tilemap2.Grid.gameObject);
			Assert.Contains(tilemap1.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap2.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap1, tilemap1.Grid.GetComponentsInChildren<Tilemap3D>());
			Assert.Contains(tilemap2, tilemap1.Grid.GetComponentsInChildren<Tilemap3D>());
			Assert.Contains(tilemap2, tilemap2.Grid.GetComponentsInChildren<Tilemap3D>());
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapCreationUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 1);

			tilemap = null; // reference goes missing upon Undo
			Undo.PerformUndo();
			Assert.Null(Object.FindObjectOfType<Grid3D>());
			Assert.Null(Object.FindObjectOfType<Tilemap3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 0);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 0);

			Undo.PerformRedo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 1);
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapMultipleCreationUndoRedo()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 2);
			Assert.Contains(tilemap1.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap1, tilemap1.Grid.GetComponentsInChildren<Tilemap3D>());
			Assert.Contains(tilemap2, tilemap1.Grid.GetComponentsInChildren<Tilemap3D>());
			Assert.Contains(tilemap2, tilemap2.Grid.GetComponentsInChildren<Tilemap3D>());

			tilemap1 = null; // reference goes missing upon Undo
			tilemap2 = null; // reference goes missing upon Undo
			Undo.PerformUndo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 1);

			Undo.PerformRedo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.IsTrue(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.IsTrue(Object.FindObjectsOfType<Tilemap3D>().Length == 2);
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void SetTilesUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			tilemap.SetChunkSize(new Vector2Int(3, 2));
			Assert.AreEqual(0, tilemap.Chunks.TileCount);

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, Tile3DData.New(tileIndex));
			Assert.AreEqual(1, tilemap.Chunks.TileCount);

			Undo.PerformUndo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.AreEqual(0, tilemap.Chunks.TileCount);

			Undo.PerformRedo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.AreEqual(1, tilemap.Chunks.TileCount);

			var tile = tilemap.GetTile(coord);
			Assert.AreEqual(tileIndex, tile.Index);
		}
	}
}
