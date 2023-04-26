// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.IO;

namespace CodeSmile
{
	public static class PathUtility
	{
		public static string TrimTrailingDirectorySeparatorChar(string path)
		{
			if (path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar) || path.EndsWith(@"\"))
				return path.Substring(0, path.Length - 1);

			return path;
		}

		public static string EnsurePathEndsWithSeparator(string path)
		{
			if (path.EndsWith(Path.DirectorySeparatorChar) == false &&
			    path.EndsWith(Path.AltDirectorySeparatorChar) == false)
				path += Path.DirectorySeparatorChar;
			return path;
		}
	}
}
