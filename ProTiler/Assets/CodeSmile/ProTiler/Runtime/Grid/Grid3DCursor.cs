// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using System;
using System.Diagnostics.Contracts;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.ProTiler.Controller
{
	public struct Grid3DCursor : IEquatable<Grid3DCursor>
	{
		private Vector3 m_Position;
		private readonly Vector3 m_CellSize;
		private Boolean m_IsValid;

		public Vector3 Position => m_Position;
		public Vector3Int Coord => Grid3DUtility.ToCoord(m_Position, m_CellSize);
		public Boolean IsValid => m_IsValid;

		public Grid3DCursor(Ray currentWorldRay, Ray lastWorldRay, Vector3 cellSize)
		{
			m_Position = default;
			m_IsValid = false;
			m_CellSize = cellSize;
			CalculateCursorCoord(currentWorldRay, lastWorldRay);
		}

		private void CalculateCursorCoord(Ray currentWorldRay, Ray lastWorldRay)
		{
			m_IsValid = IntersectsCoord(currentWorldRay, out var currentCoord, m_CellSize);
			if (IsValid)
			{
				var isLastCoordValid = IntersectsCoord(lastWorldRay, out var lastCoord, m_CellSize);
				if (isLastCoordValid == false || currentCoord != lastCoord)
				{
					m_Position = new Vector3(currentCoord.x * m_CellSize.x, currentCoord.y * m_CellSize.y,
						currentCoord.z * m_CellSize.z) + m_CellSize * .5f;
				}
			}
		}

		[Pure] private Boolean IntersectsCoord(Ray ray, out Vector3Int coord, Vector3 cellSize)
		{
			var intersects = ray.IntersectsPlane(out var point);
			coord = intersects ? Grid3DUtility.ToCoord(point, cellSize) : default;
			return intersects;
		}

		public Boolean Equals(Grid3DCursor other) => m_Position.Equals(other.m_Position) &&
		                                             m_CellSize.Equals(other.m_CellSize) &&
		                                             m_IsValid == other.m_IsValid;

		public override Boolean Equals(Object obj) => obj is Grid3DCursor other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(m_Position, m_CellSize, m_IsValid);
		public static Boolean operator ==(Grid3DCursor left, Grid3DCursor right) => left.Equals(right);
		public static Boolean operator !=(Grid3DCursor left, Grid3DCursor right) => !left.Equals(right);
	}
}