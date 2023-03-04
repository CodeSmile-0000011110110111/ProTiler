// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Tile
{
	public enum TilePivot
	{
		Center,
	}
	
	[ExecuteInEditMode]
	public sealed partial class TileWorld : MonoBehaviour
	{
		[SerializeField] private TileGrid m_Grid = new();
		[SerializeField] private List<TileLayer> m_Layers = new();
		[SerializeField] private TilePivot m_TilePivot;
		
		public TileGrid Grid => m_Grid;
		public TilePivot TilePivot { get => m_TilePivot; set => m_TilePivot = value; }
		//[SerializeField] private List<TileChunk> m_Chunks = new();

		private void Start()
		{
			if (m_Layers.Count == 0)
				m_Layers.Add(new TileLayer());
		}

		public void DrawTile(Vector3 gridPosition)
		{
			Debug.Log($"draw at {gridPosition}");
		}
	}
}