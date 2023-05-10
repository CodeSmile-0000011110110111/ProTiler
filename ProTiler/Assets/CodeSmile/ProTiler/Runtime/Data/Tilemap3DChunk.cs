// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Collections;
using System;

namespace CodeSmile.ProTiler.Data
{
	public class Tilemap3DChunk
	{
		private Tilemap3DChunkCollection m_Chunks;

		[Serializable] public class Tilemap3DChunkCollection : SerializedDictionary<long, Tilemap3DChunk> {}
	}
}
