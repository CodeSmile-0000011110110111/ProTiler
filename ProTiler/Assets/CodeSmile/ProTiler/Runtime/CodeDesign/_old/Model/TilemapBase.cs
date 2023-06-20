// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using Unity.Mathematics;

namespace CodeSmile.ProTiler.CodeDesign._old.Model
{
	public abstract class TilemapBase
	{
		private int3 m_ChunkSize;
		protected List<ITilemapDataLinear> linearMaps;
		protected List<ITilemapDataSparse> sparseMaps;

		public virtual void Serialize(IBinaryWriter writer)
		{
			//writer.Add(..);
		}

		public virtual TilemapBase Deserialize(IBinaryReader reader)
		{
			// deserialize base class fields first
			//baseField = reader.ReadNext<Byte>();
			return this;
		}

		public byte[] SerializeAll()
		{
			// used for undo/redo
			// for now keep the entire serialized map as a byte[] and make it a serializefield on the MB
			// we may later add a byte[][] SerializeModifiedChunks() to decrease the load
			// Unity can serialize jagged array if we wrap it: struct chunks{ byte[] chunkSer; }
			return default;
		}

		// ITilemapSerialization (handle different ways of serialization, may or may not compress)
		// created by abstract factory

		// TODO:
		// Undo/Redo:
	}
}
