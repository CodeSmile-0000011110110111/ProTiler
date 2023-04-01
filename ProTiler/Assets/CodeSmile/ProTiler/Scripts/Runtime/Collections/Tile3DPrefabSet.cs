// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
{
	/// <summary>
	///     Contains list of Tile3D used on the tilemap. Tile3DData indexes into this set to get the prefabs while only having
	///     to
	///     store an int per tile rather than (FileId+GUID+Type) in the serialized scene.
	/// </summary>
	[Serializable]
	public sealed class Tile3DPrefabSet
	{
		private static GameObject s_MissingTilePrefab;

		public const int InvalidIndex = 0;
		private List<GameObject> m_Prefabs;


		public Tile3DPrefabSet()
		{
			m_Prefabs = new();
			m_Prefabs.Add(MissingTilePrefab);
		}

		public static GameObject MissingTilePrefab
		{
			get
			{
				if (s_MissingTilePrefab == null || s_MissingTilePrefab.IsMissing())
					s_MissingTilePrefab = Resources.Load<GameObject>(Global.TileEditorResourcePrefabsPath + "MissingTile");
				return s_MissingTilePrefab;
			}
		}
		public GameObject this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Prefabs.Count)
					return null;

				return m_Prefabs[index];
			}
		}
		public int Count => m_Prefabs.Count;

		public int TryAddPrefab(GameObject prefab)
		{
			if (prefab == null)
				return InvalidIndex;

			var existingIndex = m_Prefabs.FindIndex(p => p.GetInstanceID() == prefab.GetInstanceID());
			if (existingIndex >= 0)
				return existingIndex;

			m_Prefabs.Add(prefab);
			return m_Prefabs.Count - 1;
		}
	}
}
