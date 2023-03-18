// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile
{
	public class ApplyMeshToMeshCollider : MonoBehaviour
	{
#if UNITY_EDITOR
		private void OnEnable()
		{
			var meshFilter = GetComponent<MeshFilter>();
			var meshCollider = GetComponent<MeshCollider>();
			if (meshFilter != null && meshCollider != null)
				meshCollider.sharedMesh = meshFilter.sharedMesh;

			DestroyImmediate(this);
		}
#endif
	}
}