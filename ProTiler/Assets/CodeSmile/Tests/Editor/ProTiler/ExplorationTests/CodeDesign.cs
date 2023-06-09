// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Editor.ProTiler.CodeDesign.Interfaces;
using CodeSmile.Tests.Editor.ProTiler.CodeDesign.Serialization;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler
{
	namespace CodeDesign
	{
		// Questions:
		// how to handle dirty flags?

		// TODO:
		/*
		 * design flow of calls/data for serialization, deserialization and undo/redo (if different at all)
		 *	Note: serialization is in charge of compression/decompression
		 *
		 * 1. entire tilemap =>
		 *		byte[] TilemapBase:Serialize() // return bytes to user
		 *		TilemapBase TilemapBase:Deserialize(byte[]) // return to user, user cast to concrete type
		 * 2. specific chunk =>
		 *		CAUTION: we can only serialize a concrete type, not an object or interface!
		 *
		 *		void TilemapDataBase:SerializeChunk(Int64 key, in LinearDataChunk<TData> data)
		 *		LinearDataChunk<TData> TilemapDataBase:DeserializeChunk(Int64 key)
		 *			calls respective methods on ISerializedDataStream field (if not null)
		 *			serialization instance passed in ctor, to avoid changing streaming approach without full reset
		 *				(doable but will require extra handling to prevent data out of sync)
		 *
		 *
		 */
		//

		namespace Interfaces
		{
			public interface ITileData {}

			public interface IChunkData<TData> where TData : unmanaged {}
			public interface IChunkDataLinear<TData> : IChunkData<TData> where TData : unmanaged {}
			public interface IChunkDataSparse<TData> : IChunkData<TData> where TData : unmanaged {}

			public interface IChunkDataLinearSerialization<TData> where TData : unmanaged
			{
				public Byte[] Serialize(Int64 chunkKey, ChunkDataLinear<TData> chunk);
				public (Int64, ChunkDataLinear<TData>) Deserialize(Byte[] bytes);
			}

			public interface ITilemapData // ITilemapDataSerialization
			{
				// Serialize
				// SerializeChunk(key)
			}

			public interface ITilemapDataLinear : ITilemapData {}
			public interface ITilemapDataSparse : ITilemapData {}

			public interface ITilemapChunkDataSerialization<TChunk, TData>
				where TChunk : unmanaged, IChunkData<TData>
				where TData : unmanaged
			{
				public void SerializeChunk<TChunk>(Int64 key, TChunk chunk);
				public TChunk DeserializeChunk<TChunk>(Int64 key);
			}

			public interface ISerializedDataStream
			{
				public Byte[] ReadSerializedData(Int64 key);
				public void WriteSerializedData(Int64 key, Byte[] bytes);
			}

			// compression is an afterthought, handled outside the tilemap stuff
			public interface IBinaryCompression
			{
				public Byte[] Compress(Byte[] bytes);
				public Byte[] Decompress(Byte[] bytes);
			}
		}

		namespace Serialization
		{
			public class TilemapChunkMemoryStream<TChunk, TData> : ITilemapChunkDataSerialization<TChunk,TData>
				where TChunk : unmanaged, IChunkData<TData>
				where TData : unmanaged
			{
				private Dictionary<Int64, byte[]> m_SerializedChunks;

				// serialize to byte[]
				public void SerializeChunk<TChunk>(Int64 key, TChunk chunk) => throw new NotImplementedException();
				// deserialize from byte[]
				public TChunk DeserializeChunk<TChunk>(Int64 key) => throw new NotImplementedException();
			}

			public class TilemapChunkBufferedFileStream<TChunk, TData> : TilemapChunkMemoryStream<TChunk, TData>
				where TChunk : unmanaged, IChunkData<TData>
				where TData : unmanaged
			{
				private Dictionary<Int64, byte[]> m_SerializedChunks;

				// serialize to byte[]
				public void SerializeChunk<TChunk>(Int64 key, TChunk chunk) => throw new NotImplementedException();
				// deserialize from byte[]
				public TChunk DeserializeChunk<TChunk>(Int64 key) => throw new NotImplementedException();
			}


			// adapters for Unity.Serialization (binary)

			internal sealed class ChunkDataLinearBinaryAdapter<TData> : IBinaryAdapter<ChunkDataLinear<TData>>
				where TData : unmanaged
			{
				public void Serialize(in BinarySerializationContext<ChunkDataLinear<TData>> context,
					ChunkDataLinear<TData> value) => throw new NotImplementedException();

				public ChunkDataLinear<TData> Deserialize(
					in BinaryDeserializationContext<ChunkDataLinear<TData>> context) =>
					throw new NotImplementedException();
			}
			// +1 for sparse chunk data
		}

		public struct TileData : ITileData
		{
			[CreateProperty] public Int32 TileIndex;
			[CreateProperty] public Int32 TileFlags;
		}

		public struct MySparseBits
		{
			[CreateProperty] public BitField32 bits;
			[CreateProperty] public UnsafeBitArray bitArray;
		}

		public struct ChunkDataLinear<TData> : IChunkDataLinear<TData> where TData : unmanaged
		{
			[CreateProperty] public UnsafeList<TData> linearData;
		}

		public struct ChunkDataSparse<TData> : IChunkDataSparse<TData> where TData : unmanaged
		{
			[CreateProperty] public UnsafeParallelHashMap<ChunkSize, TData> sparseData;
		}

		public abstract class TilemapDataBase : ITilemapData
		{
			// coord to chunk key
		}

		public class TilemapChunkDataLinear<TData> : TilemapDataBase, ITilemapDataLinear,
			ITilemapChunkDataSerialization<ChunkDataLinear<TData>, TData>
			where TData : unmanaged
		{
			// this ref comes from outside, new here is just for testing
			private ITilemapChunkDataSerialization<ChunkDataLinear<TData>, TData> m_Stream =
				new TilemapChunkBufferedFileStream<ChunkDataLinear<TData>, TData>();

			[CreateProperty] public NativeParallelHashMap<Int64, ChunkDataLinear<TData>> linearChunks;

			public void SerializeChunk<TChunk>(Int64 key, TChunk chunk) => m_Stream.SerializeChunk(key, chunk);
			public TChunk DeserializeChunk<TChunk>(Int64 key) => m_Stream.DeserializeChunk<TChunk>(key);
		}

		public class TilemapDataSparse<TData> : TilemapDataBase, ITilemapDataSparse where TData : unmanaged
		{
			[CreateProperty] public NativeParallelHashMap<Int64, ChunkDataSparse<TData>> sparseChunks;
			// public override void SerializeChunk(Int64 key, T chunk) => throw new NotImplementedException();
			// public override void DeserializeChunk(Int64 key, out Object chunk) => throw new NotImplementedException();
		}

		public abstract class TilemapBase
		{
			private ChunkSize m_ChunkSize;
			protected List<ITilemapDataLinear> linearMaps;
			protected List<ITilemapDataSparse> sparseMaps;

			// ITilemapSerialization (handle different ways of serialization, may or may not compress)
			// created by abstract factory
		}

		public class MyTilemap : TilemapBase
		{
			public MyTilemap()
			{
				linearMaps.Add(new TilemapChunkDataLinear<TileData>());
				sparseMaps.Add(new TilemapDataSparse<MySparseBits>());
			}

			public TileData GetLinearData1() => default; // TODO ...
			public MySparseBits GetSparseBits() => default; // TODO ...
		}
	}
}
