// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.ProTiler.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler.Assets
{
	[Serializable]
	public class Tile3DAssetBaseSet : ObjectSet<Tile3DAssetBase>
	{
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_EmptyTileAsset;
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_MissingTileAsset;

		public new Tile3DAssetBase this[int index] => index <= 0 ? EmptyTileAsset : base[index];

		public Tile3DAssetBase MissingTileAsset
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
		public Tile3DAssetBase EmptyTileAsset
		{
			get
			{
				if (m_EmptyTileAsset == null)
					LoadEmptyTileAsset();
				return m_EmptyTileAsset;
			}
		}

		internal static Tile3DAsset LoadTile3DAssetResource(string resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new ArgumentNullException($"failed to load tile prefab from Resources: '{resourcePath}'");

			return prefab;
		}

		public Tile3DAssetBaseSet()
			: base(null, 1) {}

		public void Init()
		{
			LoadEmptyTileAsset();
			LoadMissingTileAsset();
			SetMissingTileAsDefault();
		}

		internal void LoadMissingTileAsset() => m_MissingTileAsset = LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);
		internal void LoadEmptyTileAsset() => m_EmptyTileAsset = LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);

		internal void SetMissingTileAsDefault() => DefaultObject = MissingTileAsset;

		[ExcludeFromCodeCoverage]
		internal void LoadMissingTileAssetAndSetAsDefault()
		{
			LoadMissingTileAsset();
			SetMissingTileAsDefault();
		}
	}
}
