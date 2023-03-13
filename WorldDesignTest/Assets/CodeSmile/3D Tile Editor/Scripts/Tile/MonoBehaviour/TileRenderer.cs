// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
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
		private const int MinDrawDistance = 10;
		private const int MaxDrawDistance = 100;

		[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;

		private readonly HideFlags m_RenderHideFlags = /*HideFlags.DontSave |
		                                               HideFlags.HideInHierarchy | 
		                                               HideFlags.HideInInspector |*/
		                                               HideFlags.NotEditable;

		private readonly HideFlags m_PersistentObjectHideFlags = /*HideFlags.HideInHierarchy |
		                                                         HideFlags.HideInInspector |
		                                                         HideFlags.NotEditable |*/
		                                                         HideFlags.DontUnloadUnusedAsset;

		private TileWorld m_World;
		[NonSerialized] private GridRect m_VisibleRect;
		[NonSerialized] private GridRect m_PrevVisibleRect;
		[NonSerialized] private int m_PrevDrawDistance;
		[NonSerialized] private int m_SelectedTileIndex;

		private void Awake()
		{
			m_World = GetComponent<TileWorld>();
			CreateTileProxyPrefabOnce();
			CreateCursorOnce();
		}

		private void OnEnable()
		{
			ClampDrawDistance();
			RecreateTileProxyPool();

			RegisterTileWorldEvents();
		}

		private void OnDisable()
		{
			Debug.Log("TileWorld OnDisable");
			UnregisterTileWorldEvents();

			DisposeTileProxyPool();
		}

		private void OnDrawGizmosSelected() => DrawProxyPoolGizmos();

		private void OnRenderObject()
		{
			if (IsCurrentCameraValid() == false)
				return;

			m_VisibleRect = GetCameraRect();

			var layer = m_World.ActiveLayer;
			UpdateTileProxyObjects(layer);
			UpdateCursorTile(layer);
		}

		private void OnValidate()
		{
			ClampDrawDistance();
			if (m_DrawDistance != m_PrevDrawDistance)
			{
				m_PrevDrawDistance = m_DrawDistance;
				StartCoroutine(WaitForEndOfFrameThenRecreateTileProxyPool());
			}
		}

		private void OnDestroy()
		{
			Debug.Log("TileWorld OnDestroy");
		}

		private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);
	}
}