// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
	{
		private void OnValidate()
		{
#if UNITY_EDITOR
			ValidateLayerPrefabs();
#endif
		}

#if UNITY_EDITOR
		private void ValidateLayerPrefabs()
		{
			foreach (var tileLayer in m_Layers)
				tileLayer.OnValidate();
		}
#endif
	}
}