// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;
using WorldPos = UnityEngine.Vector3;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;
using Object = System.Object;

namespace CodeSmile.ProTiler.Grid
{
	[Serializable]
	public struct Grid3DCursor : IEquatable<Grid3DCursor>
	{
		private WorldPos m_CenterPosition;
		private GridCoord m_Coord;
		private CellSize m_CellSize;
		private Boolean m_IsValid;

		public WorldPos CenterPosition => m_CenterPosition;
		public GridCoord Coord => m_Coord;
		public CellSize CellSize => m_CellSize;
		public Boolean IsValid => m_IsValid;
		[Pure] public static Boolean operator ==(Grid3DCursor left, Grid3DCursor right) => left.Equals(right);
		[Pure] public static Boolean operator !=(Grid3DCursor left, Grid3DCursor right) => !left.Equals(right);

		public Grid3DCursor(Ray worldRay, CellSize cellSize)
		{
			m_CenterPosition = default;
			m_Coord = default;
			m_CellSize = cellSize;
			m_IsValid = false;
			CalculateCursorCoord(worldRay);
		}

		[Pure] public Boolean Equals(Grid3DCursor other) => m_CenterPosition.Equals(other.m_CenterPosition) &&
		                                                    m_CellSize.Equals(other.m_CellSize) &&
		                                                    m_IsValid == other.m_IsValid;

		private void CalculateCursorCoord(Ray worldRay)
		{
			if (IntersectsPlaneAtCoord(worldRay, out m_Coord, m_CellSize))
			{
				m_CenterPosition = Grid3DUtility.ToWorldPos(m_Coord, m_CellSize);
				m_IsValid = true;
			}
		}

		[Pure] private Boolean IntersectsPlaneAtCoord(Ray ray, out GridCoord coord, CellSize cellSize)
		{
			var intersects = ray.IntersectsPlane(out var point);
			coord = intersects ? Grid3DUtility.ToCoord(point, cellSize) : default;
			return intersects;
		}

		[ExcludeFromCodeCoverage] public override String ToString() =>
			$"Grid3DCursor(coord: {m_Coord}, pos: {m_CenterPosition}, valid: {IsValid})";

		[Pure] public override Boolean Equals(Object obj) => obj is Grid3DCursor other && Equals(other);
		[Pure] public override Int32 GetHashCode() => HashCode.Combine(m_CenterPosition, m_CellSize, m_IsValid);
	}
}
