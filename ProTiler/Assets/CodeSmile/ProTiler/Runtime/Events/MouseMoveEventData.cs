﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler.Events
{
	public struct MouseMoveEventData
	{
		public Ray CurrentWorldRay { get; }
		public Ray LastWorldRay { get; }

		public MouseMoveEventData(Ray currentWorldRay, Ray lastWorldRay)
		{
			CurrentWorldRay = currentWorldRay;
			LastWorldRay = lastWorldRay;
		}
	}
}
