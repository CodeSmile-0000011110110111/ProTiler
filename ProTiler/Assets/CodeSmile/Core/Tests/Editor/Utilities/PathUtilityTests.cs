// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.IO;

namespace CodeSmile.Tests.Editor.Utilities
{
	public class PathUtilityTests
	{
		[TestCase("/")]
		[TestCase(@"\")]
		[TestCase("Assets/SomeFolder/")]
		[TestCase(@"Assets\SomeFolder\")]
		public void PathEnsureSeparatorNotAdded(string path)
		{
			var modifiedPath = PathUtility.EnsurePathEndsWithSeparator(path);

			Assert.That(path.Equals(modifiedPath));
		}

		[TestCase("")]
		[TestCase("Assets/SomeFolder")]
		[TestCase(@"Assets\SomeFolder")]
		[TestCase(".hidden-folder")]
		[TestCase("file.name")]
		public void PathEnsureSeparatorAdded(string path)
		{
			var modifiedPath = PathUtility.EnsurePathEndsWithSeparator(path);

			Assert.That(modifiedPath.EndsWith(Path.DirectorySeparatorChar));
		}

		[Test] public void PathEnsurePathEndsThrowsIfPathNull() =>
			Assert.Throws<NullReferenceException>(() => PathUtility.EnsurePathEndsWithSeparator(null));
	}
}
