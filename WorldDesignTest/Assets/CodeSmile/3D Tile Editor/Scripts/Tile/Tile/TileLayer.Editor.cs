// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
#if UNITY_EDITOR
		public Action OnClearLayer;
			
		public void OnValidate()
		{
			if (m_ClearTiles)
			{
				m_ClearTiles = false;
				ClearTiles();
				OnClearLayer?.Invoke();
			}

			m_TileCount = m_TileContainer.Count;

			Grid.ClampGridSize();
			ClampBrushIndex();
			ValidateLayerPrefabs();
		}

		private void ClampBrushIndex() => m_ActiveTileSetIndex = Mathf.Clamp(m_ActiveTileSetIndex, 0, m_TileSet.Count);

		public void ValidateLayerPrefabs()
		{
			for (var i = 0; i < m_TileSet.Count; i++)
			{
				var tilePrefab = m_TileSet.GetPrefab(i);
				if (tilePrefab != null)
				{
					var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(tilePrefab);
					if (source == null)
					{
						Debug.LogWarning($"Tile '{tilePrefab.name}' is a scene instance. Tiles must be prefabs!");
						m_TileSet.SetPrefab(i, null);
					}
				}
			}

			// FIXME: don't call this on every validate, only when changed...
			m_TileSet.UpdateTiles();
		}
#endif
	}
}