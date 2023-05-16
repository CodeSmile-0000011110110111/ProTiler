// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Properties;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     A collection of Tile3DLayer instances.
	/// </summary>
	[Serializable]
	internal class Tile3DLayers : IEnumerable<Tile3DLayer>
	{
		[CreateProperty] private List<Tile3DLayer> m_Layers = new();
		public Tile3DLayer this[int index] => m_Layers[index];
		public int Count => m_Layers.Count;
		public int Capacity
		{
			[ExcludeFromCodeCoverage] get => m_Layers.Capacity;
			set => m_Layers.Capacity = value;
		}

		[ExcludeFromCodeCoverage]
		IEnumerator<Tile3DLayer> IEnumerable<Tile3DLayer>.GetEnumerator() => m_Layers.GetEnumerator();

		[ExcludeFromCodeCoverage] public IEnumerator GetEnumerator() => m_Layers.GetEnumerator();
		public void Add(Tile3DLayer layer) => m_Layers.Add(layer);
	}
}