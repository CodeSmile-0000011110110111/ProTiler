// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using System;
using UnityEngine;

namespace CodeSmile.Editor.Tests
{
	public class ObjectSetTests
	{
		[Test]
		[NewEmptyScene]
		[CreateGameObject("default")]
		[CreateGameObject("1")]
		public void CreateSet()
		{
			var defaultGO = GameObject.Find("default");
			var GO1 = GameObject.Find("1");
			Assert.NotNull(defaultGO);
			Assert.NotNull(GO1);

			var set = new ObjectSet<GameObject>();
			Assert.AreEqual(null, set.DefaultObject);
			Assert.AreEqual(0, set.Count);

			set = new ObjectSet<GameObject>(defaultGO);
			Assert.AreEqual(defaultGO, set.DefaultObject);
			Assert.AreEqual(0, set.Count);

			set = new ObjectSet<GameObject>(null, 1);
			set.Add(GO1, out var index);
			Assert.AreEqual(1, index);
			Assert.AreEqual(null, set.DefaultObject);
			Assert.AreEqual(1, set.Count);
		}

		[Test]
		[NewEmptyScene]
		[CreateGameObject("default")]
		[CreateGameObject("1")]
		[CreateGameObject("2")]
		[CreateGameObject("3")]
		public void AddAndRemove()
		{
			var defaultGO = GameObject.Find("default");
			var GO1 = GameObject.Find("1");
			var GO2 = GameObject.Find("2");
			var GO3 = GameObject.Find("3");
			Assert.NotNull(defaultGO);
			Assert.NotNull(GO1);
			Assert.NotNull(GO2);
			Assert.NotNull(GO3);

			var set = new ObjectSet<GameObject>();
			Assert.Throws<ArgumentNullException>(() => { set.Add(null); });

			var added = set.Add(GO1);
			Assert.AreEqual(true, added);
			Assert.AreEqual(1, set.Count);
			added = set.Add(GO1);
			Assert.IsFalse(added);
			Assert.AreEqual(1, set.Count);
			Assert.IsTrue(set.Contains(GO1));
			Assert.IsFalse(set.Contains(GO2));

			added = set.Add(GO2, out var index2);
			Assert.IsTrue(added);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, index2);
			added = set.Add(GO2, out index2);
			Assert.IsFalse(added);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, index2);
			Assert.IsTrue(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));

			var didRemove = set.Remove(GO1);
			Assert.IsTrue(didRemove);
			Assert.AreEqual(1, set.Count);
			Assert.IsFalse(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));

			didRemove = set.Remove(GO3);
			Assert.IsFalse(didRemove);
			set.Add(GO3, out var index3);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(2, index3);

			set.RemoveAt(1);
			Assert.AreEqual(1, set.Count);
			Assert.IsFalse(set.RemoveAt(1));
			Assert.IsFalse(set.Contains(GO1));
			Assert.IsFalse(set.Contains(GO2));
			Assert.IsTrue(set.Contains(GO3));

			Assert.IsFalse(set.Contains(0));
			Assert.IsFalse(set.Contains(1));
			Assert.IsTrue(set.Contains(2));

			set.Clear();
			Assert.AreEqual(0, set.Count);
			Assert.IsFalse(set.Contains(GO3));
			Assert.IsFalse(set.Contains(2));

			Assert.IsFalse(set.Contains(null));
		}

		[Test]
		[NewEmptyScene]
		[CreateGameObject("default")]
		[CreateGameObject("1")]
		[CreateGameObject("2")]
		[CreateGameObject("3")]
		public void Indexer()
		{
			var defaultGO = GameObject.Find("default");
			var GO1 = GameObject.Find("1");
			var GO2 = GameObject.Find("2");
			var GO3 = GameObject.Find("3");
			Assert.NotNull(defaultGO);
			Assert.NotNull(GO1);
			Assert.NotNull(GO2);
			Assert.NotNull(GO3);

			var set = new ObjectSet<GameObject>();
			var obj = set[0];
			Assert.AreEqual(set.DefaultObject, obj);

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);
			Assert.AreEqual(GO1, set[0]);
			Assert.AreEqual(GO2, set[1]);
			Assert.AreEqual(GO3, set[2]);

			set.Remove(GO2);
			Assert.AreEqual(set.DefaultObject, set[1]);
			set.Add(GO2);
			Assert.AreEqual(GO2, set[3]);
		}

		[Test]
		[NewEmptyScene]
		[CreateGameObject("default")]
		[CreateGameObject("1")]
		[CreateGameObject("2")]
		[CreateGameObject("3")]
		public void ReplaceObject()
		{
			var defaultGO = GameObject.Find("default");
			var GO1 = GameObject.Find("1");
			var GO2 = GameObject.Find("2");
			var GO3 = GameObject.Find("3");
			Assert.NotNull(defaultGO);
			Assert.NotNull(GO1);
			Assert.NotNull(GO2);
			Assert.NotNull(GO3);

			var set = new ObjectSet<GameObject>();
			set.Add(GO1);
			set.Add(GO2);

			Assert.Throws<IndexOutOfRangeException>(() => { set.ReplaceAt(-1, GO1); });
			Assert.Throws<IndexOutOfRangeException>(() => { set.ReplaceAt(2, GO1); });
			Assert.Throws<ArgumentException>(() => { set.ReplaceAt(1, GO1); });

			set[0] = GO3;
			Assert.AreEqual(GO3, set[0]);

			set[0] = null;
			Assert.AreEqual(set.DefaultObject, set[0]);

			set.ReplaceAt(0, GO1);
			Assert.AreEqual(GO1, set[0]);
		}
	}
}
