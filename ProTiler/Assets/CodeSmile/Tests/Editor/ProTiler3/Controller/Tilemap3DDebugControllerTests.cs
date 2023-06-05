// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Editor.ProTiler3.Controller
{
	public class Tilemap3DDebugControllerTests
	{

		[Test]
		public void Tilemap3DDebugBehaviourTestsSimplePasses()
		{
			// Use the Assert class to test conditions.
			
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityTest]
		public IEnumerator Tilemap3DDebugBehaviourTestsWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			yield return null;
		}
	}
}
