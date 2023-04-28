// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Editor.Utilities;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tests.Editor
{
	public class GameObjectExtTests
	{
		private const string ChildGameObjectName = "Child GameObject";
		private const string OriginalGameObjectName = "Original GameObject";

		[Test] [EmptyScene] [CreateGameObject]
		public void GetOrAddComponentOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			go.GetOrAddComponent<BoxCollider>();

			Assert.That(go.GetComponents<BoxCollider>().Length == 1, Is.True);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void GetOrAddSameComponentTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var collider = go.GetOrAddComponent<BoxCollider>();
			var collider2 = go.GetOrAddComponent<BoxCollider>();

			Assert.That(collider, Is.EqualTo(collider2));
			Assert.That(go.GetComponents<BoxCollider>().Length == 1, Is.True);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void GetOrAddSameComponentAfterDestroy()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			go.GetOrAddComponent<BoxCollider>().DestroyInAnyMode();
			go.GetOrAddComponent<BoxCollider>();

			Assert.That(go.GetComponents<BoxCollider>().Length == 1, Is.True);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void IsNotPrefab()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			Assert.That(go.IsPrefab(), Is.False);
		}

		[Test]
		public void IsPrefab()
		{
			var prefab = TestAssets.LoadTestPrefab();

			Assert.That(prefab.IsPrefab(), Is.True);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void FindOrCreateChildOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var child = go.FindOrCreateChild(ChildGameObjectName);

			Assert.That(go.transform.childCount == 1, Is.True);
			Assert.That(child, Is.EqualTo(go.transform.GetChild(0).gameObject));
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void FindOrCreateChildTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var child1 = go.FindOrCreateChild(ChildGameObjectName);
			var child2 = go.FindOrCreateChild(ChildGameObjectName);

			Assert.That(child1, Is.EqualTo(child2));
			Assert.That(go.transform.childCount == 1, Is.True);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void FindOrCreateChildWithNullOriginalThrows()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			Assert.Throws<ArgumentNullException>(() => { go.FindOrCreateChild(ChildGameObjectName, null); });
		}

		[Test] [EmptyScene] [CreateGameObject] [CreateGameObject(OriginalGameObjectName, typeof(BoxCollider))]
		public void FindOrCreateChildWithOriginalOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var original = GameObject.Find(OriginalGameObjectName);

			var child = go.FindOrCreateChild(ChildGameObjectName, original);

			Assert.That(go.transform.childCount == 1, Is.True);
			Assert.That(child, Is.EqualTo(go.transform.GetChild(0).gameObject));
			Assert.That(child.GetComponent<BoxCollider>() != null);
		}

		[Test] [EmptyScene] [CreateGameObject] [CreateGameObject(OriginalGameObjectName, typeof(BoxCollider))]
		public void FindOrCreateChildWithOriginalTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var original = GameObject.Find(OriginalGameObjectName);

			var child1 = go.FindOrCreateChild(ChildGameObjectName, original);
			var child2 = go.FindOrCreateChild(ChildGameObjectName, original);

			Assert.That(go.transform.childCount == 1, Is.True);
			Assert.That(child1, Is.EqualTo(child2));
			Assert.That(child1, Is.EqualTo(go.transform.GetChild(0).gameObject));
		}
	}
}
