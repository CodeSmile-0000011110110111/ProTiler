using UnityEngine;

namespace SBG.Toolbelt
{
	public static class Randomizer
	{
		public static void SetSeed(int seed)
		{
			Random.InitState(seed);
		}

		/// <summary>
		/// A random float between 0 and 100.
		/// </summary>
		public static float RandomPercent
		{
			get
			{
				return Random.Range(0f, 100f);
			}
		}

		/// <summary>
		/// A random float between 0 and 1.
		/// </summary>
		public static float RandomValue01
        {
            get
            {
				return Random.Range(0f, 1f);
            }
        }

		/// <summary>
		/// The result of a 50% chance.
		/// </summary>
		public static bool FiftyFifty
        {
            get
            {
				return Random.Range(0, 2) == 0;
            }
        }

		/// <summary>
		/// Returns the result of the given percent chance
		/// </summary>
		/// <param name="percentChance">The likelyhood of success in %</param>
		public static bool RandomChance(float percentChance)
		{
			return Random.Range(0f, 100f) <= percentChance;
		}

		/// <summary>
		/// Takes an array of percent chances (with a total of 100%)
		/// and returns The index of the chosen element.
		/// </summary>
		/// <param name="percentChances">The likelyhood of success for each element in %. The total sum of all chances should be EXACTLY 100%.</param>
		public static int WeightedRandomChance(float[] percentChances)
        {
			float percentResult = RandomPercent;
			float prevPercentSum = 0;

            for (int i = 0; i < percentChances.Length - 1; i++)
            {
				if (percentResult <= (percentChances[i] + prevPercentSum))
                {
					return i;
                }

				prevPercentSum += percentChances[i];
            }

			return percentChances.Length - 1;
        }
	}
}