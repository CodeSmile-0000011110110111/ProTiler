// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	[CreateAssetMenu(fileName = "New 3D Tile", menuName = Names.TileEditor + "/3D Tile", order = Menus.CreateAssetRootOrder)]
	public class Tile3D : Tile3DBase
	{
		public static Tile3D Create() => CreateInstance<Tile3D>();
	}
}