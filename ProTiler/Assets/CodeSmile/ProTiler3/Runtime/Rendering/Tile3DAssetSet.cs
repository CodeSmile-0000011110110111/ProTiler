﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler3.Runtime.Assets;
using CodeSmile.ProTiler3.Runtime.Grid;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using TileIndex = System.UInt16;

namespace CodeSmile.ProTiler3.Runtime.Rendering
{
	[FullCovered]
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Grid3DController))]
	public class Tile3DAssetSet : MonoBehaviour, ITile3DAssetSet
	{
		[SerializeField] [ReadOnlyField] private Tile3DAssetBaseSet m_TileAssets;

		public Tile3DAssetBase this[TileIndex tileIndex] => m_TileAssets[tileIndex];

		[ExcludeFromCodeCoverage] private void Reset()
		{
			m_TileAssets = new Tile3DAssetBaseSet();
			m_TileAssets.Init();
			LoadAllTileAssetsInProject();
		}

		private void LoadAllTileAssetsInProject()
		{
			#if UNITY_EDITOR
			var tileAssetGuids = AssetDatabase.FindAssets($"t:{nameof(Tile3DAssetBase)}");
			Debug.Log($"Loading all {tileAssetGuids.Length} {nameof(Tile3DAssetBase)} in project ...");
			foreach (var tileAssetGuid in tileAssetGuids)
			{
				var tileAssetPath = AssetDatabase.GUIDToAssetPath(tileAssetGuid);
				if (tileAssetPath.Contains("/Resources/"))
					continue;

				var tileAsset = AssetDatabase.LoadAssetAtPath<Tile3DAssetBase>(tileAssetPath);
				Add(tileAsset, out var tileIndex);
			}
			#endif
		}

		public void Add(Tile3DAssetBase tileAsset) => m_TileAssets.Add(tileAsset);
		public void Add(Tile3DAssetBase tileAsset, out Int32 index) => m_TileAssets.Add(tileAsset, out index);
		public void Remove(Tile3DAssetBase tileAsset) => m_TileAssets.Remove(tileAsset);
		public Boolean Contains(Tile3DAssetBase tileAsset) => m_TileAssets.Contains(tileAsset);
	}
}