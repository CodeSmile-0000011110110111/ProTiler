// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
{
	/// <summary>
	///     Contains all chunks of a tilemap, dividing tilemap into x/z spatial chunks.
	/// </summary>
	[Serializable]
	public class Tilemap3DChunkCollection
	{
		private Vector2Int m_Size;
		private Dictionary<long, Dictionary<int, Tile3DDataCollection>> m_Chunks;
		private int m_Count;

		public Vector2Int Size => m_Size;
		public int TileCount
		{
			get
			{
				m_Count = 0;
				foreach (var layers in m_Chunks.Values)
				{
					foreach (var layer in layers.Values)
						m_Count += layer.Count;
				}
				return m_Count;
			}
		}

		public int Count => m_Chunks.Count;

		private Tilemap3DChunkCollection()
			: this(new Vector2Int(10, 10)) {}

		public Tilemap3DChunkCollection(Vector2Int chunkSize) => ChangeChunkSize(chunkSize);

		public void ChangeChunkSize(Vector2Int newChunkSize)
		{
			if (newChunkSize == m_Size)
				return;

			if (newChunkSize.x <= 0 || newChunkSize.y <= 0)
				throw new ArgumentException($"invalid chunk size: {newChunkSize}");

			m_Size = newChunkSize;

			// TODO: recreate chunks without destroying data
			m_Chunks = new Dictionary<long, Dictionary<int, Tile3DDataCollection>>();
		}

		public void GetTiles(Vector3Int[] coords, ref Tile3DCoordData[] tileDatas)
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
				tileDatas[index++] = Tile3DCoordData.New(coord, layer[layerCoord.x, layerCoord.z]);

				if (index == tileDatas.Length)
					break;
			}
		}

		public void SetTiles(Tile3DCoordData[] tileCoordData)
		{
			foreach (var coordData in tileCoordData)
			{
				var coord = coordData.Coord;
				var chunkCoord = ToChunkCoord(coord);
				var chunk = GetOrCreateChunk(chunkCoord);

				var layer = GetOrCreateChunkLayer(chunk, coord.y);
				var layerCoord = ToLayerCoord(chunkCoord, coord);
				layer[layerCoord.x, layerCoord.z] = coordData.TileData;
			}
		}

		private Vector3Int ToLayerCoord(Vector3Int chunkCoord, Vector3Int coord) => coord - chunkCoord;

		private Vector3Int ToChunkCoord(Vector3Int coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);

		private Tile3DDataCollection GetOrCreateChunkLayer(Dictionary<int, Tile3DDataCollection> chunk, int y)
		{
			if (chunk.TryGetValue(y, out var layer))
				return layer;

			chunk[y] = layer = new Tile3DDataCollection(m_Size);
			return layer;
		}

		private Dictionary<int, Tile3DDataCollection> GetOrCreateChunk(Vector3Int chunkCoord)
		{
			var coordHash = GetChunkHash(chunkCoord);
			if (TryGetChunk(coordHash, out var chunk))
				return chunk;

			m_Chunks[coordHash] = chunk = new Dictionary<int, Tile3DDataCollection>();
			return chunk;
		}

		private bool TryGetChunk(Vector3Int chunkCoord, out Dictionary<int, Tile3DDataCollection> chunk) =>
			TryGetChunk(GetChunkHash(chunkCoord), out chunk);

		private bool TryGetChunk(long coordHash, out Dictionary<int, Tile3DDataCollection> chunk)
		{
			if (m_Chunks.TryGetValue(coordHash, out var gotChunk))
			{
				chunk = gotChunk;
				return true;
			}

			chunk = null;
			return false;
		}

		private long GetChunkHash(Vector3Int chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.z);
	}
}
