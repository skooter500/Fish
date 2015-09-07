using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that outputs a HSL gradient formed from the input colors.
	/// </summary>
	[Version(1, 1)]
	public class CreateGradientHSLNode : SimpleNode<Color, GradientHSL>
	{
		public override GradientHSL ExecuteSingle(IEnumerable<Color> input)
		{
			var gradient = new GradientHSL();
			int keyCount = input.Count();
			var colorKeys = input.Select((c, i) => new GradientHSLColorKey(c.ToHSL(), i / (keyCount - 1f))).ToArray();
				
			gradient.SetKeys(colorKeys);

			return gradient;
		}

		public override void Recompute()
		{
			//nothing to compute
		}
	}
}