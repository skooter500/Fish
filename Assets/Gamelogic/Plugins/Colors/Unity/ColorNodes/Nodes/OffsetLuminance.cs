using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that offsets the luminances of its inputs.
	/// </summary>
	[Version(1, 1)]
	public class OffsetLuminance : ColorTransformRandomNode
	{
		/// <summary>
		/// The base value of the offset.
		/// </summary>
		public float luminanceOffset;

		/// <summary>
		/// The range of random values. Random values are generated around the base value. For instance,
		/// if the base value is 0.3, and the range is 0.2, random values will be generated between 
		/// 0.2 and 0.4.
		/// </summary>
		public float range;

		override public Color Transform(Color input, float randomValue)
		{
			return input
				.ToHSL()
				.WithLuminanceOffset(GLRandom.RandomOffset(luminanceOffset, range))
				.Color;
		}
	}
}