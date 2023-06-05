// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler3.Runtime;
using CodeSmile.ProTiler3.Runtime.Controller;
using CodeSmile.ProTiler3.Runtime.Grid;
using CodeSmile.ProTiler3.Runtime.Rendering;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler3.Editor.Creation
{
	[FullCovered]
	public static class Tilemap3DCreation
	{
		private const String RectangularTilemapMenuText = "Rectangular";
		private const String IsometricTilemapMenuText = "Isometric (same as Rectangular)";
		private const String HexagonalFlatTopTilemapMenuText = "Hexagonal - Flat Top";
		private const String HexagonalPointTopTilemapMenuText = "Hexagonal - Pointed Top";

		private static readonly Type[] s_DefaultTilemapBehaviourTypes =
		{
			typeof(Tilemap3DModel),
			typeof(Tilemap3DRenderer),
			typeof(Tilemap3DDebug),
			typeof(Tilemap3DViewController),
			typeof(Tilemap3DViewControllerRuntime),
		};

		[MenuItem("GameObject/" + Names.TileEditor + "/" + Menus.TilemapMenuText + "/" + RectangularTilemapMenuText,
			priority = Menus.CreateGameObjectPriority + 0)]
		public static Tilemap3DModel CreateRectangularTilemap3D() =>
			CreateTilemap3D(CellLayout.Rectangular, s_DefaultTilemapBehaviourTypes);

		public static Tilemap3DModel CreateTilemap3D(CellLayout cellLayout, Type[] behaviourTypes)
		{
			Undo.SetCurrentGroupName("Create 3D Tilemap");
			var currentUndoGroup = Undo.GetCurrentGroup();

			var root = FindOrCreateRootGrid3D();
			var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap3D");
			var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, behaviourTypes);

			Undo.RegisterCreatedObjectUndo(tilemapGO, "Create Tilemap");
			Undo.SetTransformParent(tilemapGO.transform, root.transform, "");

			tilemapGO.transform.position = Vector3.zero;
			Selection.activeGameObject = tilemapGO;

			// TODO: switch on CellLayout

			Undo.CollapseUndoOperations(currentUndoGroup);
			Undo.IncrementCurrentGroup();

			return tilemapGO.GetComponent<Tilemap3DModel>();
		}

		private static GameObject FindOrCreateRootGrid3D()
		{
			GameObject gridGO = null;

			var activeSelection = Selection.activeGameObject;
			if (activeSelection is GameObject)
			{
				// check for it being grid3d or parent being grid3d
				var parentGrid = activeSelection.GetComponentInParent<Grid3DController>();
				if (parentGrid != null)
					gridGO = parentGrid.gameObject;
			}

			if (gridGO == null)
			{
				gridGO = ObjectFactory.CreateGameObject("Grid3D", typeof(Grid3DController),
					typeof(Tile3DAssetSet));
				Undo.RegisterCreatedObjectUndo(gridGO, "Create 3D Grid");
			}

			return gridGO;
		}
	}
}
