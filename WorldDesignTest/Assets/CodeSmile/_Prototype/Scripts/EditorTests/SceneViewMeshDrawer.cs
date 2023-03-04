// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.EditorTests
{
	[ExecuteInEditMode]
	public class SceneViewMeshDrawer : MonoBehaviour
	{
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private Mesh m_Mesh;
		[SerializeField] private Material[] m_Materials;
		[SerializeField] private Vector3 m_Distance;
		[SerializeField] private int m_HowMany;
		[SerializeField] private bool m_UpdateMesh;
		[SerializeField] private bool m_DrawInstanced;
		[SerializeField] private bool m_PerformFrustumCulling;
		[SerializeField] private bool m_PerformRangeCulling;
		[SerializeField] private float m_MaxRangeToCameraPos;

		[SerializeField] private List<GameObject> m_TilePrefabs;
		[SerializeField] private List<GameObject> m_Tiles;
		
		private Matrix4x4[] m_Matrices;

		private void OnRenderObject()
		{
			var camera = Camera.current;
			if (camera == null)
				return;

			var count = Mathf.CeilToInt(Mathf.Sqrt(m_HowMany));
			if (m_UpdateMesh && m_Prefab != null || m_Matrices == null || m_Matrices.Length == 0)
			{
				m_UpdateMesh = false;
				m_Mesh = m_Prefab.GetComponent<MeshFilter>().sharedMesh;
				m_Materials = m_Prefab.GetComponent<MeshRenderer>().sharedMaterials;
				m_Matrices = new Matrix4x4[count * count];
			}

			var camPos = camera.transform.position;
			var maxRangeSqr = m_MaxRangeToCameraPos * m_MaxRangeToCameraPos;
			var origin = transform.position;
			for (var x = 0; x < count; x++)
			{
				for (var y = 0; y < count; y++)
				{
					var position = new Vector3(x * m_Distance.x, m_Distance.y, y * m_Distance.z) + origin;
					var matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
					m_Matrices[y * count + x] = matrix;

					if (m_PerformRangeCulling)
					{
						var distanceToCameraSqr = (position - camPos).sqrMagnitude;
						if (distanceToCameraSqr > maxRangeSqr)
							continue;
					}

					if (m_PerformFrustumCulling)
					{
						var gridSize = 30f;
						var bounds = new Bounds(position, Vector3.one * gridSize);
						var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
						if (GeometryUtility.TestPlanesAABB(frustumPlanes, bounds) == false)
							continue;
					}

					if (m_DrawInstanced)
						continue;

					for (var i = 0; i < m_Materials.Length; i++)
					{
						m_Materials[i].SetPass(0);
						Graphics.DrawMeshNow(m_Mesh, matrix, i);
					}
				}
			}

			if (m_DrawInstanced)
				Graphics.DrawMeshInstanced(m_Mesh, 0, m_Materials[0], m_Matrices);
		}
	}

	[BurstCompile]
	internal struct FrustumCullingJob : IJobParallelFor
	{
		public float4x4 projectionMatrix;
		public float4x4 viewMatrix;
		public NativeArray<float3> vertices;
		public NativeArray<int> visibleIndices;

		public void Execute(int index)
		{
			var vertex = math.mul(viewMatrix, new float4(vertices[index], 1)).xyz;
			var clipPosition = math.mul(projectionMatrix, new float4(vertex, 1));
			clipPosition /= clipPosition.w;
			var isVisible = clipPosition.x >= -1 && clipPosition.x <= 1 &&
			                clipPosition.y >= -1 && clipPosition.y <= 1 &&
			                clipPosition.z >= -1 && clipPosition.z <= 1;
			if (isVisible)
				visibleIndices[index] = index;
			else
				visibleIndices[index] = -1;
		}
	}
}