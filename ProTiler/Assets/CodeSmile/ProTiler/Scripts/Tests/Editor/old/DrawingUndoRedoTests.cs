// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Tests.Editor.old
{
	[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public class DrawingUndoRedoTests
	{
		private TileLayer m_TileLayer;
		private TileLayerToolbox m_Toolbox;

		[SetUp]
		public void SetUp()
		{
			var world = new GameObject("world", typeof(TileWorld));
			m_TileLayer = world.transform.GetChild(0).GetComponent<TileLayer>();
			Assert.NotNull(m_TileLayer);

			m_Toolbox = world.transform.GetChild(0).GetComponent<TileLayerToolbox>();
			Assert.NotNull(m_Toolbox);

			var tileSet = AssetDatabase.LoadAssetAtPath<TileSet>("Assets/Art/kenney/Tower Defense (Classic)/TileSet/TD Terrain TileSet.asset");
			Assert.NotNull(tileSet);
			m_TileLayer.TileSet = tileSet;
		}

		[TearDown]
		public void TearDown() {}

		[Test]
		public void DrawLineUndoRedoTests()
		{
			var start = new GridCoord(0, 0, 0);
			var end = new GridCoord(1, 0, 1);
			m_Toolbox.DrawBrush = new TileBrush(GridCoord.zero, 0);
			m_Toolbox.DrawLine(start, end);

			Assert.AreEqual(2, m_TileLayer.TileCount);
			Assert.AreEqual(0, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(0, m_TileLayer.GetTileData(end).TileSetIndex);

			Undo.PerformUndo();

			Assert.AreEqual(0, m_TileLayer.TileCount);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(end).TileSetIndex);

			Undo.PerformRedo();

			Assert.AreEqual(2, m_TileLayer.TileCount);
			Assert.AreEqual(0, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(0, m_TileLayer.GetTileData(end).TileSetIndex);
		}

		[Test]
		public void ClearTilesUndoRedoTests()
		{
			var start = new GridCoord(-1, 0, -2);
			var end = new GridCoord(3, 0, 1);
			var tileIndex = 0;
			m_Toolbox.DrawBrush = new TileBrush(GridCoord.zero, tileIndex);
			m_Toolbox.DrawLine(start, end);
			var tileCount = m_TileLayer.TileCount;

			Assert.AreEqual(tileCount, m_TileLayer.TileCount);
			Assert.AreEqual(tileIndex, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(tileIndex, m_TileLayer.GetTileData(end).TileSetIndex);

			// FIXME: undo/redo clear all only works after doing undo/redo once before
			//Undo.PerformUndo();
			Undo.PerformRedo();
			m_Toolbox.ClearAllTiles();

			Assert.AreEqual(0, m_TileLayer.TileCount);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(end).TileSetIndex);

			Undo.PerformUndo();

			Assert.AreEqual(tileCount, m_TileLayer.TileCount);
			Assert.AreEqual(0, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(0, m_TileLayer.GetTileData(end).TileSetIndex);

			Undo.PerformRedo();

			Assert.AreEqual(0, m_TileLayer.TileCount);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(start).TileSetIndex);
			Assert.AreEqual(TileData.InvalidTileSetIndex, m_TileLayer.GetTileData(end).TileSetIndex);
		}
	}
}
