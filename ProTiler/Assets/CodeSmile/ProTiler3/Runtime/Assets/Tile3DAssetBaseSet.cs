// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Collections;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler3.Runtime.Assets
{
	[FullCovered]
	[Serializable]
	public class Tile3DAssetBaseSet : ObjectSet<Tile3DAssetBase>
	{
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_EmptyTileAsset;
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_MissingTileAsset;
		[SerializeField] [HideInInspector] private Tile3DAssetBase m_MissingPrefabAsset;

		public new Tile3DAssetBase this[Int32 index]
		{
			get
			{
				if (index <= 0)
					return EmptyTileAsset;

				var tileAsset = base[index];
				if (tileAsset.Prefab == null)
					return MissingPrefabAsset;

				return tileAsset;
			}
		}

		public Tile3DAssetBase MissingTileAsset
		{
			get
			{
				if (m_MissingTileAsset == null)
					m_MissingTileAsset = LoadMissingTileAsset();
				return m_MissingTileAsset;
			}
		}
		public Tile3DAssetBase MissingPrefabAsset
		{
			get
			{
				if (m_MissingPrefabAsset == null)
					m_MissingPrefabAsset = LoadMissingPrefabAsset();
				return m_MissingPrefabAsset;
			}
		}
		public Tile3DAssetBase EmptyTileAsset
		{
			get
			{
				if (m_EmptyTileAsset == null)
					m_EmptyTileAsset = LoadEmptyTileAsset();
				return m_EmptyTileAsset;
			}
		}

		internal static Tile3DAsset LoadTile3DAssetResource(String resourcePath)
		{
			var prefab = Resources.Load<Tile3DAsset>(resourcePath);
			if (prefab == null)
				throw new ArgumentNullException($"failed to load tile prefab from Resources: '{resourcePath}'");

			return prefab;
		}

		internal static Tile3DAsset LoadMissingTileAsset() => LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset);

		internal static Tile3DAsset LoadMissingPrefabAsset() =>
			LoadTile3DAssetResource(Paths.ResourcesMissingPrefabAsset);

		internal static Tile3DAsset LoadEmptyTileAsset() => LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset);

		public Tile3DAssetBaseSet()
			: base(null, 1) {}

		public void Init() => DefaultObject = MissingTileAsset;
	}
}
