// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.DataChunks;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps;
using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.CodeDesign.v4.Serialization;
using CodeSmile.ProTiler.CodeDesign.v4.TilemapGame.TileData;
using CodeSmile.ProTiler.CodeDesign.v4.VoxelGame.VoxelData;
using CodeSmile.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using Unity.Serialization.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using Object = System.Object;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.CodeDesign
{
	namespace v4
	{
		namespace Serialization
		{
			public sealed class LinearDataChunkBinaryAdapter<TData> : VersionedBinaryAdapter,
				IBinaryAdapter<LinearDataChunk<TData>>
				where TData : unmanaged
			{
				private const Byte CurrentVersion = 0;
				public LinearDataChunkBinaryAdapter()
					: this(CurrentVersion) {}
				private LinearDataChunkBinaryAdapter(Byte version)
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
				private const Byte CurrentVersion = 0;
				public SparseDataChunkBinaryAdapter()
					: this(CurrentVersion) {}
				private SparseDataChunkBinaryAdapter(Byte version)
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
				private const Byte CurrentVersion = 0;
				public DataMapBinaryAdapter()
					: this(CurrentVersion) {}
				private DataMapBinaryAdapter(Byte version)
					: base(version) {}

				public unsafe void Serialize(in BinarySerializationContext<TDataMap> context,
					TDataMap value) => value.Serialize(new BinaryWriter(context.Writer));

				public unsafe TDataMap Deserialize(in BinaryDeserializationContext<TDataMap> context) =>
					new TDataMap().Deserialize(new BinaryReader(context.Reader)) as TDataMap;
			}

			public sealed class GridMapBinaryAdapter<TGridMap> : VersionedBinaryAdapter, IBinaryAdapter<TGridMap>
				where TGridMap : GridMapBase, new()
			{
				public GridMapBinaryAdapter(byte version)
					: base(version) {}

				public unsafe void Serialize(in BinarySerializationContext<TGridMap> context, TGridMap map)
				{
					WriteAdapterVersion(context.Writer);
					map.Serialize(context);
				}

				public unsafe TGridMap Deserialize(in BinaryDeserializationContext<TGridMap> context)
				{
					ReadSerializedVersion(context.Reader);
					return new TGridMap().Deserialize(context, AdapterVersion) as TGridMap;
				}
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
			// avoid <TData> because of serialization?
			public abstract class DataMapBase //<TData> where TData : unmanaged
			{
				protected IMapDataStream m_Stream;
				public DataMapBase() {}

				public DataMapBase(IMapDataStream stream) => m_Stream = stream;

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

			public class LinearDataMap<TData> : DataMapBase where TData : unmanaged
			{
				[CreateProperty] protected NativeParallelHashMap<ChunkKey, LinearDataChunk<TData>> m_DataChunks;

				public LinearDataMap() {}

				public LinearDataMap(IMapDataStream stream)
					: base(stream) {}

				public Boolean TryGetChunk(ChunkKey key, out LinearDataChunk<TData> chunk) => throw
					// try get from HashMap first
					//if (base.TryGetChunk(key, out chunk)) return true;
					// try get chunk from stream
					// may decide to dispose least recently used chunks
					new NotImplementedException();
			}

			public class SparseDataMap<TData> : DataMapBase where TData : unmanaged
			{
				[CreateProperty] protected NativeParallelHashMap<ChunkKey, SparseDataChunk<TData>> sparseChunks;

				public SparseDataMap() {}

				public SparseDataMap(IMapDataStream stream)
					: base(stream) {}

				public Boolean TryGetChunk(ChunkKey key, out SparseDataChunk<TData> chunk) => throw
					// try get from HashMap first
					//if (base.TryGetChunk(key, out chunk)) return true;
					// try get chunk from stream
					// may decide to dispose least recently used chunks
					new NotImplementedException();
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

			public interface IMapDataStream {}

			public abstract class GridMapBase
			{
				private readonly List<DataMapBase> m_LinearMaps = new();
				private readonly List<DataMapBase> m_SparseMaps = new();
				private ChunkSize m_ChunkSize = new(2, 2, 2);

				protected readonly List<IBinaryAdapter> m_SerializationAdapters =new();
				public IReadOnlyList<IBinaryAdapter> SerializationAdapters => m_SerializationAdapters;

				public void AddLinearDataMap<T>(IMapDataStream stream = null) where T : unmanaged
				{
					m_LinearMaps.Add(new LinearDataMap<T>(stream));
					m_SerializationAdapters.Add(new LinearDataChunkBinaryAdapter<T>());
					m_SerializationAdapters.Add(new DataMapBinaryAdapter<LinearDataMap<T>>());
				}

				public void AddSparseDataMap<T>(IMapDataStream stream = null) where T : unmanaged
				{
					m_SparseMaps.Add(new SparseDataMap<T>(stream));
					m_SerializationAdapters.Add(new SparseDataChunkBinaryAdapter<T>());
					m_SerializationAdapters.Add(new DataMapBinaryAdapter<SparseDataMap<T>>());
				}

				public virtual void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
					where TGridMap : GridMapBase, new()
				{
					context.SerializeValue(m_ChunkSize);
					SerializeMaps(context, m_LinearMaps);
					SerializeMaps(context, m_SparseMaps);
				}

				private unsafe void SerializeMaps<TGridMap>(BinarySerializationContext<TGridMap> context,
					List<DataMapBase> list)
					where TGridMap : GridMapBase, new()
				{
					var itemCount = list.Count;
					context.Writer->Add(itemCount);
					for (var i = 0; i < itemCount; i++)
						context.SerializeValue(list[i]);
				}

				// TODO: write maps
				public virtual GridMapBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
					Byte version)
					where TGridMap : GridMapBase, new()
				{
					m_ChunkSize = context.DeserializeValue<ChunkSize>();
					return this;
				}

				public override String ToString() => $"{nameof(GridMapBase)}(ChunkSize: {m_ChunkSize})";

				protected void AddGridMapSerializationAdapter<T>(byte version) where T:GridMapBase, new()
				{
					m_SerializationAdapters.Add(new GridMapBinaryAdapter<T>(0));
				}
			}

			[ExecuteAlways]
			public abstract class GridMapBaseBehaviour<T> : MonoBehaviour, ISerializationCallbackReceiver
				where T : GridMapBase, new()
			{
				[SerializeReference] private T m_GridMap = new();
				[SerializeReference] protected GridMapBinarySerialization m_GridMapBinarySerialization = new();

				public T GridMap => m_GridMap;
				public void OnBeforeSerialize() => SerializeMap();
				public void OnAfterDeserialize() => DeserializeMap();

				public void SerializeMap() =>
					m_GridMapBinarySerialization.Serialize(m_GridMap, m_GridMap.SerializationAdapters);

				public void DeserializeMap() =>
					m_GridMap = m_GridMapBinarySerialization.Deserialize<T>(m_GridMap.SerializationAdapters);
			}

			// Consider: make abstract base?
			[Serializable]
			public class GridMapBinarySerialization
			{
				[SerializeField] protected Byte[] m_SerializedGridMap;
				[SerializeField] private SerializedChunkWrapper[] m_SerializedChunks;

				public IReadOnlyList<IBinaryAdapter> GetDefaultAdapters()
				{
					var adapters = new List<IBinaryAdapter>();
					return adapters.AsReadOnly();
				}

				public void Serialize<T>(T gridMap, IReadOnlyList<IBinaryAdapter> adapters) where T : GridMapBase
				{
					var binaryAdapters = adapters.Cast<IBinaryAdapter>().ToList();
					m_SerializedGridMap = CodeSmile.Serialization.Serialize.ToBinary(gridMap, binaryAdapters);
				}

				public T Deserialize<T>(IReadOnlyList<IBinaryAdapter> adapters) where T : GridMapBase
				{
					if (m_SerializedGridMap == null || m_SerializedGridMap.Length == 0)
						return null;

					var binaryAdapters = adapters.Cast<IBinaryAdapter>().ToList();
					return CodeSmile.Serialization.Serialize.FromBinary<T>(m_SerializedGridMap, binaryAdapters);
				}

				[Serializable]
				private struct SerializedChunkWrapper
				{
					[SerializeField] private Byte[] m_SerializedChunk;
					public Byte[] SerializedChunk
					{
						get => m_SerializedChunk;
						set => m_SerializedChunk = value;
					}
				}
			}
		}

		namespace TilemapGame
		{
			namespace TileData
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

			[Serializable]
			public class Tilemap3D : GridMapBase
			{
				public Tilemap3D()
				{
					// no streaming
					AddLinearDataMap<MyLinearTileData>();
					AddSparseDataMap<MyNavSparseTileData>();

					// streaming with customizable strategy & tech
					var streamingInterfaceDummy = new Object() as IMapDataStream;
					AddSparseDataMap<MyVisSparseTileData>(streamingInterfaceDummy);
				}

				// override when adding fields to the subclass
				public override void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
				{
					Debug.Log("Tilemap Sserialize");
					base.Serialize(context);
				}

				public override GridMapBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
					Byte version)
				{
					Debug.Log("Tilemap Deserialize");
					return base.Deserialize(context, version);
				}
			}
		}

		namespace VoxelGame
		{
			namespace VoxelData
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

			[Serializable]
			public class VoxelMap : GridMapBase
			{
				public VoxelMap()
				{
					// streaming
					var streamingInterfaceDummy = new Object() as IMapDataStream;
					AddLinearDataMap<MyLinearVoxelData>(streamingInterfaceDummy);
					// no streaming
					AddSparseDataMap<MySparseVoxelData>();

					// add the map's adapter last for minimally faster serialization
					AddGridMapSerializationAdapter<VoxelMap>(0);
				}

				// override when adding fields to the subclass
				public override void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
				{
					Debug.Log("VoxelMap Sserialize");
					base.Serialize(context);
				}

				public override GridMapBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
					Byte version)
				{
					Debug.Log("VoxelMap Deserialize");
					return base.Deserialize(context, version);
				}
			}
		}

		namespace ExternalClassStubs
		{
			public class UndoRedoEventDispatch {}

			public sealed class UnityEditorEventDispatch : ScriptableSingleton<UnityEditorEventDispatch>
			{
				private UndoRedoEventDispatch m_UndoRedoEventDispatch = new();

#if UNITY_EDITOR

				private void OnEnable()
				{
					EditorApplication.update += EditorUpdate;
					Undo.undoRedoEvent += OnUndoRedoEvent;
					EditorSceneManager.sceneSaving += OnSceneWillSave;
					AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
					AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
				}

				private void EditorUpdate() {}

				private void OnSceneWillSave(Scene scene, String path) {}

				private void OnBeforeAssemblyReload() {}
				private void OnAfterAssemblyReload() {}

				private void OnUndoRedoEvent(in UndoRedoInfo undo) =>
					Debug.Log($"UndoRedo #{undo.undoGroup} '{undo.undoName}', redo: {undo.isRedo}");
#endif
			}
		}
	}
}
