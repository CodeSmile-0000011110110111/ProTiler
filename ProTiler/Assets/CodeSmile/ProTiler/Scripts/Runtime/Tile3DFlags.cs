// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.ProTiler
{
	[Flags]
	public enum Tile3DFlags
	{
		None = 0,
		DirectionNorth = 1 << 0,
		DirectionEast = 1 << 1,
		DirectionSouth = 1 << 2,
		DirectionWest = 1 << 3,
		FlipHorizontal = 1 << 4,
		FlipVertical = 1 << 5,

		AllDirections = DirectionNorth | DirectionSouth | DirectionEast | DirectionWest,
		AllFlips = FlipHorizontal | FlipVertical,
	}
}