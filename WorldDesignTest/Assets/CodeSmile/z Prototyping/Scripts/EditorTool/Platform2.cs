// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

public class Platform2 : MonoBehaviour
{
	[SerializeField] private Vector3 m_Start = new(-10, 0f, 0f);
	[SerializeField] private Vector3 m_End = new(10f, 0f, 0f);
	[SerializeField] private float m_Speed = .2f;

	public Vector3 start { get => m_Start; set => m_Start = value; }
	public Vector3 end   { get => m_End;   set => m_End = value; }
	public float   speed { get => m_Speed; set => m_Speed = value; }

	private void Update() => SnapToPath(Time.time);
	public void SnapToPath(float time) => transform.position = Vector3.Lerp(m_Start, m_End, (Mathf.Sin(time * m_Speed) + 1) * .5f);
}