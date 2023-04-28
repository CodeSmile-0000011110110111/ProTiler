// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
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
					m_MissingTileAsset = LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);
				return m_MissingTileAsset;
			}
		}
		public Tile3DAssetBase EmptyTileAsset
		{
			get
			{
				if (m_EmptyTileAsset == null)
					m_EmptyTileAsset = LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);
				return m_EmptyTileAsset;
			}
		}

		internal static Tile3DAsset LoadTile3DAssetResource(string resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new ArgumentNullException($"failed to load tile prefab from resources: '{resourcePath}'");

			return prefab;
		}

		public Tile3DAssetBaseSet()
			: base(null, 1) {}

		public void SetMissingTileAsDefault() => DefaultObject = MissingTileAsset;
	}
}
