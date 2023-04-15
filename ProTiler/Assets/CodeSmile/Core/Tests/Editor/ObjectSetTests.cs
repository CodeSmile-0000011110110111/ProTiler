// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using NUnit.Framework;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor.Tests
{
	public class ObjectSetTests
	{
		private GameObject m_DefaultGO;
		private GameObject m_GO1;
		private GameObject m_GO2;
		private GameObject m_GO3;

		private ObjectSet<GameObject> m_Set;
		private ObjectSet<GameObject> m_FilledSet;

		[SetUp]
		public void SetUp()
		{
			m_DefaultGO = new GameObject("DEFAULT");
			m_GO1 = new GameObject("one");
			m_GO2 = new GameObject("two");
			m_GO3 = new GameObject("three");
			m_Set = new ObjectSet<GameObject>(m_DefaultGO);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(m_DefaultGO);
			Object.DestroyImmediate(m_GO1);
			Object.DestroyImmediate(m_GO2);
			Object.DestroyImmediate(m_GO3);
		}

		[Test]
		public void CreateTests()
		{
			var set = new ObjectSet<GameObject>();
			Assert.AreEqual(null, set.DefaultObject);
			Assert.AreEqual(0, set.Count);

			set = new ObjectSet<GameObject>(m_DefaultGO);
			Assert.AreEqual(m_DefaultGO, set.DefaultObject);
			Assert.AreEqual(0, set.Count);
		}

		[Test]
		public void AddAndRemoveTests()
		{
			var set = m_Set;

			Assert.Throws<ArgumentNullException>(() => { set.Add(null); });

			var added = set.Add(m_GO1);
			Assert.AreEqual(true, added);
			Assert.AreEqual(1, set.Count);
			added = set.Add(m_GO1);
			Assert.IsFalse(added);
			Assert.AreEqual(1, set.Count);
			Assert.IsTrue(set.Contains(m_GO1));
			Assert.IsFalse(set.Contains(m_GO2));

			added = set.Add(m_GO2, out var index2);
			Assert.IsTrue(added);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, index2);
			added = set.Add(m_GO2, out index2);
			Assert.IsFalse(added);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(1, index2);
			Assert.IsTrue(set.Contains(m_GO1));
			Assert.IsTrue(set.Contains(m_GO2));

			var didRemove = set.Remove(m_GO1);
			Assert.IsTrue(didRemove);
			Assert.AreEqual(1, set.Count);
			Assert.IsFalse(set.Contains(m_GO1));
			Assert.IsTrue(set.Contains(m_GO2));

			didRemove = set.Remove(m_GO3);
			Assert.IsFalse(didRemove);
			set.Add(m_GO3, out var index3);
			Assert.AreEqual(2, set.Count);
			Assert.AreEqual(2, index3);

			set.RemoveAt(1);
			Assert.AreEqual(1, set.Count);
			Assert.IsFalse(set.RemoveAt(1));
			Assert.IsFalse(set.Contains(m_GO1));
			Assert.IsFalse(set.Contains(m_GO2));
			Assert.IsTrue(set.Contains(m_GO3));

			set.Clear();
			Assert.AreEqual(0, set.Count);
			Assert.IsFalse(set.Contains(m_GO3));

			Assert.IsFalse(set.Contains(null));
		}

		[Test]
		public void IndexerTests()
		{
			var set = m_Set;

			var obj = set[0];
			Assert.AreEqual(set.DefaultObject, obj);

			set.Add(m_GO1);
			set.Add(m_GO2);
			set.Add(m_GO3);
			Assert.AreEqual(m_GO1, set[0]);
			Assert.AreEqual(m_GO2, set[1]);
			Assert.AreEqual(m_GO3, set[2]);

			set.Remove(m_GO2);
			Assert.AreEqual(set.DefaultObject, set[1]);
			set.Add(m_GO2);
			Assert.AreEqual(m_GO2, set[3]);
		}

		[Test]
		public void ReplaceTests()
		{
			var set = m_Set;
			set.Add(m_GO1);
			set.Add(m_GO2);

			Assert.Throws<IndexOutOfRangeException>(() => { set.ReplaceAt(-1, m_GO1); });
			Assert.Throws<IndexOutOfRangeException>(() => { set.ReplaceAt(2, m_GO1); });
			Assert.Throws<ArgumentException>(() => { set.ReplaceAt(1, m_GO1); });

			set[0] = m_GO3;
			Assert.AreEqual(m_GO3, set[0]);

			set[0] = null;
			Assert.AreEqual(set.DefaultObject, set[0]);

			set.ReplaceAt(0, m_GO1);
			Assert.AreEqual(m_GO1, set[0]);
		}
	}
}
