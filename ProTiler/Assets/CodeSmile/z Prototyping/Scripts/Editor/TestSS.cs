// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.z_Prototyping.Scripts.Editor
{
	public class TestSO : ScriptableObject
	{
		/*
		private void Awake() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void Reset() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnEnable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDisable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDestroy() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
	*/
	}

	public class TestSS : ScriptableSingleton<TestSS>
	{
		private UnityEditor.Editor m_CachedEditor;
		private TestSO m_TestSo;
		public new static TestSS instance
		{
			get
			{
				var inst = ScriptableSingleton<TestSS>.instance;

				if (inst.m_TestSo == null)
					inst.m_TestSo = CreateInstance<TestSO>();

				UnityEditor.Editor.CreateCachedEditor(inst.m_TestSo, null, ref inst.m_CachedEditor);

				return inst;
			}
		}
		/*
		private void Awake() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void Reset() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnEnable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDisable() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
		private void OnDestroy() => Debug.Log($"{GetType().Name}: {MethodBase.GetCurrentMethod().Name}");
	*/
	}
}
