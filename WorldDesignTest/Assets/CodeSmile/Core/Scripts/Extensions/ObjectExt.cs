// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodeSmile
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
		public static void DestroyInAnyMode(this Object self)
		{
#if UNITY_EDITOR
			if (Application.isEditor && Application.isPlaying == false)
				Object.DestroyImmediate(self);
			else
#endif
				Object.Destroy(self);
		}

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
	}
}