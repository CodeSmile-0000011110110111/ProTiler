// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[SuppressMessage("NDepend", "ND1001:AvoidTypesWithTooManyMethods")]
	[FullCovered]
	[ExecuteAlways]
	[AddComponentMenu("")] // hide from Add Component menu
	[DisallowMultipleComponent]
	public sealed partial class Tilemap3DModel : MonoBehaviour
	{
		public event Action OnTilemapCleared;
		public event Action<IEnumerable<Tile3DCoord>> OnTilemapModified;

		[SerializeField] private Tilemap3D m_Tilemap;
		[SerializeField] [HideInInspector] private Byte[] m_SerializedTilemap = new Byte[0];

		private readonly Tilemap3DSerializer m_Serializer = new();
		private readonly UndoGroupRegistry m_UndoGroupRegistry = new();

		[Pure] internal Vector2Int ChunkSize { get => m_Tilemap.ChunkSize; set => m_Tilemap.ChunkSize = value; }
		[Pure] internal Int32 ChunkCount => m_Tilemap.ChunkCount;
		[Pure] internal Int32 TileCount => m_Tilemap.TileCount;
		[Pure] public Grid3DController Grid => transform.parent.GetComponent<Grid3DController>();

		[Pure] private static Vector2Int ClampChunkSize(Vector2Int chunkSize) =>
			Tilemap3DUtility.ClampChunkSize(chunkSize);

		private void OnEnable() => RegisterEditorSceneEvents();

		private void OnDisable() => UnregisterEditorSceneEvents();

		[Pure] internal void ClearTilemap(ChunkSize chunkSize)
		{
			StartRecordUndo("Clear Tilemap", nameof(ClearTilemap));
			ClearTilemapNoUndo(ClampChunkSize(chunkSize));
			OnTilemapCleared?.Invoke();
			OnTilemapModified?.Invoke(new Tile3DCoord[0]);
			EndRecordUndo();
		}

		public void ClearTilemapNoUndo(ChunkSize chunkSize) => m_Tilemap = new Tilemap3D(chunkSize);

		[Pure] internal Int32 GetLayerCount(ChunkCoord chunkCoord) => m_Tilemap.GetLayerCount(chunkCoord);
		[Pure] public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;
		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => m_Tilemap.GetTiles(coords);
		[Pure] public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			StartRecordUndo(tileCoords.Count() > 1 ? "Set Tiles" : "Set Tile", nameof(SetTiles));
			SetTilesNoUndo(tileCoords);
			OnTilemapModified?.Invoke(tileCoords);
			EndRecordUndo();
		}

		[Pure] public void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoords) => m_Tilemap.SetTiles(tileCoords);

		[Pure] private void SerializeTilemap() => m_SerializedTilemap = m_Serializer.SerializeTilemap(m_Tilemap);
		private void DeserializeTilemap() => m_Tilemap = m_Serializer.DeserializeTilemap(m_SerializedTilemap);
	}
}