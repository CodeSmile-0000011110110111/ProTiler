// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static partial class Tile3DAssetCreation
	{
		public static T CreateInstance<T>(GameObject prefab = null) where T : Tile3DAssetBase
		{
			var instance = ScriptableObject.CreateInstance<T>();
			if (prefab != null && prefab.IsPrefab())
				instance.Prefab = prefab;
			return instance;
		}

		public static Tile3DAsset LoadMissingTile() => LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);

		public static Tile3DAsset LoadEmptyTile() => LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);

		internal static Tile3DAsset LoadTile3DAssetResource(string resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new ArgumentNullException($"failed to load tile prefab from resources: '{resourcePath}'");

			return prefab;
		}
	}
}
