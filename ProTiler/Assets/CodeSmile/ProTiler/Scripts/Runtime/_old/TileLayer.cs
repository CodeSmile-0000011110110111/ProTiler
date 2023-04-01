// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler
{
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public sealed class TileLayer : MonoBehaviour
	{
		private static TileSet s_ExampleTileSet;

		[Header("Layer Settings")]
		[SerializeField] private LayerType m_LayerType = LayerType.Tile;
		[SerializeField] private TileSet m_TileSet;
		[HideInInspector] [SerializeField] private TileDataContainer m_TileDataContainer = new();

		public int TileCount => m_TileDataContainer.Count;

		public TileSet TileSet
		{
			get
			{
				if (m_TileSet == null)
					m_TileSet = ExampleTileSet;

				return m_TileSet;
			}
			set => m_TileSet = value;
		}
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public TileGrid Grid => TileSet.Grid;
		public LayerType LayerType { get => m_LayerType; set => m_LayerType = value; }

		private void Reset() => OnEnable(); // Editor will not call OnEnable when layer added during Reset

		private void OnEnable()
		{
			if (m_LayerType != LayerType.Tile)
				throw new Exception($"layer type {m_LayerType} not supported");
		}

		public override string ToString() => name;

		private static TileSet ExampleTileSet
		{
			get
			{
				if (s_ExampleTileSet == null || s_ExampleTileSet.IsMissing())
					s_ExampleTileSet = Resources.Load<TileSet>(Global.TileEditorResourceTileSetsPath + "ExampleTileSet");
				return s_ExampleTileSet;
			}
		}

		public IDictionary<GridCoord, TileData> GetTilesInRect(GridRect rect) => m_TileDataContainer.GetTilesInRect(rect);
		public TileData GetTileData(GridCoord coord) => m_TileDataContainer.GetTile(coord);
		public float3 GetTilePosition(GridCoord coord) => Grid.ToWorldPosition(coord) + TileSet.GetTileOffset() + (float3)transform.position;

		internal (IReadOnlyList<GridCoord>, IReadOnlyList<TileData>) DrawLine(GridCoord start, GridCoord end, TileBrush brush) =>
			m_TileDataContainer.SetTileIndexes(start.MakeLine(end), brush.TileSetIndex);

		internal (IReadOnlyList<GridCoord>, IReadOnlyList<TileData>) DrawRect(RectInt rect, TileBrush brush) =>
			m_TileDataContainer.SetTileIndexes(rect, brush.TileSetIndex);

		internal void ClearAllTiles() => m_TileDataContainer.ClearAllTiles();
		internal TileFlagsOld SetTileFlags(GridCoord coord, TileFlagsOld flags) => m_TileDataContainer.SetTileFlags(coord, flags);
		internal TileFlagsOld ClearTileFlags(GridCoord coord, TileFlagsOld flags) => m_TileDataContainer.ClearTileFlags(coord, flags);
		internal TileFlagsOld RotateTile(GridCoord coord, int delta) => m_TileDataContainer.RotateTile(coord, delta);
		internal TileFlagsOld FlipTile(GridCoord coord, int delta) => m_TileDataContainer.FlipTile(coord, delta);
	}
}
