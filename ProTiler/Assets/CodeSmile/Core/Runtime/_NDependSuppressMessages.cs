// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;

// ignore compiler generated types
[assembly: SuppressMessage("NDepend", "",
	Target = "UnitySourceGeneratedAssemblyMonoScriptTypes_v1",
	Justification = "Unity generated types should not be analyzed")]
[assembly: SuppressMessage("NDepend", "",
	Target = "CodeSmile.Editor:UnitySourceGeneratedAssemblyMonoScriptTypes_v1+MonoScriptData",
	Justification = "Unity generated types should not be analyzed")]

// mutable structs are okay
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Target = "CodeSmile.ProTiler:CodeSmile.ProTiler.Data.Tile3D", Justification = "Unity devs don't expect r/o structs")]
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Target = "CodeSmile.ProTiler:CodeSmile.ProTiler.Data.Tile3DCoord", Justification = "Unity devs don't expect r/o structs")]

// ignore 3rd party code issues
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Target = "CodeSmile:CodeSmile.Collections.NativeArray2D<T>", Justification = "3rd party code")]
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Target = "CodeSmile:CodeSmile.Collections.NativeArray2D<T>+Enumerator", Justification = "3rd party code")]
[assembly: SuppressMessage("NDepend", "ND1313:OverrideEqualsAndOperatorEqualsOnValueTypes",
	Target = "CodeSmile:CodeSmile.Collections.NativeArray2D<T>+Enumerator", Justification = "3rd party code")]

// test
[assembly: SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable",
	Justification = "Unity devs don't expect readonly structs")]
