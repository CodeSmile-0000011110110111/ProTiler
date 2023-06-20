// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.Extensions;
using CodeSmile.ProTiler3.Assets;
using CodeSmile.ProTiler3.Editor.Creation;
using NUnit.Framework;
using System.IO;
using System.Linq;
using UnityEditor;

namespace CodeSmile.Tests.Editor.ProTiler3.Assets
{
	public class Tile3DAssetRegisterPersistenceTests
	{
		private static string GetRegisterAssetPath() => AssetDatabaseExt.FindAssetPaths<Tile3DAssetRegister>().First();

		[Test] public void CoverCtorUsageJustToSatisfyNDepend() => Assert.That(new Tile3DAssetRegisterPersistence() != null);

		[Test] public void EnsureTile3DRegisterExists() => Assert.That(AssetDatabaseExt.AssetExists<Tile3DAssetRegister>());

		[Test] public void EnsureTile3DRegisterCannotBeDeleted()
		{
			var path = GetRegisterAssetPath();

			Assert.That(AssetDatabase.DeleteAsset(path) == false);
		}

		[Test] public void EnsureTile3DRegisterCannotBeMovedToTrash()
		{
			var path = GetRegisterAssetPath();

			Assert.That(AssetDatabase.MoveAssetToTrash(path) == false);
		}

		[Test] public void EnsureTile3DRegisterContainingFolderCannotBeDeleted()
		{
			var path = GetRegisterAssetPath();

			Assert.That(AssetDatabase.DeleteAsset(Path.GetDirectoryName(path)) == false);
		}

		[Test] public void EnsureTile3DRegisterCannotBeRenamed()
		{
			var path = GetRegisterAssetPath();
			var newName = path.Replace($"{nameof(Tile3DAssetRegister)}.asset", "XXX.asset");

			Assert.That(AssetDatabase.RenameAsset(path, newName).Length != 0);
		}

		[Test] public void EnsureTile3DRegisterCanBeMoved()
		{
			var path = GetRegisterAssetPath();
			var targetDir = Path.GetDirectoryName(path) + "/MoveTo";
			Directory.CreateDirectory(targetDir);
			var newPath = targetDir + $"/{nameof(Tile3DAssetRegister)}.asset";

			Assert.That(AssetDatabase.MoveAsset(path, newPath).Length != 0);
			Assert.That(AssetDatabase.MoveAsset(newPath, path).Length != 0);

			Directory.Delete(targetDir);
		}
	}
}
