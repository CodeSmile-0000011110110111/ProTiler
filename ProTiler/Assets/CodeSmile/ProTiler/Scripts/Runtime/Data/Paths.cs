// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;

namespace CodeSmile.ProTiler.Editor.Data
{
	public static class Paths
	{
		public const string Tile3DAssetRegister = "Assets/" + Names.TileEditor;

		private const string ResourcesRoot = Names.TileEditor + "/";
		private const string ResourcesPrefabs = ResourcesRoot + "Prefabs/";

		public const string ResourcesMissingTilePrefab = ResourcesPrefabs + "MissingTile3D";
		public const string ResourcesEmptyTilePrefab = ResourcesPrefabs + "EmptyTile3D";

		// old ...
		public const string TileEditorIconRoot = "Assets/Gizmos/CodeSmile/TileEditor/";
		public const string OverlayIcon = TileEditorIconRoot + "Overlays/";
		public const string EditorToolsIcon = TileEditorIconRoot + "EditorTools/";
		public const string TileEditorResourcePrefabs = "TileEditor/Prefabs/";
		public const string TileEditorResourceTileSets = "TileEditor/TileSets/";

	}
}
