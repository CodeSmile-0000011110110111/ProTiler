﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	public static class Tile3DUtility
	{
		private static Tile3D s_MissingTilePrefab;

		public static Tile3D GetMissingTile()
		{
			if (s_MissingTilePrefab == null || s_MissingTilePrefab.IsMissing())
				s_MissingTilePrefab = Resources.Load<Tile3D>(Paths.TileEditorResourcePrefabs + "MissingTile");
			return s_MissingTilePrefab;
		}
	}
}