﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Rendering;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler
{
	public static class Tilemap3DCreation
	{
		private const string RectangularTilemapMenuText = "Rectangular";
		private const string IsometricTilemapMenuText = "Isometric (same as Rectangular)";
		private const string HexagonalFlatTopTilemapMenuText = "Hexagonal - Flat Top";
		private const string HexagonalPointTopTilemapMenuText = "Hexagonal - Pointed Top";

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Names.Tilemap3DMenu + "/" + RectangularTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 0)]
		public static Tilemap3D CreateRectangularTilemap3D() => CreateTilemap3D(Grid3D.Layout.Rectangular);

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Names.Tilemap3DMenu + "/" + HexagonalFlatTopTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 1)]
		public static Tilemap3D CreateHexagonalFlatTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalFlatTopTilemap3D));

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Names.Tilemap3DMenu + "/" + HexagonalPointTopTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 2)]
		public static Tilemap3D CreateHexagonalPointTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalPointTopTilemap3D));

		private static Tilemap3D CreateTilemap3D(Grid3D.Layout layout)
		{
			Tilemap3DStats.instance.TilemapCreatedCount++;

			var root = FindOrCreateRootGrid3D();
			var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap3D");
			var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap3D), typeof(Tilemap3DRenderer));
			Undo.SetTransformParent(tilemapGO.transform, root.transform, "");
			tilemapGO.transform.position = Vector3.zero;
			Selection.activeGameObject = tilemapGO;

			switch (layout)
			{
				case Grid3D.Layout.Rectangular:
					break;
				default:
					throw new NotImplementedException(layout.ToString());
			}

			Undo.SetCurrentGroupName("Create 3D Tilemap");

			return tilemapGO.GetComponent<Tilemap3D>();
		}

		private static GameObject FindOrCreateRootGrid3D()
		{
			GameObject gridGO = null;

			var active = Selection.activeGameObject;
			if (active is GameObject)
			{
				// check for it being grid3d or parent being grid3d
				var parentGrid = active.GetComponentInParent<Grid3D>();
				if (parentGrid != null)
					gridGO = parentGrid.gameObject;
			}

			if (gridGO == null)
			{
				gridGO = ObjectFactory.CreateGameObject("Grid3D", typeof(Grid3D));
				Undo.SetCurrentGroupName("Create 3D Grid");
			}

			return gridGO;
		}
	}
}
