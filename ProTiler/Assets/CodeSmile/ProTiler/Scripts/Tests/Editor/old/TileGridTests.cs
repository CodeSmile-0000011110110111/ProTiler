// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Tests.Editor.old
{
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
			var coordMin = math.min(coord1, coord2);
			var coordMax = math.max(coord1, coord2);
			var rect = new GridRect(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
			Assert.AreEqual(new GridRect(0, 0, 1, 1), rect);

			coord1 = new GridCoord(0, 0, 0);
			coord2 = new GridCoord(1, 1, 1);
			var coordMin1 = math.min(coord1, coord2);
			var coordMax1 = math.max(coord1, coord2);
			rect = new GridRect(coordMin1.x, coordMin1.z, coordMax1.x - coordMin1.x + 1, coordMax1.z - coordMin1.z + 1);
			Assert.AreEqual(new GridRect(0, 0, 2, 2), rect);

			coord1 = new GridCoord(1, 1, 1);
			coord2 = new GridCoord(0, 0, 0);
			var coordMin2 = math.min(coord1, coord2);
			var coordMax2 = math.max(coord1, coord2);
			rect = new GridRect(coordMin2.x, coordMin2.z, coordMax2.x - coordMin2.x + 1, coordMax2.z - coordMin2.z + 1);
			Assert.AreEqual(new GridRect(0, 0, 2, 2), rect);

			coord1 = new GridCoord(1, 1, 1);
			coord2 = new GridCoord(3, 3, 3);
			var coordMin3 = math.min(coord1, coord2);
			var coordMax3 = math.max(coord1, coord2);
			rect = new GridRect(coordMin3.x, coordMin3.z, coordMax3.x - coordMin3.x + 1, coordMax3.z - coordMin3.z + 1);
			Assert.AreEqual(new GridRect(1, 1, 3, 3), rect);

			coord1 = new GridCoord(3, 3, 3);
			coord2 = new GridCoord(1, 1, 1);
			var coordMin4 = math.min(coord1, coord2);
			var coordMax4 = math.max(coord1, coord2);
			rect = new GridRect(coordMin4.x, coordMin4.z, coordMax4.x - coordMin4.x + 1, coordMax4.z - coordMin4.z + 1);
			Assert.AreEqual(new GridRect(1, 1, 3, 3), rect);

			coord1 = new GridCoord(-3, -3, -3);
			coord2 = new GridCoord(1, 1, 1);
			var coordMin5 = math.min(coord1, coord2);
			var coordMax5 = math.max(coord1, coord2);
			rect = new GridRect(coordMin5.x, coordMin5.z, coordMax5.x - coordMin5.x + 1, coordMax5.z - coordMin5.z + 1);
			Assert.AreEqual(new GridRect(-3, -3, 5, 5), rect);

			coord1 = new GridCoord(-2, -2, -2);
			coord2 = new GridCoord(0, 0, 0);
			var coordMin6 = math.min(coord1, coord2);
			var coordMax6 = math.max(coord1, coord2);
			rect = new GridRect(coordMin6.x, coordMin6.z, coordMax6.x - coordMin6.x + 1, coordMax6.z - coordMin6.z + 1);
			Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);

			coord1 = new GridCoord(1, 4, 4);
			coord2 = new GridCoord(4, 1, 1);
			var coordMin7 = math.min(coord1, coord2);
			var coordMax7 = math.max(coord1, coord2);
			rect = new GridRect(coordMin7.x, coordMin7.z, coordMax7.x - coordMin7.x + 1, coordMax7.z - coordMin7.z + 1);
			Assert.AreEqual(new GridRect(1, 1, 4, 4), rect);

			coord1 = new GridCoord(-2, 0, 0);
			coord2 = new GridCoord(0, -2, -2);
			var coordMin8 = math.min(coord1, coord2);
			var coordMax8 = math.max(coord1, coord2);
			rect = new GridRect(coordMin8.x, coordMin8.z, coordMax8.x - coordMin8.x + 1, coordMax8.z - coordMin8.z + 1);
			Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);

			coord1 = new GridCoord(0, -2, -2);
			coord2 = new GridCoord(-2, 0, 0);
			var coordMin9 = math.min(coord1, coord2);
			var coordMax9 = math.max(coord1, coord2);
			rect = new GridRect(coordMin9.x, coordMin9.z, coordMax9.x - coordMin9.x + 1, coordMax9.z - coordMin9.z + 1);
			Assert.AreEqual(new GridRect(-2, -2, 3, 3), rect);
		}

		[Test]
		public void CreateWorldRectFromGridRect()
		{
			var coord1 = new GridCoord(1, 1, 1);
			var coord2 = new GridCoord(3, 3, 3);
			var coordMin = math.min(coord1, coord2);
			var coordMax = math.max(coord1, coord2);
			var rect = new GridRect(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
			var worldRect = TileGrid.ToWorldRect(rect, new GridSize(10, 1, 10));
			Assert.AreEqual(new Rect(10, 10, 30, 30), worldRect);

			coord1 = new GridCoord(-1, -1, -1);
			coord2 = new GridCoord(3, 3, 3);
			var coordMin1 = math.min(coord1, coord2);
			var coordMax1 = math.max(coord1, coord2);
			rect = new GridRect(coordMin1.x, coordMin1.z, coordMax1.x - coordMin1.x + 1, coordMax1.z - coordMin1.z + 1);
			worldRect = TileGrid.ToWorldRect(rect, new GridSize(10, 1, 10));
			Assert.AreEqual(new Rect(-10, -10, 50, 50), worldRect);
		}
	}
}*/
