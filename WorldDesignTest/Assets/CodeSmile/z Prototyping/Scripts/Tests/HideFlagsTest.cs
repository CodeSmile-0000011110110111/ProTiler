// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

public class HideFlagsTest : MonoBehaviour
{
	public HideFlags m_HideFlags;

	private void OnValidate() => gameObject.hideFlags = m_HideFlags;
}