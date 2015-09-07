using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node the linearly interpolates between the first two colors of it's inputs.
	/// </summary>
	[Version(1, 1)]
	public class MixNode2 : SimpleNode<Color, Color>
	{
		public ColorSpace mixMode;
		public float t;
		public override Color ExecuteSingle(IEnumerable<Color> input)
		{
			var colors = input.ToArray();

			if (colors.Length >= 2)
			{
				switch (mixMode)
				{
					case ColorSpace.RGB:
						return Color.Lerp(colors[0], colors[1], t);
					case ColorSpace.HSL:
						return ColorHSL.Lerp(colors[0], colors[1], t);
					default:
						throw new ArgumentOutOfRangeException();
				}
		
			}

			return Color.black;
		}

		public override void Recompute()
		{
			//nothing to compute
		}
	}
}