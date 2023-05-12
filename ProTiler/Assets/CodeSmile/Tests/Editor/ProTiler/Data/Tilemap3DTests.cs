// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tilemap3DTests
	{
		private static readonly object[] IllegalChunkSizes =
		{
			new object[] { new ChunkSize(1, 0) },
			new object[] { new ChunkSize(0, 1) },
			new object[] { new ChunkSize(2, 1) },
			new object[] { new ChunkSize(-1, 1) },
			new object[] { new ChunkSize(1, -1) },
		};

		[Test]
		public void EnsureDefaultCtorUsesMinChunkSize()
		{
			var tilemap = new Tilemap3D();

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.MinChunkSize));
		}

		[TestCaseSource(nameof(IllegalChunkSizes))]
		public void EnsureTilemapHasMinChunkSize(ChunkSize illegalChunkSize)
		{
			var tilemap = new Tilemap3D(illegalChunkSize);

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.MinChunkSize));
		}
	}
}
