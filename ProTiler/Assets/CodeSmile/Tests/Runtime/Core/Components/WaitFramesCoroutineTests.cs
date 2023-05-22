// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Tools.Attributes;
using CodeSmile.Tests.Tools.TestRunnerApi;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Runtime.Core.Components
{
	public class WaitFramesCoroutineTests
	{
		[UnityTest] public IEnumerator SimpleFrameCountTest()
		{
			int yieldCount = 0;
			var renderedFrameCount = Time.renderedFrameCount;
			var currentFrameCount = Time.frameCount;
			while (currentFrameCount == Time.frameCount)
			{
				yieldCount++;
				yield return null;
			}

			Debug.Log($"Wait 1 frameCount required {yieldCount} yields " +
			          $"over {Time.renderedFrameCount - renderedFrameCount} frames");
		}


		[Test] [CreateEmptyScene] [CreateGameObject(nameof(TestMB), typeof(TestMB))]
		public void WaitZeroFramesDoesNotExecuteImmediately()
		{
			var waitFrames = ObjectExt.FindObjectByTypeFast<TestMB>();

			var runCount = 0;
			waitFrames.WaitForFramesElapsed(0, () => { runCount++; });

			Assert.That(runCount, Is.EqualTo(0));
		}

		[UnityTest] [CreateEmptyScene] [CreateGameObject(nameof(TestMB), typeof(TestMB))]
		public IEnumerator WaitFramesDoesNotRepeat()
		{
			var waitFrames = ObjectExt.FindObjectByTypeFast<TestMB>();

			var runCount = 0;
			waitFrames.WaitForFramesElapsed(1, () => { runCount++; });
			Assert.That(runCount, Is.EqualTo(0));

			yield return null;
			Assert.That(runCount, Is.EqualTo(1));

			yield return null;
			Assert.That(runCount, Is.EqualTo(1));
		}

		[UnityTest] [CreateEmptyScene] [CreateGameObject(nameof(TestMB), typeof(TestMB))]
		public IEnumerator WaitZeroFramesWaitsOneFrame()
		{
			var waitFrames = ObjectExt.FindObjectByTypeFast<TestMB>();

			var runCount = 0;
			waitFrames.WaitForFramesElapsed(0, () => { runCount++; });
			Assert.That(runCount, Is.EqualTo(0));

			yield return null;
			Assert.That(runCount, Is.EqualTo(1));
		}

		[UnityTest] [CreateEmptyScene] [CreateGameObject(nameof(TestMB), typeof(TestMB))]
		public IEnumerator WaitOneFrameWaitsOneFrame()
		{
			var waitFrames = ObjectExt.FindObjectByTypeFast<TestMB>();

			var runCount = 0;
			waitFrames.WaitForFramesElapsed(1, () => { runCount++; });

			Assert.That(runCount, Is.EqualTo(0));

			yield return null;

			Assert.That(runCount, Is.EqualTo(1));
		}

		[UnityTest] [CreateEmptyScene] [CreateGameObject(nameof(TestMB), typeof(TestMB))]
		public IEnumerator WaitFrameWaitsExpectedNumberOfFrames()
		{
			var waitFrames = ObjectExt.FindObjectByTypeFast<TestMB>();

			var framesToWait = 4;
			var runCount = 0;
			waitFrames.WaitForFramesElapsed(framesToWait, () => { runCount++; });

			for (int i = 0; i < framesToWait; i++)
			{
				Assert.That(runCount, Is.EqualTo(0));
				yield return null;
			}

			Assert.That(runCount, Is.EqualTo(1));
		}

		private sealed class TestMB : MonoBehaviour {}
	}
}
