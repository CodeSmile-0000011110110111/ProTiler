// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public static class Global
	{
		public const string TileEditorName = "WorldCraft";

		public static readonly GridCoord InvalidCoord = new(int.MinValue, int.MinValue, int.MinValue);
	}
}