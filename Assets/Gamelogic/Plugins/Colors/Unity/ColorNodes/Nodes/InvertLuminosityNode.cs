using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that inverts the luminosity of it's inputs.
	/// </summary>
	[Version(1, 1)]
	public class InvertLuminosityNode : ColorTransformNode
	{
		public override Color Transform(Color input)
		{
			return input
				.ToHSL()
				.InvertLuminosity()
				.Color;
		}
		
		public override void Recompute()
		{
			// nothing to compute
		}
	}
}