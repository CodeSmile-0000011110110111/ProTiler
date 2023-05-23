// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Editor
{
	/// <summary>
	///     Base class for boilerplate code removal and a more natural "OnEvent" callback methodology
	///     Unity devs are more familiar with from MonoBehaviour code. And I would tend to say that the
	///     code will be cleaner when not having to switch on an EventType.
	///     All EventType events that occur in OnSceneGUI are forwarded to protected virtual methods that
	///     user can override in the subclass and just "magically" receive those event methods.
	///     The Event.current is passed in as a parameter for convenience.
	/// </summary>
	[ExcludeFromCodeCoverage]
	[SuppressMessage("NDepend", "ND1001:AvoidTypesWithTooManyMethods", Justification="unavoidable")]
	public abstract class OnSceneEventEditorBase : UnityEditor.Editor
	{
		public void OnSceneGUI() => ForwardEvents();

		/// <summary>
		/// Pick up all EventType and distribute them to calls of protected virtual 'On***' methods
		/// to mimick standard MonoBehaviour behaviour and to make for cleaner code style.
		/// Example:
		/// EventType == EventType.MouseMove calls OnMouseMove(Event.current)
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[SuppressMessage("NDepend", "ND1006:AvoidMethodsPotentiallyPoorlyCommented", Justification="needs no comments")]
		private void ForwardEvents()
		{
			var evt = Event.current;
			switch (evt.type)
			{
				case EventType.MouseDown:
					OnMouseDown(evt);
					break;
				case EventType.MouseUp:
					OnMouseUp(evt);
					break;
				case EventType.MouseMove:
					OnMouseMove(evt);
					break;
				case EventType.MouseDrag:
					OnMouseDrag(evt);
					break;
				case EventType.KeyDown:
					OnKeyDown(evt);
					break;
				case EventType.KeyUp:
					OnKeyUp(evt);
					break;
				case EventType.ScrollWheel:
					OnScrollWheel(evt);
					break;
				case EventType.Repaint:
					OnRepaint(evt);
					break;
				case EventType.Layout:
					OnLayout(evt);
					break;
				case EventType.DragUpdated:
					OnDragUpdated(evt);
					break;
				case EventType.DragPerform:
					OnDragPerform(evt);
					break;
				case EventType.DragExited:
					OnDragExited(evt);
					break;
				case EventType.Ignore:
					OnIgnore(evt);
					break;
				case EventType.Used:
					OnUsed(evt);
					break;
				case EventType.ValidateCommand:
					OnValidateCommand(evt);
					break;
				case EventType.ExecuteCommand:
					OnExecuteCommand(evt);
					break;
				case EventType.ContextClick:
					OnContextClick(evt);
					break;
				case EventType.MouseEnterWindow:
					OnMouseEnterWindow(evt);
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow(evt);
					break;
				case EventType.TouchDown:
					OnTouchDown(evt);
					break;
				case EventType.TouchUp:
					OnTouchUp(evt);
					break;
				case EventType.TouchMove:
					OnTouchMove(evt);
					break;
				case EventType.TouchEnter:
					OnTouchEnter(evt);
					break;
				case EventType.TouchLeave:
					OnTouchLeave(evt);
					break;
				case EventType.TouchStationary:
					OnTouchStationary(evt);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(evt.type));
			}
		}

		protected virtual void OnMouseDown(Event evt) {}
		protected virtual void OnMouseUp(Event evt) {}
		protected virtual void OnMouseMove(Event evt) {}
		protected virtual void OnMouseDrag(Event evt) {}
		protected virtual void OnKeyDown(Event evt) {}
		protected virtual void OnKeyUp(Event evt) {}
		protected virtual void OnScrollWheel(Event evt) {}
		protected virtual void OnRepaint(Event evt) {}
		protected virtual void OnLayout(Event evt) {}
		protected virtual void OnDragUpdated(Event evt) {}
		protected virtual void OnDragPerform(Event evt) {}
		protected virtual void OnDragExited(Event evt) {}
		protected virtual void OnIgnore(Event evt) {}
		protected virtual void OnUsed(Event evt) {}
		protected virtual void OnValidateCommand(Event evt) {}
		protected virtual void OnExecuteCommand(Event evt) {}
		protected virtual void OnContextClick(Event evt) {}
		protected virtual void OnMouseEnterWindow(Event evt) {}
		protected virtual void OnMouseLeaveWindow(Event evt) {}
		protected virtual void OnTouchDown(Event evt) {}
		protected virtual void OnTouchUp(Event evt) {}
		protected virtual void OnTouchMove(Event evt) {}
		protected virtual void OnTouchEnter(Event evt) {}
		protected virtual void OnTouchLeave(Event evt) {}
		protected virtual void OnTouchStationary(Event evt) {}
	}
}
