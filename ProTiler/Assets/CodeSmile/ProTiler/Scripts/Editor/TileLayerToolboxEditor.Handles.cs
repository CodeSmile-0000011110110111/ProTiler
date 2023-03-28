// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.ProTiler.Extensions;
using CodeSmile.Extensions;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler
{
	public partial class TileLayerToolboxEditor
	{
		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint()
		{
			if (m_IsMouseInView && IsRightMouseButtonDown() == false)
				DrawCursorHandle(ProTilerState.instance.TileEditMode);
		}

		private void DrawCursorHandle(TileEditMode editMode)
		{
			if (editMode is TileEditMode.PenDraw or TileEditMode.RectFill)
			{
				var grid = Toolbox.Layer.Grid;
				var worldRect = TileGrid.ToWorldRect(m_SelectionRect, grid.Size);
				var worldPos = Toolbox.Layer.transform.position;
				var cubePos = worldRect.GetWorldCenter() + worldPos;
				var cubeSize = worldRect.GetWorldSize(grid.Size.y);

				var prevColor = Handles.color;
				Handles.color = Global.OutlineColor;
				Handles.DrawWireCube(cubePos, cubeSize);
				Handles.color = prevColor;

				//Handles.DrawAAPolyLine();

				// FIXME: get the preview objects from the toolbox instead
				var renderer = Toolbox.Layer.GetComponent<TileLayerPreviewRenderer>();
				var cursor = renderer.transform.Find("Cursor");
				if (cursor != null)
				{
					var meshRenderer = cursor.GetComponent<MeshRenderer>();
					if (meshRenderer == null && cursor.childCount > 0)
					{
						meshRenderer = cursor.GetChild(0).GetComponent<MeshRenderer>();
						if (meshRenderer == null && cursor.GetChild(0).childCount > 0)
							meshRenderer = cursor.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
					}

					if (meshRenderer != null)
						Handles.DrawOutline(new[] { meshRenderer.gameObject }, Global.OutlineColor);
				}
			}
		}
	}
}