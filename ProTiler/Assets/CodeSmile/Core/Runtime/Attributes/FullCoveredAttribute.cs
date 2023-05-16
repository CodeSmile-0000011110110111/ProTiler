// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Attributes
{
	[FullCovered]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct |
	                AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class FullCoveredAttribute : Attribute {}
}
