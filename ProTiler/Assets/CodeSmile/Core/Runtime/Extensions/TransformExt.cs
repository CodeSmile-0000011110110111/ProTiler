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
	}
}
