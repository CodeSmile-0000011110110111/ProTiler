// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	public static class Grid3DUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex(int x, int y, int width) => y * width + x;
	}
}
