// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tile
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
	}
}