// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using NUnit.Framework;
using UnityEngine;
using GridSize = Unity.Mathematics.int3;
using GridCoord = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

public class TileGridTests
{
	[Test]
	public void ConvertWorldPosToGridCoord()
	{
		// positive position
		var worldPos = new Vector3(111, 222, 339);
		var grid = new TileGrid(new GridSize(10, 10, 10));
		var gridCoord = grid.ToGridCoord(worldPos);
		Assert.AreEqual(new GridSize(11, 22, 33), gridCoord);

		// zero position
		worldPos = new Vector3(0, 0, 0);
		grid = new TileGrid(new GridSize(1, 1, 1));
		gridCoord = grid.ToGridCoord(worldPos);
		Assert.AreEqual(new GridSize(0, 0, 0), gridCoord);

		// zero grid (clamped to 1,1,1)
		worldPos = new Vector3(10, 1, 10);
		grid = new TileGrid(new GridSize(0, 0, 0)); // clamped to 1,1,1
		Assert.AreEqual(new GridSize(1, 1, 1), grid.Size);
		gridCoord = grid.ToGridCoord(worldPos);
		Assert.AreEqual(new GridSize(10, 1, 10), gridCoord);
		
		// negative position
		worldPos = new Vector3(-.1f, 0, -.9f);
		grid = new TileGrid(new GridSize(1, 1, 1));
		gridCoord = grid.ToGridCoord(worldPos);
		Assert.AreEqual(new GridSize(-1, 0, -1), gridCoord);
		
		// negative position again
		worldPos = new Vector3(-111, -222, -339);
		grid = new TileGrid(new GridSize(13, 12, 11));
		gridCoord = grid.ToGridCoord(worldPos);
		Assert.AreEqual(new GridSize(-9, -19, -31), gridCoord);

	}

	[Test]
	public void CreateRectFromTwoCoords()
	{
		var coord1 = new GridCoord(0, 0, 0);
		var coord2 = new GridCoord(0, 0, 0);
		var rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(0, 0, 1, 1), rect);

		coord1 = new GridCoord(0, 0, 0);
		coord2 = new GridCoord(1, 1, 1);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(0, 0, 2, 2), rect);

		coord1 = new GridCoord(1, 1, 1);
		coord2 = new GridCoord(0, 0, 0);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(0, 0, 2, 2), rect);

		coord1 = new GridCoord(1, 1, 1);
		coord2 = new GridCoord(3, 3, 3);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(1, 1, 3, 3), rect);

		coord1 = new GridCoord(3, 3, 3);
		coord2 = new GridCoord(1, 1, 1);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(1, 1, 3, 3), rect);

		coord1 = new GridCoord(-3, -3, -3);
		coord2 = new GridCoord(1, 1, 1);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(-3, -3, 5, 5), rect);

		coord1 = new GridCoord(-2, -2, -2);
		coord2 = new GridCoord(0, 0, 0);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);

		coord1 = new GridCoord(1, 4, 4);
		coord2 = new GridCoord(4, 1, 1);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(1, 1, 4, 4), rect);

		coord1 = new GridCoord(-2, 0, 0);
		coord2 = new GridCoord(0, -2, -2);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);

		coord1 = new GridCoord(0, -2, -2);
		coord2 = new GridCoord(-2, 0, 0);
		rect = TileGrid.MakeRect(coord1, coord2);
		Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);
	}

	[Test]
	public void CreateWorldRectFromGridRect()
	{
		var coord1 = new GridCoord(1, 1, 1);
		var coord2 = new GridCoord(3, 3, 3);
		var rect = TileGrid.MakeRect(coord1, coord2);
		var worldRect = TileGrid.ToWorldRect(rect, new GridSize(10, 1, 10));
		Assert.AreEqual(new Rect(10, 10, 30, 30), worldRect);

		coord1 = new GridCoord(-1, -1, -1);
		coord2 = new GridCoord(3, 3, 3);
		rect = TileGrid.MakeRect(coord1, coord2);
		worldRect = TileGrid.ToWorldRect(rect, new GridSize(10, 1, 10));
		Assert.AreEqual(new Rect(-10, -10, 50, 50), worldRect);
	}
}