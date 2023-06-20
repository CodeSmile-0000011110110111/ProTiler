﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Events;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler3.Controller
{
	[ExcludeFromCodeCoverage] // the UI is a detail
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DViewController))]
	public sealed class Tilemap3DViewControllerRuntime : MonoBehaviour
	{
		private Vector3 m_LastMousePosition;

		private Tilemap3DViewController ViewController => GetComponent<Tilemap3DViewController>();

		private void Update() => CheckMouseMove();

		private void CheckMouseMove()
		{
			var camera = Camera.main;
			if (camera == null)
				return;

			var mousePos = Input.mousePosition;
			if (mousePos != m_LastMousePosition)
			{
				m_LastMousePosition = mousePos;

				var worldRay = camera.ScreenPointToRay(m_LastMousePosition);
				ViewController.OnMouseMove(new MouseMoveEventData(worldRay));
			}
		}
	}
}
