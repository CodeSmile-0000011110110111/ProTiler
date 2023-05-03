// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     TileData for a specific coordinate.
	/// </summary>
	[Serializable]
	public struct Tile3DCoord
	{
		public Vector3Int Coord;
		public Tile3D m_Tile;

		public static Tile3DCoord New(Vector3Int coord, Tile3D tile) => new() { Coord = coord, m_Tile = tile };

		[ExcludeFromCodeCoverage] public override string ToString() => $"{Coord}, {m_Tile}";
	}
}
