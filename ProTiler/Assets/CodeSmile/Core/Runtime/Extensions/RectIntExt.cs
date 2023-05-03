// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class RectIntExt
	{
		/*public static IReadOnlyList<Vector3Int> GetPositions(this RectInt gridRect)
		{
			// FIXME: use rect.allPositionsWithin ?

			var coordIndex = 0;
			if (gridRect.width <= 0 || gridRect.height <= 0)
				throw new ArgumentException($"rect {gridRect} is too small!");

			var coords = new Vector3Int[gridRect.width * gridRect.height];

			try
			{
				var endX = gridRect.x + gridRect.width;
				var endY = gridRect.y + gridRect.height;
				for (var x = gridRect.x; x < endX; x++)
					for (var y = gridRect.y; y < endY; y++)
						coords[coordIndex++] = new Vector3Int(x, 0, y);
			}
			catch (Exception e)
			{
				Debug.LogError($"{e} at coord index: {coordIndex}, rect: {gridRect}");
				throw;
			}

			return coords;
		}

		public static Rect ToWorldRect(this RectInt rect, Vector3 cellSize) => new(
			rect.x * cellSize.x, rect.y * cellSize.z,
			rect.width * cellSize.x, rect.height * cellSize.z);

		public static RectInt Union(this RectInt RA, in RectInt RB) => new()
		{
			min = Vector2Int.Min(RA.min, RB.min),
			max = Vector2Int.Max(RA.max, RB.max),
		};*/

		public static bool Intersects(this RectInt r1, RectInt r2, out RectInt intersection)
		{
			intersection = new RectInt();
			if (r1.Overlaps(r2) == false)
				return false;

			var x1 = Mathf.Min(r1.xMax, r2.xMax);
			var x2 = Mathf.Max(r1.xMin, r2.xMin);
			var y1 = Mathf.Min(r1.yMax, r2.yMax);
			var y2 = Mathf.Max(r1.yMin, r2.yMin);
			intersection.x = Mathf.Min(x1, x2);
			intersection.y = Mathf.Min(y1, y2);
			intersection.width = Mathf.Max(0, x1 - x2);
			intersection.height = Mathf.Max(0, y1 - y2);
			return true;
		}

		// public static bool IsInside(this RectInt rect, Vector3Int coord) => coord.x >= rect.x && coord.x < rect.x + rect.width &&
		//                                                                     coord.z >= rect.y && coord.z < rect.y + rect.height;
	}
}
