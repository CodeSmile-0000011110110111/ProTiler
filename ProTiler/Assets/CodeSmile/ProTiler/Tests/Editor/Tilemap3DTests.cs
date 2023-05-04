// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.ProTiler.Tests.Editor
{
	public class Tilemap3DTests
	{
		[Test] [EmptyScene]
		public void TilemapCreation()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(tilemap != null);
			Assert.That(tilemap.Grid != null);
			Assert.That(tilemap.Chunks != null);
			Assert.That(tilemap.Chunks.Count == 0);
			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tile3DSet>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 1);
			Assert.Contains(tilemap.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap, tilemap.Grid.GetComponentsInChildren<Tilemap3D>());
		}

		[Test] [EmptyScene]
		public void MultipleTilemapCreation()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tile3DSet>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 2);
			Assert.That(tilemap1.Grid, Is.EqualTo(tilemap2.Grid));
			Assert.That(tilemap1.Grid.gameObject, Is.EqualTo(tilemap2.Grid.gameObject));
		}

		[Test] [EmptyScene]
		public void TilemapCreationUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.That(tilemap != null);

			tilemap = null; // reference goes missing upon Undo
			Undo.PerformUndo();

			var rootObject = SceneManager.GetActiveScene().GetRootGameObjects();
			Assert.That(Object.FindObjectOfType<Grid3D>() == null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() == null);
			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 0);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 0);

			Undo.PerformRedo();

			Assert.That(Object.FindObjectOfType<Grid3D>() != null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() != null);
			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 1);
		}

		[Test] [EmptyScene]
		public void MultipleTilemapCreationUndoRedo()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.That(tilemap1 != null);
			Assert.That(tilemap2 != null);

			tilemap1 = null; // reference goes missing upon Undo
			tilemap2 = null; // reference goes missing upon Undo
			Undo.PerformUndo();

			Assert.That(Object.FindObjectOfType<Grid3D>() != null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() != null);
			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 1);

			Undo.PerformRedo();

			Assert.That(Object.FindObjectOfType<Grid3D>() != null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() != null);
			Assert.That(Object.FindObjectsOfType<Grid3D>().Length == 1);
			Assert.That(Object.FindObjectsOfType<Tilemap3D>().Length == 2);
		}

		[Test] [EmptyScene]
		public void SetTileUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 2);
			tilemap.ChunkSize = chunkSize;
			Assert.That(tilemap.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemap.Chunks.TileCount == 0);

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, Tile3D.New(tileIndex));
			Assert.That(tilemap.Chunks.TileCount == 1);

			Undo.PerformUndo();

			Assert.That(Object.FindObjectOfType<Grid3D>() != null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() != null);
			Assert.That(tilemap.Chunks.TileCount == 0);

			Undo.PerformRedo();

			Assert.That(Object.FindObjectOfType<Grid3D>() != null);
			Assert.That(Object.FindObjectOfType<Tilemap3D>() != null);
			Assert.That(tilemap.Chunks.TileCount == 1);

			var tile = tilemap.GetTile(coord);
			Assert.That(tile.Index == tileIndex);
		}

		[Test] [EmptyScene("TilemapTest.unity")]
		public void SetTileSurvivesSaveLoadScene()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 7);
			tilemap.ChunkSize = chunkSize;
			Assert.That(tilemap.ChunkSize, Is.EqualTo(chunkSize));

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, Tile3D.New(tileIndex));
			Assert.That(tilemap.GetTile(coord).Index == tileIndex);

			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

			var tilemaps = Object.FindObjectsByType<Tilemap3D>(FindObjectsSortMode.None);
			Assert.That(tilemaps != null);
			Assert.That(tilemaps.Length == 1);

			tilemap = tilemaps[0];
			Assert.That(tilemap != null);
			Assert.That(tilemap.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemap.GetTile(coord).Index == tileIndex);
		}
	}
}
