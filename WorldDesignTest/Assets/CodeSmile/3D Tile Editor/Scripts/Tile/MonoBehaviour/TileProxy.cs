// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public class TileProxy : MonoBehaviour
	{
		[NonSerialized] private GridCoord m_Coord = Global.InvalidGridCoord;
		[NonSerialized] private Tile m_Tile;
		[NonSerialized] private TileLayer m_Layer;
		[NonSerialized] private GameObject m_Instance;

		/*
		private void OnEnable()
		{
			m_Coord = Global.InvalidCoord;
			UpdateInstance();
		}
		*/

		public GridCoord Coord => m_Coord;

		public Tile Tile => m_Tile;

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

		public void SetCoordAndTile(GridCoord coord, Tile tile)
		{
			//Debug.Log($"update TileProxy at {coord} with tile {tile}, layer: {m_Layer}");

			// order is important!
			//if (m_Coord.Equals(coord) == false)
			{
				m_Coord = coord;
				transform.position = m_Layer.Grid.ToWorldPosition(m_Coord);
			}

			if (m_Tile != tile)
			{
				var isSamePrefab = m_Tile != null && tile != null && m_Tile.TileSetIndex == tile.TileSetIndex;
				m_Tile = tile;

				// also need to update the instance if it uses a different prefab
				if (isSamePrefab == false)
					UpdateInstance();
			}
		}

		private void UpdateInstance()
		{
			if (m_Instance != null)
			{
				// cannot delete instances here, otherwise Unity will crash within drawing MeshOutline
				// delay with a coroutine also does not work reliably due to order of execution issues
				m_Instance.SetActive(false);
				TileWorld.ToBeDeletedInstances.Add(m_Instance);
			}

			if (m_Tile != null && m_Layer.TileSet != null)
			{
				var prefab = m_Layer.TileSet.GetPrefab(m_Tile.TileSetIndex);
				var worldPos = m_Layer.GetTilePosition(m_Coord);
				m_Instance = InstantiateTileObject(prefab, worldPos, transform, m_Tile.Flags);

#if UNITY_EDITOR
				name = $"Tile#{m_Tile.TileSetIndex} @ {m_Coord}, {m_Layer}";
				//Debug.Log("UpdateInstance: " + name);
#endif
			}
		}

		private GameObject InstantiateTileObject(GameObject prefab, Vector3 position, Transform parent, TileFlags flags)
		{
			var go = Instantiate(prefab, position, Quaternion.identity, parent);
			ApplyTileFlags(go, flags);
			return go;
		}

		private void ApplyTileFlags(GameObject go, TileFlags flags)
		{
			var t = go.transform;
			t.localScale = ScaleFromTileFlags(flags, t.localScale);
			t.rotation = RotationFromTileFlags(flags);
		}

		private Vector3 ScaleFromTileFlags(TileFlags flags, Vector3 scale)
		{
			if (flags.HasFlag(TileFlags.FlipHorizontal))
				scale.x *= -1f;
			if (flags.HasFlag(TileFlags.FlipVertical))
				scale.z *= -1f;

			return scale;
		}

		private Quaternion RotationFromTileFlags(TileFlags flags)
		{
			if (flags.HasFlag(TileFlags.DirectionEast))
				return Quaternion.Euler(0f, 90f, 0f);
			if (flags.HasFlag(TileFlags.DirectionSouth))
				return Quaternion.Euler(0f, 180f, 0f);
			if (flags.HasFlag(TileFlags.DirectionWest))
				return Quaternion.Euler(0f, 270f, 0f);

			return Quaternion.identity;
		}
	}
}