using System.Collections.Generic;
using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	/// <summary>
	/// Use this class to draw 2D & 3D Gizmos more efficiently
	/// </summary>
	public static class ToolbeltGizmos
	{
		public enum WireframeMode { Wireframe, Mesh, OutlinedMesh }

		private static Dictionary<string, Mesh> _primitives = new Dictionary<string, Mesh>();

		#region CIRCLE

		public static void DrawCircle(Vector3 position, float radius, WireframeMode displayMode, Color color, int segments = 30)
        {
			DrawCircle(position, Quaternion.identity, radius, displayMode, color, segments);
        }

		public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, WireframeMode displayMode, Color color, int segments=30)
        {
#if UNITY_EDITOR

			Color lastGizmoColor = Gizmos.color;
			Gizmos.color = color;

			if (displayMode != WireframeMode.Wireframe)
            {
				Draw2DPrimitive(PrimitiveType.Sphere, position, rotation, Vector2.one * radius);
            }

			if (displayMode == WireframeMode.Mesh) return;

			if (segments < 3)
            {
				Debug.LogWarning("Circle Segments have to be more than 3");
				return;
			}

			Gizmos.color = THelper.SnapColorToOpaque(color);

			Vector3 lastPoint = position + THelper.RotateVector(THelper.GetUnitCirclePoint(segments - 1, segments) * radius, rotation);

			for (int i = 0; i < segments; i++)
            {
				Vector3 nextPoint = position + THelper.RotateVector(THelper.GetUnitCirclePoint(i, segments) * radius, rotation);
				Gizmos.DrawLine(lastPoint, nextPoint);
				lastPoint = nextPoint;
			}

			Gizmos.color = lastGizmoColor;
#endif
		}

        #endregion

        #region RECT

        public static void DrawRect(Vector3 position, Vector2 size, WireframeMode displayMode, Color color)
        {
			DrawRect(position, Quaternion.identity, size, displayMode, color);
        }

		public static void DrawRect(Vector3 position, Quaternion rotation, Vector2 size, WireframeMode displayMode, Color color)
        {
#if UNITY_EDITOR

			Color lastGizmoColor = Gizmos.color;
			Gizmos.color = color;

            switch (displayMode)
            {
                case WireframeMode.Wireframe:
					Gizmos.color = THelper.SnapColorToOpaque(color);
					DrawWireRect(position, rotation, size);
                    break;
                case WireframeMode.Mesh:
                    Draw2DPrimitive(PrimitiveType.Cube, position, rotation, size);
                    break;
                case WireframeMode.OutlinedMesh:
					Draw2DPrimitive(PrimitiveType.Cube, position, rotation, size);
					Gizmos.color = THelper.SnapColorToOpaque(color);
					DrawWireRect(position, rotation, size);
					break;
                default:
                    break;
            }

            Gizmos.color = lastGizmoColor;
#endif
		}

		private static void DrawWireRect(Vector3 position, Quaternion rotation, Vector2 size)
        {
			Vector2 halfSize = size * 0.5f;

			Vector3 topLeft = position + THelper.RotateVector(new Vector3(-halfSize.x, halfSize.y), rotation);
			Vector3 topRight = position + THelper.RotateVector(new Vector3(halfSize.x, halfSize.y), rotation);
			Vector3 bottomLeft = position + THelper.RotateVector(new Vector3(-halfSize.x, -halfSize.y), rotation);
			Vector3 bottomRight = position + THelper.RotateVector(new Vector3(halfSize.x, -halfSize.y), rotation);

			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
			Gizmos.DrawLine(bottomLeft, topLeft);
		}

        #endregion

		public static void DrawSphere(Vector3 position, float radius, WireframeMode displayMode, Color color)
        {
#if UNITY_EDITOR

			Color lastGizmoColor = Gizmos.color;
			Gizmos.color = color;

			switch (displayMode)
			{
				case WireframeMode.Wireframe:
					Gizmos.color = THelper.SnapColorToOpaque(color);
					Gizmos.DrawWireSphere(position, radius);
					break;
				case WireframeMode.Mesh:
					Gizmos.DrawSphere(position, radius);
					break;
				case WireframeMode.OutlinedMesh:
					Gizmos.DrawSphere(position, radius);
					Gizmos.color = THelper.SnapColorToOpaque(color);
					Gizmos.DrawWireSphere(position, radius);
					break;
			}

			Gizmos.color = lastGizmoColor;
#endif
		}

        #region BOX

        public static void DrawBox(Vector3 position, Vector3 size, WireframeMode displayMode, Color color)
        {
			DrawBox(position, Quaternion.identity, size, displayMode, color);
        }
		
		public static void DrawBox(Vector3 position, Quaternion rotation, Vector3 size, WireframeMode displayMode, Color color)
        {
#if UNITY_EDITOR

			Color lastGizmoColor = Gizmos.color;
			Gizmos.color = color;

			switch (displayMode)
            {
                case WireframeMode.Wireframe:
					Gizmos.color = THelper.SnapColorToOpaque(color);
					DrawWireBox(position, rotation, size);
					break;
                case WireframeMode.Mesh:
					Draw3DPrimitive(PrimitiveType.Cube, position, rotation, size);
					break;
                case WireframeMode.OutlinedMesh:
					Draw3DPrimitive(PrimitiveType.Cube, position, rotation, size);
					Gizmos.color = THelper.SnapColorToOpaque(color);
					DrawWireBox(position, rotation, size);
					break;
            }

			Gizmos.color = lastGizmoColor;
#endif
		}

        private static void DrawWireBox(Vector3 position, Quaternion rotation, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;

			Vector3 rectFront = position + THelper.RotateVector(new Vector3(0, 0, halfSize.z), rotation);
			Vector3 rectBack = position + THelper.RotateVector(new Vector3(0, 0, -halfSize.z), rotation);

			Vector3 topLeftFront = position + THelper.RotateVector(new Vector3(-halfSize.x, halfSize.y, halfSize.z), rotation);
            Vector3 topRightFront = position + THelper.RotateVector(new Vector3(halfSize.x, halfSize.y, halfSize.z), rotation);
            Vector3 bottomLeftFront = position + THelper.RotateVector(new Vector3(-halfSize.x, -halfSize.y, halfSize.z), rotation);
            Vector3 bottomRightFront = position + THelper.RotateVector(new Vector3(halfSize.x, -halfSize.y, halfSize.z), rotation);

			Vector3 topLeftBack = position + THelper.RotateVector(new Vector3(-halfSize.x, halfSize.y, -halfSize.z), rotation);
			Vector3 topRightBack = position + THelper.RotateVector(new Vector3(halfSize.x, halfSize.y, -halfSize.z), rotation);
			Vector3 bottomLeftBack = position + THelper.RotateVector(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), rotation);
			Vector3 bottomRightBack = position + THelper.RotateVector(new Vector3(halfSize.x, -halfSize.y, -halfSize.z), rotation);

			DrawWireRect(rectFront, rotation, size);
			DrawWireRect(rectBack, rotation, size);

			Gizmos.DrawLine(topLeftBack, topLeftFront);
			Gizmos.DrawLine(topRightBack, topRightFront);
			Gizmos.DrawLine(bottomLeftBack, bottomLeftFront);
			Gizmos.DrawLine(bottomRightBack, bottomRightFront);
		}

        #endregion

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
		{
#if UNITY_EDITOR
			Color lastGizmoColor = Gizmos.color;
			Gizmos.color = THelper.SnapColorToOpaque(color);

			Gizmos.DrawLine(from, to);

			Gizmos.color = lastGizmoColor;
#endif
		}


        #region PRIVATE_HELPERS
#if UNITY_EDITOR
        private static void Draw3DPrimitive(PrimitiveType primitive, Vector3 position, Quaternion rotation, Vector3 size)
        {
			Mesh m = GetPrimitive(primitive);
			Gizmos.DrawMesh(m, position, rotation, size);
		}

		private static void Draw2DPrimitive(PrimitiveType primitive, Vector3 position, Quaternion rotation, Vector2 size)
        {
			Vector3 squashedSize = new Vector3(size.x, size.y, 0.001f);
			Draw3DPrimitive(primitive, position, rotation, squashedSize);
        }

		private static Mesh GetPrimitive(PrimitiveType primitive)
        {
			string key = primitive.ToString();

			if (!_primitives.ContainsKey(key))
			{
				Mesh m = Resources.GetBuiltinResource<Mesh>($"{primitive}.fbx");
				_primitives.Add(key, m);
			}

			if (_primitives.ContainsKey(key)) return _primitives[key];

			return null;
        }
#endif
        #endregion
    }
}