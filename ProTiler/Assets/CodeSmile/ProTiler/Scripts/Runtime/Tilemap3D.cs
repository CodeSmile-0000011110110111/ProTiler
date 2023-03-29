// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	public class Tilemap3D : MonoBehaviour
	{
		[SerializeField] private Vector3 m_TileAnchor;
		[Header("Info")]
		[SerializeField] [ReadOnlyField] private int m_TileCount;
		
		public Grid3D Grid => transform.parent.GetComponent<Grid3D>();
		
		public void RefreshTile(Vector3Int coord) => throw new NotImplementedException();
		public void DrawLine(Vector3Int startSelectionCoord, Vector3Int cursorCoord) { throw new NotImplementedException(); }
		public void DrawRect(object makeRect) { throw new NotImplementedException(); }
	}
}