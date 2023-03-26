using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlesTest : MonoBehaviour
{
	[SerializeField] private float m_TestFloat;
	public float TestFloat { get => m_TestFloat; set => m_TestFloat = value; }
}
