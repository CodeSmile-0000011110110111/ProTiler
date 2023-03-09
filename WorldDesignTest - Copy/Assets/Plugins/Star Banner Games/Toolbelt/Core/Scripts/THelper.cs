using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBG.Toolbelt
{
	public delegate bool ValidateAction<T>(T item);

	/// <summary>
	/// Various Helper Functions to save time
	/// </summary>
	public static class THelper
	{
		public const float TAU = 6.28318530718f;

		#region FLIP_01

		/// <summary>
		/// Takes in either 0 or 1 and returns the opposite.
		/// </summary>
		/// <param name="value">Either 0 or 1. Other inputs will return nonsense.</param>
		public static int Flip01(int value)
        {
			return 1 - value;
        }

		/// <summary>
		/// Takes in either 0 or 1 and returns the opposite.
		/// </summary>
		/// <param name="value">Either 0 or 1. Other inputs will return nonsense.</param>
		public static float Flip01(float value)
		{
			return 1 - value;
		}

		#endregion

		#region ROTATIONS

		/// <summary>
		/// On a unit circle that is made of n equally spaced points,
		/// this function returns the point at the specified index
		/// </summary>
		/// <param name="totalCirclePoints">Needs to be 3 or more to return a valid result</param>
		public static Vector3 GetUnitCirclePoint(int pointIndex, int totalCirclePoints)
		{
			float t = pointIndex / (float)totalCirclePoints;
			float angleInRadians = t * TAU;
			return GetUnitVectorByAngle(angleInRadians);
		}

		/// <summary>
		/// Get a point on the Unit Circle using its angle
		/// </summary>
		public static Vector2 GetUnitVectorByAngle(float angleInRadians)
		{
			return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
		}

		public static Vector3 RotateVector(Vector3 vector, Quaternion rotation)
        {
			return rotation * vector;
        }

		public static Vector3 RotateVector(Vector3 vector, Vector3 eulerRotation)
		{
			return Quaternion.Euler(eulerRotation) * vector;
		}

		public static Quaternion AddRotations(Quaternion quaternionA, Quaternion quaternionB)
        {
			return quaternionA * quaternionB;
        }

		public static Quaternion AddRotations(Vector3 eulerA, Vector3 eulerB)
		{
			return Quaternion.Euler(eulerA) * Quaternion.Euler(eulerB);
		}

		public static Quaternion AddRotations(Quaternion quaternionA, Vector3 eulerB)
        {
			return quaternionA * Quaternion.Euler(eulerB);
		}

        #endregion

        #region VECTORS

        /// <summary>
        /// Adds a float to every dimension of the vector
        /// </summary>
        public static Vector2 AddToVector(Vector2 a, float b) => new Vector2(a.x + b, a.y + b);
		/// <summary>
		/// Adds a float to every dimension of the vector
		/// </summary>
		public static Vector3 AddToVector(Vector3 a, float b) => new Vector3(a.x + b, a.y + b, a.z + b);
		/// <summary>
		/// Subtracts a float from every dimension of the vector
		/// </summary>
		public static Vector2 SubFromVector(Vector2 a, float b) => AddToVector(a, -b);
		/// <summary>
		/// Subtracts a float from every dimension of the vector
		/// </summary>
		public static Vector3 SubFromVector(Vector3 a, float b) => AddToVector(a, -b);

        #endregion

        #region MISC

        public static bool IsLayerInLayermask(int layer, LayerMask mask)
		{
			return ((mask.value & (1 << layer)) > 0);
		}

		/// <summary>
		/// Returns the color with an alpha of 1
		/// </summary>
		/// <param name="hideIfZero">If true, an alpha of 0 will stay at 0</param>
		public static Color SnapColorToOpaque(Color color, bool hideIfZero = false)
		{
			if (!hideIfZero) return new Color(color.r, color.g, color.b, 1);

			float alpha = color.a > 0 ? 1 : 0;
			return new Color(color.r, color.g, color.b, alpha);
		}


		/// <summary>
		/// This function takes in a collection and removes all items that fullfill the condition
		/// you define in the ValidateAction. 
		/// </summary>
		public static IEnumerable<T> RemoveCollectionElements<T>(IEnumerable<T> collection, ValidateAction<T> removeCondition)
        {
			List<T> list = collection.ToList();

			Stack<int> removeQueue = new Stack<int>();

            for (int i = 0; i < list.Count; i++)
            {
				if (removeCondition.Invoke(list[i]))
                {
					removeQueue.Push(i);
                }
            }

            while (removeQueue.Count > 0)
            {
				list.RemoveAt(removeQueue.Pop());
            }

			return list;
        }

        #endregion
    }
}