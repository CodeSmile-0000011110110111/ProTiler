// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model._remove
{
	public unsafe class BinaryReader : IBinaryReader
	{
		private readonly UnsafeAppendBuffer.Reader* m_Reader;

		public BinaryReader(UnsafeAppendBuffer.Reader* reader) => m_Reader = reader;
		public T ReadNext<T>() where T : unmanaged => m_Reader->ReadNext<T>();
	}
}
