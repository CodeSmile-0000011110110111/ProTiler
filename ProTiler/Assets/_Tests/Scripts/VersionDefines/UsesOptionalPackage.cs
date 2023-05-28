// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.


using UnityEditor;
using UnityEngine;

namespace DependentCode
{
	[InitializeOnLoad]
	public static class UsesOptionalPackage
	{
		static UsesOptionalPackage()
		{
			Debug.Log("static ctor");

#if USE_OPTIONAL_PACKAGE
			var optional = new OptionalAssembly.OptionalCode();
			Debug.Log(optional.GetOptionalString());
#endif
		}
	}
}
