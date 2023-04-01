// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler
{
	public class TileDataProxy : MonoBehaviour
	{
		[SerializeField] private TileLayer m_Layer;
		[SerializeField] private GameObject m_Instance;
		[SerializeField] private GridCoord m_Coord = TileData.InvalidGridCoord;
		[SerializeField] private TileData m_TileData = TileData.InvalidTileData;

		private readonly List<GameObject> m_InstancesToBeDestroyed = new();
		public List<GameObject> ToBeDeletedInstances => m_InstancesToBeDestroyed;

		public GridCoord Coord => m_Coord;

		public TileData TileData => m_TileData;

		public TileLayer Layer
		{
			get => m_Layer;
			internal set
			{
				if (value == null)
					throw new ArgumentNullException("layer must not be null");

				m_Layer = value;
			}
		}

		private void Update() => ProcessInstancesScheduledForDestroy();

		public void SetCoordAndTile(GridCoord coord, TileData tileData)
		{
			//Debug.Log($"update Tile at {coord} with tile {tile}, layer: {m_Layer}");

			// order is important!
			//if (m_Coord.Equals(coord) == false)
			{
				m_Coord = coord;
				transform.position = m_Layer.Grid.ToWorldPosition(m_Coord);
			}

			var mustUpdateInstance = m_TileData != tileData || m_Instance == null;
			if (mustUpdateInstance)
			{
				m_TileData = tileData;
				UpdateInstance();
			}
		}

		private void ProcessInstancesScheduledForDestroy()
		{
			if (m_InstancesToBeDestroyed.Count > 0)
			{
				foreach (var instance in m_InstancesToBeDestroyed)
					instance.DestroyInAnyMode();
				m_InstancesToBeDestroyed.Clear();
			}
		}

		private void UpdateInstance()
		{
			if (m_Instance != null)
			{
				// cannot delete instances here, otherwise Unity will crash within drawing MeshOutline
				// delay with a coroutine also does not work reliably due to order of execution issues
				m_Instance.SetActive(false);
				m_InstancesToBeDestroyed.Add(m_Instance);
			}

			if (m_TileData.IsInvalid || m_Layer.TileSet == null || m_Layer.TileSet.IsEmpty)
				return;

			var prefab = m_Layer.TileSet.GetPrefab(m_TileData.TileSetIndex);
#if DEBUG
			if (prefab == null)
				throw new NullReferenceException($"TileDataProxy: tileset '{m_Layer.TileSet.name}' prefab with index {m_TileData.TileSetIndex} is null");
#endif

			var worldPos = m_Layer.GetTilePosition(m_Coord);
			m_Instance = InstantiateTileObject(prefab, worldPos, transform, m_TileData.Flags);

#if UNITY_EDITOR
			name = $"Tile#{m_TileData.TileSetIndex} @ {m_Coord}, {m_Layer}";
			//Debug.Log("UpdateInstance: " + name);
#endif
		}

		private GameObject InstantiateTileObject(GameObject prefab, Vector3 position, Transform parent, TileFlagsOld flags)
		{
			var go = Instantiate(prefab, position, Quaternion.identity, parent);
			ApplyTileFlags(go, flags);
			return go;
		}

		private void ApplyTileFlags(GameObject go, TileFlagsOld flags)
		{
			var t = go.transform;
			t.localScale = ScaleFromTileFlags(flags, t.localScale);
			t.rotation = RotationFromTileFlags(flags);
		}

		private Vector3 ScaleFromTileFlags(TileFlagsOld flags, Vector3 scale)
		{
			if (flags.HasFlag(TileFlagsOld.FlipHorizontal))
				scale.x *= -1f;
			if (flags.HasFlag(TileFlagsOld.FlipVertical))
				scale.z *= -1f;

			return scale;
		}

		private Quaternion RotationFromTileFlags(TileFlagsOld flags)
		{
			if (flags.HasFlag(TileFlagsOld.DirectionEast))
				return Quaternion.Euler(0f, 90f, 0f);
			if (flags.HasFlag(TileFlagsOld.DirectionSouth))
				return Quaternion.Euler(0f, 180f, 0f);
			if (flags.HasFlag(TileFlagsOld.DirectionWest))
				return Quaternion.Euler(0f, 270f, 0f);

			return Quaternion.identity;
		}

		public override string ToString() => $"Tile {m_TileData} at {m_Coord} in {m_Layer.name}";
	}
}
