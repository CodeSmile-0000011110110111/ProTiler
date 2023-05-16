// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Collections;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	[FullCovered]
	[Serializable]
	public class Tile3DAssetBaseSet : ObjectSet<Tile3DAssetBase>
	{
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_EmptyTileAsset;
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_MissingTileAsset;

		[Pure] public new Tile3DAssetBase this[Int32 index] => index <= 0 ? EmptyTileAsset : base[index];

		[Pure] public Tile3DAssetBase MissingTileAsset
		{
			get
			{
				if (m_MissingTileAsset == null)
				{
					LoadMissingTileAsset();
					SetMissingTileAsDefault();
				}
				return m_MissingTileAsset;
			}
		}
		[Pure] public Tile3DAssetBase EmptyTileAsset
		{
			get
			{
				if (m_EmptyTileAsset == null)
					LoadEmptyTileAsset();
				return m_EmptyTileAsset;
			}
		}

		[Pure] internal static Tile3DAsset LoadTile3DAssetResource(String resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new ArgumentNullException($"failed to load tile prefab from Resources: '{resourcePath}'");

			return prefab;
		}

		[Pure] public Tile3DAssetBaseSet()
			: base(null, 1) {}

		[Pure] public void Init()
		{
			LoadEmptyTileAsset();
			LoadMissingTileAsset();
			SetMissingTileAsDefault();
		}

		internal void LoadMissingTileAsset() =>
			m_MissingTileAsset = LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);

		internal void LoadEmptyTileAsset() => m_EmptyTileAsset = LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);

		[Pure] internal void SetMissingTileAsDefault() => DefaultObject = MissingTileAsset;

		[ExcludeFromCodeCoverage]
		[Pure] internal void LoadMissingTileAssetAndSetAsDefault()
		{
			LoadMissingTileAsset();
			SetMissingTileAsDefault();
		}
	}
}
