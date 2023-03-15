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
	/// <summary>
	///     Draws tile world layers using game objects.
	///     Game objects within view range (configurable) are instantiated or destroyed as needed.
	///     Note: pooling will eventually be added to this.
	///     Note: no need to keep state, when loading scene, instantiate all visible game objects initially all at once
	///     How it works:
	///     For each layer, get the tileset prefabs.
	///     enumerating tiles in visible range => change to rectangular XZ view range
	///     try get tile for each coordinate from the layer TileContainer
	///     keep a collection of currently instantiated tiles (game object + coord)
	///     or: rely on object naming
	///     enumerate that collection for visibility (not necessarily every frame)
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileWorld))]
	public sealed partial class TileRenderer : MonoBehaviour
	{
		private const int MinDrawDistance = 2;
		private const int MaxDrawDistance = 100;

		[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;
		[SerializeField] private float m_VisibleRectDistance = 20f;

		private TileWorld m_World;

		private void Awake()
		{
			m_World = GetComponent<TileWorld>();
			CreateTileProxyPrefabOnce();
		}

		private void OnEnable()
		{
			ClampDrawDistance();
			RecreateTileProxyPool();

			RegisterTileWorldEvents();
		}

		private void OnDisable()
		{
			UnregisterTileWorldEvents();

			DisposeTileProxyPool();
		}

		private void OnDestroy() => Debug.Log("TileWorld OnDestroy");

		private void OnDrawGizmosSelected() => DrawProxyPoolGizmos();

		private void OnRenderObject()
		{
			if (CameraExt.IsCurrentCameraValid() == false)
				return;

			UpdateTileProxiesInVisibleRect();
		}

		private void OnValidate()
		{
			ClampDrawDistance();
			if (m_DrawDistance != m_PrevDrawDistance)
			{
				m_PrevDrawDistance = m_DrawDistance;
				StopAllCoroutines();
				StartCoroutine(WaitForEndOfFrameThenRecreateTileProxyPool());
			}
		}

		private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);
	}
}