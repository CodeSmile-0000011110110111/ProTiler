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
	[CreateAssetMenu(fileName = "New TileSet", menuName = Global.TileEditorName + "/TileSet", order = 0)]
	public partial class TileSet : ScriptableObject
	{
		[SerializeField] private TileGrid m_Grid = new(new GridSize(10, 1, 10));
		[SerializeField] private TileAnchor m_TileAnchor;
		[SerializeField] private List<GameObject> m_DragDropPrefabsHereToAdd = new();
		[SerializeField] private List<TileSetTile> m_Tiles = new();
		[SerializeField] private float m_TileCursorHeight = 5f;
		public TileGrid Grid { get => m_Grid; set => m_Grid = value; }

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

		public GameObject GetPrefab(int index) => index >= 0 && index < m_Tiles.Count ? m_Tiles[index].Prefab : null;

		public void SetPrefab(int index, GameObject prefab) => m_Tiles[index].Prefab = prefab;

		//public Tile GetPrefabIndex(int index) => m_Tiles[index];

		public int Count => m_Tiles.Count;
		public IReadOnlyList<TileSetTile> Tiles => m_Tiles.AsReadOnly();
		public float TileCursorHeight { get => m_TileCursorHeight; set => m_TileCursorHeight = value; }
	}
}