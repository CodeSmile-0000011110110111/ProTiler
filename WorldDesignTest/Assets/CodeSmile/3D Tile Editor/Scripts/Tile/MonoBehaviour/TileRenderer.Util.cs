// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
	{
		public partial class TileRenderer
		{
			private static bool IsCameraValid(out Camera camera)
			{
				camera = Camera.current;
				return camera != null;
			}

			private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);

			private void SetTileFlags(int3 coord, TileFlags flags)
			{
				if (TryGetGameObjectAtCoord(coord, out var go))
					ApplyTileFlags(go, flags);
			}

			private bool IsGameObjectAtCoord(int3 coord) => m_ActiveObjects.ContainsKey(coord);
			private bool TryGetGameObjectAtCoord(int3 coord, out GameObject go) => m_ActiveObjects.TryGetValue(coord, out go);

			private Transform CreateChildObject(string name, GameObject prefab = null, HideFlags hideFlags = RenderHideFlags)
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

					child.gameObject.hideFlags = hideFlags;
				}
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

			private void SetCursorPosition(TileLayer layer, int3 cursorCoord)
			{
				m_CursorRenderCoord = cursorCoord;
				m_Cursor.position = layer.Grid.ToWorldPosition(m_CursorRenderCoord) + layer.TileSet.GetTileOffset();
			}

			private RectInt GetCameraRect(Camera camera)
			{
				var camForward = camera.transform.forward;
				var rayOrigin = camera.transform.position + camForward * (m_DrawDistance * 10f);
				var camRay = new Ray(rayOrigin, Vector3.down);
				camRay.IntersectsPlane(out float3 point);

				//var camPos = (float3)camera.transform.position;
				var camPos = point;
				var camCoord = m_World.ActiveLayer.Grid.ToGridCoord(camPos);
				var coord1 = new int3(camCoord.x - m_DrawDistance, 0, camCoord.z - m_DrawDistance);
				var coord2 = new int3(camCoord.x + m_DrawDistance, 0, camCoord.z + m_DrawDistance);
				return TileGrid.MakeRect(coord1, coord2);
			}
		}
	}
}