// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile
{
	public static class RectIntExt
	{
		public static IReadOnlyList<GridCoord> GetTileCoords(this GridRect gridRect)
		{
			// FIXME: use rect.allPositionsWithin ?

			var coordIndex = 0;
			if (gridRect.width <= 0 || gridRect.height <= 0)
				throw new ArgumentException($"rect {gridRect} is too small!");

			var coords = new GridCoord[gridRect.width * gridRect.height];

			try
			{
				var endX = gridRect.x + gridRect.width;
				var endY = gridRect.y + gridRect.height;
				for (var x = gridRect.x; x < endX; x++)
					for (var y = gridRect.y; y < endY; y++)
						coords[coordIndex++] = new GridCoord(x, 0, y);
			}
			catch (Exception e)
			{
				Debug.LogError($"{e} at coord index: {coordIndex}, rect: {gridRect}");
				throw;
			}

			return coords;
		}

		public static bool IsInside(this GridRect rect, GridCoord coord) => coord.x >= rect.x && coord.x < rect.x + rect.width &&
		                                                                    coord.z >= rect.y && coord.z < rect.y + rect.height;
	}
}