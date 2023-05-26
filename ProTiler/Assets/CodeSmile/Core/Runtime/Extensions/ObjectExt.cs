// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Extensions
{
	[FullCovered]
	public static class ObjectExt
	{
		/// <summary>
		///     Checks if a Unity Object instance is 'missing' where missing refers to the state where the
		///     underlying C++ instance has been cleaned up but the C# object still remains in memory.
		///     Technically it tests for ReferenceEquals(obj, null).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if this is a valid object, false if the object is in the 'missing' state</returns>
		public static Boolean IsMissing(this Object obj) => ReferenceEquals(obj, null);

		/// <summary>
		///     Destroys the object safely, regardless of Edit or Play mode.
		///     Depending on the mode it calls either DestroyImmediate or Destroy.
		///     If used on a Transform object, it will transparently use Transform's gameObject
		///     since it is assumed that the intention is to destroy the GameObject,
		///     not the Transform (which isn't allowed).
		/// </summary>
		/// <param name="self"></param>
#if UNITY_EDITOR
		public static void DestroyInAnyMode(this Object self)
		{
			if (self is Transform t)
				self = t.gameObject;

			if (Application.isPlaying == false)
				self.EditorDestroy();
			else
				self.RuntimeDestroy();
		}
#else
		public static void DestroyInAnyMode(this Object self) => self.RuntimeDestroy();
#endif

		public static T[] FindObjectsByTypeFast<T>(Boolean findInactive = false) where T : Object
		{
#if UNITY_2022_2_OR_NEWER
			return Object.FindObjectsByType<T>(
				findInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
			return Object.FindObjectsOfType<T>(findInactive);
#endif
		}

		public static T FindObjectByTypeFast<T>(Boolean findInactive = false) where T : Object
		{
			var objects = FindObjectsByTypeFast<T>(findInactive);
			if (objects.Length > 0)
				return objects[0];

			return null;
		}

		private static void EditorDestroy(this Object self) => Object.DestroyImmediate(self);

		private static void RuntimeDestroy(this Object self) => Object.Destroy(self);
	}
}
