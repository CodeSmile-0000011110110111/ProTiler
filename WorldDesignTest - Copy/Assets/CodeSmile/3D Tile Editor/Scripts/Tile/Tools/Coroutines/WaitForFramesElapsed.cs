// Copyright (C) 2021-2022 Steffen Itterheim
// Usage is bound to the Unity Asset Store Terms of Service and EULA: https://unity3d.com/legal/as_terms

using System;
using System.Collections;
using UnityEngine;

namespace CodeSmile
{
	/// <summary>
	/// Invokes callback Action after the specified number of frames.
	/// </summary>
	public class WaitForFramesElapsed : IEnumerator
	{
		private readonly int m_CallbackFrameCount;
		private readonly Action m_Action;

		public object Current => null;

		public WaitForFramesElapsed(int numberOfFramesToWait, Action action)
		{
			if (numberOfFramesToWait <= 0)
				throw new ArgumentException($"{nameof(numberOfFramesToWait)} must be 1 or greater");
			if (action == null)
				throw new ArgumentNullException("callback Action must not be null");

			m_CallbackFrameCount = Time.frameCount + numberOfFramesToWait;
			m_Action = action;
		}

		public bool MoveNext()
		{
			if (Time.frameCount >= m_CallbackFrameCount)
			{
				m_Action.Invoke();
				return false;
			}

			return true;
		}

		public void Reset() {}
	}
}