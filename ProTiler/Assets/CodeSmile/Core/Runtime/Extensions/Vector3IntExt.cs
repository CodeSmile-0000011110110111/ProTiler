// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class Vector3IntExt
	{
		// Note: a simple int-cast won't do because (int)-0.1f should be -1 and not 0
		// floor() rounds towards infinity vs int-cast rounds towards zero
		public static Vector3Int ToGridCoord(this Vector3 position, Vector3 cellSize) => new(
			Mathf.FloorToInt(position.x * (1f / cellSize.x)),
			Mathf.FloorToInt(position.y * (1f / cellSize.y)),
			Mathf.FloorToInt(position.z * (1f / cellSize.z)));

		public static RectInt MakeRect(this Vector3Int coord, Vector3Int other)
		{
			var coordMin = Vector3Int.Min(coord, other);
			var coordMax = Vector3Int.Max(coord, other);
			return new RectInt(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
		}
	}
}
