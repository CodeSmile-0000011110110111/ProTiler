// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
	{
		private void OnValidate()
		{
#if UNITY_EDITOR
			ClampLayerIndex();
			ValidateLayerPrefabs();
#endif
		}

#if UNITY_EDITOR
		private void ClampLayerIndex() => m_ActiveLayerIndex = Mathf.Clamp(m_ActiveLayerIndex, 0, m_Layers.Count);

		private void ValidateLayerPrefabs()
		{
			foreach (var tileLayer in m_Layers)
				tileLayer.OnValidate();
		}
#endif
	}
}