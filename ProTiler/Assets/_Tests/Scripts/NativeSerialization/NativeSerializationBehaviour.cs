// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using UnityEditor;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using Object = UnityEngine.Object;
using WorldPos = Unity.Mathematics.float3;

[Flags]
public enum TileFlags : UInt16
{
	None = 0,

	DirectionNorth = 1 << 0,
	DirectionEast = 1 << 1,
	DirectionSouth = 1 << 2,
	DirectionWest = 1 << 3,
	FlipHorizontal = 1 << 4,
	FlipVertical = 1 << 5,
	FlipBoth = FlipHorizontal | FlipVertical,

	BitCount = 6,
}

public interface ITileData
{
	public UInt16 TileIndex { get; set; }
	public TileFlags TileFlags { get; set; }
	public UInt32 TileIndexFlags { get; set; }
}

/// <summary>
///     Data for every tile in a chunk/layer. Be very considerate of what to add here and what type as
///     this can have a huge impact on memory/disk usage of the map.
/// </summary>
[BurstCompile]
[StructLayout(LayoutKind.Explicit)]
public struct TileData : ITileData
{
	[FieldOffset(0)]
	private UInt16 m_TileIndex;
	[FieldOffset(2)]
	private TileFlags m_TileFlags;
	[FieldOffset(0)]
	private UInt32 m_TileIndexFlags;

	public UInt16 TileIndex { get => m_TileIndex; set => m_TileIndex = value; }
	public TileFlags TileFlags { get => m_TileFlags; set => m_TileFlags = value; }
	public UInt32 TileIndexFlags { get => m_TileIndexFlags; set => m_TileIndexFlags = value; }
}

/// <summary>
///     Data for specific tiles (based on coordinates) in a chunk/layer.
///     Use this for flagging tiles where most tiles use default data and only some tiles require
///     additional or custom data.
/// </summary>
[BurstCompile]
[StructLayout(LayoutKind.Sequential)]
public struct SparseTileData
{
	// empty is default
	// extend this as needed on per-project basis
}

[BurstCompile]
[StructLayout(LayoutKind.Sequential)]
public struct ChunkLinearData<TLinearData> where TLinearData : unmanaged, ILinearData
{
	private readonly UnsafeList<UnsafeList<TLinearData>> m_ChunkData;
}

public interface ILinearData {}
public interface ISparseData {}

[BurstCompile]
[StructLayout(LayoutKind.Sequential)]
public struct ChunkData<TLinear, TSparse>
	where TLinear : unmanaged, ILinearData
	where TSparse : unmanaged, ISparseData
{
	private readonly ChunkSize m_ChunkSize;
	private readonly UnsafeList<UnsafeList<TLinear>> m_LinearData;
	private readonly UnsafeList<UnsafeParallelHashMap<GridCoord, TSparse>> m_SparseData;
}

// generic type can be simple types (int2, int4) or structs
// MB acts as a factory for various types, inspector lets you select predefined types
// MB has mechanism to extend with custom types - how? static "create tilemap" event method
// that allows you to pass types?
public class Tilemap3DBase<TLinear, TSparse>
	where TLinear : unmanaged, ILinearData
	where TSparse : unmanaged, ISparseData
{
	private NativeParallelHashMap<ChunkKey, ChunkData<TLinear, TSparse>> m_ChunkData;
	private FixedString512Bytes m_ExpectedChunkDataTypes;

	private NativeParallelHashMap<ChunkKey, ChunkLinearData<TLinear>> m_ChunkLinearData;
	// chunked Tile Index + Flags
	private NativeParallelHashMap<ChunkKey, UnsafeList<TLinear>> m_ChunkedTileData;
	// chunked user-defined per-coord struct
	private NativeParallelHashMap<ChunkKey, UnsafeParallelHashMap<GridCoord, TSparse>> m_ChunkedCoordTileData;
	//FixedList4096Bytes<T>
	//NativeArray<>
}

//public class Tilemap3D : Tilemap3DBase {}

//[Serializable]
[BurstCompile]
[StructLayout(LayoutKind.Sequential)]
public struct DataContainer
{
	public UInt16 TileIndex;
	public UInt16 TileFlags;

	public DataContainer(Byte startValue)
	{
		TileIndex = startValue;
		TileFlags = (Byte)(TileIndex + 1);
	}

	[BurstCompile]
	public void Test()
	{
		var x = TileIndex + TileFlags;
		TileIndex = (UInt16)(TileFlags - x);
	}
}

public struct NativeListDataContainerAdapter : IBinaryAdapter<NativeList<DataContainer>>
{
	public unsafe void Serialize(in BinarySerializationContext<NativeList<DataContainer>> context,
		NativeList<DataContainer> value)
	{
		var writer = context.Writer;

		if (value.Length > UInt16.MaxValue)
			throw new ArgumentException();

		writer->Add((UInt16)value.Length);

		for (var i = 0; i < value.Length; i++)
		{
			writer->Add(value[i].TileIndex);
			writer->Add(value[i].TileFlags);

			// var byteArray = value[i].ByteArray;
			// writer->Add(byteArray.Length);
			// for (var k = 0; k < byteArray.Length; k++)
			// 	writer->Add(byteArray[k]);
		}
	}

	public unsafe NativeList<DataContainer> Deserialize(
		in BinaryDeserializationContext<NativeList<DataContainer>> context)
	{
		var reader = context.Reader;

		var containerLength = reader->ReadNext<UInt16>();
		var value = new NativeList<DataContainer>(containerLength, Allocator.Persistent);

		for (var i = 0; i < containerLength; i++)
		{
			var data = new DataContainer();

			data.TileIndex = reader->ReadNext<UInt16>();
			data.TileFlags = reader->ReadNext<UInt16>();

			// var unsafeListLength = reader->ReadNext<Int32>();
			// data.ByteArray = new UnsafeList<Byte>(unsafeListLength, Allocator.Persistent);
			// for (var k = 0; k < unsafeListLength; k++)
			// 	data.ByteArray.Add(reader->ReadNext<Byte>());

			value.Add(data);
		}

		return value;
	}
}

[ExecuteAlways]
public class NativeSerializationBehaviour : MonoBehaviour
{
	[SerializeField] private Object m_TestFolderAsset;
	private NativeList<DataContainer> m_DataContainers;

	private void Update()
	{
		DisposeDataContainer(m_DataContainers);

		m_DataContainers = new NativeList<DataContainer>(Allocator.Persistent);
		m_DataContainers.Add(new DataContainer(4));

		var bytes = Serialize(m_DataContainers);

		var deserializedList = Deserialize(bytes);
		var deserializedBytes = Serialize(deserializedList);
		DisposeDataContainer(deserializedList);

		var sb = new StringBuilder();
		foreach (var b in bytes)
			sb.Append(b);

		var sb2 = new StringBuilder();
		foreach (var b in deserializedBytes)
			sb2.Append(b);

		Debug.Log($"{sb} ?? {sb2} == {sb.Equals(sb2)}");

#if UNITY_EDITOR
		var folderAsset = AssetDatabase.LoadAssetAtPath<Object>("Assets/_Tests/FolderAsset");
		Debug.Log($"asset for folder: {folderAsset} {AssetDatabase.GetAssetPath(folderAsset)}");
		if (folderAsset != null)
			m_TestFolderAsset = folderAsset;
		else if (m_TestFolderAsset != null)
			Debug.Log($"folder asset after move: {AssetDatabase.GetAssetPath(m_TestFolderAsset)}");
#endif
	}

	private void OnDisable() => DisposeDataContainer(m_DataContainers);

	private unsafe NativeList<DataContainer> Deserialize(Byte[] bytes)
	{
		fixed (Byte* ptr = bytes)
		{
			var reader = new UnsafeAppendBuffer.Reader(ptr, bytes.Length);

			return BinarySerialization.FromBinary<NativeList<DataContainer>>(&reader,
				new BinarySerializationParameters
					{ UserDefinedAdapters = new List<IBinaryAdapter> { new NativeListDataContainerAdapter() } });
		}
	}

	private unsafe Byte[] Serialize(NativeList<DataContainer> dataContainers)
	{
		var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp);
		BinarySerialization.ToBinary(&stream, dataContainers,
			new BinarySerializationParameters
				{ UserDefinedAdapters = new List<IBinaryAdapter> { new NativeListDataContainerAdapter() } });

		var bytes = stream.ToBytesNBC();
		stream.Dispose();
		return bytes;
	}

	private void DisposeDataContainer(NativeList<DataContainer> dataContainers)
	{
		if (dataContainers.IsCreated)
		{
			// foreach (var dataContainer in dataContainers)
			// 	dataContainer.ByteArray.Dispose();
			dataContainers.Dispose();
		}
	}
}
