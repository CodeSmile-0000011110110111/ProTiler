// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.ProTiler3
{
	public static class Menus
	{
		public const int CreateAssetRootOrder = 0;
		public const int CreateGameObjectPriority = 0;

		public const string CreateAssetMenuPrefix = "Assets/Create/";

		public const string RootMenu = "Tools";
		public const string TilemapMenuText = "3D Tilemap";

		public const string CreateTileAssetMenuText = CreateAssetMenuPrefix + Names.TileEditor + "/3D Tile";
		public const string CreateTilesFromSelectedPrefabs =
			RootMenu + "/" + Names.TileEditor + "/Create Tile3D from Selection";
	}
}
