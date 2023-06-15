// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.CodeDesign.v4.TilemapGame.TileData;
using CodeSmile.ProTiler.CodeDesign.v4.VoxelGame.VoxelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Serialization.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ChunkCoord = Unity.Mathematics.int2;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using Object = System.Object;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.CodeDesign
{
	namespace v4
	{

		namespace GridMap
		{
			public interface IDataMapStream {}

			[ExecuteAlways]
			public abstract class GridMapBaseBehaviour<T> : MonoBehaviour, ISerializationCallbackReceiver
				where T : GridBase, new()
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

				public void Serialize<T>(T gridMap, IReadOnlyList<IBinaryAdapter> adapters) where T : GridBase
				{
					var binaryAdapters = adapters.Cast<IBinaryAdapter>().ToList();
					m_SerializedGridMap = CodeSmile.Serialization.Serialize.ToBinary(gridMap, binaryAdapters);
				}

				public T Deserialize<T>(IReadOnlyList<IBinaryAdapter> adapters) where T : GridBase
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
			public class Tilemap3D : GridBase
			{
				public Tilemap3D()
				{
					// no streaming
					AddLinearDataMap<MyLinearTileData>(0);
					AddSparseDataMap<MyNavSparseTileData>(0);

					// streaming with customizable strategy & tech
					var streamingInterfaceDummy = new Object() as IDataMapStream;
					AddSparseDataMap<MyVisSparseTileData>(0, streamingInterfaceDummy);
				}

				// override when adding fields to the subclass
				public override void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
				{
					Debug.Log("Tilemap Sserialize");
					base.Serialize(context);
				}

				public override GridBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
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
			public class Voxel : GridBase
			{
				public Voxel()
				{
					// streaming
					var streamingInterfaceDummy = new Object() as IDataMapStream;
					AddLinearDataMap<MyLinearVoxelData>(0,streamingInterfaceDummy);
					// no streaming
					AddSparseDataMap<MySparseVoxelData>(0);

					// add the map's adapter last for minimally faster serialization
					AddGridMapSerializationAdapter<Voxel>(0);
				}

				// override when adding fields to the subclass
				public override void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
				{
					Debug.Log("VoxelMap Sserialize");
					base.Serialize(context);
				}

				public override GridBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
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
