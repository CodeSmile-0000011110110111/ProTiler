// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model._remove
{
	public unsafe class BinaryWriter : IBinaryWriter
	{
		private readonly UnsafeAppendBuffer* m_Writer;
		public BinaryWriter(UnsafeAppendBuffer* writer) => m_Writer = writer;
		public void Add<T>(T value) where T : unmanaged => m_Writer->Add(value);
	}
}
