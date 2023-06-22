// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class GridBaseTests
	{

		[Test]
		public void TestGrid_WhenInstantiated_IsNotNull()
		{
			var grid = new GridBaseTestImpl();

			Assert.That(grid != null);
		}

		public struct MyLinearTestData : IBinarySerializable
		{
			public byte byteValue;
			public unsafe void Serialize(UnsafeAppendBuffer* writer)
			{
				throw new NotImplementedException();
			}

			public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion)
			{
				throw new NotImplementedException();
			}
		}
		public struct MySparseTestData: IBinarySerializable
		{
			public byte byteValue;
			public unsafe void Serialize(UnsafeAppendBuffer* writer)
			{
				throw new NotImplementedException();
			}

			public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion)
			{
				throw new NotImplementedException();
			}
		}

		public class GridBaseTestImpl : GridBase
		{
			public GridBaseTestImpl() : base(1)
			{
				AddLinearDataMap<MyLinearTestData>(2);
				// no streaming
				AddSparseDataMap<MySparseTestData>(3);
			}
		}

	}
}
