// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

public enum CellType
{
	Air,
	Sand,
}

public enum Neighbour
{
	None = 0,
	PositiveX,
	PositiveZ,
	NegativeX,
	NegativeZ,

	Count = 4,
}

public struct Cell : IBinarySerializable
{
	public CellType Type;
	public Int32 CubeIndex;
	public WorldPos CubePosition;

	public void SetPosition(WorldCoord coord)
	{
		CubePosition = coord;
		CubePosition.y += .5f;
	}

	public unsafe void Serialize(UnsafeAppendBuffer* writer) => throw new NotImplementedException();

	public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion) =>
		throw new NotImplementedException();
}

[ExecuteAlways]
public class FallingSandTest : MonoBehaviour
{
	private static readonly Cell EmptyCell = new() { Type = CellType.Air, CubeIndex = -1 };

	[SerializeField] private Single m_SimulationUpdateRate = 0.1f;
	[SerializeField] private Boolean m_DrawWireframeCubes;
	[SerializeField] private Boolean m_ContinueInsteadOfBreakBug;
	[SerializeField] private Boolean m_RandomNeighbourPicking;
	[SerializeField] private Boolean m_Reset;

	private readonly List<Transform> m_Cubes = new();
	private readonly ChunkSize m_ChunkSize = new(20, 20, 20);
	private LinearDataMapChunk<Cell> m_Chunk;

	private Single m_NextUpdateTime;

	private void OnValidate() => StartCoroutine(CheckReset());

	private void OnEnable()
	{
		AssemblyReloadEvents.beforeAssemblyReload += CreateWorld;
		AssemblyReloadEvents.afterAssemblyReload += CreateWorld;
	}

	private IEnumerator CheckReset()
	{
		yield return null;

		if (m_Reset)
		{
			m_Reset = false;

			CreateWorld();
		}
	}

	private void CreateWorld()
	{
		transform.DestroyAllChildren();
		m_Cubes.Clear();
		m_Chunk.Dispose();
		m_Chunk = new LinearDataMapChunk<Cell>(m_ChunkSize);
	}

	private void Start()
	{
		CreateWorld();
		AdvanceSimulationTime();
	}

	private void AdvanceSimulationTime() => m_NextUpdateTime = Time.time + m_SimulationUpdateRate;

	private void Update()
	{
		if (Time.time >= m_NextUpdateTime)
		{
			AdvanceSimulationTime();

			CreateSandCell();

			SimulateCells();
			UpdateCubes();
			//DestroyCubes();
		}
	}

	private void SimulateCells()
	{
		var layerCellCount = m_Chunk.Size.x * m_Chunk.Size.z;
		var cells = m_Chunk.GetWritableData();
		for (var index = 0; index < cells.Length; index++)
		{
			var cell = cells[index];
			if (cell.Type != CellType.Air)
			{
				var oneLayerDownIndex = index - layerCellCount;
				if (oneLayerDownIndex < 0)
					continue;

				var cellBelow = cells[oneLayerDownIndex];
				if (cellBelow.Type == CellType.Air)
				{
					cell.CubePosition.y -= 1f;
					cells[oneLayerDownIndex] = cell;
					cells[index] = EmptyCell;
					continue;
				}

				var randomOffset = (m_RandomNeighbourPicking ? UnityEngine.Random.Range(0, 4) : 0);
				for (var k = 0; k < 4; k++)
				{
					var n = (k + randomOffset) % 4;

					var neighbourIndex = n switch
					{
						0 => oneLayerDownIndex - m_ChunkSize.x,
						1 => oneLayerDownIndex + m_ChunkSize.x,
						2 => oneLayerDownIndex - 1,
						3 => oneLayerDownIndex + 1,
						_ => -1,
					};

					if (neighbourIndex >= 0)
					{
						var neighbourCell = cells[neighbourIndex];
						if (neighbourCell.Type == CellType.Air)
						{
							cell.CubePosition.x += n == 0 ? -1 : n == 1 ? 1 : 0;
							if (cell.CubePosition.y >= 1f)
								cell.CubePosition.y -= 1f;
							cell.CubePosition.z += n == 2 ? -1 : n == 3 ? 1 : 0;

							cells[neighbourIndex] = cell;
							cells[index] = EmptyCell;

							if (m_ContinueInsteadOfBreakBug)
								continue;

							break;
						}
					}
				}
			}
		}
	}

	private void UpdateCubes()
	{
		var cells = m_Chunk.GetWritableData();
		for (var index = 0; index < cells.Length; index++)
		{
			var cell = cells[index];
			if (cell.Type != CellType.Air)
			{
				if (cell.CubeIndex >= 0 && cell.CubeIndex < m_Cubes.Count)
				{
					var cube = m_Cubes[cell.CubeIndex];
					if (cube != null)
						cube.position = cell.CubePosition;
				}
			}
		}
	}

	private void DestroyCubes()
	{
		var layerCellCount = m_Chunk.Size.x * m_Chunk.Size.z;
		var cells = m_Chunk.GetWritableData();
		for (var index = 0; index < layerCellCount; index++)
		{
			var cell = cells[index];
			if (cell.Type != CellType.Air)
			{
				var cubeIndex = cell.CubeIndex;
				if (cubeIndex >= 0 && cubeIndex < m_Cubes.Count)
				{
					var cube = m_Cubes[cell.CubeIndex];
					if (cube != null)
					{
						m_Cubes[cell.CubeIndex].gameObject.DestroyInAnyMode();
						m_Cubes[cell.CubeIndex] = null;
						cells[index] = EmptyCell;
					}
				}
			}
		}
	}

	private void CreateSandCell()
	{
		var startPos = new WorldCoord(9, 19, 9);
		var startPosCell = m_Chunk[startPos];
		if (startPosCell.Type != CellType.Air)
			return;

		var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		var cubeTransform = cube.transform;
		cubeTransform.parent = transform;

		var cubeIndex = m_Cubes.IndexOf(null);
		if (cubeIndex < 0)
		{
			cubeIndex = m_Cubes.Count;
			m_Cubes.Add(cubeTransform);
		}
		else
			m_Cubes[cubeIndex] = cubeTransform;

		var cell = new Cell { Type = CellType.Sand, CubeIndex = cubeIndex };
		cell.SetPosition(startPos);
		m_Chunk.SetData(startPos, cell);
	}

	private void OnDrawGizmosSelected()
	{
		if (m_DrawWireframeCubes)
		{
			var cells = m_Chunk.GetWritableData();
			for (var index = 0; index < cells.Length; index++)
			{
				var cell = cells[index];
				if (cell.Type != CellType.Air)
					Handles.DrawWireCube(cell.CubePosition, Vector3.one);
			}
		}
	}
}
