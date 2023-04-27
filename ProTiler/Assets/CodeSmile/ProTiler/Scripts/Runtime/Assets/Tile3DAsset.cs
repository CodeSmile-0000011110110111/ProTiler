// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using CodeSmile.ProTiler.Editor.Creation;
using UnityEditor;

namespace CodeSmile.ProTiler.Assets
{
	public class Tile3DAsset : Tile3DAssetBase
	{
#if UNITY_EDITOR
		[MenuItem(Menus.CreateTileAssetMenuText)]
		private static void CreateTileAssetWithSelection() =>
			Tile3DAssetCreation.CreateRegisteredAssetWithSelection<Tile3DAsset>(nameof(Tile3DAsset));
#endif
	}
}
