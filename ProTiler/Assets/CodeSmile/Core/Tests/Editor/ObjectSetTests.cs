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
		private const string GameObjectNameOne = "1";
		private const string GameObjectNameTwo = "2";
		private const string GameObjectNameThree = "3";

		[Test] [EmptyScene]
		public void CreateEmptySet()
		{
			var set = new ObjectSet<GameObject>();

			Assert.AreEqual(null, set.DefaultObject);
			Assert.AreEqual(0, set.Count);
		}

		[Test] [EmptyScene] [CreateGameObject]
		public void CreateEmptySetWithDefaultObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var set = new ObjectSet<GameObject>(defaultGO);

			Assert.AreEqual(defaultGO, set.DefaultObject);
			Assert.AreEqual(0, set.Count);
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObjectToSetWithStartIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var startIndex = 3;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var actualIndex);

			Assert.AreEqual(1, set.Count);
			Assert.AreEqual(startIndex, actualIndex);
			Assert.AreEqual(GO1, set[actualIndex]);
		}

		[Test] [EmptyScene]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AddMultipleObjectsToSetWithStartIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var startIndex = 7;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var actualIndex1);
			set.Add(GO2, out var actualIndex2);
			set.Add(GO3, out var actualIndex3);

			Assert.AreEqual(3, set.Count);
			Assert.AreEqual(GO1, set[actualIndex1]);
			Assert.AreEqual(GO2, set[actualIndex2]);
			Assert.AreEqual(GO3, set[actualIndex3]);
		}

		[Test]
		public void AddNullObjectThrows()
		{
			var set = new ObjectSet<GameObject>();

			Assert.Throws<ArgumentNullException>(() => { set.Add(null); });
		}

		[Test]
		public void DoesNotContainNull()
		{
			var set = new ObjectSet<GameObject>();

			Assert.IsFalse(set.Contains(null));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);

			Assert.IsTrue(didAdd1);
			Assert.IsTrue(set.Contains(GO1));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddSameObjectTwice()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);
			var didAdd2 = set.Add(GO1);

			Assert.IsTrue(didAdd1);
			Assert.IsFalse(didAdd2);
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AddObjectsWithIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 11;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var index1);
			set.Add(GO2, out var index2);

			Assert.IsTrue(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));
			Assert.IsTrue(set.Contains(startIndex + 0));
			Assert.IsTrue(set.Contains(startIndex + 1));
			Assert.AreEqual(index1, startIndex + 0);
			Assert.AreEqual(index2, startIndex + 1);
		}

		[Test] [EmptyScene] [CreateGameObject]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AddMultipleObjects()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);
			var didAdd2 = set.Add(GO2);
			var didAdd3 = set.Add(GO3);

			Assert.IsTrue(didAdd1);
			Assert.IsTrue(didAdd2);
			Assert.IsTrue(didAdd3);
			Assert.IsTrue(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));
			Assert.IsTrue(set.Contains(GO3));
			Assert.IsTrue(set.Contains(0));
			Assert.IsTrue(set.Contains(1));
			Assert.IsTrue(set.Contains(2));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void RemoveNull()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(null);

			Assert.IsTrue(set.Count == 1);
			Assert.IsTrue(set.Contains(GO1));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void RemoveLastOne()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(GO1);

			Assert.IsTrue(set.Count == 0);
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void RemoveNotExisting()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(GO2);

			Assert.IsTrue(set.Count == 1);
			Assert.IsTrue(set.Contains(GO1));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void RemoveOne()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);
			set.Remove(GO2);

			Assert.IsTrue(set.Count == 2);
			Assert.IsFalse(set.Contains(GO2));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void RemoveOneByIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var startIndex = 1;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);
			set.RemoveAt(2);

			Assert.IsTrue(set.Count == 2);
			Assert.IsFalse(set.Contains(GO2));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void ClearSet()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);
			set.Clear();

			Assert.IsTrue(set.Count == 0);
			Assert.IsFalse(set.Contains(GO1));
		}

		[Test] [EmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void GetReturnsObjectAtIndex()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 7;
			var set = new ObjectSet<GameObject>(defaultGO, startIndex);

			set.Add(GO1);
			set.Add(GO2);

			Assert.AreEqual(GO1, set[startIndex + 0]);
			Assert.AreEqual(GO2, set[startIndex + 1]);
		}

		[Test] [EmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void GetOutOfBoundsReturnsDefaultObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var startIndex = 3;
			var set = new ObjectSet<GameObject>(defaultGO, startIndex);

			Assert.AreEqual(defaultGO, set[0]);
		}

		[Test] [EmptyScene]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AssignObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set[0] = GO1;
			set[1] = GO2;
			set[2] = GO3;

			Assert.IsTrue(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));
			Assert.IsTrue(set.Contains(GO3));
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AssignObjectReplaceExisting()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var set = new ObjectSet<GameObject>();

			set[0] = GO1;
			set[0] = GO2;

			Assert.IsFalse(set.Contains(GO1));
			Assert.IsTrue(set.Contains(GO2));
		}

		[Test]
		public void AssignObjectOutOfBoundsThrows()
		{
			var set = new ObjectSet<GameObject>();

			Assert.Throws<IndexOutOfRangeException>(() => { set[-1] = null; });
			Assert.Throws<IndexOutOfRangeException>(() => { set[1] = null; });
		}

		[Test] [EmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AssignObjectAlreadyInSetThrows()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);

			Assert.Throws<ArgumentException>(() => { set[0] = GO1; });
		}

		[Test] [EmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)]
		public void AssignNullRemovesObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>(defaultGO);

			set.Add(GO1);
			set[0] = null;

			Assert.AreEqual(defaultGO, set[0]);
			Assert.IsFalse(set.Contains(GO1));
			Assert.IsTrue(set.Count == 0);
		}
	}
}
