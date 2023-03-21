// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile
{
	public static class RectIntExt
	{
		public static Rect ToWorldRect(this RectInt r, GridSize scale)
		{
			var pos = new Vector2(r.x * scale.x, r.y * scale.z);
			var size = new Vector2(r.size.x * scale.x, r.size.y * scale.z);
			return new Rect(pos, size);
		}

		public static RectInt Union(this RectInt RA, in RectInt RB)
		{
			var union = new RectInt();
			union.min = Vector2Int.Min(RA.min, RB.min);
			union.max = Vector2Int.Max(RA.max, RB.max);
			return union;
		}

		public static bool Intersects(this RectInt r1, RectInt r2, out RectInt intersection)
		{
			intersection = new RectInt();
			var overlaps = r2.Overlaps(r1);
			if (overlaps)
			{
				var x1 = math.min(r1.xMax, r2.xMax);
				var x2 = math.max(r1.xMin, r2.xMin);
				var y1 = math.min(r1.yMax, r2.yMax);
				var y2 = math.max(r1.yMin, r2.yMin);
				intersection.x = math.min(x1, x2);
				intersection.y = math.min(y1, y2);
				intersection.width = math.max(0, x1 - x2);
				intersection.height = math.max(0, y1 - y2);
			}

			return overlaps;
		}
		
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