// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler.Events
{
	[SuppressMessage("NDepend", "ND1313:OverrideEqualsAndOperatorEqualsOnValueTypes")]
	public struct MouseMoveEventData
	{
		public Ray WorldRay { get; }

		public MouseMoveEventData(Ray worldRay) => WorldRay = worldRay;
	}
}
