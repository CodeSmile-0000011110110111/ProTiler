// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.Serialization
{
	/// <summary>
	///     Adapters can subclass this to be able to identify the version of the serialized data for the adapter.
	///     You are supposed to make calls to ReadVersion/WriteVersion as the first read/write operation within
	///     the Deserialize/Serialize IBinaryAdapter<> interface methods to read/write the version.
	///     In ReadVersion, you can then switch on the Version property to determine whether the serialized data
	///     is from the current or a previous version and handle these cases appropriately to make the older formats
	///     still readable in the current format. To do so, read values for deleted fields but discard the value,
	///     and use default or computed values for fields that have been added. Renamed fields need no special handling
	///     since the binary data does not care about field names.
	///     Usually you would bump the version when adding/removing fields AND whenever you want previous data
	///     to still be readable. You might think that a byte is not many versions but you will rarely maintain
	///     more than a few versions back, particularly during development. You most certainly do NOT want to
	///     maintain over 256 different versions of your serialized data after release. Trust me. ;)
	/// </summary>
	public abstract class VersionedBinaryAdapter
	{
		/// <summary>
		///     Version of the adapter. This is the "current" version.
		/// </summary>
		protected Byte AdapterVersion { get; }

		/// <summary>
		///     Create adapter with version.
		/// </summary>
		/// <param name="adapterVersion"></param>
		public VersionedBinaryAdapter(Byte adapterVersion) => AdapterVersion = adapterVersion;

		/// <summary>
		///     Adds the current Version to the writer.
		/// </summary>
		/// <param name="writer"></param>
		protected unsafe void WriteAdapterVersion(UnsafeAppendBuffer* writer) => writer->Add(AdapterVersion);

		/// <summary>
		///     Reads the serialized data's version from the reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		protected unsafe Byte ReadSerializedVersion(UnsafeAppendBuffer.Reader* reader) => reader->ReadNext<Byte>();
	}
}
