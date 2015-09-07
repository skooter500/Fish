namespace Gamelogic
{
	/// <summary>
	/// Some convenience functions for random bools and integers.
	/// </summary>
	[Version(1, 2)]
	public static class GLRandom
	{
		/// <summary>
		/// Globally accessible <see cref="System.Random"/> object for random calls
		/// </summary>
		static private readonly System.Random GlobalRandom = new System.Random();

		/// <summary>
		/// Generates a random bool, true with the given probability.
		/// </summary>
		/// <param name="probability"></param>
		/// <returns></returns>
		public static bool Bool(float probability)
		{
			return GlobalRandom.NextDouble() < probability;
		}

		/// <summary>
		/// Generates a Random integer between 0 inclusive and the given max, exclusive.
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Range(int max)
		{
			return GlobalRandom.Next(max);
		}

		/// <summary>
		/// Generates a Random integer between 0 inclusive and the given max, exclusive.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Range(int min, int max)
		{
			return GlobalRandom.Next(min, max);
		}

		public static float RandomOffset(float value, float range)
		{
			var offset = GlobalRandom.NextDouble()*range - range/2;
			return (float) (value + offset);
		}
	}
}