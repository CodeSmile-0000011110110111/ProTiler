// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class GameObjectExt
	{
		public static T GetOrAddComponent<T>(this GameObject go) where T: Component
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
				transform =
				{
					parent = parent.transform,
				},
			};
		}
	}
}