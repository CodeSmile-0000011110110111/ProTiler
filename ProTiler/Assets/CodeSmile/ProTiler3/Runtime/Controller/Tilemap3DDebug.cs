// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using CodeSmile.ProTiler3.Runtime.Grid;
using CodeSmile.ProTiler3.Runtime.Model;
using CodeSmile.ProTiler3.Runtime.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using Random = UnityEngine.Random;

namespace CodeSmile.ProTiler3.Runtime.Controller
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModel))]
	public sealed class Tilemap3DDebug : MonoBehaviour
	{
		[SerializeField] private ChunkSize m_ChunkSize = new(8, 8);
		[SerializeField] private ChunkCoord m_ActiveChunkCoord;
		[SerializeField] private Int32 m_ActiveLayerIndex;
		[SerializeField] private Boolean m_ClearTilemap;
		[SerializeField] private Boolean m_FillChunkLayer;
		[SerializeField] private Boolean m_FillChunkLayersFromOrigin;
		[SerializeField] private Boolean m_EnableRendererDebugDrawing;
		[SerializeField] [ReadOnlyField] private Int32 m_TileCount;

		private GameObject m_CursorObject;
		private Grid3DCursor m_Cursor;

		private Tilemap3DModel TilemapModel => GetComponent<Tilemap3DModel>();
		private Tilemap3DViewController TilemapViewController => GetComponent<Tilemap3DViewController>();
		private Grid3DController Grid => TilemapViewController.Grid;

		private void Awake()
		{
#if !UNITY_EDITOR
			Destroy(this);
#endif
		}

		private void Update()
		{
			UpdateTileCount();
			DrawCursorObject();
		}

		private void OnEnable()
		{
			UpdateTileCount();
			TilemapViewController.OnCursorUpdate += OnCursorUpdate;
#if UNITY_EDITOR
			AssemblyReloadEvents.afterAssemblyReload += UpdateRendererDebugDrawing;
#endif
		}

		private void OnDisable() => TilemapViewController.OnCursorUpdate -= OnCursorUpdate;

		[ExcludeFromCodeCoverage]
		private void OnDrawGizmosSelected() =>
			//DrawActiveChunkTileIndexes();
			DrawCursor();

		[ExcludeFromCodeCoverage]
		private void OnValidate()
		{
			m_ActiveLayerIndex = Mathf.Max(0, m_ActiveLayerIndex);

			// prevent occassional "coroutine not started" on disabled objects warnings
			if (enabled && gameObject.activeInHierarchy)
			{
				// prevent issues with doing stuff in OnValidate that isn't allowed (ie Destroy during validate)
				this.WaitForFramesElapsed(1, () =>
				{
					if (m_ClearTilemap)
					{
						m_ClearTilemap = false;
						ClearTilemap();
					}

					if (m_FillChunkLayer)
					{
						m_FillChunkLayer = false;
						FillActiveLayerWithIncrementingTiles(m_ActiveChunkCoord, m_ActiveLayerIndex);
					}

					if (m_FillChunkLayersFromOrigin)
					{
						m_FillChunkLayersFromOrigin = false;
						FillActiveLayerFromOrigin();
					}

					UpdateTileCount();
					UpdateRendererDebugDrawing();
				});
			}
		}

		private void UpdateRendererDebugDrawing()
		{
			var renderer = GetComponent<Tilemap3DRenderer>();
			if (renderer != null)
				renderer.EnableDebugDrawing = m_EnableRendererDebugDrawing;
		}

		private void DrawCursorObject()
		{
			if (m_CursorObject == null)
			{
				const String DebugCursorName = "__DebugCursor__";
				m_CursorObject = GameObject.Find(DebugCursorName);
				if (m_CursorObject == null)
				{
					m_CursorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
					m_CursorObject.name = DebugCursorName;
					m_CursorObject.hideFlags = HideFlags.HideAndDontSave;
					m_CursorObject.transform.parent = transform;
					var scale = Grid.CellSize * 0.99f; // downscale to prevent z-fighting
					m_CursorObject.transform.localScale = scale;
				}
			}

			m_CursorObject.transform.position = m_Cursor.CenterPosition;
		}

		[ExcludeFromCodeCoverage]
		private void OnCursorUpdate(Grid3DCursor cursor) => m_Cursor = cursor;

		[ExcludeFromCodeCoverage]
		private void DrawCursor()
		{
			if (m_Cursor.IsValid)
				Gizmos.DrawWireCube(m_Cursor.CenterPosition, m_Cursor.CellSize);
		}

		[ExcludeFromCodeCoverage]
		private void ClearTilemap() => TilemapModel.ClearTilemap(m_ChunkSize);

		[ExcludeFromCodeCoverage]
		private void UpdateTileCount() => m_TileCount = TilemapModel != null ? TilemapModel.TileCount : -1;

		[ExcludeFromCodeCoverage]
		private void DrawActiveChunkTileIndexes()
		{
#if UNITY_EDITOR
			var normalStyle = new GUIStyleState { textColor = Color.yellow };
			var labelStyle = new GUIStyle { fontSize = 11, normal = normalStyle };

			var tilemapBehaviour = TilemapModel;
			var chunkSize = tilemapBehaviour.ChunkSize;
			var cellSize = Grid.CellSize;
			var halfCellSize = cellSize * .5f;

			var layerCountInChunk = tilemapBehaviour.GetLayerCount(m_ActiveChunkCoord);
			for (var height = 0; height < layerCountInChunk; height++)
			{
				var coords =
					Tilemap3DUtility.GetChunkGridCoords(m_ActiveChunkCoord, chunkSize, height);
				var tileCoords = tilemapBehaviour.GetExistingTiles(coords);

				foreach (var tileCoord in tileCoords)
				{
					var coord = tileCoord.Coord;
					var pos = new Vector3(coord.x * cellSize.x, coord.y * cellSize.y, coord.z * cellSize.z) +
					          halfCellSize;

					Gizmos.DrawWireCube(pos, cellSize);
					if (height == 0)
						Handles.Label(pos, $"{tileCoord.Tile.Index}: {pos}", labelStyle);
				}
			}
#endif
		}

		[ExcludeFromCodeCoverage]
		private void FillActiveLayerWithIncrementingTiles(ChunkCoord chunkCoord, Int32 height)
		{
			var tileCoords = GetRandomIndexChunkTileCoords(chunkCoord, TilemapModel.ChunkSize, height);
			TilemapModel.SetTiles(tileCoords);
			UpdateTileCount();
			Debug.Log($"filled chunk {chunkCoord} layer {height} with {tileCoords.Length} tiles");
		}

		[ExcludeFromCodeCoverage]
		private void FillActiveLayerFromOrigin()
		{
			var allTileCoords = new List<Tile3DCoord>();

			for (var x = 0; x <= m_ActiveChunkCoord.x; x++)
			{
				for (var y = 0; y <= m_ActiveChunkCoord.y; y++)
				{
					for (var h = 0; h <= m_ActiveLayerIndex; h++)
					{
						var coord = new ChunkCoord(x, y);
						var tileCoords =
							GetRandomIndexChunkTileCoords(coord, TilemapModel.ChunkSize, h);
						allTileCoords.AddRange(tileCoords);
					}
				}
			}

			TilemapModel.SetTiles(allTileCoords);
			UpdateTileCount();
		}

		[ExcludeFromCodeCoverage]
		private Tile3DCoord[] GetRandomIndexChunkTileCoords(ChunkCoord chunkCoord,
			ChunkSize chunkSize, Int32 height, Int32 minValue = 1, Int32 maxValue = 33)
		{
			var width = chunkSize.x;
			var length = chunkSize.y;
			var chunkOrigin = chunkCoord * chunkSize;
			var coordDataCount = width * length;
			var tileCoords = new Tile3DCoord[coordDataCount];
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < length; z++)
				{
					var coord = new GridCoord(chunkOrigin.x + x, height, chunkOrigin.y + z);
					var tileIndex = Random.Range(minValue, maxValue);
					tileCoords[x * width + z] = new Tile3DCoord(coord, new Tile3D(tileIndex));
				}
			}
			return tileCoords;
		}
	}
}
