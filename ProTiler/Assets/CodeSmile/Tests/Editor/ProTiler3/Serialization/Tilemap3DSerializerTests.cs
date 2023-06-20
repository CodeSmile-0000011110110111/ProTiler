// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Model;
using CodeSmile.ProTiler3.Serialization;
using NUnit.Framework;

namespace CodeSmile.Tests.Editor.ProTiler3.Serialization
{
	public class Tilemap3DSerializerTests
	{
		[Test]
		public void SerializeNullCreatesEmptyArray()
		{
			var serializer = new Tilemap3DSerializer();
			var data = serializer.SerializeTilemap(null);

			Assert.That(data != null);
			Assert.That(data.Length, Is.EqualTo(0));
		}

		[Test]
		public void SerializeEmptyTilemapCreatesNonEmptyArray()
		{
			var serializer = new Tilemap3DSerializer();
			var data = serializer.SerializeTilemap(new Tilemap3D());

			Assert.That(data != null);
			Assert.That(data.Length, Is.GreaterThan(0));
		}

		[Test]
		public void SerializeAndDeserializeEmptyTilemapCorrectly()
		{
			var serializer = new Tilemap3DSerializer();
			var data = serializer.SerializeTilemap(new Tilemap3D());
			var tilemap = serializer.DeserializeTilemap(data);

			Assert.That(tilemap != null);
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.DefaultChunkSize));
		}

	}
}
