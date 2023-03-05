// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class TileGrid
	{
		[SerializeField] private Vector3Int m_Size;
		//[SerializeField] private Vector3Int m_Gap;
		
		public Vector3Int Size { get => m_Size; set => m_Size = value; }
		//public Vector3Int Gap { get => m_Gap; set => m_Gap = value; }
	}
}