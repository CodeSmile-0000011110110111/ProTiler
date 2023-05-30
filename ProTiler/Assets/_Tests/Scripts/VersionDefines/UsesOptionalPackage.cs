// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using OptionalAssembly;
using UnityEditor;

namespace DependentCode
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public static class UsesOptionalPackage
	{
		static UsesOptionalPackage()
		{
			//Debug.Log("static ctor");

#if USE_OPTIONAL_PACKAGE
			var optional = new OptionalCode();
			//Debug.Log(optional.GetOptionalString());
#endif
		}
	}
}
