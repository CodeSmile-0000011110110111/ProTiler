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
	public sealed partial class TileWorld
	{
		public partial class TileRenderer
		{
			private static bool IsCurrentCameraValid() => Camera.current != null;

			private void SetTileFlags(GridCoord coord, TileFlags flags)
			{
				if (TryGetGameObjectAtCoord(coord, out var go))
					ApplyTileFlags(go, flags);
			}

			private bool IsGameObjectAtCoord(GridCoord coord) => m_ActiveObjects.ContainsKey(coord);
			private bool TryGetGameObjectAtCoord(GridCoord coord, out GameObject go) => m_ActiveObjects.TryGetValue(coord, out go);

			private Transform FindOrCreateChildObject(string name, GameObject prefab = null, HideFlags hideFlags = RenderHideFlags)
			{
				var child = transform.Find(name);
				if (child == null)
				{
					if (prefab == null)
					{
						child = new GameObject(name).transform;
						child.parent = transform;
					}
					else
					{
						child = Instantiate(prefab, transform).transform;
						child.name = name;
					}
				}

				child.gameObject.hideFlags = hideFlags;
				return child;
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
				var go = Instantiate(prefab, position, quaternion.identity, parent);
				ApplyTileFlags(go, flags);
				go.hideFlags = HideFlags.HideAndDontSave;
				return go;
			}

			private void SetCursorPosition(TileLayer layer, GridCoord cursorCoord)
			{
				m_CursorRenderCoord = cursorCoord;
				m_Cursor.position = layer.Grid.ToWorldPosition(m_CursorRenderCoord) + layer.TileSet.GetTileOffset();
			}

			private RectInt GetCameraRect()
			{
				if (IsCurrentCameraValid() == false)
					return new GridRect();
					
				var camera = Camera.current;
				var camForward = camera.transform.forward;
				var rayOrigin = camera.transform.position + camForward * (m_DrawDistance * 10f);
				var camRay = new Ray(rayOrigin, Vector3.down);
				camRay.IntersectsPlane(out float3 camPos);

				var camCoord = m_World.ActiveLayer.Grid.ToGridCoord(camPos);
				return new GridRect(camCoord.x - m_DrawDistance / 2, camCoord.z - m_DrawDistance / 2, m_DrawDistance, m_DrawDistance);
			}
		}
	}
}