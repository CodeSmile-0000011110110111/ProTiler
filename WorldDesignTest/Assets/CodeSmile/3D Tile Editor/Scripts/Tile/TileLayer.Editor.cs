// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
#if UNITY_EDITOR
		public void OnValidate()
		{
			m_BrushIndex = Mathf.Clamp(m_BrushIndex, 0, m_PrefabTileset.Count);
			ValidateLayerPrefabs();
		}
		
		public void ValidateLayerPrefabs()
		{
			for (int i = 0; i < m_PrefabTileset.Count; i++)
			{
				var prefabTile = m_PrefabTileset[i];
				if (prefabTile != null)
				{
					var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefabTile);
					if (source == null)
					{
						Debug.LogWarning($"Tile '{prefabTile.name}' is a scene instance. Tiles must be prefabs!");
						m_PrefabTileset[i] = null;
					}
				}
			}
		}
#endif
	}
}