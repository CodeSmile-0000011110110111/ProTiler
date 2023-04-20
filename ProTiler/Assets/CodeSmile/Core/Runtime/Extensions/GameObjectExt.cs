// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Extensions
{
	public static class GameObjectExt
	{
		public static T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			var component = go.GetComponent<T>();
			if (component == null)
				component = go.AddComponent<T>();
			return component;
		}

		public static GameObject FindOrCreateChild(this GameObject parent, string name, HideFlags hideFlags = HideFlags.None)
		{
			var t = parent.transform.Find(name);
			if (t != null)
				return t.gameObject;

			return new GameObject(name)
			{
				hideFlags = hideFlags,
				transform = { parent = parent.transform },
			};
		}

		public static GameObject FindOrCreateChild(this GameObject parent, string name, GameObject original,
			HideFlags hideFlags = HideFlags.None)
		{
			var t = parent.transform.Find(name);
			if (t != null)
				return t.gameObject;

			if (original == null)
				throw new ArgumentNullException("prefab is null");

			var child = Object.Instantiate(original, t);
			child.name = name;
			child.hideFlags = hideFlags;
			child.transform.parent = parent.transform;

			return child;
		}

		public static bool IsPrefab(this GameObject go) => go.scene.rootCount == 0;
	}
}
