// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Tilemap
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class Tilemap3DBehaviour : MonoBehaviour
	{
		//[SerializeField] private Vector3 m_TileAnchor;
		[SerializeField] private Vector2Int m_ChunkSize = new(16, 16);
		[SerializeField] private Boolean m_UseBinarySerialization = true;
		[SerializeField] private Tilemap3D m_Map;
		[SerializeField] private Byte[] m_SerializedMap;

		[Pure] public Vector2Int ChunkSize
		{
			get => m_Map.ChunkSize;
			set => SetChunkSize(value);
		}

		[Pure] internal Int32 ChunkCount => m_Map.ChunkCount;
		[Pure] internal Int32 TileCount => m_Map.TileCount;

		[Pure] public Grid3DBehaviour Grid => transform.parent.GetComponent<Grid3DBehaviour>();

		[Pure] private void Awake() => SetChunkSize(m_ChunkSize);

		[Pure] private void Reset() => SetChunkSize(m_ChunkSize);

		[Pure] private void OnEnable() => RegisterEditorSceneEvents();

		[Pure] private void OnDisable() => UnregisterEditorSceneEvents();

		[Pure] private void OnValidate() => SetChunkSize(m_ChunkSize);

		[Pure] private void RegisterEditorSceneEvents()
		{
			UnregisterEditorSceneEvents();

#if UNITY_EDITOR
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneSaving += OnSceneSaving;
#endif
		}

		[Pure] private void UnregisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneSaving -= OnSceneSaving;
#endif
		}

		private void DeserializeTilemap()
		{
			if (m_UseBinarySerialization)
				m_Map = Tilemap3DSerialization.FromBinary(m_SerializedMap);
			else
			{
				var json = Encoding.UTF8.GetString(m_SerializedMap);
				m_Map = Tilemap3DSerialization.FromJson(json);
			}
		}

		[Pure] private void SerializeTilemap()
		{
			if (m_Map != null)
			{
				if (m_UseBinarySerialization)
					m_SerializedMap = Tilemap3DSerialization.ToBinary(m_Map);
				else
				{
					var json = Tilemap3DSerialization.ToJson(m_Map, false);
					m_SerializedMap = Encoding.UTF8.GetBytes(json);
				}
			}
		}

		[Pure] public Int32 GetLayerCount(ChunkCoord chunkCoord) => m_Map.GetLayerCount(chunkCoord);

		private void CreateMap(ChunkSize chunkSize) => m_Map = new Tilemap3D(chunkSize);

		private void SetChunkSize(ChunkSize chunkSize)
		{
			var clampedChunkSize = Tilemap3DUtility.ClampChunkSize(chunkSize);
			if (m_Map == null || clampedChunkSize != m_Map.ChunkSize)
			{
				m_ChunkSize = clampedChunkSize;
				CreateMap(m_ChunkSize);
			}
		}

		[Pure] public Tile3D GetTile(GridCoord coord) => GetTiles(new[] { coord }).FirstOrDefault().Tile;

		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> coords) => m_Map.GetTiles(coords);

		[Pure] public void SetTile(GridCoord coord, Tile3D tile) => SetTiles(new[] { new Tile3DCoord(coord, tile) });

		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> tileCoordDatas)
		{
			this.RecordUndoInEditor(nameof(SetTiles));
			SetTilesNoUndo(tileCoordDatas);
			this.SetDirtyInEditor();
		}

		[Pure] private void SetTilesNoUndo(IEnumerable<Tile3DCoord> tileCoordDatas) => m_Map.SetTiles(tileCoordDatas);

#if UNITY_EDITOR
		[Pure] private void OnSceneOpened(Scene scene, OpenSceneMode mode) => DeserializeTilemap();
		[Pure] private void OnSceneSaving(Scene scene, String path) => SerializeTilemap();
#endif
	}
}
