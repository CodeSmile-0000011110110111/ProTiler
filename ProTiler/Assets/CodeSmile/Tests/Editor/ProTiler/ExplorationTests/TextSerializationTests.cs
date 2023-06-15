// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Serialization.Binary;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public class TextSerializationTests
	{
		public static List<IBinaryAdapter> TextSerializationAdapters = new()
		{
			//new ToStringAdapter<Int32>(),
			//new ToStringAdapter<TestStruct>(),
			new TestStruct.TextSerializationAdapter(),
		};

		[Test]
		public void TestStruct_Serializing_DeserializesCorrectly()
		{
			var testStruct = new TestStruct
			{
				intValue = 123,
				//floatValue = 1.234f, stringValue = "text"
			};

			var bytes = Serialize.ToBinary(testStruct, TextSerializationAdapters);
			Debug.Log($"Bytes ({bytes.Length}):\n{bytes.AsString()}");
			Debug.Log($"Hex:\n{BitConverter.ToString(bytes)}");
			Debug.Log($"Text:\n{Encoding.ASCII.GetString(bytes)}");
			var deserializedStruct = Serialize.FromBinary<TestStruct>(bytes, TextSerializationAdapters);

			Assert.That(deserializedStruct.intValue, Is.EqualTo(testStruct.intValue));
		}

		public struct TestStruct
		{
			public Int32 intValue;
			//public Single floatValue;
			//public String stringValue;

			public class TextSerializationAdapter : IBinaryAdapter<TestStruct>
			{
				public unsafe void Serialize(in BinarySerializationContext<TestStruct> context, TestStruct value)
				{
					var sb = new StringBuilder(nameof(TestStruct));
					sb.AppendLine("{");
					sb.AppendLine($"{nameof(value.intValue)}={value.intValue}");
					sb.AppendLine("}");

					var bytes = Encoding.ASCII.GetBytes(sb.ToString());
					fixed (Byte* bytePtr = bytes)
						context.Writer->Add(bytePtr, sb.Length);
				}

				public unsafe TestStruct Deserialize(in BinaryDeserializationContext<TestStruct> context)
				{
					Byte character;
					{
						var bytes = new List<Byte>();
						while ((character = context.Reader->ReadNext<Byte>()) != '{')
						{
							bytes.Add(character);
						}
						Debug.Log($"Reading Type: {Encoding.ASCII.GetString(bytes.ToArray())}");

						// skip newline
						while (context.Reader->ReadNext<Byte>() != '\n') {}
					}
					{
						var bytes = new List<Byte>();
						while ((character = context.Reader->ReadNext<Byte>()) != '=')
						{
							bytes.Add(character);
						}
						Debug.Log($"Reading Field: {Encoding.ASCII.GetString(bytes.ToArray())}");
					}
					{
						var bytes = new List<Byte>();
						while ((character = context.Reader->ReadNext<Byte>()) != '\n')
						{
							bytes.Add(character);
						}
						var valueStr = Encoding.ASCII.GetString(bytes.ToArray());
						Debug.Log($"Reading Value: {valueStr}");

						var value = new TestStruct();
						value.intValue = Convert.ToInt32(valueStr);

						return value;
					}
				}
			}
		}

		public class ToStringAdapter<T> : IBinaryAdapter<T> where T : unmanaged
		{
			public unsafe void Serialize(in BinarySerializationContext<T> context, T value)
			{
				var str = value.ToString();
				var bytes = Encoding.ASCII.GetBytes(str);
				fixed (Byte* bytePtr = bytes)
					context.Writer->Add(bytePtr, str.Length);
				context.Writer->Add('\n');
			}

			public unsafe T Deserialize(in BinaryDeserializationContext<T> context)
			{
				var bytes = new List<Byte>();

				Byte character;
				while ((character = context.Reader->ReadNext<Byte>()) != '\n')
				{
					bytes.Add(character);
				}

				var genericType = typeof(T);
				switch (genericType)
				{
					case Type _ when genericType == typeof(Int32):
						return (T)(Object)Convert.ToInt32(bytes);
					case Type _ when genericType == typeof(Int64):
						return (T)(Object)Convert.ToInt64(bytes);

					default:
						return new T();
				}
			}
		}
	}
}
