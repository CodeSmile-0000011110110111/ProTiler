// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Reflection;

namespace CodeSmileEditor.Tile
{
	public static class GizmosExt
	{
		private static Type s_AnnotationUtil;
		private static PropertyInfo s_ShowOutline;

		public static bool ShowSelectionOutline
		{
			get
			{
				InitAnnotationUtility();
				return (bool)s_ShowOutline.GetValue(null);
			}
			set
			{
				InitAnnotationUtility();
				s_ShowOutline.SetValue(null, value);
			}
		}

		private static void InitAnnotationUtility()
		{
			if (s_AnnotationUtil == null)
			{
				s_AnnotationUtil = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");

				s_ShowOutline = s_AnnotationUtil.GetProperty("showSelectionOutline",
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			}
		}
	}
}