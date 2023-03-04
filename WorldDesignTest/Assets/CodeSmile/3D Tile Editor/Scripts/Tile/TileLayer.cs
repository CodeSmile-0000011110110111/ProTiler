// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed partial class TileLayer
	{
		[NonSerialized] private TileWorld m_World;
		[SerializeField] private TileGrid m_Grid = new();
		
		// private List<TileLayer> m_Layers = new();
		[SerializeField] private int m_BrushIndex;
		[SerializeField] private List<GUID> m_Tiles = new();
		[SerializeField] private List<GameObject> m_PrefabTileset = new();
		
	}
}