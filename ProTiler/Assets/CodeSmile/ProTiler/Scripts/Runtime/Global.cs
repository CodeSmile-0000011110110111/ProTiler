// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler
{
	public static class Global
	{
		public const string TileEditorName = "ProTiler";
		// Previously named (but not known) as:
		//public const string TileEditorName = "Fliesenleger 3D";

		public const int CreateAssetMenuRootOrder = 0;
		public const int CreateGameObjectMenuPriority = 0;

		public const string RootMenuName = "Tools";
		public const string Tile3DPaletteWindowName = "3D Tile Palette";
		public const string Tilemap3DMenuName = "Tilemap 3D";

		public const string TileEditorIconPathRoot = "Assets/Gizmos/CodeSmile/TileEditor/";
		public const string OverlayIconPath = TileEditorIconPathRoot + "Overlays/";
		public const string EditorToolsIconPath = TileEditorIconPathRoot + "EditorTools/";
		public const string TileEditorResourcePrefabsPath = "TileEditor/Prefabs/";
		public const string TileEditorResourceTileSetsPath = "TileEditor/TileSets/";

#if DEBUG
		public const HideFlags TileHideFlags = HideFlags.DontSave;
#else
		public const HideFlags TileHideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
#endif

		public static readonly Color OutlineColor = new(1f, .7f, .4f);
	}
}