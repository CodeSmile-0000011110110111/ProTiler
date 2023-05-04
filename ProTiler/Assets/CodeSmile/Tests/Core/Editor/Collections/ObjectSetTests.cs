// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using UnityEngine;

namespace CodeSmile.Tests.Core.Editor.Collections
{
	public class ObjectSetTests
	{
		private const string GameObjectNameOne = "1";
		private const string GameObjectNameTwo = "2";
		private const string GameObjectNameThree = "3";

		[Test] [CreateEmptyScene]
		public void CreateEmptySet()
		{
			var set = new ObjectSet<GameObject>();

			Assert.That(set.DefaultObject == null);
			Assert.That(set.Count == 0);
		}

		[Test] [CreateEmptyScene] [CreateGameObject]
		public void CreateEmptySetWithDefaultObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);

			var set = new ObjectSet<GameObject>(defaultGO);

			Assert.That(set.Count == 0);
			Assert.That(defaultGO, Is.EqualTo(set.DefaultObject));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObjectToSetWithStartIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var startIndex = 3;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var actualIndex);

			Assert.That(set.Count == 1);
			Assert.That(startIndex == actualIndex);
			Assert.That(GO1, Is.EqualTo(set[actualIndex]));
		}

		[Test] [CreateEmptyScene]
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

			Assert.That(set.Count == 3);
			Assert.That(GO1, Is.EqualTo(set[actualIndex1]));
			Assert.That(GO2, Is.EqualTo(set[actualIndex2]));
			Assert.That(GO3, Is.EqualTo(set[actualIndex3]));
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

			Assert.That(set.Contains(null), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);

			Assert.That(didAdd1, Is.True);
			Assert.That(set.Contains(GO1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddSameObjectTwice()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);
			var didAdd2 = set.Add(GO1);

			Assert.That(didAdd1, Is.True);
			Assert.That(didAdd2, Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AddObjectsWithIndex()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 11;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var index1);
			set.Add(GO2, out var index2);

			Assert.That(set.Contains(GO1), Is.True);
			Assert.That(set.Contains(GO2), Is.True);
			Assert.That(set.Contains(startIndex + 0), Is.True);
			Assert.That(set.Contains(startIndex + 1), Is.True);
			Assert.That(index1, Is.EqualTo(startIndex + 0));
			Assert.That(index2, Is.EqualTo(startIndex + 1));
		}

		[Test] [CreateEmptyScene] [CreateGameObject]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AddMultipleObjects()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);

			Assert.That(set.Contains(GO1), Is.True);
			Assert.That(set.Contains(GO2), Is.True);
			Assert.That(set.Contains(GO3), Is.True);
			Assert.That(set.Contains(0), Is.True);
			Assert.That(set.Contains(1), Is.True);
			Assert.That(set.Contains(2), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void RemoveNull()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(null);

			Assert.That(set.Count == 1, Is.True);
			Assert.That(set.Contains(GO1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void RemoveLastOne()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(GO1);

			Assert.That(set.Count == 0, Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void RemoveNotExisting()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(GO2);

			Assert.That(set.Count == 1, Is.True);
			Assert.That(set.Contains(GO1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
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

			Assert.That(set.Count == 2, Is.True);
			Assert.That(set.Contains(GO2), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
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

			Assert.That(set.Count == 2, Is.True);
			Assert.That(set.Contains(GO2), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
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

			Assert.That(set.Count == 0, Is.True);
			Assert.That(set.Contains(GO1), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void GetReturnsObjectAtIndex()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 7;
			var set = new ObjectSet<GameObject>(defaultGO, startIndex);

			set.Add(GO1);
			set.Add(GO2);

			Assert.That(GO1, Is.EqualTo(set[startIndex + 0]));
			Assert.That(GO2, Is.EqualTo(set[startIndex + 1]));
		}

		[Test] [CreateEmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void GetOutOfBoundsReturnsDefaultObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var startIndex = 3;
			var set = new ObjectSet<GameObject>(defaultGO, startIndex);

			Assert.That(defaultGO, Is.EqualTo(set[0]));
		}

		[Test] [CreateEmptyScene]
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

			Assert.That(set.Contains(GO1), Is.True);
			Assert.That(set.Contains(GO2), Is.True);
			Assert.That(set.Contains(GO3), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AssignObjectReplaceExisting()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var set = new ObjectSet<GameObject>();

			set[0] = GO1;
			set[0] = GO2;

			Assert.That(set.Contains(GO1), Is.False);
			Assert.That(set.Contains(GO2), Is.True);
		}

		[Test] public void AssignObjectOutOfBoundsThrows()
		{
			var set = new ObjectSet<GameObject>();

			Assert.Throws<IndexOutOfRangeException>(() => { set[-1] = null; });
			Assert.Throws<IndexOutOfRangeException>(() => { set[1] = null; });
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AssignObjectAlreadyInSetThrows()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);

			Assert.Throws<ArgumentException>(() => { set[0] = GO1; });
		}

		[Test] [CreateEmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)]
		public void AssignNullRemovesObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>(defaultGO);

			set.Add(GO1);
			set[0] = null;

			Assert.That(defaultGO, Is.EqualTo(set[0]));
			Assert.That(set.Contains(GO1), Is.False);
			Assert.That(set.Count == 0, Is.True);
		}
	}
}
