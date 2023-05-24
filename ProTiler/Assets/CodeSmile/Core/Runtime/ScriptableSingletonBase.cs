// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile
{
	/// <summary>
	///     ScriptableSingleton without persistence ie it is not an asset and not stored to disk.
	///     Otherwise it is much like ScriptableSingleton<> but also works at runtime.
	///     See also:
	///     https://forum.unity.com/threads/scriptable-object-singleton-pattern-that-doesnt-use-resources.1195315/#post-9028357
	/// </summary>
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public abstract class ScriptableSingletonBase<T> : ScriptableObject where T : ScriptableSingletonBase<T>
	{
		[SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="unavoidable")]
		private static T s_Instance;
		public static T Singleton => GetOrCreateInstance();
		protected static Boolean IsCreated => s_Instance != null;
		private static T GetOrCreateInstance() => IsCreated ? s_Instance : CreateSingletonInstance();

		private static T CreateSingletonInstance()
		{
			s_Instance = CreateInstance<T>();
			s_Instance.OnInstanceCreated();
			return s_Instance;
		}

		protected virtual void OnInstanceCreated() {}

#if UNITY_EDITOR
		[ExcludeFromCodeCoverage] static ScriptableSingletonBase()
		{
			EditorApplication.delayCall += DelayedCall;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
		}

		[ExcludeFromCodeCoverage] private static void DelayedCall() => GetOrCreateInstance(); // cannot CreateInstance from static ctor
		[ExcludeFromCodeCoverage] private static void OnBeforeAssemblyReload()
		{
			// prevent accumulating instances
			if (IsCreated)
				DestroyImmediate(s_Instance);
		}
#endif
	}
}
