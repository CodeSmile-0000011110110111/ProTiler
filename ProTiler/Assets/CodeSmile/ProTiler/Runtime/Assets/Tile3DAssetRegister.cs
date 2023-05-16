// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	[FullCovered]
	public sealed class Tile3DAssetRegister : ScriptableObject
	{
		[SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification = "cannot be readonly")]
		[SuppressMessage("NDepend", "ND1902:AvoidStaticFieldsWithAMutableFieldType",
			Justification = "cannot be readonly")]
		private static Tile3DAssetRegister s_Singleton;

		[SerializeField] private Tile3DAssetBaseSet m_TileAssets;

		[Pure] public Tile3DAssetBase this[Int32 index] => m_TileAssets[index];

		[SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification = "cannot be readonly")]
		[SuppressMessage("NDepend", "ND1902:AvoidStaticFieldsWithAMutableFieldType",
			Justification = "cannot be readonly")]
		[Pure] public static Tile3DAssetRegister Singleton => s_Singleton;

		[Pure] public Int32 Count => m_TileAssets.Count;
		[Pure] public Tile3DAssetBase MissingTileAsset => m_TileAssets.MissingTileAsset;
		[Pure] public Tile3DAssetBase EmptyTileAsset => m_TileAssets.EmptyTileAsset;

		/// <summary>
		///     assigns singleton when created and at runtime (builds)
		/// </summary>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] [Pure] private void Awake() => AssignSingletonInstance();

		[ExcludeFromCodeCoverage] [Pure] private void Reset() => CreateTileAssetSet();

		/// <summary>
		///     assigns singleton after domain reload (script compile)
		/// </summary>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] [Pure] private void OnEnable() => AssignSingletonInstance();

		internal void AssignSingletonInstance() => s_Singleton = this;

		private void CreateTileAssetSet()
		{
			m_TileAssets = new Tile3DAssetBaseSet();
			m_TileAssets.Init();
		}

		[Pure] public void Add(Tile3DAssetBase tileAsset) => m_TileAssets.Add(tileAsset);
		[Pure] public void Add(Tile3DAssetBase tileAsset, out Int32 index) => m_TileAssets.Add(tileAsset, out index);
		[Pure] public void Remove(Tile3DAssetBase tileAsset) => m_TileAssets.Remove(tileAsset);
		[Pure] public Boolean Contains(Tile3DAssetBase tileAsset) => m_TileAssets.Contains(tileAsset);

		[ExcludeFromCodeCoverage]
		[Pure] internal void LoadMissingTileAssetAndSetAsDefault() => m_TileAssets.LoadMissingTileAssetAndSetAsDefault();
	}
}
