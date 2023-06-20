// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using System;

namespace CodeSmile.Tests.Editor.Extensions
{
	public class ByteArrayExtTests
	{
		private static Byte[] CreateByteArray(Int32 length, Int32 modulo = 2)
		{
			length = Math.Max(0, length);
			modulo = Math.Max(1, modulo);

			var buffer = new Byte[length];
			for (var i = 0; i < length; i++)
				buffer[i] = (Byte)(i % modulo);
			return buffer;
		}

		[Test]
		public void CompressByteArray()
		{
			var buffer = CreateByteArray(1111);

			var zip = buffer.Compress();
			Assert.That(zip.Length, Is.EqualTo(31));
		}

		[TestCase(0, 0)]
		[TestCase(1, 1)]
		[TestCase(22, 2)]
		[TestCase(333, 3)]
		[TestCase(1234, 7)]
		[TestCase(6767, 33)]
		public void CompressAndDecompressByteArray(Int32 bufferLength, Int32 modulo)
		{
			var buffer = CreateByteArray(bufferLength, modulo);

			var zip = buffer.Compress();
			var unzipped = zip.Decompress();

			Assert.That(unzipped.Length, Is.EqualTo(buffer.Length));
			for (var i = 0; i < buffer.Length; i++)
				Assert.That(unzipped[i], Is.EqualTo(buffer[i]));
		}
	}
}
