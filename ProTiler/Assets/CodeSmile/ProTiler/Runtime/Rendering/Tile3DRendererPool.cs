// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using CellSize = UnityEngine.Vector3;

namespace CodeSmile.ProTiler.Rendering
{
	[AddComponentMenu("")] // hide from Add Component list
	public class Tile3DRendererPool : MonoBehaviour
	{
		internal const String ActiveRenderersFolderName = "[Active " + nameof(Tile3DRenderer) + "s]";
		internal const String PooledRenderersFolderName = "[Pooled " + nameof(Tile3DRenderer) + "s]";
		internal const String TemplateGameObjectName = "*" + nameof(Tile3DRenderer);

		private const HideFlags ChildHideFlags = HideFlags.DontSave;
		internal const Int32 InitialPoolSize = 32;

		private readonly TileRenderers m_ActiveRenderers = new();

		protected Transform m_ActiveRenderersFolder;
		protected Transform m_PooledRenderersFolder;
		protected GameObject m_TemplateGameObject;
		protected ComponentPool<Tile3DRenderer> m_ComponentPool;

		private void OnEnable()
		{
			GetOrCreateActiveRenderersFolder();
			GetOrCreatePooledRenderersFolder();
			CreateTemplateGameObject();
			CreateComponentPool();
		}

		private void OnDisable()
		{
			DisposeComponentPool();
			DestroyActiveRenderersFolder();
			DestroyPooledRenderersFolder();
		}

		private void GetOrCreateActiveRenderersFolder() => m_ActiveRenderersFolder =
			transform.FindOrCreateChild(ActiveRenderersFolderName, ChildHideFlags);

		private void GetOrCreatePooledRenderersFolder()
		{
			m_PooledRenderersFolder = transform.FindOrCreateChild(PooledRenderersFolderName, ChildHideFlags);
			m_PooledRenderersFolder.gameObject.SetActive(false);
		}

		private void DestroyActiveRenderersFolder()
		{
			if (m_ActiveRenderersFolder != null)
			{
				m_ActiveRenderersFolder.DestroyInAnyMode();
				m_ActiveRenderersFolder = null;
			}
		}

		private void DestroyPooledRenderersFolder()
		{
			if (m_PooledRenderersFolder != null)
			{
				m_PooledRenderersFolder.DestroyInAnyMode();
				m_PooledRenderersFolder = null;
			}
		}

		private void CreateTemplateGameObject()
		{
			m_TemplateGameObject = gameObject.FindOrCreateChild(TemplateGameObjectName, ChildHideFlags);
			m_TemplateGameObject.GetOrAddComponent<Tile3DRenderer>();
		}

		private void CreateComponentPool() => m_ComponentPool =
			new ComponentPool<Tile3DRenderer>(m_TemplateGameObject, m_PooledRenderersFolder.gameObject,
				InitialPoolSize, ChildHideFlags);

		private void DisposeComponentPool()
		{
			if (m_ComponentPool != null)
			{
				m_ComponentPool.Dispose();
				m_ComponentPool = null;
			}
		}

		public void SetVisibleCoords(IEnumerable<GridCoord> visibleCoords, CellSize cellSize)
		{
			GrowComponentPool(visibleCoords.Count());
			UpdateVisibleTileRenderers(visibleCoords, cellSize);
			DeactivateNonVisibleTileRenderers(visibleCoords);
		}

		private void GrowComponentPool(Int32 visibleCount)
		{
			if (m_ComponentPool.Count <= visibleCount)
				m_ComponentPool.SetPoolSize(visibleCount);
		}

		private void UpdateVisibleTileRenderers(IEnumerable<Vector3Int> visibleCoords, Vector3 cellSize)
		{
			foreach (var coord in visibleCoords)
			{
				if (m_ActiveRenderers.TryGetValue(coord, out var tileRenderer) == false)
				{
					tileRenderer = GetPooledTileRenderer();
					m_ActiveRenderers.Add(coord, tileRenderer);
				}

				SetTileRendererPositionAndScale(tileRenderer, coord, cellSize);
			}
		}

		private void DeactivateNonVisibleTileRenderers(IEnumerable<Vector3Int> visibleCoords)
		{
			foreach (var lastVisibleCoord in m_ActiveRenderers.Keys.Where(lastVisibleCoord =>
				         visibleCoords.Contains(lastVisibleCoord) == false))
			{
				var tileRenderer = m_ActiveRenderers[lastVisibleCoord];
				ReturnTileRendererToPool(tileRenderer);
			}
		}

		private void SetTileRendererPositionAndScale(Tile3DRenderer tileRenderer, Vector3Int coord, Vector3 cellSize)
		{
			var tileRendererTransform = tileRenderer.transform;
			tileRendererTransform.position = Grid3DUtility.ToWorldPos(coord, cellSize);
			tileRendererTransform.localScale = cellSize;
		}

		private Tile3DRenderer GetPooledTileRenderer()
		{
			var tileRenderer = m_ComponentPool.GetFromPool();
			SetActiveFolderAsParent(tileRenderer);
			return tileRenderer;
		}

		private void ReturnTileRendererToPool(Tile3DRenderer tileRenderer)
		{
			m_ComponentPool.ReturnToPool(tileRenderer);
			SetPoolFolderAsParent(tileRenderer);
		}

		private void SetActiveFolderAsParent(Tile3DRenderer tileRenderer) =>
			tileRenderer.transform.parent = m_ActiveRenderersFolder;

		private void SetPoolFolderAsParent(Tile3DRenderer tileRenderer) =>
			tileRenderer.transform.parent = m_PooledRenderersFolder;

		private sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}
	}
}
