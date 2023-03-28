// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using UnityEngine;

namespace CodeSmile
{
	/// <summary>
	///     Invokes callback Action after the specified time in seconds.
	///     If 0 seconds is specified it will wait at least one frame to prevent memory leaks.
	/// </summary>
	public class WaitForSecondsElapsed : IEnumerator
	{
		private readonly float m_CallbackSeconds;
		private readonly Action m_Callback;
		private readonly bool m_UseRealtimeSeconds;
		public object Current => null;

		public WaitForSecondsElapsed(float secondsToWait, Action callback, bool realtimeSeconds = false)
		{
			m_UseRealtimeSeconds = realtimeSeconds;
			m_CallbackSeconds = GetCurrentTime() + secondsToWait;
			m_Callback = callback ?? throw new ArgumentNullException("callback Action must not be null");
		}

		public bool MoveNext()
		{
			if (GetCurrentTime() > m_CallbackSeconds)
			{
				m_Callback.Invoke();
				return false;
			}

			return true;
		}

		public void Reset() {}
		private float GetCurrentTime() => m_UseRealtimeSeconds ? Time.realtimeSinceStartup : Time.timeSinceLevelLoad;
	}
}