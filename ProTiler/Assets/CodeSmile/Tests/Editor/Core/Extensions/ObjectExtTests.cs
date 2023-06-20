// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.Extensions
{
	public class ObjectExtTests
	{
		[Test] [CreateEmptyScene]
		public void FindNonExistingTypeReturnsNull()
		{
			var obj = ObjectExt.FindObjectByTypeFast<BoxCollider>();

			Assert.That(obj, Is.Null);
		}

		[Test] [CreateDefaultScene]
		public void FindExistingTypeReturnsObject()
		{
			var camera = ObjectExt.FindObjectByTypeFast<Camera>();

			Assert.That(camera, Is.EqualTo(Camera.main));
		}
	}
}
