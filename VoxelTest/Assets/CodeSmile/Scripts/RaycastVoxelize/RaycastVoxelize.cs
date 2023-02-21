using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class RaycastVoxelize : MonoBehaviour
{
	[SerializeField] private float3 m_RaycastDirection = new(-1f, 0f, 0f);

	public void Voxelize(bool inverseDirection = false)
	{

			var origin = transform.position;
			var direction = m_RaycastDirection;
			if (inverseDirection)
				direction = m_RaycastDirection * -1f;
			var didHit = Physics.Raycast(origin, direction, out var hitInfo, 100f);

			if (didHit)
			{
				//Debug.Log($"Hit at {hitInfo.point}");
				transform.position = hitInfo.point;
				var sphereCollider = GetComponent<SphereCollider>();
				if (sphereCollider)
					sphereCollider.enabled = false;
				var boxCollider = GetComponent<BoxCollider>();
				if (boxCollider)
					boxCollider.enabled = false;
			}
			else
			{
				//Debug.Log($"I missed ...");
				DestroyImmediate(gameObject);
			}
	}
}