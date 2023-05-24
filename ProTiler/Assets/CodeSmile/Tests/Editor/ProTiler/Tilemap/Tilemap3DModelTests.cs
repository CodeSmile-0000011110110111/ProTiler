// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tilemap;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.Tests.Editor.ProTiler.Tilemap
{
	public class Tilemap3DModelTests
	{
		[Test] [CreateEmptyScene]
		public void TilemapCreation()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(tilemapBehaviour != null);
			Assert.That(tilemapBehaviour.Grid != null);
			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DAssetController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(1));
			Assert.Contains(tilemapBehaviour.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());
			Assert.Contains(tilemapBehaviour, tilemapBehaviour.Grid.GetComponentsInChildren<Tilemap3DModel>());
		}

		[Test] [CreateEmptyScene]
		public void MultipleTilemapCreation()
		{
			var tilemapBehaviour1 = Tilemap3DCreation.CreateRectangularTilemap3D();
			var tilemapBehaviour2 = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(ObjectExt.FindObjectsByTypeFast<Grid3DController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tile3DAssetController>().Length, Is.EqualTo(1));
			Assert.That(ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>().Length, Is.EqualTo(2));
			Assert.That(tilemapBehaviour1.Grid, Is.EqualTo(tilemapBehaviour2.Grid));
			Assert.That(tilemapBehaviour1.Grid.gameObject, Is.EqualTo(tilemapBehaviour2.Grid.gameObject));
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
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 1234;
			var coord = Vector3Int.one;
			tilemapBehaviour.SetTile(coord, new Tile3D(tileIndex));

			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.GetLayerCount(new Vector2Int(0, 0)), Is.EqualTo(2));
			Assert.That(tilemapBehaviour.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndo()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 1234;
			var coord = Vector3Int.one;
			tilemapBehaviour.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Debug.Log($"after Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(0));
			Assert.That(tilemapBehaviour.GetTile(coord).Index, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndoRedo()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 12345;
			var coord = Vector3Int.one;
			tilemapBehaviour.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Debug.Log($"after Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformRedo();
			Debug.Log($"after Redo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenUndoRedoTwice()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			var tileIndex = 222;
			var coord = Vector3Int.one;
			tilemapBehaviour.SetTile(coord, new Tile3D(tileIndex));

			Debug.Log($"before Undo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");
			Undo.PerformUndo();
			Undo.PerformRedo();
			Undo.PerformUndo();
			Undo.PerformRedo();
			Debug.Log($"after Redo {Undo.GetCurrentGroup()}: {Undo.GetCurrentGroupName()}");

			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.GetTile(coord).Index, Is.EqualTo(tileIndex));
		}

		[Test] [CreateEmptyScene]
		public void CreateTilemapSetTileUndoRedo()
		{
			var tilemapController = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = tilemapController.ChunkSize;

			var chunkSize = new ChunkSize(3, 4);
			tilemapController.ClearTilemap(chunkSize);

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemapController.SetTile(coord, new Tile3D(tileIndex));
			Assert.That(tilemapController.ChunkSize, Is.EqualTo(chunkSize));

			Undo.PerformUndo();

			Assert.That(tilemapController.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemapController.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemapController.TileCount, Is.EqualTo(0));
			Assert.That(tilemapController.GetTile(coord).Index, Is.EqualTo(0));

			Undo.PerformRedo();

			Assert.That(tilemapController.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemapController.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapController.TileCount, Is.EqualTo(1));
			Assert.That(tilemapController.GetTile(coord).Index, Is.EqualTo(tileIndex));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(tilemapController.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void SetTileThenClearTilemap()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			tilemapBehaviour.SetTile(Vector3Int.zero, new Tile3D(234));
			tilemapBehaviour.ClearTilemap(new ChunkSize(6, 9));

			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene]
		public void ClearTilemapUndoRedoRestoresChunkSize()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = tilemapBehaviour.ChunkSize;

			var firstChunkSize = new ChunkSize(12, 14);
			tilemapBehaviour.ClearTilemap(firstChunkSize);
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(initialChunkSize));

			Undo.PerformRedo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void ClearTilemapTwiceUndoRedoRestoresChunkSize()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = tilemapBehaviour.ChunkSize;

			//Undo.RegisterCompleteObjectUndo(tilemapBehaviour, "test1");

			var firstChunkSize = new ChunkSize(10, 11);
			tilemapBehaviour.ClearTilemap(firstChunkSize);
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(firstChunkSize));

			//Undo.ClearAll();
			//Undo.RegisterCompleteObjectUndo(tilemapBehaviour, "test2");

			var secondChunkSize = new ChunkSize(12, 14);
			tilemapBehaviour.ClearTilemap(secondChunkSize);
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformRedo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();
			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(initialChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void CreateAndClearTilemapUndoRedo()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();

			var firstChunkSize = new ChunkSize(7, 4);
			tilemapBehaviour.ClearTilemap(firstChunkSize);

			var secondChunkSize = new ChunkSize(14, 9);
			tilemapBehaviour.ClearTilemap(secondChunkSize);
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(secondChunkSize));

			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(firstChunkSize));

			Undo.PerformRedo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(secondChunkSize));
		}

		[Test] [CreateEmptyScene]
		public void CreateSetTileAndClearTilemapUndoRedo()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();
			var initialChunkSize = new ChunkSize(8, 5);
			tilemapBehaviour.ClearTilemap(initialChunkSize);

			tilemapBehaviour.SetTile(Vector3Int.one, new Tile3D(456));
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(initialChunkSize));
			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(1));

			var chunkSize = new ChunkSize(11, 13);
			tilemapBehaviour.ClearTilemap(chunkSize);
			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(0));

			Undo.PerformUndo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(initialChunkSize));
			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(1));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(1));

			Undo.PerformRedo();

			Assert.That(tilemapBehaviour.ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemapBehaviour.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemapBehaviour.TileCount, Is.EqualTo(0));
		}

		[Test] [CreateEmptyScene("TilemapTest.unity")]
		public void SetTileSurvivesSaveLoadScene()
		{
			var tilemapBehaviour = Tilemap3DCreation.CreateRectangularTilemap3D();
			var chunkSize = new ChunkSize(5, 7);
			tilemapBehaviour.ChunkSize = chunkSize;

			var tileIndex = 123;
			var coord = Vector3Int.one;
			tilemapBehaviour.SetTile(coord, new Tile3D(tileIndex));

			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

			var tilemapBehaviours = ObjectExt.FindObjectsByTypeFast<Tilemap3DModel>();
			Assert.That(tilemapBehaviours.Length, Is.EqualTo(1));
			Assert.That(tilemapBehaviours.First() != null);
			Assert.That(tilemapBehaviours.First().ChunkSize, Is.EqualTo(chunkSize));
			Assert.That(tilemapBehaviours.First().GetTile(coord).Index, Is.EqualTo(tileIndex));
		}
	}
}
