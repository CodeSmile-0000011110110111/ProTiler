// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// This assembly is required to be fully covered by tests
[assembly: FullCovered]

[assembly: InternalsVisibleTo("CodeSmile.Tests.Editor.ProTiler")]
[assembly: InternalsVisibleTo("CodeSmile.Tests.Runtime.ProTiler")]

// ********************************************************************************
// NDepend: Suppressed Messages
// ********************************************************************************
