// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.Core.Extensions
{
	public class TransformExtTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(BoxCollider), typeof(BoxCollider))]
		public void DestroyAllChildren()
		{
			var go = ObjectExt.FindObjectByTypeFast<BoxCollider>().gameObject;
			var childCount = 10;
			for (var i = 0; i < childCount; i++)
				go.FindOrCreateChild(i.ToString());

			go.transform.DestroyAllChildren();

			Assert.That(0, Is.EqualTo(go.transform.childCount));
		}
	}
}
