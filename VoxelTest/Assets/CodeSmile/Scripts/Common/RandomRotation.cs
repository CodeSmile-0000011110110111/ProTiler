using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotation : MonoBehaviour
{
	[SerializeField] private float3 m_Rotation;

	private void OnEnable() => transform.rotation = Quaternion.Euler(new Vector3(Random.value * m_Rotation.x,
		Random.value * m_Rotation.y, Random.value * m_Rotation.z));
}