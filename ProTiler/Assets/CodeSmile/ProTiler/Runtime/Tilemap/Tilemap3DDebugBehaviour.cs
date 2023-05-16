// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Tilemap
{
	[RequireComponent(typeof(Tilemap3DBehaviour))]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[ExcludeFromCodeCoverage]
	public class Tilemap3DDebugBehaviour : MonoBehaviour
	{
		[SerializeField] private ChunkCoord m_ActiveChunkCoord;
		[SerializeField] private Int32 m_ActiveLayerIndex;
		[SerializeField] private Boolean m_FillChunkLayer;
		[SerializeField] [ReadOnlyField] private Int32 m_MaxLayerIndex;
		[SerializeField] [ReadOnlyField] private Int32 m_TileCount;

		private Tilemap3DBehaviour m_Tilemap;

		[Pure] private static Tile3DCoord[] GetIncrementingIndexChunkTileCoords(ChunkCoord chunkCoord,
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

		private void Awake() => m_Tilemap = GetComponent<Tilemap3DBehaviour>();

		[Pure] private void OnDrawGizmosSelected() => DrawActiveChunkTileIndexes();

		private void OnValidate()
		{
			m_ActiveLayerIndex = Mathf.Max(0, m_ActiveLayerIndex);

			// TODO
			m_MaxLayerIndex = -1;
			UpdateTileCount();

			if (m_FillChunkLayer)
			{
				m_FillChunkLayer = false;
				FillActiveLayerWithIncrementingTiles();
			}
		}

		private void UpdateTileCount() => m_TileCount = m_Tilemap != null ? m_Tilemap.TileCount : -1;

		[Pure] private void DrawActiveChunkTileIndexes()
		{
#if UNITY_EDITOR
			var normalStyle = new GUIStyleState { textColor = Color.yellow };
			var labelStyle = new GUIStyle { fontSize = 11, normal = normalStyle };

			var chunkSize = m_Tilemap.ChunkSize;
			var cellSize = m_Tilemap.Grid.CellSize;
			var halfCellSize = cellSize * .5f;

			var layerCountInChunk = m_Tilemap.GetLayerCount(m_ActiveChunkCoord);
			for (var height = 0; height < layerCountInChunk; height++)
			{
				var coords =
					Tilemap3DUtility.GetAllChunkLayerCoords(m_ActiveChunkCoord, chunkSize, height);
				var tileCoords = m_Tilemap.GetTiles(coords);

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

		[Pure] private void FillActiveLayerWithIncrementingTiles()
		{
			var tileCoords =
				GetIncrementingIndexChunkTileCoords(m_ActiveChunkCoord, m_Tilemap.ChunkSize, m_ActiveLayerIndex);
			m_Tilemap.SetTiles(tileCoords);
			UpdateTileCount();
			Debug.Log($"filled chunk {m_ActiveChunkCoord} layer {m_ActiveLayerIndex} with {tileCoords.Length} tiles");
		}
	}
}
