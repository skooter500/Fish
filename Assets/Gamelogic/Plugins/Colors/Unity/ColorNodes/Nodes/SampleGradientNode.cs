using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node whose input is a gradient, and whose output is colors selected from that gradient.
	/// </summary>
	[Version(1, 1)]
	public class SampleGradientNode : Node<Gradient, Color>
	{
		/// <summary>
		/// How many colors to  sample from the gradient.
		/// </summary>
		public int colorCount;

		/// <summary>
		/// How to select colors from the gradient.
		/// </summary>
		public GradientSelectionMode selectionMode;

		public override List<Color> Execute(IEnumerable<Gradient> input)
		{
			var result = new List<Color>();
		
			foreach (var gradient in input)
			{
				List<Color> colors;

				switch (selectionMode)
				{
					case GradientSelectionMode.Even:
						colors = ProceduralPalette.GenerateColorsEvenGradient(colorCount, gradient, 0);
						break;
					case GradientSelectionMode.Random:
						colors = ProceduralPalette.GenerateColorsRandomGradient(colorCount, gradient);
						break;
					case GradientSelectionMode.GoldenRatio:
						colors = ProceduralPalette.GenerateColorsGoldenRatioGradient(colorCount, gradient);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				result.AddRange(colors);
			}

			return result;
		}

		public override void Recompute()
		{
			//nothing to compute
		}
	}
}