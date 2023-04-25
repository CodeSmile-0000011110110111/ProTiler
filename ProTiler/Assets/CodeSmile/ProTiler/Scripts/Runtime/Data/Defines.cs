﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.ProTiler.Data
{
	public static class Names
	{
		public const string TileEditor = "ProTiler";
		public const string Tile3DPaletteWindow = "3D Tile Palette";
		public const string Tilemap3DMenu = "Tilemap 3D";
		public const string RootMenu = "Tools";
	}

	public static class Menus
	{
		public const int CreateAssetRootOrder = 0;
		public const int CreateGameObjectPriority = 0;
	}

	public static class HideFlags
	{
		public const UnityEngine.HideFlags Tile = UnityEngine.HideFlags.DontSave;
		//public const HideFlags TileHideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
	}

	public static class Colors
	{
		public static readonly UnityEngine.Color OutlineColor = new(1f, .7f, .4f);
	}
}
