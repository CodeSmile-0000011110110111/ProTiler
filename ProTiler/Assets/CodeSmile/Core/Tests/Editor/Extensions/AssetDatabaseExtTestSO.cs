// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Utilities;
using UnityEngine;

namespace CodeSmile.Tests.Editor
{
	public class AssetDatabaseExtTestSO : ScriptableObject
	{
		public static string TestPath =>
			TestPaths.TempTestAssets + "CreateTest/" + nameof(AssetDatabaseExtTestSO) + ".asset";
	}
}
