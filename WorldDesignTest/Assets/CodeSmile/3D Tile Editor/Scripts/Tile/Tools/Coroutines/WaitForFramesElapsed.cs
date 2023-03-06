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
		private readonly int _callbackFrameCount;
		private readonly Action _callback;

		public object Current => null;

		public WaitForFramesElapsed(int numberOfFramesToWait, Action callback)
		{
			if (numberOfFramesToWait <= 0)
				throw new ArgumentException("number of frames to wait must be 1 or greater");
			if (callback == null)
				throw new ArgumentNullException("callback Action must not be null");

			_callbackFrameCount = Time.frameCount + numberOfFramesToWait;
			_callback = callback;
		}

		public bool MoveNext()
		{
			if (Time.frameCount >= _callbackFrameCount)
			{
				_callback.Invoke();
				return false;
			}

			return true;
		}

		public void Reset() {}
	}
}