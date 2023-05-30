// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor;

namespace CodeSmile
{
	/// <summary>
	///     Keeps a history of specific Undo.GetCurrentGroup identifiers so that you can register for
	///     OnRegisteredUndoRedoEvent such that you only get callbacks when one of the Undo operations
	///     you are interested in raises the event.
	///     The main intention is to use this class to filter out unwanted undo/redo operations because
	///     Undo doesn't give you a target object before Unity 2022, and even with 2022 it is cumbersome.
	/// </summary>
	public sealed class UndoGroupRegistry
	{
		public Action OnRegisteredUndoRedoEvent;

		public void RegisterUndoRedoEvents(Action callback)
		{
#if UNITY_EDITOR
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
			Undo.willFlushUndoRecord += OnWillFlushUndoRecord;
			OnRegisteredUndoRedoEvent += callback;
#endif
		}

		public void UnregisterUndoRedoEvents(Action callback)
		{
#if UNITY_EDITOR
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
			Undo.willFlushUndoRecord -= OnWillFlushUndoRecord;
			OnRegisteredUndoRedoEvent -= callback;
#endif
		}

		public void RegisterCurrentUndoGroup()
		{
#if UNITY_EDITOR
			m_UndoGroups.Add(Undo.GetCurrentGroup());
#endif
		}

		private void OnUndoRedoPerformed()
		{
#if UNITY_EDITOR
			if (m_UndoGroups.Contains(m_CurrentUndoGroup))
				OnRegisteredUndoRedoEvent?.Invoke();
#endif
		}

		private void OnWillFlushUndoRecord()
		{
#if UNITY_EDITOR
			m_CurrentUndoGroup = Undo.GetCurrentGroup();
#endif
		}

#if UNITY_EDITOR
		private readonly HashSet<Int32> m_UndoGroups = new();
		private Int32 m_CurrentUndoGroup;
#endif
	}
}
