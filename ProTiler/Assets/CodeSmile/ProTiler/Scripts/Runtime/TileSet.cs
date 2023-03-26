// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler
{
	[CreateAssetMenu(fileName = "New TileSet", menuName = Global.TileEditorName + "/TileSet", order = 0)]
	public class TileSet : ScriptableObject
	{
		private static GameObject s_MissingTilePrefab;
		private static GameObject s_ClearingTilePrefab;

		[SerializeField] private TileGrid m_Grid = new(new GridSize(10, 1, 10));
		[SerializeField] private TileAnchor m_TileAnchor;
		[SerializeField] private List<GameObject> m_DragDropPrefabsHereToAdd = new();
		[SerializeField] private List<TileSetTile> m_Tiles = new();

		private void OnEnable()
		{
			UpdateMissingTileSize(m_Grid.Size);
		}

		public TileGrid Grid
		{
			get => m_Grid;
			set
			{
				if (m_Grid != value)
				{
					m_Grid = value;
					UpdateMissingTileSize(m_Grid.Size);
				}
			}
		}

		//public Tile GetPrefabIndex(int index) => m_Tiles[index];

		public bool IsEmpty => m_Tiles.Count == 0;
		public int Count => m_Tiles.Count;
		public IReadOnlyList<TileSetTile> Tiles => m_Tiles.AsReadOnly();
		public static GameObject ClearingTilePrefab
		{
			get
			{
				if (s_ClearingTilePrefab == null)
					s_ClearingTilePrefab = Resources.Load<GameObject>(Global.TileEditorResourcePrefabsPath + "ClearingTile");
				return s_ClearingTilePrefab;
			}
		}
		public static GameObject MissingTilePrefab
		{
			get
			{
				if (s_MissingTilePrefab == null)
					s_MissingTilePrefab = Resources.Load<GameObject>(Global.TileEditorResourcePrefabsPath + "MissingTile");
				return s_MissingTilePrefab;
			}
		}

		private static void UpdateMissingTileSize(GridSize size) =>
			MissingTilePrefab.transform.localScale = new Vector3(size.x, size.y, size.z);

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

		private GameObject GetSpecialTilePrefab(int index) => index < 0 ? ClearingTilePrefab : MissingTilePrefab;

		public void SetPrefab(int index, GameObject prefab) => m_Tiles[index].Prefab = prefab;

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_DragDropPrefabsHereToAdd.Count > 0)
				AddPrefabs();

			ValidateLayerPrefabs();
			Grid.ClampGridSize();
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