// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler3.Runtime.Rendering
{
	public partial class Tilemap3DRenderer
	{
		private Boolean m_LastEnableDebugDrawing;
		private Boolean m_EnableDebugDrawing;
		public Boolean EnableDebugDrawing
		{
			get => m_EnableDebugDrawing;
			set
			{
				m_EnableDebugDrawing = value;
				UpdateDebugDrawingFlag();
			}
		}

		private void UpdateDebugDrawingFlag()
		{
			if (m_EnableDebugDrawing != m_LastEnableDebugDrawing)
			{
				m_LastEnableDebugDrawing = m_EnableDebugDrawing;

				if (m_ActiveRenderersFolder != null)
					SetEnableDebugDrawingOnRenderersInFolder(m_ActiveRenderersFolder, m_EnableDebugDrawing);
				if (m_PooledRenderersFolder != null)
					SetEnableDebugDrawingOnRenderersInFolder(m_PooledRenderersFolder, m_EnableDebugDrawing);
			}
		}

		private void SetEnableDebugDrawingOnRenderersInFolder(Transform folder, Boolean enableDebugDrawing)
		{
			foreach (Transform child in folder)
			{
				var renderer = child.GetComponent<Tile3DRenderer>();
				if (renderer != null)
					renderer.EnableDebugDrawing = enableDebugDrawing;
			}
		}
	}
}
