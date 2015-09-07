using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that outputs it's input colors with the hue set.
	/// </summary>
	[Version(1, 1)]
	public class SetHue : ColorTransformRandomNode
	{
		/// <summary>
		/// The base of the hue value.
		/// </summary>
		public float hue;

		/// <summary>
		/// The range of the values to generate. For example, if hue is at 0.3, and range is ar 0.2
		/// the algorithm will set hues between 0.2 and 0.4.
		/// </summary>
		public float range;

		override public Color Transform(Color input, float randomValue)
		{
			return input
				.ToHSL()
				.WithHue(GLRandom.RandomOffset(hue, range))
				.Color;
		
		}
	}
}