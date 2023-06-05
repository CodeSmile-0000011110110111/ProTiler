// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// This assembly is required to be fully covered by tests
[assembly: FullCovered]

[assembly: InternalsVisibleTo("CodeSmile.ProTiler.Editor")]
[assembly: InternalsVisibleTo("CodeSmile.Tests.Editor.ProTiler")]
[assembly: InternalsVisibleTo("CodeSmile.Tests.Runtime.ProTiler")]

// ********************************************************************************
// NDepend: Suppressed Messages
// ********************************************************************************
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Scope = "deep", Target = "", Justification = "Unity devs won't expect immutable struct")]
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Scope = "deep", Target = "", Justification = "Unity devs won't expect immutable struct")]
