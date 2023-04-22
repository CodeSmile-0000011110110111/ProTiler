// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace CodeSmile.ProTiler.Tests.Utilities
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CreateGameObjectAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		public const string DefaultName = "Test GameObject";

		private readonly string m_Name;
		private readonly Type[] m_Components;
		private GameObject m_GameObject;

		public CreateGameObjectAttribute(string name = DefaultName, params Type[] components)
		{
			m_Name = name;
			m_Components = components;
		}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			m_GameObject = EditorUtility.CreateGameObjectWithHideFlags(m_Name, HideFlags.None, m_Components);
			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			if (m_GameObject != null)
				m_GameObject.DestroyInAnyMode();
			yield return null;
		}
	}
}
