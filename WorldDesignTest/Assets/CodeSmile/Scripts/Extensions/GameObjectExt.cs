// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.EditorTests
{
	public static class GameObjectExt
	{
		public static bool IsNullOrMissing(this GameObject instance) => instance == null || ReferenceEquals(instance, null);
	}
}