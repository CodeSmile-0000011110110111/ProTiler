// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;
using Object = UnityEngine.Object;
using TileIndex = System.UInt16;

namespace CodeSmile.ProTiler.Rendering
{
	internal sealed class Tile3DRendererPool : IDisposable
	{
		private static readonly String RendererFolderName = $"[{nameof(Tile3DRenderer)}s]";
		private TileRenderers m_ActiveRenderers = new();

		private GameObject m_RendererTemplate;
		private ComponentPool<Tile3DRenderer> m_RendererPool;
		private Transform m_RendererFolder;
		private readonly GameObject m_TilemapObject;
		private Transform RendererFolder
		{
			get
			{
				if (m_RendererFolder == null)
					InitRendererFolder();

				return m_RendererFolder;
			}
		}

		public Tile3DRendererPool(GameObject tilemapObject)
		{
			m_TilemapObject = tilemapObject;
			InitRendererFolder();
			InitRendererTemplate();
		}

		public void Dispose() => RendererFolder.DestroyAllChildren();

		private void InitRendererTemplate()
		{
			m_RendererTemplate = m_TilemapObject.FindOrCreateChild($"*{nameof(Tile3DRenderer)}", HideFlags.DontSave);
			m_RendererTemplate.GetOrAddComponent<Tile3DRenderer>();
		}

		private void InitRendererFolder()
		{
			var folderObject = m_TilemapObject.FindOrCreateChild(RendererFolderName, HideFlags.DontSave);
			m_RendererFolder = folderObject.transform;
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

		internal void UpdateActiveRenderers(IEnumerable<Vector3Int> visibleCoords, in CellSize cellSize)
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

		private Tile3DRenderer GetOrCreateTileRenderer(Vector3 worldPosition)
		{
			var go = Object.Instantiate(m_RendererTemplate, worldPosition, Quaternion.identity, RendererFolder);
			go.hideFlags = HideFlags.DontSave;
			return go.GetComponent<Tile3DRenderer>();
		}

		private sealed class TileRenderers : Dictionary<GridCoord, Tile3DRenderer> {}
	}

	public interface ITile3DAssetIndexer
	{
		public Tile3DAssetBase this[TileIndex tileIndex] { get; }
	}

	[ExecuteAlways]
	[RequireComponent(typeof(Tilemap3DModel))]
	[DisallowMultipleComponent]
	public sealed class Tilemap3DRenderer : MonoBehaviour
	{
		[SerializeReference] [HideInInspector] private Tilemap3DFrustumCullingBase m_FrustumCulling;

		private Tile3DRendererPool m_TileRendererPool;
		private Tile3DAssetSet m_TileAssetSet;
		private Tile3DRendererPool TileRendererPool
		{
			get
			{
				if (m_TileRendererPool == null)
					CreateTileRendererPool();
				return m_TileRendererPool;
			}
		}
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
		public Tilemap3DFrustumCullingBase FrustumCulling { get => m_FrustumCulling; set => m_FrustumCulling = value; }

		private void Init()
		{
#if UNITY_EDITOR
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
			CreateDefaultFrustumCulling();
			CreateTileRendererPool();
		}

		private void Awake() => Init();
		private void Reset() => Init();
		private void OnEnable() => RegisterModelEvents();
		private void OnDisable() => UnregisterModelEvents();

		private void OnRenderObject()
		{
			var visibleCoords = m_FrustumCulling.GetVisibleCoords();
			TileRendererPool.UpdateActiveRenderers(visibleCoords, TilemapModel.Grid.CellSize);
		}

#if UNITY_EDITOR
		private void OnBeforeAssemblyReload() => DisposeTileRendererPool();
#endif

		private void CreateDefaultFrustumCulling() => m_FrustumCulling = new Tilemap3DFrustumCulling();

		private void CreateTileRendererPool()
		{
			DisposeTileRendererPool();
			m_TileRendererPool = new Tile3DRendererPool(gameObject);
		}

		private void DisposeTileRendererPool()
		{
			if (m_TileRendererPool != null)
				m_TileRendererPool.Dispose();
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

		private void OnTilemapCleared() => CreateTileRendererPool();

		private void OnTilemapModified(IEnumerable<Tile3DCoord> tileCoords) =>
			m_TileRendererPool.UpdateModifiedTiles(tileCoords, TilemapModel.Grid.CellSize, TileAssetSet);
	}
}

//[SerializeField] private int m_DrawDistance = 20;
// private Vector3Int[] m_VisibleCoords;
// private Tile3DCoord[] m_TileCoordDatas;

//private Tilemap3D Tilemap => GetComponent<Tilemap3D>();

/*
 * Todo:
 *
 * decisions:
 *
 * Prio 1:
 *	work on Tile3D assets and pooling
 *  connect Tile3DData.TileIndex with an indexed Tile3D asset from pool
 *  create Tile3DAsset from selected prefab(s)
 *
 * Prio 2:
 *	ComponentPool tests
 *  pooling strategies (increase, instantiate/destroy, shrink)
 *
 *
 * get the visible tile coords from a callback
 * get the tiles from the tilemap using those coordinates
 * instantiate/update the pooled tile proxy objects with tiledata
 *
 * needs a pool of RenderTile3D
 *	size equals visible coords
 *  if visible coord count changes, pool only expands automatically but won't shrink
 *	pool can be shrinked through manual set size call
 *
 * needs a pool of Tile3D prefab instances
 *  holds instances of previously instantiated tile instances of a given index (ie cache)
 *	reasonable base count, expands automatically, shrinks if called manually
 *
 *
 * tile proxy:
 * get the prefab from the objectset using TileIndex
 * instantiate prefab instance if it changed
 *
 */

/*
private void OnRenderObject()
{
	if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
		return;

	if (m_VisibleCoords == null)
	{
		m_VisibleCoords = new Vector3Int[20 * 20];
		m_TileCoordDatas = new Tile3DCoord[m_VisibleCoords.Length];
	}

	UpdateVisibleTileCoordinates(ref m_VisibleCoords);
	UpdateVisibleTiles();
}

private void UpdateVisibleTiles()
{
	Tilemap.GetTiles(m_VisibleCoords, ref m_TileCoordDatas);

	// TODO dont destroy children, implement pooling
	transform.DestroyAllChildren();

	GameObject instance = null;

	Debug.LogWarning("TODO: get prefab from tiledata");
	foreach (var tile3DCoordData in m_TileCoordDatas)
	{
		var tile = tile3DCoordData.TileData.Tile;
		if (tile == null || tile.Prefab == null)
			continue;

		if (instance != null)
			DestroyImmediate(instance);

		instance = Instantiate(tile.Prefab, transform);

	}
}

private void UpdateVisibleTileCoordinates(ref Vector3Int[] coords)
{
	for (var x = 0; x < 20; x++)
	{
		for (var y = 0; y < 20; y++)
		{
			var index = Grid3DUtility.ToIndex2D(x, y, 20);
			coords[index].x = x;
			coords[index].y = 0;
			coords[index].z = y;
		}
	}
}
*/
