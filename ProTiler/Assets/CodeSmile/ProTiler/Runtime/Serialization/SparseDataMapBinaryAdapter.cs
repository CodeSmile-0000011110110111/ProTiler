// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.Model;
using CodeSmile.Serialization;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.CodeDesign.Serialization
{
	public class SparseDataMapBinaryAdapter<TData> : VersionedBinaryAdapterBase, IBinaryAdapter<LinearDataMap<TData>>
		where TData : unmanaged
	{
		public Byte DataVersion { get; }

		public SparseDataMapBinaryAdapter(Byte adapterVersion, Byte dataVersion)
			: base(adapterVersion) => DataVersion = dataVersion;

		public void Serialize(in BinarySerializationContext<LinearDataMap<TData>> context,
			LinearDataMap<TData> value) => throw new NotImplementedException();

		public LinearDataMap<TData> Deserialize(in BinaryDeserializationContext<LinearDataMap<TData>> context) =>
			throw new NotImplementedException();
	}
}
