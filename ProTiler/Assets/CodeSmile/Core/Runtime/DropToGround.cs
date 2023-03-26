// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile
{
	[ExecuteInEditMode]
	public class DropToGround : MonoBehaviour
	{
		[SerializeField] private bool m_SnapPosition;
		[SerializeField] private Vector3 m_PositionOffset;
		[SerializeField] private bool m_SnapPositionToBounds;
		[SerializeField] private bool m_SnapRotation;
		//[SerializeField] private bool m_SnapCollider;

		private Vector3 m_EditorPosition;
		private Quaternion m_EditorRotation;

		private void Update()
		{
			var offset = 1000f;
			var rayOrigin = transform.position + Vector3.up * offset;

			var hits = Physics.RaycastAll(rayOrigin, Vector3.down, offset * 2f, Physics.AllLayers);
			foreach (var hit in hits)
			{
				if (hit.transform == transform)
					continue;

				var posOffset = Vector3.zero;
				if (m_SnapPositionToBounds)
				{
					var collider = GetComponent<Collider>();
					if (collider != null)
						posOffset.y = collider.bounds.size.y / 2f;
				}

				if (m_SnapPosition)
					transform.position = hit.point + posOffset + m_PositionOffset;
				if (m_SnapRotation)
				{
					var rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					var angles = rotation.eulerAngles;
					angles.y = m_EditorRotation.y;
					transform.rotation = Quaternion.Euler(angles);
				}

				break;
			}
		}

		private void OnEnable()
		{
			m_EditorPosition = transform.position;
			m_EditorRotation = transform.rotation;
		}
	}
}