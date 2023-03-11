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
		private GridCoord m_Coord;
		private Tile m_Tile;
		private TileLayer m_Layer;
		private GameObject m_Instance;

		private void OnEnable() => UpdateInstance();

		public GridCoord Coord
		{
			get => m_Coord;
			set
			{
				if (m_Coord.Equals(value) == false)
				{
					m_Coord = value;
					MoveToCoord();
				}
			}
		}

		public Tile Tile
		{
			get => m_Tile;
			set
			{
				if (m_Tile != value)
				{
					m_Tile = value;
					UpdateInstance();
				}
			}
		}
		public TileLayer Layer
		{
			get => m_Layer;
			set
			{
				Debug.Log($"proxy set layer to: {value}");
				if (value == null)
					throw new ArgumentNullException("layer must not be null");
				
				m_Layer = value;
			}
		}

		private void UpdateInstance()
		{
			Debug.Log($"update instance at {m_Coord} with tile {m_Tile}");
			if (m_Instance != null)
			{
				m_Instance.DestroyInAnyMode();
				m_Instance = null;
			}

			if (m_Tile != null && m_Layer != null && m_Layer.TileSet != null)
			{
				var prefab = m_Layer.TileSet.GetPrefab(m_Tile.TileSetIndex);
				var worldPos = m_Layer.GetTilePosition(m_Coord);
				m_Instance = InstantiateTileObject(prefab, worldPos, transform, m_Tile.Flags);

#if UNITY_EDITOR
				m_Instance.name = $"({m_Coord} Tile#{m_Tile.TileSetIndex}, Layer: {m_Layer}";
#endif
			}
		}

		private void MoveToCoord() => transform.position = m_Layer.Grid.ToWorldPosition(m_Coord);

		private GameObject InstantiateTileObject(GameObject prefab, Vector3 position, Transform parent, TileFlags flags)
		{
			var go = Instantiate(prefab, position, Quaternion.identity, parent);
			ApplyTileFlags(go, flags);
			go.hideFlags = TileWorld.TileRenderer.RenderHideFlags;
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