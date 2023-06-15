// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.DataChunks;
using CodeSmile.ProTiler.CodeDesign.v4.DataMaps;
using CodeSmile.ProTiler.CodeDesign.v4.DataMapStreaming;
using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.CodeDesign.v4.Serialization;
using CodeSmile.ProTiler.CodeDesign.v4.TilemapGame.TileData;
using CodeSmile.ProTiler.CodeDesign.v4.UnitySerialization;
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
		namespace UnitySerialization
		{
			public abstract class VersionedBinaryAdapter
			{
				/// <summary>
				///     Can be used to identify the version of the serialized data.
				///     Usually you would bump the version when adding fields AND whenever you want existing data
				///     to be preserved. Then add a version switch to read the old data format and set any new
				///     fields to default/computed values or read & skip data whose fields no longer exist.
				///     You might think that a byte is insufficient but in fact you will rarely maintain more than a few
				///     versions back, particularly during development. You most certainly do NOT want to maintain
				///     over 256 different versions of your serialized data after release. Trust me. ;)
				///     In general you are well advised to retain backward compatibility as few versions back as possible
				///     because maintaining those versions will be a pain in the back since you will always have to ensure
				///     that loading a version "n-1" to "n-x" binary formats will all continue to work and you need
				///     to test each migration path separately.
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

				public unsafe void Serialize(in BinarySerializationContext<TGridMap> context, TGridMap map)
				{
					WriteVersion(context.Writer);
					map.Serialize(context);
				}

				public unsafe TGridMap Deserialize(in BinaryDeserializationContext<TGridMap> context)
				{
					ReadVersion(context.Reader);
					return new TGridMap().Deserialize(context, Version) as TGridMap;
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

				public virtual Boolean TryGetChunk(ChunkKey key, out LinearDataChunk<TData> chunk) =>
					throw new NotImplementedException();
			}

			public class SparseDataMap<TData> : DataMapBase where TData : unmanaged
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

			[Serializable]
			public abstract class GridMapBase
			{
				[CreateProperty] private readonly List<DataMapBase> m_LinearMaps = new();
				[CreateProperty] private readonly List<DataMapBase> m_SparseMaps = new();
				[CreateProperty] private ChunkSize m_ChunkSize = new(2, 2, 2);

				public void AddLinearDataMap(DataMapBase dataMap) => m_LinearMaps.Add(dataMap);
				public void AddSparseDataMap(DataMapBase dataMap) => m_SparseMaps.Add(dataMap);

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

				public virtual List<Object> GetBinaryAdapters() => new();

				public override String ToString() => $"{nameof(GridMapBase)}(ChunkSize: {m_ChunkSize})";
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
					m_GridMapBinarySerialization.Serialize(m_GridMap, m_GridMap.GetBinaryAdapters());

				public void DeserializeMap() =>
					m_GridMap = m_GridMapBinarySerialization.Deserialize<T>(m_GridMap.GetBinaryAdapters());
			}

			// Consider: make abstract base?
			[Serializable]
			public class GridMapBinarySerialization
			{
				[SerializeField] protected Byte[] m_SerializedGridMap;
				[SerializeField] private SerializedChunkWrapper[] m_SerializedChunks;

				public List<Object> GetDefaultAdapters()
				{
					var adapters = new List<IBinaryAdapter>();
					return adapters.Cast<Object>().ToList();
				}

				public void Serialize<T>(T gridMap, List<Object> adapters) where T : GridMapBase
				{
					var binaryAdapters = adapters.Cast<IBinaryAdapter>().ToList();
					m_SerializedGridMap = CodeSmile.Serialization.Serialize.ToBinary(gridMap, binaryAdapters);
				}

				public T Deserialize<T>(List<Object> adapters) where T : GridMapBase
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
					AddLinearDataMap(new LinearDataMap<MyLinearTileData>());
					AddSparseDataMap(new SparseDataMap<MyNavSparseTileData>());

					// streaming with customizable strategy & tech
					AddLinearDataMap(new StreamingLinearDataMap<MyLinearTileData>());
					AddSparseDataMap(new StreamingSparseDataMap<MyVisSparseTileData>());
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
					// no streaming
					AddLinearDataMap(new LinearDataMap<MyLinearVoxelData>());
					AddSparseDataMap(new SparseDataMap<MySparseVoxelData>());

					// streaming with customizable strategy & tech
					AddLinearDataMap(new StreamingLinearDataMap<MyLinearVoxelData>());
					AddSparseDataMap(new StreamingSparseDataMap<MySparseVoxelData>());
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

				public override List<Object> GetBinaryAdapters()
				{
					// TODO: can these adapters be inferred from usage? see ctor
					var list = base.GetBinaryAdapters();
					list.Add(new LinearDataChunkBinaryAdapter<MyLinearVoxelData>(0));
					list.Add(new DataMapBinaryAdapter<LinearDataMap<MyLinearVoxelData>>(0));

					list.Add(new SparseDataChunkBinaryAdapter<MySparseVoxelData>(0));
					list.Add(new DataMapBinaryAdapter<SparseDataMap<MySparseVoxelData>>(0));

					list.Add(new GridMapBaseBinaryAdapter<VoxelMap>(0));
					return list;
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
