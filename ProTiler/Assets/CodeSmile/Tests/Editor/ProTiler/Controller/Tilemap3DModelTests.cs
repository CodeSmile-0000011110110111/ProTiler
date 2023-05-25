// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.Tests.Editor.ProTiler.Controller
{
	public class Tilemap3DModelTests
	{
		[Test] [CreateEmptyScene]
		public void TilemapCreation()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(model != null);
			Assert.That(model.Grid != null);
			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DAssetSet>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(1));
			Assert.Contains(model.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(model, model.Grid.GetComponentsInChildren<Tilemap3DModel>());
		}

		[Test] [CreateEmptyScene]
		public void MultipleTilemapCreation()
		{
			var model1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var model2 = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DAssetSet>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(2));
			Assert.That(model1.Grid, Is.EqualTo(model2.Grid));
			Assert.That(model1.Grid.gameObject, Is.EqualTo(model2.Grid.gameObject));
		}

		[Test] [CreateEmptyScene]
		public void TilemapCreationUndoRedo()
		{
			Tilemap3DCreation.CreateRectangularTilemap3D();

			Debug.Log($"before Undo {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Debug.Log($"after Undo {Undo.GetCurrentGroupName()}");

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(0));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(0));

			Debug.Log($"before Redo {Undo.GetCurrentGroupName()}");
			Undo.PerformRedo();
			Debug.Log($"after Redo {Undo.GetCurrentGroupName()}");

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(1));
		}

		[Test] [CreateEmptyScene]
		public void MultipleTilemapCreationUndoRedo()
		{
			Tilemap3DCreation.CreateRectangularTilemap3D();
			Tilemap3DCreation.CreateRectangularTilemap3D();

			Undo.PerformUndo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(1));

			Undo.PerformRedo();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(2));
		}

		[Test] [CreateEmptyScene]
		public void SetTile()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 1234;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));

			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));
			Assert.That(model.GetLayerCount(new Vector2Int(0, 0)), Is.EqualTo(2));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndo()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 1234;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Debug.Log($"after Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(model.ChunkCount, Is.EqualTo(0));
			Assert.That(model.TileCount, Is.EqualTo(0));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndoRedo()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 12345;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Debug.Log($"after Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformRedo();
			Debug.Log($"after Redo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndoRedoTwice()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 222;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Undo.PerformRedo();
			Undo.PerformUndo();
			Undo.PerformRedo();
			Debug.Log($"after Redo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void CreateTilemapSetTileUndoRedo()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = model.ChunkSize;

			var chunkSize = new ChunkSize(3, 4);
			model.ClearTilemap(chunkSize);

			var tileIndex = 123;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));
			Assert.That(model.ChunkSize, Is.EqualTo(chunkSize));

			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(0));
			Assert.That(model.TileCount, Is.EqualTo(0));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(0));

			Undo.PerformRedo();

			Assert.That(model.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));
			Assert.That(model.GetTile(coord).Index, Is.EqualTo(tileIndex));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenClearTilemap()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			model.SetTile(Vector3Int.zero, new Tile3D(234));
			model.ClearTilemap(new ChunkSize(6, 9));

			Assert.That(model.ChunkCount, Is.EqualTo(0));
			Assert.That(model.TileCount, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene]
		public void ClearTilemapUndoRedoRestoresChunkSize()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = model.ChunkSize;

			var firstChunkSize = new ChunkSize(12, 14);
			model.ClearTilemap(firstChunkSize);
			Assert.That(model.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));

			Undo.PerformRedo();

			Assert.That(model.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void ClearTilemapTwiceUndoRedoRestoresChunkSize()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = model.ChunkSize;

			//Undo.RegisterCompleteObjectUndo(tilemapBehaviour, "test1");

			var firstChunkSize = new ChunkSize(10, 11);
			model.ClearTilemap(firstChunkSize);
			Assert.That(model.ChunkSize, Is.EqualTo(firstChunkSize));

			//Undo.ClearAll();
			//Undo.RegisterCompleteObjectUndo(tilemapBehaviour, "test2");

			var secondChunkSize = new ChunkSize(12, 14);
			model.ClearTilemap(secondChunkSize);
			Assert.That(model.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformRedo();

			Assert.That(model.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void CreateAndClearTilemapUndoRedo()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			var firstChunkSize = new ChunkSize(7, 4);
			model.ClearTilemap(firstChunkSize);

			var secondChunkSize = new ChunkSize(14, 9);
			model.ClearTilemap(secondChunkSize);
			Assert.That(model.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformRedo();

			Assert.That(model.ChunkSize, Is.EqualTo(secondChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void CreateSetTileAndClearTilemapUndoRedo()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = new ChunkSize(8, 5);
			model.ClearTilemap(initialChunkSize);

			model.SetTile(Vector3Int.one, new Tile3D(456));
			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));

			var chunkSize = new ChunkSize(11, 13);
			model.ClearTilemap(chunkSize);
			Assert.That(model.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(0));
			Assert.That(model.TileCount, Is.EqualTo(0));

			Undo.PerformUndo();

			Assert.That(model.ChunkSize, Is.EqualTo(initialChunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(1));
			Assert.That(model.TileCount, Is.EqualTo(1));

			Undo.PerformRedo();

			Assert.That(model.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(model.ChunkCount, Is.EqualTo(0));
			Assert.That(model.TileCount, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene("TilemapTest.unity")]
		public void SetTileSurvivesSaveLoadScene()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new ChunkSize(5, 7);
			model.ChunkSize = chunkSize;

			var tileIndex = 123;
			var coord = Vector3Int.one;
			model.SetTile(coord, new Tile3D(tileIndex));

			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

			var models = ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>();
			Assert.That(models.Length, Is.EqualTo(1));
			Assert.That(models.First() != null);
			Assert.That(models.First().ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(models.First().GetTile(coord).Index, Is.EqualTo(tileIndex));
		}
	}
}
