// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModel))]
	public sealed class Tilemap3DDebugController : MonoBehaviour
	{
		[SerializeField] private ChunkSize m_ChunkSize = new(8, 8);
		[SerializeField] private ChunkCoord m_ActiveChunkCoord;
		[SerializeField] private Int32 m_ActiveLayerIndex;
		[SerializeField] private Boolean m_CreateTilemap;
		[SerializeField] private Boolean m_FillChunkLayer;
		[SerializeField] private Boolean m_FillChunkLayersFromOrigin;
		[SerializeField] [ReadOnlyField] private Int32 m_TileCount;

		private GameObject m_CursorObject;
		private Grid3DCursor m_Cursor;

		[Pure] private Tilemap3DModel TilemapModel => GetComponent<Tilemap3DModel>();
		[Pure] private Tilemap3DViewController TilemapViewController => GetComponent<Tilemap3DViewController>();
		[Pure] private Grid3DController Grid => TilemapViewController.Grid;

		[Pure] private void Awake()
		{
#if !UNITY_EDITOR
			Destroy(this);
#endif
		}

		[Pure] private void Update()
		{
			UpdateTileCount();
			DrawCursorObject();
		}

		[Pure] private void OnEnable()
		{
			UpdateTileCount();
			TilemapViewController.OnCursorUpdate += OnCursorUpdate;
		}

		[Pure] private void OnDisable() => TilemapViewController.OnCursorUpdate -= OnCursorUpdate;

		[ExcludeFromCodeCoverage]
		[Pure] private void OnDrawGizmosSelected()
		{
			DrawActiveChunkTileIndexes();
			DrawCursor();
		}

		[ExcludeFromCodeCoverage]
		private void OnValidate()
		{
			m_ActiveLayerIndex = Mathf.Max(0, m_ActiveLayerIndex);

			if (m_CreateTilemap)
			{
				m_CreateTilemap = false;
				CreateMap();
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
					m_CursorObject.transform.parent = transform;
					var scale = Grid.CellSize;
					//scale.y = 0.01f;
					m_CursorObject.transform.localScale = scale;
				}
			}

			m_CursorObject.transform.position = m_Cursor.CenterPosition;
		}

		[ExcludeFromCodeCoverage]
		private void OnCursorUpdate(Grid3DCursor cursor) => m_Cursor = cursor;

		[ExcludeFromCodeCoverage]
		[Pure] private void DrawCursor()
		{
			if (m_Cursor.IsValid)
				Gizmos.DrawWireCube(m_Cursor.CenterPosition, m_Cursor.CellSize);
		}

		[ExcludeFromCodeCoverage]
		[Pure] private void CreateMap() => TilemapModel.ClearTilemap(m_ChunkSize);

		[ExcludeFromCodeCoverage]
		private void UpdateTileCount()
		{
			if (enabled && gameObject.activeInHierarchy)
			{
				this.WaitForFramesElapsed(1, () =>
				{
					m_TileCount = TilemapModel != null ? TilemapModel.TileCount : -1;
				});
			}
		}

		[ExcludeFromCodeCoverage]
		[Pure] private void DrawActiveChunkTileIndexes()
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
					Tilemap3DUtility.GetAllChunkLayerCoords(m_ActiveChunkCoord, chunkSize, height);
				var tileCoords = tilemapBehaviour.GetTiles(coords);

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
		[Pure] private void FillActiveLayerWithIncrementingTiles(Vector2Int chunkCoord, Int32 height)
		{
			var tileCoords = GetIncrementingIndexChunkTileCoords(chunkCoord, TilemapModel.ChunkSize, height);
			TilemapModel.SetTiles(tileCoords);
			UpdateTileCount();
			Debug.Log($"filled chunk {chunkCoord} layer {height} with {tileCoords.Length} tiles");
		}

		[ExcludeFromCodeCoverage]
		[Pure] private void FillActiveLayerFromOrigin()
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
							GetIncrementingIndexChunkTileCoords(coord, TilemapModel.ChunkSize, h);
						allTileCoords.AddRange(tileCoords);
					}
				}
			}

			TilemapModel.SetTiles(allTileCoords);
			UpdateTileCount();
		}

		[ExcludeFromCodeCoverage]
		[Pure] private Tile3DCoord[] GetIncrementingIndexChunkTileCoords(ChunkCoord chunkCoord,
			ChunkSize chunkSize, Int32 height)
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
					var tileIndex = Grid3DUtility.ToIndex2D(x, z, width);
					tileCoords[tileIndex] = new Tile3DCoord(coord, new Tile3D(tileIndex + 1));
				}
			}
			return tileCoords;
		}
	}
}
