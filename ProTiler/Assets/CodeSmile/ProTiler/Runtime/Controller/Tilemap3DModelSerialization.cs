// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Tilemap;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[AddComponentMenu("")] // hide from Add Component menu
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModelController))]
	public sealed class Tilemap3DModelSerialization : MonoBehaviour
	{
		private const Boolean m_UseBinarySerialization = true;

		[SerializeField] private Byte[] m_SerializedData = new Byte[0];

		private readonly HashSet<Int32> m_UndoGroups = new();

		[Pure] private Tilemap3DModelController TilemapModelController => GetComponent<Tilemap3DModelController>();
		[Pure] private Tilemap3D Tilemap
		{
			get => TilemapModelController.Tilemap;
			set => TilemapModelController.Tilemap = value;
		}

		[Pure] private void OnEnable()
		{
			TilemapModelController.OnTilemapReplaced += OnTilemapReplaced;
			TilemapModelController.OnTilemapModified += OnTilemapModified;
			RegisterEditorSceneEvents();
		}

		[Pure] private void OnDisable()
		{
			TilemapModelController.OnTilemapReplaced -= OnTilemapReplaced;
			TilemapModelController.OnTilemapModified -= OnTilemapModified;
			UnregisterEditorSceneEvents();
		}

		[Pure] private void RegisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			UnregisterEditorSceneEvents();
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneSaving += OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
			Undo.willFlushUndoRecord += OnWillFlushUndoRecord;
#endif
		}

		[Pure] private void UnregisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneSaving -= OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
			Undo.willFlushUndoRecord += OnWillFlushUndoRecord;
#endif
		}

		[Pure] private void OnTilemapReplaced()
		{
			SetCurrentUndoGroupAsUndoable();
			SerializeTilemap(Tilemap);
		}
		[Pure] private void OnTilemapModified(IEnumerable<Tile3DCoord> tileCoords)
		{
			SetCurrentUndoGroupAsUndoable();
			SerializeTilemap(Tilemap);
		}

		private void SetCurrentUndoGroupAsUndoable()
		{
#if UNITY_EDITOR
			m_UndoGroups.Add(Undo.GetCurrentGroup());
#endif
		}

		private void SerializeTilemap(Tilemap3D tilemap)
		{
			// FIXME: let this go through the controller
			this.UndoRecordObjectInEditor("Serialize Tilemap");

			m_SerializedData = m_UseBinarySerialization
				? Tilemap3DSerialization.ToBinary(tilemap).Compress()
				: Encoding.UTF8.GetBytes(Tilemap3DSerialization.ToJson(tilemap, false)).Compress();

			Debug.Log($"{tilemap} => {m_SerializedData.Length} bytes ({m_SerializedData.CalculateMd5Hash()})");
		}

		[Pure] private Tilemap3D DeserializeTilemap()
		{
			var tilemap = m_UseBinarySerialization
				? Tilemap3DSerialization.FromBinary(m_SerializedData.Decompress())
				: Tilemap3DSerialization.FromJson(Encoding.UTF8.GetString(m_SerializedData.Decompress()));

			Debug.Log($"{tilemap} <= {m_SerializedData.Length} bytes ({m_SerializedData.CalculateMd5Hash()})");

			return tilemap;
		}

#if UNITY_EDITOR
		[ExcludeFromCodeCoverage]
		private void VerifyBufferMatches(Tilemap3D tilemap)
		{
			var serializedTilemap = m_UseBinarySerialization
				? Tilemap3DSerialization.ToBinary(tilemap).Compress()
				: Encoding.UTF8.GetBytes(Tilemap3DSerialization.ToJson(tilemap, false)).Compress();

			if (serializedTilemap.Length != m_SerializedData.Length)
			{
				Debug.LogWarning("Save scene: current serialized tilemap did not match newly serialized one. " +
				                 "This indicates changes that bypassed serialization. " +
				                 "Serializing the current state.");
				m_SerializedData = serializedTilemap;
			}
		}

		/// <summary>
		///     Save the map before domain reload.
		/// </summary>
		[Pure] [ExcludeFromCodeCoverage] private void OnBeforeAssemblyReload() => SerializeTilemap(Tilemap);

		/// <summary>
		///     Save the map before saving the scene.
		/// </summary>
		[Pure] private void OnSceneSaving(Scene scene, String path) => VerifyBufferMatches(Tilemap);

		/// <summary>
		///     Restore the map after domain reload.
		/// </summary>
		[Pure] [ExcludeFromCodeCoverage] private void OnAfterAssemblyReload() => Tilemap = DeserializeTilemap();

		/// <summary>
		///     Restore the map after opening the scene.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"></param>
		[Pure] private void OnSceneOpened(Scene scene, OpenSceneMode mode) => Tilemap = DeserializeTilemap();

		/// <summary>
		///     Restore the map after Undo/Redo.
		///     This merely requires deserialization for both Undo and Redo because Unity's Undo/Redo system
		///     has already restored the MonoBehaviour's tilemap byte array to the expected map state,
		///     thus we only need to deserialize the tilemap from its buffer. Neat! :)
		/// </summary>
		private Int32 m_CurrentUndoGroup;
		private String m_CurrentUndoGroupName;

		[Pure] private void OnUndoRedoPerformed()
		{
			Debug.Log($"OnUndoRedoPerformed => {m_CurrentUndoGroup}: {m_CurrentUndoGroupName}");
			if (m_UndoGroups.Contains(m_CurrentUndoGroup))
				Tilemap = DeserializeTilemap();
		}

		private void OnWillFlushUndoRecord()
		{
			m_CurrentUndoGroup = Undo.GetCurrentGroup();
			m_CurrentUndoGroupName = Undo.GetCurrentGroupName();
			//Debug.Log($"OnWillFlushUndoRecord => {m_CurrentUndoGroup}: {m_CurrentUndoGroupName}");
		}
#endif
	}
}
