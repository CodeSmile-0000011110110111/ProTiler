// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Collections;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[ExecuteAlways]
	public class Tilemap3D : MonoBehaviour
	{
		[SerializeField] private Vector3 m_TileAnchor;
		[SerializeField] private Vector2Int m_ChunkSize = new(32, 32);

		[Header("Info")]
		[SerializeField] [ReadOnlyField] private int m_TileCount;

		private Vector2Int m_LastChunkSize;
		private Tilemap3DChunkCollection m_Chunks;
		internal Tilemap3DChunkCollection Chunks
		{
			get
			{
				InitChunks();
				return m_Chunks;
			}
		}

		public Grid3D Grid
		{
			get
			{
				if (transform.parent == null)
					return null;

				return transform.parent.GetComponent<Grid3D>();
			}
		}

		private void OnValidate()
		{
			if (m_ChunkSize != m_LastChunkSize)
			{
				Chunks.ChangeChunkSize(m_ChunkSize);
				m_ChunkSize = m_LastChunkSize;
			}
		}

		private void InitChunks()
		{
			if (m_Chunks == null)
			{
				m_Chunks = new Tilemap3DChunkCollection(m_ChunkSize);
				m_TileCount = m_Chunks.Count;
				m_LastChunkSize = m_ChunkSize;
			}
		}

		public void SetTiles(Tile3DCoordData[] tileChangeData) => m_Chunks.SetTiles(tileChangeData);
		public void RefreshTile(Vector3Int coord) => throw new NotImplementedException();

		public void DrawLine(Vector3Int startSelectionCoord, Vector3Int cursorCoord)
		{
			// this.RecordUndoInEditor(DrawBrush.IsClearing ? "Clear Tiles" : "Draw Tiles");
			// var (coords, tiles) = GraphView.Layer.DrawLine(start, end, DrawBrush);
			// this.SetDirtyInEditor();
			//
			// LayerRenderer.RedrawTiles(coords, tiles);
			//DebugUpdateTileCount();
		}

		public void DrawRect(object makeRect) => throw new NotImplementedException();

		public void GetTileData(Vector3Int[] coords, ref Tile3DCoordData[] tileCoordDatas) => m_Chunks.GetTileData(coords, ref tileCoordDatas);
	}
}
