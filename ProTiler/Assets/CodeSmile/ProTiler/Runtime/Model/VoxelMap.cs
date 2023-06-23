// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;

namespace CodeSmile.ProTiler.Model
{
	public class VoxelMap : GridMapBase
	{
		public VoxelMap(int3 chunkSize, Byte gridVersion) : base(chunkSize, gridVersion) {}
	}
}
