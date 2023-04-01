// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[Serializable]
	public sealed class TileSetTile
	{
		[HideInInspector] [SerializeField] private string m_DisplayName = "test";
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private string m_Category;
		[SerializeField] private int m_CustomFlags;

		public GameObject Prefab
		{
			get => m_Prefab;
			set
			{
				m_Prefab = value;
				SetDisplayName();
			}
		}
		public string Category
		{
			get => m_Category;
			set
			{
				m_Category = value;
				SetDisplayName();
			}
		}
		public string DisplayName
		{
			get => m_DisplayName;
			set => m_DisplayName = value;
		}

		public TileSetTile(GameObject prefab, string category = "Uncategorized")
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
	}
}