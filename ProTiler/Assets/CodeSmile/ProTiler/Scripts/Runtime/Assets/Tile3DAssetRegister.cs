// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	public sealed class Tile3DAssetRegister : ScriptableObject
	{
		private static Tile3DAssetRegister s_Singleton;

		[SerializeField] private Tile3DAssetBaseSet m_TileAssets;

		public Tile3DAssetBase this[int index] => m_TileAssets[index];

		public static Tile3DAssetRegister Singleton => s_Singleton;

		public int Count => m_TileAssets.Count;
		public Tile3DAssetBase MissingTileAsset => m_TileAssets.MissingTileAsset;
		public Tile3DAssetBase EmptyTileAsset => m_TileAssets.EmptyTileAsset;

		// assigns singleton when created and at runtime (builds)
		[ExcludeFromCodeCoverage] private void Awake() => AssignSingletonInstance(this);

		[ExcludeFromCodeCoverage] private void Reset() => CreateTileAssetSet();

		// assigns singleton after domain reload (script compile)
		[ExcludeFromCodeCoverage] private void OnEnable()
		{
			AssignSingletonInstance(this);
			m_TileAssets.SetMissingTileAsDefault();
		}

		internal void AssignSingletonInstance(Tile3DAssetRegister register) => s_Singleton = register;

		private void CreateTileAssetSet() => m_TileAssets = new Tile3DAssetBaseSet();

		public void Add(Tile3DAssetBase tileAsset) => m_TileAssets.Add(tileAsset);
		public void Add(Tile3DAssetBase tileAsset, out int index) => m_TileAssets.Add(tileAsset, out index);
		public void Remove(Tile3DAssetBase tileAsset) => m_TileAssets.Remove(tileAsset);
		public bool Contains(Tile3DAssetBase tileAsset) => m_TileAssets.Contains(tileAsset);
	}
}
