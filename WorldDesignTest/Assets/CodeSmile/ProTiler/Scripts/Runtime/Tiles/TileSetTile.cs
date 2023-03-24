// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class TileSetTile
	{
		[HideInInspector] [SerializeField] private string m_DisplayName = "test";
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private TileSetCategory m_Category;
		[SerializeField] private int m_CustomFlags;

		public TileSetTile(GameObject prefab, TileSetCategory category = 0)
		{
			m_Prefab = prefab;
			m_Category = category;
			SetDisplayName();
		}

		public void SetDisplayName()
		{
			var prefabName = m_Prefab != null ? m_Prefab.name : "<missing>";
			m_DisplayName = $"{m_Category}: {prefabName}";
		}

		public GameObject Prefab
		{
			get => m_Prefab;
			set
			{
				m_Prefab = value;
				SetDisplayName();
			}
		}
		public TileSetCategory Category
		{
			get => m_Category;
			set
			{
				m_Category = value;
				SetDisplayName();
			}
		}
	}
}