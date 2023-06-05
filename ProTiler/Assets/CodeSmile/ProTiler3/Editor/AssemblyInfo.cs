// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// This assembly is required to be fully covered by tests
[assembly: FullCovered]

[assembly: InternalsVisibleTo("CodeSmile.Tests.Editor.ProTiler3")]
[assembly: InternalsVisibleTo("CodeSmile.Tests.Runtime.ProTiler3")]

// ********************************************************************************
// NDepend: Suppressed Messages
// ********************************************************************************
[assembly: SuppressMessage("NDepend", "ND1606:TypesThatUsedToBe100PercentCoveredByTestsShouldStillBe100PercentCovered",
	Target = "CodeSmile.ProTiler.Editor.Creation.Tile3DAssetRegisterPersistence..cctor()",
	Justification = "NDepend keeps complaining even with no ctor or ctor with ExcludeFromCodeCoverage attribute so just STFU!")]
