// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public interface IBinaryReader
	{
		public T ReadNext<T>() where T : unmanaged;
	}
}
