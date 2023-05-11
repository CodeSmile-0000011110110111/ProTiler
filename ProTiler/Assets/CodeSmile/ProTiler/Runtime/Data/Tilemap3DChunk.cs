// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Properties;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tilemap3DChunk
	{
		[CreateProperty] private ChunkSize m_Size;
		[CreateProperty] private List<Tile3DLayer> m_Layers;
		public ChunkSize Size => m_Size;

		public Tilemap3DChunk(ChunkSize size)
		{
			m_Size = size;
			m_Layers = new();
		}

		private Tile3DLayer GetOrCreateHeightLayer(int y)
		{
			//if (m_Layers.TryGetValue(y, out var layer)) return layer;

			var newLayer = new Tile3DLayer();
			return newLayer;
		}
	}
}
