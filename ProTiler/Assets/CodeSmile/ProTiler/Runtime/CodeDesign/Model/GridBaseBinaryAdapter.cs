// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Runtime.Serialization;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public sealed class GridBaseBinaryAdapter<TGridMap> : VersionedBinaryAdapterBase, IBinaryAdapter<TGridMap>
		where TGridMap : GridBase, new()
	{
		public GridBaseBinaryAdapter(Byte version)
			: base(version) {}

		public unsafe void Serialize(in BinarySerializationContext<TGridMap> context, TGridMap map)
		{
			WriteAdapterVersion(context.Writer);
			map.Serialize(context);
		}

		public unsafe TGridMap Deserialize(in BinaryDeserializationContext<TGridMap> context)
		{
			ReadAdapterVersion(context.Reader);
			return new TGridMap().Deserialize(context, AdapterVersion) as TGridMap;
		}
	}
}
