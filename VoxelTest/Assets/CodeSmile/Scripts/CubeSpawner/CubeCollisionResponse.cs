using UnityEngine;

namespace CodeSmile.VoxelTest
{
	[RequireComponent(typeof(Rigidbody))]
	public class CubeCollisionResponse : MonoBehaviour
	{
		[SerializeField] private float m_ForceFactor = 1.0f;
		private Rigidbody m_Rigidbody;

		private bool m_ApplySpreadForce = true;

		private void OnEnable() => m_Rigidbody = GetComponent<Rigidbody>();

		private void OnCollisionExit(Collision other)
		{
			if (m_ApplySpreadForce)
			{
				m_ApplySpreadForce = false;
				
				var velocity = m_Rigidbody.velocity;
				m_Rigidbody.AddForce(velocity + Random.insideUnitSphere * m_ForceFactor, ForceMode.VelocityChange);
			}
		}
	}
}