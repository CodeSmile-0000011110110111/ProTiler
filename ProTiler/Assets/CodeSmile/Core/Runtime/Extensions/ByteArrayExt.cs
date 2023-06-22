// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CodeSmile.Extensions
{
	public static class ByteArrayExt
	{
		/// <summary>
		///     Compresses a byte array and returns the GZip compressed buffer.
		/// </summary>
		/// <param name="uncompressedBuffer"></param>
		/// <returns></returns>
		public static Byte[] Compress(this Byte[] uncompressedBuffer)
		{
			using (var memoryStream = new MemoryStream())
			using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
			{
				zipStream.Write(uncompressedBuffer, 0, uncompressedBuffer.Length);
				zipStream.Close();
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		///     Decompresses a GZip byte array and returns the uncompressed buffer.
		/// </summary>
		/// <param name="compressedBuffer"></param>
		/// <returns></returns>
		public static Byte[] Decompress(this Byte[] compressedBuffer)
		{
			using (var compressedStream = new MemoryStream(compressedBuffer))
			using (var unzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var uncompressedStream = new MemoryStream())
			{
				unzipStream.CopyTo(uncompressedStream);
				return uncompressedStream.ToArray();
			}
		}

		public static String AsString(this Byte[] bytes)
		{
			return BitConverter.ToString(bytes);
			//return System.Convert.ToHexString(bytes);
			/*BitConverter.ToString(bytes)
			var sb = new StringBuilder();
			foreach (var b in bytes)
				sb.Append(b);
			return sb.ToString();*/
		}
	}
}
