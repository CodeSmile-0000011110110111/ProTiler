// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Collections;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[ExecuteAlways]
	public class Tilemap3D : MonoBehaviour
	{
		//[SerializeField] private Vector3 m_TileAnchor;
		[SerializeField] private Vector2Int m_ChunkSize = new(16, 16);
		[SerializeField] private Tilemap3DChunkCollection m_Chunks;

		private Vector2Int m_CurrentChunkSize;
		public Vector2Int ChunkSize
		{
			get => m_ChunkSize;
			set => SetChunkSize(value);
		}
		public Tilemap3DChunkCollection Chunks => m_Chunks;

		public Grid3D Grid
		{
			get
			{
#if DEBUG
				if (transform.parent == null)
					throw new NullReferenceException($"{nameof(Tilemap3D)} has no parent");
#endif

				return transform.parent.GetComponent<Grid3D>();
			}
		}

		private void Awake() => SetChunkSize(m_ChunkSize);

		private void Reset() => SetChunkSize(m_ChunkSize);

		private void OnValidate() => SetChunkSize(m_ChunkSize);

		private void SetChunkSize(Vector2Int chunkSize)
		{
			Tilemap3DChunkCollection.ClampChunkSize(ref chunkSize);
			if (chunkSize != m_CurrentChunkSize)
			{
				m_ChunkSize = m_CurrentChunkSize = chunkSize;

				InitChunks();
				Chunks.ChangeChunkSize(m_ChunkSize);
			}
		}

		private void InitChunks()
		{
			if (m_Chunks == null)
			{
				m_Chunks = new Tilemap3DChunkCollection(m_ChunkSize);
				m_CurrentChunkSize = m_ChunkSize;
			}
		}

		public Tile3DData GetTile(Vector3Int coord)
		{
			var tileDatas = new Tile3DCoordData[1];
			GetTiles(new[] { coord }, ref tileDatas);
			return tileDatas[0].TileData;
		}

		public void GetTiles(Vector3Int[] coords, ref Tile3DCoordData[] tileCoordDatas) => m_Chunks.GetTiles(coords, ref tileCoordDatas);

		public void SetTile(Vector3Int coord, Tile3DData tileData) => SetTiles(new[] { Tile3DCoordData.New(coord, tileData) });

		public void SetTiles(Tile3DCoordData[] tileCoordDatas)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoordDatas);
			this.SetDirtyInEditor();
		}

		public void SetTilesNoUndo(Tile3DCoordData[] tileCoordDatas) => m_Chunks.SetTiles(tileCoordDatas);

		/*
		public void RefreshTile(Vector3Int coord) => throw new NotImplementedException();

		public void DrawLine(Vector3Int startSelectionCoord, Vector3Int cursorCoord) => throw new NotImplementedException();

		public void DrawRect(object makeRect) => throw new NotImplementedException();
	*/
	}
}
