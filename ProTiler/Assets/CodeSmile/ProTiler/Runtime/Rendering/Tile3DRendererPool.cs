// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;
using Object = UnityEngine.Object;
using WorldPos = UnityEngine.Vector3;

namespace CodeSmile.ProTiler.Rendering
{
	internal sealed class Tile3DRendererPool : IDisposable
	{
		internal static readonly String RendererFolderName = $"[{nameof(Tile3DRenderer)}s]";
		private readonly GameObject m_ParentObject;

		private TileRenderers m_ActiveRenderers = new();
		private GameObject m_RendererTemplate;
		private ComponentPool<Tile3DRenderer> m_RendererPool;
		private Transform m_RendererFolder;

		private Transform RendererFolder
		{
			get
			{
				if (m_RendererFolder == null)
					InitRendererFolder();

				return m_RendererFolder;
			}
		}

		public Tile3DRendererPool(GameObject parentObject)
		{
			m_ParentObject = parentObject;
			InitRendererFolder();
			InitRendererTemplate();
		}

		public void Dispose() => DestroyRendererFolder();

		private void InitRendererTemplate()
		{
			m_RendererTemplate = m_ParentObject.FindOrCreateChild($"*{nameof(Tile3DRenderer)}", HideFlags.DontSave);
			m_RendererTemplate.GetOrAddComponent<Tile3DRenderer>();
		}

		private void InitRendererFolder()
		{
			var folderObject = m_ParentObject.FindOrCreateChild(RendererFolderName, HideFlags.DontSave);
			m_RendererFolder = folderObject.transform;
		}

		private void DestroyRendererFolder()
		{
			if (m_RendererFolder != null)
			{
				m_RendererFolder.DestroyInAnyMode();
				m_RendererFolder = null;
			}
		}

		internal void UpdateModifiedTiles(IEnumerable<Tile3DCoord> tileCoords, CellSize cellSize,
			ITile3DAssetIndexer prefabLookup)
		{
			foreach (var tileCoord in tileCoords)
			{
				if (m_ActiveRenderers.TryGetValue(tileCoord.Coord, out var tileRenderer))
					tileRenderer.OnTileModified(tileCoord, prefabLookup);
			}
		}

		internal void UpdateActiveRenderers(IEnumerable<GridCoord> visibleCoords, in CellSize cellSize)
		{
			// set visible
			var visibleRenderers = new TileRenderers();
			foreach (var coord in visibleCoords)
			{
				if (m_ActiveRenderers.TryGetValue(coord, out var tileRenderer))
					visibleRenderers.Add(coord, tileRenderer);
				else
				{
					// create a new one
					var worldPos = Grid3DUtility.ToWorldPos(coord, cellSize);
					visibleRenderers.Add(coord, GetOrCreateTileRenderer(worldPos));
				}
			}

			m_ActiveRenderers = visibleRenderers;
		}

		private Tile3DRenderer GetOrCreateTileRenderer(WorldPos worldPosition)
		{
			var go = Object.Instantiate(m_RendererTemplate, worldPosition, Quaternion.identity, RendererFolder);
			go.hideFlags = HideFlags.DontSave;
			//Debug.Log($"Created: {go} in {RendererFolder} at pos {worldPosition}");
			return go.GetComponent<Tile3DRenderer>();
		}

		private sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}

		public void Clear()
		{
			m_ActiveRenderers.Clear();
			//m_RendererPool.Clear();
			RendererFolder.DestroyAllChildren();
		}
	}
}
