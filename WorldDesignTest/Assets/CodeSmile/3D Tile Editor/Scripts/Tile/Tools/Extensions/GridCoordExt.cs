// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public static class GridCoordExt
	{
		public static Vector2Int ToCoord2d(this GridCoord coord)
		{
			return new Vector2Int(coord.x, coord.z);
		}
	}
}