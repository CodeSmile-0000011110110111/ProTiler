// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tilemap3DChunkTests
	{
		[Test] public void CreateTilemap3DChunkIsNotNull()
		{
			CreateChunk(5, 5);
			//Assert.That();
		}

		private static void CreateChunk(int width, int height)
		{
			var chunk = new Tilemap3DChunk(new ChunkSize(width, height));
		}
	}
}
