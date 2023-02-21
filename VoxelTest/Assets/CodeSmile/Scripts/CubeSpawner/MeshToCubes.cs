// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeSmile.VoxelTest
{
	[RequireComponent(typeof(Rigidbody))]
	public class MeshToCubes : MonoBehaviour
	{
		[SerializeField] private Mesh m_Mesh;
		[SerializeField] private GameObject m_ObjectToDisable;
		[SerializeField] private Transform m_CubeParent;

		[SerializeField] private GameObject m_CubePrefab;
		[SerializeField] private float m_DistributionScale;

		[SerializeField] private float3 m_ParticleArea = new float3(7,24,1);

		[Header("Physics Tweaking")]
		[SerializeField] private float m_GibScaleVariation = .02f;
		[SerializeField] private float m_IncomingVelocityFactor = 1f;
		[SerializeField] private float m_DisperseVelocityFactor = 1f;
		[SerializeField] private ForceMode m_ImpactForceMode;

		private void Start() => CreateCubesFromMesh();

		private void OnCollisionEnter(Collision collision) => TurnToCubes(collision.relativeVelocity);
		private void OnTriggerEnter(Collider collider) => TurnToCubes(Vector3.zero);

		private void CreateCubesFromMesh()
		{
			m_CubeParent?.gameObject.SetActive(false);

			/*
			var meshDataArray = Mesh.AcquireReadOnlyMeshData(m_Mesh);
			var meshData = meshDataArray[0];
			var vertices = new NativeArray<Vector3>(meshData.vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			meshData.GetVertices(vertices);
			*/

			var size = m_ParticleArea;
			for (var x = 0; x < size.x; x++)
			{
				for (var y = 0; y < size.y; y++)
				{
					for (var z = 0; z < size.z; z++)
					{
						var cube = Instantiate(m_CubePrefab, m_CubeParent);
						cube.transform.localPosition = new float3(x * m_DistributionScale, y * m_DistributionScale, 0f);
						cube.transform.localScale = cube.transform.localScale * (Random.value * m_GibScaleVariation);
						//cube.transform.parent = m_CubeParent;
					}
				}
			}
		}

		private void TurnToCubes(Vector3 velocity)
		{
			m_ObjectToDisable?.SetActive(false);
			m_CubeParent?.gameObject.SetActive(true);

			foreach (Transform cube in m_CubeParent)
			{
				var body = cube.GetComponent<Rigidbody>();
				body.AddForce(Vector3.down * m_IncomingVelocityFactor + Random.onUnitSphere * m_DisperseVelocityFactor, m_ImpactForceMode);
			}
			
			Destroy(GetComponent<CapsuleCollider>());
			Destroy(GetComponent<MeshToCubes>());
			Destroy(GetComponent<Rigidbody>());
		}
	}
}