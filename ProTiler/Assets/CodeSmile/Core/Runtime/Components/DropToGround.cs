// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Components
{
	[ExcludeFromCodeCoverage]
	[ExecuteInEditMode]
	public class DropToGround : MonoBehaviour
	{
		private const float RaycastDistance = 1000f;
		[SerializeField] private bool m_SnapPosition;
		[SerializeField] private Vector3 m_PositionOffset;
		[SerializeField] private bool m_SnapPositionToBounds;
		[SerializeField] private bool m_SnapRotation;
		//[SerializeField] private bool m_SnapCollider;

		private Vector3 m_EditorPosition;
		private Quaternion m_EditorRotation;

		[ExcludeFromCodeCoverage]
		private void Update() => DoDropToGround();

		[ExcludeFromCodeCoverage]
		private void OnEnable()
		{
			m_EditorPosition = transform.position;
			m_EditorRotation = transform.rotation;
		}

		[ExcludeFromCodeCoverage]
		private void DoDropToGround()
		{
			var rayOrigin = transform.position + Vector3.up * RaycastDistance;
			var hits = Physics.RaycastAll(rayOrigin, Vector3.down, RaycastDistance * 2f, Physics.AllLayers);
			foreach (var hit in hits)
			{
				if (IsSelf(hit))
					continue;

				var posOffset = SnapPositionToBounds(Vector3.zero);
				SnapPosition(hit, posOffset);
				SnapRotation(hit);
				break;
			}
		}

		private bool IsSelf(RaycastHit hit) => hit.transform == transform;

		private void SnapRotation(RaycastHit hit)
		{
			if (m_SnapRotation)
			{
				var rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				var angles = rotation.eulerAngles;
				angles.y = m_EditorRotation.y;
				transform.rotation = Quaternion.Euler(angles);
			}
		}

		private void SnapPosition(RaycastHit hit, Vector3 posOffset)
		{
			if (m_SnapPosition)
				transform.position = hit.point + posOffset + m_PositionOffset;
		}

		private Vector3 SnapPositionToBounds(Vector3 posOffset)
		{
			if (m_SnapPositionToBounds)
			{
				var collider = GetComponent<Collider>();
				if (collider != null)
					posOffset.y = collider.bounds.size.y / 2f;
			}
			return posOffset;
		}
	}
}
