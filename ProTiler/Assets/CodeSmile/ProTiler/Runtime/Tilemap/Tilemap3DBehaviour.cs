// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Tilemap
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class Tilemap3DBehaviour : MonoBehaviour, ISerializationCallbackReceiver
	{
		//[SerializeField] private Vector3 m_TileAnchor;
		[SerializeField] private Vector2Int m_ChunkSize = new(16, 16);
		private Tilemap3D m_Map;

		public Vector2Int ChunkSize
		{
			get => m_Map.ChunkSize;
			set => SetChunkSize(value);
		}

		internal int TileCount => m_Map.TileCount;

		public Grid3DBehaviour Grid => transform.parent.GetComponent<Grid3DBehaviour>();

		public void OnBeforeSerialize()
		{
			if (m_Map != null)
			{

			}
		}

		public void OnAfterDeserialize() {}

		private void Awake() => SetChunkSize(m_ChunkSize);

		private void Reset() => SetChunkSize(m_ChunkSize);

		private void OnValidate() => SetChunkSize(m_ChunkSize);

		public int GetLayerCount(ChunkCoord chunkCoord) => m_Map.GetLayerCount(chunkCoord);

		private void CreateMap(ChunkSize chunkSize)
		{
			Debug.Log($"Creating tilemap with size {chunkSize}");
			m_Map = new Tilemap3D(chunkSize);
		}

		private void SetChunkSize(ChunkSize chunkSize)
		{
			var clampedChunkSize = Tilemap3DUtility.ClampChunkSize(chunkSize);
			if (m_Map == null || clampedChunkSize != m_Map.ChunkSize)
			{
				m_ChunkSize = clampedChunkSize;
				CreateMap(m_ChunkSize);
			}
		}

		public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;

		public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => m_Map.GetTiles(coords);

		public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		public void SetTiles(IEnumerable<Tile3DCoord> tileCoordDatas)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoordDatas);
			this.SetDirtyInEditor();
		}

		internal void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoordDatas) => m_Map.SetTiles(tileCoordDatas);
	}
}
