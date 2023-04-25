// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using Microsoft.SqlServer.Server;

namespace CodeSmile.ProTiler.Editor
{
	public static class Paths
	{
		public const string Tile3DAssetRegisterPath = "Assets/" + Names.TileEditor;


		public const string TileEditorIconRoot = "Assets/Gizmos/CodeSmile/TileEditor/";
		public const string OverlayIcon = TileEditorIconRoot + "Overlays/";
		public const string EditorToolsIcon = TileEditorIconRoot + "EditorTools/";
		public const string TileEditorResourcePrefabs = "TileEditor/Prefabs/";
		public const string TileEditorResourceTileSets = "TileEditor/TileSets/";
	}
}
