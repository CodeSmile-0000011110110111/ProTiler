// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class LinearDataMapSerializationTests
	{

		[Test]
		public void LinearDataMapSerializationTestsSimplePasses()
		{
			var map = new LinearDataMap<SerializationTestData>();

			Assert.NotNull(map);
		}


	}
}
