// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("NDepend", "ND1100:FromNowAllTypesAddedShouldRespectBasicQualityPrinciples",
	Scope = "deep", Target="", Justification="is okay in test classes")]

[assembly: SuppressMessage("NDepend", "ND1001:AvoidTypesWithTooManyMethods",
	Scope = "deep", Target = "", Justification = "is okay in test classes")]
[assembly: SuppressMessage("NDepend", "ND1206:AStatelessClassOrStructureMightBeTurnedIntoAStaticType",
	Scope = "deep", Target="", Justification="is okay in test classes")]
[assembly: SuppressMessage("NDepend", "ND1207:NonStaticClassesShouldBeInstantiatedOrTurnedToStatic",
	Scope = "deep", Target="", Justification="is okay in test classes")]
