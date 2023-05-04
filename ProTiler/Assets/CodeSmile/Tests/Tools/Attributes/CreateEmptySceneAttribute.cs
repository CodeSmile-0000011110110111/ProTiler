// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Tests.Tools.Attributes
{
	/// <summary>
	///     Creates a new empty scene for a unit test method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CreateEmptySceneAttribute : CreateSceneAttribute
	{
		/// <summary>
		///     Creates a new empty scene for a unit test method.
		/// </summary>
		/// <param name="scenePath">
		///     if non-empty, the scene will be saved as an asset for tests that verify correctness of
		///     save/load of a scene's contents. The saved scene asset is deleted after the test ran.
		/// </param>
		public CreateEmptySceneAttribute(string scenePath = null)
			: base(scenePath) {}
	}
}
