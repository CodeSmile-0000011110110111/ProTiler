// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using CellSize = UnityEngine.Vector3;

namespace CodeSmile.ProTiler.Rendering
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModel))]
	public partial class Tilemap3DRenderer : MonoBehaviour
	{
		internal const String ActiveRenderersFolderName = "[Active " + nameof(Tile3DRenderer) + "s]";
		internal const String PooledRenderersFolderName = "[Pooled " + nameof(Tile3DRenderer) + "s]";
		internal const String TemplateGameObjectFolderName = "[Template " + nameof(Tile3DRenderer) + "]";
		internal const String TemplateGameObjectName = "*" + nameof(Tile3DRenderer);

		// this needs some work because "DontSave" causes serious issues, eg
		// https://answers.unity.com/questions/609621/hideflagsdontsave-causes-checkconsistency-transfor.html
		private const HideFlags ChildHideFlags = HideFlags.None;

		[SerializeReference] [HideInInspector] private Tilemap3DCullingBase m_Culling;

		protected readonly TileRenderers m_ActiveRenderers = new();
		protected Transform m_ActiveRenderersFolder;
		protected Transform m_PooledRenderersFolder;
		protected Transform m_TemplateGameObjectFolder;
		protected GameObject m_TemplateGameObject;
		protected ComponentPool<Tile3DRenderer> m_ComponentPool;

		private ITile3DAssetSet m_TileAssetSet;
		internal ITile3DAssetSet TileAssetSet
		{
			get
			{
				if (m_TileAssetSet == null)
					m_TileAssetSet = transform.parent.GetComponent<ITile3DAssetSet>();
				return m_TileAssetSet;
			}
		}
		private Tilemap3DModel TilemapModel => GetComponent<Tilemap3DModel>();
		public Tilemap3DCullingBase Culling { get => m_Culling; set => m_Culling = value; }

		private void OnValidate() => UpdateDebugDrawingFlag();

		private void Awake() => CreateDefaultFrustumCulling();
		private void Reset() => CreateDefaultFrustumCulling();

		private void OnEnable()
		{
#if UNITY_EDITOR
			AssemblyReloadEvents.beforeAssemblyReload += ClearTileRenderers;
#endif

			m_ActiveRenderersFolder = GetOrCreateFolder(ActiveRenderersFolderName);
			m_PooledRenderersFolder = GetOrCreateFolder(PooledRenderersFolderName, false);
			m_TemplateGameObjectFolder = GetOrCreateFolder(TemplateGameObjectFolderName, false);

			m_TemplateGameObject = GetOrCreateTemplateGameObject();
			m_ComponentPool = CreateComponentPool();

			RegisterModelEvents();
		}

		private void OnDisable()
		{
			UnregisterModelEvents();
			ClearTileRenderers();
		}

		private void LateUpdate() => CullAndSetVisibleCoords();

		private Transform GetOrCreateFolder(String folderName, Boolean active = true,
			HideFlags hideFlags = HideFlags.None)
		{
			var folder = transform.FindOrCreateChild(folderName, hideFlags);
			folder.gameObject.SetActive(active);
			return folder;
		}

		private GameObject GetOrCreateTemplateGameObject() => m_TemplateGameObjectFolder.childCount > 0
			? GetTemplateGameObject()
			: CreateTemplateGameObject();

		private GameObject GetTemplateGameObject() => m_TemplateGameObjectFolder.GetChild(0).gameObject;

		private GameObject CreateTemplateGameObject() => new(TemplateGameObjectName, typeof(Tile3DRenderer))
		{
			hideFlags = ChildHideFlags,
			transform = { parent = m_TemplateGameObjectFolder },
		};

		private ComponentPool<Tile3DRenderer> CreateComponentPool()
		{
			if (m_ComponentPool != null)
				m_ComponentPool.Dispose();
			return new ComponentPool<Tile3DRenderer>(m_TemplateGameObject,
				m_PooledRenderersFolder.gameObject, 0);
		}

		private void CreateDefaultFrustumCulling() => m_Culling = new Tilemap3DTopDownCulling();

		public void ClearTileRenderers()
		{
			m_ComponentPool.Clear();
			m_ActiveRenderers.Clear();

			if (m_PooledRenderersFolder != null)
				m_PooledRenderersFolder.DestroyAllChildren();
			if (m_ActiveRenderersFolder != null)
				m_ActiveRenderersFolder.DestroyAllChildren();
		}

		private void CullAndSetVisibleCoords()
		{
			if (m_Culling != null)
			{
				var chunkSize = TilemapModel.ChunkSize;
				var cellSize = TilemapModel.Grid.CellSize;
				var visibleCoords = m_Culling.GetVisibleCoords(chunkSize, cellSize);
				SetVisibleCoords(visibleCoords, TilemapModel.Grid.CellSize);
			}
		}

		public void SetVisibleCoords(IEnumerable<GridCoord> visibleCoords, CellSize cellSize)
		{
			GrowComponentPool(visibleCoords.Count());
			ReturnNonVisibleTileRenderersToPool(visibleCoords);
			UpdateVisibleTileRenderers(visibleCoords, cellSize);
		}

		private void GrowComponentPool(Int32 visibleCount)
		{
			if (m_ComponentPool.Count <= visibleCount)
				m_ComponentPool.SetPoolSize(visibleCount);
		}

		private void ReturnNonVisibleTileRenderersToPool(IEnumerable<GridCoord> visibleCoords)
		{
			var culledRenderers = GetCulledRenderers(visibleCoords);
			ReturnCulledTileRenderersToPool(culledRenderers);
			SetCulledTileRenderersAsInactive(culledRenderers);
		}

		private IReadOnlyList<GridCoord> GetCulledRenderers(IEnumerable<GridCoord> visibleCoords) =>
			m_ActiveRenderers.Keys.Where(coord => visibleCoords.Contains(coord) == false).ToList();

		private void ReturnCulledTileRenderersToPool(IReadOnlyList<GridCoord> culledRenderers)
		{
			foreach (var coord in culledRenderers)
			{
				var tileRenderer = m_ActiveRenderers[coord];
				ReturnTileRendererToPool(tileRenderer);
			}
		}

		private void SetCulledTileRenderersAsInactive(IReadOnlyList<GridCoord> culledRenderers)
		{
			for (var i = culledRenderers.Count - 1; i >= 0; i--)
				m_ActiveRenderers.Remove(culledRenderers[i]);
		}

		private void UpdateVisibleTileRenderers(IEnumerable<GridCoord> visibleCoords,
			CellSize cellSize)
		{
			var tileCoords = TilemapModel.GetTiles(visibleCoords);
			foreach (var pair in tileCoords)
			{
				var coord = pair.Key;
				var tileRenderer = GetOrActivateTileRenderer(coord);
				tileRenderer.UpdateTransform(coord, cellSize);

				var tileCoord = pair.Value;
				if (tileRenderer.TileCoord.Tile != tileCoord.Tile)
					tileRenderer.UpdateTileCoord(tileCoord, TileAssetSet);

				tileRenderer.EnableDebugDrawing = m_EnableDebugDrawing;
			}
		}

		private Tile3DRenderer GetOrActivateTileRenderer(GridCoord coord)
		{
			if (m_ActiveRenderers.TryGetValue(coord, out var tileRenderer) == false)
			{
				tileRenderer = GetAndActivatePooledTileRenderer();
				m_ActiveRenderers.Add(coord, tileRenderer);
			}
			return tileRenderer;
		}

		private Tile3DRenderer GetAndActivatePooledTileRenderer()
		{
			var tileRenderer = m_ComponentPool.GetFromPool();
			if (tileRenderer == null)
				Debug.Log($"got null from pool, total count: {m_ComponentPool.Count}");

			tileRenderer.transform.parent = m_ActiveRenderersFolder;
			return tileRenderer;
		}

		private void ReturnTileRendererToPool(Tile3DRenderer tileRenderer)
		{
			m_ComponentPool.ReturnToPool(tileRenderer);
			tileRenderer.transform.parent = m_PooledRenderersFolder;
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

		private void OnTilemapCleared() => ClearTileRenderers();

		private void OnTilemapModified(IEnumerable<Tile3DCoord> tileCoords) => CullAndSetVisibleCoords();

		protected internal sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}
	}
}
