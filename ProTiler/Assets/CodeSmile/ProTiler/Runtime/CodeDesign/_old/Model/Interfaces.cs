// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.ProTiler.CodeDesign._old.Model
{

	// Questions:
	// how to handle dirty flags?

	public interface IChunkDataSparse<TData> : IChunkData<TData> where TData : unmanaged {}
	public interface IChunkDataLinear<TData> : IChunkData<TData> where TData : unmanaged {}
	public interface IChunkData<TData> where TData : unmanaged {}

	public interface ITilemapDataSparse : ITilemapData {}
	public interface ITilemapDataLinear : ITilemapData {}

	public interface ITilemapData // ITilemapDataSerialization
	{
		// Serialize
		// SerializeChunk(key)
	}

	public interface IBinaryReader
	{
		public T ReadNext<T>() where T : unmanaged;
	}

	public interface IBinaryWriter
	{
		public void Add<T>(T value) where T : unmanaged;
	}


	// TODO:
	/*
	 * design flow of calls/data for serialization, deserialization and undo/redo (if different at all)
	 *	Note: serialization is in charge of compression/decompression
	 *
	 * 1. entire tilemap =>
	 *		byte[] TilemapBase:Serialize() // return bytes to user
	 *		TilemapBase TilemapBase:Deserialize(byte[]) // return to user, user cast to concrete type
	 * 2. specific chunk =>
	 *		CAUTION: we can only serialize a concrete type, not an object or interface!
	 *
	 *		void TilemapDataBase:SerializeChunk(Int64 key, in LinearDataChunk<TData> data)
	 *		LinearDataChunk<TData> TilemapDataBase:DeserializeChunk(Int64 key)
	 *			calls respective methods on ISerializedDataStream field (if not null)
	 *			serialization instance passed in ctor, to avoid changing streaming approach without full reset
	 *				(doable but will require extra handling to prevent data out of sync)
	 *
	 *
	 */
	//
	public interface ITilemapChunkDataSerialization<TChunk, TData>
		where TChunk : unmanaged, IChunkData<TData>
		where TData : unmanaged
	{
		public void SerializeChunk<TChunk>(Int64 key, TChunk chunk);
		public TChunk DeserializeChunk<TChunk>(Int64 key);
	}
	public interface IChunkDataLinearSerialization<TData> where TData : unmanaged
	{
		public Byte[] Serialize(Int64 chunkKey, ChunkDataLinear<TData> chunk);
		public (Int64, ChunkDataLinear<TData>) Deserialize(Byte[] bytes);
	}

	public interface ISerializedDataStream
	{
		public Byte[] ReadSerializedData(Int64 key);
		public void WriteSerializedData(Int64 key, Byte[] bytes);
	}

	// compression is an afterthought, handled outside the tilemap stuff
	public interface IBinaryCompression
	{
		public Byte[] Compress(Byte[] bytes);
		public Byte[] Decompress(Byte[] bytes);
	}
}
