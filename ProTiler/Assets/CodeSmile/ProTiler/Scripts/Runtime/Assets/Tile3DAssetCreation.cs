// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static class Tile3DAssetCreation
	{
		public static T CreateInstance<T>() where T : Tile3DAssetBase => ScriptableObject.CreateInstance<T>();

		public static T CreateAsset<T>(string path) where T : Tile3DAssetBase
		{
#if UNITY_EDITOR
			return AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<T>(path);
#else
			throw new NotImplementedException("cannot create assets at runtime");
#endif
		}

		public static Tile3DAsset LoadMissingTile() => LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);

		public static Tile3DAsset LoadEmptyTile() => LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);

		private static Tile3DAsset LoadTile3DAssetResource(string resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new Exception($"failed to load tile prefab from resources: '{resourcePath}'");

			return prefab;
		}
	}
}
