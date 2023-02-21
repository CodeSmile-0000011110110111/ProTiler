using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class HitMeWithAWallOfCubes : MonoBehaviour
{
	[SerializeField] private GameObject m_CubePrefab;
	[SerializeField] private Transform m_CubesParent;
	[SerializeField] private float m_CubeSize;
	[SerializeField] private float2 m_WallSize;
	[SerializeField] private Vector3 m_CubeRaycastOffset;

	[SerializeField] private bool m_Update;
	[SerializeField] private bool m_Voxelize;

	private void OnValidate()
	{
		if (m_Update)
		{
			m_Update = false;
			StartCoroutine(HitMeAfterDelay());
		}
	}

	private IEnumerator HitMeAfterDelay()
	{
		yield return null;

		DeleteCubes();
		HitMe();
	}

	private void DeleteCubes()
	{
		while (m_CubesParent.childCount > 0)
		{
			foreach (Transform cube in m_CubesParent)
				DestroyImmediate(cube.gameObject);
		}
	}
	
	private void HitMe()
	{
		var size = new float2(m_WallSize.x / m_CubeSize, m_WallSize.y / m_CubeSize);
		for (var x = 0; x < size.x; x++)
		{
			for (var y = 0; y < size.y; y++)
			{
				var cube = Instantiate(m_CubePrefab, m_CubesParent);
				cube.transform.localScale = new float3(m_CubeSize);
				cube.transform.position = transform.position + m_CubeRaycastOffset + new Vector3(0f, y, x) * m_CubeSize;

				if (m_Voxelize)
				{
					var voxelize = cube.GetComponent<RaycastVoxelize>();
					voxelize.Voxelize();
				}
			}
		}
		
		for (var x = 0; x < size.x; x++)
		{
			for (var y = 0; y < size.y; y++)
			{
				var cube = Instantiate(m_CubePrefab, m_CubesParent);
				cube.transform.localScale = new float3(m_CubeSize);
				cube.transform.position = transform.position + m_CubeRaycastOffset + 
				                          new Vector3(m_CubeRaycastOffset.x * -2f, m_CubeRaycastOffset.y, m_CubeRaycastOffset.z * -.2f) +
				                          new Vector3(0f, y, x) * m_CubeSize;

				if (m_Voxelize)
				{
					var voxelize = cube.GetComponent<RaycastVoxelize>();
					voxelize.Voxelize(true);
				}
			}
		}
	}
}