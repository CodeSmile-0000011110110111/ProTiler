// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Assets;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	public class Tile3DSet : MonoBehaviour
	{
		[SerializeField] [ReadOnlyField] private Tile3DAssetBaseSet m_TileAssets;

		[ExcludeFromCodeCoverage] private void Reset()
		{
			m_TileAssets = new Tile3DAssetBaseSet();
			m_TileAssets.Init();
		}

		public void Add(Tile3DAssetBase tileAsset) => m_TileAssets.Add(tileAsset);
		public void Add(Tile3DAssetBase tileAsset, out int index) => m_TileAssets.Add(tileAsset, out index);
		public void Remove(Tile3DAssetBase tileAsset) => m_TileAssets.Remove(tileAsset);
		public bool Contains(Tile3DAssetBase tileAsset) => m_TileAssets.Contains(tileAsset);
	}
}
