// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using System;
using System.Diagnostics.Contracts;
using UnityEngine;
using WorldPos = UnityEngine.Vector3;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;
using Object = System.Object;

namespace CodeSmile.ProTiler.Controller
{
	[Serializable]
	public struct Grid3DCursor : IEquatable<Grid3DCursor>
	{
		private WorldPos m_Position;
		private GridCoord m_Coord;
		private CellSize m_CellSize;
		private Boolean m_IsValid;

		public WorldPos Position => m_Position;
		public GridCoord Coord => m_Coord;
		public CellSize CellSize => m_CellSize;
		public Boolean IsValid => m_IsValid;

		public Grid3DCursor(Ray worldRay, CellSize cellSize)
		{
			m_Position = default;
			m_Coord = default;
			m_CellSize = cellSize;
			m_IsValid = false;
			CalculateCursorCoord(worldRay);
		}

		private void CalculateCursorCoord(Ray worldRay)
		{
			if (IntersectsCoord(worldRay, out m_Coord, m_CellSize))
			{
				m_Position = Grid3DUtility.ToWorldPos(m_Coord, m_CellSize);
				m_IsValid = true;
			}
		}

		[Pure] private Boolean IntersectsCoord(Ray ray, out GridCoord coord, CellSize cellSize)
		{
			var intersects = ray.IntersectsPlane(out var point);
			coord = intersects ? Grid3DUtility.ToCoord(point, cellSize) : default;
			return intersects;
		}

		public Boolean Equals(Grid3DCursor other) => m_Position.Equals(other.m_Position) &&
		                                             m_CellSize.Equals(other.m_CellSize) &&
		                                             m_IsValid == other.m_IsValid;

		public override String ToString() => $"Grid3DCursor(coord: {m_Coord}, pos: {m_Position}, valid: {IsValid})";
		public override Boolean Equals(Object obj) => obj is Grid3DCursor other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(m_Position, m_CellSize, m_IsValid);
		public static Boolean operator ==(Grid3DCursor left, Grid3DCursor right) => left.Equals(right);
		public static Boolean operator !=(Grid3DCursor left, Grid3DCursor right) => !left.Equals(right);
	}
}
