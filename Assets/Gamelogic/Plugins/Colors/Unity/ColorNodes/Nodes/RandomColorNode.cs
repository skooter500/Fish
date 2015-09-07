using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node with no inputs that outputs a random color.
	/// </summary>
	[Version(1, 1)]
	public class RandomColorNode : Node<None, Color>
	{
		/// <summary>
		/// The color space in which the random selection is done. Each channel value
		/// of the selected space is selected uniformely. 
		/// </summary>
		public ColorSpace colorSpace;

		/// <summary>
		/// Hoe many colors to generate.
		/// </summary>
		public int colorCount;
	
		private List<Color> colors;
		
		public override List<Color> Execute(IEnumerable<None> input)
		{
			return colors;
		}

		public override void Recompute()
		{
			switch (colorSpace)
			{
				case ColorSpace.RGB:
					colors = ProceduralPalette.GenerateUniform(colorCount);
					break;
				case ColorSpace.HSL:
					colors = ProceduralPalette.GenerateUniformHSL(colorCount);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}