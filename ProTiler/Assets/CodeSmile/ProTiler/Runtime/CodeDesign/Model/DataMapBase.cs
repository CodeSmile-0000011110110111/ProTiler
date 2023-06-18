// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model._remove;
using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using System;
using System.Runtime.CompilerServices;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;
using Math = Unity.Mathematics.math;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public abstract class DataMapBase
	{
		/// <summary>
		///     Chunks must be at least 2x2 in X/Z to avoid hashed chunk coords from clashing.
		/// </summary>
		protected static readonly ChunkSize s_MinChunkSize = new(2, 0, 2);

		protected IDataMapStream m_Stream;
		protected ChunkSize m_ChunkSize;

		public DataMapBase()
			: this(s_MinChunkSize) {}

		public DataMapBase(ChunkSize chunkSize, IDataMapStream stream = null)
		{
			m_ChunkSize = Math.max(s_MinChunkSize, chunkSize);
			m_Stream = stream;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ChunkKey ToChunkKey(ChunkCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ChunkKey ToChunkKey(WorldCoord worldCoord) => ToChunkKey(ToChunkCoord(worldCoord));

		/// <summary>
		///     Examples for ChunkSize(2,2):
		///     Grid(-1,0,-1) => Chunk(-1,-1)
		///     Grid(-2,0,-2) => Chunk(-1,-1)
		///     Grid(-3,0,-3) => Chunk(-2,-2)
		///     Grid(-4,0,-4) => Chunk(-2,-2)
		/// </summary>
		/// <param name="worldCoord"></param>
		/// <param name="m_ChunkSize"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ChunkCoord ToChunkCoord(WorldCoord worldCoord) => new(
			// TODO: the +1 for negative coords can probably be refactored to a more generic algorithm
			// Explanation: negative grid coordinates result in negative chunk coordinates - but offset by 1.
			worldCoord.x < 0 ? -(Math.abs(worldCoord.x + 1) / m_ChunkSize.x + 1) : worldCoord.x / m_ChunkSize.x,
			worldCoord.z < 0 ? -(Math.abs(worldCoord.z + 1) / m_ChunkSize.z + 1) : worldCoord.z / m_ChunkSize.z);


		// create instance of undo/redo system (editor and runtime edit-mode)
		public virtual void Serialize(IBinaryWriter writer)
		{
			//writer.Add(..);
		}

		public virtual DataMapBase Deserialize(IBinaryReader reader, Byte userDataVersion) =>
			// deserialize base class fields first
			//baseField = reader.ReadNext<Byte>();
			this;
	}
}
