// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tilemap;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	[FullCovered]
	public static class Tilemap3DCreation
	{
		private const string RectangularTilemapMenuText = "Rectangular";
		private const string IsometricTilemapMenuText = "Isometric (same as Rectangular)";
		private const string HexagonalFlatTopTilemapMenuText = "Hexagonal - Flat Top";
		private const string HexagonalPointTopTilemapMenuText = "Hexagonal - Pointed Top";

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Menus.TilemapMenuText + "/" + RectangularTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 0)]
		public static Tilemap3DBehaviour CreateRectangularTilemap3D() => CreateTilemap3D(CellLayout.Rectangular);

		/*
		[MenuItem("GameObject/" + Names.TileEditor + "/" + Menus.TilemapMenuText + "/" + HexagonalFlatTopTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 1)]
		public static Tilemap3D CreateHexagonalFlatTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalFlatTopTilemap3D));

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Menus.TilemapMenuText + "/" + HexagonalPointTopTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 2)]
		public static Tilemap3D CreateHexagonalPointTopTilemap3D() => throw new NotImplementedException(nameof(CreateHexagonalPointTopTilemap3D));
		*/

		private static Tilemap3DBehaviour CreateTilemap3D(CellLayout cellLayout)
		{
			Undo.SetCurrentGroupName("Create 3D Tilemap");

			var root = FindOrCreateRootGrid3D();
			var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap3D");
			var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap3DBehaviour), typeof(Tilemap3DRendererBehaviour));
			Undo.RegisterCreatedObjectUndo(tilemapGO, "Create Tilemap");
			Undo.SetTransformParent(tilemapGO.transform, root.transform, "");
			tilemapGO.transform.position = Vector3.zero;
			Selection.activeGameObject = tilemapGO;

			/*
			switch (cellLayout)
			{
				case CellLayout.Rectangular:
					break;
				default:
					throw new NotImplementedException(cellLayout.ToString());
			}
			*/

			Undo.IncrementCurrentGroup();

			return tilemapGO.GetComponent<Tilemap3DBehaviour>();
		}

		private static GameObject FindOrCreateRootGrid3D()
		{
			GameObject gridGO = null;

			var activeSelection = Selection.activeGameObject;
			if (activeSelection is GameObject)
			{
				// check for it being grid3d or parent being grid3d
				var parentGrid = activeSelection.GetComponentInParent<Grid3DBehaviour>();
				if (parentGrid != null)
					gridGO = parentGrid.gameObject;
			}

			if (gridGO == null)
			{
				gridGO = ObjectFactory.CreateGameObject("Grid3D", typeof(Grid3DBehaviour), typeof(Grid3DTileSetBehaviour));
				Undo.RegisterCreatedObjectUndo (gridGO, "Create 3D Grid");
			}

			return gridGO;
		}
	}
}
