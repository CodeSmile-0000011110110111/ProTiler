// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using CodeSmile.ProTiler.Runtime.CodeDesign.v4.TilemapGame.TileData;
using CodeSmile.ProTiler.Runtime.CodeDesign.v4.VoxelGame.VoxelData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Serialization.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace CodeSmile.ProTiler.Runtime.CodeDesign
{
	namespace v4
	{

		namespace GridMap
		{
			public interface IDataMapStream {}

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
					m_SerializedGridMap = Core.Serialization.Serialize.ToBinary(gridMap, adapters);
				}

				public T Deserialize<T>(IReadOnlyList<IBinaryAdapter> adapters) where T : GridBase
				{
					if (m_SerializedGridMap == null || m_SerializedGridMap.Length == 0)
						return null;

					return Core.Serialization.Serialize.FromBinary<T>(m_SerializedGridMap, adapters);
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
				public Tilemap3D() : base(0)
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
				public Voxel() : base(0)
				{
					// streaming
					var streamingInterfaceDummy = new Object() as IDataMapStream;
					AddLinearDataMap<MyLinearVoxelData>(0,streamingInterfaceDummy);
					// no streaming
					AddSparseDataMap<MySparseVoxelData>(0);
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
