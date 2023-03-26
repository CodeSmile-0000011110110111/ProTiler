// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile
{
	public class TileDrawer : MonoBehaviour
	{
		#pragma warning disable 0414
		[SerializeField] private bool m_PickRandomTile = true;
		[SerializeField] private int m_SelectedTileIndex;
		[SerializeField] private List<GameObject> m_TilePrefabs;
		public IReadOnlyList<GameObject> TilePrefabs => m_TilePrefabs.AsReadOnly();
		#pragma warning restore

		public void CreateTileAt(Vector3 position)
		{
#if UNITY_EDITOR
			m_SelectedTileIndex = Mathf.Clamp(m_SelectedTileIndex, 0, m_TilePrefabs.Count);

			var tilePrefab = GetTile();
			if (m_PickRandomTile || tilePrefab == null)
				tilePrefab = GetRandomTile();

			// make sure instances are linked to their prefab
			var tileInstance = PrefabUtility.InstantiatePrefab(tilePrefab, transform) as GameObject;
			tileInstance.transform.position = position;
			//var tileInstance = Instantiate(tilePrefab, position, Quaternion.identity, transform);
#endif
		}

		private GameObject GetTile()
		{
			if (m_SelectedTileIndex >= 0 && m_SelectedTileIndex < m_TilePrefabs.Count)
				return m_TilePrefabs[m_SelectedTileIndex];

			return null;
		}

		private GameObject GetRandomTile() => m_TilePrefabs[Random.Range(0, m_TilePrefabs.Count)];
	}
}