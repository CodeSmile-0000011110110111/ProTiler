// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler3.Model;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.ProTiler3.Assets
{
	[FullCovered]
	public abstract class Tile3DAssetBase : ScriptableObject
	{
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private Tile3DFlags m_Flags;
		[SerializeField] [HideInInspector] private Matrix4x4 m_Transform;

		public GameObject Prefab
		{
			get => m_Prefab;
			set => m_Prefab = value;
		}

		public Tile3DFlags Flags
		{
			get => m_Flags;
			set => m_Flags = value;
		}

		public Matrix4x4 Transform
		{
			get => m_Transform;
			set => m_Transform = value;
		}

		[ExcludeFromCodeCoverage]
		private void Reset() => SetDefaultFlags();

		internal void SetDefaultFlags() => m_Flags = Tile3DFlags.DirectionNorth;
	}
}
