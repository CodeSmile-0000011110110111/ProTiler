// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tilemap;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[AddComponentMenu("")] // hide from Add Component menu
	[DisallowMultipleComponent]
	public sealed class Tilemap3DModelController : MonoBehaviour
	{
		public event Action OnTilemapReplaced;
		public event Action<IEnumerable<Tile3DCoord>> OnTilemapModified;

		[SerializeField] private Tilemap3D m_Tilemap;

		internal Tilemap3D Tilemap
		{
			get => m_Tilemap;
			set
			{
				m_Tilemap = value;
				OnTilemapReplaced?.Invoke();
			}
		}

		[Pure] internal Vector2Int ChunkSize { get => Tilemap.ChunkSize; set => Tilemap.ChunkSize = value; }
		[Pure] internal Int32 ChunkCount => Tilemap.ChunkCount;
		[Pure] internal Int32 TileCount => Tilemap.TileCount;
		[Pure] public Grid3DController Grid => transform.parent.GetComponent<Grid3DController>();

		[Pure] private static Vector2Int ClampChunkSize(Vector2Int chunkSize) =>
			Tilemap3DUtility.ClampChunkSize(chunkSize);

		[Pure] internal void ClearTilemap(ChunkSize chunkSize)
		{
			this.UndoSetCurrentGroupName("Clear Tilemap");
			this.UndoRecordObjectInEditor(nameof(ClearTilemap));
			ClearTilemapNoUndo(ClampChunkSize(chunkSize));
			OnTilemapReplaced?.Invoke();
			this.UndoIncrementCurrentGroup();
		}

		public void ClearTilemapNoUndo(ChunkSize chunkSize) => Tilemap = new Tilemap3D(chunkSize);

		[Pure] internal Int32 GetLayerCount(ChunkCoord chunkCoord) => Tilemap.GetLayerCount(chunkCoord);
		[Pure] public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;
		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => Tilemap.GetTiles(coords);
		[Pure] public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			this.UndoSetCurrentGroupName("Set Tile(s)");
			this.UndoRecordObjectInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoords);
			OnTilemapModified?.Invoke(tileCoords);
			this.UndoIncrementCurrentGroup();
		}

		[Pure] public void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoords) => Tilemap.SetTiles(tileCoords);
	}
}
