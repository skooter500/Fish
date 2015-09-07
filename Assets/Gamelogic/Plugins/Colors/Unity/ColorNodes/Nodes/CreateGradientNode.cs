using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Creates a RGB gradient from the input colors.
	/// </summary>
	[Version(1, 1)]
	public class CreateGradientNode : SimpleNode<Color, Gradient>
	{
		public override Gradient ExecuteSingle(IEnumerable<Color> input)
		{
			var gradient = new Gradient();
			int keyCount = input.Count();
			var colorKeys = input.Select((c, i) => new GradientColorKey(c, i/(keyCount - 1f))).ToArray();
			var alphaKeys = input.Select((c, i) => new GradientAlphaKey(c.a, i / (keyCount - 1f))).ToArray();
			gradient.SetKeys(colorKeys, alphaKeys);
		
			return gradient;
		}

		public override void Recompute()
		{
			//nothing to compute
		}
	}
}