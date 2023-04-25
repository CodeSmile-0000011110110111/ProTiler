// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New 3D Tile", menuName = Names.TileEditor + "/3D Tile Asset", order = Menus.CreateAssetRootOrder)]
#endif
	public class Tile3DAsset : Tile3DAssetBase
	{
	}
}
