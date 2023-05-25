// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Model;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using UnityEngine;

namespace CodeSmile.ProTiler.Serialization
{
	public sealed class Tilemap3DSerializer
	{
		private const Boolean m_UseBinarySerialization = true;

		internal Byte[] SerializeTilemap(Tilemap3D tilemap)
		{
			var data = m_UseBinarySerialization
				? UnitySerializer.ToBinary(tilemap).Compress()
				: Encoding.UTF8.GetBytes(UnitySerializer.ToJson(tilemap, false)).Compress();

			Debug.Log($"{tilemap} => {data.Length} bytes ({data.CalculateMd5Hash()})");
			return data;
		}

		[Pure] internal Tilemap3D DeserializeTilemap(Byte[] data)
		{
			var tilemap = m_UseBinarySerialization
				? UnitySerializer.FromBinary<Tilemap3D>(data.Decompress())
				: UnitySerializer.FromJson<Tilemap3D>(Encoding.UTF8.GetString(data.Decompress()));

			Debug.Log($"{tilemap} <= {data.Length} bytes ({data.CalculateMd5Hash()})");

			return tilemap;
		}
	}
}
