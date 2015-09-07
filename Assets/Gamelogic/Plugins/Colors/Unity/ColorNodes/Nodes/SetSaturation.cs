using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that takes input colors, and return the same
	/// colors with the saturation set.
	/// </summary>
	[Version(1, 1)]
	public class SetSaturation : ColorTransformRandomNode 
	{
		/// <summary>
		/// The base of the saturation value.
		/// </summary>
		public float saturation;

		/// <summary>
		/// The range of the values to generate. For example, if saturation is at 0.3, and range is ar 0.2
		/// the algorithm will set saturations between 0.2 and 0.4.
		/// </summary>
		public float range;

		override public Color Transform(Color input, float randomValue)
		{
			return input
				.ToHSL()
				.WithSaturation(GLRandom.RandomOffset(saturation, range))
				.Color;
		}
	}
}