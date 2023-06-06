// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using ChunkKey = System.Int64;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public class NestedNativeCollectionSerializationTests
	{
		[Test] public void CanSerializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(1, TileFlags.DirectionWest);

			var bytes = BinarySerializer.Serialize(linearData);

			Assert.NotNull(bytes);
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(bytes.Length, Is.EqualTo(sizeof(UInt32)));
		}

		[Test] public void CanSerializeAndDeserializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(123, TileFlags.DirectionEast | TileFlags.FlipBoth);

			var bytes = BinarySerializer.Serialize(linearData);
			var deserializedData = BinarySerializer.Deserialize<LinearTileData>(bytes);

			Assert.That(deserializedData, Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeNativeListOfLinearTileDataStruct()
		{
			var linearData = new LinearTileData(2, TileFlags.DirectionSouth);
			var list = new NativeList<LinearTileData>(1, Allocator.Temp);
			list.Add(linearData);

			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.NativeListAdapter<LinearTileData>(Allocator.Temp),
			};
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<NativeList<LinearTileData>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(1));
			Assert.That(deserialList[0], Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeUnsafeListOfLinearTileDataStruct()
		{
			var linearData = new LinearTileData(2, TileFlags.DirectionSouth);
			var list = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			list.Add(linearData);

			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.UnsafeListAdapter<LinearTileData>(Allocator.Temp),
			};
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<UnsafeList<LinearTileData>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(1));
			Assert.That(deserialList[0], Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeNativeListOfUnsafeListOfLinearTileDataStruct()
		{
			var linearData1 = new LinearTileData(2, TileFlags.DirectionSouth);
			var linearData2 = new LinearTileData(3, TileFlags.DirectionWest);
			var list = new NativeList<UnsafeList<LinearTileData>>(2, Allocator.Temp);
			var list0 = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			var list1 = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			list0.Add(linearData1);
			list1.Add(linearData2);
			list.Add(list0);
			list.Add(list1);

			Assert.That(list0[0], Is.EqualTo(linearData1));
			Assert.That(list1[0], Is.EqualTo(linearData2));

			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.NativeListAdapter<UnsafeList<LinearTileData>>(Allocator.Temp),
				new BinaryAdapters.UnsafeListAdapter<LinearTileData>(Allocator.Temp),
			};
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<NativeList<UnsafeList<LinearTileData>>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(2));
			var deserialList0 = deserialList[0];
			var deserialList1 = deserialList[1];
			Assert.That(deserialList0[0], Is.EqualTo(linearData1));
			Assert.That(deserialList1[0], Is.EqualTo(linearData2));
		}

		[Test] public void CanSerializeAndDeserializeParallelHashMap()
		{
			var allocator = Allocator.Temp;
			var chunkSize = new ChunkSize(4, 3, 2);
			var chunks = new NativeParallelHashMap<Int64,
				TilemapChunk<LinearTileData, SparseTileData>>(0, allocator);
			var chunk = new TilemapChunk<LinearTileData, SparseTileData>(chunkSize, allocator);

			var linearData = new LinearTileData(2, (TileFlags)3);
			chunk.AddTileData(linearData);
			chunk.AddTileData(linearData);
			chunks.Add(9, chunk);


			var coord = new GridCoord(4, 5, 6);
			var sparseData = new SparseTileData(1234567890);
			chunk.SetTileData(coord, sparseData);
			chunk.SetTileData(coord + new GridCoord(1,1,1), sparseData);


			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.UnsafeListAdapter<LinearTileData>(allocator),
				new BinaryAdapters.UnsafeParallelHashMapAdapter<GridCoord, SparseTileData>(allocator),
				TilemapChunk<LinearTileData, SparseTileData>.BinarySerializationAdapter,
				new BinaryAdapters.NativeParallelHashMapAdapter<Int64, TilemapChunk<LinearTileData, SparseTileData>>(allocator),
			};
			var bytes = BinarySerializer.Serialize(chunks, adapters);
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			var deserialChunks = BinarySerializer.Deserialize<NativeParallelHashMap<Int64,
					TilemapChunk<LinearTileData, SparseTileData>>>(bytes, adapters);

			Assert.That(deserialChunks.Count(), Is.EqualTo(1));
			Assert.That(deserialChunks[9].LinearDataCount, Is.EqualTo(2));
			Assert.That(deserialChunks[9].SparseDataCount, Is.EqualTo(2));
			Assert.That(deserialChunks[9][0], Is.EqualTo(linearData));
			Assert.That(deserialChunks[9][coord], Is.EqualTo(sparseData));
		}
	}
}
