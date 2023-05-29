// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using CellSize = UnityEngine.Vector3;

namespace CodeSmile.ProTiler.Rendering
{
	[ExecuteAlways]
	[AddComponentMenu("")] // hide from Add Component list
	public class Tile3DRendererPool : MonoBehaviour
	{
		internal const String ActiveRenderersFolderName = "[Active " + nameof(Tile3DRenderer) + "s]";
		internal const String PooledRenderersFolderName = "[Pooled " + nameof(Tile3DRenderer) + "s]";
		internal const String TemplateGameObjectFolderName = "[Template " + nameof(Tile3DRenderer) + "]";
		internal const String TemplateGameObjectName = "*" + nameof(Tile3DRenderer);

		private const HideFlags ChildHideFlags = HideFlags.DontSave;

		protected readonly TileRenderers m_ActiveRenderers = new();

		protected Transform m_ActiveRenderersFolder;
		protected Transform m_PooledRenderersFolder;
		protected Transform m_TemplateGameObjectFolder;
		protected GameObject m_TemplateGameObject;
		protected ComponentPool<Tile3DRenderer> m_ComponentPool;

		[Pure] private void OnEnable()
		{
#if UNITY_EDITOR
			AssemblyReloadEvents.beforeAssemblyReload += Clear;
#endif

			m_ActiveRenderersFolder = GetOrCreateFolder(ActiveRenderersFolderName, true, ChildHideFlags);
			m_PooledRenderersFolder = GetOrCreateFolder(PooledRenderersFolderName, false, ChildHideFlags);
			m_TemplateGameObjectFolder = GetOrCreateFolder(TemplateGameObjectFolderName, false, ChildHideFlags);

			m_TemplateGameObject = GetOrCreateTemplateGameObject();
			CreateComponentPool();
		}

		[Pure] private void OnDisable() => Clear();

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

		private void CreateComponentPool() => m_ComponentPool = new ComponentPool<Tile3DRenderer>(m_TemplateGameObject,
			m_PooledRenderersFolder.gameObject, 0, ChildHideFlags);

		[Pure] public void Clear()
		{
			m_ComponentPool.Clear();
			m_ActiveRenderers.Clear();
			m_PooledRenderersFolder.DestroyAllChildren();
			m_ActiveRenderersFolder.DestroyAllChildren();
		}

		[Pure] public void SetVisibleCoords(IEnumerable<GridCoord> visibleCoords, CellSize cellSize)
		{
			GrowComponentPool(visibleCoords.Count());
			ReturnNonVisibleTileRenderersToPool(visibleCoords);
			TakeFromPoolAndUpdateVisibleTileRenderers(visibleCoords, cellSize);
		}

		[Pure] private void GrowComponentPool(Int32 visibleCount)
		{
			if (m_ComponentPool.Count <= visibleCount)
				m_ComponentPool.SetPoolSize(visibleCount);
		}

		[Pure] private void ReturnNonVisibleTileRenderersToPool(IEnumerable<GridCoord> visibleCoords)
		{
			var culledRenderers = GetCulledRenderers(visibleCoords);
			ReturnCulledTileRenderersToPool(culledRenderers);
			SetCulledTileRenderersAsInactive(culledRenderers);
		}

		[Pure] private IReadOnlyList<GridCoord> GetCulledRenderers(IEnumerable<GridCoord> visibleCoords) =>
			m_ActiveRenderers.Keys.Where(coord => visibleCoords.Contains(coord) == false).ToList();

		[Pure] private void ReturnCulledTileRenderersToPool(IReadOnlyList<GridCoord> culledRenderers)
		{
			foreach (var coord in culledRenderers)
			{
				var tileRenderer = m_ActiveRenderers[coord];
				ReturnTileRendererToPool(tileRenderer);
			}
		}

		[Pure] private void SetCulledTileRenderersAsInactive(IReadOnlyList<GridCoord> culledRenderers)
		{
			for (var i = culledRenderers.Count - 1; i >= 0; i--)
				m_ActiveRenderers.Remove(culledRenderers[i]);
		}

		[Pure] private void TakeFromPoolAndUpdateVisibleTileRenderers(IEnumerable<GridCoord> visibleCoords,
			CellSize cellSize)
		{
			foreach (var coord in visibleCoords)
			{
				var tileRenderer = GetOrActivateTileRenderer(coord);
				UpdateTileRendererTransform(tileRenderer, coord, cellSize);
			}
		}

		[Pure] private Tile3DRenderer GetOrActivateTileRenderer(GridCoord coord)
		{
			if (m_ActiveRenderers.TryGetValue(coord, out var tileRenderer) == false)
			{
				tileRenderer = GetAndActivatePooledTileRenderer();
				m_ActiveRenderers.Add(coord, tileRenderer);
			}
			return tileRenderer;
		}

		[Pure] private Tile3DRenderer GetAndActivatePooledTileRenderer()
		{
			var tileRenderer = m_ComponentPool.GetFromPool();
			if (tileRenderer == null)
				Debug.Log($"got null from pool, total count: {m_ComponentPool.Count}");

			tileRenderer.transform.parent = m_ActiveRenderersFolder;
			return tileRenderer;
		}

		[Pure] private void ReturnTileRendererToPool(Tile3DRenderer tileRenderer)
		{
			m_ComponentPool.ReturnToPool(tileRenderer);
			tileRenderer.transform.parent = m_PooledRenderersFolder;
		}

		[Pure] private void UpdateTileRendererTransform(Tile3DRenderer tileRenderer, GridCoord coord, CellSize cellSize)
		{
			var tileRendererTransform = tileRenderer.transform;
			tileRendererTransform.position = Grid3DUtility.ToWorldPos(coord, cellSize);
			tileRendererTransform.localScale = cellSize;
		}

		protected internal sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}
	}
}
