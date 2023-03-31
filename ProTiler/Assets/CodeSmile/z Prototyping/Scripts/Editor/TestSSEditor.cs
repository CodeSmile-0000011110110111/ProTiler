// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.z_Prototyping.Scripts.Editor
{
	[CustomEditor(typeof(TestSS))]
	public class TestSSEditor : UnityEditor.Editor
	{
		/*
		private void Awake() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void Reset() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnEnable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDisable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDestroy() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnPreSceneGUI() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnSceneGUI() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
	*/
	}

	[CustomEditor(typeof(TestSO))]
	public class TestSOEditor : UnityEditor.Editor
	{
		/*
		private void Awake() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void Reset() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnEnable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnSceneGUI() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDisable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDestroy() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
	*/
	}
}
