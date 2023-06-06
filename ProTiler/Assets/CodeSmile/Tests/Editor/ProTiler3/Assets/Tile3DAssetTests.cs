﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Editor.Creation;
using CodeSmile.ProTiler3.Runtime.Assets;
using CodeSmile.ProTiler3.Runtime.Model;
using CodeSmile.Tests.Editor.ProTiler3.Model;
using CodeSmile.Tests.Tools;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler3.Assets
{
	public class Tile3DAssetTests
	{
		[Test] public void GetSetPrefabProperty()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();
			var testPrefab = TestAssets.LoadTestPrefab();

			tileAsset.Prefab = testPrefab;

			Assert.That(tileAsset.Prefab == testPrefab);
		}

		[Test] public void GetSetFlags()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();
			var flags = Tile3DTestCaseSource.AllDirectionsMask | Tile3DFlags.FlipBoth;

			tileAsset.Flags = flags;

			Assert.That(tileAsset.Flags == flags);
		}

		[Test] public void GetSetTransform()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();
			var transform = Matrix4x4.TRS(new Vector3(1.23f, 2.34f, -3.45f),
				Quaternion.Euler(new Vector3(12f, 34f, -78f)), new Vector3(1.5f, .5f, -.5f));

			tileAsset.Transform = transform;

			Assert.That(tileAsset.Transform == transform);
		}

		[Test] public void SetDefaultFlags()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();
			tileAsset.Flags = Tile3DTestCaseSource.AllDirectionsMask | Tile3DFlags.FlipBoth;

			tileAsset.SetDefaultFlags();

			Assert.That(tileAsset.Flags == Tile3DFlags.DirectionNorth);
		}
	}
}