// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class int3Ext
	{
		public static Vector2Int ToCoord2(this int3 coord) => new(coord.x, coord.z);

		public static RectInt MakeRect(this int3 coord, int3 other)
		{
			var coordMin = math.min(coord, other);
			var coordMax = math.max(coord, other);
			return new RectInt(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
		}

		public static IReadOnlyList<int3> MakeLine(this int3 coord1, int3 coord2) =>
			MakeLine(coord1.x, coord1.z, coord2.x, coord2.z);

		/// <summary>
		///     Source: https://stackoverflow.com/a/11683720
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		private static IReadOnlyList<int3> MakeLine(int x1, int y1, int x2, int y2)
		{
			var coords = new List<int3>();

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
				coords.Add(new int3(x1, 0, y1));

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
}*/
