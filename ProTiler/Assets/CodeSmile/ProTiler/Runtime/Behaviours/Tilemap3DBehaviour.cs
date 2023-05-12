// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Data;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeSmile.ProTiler.Behaviours
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class Tilemap3DBehaviour : MonoBehaviour, ISerializationCallbackReceiver
	{
		//[SerializeField] private Vector3 m_TileAnchor;
		[SerializeField] private Vector2Int m_ChunkSize = new(16, 16);
		[SerializeField] private _OldTilemap3D m_Chunks;

		private Vector2Int m_CurrentChunkSize;

		/*
		public void RefreshTile(Vector3Int coord) => throw new NotImplementedException();

		public void DrawLine(Vector3Int startSelectionCoord, Vector3Int cursorCoord) => throw new NotImplementedException();

		public void DrawRect(object makeRect) => throw new NotImplementedException();
	*/

		public Vector2Int ChunkSize
		{
			get => m_ChunkSize;
			set => SetChunkSize(value);
		}
		public _OldTilemap3D Chunks => m_Chunks;

		public Grid3DBehaviour Grid => transform.parent.GetComponent<Grid3DBehaviour>();

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		private void Awake() => SetChunkSize(m_ChunkSize);

		private void Reset() => SetChunkSize(m_ChunkSize);

		private void OnValidate()
		{
			SetChunkSize(m_ChunkSize);
		}

		private void SetChunkSize(Vector2Int chunkSize)
		{
			_OldTilemap3D.ClampChunkSize(ref chunkSize);
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
				m_Chunks = new _OldTilemap3D(m_ChunkSize);
				m_CurrentChunkSize = m_ChunkSize;
			}
		}

		public Tile3D GetTile(Vector3Int coord)
		{
			var tileDatas = new Tile3DCoord[1];
			GetTiles(new[] { coord }, ref tileDatas);
			return tileDatas[0].Tile;
		}

		public void GetTiles(Vector3Int[] coords, ref Tile3DCoord[] tileCoordDatas) =>
			m_Chunks.GetTiles(coords, ref tileCoordDatas);

		public void SetTile(Vector3Int coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		public void SetTiles(Tile3DCoord[] tileCoordDatas)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoordDatas);
			this.SetDirtyInEditor();
		}

		public void SetTilesNoUndo(Tile3DCoord[] tileCoordDatas) => m_Chunks.SetTiles(tileCoordDatas);
	}
}
