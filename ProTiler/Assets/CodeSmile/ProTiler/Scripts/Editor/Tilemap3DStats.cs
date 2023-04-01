// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor
{
	/// <summary>
	/// Just for fun, this class persists Tilemap3D operations for gamification purposes.
	/// </summary>
	[FilePath("ProjectSettings/Tilemap3DStats.asset", FilePathAttribute.Location.ProjectFolder)]
	public class Tilemap3DStats : ScriptableSingleton<Tilemap3DStats>
	{
		[SerializeField] private int m_DrawTileCount;
		[SerializeField] private int m_ClearTileCount;
		[SerializeField] private int m_RotateTileCount;
		[SerializeField] private int m_FlipTileCount;
		[SerializeField] private int m_TilemapCreatedCount;

		public int DrawTileCount
		{
			get => m_DrawTileCount;
			set => m_DrawTileCount = value;
		}
		public int ClearTileCount
		{
			get => m_ClearTileCount;
			set => m_ClearTileCount = value;
		}
		public int RotateTileCount
		{
			get => m_RotateTileCount;
			set => m_RotateTileCount = value;
		}
		public int FlipTileCount
		{
			get => m_FlipTileCount;
			set => m_FlipTileCount = value;
		}
		public int TilemapCreatedCount { get { return m_TilemapCreatedCount; } set { m_TilemapCreatedCount = value; Save(true); } }

		private void OnEnable() => Save(true);

		private void OnDisable() => Save(true);
	}
}
