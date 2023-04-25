// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	public sealed class Tile3DAssetRegister : ScriptableObject
	{
		private static Tile3DAssetRegister s_Singleton;

		private readonly ObjectSet<Tile3DAssetBase> m_TileSet = new();

		public static Tile3DAssetRegister Singleton => s_Singleton;

#if UNITY_EDITOR
		[InitializeOnLoadMethod] [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void OnLoad() => s_Singleton = AssetDatabaseExt.LoadAssets<Tile3DAssetRegister>().First();
#endif

		private void Awake() => s_Singleton = this;

		public void Add(Tile3DAssetBase tileAsset)
		{
			if (m_TileSet.Add(tileAsset, out var index))
				Debug.Log($"Added tile asset {tileAsset.GetInstanceID()} with index: {index}");
		}

		public void Remove(Tile3DAssetBase tileAsset)
		{
			if (m_TileSet.Remove(tileAsset))
				Debug.Log($"Removed tile asset {tileAsset.GetInstanceID()} {tileAsset.name}");
		}
	}
}
