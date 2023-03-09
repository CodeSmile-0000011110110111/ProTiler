// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CodeSmile.EditorTests
{
	[Serializable]
	public class TestDict : SerializedDictionary<KeyClass, string>
	{
		
	}

	[Serializable]
	public class KeyClass
	{
		[SerializeField]private Vector3 m_Vector3;
		public KeyClass(Vector3 input)
		{
			m_Vector3 = input;
		}
	}
	
	public class SerializeDictionaryTest : MonoBehaviour
	{
		[SerializeField] private TestDict m_TestDict;
		[SerializeField] private bool m_AddSomeValues;

		private void OnValidate()
		{
			if (m_AddSomeValues)
			{
				m_AddSomeValues = false;
				m_TestDict.Add(new KeyClass(new Vector3(Time.frameCount, Time.frameCount, Time.frameCount)), Time.realtimeSinceStartupAsDouble.ToString());

				LogValues();
			}
		}

		private void LogValues()
		{
			Debug.Log("==============================");
			Debug.Log(m_TestDict.Count);
			foreach (var kvp in m_TestDict)
			{
				Debug.Log($"{kvp.Key} = {kvp.Value}");
			}
		}
	}
}