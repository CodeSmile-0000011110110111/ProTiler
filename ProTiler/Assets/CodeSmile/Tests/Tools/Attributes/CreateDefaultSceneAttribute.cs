// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor.SceneManagement;

namespace CodeSmile.Tests.Tools.Attributes
{
	/// <summary>
	///     Creates a new default scene (with Camera + Direct Light) for a unit test method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CreateDefaultSceneAttribute : CreateSceneAttribute
	{
		/// <summary>
		///     Creates a new default scene (with Camera + Direct Light) for a unit test method.
		///     Caution: the scene contents will be deleted with the exception of the default game objects named
		///     "Main Camera" and "Directional Light". Any changes to these two objects will persist between tests.
		///     If you rename these objects they will be deleted and not restored for other tests.
		/// </summary>
		/// <param name="scenePath">
		///     if non-empty, the scene will be saved as an asset for tests that verify correctness of save/load of a scene's
		///     contents. The saved scene asset is deleted after the test ran.
		/// </param>
		public CreateDefaultSceneAttribute(string scenePath = null)
			: base(scenePath, NewSceneSetup.DefaultGameObjects) {}
	}
}
