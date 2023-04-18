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

		public Tilemap3DChunkCollection(Vector2Int chunkSize) => ChangeChunkSize(chunkSize);

		public void ChangeChunkSize(Vector2Int newChunkSize)
		{
			if (newChunkSize == m_Size)
				return;

			m_Size = newChunkSize;

			// TODO: recreate chunks without destroying data
			m_Chunks = new Dictionary<long, Dictionary<int, Tile3DDataCollection>>();
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
			if (m_Chunks.TryGetValue(coordHash, out var chunk))
				return chunk;

			m_Chunks[coordHash] = chunk = new Dictionary<int, Tile3DDataCollection>();
			return chunk;
		}

		private long GetChunkHash(Vector3Int chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.z);

		public void GetTileData(Vector3Int[] coords, ref Tile3DCoordData[] tileDatas)
		{
			var index = 0;
			foreach (var coord in coords)
			{
				var coordHash = HashUtility.GetHash(coord.x, coord.z);
				if (m_Chunks.ContainsKey(coordHash) == false)
					continue;

				var chunk = m_Chunks[coordHash];
				if (chunk.ContainsKey(coord.y) == false)
					continue;

				var layer = chunk[coord.y];
				tileDatas[index++] = Tile3DCoordData.New(coord, layer[coord.x, coord.z]);

				//layer[coord.x, coord.z] = changeData.TileData;
			}
		}
	}
}
