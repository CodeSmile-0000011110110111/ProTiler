// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Model;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Rendering
{
	[ExecuteAlways]
	[RequireComponent(typeof(Tilemap3DModel))]
	[DisallowMultipleComponent]
	public sealed class Tilemap3DRenderer : MonoBehaviour
	{
		[SerializeReference] [HideInInspector] private Tilemap3DCullingBase m_Culling;

		private Tile3DAssetSet m_TileAssetSet;
		private Tile3DRendererPool m_TileRendererPool;
		/*private Tile3DRendererPoolFirstTry m_TileRendererPool;
		private Tile3DRendererPoolFirstTry TileRendererPool
		{
			get
			{
				if (m_TileRendererPool == null)
					CreateTileRendererPool();
				return m_TileRendererPool;
			}
		}*/
		internal Tile3DAssetSet TileAssetSet
		{
			get
			{
				if (m_TileAssetSet == null)
					m_TileAssetSet = transform.parent.GetComponent<Tile3DAssetSet>();
				return m_TileAssetSet;
			}
		}
		private Tilemap3DModel TilemapModel => GetComponent<Tilemap3DModel>();
		public Tilemap3DCullingBase Culling { get => m_Culling; set => m_Culling = value; }

		private void Init()
		{
#if UNITY_EDITOR
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif

			m_TileRendererPool = gameObject.GetOrAddComponent<Tile3DRendererPool>();

			CreateDefaultFrustumCulling();
			CreateTileRendererPool();
		}

		private void Awake() => Init();
		private void Reset() => Init();
		private void OnEnable() => RegisterModelEvents();
		private void OnDisable() => UnregisterModelEvents();
		private void OnDestroy() => DisposeTileRendererPool();

		private void LateUpdate()
		{
			var visibleCoords = m_Culling.GetVisibleCoords();
			//TileRendererPool.UpdateActiveRenderers(visibleCoords, TilemapModel.Grid.CellSize);
		}

#if UNITY_EDITOR
		private void OnBeforeAssemblyReload() => DisposeTileRendererPool();
#endif

		private void CreateDefaultFrustumCulling() => m_Culling = new Tilemap3DFrustumCulling();

		private void CreateTileRendererPool()
		{
			DisposeTileRendererPool();
			//m_TileRendererPool = new Tile3DRendererPoolFirstTry(gameObject);
		}

		private void DisposeTileRendererPool()
		{
			// if (m_TileRendererPool != null)
			// 	m_TileRendererPool.Dispose();
		}

		private void RegisterModelEvents()
		{
			TilemapModel.OnTilemapCleared += OnTilemapCleared;
			TilemapModel.OnTilemapModified += OnTilemapModified;
		}

		private void UnregisterModelEvents()
		{
			TilemapModel.OnTilemapCleared -= OnTilemapCleared;
			TilemapModel.OnTilemapModified -= OnTilemapModified;
		}

		private void OnTilemapCleared()
		{
			//m_TileRendererPool.Clear();
		}

		private void OnTilemapModified(IEnumerable<Tile3DCoord> tileCoords)
		{
			//m_TileRendererPool.UpdateModifiedTiles(tileCoords, TilemapModel.Grid.CellSize, TileAssetSet);
		}
	}
}
