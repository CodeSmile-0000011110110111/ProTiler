

namespace SBG.Toolbelt
{
	/// <summary>
	/// Value Range can be used to create a clear Min/Max Range for a variable.
	/// This is meant to make exposed fields cleaner and your variable list a bit shorter.
	/// </summary>
	[System.Serializable]
	public struct ValueRange<T>
	{
		public T Min;
		public T Max;

		public ValueRange(T min, T max)
		{
			Min = min;
			Max = max;
		}
	}
}