// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	[CreateAssetMenu(fileName = "New 3D Tile", menuName = Names.TileEditor + "/3D Tile", order = Menus.CreateAssetRootOrder)]

	public class Tile3DAsset : Tile3DAssetBase
	{
		//[MenuItem("GameObject/" + Names.TileEditor + "/" + Names.Tilemap3DMenu + "/" + RectangularTilemapMenuText, priority = Menus.CreateGameObjectPriority + 0)]

		[MenuItem(Names.TileEditor + "/test create tile")]
		public static Tile3DAsset CreateInstance() => CreateInstance<Tile3DAsset>();
	}
}
