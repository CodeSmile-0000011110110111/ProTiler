// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Tilemap
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public partial class Tilemap3DBehaviour : MonoBehaviour
	{
		[SerializeField] private Vector2Int m_ChunkSize = new(16, 16);
		[SerializeField] private Tilemap3D m_Tilemap;
		[SerializeField] private Serialization m_Serialization = new();
		[Pure] internal Vector2Int ChunkSize
		{
			get => m_Tilemap.ChunkSize;
			set => UpdateChunkSize(value);
		}
		[Pure] internal Int32 ChunkCount => m_Tilemap.ChunkCount;
		[Pure] internal Int32 TileCount => m_Tilemap.TileCount;
		[Pure] public Grid3DBehaviour Grid => transform.parent.GetComponent<Grid3DBehaviour>();

		[Pure] private static Vector2Int ClampChunkSize(Vector2Int chunkSize) =>
			Tilemap3DUtility.ClampChunkSize(chunkSize);

		[Pure] private void Awake() => UpdateChunkSize(ClampChunkSize(m_ChunkSize));
		[Pure] private void Reset() => UpdateChunkSize(ClampChunkSize(m_ChunkSize));
		[Pure] private void OnEnable() => RegisterEditorSceneEvents();
		[Pure] private void OnDisable() => UnregisterEditorSceneEvents();
		[Pure] private void OnValidate() => UpdateChunkSize(ClampChunkSize(m_ChunkSize));
		private void LoadTilemap() => m_Tilemap = m_Serialization.DeserializeTilemap();
		[Pure] private void SaveTilemap() => m_Serialization.SerializeTilemap(m_Tilemap);

		[Pure] private void UpdateChunkSize(ChunkSize chunkSize)
		{
			if (ChunkSizeChanged(chunkSize))
			{
				m_ChunkSize = chunkSize;
				CreateMap(m_ChunkSize);
			}
		}

		[Pure] private Boolean ChunkSizeChanged(Vector2Int chunkSize) => IsTilemapChunkSizeEqualTo(chunkSize) == false;

		[Pure] private Boolean IsTilemapChunkSizeEqualTo(Vector2Int clampedChunkSize) =>
			m_Tilemap != null && clampedChunkSize == m_Tilemap.ChunkSize;

		private void CreateMap(ChunkSize chunkSize) => m_Tilemap = new Tilemap3D(chunkSize);
		[Pure] public Int32 GetLayerCount(ChunkCoord chunkCoord) => m_Tilemap.GetLayerCount(chunkCoord);
		[Pure] public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;
		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => m_Tilemap.GetTiles(coords);
		[Pure] public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoords);
			this.SetDirtyInEditor();
		}

		[Pure] private void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoords) => m_Tilemap.SetTiles(tileCoords);

		[FullCovered] [Serializable]
		private sealed class Serialization
		{
			private const Boolean m_UseBinarySerialization = true;
			[SerializeField] private Byte[] m_SerializedTilemap;

			[Pure] internal Tilemap3D DeserializeTilemap() => m_UseBinarySerialization
				? Tilemap3DSerialization.FromBinary(m_SerializedTilemap)
				: Tilemap3DSerialization.FromJson(Encoding.UTF8.GetString(m_SerializedTilemap));

			[Pure] internal void SerializeTilemap(Tilemap3D tilemap) => m_SerializedTilemap = m_UseBinarySerialization
				? Tilemap3DSerialization.ToBinary(tilemap)
				: Encoding.UTF8.GetBytes(Tilemap3DSerialization.ToJson(tilemap, false));
		}
	}
}
