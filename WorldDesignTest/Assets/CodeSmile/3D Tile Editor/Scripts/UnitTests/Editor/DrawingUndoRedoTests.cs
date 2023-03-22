// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

public class DrawingUndoRedoTests
{
	private TileLayer m_TileLayer;

	[SetUp]
	public void SetUp()
	{
		var world = new GameObject("world", typeof(TileWorld));
		m_TileLayer = world.transform.GetChild(0).GetComponent<TileLayer>();
		Assert.NotNull(m_TileLayer);

		var tileSet = AssetDatabase.LoadAssetAtPath<TileSet>("Assets/Art/kenney/Tower Defense (Classic)/TileSet/TD Terrain TileSet.asset");
		Assert.NotNull(tileSet);
		m_TileLayer.TileSet = tileSet;
	}

	[Test]
	public void DoUndoTest()
	{
		var start = new GridCoord(0, 0, 0);
		var end = new GridCoord(1, 0, 1);
		m_TileLayer.DrawBrush = new TileBrush(GridCoord.zero, 0);
		m_TileLayer.DrawLine(start, end);

		Assert.AreEqual(2, m_TileLayer.TileDataContainer.Count);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(start).TileSetIndex);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(end).TileSetIndex);

		Undo.PerformUndo();

		Assert.AreEqual(0, m_TileLayer.TileDataContainer.Count);
		Assert.AreEqual(Global.InvalidTileSetIndex, m_TileLayer.TileDataContainer.GetTile(start).TileSetIndex);
		Assert.AreEqual(Global.InvalidTileSetIndex, m_TileLayer.TileDataContainer.GetTile(end).TileSetIndex);

		Undo.PerformRedo();

		Assert.AreEqual(2, m_TileLayer.TileDataContainer.Count);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(start).TileSetIndex);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(end).TileSetIndex);

		m_TileLayer.ClearAllTiles();

		Assert.AreEqual(0, m_TileLayer.TileDataContainer.Count);
		Assert.AreEqual(Global.InvalidTileSetIndex, m_TileLayer.TileDataContainer.GetTile(start).TileSetIndex);
		Assert.AreEqual(Global.InvalidTileSetIndex, m_TileLayer.TileDataContainer.GetTile(end).TileSetIndex);

		Undo.PerformUndo();

		Assert.AreEqual(2, m_TileLayer.TileDataContainer.Count);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(start).TileSetIndex);
		Assert.AreEqual(0, m_TileLayer.TileDataContainer.GetTile(end).TileSetIndex);
	}
}