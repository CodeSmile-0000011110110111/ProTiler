// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

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
			m_TileRendererPool = GetComponent<Tile3DRendererPool>();
			CreateDefaultFrustumCulling();
		}

		private void Awake() => Init();
		private void Reset() => Init();
		private void OnEnable() => RegisterModelEvents();
		private void OnDisable() => UnregisterModelEvents();

		private void LateUpdate()
		{
			var visibleCoords = m_Culling.GetVisibleCoords();
			m_TileRendererPool.SetVisibleCoords(visibleCoords, TilemapModel.Grid.CellSize);
		}


		private void CreateDefaultFrustumCulling() => m_Culling = new Tilemap3DFrustumCulling();

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
			m_TileRendererPool.Clear();
		}

		private void OnTilemapModified(IEnumerable<Tile3DCoord> tileCoords)
		{
			//m_TileRendererPool.UpdateModifiedTiles(tileCoords, TilemapModel.Grid.CellSize, TileAssetSet);
		}
	}
}
