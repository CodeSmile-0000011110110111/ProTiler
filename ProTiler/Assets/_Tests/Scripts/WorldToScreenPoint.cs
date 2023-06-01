// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WorldToScreenPoint : MonoBehaviour
{
	private void Update()
	{
		var camera = Camera.main;
		if (Application.isEditor && Application.isPlaying == false)
			camera = SceneView.lastActiveSceneView.camera;

		if (camera == null)
			return;

		var pos = Vector3.zero;

		var isInside = camera.IsPositionInViewport(pos);

		Debug.Log($"{camera.name} => pos {pos} is inside viewport: {isInside}");
	}
}
