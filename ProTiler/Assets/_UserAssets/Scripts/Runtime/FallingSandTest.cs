// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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

public struct Cell : IBinarySerializable
{
	public CellType Type;
	public Int32 CubeIndex;

	public unsafe void Serialize(UnsafeAppendBuffer* writer) => throw new NotImplementedException();

	public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion) =>
		throw new NotImplementedException();
}

[ExecuteAlways]
public class FallingSandTest : MonoBehaviour
{
	private static readonly Cell EmptyCell = new() { Type = CellType.Air, CubeIndex = -1 };

	[SerializeField] private Single m_SimulationUpdateRate = 0.1f;
	[SerializeField] private Boolean m_Reset;

	private readonly List<Transform> m_Cubes = new();
	private readonly ChunkSize m_ChunkSize = new(20, 20, 20);
	private LinearDataMapChunk<Cell> m_Chunk;

	private Single m_NextUpdateTime;

	private void OnValidate() => StartCoroutine(CheckReset());

	private IEnumerator CheckReset()
	{
		yield return null;

		if (m_Reset)
		{
			m_Reset = false;

			transform.DestroyAllChildren();
			m_Cubes.Clear();
			CreateChunk();
		}
	}

	private void CreateChunk()
	{
		m_Chunk.Dispose();
		m_Chunk = new LinearDataMapChunk<Cell>(m_ChunkSize);
	}

	private void Awake()
	{
		CreateChunk();
		AdvanceTimer();
	}

	private void AdvanceTimer() => m_NextUpdateTime = Time.time + m_SimulationUpdateRate;

	private void Update()
	{
		if (Time.time >= m_NextUpdateTime)
		{
			AdvanceTimer();
			MoveCubes();
			DestroyCubes();

			if (Time.frameCount % 3 == 0)
				CreateCube();
		}
	}

	private void MoveCubes()
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
					cells[oneLayerDownIndex] = cell;
					cells[index] = EmptyCell;

					if (cell.CubeIndex >= 0 && cell.CubeIndex < m_Cubes.Count)
					{
						var cube = m_Cubes[cell.CubeIndex];
						if (cube != null)
						{
							var height = index / layerCellCount;
							var indexOnLayer = index - height * layerCellCount;
							var cubeCoord = new ChunkSize(indexOnLayer % m_Chunk.Size.x, height,
								indexOnLayer / m_Chunk.Size.z);

							cube.position = new Vector3(cubeCoord.x, cubeCoord.y + 0.5f, cubeCoord.z);
						}
					}
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
					}
				}

				cells[index] = EmptyCell;
			}
		}
	}

	private void CreateCube()
	{
		var startPos = new WorldCoord(0, 19, 0);
		var startPosCell = m_Chunk[startPos];
		if (startPosCell.Type != CellType.Air)
			return;

		var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		var cubeTransform = cube.transform;
		cubeTransform.parent = transform;
		cubeTransform.position = new Vector3(0f, 19.5f, 0f);

		var cubeIndex = m_Cubes.IndexOf(null);
		if (cubeIndex < 0)
		{
			cubeIndex = m_Cubes.Count;
			m_Cubes.Add(cubeTransform);
		}
		else
			m_Cubes[cubeIndex] = cubeTransform;

		m_Chunk.SetData(startPos, new Cell { Type = CellType.Sand, CubeIndex = cubeIndex });
	}
}
