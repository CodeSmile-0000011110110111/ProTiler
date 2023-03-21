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
		public const string TileEditorName = "Fliesenleger 3D";

		public const string TileEditorIconPathRoot = "Assets/Gizmos/CodeSmile/TileEditor/";
		public const string OverlayIconPath = TileEditorIconPathRoot + "Overlays/";
		public const string EditorToolsIconPath = TileEditorIconPathRoot + "EditorTools/";
		

		public const int InvalidTileSetIndex = -1;

#if DEBUG
		public const HideFlags TileHideFlags = HideFlags.DontSave;
#else
		public const HideFlags TileRenderHideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
#endif

		public static readonly TileData InvalidTileData = new(InvalidTileSetIndex);
		public static readonly GridCoord InvalidGridCoord = new(int.MinValue, int.MinValue, int.MinValue);
		public static readonly GridRect InvalidGridRect = new(int.MinValue, int.MinValue, 0, 0);
		public static readonly TileGrid DefaultGrid = new();

	}
}