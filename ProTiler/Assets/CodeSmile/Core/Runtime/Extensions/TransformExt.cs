// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using UnityEngine;

namespace CodeSmile.Extensions
{
	[FullCovered]
	public static class TransformExt
	{
		public static void DestroyAllChildren(this Transform t)
		{
			for (var i = t.childCount - 1; i >= 0; i--)
				t.GetChild(i).gameObject.DestroyInAnyMode();
		}

#if UNITY_EDITOR
		public static void DestroyInAnyMode(this Transform self)
		{
			if (Application.isPlaying == false)
				Object.DestroyImmediate(self.gameObject);
			else
				Object.Destroy(self.gameObject);
		}
#else
		public static void DestroyInAnyMode(this Transform self) => Object.Destroy(self.gameObject);
#endif

		public static Transform FindOrCreateChild(this Transform parent, string name, HideFlags hideFlags = HideFlags.None)
		{
			var t = parent.Find(name);
			if (t != null)
				return t;

			return new GameObject(name)
			{
				hideFlags = hideFlags,
				transform = { parent = parent.transform },
			}.transform;
		}
	}
}
