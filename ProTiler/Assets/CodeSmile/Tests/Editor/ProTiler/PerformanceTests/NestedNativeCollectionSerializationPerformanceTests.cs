// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Editor.ProTiler.ExplorationTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.PerformanceTesting;
using Unity.Serialization.Binary;

namespace CodeSmile.Tests.Editor.ProTiler.PerformanceTests
{
	public class NestedNativeCollectionSerializationPerformanceTests
	{
		[Performance] [TestCase(1000, 65536*4)]
		public void NestedNativeCollectionSerializeAndDeserializePerformance(int listCount, int subListCount)
		{
			var linearData1 = new LinearTileData(2, TileFlags.DirectionSouth);

			var list = new NativeList<UnsafeList<LinearTileData>>(listCount, Allocator.Temp);
			for (var l = 0; l < listCount; l++)
			{
				var subList = new UnsafeList<LinearTileData>(subListCount, Allocator.Temp);
				list.Add(subList);

				for (var i = 0; i < subListCount; i++)
					subList.Add(linearData1);
			}

			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.NativeListAdapter<UnsafeList<LinearTileData>>(Allocator.Temp),
				new BinaryAdapters.UnsafeListAdapter<LinearTileData>(Allocator.Temp),
			};

			Byte[] bytes = null;
			Measure.Method(() =>
				{
					bytes = Serialize.ToBinary(list, adapters);
				})
				.DynamicMeasurementCount()
				.SampleGroup("Serialize")
				.Run();

			list = default;
			Measure.Method(() =>
				{
					list = Serialize.FromBinary<NativeList<UnsafeList<LinearTileData>>>(bytes, adapters);
				})
				.DynamicMeasurementCount()
				.SampleGroup("De-Serialize")
				.Run();
		}
	}
}
