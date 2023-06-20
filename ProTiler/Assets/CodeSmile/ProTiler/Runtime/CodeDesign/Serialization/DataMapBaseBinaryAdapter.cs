// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using CodeSmile.ProTiler.Runtime.CodeDesign.Model._remove;
using CodeSmile.Serialization;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization
{
	public sealed class DataMapBaseBinaryAdapter<TDataMap> : VersionedBinaryAdapterBase, IBinaryAdapter<TDataMap>
		where TDataMap : DataMapBase, new()
	{
		private const Byte CurrentAdapterVersion = 0;

		private readonly Byte m_UserDataVersion;

		public DataMapBaseBinaryAdapter()
			: base(CurrentAdapterVersion) {}

		public DataMapBaseBinaryAdapter(Byte userDataVersion)
			: base(CurrentAdapterVersion) => m_UserDataVersion = userDataVersion;

		public unsafe void Serialize(in BinarySerializationContext<TDataMap> context, TDataMap value)
		{
			var writer = context.Writer;
			WriteAdapterVersion(writer);
			writer->Add(m_UserDataVersion);
			value.Serialize(new BinaryWriter(context.Writer));
		}

		public unsafe TDataMap Deserialize(in BinaryDeserializationContext<TDataMap> context)
		{
			var reader = context.Reader;
			var serializedAdapterVersion = ReadAdapterVersion(reader);
			if (serializedAdapterVersion == CurrentAdapterVersion)
			{
				var serializedUserDataVersion = reader->ReadNext<Byte>();
				var map = new TDataMap();
				return (TDataMap)map.Deserialize(new BinaryReader(context.Reader), serializedUserDataVersion);
			}

			if (serializedAdapterVersion > CurrentAdapterVersion)
			{
				throw new SerializationVersionException(
					GetFutureVersionExceptionMessage(serializedAdapterVersion, CurrentAdapterVersion));
			}

			throw new SerializationVersionException(GetLegacyVersionExceptionMessage(serializedAdapterVersion,
				CurrentAdapterVersion));
		}
	}
}
