// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.ProTiler3
{
	public static class Paths
	{
		public const string Tile3DAssetRegister = "Assets/" + Names.TileEditor;

		private const string ResourcesRoot = Names.TileEditor + "/";
		private const string ResourcesPrefabs = ResourcesRoot + "Prefabs/";
		private const string ResourcesTiles = ResourcesRoot + "Tiles/";

		public const string ResourcesMissingTileAsset = ResourcesTiles + "MissingTile3DAsset";
		public const string ResourcesMissingPrefabAsset = ResourcesTiles + "MissingTile3DAssetPrefab";
		public const string ResourcesEmptyTileAsset = ResourcesTiles + "EmptyTile3DAsset";

		// old ...
		public const string TileEditorIconRoot = "Assets/Gizmos/CodeSmile/TileEditor/";
		public const string OverlayIcon = TileEditorIconRoot + "Overlays/";
		public const string EditorToolsIcon = TileEditorIconRoot + "EditorTools/";
		public const string TileEditorResourcePrefabs = "TileEditor/Prefabs/";
		public const string TileEditorResourceTileSets = "TileEditor/TileSets/";

	}
}
