using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// A node that randomly mixes the first three colors of its inputs.
	/// </summary>
	[Version(1, 1)]
	public class RandomMix3Node : Node<Color, Color>
	{
		/// <summary>
		/// The number of colors to generate
		/// </summary>
		public int colorCount;

		/// <summary>
		/// The type of mixing.
		/// </summary>
		public ColorSpace mixMode;

		/// <summary>
		/// Determine the influence of one of the three nodes (randomly chosen). Lower values will result
		/// in less grey values being created if the three inputs are primaries.
		/// </summary>
		public float greyControl;

		private List<float> redMixFactors;
		private List<float> greenMixFactors;
		private List<float> blueMixFactors;
		private List<int> selectorIndices; 

		public override List<Color> Execute(IEnumerable<Color> input)
		{
			var colors = input.ToArray();
			var outputColors = new List<Color>();

			if (colors.Length >= 3)
			{
				for (int i = 0; i < colorCount; i++)
				{
					outputColors.Add(
						ProceduralPalette.Mix(
							colors[0],
							colors[1],
							colors[2],
							greyControl,
							selectorIndices[i],
							redMixFactors[i],
							greenMixFactors[i],
							blueMixFactors[i],
							mixMode));
				}
			}

			return outputColors;
		}

		public override void Recompute()
		{
			redMixFactors = new List<float>();
			greenMixFactors = new List<float>();
			blueMixFactors = new List<float>();
			selectorIndices = new List<int>();

			for (int i = 0; i < colorCount; i++)
			{
				redMixFactors.Add(Random.value);
				greenMixFactors.Add(Random.value);
				blueMixFactors.Add(Random.value);
				selectorIndices.Add(Random.Range(0, 3));
			}
		}
	}
}