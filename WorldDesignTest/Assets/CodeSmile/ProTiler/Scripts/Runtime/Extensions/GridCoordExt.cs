// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public static class GridCoordExt
	{
		public static Vector2Int ToCoord2d(this GridCoord coord) => new(coord.x, coord.z);

		public static RectInt MakeRect(this GridCoord coord, GridCoord other) => TileGrid.MakeRect(coord, other);

		public static IReadOnlyList<GridCoord> MakeLine(this GridCoord coord1, GridCoord coord2) =>
			MakeLine(coord1.x, coord1.z, coord2.x, coord2.z);

		/// <summary>
		///     Source: https://stackoverflow.com/a/11683720
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="clear"></param>
		/// <param name="callback"></param>
		private static IReadOnlyList<GridCoord> MakeLine(int x1, int y1, int x2, int y2)
		{
			var coords = new List<GridCoord>();

			// TODO: refactor ...
			var w = x2 - x1;
			var h = y2 - y1;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

			if (w < 0) dx1 = -1;
			else if (w > 0) dx1 = 1;

			if (h < 0) dy1 = -1;
			else if (h > 0) dy1 = 1;

			if (w < 0) dx2 = -1;
			else if (w > 0) dx2 = 1;

			var longest = math.abs(w);
			var shortest = math.abs(h);
			if (!(longest > shortest))
			{
				longest = math.abs(h);
				shortest = math.abs(w);
				if (h < 0) dy2 = -1;
				else if (h > 0) dy2 = 1;
				dx2 = 0;
			}

			var numerator = longest >> 1;
			for (var i = 0; i <= longest; i++)
			{
				coords.Add(new GridCoord(x1, 0, y1));

				numerator += shortest;
				if (!(numerator < longest))
				{
					numerator -= longest;
					x1 += dx1;
					y1 += dy1;
				}
				else
				{
					x1 += dx2;
					y1 += dy2;
				}
			}

			return coords;
		}
	}
}