﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Assets;
using System;

namespace CodeSmile.ProTiler3.Rendering
{
	public interface ITile3DAssetSet
	{
		public Tile3DAssetBase this[UInt16 tileIndex] { get; }
	}
}
