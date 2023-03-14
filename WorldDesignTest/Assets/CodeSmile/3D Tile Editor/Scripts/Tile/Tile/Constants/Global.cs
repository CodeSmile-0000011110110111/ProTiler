// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public static class Global
	{
		public const string TileEditorName = "WorldCraft";

		public const int InvalidTileSetIndex = -1;
		public static readonly GridCoord InvalidGridCoord = new(int.MinValue, int.MinValue, int.MinValue);
		public static readonly GridRect InvalidGridRect = new(int.MinValue, int.MinValue, 0, 0);
		public static readonly HideFlags TileRenderHideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
	}
}