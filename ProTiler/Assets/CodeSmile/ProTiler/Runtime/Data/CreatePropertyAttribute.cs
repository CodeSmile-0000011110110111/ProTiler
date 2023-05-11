// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Data
{
#if !UNITY_PROPERTIES_EXISTS
	internal class CreatePropertyAttribute : Attribute
	{
		internal CreatePropertyAttribute() => Debug.Log("use of internal CreateProperty attribute");
	}
#endif
}
