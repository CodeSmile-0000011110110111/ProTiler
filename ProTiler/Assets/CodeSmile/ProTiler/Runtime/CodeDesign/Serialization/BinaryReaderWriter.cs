// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization
{
	public unsafe struct BinaryReader : IBinaryReader
	{
		private readonly UnsafeAppendBuffer.Reader* m_Reader;

		public BinaryReader(UnsafeAppendBuffer.Reader* reader) => m_Reader = reader;
		public T ReadNext<T>() where T : unmanaged => m_Reader->ReadNext<T>();
	}

	public unsafe struct BinaryWriter : IBinaryWriter
	{
		private readonly UnsafeAppendBuffer* m_Writer;
		public BinaryWriter(UnsafeAppendBuffer* writer) => m_Writer = writer;
		public void Add<T>(T value) where T : unmanaged => m_Writer->Add(value);
	}
}
