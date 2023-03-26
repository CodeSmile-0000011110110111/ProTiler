// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile
{
	[ExecuteInEditMode]
	public class MoveToCategoryInHierarchy : MonoBehaviour
	{
		public enum MoveTo
		{
			ParentWithPrefabName,
		}

#pragma warning disable 0414
		[SerializeField] private string m_ParentObjectPrefix = "_";
		[SerializeField] private MoveTo m_MoveTo;
#pragma warning restore

		private void Start()
		{
			if (Application.isPlaying == false)
			{
#if UNITY_EDITOR
				var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
				if (prefab != null)
				{
					//Debug.Log($"I'm that prefab: {prefab.name}");

					var folderName = $"{m_ParentObjectPrefix}{prefab.name}";
					var parent = GameObject.Find(folderName);
					if (parent == null)
						parent = new GameObject(folderName);

					transform.parent = parent.transform;

					//var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

					switch (m_MoveTo)
					{
						case MoveTo.ParentWithPrefabName:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				//DestroyImmediate(this);
#endif
			}
			else
			{
				Debug.Log("Runtime START");
				Destroy(this);
			}
		}
	}
}