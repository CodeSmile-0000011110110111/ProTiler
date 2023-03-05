// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	public sealed partial class TileWorld : MonoBehaviour
	{
		[SerializeField] private int m_ActiveLayerIndex;
		[SerializeField] private List<TileLayer> m_Layers = new();

		private void Start()
		{
			m_ActiveLayerIndex = 0;
			//if (m_Layers.Count == 0) m_Layers.Add(new TileLayer());
		}

		public TileLayer ActiveLayer => m_Layers[m_ActiveLayerIndex];
	}
}