// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using UnityEngine;

namespace CodeSmile.Coroutines
{
	/// <summary>
	///     Invokes callback Action after the specified number of frames.
	/// </summary>
	public class InvokeOnFramesElapsed : IEnumerator
	{
		private readonly int m_CallbackFrameCount;
		private readonly Action m_Action;
		public object Current => null;

		public InvokeOnFramesElapsed(int numberOfFramesToWait, Action action)
		{
			if (numberOfFramesToWait <= 0)
				throw new ArgumentException($"{nameof(numberOfFramesToWait)} must be 1 or greater");

			m_CallbackFrameCount = Time.frameCount + numberOfFramesToWait;
			m_Action = action ?? throw new ArgumentNullException("callback Action must not be null");
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
