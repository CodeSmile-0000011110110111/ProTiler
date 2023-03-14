using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.MeshExtractor
{
    class UndoStack<T>
    {
        /// <summary>
        /// List of states
        /// </summary>
        protected LinkedList<T> _undoStack = new LinkedList<T>();
        protected LinkedList<T> _redoStack = new LinkedList<T>();

        protected double _lastSelectionUndoRegistrationTime = 0;

        protected System.Func<T, T> _copyStateFunc;
        protected System.Action<T> _assignNewStateFunc;
        public int MaxEntries;

        /// <summary>
        /// A custom undo redo stack.<br />
        /// If the stack is empty then you should add a single initial state which it can return to.
        /// </summary>
        /// <param name="copyStateFunc">A function that returns a copy of the current state. This copy will be stored in the undo stack.</param>
        /// <param name="assignNewStateFunc">A function to apply the given value as the new current state.
        /// The parameter will be one of the copies created by the 'copyStateFunc'.
        /// You may want to copy the given value again if it is a reference type.</param>
        public UndoStack(System.Func<T, T> copyStateFunc, System.Action<T> assignNewStateFunc, int maxEntries = 20)
        {
            _copyStateFunc = copyStateFunc;
            _assignNewStateFunc = assignNewStateFunc;
            MaxEntries = maxEntries;
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public T Peek()
        {
            if (HasUndoActions())
            {
                return _undoStack.Last.Value;
            }    
            else
            {
                return default;
            }
        }

        public bool HasUndoActions()
        {
            return _undoStack.Count > 0;
        }

        public bool IsEmpty()
        {
            return !HasUndoActions() && !HasRedoActions();
        }

        public bool HasRedoActions()
        {
            return _redoStack.Count > 0;
        }

        /// <summary>
        /// Register (or update) a new undo action.
        /// </summary>
        /// <param name="state">The current state to record.</param>
        /// <param name="minTimeDelta">Register a new undo if the last one is older than # seconds, otherwise update the current one.</param>
        /// <param name="forceNewGroup"></param>
        public void Add(T state, double minTimeDelta = 0d, bool forceNewGroup = false)
        {
            if (_undoStack.Count > MaxEntries)
            {
                _undoStack.RemoveFirst();
            }

            if (forceNewGroup)
            {
                var copy = _copyStateFunc.Invoke(state);
                _undoStack.AddLast(copy);
            }
            else
            {
                // Register a new undo if the last one is older than # seconds, otherwise update the current one.
                if (EditorApplication.timeSinceStartup - _lastSelectionUndoRegistrationTime < minTimeDelta)
                {
                    if (_undoStack.Count > 0)
                    {
                        _undoStack.RemoveLast();
                    }
                }
                var copy = _copyStateFunc.Invoke(state);
                _undoStack.AddLast(copy);
            }
            _lastSelectionUndoRegistrationTime = EditorApplication.timeSinceStartup;
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0) 
            {
                var last = _undoStack.Last.Value;
                _undoStack.RemoveLast();
                _redoStack.AddLast(last);
                if (_undoStack.Count > 0)
                {
                    _assignNewStateFunc.Invoke(_undoStack.Last.Value);
                }
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var last = _redoStack.Last.Value;
                _redoStack.RemoveLast();
                _undoStack.AddLast(last);
                _assignNewStateFunc(last);
            }
        }
    }
}
