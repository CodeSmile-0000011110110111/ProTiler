// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor
{
	internal static class Tilemap3DCreation
	{
		private const string RectangularTilemapMenuText = "Rectangular";
		private const string IsometricTilemapMenuText = "Isometric (same as Rectangular)";
		private const string HexagonalFlatTopTilemapMenuText = "Hexagonal - Flat Top";
		private const string HexagonalPointTopTilemapMenuText = "Hexagonal - Pointed Top";

		[MenuItem("GameObject/" + Global.TileEditorName + "/" + Global.Tilemap3DMenuName + "/" + RectangularTilemapMenuText,
			priority = Global.CreateGameObjectMenuPriority + 0)]
		internal static void CreateRectangularTilemap3D() => CreateTilemap3D(Grid3DLayout.Rectangular);

		[MenuItem("GameObject/" + Global.TileEditorName + "/" + Global.Tilemap3DMenuName + "/" + IsometricTilemapMenuText,
			priority = Global.CreateGameObjectMenuPriority + 3)]
		internal static void CreateIsometricTilemap3D()
		{
			Debug.LogWarning("Creating a rectangular 3D tilemap because 'isometric' is just a matter of camera perspective in 3D.");
			CreateRectangularTilemap3D();
		}

		[MenuItem("GameObject/" + Global.TileEditorName + "/" + Global.Tilemap3DMenuName + "/" + HexagonalFlatTopTilemapMenuText,
			priority = Global.CreateGameObjectMenuPriority + 1)]
		internal static void CreateHexagonalFlatTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalFlatTopTilemap3D));

		[MenuItem("GameObject/" + Global.TileEditorName + "/" + Global.Tilemap3DMenuName + "/" + HexagonalPointTopTilemapMenuText,
			priority = Global.CreateGameObjectMenuPriority + 2)]
		internal static void CreateHexagonalPointTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalPointTopTilemap3D));

		internal static void CreateTilemap3D(Grid3DLayout layout)
		{
			var root = FindOrCreateRootGrid3D();
			var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap3D");
			var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap3D), typeof(Tilemap3DRenderer));
			Undo.SetTransformParent(tilemapGO.transform, root.transform, "");
			tilemapGO.transform.position = Vector3.zero;
			Selection.activeGameObject = tilemapGO;

			switch (layout)
			{
				case Grid3DLayout.Rectangular:
					break;
				default:
					throw new NotImplementedException(layout.ToString());
			}

			Undo.SetCurrentGroupName("Create 3D Tilemap");
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