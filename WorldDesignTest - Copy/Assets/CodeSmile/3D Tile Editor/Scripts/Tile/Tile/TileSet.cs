// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class TileSet
	{
		[SerializeField] private List<GameObject> m_Prefabs = new();
		[HideInInspector] [SerializeField] [ReadOnlyField] private List<Tile> m_Tiles = new();

		//public GameObject this[int index] { get => m_Prefabs[index]; set => m_Prefabs[index] = value; }
		public GameObject GetPrefab(int index) => m_Prefabs[index];

		public void SetPrefab(int index, GameObject prefab) => m_Prefabs[index] = prefab;

		public Tile GetTile(int index) => m_Tiles[index];

		public int Count => m_Prefabs.Count;
		public IReadOnlyList<GameObject> Prefabs => m_Prefabs;

		public void UpdateTiles()
		{
			m_Tiles.Clear();
			for (var i = 0; i < m_Prefabs.Count; i++)
			{
				m_Tiles.Add(new Tile(i));
			}
		}
	}
}