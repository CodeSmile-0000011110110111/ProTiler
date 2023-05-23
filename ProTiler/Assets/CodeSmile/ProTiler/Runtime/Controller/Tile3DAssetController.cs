// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Grid;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace CodeSmile.ProTiler.Controller
{
	[FullCovered]
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Grid3DController))]
	public class Tile3DAssetController : MonoBehaviour
	{
		[SerializeField] [ReadOnlyField] private Tile3DAssetBaseSet m_TileAssets;

		[ExcludeFromCodeCoverage] private void Reset()
		{
			m_TileAssets = new Tile3DAssetBaseSet();
			m_TileAssets.Init();
		}

		[Pure] public void Add(Tile3DAssetBase tileAsset) => m_TileAssets.Add(tileAsset);
		[Pure] public void Add(Tile3DAssetBase tileAsset, out int index) => m_TileAssets.Add(tileAsset, out index);
		[Pure] public void Remove(Tile3DAssetBase tileAsset) => m_TileAssets.Remove(tileAsset);
		[Pure] public bool Contains(Tile3DAssetBase tileAsset) => m_TileAssets.Contains(tileAsset);
	}
}
