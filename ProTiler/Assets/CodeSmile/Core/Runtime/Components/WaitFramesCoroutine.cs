// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Components
{
	/// <summary>
	///     Invokes callback Action once after the specified number of frames have elapsed.
	/// </summary>
	public sealed class WaitFramesCoroutine : IEnumerator
	{
		private const Int32 MinFramesToWait = 1;

		private readonly Int32 m_CallbackFrameCount;
		private readonly Action m_Callback;

		public Object Current => null;

		/// <summary>
		///     Minimum frame count is 1. Lower values will be clamped.
		/// </summary>
		/// <param name="framesToWait"></param>
		/// <param name="callback"></param>
		public WaitFramesCoroutine(Int32 framesToWait, Action callback)
		{
			m_CallbackFrameCount = Time.frameCount + Mathf.Max(MinFramesToWait, framesToWait);
			m_Callback = callback;
		}

		public Boolean MoveNext()
		{
			if (Time.frameCount >= m_CallbackFrameCount)
			{
				m_Callback?.Invoke();
				return false;
			}

			return true;
		}

		public void Reset() => throw new NotImplementedException();
	}
}
