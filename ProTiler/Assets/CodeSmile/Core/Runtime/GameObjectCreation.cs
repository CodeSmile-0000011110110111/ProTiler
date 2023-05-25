// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile
{
	public static class GameObjectCreation
	{
		public static GameObject Create(string name, Transform parent, HideFlags hideFlags, params Type[] components)
		{
			var go = new GameObject(name, components);
			go.transform.parent = parent;
			go.hideFlags = hideFlags;
			return go;
		}
	}
}
