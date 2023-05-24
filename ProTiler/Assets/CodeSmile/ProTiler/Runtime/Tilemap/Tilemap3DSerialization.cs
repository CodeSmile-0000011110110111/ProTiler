// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Tilemap;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using UnityEngine;

namespace CodeSmile.ProTiler.Controller
{
	[Serializable]
	public sealed class Tilemap3DSerialization
	{
		private const Boolean m_UseBinarySerialization = true;
		[SerializeField] private Byte[] m_SerializedData = new Byte[0];

		internal void SerializeTilemap(Tilemap3D tilemap)
		{
			m_SerializedData = m_UseBinarySerialization
				? UnitySerialization.ToBinary(tilemap).Compress()
				: Encoding.UTF8.GetBytes(UnitySerialization.ToJson(tilemap, false)).Compress();

			Debug.Log($"{tilemap} => {m_SerializedData.Length} bytes ({m_SerializedData.CalculateMd5Hash()})");
		}

		[Pure] internal Tilemap3D DeserializeTilemap()
		{
			var tilemap = m_UseBinarySerialization
				? UnitySerialization.FromBinary<Tilemap3D>(m_SerializedData.Decompress())
				: UnitySerialization.FromJson<Tilemap3D>(Encoding.UTF8.GetString(m_SerializedData.Decompress()));

			Debug.Log($"{tilemap} <= {m_SerializedData.Length} bytes ({m_SerializedData.CalculateMd5Hash()})");

			return tilemap;
		}
	}
}
