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

		[SerializeField] [HideInInspector] private Tile3DAssetBase m_EmptyTileAsset;
		[SerializeField] private Tile3DAssetBase m_MissingTileAsset;
		[SerializeField] private ObjectSet<Tile3DAssetBase> m_TileAssetSet;

		public Tile3DAssetBase this[int index] => index <= 0 ? EmptyTileAsset : m_TileAssetSet[index];
		public Tile3DAssetBase MissingTileAsset
		{
			get
			{
				if (m_MissingTileAsset == null)
					LoadMissingTileAsset();
				return m_MissingTileAsset;
			}
		}
		public Tile3DAssetBase EmptyTileAsset
		{
			get
			{
				if (m_EmptyTileAsset == null)
					LoadEmptyTileAsset();
				return m_EmptyTileAsset;
			}
		}

		public static Tile3DAssetRegister Singleton => s_Singleton;

		[ExcludeFromCodeCoverage] private void Reset() => OnCreated();

		private void OnValidate() => m_TileAssetSet.DefaultObject = m_MissingTileAsset;

		private void LoadMissingTileAsset()
		{
			m_MissingTileAsset = Tile3DAssetCreation.LoadMissingTile();
			m_TileAssetSet.DefaultObject = m_MissingTileAsset;
		}

		private void LoadEmptyTileAsset() => m_EmptyTileAsset = Tile3DAssetCreation.LoadEmptyTile();

		private void CreateTileAssetSet() => m_TileAssetSet = new ObjectSet<Tile3DAssetBase>(m_MissingTileAsset, 1);

		internal void OnCreated()
		{
			s_Singleton = this;
			CreateTileAssetSet();
		}

		public void Add(Tile3DAssetBase tileAsset) => Add(tileAsset, out var _);
		public void Add(Tile3DAssetBase tileAsset, out int index) => m_TileAssetSet.Add(tileAsset, out index);

		public void Remove(Tile3DAssetBase tileAsset) => m_TileAssetSet.Remove(tileAsset);

		public bool Contains(Tile3DAssetBase tileAsset) => m_TileAssetSet.Contains(tileAsset);

#if UNITY_EDITOR
		[ExcludeFromCodeCoverage]
		[InitializeOnLoadMethod] [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void OnLoad() => LoadSingletonInstance();

		private static void LoadSingletonInstance() => s_Singleton = AssetDatabaseExt.LoadAssets<Tile3DAssetRegister>().First();
#endif
	}
}
