// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using NUnit.Framework;

namespace CodeSmile.ProTiler.Tests.Editor.Assets
{
	public class Tile3DAssetRegisterTests
	{
		[Test] public void EnsureTile3DRegisterSingletonNotNull() => Assert.That(Tile3DAssetRegister.Singleton != null);
	}
}
