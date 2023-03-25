// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	[CreateAssetMenu(fileName = "New TileSet", menuName = Const.TileEditorName + "/TileSet", order = 0)]
	public partial class TileSet : ScriptableObject
	{
		private static GameObject s_MissingTilePrefab;
		private static GameObject s_ClearingTilePrefab;
		[SerializeField] private TileGrid m_Grid = new(new GridSize(10, 1, 10));
		[SerializeField] private TileAnchor m_TileAnchor;
		[SerializeField] private List<GameObject> m_DragDropPrefabsHereToAdd = new();
		[SerializeField] private List<TileSetTile> m_Tiles = new();

		public TileGrid Grid
		{
			get => m_Grid;
			set
			{
				if (m_Grid != value)
				{
					m_Grid = value;
					UpdateMissingTileSize();
				}
			}
		}

		//public Tile GetPrefabIndex(int index) => m_Tiles[index];

		public bool IsEmpty { get => m_Tiles.Count == 0; }
		public int Count { get => m_Tiles.Count; }
		public IReadOnlyList<TileSetTile> Tiles { get => m_Tiles.AsReadOnly(); }

		private void UpdateMissingTileSize()
		{
			if (s_MissingTilePrefab != null)
			{
				var size = m_Grid.Size;
				s_MissingTilePrefab.transform.localScale = new Vector3(size.x, size.y, size.z);
			}
		}

		public float3 GetTileOffset()
		{
			var gridSize = m_Grid.Size;
			return m_TileAnchor switch
			{
				TileAnchor.Center => new float3(gridSize.x * .5f, gridSize.y * .5f, gridSize.z * .5f),
				TileAnchor.BottomRight => float3.zero,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		// TODO:
		// add default grid
		// add default tile pivot

		private void AddPrefabs()
		{
			foreach (var prefab in m_DragDropPrefabsHereToAdd)
				m_Tiles.Add(new TileSetTile(prefab));
			m_DragDropPrefabsHereToAdd.Clear();
		}

		public GameObject GetPrefab(int index) => index >= 0 && index < m_Tiles.Count ? m_Tiles[index].Prefab : GetSpecialTilePrefab(index);

		private GameObject GetSpecialTilePrefab(int index)
		{
			if (index < 0)
			{
				if (s_ClearingTilePrefab == null)
					s_ClearingTilePrefab = Resources.Load<GameObject>(Const.TileEditorResourcePrefabsPath + "ClearingTile");
				return s_ClearingTilePrefab;
			}

			if (s_MissingTilePrefab == null)
				s_MissingTilePrefab = Resources.Load<GameObject>(Const.TileEditorResourcePrefabsPath + "MissingTile");

			UpdateMissingTileSize();
			return s_MissingTilePrefab;
		}

		public void SetPrefab(int index, GameObject prefab) => m_Tiles[index].Prefab = prefab;
	}
}