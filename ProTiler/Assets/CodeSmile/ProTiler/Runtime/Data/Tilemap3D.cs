// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using System;
using System.Diagnostics.CodeAnalysis;

using GridCoord = UnityEngine.Vector3Int;
using ChunkSize = UnityEngine.Vector2Int;
using Math = UnityEngine.Mathf;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Contains all chunks of a tilemap, dividing tilemap into x/z spatial chunks.
	/// </summary>
	[Serializable]
	public class Tilemap3D
	{
		public const int MinChunkSize = 2;

		//[SerializeField]
		private ChunkCollection m_Chunks;
		//[SerializeField]
		private ChunkSize m_ChunkSize;
		//[SerializeField]
		private int m_Count;

		public ChunkSize ChunkSize => m_ChunkSize;
		public int TileCount
		{
			get
			{
				m_Count = 0;
				foreach (var layers in m_Chunks.Values)
				{
					foreach (var layer in layers.Values)
						m_Count += layer.TileCount;
				}
				return m_Count;
			}
		}

		public int Count => m_Chunks.Count;

		public static void ClampChunkSize(ref ChunkSize chunkSize)
		{
			chunkSize.x = Math.Max(MinChunkSize, chunkSize.x);
			chunkSize.y = Math.Max(MinChunkSize, chunkSize.y);
		}

		public Tilemap3D()
			: this(new ChunkSize(16, 16)) {}

		public Tilemap3D(ChunkSize chunkSize) => ChangeChunkSize(chunkSize);

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tilemap3D)}(Size: {ChunkSize}, Count: {Count}, TileCount: {TileCount})";

		public void ChangeChunkSize(ChunkSize newChunkSize)
		{
			ClampChunkSize(ref newChunkSize);
			if (newChunkSize == m_ChunkSize)
				return;

			m_ChunkSize = newChunkSize;

			// TODO: recreate chunks without destroying data
			m_Chunks = new ChunkCollection();
		}

		public void GetTiles(GridCoord[] coords, ref Tile3DCoord[] tileDatas)
		{
			if (coords == null || coords.Length == 0 || tileDatas == null || tileDatas.Length == 0)
				return;

			var index = 0;
			foreach (var coord in coords)
			{
				var chunkCoord = ToChunkCoord(coord);
				if (TryGetChunk(chunkCoord, out var chunk) == false)
					continue;

				if (chunk.TryGetValue(coord.y, out var layer) == false)
					continue;

				var layerCoord = ToLayerCoord(chunkCoord, coord);
				var tileCoordIndex = Grid3DUtility.ToIndex2D(layerCoord.x, layerCoord.z, m_ChunkSize.x);
				tileDatas[index++] = new Tile3DCoord(coord, layer[tileCoordIndex]);

				if (index == tileDatas.Length)
					break;
			}
		}

		public void SetTiles(Tile3DCoord[] tileCoordData)
		{
			foreach (var coordData in tileCoordData)
			{
				var coord = coordData.Coord;
				var chunkCoord = ToChunkCoord(coord);
				var chunk = GetOrCreateChunk(chunkCoord);

				var layer = GetOrCreateChunkLayer(chunk, coord.y);
				var layerCoord = ToLayerCoord(chunkCoord, coord);
				var tileCoordIndex = Grid3DUtility.ToIndex2D(layerCoord.x, layerCoord.z, m_ChunkSize.x);
				layer[tileCoordIndex] = coordData.Tile;
			}
		}

		internal GridCoord ToLayerCoord(GridCoord chunkCoord, GridCoord coord) => coord - chunkCoord;

		internal GridCoord ToChunkCoord(GridCoord coord) => new(coord.x / m_ChunkSize.x, coord.y, coord.z / m_ChunkSize.y);

		private Tile3DLayer GetOrCreateChunkLayer(Tile3DLayerCollection chunk, int y)
		{
			if (chunk.TryGetValue(y, out var layer))
				return layer;

			chunk[y] = layer = new Tile3DLayer(m_ChunkSize);
			return layer;
		}

		private Tile3DLayerCollection GetOrCreateChunk(GridCoord chunkCoord)
		{
			var chunkKey = GetChunkKey(chunkCoord);
			if (TryGetChunk(chunkKey, out var chunk))
				return chunk;

			m_Chunks[chunkKey] = chunk = new Tile3DLayerCollection();
			return chunk;
		}

		private bool TryGetChunk(GridCoord chunkCoord, out Tile3DLayerCollection chunk) => TryGetChunk(GetChunkKey(chunkCoord), out chunk);

		private bool TryGetChunk(long key, out Tile3DLayerCollection chunk) => m_Chunks.TryGetValue(key, out chunk);

		private long GetChunkKey(GridCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.z);

		[Serializable] public class ChunkCollection : SerializedDictionary<long, Tile3DLayerCollection> {}
		[Serializable] public class Tile3DLayerCollection : SerializedDictionary<int, Tile3DLayer> {}
	}
}
