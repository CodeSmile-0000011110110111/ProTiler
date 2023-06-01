// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Runtime.ProTiler.Rendering
{
	public class Tilemap3DFrustumCullingRuntimeTests
	{
		[Test]
		public void Tilemap3DFrustumCullingRuntimeTestsSimplePasses()
		{
			// Use the Assert class to test conditions.
			
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityTest]
		public IEnumerator Tilemap3DFrustumCullingRuntimeTestsWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			yield return null;
		}
	}
}