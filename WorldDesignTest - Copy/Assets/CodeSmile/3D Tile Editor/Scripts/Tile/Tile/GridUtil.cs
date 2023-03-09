// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public static class GridUtil
	{
		public static GridRect MakeRect(GridCoord coord1, GridCoord coord2)
		{
			var coordMin = math.min(coord1, coord2);
			var coordMax = math.max(coord1, coord2);
			return new GridRect(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
		}

		public static WorldRect ToWorldRect(GridRect rect, GridSize gridSize) => new(
			rect.x * gridSize.x, rect.y * gridSize.z,
			rect.width * gridSize.x, rect.height * gridSize.z);
		
	}
}