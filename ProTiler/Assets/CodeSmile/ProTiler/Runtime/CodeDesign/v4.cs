// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.DataChunks;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps;
using CodeSmile.ProTiler.CodeDesign.v4.DataMapStreaming;
using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.CodeDesign.v4.Serialization;
using CodeSmile.ProTiler.CodeDesign.v4.TilemapGame.TilemapData;
using CodeSmile.ProTiler.CodeDesign.v4.UnitySerialization;
using CodeSmile.ProTiler.CodeDesign.v4.VoxelGame.VoxelMapData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Properties;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.CodeDesign
{
	namespace v4
	{
		namespace DataChunks
		{
			// public interface IDataChunk<TData> where TData : unmanaged {}
			// public interface ILinearDataChunk<TData> : IDataChunk<TData> where TData : unmanaged {}
			// public interface ISparseDataChunk<TData> : IDataChunk<TData> where TData : unmanaged {}

			public struct LinearDataChunk<TData> where TData : unmanaged
			{
				[CreateProperty] public UnsafeList<TData> linearData;
			}

			public struct SparseDataChunk<TData> where TData : unmanaged
			{
				[CreateProperty] public UnsafeParallelHashMap<ChunkSize, TData> sparseData;
			}
		}

		namespace DataMaps
		{
			public interface ILinearDataMap {}
			public interface ISparseDataMap {}

			// avoid <TData> because of serialization?
			public abstract class DataMapBase //<TData> where TData : unmanaged
			{
				// coord to chunk key
				// hashmap of modified (unsaved) chunks
				// possibly: hashmap of loaded chunks together with access timestamp

				// create instance of undo/redo system (editor and runtime edit-mode)
				public virtual void Serialize(IBinaryWriter writer)
				{
					//writer.Add(..);
				}

				public virtual DataMapBase Deserialize(IBinaryReader reader) =>
					// deserialize base class fields first
					//baseField = reader.ReadNext<Byte>();
					this;
			}

			public class LinearDataMap<TData> : DataMapBase, ILinearDataMap where TData : unmanaged
			{
				[CreateProperty] protected NativeParallelHashMap<ChunkKey, LinearDataChunk<TData>> m_DataChunks;

				public virtual Boolean TryGetChunk(ChunkKey key, out LinearDataChunk<TData> chunk) =>
					throw new NotImplementedException();
			}

			public class SparseDataMap<TData> : DataMapBase, ISparseDataMap where TData : unmanaged
			{
				[CreateProperty] protected NativeParallelHashMap<ChunkKey, SparseDataChunk<TData>> sparseChunks;

				public virtual Boolean TryGetChunk(ChunkKey key, out SparseDataChunk<TData> chunk) =>
					throw new NotImplementedException();
			}
		}

		namespace DataMapStreaming
		{
			public class StreamingLinearDataMap<TData> : LinearDataMap<TData> where TData : unmanaged
			{
				public override Boolean TryGetChunk(ChunkKey key, out LinearDataChunk<TData> chunk)
				{
					if (base.TryGetChunk(key, out chunk))
						return true;

					// try get chunk from file system
					// may decide to dispose least recently used chunks
					throw new NotImplementedException();
				}
			}

			public class StreamingSparseDataMap<TData> : SparseDataMap<TData> where TData : unmanaged
			{
				public override Boolean TryGetChunk(ChunkKey key, out SparseDataChunk<TData> chunk)
				{
					if (base.TryGetChunk(key, out chunk))
						return true;

					// try get chunk from file system
					// may decide to dispose least recently used chunks
					throw new NotImplementedException();
				}
			}
		}

		namespace GridMap
		{
			public interface IBinaryReader
			{
				public T ReadNext<T>() where T : unmanaged;
			}

			public interface IBinaryWriter
			{
				public void Add<T>(T value) where T : unmanaged;
			}

			public abstract class GridMapBase
			{
				private readonly List<ILinearDataMap> m_LinearMaps = new();
				private readonly List<ISparseDataMap> m_SparseMaps = new();
				private ChunkSize m_ChunkSize = new(2, 2, 2);

				public void Add(ILinearDataMap dataMap) => m_LinearMaps.Add(dataMap);
				public void Add(ISparseDataMap dataMap) => m_SparseMaps.Add(dataMap);

				public virtual void Serialize(IBinaryWriter writer)
				{
					//writer.Add(..);
				}

				public virtual GridMapBase Deserialize(IBinaryReader reader) =>
					// deserialize base class fields first
					//baseField = reader.ReadNext<Byte>();
					this;

				// used for undo/redo
				// for now keep the entire serialized map as a byte[] and make it a serializefield on the MB
				// we may later add a byte[][] SerializeModifiedChunks() to decrease the load
				// Unity can serialize jagged array if we wrap it: struct chunks{ byte[] chunkSer; }
				//public Byte[] SerializeAll() => default;

				// ITilemapSerialization (handle different ways of serialization, may or may not compress)
				// created by abstract factory

				// TODO:
				// Undo/Redo:
			}

			public class GridMapUndoRedo {}
		}

		namespace Serialization
		{
			public unsafe struct BinaryReader : IBinaryReader
			{
				private readonly UnsafeAppendBuffer.Reader* m_Reader;

				public BinaryReader(UnsafeAppendBuffer.Reader* reader) => m_Reader = reader;
				public T ReadNext<T>() where T : unmanaged => m_Reader->ReadNext<T>();
			}

			public unsafe struct BinaryWriter : IBinaryWriter
			{
				private readonly UnsafeAppendBuffer* m_Writer;
				public BinaryWriter(UnsafeAppendBuffer* writer) => m_Writer = writer;
				public void Add<T>(T value) where T : unmanaged => m_Writer->Add(value);
			}
		}

		namespace UnitySerialization
		{
			public static class Serialize
			{
				public static unsafe Byte[] ToBinary<T>(T data, List<IBinaryAdapter> adapters = null)
				{
					var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp);
					var parameters = new BinarySerializationParameters { UserDefinedAdapters = adapters };
					BinarySerialization.ToBinary(&stream, data, parameters);

					var bytes = stream.ToBytesNBC();
					stream.Dispose();

					return bytes;
				}

				public static unsafe T FromBinary<T>(Byte[] bytes, List<IBinaryAdapter> adapters = null)
				{
					fixed (Byte* ptr = bytes)
					{
						var reader = new UnsafeAppendBuffer.Reader(ptr, bytes.Length);
						var parameters = new BinarySerializationParameters { UserDefinedAdapters = adapters };
						return BinarySerialization.FromBinary<T>(&reader, parameters);
					}
				}
			}

			public abstract class VersionedBinaryAdapter
			{
				/// <summary>
				///     Can be used to identify the version of the serialized data.
				///     Usually you would bump the version when adding fields AND whenever you want existing data
				///     to be preserved. Then add a version switch to read the old data format and set any new
				///     fields to default or computed values or skip fields that no longer exist.
				///     You might think that a byte is insufficient but in fact you will rarely maintain more than a few
				///     versions back, particularly during development. Once deployed, you will have to make old data
				///     incompatible after 256 changes to the format - highly unlikely after release.
				///     In general you are well advised to retain backward compatibility only a few versions back because
				///     maintaining those versions will be a pain in the back since you will always have to ensure that
				///     loading a version "n-1" to "n-x" binary format will all continue to work and you need to test each
				///     migration path.
				/// </summary>
				protected Byte Version { get; set; }
				public VersionedBinaryAdapter(Byte version) => Version = version;
				protected unsafe void WriteVersion(UnsafeAppendBuffer* writer) => writer->Add(Version);

				protected unsafe void ReadVersion(UnsafeAppendBuffer.Reader* reader) =>
					Version = reader->ReadNext<Byte>();
			}

			public sealed class LinearDataChunkBinaryAdapter<TData> : VersionedBinaryAdapter,
				IBinaryAdapter<LinearDataChunk<TData>>
				where TData : unmanaged
			{
				public LinearDataChunkBinaryAdapter(Byte version)
					: base(version) {}

				public void Serialize(in BinarySerializationContext<LinearDataChunk<TData>> context,
					LinearDataChunk<TData> value) => throw new NotImplementedException();

				public LinearDataChunk<TData> Deserialize(
					in BinaryDeserializationContext<LinearDataChunk<TData>> context) =>
					throw new NotImplementedException();
			}

			public sealed class SparseDataChunkBinaryAdapter<TData> : VersionedBinaryAdapter,
				IBinaryAdapter<SparseDataChunk<TData>>
				where TData : unmanaged
			{
				public SparseDataChunkBinaryAdapter(Byte version)
					: base(version) {}

				public void Serialize(in BinarySerializationContext<SparseDataChunk<TData>> context,
					SparseDataChunk<TData> value) => throw new NotImplementedException();

				public SparseDataChunk<TData> Deserialize(
					in BinaryDeserializationContext<SparseDataChunk<TData>> context) =>
					throw new NotImplementedException();
			}

			public sealed class DataMapBinaryAdapter<TDataMap> : VersionedBinaryAdapter, IBinaryAdapter<TDataMap>
				where TDataMap : DataMapBase, new()
			{
				public DataMapBinaryAdapter(Byte version)
					: base(version) {}

				public unsafe void Serialize(in BinarySerializationContext<TDataMap> context,
					TDataMap value) => value.Serialize(new BinaryWriter(context.Writer));

				public unsafe TDataMap Deserialize(in BinaryDeserializationContext<TDataMap> context) =>
					new TDataMap().Deserialize(new BinaryReader(context.Reader)) as TDataMap;
			}

			public sealed class GridMapBaseBinaryAdapter<TGridMap> : VersionedBinaryAdapter, IBinaryAdapter<TGridMap>
				where TGridMap : GridMapBase, new()
			{
				public GridMapBaseBinaryAdapter(Byte version)
					: base(version) {}

				public unsafe void Serialize(in BinarySerializationContext<TGridMap> context, TGridMap value) =>
					value.Serialize(new BinaryWriter(context.Writer));

				public unsafe TGridMap Deserialize(in BinaryDeserializationContext<TGridMap> context) =>
					new TGridMap().Deserialize(new BinaryReader(context.Reader)) as TGridMap;
			}
		}

		namespace TilemapGame
		{
			namespace TilemapData
			{
				[StructLayout(LayoutKind.Sequential)]
				public struct MyLinearTileData
				{
					public UInt16 TileIndex;
				}

				[StructLayout(LayoutKind.Sequential)]
				public struct MyNavSparseTileData
				{
					public UInt32 NavigationFlags;
					public UInt16 AIStateFlags;
				}

				[StructLayout(LayoutKind.Sequential)]
				public struct MyVisSparseTileData
				{
					public UInt16 RenderFlags;
					public Byte TileFlags;
				}
			}

			public class Tilemap3D : GridMapBase
			{
				public Tilemap3D()
				{
					// no streaming
					Add(new LinearDataMap<MyLinearTileData>());
					Add(new SparseDataMap<MyNavSparseTileData>());

					// streaming with customizable strategy & tech
					Add(new StreamingLinearDataMap<MyLinearTileData>());
					Add(new StreamingSparseDataMap<MyVisSparseTileData>());
				}

				// override when adding fields to the subclass
				public override void Serialize(IBinaryWriter writer) => base.Serialize(writer);
				public override GridMapBase Deserialize(IBinaryReader reader) => base.Deserialize(reader);
			}
		}

		namespace VoxelGame
		{
			namespace VoxelMapData
			{
				[StructLayout(LayoutKind.Sequential)]
				public struct MyLinearVoxelData
				{
					public Byte ColorIndex;
				}

				[StructLayout(LayoutKind.Sequential)]
				public struct MySparseVoxelData
				{
					public Byte InteractionFlags;
					public Byte RenderFlags;
				}
			}

			public class VoxelMap : GridMapBase
			{
				public VoxelMap()
				{
					// no streaming
					Add(new LinearDataMap<MyLinearVoxelData>());
					Add(new SparseDataMap<MySparseVoxelData>());

					// streaming with customizable strategy & tech
					Add(new StreamingLinearDataMap<MyLinearVoxelData>());
					Add(new StreamingSparseDataMap<MySparseVoxelData>());
				}

				// override when adding fields to the subclass
				public override void Serialize(IBinaryWriter writer) => base.Serialize(writer);
				public override GridMapBase Deserialize(IBinaryReader reader) => base.Deserialize(reader);
			}
		}

		namespace ExternalClassStubs
		{
			public sealed class SaveLoadExample<TGridMap, TLinearData, TSparseData>
				where TGridMap : GridMapBase, new() where TLinearData : unmanaged where TSparseData : unmanaged
			{
				private List<IBinaryAdapter> s_GridMapAdapters = new()
				{
					new LinearDataChunkBinaryAdapter<LinearDataChunk<TLinearData>>(0),
					new DataMapBinaryAdapter<LinearDataMap<TLinearData>>(0),

					new SparseDataChunkBinaryAdapter<SparseDataChunk<TSparseData>>(0),
					new DataMapBinaryAdapter<LinearDataMap<TSparseData>>(0),

					new GridMapBaseBinaryAdapter<TGridMap>(0),
				};

				public List<IBinaryAdapter> GridMapAdapters
				{
					get => s_GridMapAdapters;
					set => s_GridMapAdapters = value;
				}

				public Byte[] SerializeGridMap(GridMapBase map) => Serialize.ToBinary(map, GridMapAdapters);

				public GridMapBase DeserializeGridMap<T>(Byte[] bytes) where T : GridMapBase =>
					Serialize.FromBinary<T>(bytes, GridMapAdapters);
			}
		}
	}
}
