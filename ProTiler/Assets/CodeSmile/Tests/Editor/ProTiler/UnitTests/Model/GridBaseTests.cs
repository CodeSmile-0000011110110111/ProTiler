// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class GridBaseTests
	{

		[Test]
		public void TestGrid_WhenInstantiated_IsNotNull()
		{
			var grid = new TestGrid();

			Assert.That(grid != null);
		}

		public struct MyLinearTestData
		{
			public byte byteValue;
		}
		public struct MySparseTestData
		{
			public byte byteValue;
		}

		public class TestGrid : GridBase
		{
			public TestGrid() : base(1)
			{
				AddLinearDataMap<MyLinearTestData>(2);
				// no streaming
				AddSparseDataMap<MySparseTestData>(3);
			}
		}

	}
}
