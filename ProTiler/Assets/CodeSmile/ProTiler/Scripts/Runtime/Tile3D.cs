// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler
{
	[CreateAssetMenu(fileName = "New 3D Tile", menuName = Global.TileEditorName + "/3D Tile", order = Global.CreateAssetMenuRootOrder)]
	public class Tile3D : Tile3DBase
	{
		public static Tile3D Create() => CreateInstance<Tile3D>();
	}
}
