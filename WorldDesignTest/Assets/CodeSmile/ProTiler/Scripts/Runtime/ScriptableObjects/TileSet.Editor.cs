// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	public partial class TileSet
	{
#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_DragDropPrefabsHereToAdd.Count > 0)
				AddPrefabs();

			ValidateLayerPrefabs();
		}

		public void ValidateLayerPrefabs()
		{
			for (var i = m_Tiles.Count - 1; i >= 0; i--)
			{
				var tilePrefab = GetPrefab(i);
				if (tilePrefab != null)
				{
					var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(tilePrefab);
					if (source == null)
					{
						Debug.LogWarning($"'{tilePrefab.name}' is an instance. Tiles must be prefabs!");
						m_Tiles.RemoveAt(i);
					}
				}
			}
		}
#endif
	}
}