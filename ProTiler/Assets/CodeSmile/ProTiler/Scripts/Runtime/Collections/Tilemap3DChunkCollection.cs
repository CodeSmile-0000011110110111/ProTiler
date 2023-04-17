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
		private Vector2Int m_ChunkSize;
		private Dictionary<long, Dictionary<int, Tile3DDataCollection>> m_Chunks;
		private int m_Count;
		public Vector2Int ChunkSize => m_ChunkSize;
		public int Count
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
		public Tilemap3DChunkCollection(Vector2Int chunkSize) => ChangeChunkSize(chunkSize);

		public void ChangeChunkSize(Vector2Int newChunkSize)
		{
			if (newChunkSize == m_ChunkSize)
				return;

			m_ChunkSize = newChunkSize;

			// TODO: recreate chunks without destroying data
			m_Chunks = new Dictionary<long, Dictionary<int, Tile3DDataCollection>>();
			m_Chunks.Add(HashUtility.GetHash(0, 0), new Dictionary<int, Tile3DDataCollection>());

			// TODO: dummy create one tile
			/*
			var newTiles = new[] { new Tile3DCoordData { Coord = Vector3Int.zero, TileData = new Tile3DData { Tile = Tile3D.Create() } } };
			newTiles[0].TileData.Tile.Prefab = AssetDatabase.LoadAssetAtPath(
				"Assets/Art/kenney/Tower Defense (Classic)/Prefabs/tile_005.prefab",
				typeof(GameObject)) as GameObject;
				*/
			throw new NotImplementedException("create new dummy tiledata");

			var newTiles = new[] { new Tile3DCoordData() };
			SetTiles(newTiles);
		}

		public void SetTiles(Tile3DCoordData[] tileChangeData)
		{
			foreach (var changeData in tileChangeData)
			{
				var coord = changeData.Coord;
				var coordHash = HashUtility.GetHash(coord.x, coord.z);
				var chunk = m_Chunks[coordHash];

				if (chunk.ContainsKey(coord.y) == false)
					chunk.Add(coord.y, new Tile3DDataCollection(20, 20));

				var layer = chunk[coord.y];
				layer[coord.x, coord.z] = changeData.TileData;
			}
		}

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
