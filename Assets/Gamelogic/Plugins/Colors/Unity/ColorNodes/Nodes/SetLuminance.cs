using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that outputs it's input colors with the luminance set.
	/// </summary>
	[Version(1, 1)]
	public class SetLuminance : ColorTransformRandomNode
	{
		/// <summary>
		/// The base of the luminance value.
		/// </summary>
		public float luminance;

		/// <summary>
		/// The range of the values to generate. For example, if luminance is at 0.3, and range is ar 0.2
		/// the algorithm will set luminances between 0.2 and 0.4.
		/// </summary>
		public float range;

		override public Color Transform(Color input, float randomValue)
		{
			var hsl = input.ToHSL();

			return hsl.WithLuminance(GLRandom.RandomOffset(luminance, range)).Color;
		}
	}
}