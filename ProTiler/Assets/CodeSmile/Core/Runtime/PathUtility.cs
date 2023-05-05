// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.IO;

namespace CodeSmile
{
	[FullCovered]
	public static class PathUtility
	{
		public static string TrimTrailingDirectorySeparatorChar(string path) =>
			path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

		public static string AppendDirectorySeparatorChar(string path)
		{
			if (path.EndsWith(Path.DirectorySeparatorChar) == false && path.EndsWith(Path.AltDirectorySeparatorChar) == false)
				path += Path.DirectorySeparatorChar;
			return path;
		}
	}
}
