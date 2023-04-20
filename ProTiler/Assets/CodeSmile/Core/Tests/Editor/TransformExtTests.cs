// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor
{
	public class TransformExtTests
	{
		[Test]
		[NewScene]
		[CreateGameObject("go", typeof(BoxCollider))]
		public void DestroyAllChildren()
		{
			var go = Object.FindObjectOfType<BoxCollider>().gameObject;

			var childCount = 100;
			for (var i = 0; i < childCount; i++)
				go.FindOrCreateChild(i.ToString());

			Assert.AreEqual(childCount, go.transform.childCount);

			go.transform.DestroyAllChildren();
			Assert.AreEqual(0, go.transform.childCount);
		}
	}
}
