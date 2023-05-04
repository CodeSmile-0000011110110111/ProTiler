// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Utilities;
using NUnit.Framework;
using System;
using System.IO;

namespace CodeSmile.Tests.Core.Editor.Utilities
{
	public class PathUtilityTests
	{
		[TestCase("/")]
		[TestCase("Assets/SomeFolder/")]
#if UNITY_EDITOR_WIN
		[TestCase(@"\")]
		[TestCase(@"Assets\SomeFolder\")]
#endif
		public void PathEnsureSeparatorNotAdded(string path)
		{
			var modifiedPath = PathUtility.AppendDirectorySeparatorChar(path);

			Assert.That(path.Equals(modifiedPath));
		}

		[TestCase("")]
		[TestCase("Assets/SomeFolder")]
		[TestCase(@"Assets\SomeFolder")]
		[TestCase(".hidden-folder")]
		[TestCase("file.name")]
		public void PathEnsureSeparatorAdded(string path)
		{
			var modifiedPath = PathUtility.AppendDirectorySeparatorChar(path);

			Assert.That(modifiedPath.EndsWith(Path.DirectorySeparatorChar));
		}

		[TestCase("")]
		[TestCase("Assets/SomeFolder")]
		[TestCase("/")]
		[TestCase(@"\")]
		[TestCase("Assets/SomeFolder/")]
		[TestCase(@"Assets\SomeFolder\")]
		public void PathTrimTrailingDirectorySeparator(string path)
		{
			var modifiedPath = PathUtility.TrimTrailingDirectorySeparatorChar(path);

			Assert.That(modifiedPath.EndsWith(Path.DirectorySeparatorChar) == false);
			Assert.That(modifiedPath.EndsWith(Path.AltDirectorySeparatorChar) == false);
		}

		[Test] public void PathEnsurePathEndsThrowsIfPathNull() =>
			Assert.Throws<NullReferenceException>(() => PathUtility.AppendDirectorySeparatorChar(null));
	}
}
