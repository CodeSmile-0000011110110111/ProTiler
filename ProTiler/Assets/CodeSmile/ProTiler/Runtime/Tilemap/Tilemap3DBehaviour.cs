// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
		private static readonly ChunkSize DefaultChunkSize = new(16, 16);

		[SerializeField] private Tilemap3D m_Tilemap;
		[SerializeField] private Serialization m_Serialization;
		[Pure] internal Vector2Int ChunkSize { get => m_Tilemap.ChunkSize; set => m_Tilemap.ChunkSize = value; }
		[Pure] internal Int32 ChunkCount => m_Tilemap.ChunkCount;
		[Pure] internal Int32 TileCount => m_Tilemap.TileCount;
		[Pure] public Grid3DBehaviour Grid => transform.parent.GetComponent<Grid3DBehaviour>();

		[Pure] private static Vector2Int ClampChunkSize(Vector2Int chunkSize) =>
			Tilemap3DUtility.ClampChunkSize(chunkSize);

		[Pure] private void Reset()
		{
			m_Tilemap = new Tilemap3D(DefaultChunkSize);
			m_Serialization = new Serialization();
			SerializeTilemap();

			//CreateTilemap(DefaultChunkSize);
			//CreateTilemapNoUndo(DefaultChunkSize);
			//SerializeTilemap();
		}

		[Pure] private void OnEnable() => RegisterEditorEvents();
		[Pure] private void OnDisable() => UnregisterEditorSceneEvents();
		private void DeserializeTilemap() => m_Tilemap = m_Serialization.DeserializeTilemap();

		[Pure] private void SerializeTilemap() => m_Serialization.SerializeTilemap(m_Tilemap);

		[Pure] internal void CreateTilemap(ChunkSize chunkSize)
		{
			this.RecordUndoInEditor(nameof(CreateTilemap));
			CreateTilemapNoUndo(ClampChunkSize(chunkSize));
			SerializeTilemap();
			IncrementCurrentUndoGroup();
		}

		internal void CreateTilemapNoUndo(ChunkSize chunkSize) => m_Tilemap = new Tilemap3D(chunkSize);

		[Pure] internal Int32 GetLayerCount(ChunkCoord chunkCoord) => m_Tilemap.GetLayerCount(chunkCoord);
		[Pure] public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;
		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => m_Tilemap.GetTiles(coords);
		[Pure] public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoords);
			SerializeTilemap();
			IncrementCurrentUndoGroup();
		}

		[Pure] private void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoords) => m_Tilemap.SetTiles(tileCoords);

		/// <summary>
		///     Handles Tilemap data serialization.
		///     Noteworthy: compressing the byte array speeds up undo/redo of larger maps significantly.
		///     TODO: split saving into saving each individual chunk separately for performance.
		/// </summary>
		[FullCovered] [Serializable]
		private sealed class Serialization
		{
			private const Boolean m_UseBinarySerialization = true;

			[SerializeField] [HideInInspector] private Byte[] m_CompressedTilemap = new Byte[0];

			internal void SerializeTilemap(Tilemap3D tilemap)
			{
				m_CompressedTilemap = m_UseBinarySerialization
					? Tilemap3DSerialization.ToBinary(tilemap).Compress()
					: Encoding.UTF8.GetBytes(Tilemap3DSerialization.ToJson(tilemap, false)).Compress();

				Debug.Log(
					$"{tilemap} => {m_CompressedTilemap.Length} bytes ({m_CompressedTilemap.CalculateMd5Hash()})");
			}

			[Pure] internal Tilemap3D DeserializeTilemap()
			{
				var tilemap = m_UseBinarySerialization
					? Tilemap3DSerialization.FromBinary(m_CompressedTilemap.Decompress())
					: Tilemap3DSerialization.FromJson(Encoding.UTF8.GetString(m_CompressedTilemap.Decompress()));

				Debug.Log(
					$"{tilemap} <= {m_CompressedTilemap.Length} bytes ({m_CompressedTilemap.CalculateMd5Hash()})");

				return tilemap;
			}

			[ExcludeFromCodeCoverage]
			public void VerifyBufferMatches(Tilemap3D tilemap)
			{
#if DEBUG
				var serializedTilemap = m_UseBinarySerialization
					? Tilemap3DSerialization.ToBinary(tilemap).Compress()
					: Encoding.UTF8.GetBytes(Tilemap3DSerialization.ToJson(tilemap, false)).Compress();

				if (serializedTilemap.Length != m_CompressedTilemap.Length)
				{
					Debug.LogWarning("Save scene: current serialized tilemap did not match newly serialized one. " +
					                 "This indicates changes that bypassed serialization. " +
					                 "Serializing the current state.");
					m_CompressedTilemap = serializedTilemap;
				}
#endif
			}
		}
	}
}
