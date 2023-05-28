// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;

namespace CodeSmile.Tests.Editor.Core
{
	public class ScriptableSingletonTests
	{
		[Test] public void ScriptableSingletonIsNotNull() => Assert.That(ScriptableSingletonTestImpl.Singleton != null);

		[Test] public void ScriptableSingletonIsNotCreatedInitially() =>
			Assert.That(ScriptableSingletonTestImpl.IsCreated);

		[Test] public void ScriptableSingletonIsCreatedAfterSingletonAccess()
		{
			var singleton = ScriptableSingletonTestImpl.Singleton;

			Assert.That(ScriptableSingletonTestImpl.IsCreated);
		}

		[Test] public void ScriptableSingletonInstanceCreatedOverrideGotCalled() =>
			Assert.That(ScriptableSingletonTestImpl.Singleton.InstanceCreatedWasCalled);

		private sealed class ScriptableSingletonTestImpl : ScriptableSingletonBase<ScriptableSingletonTestImpl>
		{
			public Boolean InstanceCreatedWasCalled;
			public new static Boolean IsCreated => ScriptableSingletonBase<ScriptableSingletonTestImpl>.IsCreated;
			protected override void OnInstanceCreated()
			{
				base.OnInstanceCreated();
				InstanceCreatedWasCalled = true;
			}
		}
	}
}
