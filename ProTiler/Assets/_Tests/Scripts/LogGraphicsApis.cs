﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace _Tests.Scripts
{
	public class LogGraphicsApis : MonoBehaviour
	{
		[SerializeField] private BuildTarget m_BuildTarget;
		private void OnValidate()
		{
			#if UNITY_EDITOR
			var apis = PlayerSettings.GetGraphicsAPIs(m_BuildTarget);
			for (var index = 0; index < apis.Length; index++)
			{
				var api = apis[index];
				Debug.Log($"Graphics API for index #{index} is '{api}'");
			}
			#endif
		}
	}
}
