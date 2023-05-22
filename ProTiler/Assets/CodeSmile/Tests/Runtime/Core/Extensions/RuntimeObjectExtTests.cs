// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Runtime.Core.Extensions
{
	public class RuntimeObjectExtTests
	{
		private const string ShouldGetDestroyedName = "ShouldGetDestroyed";

		[Test]
		public void DestroyInAnyModeDoesNotDestroyImmediately()
		{
			var name = "ShouldGetDestroyed";
			var go = new GameObject(name);

			go.DestroyInAnyMode();

			Assert.That(GameObject.Find(name) != null);
		}

		[UnityTest] [CreateGameObject(ShouldGetDestroyedName)]
		public IEnumerator DestroyInAnyModeRemovesObjectAfterYield()
		{
			var go = GameObject.Find(ShouldGetDestroyedName);

			go.DestroyInAnyMode();
			yield return null;

			Assert.That(GameObject.Find(ShouldGetDestroyedName), Is.Null);
		}

		[UnityTest] [CreateGameObject(ShouldGetDestroyedName, typeof(BoxCollider))]
		public IEnumerator DestroyInAnyModeDestroysComponentAfterYield()
		{
			var go = GameObject.Find(ShouldGetDestroyedName);
			Assert.That(go.GetComponent<BoxCollider>() != null);

			go.GetComponent<BoxCollider>().DestroyInAnyMode();
			yield return null;

			Assert.That(go.GetComponent<BoxCollider>(), Is.Null);
		}
	}
}
