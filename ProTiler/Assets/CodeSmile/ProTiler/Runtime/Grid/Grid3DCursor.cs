// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using WorldPos = Unity.Mathematics.float3;
using CellSize = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
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
		public static Boolean operator ==(Grid3DCursor left, Grid3DCursor right) => left.Equals(right);
		public static Boolean operator !=(Grid3DCursor left, Grid3DCursor right) => !left.Equals(right);

		public Grid3DCursor(Ray worldRay, CellSize cellSize)
		{
			m_CenterPosition = default;
			m_Coord = default;
			m_CellSize = cellSize;
			m_IsValid = false;
			CalculateCursorCoord(worldRay);
		}

		public Boolean Equals(Grid3DCursor other) => m_CenterPosition.Equals(other.m_CenterPosition) &&
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

		private Boolean IntersectsPlaneAtCoord(Ray ray, out GridCoord coord, CellSize cellSize)
		{
			var intersects = ray.IntersectsPlane(out var point);
			coord = intersects ? Grid3DUtility.ToGridCoord(point, cellSize) : default;
			return intersects;
		}

		[ExcludeFromCodeCoverage] public override String ToString() =>
			$"Grid3DCursor(coord: {m_Coord}, pos: {m_CenterPosition}, valid: {IsValid})";

		public override Boolean Equals(Object obj) => obj is Grid3DCursor other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(m_CenterPosition, m_CellSize, m_IsValid);
	}
}
