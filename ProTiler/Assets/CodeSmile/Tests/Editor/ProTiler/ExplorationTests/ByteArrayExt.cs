﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Text;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public static class ByteArrayExt
	{
		public static String AsString(this Byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (var b in bytes)
				sb.Append(b);
			return sb.ToString();
		}
	}
}