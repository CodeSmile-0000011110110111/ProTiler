// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Tests.Utilities;
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

		[Test] [NewScene] [CreateGameObject]
		public void GetOrAddComponentOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			go.GetOrAddComponent<BoxCollider>();

			Assert.IsTrue(go.GetComponents<BoxCollider>().Length == 1);
		}

		[Test] [NewScene] [CreateGameObject]
		public void GetOrAddSameComponentTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var collider = go.GetOrAddComponent<BoxCollider>();
			var collider2 = go.GetOrAddComponent<BoxCollider>();

			Assert.AreEqual(collider, collider2);
			Assert.IsTrue(go.GetComponents<BoxCollider>().Length == 1);
		}

		[Test] [NewScene] [CreateGameObject]
		public void GetOrAddSameComponentAfterDestroy()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			go.GetOrAddComponent<BoxCollider>().DestroyInAnyMode();
			go.GetOrAddComponent<BoxCollider>();

			Assert.IsTrue(go.GetComponents<BoxCollider>().Length == 1);
		}

		[Test] [NewScene] [CreateGameObject]
		public void IsNotPrefab()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			Assert.IsFalse(go.IsPrefab());
		}

		[Test]
		public void IsPrefab()
		{
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Defines.TestAssetsPath + "TestPrefab.prefab");

			Assert.NotNull(prefab);
			Assert.IsTrue(prefab.IsPrefab());
		}

		[Test] [NewScene] [CreateGameObject]
		public void FindOrCreateChildOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var child = go.FindOrCreateChild(ChildGameObjectName);

			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);
		}

		[Test] [NewScene] [CreateGameObject]
		public void FindOrCreateChildTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var child1 = go.FindOrCreateChild(ChildGameObjectName);
			var child2 = go.FindOrCreateChild(ChildGameObjectName);

			Assert.NotNull(child1);
			Assert.AreEqual(child1, child2);
			Assert.IsTrue(go.transform.childCount == 1);
		}

		[Test] [NewScene] [CreateGameObject]
		public void FindOrCreateChildWithNullOriginalThrows()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			Assert.Throws<ArgumentNullException>(() => { go.FindOrCreateChild(ChildGameObjectName, null); });
		}

		[Test] [NewScene] [CreateGameObject] [CreateGameObject(OriginalGameObjectName, typeof(BoxCollider))]
		public void FindOrCreateChildWithOriginalOnce()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var original = GameObject.Find(OriginalGameObjectName);

			Assert.Throws<ArgumentNullException>(() => { go.FindOrCreateChild(ChildGameObjectName, null); });

			var child = go.FindOrCreateChild(ChildGameObjectName, original);

			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);
			Assert.NotNull(child.GetComponent<BoxCollider>());
		}

		[Test] [NewScene] [CreateGameObject] [CreateGameObject(OriginalGameObjectName, typeof(BoxCollider))]
		public void FindOrCreateChildWithOriginalTwice()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var original = GameObject.Find(OriginalGameObjectName);

			var child1 = go.FindOrCreateChild(ChildGameObjectName, original);
			var child2 = go.FindOrCreateChild(ChildGameObjectName, original);

			Assert.NotNull(child1);
			Assert.AreEqual(child1,child2);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child1, go.transform.GetChild(0).gameObject);
		}
	}
}
