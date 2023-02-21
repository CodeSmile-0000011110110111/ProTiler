using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace CodeSmile.VoxelTest
{
	public class CubeSpawner : MonoBehaviour
	{
		[SerializeField] private Transform m_CubesParent;
		[SerializeField] private GameObject[] m_Prefabs;
		[SerializeField] private int3 m_CuboidSize;
		[SerializeField] private float m_CubeScale = 1f;

		[Header("Motion")]
		[SerializeField] private float3 m_InitialForce;
		[SerializeField] private ForceMode m_ForceMode;
		[SerializeField] private bool m_ApplyRelativeForce;
		[SerializeField] private float m_ForceDuration;

		[Header("Info")]
		[SerializeField] private int m_CubeCount;

		[Header("Buttons :)")]
		[SerializeField] private bool m_Update;

		private Random m_Random;
		private int m_UpdateCount;

		private void Update()
		{
			m_UpdateCount++;
			if (m_UpdateCount < m_ForceDuration)
			{
				foreach (Transform cube in m_CubesParent)
				{
					var body = cube.GetComponent<Rigidbody>();
					if (m_ApplyRelativeForce)
						body.AddRelativeForce(m_InitialForce, m_ForceMode);
					else
						body.AddForce(m_InitialForce, m_ForceMode);
				}
			}
		}

		private void OnEnable() => StartCoroutine(CreateCubesAfterDelay());

		private void OnValidate()
		{
			UpdateCubeCount();

			if (m_Update)
			{
				m_Update = false;
				StartCoroutine(CreateCubesAfterDelay());
			}
		}

		private void UpdateCubeCount() => m_CubeCount = m_CuboidSize.x * m_CuboidSize.y * m_CuboidSize.z;

		private IEnumerator CreateCubesAfterDelay()
		{
			yield return null;

			DeleteCubes();
			CreateCubes();
		}

		private void DeleteCubes()
		{
			while (m_CubesParent.childCount > 0)
			{
				foreach (Transform cube in m_CubesParent)
					DestroyImmediate(cube.gameObject);
			}
		}

		private void CreateCubes()
		{
			m_Random = Random.CreateFromIndex((uint)DateTime.Now.Millisecond);

			for (var x = 0; x < m_CuboidSize.x; x++)
			{
				for (var y = 0; y < m_CuboidSize.y; y++)
				{
					for (var z = 0; z < m_CuboidSize.z; z++)
					{
						var cube = Instantiate(GetRandomPrefab(), m_CubesParent);
						cube.transform.position = new float3(x, y, z);
						cube.transform.localScale = new float3(m_CubeScale);
					}
				}
			}
		}

		private GameObject GetRandomPrefab() => m_Prefabs[m_Random.NextInt(0, m_Prefabs.Length)];
	}
}