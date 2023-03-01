// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile
{
	public class TileDrawer : MonoBehaviour
	{
		[SerializeField] private List<GameObject> m_TilePrefabs;
		public IReadOnlyList<GameObject> TilePrefabs => m_TilePrefabs;

		public void CreateTileAt(Vector3 position)
		{
			var tilePrefab = GetRandomTile();
			var tileInstance = Instantiate(tilePrefab, position, Quaternion.identity, transform);
		}

		private GameObject GetRandomTile()
		{
			return m_TilePrefabs[Random.Range(0, m_TilePrefabs.Count)];
		}
	}
}