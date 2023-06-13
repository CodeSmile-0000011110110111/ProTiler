// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.DataChunks;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps.Chunks;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps.Streaming;
using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.CodeDesign.v4.DataMapStreaming;
using CodeSmile.ProTiler.CodeDesign.v4.TilemapGame.TilemapData;
using CodeSmile.ProTiler.CodeDesign.v4.VoxelGame.VoxelMapData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
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

			public abstract class DataMapBase<TData> where TData : unmanaged
			{
				// coord to chunk key
				// hashmap of modified (unsaved) chunks
				// possibly: hashmap of loaded chunks together with access timestamp

				// create instance of undo/redo system (editor and runtime edit-mode)
			}

			public class LinearDataMap<TData> : DataMapBase<TData>, ILinearDataMap where TData : unmanaged
			{
				[CreateProperty] protected NativeParallelHashMap<ChunkKey, LinearDataChunk<TData>> m_DataChunks;

				public virtual Boolean TryGetChunk(ChunkKey key, out LinearDataChunk<TData> chunk) =>
					throw new NotImplementedException();
			}

			public class SparseDataMap<TData> : DataMapBase<TData>, ISparseDataMap where TData : unmanaged
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

			public class GridMapUndoRedo
			{

			}
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

				// only override when adding fields to the custom subclass
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

				// only override when adding fields to the custom subclass
				public override void Serialize(IBinaryWriter writer) => base.Serialize(writer);
				public override GridMapBase Deserialize(IBinaryReader reader) => base.Deserialize(reader);
			}
		}
	}
}
