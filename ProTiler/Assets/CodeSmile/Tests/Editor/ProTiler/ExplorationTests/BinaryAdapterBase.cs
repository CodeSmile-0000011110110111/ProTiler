// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public abstract class BinaryAdapterBase
	{
		protected Byte Version { get; set; }
		public BinaryAdapterBase(Byte version) => Version = version;
		protected unsafe void WriteVersion(UnsafeAppendBuffer* writer) => writer->Add(Version);
		protected unsafe void ReadVersion(UnsafeAppendBuffer.Reader* reader) => Version = reader->ReadNext<Byte>();
	}
}
