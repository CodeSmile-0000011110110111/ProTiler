﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Editor.Creation;
using CodeSmile.ProTiler3.Runtime.Controller;
using CodeSmile.ProTiler3.Runtime.Events;
using CodeSmile.ProTiler3.Runtime.Grid;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;
using CellSize = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler3.Controller
{
	public class Tilemap3DViewControllerTests
	{
		private static Tilemap3DViewController GetTilemap3DViewController() => Tilemap3DCreation
			.CreateRectangularTilemap3D()
			.GetComponent<Tilemap3DViewController>();

		[Test] [CreateEmptyScene]
		public void OnCursorUpdateRaisedAfterMouseMoveEvent()
		{
			var controller = GetTilemap3DViewController();

			var didRaise = false;
			controller.OnCursorUpdate += cursor => { didRaise = true; };
			controller.OnMouseMove(new MouseMoveEventData(new Ray(Vector3.one, Vector3.down)));

			Assert.That(didRaise);
		}

		[Test] [CreateEmptyScene]
		public void CursorIsValidForMouseIntersectingPlane()
		{
			var controller = GetTilemap3DViewController();

			var cursor = new Grid3DCursor();
			controller.OnCursorUpdate += c => { cursor = c; };
			controller.OnMouseMove(new MouseMoveEventData(new Ray(Vector3.one, Vector3.down)));

			Assert.That(cursor.IsValid);
		}

		[Test] [CreateEmptyScene]
		public void CursorIsNotValidForMouseMissingPlane()
		{
			var controller = GetTilemap3DViewController();

			var cursor = new Grid3DCursor();
			controller.OnCursorUpdate += c => { cursor = c; };
			controller.OnMouseMove(new MouseMoveEventData(new Ray(Vector3.one, Vector3.left)));

			Assert.That(cursor.IsValid == false);
		}

		[Test] [CreateEmptyScene]
		public void CursorPositionAndCoordAsExpected()
		{
			var controller = GetTilemap3DViewController();

			var origin = new Vector3(1.53f, 2.79f, 4.56f);
			var cursor = new Grid3DCursor();
			controller.OnCursorUpdate += c => { cursor = c; };
			controller.OnMouseMove(new MouseMoveEventData(new Ray(origin, Vector3.down)));

			Assert.That(cursor.IsValid);
			Assert.That(cursor.Coord,
				Is.EqualTo(Grid3DUtility.ToGridCoord(cursor.CenterPosition, new CellSize(1, 1, 1))));
			Assert.That(cursor.CenterPosition, Is.EqualTo(cursor.Coord + new CellSize(1, 1, 1) / 2f));
			Assert.That(cursor.CellSize, Is.EqualTo(new CellSize(1, 1, 1)));
		}

		[Test] [CreateEmptyScene]
		public void DisableCursorSendsInvalidCursor()
		{
			var controller = GetTilemap3DViewController();

			var cursor = new Grid3DCursor();
			controller.OnCursorUpdate += c => { cursor = c; };
			controller.OnMouseMove(new MouseMoveEventData(new Ray(Vector3.one, Vector3.down)));
			controller.DisableCursor();

			Assert.That(cursor.IsValid == false);
		}

		[Test] [CreateEmptyScene]
		public void DisableCursorSendsNoFurtherUpdates()
		{
			var controller = GetTilemap3DViewController();

			var raiseCount = 0;
			controller.OnCursorUpdate += c => { raiseCount++; };
			controller.DisableCursor();
			controller.OnMouseMove(new MouseMoveEventData(new Ray(Vector3.one, Vector3.down)));

			Assert.That(raiseCount, Is.EqualTo(1));
		}

		[Test] [CreateEmptyScene]
		public void DisableThenEnableCursorSendsCurrentCursor()
		{
			var controller = GetTilemap3DViewController();

			var origin = new Vector3(1.9f, 0f, 3.4f);
			var cursor = new Grid3DCursor();
			controller.OnCursorUpdate += c => { cursor = c; };
			controller.DisableCursor();
			controller.OnMouseMove(new MouseMoveEventData(new Ray(origin, Vector3.down)));
			controller.EnableCursor();

			Assert.That(cursor.IsValid);
			Assert.That(cursor.Coord, Is.EqualTo(Grid3DUtility.ToGridCoord(origin, new CellSize(1, 1, 1))));
		}
	}
}