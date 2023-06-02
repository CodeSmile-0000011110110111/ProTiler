// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using UnityEngine;

//[Serializable]
public struct DataContainer
{
	public Byte ByteValue1;
	public Byte ByteValue2;
	public Byte ByteValue3;
	public Byte ByteValue4;
	public UnsafeList<Byte> ByteArray;

	public DataContainer(Byte startValue)
	{
		ByteValue1 = startValue;
		ByteValue2 = (Byte)(ByteValue1 + 1);
		ByteValue3 = (Byte)(ByteValue2 + 1);
		ByteValue4 = (Byte)(ByteValue3 + 1);

		ByteArray = new UnsafeList<Byte>(5, Allocator.Persistent);
		ByteArray.Add(ByteValue1);
		ByteArray.Add(ByteValue2);
		ByteArray.Add(ByteValue3);
		ByteArray.Add(ByteValue4);
		ByteArray.Add((Byte)(ByteValue1 + ByteValue2 + ByteValue3 + ByteValue4));
	}

	// [CreateProperty] public Int32 IntValue;
	// [CreateProperty] public Single FloatValue;
	// [CreateProperty] public Double DoubleValue;
	// [CreateProperty] public int4 Int4Value;
}

public class NativeListDataContainerAdapter : IBinaryAdapter<NativeList<DataContainer>>
{
	public unsafe void Serialize(in BinarySerializationContext<NativeList<DataContainer>> context,
		NativeList<DataContainer> value)
	{
		var writer = context.Writer;

		writer->Add(value.Length);

		for (var i = 0; i < value.Length; i++)
		{
			writer->Add(value[i].ByteValue1);
			writer->Add(value[i].ByteValue2);
			writer->Add(value[i].ByteValue3);
			writer->Add(value[i].ByteValue4);

			var byteArray = value[i].ByteArray;
			writer->Add(byteArray.Length);
			for (var k = 0; k < byteArray.Length; k++)
				writer->Add(byteArray[k]);
		}
	}

	public unsafe NativeList<DataContainer> Deserialize(
		in BinaryDeserializationContext<NativeList<DataContainer>> context)
	{
		var reader = context.Reader;

		var containerLength = reader->ReadNext<Int32>();
		var value = new NativeList<DataContainer>(containerLength, Allocator.Persistent);

		for (var i = 0; i < containerLength; i++)
		{
			var data = new DataContainer();

			data.ByteValue1 = reader->ReadNext<Byte>();
			data.ByteValue2 = reader->ReadNext<Byte>();
			data.ByteValue3 = reader->ReadNext<Byte>();
			data.ByteValue4 = reader->ReadNext<Byte>();

			var unsafeListLength = reader->ReadNext<Int32>();
			data.ByteArray = new UnsafeList<Byte>(unsafeListLength, Allocator.Persistent);
			for (var k = 0; k < unsafeListLength; k++)
				data.ByteArray.Add(reader->ReadNext<Byte>());

			value.Add(data);
		}

		return value;
	}
}

[ExecuteAlways]
public class NativeSerializationBehaviour : MonoBehaviour
{
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
			foreach (var dataContainer in dataContainers)
				dataContainer.ByteArray.Dispose();
			dataContainers.Dispose();
		}
	}

	/*
	private Byte[] ObjectToByteArray<T>(T obj)
	{
		using (var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp))
		{
			unsafe
			{
				BinarySerialization.ToBinary(&stream, obj, new BinarySerializationParameters
				{
					UserDefinedAdapters = new List<IBinaryAdapter>
					{
						new NativeArrayAdapter(),
					},
				});
			}

			return stream.ToBytes();
		}
	}

	private T ByteArrayToObject<T>(Byte[] arr)
	{
		unsafe
		{
			fixed (Byte* ptr = arr)
			{
				var reader = new UnsafeAppendBuffer.Reader(ptr, arr.Length);

				return BinarySerialization.FromBinary<T>(&reader, new BinarySerializationParameters
				{
					UserDefinedAdapters = new List<IBinaryAdapter>
					{
						new NativeArrayAdapter(),
					},
				});
			}
		}
	}

	private class NativeArrayAdapter : IBinaryAdapter<NativeArray<Int32>>
	{
		public unsafe void Serialize(in BinarySerializationContext<NativeArray<Int32>> context,
			NativeArray<Int32> value)
		{
			var writer = context.Writer;
			writer->Add(value.Length);

			for (var i = 0; i < value.Length; i++)
				writer->Add(value[i]);
		}

		public unsafe NativeArray<Int32> Deserialize(in BinaryDeserializationContext<NativeArray<Int32>> context)
		{
			var reader = context.Reader;
			var length = reader->ReadNext<Int32>();

			var value = new NativeArray<Int32>(length, Allocator.Persistent);

			for (var i = 0; i < length; i++)
				value[i] = reader->ReadNext<Int32>();

			return value;
		}
	}*/
}
