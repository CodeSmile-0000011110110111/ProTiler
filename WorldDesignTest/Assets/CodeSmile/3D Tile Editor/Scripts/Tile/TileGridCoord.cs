// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.Tile
{
	public struct TileGridCoord : IEquatable<TileGridCoord>
	{
		private Vector3Int m_Coord;
		

		public TileGridCoord(Vector3 position) => m_Coord = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
		
		public int x { get => m_Coord.x; set => m_Coord.x = value; }
		public int y { get => m_Coord.y; set => m_Coord.y = value; }
		public int z { get => m_Coord.z; set => m_Coord.z = value; }

		public Vector3 ToWorldPosition(Vector3Int gridSize, Vector3 worldOrigin)
		{
			return new Vector3(m_Coord.x * gridSize.x, 0f, m_Coord.z * gridSize.z);
		}
		
		public override string ToString() => $"{nameof(TileGridCoord)}({m_Coord.x},{m_Coord.y},{m_Coord.z})";

		public bool Equals(TileGridCoord other) => m_Coord.Equals(other.m_Coord);

		public override bool Equals(object obj) => obj is TileGridCoord other && Equals(other);

		public override int GetHashCode() => m_Coord.GetHashCode();

		public static bool operator ==(TileGridCoord left, TileGridCoord right) => left.Equals(right);
		public static bool operator !=(TileGridCoord left, TileGridCoord right) => !left.Equals(right);
	}
}