// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;

namespace CodeSmile
{
	/// <summary>
	///     Invokes callback Action after the specified number of frames.
	/// </summary>
	public class WaitForEndOfFrame : IEnumerator
	{
		private readonly Action m_Action;
		private bool m_MoveOnce = false;

		public object Current => null;

		public WaitForEndOfFrame(Action action)
		{
			if (action == null)
				throw new ArgumentNullException("callback Action must not be null");

			m_Action = action;
		}

		public bool MoveNext()
		{
			if (m_MoveOnce == false)
			{
				m_MoveOnce = true;
				return false;
			}
			
			m_Action.Invoke();
			return true;
		}

		public void Reset() {}
	}
}