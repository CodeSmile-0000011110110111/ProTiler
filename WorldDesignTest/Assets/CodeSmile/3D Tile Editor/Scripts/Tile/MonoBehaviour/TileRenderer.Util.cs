// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public partial class TileRenderer
	{
		[SerializeField] private float m_VisibleRectDistanceMultiplier = 10f;
		
		private static bool IsCurrentCameraValid() => Camera.current != null;

		private void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			Debug.LogWarning("SetTileFlags not implemented");
			// if (TryGetGameObjectAtCoord(coord, out var go))
			// 	ApplyTileFlags(go, flags);
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

		private GameObject InstantiateTileObject(GameObject prefab, Vector3 position, Transform parent, TileFlags flags)
		{
			var go = Object.Instantiate(prefab, position, quaternion.identity, parent);
			ApplyTileFlags(go, flags);
			go.hideFlags = HideFlags.HideAndDontSave;
			return go;
		}

		private RectInt GetCameraRect()
		{
			if (IsCurrentCameraValid() == false)
				return new GridRect();
					
			var camera = Camera.current;
			var camForward = camera.transform.forward;
			var rayOrigin = camera.transform.position + camForward * (m_DrawDistance * m_VisibleRectDistanceMultiplier);
			var camRay = new Ray(rayOrigin, Vector3.down);
			camRay.IntersectsPlane(out float3 camPos);

			var camCoord = m_World.ActiveLayer.Grid.ToGridCoord(camPos);
			return new GridRect(camCoord.x - m_DrawDistance / 2, camCoord.z - m_DrawDistance / 2, m_DrawDistance, m_DrawDistance);
		}

		private GameObject FindOrCreateGameObject(string name, HideFlags hideFlags = HideFlags.None)
		{
			var prefab = transform.Find(name);
			if (prefab == null)
			{
				prefab = new GameObject(name).transform;
				prefab.hideFlags = hideFlags;
				prefab.transform.parent = transform;
			}
			return prefab.gameObject;
		}
	}
}