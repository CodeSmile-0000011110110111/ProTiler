// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Rendering
{
	[AddComponentMenu("")] // hide from Add Component list
	public class Tile3DRenderer : MonoBehaviour
	{
		private GameObject m_PrefabInstance;
		private Tile3DCoord m_TileCoord;

		internal void OnTileModified(Tile3DCoord tileCoord, ITile3DAssetIndexer prefabLookup)
		{
			var tileIndexChanged = m_TileCoord.Tile.Index != tileCoord.Tile.Index;

			if (m_TileCoord != tileCoord)
				m_TileCoord = tileCoord;

			if (tileIndexChanged)
			{
				var tileIndex = tileCoord.Tile.Index;
				var tileAsset = prefabLookup[tileIndex];

				if (m_PrefabInstance != null)
					m_PrefabInstance.DestroyInAnyMode();

				if (tileAsset.Prefab == null)
				{
					Debug.LogWarning($"tileAsset prefab is null for {tileAsset}");
					return;
				}

				Debug.Log($"instantiate tile index {tileCoord.Tile.Index} => {tileAsset.Prefab.name}");
				m_PrefabInstance = Instantiate(tileAsset.Prefab, tileCoord.Coord, Quaternion.identity, transform);
			}
		}

#if UNITY_EDITOR
		[Pure] private Grid3DController Grid => transform.parent.parent.parent.GetComponent<Grid3DController>();
		private void OnDrawGizmos()
		{
			if (EditorPrefs.GetBool("CodeSmile.TestRunner.Running"))
				return;

			if (m_TileCoord.Tile.IsEmpty == false)
				Gizmos.DrawWireCube(transform.position, Grid.CellSize);
			else
			{
				Gizmos.DrawWireSphere(transform.position, (Grid.CellSize.x + Grid.CellSize.z) / 4f);
			}
		}
#endif
	}
}
