using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that inverts the staturation of its inputs.
	/// </summary>
	[Version(1, 1)]
	public class InvertSaturationNode : ColorTransformNode
	{
		public override Color Transform(Color input)
		{
			return input
				.ToHSL()
				.InvertSaturation()
				.Color;
		}

		public override void Recompute()
		{
			// nothing to compute
		}
	}
}