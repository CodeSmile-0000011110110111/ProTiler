// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class ObjectExt
	{
		/// <summary>
		///     Checks if a Unity Object instance is 'missing' where missing refers to the state where the
		///     underlying C++ instance has been cleaned up but the C# object still remains in memory.
		///     Technically it tests for ReferenceEquals(obj, null).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if this is a valid object, false if the object is in the 'missing' state</returns>
		public static bool IsMissing(this Object obj) => ReferenceEquals(obj, null);

		/// <summary>
		///     Destroys the object safely, regardless of mode.
		///     Depending on the mode (editor vs playmode/player) it calls either DestroyImmediate or Destroy.
		/// </summary>
		/// <param name="self"></param>
#if UNITY_EDITOR
		public static void DestroyInAnyMode(this Object self)
		{
			if (Application.isPlaying == false)
				Object.DestroyImmediate(self);
			else
				Object.Destroy(self);
		}
#else
		public static void DestroyInAnyMode(this Object self) => Object.Destroy(self);
#endif

		public static void RecordUndoInEditor(this Object obj, string undoActionName)
		{
#if UNITY_EDITOR
			if (EditorApplication.isPlaying == false)
				Undo.RecordObject(obj, undoActionName);
#endif
		}

		public static void SetDirtyInEditor(this Object obj)
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(obj);
#endif
		}

		public static T[] FindObjectsByTypeFast<T>(bool findInactive = false) where T : Object
		{
#if UNITY_2021_3_OR_NEWER
			return Object.FindObjectsByType<T>(
				findInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
			return Object.FindObjectsOfType<T>(findInactive);
#endif
		}

		public static T FindObjectByTypeFast<T>(bool findInactive = false) where T : Object
		{
			var objects = FindObjectsByTypeFast<T>();
			if (objects.Length > 0)
				return objects[0];

			return null;
		}
	}
}
