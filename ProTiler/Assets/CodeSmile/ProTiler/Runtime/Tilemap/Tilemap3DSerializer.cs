// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Serialization.Json;

namespace CodeSmile.ProTiler.Tilemap
{
	internal static class Tilemap3DSerializer
	{
		public static string ToJson(Tilemap3D tilemap, bool minified = true) => JsonSerialization.ToJson(tilemap,
			new JsonSerializationParameters { Minified = minified });

		public static Tilemap3D FromJson(string json) => JsonSerialization.FromJson<Tilemap3D>(json);
	}
}
