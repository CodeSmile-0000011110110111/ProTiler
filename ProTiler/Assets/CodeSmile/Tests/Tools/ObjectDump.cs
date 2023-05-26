// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tests.Tools
{
	public static class ObjectDump
	{
		public static String DumpAll(this Scene scene)
		{
			var rootObjects = scene.GetRootGameObjects();
			if (rootObjects.Length == 0)
				return $"<'{scene.name}' is empty>";

			var sb = new StringBuilder($"{scene.name}:\n");
			foreach (var go in scene.GetRootGameObjects())
				sb.AppendLine(go.DumpAll());

			return sb.ToString();
		}

		public static String DumpAll(this Component component) => component.transform.Dump(DumpParams.DumpAll());

		public static String DumpAll(this GameObject go) => go.transform.Dump(DumpParams.DumpAll());

		public static String DumpAll(this Transform t) => t.Dump(DumpParams.DumpAll());

		public static String Dump(this Component component, DumpParams dumpParams = default) =>
			Dump(component.transform, dumpParams);

		public static String Dump(this GameObject go, DumpParams dumpParams = default) =>
			Dump(go.transform, dumpParams);

		public static String Dump(this Transform t, DumpParams dumpParams = default)
		{
			dumpParams.EnsureIndentNotNull();
			var sb = new StringBuilder();
			DumpInternal(sb, t, dumpParams);
			return sb.ToString();
		}

		public static void DumpInternal(StringBuilder sb, Transform t, DumpParams dumpParams = default)
		{
			var childCount = t.childCount;
			sb.Append($"{dumpParams.Indent}-> {t.name}");

			if (childCount > 0)
				sb.Append($" (+{childCount})");

			if (dumpParams.IncludeComponents)
			{
				sb.Append(" {");
				var first = true;
				foreach (var component in t.GetComponents<Component>())
				{
					if (component is Transform)
						continue;

					if (first)
						first = false;
					else
						sb.Append(", ");

					sb.Append(component.GetType().Name);
				}
				sb.Append("}");
			}

			sb.AppendLine();
			dumpParams.Indent += "   ";

			foreach (Transform childTransform in t)
				DumpInternal(sb, childTransform, dumpParams);
		}
	}

	public struct DumpParams
	{
		public Boolean IncludeComponents;
		public Boolean Recursive;
		public String Indent;

		public void EnsureIndentNotNull()
		{
			if (Indent == null)
				Indent = String.Empty;
		}

		public static DumpParams DumpAll() => new() { IncludeComponents = true, Recursive = true };
	}
}
