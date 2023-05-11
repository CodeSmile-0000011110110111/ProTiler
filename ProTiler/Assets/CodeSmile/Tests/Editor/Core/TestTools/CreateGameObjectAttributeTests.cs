// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.Core.TestTools
{
	public class CreateGameObjectAttributeTests
	{
		private const string TestObjectName = "Test Object";

		[Test] [CreateEmptyScene] [CreateGameObject(TestObjectName)]
		public void CreateGameObjectCanBeFoundByName() => Assert.That(GameObject.Find(TestObjectName) != null);
	}
}
