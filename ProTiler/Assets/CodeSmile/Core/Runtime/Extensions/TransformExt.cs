// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Extensions
{
	public static class TransformExt
	{
		public static void DestroyAllChildren(this Transform t)
		{
			for (var i = t.childCount - 1; i >= 0; i--)
				t.GetChild(i).gameObject.DestroyInAnyMode();
		}

		public static Transform FindOrCreateChildObject(this Transform t, string name, GameObject prefab, HideFlags hideFlags)
		{
			var child = t.Find(name);
			if (child == null)
			{
				if (prefab == null)
				{
					child = new GameObject(name).transform;
					child.parent = t;
				}
				else
				{
#if DEBUG
					if (prefab == null)
						throw new NullReferenceException("FindOrCreateChildObject: prefab is null");
#endif

					child = Object.Instantiate(prefab, t).transform;
					child.name = name;
				}
			}

			child.gameObject.hideFlags = hideFlags;
			return child;
		}
	}
}