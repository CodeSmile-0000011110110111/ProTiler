// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using UnityEngine;

namespace CodeSmile.Tests.Editor.Collections
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

			Assert.That(set.DefaultObject, Is.EqualTo(defaultGO));
			Assert.That(set.Count == 0);
		}


		[Test] [CreateEmptyScene] [CreateGameObject]
		public void FirstObjectAddedReturnsCountOne()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var set = new ObjectSet<GameObject>();

			set.Add(go);

			Assert.That(set.Count , Is.EqualTo(1));
		}

		[Test] [CreateEmptyScene] [CreateGameObject]
		public void FirstObjectAddedIsFoundAtCustomStartIndex()
		{
			var go = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var startIndex = 123;

			var set = new ObjectSet<GameObject>(null, startIndex);
			set.Add(go);

			Assert.That(set[startIndex], Is.EqualTo(go));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void FirstAddObjectReturnsStartIndex()
		{
			var go = GameObject.Find(GameObjectNameOne);
			var startIndex = 13;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(go, out var actualIndex);

			Assert.That(startIndex == actualIndex);
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AddMultipleObjectsHasCorrectCount()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);

			Assert.That(set.Count == 3);
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AddMultipleObjectsHasObjectsAtCorrectIndexes()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var startIndex = 17;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1, out var actualIndex1);
			set.Add(GO2, out var actualIndex2);
			set.Add(GO3, out var actualIndex3);

			Assert.That(set[actualIndex1], Is.EqualTo(GO1));
			Assert.That(set[actualIndex2], Is.EqualTo(GO2));
			Assert.That(set[actualIndex3], Is.EqualTo(GO3));
		}

		[Test]
		public void TryAddNullObjectThrows()
		{
			var set = new ObjectSet<GameObject>();

			Assert.Throws<ArgumentNullException>(() => { set.Add(null); });
		}

		[Test]
		public void EmptySetDoesNotContainNull()
		{
			var set = new ObjectSet<GameObject>();

			Assert.That(set.Contains(null), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObjectReturnsTrue()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);

			Assert.That(didAdd1, Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddObjectContainsThatObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);

			Assert.That(set.Contains(GO1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void AddSameObjectTwiceReturnsFalseForSecondAdd()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			var didAdd1 = set.Add(GO1);
			var didAdd2 = set.Add(GO1);

			Assert.That(didAdd1, Is.True);
			Assert.That(didAdd2, Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AddObjectsWithStartIndexContainsObjects()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 11;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1);
			set.Add(GO2);

			Assert.That(set.Contains(GO1), Is.True);
			Assert.That(set.Contains(GO2), Is.True);
		}
		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void AddObjectsWithStartIndexContainsIndexes()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var startIndex = 13;
			var set = new ObjectSet<GameObject>(null, startIndex);

			set.Add(GO1);
			set.Add(GO2);

			Assert.That(set.Contains(startIndex + 0), Is.True);
			Assert.That(set.Contains(startIndex + 1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void TryRemoveNullDoesNotRemoveAnExistingObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(null);

			Assert.That(set.Count == 1, Is.True);
			Assert.That(set.Contains(GO1), Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)]
		public void RemoveLastObjectHasCountZero()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Remove(GO1);

			Assert.That(set.Count == 0, Is.True);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void TryRemoveNotExistingObjectDoesNothing()
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
		public void RemoveAnInbetweenObjectHasCorrectCount()
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
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void RemoveAnInbetweenObjectDoesNotContainThatObjectAnymore()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var GO3 = GameObject.Find(GameObjectNameThree);
			var set = new ObjectSet<GameObject>();

			set.Add(GO1);
			set.Add(GO2);
			set.Add(GO3);
			set.Remove(GO2);

			Assert.That(set.Contains(GO2), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void RemoveOneObjectByIndexHasCorrectCount()
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
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void RemoveOneObjectByIndexDoesNotContainThatObject()
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

			Assert.That(set.Contains(GO2), Is.False);
		}

		[Test] [CreateEmptyScene] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		[CreateGameObject(GameObjectNameThree)]
		public void ClearSetWithObjectsHasCountZero()
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

			Assert.That(set[startIndex + 0], Is.EqualTo(GO1));
			Assert.That(set[startIndex + 1], Is.EqualTo(GO2));
		}

		[TestCase(int.MinValue)][TestCase(int.MaxValue)]
		[CreateEmptyScene] [CreateGameObject] [CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)]
		public void TryGetOutOfBoundsIndexReturnsDefaultObject(int indexOutOfBounds)
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var set = new ObjectSet<GameObject>(defaultGO);

			Assert.That(set[indexOutOfBounds], Is.EqualTo(defaultGO));
		}

		[Test] [CreateEmptyScene]
		[CreateGameObject(GameObjectNameOne)] [CreateGameObject(GameObjectNameTwo)] [CreateGameObject(GameObjectNameThree)]
		public void AssignObjectsWithIndexersContainsObjects()
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
		public void AssignObjectToExistingIndexReplacesExistingObject()
		{
			var GO1 = GameObject.Find(GameObjectNameOne);
			var GO2 = GameObject.Find(GameObjectNameTwo);
			var set = new ObjectSet<GameObject>();

			set[0] = GO1;
			set[0] = GO2;

			Assert.That(set.Contains(GO1), Is.False);
			Assert.That(set.Contains(GO2), Is.True);
		}

		[TestCase(int.MinValue)][TestCase(int.MaxValue)]
		public void AssignObjectOutOfBoundsThrows(int indexOutOfBounds)
		{
			var set = new ObjectSet<GameObject>();

			Assert.Throws<ArgumentException>(() => { set[indexOutOfBounds] = null; });
			Assert.Throws<ArgumentException>(() => { set[indexOutOfBounds] = null; });
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
		public void AssignNullToIndexerRemovesExistingObject()
		{
			var defaultGO = GameObject.Find(CreateGameObjectAttribute.DefaultName);
			var GO1 = GameObject.Find(GameObjectNameOne);
			var set = new ObjectSet<GameObject>(defaultGO);

			set.Add(GO1);
			set[0] = null;

			Assert.That(set.Count == 0, Is.True);
			Assert.That(set.Contains(GO1), Is.False);
		}
	}
}
