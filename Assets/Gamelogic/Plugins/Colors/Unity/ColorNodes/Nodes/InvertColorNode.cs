using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that inverts the colors of its inputs.
	/// </summary>
	[Version(1, 1)]
	public class InvertColorNode : ColorTransformNode
	{
		public override Color Transform(Color input)
		{
			return input.Invert();
		}

		public override void Recompute()
		{
			// nothing to compute
		}
	}
}