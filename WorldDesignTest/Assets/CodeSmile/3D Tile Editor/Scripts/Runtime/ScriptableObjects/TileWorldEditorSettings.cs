// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tile
{
	[CreateAssetMenu(fileName = "New TileWorldEditor Settings", menuName = Global.TileEditorName + "/New Settings", order = 0)]
	public class TileWorldEditorSettings : ScriptableObject
	{
		[SerializeField] private GameObject m_MissingTilePrefab;
		[SerializeField] private GameObject m_TilePrefab;
	}
}