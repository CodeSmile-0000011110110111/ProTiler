// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileRenderer))]
	public sealed partial class TileWorld : MonoBehaviour
	{
		[SerializeField] private int m_ActiveLayerIndex;
		[SerializeField] private List<TileLayer> m_Layers = new();

		private void Reset()
		{
			if (m_Layers.Count == 0)
			{
				name = nameof(TileWorld);
				m_ActiveLayerIndex = 0;
				m_Layers.Add(new TileLayer());
			}
			
			if (GetComponent<TileRenderer>() == null)
				gameObject.AddComponent<TileRenderer>();
		}

		public TileLayer ActiveLayer => m_Layers[m_ActiveLayerIndex];
	}
}