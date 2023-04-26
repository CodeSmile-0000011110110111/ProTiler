// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Editor.Creation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	public sealed class Tile3DAssetRegister : ScriptableObject
	{
		private static Tile3DAssetRegister s_Singleton;

		[SerializeField] internal Tile3DAsset m_MissingTileAsset;
		[SerializeField] internal Tile3DAsset m_EmptyTileAsset;
		[SerializeField] private ObjectSet<Tile3DAssetBase> m_TileAssetSet;

		public Tile3DAssetBase this[int index] => m_TileAssetSet[index];

		public static Tile3DAssetRegister Singleton => s_Singleton;

#if UNITY_EDITOR
		[ExcludeFromCodeCoverage]
		[InitializeOnLoadMethod] [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
		private static void OnLoad()
		{
			Debug.LogWarning("OnLoad register");
			if (s_Singleton == null)
				Debug.Log("singleton is actually null");
			if (s_Singleton != null && s_Singleton.IsMissing())
				Debug.Log("singleton is MISSING");

			LoadSingletonInstance();
		}

		private static void LoadSingletonInstance()
		{
			s_Singleton = AssetDatabaseExt.LoadAssets<Tile3DAssetRegister>().First();
			Debug.LogWarning("load singleton instance with ID: " + s_Singleton.GetInstanceID());
		}

		[ExcludeFromCodeCoverage] private void Awake() => OnCreated();
		[ExcludeFromCodeCoverage] private void Reset() => OnCreated();

		private void CreateMissingTileAsset()
		{
			if (m_MissingTileAsset == null)
				m_MissingTileAsset = Tile3DAssetCreation.CreateMissingTile();
		}

		private void CreateEmptyTileAsset()
		{
			if (m_EmptyTileAsset == null)
				m_EmptyTileAsset = Tile3DAssetCreation.CreateEmptyTile();
		}

		private void CreateTileAssetSet()
		{
			if (m_TileAssetSet == null)
			{
				// index 0 == "empty tile"
				m_TileAssetSet = new ObjectSet<Tile3DAssetBase>(Singleton.m_MissingTileAsset);
				m_TileAssetSet.Add(Singleton.m_EmptyTileAsset);
			}
		}

		internal void OnCreated()
		{
			s_Singleton = this;
			Debug.LogWarning("OnCreated register, singleton: " + s_Singleton.GetInstanceID());
			CreateEmptyTileAsset();
			CreateMissingTileAsset();
			CreateTileAssetSet();
		}

		public void Add(Tile3DAssetBase tileAsset) => Add(tileAsset, out var _);
		public void Add(Tile3DAssetBase tileAsset, out int index) => m_TileAssetSet.Add(tileAsset, out index);

		public void Remove(Tile3DAssetBase tileAsset) => m_TileAssetSet.Remove(tileAsset);

		public bool Contains(Tile3DAsset tileAsset) => m_TileAssetSet.Contains(tileAsset);
	}
}
