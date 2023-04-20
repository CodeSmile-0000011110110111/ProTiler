﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
{
	[Serializable]
	public class ChunkDictionary : SerializedDictionary<long, TileLayerDictionary> {}

	[Serializable]
	public class TileLayerDictionary : SerializedDictionary<int, Tile3DDataCollection>
	{
		public int Uses;
	}

	/// <summary>
	///     Contains all chunks of a tilemap, dividing tilemap into x/z spatial chunks.
	/// </summary>
	[Serializable]
	public class Tilemap3DChunkCollection
	{
		public const int MinChunkSize = 2;

		[SerializeField] private ChunkDictionary m_Chunks;
		[SerializeField] private Vector2Int m_Size;
		[SerializeField] private int m_Count;

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
		//Grid3DUtility.ToIndex2D(chunkCoord.x, chunkCoord.z, m_Size.x);

		public static void ClampChunkSize(ref Vector2Int chunkSize)
		{
			chunkSize.x = Mathf.Max(MinChunkSize, chunkSize.x);
			chunkSize.y = Mathf.Max(MinChunkSize, chunkSize.y);
		}

		private Tilemap3DChunkCollection()
			: this(16, 16) {}

		public Tilemap3DChunkCollection(int x, int y)
			: this(new Vector2Int(x, y)) {}

		public Tilemap3DChunkCollection(Vector2Int chunkSize) => ChangeChunkSize(chunkSize);

		public override string ToString() => $"{nameof(Tilemap3DChunkCollection)}(Size: {Size}, Count: {Count}, TileCount: {TileCount})";

		public void ChangeChunkSize(Vector2Int newChunkSize)
		{
			ClampChunkSize(ref newChunkSize);
			if (newChunkSize == m_Size)
				return;

			m_Size = newChunkSize;

			// TODO: recreate chunks without destroying data
			m_Chunks = new ChunkDictionary();
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

		internal Vector3Int ToLayerCoord(Vector3Int chunkCoord, Vector3Int coord) => coord - chunkCoord;

		internal Vector3Int ToChunkCoord(Vector3Int coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);

		private Tile3DDataCollection GetOrCreateChunkLayer(TileLayerDictionary chunk, int y)
		{
			if (chunk.TryGetValue(y, out var layer))
				return layer;

			chunk[y] = layer = new Tile3DDataCollection(m_Size);
			return layer;
		}

		private TileLayerDictionary GetOrCreateChunk(Vector3Int chunkCoord)
		{
			var chunkKey = GetChunkKey(chunkCoord);
			if (TryGetChunk(chunkKey, out var chunk))
				return chunk;

			m_Chunks[chunkKey] = chunk = new TileLayerDictionary();
			return chunk;
		}

		private bool TryGetChunk(Vector3Int chunkCoord, out TileLayerDictionary chunk) => TryGetChunk(GetChunkKey(chunkCoord), out chunk);

		private bool TryGetChunk(long key, out TileLayerDictionary chunk) => m_Chunks.TryGetValue(key, out chunk);

		private long GetChunkKey(Vector3Int chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.z);
	}
}
