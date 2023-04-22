// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tilemap3DTests
	{
		[Test]
		[EmptyScene]
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
		[EmptyScene]
		public void MultipleTilemapCreation()
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
		[EmptyScene]
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
		[EmptyScene]
		public void MultipleTilemapCreationUndoRedo()
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
		[EmptyScene]
		public void SetTileUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 2);
			tilemap.ChunkSize = chunkSize;
			Assert.AreEqual(chunkSize, tilemap.ChunkSize);
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

		[Test]
		[EmptyScene("TilemapTest.unity")]
		public void SetTileSurvivesSaveLoadScene()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 7);
			tilemap.ChunkSize = chunkSize;
			Assert.AreEqual(chunkSize, tilemap.ChunkSize);

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, Tile3DData.New(tileIndex));
			Assert.AreEqual(tileIndex, tilemap.GetTile(coord).Index);

			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

			var tilemaps = Object.FindObjectsOfType<Tilemap3D>();
			Assert.NotNull(tilemaps);
			Assert.AreEqual(1, tilemaps.Length);

			tilemap = tilemaps[0];
			Assert.NotNull(tilemap);
			Assert.AreEqual(chunkSize, tilemap.ChunkSize);
			Assert.AreEqual(tileIndex, tilemap.GetTile(coord).Index);
		}
	}
}
