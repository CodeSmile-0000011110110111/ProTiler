// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using Unity.Serialization;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using ChunkKey = System.Int64;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public interface ILinearTileData
	{
		public UInt16 TileIndex { get; set; }
		public TileFlags TileFlags { get; set; }
		public UInt32 TileIndexAndFlags { get; set; }
	}

	public interface ISparseTileData {}

	/// <summary>
	///     Data for every tile in a chunk/layer. Be very considerate of what to add here and what type as
	///     this can have a huge impact on memory/disk usage of the map.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct LinearTileData : ILinearTileData, IEquatable<LinearTileData>
	{
		[CreateProperty]
		[FieldOffset(0)] private UInt32 m_TileIndexAndFlags;
		[FieldOffset(0)] private UInt16 m_TileIndex;
		[FieldOffset(2)] private TileFlags m_TileFlags;

		public UInt16 TileIndex { get => m_TileIndex; set => m_TileIndex = value; }
		public TileFlags TileFlags { get => m_TileFlags; set => m_TileFlags = value; }
		public UInt32 TileIndexAndFlags { get => m_TileIndexAndFlags; set => m_TileIndexAndFlags = value; }
		public static Boolean operator ==(LinearTileData left, LinearTileData right) => left.Equals(right);
		public static Boolean operator !=(LinearTileData left, LinearTileData right) => !left.Equals(right);

		public LinearTileData(UInt16 tileIndex, TileFlags tileFlags)
		{
			m_TileIndexAndFlags = 0;
			m_TileIndex = tileIndex;
			m_TileFlags = tileFlags;
		}

		public Boolean Equals(LinearTileData other) => m_TileIndexAndFlags == other.m_TileIndexAndFlags;
		public override Boolean Equals(Object obj) => obj is LinearTileData other && Equals(other);
		public override Int32 GetHashCode() => (Int32)m_TileIndexAndFlags;
		public override String ToString() => $"{nameof(LinearTileData)}(Index {m_TileIndex}, {m_TileFlags})";
	}

	/// <summary>
	///     Data for specific tiles (based on coordinates) in a chunk/layer.
	///     Use this for flagging tiles where most tiles use default data and only some tiles require
	///     additional or custom data.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SparseTileData : ISparseTileData, IEquatable<SparseTileData>
	{
		[CreateProperty] private Int32 m_Value;
		public Int32 Value
		{
			get => m_Value;
			set => m_Value = value;
		}
		public static Boolean operator ==(SparseTileData left, SparseTileData right) => left.Equals(right);
		public static Boolean operator !=(SparseTileData left, SparseTileData right) => !left.Equals(right);
		public SparseTileData(Int32 value) => m_Value = value;
		public Boolean Equals(SparseTileData other) => m_Value == other.m_Value;
		public override Boolean Equals(Object obj) => obj is SparseTileData other && Equals(other);
		public override Int32 GetHashCode() => m_Value;
		public override String ToString() => $"{nameof(SparseTileData)}({m_Value})";
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TilemapChunk<TLinear, TSparse> : IDisposable
		where TLinear : unmanaged, ILinearTileData, IEquatable<TLinear>
		where TSparse : unmanaged, ISparseTileData, IEquatable<TSparse>
	{
		private ChunkSize m_ChunkSize;
		private UnsafeList<TLinear> m_LinearTileData;
		private UnsafeParallelHashMap<GridCoord, TSparse> m_SparseTileData;
		public Int32 LinearDataCount => m_LinearTileData.Length;
		public Int32 SparseDataCount => m_SparseTileData.Count();

		public TilemapChunk(ChunkSize chunkSize, Allocator allocator)
		{
			m_ChunkSize = chunkSize;

			var initialCapacity = chunkSize.x * chunkSize.y * chunkSize.z;
			m_LinearTileData = new UnsafeList<TLinear>(initialCapacity, allocator);
			m_SparseTileData = new UnsafeParallelHashMap<GridCoord, TSparse>(0, allocator);
		}

		private TilemapChunk(ChunkSize chunkSize, UnsafeList<TLinear> linearData,
			UnsafeParallelHashMap<GridCoord, TSparse> sparseData)
		{
			m_ChunkSize = chunkSize;
			m_LinearTileData = linearData;
			m_SparseTileData = sparseData;
		}

		public void AddTileData(TLinear data) => m_LinearTileData.Add(data);

		public void SetTileData(GridCoord coord, TSparse data) => m_SparseTileData[coord] = data;

		public TLinear this[Int32 index]
		{
			get => m_LinearTileData[index];
			set => m_LinearTileData[index] = value;
		}

		public TSparse this[GridCoord coord]
		{
			get => m_SparseTileData.TryGetValue(coord, out var data) ? data : default;
			set => m_SparseTileData[coord] = value;
		}

		// GetLayerCount => m_LinearTileData.Length / (m_ChunkSize.x * m_ChunkSize.z)
		public void Dispose()
		{
			m_ChunkSize = ChunkSize.zero;
			m_LinearTileData.Dispose();
			m_SparseTileData.Dispose();
		}

		public override String ToString() =>
			$"{nameof(TilemapChunk<TLinear, TSparse>)}: {m_ChunkSize}, #{LinearDataCount} Linear, #{SparseDataCount} Sparse";

		public static IBinaryAdapter<TilemapChunk<TLinear, TSparse>> GetBinarySerializationAdapter() =>
			new BinaryAdapter();

		private class BinaryAdapter : BinaryAdapterBase, IBinaryAdapter<TilemapChunk<TLinear, TSparse>>
		{
			public BinaryAdapter() : base(version:0) {}

			public unsafe void Serialize(in BinarySerializationContext<TilemapChunk<TLinear, TSparse>> context,
				TilemapChunk<TLinear, TSparse> chunk)
			{
				var writer = context.Writer;
				WriteVersion(writer);

				writer->Add(chunk.m_ChunkSize);
				context.SerializeValue(chunk.m_LinearTileData);
				context.SerializeValue(chunk.m_SparseTileData);
			}

			public unsafe TilemapChunk<TLinear, TSparse> Deserialize(
				in BinaryDeserializationContext<TilemapChunk<TLinear, TSparse>> context)
			{
				var reader = context.Reader;
				ReadVersion(reader);
				if (Version == 0)
				{
					var chunkSize = reader->ReadNext<ChunkSize>();
					var linearData = context.DeserializeValue<UnsafeList<TLinear>>();
					var sparseData = context.DeserializeValue<UnsafeParallelHashMap<GridCoord, TSparse>>();

					return new TilemapChunk<TLinear, TSparse>(chunkSize, linearData, sparseData);
				}
				// fallbacks go here

				throw new SerializationException($"{nameof(TilemapChunk<TLinear, TSparse>)} data version " +
				                                 $"{Version} is unhandled. Possibly data was created with a " +
				                                 "newer version of the serializer?");
			}
		}
	}

	// generic type can be simple types (int2, int4) or structs
// MB acts as a factory for various types, inspector lets you select predefined types
// MB has mechanism to extend with custom types - how? static "create tilemap" event method
// that allows you to pass types?
	public abstract class Tilemap3DBase<TLinear, TSparse> : IDisposable
		where TLinear : unmanaged, ILinearTileData, IEquatable<TLinear>
		where TSparse : unmanaged, ISparseTileData, IEquatable<TSparse>
	{
		private readonly ChunkSize m_ChunkSize;

		// chunks as separate structs
		private NativeParallelHashMap<ChunkKey, TilemapChunk<TLinear, TSparse>> m_Chunks;
		//private FixedString512Bytes m_ExpectedChunkDataTypes;

		public Tilemap3DBase(ChunkSize chunkSize, Allocator allocator)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new NativeParallelHashMap<Int64, TilemapChunk<TLinear, TSparse>>(0, allocator);
		}

		public void Dispose() => m_Chunks.Dispose();
	}

	public class Tilemap3D : Tilemap3DBase<LinearTileData, SparseTileData>
	{
		public Tilemap3D(ChunkSize chunkSize, Allocator allocator)
			: base(chunkSize, allocator) {}
	}
}
