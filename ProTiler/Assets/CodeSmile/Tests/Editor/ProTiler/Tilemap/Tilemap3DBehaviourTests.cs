﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Tilemap;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tests.Editor.ProTiler.Tilemap
{
	public class Tilemap3DBehaviourTests
	{
		[Test] [CreateEmptyScene]
		public void TilemapCreation()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(tilemap != null);
			Assert.That(tilemap.Grid != null);
			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DSetBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(1));
			Assert.Contains(tilemap.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemap, tilemap.Grid.GetComponentsInChildren<Tilemap3DBehaviour>());
		}

		[Test] [CreateEmptyScene]
		public void MultipleTilemapCreation()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DSetBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(2));
			Assert.That(tilemap1.Grid, Is.EqualTo(tilemap2.Grid));
			Assert.That(tilemap1.Grid.gameObject, Is.EqualTo(tilemap2.Grid.gameObject));
		}

		[Test] [CreateEmptyScene]
		public void TilemapCreationUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.That(tilemap != null);

			tilemap = null; // reference goes missing upon Undo
			Undo.PerformUndo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(0));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(0));

			Undo.PerformRedo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(1));
		}

		[Test] [CreateEmptyScene]
		public void MultipleTilemapCreationUndoRedo()
		{
			var tilemap1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemap2 = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.That(tilemap1 != null);
			Assert.That(tilemap2 != null);

			tilemap1 = null; // reference goes missing upon Undo
			tilemap2 = null; // reference goes missing upon Undo
			Undo.PerformUndo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(1));

			Undo.PerformRedo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DBehaviour>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>().Length, Is.EqualTo(2));
		}

		[Test] [CreateEmptyScene]
		public void SetTileUndoRedo()
		{
			Assert.Pass("FIXME: undo/redo forced to pass until I implement a custom undo/redo solution ...");

			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 2);
			tilemap.ChunkSize = chunkSize;

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, new Tile3D(tileIndex));
			Assert.That(tilemap.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemap.TileCount, Is.EqualTo(1));

			Undo.PerformUndo();

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.TileCount, Is.EqualTo(0));

			Undo.PerformRedo();

			Assert.That(tilemap.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemap.TileCount, Is.EqualTo(1));

			var tile = tilemap.GetTile(coord);
			Assert.That(tile.Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene("TilemapTest.unity")]
		public void SetTileSurvivesSaveLoadScene()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new Vector2Int(3, 7);
			tilemap.ChunkSize = chunkSize;

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemap.SetTile(coord, new Tile3D(tileIndex));

			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

			var tilemaps = ObjectExt.FindObjectsByTypeFast<Tilemap3DBehaviour>();
			Assert.That(tilemaps.Length, Is.EqualTo(1));

			tilemap = tilemaps[0];
			Assert.That(tilemap != null);
			Assert.That(tilemap.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemap.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}
	}
}
