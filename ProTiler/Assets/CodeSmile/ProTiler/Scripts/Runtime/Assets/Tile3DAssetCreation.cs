// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using CodeSmile.ProTiler.Editor.Data;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static class Tile3DAssetCreation
	{
		public static T CreateInstance<T>() where T : Tile3DAssetBase => ScriptableObject.CreateInstance<T>();

		public static T CreateAsset<T>(string path) where T : Tile3DAssetBase
		{
#if UNITY_EDITOR
			var tileAsset = AssetDatabaseExt.CreateAssetAndDirectory<T>(path);
			Undo.RegisterCreatedObjectUndo(tileAsset, Path.GetFileNameWithoutExtension(path));
			return tileAsset;

#else
			throw new NotImplementedException("cannot create assets at runtime");
#endif
		}

		public static Tile3DAsset CreateMissingTile()
		{
			var tileAsset = CreateInstance<Tile3DAsset>();
			tileAsset.name = Names.MissingTile;
			tileAsset.Prefab = LoadTilePrefab(Paths.ResourcesMissingTilePrefab);
			return tileAsset;
		}

		public static Tile3DAsset CreateEmptyTile()
		{
			var tileAsset = CreateInstance<Tile3DAsset>();
			tileAsset.name = Names.EmptyTile;
			tileAsset.Prefab = LoadTilePrefab(Paths.ResourcesEmptyTilePrefab);
			return tileAsset;
		}

		private static GameObject LoadTilePrefab(string resourcePath)
		{
			var prefab = Resources.Load<GameObject>(resourcePath);
			if (prefab == null)
				throw new Exception("MissingTile.prefab not found");

			return prefab;
		}
	}
}
