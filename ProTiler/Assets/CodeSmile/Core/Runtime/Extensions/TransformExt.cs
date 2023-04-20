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
	}
}
