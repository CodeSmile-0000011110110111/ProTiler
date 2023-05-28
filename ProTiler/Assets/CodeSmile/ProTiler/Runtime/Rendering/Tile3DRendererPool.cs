// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
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

		private void GetOrCreatePooledRenderersFolder() => m_PooledRenderersFolder =
			transform.FindOrCreateChild(PooledRenderersFolderName, ChildHideFlags);

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
			//var activeAndVisible = m_ActiveRenderers.Keys.Union(visibleCoords);

			GrowComponentPool(visibleCoords.Count());

			foreach (var coord in visibleCoords)
			{
				var tileRenderer = m_ComponentPool.GetFromPool();
				tileRenderer.transform.parent = m_ActiveRenderersFolder;
				m_ActiveRenderers.Add(coord, tileRenderer);
				// if (m_ActiveRenderers.TryGetValue(coord, out var tileRenderer) == false)
				// {
				// 	// create a new one
				// 	var worldPos = Grid3DUtility.ToWorldPos(coord, cellSize);
				// 	visibleRenderers.Add(coord, GetOrCreateTileRenderer(worldPos));
				// }
			}

			// m_ActiveRenderersFolder = visibleRenderers;
		}

		private void GrowComponentPool(Int32 visibleCount)
		{
			if (m_ComponentPool.Count <= visibleCount)
				m_ComponentPool.SetPoolSize(visibleCount);
		}

		private sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}
	}
}
