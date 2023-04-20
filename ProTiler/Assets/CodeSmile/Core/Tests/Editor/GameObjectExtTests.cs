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
		[Test]
		[NewScene]
		[CreateGameObject("test")]
		public void GetOrAddComponent()
		{
			var go = GameObject.Find("test");
			Assert.NotNull(go);
			Assert.IsNull(go.GetComponent<BoxCollider>());

			var collider = go.GetOrAddComponent<BoxCollider>();
			Assert.NotNull(go.GetComponent<BoxCollider>());
			Assert.IsTrue(go.GetComponents<BoxCollider>().Length == 1);

			var collider2 = go.GetOrAddComponent<BoxCollider>();
			Assert.AreEqual(collider, collider2);
			Assert.NotNull(go.GetComponent<BoxCollider>());
			Assert.IsTrue(go.GetComponents<BoxCollider>().Length == 1);

			collider.DestroyInAnyMode();
			Assert.IsNull(go.GetComponent<BoxCollider>());
			collider = go.GetOrAddComponent<BoxCollider>();
			Assert.NotNull(go.GetComponent<BoxCollider>());
		}

		[Test]
		[NewScene]
		[CreateGameObject("test")]
		public void IsPrefab()
		{
			var go = GameObject.Find("test");
			Assert.NotNull(go);
			Assert.IsFalse(go.IsPrefab());

			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Defines.TestAssetsPath + "TestPrefab.prefab");
			Assert.NotNull(prefab);
			Assert.IsTrue(prefab.IsPrefab());
		}

		[Test]
		[NewScene]
		[CreateGameObject("parent")]
		public void FindOrCreateChild()
		{
			var go = GameObject.Find("parent");
			Assert.NotNull(go);

			var child = go.FindOrCreateChild("child");
			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);

			child = go.FindOrCreateChild("child");
			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);
		}

		[Test]
		[NewScene]
		[CreateGameObject("parent")]
		[CreateGameObject("original", typeof(SphereCollider))]
		public void FindOrCreateChildWithOriginal()
		{
			var go = GameObject.Find("parent");
			Assert.NotNull(go);
			var original = GameObject.Find("original");
			Assert.NotNull(original);

			Assert.Throws<ArgumentNullException>(() => { go.FindOrCreateChild("child", null); });

			var child = go.FindOrCreateChild("child", original);
			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);
			Assert.NotNull(child.GetComponent<SphereCollider>());

			child = go.FindOrCreateChild("child", original);
			Assert.NotNull(child);
			Assert.IsTrue(go.transform.childCount == 1);
			Assert.AreEqual(child, go.transform.GetChild(0).gameObject);
		}
	}
}
