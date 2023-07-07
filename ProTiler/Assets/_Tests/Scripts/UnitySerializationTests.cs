// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

#pragma warning disable 0414

#if UNITY_2022_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
#endif

public class UnitySerializationTests : UnityEngine.MonoBehaviour
{
#if UNITY_2022_3_OR_NEWER
	public enum SerializationFormat
	{
		Json,
		Binary,
	}

	public enum BinaryCopyStrategy
	{
		ByteByByte,
		MarshalCopy,
		UnsafeMemCopy,
	}

	public bool m_New;
	public bool m_Save;
	public bool m_Load;
	public bool m_SaveAndLoad;

	public SerializationFormat m_Format;
	public BinaryCopyStrategy m_BinaryCopyStrategy;
	public string m_Json = "{}";
	public byte[] m_Binary;

	//public Tilemap3D m_Tilemap;
	public Data m_Data;
	public UnsafeAppendBuffer m_Stream;

	private static string GetPathFromScene(string extension) =>
		Path.ChangeExtension(SceneManager.GetActiveScene().path, extension);

	private void OnValidate()
	{
		if (m_New)
			CreateNewData();

		if (m_SaveAndLoad)
		{
			m_SaveAndLoad = false;
			m_Save = true;
			m_Load = true;
		}

		if (m_Save)
			Save();

		if (m_Load)
			Load();
	}

	private void CreateNewData()
	{
		m_New = false;
		m_Data = new Data();
		InitSerializationOutputFields();
	}

	private void Save()
	{
		m_Save = false;

		//JsonSerialization.AddGlobalAdapter(new SerializedDataAdapter());
		InitSerializationOutputFields();

		switch (m_Format)
		{
			case SerializationFormat.Json:
				LogTimeForAction("Json serialization time", () =>
				{
					var path = GetPathFromScene(".json");
					var fileInfo = new FileInfo(path);
					JsonSerialization.ToJson(fileInfo, m_Data, new JsonSerializationParameters
					{
						Minified = false,
						Simplified = false,
					});
				});
				break;
			case SerializationFormat.Binary:
				LogTimeForAction("Binary serialization time", () =>
				{
					unsafe
					{
						var stream = new UnsafeAppendBuffer(256, 8, Allocator.Persistent);
						BinarySerialization.ToBinary(&stream, m_Data);
						m_Stream = stream;
						StreamToBinary();
						WriteBinaryFile();
					}
				});
				break;
		}

		m_Data = new Data();

#if UNITY_EDITOR
		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
#endif
	}

	private void WriteJsonFile()
	{
		var path = GetPathFromScene(".json");
		//Debug.Log($"Write json to: {path}");
		File.WriteAllText(path, m_Json);
	}

	private void WriteBinaryFile()
	{
		var path = GetPathFromScene(".bin");
		//Debug.Log($"Write binary to: {path}");
		File.WriteAllBytes(path, m_Binary);
	}

	private void StreamToBinary()
	{
		if (m_BinaryCopyStrategy == BinaryCopyStrategy.ByteByByte)
		{
			var list = new List<byte>();
			var reader = m_Stream.AsReader();
			while (reader.EndOfBuffer == false)
			{
				list.Add(reader.ReadNext<byte>());
			}

			m_Binary = list.ToArray();
		}
		else
		{
			unsafe
			{
				var reader = m_Stream.AsReader();
				if (m_Binary.Length != reader.Size)
					m_Binary = new byte[reader.Size];

				if (m_BinaryCopyStrategy == BinaryCopyStrategy.MarshalCopy)
					Marshal.Copy((IntPtr)reader.Ptr, m_Binary, 0, reader.Size);
				else
					throw new NotImplementedException();
				//UnsafeUtility.MemCpy(&m_Binary, reader.Ptr, reader.Size);
			}
		}
	}

	private void Load()
	{
		m_Load = false;

		switch (m_Format)
		{
			case SerializationFormat.Json:
				LogTimeForAction("Json DE-serialization time", () =>
				{
					m_Data = JsonSerialization.FromJson<Data>(m_Json);
				});
				break;
			case SerializationFormat.Binary:
				LogTimeForAction("Binary DE-serialization time", () =>
				{
					unsafe
					{
						var reader = m_Stream.AsReader();
						m_Data = BinarySerialization.FromBinary<Data>(&reader);
					}
				});
				break;
		}
	}

	private void LogTimeForAction(string message, Action action)
	{
		var sw = new Stopwatch();
		sw.Start();

		action.Invoke();

		sw.Stop();
		Debug.Log($"{message}: {sw.ElapsedMilliseconds} ms");
	}

	private void InitSerializationOutputFields()
	{
		m_Json = "{}";
		m_Binary = new byte[0];
	}

	[Serializable]
	public class Data
	{
		[SerializeField] [CreateProperty] private int IntValue = -1234;
		[SerializeField] [CreateProperty] private float FloatValue = -1.23456f;

		[SerializeField] [CreateProperty] private double DoubleValue = -10.0123456789;
		[SerializeField] [CreateProperty] private bool BoolValue = true;

		[SerializeField] [CreateProperty] private int[] IntArray = { 1, 23, 456, 7890 };
		[SerializeField] [CreateProperty] private List<int> IntList = new() { -1, -23, -456, -7890 };
		[SerializeReference] [CreateProperty] private Dictionary<string, int> StringIntDict = new()
		{
			{ "first", 111 },
			{ "second", 222 },
			{ "third", 333 },
			{ "fourth", 444 },
		};

		public override string ToString() => $"dict: {StringIntDict}";
	}
#endif
}

/*
internal class SerializedDataAdapter : IJsonAdapter<Serialize.Data>
{
	void IJsonAdapter<Serialize.Data>.Serialize(in JsonSerializationContext<Serialize.Data> context, Serialize.Data value)
	{
		var val = -1234;
		context.Writer.WriteValue(val);
		Debug.Log($"saved: {val} - context: {context}");
	}

	Serialize.Data IJsonAdapter<Serialize.Data>.Deserialize(in JsonDeserializationContext<Serialize.Data> context)
	{
		var value = context.SerializedValue.AsInt32();
		Debug.Log($"loaded: {value} - context: {context}");
		return new Serialize.Data();
	}
}
*/
